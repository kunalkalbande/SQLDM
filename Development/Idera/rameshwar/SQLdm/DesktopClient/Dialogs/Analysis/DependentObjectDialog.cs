using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Infragistics.Win.UltraWinTree;
using System.Diagnostics;
using System.Collections;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
//------------------------------------------------------------------------------
// <copyright file="DependentObjectDialog.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// Author : Srishti Purohit
// Date : 21Aug2015
// Release : SQLdm 10.0
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class DependentObjectDialog : Form
    {
        private readonly TracerX.Logger _logX = TracerX.Logger.GetLogger(typeof(DependentObjectDialog));

        public DatabaseObjectName ObjectName;
        private int instanceID;

        internal DependentObjectDialog(int instanceId)
        {
            using (_logX.DebugCall("DependentObjectDialog()"))
            {
                try
                {
                    InitializeComponent();
                    this.instanceID = instanceId;
                }
                catch (Exception ex)
                {
                    _logX.Error("DependentObjectDialog() Exception:", ex);
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void loadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (_logX.DebugCall("loadWorker_DoWork"))
            {
                try
                {
                    BackgroundWorker worker = (BackgroundWorker)sender;
                    if (null == ObjectName) return;

                    DataTable table = null;
                    Idera.SQLdm.Common.Services.IManagementService managementService =
                          Idera.SQLdm.DesktopClient.Helpers.ManagementServiceHelper.GetDefaultService(
                              Idera.SQLdm.DesktopClient.Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    var snapshot = managementService.GetTableDependentObjects(instanceID, ObjectName);
                    if (snapshot != null && snapshot.Error != null)
                    {
                        ApplicationMessageBox.ShowError(this,
                                                              "Error fetching dependent objects",
                                                              snapshot.Error);
                    }
                    else if (snapshot != null)
                    {
                        table = snapshot.DependentObject;
                        if (table == null)
                            table = new DataTable();
                    }
                    _logX.Debug("adapter.Fill() Begin...");
                    _logX.Debug("adapter.Fill() Complete.");

                    if (null != table.Rows) _logX.DebugFormat("table.Rows:{0}", table.Rows.Count);

                    if (worker.CancellationPending)
                        e.Cancel = true;
                    else
                        e.Result = table;
                    _logX.Debug("Complete background worker.");

                    if (worker.CancellationPending)
                        e.Cancel = true;
                    else
                        e.Result = table;
                    _logX.Debug("Complete background worker.");
                }
                catch (Exception ex)
                {
                    e.Result = ex;
                    _logX.Error("loadWorker_DoWork Exception:", ex);
                }
            }
        }

        private void loadWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            using (_logX.DebugCall("loadWorker_RunWorkerCompleted"))
            {
                if (IsDisposed) return;
                if (e.Cancelled) return;
                try
                {
                    Exception ex = e.Result as Exception;
                    if (null == ex) ex = e.Error;
                    if (ex != null)
                    {
                        ApplicationMessageBox.ShowError(this, "Error trying to retrieve object dependency information.", ex);
                        return;
                    }

                    dependencyTree.Override.Sort = SortType.Ascending;

                    IComparer comparer = dependencyTree.Override.SortComparer;
                    dependencyTree.Override.SortComparer = null;
                    try
                    {
                        DataTable table = e.Result as DataTable;
                        if (table != null)
                        {
                            _logX.Debug("Load nodeMap begin...");
                            Dictionary<int, UltraTreeNode> nodeMap = new Dictionary<int, UltraTreeNode>();
                            foreach (DataRow row in table.Rows)
                            {
                                int nodeId = (int)row["object_id"];

                                if (!nodeMap.ContainsKey(nodeId))
                                {
                                    UltraTreeNode upNode = new UltraTreeNode(null, (string)row["object_name"]);
                                    nodeMap.Add(nodeId, upNode);
                                }
                            }
                            _logX.Debug("Load nodeMap complete.");

                            _logX.Debug("Load relationMap begin...");
                            Dictionary<int, List<int>> relationMap = new Dictionary<int, List<int>>();
                            foreach (DataRow row in table.Rows)
                            {
                                if (row.IsNull("relative_id"))
                                    continue;

                                int parentId = (int)row["object_id"];
                                int childId = (int)row["relative_id"];

                                List<int> parentList;
                                if (!relationMap.TryGetValue(childId, out parentList))
                                {
                                    parentList = new List<int>();
                                    parentList.Add(parentId);
                                    relationMap.Add(childId, parentList);
                                }
                                else
                                    if (!parentList.Contains(parentId))
                                        parentList.Add(parentId);
                            }
                            _logX.Debug("Load relationMap complete.");

                            _logX.Debug("Clean up child relation mapping begin...");
                            foreach (int childId in relationMap.Keys)
                            {
                                UltraTreeNode child;
                                if (nodeMap.TryGetValue(childId, out child))
                                {
                                    foreach (int parentId in relationMap[childId])
                                    {
                                        UltraTreeNode parent;
                                        if (nodeMap.TryGetValue(parentId, out parent))
                                        {
                                            if (child.Parent == null)
                                                AddChild(parent, child);
                                            else
                                            {
                                                child = (UltraTreeNode)child.Clone();
                                                parent.Nodes.Add(child);
                                            }
                                        }
                                    }
                                }
                            }
                            _logX.Debug("Clean up child relation mapping complete.");

                            // add all nodes that have no parents to the tree
                            _logX.Debug("Add parentless nodes begin.");
                            foreach (UltraTreeNode node in nodeMap.Values)
                            {
                                if (node.Parent == null)
                                {
                                    dependencyTree.Nodes.Add(node);
                                    DumpNode(node);
                                }
                            }
                            _logX.Debug("Add parentless nodes complete...");
                        }
                    }
                    finally
                    {
                        dependencyTree.Override.SortComparer = comparer;
                        loadingCircle.Active = false;
                        dependencyTree.Visible = true;
                        dependencyTree.BringToFront();
                    }
                }
                catch (Exception ex)
                {
                    _logX.Error("loadWorker_RunWorkerCompleted() Exception:", ex);
                }
            }
        }

        private void AddChild(UltraTreeNode parent, UltraTreeNode child)
        {
            bool recursive = false;
            int depth = 0;
            UltraTreeNode node = parent;
            while (null != node)
            {
                ++depth;
                if (node == child)
                {
                    recursive = true;
                    _logX.DebugFormat("Recursive child node being added Child:{0} Parent:{1}", child, parent);
                    break;
                }
                node = node.Parent;
            }
            if (depth > 1000)
            {
                _logX.DebugFormat("Too deep! Child:{0} Parent:{1} Depth:{2}", child, parent, depth);
                return;
            }
            if (!recursive) parent.Nodes.Add(child);
            else
            {
                child = (UltraTreeNode)child.Clone();
                parent.Nodes.Add(child);
            }
        }

        static int indent = 0;
        private void DumpNode(UltraTreeNode node)
        {
            indent += 4;
            string pad = String.Empty.PadLeft(indent);
            Debug.Print("{0}Node: {1}", pad, node.Text);
            foreach (UltraTreeNode child in node.Nodes)
            {
                DumpNode(child);
            }
            indent -= 4;
        }

        private void DependentObjectDialog_Load(object sender, EventArgs e)
        {
            using (_logX.DebugCall("DependentObjectDialog_Load()"))
            {
                textBox1.Text = ObjectName.ToString();
                loadingCircle.Active = true;
                if (!loadWorker.IsBusy) loadWorker.RunWorkerAsync();
            }
        }
    }
}
