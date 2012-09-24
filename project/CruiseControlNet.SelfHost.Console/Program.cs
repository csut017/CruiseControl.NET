namespace CruiseControlNet.SelfHost.Console
{
    using System;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting;
    using System.Threading;

    /// <summary>
    /// Main application module.
    /// </summary>
    public class Program
        : IDisposable
    {
        #region Private fields
        /// <summary>
        /// Main lock object.
        /// </summary>
        private static readonly object LockObject = new object();

        /// <summary>
        /// The watcher for detecting any file system changes.
        /// </summary>
        private readonly FileSystemWatcher watcher;

        /// <summary>
        /// A flag indicating whether this program has been disposed yet.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// A flag indicating whether the program is restarting.
        /// </summary>
        private bool isRestarting = true;

        /// <summary>
        /// A flag indicating whether shaow copying is turned on.
        /// </summary>
        private bool useShadowCopying;

        /// <summary>
        /// When the restart should start.
        /// </summary>
        private DateTime restartTime;

        /// <summary>
        /// The actual application runner.
        /// </summary>
        private AppRunner appRunner;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="Program"/> class.
        /// </summary>
        private Program()
        {
            // Initialise the file system watcher
            this.watcher = new FileSystemWatcher(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
                {
                    NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size
                };
            this.watcher.Changed += OnFileChanged;

            // The default shadow copy setting - this may be changed if shadow copying is not possible
            var setting = ConfigurationManager.AppSettings["ShadowCopy"] ?? string.Empty;
            this.useShadowCopying = !(string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(setting, "false", StringComparison.OrdinalIgnoreCase));
        }
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Runs the program.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>
        /// Th exit code to return.
        /// </returns>
        public int Run(string[] args)
        {
            if (this.isDisposed)
            {
                throw new StartUpException("Program has already been disposed");
            }

            // Initialise all variables
            var location = Assembly.GetExecutingAssembly().Location;
            var fullName = typeof(AppRunner).FullName;
            if (fullName == null)
            {
                throw new StartUpException("Unable to retrieve full name of application runner");
            }

            // Start monitoring any file changes
            watcher.EnableRaisingEvents = true;

            // Begin the main application loop
            var result = 0;
            while (this.isRestarting)
            {
                this.isRestarting = false;

                // Load the domain and start the runner
                AppDomain runnerDomain;
                try
                {
                    runnerDomain = CreateNewDomain(useShadowCopying);
                }
                catch (FileLoadException)
                {
                    // Unable to use shadow-copying (no user profile?), therefore turn off shadow-copying
                    useShadowCopying = false;
                    runnerDomain = CreateNewDomain(false);
                }

                // Generate the new application runner
                this.appRunner = runnerDomain.CreateInstanceFromAndUnwrap(location, fullName) as AppRunner;
                if (appRunner == null)
                {
                    throw new StartUpException("Unable to initialise application runner");
                }

                // Start the runner and wait
                result = this.appRunner.Run(args, useShadowCopying);
                AppDomain.Unload(runnerDomain);

                // Allow any change events to finish (i.e. if multiple files are being copied)
                while (DateTime.Now < restartTime)
                {
                    Thread.Sleep(500);
                }
            }

            // Clean and return the result
            watcher.EnableRaisingEvents = false;
            return result;
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Cleans up any resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            // Clean up
            this.isDisposed = true;
            this.watcher.Dispose();
        }
        #endregion
        #endregion

        #region Internal methods
        #region Main()
        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Any arguments from the command line.</param>
        /// <returns>
        /// The exist status.
        /// </returns>
        [STAThread]
        internal static int Main(string[] args)
        {
            var program = new Program();
            return program.Run(args);
        }
        #endregion
        #endregion

        #region Private methods
        #region CreateNewDomain()
        /// <summary>
        /// Creates the new runner domain.
        /// </summary>
        /// <param name="useShadowCopying">If set to <c>true</c> shadow copying will be used.</param>
        /// <returns>The new <see cref="AppDomain"/>.</returns>
        private static AppDomain CreateNewDomain(bool useShadowCopying)
        {
            return AppDomain.CreateDomain(
                "CruiseControl.Net",
                null,
                AppDomain.CurrentDomain.BaseDirectory,
                AppDomain.CurrentDomain.RelativeSearchPath,
                useShadowCopying);
        }
        #endregion

        #region OnFileChanged()
        /// <summary>
        /// Called when a file has changed.
        /// </summary>
        /// <param name="sender">The <see cref="FileSystemWatcher"/> that fired the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // Either we are already restarting or there is no application runner - therefore no point in continuing...
            if (this.isRestarting || (this.appRunner == null))
            {
                return;
            }

            // Make sure we only stop the application runner once!
            lock (LockObject)
            {
                this.isRestarting = true;
                this.restartTime = DateTime.Now.AddSeconds(10);
                try
                {
                    appRunner.Stop("One or more DLLs have changed");
                }
                catch (RemotingException)
                {
                    // Sometimes this exception occurs - the lock statement should catch it, but...
                }
            }
        }
        #endregion
        #endregion
    }
}