namespace CruiseControlNet.SelfHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Web.Http.Dependencies;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// A scope container for connecting the server with the API controllers.
    /// </summary>
    public class ScopeContainer
        : IDependencyResolver
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
            var canInitialise = serviceType
                .GetConstructors()
                .Any(CanBeInitialised);
            return canInitialise
                ? Activator.CreateInstance(serviceType, this.factory.ServerInstance)
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
            var canInitialise = serviceType
                .GetConstructors()
                .Any(CanBeInitialised);
            return canInitialise
                ? new[] { Activator.CreateInstance(serviceType, this.factory.ServerInstance) }
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

        #region Private methods
        #region CanBeInitialised()
        /// <summary>
        /// Checks if a constructor has the necessary parameters to be initialised.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns>
        /// <c>true</c> if the constructor can be initialised; <c>false</c> otherwise.
        /// </returns>
        private static bool CanBeInitialised(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Count() == 1)
            {
                return parameters[0].ParameterType == typeof(ICruiseServer);
            }

            return false;
        }
        #endregion
        #endregion
    }
}