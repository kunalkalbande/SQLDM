using System;
using System.Data;
using System.Diagnostics;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Idera.SQLdm.Common.UI.Dialogs;
using System.Collections.Generic;
using SqlConnectionInfo = Idera.SQLdm.Common.Configuration.SqlConnectionInfo;
using Idera.SQLdm.Common.Services;
using System.ComponentModel;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.Common.UI.Dialogs
{
	//SQLdm 9.1 (Ankit Srivastava) Disk Size Report - Added new dialog class for selecting disks
    public partial class DiskBrowserDialog : Form
    {
        private SqlConnectionInfo connectionInfo;
        private IManagementService managementServiceInterface;
        private int serverInstanceId = -1;
        private string serverInstanceName;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Common.UI.Dialogs.DiskBrowserDialog");

        private string selectedDisk;
        private List<string> checkedDisks;

        private bool emptySelectionAllowed = false;
        private string helpTopic = HelpTopics.DiskBrowser;

        #region Ctors

        public DiskBrowserDialog()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.RepositoryConnectionDialog;
            browseProgressControl.Active = true;
        }

        public DiskBrowserDialog(SqlConnectionInfo connectionInfo)
            : this()
        {
            this.connectionInfo = connectionInfo;
            this.serverInstanceName = connectionInfo.InstanceName;
        }

        public DiskBrowserDialog(SqlConnectionInfo connectionInfo, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText, string title)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.Text = title;
            this.connectionInfo = connectionInfo;
        }

        public DiskBrowserDialog(IManagementService managementService, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText, string title)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.Text = title;
            this.managementServiceInterface = managementService;
        }

        public DiskBrowserDialog(IManagementService managementService, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.managementServiceInterface = managementService;
        }

        public DiskBrowserDialog(int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText)
            : this()
        {
            this.serverInstanceId = serverInstanceId;
            this.serverInstanceName = serverInstanceName;
            diskTreeView.CheckBoxes = multiselect;
            descriptionLabel.Text = descriptionLabelText;
            emptySelectionAllowed = !selectionRequired;
        }

        #endregion

        public string SelectedDisk
        {
            get { return selectedDisk; }
            set { selectedDisk = value; }
        }

        public List<string> CheckedDisks
        {
            get { return checkedDisks; }
            set { checkedDisks = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            backgroundWorker.RunWorkerAsync();
        }

        public string HelpTopic
        {
            get { return helpTopic; }
            set { helpTopic = value; }
        }

        #region backgroundWorker

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (connectionInfo == null)
                e.Result = GetDisks(managementServiceInterface, serverInstanceId);//Calling the Mgmt Service
            else if (serverInstanceId == -1)
                e.Result = GetDisks(connectionInfo); //firing queries on instances
            else
                e.Result = GetDisks(connectionInfo, serverInstanceId); // using procedure directly
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            browseProgressControl.Active = false;
            browseProgressControl.Hide();
            diskTreeView.Nodes.Clear();

            if (DialogResult == DialogResult.Cancel)
            {
                return;
            }
            else if (e.Error != null)
            {
                ApplicationMessageBox.ShowError(this, e.Error);
                Close();
            }
            else
            {
                IList<string> disks = e.Result as IList<string>;

                if (disks == null || disks.Count == 0)
                {
                    ApplicationMessageBox.ShowWarning(this, "No disks are available.");
                    Close();
                }
                else
                {
                    diskTreeView.SuspendLayout();

                    TreeNode serverNode = new TreeNode(serverInstanceName, 2, 2);//Server Image applied to the root node
					diskTreeView.Nodes.Add(serverNode);
					//SQLdm 9.1 (Ankit Srivastava) Removed Folder Node from the Disk Browser
                    
                    foreach (string diskName in disks)
                    {
                        TreeNode diskNode = new TreeNode(diskName, 1, 1);
                        diskNode.Tag = diskName;
                        serverNode.Nodes.Add(diskNode);//SQLdm 9.1 (Ankit Srivastava) Removed Folder Node from the Disk Browser -- adding the individual disk to the server node instead of disks folder
                        MaybeSelectNode(diskNode);
                    }

                    diskTreeView.ExpandAll();
                    diskTreeView.Sort();
                    diskTreeView.ResumeLayout();
                }
            }

            if (emptySelectionAllowed)
                okButton.Enabled = true;
        }

        #endregion

        #region GetDisks

        /// <summary>
        /// Gets the disks from the repository rather than SMO or the management service.
        /// The dictionary contains the disk name and if it's a system disk.
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static IList<string> GetDisks(SqlConnectionInfo connectionInfo, int instanceId)
        {
            IList<string> list = new List<string>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GetAllDisks"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, instanceId);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string diskName = dataReader["DriveName"] == DBNull.Value ? string.Empty : (string)dataReader["DriveName"];
                            list.Add(diskName);
                        }

                        dataReader.Close();
                    }
                }

                return list;
            }
        }
        
        /// <summary>
        /// Returns all Disks from sysDisks / master.sys.Disks
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static IList<string> GetDisks(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            IList<string> result = new List<string>();

            using (SqlConnection connection = connectionInfo.GetConnection())
            {
                connection.Open();
                var serverVersion = new ServerVersion(connection.ServerVersion);

                // Fixed Drive if SQL Server version >= 2008
                var fixedDrives = serverVersion.IsGreaterThanSql2008Sp1R2()
                                      ? string.Format(Constants.FixedDrives2008, Constants.ExecMasterDboXpFixedDrives)
                                      : Constants.ExecMasterDboXpFixedDrives;
                
                // Remote Drive if SQL Server version >= 2008
                var remoteDrives = serverVersion.IsGreaterThanSql2008Sp1R2()
                                       ? string.Format(
                                           Constants.FixedDrives2008,
                                           Constants.ExecMasterDboXpFixedDrivesRemote)
                                       : Constants.ExecMasterDboXpFixedDrivesRemote;

                using (SqlCommand command = new SqlCommand(fixedDrives, connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string drive = reader.GetString(0);
                            if (!result.Contains(drive))
                            {
                                result.Add(drive);
                            }
                        }
                    }
                }

                using (SqlCommand command = new SqlCommand(remoteDrives, connection))
                {
                    command.CommandType = CommandType.Text;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string drive = reader.GetString(0);
                            if (!result.Contains(drive))
                            {
                                result.Add(drive);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Connects to the management service to get a list of disks from the collection service.  Use this
        /// method when trying to get the disk list for a monitored server. 
        /// </summary>
        /// <param name="managementService"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static IList<string> GetDisks(IManagementService managementService, int instanceId)
        {
            if (managementService == null)
                throw new ArgumentException("managementService");

            return managementService.GetDisks(instanceId);
        }

        #endregion

        // Select (or check) the node if its disk name is in selectedDisk (or checkedDisks).
        private void MaybeSelectNode(TreeNode node)
        {
            string lowerName = node.Text.ToLower();
            
            if (diskTreeView.CheckBoxes)
            {
                if (checkedDisks != null)
                {
                    foreach (string nameToCheck in checkedDisks)
                    {
                        if (nameToCheck.ToLower() == lowerName)
                        {
                            node.Checked = true;
                            MaybeCheckParent(node);
                            return;
                        }
                    }

                    // Getting here means the node will remain unchecked and therefore
                    // the parent should be unchecked.
                    UncheckParent(node);
                }
            }
            else
            {
                if (selectedDisk != null && selectedDisk.ToLower() == lowerName)
                {
                    node.TreeView.SelectedNode = node;
                }
            }
        }        

        private void diskTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!diskTreeView.CheckBoxes)
            {
                if (diskTreeView.SelectedNode != null)
                {
                    selectedDisk = diskTreeView.SelectedNode.Tag as string;
                }
                else
                {
                    selectedDisk = null;
                }

                if (!emptySelectionAllowed)
                {
                    okButton.Enabled = selectedDisk != null;
                }
            }
        }

        private void diskTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Double click does nothing when checkboxes are shown.
            if (!diskTreeView.CheckBoxes && selectedDisk != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        // Check or uncheck all the children and subchildren of the node.
        private void CheckChildren(TreeNode node, bool checkState)
        {
            foreach (TreeNode subnode in node.Nodes)
            {
                subnode.Checked = checkState;
                CheckChildren(subnode, checkState);
            }
        }

        // Uncheck all of the node's ancestors.
        private void UncheckParent(TreeNode node)
        {
            if (node.Parent != null)
            {
                node.Parent.Checked = false;
                UncheckParent(node.Parent);
            }
        }

        // Check the node's parent if all siblings are already checked.
        private void MaybeCheckParent(TreeNode node)
        {
            if (node.Parent != null)
            {
                foreach (TreeNode sibling in node.Parent.Nodes)
                {
                    if (!sibling.Checked) return;
                }

                node.Parent.Checked = true;
                MaybeCheckParent(node.Parent);
            }
        }

        private IEnumerable<string> EnumCheckedDisks(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                // Only nodes with non-null tags represent disks.
                if (node.Tag != null && node.Checked)
                {
                    yield return node.Text;
                }
                else
                {
                    // Look in the node's children.
                    foreach (string dbName in EnumCheckedDisks(node.Nodes))
                    {
                        yield return dbName;
                    }
                }
            }
        }

        // When a node is checked, check all its children.
        // When a node is unchecked, uncheck its parent so it doesn't
        // look like the entire branch is checked.
        private void diskTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                // Likely a recursive call.
                return;
            }

            //Debug.Print("diskTreeView_AfterCheck");
            diskTreeView.BeginUpdate();
            if (e.Node.Checked)
            {
                CheckChildren(e.Node, true);
                MaybeCheckParent(e.Node);
            }
            else
            {
                CheckChildren(e.Node, false);
                UncheckParent(e.Node);
            }
            diskTreeView.EndUpdate();

            // Enable the OK button if any node representing a disk is checked.
            if (!emptySelectionAllowed)
            {
                okButton.Enabled = false;
                
                foreach (string dbName in EnumCheckedDisks(diskTreeView.Nodes))
                {
                    okButton.Enabled = true;
                    break;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            checkedDisks = new List<string>();

            if (diskTreeView.CheckBoxes)
            {
                foreach (string dbName in EnumCheckedDisks(diskTreeView.Nodes))
                {
                    checkedDisks.Add(dbName);
                }
            }
        }

        #region Help

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            e.Cancel = true;
            Idera.SQLdm.Common.UI.Helpers.ApplicationHelper.ShowHelpTopic(helpTopic);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            hevent.Handled = true;
            Idera.SQLdm.Common.UI.Helpers.ApplicationHelper.ShowHelpTopic(helpTopic);
        }

        #endregion

    }
}