namespace CruiseControlNet.Common
{
    /// <summary>
    /// The status of a <see cref="ProjectSummary" />.
    /// </summary>
    public enum ProjectSummaryStatus
    {
        /// <summary>
        /// Status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Project is running.
        /// </summary>
        Running,

        /// <summary>
        /// Project is stopping.
        /// </summary>
        Stopping,

        /// <summary>
        /// Project is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// Project is starting.
        /// </summary>
        Starting
    }
}