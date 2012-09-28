namespace CruiseControlNet.SelfHost.Models
{
    using System;

    /// <summary>
    /// The model for a build summary.
    /// </summary>
    public class BuildSummary
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Date
        /// <summary>
        /// Gets or sets the build date/time.
        /// </summary>
        public DateTime Date { get; set; }
        #endregion

        #region Success
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="BuildSummary" /> is for a successful build.
        /// </summary>
        public bool Success { get; set; }
        #endregion

        #region Label
        /// <summary>
        /// Gets or sets the label.
        /// </summary>
        public string Label { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the child tasks.
        /// </summary>
        public TaskSummary[] Tasks { get; set; }
        #endregion

        #region Log
        /// <summary>
        /// Gets or sets the log.
        /// </summary>
        public string Log { get; set; }
        #endregion
        #endregion
    }
}