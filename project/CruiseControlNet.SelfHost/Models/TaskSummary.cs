namespace CruiseControlNet.SelfHost.Models
{
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// The summary of a task.
    /// </summary>
    public class TaskSummary
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Description
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the child tasks.
        /// </summary>
        public TaskSummary[] Tasks { get; set; }
        #endregion

        #region Error
        public string Error { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public ItemBuildStatus Status { get; set; }
        #endregion

        #region Times
        /// <summary>
        /// Gets or sets the times.
        /// </summary>
        public TaskSummaryTimes Times { get; set; }
        #endregion
        #endregion
    }
}