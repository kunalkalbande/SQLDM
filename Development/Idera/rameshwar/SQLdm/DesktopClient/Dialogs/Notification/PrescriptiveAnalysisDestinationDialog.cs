using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;
    using System.Collections.Generic;
    using System.Data;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Idera.SQLdm.Common;
    using Idera.SQLdm.Common.Analysis;
    using System.ComponentModel;

    public partial class PrescriptiveAnalysisDestinationDialog : Form
    {
        private List<int> blockedCategoryID = null;
        public List<RecommendationCategory> blockedCategoriesList = new List<RecommendationCategory>();
        private Dictionary<int, string> allCategoriesObject = new Dictionary<int, string>();
        public bool isConfigured = false;
        private int totalCategoriesCount = 0;
        //To set default configuration for blocked categories implementing list of master CategoryID as per table data available
        private Dictionary<int, string> allCategories = new Dictionary<int, string>();

        public PrescriptiveAnalysisDestinationDialog()
        {
            InitializeComponent();
            AdaptFontSize();
            allCategories = new Dictionary<int, string> { { 1, "Blocking Processes" }, { 2, "Disk" }, { 3, "Database Configuration" }, { 4, "DeadLocks" }, { 5, "Index Optimization" }, { 6, "Long Running Jobs" }, { 7, "Memory" }, { 8, "Network" }, { 9, "Open Transactions" }, { 10, "Processor" }, { 11, "Query Optimization" }, { 12, "Disaster Recovery" }, { 13, "Security" }, { 14, "Server Configuration" }, { 15, "System Page Files" }, { 16, "System Upgrade Suggestion" }, { 17, "Virtual Log Files" }, { 18, "Wait Stats" } };
        }

        #region Property

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
                    okButton.Enabled = false;
                    return blockedCategoryID;
                }
                else if (blockedCategoryID.Count == 18)
                {
                    okButton.Enabled = false;
                }
                else
                {
                    okButton.Enabled = true;
                }
                return blockedCategoryID;
            }
            set
            {
                blockedCategoryID = value;
                if (blockedCategoryID == null || blockedCategoryID.Count == 18)
                    okButton.Enabled = false;
                else
                {
                    okButton.Enabled = true;
                }
            }
        }

        #endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            //if (blockedCategoryID.Count == 0)
            //    blockedCategoryID.Add(-1);
            blockedCategoriesList.Clear();
            if (blockedCategoryID.Count > 0)
            {
                foreach (int item in blockedCategoryID)
                {
                    string categoryName = null;
                    allCategoriesObject.TryGetValue(item, out categoryName);
                    if (categoryName != null)
                        blockedCategoriesList.Add(new RecommendationCategory(item, categoryName));
                }
            }
        }
        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ServerPropertiesAnalysisConfiguration);
        }

        private void PrescriptiveAnalysisDestinationDialog_Load(object sender, EventArgs e)
        {
            InitializeCategoryTree();
            TreeViewCategories.ExpandAll();
            foreach (TreeNode node in TreeViewCategories.Nodes)
            {
                node.Checked = true;
                if (node.Nodes.Count > 0)
                    CheckAllChildNodes((TreeNodeWithValue)node, true);
            }
            if (isConfigured)
            {
                blockedCategoryID.Clear();
                foreach (RecommendationCategory item in blockedCategoriesList)
                {
                    blockedCategoryID.Add(item.CategoryId);
                }
            }
            else
                blockedCategoryID = null;
            unchekBlockedCategory(BlockedCategoryID);
        }
        private void InitializeCategoryTree()
        {
            try
            {
                if(TreeViewCategories.Nodes != null)
                    TreeViewCategories.Nodes.Clear();
                DataTable categoryDataTable = new DataTable();
                Pair<int, DataTable> categories = RepositoryHelper.GetRecommendationCategories(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                if (categories.First != 0 && categories.Second != null)
                {
                    categoryDataTable = categories.Second;
                    totalCategoriesCount = categories.First;
                }
                else
                {
                    throw new Exception("SQL Error occured in GetRecommendationCategories.");
                }
                //In Page load
                //select where id is null to retrieve the parent nodes 
                //in a datatable (called table here)

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

                    }
                    else
                    {
                    }
                    allCategoriesObject.Add(Convert.ToInt32(row["CategoryID"]), row["Name"].ToString());
                }

            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this.ParentForm.Owner, ex + " : Error while initializing Category Tree");
                throw;
            }
        }
        public void SearchAndAdd(TreeNodeWithValue rootNode, string searchKey, string newValue, int key, string description)
        {
            TreeNodeWithValue newNode;
            if (rootNode.Text == searchKey)
            {
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
        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNodeWithValue treeNode, bool nodeChecked)
        {
            foreach (TreeNodeWithValue node in treeNode.Nodes)
            {
                node.Checked = nodeChecked;
                if (node.Nodes.Count > 0)
                {
                    // If the current node has child nodes, call the CheckAllChildsNodes method recursively.
                    this.CheckAllChildNodes(node, nodeChecked);
                }
                else
                {
                    if (node.Checked && node.ValueKey.Count > 0)
                    {
                        BlockedCategoryID.Remove(node.ValueKey[0]);
                        //blockedCategoriesList.RemoveAll(x => x.CategoryId == node.ValueKey[0]);
                    }
                    else if (node.ValueKey.Count > 0)
                    {
                        foreach (int valueCategory in node.ValueKey)
                        {
                            if (!BlockedCategoryID.Contains(valueCategory))
                            {
                                BlockedCategoryID.Add(valueCategory);
                                //blockedCategoriesList.Add(new RecommendationCategory(node.ValueKey[0], node.Description.ToString()));
                            }
                        }
                    }
                }
                if (BlockedCategoryID.Count == 18)
                {
                    okButton.Enabled = false;
                }
                else
                {
                    okButton.Enabled = true;
                }
            }

        }
        //To Traverse Nodes for description
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
        private void unchekBlockedCategory(List<int> blockedCatgory)
        {
            ((TreeNodeWithValue)TreeViewCategories.Nodes[0]).Checked = !traverodes((TreeNodeWithValue)TreeViewCategories.Nodes[0], blockedCatgory);
            if (BlockedCategoryID == null || BlockedCategoryID.Count == 0)
                TreeViewCategories.Nodes[0].Checked = true;
            else
                TreeViewCategories.Nodes[0].Checked = false;
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
        #region events
        private void node_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // The code only executes if the user caused the checked state to change.
            if (e.Action != TreeViewAction.Unknown)
            {
                e.Node.TreeView.SelectedNode = e.Node;
                if (e.Node.Nodes.Count > 0) // this code propagates changes down the tree
                {
                    /* Calls the CheckAllChildNodes method, passing in the current 
                    Checked value of the TreeNode whose checked state changed. */
                    this.CheckAllChildNodes((TreeNodeWithValue)e.Node, e.Node.Checked);
                }
                else
                {
                    //Remove From blocked category ID 
                    if (e.Node.Checked)
                    {
                        foreach (int id in ((TreeNodeWithValue)e.Node).ValueKey)
                        {
                            BlockedCategoryID.Remove(id);
                            //blockedCategoriesList.RemoveAll(x => x.CategoryId == id);
                        }

                    }
                    else
                    {
                        foreach (int valueCategory in ((TreeNodeWithValue)e.Node).ValueKey)
                        {
                            if (!BlockedCategoryID.Contains(valueCategory))
                            {
                                BlockedCategoryID.Add(valueCategory);
                                //string categoryName = null;
                                //allCategoriesObject.TryGetValue(valueCategory, out categoryName);
                                //if (categoryName != null)
                                //    blockedCategoriesList.Add(new RecommendationCategory(valueCategory, categoryName));
                            }
                        }
                    }
                    if (BlockedCategoryID.Count == 18)
                    {
                        okButton.Enabled = false;
                    }
                    else
                    {
                        okButton.Enabled = true;
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
                labelCategoryTree.Text = ((TreeNodeWithValue)nodes[0]).Description;
            else
                labelCategoryTree.Text = "";

        }

        #endregion

        #region class
        //To give value part to node
        public class TreeNodeWithValue : TreeNode
        {
            public TreeNodeWithValue(string name)
                : base(name)
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
}