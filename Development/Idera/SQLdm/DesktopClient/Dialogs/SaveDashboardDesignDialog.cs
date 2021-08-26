using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    internal partial class SaveDashboardDesignDialog : BaseDialog
    {
        private readonly string userName;
        private readonly int instanceID;
        private readonly int dashboardLayoutID;
        private bool shiftDown = false;
        private bool controlDown = false;

        public SaveDashboardDesignDialog(int instanceID, string userName, int dashboardLayoutID, string dashboardName)
        {
            this.DialogHeader = "Save Dashboard Design";
            InitializeComponent();

            ReplaceDashboardLayoutId =
                this.dashboardLayoutID = dashboardLayoutID;
            this.userName = userName;
            DashboardName =
                dashboardNameTextBox.Text = dashboardName;

            AvailableInstances = ApplicationModel.Default.ActiveInstances;
            SelectedInstances = new MonitoredSqlServerCollection();
            if (AvailableInstances.Contains(instanceID))
            {
                SelectedInstances.Add(AvailableInstances[instanceID]);
            }

            SaveToFile = false;

            Initialize();
            AdaptFontSize();
        }

        public string DashboardName { get; private set; }
        public int ReplaceDashboardLayoutId { get; private set; }
        public bool SaveToFile { get; private set; }

        public MonitoredSqlServerCollection AvailableInstances { get; private set; }
        public MonitoredSqlServerCollection SelectedInstances { get; private set; }

        private void Initialize()
        {
            availableServersListBox.Items.Clear();
            if (AvailableInstances.Count > 0)
            {
                foreach (var instance in AvailableInstances)
                {
                    availableServersListBox.Items.Add(instance.InstanceName);
                }
                foreach (var instance in SelectedInstances)
                {
                    if (AvailableInstances.Contains(instance.Id))
                    {
                        availableServersListBox.SelectedItems.Add(instance.InstanceName);
                    }
                }
                AddServers();
            }
            else
            {
                availableServersLabel.BringToFront();
            }
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            addServersButton.Enabled = (availableServersListBox.SelectedItems.Count > 0);
            removeServersButton.Enabled = (selectedServersListBox.SelectedItems.Count > 0);
            okButton.Enabled = DashboardName.Length > 0;
        }

        private void AddServers()
        {
            string[] selectedItems = new string[availableServersListBox.SelectedItems.Count];
            availableServersListBox.SelectedItems.CopyTo(selectedItems, 0);
            availableServersListBox.SelectedItems.Clear();

            foreach (var item in selectedItems)
            {
                foreach(var instance in AvailableInstances)
                {
                    if (instance.InstanceName == item)
                    {
                        availableServersListBox.Items.Remove(item);
                        SelectedInstances.Add(instance);
                        selectedServersListBox.Items.Add(item);
                        break;
                    }
                }
            }

            UpdateButtons();
        }

        private void RemoveServers()
        {
            if (selectedServersListBox.SelectedItems.Count > 0)
            {
                string[] selectedItems = new string[selectedServersListBox.SelectedItems.Count];
                selectedServersListBox.SelectedItems.CopyTo(selectedItems, 0);
                selectedServersListBox.SelectedItems.Clear();

                var removeInstances = new List<MonitoredSqlServerWrapper>();
                foreach (var item in selectedItems)
                {
                    foreach (var instance in SelectedInstances)
                    {
                        if (instance.InstanceName == item)
                        {
                            removeInstances.Add(instance);
                            selectedServersListBox.Items.Remove(item);
                            availableServersListBox.Items.Add(item);
                            break;
                        }
                    }
                }

                SelectedInstances.RemoveRange(removeInstances);
            }

            UpdateButtons();
        }

        public bool UseAsGlobalDefault
        {
            get { return useAsGlobalDefaultCheckBox.Checked; }
        }

        private bool ValidateFields()
        {
            DashboardName = dashboardNameTextBox.Text;
            if (DashboardName.Length == 0)
            {
                ApplicationMessageBox.ShowError(this, "A name must be entered for the dashboard design.");
                return false;
            }
            else
            {
                ReplaceDashboardLayoutId = RepositoryHelper.GetDashboardLayoutID(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                        userName,
                                                        DashboardName);
                if (ReplaceDashboardLayoutId != 0)
                {
                    if (DialogResult.No == ApplicationMessageBox.ShowQuestion(this, string.Format("A dashboard already exists with the name '{0}'. Do you want to replace it?", DashboardName)))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ServerDashboardSave);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveToFile = shiftDown && controlDown;
            if (ValidateFields())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                DialogResult = DialogResult.None;
            }
        }

        private void addServersButton_Click(object sender, EventArgs e)
        {
            AddServers();
        }

        private void removeServersButton_Click(object sender, EventArgs e)
        {
            RemoveServers();
        }

        private void availableServersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void selectedServersListBox_SelectedValueChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void SaveDashboardDesignDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftDown = true;
                //Debug.Print("shift Down");
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                {
                    controlDown = true;
                    //Debug.Print("control Down");
                }
            }
        }

        private void SaveDashboardDesignDialog_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey)
            {
                shiftDown = false;
                //Debug.Print("shift Up");
            }
            else if (e.KeyCode == Keys.ControlKey)
            {
                {
                    controlDown = false;
                    //Debug.Print("control Up");
                }
            }
        }

        private void SaveDashboardDesignDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            ShowHelp();
        }

        private void SaveDashboardDesignDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
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
