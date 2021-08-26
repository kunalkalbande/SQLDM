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
    public partial class DatabaseBrowserDialog : Form
    {
        private SqlConnectionInfo connectionInfo;
        private IManagementService managementServiceInterface;
        private int serverInstanceId = -1;
        private string serverInstanceName;
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("Common.UI.Dialogs.DatabaseBrowserDialog");

        private string selectedDatabase;
        private List<string> checkedDatabases;

        private bool emptySelectionAllowed = false;
        private string helpTopic = HelpTopics.DatabaseBrowser;

        #region Ctors

        public DatabaseBrowserDialog()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.RepositoryConnectionDialog;
            browseProgressControl.Active = true;
        }

        public DatabaseBrowserDialog(SqlConnectionInfo connectionInfo)
            : this()
        {
            this.connectionInfo = connectionInfo;
            this.serverInstanceName = connectionInfo.InstanceName;
        }

        public DatabaseBrowserDialog(SqlConnectionInfo connectionInfo, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText, string title)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.Text = title;
            this.connectionInfo = connectionInfo;
        }

        public DatabaseBrowserDialog(IManagementService managementService, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText, string title)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.Text = title;
            this.managementServiceInterface = managementService;
        }

        public DatabaseBrowserDialog(IManagementService managementService, int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText)
            : this(serverInstanceId, serverInstanceName, multiselect, selectionRequired, descriptionLabelText)
        {
            this.managementServiceInterface = managementService;
        }

        public DatabaseBrowserDialog(int serverInstanceId, string serverInstanceName, bool multiselect, bool selectionRequired, string descriptionLabelText)
            : this()
        {
            this.serverInstanceId = serverInstanceId;
            this.serverInstanceName = serverInstanceName;
            databaseTreeView.CheckBoxes = multiselect;
            descriptionLabel.Text = descriptionLabelText;
            emptySelectionAllowed = !selectionRequired;
        }

        #endregion

        public string SelectedDatabase
        {
            get { return selectedDatabase; }
            set { selectedDatabase = value; }
        }

        public List<string> CheckedDatabases
        {
            get { return checkedDatabases; }
            set { checkedDatabases = value; }
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
                e.Result = GetDatabases(managementServiceInterface, serverInstanceId);
            else if (serverInstanceId == -1)
                e.Result = GetDatabases(connectionInfo);
            else
                e.Result = GetDatabases(connectionInfo, serverInstanceId);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            browseProgressControl.Active = false;
            browseProgressControl.Hide();
            databaseTreeView.Nodes.Clear();

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
                IDictionary<string, bool> databases = e.Result as IDictionary<string, bool>;

                if (databases == null || databases.Count == 0)
                {
                    ApplicationMessageBox.ShowWarning(this, "No databases are available.");
                    Close();
                }
                else
                {
                    databaseTreeView.SuspendLayout();

                    TreeNode serverNode = new TreeNode(serverInstanceName, 1, 1);
                    TreeNode systemDatabasesNode = new TreeNode("System Databases", 0, 0);
                    TreeNode userDatabasesNode = new TreeNode("User Databases", 0, 0);
                    databaseTreeView.Nodes.Add(serverNode);
                    serverNode.Nodes.Add(systemDatabasesNode);
                    serverNode.Nodes.Add(userDatabasesNode);

                    foreach (string databaseName in databases.Keys)
                    {
                        TreeNode databaseNode = new TreeNode(databaseName, 1, 1);
                        databaseNode.Tag = databaseName;

                        if (databases[databaseName] == true)
                        {
                            systemDatabasesNode.Nodes.Add(databaseNode);
                        }
                        else
                        {
                            userDatabasesNode.Nodes.Add(databaseNode);
                        }

                        MaybeSelectNode(databaseNode);
                    }

                    databaseTreeView.ExpandAll();
                    databaseTreeView.Sort();
                    databaseTreeView.ResumeLayout();
                }
            }

            if (emptySelectionAllowed)
                okButton.Enabled = true;
        }

        #endregion

        #region GetDatabases

        /// <summary>
        /// Gets the databases from the repository rather than SMO or the management service.
        /// The dictionary contains the database name and if it's a system database.
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static IDictionary<string, bool> GetDatabases(SqlConnectionInfo connectionInfo, int instanceId)
        {
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();

            using (SqlConnection connection = connectionInfo.GetConnection(Constants.DesktopClientConnectionStringApplicationName))
            {
                connection.Open();
                
                using (SqlCommand command = SqlHelper.CreateCommand(connection, "p_GetDatabases"))
                {
                    SqlHelper.AssignParameterValues(command.Parameters, instanceId);

                    using (SqlDataReader dataReader = command.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string databaseName = dataReader["DatabaseName"] == DBNull.Value ? string.Empty : (string) dataReader["DatabaseName"];
                            bool isSystemDatabase = dataReader["SystemDatabase"] == DBNull.Value ? false : (bool) dataReader["SystemDatabase"];

                            if (!dictionary.ContainsKey(databaseName) && !string.IsNullOrEmpty(databaseName))
                            {
                                dictionary.Add(databaseName, isSystemDatabase);
                            }
                            else
                            {
                                LOG.Warn(string.Format("Duplicate DatabaseName found: DBID:{0} DBName:{1}", dataReader["DatabaseID"], databaseName));
                            }
                        }

                        dataReader.Close();
                    }
                }

                return dictionary;
            }
        }
        
        /// <summary>
        /// Returns all Databases from sysDatabases / master.sys.Databases
        /// </summary>
        /// <param name="connectionInfo"></param>
        /// <returns></returns>
        public static IDictionary<string, bool> GetDatabases(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            Dictionary<string, bool> result = new Dictionary<string, bool>();

            using (SqlConnection connection = connectionInfo.GetConnection())
            {
                connection.Open();
                ServerVersion serverVersion = new ServerVersion(connection.ServerVersion);

                StringBuilder builder = new StringBuilder();
                string isSysDB;

                if (serverVersion.Major < 9)
                {
                    isSysDB = "cast(case when name in ('master','model','msdb','tempdb') then 1 else category & 16 end as bit)";
                    builder.AppendFormat("select name, {0} as [is_distributor] from sysdatabases", isSysDB);
                }
                else
                {
                    isSysDB = "cast(case when name in ('master','model','msdb','tempdb') then 1 else is_distributor end as bit)";
                    builder.AppendFormat("select name, {0} from master.sys.databases", isSysDB);
                }

                using (SqlCommand command = new SqlCommand(builder.ToString(), connection))
                {
                    command.CommandType = CommandType.Text;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string name = reader.GetString(0);
                            bool isSystemDatabase = reader.GetBoolean(1);

                            if (!result.ContainsKey(name))
                            {
                                result.Add(name, isSystemDatabase);    
                            }
                            else
                            {
                                LOG.Warn(string.Format("Duplicate DatabaseName found: DBName:{0}", name));
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Connects to the management service to get a list of databases from the collection service.  Use this
        /// method when trying to get the database list for a monitored server. 
        /// </summary>
        /// <param name="managementService"></param>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static IDictionary<string, bool> GetDatabases(IManagementService managementService, int instanceId)
        {
            if (managementService == null)
                throw new ArgumentException("managementService");

            return managementService.GetDatabases(instanceId, true, true);
        }

        #endregion

        // Select (or check) the node if its database name is in selectedDatabase (or checkedDatabases).
        private void MaybeSelectNode(TreeNode node)
        {
            string lowerName = node.Text.ToLower();
            
            if (databaseTreeView.CheckBoxes)
            {
                if (checkedDatabases != null)
                {
                    foreach (string nameToCheck in checkedDatabases)
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
                if (selectedDatabase != null && selectedDatabase.ToLower() == lowerName)
                {
                    node.TreeView.SelectedNode = node;
                }
            }
        }        

        private void databaseTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (!databaseTreeView.CheckBoxes)
            {
                if (databaseTreeView.SelectedNode != null)
                {
                    selectedDatabase = databaseTreeView.SelectedNode.Tag as string;
                }
                else
                {
                    selectedDatabase = null;
                }

                if (!emptySelectionAllowed)
                {
                    okButton.Enabled = selectedDatabase != null;
                }
            }
        }

        private void databaseTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            // Double click does nothing when checkboxes are shown.
            if (!databaseTreeView.CheckBoxes && selectedDatabase != null)
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

        private IEnumerable<string> EnumCheckedDatabases(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                // Only nodes with non-null tags represent databases.
                if (node.Tag != null && node.Checked)
                {
                    yield return node.Text;
                }
                else
                {
                    // Look in the node's children.
                    foreach (string dbName in EnumCheckedDatabases(node.Nodes))
                    {
                        yield return dbName;
                    }
                }
            }
        }

        // When a node is checked, check all its children.
        // When a node is unchecked, uncheck its parent so it doesn't
        // look like the entire branch is checked.
        private void databaseTreeView_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.Unknown)
            {
                // Likely a recursive call.
                return;
            }

            //Debug.Print("databaseTreeView_AfterCheck");
            databaseTreeView.BeginUpdate();
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
            databaseTreeView.EndUpdate();

            // Enable the OK button if any node representing a database is checked.
            if (!emptySelectionAllowed)
            {
                okButton.Enabled = false;
                
                foreach (string dbName in EnumCheckedDatabases(databaseTreeView.Nodes))
                {
                    okButton.Enabled = true;
                    break;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            checkedDatabases = new List<string>();

            if (databaseTreeView.CheckBoxes)
            {
                foreach (string dbName in EnumCheckedDatabases(databaseTreeView.Nodes))
                {
                    checkedDatabases.Add(dbName);
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