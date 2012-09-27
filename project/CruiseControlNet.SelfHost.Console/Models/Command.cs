namespace CruiseControlNet.SelfHost.Console.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// A command to execute.
    /// </summary>
    public class Command
    {
        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the command name.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Arguments
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }
        #endregion
        #endregion
    }
}