namespace CruiseControlNet.SelfHost.Console
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http.Dependencies;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A scope container for connecting the server with the API controllers.
    /// </summary>
    public class ScopeContainer
        : IDependencyScope, IDependencyResolver
    {
        #region Private fields
        /// <summary>
        /// The <see cref="WebApiServerFactory"/> instance.
        /// </summary>
        private readonly WebApiServerFactory factory;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new instance of the <see cref="ScopeContainer"/> class.
        /// </summary>
        /// <param name="factory">The factory with the server scope.</param>
        public ScopeContainer(WebApiServerFactory factory)
        {
            this.factory = factory;
        }
        #endregion

        #region Public methods
        #region BeginScope()
        /// <summary>
        /// Begins a new dependency scope.
        /// </summary>
        /// <returns>
        /// The scope to use.
        /// </returns>
        public IDependencyScope BeginScope()
        {
            return this;
        }
        #endregion

        #region GetService()
        /// <summary>
        /// Gets the implementation of a service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>
        /// The implementation of <see cref="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return serviceType == typeof(ICruiseServer)
                ? this.factory.ServerInstance
                : null;
        }
        #endregion

        #region GetServices()
        /// <summary>
        /// Gets the implementations of a service.
        /// </summary>
        /// <param name="serviceType">The service type.</param>
        /// <returns>
        /// The implementations of <see cref="serviceType"/>.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceType == typeof(ICruiseServer)
                ? new[] { this.factory.ServerInstance }
                : new object[0];
        }
        #endregion

        #region Dispose()
        /// <summary>
        /// Cleans up.
        /// </summary>
        public void Dispose()
        {
        }
        #endregion
        #endregion
    }
}