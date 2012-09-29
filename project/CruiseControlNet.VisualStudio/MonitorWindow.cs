namespace CruiseControlNet.VisualStudio
{
    using CruiseControlNet.Common;
    using Microsoft.VisualStudio.Shell;
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Timers;
    using ProjectSummary = CruiseControlNet.VisualStudio.Models.ProjectSummary;
    using ServerSummary = CruiseControlNet.VisualStudio.Models.ServerSummary;

    [Guid("dd4c46c8-11e5-4a5e-9596-5d55c33aa399")]
    public class MonitorWindow
        : ToolWindowPane, INotifyPropertyChanged
    {
        #region Private fields
        /// <summary>
        /// The monitor control.
        /// </summary>
        private readonly MonitorControl monitorControl = new MonitorControl();

        /// <summary>
        /// The projects.
        /// </summary>
        private readonly ObservableCollection<ProjectSummary> projects;

        /// <summary>
        /// The current status.
        /// </summary>
        private string status;

        /// <summary>
        /// The client for checking the server.
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// The update timer.
        /// </summary>
        private readonly Timer timer = new Timer(5000);

        /// <summary>
        /// A flag to indicate whether there is currently an error.
        /// </summary>
        private bool hasError;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitorWindow" /> class.
        /// </summary>
        public MonitorWindow() :
            base(null)
        {
            this.ToolBar = new CommandID(CommandTableGuids.guidCruiseControlNetCmdSet, (int)CommandTableIdentifiers.menuMonitor);
            this.Caption = Resources.ToolWindowTitle;
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
            this.monitorControl.DataContext = this;
            base.Content = this.monitorControl;
            this.client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:8810/")
                };
            this.CheckServer();
            this.timer.Elapsed += (o, e) => this.CheckServer();
            this.projects = new ObservableCollection<ProjectSummary>();
        }
        #endregion

        #region Public events
        #region PropertyChanged
        /// <summary>
        /// Occurs when a property has changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        #region Public properties
        #region Projects
        /// <summary>
        /// Gets the projects.
        /// </summary>
        /// <value>
        /// The projects.
        /// </value>
        public ObservableCollection<ProjectSummary> Projects
        {
            get
            {
                return this.projects;
            }
        }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public string Status
        {
            get
            {
                return this.status;
            }

            set
            {

                this.status = value;
                this.OnPropertyChanged("Status");
                var package = this.Package as VisualStudioPackage;
                if (package != null)
                {
                    package.SetStatus(this.status);
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region LogMessage()
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void LogMessage(string message)
        {
            var package = this.Package as VisualStudioPackage;
            if (package != null)
            {
                package.LogMessage(message);
            }
        }
        #endregion

        #region OnPropertyChanged()
        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        #region CheckServer()
        /// <summary>
        /// Checks the server.
        /// </summary>
        private void CheckServer()
        {
            this.timer.Stop();
            this.Status = "Checking CruiseControl.NET server...";
            this.client.GetAsync("api/server").ContinueWith(RetrieveSummaries);
        }
        #endregion

        #region RetrieveSummaries()
        /// <summary>
        /// Retrieves the summaries.
        /// </summary>
        /// <param name="response">The response.</param>
        private void RetrieveSummaries(Task<HttpResponseMessage> response)
        {
            if ((response.Status == TaskStatus.RanToCompletion) && response.Result.IsSuccessStatusCode)
            {
                this.Status = string.Empty;
                response.Result
                    .Content
                    .ReadAsAsync<ServerSummary>()
                    .ContinueWith(r =>
                        {
                            if (r.Status == TaskStatus.RanToCompletion)
                            {
                                this.monitorControl.Dispatcher.Invoke(() => this.SynchroniseProjects(r.Result));
                            }
                        });
            }
            else
            {
                this.Status = "CruiseControl.NET server failed to respond";
                if (!hasError)
                {
                    hasError = true;
                    this.LogMessage("Server failed to respond");
                    foreach (var project in this.projects)
                    {
                        project.Status = ProjectSummaryStatus.Unknown;
                        project.Activity = ProjectSummaryActivity.Unknown;
                        project.BuildStatus = ProjectSummaryBuildStatus.Unknown;
                        project.Labels = null;
                        project.Messages.Clear();
                        project.Times = null;
                    }
                }
            }

            this.timer.Start();
        }
        #endregion

        #region SynchroniseProjects
        /// <summary>
        /// Synchronises the projects.
        /// </summary>
        /// <param name="result">The server summary with the projects.</param>
        private void SynchroniseProjects(ServerSummary result)
        {
            var current = this.projects.ToDictionary(p => p.Name);
            foreach (var project in result.Projects)
            {
                ProjectSummary currentProject;
                if (current.TryGetValue(project.Name, out currentProject))
                {
                    current.Remove(project.Name);
                    currentProject.Status = project.Status;
                    currentProject.Activity = project.Activity;
                    currentProject.BuildStatus = project.BuildStatus;
                    currentProject.Labels = project.Labels;
                    currentProject.Times = project.Times;
                    currentProject.Messages.Clear();
                    foreach (var message in project.Messages)
                    {
                        currentProject.Messages.Add(message);
                    }
                }
                else
                {
                    this.projects.Add(project);
                }
            }

            foreach (var project in current.Values)
            {
                this.projects.Remove(project);
            }
        }
        #endregion
        #endregion
    }
}
