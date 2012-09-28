namespace CruiseControlNet.SelfHost.Models
{
    /// <summary>
    /// Defines the labels in a project summary.
    /// </summary>
    public class ProjectSummaryLabels
    {
        #region Public properties
        #region Last
        /// <summary>
        /// Gets or sets the last build label.
        /// </summary>
        public string Last { get; set; }
        #endregion

        #region LastSuccessful
        /// <summary>
        /// Gets or sets the last successful build label.
        /// </summary>
        public string LastSuccessful { get; set; }
        #endregion
        #endregion
    }
}