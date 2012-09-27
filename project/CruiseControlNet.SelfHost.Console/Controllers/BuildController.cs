namespace CruiseControlNet.SelfHost.Console.Controllers
{
    using CruiseControlNet.SelfHost.Console.Helpers;
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Exposes build information.
    /// </summary>
    public class BuildController
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
        /// Initializes a new instance of the <see cref="ProjectController" /> class.
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        public BuildController(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Public methods
        #region GetAll()
        /// <summary>
        /// Gets the builds for a project.
        /// </summary>
        /// <param name="project">The project identifier (name).</param>
        /// <returns>
        /// The build details.
        /// </returns>
        [Queryable]
        public IQueryable<Models.BuildSummary> Index(string project)
        {
            if (this.cruiseServer == null)
            {
                return null;
            }

            var properName = this.RetrieveProperProjectName(project);
            if (properName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var builds = this.cruiseServer.GetBuildNames(new ProjectRequest(null, properName));
            return builds.Data
                .Select(b => new LogFile(b))
                .Select(b => b.ToModel()).AsQueryable();
        }
        #endregion

        #region GetById()
        /// <summary>
        /// Gets the details on a build.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="id">The id.</param>
        /// <returns>
        /// The build details.
        /// </returns>
        public Models.BuildSummary Details(string project, string id)
        {
            if (this.cruiseServer == null)
            {
                return null;
            }

            var properName = this.RetrieveProperProjectName(project);
            if (properName == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = new LogFile(id).ToModel();
            var status = this.cruiseServer.GetFinalBuildStatus(new BuildRequest(null, properName) { BuildName = id });
            model.Tasks = status.Snapshot.ToModel();
            return model;
        }
        #endregion
        #endregion

        #region Private methods
        #region RetrieveProperProjectName()
        /// <summary>
        /// Retrieves the proper (case-sensitive) name for a project.
        /// </summary>
        /// <param name="name">The name to match.</param>
        /// <returns>
        /// The matched name.
        /// </returns>
        private string RetrieveProperProjectName(string name)
        {
            var projects = this.cruiseServer.GetProjectStatus(new ServerRequest());
            var project = projects.Projects
                .FirstOrDefault(p => string.Equals(name, p.Name, StringComparison.InvariantCultureIgnoreCase));
            return project == null ? null : project.Name;
        }
        #endregion
        #endregion
    }
}