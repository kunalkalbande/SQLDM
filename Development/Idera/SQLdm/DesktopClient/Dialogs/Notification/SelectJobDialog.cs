using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System.Threading;
    using BBS.TracerX;
    using Helpers;
    using Common.Configuration;
    using Common.Configuration.ServerActions;
    using Common.Objects;
    using Common.Services;
    using Common.Snapshots;
    using Common.UI.Dialogs;
    using Infragistics.Win;
    using Properties;

    public partial class SelectJobDialog : BaseDialog
    {
        private static Logger LOG = Logger.GetLogger("SelectJobDialog");
        private enum SelectJobMode { Job, JobStep }

        private SelectJobMode mode;
        private string serverName;
        private string jobName;
        private string jobStep;
        private IManagementService managementService;
        private bool changingstuff = false;
        private JobsAndStepsConfiguration configuration;

        private SelectJobDialog(SelectJobMode mode)
        {
            this.DialogHeader = "Select Job";
            this.mode = mode;
            InitializeComponent();

            // Auto scale font size.
            AdaptFontSize();
        }

        public static DialogResult SelectJobName(IWin32Window owner, ref string serverName, ref string jobName)
        {
            using (LOG.InfoCall("SelectJobName"))
            {
                DialogResult result = DialogResult.None;
                using (SelectJobDialog dlg = new SelectJobDialog(SelectJobMode.Job))
                {
                    dlg.serverName = serverName ?? String.Empty;
                    dlg.jobName = jobName;
                    result = dlg.ShowDialog(owner);
                    if (result == DialogResult.OK)
                    {
                        jobName = dlg.GetSelectedName();
                        serverName = dlg.serverName;
                    }
                }
                return result;
            }
        }

        public static DialogResult SelectJobStep(IWin32Window owner, ref string serverName, string jobName, ref string jobStep)
        {
            using (LOG.InfoCall("SelectJobStep"))
            {
                DialogResult result = DialogResult.None;
                using (SelectJobDialog dlg = new SelectJobDialog(SelectJobMode.JobStep))
                {
                    dlg.serverName = serverName ?? String.Empty;
                    dlg.jobName = jobName;
                    dlg.jobStep = jobStep;
                    result = dlg.ShowDialog(owner);
                    if (result == DialogResult.OK)
                    {
                        jobStep = dlg.GetSelectedName();
                        if (String.IsNullOrEmpty(serverName))
                            serverName = dlg.serverName;
                    }
                }
                return result;
            }
        }

        private string GetSelectedName()
        {
            if (jobListBox.CheckedIndices.Count > 0)
                return jobListBox.Items[jobListBox.CheckedIndices[0]].ToString();

            return String.Empty;
        }

        private void serverComboBox_SelectionChanged(object sender, EventArgs e)
        {
            ValueListItem selectedItem = serverComboBox.SelectedItem;
            if (selectedItem != null)
            {
                MonitoredSqlServer server = (MonitoredSqlServer)selectedItem.ListObject;
                serverName = server.InstanceName;
            }
            UpdateControls();
        }

        private void SelectJobDialog_Load(object sender, EventArgs e)
        {
            if (mode == SelectJobMode.JobStep)
            {
                Text = "Select Job Step";
                jobNameLabel.Text = jobName;
                jobPanel.Visible = true;
            }
            contentStackPanel.ActiveControl = instructionContentPanel;

            MonitoredSqlServer listObject = null;
            foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
            {
                bindingSource.Add(server);
                if (String.Equals(server.InstanceName, serverName, StringComparison.CurrentCultureIgnoreCase))
                    listObject = server;
            }
            
            if (listObject != null)
            {
                ValueListItem item = serverComboBox.Items.ValueList.FindByDataValue(listObject.Id);
                serverComboBox.SelectedItem = item;
            }

            UpdateControls();
        }

        private int FindServerIndex(string name)
        {
            return serverComboBox.Items.ValueList.FindString(name);
        }

        private void UpdateControls()
        {
            ValueListItem selectedItem = serverComboBox.SelectedItem;
            loadButton.Enabled = (selectedItem != null);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.CurrentThread.Name = "JobBackgroundWorker";
            using (LOG.InfoCall("backgroundWorker_DoWork"))
            {
                BackgroundWorker worker = (BackgroundWorker)sender;
                MonitoredSqlServer instance = e.Argument as MonitoredSqlServer;
                configuration = new JobsAndStepsConfiguration(instance.Id);
                if (instance != null)
                {
                    string sql;
                    configuration.JobName = jobName.Replace("'", "''");
                    configuration.IsSelectedJobMode = mode == SelectJobMode.Job ? true : false;
                    
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    if (managementService == null)
                    {
                        SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                        managementService = ManagementServiceHelper.GetDefaultService(connectionInfo);
                    }

                    JobsAndStepsSnapshot result = managementService.GetJobsAndSteps(configuration);
                    e.Result = result;

                    if (worker.CancellationPending)
                        e.Cancel = true;
                }
            }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (LOG.InfoCall("backgroundWorker_RunWorkerCompleted"))
            {
                if (e.Cancelled)
                    return;

                noJobsLabel.Visible = false;
                serverComboBox.Enabled = true;
                loadButton.Text = "Load";

                if (e.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, e.Error);
                    contentStackPanel.ActiveControl = instructionContentPanel;
                    return;
                }
                
                JobsAndStepsSnapshot snapshot = e.Result as JobsAndStepsSnapshot;
                if (snapshot == null)
                {
                    serverComboBox.SelectedItem = null;
                    contentStackPanel.ActiveControl = instructionContentPanel;
                    return;
                }

                if (snapshot.Error != null)
                {
                    ApplicationMessageBox.ShowError(this, snapshot.Error);
                    contentStackPanel.ActiveControl = instructionContentPanel;
                    return;
                }

                // this just to check the item if exists
                string compareValue = jobName ?? String.Empty;
                if (mode == SelectJobMode.JobStep)
                {
                    compareValue = jobStep ?? String.Empty;
                }

                jobListBox.Items.Clear();
                
                if(snapshot.JobsAndSteps != null && snapshot.JobsAndSteps.Count > 0)
                {
                    foreach (string jobsAndStep in snapshot.JobsAndSteps)
                    {
                        bool check = String.Equals(jobsAndStep, compareValue);
                        jobListBox.Items.Add(jobsAndStep, check);
                    }
                    contentStackPanel.ActiveControl = jobContentPanel;
                }
                else
                {
                    noJobsLabel.Visible = true;
                    noJobsLabel.Text = String.Format("No job steps found on '{0}'.", snapshot.ServerName);
                    contentStackPanel.ActiveControl = instructionContentPanel;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {

        }

        private void SelectJobDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (loadButton.Text == "Cancel")
            {
                if (backgroundWorker.IsBusy)
                    backgroundWorker.CancelAsync();
                
                loadButton.Text = "Load";
                contentStackPanel.ActiveControl = instructionContentPanel;
            }
            else
            {
                ValueListItem selectedItem = serverComboBox.SelectedItem;
                if (selectedItem != null)
                {
                    backgroundWorker = new BackgroundWorker();
                    backgroundWorker.WorkerSupportsCancellation = true;
                    backgroundWorker.DoWork += this.backgroundWorker_DoWork;
                    backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

                    MonitoredSqlServer selectedInstance = selectedItem.ListObject as MonitoredSqlServer;
                    backgroundWorker.RunWorkerAsync(selectedInstance);

                    loadButton.Text = "Cancel";
                    contentStackPanel.ActiveControl = this.waitingContentPanel;
                    progressBar1.Enabled = true;
                    serverComboBox.Enabled = false;
                }
            }
        }

        private void jobListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (changingstuff)
                return;

            // list box is single check so uncheck all if this is a check event
            changingstuff = true;
            try
            {
                if (e.NewValue == CheckState.Checked)
                {
                    while (jobListBox.CheckedIndices.Count > 0)
                    {
                        jobListBox.SetItemChecked(jobListBox.CheckedIndices[0], false);
                    }
                }
            } finally
            {
                changingstuff = false;
            }
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}