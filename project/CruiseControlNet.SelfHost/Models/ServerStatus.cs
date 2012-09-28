namespace CruiseControlNet.SelfHost.Models
{
    /// <summary>
    /// The possible server statuses.
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// The status is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The server is not current running.
        /// </summary>
        NotRunning,

        /// <summary>
        /// The server is running.
        /// </summary>
        Running
    }
}