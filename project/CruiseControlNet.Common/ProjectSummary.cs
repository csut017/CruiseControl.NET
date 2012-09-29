namespace CruiseControlNet.Common
{
    /// <summary>
    /// The model for a project summary.
    /// </summary>
    public class ProjectSummary
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Category
        /// <summary>
        /// Gets or sets the project category.
        /// </summary>
        public string Category { get; set; }
        #endregion

        #region Activity
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        public ProjectSummaryActivity Activity { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public ProjectSummaryStatus Status { get; set; }
        #endregion

        #region BuildStatus
        /// <summary>
        /// Gets or sets the build status.
        /// </summary>
        public ProjectSummaryBuildStatus BuildStatus { get; set; }
        #endregion

        #region BuildStage
        /// <summary>
        /// Gets or sets the build stage.
        /// </summary>
        public string BuildStage { get; set; }
        #endregion

        #region Times
        /// <summary>
        /// Gets or sets the times.
        /// </summary>
        public ProjectSummaryTimes Times { get; set; }
        #endregion

        #region Labels
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        public ProjectSummaryLabels Labels { get; set; }
        #endregion

        #region Messages
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public BuildMessage[] Messages { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the child tasks.
        /// </summary>
        public TaskSummary[] Tasks { get; set; }
        #endregion
        #endregion
    }
}