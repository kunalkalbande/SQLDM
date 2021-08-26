using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Controls;
using UserControl = System.Windows.Controls.UserControl;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server
{
    public class WpfServerBaseView : UserControl, IServerView
    {
        private bool _disposed;
        private Logger _logger;
        protected int _instanceId;

        private BackgroundWorker _refreshBackgroundWorker = null;
        private readonly Stopwatch _refreshStopwatch = new Stopwatch();

        protected AlertConfiguration alerts;

        public WpfServerBaseView()
        {
            _logger = Logger.GetLogger(GetType().Name);
        }

        public WpfServerBaseView(int instanceId)
        {
            _instanceId = instanceId;
            
            if (ApplicationModel.Default.ActiveInstances.Contains(instanceId))
            {
                InitializeLogger(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName);
            }
        }

        #region disposal

        ~WpfServerBaseView()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelRefresh();

                var parent = Parent as System.Windows.Controls.Panel;
                if (parent != null)
                {
                    parent.Children.Remove(this);
                }

                if (_refreshBackgroundWorker != null && !_refreshBackgroundWorker.IsBusy)
                    _refreshBackgroundWorker.Dispose();
            }

            _disposed = true;
        }

        public bool IsDisposed { get { return _disposed; } }

        #endregion

        public int InstanceId { get { return _instanceId; } }

        public ServerViewMode ViewMode { get; set; }

        /// <summary>
        /// Creates a logger for the class based on the type name. A metadata string
        /// can be passed in to help uniquely identify the logger.
        /// </summary>
        protected void InitializeLogger(string metadata)
        {
            string typeName = GetType().Name;

            _logger = Logger.GetLogger(!string.IsNullOrEmpty(metadata) ? string.Format("{0} - {1}", typeName, metadata) : typeName);

            Log.Info("Logger initialized.");
        }

        public virtual void UpdateTheme(ThemeName theme)
        {
            throw new System.Exception("UpdateTheme must be implemented in derived class");
        }

        /// <summary>
        /// Override this property to manage the historical snapshot start timestamp
        /// </summary>
        public virtual DateTime? HistoricalStartDateTime { get; set; }

        /// <summary>
        /// Override this property to manage the historical snapshot end timestamp
        /// </summary>
        public virtual DateTime? HistoricalSnapshotDateTime
        {
            get { return null; }
            set { }
        }

        public Logger Log
        {
            get
            {
                if (_logger == null)
                {
                    InitializeLogger(null);
                }

                return _logger;
            }
        }

        public virtual void SetArgument(object argument)
        {

        }

        public virtual void ApplySettings()
        {

        }

        public virtual void SaveSettings()
        {

        }

        public virtual void ShowHelp()
        {

        }

        public virtual void UpdateUserTokenAttributes()
        {

        }

        #region Server Base View 

        internal AlertConfiguration GetSharedAlertConfiguration()
        {
            if (alerts == null)
            {
                alerts = ApplicationModel.Default.GetAlertConfiguration(_instanceId);
            }
            return alerts;
        }

        #endregion

        #region Refresh Worker Stuff

        protected enum Progress
        {
            Backfill
        }

        protected bool RefreshReportsProgress { get; set; }

        private void InitializeRefreshBackgroundWorker()
        {
            _refreshBackgroundWorker = new BackgroundWorker();
            _refreshBackgroundWorker.WorkerSupportsCancellation = true;
            _refreshBackgroundWorker.DoWork += refreshBackgroundWorker_DoWork;
            _refreshBackgroundWorker.RunWorkerCompleted += refreshBackgroundWorker_RunWorkerCompleted;
            if (RefreshReportsProgress)
            {
                _refreshBackgroundWorker.WorkerReportsProgress = true;
                _refreshBackgroundWorker.ProgressChanged += refreshBackgroundWorker_ProgressChanged;
            }
        }

        public bool IsRefreshBackgroundWorkerBusy
        {
            get
            {
                return (_refreshBackgroundWorker != null) ?
                    _refreshBackgroundWorker.IsBusy :
                    false;
            }
        }

        /// <summary>
        /// Kicks off a worker thread to perform a refresh in the background.
        /// Override this method if a background worker is not desired to perform refresh work.
        /// </summary>
        public virtual void RefreshView()
        {
            if (_refreshBackgroundWorker == null)
            {
                InitializeRefreshBackgroundWorker();
            }

            if (!_refreshBackgroundWorker.IsBusy)
            {
                ApplicationController.Default.OnRefreshActiveViewStarted(EventArgs.Empty);
                Log.Info("Refreshing...");
                _refreshStopwatch.Reset();
                _refreshStopwatch.Start();
                _refreshBackgroundWorker.RunWorkerAsync();
            }
            else
            {
                Log.Info("A refresh could not be started because another refresh is in progress.");
            }
        }

        /// <summary>
        /// Notifies the refresh worker that a cancellation is pending. This method is generally
        /// called by controllers to cancel refreshes that may be occurring when a view is changing.
        /// </summary>
        public virtual void CancelRefresh()
        {
            if (_refreshBackgroundWorker != null)
            {
                Log.Info("Refresh cancelled.");
                _refreshBackgroundWorker.CancelAsync();
                //_refreshBackgroundWorker = null;
            }
        }

        /// <summary>
        /// Override this method to perform refresh work such as retrieving data from the
        /// repository or management service. It is not necessary to call this base method.
        /// </summary>
        /// <param name="backgroundWorker">BackgroundWorker object that invoked this method.</param>
        /// <returns>Returns data for display in the view.</returns>
        public virtual object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            return DoRefreshWork();
        }


        /// <summary>
        /// Override this method to perform refresh work such as retrieving data from the
        /// repository or management service. It is not necessary to call this base method.
        /// </summary>
        public virtual object DoRefreshWork()
        {
            return null;
        }

        protected virtual void UpdateProgress(object state, int progress)
        {
        }

        /// <summary>
        /// Override this method to update data in the view, such as a grid or chart. The data
        /// collected in DoRefreshWork is passed to this method when the refresh worker completes.
        /// </summary>
        /// <param name="data">Data passed from the DoRefreshWork method.</param>
        public virtual void UpdateData(object data)
        {
        }

        /// <summary>
        /// Override this method to provide custom handling of background worker errors.
        /// The base method should be called to ensure that the refresh completed event is fired.
        /// </summary>
        /// <param name="e">The exception that occurred while performing background work.</param>
        public virtual void HandleBackgroundWorkerError(Exception e)
        {
            Log.Error("An error occurred while refreshing.", e);
            ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now, e));
        }


        private void refreshBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "BaseViewRefresh";

            BackgroundWorker backgroundWorker = sender as BackgroundWorker;

            try
            {
                e.Result = DoRefreshWork(backgroundWorker);
            }
            catch
            {
                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    return;
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                if (backgroundWorker != null && backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private void refreshBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                if (_refreshBackgroundWorker != null)
                    _refreshBackgroundWorker.Dispose();

                _refreshBackgroundWorker = null;
                return;
            }
            else if (e.Error != null)
            {
                HandleBackgroundWorkerError(e.Error);
            }
            else if (!IsDisposed)
            {
                _refreshStopwatch.Stop();
                Log.Info(string.Format("Refresh completed (Duration = {0}).", _refreshStopwatch.Elapsed));

                Stopwatch dataUpdateStopwatch = new Stopwatch();
                dataUpdateStopwatch.Start();
                Log.Info("Updating view...");

                UpdateData(e.Result);

                dataUpdateStopwatch.Stop();
                Log.Info(string.Format("View update completed (Duration = {0}).", dataUpdateStopwatch.Elapsed));
            }
            else if (IsDisposed)
            {
                if (_refreshBackgroundWorker != null)
                    _refreshBackgroundWorker.Dispose();
            }
        }

        private void refreshBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (IsDisposed) return;

            var dataUpdateStopwatch = new Stopwatch();
            dataUpdateStopwatch.Start();
            Log.Info("Updating view progress...");

            UpdateProgress(e.UserState, e.ProgressPercentage);

            dataUpdateStopwatch.Stop();
            Log.Info(string.Format("View update progress completed (Duration = {0}).", dataUpdateStopwatch.Elapsed));
        }

        #endregion
    }

    public class WpfServerDesignBaseView : WpfServerBaseView, IServerDesignView
    {
        protected bool designMode = false;
        protected bool hasDesignChanges = false;

        /// <summary>
        /// The default constructor is required so the designers will work. Generally any derived
        /// view should call the base constructor and pass in the instance ID.
        /// </summary>
        public WpfServerDesignBaseView()
        {
        }

        public WpfServerDesignBaseView(int instanceId)
            : base(instanceId)
        {
        }

        public string DesignTab { get; set; }

        public bool DesignModeEnabled
        {
            get { return designMode; }
        }

        public void ToggleDesignMode()
        {
            ToggleDesignMode(!designMode);
        }

        public virtual void ToggleDesignMode(bool enabled)
        {
            designMode = enabled;
        }

        public virtual void CheckIfSaveNeeded()
        {
            if (hasDesignChanges)
            {
                if (DialogResult.Yes == ApplicationMessageBox.ShowQuestion(this.GetWinformWindow(), "You have made changes to the current design. Do you wish to save the changes before continuing?"))
                {
                    SaveDashboardDesign();
                }
            }
        }

        public virtual void SaveDashboardDesign()
        {

        }
    }
}
