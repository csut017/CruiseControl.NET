namespace CruiseControlNet.VisualStudio.Models
{
    using CruiseControlNet.Common;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The model for the server summary.
    /// </summary>
    public class ServerSummary
        : ModelBase
    {
        #region Private fields
        private string version;
        private ServerStatus status;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSummary" /> class.
        /// </summary>
        public ServerSummary()
        {
            this.Projects = new ObservableCollection<ProjectSummary>();
        }
        #endregion

        #region Public properties
        #region Version
        /// <summary>
        /// Gets or sets the server version.
        /// </summary>
        public string Version
        {
            get
            {
                return this.version;
            }

            set
            {
                this.version = value;
                this.OnPropertyChanged("Version");
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the server status.
        /// </summary>
        public ServerStatus Status
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

        #region Projects
        /// <summary>
        /// Gets or sets the project summaries.
        /// </summary>
        public ObservableCollection<ProjectSummary> Projects { get; private set; }
        #endregion
        #endregion
    }
}