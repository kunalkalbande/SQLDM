using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Data;
using System.Data.SqlClient;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Dialogs.Analysis;
using System.Data;
using System.Globalization;
using Wintellect.PowerCollections;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Controls.Analysis
{
    public partial class AnalysisConfigurationPage : UserControl
    {
        public int? CloudProviderId { get; private set; }

        #region constants
        private const string All_Category = "All Categories";
        #endregion

        private static AnalysisConfiguration analysisConfig;
        private bool isInit;
        private string errorMessage;
        private int instanceId;
        private List<int> setOfAllCategories;
        private bool isDefaultView = true;
        private int totalCategoriesCount = 0;
        //To log errors
        Logger Log = Logger.GetLogger("AnalysisConfigurationPage");

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

        public int BorderWidth
        {
            get { return this.office2007PropertyPage1.BorderWidth; }
            set { this.office2007PropertyPage1.BorderWidth = value; }
        }
        [Browsable(false)]
        public string ErrorMessage
        {
            get { return errorMessage; }
        }
        public AnalysisConfigurationPage(int instanceID, int? cloudProviderId)
        {
            try
            {
                CloudProviderId = cloudProviderId;
                Log.Info("Constructing Analysis Configuration page.");
                this.instanceId = instanceID;
                InitializeComponent();
                if (analysisConfig == null)
                {
                    analysisConfig = new AnalysisConfiguration(instanceId);
                }
                //analysisConfig.BlockedCategoryID = new List<int>();
                setOfAllCategories = new List<int>();
                analysisConfig.BlockedDatabaseIDList = new List<int>();
                analysisConfig.BlockedRecommendationID = new List<string>();
                Init();
                Log.Info("Successfully constructed Analysis Configuration page.");
                SetPropertiesTheme();
                Infragistics.Windows.Themes.ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            }
            catch (Exception ex)
            {
                Log.Error("Error while constructing analysis configuration view.", ex);
                throw;
            }
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }

        public bool CheckForErrors()
        {
            errorMessage = string.Empty;
            AnalysisConfiguration template = analysisConfig;


            if (!template.UseDefault)
            {
                TimeSpan resultTime;
                if (TimeSpan.TryParseExact(template.StartTime.ToString(),
                                    "hhmmss",
                                    CultureInfo.InvariantCulture,
                                    out resultTime))
                {
                    errorMessage = "The time is not in valid format.";
                    return true;
                }

            }

            if (template.AnalysisDuration < 0 && template.AnalysisDuration >500)
            {
                errorMessage = "The duration should always be greater than 0 and less than 500 mins to run analysis for scheduled time.";
                //Changing default values for scheduling
                //return true;
            }

            if (template.SelectedDays == 0)
            {
                errorMessage = "The days of the week to include in the analysis should be selected for scheduling.";
                //Changing default values for scheduling
                //return true;
            }
            //Changes to make atleast one category mandatory
            //if (template.BlockedCategoryID != null && template.BlockedCategoryID.Count == totalCategoriesCount)
            //{
            //    errorMessage = "Selection of one category mandatory for analysis configuration.";
            //    //Changing default values for scheduling
            //    return true;
            //}
            return false;
        }

        public void Init()
        {
            if (analysisConfig == null)
            {
                analysisConfig = new AnalysisConfiguration(instanceId);
                //analysisConfig.BlockedCategoryID = new List<int>();
                analysisConfig.BlockedDatabaseIDList = new List<int>();
                analysisConfig.BlockedRecommendationID = new List<string>();
            }

            analysisConfig.StartTime = DateTime.Parse("1753-1-1").Date + beginTimeCombo.Time;
            analysisConfig.AnalysisDuration = Convert.ToInt32(NumericAnalysisDuration.Value);
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
                Pair<int, DataTable> categories = RepositoryHelper.GetRecommendationCategories(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, analysisConfig.SqlServerID, CloudProviderId);
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
                //In Page load
                //select where id is null to retrieve the parent nodes 
                //in a datatable (called table here)

                //Add root node
                TreeNodeWithValue rootNode = new TreeNodeWithValue(categoryDataTable.Rows[0]["Name"].ToString());
                rootNode.Description = categoryDataTable.Rows[0]["Description"].ToString();
                rootNode.ValueKey.Add( Convert.ToInt32(categoryDataTable.Rows[0]["CategoryID"]));
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

        private void Update(object sender, EventArgs e)
        {
            if (isInit)
                return;

            short days = 0;

            if (sundayCheckbox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Sunday);
            if (mondayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Monday);
            if (tuesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Tuesday);
            if (wednesdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Wednesday);
            if (thursdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Thursday);
            if (fridayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Friday);
            if (saturdayCheckBox.Checked) days |= MonitoredSqlServer.DayOfWeekToShort(DayOfWeek.Saturday);

            if (analysisConfig == null)
            {
                analysisConfig = new AnalysisConfiguration(instanceId);
            }

            try
            {
                analysisConfig.SelectedDays = days;
                analysisConfig.StartTime = DateTime.Parse("1753-1-1").Date + beginTimeCombo.Time;
                analysisConfig.AnalysisDuration = Convert.ToInt32(NumericAnalysisDuration.Value);
            }
            catch(Exception ex) {
                Log.Error("Error while updating analysis configuration object values : ", ex);
                throw;
            }
        }
        // Updates all child tree nodes recursively.
        private void CheckAllChildNodes(TreeNodeWithValue treeNode, bool nodeChecked)
        {
            try
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
                            analysisConfig.BlockedCategoryID.Remove(node.ValueKey[0]);
                        }
                        else if (node.ValueKey.Count > 0)
                        {
                            foreach (int valueCategory in node.ValueKey)
                            {
                                if (!analysisConfig.BlockedCategoryID.Contains(valueCategory))
                                {
                                    analysisConfig.BlockedCategoryID.Add(valueCategory);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error while selecting category tree items for analysis configuration : ", ex);
            }

        }

        #region Event Handlers
        private void productionServerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            analysisConfig.ProductionServer = productionServerCheckBox.Checked;
        }

        // Start:SQLDm 10.0 - Praveen Suhalka - Scheduling status
        private void schedulingStatusCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            daysChkContainer.Enabled = schedulingStatusCheckBox.Checked;
            timeDateContainer.Enabled = schedulingStatusCheckBox.Checked;
            
            analysisConfig.SchedulingStatus = schedulingStatusCheckBox.Checked;

        }
        // End:SQLDm 10.0 - Praveen Suhalka - Scheduling status

        private void onlineTranscationProcessingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            analysisConfig.IsOLTP = onlineTranscationProcessingCheckBox.Checked;
        }
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
                            analysisConfig.BlockedCategoryID.Remove(id);
                        }

                    }
                    else
                    {
                        foreach (int valueCategory in ((TreeNodeWithValue)e.Node).ValueKey)
                        {
                            if (!analysisConfig.BlockedCategoryID.Contains(valueCategory))
                            {
                                analysisConfig.BlockedCategoryID.Add(valueCategory);
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
                        return nodeList.ToArray<TreeNode>();
                    }
                    else if (childNode.Nodes.Count > 0)
                    {
                        TreeNode[] nodeFound = FindNodesForDescription(childNode, findNodeText);
                        if (nodeFound.Length > 0)
                            return nodeFound;
                    }

                }
            }
            return nodeList.ToArray<TreeNode>();
        }

        private void TreeViewCategories_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var nodes = FindNodesForDescription(TreeViewCategories.Nodes[0], e.Node.Text);
            if (nodes.Length > 0)
                labelCategoryTree.Text = ((TreeNodeWithValue)nodes[0]).Description;
            else
                labelCategoryTree.Text = "";
                
        }

        private void btnAdvanceSetting_Click(object sender, EventArgs e)
        {
            try
            {
                using (ServerSettingsDialog dialog = new ServerSettingsDialog(analysisConfig.SqlServerID, analysisConfig.IncludeDatabase, analysisConfig.FilterApplication, analysisConfig.BlockedDatabaseIDList, analysisConfig.BlockedRecommendationID))
                {
                    if (dialog.ShowDialog(this) == DialogResult.OK)
                    {
                        analysisConfig.FilterApplication = dialog.filtersSettings.FilterApplication;
                        analysisConfig.IncludeDatabase = dialog.filtersSettings.IncludeDatabase;
                        analysisConfig.BlockedRecommendationID = dialog.blockedRecommendations.BlockedRecommendationID;
                        analysisConfig.BlockedDatabaseIDList = dialog.blockedDatabases.BlockedDatabaseIDList;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error on Advance analysi configuration page instance.",ex);
                throw;

            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            analysisConfig.AnalysisDuration = Convert.ToInt32(NumericAnalysisDuration.Value);
        }

        public void updateAnalysisControlsUsingAvailalbleData(AnalysisConfiguration analysisConfigData)
        {
            if (analysisConfigData != null)
            {
                isInit = true;
                analysisConfig = analysisConfigData;
                //type of server
                productionServerCheckBox.Checked = analysisConfigData.ProductionServer;
                onlineTranscationProcessingCheckBox.Checked = analysisConfigData.IsOLTP;

                schedulingStatusCheckBox.Checked = analysisConfigData.SchedulingStatus; // SQLDm 10.0 - Praveen Suhalka - Scheduling status

                daysChkContainer.Enabled = schedulingStatusCheckBox.Checked;
                timeDateContainer.Enabled = schedulingStatusCheckBox.Checked;
                //days selection

                sundayCheckbox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Sunday, analysisConfigData.SelectedDays);
                mondayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Monday, analysisConfigData.SelectedDays);
                tuesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Tuesday, analysisConfigData.SelectedDays);
                wednesdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Wednesday, analysisConfigData.SelectedDays);
                thursdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Thursday, analysisConfigData.SelectedDays);
                fridayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Friday, analysisConfigData.SelectedDays);
                saturdayCheckBox.Checked = MonitoredSqlServer.MatchDayOfWeek(DayOfWeek.Saturday, analysisConfigData.SelectedDays);

                //Time ana duration
                beginTimeCombo.Time = analysisConfigData.StartTime.TimeOfDay;
                NumericAnalysisDuration.Value = analysisConfigData.AnalysisDuration > 0 ? analysisConfigData.AnalysisDuration : 1;
                if (analysisConfigData.BlockedCategoryID != null && analysisConfigData.BlockedCategoryID.Count != 0)
                    unchekBlockedCategory(analysisConfigData.BlockedCategoryID);

                isInit = false;
            }
            else
            {
                analysisConfigData = new AnalysisConfiguration(instanceId);
                analysisConfig = analysisConfigData;

                //Default the scheduling should be disabled
                schedulingStatusCheckBox.Checked = analysisConfigData.SchedulingStatus;

                daysChkContainer.Enabled = schedulingStatusCheckBox.Checked;
                timeDateContainer.Enabled = schedulingStatusCheckBox.Checked;
                unchekBlockedCategory(analysisConfigData.BlockedCategoryID);
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
        #endregion


        private void ScaleControlsAsPerResolution()
        {
            this.propertiesHeaderStripSelectCategory.Dock = DockStyle.None;
            this.btnAdvancedSettingButton.Anchor = AnchorStyles.None;
            this.btnAdvancedSettingButton.Location = new Point(this.btnAdvancedSettingButton.Location.X + 100, this.btnAdvancedSettingButton.Location.Y);
        }
    }
}
