namespace CruiseControlNet.SelfHost.Console
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.State;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A <see cref="ICruiseServerFactory"/> that also exposes a Web API interface.
    /// </summary>
    public class WebApiServerFactory
        : ICruiseServerFactory
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
            this.ServerInstance =(remote)
                ? this.CreateRemote(configFile)
                : this.CreateLocal(configFile);
            return this.ServerInstance;
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
            IProjectStateManager stateManager = new XmlProjectStateManager();
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