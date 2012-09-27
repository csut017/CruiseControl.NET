namespace CruiseControlNet.SelfHost.Console.Helpers
{
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Extensions to <see cref="Message"/>.
    /// </summary>
    public static class BuildMessageExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts a <see cref="Message"/> to a <see cref="Models.BuildMessage"/>
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <returns>
        /// The converted <see cref="Models.BuildMessage"/>.
        /// </returns>
        public static Models.BuildMessage ToModel(this Message message)
        {
            return new Models.BuildMessage
                {
                    Text = message.Text,
                    Type = message.Kind
                };
        }
        #endregion
        #endregion
    }
}