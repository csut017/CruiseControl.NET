namespace CruiseControlNet.SelfHost.Console.Models
{
    /// <summary>
    /// The status of an action.
    /// </summary>
    public class CommandStatus
    {
        #region Public properties
        #region Success
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CommandStatus" /> is success.
        /// </summary>
        public bool Success { get; set; }
        #endregion

        #region Message
        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }
        #endregion
        #endregion
    }
}