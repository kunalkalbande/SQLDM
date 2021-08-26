using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class BrowseJobStepsDialog : BaseDialog
    {
        private const string WILDCARD = "%";

        private readonly int instanceId;
        private MonitoredSqlServer activeInstance;

        private string category;
        private string jobName;
        private string stepName;

        public BrowseJobStepsDialog(int instanceId)
        {
            InitializeComponent();

            this.instanceId = instanceId;

            activeInstance = RepositoryHelper.GetMonitoredSqlServer(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId);

            Text = String.Format(Text, activeInstance.InstanceName);

            okButton.Enabled = false;

            refreshFilterOptions();
            AdaptFontSize();
        }

        public string Category
        {
            get { return category; }
            set { this.category = value; }
        }

        public string JobName
        {
            get { return jobName; }
            set { this.jobName = value; }
        }

        public string StepName
        {
            get { return stepName; }
            set { this.stepName = value; }
        }

        public void refreshFilterOptions()
        {
            refreshProgressControl.Visible = true;
            refreshProgressControl.Active = true;
            refreshBackgroundWorker.RunWorkerAsync();
        }

        private void refreshBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            try
            {
                IManagementService managementService = ManagementServiceHelper.GetDefaultService();
                e.Result = managementService.GetAgentJobStepList(instanceId);
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

        private void refreshBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                refreshProgressControl.Active = false;
                refreshProgressControl.Visible = false;

                if (e.Error == null)
                {
                    if (e.Result is IEnumerable<CategoryJobStep>)
                    {
                        filterOptionsTreeView.BeginUpdate();
                        filterOptionsTreeView.Nodes.Clear();

                        filterOptionsTreeView.Nodes.Add(activeInstance.InstanceName);

                        string prvCategory = "";
                        string prvJobName = "";
                        string prvStepName = "";

                        int categoryNodes = -1;
                        int jobNodes = -1; 
                        int stepNodes = -1;

                        foreach (CategoryJobStep jobStep in (IEnumerable<CategoryJobStep>)e.Result)
                        {
                            if (!prvCategory.Equals(jobStep.Category))
                            {
                                categoryNodes++;
                                jobNodes = -1;
                                prvJobName = "";
                                prvCategory = jobStep.Category;
                                filterOptionsTreeView.Nodes[0].Nodes.Add(jobStep.Category);
                            }
                            if (!prvJobName.Equals(jobStep.JobName))
                            {
                                jobNodes++;
                                stepNodes = -1;
                                prvStepName = "";
                                prvJobName = jobStep.JobName;
                                filterOptionsTreeView.Nodes[0].Nodes[categoryNodes].Nodes.Add(jobStep.JobName);
                            }
                            if (!prvStepName.Equals(jobStep.StepName))
                            {
                                stepNodes++;
                                prvStepName = jobStep.StepName;
                                filterOptionsTreeView.Nodes[0].Nodes[categoryNodes].Nodes[jobNodes].Nodes.Add(jobStep.StepName);
                            }
                        }

                        filterOptionsTreeView.Nodes[0].ExpandAll();
                        filterOptionsTreeView.Nodes[0].EnsureVisible();
                        filterOptionsTreeView.EndUpdate();
                    }
                    else
                    {
                        refreshLinkLabel.Visible = true;
                        ApplicationMessageBox.ShowError(this, "An list of job steps was returned.");
                    }
                }
                else
                {
                    refreshLinkLabel.Visible = true;
                    ApplicationMessageBox.ShowError(this, "An error occurred while loading the filter options.", e.Error);
                }
            }
        }

        private void refreshLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            refreshFilterOptions();
        }

        private void BrowseJobStepsDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            refreshBackgroundWorker.CancelAsync();
        }

        private void filterOptionsTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (filterOptionsTreeView.SelectedNode != null)
            {
                switch (filterOptionsTreeView.SelectedNode.Level)
                {
                    case 1:
                        category = filterOptionsTreeView.SelectedNode.Text;
                        jobName = stepName = WILDCARD;
                        break;
                    case 2:
                        category = filterOptionsTreeView.SelectedNode.Parent.Text;
                        jobName = filterOptionsTreeView.SelectedNode.Text;
                        stepName = WILDCARD;
                        break;
                    case 3:
                        category = filterOptionsTreeView.SelectedNode.Parent.Parent.Text;
                        jobName = filterOptionsTreeView.SelectedNode.Parent.Text;
                        stepName = filterOptionsTreeView.SelectedNode.Text;
                        break;
                    default:
                        category = jobName = stepName = WILDCARD;
                        break;
                }
                okButton.Enabled = true;
            }

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