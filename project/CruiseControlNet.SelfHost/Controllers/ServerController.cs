namespace CruiseControlNet.SelfHost.Controllers
{
    using CruiseControlNet.SelfHost.Helpers;
    using CruiseControlNet.SelfHost.Models;
    using System.Linq;
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

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
        /// Initializes a new instance of the <see cref="ServerController" /> class.
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        public ServerController(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Public methods
        #region Get()
        /// <summary>
        /// Gets the server details.
        /// </summary>
        /// <returns>
        /// The server details.
        /// </returns>
        public ServerSummary Get()
        {
            if (this.cruiseServer == null)
            {
                return new ServerSummary
                    {
                        Status = ServerStatus.NotRunning
                    };
            }

            var projects = this.cruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var model = new ServerSummary
                {
                    Version = this.cruiseServer.GetType().Assembly.GetName().Version.ToString(),
                    Status = ServerStatus.Running,
                    Projects = projects.Snapshot.ProjectStatuses.Select(p => p.ToModel()).ToArray()
                };
            return model;
        }
        #endregion
        #endregion
    }
}