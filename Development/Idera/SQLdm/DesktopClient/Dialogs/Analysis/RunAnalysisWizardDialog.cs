using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.Common.Analysis;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Presenters.GridEntries;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Analysis;
using BBS.TracerX;
using System.ComponentModel;

namespace Idera.SQLdm.DesktopClient.Dialogs.Analysis
{
    enum WizardPage
    {
        CategoriesPage,
        FiltersPage
    }

    public partial class RunAnalysisWizard : Form
    {
        #region constants
        private const string QUERY_OPTIMIZATION = "Query Optimization";
        private const string INDEX_OPTIMIZATION = "Index Optimization";
        private const string WORKLOAD = "Workload";
        private const string ALL_CATEGORY = "All Category";
        #endregion

        private static AnalysisConfiguration analysisConfig;
        private int instanceId;
        private List<int> setOfAllCategories;
        private bool isDefaultView = true;
        private int totalCategoriesCount = 0;
        private int instanceID;
        private int includeDatabase;
        private string filterApplicationText;
        private MonitoredSqlServer serverInstance;
        private Dictionary<int, string> allCategoriesObject = new Dictionary<int, string>();
        private List<DatabaseInformation> _databases = new List<DatabaseInformation>();
        public event EventHandler SettingsChanged;
        private bool _modified = false;
        public bool Modified { get { return (_modified); } }
        public static List<DatabaseInformation> Databases = new List<DatabaseInformation>();
        private List<int> blockedCategoryID = null;
        public List<RecommendationCategory> blockedCategoriesList = new List<RecommendationCategory>();
        public bool isConfigured = false;
        private Dictionary<int, string> allCategories = new Dictionary<int, string>();
        string errorMessage = string.Empty;
        string queryFromDiagnose = string.Empty;
        string databaseNameDiagnose = "master";
        ActiveWaitsConfiguration waitConfig;
        QueryMonitorConfiguration queryConfig;
        bool isWorkloadAnalysis = false;
        bool isInit = false;
        ViewContainer viewHost;
        private static AnalysisStatusDialog analysisStatusDialog;
        private Dictionary<int, string> blockedCategories = new Dictionary<int, string>();

        private static AnalysisStatusDialog GetAnalysisStatusDialog(
            MonitoredSqlServer serverInstance,
            List<int> BlockedCategoryID,
            ActiveWaitsConfiguration waitConfig,
            QueryMonitorConfiguration queryConfig,
            string queryFromDignose,
            string databaseNameDiagnose,
            bool isWorkloadAnalysis,
            ViewContainer viewHost,
            int analysisDuration,
            Dictionary<int,string> BlockedCategories
        )
        {
            if (analysisStatusDialog == null || analysisStatusDialog.IsDisposed)
            {
                analysisStatusDialog = new AnalysisStatusDialog(
                                                        serverInstance,
                                                        analysisConfig.BlockedCategoryID,
                                                        waitConfig,
                                                        queryConfig,
                                                        queryFromDignose,
                                                        databaseNameDiagnose,
                                                        isWorkloadAnalysis,
                                                        viewHost,
                                                        DisposeAnalysisStatusDialog,
                                                        analysisDuration,
                                                        analysisConfig.BlockedCategories
                                                );
            }
            return analysisStatusDialog;
        }

        public static void DisposeAnalysisStatusDialog()
        {
            if (analysisStatusDialog != null && analysisStatusDialog.IsDisposed == false)
            {
                analysisStatusDialog.Dispose();
            }
            analysisStatusDialog = null;
        }

        //To log errors
        Logger Log = Logger.GetLogger("RunAnalysisWizard");

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AnalysisConfiguration AnalysisConfig
        {
            get { return analysisConfig; }
            set { analysisConfig = value; Init(); }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; }
        }

        [Browsable(false)]
        public string ErrorMessage
        {
            get { return errorMessage; }
        }

        internal static Form GetAnalysisForm(MonitoredSqlServer serverInstance, ViewContainer vHost)
        {
            Form form = analysisStatusDialog;
            if (form == null || form.IsDisposed)
            {
                return new RunAnalysisWizard(serverInstance, vHost);
            }
            return form;
        }

