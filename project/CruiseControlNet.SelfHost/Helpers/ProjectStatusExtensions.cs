namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.SelfHost.Models;
    using System.Linq;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Extension methods for <see cref="ProjectStatus"/>.
    /// </summary>
    public static class ProjectStatusExtensions
    {
        #region Public methods
        #region ToModel()
        /// <summary>
        /// Converts a <see cref="ProjectStatus"/> to a model.
        /// </summary>
        /// <param name="projectStatus">The project status.</param>
        /// <returns>
        /// The new <see cref="ProjectSummary"/>.
        /// </returns>
        public static ProjectSummary ToModel(this ProjectStatus projectStatus)
        {
            // These properties are always needed
            var model = new ProjectSummary
                {
                    Name = projectStatus.Name,
                    Activity = projectStatus.Activity.ToString(),
                    BuildStage = projectStatus.BuildStage.Length == 0 ? null : projectStatus.BuildStage,
                    Status = projectStatus.Status,
                    BuildStatus = projectStatus.BuildStatus,
                    Times = new ProjectSummaryTimes
                        {
                            LastRun = projectStatus.LastBuildDate
                        },
                    Messages = projectStatus.Messages.Length == 0 ? null : projectStatus.Messages.Select(m => m.ToModel()).ToArray()
                };

            // Only add the next run time if we are waiting
            if ((projectStatus.Status == ProjectIntegratorState.Running) && projectStatus.Activity.IsSleeping())
            {
                model.Times.NextRun = projectStatus.NextBuildTime;
            }

            // Work out which labels are needed
            var hasLastBuild = !string.IsNullOrWhiteSpace(projectStatus.LastBuildLabel);
            var hasLastSuccessfulBuild = !string.IsNullOrWhiteSpace(projectStatus.LastSuccessfulBuildLabel)
                && (projectStatus.BuildStatus != IntegrationStatus.Success);
            if (hasLastBuild || hasLastSuccessfulBuild)
            {
                model.Labels = new ProjectSummaryLabels();
                if (hasLastBuild)
                {
                    model.Labels.Last = projectStatus.LastBuildLabel;
                }

                // Only set the last successful if it is different from the last build (and we are not in a success state)
                if (hasLastSuccessfulBuild && (projectStatus.LastBuildLabel != projectStatus.LastSuccessfulBuildLabel))
                {
                    model.Labels.LastSuccessful = projectStatus.LastSuccessfulBuildLabel;
                }
            }

            return model;
        }
        #endregion
        #endregion
    }
}