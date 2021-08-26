using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public enum AlertFilterType
    {
        Database,
        Job,
        JobCategory,
        Filegroup      // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --added new filter type for filegroup alerts
    }

    public partial class AlertFilterDialog : Form
    {
        private readonly int instanceId;
        private readonly AlertFilterType filterType;
        private readonly string databaseName;
        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --added default threshold name field
        public static string DEFAULT_THRESHOLD_NAME = "< Default Threshold >";

        public AlertFilterDialog(int instanceId, AlertFilterType filterType, string excludeFilter)
        {
            InitializeComponent();
            ResizeFilterOptionsListViewColumns();
            
            this.instanceId = instanceId;
            this.filterType = filterType;

            InitializeDialogTitle();
            excludeFilterTextBox.Text = excludeFilter;
            RefreshFilterOptions();
            AdaptFontSize();
        }

        public AlertFilterDialog(int instanceId, AlertFilterType filterType, string excludeFilter, string thresholdInstanceName) : this(instanceId, filterType, excludeFilter)
        {
            this.databaseName = thresholdInstanceName;
        }

        public string ExcludeFilterText
        {
            get { return excludeFilterTextBox.Text; }
        }

        private void InitializeDialogTitle()
        {
            switch (filterType)
            {
                case AlertFilterType.Database:
                    Text = string.Format(Text, "Databases");
                    break;
                case AlertFilterType.Job:
                    Text = string.Format(Text, "Jobs");
                    break;
                case AlertFilterType.JobCategory:
                    Text = string.Format(Text, "Job Categories");
                    break;
                case AlertFilterType.Filegroup:        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --added new filter type for filegroup alerts
                    Text = string.Format(Text, "Filegroups");
                    break;
            }
        }

        private void RefreshFilterOptions()
        {
            refreshLinkLabel.Visible = false;
            refreshProgressControl.Visible = true;
            refreshProgressControl.Active = true;
            refreshBackgroundWorker.RunWorkerAsync();
        }

        private void refreshBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                switch (filterType)
                {
                    case AlertFilterType.Database:
                        e.Result = LoadDatabases();
                        break;
                    case AlertFilterType.Job:
                        e.Result = LoadJobs();
                        break;
                    case AlertFilterType.JobCategory:
                        e.Result = LoadJobCategories();
                        break;
                    case AlertFilterType.Filegroup:  // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --load all the filegroups
                        e.Result = LoadFilegroups();  
                        break;
                }
            }
            catch
            {
                if (refreshBackgroundWorker.CancellationPending)
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
                if (refreshBackgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                }
            }
        }

        private IEnumerable<string> LoadDatabases()
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            IDictionary<string, bool> databases = managementService.GetDatabases(instanceId, true, true);
            return databases.Keys;
        }

        private IEnumerable<string> LoadJobs()
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            return managementService.GetAgentJobNames(instanceId);
        }

        private IEnumerable<string> LoadJobCategories()
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            return managementService.GetAgentJobCategories(instanceId);
        }

        // SQLdm 9.1 (Abhishek Joshi) -Filegroup and Mountpoint Monitoring Improvements --load all the filegroups using management service helper function
        private IEnumerable<string> LoadFilegroups()
        {
            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            bool isDefaultThreshold = databaseName == DEFAULT_THRESHOLD_NAME;
            return managementService.GetFilegroups(instanceId, databaseName, isDefaultThreshold);
        }

        private void refreshBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                refreshProgressControl.Active = false;
                refreshProgressControl.Visible = false;

                if (e.Error == null)
                {
                    if (e.Result is IEnumerable<string>)
                    {
                        filterOptionsListView.BeginUpdate();
                        filterOptionsListView.Items.Clear();

                        foreach (string filterOption in (IEnumerable<string>)e.Result)
                        {
                            filterOptionsListView.Items.Add(filterOption);
                        }

                        filterOptionsListView.EndUpdate();
                    }
                    else
                    {
                        refreshLinkLabel.Visible = true;
                        ApplicationMessageBox.ShowError(this, "An invalid filter options set was returned.");
                    }
                }
                else
                {
                    refreshLinkLabel.Visible = true;
                    ApplicationMessageBox.ShowError(this, "An error occurred while loading the filter options.", e.Error);
                }
            }
        }

        private void AlertFilterDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            refreshBackgroundWorker.CancelAsync();
        }

        private void filterOptionsListView_Resize(object sender, EventArgs e)
        {
            ResizeFilterOptionsListViewColumns();
        }

        private void ResizeFilterOptionsListViewColumns()
        {
            filterOptionsListView.Columns[0].Width = filterOptionsListView.Width - 5;
        }

        private void filterOptionsListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AppendSelectedFilterOptions();
        }

        private void AppendSelectedFilterOptions()
        {
            if (filterOptionsListView.SelectedItems.Count > 0)
            {
                StringBuilder excludeFilter = new StringBuilder(excludeFilterTextBox.Text);

                foreach (ListViewItem item in filterOptionsListView.SelectedItems)
                {
                    if (excludeFilter.Length != 0)
                    {
                        excludeFilter.Append("; ");
                    }

                    excludeFilter.Append("[");
                    excludeFilter.Append(item.Text);
                    excludeFilter.Append("]");
                }

                excludeFilterTextBox.Text = excludeFilter.ToString();
            }
        }

        private void appendExcludeFiltersButton_Click(object sender, EventArgs e)
        {
            AppendSelectedFilterOptions();
        }

        private static void UpdateTextBoxConrols(Control parentControl, TextBox textBox)
        {
            if (parentControl != null && textBox != null)
            {
                int lastLine = textBox.TextLength == 0
                                   ? 0
                                   : textBox.GetLineFromCharIndex(textBox.TextLength - 1);

                parentControl.Height = 30 + lastLine*TextRenderer.MeasureText(" ", textBox.Font).Height;
            }
        }

        private void excludeFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(excludePanel, excludeFilterTextBox);
        }

        private void excludeFilterTextBox_Resize(object sender, EventArgs e)
        {
            UpdateTextBoxConrols(excludePanel, excludeFilterTextBox);
        }

        private void refreshLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RefreshFilterOptions();
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