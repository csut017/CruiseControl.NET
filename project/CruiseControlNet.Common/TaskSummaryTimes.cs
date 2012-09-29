namespace CruiseControlNet.Common
{
    using System;

    /// <summary>
    /// Times for a task summary.
    /// </summary>
    public class TaskSummaryTimes
    {
        #region Public properties
        #region TimeStarted
        /// <summary>
        /// Gets or sets the time started.
        /// </summary>
        public DateTime? Started { get; set; }
        #endregion

        #region TimeCompleted
        /// <summary>
        /// Gets or sets the time completed.
        /// </summary>
        public DateTime? Completed { get; set; }
        #endregion
        #endregion
    }
}