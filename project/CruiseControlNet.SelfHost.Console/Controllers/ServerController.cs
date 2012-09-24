namespace CruiseControlNet.SelfHost.Console.Controllers
{
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
        public Models.ServerSummary Get()
        {
            if (this.cruiseServer == null)
            {
                return new Models.ServerSummary
                    {
                        Status = Models.ServerStatus.NotRunning
                    };
            }

            var projects = this.cruiseServer.GetProjectStatus(new ServerRequest());
            var model = new Models.ServerSummary
                {
                    Version = this.cruiseServer.GetType().Assembly.GetName().Version.ToString(),
                    Status = Models.ServerStatus.Running,
                    Projects = projects.Projects.Select(p => new Models.ProjectSummary
                        {
                            Name = p.Name
                        }).ToArray()
                };
            return model;
        }
        #endregion
        #endregion
    }
}