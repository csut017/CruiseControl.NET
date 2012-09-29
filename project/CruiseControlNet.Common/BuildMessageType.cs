namespace CruiseControlNet.Common
{
    /// <summary>
    /// The type of a <see cref="BuildMessage"/>.
    /// </summary>
    public enum BuildMessageType
    {
        /// <summary>
        /// An unknown kind.
        /// </summary>
        /// <remarks></remarks>
        Unknown,

        /// <summary>
        /// The breakers of the build.
        /// </summary>
        Breakers = 1,

        /// <summary>
        /// The fixers of the build.
        /// </summary>
        Fixer = 2,

        /// <summary>
        /// The tasks that failed.
        /// </summary>
        FailingTasks = 3,

        /// <summary>
        /// A build status message.
        /// </summary>
        BuildStatus = 4,

        /// <summary>
        /// The person who aborted the build.
        /// </summary>
        BuildAbortedBy = 5
    }
}