        private RunAnalysisWizard(MonitoredSqlServer serverInstance, ViewContainer vHost)
        {
            viewHost = vHost;
            try
            {
                Log.Info("Constructing Run Analysis Configuration popup page.");
                this.instanceID = serverInstance.Id;
                this.includeDatabase = serverInstance.AnalysisConfiguration.IncludeDatabase;
                this.filterApplicationText = serverInstance.AnalysisConfiguration.FilterApplication;
                this.serverInstance = serverInstance;
                InitializeComponent();
                PanelAnalysisCategories.AutoScroll = false;
                PanelAnalysisCategories.HorizontalScroll.Enabled = false;
                PanelAnalysisCategories.HorizontalScroll.Visible = false;
                PanelAnalysisCategories.HorizontalScroll.Maximum = 0;
                PanelAnalysisCategories.AutoScroll = true;
                if (analysisConfig == null)
                {
                    analysisConfig = new AnalysisConfiguration(this.instanceID);
                }
                //analysisConfig.BlockedCategoryID = new List<int>();
                setOfAllCategories = new List<int>();
                analysisConfig.BlockedDatabaseIDList = new List<int>();
                analysisConfig.BlockedRecommendationID = new List<string>();
                waitConfig = ApplicationModel.Default.AllInstances[this.instanceID].ActiveWaitsConfiguration;
                queryConfig = ApplicationModel.Default.AllInstances[this.instanceID].QueryMonitorConfiguration;
                Log.Info("Successfully constructed Run Analysis Configuration popup page.");
            }
            catch (Exception ex)
            {
                Log.Error("Error while constructing analysis configuration view.", ex);
                throw;
            }
        }

        private void RunAnalysisWizard_Load(object sender, EventArgs e)
        {
            _databases = GetDatabaseList();
            populateDatabaseCombobox();
            Init();

            if (isConfigured)
            {
                blockedCategoryID.Clear();
                foreach (RecommendationCategory item in blockedCategoriesList)
                {
                    blockedCategoryID.Add(item.CategoryId);
                    //blockedCategories.Add(item.CategoryId, item.CategoryName);
                }
            }
            else
                blockedCategoryID = null;
            unchekBlockedCategory(analysisConfig.BlockedCategoryID);
            this.TreeViewCategories.Nodes[0].EnsureVisible();
            ShowSelectedText();
        }

        private List<DatabaseInformation> GetDatabaseList()
        {
            if (Settings.Default == null || Settings.Default.ActiveRepositoryConnection == null || Settings.Default.ActiveRepositoryConnection.ConnectionInfo == null)
            {
                // Log.Error("No databases found because, Could not get Connection Info of active repository. Object 'Settings.Default.ActiveRepositoryConnection.ConnectionInfo' value is null.");
                Databases = new List<DatabaseInformation>();
            }
            else
                Databases = RepositoryHelper.GetDatabasesForInstance(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, this.instanceID);
            return Databases;
        }

