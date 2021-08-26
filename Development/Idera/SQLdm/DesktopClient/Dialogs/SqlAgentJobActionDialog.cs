using System;
using System.ComponentModel;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SqlAgentJobActionDialog : BaseDialog
    {
        private readonly int instanceId;
        private readonly string jobName;
        private readonly JobControlAction jobAction;
        private Exception error;
        private Snapshot result;

        public SqlAgentJobActionDialog(int instanceId, string jobName, JobControlAction jobAction)
        {
            this.DialogHeader = "< Action Title >";
            InitializeComponent();

            this.instanceId = instanceId;
            this.jobName = jobName;
            this.jobAction = jobAction;

            switch (this.jobAction)
            {
                case JobControlAction.Start:
                    Text = "Starting SQL Agent Job";
                    descriptionLabel.Text = string.Format("Starting '{0}'...", this.jobName);
                    break;
                case JobControlAction.Stop:
                    Text = "Stopping SQL Agent Job";
                    descriptionLabel.Text = string.Format("Stopping '{0}'...", this.jobName);
                    break;
            }
            AdaptFontSize();
        }

        public Exception Error
        {
            get { return error; }
        }

        public Snapshot Result
        {
            get { return result; }
        }

        protected override void OnShown(EventArgs e)
        {
            backgroundWorker.RunWorkerAsync();
            base.OnShown(e);
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);
                e.Result = managementService.SendJobControl(
                    new JobControlConfiguration(instanceId, jobName, jobAction));
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

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    result = e.Result as Serialized<Snapshot>;
                }
                else
                {
                    error = e.Error;
                }
            }

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            backgroundWorker.CancelAsync();
            base.OnClosing(e);
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {

            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}