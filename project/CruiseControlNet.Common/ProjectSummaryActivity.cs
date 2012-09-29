namespace CruiseControlNet.Common
{
    /// <summary>
    /// The current activity of a project.
    /// </summary>
    public enum ProjectSummaryActivity
    {
        /// <summary>
        /// The activity is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The project is building.
        /// </summary>
        Building,

        /// <summary>
        /// The project is checking for modifications.
        /// </summary>
        CheckingModifications,

        /// <summary>
        /// The project is pending a build.
        /// </summary>
        Pending,

        /// <summary>
        /// The project is sleeping.
        /// </summary>
        Sleeping,

        /// <summary>
        /// The project is not running.
        /// </summary>
        NotRunning
    }
}