namespace CruiseControlNet.SelfHost.Console.Models
{
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
        #endregion
    }
}