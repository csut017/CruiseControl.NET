namespace CruiseControlNet.SelfHost.Console.Helpers
{
    using ThoughtWorks.CruiseControl.Core;

    /// <summary>
    /// Extension methods for <see cref="LogFile"/>.
    /// </summary>
    public static class LogFileExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts a <see cref="LogFile"/> to a <see cref="Models.BuildSummary"/>.
        /// </summary>
        /// <param name="logFile">The <see cref="LogFile"/> to convert.</param>
        /// <returns>
        /// The converted <see cref="Models.BuildSummary"/>.
        /// </returns>
        public static Models.BuildSummary ToModel(this LogFile logFile)
        {
            var model = new Models.BuildSummary
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