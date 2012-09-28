namespace CruiseControlNet.SelfHost.Models
{
    using System;
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

        #region TimeStarted
        /// <summary>
        /// Gets or sets the time started.
        /// </summary>
        public DateTime? TimeStarted { get; set; }
        #endregion

        #region TimeCompleted
        /// <summary>
        /// Gets or sets the time completed.
        /// </summary>
        public DateTime? TimeCompleted { get; set; }
        #endregion
        #endregion
    }
}