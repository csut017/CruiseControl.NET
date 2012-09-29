namespace CruiseControlNet.SelfHost.Helpers
{
    using CruiseControlNet.Common;
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
                    BuildStage = projectStatus.BuildStage.Length == 0 ? null : projectStatus.BuildStage,
                    Status = Convert(projectStatus.Status),
                    BuildStatus = Convert(projectStatus.BuildStatus),
                    Times = new ProjectSummaryTimes
                        {
                            LastRun = projectStatus.LastBuildDate
                        },
                    Messages = projectStatus.Messages.Length == 0 ? null : projectStatus.Messages.Select(m => m.ToModel()).ToArray()
                };

            // Activity is only valid if the project is running
            model.Activity = projectStatus.Status == ProjectIntegratorState.Running
                ? Convert(projectStatus.Activity)
                : ProjectSummaryActivity.NotRunning;

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

        /// <summary>
        /// Converts a <see cref="ProjectActivity"/> to a <see cref="ProjectSummaryActivity"/>.
        /// </summary>
        /// <param name="activity">The <see cref="ProjectActivity"/>.</param>
        /// <returns>
        /// The <see cref="ProjectSummaryActivity"/>.
        /// </returns>
        private static ProjectSummaryActivity Convert(ProjectActivity activity)
        {
            if (activity.IsBuilding())
            {
                return ProjectSummaryActivity.Building;
            }

            if (activity.IsCheckingModifications())
            {
                return ProjectSummaryActivity.CheckingModifications;
            }

            if (activity.IsPending())
            {
                return ProjectSummaryActivity.Pending;
            }

            if (activity.IsSleeping())
            {
                return ProjectSummaryActivity.Sleeping;
            }

            return ProjectSummaryActivity.Unknown;
        }
        #endregion
        #endregion

        #region Private methods
        #region Convert()
        /// <summary>
        /// Converts a <see cref="IntegrationStatus"/> to a <see cref="ProjectSummaryBuildStatus"/>.
        /// </summary>
        /// <param name="buildStatus">The <see cref="IntegrationStatus"/>.</param>
        /// <returns>
        /// The <see cref="ProjectSummaryBuildStatus"/>.
        /// </returns>
        private static ProjectSummaryBuildStatus Convert(IntegrationStatus buildStatus)
        {
            switch (buildStatus)
            {
                case IntegrationStatus.Cancelled:
                    return ProjectSummaryBuildStatus.Cancelled;

                case IntegrationStatus.Exception:
                    return ProjectSummaryBuildStatus.Exception;

                case IntegrationStatus.Failure:
                    return ProjectSummaryBuildStatus.Failure;

                case IntegrationStatus.Success:
                    return ProjectSummaryBuildStatus.Success;
            }

            return ProjectSummaryBuildStatus.Unknown;
        }

        /// <summary>
        /// Converts a <see cref="ProjectIntegratorState"/> to a <see cref="ProjectSummaryStatus"/>.
        /// </summary>
        /// <param name="status">The <see cref="ProjectIntegratorState"/>.</param>
        /// <returns>
        /// The <see cref="ProjectSummaryStatus"/>.
        /// </returns>
        private static ProjectSummaryStatus Convert(ProjectIntegratorState status)
        {
            switch (status)
            {
                case ProjectIntegratorState.Running:
                    return ProjectSummaryStatus.Running;

                case ProjectIntegratorState.Stopped:
                    return ProjectSummaryStatus.Stopped;

                case ProjectIntegratorState.Stopping:
                    return ProjectSummaryStatus.Stopping;

                case ProjectIntegratorState.Unknown:
                    // Project has not started
                    return ProjectSummaryStatus.Stopped;
            }

            return ProjectSummaryStatus.Unknown;
        }
        #endregion
        #endregion
    }
}