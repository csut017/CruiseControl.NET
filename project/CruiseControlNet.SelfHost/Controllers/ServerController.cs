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
        : ApiControllerBase
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerController" /> class.
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        public ServerController(ICruiseServer cruiseServer)
            : base(cruiseServer)
        {
        }
        #endregion

        #region Public methods
        #region Index()
        /// <summary>
        /// Gets the server details.
        /// </summary>
        /// <returns>
        /// The server details.
        /// </returns>
        [HttpGet]
        public ServerSummary Index()
        {
            if (this.CruiseServer == null)
            {
                return new ServerSummary
                    {
                        Status = ServerStatus.NotRunning
                    };
            }

            var projects = this.CruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var model = new ServerSummary
                {
                    Version = this.CruiseServer.GetType().Assembly.GetName().Version.ToString(),
                    Status = ServerStatus.Running,
                    Projects = projects.Snapshot.ProjectStatuses.Select(p => p.ToModel()).ToArray()
                };
            return model;
        }
        #endregion
        #endregion
    }
}