namespace CruiseControlNet.Common
{
    /// <summary>
    /// The status of a <see cref="TaskSummary"/>.
    /// </summary>
    public enum TaskSummaryStatus
    {
        /// <summary>
        /// Status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Task is pending.
        /// </summary>
        Pending,

        /// <summary>
        /// Task is running.
        /// </summary>
        Running,

        /// <summary>
        /// Task has successfully completed.
        /// </summary>
        CompletedSuccess,

        /// <summary>
        /// Task has failed.
        /// </summary>
        CompletedFailed,

        /// <summary>
        /// Task was cancelled.
        /// </summary>
        Cancelled
    }
}