namespace CruiseControlNet.SelfHost
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Web.Http;
    using System.Web.Http.SelfHost;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.State;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A <see cref="ICruiseServerFactory"/> that also exposes a Web API interface.
    /// </summary>
    public class WebApiServerFactory
        : ICruiseServerFactory, IDisposable
    {
        #region Private fields
        /// <summary>
        /// The remoting confiruation file.
        /// </summary>
        private static readonly string RemotingConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;

        /// <summary>
        /// A flag indicating whether the config file should be watched.
        /// </summary>
        private readonly bool watchConfigFile;

        /// <summary>
        /// The Web API server.
        /// </summary>
        private HttpSelfHostServer apiServer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="WebApiServerFactory"/> class.
        /// </summary>
        public WebApiServerFactory()
        {
            var value = ConfigurationManager.AppSettings["WatchConfigFile"];
            watchConfigFile = (value == null) || Convert.ToBoolean(value);
        }
        #endregion

        #region Public properties
        #region ServerInstance
        /// <summary>
        /// Gets the current <see cref="ICruiseServer"/> instance.
        /// </summary>
        public ICruiseServer ServerInstance { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region Create()
        /// <summary>
        /// Generates a <see cref="ICruiseServer"/> instance.
        /// </summary>
        /// <param name="remote">A flag indicating whether it should be a local or remote instance.</param>
        /// <param name="configFile">The configuration file to</param>
        /// <returns>
        /// The new <see cref="ICruiseServer"/> instance.
        /// </returns>
        public ICruiseServer Create(bool remote, string configFile)
        {
            this.ServerInstance = (remote)
                ? this.CreateRemote(configFile)
                : this.CreateLocal(configFile);
            return this.ServerInstance;
        }
        #endregion

        #region StartWebApi()
        /// <summary>
        /// Starts the web API.
        /// </summary>
        /// <param name="factory">The factory to use.</param>
        /// <param name="options">The options to use.</param>
        public void StartWebApi(WebApiServerFactory factory, WebApiOptions options)
        {
            Log.Info("Starting web API server...");
            var config = new HttpSelfHostConfiguration(options.BaseEndpoint);

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            json.SerializerSettings.Converters.Add(new StringEnumConverter());

            config.Routes.MapHttpRoute("API Build", "api/{controller}/{project}/{id}", new { id = RouteParameter.Optional }, new { controller = "build" });
            config.Routes.MapHttpRoute("API Default", "api/{controller}/{id}", new { id = RouteParameter.Optional });
            config.DependencyResolver = new ScopeContainer(factory);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            this.apiServer = new HttpSelfHostServer(config);
            apiServer.OpenAsync().Wait();
            Log.Info("...web API server started ({0})", options.BaseEndpoint);
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.apiServer == null)
            {
                return;
            }

            Log.Info("Closing web API server...");
            this.apiServer.Dispose();
            this.apiServer = null;
            Log.Info("...web API server closed");
        }
        #endregion
        #endregion

        #region Private methods
        #region CreateLocal()
        /// <summary>
        /// Generates a local <see cref="ICruiseServer"/> instance.
        /// </summary>
        /// <param name="configFile">The configuration file to use.</param>
        /// <returns>
        /// The new <see cref="ICruiseServer"/> instance.
        /// </returns>
        private ICruiseServer CreateLocal(string configFile)
        {
            var stateManager = new XmlProjectStateManager();
            var configuration = ConfigurationManager.GetSection("cruiseServer") as ServerConfiguration;
            List<ExtensionConfiguration> extensionList = null;
            if (configuration != null)
            {
                extensionList = configuration.Extensions;
            }

            PathUtils.ConfigFileLocation = Path.IsPathRooted(configFile)
                                               ? configFile
                                               : Path.Combine(Environment.CurrentDirectory, configFile);
            var server = new CruiseServer(
                NewConfigurationService(configFile),
                new ProjectIntegratorListFactory(),
                new NetReflectorProjectSerializer(),
                stateManager,
                new SystemIoFileSystem(),
                new ExecutionEnvironment(),
                extensionList);
            server.InitialiseServices();
            this.ServerInstance = server;
            return server;
        }
        #endregion

        #region NewConfigurationService()
        /// <summary>
        /// Generates the configuration service.
        /// </summary>
        /// <param name="configFile">The configuration file.</param>
        /// <returns>
        /// The new <see cref="IConfigurationService"/>.
        /// </returns>
        private IConfigurationService NewConfigurationService(string configFile)
        {
            IConfigurationService service = new FileConfigurationService(
                new DefaultConfigurationFileLoader(),
                new DefaultConfigurationFileSaver(new NetReflectorProjectSerializer()),
                new FileInfo(configFile));
            if (this.watchConfigFile)
            {
                service = new FileWatcherConfigurationService(service, new FileChangedWatcher(configFile));
            }

            return new CachingConfigurationService(service);
        }
        #endregion

        #region CreateRemote()
        /// <summary>
        /// Generates a remote connection to a <see cref="ICruiseServer"/> instance.
        /// </summary>
        /// <param name="configFile">The configuration file to use.</param>
        /// <returns>
        /// The new <see cref="ICruiseServer"/> instance.
        /// </returns>
        private ICruiseServer CreateRemote(string configFile)
        {
            return new RemoteCruiseServer(
                CreateLocal(configFile),
                RemotingConfigurationFile);
        }
        #endregion
        #endregion
    }
}