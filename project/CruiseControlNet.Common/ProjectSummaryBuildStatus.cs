namespace CruiseControlNet.Common
{
    /// <summary>
    /// The build status for a <see cref="ProjectSummary"/>.
    /// </summary>
    public enum ProjectSummaryBuildStatus
    {
        /// <summary>
        /// Build status is unknown
        /// </summary>
        Unknown,

        /// <summary>
        /// Last build was successful.
        /// </summary>
        Success,

        /// <summary>
        /// Last build failed.
        /// </summary>
        Failure,

        /// <summary>
        /// An error occurred during the last build.
        /// </summary>
        Exception,

        /// <summary>
        /// Last build was cancelled.
        /// </summary>
        Cancelled
    }
}