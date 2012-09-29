namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.Common;
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
                    Status = Convert(status.Status),
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

        /// <summary>
        /// Converts a <see cref="ItemBuildStatus"/> to a <see cref="TaskSummaryStatus"/>.
        /// </summary>
        /// <param name="status">The <see cref="ItemBuildStatus"/>.</param>
        /// <returns>
        /// The <see cref="TaskSummaryStatus"/>.
        /// </returns>
        private static TaskSummaryStatus Convert(ItemBuildStatus status)
        {
            switch (status)
            {
                case ItemBuildStatus.Cancelled:
                    return TaskSummaryStatus.Cancelled;

                case ItemBuildStatus.CompletedFailed:
                    return TaskSummaryStatus.CompletedFailed;

                case ItemBuildStatus.CompletedSuccess:
                    return TaskSummaryStatus.CompletedSuccess;

                case ItemBuildStatus.Pending:
                    return TaskSummaryStatus.Pending;

                case ItemBuildStatus.Running:
                    return TaskSummaryStatus.Running;
            }

            return TaskSummaryStatus.Unknown;
        }
        #endregion
        #endregion
    }
}