        private void populateDatabaseCombobox()
        {
            try
            {
                if (null == _databases)
                {
                    _databases = new List<DatabaseInformation>();
                }
                DatabaseInformation defaultDI = new DatabaseInformation();
                defaultDI.DatabaseID = 0;
                defaultDI.DatabaseName = "Select Database to filter.";
                defaultDI.IsSystemDatabase = false;
                _databases.Insert(0, defaultDI);
                _databaseFilterTypeComboBox.DataSource = _databases;
                _databaseFilterTypeComboBox.ValueMember = "DatabaseID";
                _databaseFilterTypeComboBox.DisplayMember = "DatabaseName";
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this, "Not able to populate data bases.", ex);
                throw;
            }
        }

        private void unchekBlockedCategory(List<int> blockedCatgory)
        {
            ((TreeNodeWithValue)TreeViewCategories.Nodes[0]).Checked = !traverodes((TreeNodeWithValue)TreeViewCategories.Nodes[0], blockedCatgory);
            if (analysisConfig.BlockedCategoryID == null || analysisConfig.BlockedCategoryID.Count == 0)
                TreeViewCategories.Nodes[0].Checked = true;
            else
                TreeViewCategories.Nodes[0].Checked = false;
        }

        public void Init()
        {
            if (analysisConfig == null)
            {
                analysisConfig = new AnalysisConfiguration(instanceId);
                //analysisConfig.BlockedCategoryID = new List<int>();
                analysisConfig.BlockedDatabaseIDList = new List<int>();
                analysisConfig.BlockedRecommendationID = new List<string>();
                analysisConfig.BlockedCategories = new Dictionary<int, string>();
            }

            isInit = true;

            InitializeCategoryTree();

            //Default all category should be unselected
            if (isDefaultView)
            {
                foreach (TreeNode node in TreeViewCategories.Nodes)
                {
                    node.Checked = true;
                    if (node.Nodes.Count > 0)
                        CheckAllChildNodes((TreeNodeWithValue)node, true);
                }
            }

            TreeViewCategories.ExpandAll();
            isInit = false;
        }

        private void InitializeCategoryTree()
        {
            try
            {
                TreeViewCategories.Nodes.Clear();
                DataTable categoryDataTable = new DataTable();
                Pair<int, DataTable> categories = RepositoryHelper.GetRecommendationCategories(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serverInstance.Id, serverInstance.CloudProviderId);
                if (categories.First != 0 && categories.Second != null)
                {
                    categoryDataTable = categories.Second;
                    totalCategoriesCount = categories.First;
                }
                else
                {
                    //throw new Exception("SQL Error occured in GetRecommendationCategories.");
                    Log.Error("SQL Error occured in GetRecommendationCategories. Please check PrescriptiveRecommendation and PrescriptiveRecommendationCategory tables. Master categories not found.");
                    return;
                }
                //In Page load select where id is null to retrieve the parent nodes in a datatable (called table here)
                //Add root node
                TreeNodeWithValue rootNode = new TreeNodeWithValue(categoryDataTable.Rows[0]["Name"].ToString());
                rootNode.Description = categoryDataTable.Rows[0]["Description"].ToString();
                rootNode.ValueKey.Add(Convert.ToInt32(categoryDataTable.Rows[0]["CategoryID"]));
                TreeViewCategories.Nodes.Add(rootNode);
                foreach (DataRow row in categoryDataTable.Rows)
                {
                    if (!string.IsNullOrWhiteSpace(row["Parent"].ToString()) && row["Parent"].ToString() == rootNode.Text)
                    {
                        SearchAndAdd(rootNode, row["Parent"].ToString(), row["Name"].ToString(), Convert.ToInt32(row["CategoryID"]), row["Description"].ToString());
                    }
                }
                foreach (DataRow row in categoryDataTable.Rows)
                {
                    //TreeNodeWithValue node;
                    if (!string.IsNullOrWhiteSpace(row["Parent"].ToString()) && row["Parent"].ToString() != rootNode.Text)
                    {
                        SearchAndAdd(rootNode, row["Parent"].ToString(), row["Name"].ToString(), Convert.ToInt32(row["CategoryID"]), row["Description"].ToString());

                        //setOfAllCategories.Add(Convert.ToInt32(row["CategoryID"]));
                    }
                    else
                    {
                        //setOfAllCategories.Add(Convert.ToInt32(row["CategoryID"]));
                        //node = new TreeNodeWithValue(row["Name"].ToString());
                        //node.ValueKey.Add(Convert.ToInt32(row["CategoryID"]));

                        //rootNode.Nodes.Add(node);
                    }
                    //rootNode.ValueKey = analysisConfig.BlockedCategoryID;
                }
                //setOfAllCategories = analysisConfig.BlockedCategoryID;
                if (!isDefaultView)
                    analysisConfig.BlockedCategoryID.Clear();

            }
            catch (Exception ex)
            {
                Log.Error("Error while initializing Category Tree", ex);
                throw;
            }
        }

        public void SearchAndAdd(TreeNodeWithValue rootNode, string searchKey, string newValue, int key, string description)
        {
            TreeNodeWithValue newNode;
            if (rootNode.Text == searchKey)
            {
                //newNode = new TreeNodeWithValue(searchKey);
                //newNode.ValueKey.Add(key);
                //rootNode.Nodes.Add(newNode);
                TreeNodeWithValue childNode = new TreeNodeWithValue(newValue);
                childNode.ValueKey.Add(key);
                childNode.Description = description;
                rootNode.Nodes.Add(childNode);
            }
            else
            {
                for (int index = 0; index < rootNode.Nodes.Count; index++)
                {
                    TreeNodeWithValue node = (TreeNodeWithValue)rootNode.Nodes[index];
                    if (node.Text == searchKey)
                    {
                        newNode = new TreeNodeWithValue(newValue);
                        newNode.ValueKey.Add(key);
                        newNode.Description = description;
                        node.Nodes.Add(newNode);
                        break;
                    }
                    if (index == rootNode.Nodes.Count - 1)
                    {
                        newNode = new TreeNodeWithValue(searchKey);
                        newNode.ValueKey.Add(key);
                        newNode.Description = description;
                        rootNode.Nodes.Add(newNode);
                        TreeNodeWithValue childNode = new TreeNodeWithValue(newValue);
                        childNode.ValueKey.Add(key);
                        childNode.Description = description;
                        newNode.Nodes.Add(childNode);
                    }
                }
            }
        }

        public List<int> BlockedCategoryID
        {
            get
            {
                if (blockedCategoryID == null)
                {
                    //isConfigured = true;
                    if (allCategories != null)
                    {
                        blockedCategoryID = new List<int>();
                        foreach (var key in allCategories.Keys)
                        {
                            blockedCategoryID.Add(key);
                        }
                    }
                    // okButton.Enabled = false;
                    return blockedCategoryID;
                }
                else if (blockedCategoryID.Count == 18)
                {
                    // okButton.Enabled = false;
                }
                else
                {
                    // okButton.Enabled = true;
                }
                return blockedCategoryID;
            }
            set
            {
                blockedCategoryID = value;
                //if (blockedCategoryID == null || blockedCategoryID.Count == 18)
                //  //  okButton.Enabled = false;
                //else
                //{
                //  //  okButton.Enabled = true;
                //}
            }
        }

        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNodeWithValue treeNode, bool nodeChecked)
        {
            foreach (TreeNodeWithValue node in treeNode.Nodes)
            {
                node.Checked = (isInit && (node.Text.Equals(QUERY_OPTIMIZATION) ||
                                node.Text.Equals(INDEX_OPTIMIZATION) ||
                                node.Text.Equals(WORKLOAD))) ?
                                false :
                                nodeChecked;

                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
                else
                {
                    if (node.Checked && node.ValueKey.Count > 0)
                    {
                        analysisConfig.BlockedCategoryID.Remove(node.ValueKey[0]);
                        analysisConfig.BlockedCategories.Remove(node.ValueKey[0]);
                        //blockedCategoriesList.RemoveAll(x => x.CategoryId == node.ValueKey[0]);
                    }
                    else if (node.ValueKey.Count > 0)
                    {
                        foreach (int valueCategory in node.ValueKey)
                        {
                            if (!analysisConfig.BlockedCategoryID.Contains(valueCategory))
                            {
                                analysisConfig.BlockedCategoryID.Add(valueCategory);
                                analysisConfig.BlockedCategories.Add(valueCategory, node.Text);
                                //blockedCategoriesList.Add(new RecommendationCategory(node.ValueKey[0], node.Description.ToString()));
                            }
                        }
                    }
                }
                if (analysisConfig.BlockedCategoryID.Count == 18)
                {
                    // okButton.Enabled = false;
                }
                else
                {
                    //  okButton.Enabled = true;
                }
            }
        }

        private void btnAnalysisCategoryWizardCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnAnalysisCategoryWizardFinish_Click(object sender, EventArgs e)
        {
            try
            {
                queryFromDiagnose = txtQueryFromDiagnose.Text.Trim().ToString();
                databaseNameDiagnose = ((DatabaseInformation)_databaseFilterTypeComboBox.SelectedItem).DatabaseName.ToString();
                if (databaseNameDiagnose == "Select Database to filter.")
                {
                    databaseNameDiagnose = "master";
                }

                this.Hide();
                GetAnalysisStatusDialog(
                        serverInstance,
                        analysisConfig.BlockedCategoryID,
                        waitConfig,
                        queryConfig,
                        queryFromDiagnose,
                        databaseNameDiagnose,
                        isWorkloadAnalysis,
                        viewHost,
                        (int)numericUpDown1.Value,
                        analysisConfig.BlockedCategories
                ).ShowDialog(this);
            }
            catch (Exception ex)
            {
                Log.Error("Error while opening RunAnalysisWizard.", ex);
            }
        }

        private void _databaseFilterTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            includeDatabase = ((DatabaseInformation)_databaseFilterTypeComboBox.SelectedItem).DatabaseID;
            OnSettingsChanged(EventArgs.Empty);
        }

        private void OnSettingsChanged(EventArgs e)
        {
            //if (_loading) return;
            _modified = true;
            if (null != SettingsChanged) SettingsChanged(this, e);
        }

        private bool traverodes(TreeNodeWithValue tree, List<int> blockedCatgory)
        {
            bool isChildUnchecked = false;
            foreach (TreeNodeWithValue node in tree.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    isChildUnchecked = traverodes(node, blockedCatgory);
                    if (isChildUnchecked)
                    {
                        node.Checked = !isChildUnchecked;
                    }
                }
                else
                {
                    if (blockedCatgory.Contains(node.ValueKey[0]))
                    {
                        node.Checked = false;
                        isChildUnchecked = true;
                    }
                }
            }
            return isChildUnchecked;
        }

        private void node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                e.Node.TreeView.SelectedNode = e.Node;
                if (e.Node.Text.Equals(QUERY_OPTIMIZATION) ||
                    e.Node.Text.Equals(INDEX_OPTIMIZATION) ||
                    e.Node.Text.Equals(WORKLOAD) ||
                    e.Node.Text.Equals(ALL_CATEGORY))
                {
                    if (e.Node.Checked == true)
                    {
                        isWorkloadAnalysis = true;
                    }
                    else
                    {
                        isWorkloadAnalysis = false;
                    }
                }

                if (e.Node.Nodes.Count > 0) // this code propagates changes down the tree
                {
                    /* Calls the CheckAllChildNodes method, passing in the current Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes((TreeNodeWithValue)e.Node, e.Node.Checked);
                }
                else
                {
                    //Remove From blocked category ID 
                    if (e.Node.Checked)
                    {
                        foreach (int id in ((TreeNodeWithValue)e.Node).ValueKey)
                        {
                            analysisConfig.BlockedCategoryID.Remove(id);
                            analysisConfig.BlockedCategories.Remove(id);
                        }
                    }
                    else
                    {
                        foreach (int valueCategory in ((TreeNodeWithValue)e.Node).ValueKey)
                        {
                            if (!analysisConfig.BlockedCategoryID.Contains(valueCategory))
                            {
                                analysisConfig.BlockedCategoryID.Add(valueCategory);
                                analysisConfig.BlockedCategories.Add(valueCategory, ((TreeNodeWithValue)e.Node).Text);
                            }
                        }
                    }
                }

                TreeNode n = e.Node;
                while (null != n.Parent)    // this code propagates changes up the tree
                {
                    bool AllSiblingsChecked = true;
                    foreach (TreeNode sibling in n.Parent.Nodes)
                        AllSiblingsChecked = AllSiblingsChecked && sibling.Checked;
                    n.Parent.Checked = AllSiblingsChecked;
                    n = n.Parent;
                }
            }
        }

        private void TreeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodes = FindNodesForDescription(TreeViewCategories.Nodes[0], e.Node.Text);
            if (nodes.Length > 0)
            {
                labelCategoryTree.Text = ((TreeNodeWithValue)nodes[0]).Description;
            }
            else
            {
                labelCategoryTree.Text = "";
            }
        }
        private void ShowSelectedText()
        {
            var nodes = FindNodesForDescription(TreeViewCategories.Nodes[0], TreeViewCategories.Nodes[0].Text);
            if (nodes.Length > 0)
            {
                labelCategoryTree.Text = ((TreeNodeWithValue)nodes[0]).Description;
            }
            else
            {
                labelCategoryTree.Text = "";
            }
        }
        public TreeNode[] FindNodesForDescription(TreeNode motherNode, string findNodeText)
        {
            List<TreeNode> nodeList = new List<TreeNode>();
            if (motherNode.Text == findNodeText)
            {
                nodeList.Add(motherNode);
            }
            else
            {
                foreach (TreeNode childNode in motherNode.Nodes)
                {
                    if (childNode.Text.Equals(findNodeText, StringComparison.CurrentCulture))
                    {
                        nodeList.Add(childNode);
                        return nodeList.ToArray();
                    }
                    else if (childNode.Nodes.Count > 0)
                    {
                        TreeNode[] nodeFound = FindNodesForDescription(childNode, findNodeText);
                        if (nodeFound.Length > 0)
                            return nodeFound;
                    }
                }
            }
            return nodeList.ToArray();
        }
    }

    #region class
    //To give value part to node
    public class TreeNodeWithValue : TreeNode
    {
        public TreeNodeWithValue(string name) : base(name)
        {
            if (valueKey == null)
                valueKey = new List<int>();
        }

        private List<int> valueKey;
        public List<int> ValueKey
        {
            get
            {
                return valueKey;
            }
            set
            {
                valueKey = value;
            }
        }

        private string description;
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value;
            }
        }
    }
    #endregion
}