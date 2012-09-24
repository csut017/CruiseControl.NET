namespace CruiseControlNet.SelfHost.Console.Models
{
    /// <summary>
    /// The model for the server summary.
    /// </summary>
    public class ServerSummary
    {
        #region Public properties
        #region Version
        /// <summary>
        /// Gets or sets the server version.
        /// </summary>
        public string Version { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the server status.
        /// </summary>
        public ServerStatus Status { get; set; }
        #endregion

        #region Projects
        /// <summary>
        /// Gets or sets the project summaries.
        /// </summary>
        public ProjectSummary[] Projects { get; set; }
        #endregion
        #endregion
    }
}