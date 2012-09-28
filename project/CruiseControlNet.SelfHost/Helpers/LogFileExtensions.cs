namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.SelfHost.Models;
    using ThoughtWorks.CruiseControl.Core;

    /// <summary>
    /// Extension methods for <see cref="LogFile"/>.
    /// </summary>
    public static class LogFileExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts a <see cref="LogFile"/> to a <see cref="BuildSummary"/>.
        /// </summary>
        /// <param name="logFile">The <see cref="BuildSummary"/> to convert.</param>
        /// <returns>
        /// The converted <see cref="BuildSummary"/>.
        /// </returns>
        public static BuildSummary ToModel(this LogFile logFile)
        {
            var model = new BuildSummary
                {
                    Name = logFile.Filename,
                    Date = logFile.Date,
                    Success = logFile.Succeeded
                };
            if (logFile.Succeeded)
            {
                model.Label = logFile.Label;
            }

            return model;
        }
        #endregion
        #endregion
    }
}