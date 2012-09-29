namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.Common;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Extensions to <see cref="Message"/>.
    /// </summary>
    public static class BuildMessageExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts a <see cref="Message"/> to a <see cref="BuildMessage"/>
        /// </summary>
        /// <param name="message">The message to convert.</param>
        /// <returns>
        /// The converted <see cref="BuildMessage"/>.
        /// </returns>
        public static BuildMessage ToModel(this Message message)
        {
            return new BuildMessage
                {
                    Text = message.Text,
                    Type = Convert(message.Kind)
                };
        }
        #endregion
        #endregion

        #region Private methods
        #region Convert()
        /// <summary>
        /// Conerts a <see cref="Message.MessageKind"/> to a <see cref="BuildMessageType"/>.
        /// </summary>
        /// <param name="kind">The <see cref="Message.MessageKind"/>.</param>
        /// <returns>
        /// The <see cref="BuildMessageType"/>.
        /// </returns>
        private static BuildMessageType Convert(Message.MessageKind kind)
        {
            switch (kind)
            {
                case Message.MessageKind.Breakers:
                    return BuildMessageType.Breakers;

                case Message.MessageKind.BuildAbortedBy:
                    return BuildMessageType.BuildAbortedBy;

                case Message.MessageKind.BuildStatus:
                    return BuildMessageType.BuildStatus;

                case Message.MessageKind.FailingTasks:
                    return BuildMessageType.FailingTasks;

                case Message.MessageKind.Fixer:
                    return BuildMessageType.Fixer;
            }

            return BuildMessageType.Unknown;
        }
        #endregion
        #endregion
    }
}