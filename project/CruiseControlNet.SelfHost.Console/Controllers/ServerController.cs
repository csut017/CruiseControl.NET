namespace CruiseControlNet.SelfHost.Console.Controllers
{
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Exposes server information.
    /// </summary>
    public class ServerController
        : ApiController
    {
        #region Private fields
        /// <summary>
        /// The associated <see cref="ICruiseServer"/>.
        /// </summary>
        private readonly ICruiseServer cruiseServer;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="ServerController"/> class.
        /// </summary>
        /// <param name="cruiseServer">The <see cref="ICruiseServer"/> to use.</param>
        public ServerController(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Public methods
        #region Get()
        /// <summary>
        /// Gets the server detials.
        /// </summary>
        /// <returns>
        /// The server details.
        /// </returns>
        public Models.Server Get()
        {
            if (this.cruiseServer == null)
            {
                return new Models.Server
                    {
                        Status = Models.ServerStatus.NotRunning
                    };
            }

            var model = new Models.Server
                {
                    Version = this.cruiseServer.GetType().Assembly.GetName().Version.ToString()
                };
            return model;
        }
        #endregion
        #endregion
    }
}