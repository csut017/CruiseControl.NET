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
                             .Select(i => new TaskSummary
                                 {
                                     Description = i.Description,
                                     Name = i.Name,
                                     Tasks = i.ChildItems.Any() ? i.ToModel() : null
                                 })
                             .ToArray();
        }
        #endregion
        #endregion
    }
}