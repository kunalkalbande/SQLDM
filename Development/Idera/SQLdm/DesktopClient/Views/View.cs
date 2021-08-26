using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common;
using System.Collections.Generic;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Controls;

namespace Idera.SQLdm.DesktopClient.Views
{
    internal class View : UserControl, IView
    {
        private Logger logger;
        private BackgroundWorker refreshBackgroundWorker = null;
        private Stopwatch refreshStopwatch = new Stopwatch();

        public View()
        {
        //    InitializeRefreshBackgroundWorker();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CancelRefresh();
            }

            base.Dispose(disposing);
        }

        public Logger Log
        {
            get
            {
                if (logger == null)
                {
                    InitializeLogger(null);
                }

                return logger;
            }
        }

        protected enum Progress
        {
            Backfill
        }

        protected bool RefreshReportsProgress { get; set; }

        private void InitializeRefreshBackgroundWorker()
        {
            refreshBackgroundWorker = new BackgroundWorker();
            refreshBackgroundWorker.WorkerSupportsCancellation = true;
            refreshBackgroundWorker.DoWork += refreshBackgroundWorker_DoWork;
            refreshBackgroundWorker.RunWorkerCompleted += refreshBackgroundWorker_RunWorkerCompleted;
            if (RefreshReportsProgress)
            {
                refreshBackgroundWorker.WorkerReportsProgress = true;
                refreshBackgroundWorker.ProgressChanged += refreshBackgroundWorker_ProgressChanged;
            }
        }

        public bool IsRefreshBackgroundWorkerBusy
        {
            get
            {
                return (refreshBackgroundWorker != null) ? 
                    refreshBackgroundWorker.IsBusy :
                    false;
            }
        }


        /// <summary>
        /// Sets sysAdmin to true if monitored sql server has sysadmin rights
        /// </summary>
        //protected bool isSysAdmin(int instanceId)
        //{
        //    if (ApplicationModel.Default.InstancePrivilege.ContainsKey(instanceId))
        //        return ApplicationModel.Default.InstancePrivilege[instanceId];
        //    else
        //    {
        //        IManagementService managementService = ManagementServiceHelper.GetDefaultService();
        //        return managementService.isSysAdmin(instanceId);

        //    }
        //}


        private void ChangeAllControlsBackColor(ControlCollection controls, System.Drawing.Color newColor)
        {
            foreach (Control c in controls)
            {
                try
                {
                    c.BackColor = newColor;
                    System.Diagnostics.Debug.WriteLine("Control: " + c.Name + " newColor: " + newColor.ToString());
                }
                catch { }

                if (c.HasChildren)
                    ChangeAllControlsBackColor(c.Controls, newColor);


            }
        }

        public virtual void UpdateTheme(ThemeName theme)
        {
            //throw new System.Exception("UpdateTheme must be implemented in derived class");
            if (theme == ThemeName.Dark)
            {
                ChangeAllControlsBackColor(this.Controls, System.Drawing.Color.FromArgb(2, 16, 23));
            }
            else
            {
                ChangeAllControlsBackColor(this.Controls, System.Drawing.Color.White);
            }
        }

        /// <summary>
        /// Each view should override this to show the appropriate help topic,
        /// which may vary depending on the view's internal state.
        /// </summary>
        public virtual void ShowHelp() {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.HelpStartPage);
        }

        /// <summary>
        /// Creates a logger for the class based on the type name. A metadata string
        /// can be passed in to help uniquely identify the logger.
        /// </summary>
        protected void InitializeLogger(string metadata)
        {
            string typeName = GetType().Name;

            if (metadata != null && metadata.Length > 0)
            {
                logger = Logger.GetLogger(string.Format("{0} - {1}", typeName, metadata));
            }
            else
            {
                logger = Logger.GetLogger(typeName);
            }

            Log.Info("Logger initialized.");
        }

        /// <summary>
        /// Kicks off a worker thread to perform a refresh in the background.
        /// Override this method if a background worker is not desired to perform refresh work.
        /// </summary>
        public virtual void RefreshView()
        {
            if (refreshBackgroundWorker == null)
            {
                InitializeRefreshBackgroundWorker();
            }
            
            if (!refreshBackgroundWorker.IsBusy)
            {
                ApplicationController.Default.OnRefreshActiveViewStarted(EventArgs.Empty);
                Log.Info("Refreshing...");
                refreshStopwatch.Reset();
                refreshStopwatch.Start();
                refreshBackgroundWorker.RunWorkerAsync();
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
            if (refreshBackgroundWorker != null)
            {
                Log.Info("Refresh cancelled.");
                refreshBackgroundWorker.CancelAsync();
                refreshBackgroundWorker = null;
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

        /// <summary>
        /// Override this method to update data in the view, such as a grid or chart. The data
        /// collected in DoRefreshWork is passed to this method when the refresh worker completes.
        /// </summary>
        /// <param name="data">Data passed from the DoRefreshWork method.</param>
        public virtual void UpdateData(object data)
        {
        }

        /// <summary>
        /// Override this method to handle a view specific argument.
        /// </summary>
        /// <param name="argument">A view specific argument.</param>
        public virtual void SetArgument(object argument)
        {
        }

        /// <summary>
        /// Override this method to apply user settings to a view.
        /// </summary>
        public virtual void ApplySettings()
        {
        }

        /// <summary>
        /// Override this method to save user settings for a view.
        /// </summary>
        public virtual void SaveSettings()
        {
        }

        /// <summary>
        /// Override this method to update view attributes associated with user token settings.
        /// </summary>
        public virtual void UpdateUserTokenAttributes()
        {
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
                     //throw;
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
                return;
            }
            else if (e.Error != null)
            {
                HandleBackgroundWorkerError(e.Error);
            }
            else if (!IsDisposed)
            {
                refreshStopwatch.Stop();
                Log.Info(string.Format("Refresh completed (Duration = {0}).", refreshStopwatch.Elapsed));

                Stopwatch dataUpdateStopwatch = new Stopwatch();
                dataUpdateStopwatch.Start();
                Log.Info("Updating view...");

                UpdateData(e.Result);
                
                dataUpdateStopwatch.Stop();
                Log.Info(string.Format("View update completed (Duration = {0}).", dataUpdateStopwatch.Elapsed));
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
    }
}
