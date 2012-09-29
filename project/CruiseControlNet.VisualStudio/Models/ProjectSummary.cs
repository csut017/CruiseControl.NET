namespace CruiseControlNet.VisualStudio.Models
{
    using CruiseControlNet.Common;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The model for a project summary.
    /// </summary>
    public class ProjectSummary
        : ModelBase
    {
        #region Private fields
        private string name;
        private string category;
        private ProjectSummaryActivity activity;
        private ProjectSummaryStatus status;
        private ProjectSummaryBuildStatus buildStatus;
        private string buildStage;
        private ProjectSummaryTimes times;
        private ProjectSummaryLabels labels;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectSummary" /> class.
        /// </summary>
        public ProjectSummary()
        {
            this.Messages = new ObservableCollection<BuildMessage>();
            this.Tasks = new ObservableCollection<TaskSummary>();
        }
        #endregion

        #region Public properties
        #region Name
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.OnPropertyChanged("Name");
            }
        }
        #endregion

        #region Category
        /// <summary>
        /// Gets or sets the project category.
        /// </summary>
        public string Category
        {
            get
            {
                return this.category;
            }

            set
            {
                this.category = value;
                this.OnPropertyChanged("Category");
            }
        }
        #endregion

        #region Activity
        /// <summary>
        /// Gets or sets the activity.
        /// </summary>
        public ProjectSummaryActivity Activity
        {
            get
            {
                return this.activity;
            }

            set
            {
                this.activity = value;
                this.OnPropertyChanged("Activity");
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public ProjectSummaryStatus Status
        {
            get
            {
                return this.status;
            }

            set
            {
                this.status = value;
                this.OnPropertyChanged("Status");
            }
        }
        #endregion

        #region BuildStatus
        /// <summary>
        /// Gets or sets the build status.
        /// </summary>
        public ProjectSummaryBuildStatus BuildStatus
        {
            get
            {
                return this.buildStatus;
            }

            set
            {
                this.buildStatus = value;
                this.OnPropertyChanged("BuildStatus");
            }
        }
        #endregion

        #region BuildStage
        /// <summary>
        /// Gets or sets the build stage.
        /// </summary>
        public string BuildStage
        {
            get
            {
                return this.buildStage;
            }

            set
            {
                this.buildStage = value;
                this.OnPropertyChanged("BuildStage");
            }
        }
        #endregion

        #region Times
        /// <summary>
        /// Gets or sets the times.
        /// </summary>
        public ProjectSummaryTimes Times
        {
            get
            {
                return this.times;
            }

            set
            {
                this.times = value;
                this.OnPropertyChanged("Times");
            }
        }
        #endregion

        #region Labels
        /// <summary>
        /// Gets or sets the labels.
        /// </summary>
        public ProjectSummaryLabels Labels
        {
            get
            {
                return this.labels;
            }

            set
            {
                this.labels = value;
                this.OnPropertyChanged("Labels");
            }
        }
        #endregion

        #region Messages
        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public ObservableCollection<BuildMessage> Messages { get; set; }
        #endregion

        #region Tasks
        /// <summary>
        /// Gets or sets the child tasks.
        /// </summary>
        public ObservableCollection<TaskSummary> Tasks { get; private set; }
        #endregion
        #endregion
    }
}