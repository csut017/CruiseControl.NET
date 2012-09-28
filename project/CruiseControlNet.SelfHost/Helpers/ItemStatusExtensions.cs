namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.SelfHost.Models;
    using System.Linq;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Extensions to <see cref="ItemStatus"/>.
    /// </summary>
    public static class ItemStatusExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts an <see cref="ItemStatus"/> to a model.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// The converted model.
        /// </returns>
        public static TaskSummary[] ToModel(this ItemStatus status)
        {
            return status == null
                       ? null
                       : status.ChildItems
                             .Select(Convert)
                             .ToArray();
        }
        #endregion
        #endregion

        #region Private methods
        #region Convert()
        /// <summary>
        /// Converts the specified status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns>
        /// The summary.
        /// </returns>
        private static TaskSummary Convert(ItemStatus status)
        {
            var model = new TaskSummary
                {
                    Description = status.Description,
                    Name = status.Name,
                    Tasks = status.ChildItems.Any() ? status.ToModel() : null,
                    Status = status.Status,
                    Error = status.Error
                };
            if (status.TimeStarted.HasValue || status.TimeCompleted.HasValue)
            {
                model.Times = new TaskSummaryTimes
                    {
                        Started = status.TimeStarted,
                        Completed = status.TimeCompleted
                    };
            }

            return model;
        }
        #endregion
        #endregion
    }
}