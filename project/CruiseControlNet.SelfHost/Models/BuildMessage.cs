namespace CruiseControlNet.SelfHost.Models
{
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Defines the model for a build message.
    /// </summary>
    public class BuildMessage
    {
        #region Public properties
        #region Text
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
        #endregion

        #region Type
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public Message.MessageKind Type { get; set; }
        #endregion
        #endregion
    }
}