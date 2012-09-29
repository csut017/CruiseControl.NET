namespace CruiseControlNet.SelfHost.Controllers
{
    using CruiseControlNet.Common;
    using CruiseControlNet.SelfHost.Helpers;
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Exposes project information.
    /// </summary>
    public class ProjectController
        : ApiControllerBase
    {
        #region Private fields
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectController" /> class.
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        public ProjectController(ICruiseServer cruiseServer)
            : base(cruiseServer)
        {
        }
        #endregion

        #region Public methods
        #region Index()
        /// <summary>
        /// Gets a project by its identifier (name).
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// The project details.
        /// </returns>
        /// <exception cref="HttpResponseException"></exception>
        [HttpGet]
        public ProjectSummary Index(string project)
        {
            this.ValidateServer();

            // Try to find the project
            var projects = this.CruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var entity = projects.Snapshot
                .ProjectStatuses
                .FirstOrDefault(p => string.Equals(p.Name, project, StringComparison.InvariantCultureIgnoreCase));
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = entity.ToModel();
            var status = this.CruiseServer.TakeStatusSnapshot(new ProjectRequest(null, entity.Name));
            model.Tasks = status.Snapshot.ToModel();
            return model;
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts a project.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// A NoContent response.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage Start(string project)
        {
            this.ValidateServer();
            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.CruiseServer.Start(new ProjectRequest(null, entity.Name));
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops a project.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// A NoContent response.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage Stop(string project)
        {
            this.ValidateServer();
            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.CruiseServer.Stop(new ProjectRequest(null, entity.Name));
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion

        #region Build()
        /// <summary>
        /// Queues a build for a project.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// A NoContent response.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage Build(string project)
        {
            this.ValidateServer();
            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.CruiseServer.ForceBuild(new ProjectRequest(null, entity.Name));
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion

        #region Abort()
        /// <summary>
        /// Aborts a project build.
        /// </summary>
        /// <param name="project">The project name.</param>
        /// <returns>
        /// A NoContent response.
        /// </returns>
        [HttpPost]
        public HttpResponseMessage Abort(string project)
        {
            this.ValidateServer();
            var entity = this.RetrieveProject(project);
            if (entity == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.CruiseServer.AbortBuild(new ProjectRequest(null, entity.Name));
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }
        #endregion
        #endregion
    }
}