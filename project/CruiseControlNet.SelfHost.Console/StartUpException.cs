namespace CruiseControlNet.SelfHost.Console
{
    using System;

    /// <summary>
    /// An exception that has occurred during start-up.
    /// </summary>
    public class StartUpException
        : ApplicationException
    {
        #region Constructors
        /// <summary>
        /// Starts a new instance of the <see cref="StartUpException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public StartUpException(string message)
            : base(message)
        {
        }
        #endregion
    }
}