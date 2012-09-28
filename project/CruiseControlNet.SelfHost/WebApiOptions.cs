namespace CruiseControlNet.SelfHost
{
    /// <summary>
    /// The options for running the web API.
    /// </summary>
    public class WebApiOptions
    {
        #region Public properties
        #region IsConfigured
        /// <summary>
        /// Gets a flag indicating whether the Web API has been configured.
        /// </summary>
        public bool IsConfigured
        {
            get { return !string.IsNullOrEmpty(this.BaseEndpoint); }
        }
        #endregion

        #region BaseEndpoint
        /// <summary>
        /// Gets or sets the base endpoint.
        /// </summary>
        public string BaseEndpoint { get; set; }
        #endregion
        #endregion
    }
}