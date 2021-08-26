using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    internal partial class AnalysisConfigureAdvanceSettingDialog : BaseDialog
    {
        private readonly List<MonitoredSqlServer> selectedInstances = new List<MonitoredSqlServer>();
        private MonitoredSqlServerCollection availableInstances;
        private List<string> instancesToExlude = new List<string>();
        private readonly string addedServersFormat;
        private string helpTopic;

        public string HeaderText
        {
            get { return lblHeader.Text;  }
            set { lblHeader.Text = value; }
        }

        public string HelpTopic
        {
            get { return helpTopic; }
            set { helpTopic = value; }
        }

        public List<string> InstancesToExclude
        {
            get { return instancesToExlude; }
            set { instancesToExlude.AddRange(value); }
        }

        public IList<MonitoredSqlServer> AddedInstances
        {
            get { return selectedInstances; }
        }

        public AnalysisConfigureAdvanceSettingDialog()
        {
            this.DialogHeader = "Select Monitored SQL Server Instances";
            InitializeComponent();

            addedServersFormat = addedInstancesLabel.Text;
            loadingServersProgressControl.Active = true;
            loadServersWorker.RunWorkerAsync();
            AdaptFontSize();
        }

        private void loadServersWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null)
                System.Threading.Thread.CurrentThread.Name = "LoadMonitoredServersWorker";

            if (!loadServersWorker.CancellationPending)
            {
                e.Result = ApplicationModel.Default.ActiveInstances;
            }

            e.Cancel = loadServersWorker.CancellationPending;
        }

        private void loadServersWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                loadingServersProgressControl.Active = false;
                loadingServersProgressControl.Hide();

                if (e.Error != null)
                {
                    //Log.Error("Unable to retrieve available SQL Server instances from the management service.", e.Error);
                    availableInstancesListBox.Items.Clear();
                    availableInstancesStatusLabel.Show();
                }
                else
                {
                    ListAvailableInstances(e.Result as MonitoredSqlServerCollection);
                }

                availableInstances = e.Result as MonitoredSqlServerCollection;
                UpdateControls();
            }
        }

        private void ListAvailableInstances(MonitoredSqlServerCollection collection)
        {
            availableInstancesListBox.Items.Clear();

            if (collection == null || collection.Count == 0)
            {
                availableInstancesStatusLabel.Show();
            }
            else
            {
                availableInstancesStatusLabel.Hide();
                availableInstancesListBox.Items.Clear();

                foreach (MonitoredSqlServer instance in collection)
                {
                    string instanceName = instance.InstanceName;

                    if (instancesToExlude.Contains(instanceName))
                        continue;

                    if (GetAddedInstanceIndex(instanceName) == -1)
                    {
                        string instanceVersion = instance.MostRecentSQLVersion != null ? instance.MostRecentSQLVersion.Version : "";
                        string itemText = instanceVersion.Length > 0
                                                ? instanceName + "  (" + instanceVersion + ")"
                                                : instanceName;

                        ListViewItem instanceItem = new ListViewItem(itemText);
                        instanceItem.Tag = instanceName;
                        availableInstancesListBox.Items.Add(instanceItem);
                    }
                }                
            }
        }

        private int GetAvailableInstanceIndex(string instanceName)
        {
            if (availableInstancesListBox.Items.Count > 0)
            {
                for (int index = 0; index < availableInstancesListBox.Items.Count; index++)
                {
                    ListViewItem existingInstance = availableInstancesListBox.Items[index] as ListViewItem;

                    if (existingInstance != null && string.Compare(existingInstance.Tag as string, instanceName, true) == 0)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        private int GetAddedInstanceIndex(string instanceName)
        {
            if (addedInstancesListBox.Items.Count > 0)
            {
                for (int index = 0; index < addedInstancesListBox.Items.Count; index++)
                {
                    ListViewItem existingInstance = addedInstancesListBox.Items[index] as ListViewItem;

                    if (existingInstance != null && string.Compare(existingInstance.Tag as string, instanceName, true) == 0)
                    {
                        return index;
                    }
                }
            }

            return -1;
        }

        private void UpdateControls()
        {
            addedInstancesLabel.Text = string.Format(addedServersFormat, addedInstancesListBox.Items.Count);

            removeInstancesButton.Enabled = addedInstancesListBox.SelectedItems.Count > 0;
            addInstancesButton.Enabled    = availableInstancesListBox.SelectedItems.Count > 0;

            btnOK.Enabled = addedInstancesListBox.Items.Count > 0;
        }

        private void addInstancesButton_Click(object sender, EventArgs e)
        {
            AddSelectedInstances();
        }

        private void removeInstancesButton_Click(object sender, EventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void AddSelectedInstances()
        {
            List<int> addedIndices = new List<int>();

            foreach (int selectedIndex in availableInstancesListBox.SelectedIndices)
            {
                ListViewItem selectedInstance = availableInstancesListBox.Items[selectedIndex] as ListViewItem;

                if (selectedInstance != null)
                {
                    addedIndices.Insert(0, selectedIndex);
                    addedInstancesListBox.Items.Add(selectedInstance);
                }
            }

            foreach (int addedIndex in addedIndices)
            {
                availableInstancesListBox.Items.RemoveAt(addedIndex);
            }

            UpdateControls();
        }

        private void RemoveSelectedInstances()
        {
            List<int> removedIndices = new List<int>();

            foreach (int selectedIndex in addedInstancesListBox.SelectedIndices)
            {
                ListViewItem selectedInstance = addedInstancesListBox.Items[selectedIndex] as ListViewItem;

                if (selectedInstance != null)
                {
                    removedIndices.Insert(0, selectedIndex);
                    availableInstancesListBox.Items.Add(selectedInstance);
                }
            }

            foreach (int removedIndex in removedIndices)
            {
                addedInstancesListBox.Items.RemoveAt(removedIndex);
            }

            UpdateControls();
        }

        private void availableInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddSelectedInstances();
        }

        private void addedInstancesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RemoveSelectedInstances();
        }

        private void availableInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void addedInstancesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void SelectMonitoredServersDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) 
                hlpevent.Handled = true;

            ShowHelp();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (addedInstancesListBox.Items.Count == 0 || availableInstances == null || availableInstances.Count == 0)
                return;

            for (int index = 0; index < addedInstancesListBox.Items.Count; index++)
            {
                ListViewItem existingInstance = addedInstancesListBox.Items[index] as ListViewItem;

                string instanceName = existingInstance.Tag as string;

                MonitoredSqlServer config = GetInstanceConfiguration(instanceName);

                if (config != null)
                    selectedInstances.Add(config);
            }
        }

        private MonitoredSqlServer GetInstanceConfiguration(string instanceName)
        {
            foreach (MonitoredSqlServer server in availableInstances)
            {
                if (server.InstanceName == instanceName)
                    return server;
            }

            return null;
        }

        private void SelectMonitoredServersDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) 
                e.Cancel = true;

            ShowHelp();
        }

        private void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(helpTopic);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}
