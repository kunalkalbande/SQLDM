using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows.Forms;
using BBS.TracerX;
using Idera.SQLdm.Common.UI.Dialogs;
using Microsoft.SqlServer.Management.Smo;

namespace Idera.SQLdm.Common.UI.Dialogs
{
    public partial class SqlServerBrowserDialog : Form
    {
        private string selectedInstance = null;
        private bool networkSearchInitiated = false;

        public SqlServerBrowserDialog()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.RepositoryConnectionDialog;

            localInstancesProgressControl.Active = true;
            localInstancesWorker.RunWorkerAsync();
        }

        public string SelectedInstance
        {
            get { return selectedInstance; }
        }

        private void localInstancesWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            e.Result = SmoApplication.EnumAvailableSqlServers(true);
        }

        private void localInstancesWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            localInstancesProgressControl.Active = false;
            localInstancesProgressControl.Hide();

            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
            }
            else
            {
                DataTable localInstances = e.Result as DataTable;

                if (localInstances == null || localInstances.Rows.Count == 0)
                {
                    localInstancesStatusLabel.Show();
                }
                else
                {
                    localInstancesStatusLabel.Hide();
                    localInstancesTreeView.SuspendLayout();
                    localInstancesTreeView.Nodes.Clear();

                    foreach (DataRow instance in localInstances.Rows)
                    {
                        string instanceName = instance[0].ToString();
                        string instanceVersion = instance[4].ToString();
                        string nodeText = instanceVersion.Length > 0
                                              ? instanceName + "  (" + instanceVersion + ")"
                                              : instanceName;

                        TreeNode instanceNode = new TreeNode(nodeText, 0, 0);
                        instanceNode.Tag = instanceName;
                        localInstancesTreeView.Nodes.Add(instanceNode);
                    }

                    localInstancesTreeView.Sort();
                    localInstancesTreeView.ResumeLayout();
                }
            }
        }

        private void networkInstancesWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            e.Result = SmoApplication.EnumAvailableSqlServers();
        }

        private void networkInstancesWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
            }
            else
            {
                DataTable networkInstances = e.Result as DataTable;

                networkInstancesProgressControl.Active = false;
                networkInstancesProgressControl.Hide();

                if (networkInstances == null || networkInstances.Rows.Count == 0)
                {
                    networkInstancesStatusLabel.Show();
                }
                else
                {
                    networkInstancesStatusLabel.Hide();
                    networkInstancesTreeView.SuspendLayout();
                    networkInstancesTreeView.Nodes.Clear();

                    foreach (DataRow instance in networkInstances.Rows)
                    {
                        string instanceName = instance[0].ToString();
                        string instanceVersion = instance[4].ToString();
                        string nodeText = instanceVersion.Length > 0
                                              ? instanceName + "  (" + instanceVersion + ")"
                                              : instanceName;

                        TreeNode instanceNode = new TreeNode(nodeText, 0, 0);
                        instanceNode.Tag = instanceName;
                        networkInstancesTreeView.Nodes.Add(instanceNode);
                    }

                    networkInstancesTreeView.Sort();
                    networkInstancesTreeView.ResumeLayout();
                }
            }
        }

        private void searchScopeTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (searchScopeTabControl.SelectedIndex == 1 && !networkSearchInitiated)
            {
                networkSearchInitiated = true;
                networkInstancesProgressControl.Active = true;
                networkInstancesWorker.RunWorkerAsync();
            }
        }

        private void localInstancesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            networkInstancesTreeView.SelectedNode = null;
            selectedInstance = localInstancesTreeView.SelectedNode != null 
                ? localInstancesTreeView.SelectedNode.Tag as string 
                : null;
            okButton.Enabled = selectedInstance != null;
        }

        private void networkInstancesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            localInstancesTreeView.SelectedNode = null;
            selectedInstance = networkInstancesTreeView.SelectedNode != null 
                ? networkInstancesTreeView.SelectedNode.Tag as string 
                : null;
            okButton.Enabled = selectedInstance != null;
        }

        private void localInstancesTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedInstance != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void networkInstancesTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (selectedInstance != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            e.Cancel = true;
            Idera.SQLdm.Common.UI.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SqlServerBrowser);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            hevent.Handled = true;
            Idera.SQLdm.Common.UI.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.SqlServerBrowser);
        }

    }
}