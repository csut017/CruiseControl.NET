namespace CruiseControlNet.SelfHost.Console
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote.Mono;

    /// <summary>
    /// Main application runner.
    /// </summary>
    /// <remarks>
    /// This allows the application to be run in a seperate application domain - allowing for hot-swapping.
    /// </remarks>
    public class AppRunner
        : MarshalByRefObject
    {
        #region Private fields
        /// <summary>
        /// The console runner exposed by Core.
        /// </summary>
        private ConsoleRunner runner;

        /// <summary>
        /// A flag indicating whether we are stopping.
        /// </summary>
        private bool isStopping;

        /// <summary>
        /// The runner lock object.
        /// </summary>
        private readonly object lockObject = new object();
        #endregion

        #region Public methods
        #region Start()
        /// <summary>
        /// Starts the actual application.
        /// </summary>
        /// <param name="args">The arguments for the application.</param>
        /// <param name="usesShadowCopying">A flag indicating whether shadow copying should be used.</param>
        /// <returns>
        /// The return code for the application.
        /// </returns>
        public int Run(string[] args, bool usesShadowCopying)
        {
            // Parse the command line arguments
            var webOptions = new WebApiOptions();
            var consoleArgs = new ConsoleRunnerArguments();
            var opts = new OptionSet();
            opts.Add("h|?|help", "display this help screen", v => consoleArgs.ShowHelp = v != null)
                .Add("c|config=", "the configuration file to use (defaults to ccnet.conf)", v => consoleArgs.ConfigFile = v)
                .Add("r|remoting=", "turn remoting on/off (defaults to on)", v => consoleArgs.UseRemoting = v == "on")
                .Add("p|project=", "the project to integrate (???)", v => consoleArgs.Project = v)
                .Add("v|validate", "validate the configuration file and exit", v => consoleArgs.ValidateConfigOnly = v != null)
                .Add("l|logging=", "turn logging on/off (defaults to on)", v => consoleArgs.Logging = v == "on")
                .Add("sc|shadowCopy=", "turn shadow copying on/off (defaults to on)", v => usesShadowCopying = v == "on")
                .Add("e|errorpause=", "turn pause on error on/off (defaults to on)", v => consoleArgs.PauseOnError = v == "on")
                .Add("we|webEndPoint=", "the base endpoint for the web API (default none)", v => webOptions.BaseEndpoint = v);
            try
            {
                opts.Parse(args);
            }
            catch (OptionException e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return 1;
            }

            // Display the help
            if (consoleArgs.ShowHelp)
            {
                DisplayHelp(opts);
                return 0;
            }

            ICruiseServerFactory factory = null;
            try
            {
                // Start the actual console runner
                if (webOptions.IsConfigured)
                {
                    var apiFactory = new WebApiServerFactory();
                    apiFactory.StartWebApi(apiFactory, webOptions);
                    factory = apiFactory;
                }
                else
                {
                    factory = new CruiseServerFactory();
                }

                runner = new ConsoleRunner(consoleArgs, factory);
                if (!usesShadowCopying)
                {
                    Log.Warning("Shadow-copying has been turned off - hot-swapping will not work!");
                }

                runner.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                if (consoleArgs.PauseOnError)
                {
                    Console.WriteLine("An unexpected error has caused the console to crash");
                    Console.ReadKey();
                }
                return 2;
            }
            finally
            {
                // Clean up 
                runner = null;
                var disposable = factory as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        #endregion

        #region InitializeLifetimeService()
        /// <summary>
        /// Initialise the lifetime service.
        /// </summary>
        /// <returns>
        /// <c>null</c> as we don't want any instances to expire.
        /// </returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stop the application.
        /// </summary>
        /// <param name="reason">The reason for stopping.</param>
        public void Stop(string reason)
        {
            // Since there may be a race condition around stopping the runner, check if it should be stopped
            // within a lock statement
            var stopRunner = false;
            lock (lockObject)
            {
                if (!isStopping)
                {
                    stopRunner = true;
                    isStopping = true;
                }
            }

            if (!stopRunner)
            {
                return;
            }

            // Perform the actual stop
            Log.Info("Stopping console: " + reason);
            try
            {
                runner.Stop();
            }
            catch (RemotingException)
            {
                // Sometimes this exception gets thrown and not sure why. 
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region DisplayHelp()
        /// <summary>
        /// Displays the help.
        /// </summary>
        /// <param name="opts">The command line options.</param>
        private static void DisplayHelp(OptionSet opts)
        {
            var resourceName = (typeof(AppRunner).FullName ?? "AppRunner").Replace("AppRunner", "Help.txt");
            var thisApp = Assembly.GetExecutingAssembly();
            using (var helpStream = thisApp.GetManifestResourceStream(resourceName))
            {
                if (helpStream == null)
                {
                    Console.WriteLine("Unable to retrieve help contents");
                }
                else
                {
                    using (var reader = new StreamReader(helpStream))
                    {
                        var data = reader.ReadToEnd();
                        Console.Write(data);
                    }
                }
            }

            opts.WriteOptionDescriptions(Console.Out);
        }
        #endregion
        #endregion
    }
}