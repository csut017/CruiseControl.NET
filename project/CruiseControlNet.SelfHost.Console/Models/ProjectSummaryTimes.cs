namespace CruiseControlNet.SelfHost.Console.Models
{
    using System;

    /// <summary>
    /// Defines the times in a project summary.
    /// </summary>
    public class ProjectSummaryTimes
    {
        #region Public properties
        #region LastRun
        /// <summary>
        /// Gets or sets the time of the last run.
        /// </summary>
        public DateTime? LastRun { get; set; }
        #endregion

        #region NextRun
        /// <summary>
        /// Gets or sets the time for the next run.
        /// </summary>
        public DateTime? NextRun { get; set; }
        #endregion
        #endregion
    }
}