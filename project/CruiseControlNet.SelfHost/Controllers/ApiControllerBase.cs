namespace CruiseControlNet.SelfHost.Controllers
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Web.Http;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    /// <summary>
    /// Common base functionality.
    /// </summary>
    public abstract class ApiControllerBase
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
        protected ApiControllerBase(ICruiseServer cruiseServer)
        {
            this.cruiseServer = cruiseServer;
        }
        #endregion

        #region Protected properties
        #region CruiseServer
        /// <summary>
        /// The associated <see cref="ICruiseServer"/>.
        /// </summary>
        protected ICruiseServer CruiseServer
        {
            get { return cruiseServer; }
        }
        #endregion
        #endregion

        #region Protected methods
        #region ValidateServer()
        /// <summary>
        /// Validates the server.
        /// </summary>
        /// <exception cref="HttpResponseException">Thrown if the server has not been set.</exception>
        protected void ValidateServer()
        {
            if (this.cruiseServer == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadGateway);
            }
        }
        #endregion

        #region RetrieveProject()
        /// <summary>
        /// Retrieves a project.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>
        /// The project if found; <c>null</c> otherwise.
        /// </returns>
        protected ProjectStatus RetrieveProject(string name)
        {
            var projects = this.CruiseServer.GetProjectStatus(new ServerRequest());
            var project = projects.Projects
                .FirstOrDefault(p => string.Equals(name, p.Name, StringComparison.InvariantCultureIgnoreCase));
            return project;
        }
        #endregion
        #endregion
    }
}