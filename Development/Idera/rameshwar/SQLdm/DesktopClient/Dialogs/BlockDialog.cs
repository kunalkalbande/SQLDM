//-----------------------------------------------------------------------
// <copyright file="BlockDialog.cs" company="Idera Technologies, Inc.">
//     Copyright (C) BBS Technologies, Inc..  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    using System.Collections.Specialized;
    using System.Configuration;
    using Controls;
    using Helpers;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.UI.Dialogs;
    using Infragistics.Win;
    using Infragistics.Win.UltraWinDataSource;
    using Infragistics.Win.UltraWinGrid;
    using Infragistics.Win.UltraWinToolbars;
    using Idera.SQLdm.DesktopClient.Objects;
    using Properties;
    using Wintellect.PowerCollections;

    public partial class BlockDialog : Form
    {
        private const string BAND_RESOURCE = "Resource";
        private const string LOCK_TYPE = "Lock Type";
        private const string WAIT_RESOURCE = "Wait Resource";
        private const string OWN_WAIT = "Own/Wait";
        private const string MODE = "Mode";
        private const string SPID = "SPID";
        private const string ECID = "ECID";
        private const string DATABASE = "Database";
        private const string PROCEDURE = "Procedure";
        private const string SQL = "Sql";
        private const string APPLICATION = "Application";
        private const string LINE = "Line";
        private const string VICTIM = "Victim";
        private const string BLOCK_PROCESS = "BlockProcess";

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("BlockDialog");

        private string blockXML;
        private UltraGridColumn selectedColumn;
        private Control focused = null;

        public BlockDialog(string argBlockXML)
        {
            this.blockXML = argBlockXML;
  
            // testing override
            // this.xdl = System.IO.File.ReadAllText("C:\\Documents and Settings\\KGOOLSBEE\\My Documents\\deadlock-korean.xdl");

            InitializeComponent();
            AdaptFontSize();
        }

        private void BlockDialog_Load(object sender, EventArgs args)
        {
            Form owner = this.Owner;
            if (owner != null)
                this.Icon = owner.Icon;

            try
            {
                BlockData blockData = null;

                blockData = BlockData.FromXML(blockXML);
                //UltraDataSource table = FlattenBlockData(blockData);
                
                UpdateBlockedProcess(blockData.Blocked.Blocked);
                UpdateBlockingProcess(blockData.Blocking.Blocking);

            } catch (Exception e)
            {
                Size = new Size(525, 200);
                noDataLabel.Text = "There was an error trying to parse the block xml document.  You can still use the Export XML button to save the document to a file for viewing with SQL Server Management Studio or SQL Server Profiler.";
                splitContainer1.Panel2Collapsed = true;
                LOG.Error("Error parsing XML: ", e);
                LOG.Verbose("Goofed up XML: ", blockXML);
            }

            splitContainer1.SplitterDistance = Settings.Default.DeadlockViewSplitterDistance;
            Settings.Default.SettingChanging += Settings_SettingChanging;
            //ApplySettings();
        }

        private void ConfigureDetailsPanelExpanded(bool expanded)
        {
            splitContainer1.Panel2Collapsed = !expanded;            
        }

        private void Settings_SettingChanging(object sender, SettingChangingEventArgs e)
        {
            switch (e.SettingName)
            {
                case "DeadlockViewDetailsPanelExpanded":
                    ConfigureDetailsPanelExpanded((bool)e.NewValue);
                    break;
            }
        }

////        public void AddProcessInformation(UltraDataRow row, BlockProcess process)
////        {
////            row[BLOCK_PROCESS] = process;
////            //table.Columns.Add("Host", typeof(string));
////            row["Host"] = (object)process.hostname ?? DBNull.Value;

////            row["Isolation Level"] = (object)process.isolationlevel ?? DBNull.Value;
////            row["Lock Mode"] = (object)process.lockMode ?? DBNull.Value;
////            row["Lock Timeout"] = (object)process.lockTimeout ?? DBNull.Value;
////            row["Login"] = (object)process.loginname ?? DBNull.Value;
////            row["Priority"] = (object)process.priority ?? DBNull.Value;
////            row["Status"] = (object)process.status ?? DBNull.Value;
////            row["Transaction Name"] = (object)process.transactionname ?? DBNull.Value;
//////            row["Wait Resource"] = (object)process.waitresource ?? DBNull.Value;
////            row["Wait Time"] = (object)process.waittime ?? DBNull.Value;
////            row[VICTIM] = process.Victim;
////            if (String.IsNullOrEmpty(process.spid))
////                row[SPID] = DBNull.Value;
////            else
////                row[SPID] = Int32.Parse(process.spid);
////            if (String.IsNullOrEmpty(process.ecid))
////                row[ECID] = DBNull.Value;
////            else
////                row[ECID] = Int32.Parse(process.ecid);

////            row[APPLICATION] = process.clientapp;

////            // Set process sql = inputbuf.  If inputbuf is empty then use last stack frame.  if still empty then use spaces.
////            string inputbuf = process.inputbuf;
////            if (!String.IsNullOrEmpty(inputbuf))
////            {
////                row["Procedure"] = String.Empty;
////                row["Line"] = DBNull.Value;
////                row[SQL] = FixupLineBreaks(inputbuf ?? String.Empty);
////            } else
////            {
////                BlockExecutionStackFrame frame = GetLastStackFrame(process);
////                if (frame == null || String.IsNullOrEmpty(frame.Value))
////                {
////                    row["Procedure"] = String.Empty;
////                    row["Line"] = DBNull.Value;
////                    row[SQL] = String.Empty;
////                }
////                else
////                {
////                    row["Procedure"] = (object)frame.procname ?? DBNull.Value;
////                    if (String.IsNullOrEmpty(frame.line))
////                        row["Line"] = DBNull.Value;
////                    else
////                        row["Line"] = Int32.Parse(frame.line);

////                    if (!String.IsNullOrEmpty(frame.Value))
////                        row[SQL] = FixupLineBreaks(frame.Value.TrimStart('\n'));
////                    else
////                        row[SQL] = String.Empty;
////                }
////            }

////            if (process.executionStack != null)
////            {
////                // add all the stack frames as child rows
////                UltraDataRowsCollection frames = row.GetChildRows("Frame");
////                foreach (BlockExecutionStackFrame f in process.executionStack)
////                {
////                    UltraDataRow frow = frames.Add();
////                    frow["Procedure"] = (object)f.procname ?? DBNull.Value;
////                    if (String.IsNullOrEmpty(f.line))
////                        frow["Line"] = DBNull.Value;
////                    else
////                        frow["Line"] = Int32.Parse(f.line);

////                    if (!String.IsNullOrEmpty(f.Value))
////                        frow[SQL] = FixupLineBreaks(f.Value.TrimStart('\n'));
////                    else
////                        frow[SQL] = String.Empty;
////                }
////            }
////        }

//        //private void AddResourceInformation(UltraDataRow row, DeadlockProcess process, DeadlockResource resource)
//        //{

//        //    string resourceType = resource.ResourceType;
//        //    string waitResource = process.waitresource ?? String.Empty;

//        //    switch (resource.ResourceType)
//        //    {
//        //        case "pagelock":
//        //            resourceType = "Page";
//        //            waitResource = FormatPageId(resource, waitResource);
//        //            break;
//        //        case "keylock":
//        //            resourceType = "Key";
//        //            waitResource = FormatKeyId(resource, waitResource);
//        //            break;
//        //        case "ridlock":
//        //            resourceType = "RID";
//        //            break;
//        //        case "objectlock":
//        //            resourceType = "Object";
//        //            waitResource = FormatObjectId(resource, waitResource);
//        //            break;
//        //        case "hobtlock":
//        //            resourceType = "HOBT";
//        //            waitResource = FormatHobtId(resource, waitResource);
//        //            break;
//        //        case "databaselock":
//        //            resourceType = "DATABASE";
//        //            waitResource = FormatDatabaseId(resource, waitResource);
//        //            break;
//        //        case "applicationlock":
//        //            resourceType = "APPLICATION";
//        //            waitResource = FormatAppId(resource, waitResource);
//        //            break;
//        //        case "exchangeEvent":
//        //            resourceType = "Event";
//        //            break;
//        //        case "threadpool":
//        //            resourceType = "Thread";
//        //            break;
//        //        default:
//        //            break;
//        //    }
//        //    row[LOCK_TYPE] = resourceType;
//        //    row["Wait Resource"] = waitResource;

//        //    AddDynamicProperties(row, resource);
//        //}

//        private string FormatAppId(DeadlockResource resource, string defaultValue)
//        {
//            bool complete = true;
//            string dbid = GetDynamicProperty(resource, "dbid");
//            if (String.IsNullOrEmpty(dbid))
//                complete = false;
//            string pid = GetDynamicProperty(resource, "databasePrincipalId");
//            if (String.IsNullOrEmpty(pid))
//                complete = false;
//            string hash = GetDynamicProperty(resource, "hash");
//            if (String.IsNullOrEmpty(hash))
//                complete = false;

//            if (!complete)
//                return defaultValue;

//            return String.Format("APPLICATION: {0}:{1}:{2}", dbid, pid, hash);
//        }

        //private string FormatDatabaseId(DeadlockResource resource, string defaultValue)
        //{
        //    bool complete = true;
        //    string dbid = GetDynamicProperty(resource, "dbid");
        //    if (String.IsNullOrEmpty(dbid))
        //        complete = false;

        //    if (!complete)
        //        return defaultValue;

        //    return String.Format("DATABASE: {0}", dbid);
        //}

        //private string FormatHobtId(DeadlockResource resource, string defaultValue)
        //{
        //    bool complete = true;
        //    string dbid = GetDynamicProperty(resource, "dbid");
        //    if (String.IsNullOrEmpty(dbid))
        //        complete = false;
        //    string hobt = GetDynamicProperty(resource, "hobtid");
        //    if (String.IsNullOrEmpty(hobt))
        //        complete = false;

        //    if (!complete)
        //        return defaultValue;

        //    return String.Format("HOBT: {0}:{1}", dbid, hobt);
        //}

        //private string FormatPageId(DeadlockResource resource, string defaultValue)
        //{
        //    bool complete = true;
        //    string dbid = GetDynamicProperty(resource, "dbid");
        //    if (String.IsNullOrEmpty(dbid))
        //        complete = false;
        //    string file = GetDynamicProperty(resource, "fileid");
        //    if (String.IsNullOrEmpty(file))
        //        complete = false;
        //    string page = GetDynamicProperty(resource, "pageid");
        //    if (String.IsNullOrEmpty(page))
        //        complete = false;

        //    if (!complete)
        //        return defaultValue;

        //    return String.Format("PAGE: {0}:{1}:{2}", dbid, file, page);
        //}

        //private string FormatKeyId(DeadlockResource resource, string defaultValue)
        //{
        //    bool complete = true;
        //    string dbid = GetDynamicProperty(resource, "dbid");
        //    if (String.IsNullOrEmpty(dbid))
        //        complete = false;
        //    string hobtid = GetDynamicProperty(resource, "hobtid");
        //    if (String.IsNullOrEmpty(hobtid))
        //        complete = false;

        //    if (!complete)
        //        return defaultValue;

        //    return String.Format("KEY: {0}:{1}", dbid, hobtid);
        //}

        ////private string FormatObjectId(DeadlockResource resource, string defaultValue)
        ////{
        ////    bool complete = true;
        ////    string dbid = GetDynamicProperty(resource, "dbid");
        ////    if (String.IsNullOrEmpty(dbid))
        ////        complete = false;
        ////    string objid = GetDynamicProperty(resource, "objid");
        ////    if (String.IsNullOrEmpty(objid))
        ////        complete = false;

        ////    if (!complete)
        ////        return defaultValue;

        ////    return String.Format("OBJECT: {0}:{1}", dbid, objid);
        ////}


        //private string GetDynamicProperty(IDynamicPropertyProvider propProvider, string key)
        //{
        //    if (propProvider == null)
        //        return null;
            
        //    NameValueCollection values = propProvider.DynamicProperties;
        //    if (values != null)
        //        return values.Get(key);

        //    return null;
        //}

        //private void AddDynamicProperties(UltraDataRow row, IDynamicPropertyProvider propProvider)
        //{
        //    if (propProvider == null || propProvider.DynamicProperties == null)
        //        return;
            
        //    foreach (string key in propProvider.DynamicProperties.AllKeys)
        //    {
        //        string value = propProvider.DynamicProperties[key];
        //        if (row.Band.Columns.Contains(key))
        //            row[key] = value;
        //    }
        //}

        //public static BlockExecutionStackFrame GetFirstStackFrame(BlockProcess process)
        //{
        //    BlockExecutionStackFrame[] stackFrames = process.executionStack;
        //    if (stackFrames == null || stackFrames.Length == 0)
        //        return null;

        //    return stackFrames[0];
        //}

        //public static BlockExecutionStackFrame GetLastStackFrame(BlockProcess process)
        //{
        //    BlockExecutionStackFrame[] stackFrames = process.executionStack;
        //    if (stackFrames == null || stackFrames.Length == 0)
        //        return null;

        //    return stackFrames[stackFrames.Length - 1];
        //}

        //public void AddDynamicColumns(UltraDataSource ds, BlockData data)
        //{
        //    foreach (Deadlock deadlock in data.Items)
        //    {
        //        DeadlockResourceList resourceList = deadlock.resourceList;
        //        if (resourceList != null)
        //        {
        //            foreach (DeadlockResource resource in resourceList.ResourceList)
        //            {
        //                // add a column for each dynamic property in the resource
        //                NameValueCollection props = resource.DynamicProperties;
        //                if (props != null && props.Count > 0)
        //                {
        //                    foreach (string key in props.AllKeys)
        //                    {
        //                        if (!ds.Band.Columns.Exists(key))
        //                        {
        //                            ds.Band.Columns.Add(key);
        //                        }
        //                    }
        //                }

        //                // add a column for each dynamic property in the owners
        //                foreach (DeadlockResourceOwner owner in resource.ownerlist)
        //                {
        //                    props = owner.DynamicProperties;
        //                    if (props != null && props.Count > 0)
        //                    {
        //                        foreach (string key in props.AllKeys)
        //                        {
        //                            if (!ds.Band.Columns.Exists(key))
        //                            {
        //                                ds.Band.Columns.Add(key);
        //                            }
        //                        }
        //                    }
        //                }
        //                // add a column for each dynamic property in the waiters
        //                foreach (DeadlockResourceWaiter waiter in resource.waiterlist)
        //                {
        //                    props = waiter.DynamicProperties;
        //                    if (props != null && props.Count > 0)
        //                    {
        //                        foreach (string key in props.AllKeys)
        //                        {
        //                            if (!ds.Band.Columns.Exists(key))
        //                            {
        //                                ds.Band.Columns.Add(key);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        private string FixupLineBreaks(string input)
        {
            StringBuilder result = new StringBuilder();
            
            char pch = ' ';
            foreach (char ch in input)
            {
                if (ch == '\n' && pch != '\r')
                {   // make sure all \n are preceeded with a \r
                    result.Append('\r');
                }
                if (ch == '\t')
                {   // expand tabs
                    result.Append("   ");
                    pch = ' ';
                    continue;
                }
                result.Append(ch);
                pch = ch;
            }
           
            // strip all leading control characters
            while (result.Length > 0 && Char.IsControl(result[0]))
                result.Remove(0, 1);

            return result.ToString();
        }

        internal static void Show(IWin32Window owner, long alertId)
        {
            try
            {
                int serverID;
                DateTime collected;
                string blockXML;

                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                if (!RepositoryHelper.GetLinkedBlockingAlertData(connectionInfo, alertId, out serverID, out collected, out blockXML) || blockXML == null)
                {
                    ApplicationMessageBox.ShowError(owner, "The linked block graph is not available in the SQLDM Repository. Enable capture of the blocked process report by enabling blocking on the Activity Monitor tab of the SQL Server properties.");
                    return;
                }

                MonitoredSqlServerWrapper mssw = ApplicationModel.Default.ActiveInstances[serverID];

                try
                {
                    BlockDialog dialog = new BlockDialog(blockXML);
                    if (mssw != null)
                    {
                        dialog.Text = String.Format("Block - {0} at {1}", mssw.InstanceName, collected.ToLocalTime());
                    }
                    else
                        dialog.Text = String.Format("Block - {0}", collected.ToLocalTime());

                    dialog.Show(owner);
                }
                catch (Exception)
                {
                    
                }
            } catch (Exception)
            {
                
            }
        }

        internal static void Show(IWin32Window owner, DateTime collected, string bXML)
        {
            try
            {
                BlockDialog dialog = new BlockDialog(bXML);
                dialog.Text = String.Format("Block - {0}", collected.ToLocalTime());
                dialog.Show(owner);
            }
            catch (Exception e)
            {
                LOG.Error("Error Showing the Block Report: ", e);
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            ExportHelper.ExportBlockGraph(this, blockXML, "Block.xml");
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private static Control GetFocusedControl(Control.ControlCollection controls)
        {
            Control focusedControl = null;

            foreach (Control control in controls)
            {
                if (control.Focused)
                {
                    focusedControl = control;
                }
                else if (control.ContainsFocus)
                {
                    return GetFocusedControl(control.Controls);
                }
            }

            return focusedControl != null ? focusedControl : controls[0];
        }

        private void splitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private void BlockDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            //SaveSettings();
        }

        public void SaveSetting()
        {
            Settings.Default.BlockViewSplitterDistance = splitContainer1.SplitterDistance;
        }

        private void UpdateBlockingProcess(BlockProcess process)
        {
            //BlockProcess process = null;
            //if (row != null)
            //{
            //    UltraDataRow parentRow = row;
            //    if (row.Band.Key != BAND_RESOURCE)
            //    {
            //        parentRow = row.ParentRow;
            //    }

            //    process = parentRow[BLOCK_PROCESS] as DeadlockProcess;
            //}

            if (process == null)
            {
                ClearBlockingProcess();
                return;
            }

            lblBoundBlockingSpid.Text = process.spid;
            lblBoundBlockingUser.Text = process.loginname;
            lblBoundBlockingHost.Text = process.hostname;
            lblBoundBlockingDatabase.Text = process.Database;

            lblBoundBlockingStatus.Text = process.status;
            lblBoundBlockingOpenTrans.Text = process.trancount;
            lblBoundBlockingApplication.Text = process.clientapp;
            lblBoundBlockingExecutionContext.Text = process.ecid;
            //lblBoundBlockingWaitTime.Text = String.Format("{0}", process.waittime);
            lblBoundBlockingWaitType.Text = process.status;
            //lblBoundBlockingWaitResource.Text = process.waitresource;

            lblBoundBlockingLastBatchCompleted.Text = FormatXmlDateTime(process.lastbatchcompleted);
            lblBoundBlockingLastBatchStarted.Text = FormatXmlDateTime(process.lastbatchstarted);

            lblBoundBlockingTranName.Text = process.transactionname;
            lblBoundBlockingTranId.Text = process.xactid;

            lblBlockingLastCommand.Text = (FixupLineBreaks(process.inputbuf)) ?? String.Empty;
        }

        private void UpdateBlockedProcess(BlockProcess process)
        {
            //BlockProcess process = null;
            //if (row != null)
            //{
            //    UltraDataRow parentRow = row;
            //    if (row.Band.Key != BAND_RESOURCE)
            //    {
            //        parentRow = row.ParentRow;
            //    }

            //    process = parentRow[BLOCK_PROCESS] as DeadlockProcess;
            //}

            if (process == null)
            {
                ClearBlockedProcess();
                return;
            }

            boundSpidLabel.Text = process.spid;
            boundUserLabel.Text = process.loginname;
            boundHostLabel.Text = process.hostname;
            boundDatabaseLabel.Text = process.Database;

            boundStatusLabel.Text = process.status;
            boundOpenTransactionsLabel.Text = process.trancount;
            boundApplicationLabel.Text = process.clientapp;
            boundExecutionContextLabel.Text = process.ecid;
            boundWaitTimeLabel.Text = String.Format("{0}", process.waittime);
            boundWaitTypeLabel.Text = process.status;
            boundWaitResourceLabel.Text = process.waitresource;

            boundBatchCompleteLabel.Text = FormatXmlDateTime(process.lastbatchcompleted);
            boundBatchStartLabel.Text = FormatXmlDateTime(process.lastbatchstarted);
            boundLastTransLabel.Text = FormatXmlDateTime(process.lasttranstarted);

            boundTransNameLabel.Text = process.transactionname;
            boundXactIdLabel.Text = process.xactid;

            boundLastCommandTextBox.Text = (FixupLineBreaks(process.inputbuf)) ?? String.Empty;
        }

        private static string FormatXmlDateTime(string p)
        {
            String result = String.Empty;
            try
            {
                DateTime dt = Convert.ToDateTime(p);
                result = String.Format("{0}", dt);
            } catch (Exception)
            {
            }

            if (String.IsNullOrEmpty(result))
                result = p;

            return result ?? String.Empty;
        }

        private void ClearBlockedProcess()
        {
            boundSpidLabel.Text = String.Empty;
            boundUserLabel.Text = String.Empty;
            boundHostLabel.Text = String.Empty;
            boundDatabaseLabel.Text = String.Empty;
            boundStatusLabel.Text = String.Empty;
            boundOpenTransactionsLabel.Text = String.Empty;
            boundApplicationLabel.Text = String.Empty;
            boundExecutionContextLabel.Text = String.Empty;
            boundWaitTimeLabel.Text = String.Empty;
            boundWaitTypeLabel.Text = String.Empty;
            boundWaitResourceLabel.Text = String.Empty;
            boundLastCommandTextBox.Text = String.Empty;

            boundTransNameLabel.Text = String.Empty;
            boundXactIdLabel.Text = String.Empty;

            boundBatchCompleteLabel.Text = String.Empty;
            boundBatchStartLabel.Text = String.Empty;
            boundLastTransLabel.Text = String.Empty;

        }
        private void ClearBlockingProcess()
        {
            lblBoundBlockingSpid.Text = String.Empty;
            lblBoundBlockingUser.Text = String.Empty;
            lblBoundBlockingHost.Text = String.Empty;
            lblBoundBlockingDatabase.Text = String.Empty;
            lblBoundBlockingStatus.Text = String.Empty;
            lblBoundBlockingOpenTrans.Text = String.Empty;
            lblBoundBlockingApplication.Text = String.Empty;
            lblBoundBlockingExecutionContext.Text = String.Empty;
            //lblBoundBlockingWaitTime.Text = String.Empty;
            lblBoundBlockingWaitType.Text = String.Empty;
            //lblBoundBlockingWaitResource.Text = String.Empty;
            lblBlockingLastCommand.Text = String.Empty;

            lblBoundBlockingTranName.Text = String.Empty;
            lblBoundBlockingTranId.Text = String.Empty;

            lblBoundBlockingLastBatchCompleted.Text = String.Empty;
            lblBoundBlockingLastBatchStarted.Text = String.Empty;
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