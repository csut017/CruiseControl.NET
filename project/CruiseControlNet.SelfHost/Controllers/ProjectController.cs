namespace CruiseControlNet.SelfHost.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using CruiseControlNet.SelfHost.Helpers;
    using CruiseControlNet.SelfHost.Models;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Exposes project information.
    /// </summary>
    public class ProjectController
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
        public ProjectController(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Public methods
        #region GetAll()
        /// <summary>
        /// Gets all the projects.
        /// </summary>
        /// <returns>
        /// The project details.
        /// </returns>
        [Queryable]
        public IQueryable<ProjectSummary> GetAll()
        {
            if (this.cruiseServer == null)
            {
                return null;
            }

            var projects = this.cruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var model = projects.Snapshot.ProjectStatuses.Select(p => p.ToModel()).AsQueryable();
            return model;
        }
        #endregion

        #region GetByCategory()
        /// <summary>
        /// Gets all the projects in a category.
        /// </summary>
        /// <param name="category">The category to get the projects for.</param>
        /// <returns>
        /// The project details.
        /// </returns>
        [Queryable]
        public IQueryable<ProjectSummary> GetByCategory(string category)
        {
            if (this.cruiseServer == null)
            {
                return null;
            }

            var projects = this.cruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var model = projects.Snapshot
                .ProjectStatuses
                .Where(p => string.Equals(p.Category, category, StringComparison.InvariantCultureIgnoreCase))
                .Select(p => p.ToModel()).AsQueryable();
            return model;
        }
        #endregion

        #region GetById()
        /// <summary>
        /// Gets a project by its identifier (name).
        /// </summary>
        /// <param name="id">The identifier (name) of the project.</param>
        /// <returns>
        /// The project details.
        /// </returns>
        public ProjectSummary GetById(string id)
        {
            if (this.cruiseServer == null)
            {
                return null;
            }

            // Try to find the project
            var projects = this.cruiseServer.GetCruiseServerSnapshot(new ServerRequest());
            var project = projects.Snapshot
                .ProjectStatuses
                .FirstOrDefault(p => string.Equals(p.Name, id, StringComparison.InvariantCultureIgnoreCase));
            if (project == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            var model = project.ToModel();
            var status = this.cruiseServer.TakeStatusSnapshot(new ProjectRequest(null, project.Name));
            model.Tasks = status.Snapshot.ToModel();
            return model;
        }
        #endregion

        #region PostCommand()
        public CommandStatus Force(string id, Command command)
        {
            return new CommandStatus { Success = true, Message = "Build has been queued" };
        }
        #endregion
        #endregion
    }
}