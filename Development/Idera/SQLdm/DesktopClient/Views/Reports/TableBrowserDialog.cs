using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using System.Collections.Generic;
using BBS.TracerX;
using System.ComponentModel;
using Idera.SQLdm.DesktopClient.Helpers;
using Wintellect.PowerCollections;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    public partial class TableBrowserDialog : Form {
        private int _serverId;
        private string _databaseName;
        private List<Triple<string, string, bool>> _checkedTables;
        private static readonly Logger Log = Logger.GetLogger("TableBrowserDialog");

        public TableBrowserDialog(int serverId, string dbName) {
            InitializeComponent();
            browseProgressControl.Active = true;
            _serverId = serverId;
            _databaseName = dbName;
            tableTreeView.CheckBoxes = true;

            // Adapt font size.
            AdaptFontSize();
        }

        public List<Triple<string, string, bool>> CheckedTables {
            get { return _checkedTables; }
            set { _checkedTables = value; }
        }

        private static List<Triple<string, string, bool>> GetTables(int serverId, string databaseName) {
            using (Log.DebugCall()) {
                return RepositoryHelper.GetTables(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, serverId, databaseName);
                
                //return ManagementServiceHelper.IManagementService.GetTables(serverId, databaseName, true, true);
            }
        }

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            browseProgressControl.Active = true;
            this.Cursor = Cursors.WaitCursor;
            cancelButton.Cursor = Cursors.Default;
            backgroundWorker.RunWorkerAsync();
        }

        private void backgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "TableBrowserWorker";
            e.Result = GetTables(_serverId, _databaseName);
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e) {
            using (Log.DebugCall()) {
                browseProgressControl.Active = false;
                browseProgressControl.Hide();
                this.Cursor = Cursors.Default;
                tableTreeView.Nodes.Clear();

                if (DialogResult == DialogResult.Cancel) {
                    return;
                } else if (e.Error != null) {
                    ApplicationMessageBox.ShowError(this, e.Error);
                    Close();
                } else {
                    List<Triple<string, string, bool>> tables = e.Result as List<Triple<string, string, bool>>;

                    if (tables == null || tables.Count == 0) {
                        ApplicationMessageBox.ShowWarning(this, String.Format("Table statistics for the {0} database have not been collected.  Table information will not appear until this collection has occurred.", _databaseName));
                        Close();
                    } else {
                        tableTreeView.BeginUpdate();
                        tableTreeView.SuspendLayout();

                        TreeNode serverNode = new TreeNode(_databaseName, 1, 1);
                        tableTreeView.Nodes.Add(serverNode);
                        TreeNode systemDatabasesNode = null;
                        TreeNode userDatabasesNode = null;

                        foreach (Triple<string, string, bool> triple in tables) {
                            TreeNode databaseNode = new TreeNode(triple.First + "." + triple.Second, 1, 1);
                            databaseNode.Tag = triple;

                            if (triple.Third) {
                                if (systemDatabasesNode == null) {
                                    systemDatabasesNode = new TreeNode("System Tables", 0, 0);
                                    serverNode.Nodes.Add(systemDatabasesNode);
                                }
                                systemDatabasesNode.Nodes.Add(databaseNode);
                            } else {
                                if (userDatabasesNode == null) {
                                    userDatabasesNode = new TreeNode("User Tables", 0, 0);
                                    serverNode.Nodes.Add(userDatabasesNode);
                                }
                                userDatabasesNode.Nodes.Add(databaseNode);
                            }

                            databaseNode.Checked = (_checkedTables != null && _checkedTables.Contains(triple));
                        }

                        if (systemDatabasesNode != null) systemDatabasesNode.Checked = AllChildrenAreChecked(systemDatabasesNode);
                        if (userDatabasesNode != null) userDatabasesNode.Checked = AllChildrenAreChecked(userDatabasesNode);
                        serverNode.Checked = AllChildrenAreChecked(serverNode);

                        tableTreeView.Sort();
                        tableTreeView.ExpandAll();
                        if (systemDatabasesNode != null && userDatabasesNode != null) {
                            systemDatabasesNode.Collapse(false);
                        }
                        tableTreeView.Nodes[0].EnsureVisible();
                        tableTreeView.ResumeLayout();
                        tableTreeView.EndUpdate();
                    }
                }
            }
        }

        // Check all the children and subchildren of the node.
        private void CheckChildren(TreeNode node, bool checkState) {
            foreach (TreeNode subnode in node.Nodes) {
                subnode.Checked = checkState;
                CheckChildren(subnode, checkState);
            }
        }

        // Uncheck all of the node's ancestors.
        private void UncheckParent(TreeNode node) {
            if (node.Parent != null) {
                node.Parent.Checked = false;
                UncheckParent(node.Parent);
            }
        }

        // Check the node's parent if all siblings are already checked.
        private void MaybeCheckParent(TreeNode node) {
            if (node.Parent != null && AllChildrenAreChecked(node.Parent)) {
                node.Parent.Checked = true;
                MaybeCheckParent(node.Parent);
            }
        }

        // Check the node if all of its children are already checked.
        private bool AllChildrenAreChecked(TreeNode node) {
            if (node == null) return false;

            foreach (TreeNode child in node.Nodes) {
                if (!child.Checked) return false;
            }

            return true;
        }

        private IEnumerable<Triple<string,string,bool>> EnumCheckedTables(TreeNodeCollection nodes) {
            foreach (TreeNode node in nodes) {
                // Only nodes with non-null tags represent tables.
                if (node.Tag != null && node.Checked) {
                    yield return (Triple<string, string, bool>)node.Tag;
                } else {
                    // Look in the node's children.
                    foreach (Triple<string, string, bool> t in EnumCheckedTables(node.Nodes)) {
                        yield return t;
                    }
                }
            }
        }

        // When a node is checked/unchecked, check/uncheck all its children.
        // When a node is unchecked, uncheck its parent .
        private void tableTreeView_AfterCheck(object sender, TreeViewEventArgs e) {
            if (e.Action == TreeViewAction.Unknown) {
                // Likely a recursive call.
                return;
            }

            //Debug.Print("tableTreeView_AfterCheck");
            tableTreeView.BeginUpdate();
            if (e.Node.Checked) {
                CheckChildren(e.Node, true);
                MaybeCheckParent(e.Node);
            } else {
                CheckChildren(e.Node, false);
                UncheckParent(e.Node);
            }
            tableTreeView.EndUpdate();

            // Enable the OK button if any node representing a table is checked.
            okButton.Enabled = false;
            foreach (object o in EnumCheckedTables(tableTreeView.Nodes)) {
                okButton.Enabled = true;
                break;
            }
        }

        private void okButton_Click(object sender, EventArgs e) {
            _checkedTables = new List<Triple<string, string, bool>>();

            if (tableTreeView.CheckBoxes) {
                foreach (Triple<string, string, bool> table in EnumCheckedTables(tableTreeView.Nodes)) {
                    _checkedTables.Add(table);
                }
            }
        }
            
        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
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