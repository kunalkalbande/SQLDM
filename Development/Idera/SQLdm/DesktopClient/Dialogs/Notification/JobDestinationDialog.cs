using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Helpers;
    using Common.Configuration;
    using Common.Notification.Providers;
    using Common.Objects;
    using Common.Services;
    using Common.UI.Dialogs;
    using Infragistics.Win;

    public partial class JobDestinationDialog : Form
    {
        private JobDestination destination;
        private MonitoredSqlServer dollarInstance;
        private string lastBrowseServer = String.Empty;

        public JobDestinationDialog()
        {
            InitializeComponent();
            MonitoredSqlServerConfiguration dollarInstanceConfig = new MonitoredSqlServerConfiguration("$(Instance)");
            dollarInstance = new MonitoredSqlServer(0, DateTime.Now, dollarInstanceConfig);

            // Adapt fontsize.
            AdaptFontSize();
        }

        public JobDestination Destination
        {
            get { return destination;  }
            set { destination = value; }
        }

        private void browseJobButton_Click(object sender, EventArgs e)
        {
            string serverName = String.Empty;

            ValueListItem selectedItem = serverComboBox.SelectedItem;
            if (selectedItem != null)
            {
                MonitoredSqlServer server = serverComboBox.SelectedItem.ListObject as MonitoredSqlServer;
                if (server!= null && server != dollarInstance)
                    serverName = server.InstanceName;
            }

            if (!String.IsNullOrEmpty(lastBrowseServer))
                serverName = lastBrowseServer;

            string jobName = jobNameTextBox.Text.Trim();
            if (SelectJobDialog.SelectJobName(this, ref serverName, ref jobName) == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(serverName))
                    lastBrowseServer = serverName;             
                if (selectedItem == null)
                {
                    SelectServer(serverName);
                }
                jobNameTextBox.Text = jobName;
            }
        }

        private void SelectServer(string serverName)
        {
            foreach (ValueListItem item in serverComboBox.Items)
            {
                MonitoredSqlServer server = item.ListObject as MonitoredSqlServer;
                if (server != null && String.Equals(serverName, server.InstanceName))
                {
                    serverComboBox.SelectedItem = item;
                    break;
                }
            }
        }

        private void browseJobStepButton_Click(object sender, EventArgs e)
        {
            string serverName = String.Empty;

            ValueListItem selectedItem = serverComboBox.SelectedItem;
            if (selectedItem != null)
            {
                MonitoredSqlServer server = serverComboBox.SelectedItem.ListObject as MonitoredSqlServer;
                if (server != null && server != dollarInstance)
                    serverName = server.InstanceName;
            }

            if (!String.IsNullOrEmpty(lastBrowseServer))
                serverName = lastBrowseServer;

            string jobName = jobNameTextBox.Text.Trim();
            string jobStep = jobStepTextBox.Text.Trim();
            if (SelectJobDialog.SelectJobStep(this, ref serverName, jobName, ref jobStep) == DialogResult.OK)
            {
                if (!String.IsNullOrEmpty(serverName))
                    lastBrowseServer = serverName;
                if (selectedItem == null)
                {
                    SelectServer(serverName);
                }
                jobStepTextBox.Text = jobStep;
            }
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            using (TestSqlActionDialog testDialog = new TestSqlActionDialog())
            {
                JobDestination dest = new JobDestination();
                UpdateDestination(dest);
                testDialog.Destination = dest;
                testDialog.ShowDialog(this);
            }
        }

        private void UpdateDestination(JobDestination dest)
        {
            MonitoredSqlServer server = serverComboBox.SelectedItem.ListObject as MonitoredSqlServer;
            string serverName = String.Empty;
            if (server != null)
                serverName = server.InstanceName;

            dest.Server = serverName;
            dest.Job = jobNameTextBox.Text.Trim();
            dest.Step = jobStepTextBox.Text.Trim();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!ValidateDestination())
            {
                DialogResult = DialogResult.None;
                return;
            }

            UpdateDestination(destination);
        }

        private bool ValidateDestination()
        {
            IManagementService ms = ManagementServiceHelper.GetDefaultService();
            try
            {
                JobDestination pd = new JobDestination();

                MonitoredSqlServer server = serverComboBox.SelectedItem.ListObject as MonitoredSqlServer;
                string serverName = String.Empty;
                if (server != null)
                    serverName = server.InstanceName;

                pd.Server = serverName;
                pd.Job = jobNameTextBox.Text.Trim();
                pd.Step = jobStepTextBox.Text.Trim();
                ms.ValidateDestination(pd);
            }
            catch (Exception e)
            {
                ApplicationMessageBox.ShowError(this,
                    "The job action settings do not appear to be valid.  Please correct the values and try again.",
                    e);
                return false;
            }
            return true;

        }

        private void jobNameTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void UpdateControls()
        {
            ValueListItem item = serverComboBox.SelectedItem;
            browseJobButton.Enabled = item != null;

            string jobname = jobNameTextBox.Text.Trim();
            string stepname = jobStepTextBox.Text.Trim();
        
            jobNameTextBox.Enabled = browseJobButton.Enabled || jobname.Length > 0;

            browseJobStepButton.Enabled = browseJobButton.Enabled && jobname.Length > 0;

            jobStepTextBox.Enabled = browseJobStepButton.Enabled || stepname.Length > 0;

            okButton.Enabled = browseJobStepButton.Enabled;
            testButton.Enabled = browseJobStepButton.Enabled;
        }

        private void JobDestinationDialog_Load(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(destination.Server))
                LoadServers();
            else
            {
                serverComboBox.DataSource = null;
                serverComboBox.Value = null;
            }

            jobNameTextBox.Text = destination.Job;
            jobStepTextBox.Text = destination.Step;

            UpdateControls();
        }

        private void LoadServers()
        {
            if (bindingSource.Count > 0)
                return;

            serverComboBox.DataSource = bindingSource;

            MonitoredSqlServer selected = null;

            string destinationServer = destination.Server ?? String.Empty;
            bindingSource.Add(dollarInstance);
            if (String.Equals(destinationServer, dollarInstance.InstanceName, StringComparison.CurrentCultureIgnoreCase))
                selected = dollarInstance;

            foreach (MonitoredSqlServer server in ApplicationModel.Default.ActiveInstances)
            {
                bindingSource.Add(server);
                if (String.Equals(server.InstanceName, destinationServer, StringComparison.CurrentCultureIgnoreCase))
                    selected = server;
            }

            if (selected != null)
            {
                ValueListItem item = serverComboBox.Items.ValueList.FindByDataValue(selected.InstanceName);
                serverComboBox.SelectedItem = item;
            }
            else
                serverComboBox.Value = null;

        }

        private void serverComboBox_BeforeDropDown(object sender, CancelEventArgs e)
        {
            LoadServers();
        }

        private void serverComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            UpdateControls();
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