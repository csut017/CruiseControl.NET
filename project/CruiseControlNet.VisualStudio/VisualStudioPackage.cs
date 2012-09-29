namespace CruiseControlNet.VisualStudio
{
    using CruiseControlNet.Common;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Globalization;
    using System.Net.Http;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using ProjectSummary = CruiseControlNet.VisualStudio.Models.ProjectSummary;

    /// <summary>
    /// The main package class.
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(MonitorWindow))]
    [Guid(CommandTableStrings.guidCruiseControlNetPackage)]
    public sealed class VisualStudioPackage
        : Package, INotifyPropertyChanged
    {
        #region Private fields
        /// <summary>
        /// The commands that have been added by this package.
        /// </summary>
        private readonly Dictionary<uint, OleMenuCommand> commands = new Dictionary<uint, OleMenuCommand>();

        /// <summary>
        /// The client for checking the server.
        /// </summary>
        private readonly HttpClient client;

        /// <summary>
        /// The menu command service to use.
        /// </summary>
        private OleMenuCommandService menuCommandService;

        /// <summary>
        /// The output pane to use.
        /// </summary>
        private IVsOutputWindowPane outputPane;

        /// <summary>
        /// Interface to the status bar.
        /// </summary>
        private IVsStatusbar statusBar;

        /// <summary>
        /// The currently selected project.
        /// </summary>
        private ProjectSummary selectedProject;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="VisualStudioPackage" /> class.
        /// </summary>
        public VisualStudioPackage()
        {
            this.client = new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:8810/")
                };
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
        #region SelectedProject
        /// <summary>
        /// Gets or sets the selected project.
        /// </summary>
        public ProjectSummary SelectedProject
        {
            get
            {
                return this.selectedProject;
            }

            set
            {
                if (this.selectedProject != null)
                {
                    this.selectedProject.PropertyChanged -= this.OnProjectChange;
                }

                this.selectedProject = value;
                this.OnPropertyChanged("SelectedProject");
                this.UpdateCommands();
                if (this.selectedProject != null)
                {
                    this.selectedProject.PropertyChanged += this.OnProjectChange;
                }
            }
        }
        #endregion
        #endregion

        #region Public methods
        #region LogMessage()
        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void LogMessage(string message)
        {
            if (this.outputPane == null)
            {
                return;
            }

            this.outputPane.OutputString(message + Environment.NewLine);
        }
        #endregion

        #region SetStatus()
        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="status">The status.</param>
        public void SetStatus(string status)
        {
            if (this.statusBar == null)
            {
                this.statusBar = (IVsStatusbar)GetService(typeof(SVsStatusbar));
            }

            if (this.statusBar == null)
            {
                return;
            }

            int frozen;
            this.statusBar.IsFrozen(out frozen);
            if (frozen != 0)
            {
                return;
            }

            this.statusBar.SetText(status ?? string.Empty);
        }
        #endregion
        #endregion

        #region Protected methods
        #region Initialize()
        /// <summary>
        /// Called when the VSPackage is loaded by Visual Studio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Retrieve the output pane
            var output = (IVsOutputWindow)GetService(typeof(SVsOutputWindow));
            IVsOutputWindowPane pane;
            var paneGuid = new Guid("{3F364759-2F9D-4C53-984F-5E790335ECC4}");
            output.CreatePane(ref paneGuid, "CruiseControl.NET", Convert.ToInt32(true), Convert.ToInt32(false));
            output.GetPane(ref paneGuid, out pane);
            this.outputPane = pane;

            // Initialise the menu services
            this.menuCommandService = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == this.menuCommandService)
            {
                return;
            }

            // Add the commands
            this.AddMenuCommandHandler(CommandTableIdentifiers.cmdSettings, this.ShowSettings);
            this.AddMenuCommandHandler(CommandTableIdentifiers.cmdForceProject, this.ForceProject);
            this.AddMenuCommandHandler(CommandTableIdentifiers.cmdAbortProject, this.AbortProject);
            this.AddMenuCommandHandler(CommandTableIdentifiers.cmdStartProject, this.StartProject);
            this.AddMenuCommandHandler(CommandTableIdentifiers.cmdStopProject, this.StopProject);
            this.AddToolWindowMenuCommand<MonitorWindow>(CommandTableIdentifiers.cmdMonitorWindow);
            this.UpdateCommands();

            this.LogMessage("CruiseControl.NET monitor initialised");
        }
        #endregion
        #endregion

        #region Private methods
        #region AddMenuCommandHandler()
        /// <summary>
        /// Adds a command handler for a menu.
        /// </summary>
        /// <param name="commandId">The command identifier.</param>
        /// <param name="menuItemCallback">The command handler.</param>
        private void AddMenuCommandHandler(uint commandId, EventHandler menuItemCallback)
        {
            var menuCommandId = new CommandID(CommandTableGuids.guidCruiseControlNetCmdSet, (int)commandId);
            var menuItem = new OleMenuCommand(menuItemCallback, menuCommandId);
            this.menuCommandService.AddCommand(menuItem);
            this.commands.Add(commandId, menuItem);
        }
        #endregion

        #region AddToolWindowMenuCommand()
        /// <summary>
        /// Adds a menu command for showing a tool window.
        /// </summary>
        /// <typeparam name="TToolWindow">The type of the tool window to show.</typeparam>
        /// <param name="commandId">The command identifier.</param>
        private void AddToolWindowMenuCommand<TToolWindow>(uint commandId)
            where TToolWindow : ToolWindowPane
        {
            this.AddMenuCommandHandler(
                commandId,
                (o, e) => this.ShowToolWindow<TToolWindow>());
        }
        #endregion

        #region ShowToolWindow()
        /// <summary>
        /// Shows the tool window.
        /// </summary>
        private void ShowToolWindow<TToolWindow>()
            where TToolWindow : ToolWindowPane
        {
            var window = this.FindToolWindow(typeof(TToolWindow), 0, true);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Unable to find " + typeof(TToolWindow).Name);
            }

            var windowFrame = (IVsWindowFrame)window.Frame;
            ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }
        #endregion

        #region ShowSettings()
        /// <summary>
        /// The menu item callback.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ShowSettings(object sender, EventArgs e)
        {
            var uiShell = (IVsUIShell)GetService(typeof(SVsUIShell));
            var clsid = Guid.Empty;
            int result;
            ErrorHandler.ThrowOnFailure(uiShell.ShowMessageBox(
                       0,
                       ref clsid,
                       "CruiseControl.Net Plugin",
                       string.Format(CultureInfo.CurrentCulture, "Inside {0}.ShowSettings()", this),
                       string.Empty,
                       0,
                       OLEMSGBUTTON.OLEMSGBUTTON_OK,
                       OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                       OLEMSGICON.OLEMSGICON_INFO,
                       0,
                       out result));
        }
        #endregion

        #region ForceProject()
        /// <summary>
        /// Forces a project build.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void ForceProject(object sender, EventArgs e)
        {
            var project = this.selectedProject;
            this.client.PostAsJsonAsync("api/project/" + project.Name + "/build", (string)null)
                .ContinueWith(r =>
                    {
                        project.Activity = ProjectSummaryActivity.Pending;
                        this.CheckResult(r, "Building project");
                        this.UpdateCommands();
                    });
        }
        #endregion

        #region AbortProject()
        /// <summary>
        /// Aborts a project build.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void AbortProject(object sender, EventArgs e)
        {
            var project = this.selectedProject;
            this.client.PostAsJsonAsync("api/project/" + project.Name + "/abort", (string)null)
                .ContinueWith(r =>
                    {
                        this.CheckResult(r, "Aborting project");
                        this.UpdateCommands();
                    });
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Starts a project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StartProject(object sender, EventArgs e)
        {
            var project = this.selectedProject;
            this.client.PostAsJsonAsync("api/project/" + project.Name + "/start", (string)null)
                .ContinueWith(r =>
                    {
                        project.Status = ProjectSummaryStatus.Starting;
                        this.CheckResult(r, "Project starting");
                        this.UpdateCommands();
                    });
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stops a project.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        private void StopProject(object sender, EventArgs e)
        {
            var project = this.selectedProject;
            this.client.PostAsJsonAsync("api/project/" + project.Name + "/stop", (string)null)
                .ContinueWith(r =>
                    {
                        project.Status = ProjectSummaryStatus.Stopping;
                        this.CheckResult(r, "Project stopping");
                        this.UpdateCommands();
                    });
        }
        #endregion

        #region CheckResult()
        /// <summary>
        /// Checks the result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="message">The message.</param>
        private void CheckResult(Task<HttpResponseMessage> result, string message)
        {
            if ((result.Status == TaskStatus.RanToCompletion) && result.Result.IsSuccessStatusCode)
            {
                this.LogMessage(message);
                this.SetStatus(message);
            }
            else
            {
                this.SetStatus("CruiseControl.NET server unable to perform action");
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

        #region UpdateCommands()
        /// <summary>
        /// Updates the commands.
        /// </summary>
        private void UpdateCommands()
        {
            var hasProject = this.selectedProject != null;
            this.commands[CommandTableIdentifiers.cmdForceProject].Enabled = hasProject
                && (this.selectedProject.Status == ProjectSummaryStatus.Running)
                && (this.selectedProject.Activity == ProjectSummaryActivity.Sleeping);
            this.commands[CommandTableIdentifiers.cmdAbortProject].Enabled = hasProject
                && (this.selectedProject.Status == ProjectSummaryStatus.Running)
                && (this.selectedProject.Activity != ProjectSummaryActivity.Sleeping)
                && (this.selectedProject.Activity != ProjectSummaryActivity.Unknown);
            this.commands[CommandTableIdentifiers.cmdStartProject].Enabled = hasProject
                && (this.selectedProject.Status == ProjectSummaryStatus.Stopped);
            this.commands[CommandTableIdentifiers.cmdStopProject].Enabled = hasProject
                && (this.selectedProject.Status == ProjectSummaryStatus.Running);
        }
        #endregion

        #region OnProjectChange()
        /// <summary>
        /// Called when the selected project has changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void OnProjectChange(object sender, PropertyChangedEventArgs e)
        {
            this.UpdateCommands();
        }
        #endregion
        #endregion
    }
}
