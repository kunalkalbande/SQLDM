using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using Infragistics.Windows.Themes;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class SessionTraceDialog : BaseDialog
    {
        #region constants

        private enum StatusPanel
        {
            Context,
            TraceStatus,
            TraceItems,
            RefreshStatus
        }

        private const string TITLEBAR_MSG = "SPID {0} - Session Trace";
        private const string INSTANCE_SESSION_MSG = "{0} Session ID {1}";
        private const string REFRESHING_MSG = @"Refreshing...";
        private const string REFRESHED_MSG = "Refreshed: {0}";
        private const string REFRESH_ERROR_MSG = @"Refresh Error - click here";
        private const string TRACE_STARTED_MSG = @"Trace Started";
        private const string TRACE_STOPPED_MSG = @"Trace Stopped";
        private const string ITEM_COUNT_MSG = "{0} Item{1}";

        private static Color OK_BACKCOLOR = Color.White;
        private static Color WARN_BACKCOLOR = Color.Yellow;
        private static Color ALERT_BACKCOLOR = Color.Red;

        #endregion

        #region fields

        private readonly int instanceId;
        private SessionDetailsConfiguration configurationDetails;
        private int selectedSpid;

        private bool initialized = false;
        private UltraGridColumn selectedColumn = null;
        private static readonly object updateLock = new object();
        private BackgroundWorker refreshBackgroundWorker = null;
        private Exception refreshError = null;

        private bool _traceOn;
        private bool traceOn
        {
            get { return _traceOn; }
            set
            {
                _traceOn = value;

                // fix the buttons to reflect the current state and only enable the correct ones.
                startTraceToolStripButton.Enabled = !_traceOn;
                stopTraceToolStripButton.Enabled = _traceOn;
            }
        }
        private bool formClosing = false;

        //last Settings values used to determine if changed for saving when leaving
        private GridSettings lastMainGridSettings = null;

        #endregion

        #region constructors

        public SessionTraceDialog(int instanceId, int sessionId)
        {
            this.DialogHeader = "Session Trace";
            InitializeComponent();
            Icon = Properties.Resources.SessionTraceIcon;
            traceGrid.DrawFilter = new HideFocusRectangleDrawFilter();

            this.instanceId = instanceId;
            this.selectedSpid = sessionId;
            configurationDetails = new SessionDetailsConfiguration(instanceId, selectedSpid, traceOn);
            traceOn = true;
            InitializeRefreshBackgroundWorker();

            this.Text = String.Format(TITLEBAR_MSG, selectedSpid.ToString());

            statusBar.Panels[(int)StatusPanel.Context].Text = String.Format(INSTANCE_SESSION_MSG,
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        selectedSpid.ToString());
            statusBar.Panels[(int)StatusPanel.TraceStatus].Text = TRACE_STARTED_MSG;

            // Start the trace
            StartTrace();
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        #endregion

        #region helpers

        private void ApplySettings()
        {
            if (Settings.Default.SessionTraceDialogMainGrid is GridSettings)
            {
                lastMainGridSettings = Settings.Default.SessionTraceDialogMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, traceGrid);
            }
        }

        private void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(traceGrid);
            // save all settings only if anything has changed
            if (!mainGridSettings.Equals(lastMainGridSettings))
            {
                lastMainGridSettings =
                    Settings.Default.SessionTraceDialogMainGrid = mainGridSettings;
            }
        }

        #region refresh
        // the refresh of this screen is independent of the rest of the app
        // but I've tried to use the same code and names wherever possible to be consistent
        private void InitializeRefreshBackgroundWorker()
        {
            refreshBackgroundWorker = new BackgroundWorker();
            refreshBackgroundWorker.WorkerSupportsCancellation = true;
            refreshBackgroundWorker.DoWork += new DoWorkEventHandler(refreshBackgroundWorker_DoWork);
            refreshBackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(refreshBackgroundWorker_RunWorkerCompleted);
        }

        private void RefreshView()
        {
            statusBar.Panels[(int)StatusPanel.RefreshStatus].Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            statusBar.Panels[(int)StatusPanel.RefreshStatus].Text = REFRESHING_MSG;

            if (refreshBackgroundWorker == null)
            {
                InitializeRefreshBackgroundWorker();
            }

            if (!refreshBackgroundWorker.IsBusy)
            {
                refreshBackgroundWorker.RunWorkerAsync();
            }
        }

        private void CancelRefresh()
        {
            if (refreshBackgroundWorker != null)
            {
                refreshBackgroundWorker.CancelAsync();
                refreshBackgroundWorker = null;
            }
            else
            {

            }
        }

        private object DoRefreshWork()
        {
            try
            {
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                configurationDetails.TraceOn = traceOn;
                Snapshot snapshot = managementService.GetSessionDetails(configurationDetails);

                if (!traceOn)
                {
                    managementService.SendStopSessionDetailsTrace(new StopSessionDetailsTraceConfiguration(configurationDetails));
                }
                return snapshot;
            }
            catch (Exception e)
            {
                return e;
            }
        }

        private void UpdateData(object data)
        {
            Exception e = null;

            Cursor = Cursors.WaitCursor;

            if (data != null && data is SessionDetailSnapshot)
            {
                lock (updateLock)
                {
                    SessionDetailSnapshot snapshot = (SessionDetailSnapshot)data;

                    if (snapshot.Error == null)
                    {
                        killSessionToolStripButton.Enabled = !snapshot.Details.IsSystemProcess;

                        if (snapshot.Details.WaitType != null)
                        {
                            statusValueLabel.Text = String.Format("Waiting - {0}", snapshot.Details.WaitType);
                        }
                        else
                        {
                            statusValueLabel.Text = ApplicationHelper.GetEnumDescription(snapshot.Details.Status);
                        }

                        // first row
                        cpuTimeTextBox.Text = snapshot.Details.Cpu != null && snapshot.Details.Status != SessionStatus.Ended ? snapshot.Details.Cpu.TotalMilliseconds.ToString("N0") : null;
                        rowCountTextBox.Text = snapshot.Details.RowCount != null ? snapshot.Details.RowCount.Value.ToString("N0") : null;
                        readsTextBox.Text = snapshot.Details.Reads != null ? snapshot.Details.Reads.Value.ToString("N0") : null;
                        writesTextBox.Text = snapshot.Details.Writes != null ? snapshot.Details.Writes.Value.ToString("N0") : null;
                        cursorFetchStatusTextBox.Text = snapshot.Details.CursorFetchStatus != null ? ApplicationHelper.GetEnumDescription(snapshot.Details.CursorFetchStatus.Value) : null;
                        cursorSetRowsTextBox.Text = snapshot.Details.CursorSetRows != null ? snapshot.Details.CursorSetRows.Value.ToString("N0") : null;
                        openTransactionsTextBox.Text = snapshot.Details.OpenTransactions != null ? snapshot.Details.OpenTransactions.Value.ToString("N0") : null;

                        // second row
                        lastErrorNumberTextBox.Text = snapshot.Details.LastError != null ? snapshot.Details.LastError.Value.ToString() : null;
                        lineNumberTextBox.Text = snapshot.Details.LineNumber != null ? snapshot.Details.LineNumber.Value.ToString() : null;
                        textSizeTextBox.Text = snapshot.Details.TextSize != null ? snapshot.Details.TextSize.Value.ToString("N0") : null;
                        nestingLevelTextBox.Text = snapshot.Details.NestingLevel != null ? snapshot.Details.NestingLevel.Value.ToString("N0") : null;

                        string displayString = string.Empty;
                        if (snapshot.Details.LockWaitTimeout.HasValue)
                        {
                            if (snapshot.Details.LockWaitTimeout.Value.Days > 0)
                            {
                                displayString = string.Concat(snapshot.Details.LockWaitTimeout.Value.TotalDays.ToString("N1"), " Days");
                            }
                            else if (snapshot.Details.LockWaitTimeout.Value.Hours > 0)
                            {
                                displayString = string.Concat(snapshot.Details.LockWaitTimeout.Value.TotalHours.ToString("N1"), " Hours");
                            }
                            else if (snapshot.Details.LockWaitTimeout.Value.Minutes > 0)
                            {
                                displayString = string.Concat(snapshot.Details.LockWaitTimeout.Value.TotalMinutes.ToString("N1"), " Minutes");
                            }
                            else if (snapshot.Details.LockWaitTimeout.Value.Seconds > 0)
                            {
                                displayString = string.Concat(snapshot.Details.LockWaitTimeout.Value.TotalSeconds.ToString("N1"), " Seconds");
                            }
                            else if (snapshot.Details.LockWaitTimeout.Value.TotalMilliseconds > 0)
                            {
                                displayString = string.Concat(snapshot.Details.LockWaitTimeout.Value.TotalMilliseconds.ToString("N1"), " Milliseconds");
                            }
                        }
                        else if (snapshot.Details.Status != SessionStatus.Ended)
                        {
                            displayString = "N/A";
                        }
                        lockWaitTextBox.Text = displayString;
                        languageTextBox.Text = snapshot.Details.Language;
                        deadlockPriorityTextBox.Text = snapshot.Details.DeadlockPriorityName;

                        // third row
                        isolationLevelTextBox.Text = snapshot.Details.TransactionIsolationLevel != null ? ApplicationHelper.GetEnumDescription(snapshot.Details.TransactionIsolationLevel.Value) : null;
                        commandTextBox.Text = snapshot.Details.Command;

                        // fourth row
                        optionsTextBox.Text = snapshot.Details.Options.StatusList;

                        // fifth row
                        if (snapshot.Details.LastCommand != null)
                        {
                            lastCommandTextBox.Text = snapshot.Details.LastCommand;
                        }
                        else if (snapshot.Details.Status != SessionStatus.Ended)
                        {
                            lastCommandTextBox.Text = "<SQL Command not available>";
                        }

                        //Set visual color indicators based on values...
                        if (snapshot.Details.Status == SessionStatus.Ended)
                        {
                            statusPulseHeaderPanel.SetColorPalette(PulseHeaderPanelColorPalette.Red);
                        }
                        else if (snapshot.Details.WaitType != null)
                        {
                            statusPulseHeaderPanel.SetColorPalette(PulseHeaderPanelColorPalette.Yellow);
                        }
                        else
                        {
                            statusPulseHeaderPanel.ResetColors();
                        }

                        updateTextBoxColor(openTransactionsTextBox,
                                                snapshot.Details.OpenTransactions.HasValue
                                                && (long)snapshot.Details.OpenTransactions.Value > 0);

                        updateTextBoxColor(lastErrorNumberTextBox,
                                                snapshot.Details.LastError.HasValue
                                                && (long)snapshot.Details.LastError.Value > 0);

                        updateTextBoxColor(isolationLevelTextBox,
                                                snapshot.Details.TransactionIsolationLevel.HasValue
                                                && snapshot.Details.TransactionIsolationLevel == TransactionIsolation.read_uncommitted);

                        updateTextBoxColor(optionsTextBox,
                                                snapshot.Details.Options.IsInWarning);

                        // grid
                        traceGridDataSource.SuspendBindingNotifications();

                        configurationDetails.NextSequenceNumber += snapshot.TraceItems.Count;

                        foreach (TraceStatement trace in snapshot.TraceItems)
                        {
                            UltraDataRow newRow = traceGridDataSource.Rows.Add(
                                new object[]
                                        {
                                            trace.CompletionTime.HasValue ? (object)((DateTime)trace.CompletionTime.Value).ToLocalTime() : null,
                                            trace.EventType.HasValue ? (object)trace.EventType.Value.ToString() : null,
                                            trace.Duration.HasValue ? (object)(long)Math.Truncate(trace.Duration.Value.TotalMilliseconds) : null,
                                            trace.CpuTime.HasValue ? (object)(long)Math.Truncate(trace.CpuTime.Value.TotalMilliseconds) : null,
                                            trace.Reads,
                                            trace.Writes,
                                            trace.SqlText,
                                            trace.SequenceNumber
                                        });
                        }

                        traceGridDataSource.ResumeBindingNotifications();

                        if (!initialized)
                        {
                            if (lastMainGridSettings != null)
                            {
                                GridSettings.ApplySettingsToGrid(lastMainGridSettings, traceGrid);

                                initialized = true;
                            }
                            else if (snapshot.TraceItems.Count > 0)
                            {
                                foreach (UltraGridColumn column in traceGrid.DisplayLayout.Bands[0].Columns)
                                {
                                    column.PerformAutoResize(PerformAutoSizeType.AllRowsInBand, true);
                                }
                                if (traceGridDataSource.Rows.Count > 1)
                                {
                                    initialized = true;
                                }
                            }
                        }

                        long duration;
                        long threshold = (int)durationSpinner.Value;
                        Color rowColor;
                        foreach (UltraGridRow row in traceGrid.Rows.GetAllNonGroupByRows())
                        {
                            rowColor = Color.Black;
                            if (row.Cells["duration"].Value is long)
                            {
                                duration = (long)row.Cells["duration"].Value;
                                if (duration > threshold)
                                {
                                    rowColor = Color.Red;
                                }
                            }
                            row.Appearance.ForeColor = rowColor;
                        }

                        statusBar.Panels[(int)StatusPanel.TraceItems].Text = String.Format(ITEM_COUNT_MSG,
                                                        traceGridDataSource.Rows.Count,
                                                        traceGridDataSource.Rows.Count == 1 ? string.Empty : "s");
                    }
                    else
                    {
                        e = snapshot.Error;
                    }
                }
            }
            else if (data != null && data is Exception)
            {
                e = (Exception)data;
            }

            if (e == null)
            {
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Appearance.Image = null;
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Text = string.Format(REFRESHED_MSG, DateTime.Now.ToString("G"));
            }
            else
            {
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Text = REFRESH_ERROR_MSG;
            }

            refreshError = e;

            Cursor = Cursors.Default;
        }

        private void updateTextBoxColor(TextBox textBox, bool warn)
        {
            if (warn)
            {
                textBox.BackColor = WARN_BACKCOLOR;
            }
            else
            {
                textBox.BackColor = OK_BACKCOLOR;
            }
        }

        #endregion

        #region grid

        private void PrintGrid()
        {
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft = ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} {1} as of {2}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        this.Text,
                                        DateTime.Now.ToString("G")
                                    );

            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(traceGrid, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {
                traceGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                traceGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                traceGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                traceGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        public void ToggleGroupByBox()
        {
            traceGrid.DisplayLayout.GroupByBox.Hidden = !traceGrid.DisplayLayout.GroupByBox.Hidden;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    traceGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    traceGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }
            }
        }

        private void RemoveSelectedColumn()
        {
            if (selectedColumn != null)
            {
                selectedColumn.Hidden = true;
            }
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(traceGrid);
            dialog.Show(this);
        }

        #endregion

        #region trace

        private void StartTrace()
        {
            traceOn = true;

            UltraDataRow newRow = traceGridDataSource.Rows.Add(
                new object[]
                        {
                            DateTime.Now,
                            TRACE_STARTED_MSG,
                            null,
                            null,
                            null,
                            null,
                            String.Empty,
                            null
                        });

            statusBar.Panels[(int)StatusPanel.TraceStatus].Appearance.Image = null;
            statusBar.Panels[(int)StatusPanel.TraceStatus].Text = TRACE_STARTED_MSG;
            statusBar.Panels[(int)StatusPanel.TraceItems].Text = String.Format(ITEM_COUNT_MSG,
                                            traceGridDataSource.Rows.Count,
                                            traceGridDataSource.Rows.Count == 0 ? string.Empty : "s");

            // force a refresh and restart the timer interval
            automatedRefreshTimer.Enabled = false;
            automatedRefreshTimer_Tick(this, EventArgs.Empty);
            automatedRefreshTimer.Interval = (int)autoRefreshSpinner.Value * 1000;
            automatedRefreshTimer.Enabled = true;

            LogStartTrace();
        }

        private void LogStartTrace()
        {
            AuditingEngine.Instance.ManagementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            AuditingEngine.Instance.SQLUser =
                Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UseIntegratedSecurity
                    ? AuditingEngine.GetWorkstationUser()
                    : Settings.Default.ActiveRepositoryConnection.ConnectionInfo.UserName;
            
            AuditingEngine.Instance.LogAction(GetAuditable(),
                                              AuditableActionType.TraceSession,
                                              AuditingEngine.Instance.SQLUser);
        }

        private AuditableEntity GetAuditable()
        {
            var result = new AuditableEntity();

            IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            SessionDetailSnapshot detail = managementService.GetSessionDetails(configurationDetails);

            result.Name = detail.ServerName;
            result.AddMetadataProperty("Session ID", detail.Details.Spid.ToString());
            result.AddMetadataProperty("Server Name", detail.ServerName);
            result.AddMetadataProperty("Database Name", detail.Details.Database);

            return result;
        }

        private void StopTrace()
        {
            traceOn = false;

            UltraDataRow newRow = traceGridDataSource.Rows.Add(
                new object[]
                        {
                            DateTime.Now,
                            TRACE_STOPPED_MSG,
                            null,
                            null,
                            null,
                            null,
                            String.Empty,
                            null
                        });

            statusBar.Panels[(int)StatusPanel.TraceStatus].Appearance.Image = null;
            statusBar.Panels[(int)StatusPanel.TraceStatus].Text = TRACE_STOPPED_MSG;
            statusBar.Panels[(int)StatusPanel.TraceItems].Text = String.Format(ITEM_COUNT_MSG,
                                traceGridDataSource.Rows.Count,
                                traceGridDataSource.Rows.Count == 0 ? string.Empty : "s");

            // stop the timer and post the stop request, but ignore the data coming back
            //automatedRefreshTimer.Enabled = false;
            DoRefreshWork();
        }

        #endregion

        private void KillSession()
        {
            if (DialogResult.Yes == ApplicationMessageBox.ShowWarning(this,
                                    String.Format("You are about to kill Session ID {0}. Do you want to continue?",
                                                    selectedSpid.ToString()),
                                                    null, Microsoft.SqlServer.MessageBox.ExceptionMessageBoxButtons.YesNo))
            {
                StopTrace();
                IManagementService managementService =
                    ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                KillSessionConfiguration config = new KillSessionConfiguration(instanceId, selectedSpid);
                Snapshot snapshot = managementService.SendKillSession(config);

                if (snapshot.Error == null)
                {
                    ApplicationMessageBox.ShowMessage("The Session has been terminated.");
                    Close();
                }
                else
                {
                    throw snapshot.Error;
                }
            }
        }

        #endregion

        #region events

        protected override void OnHelpButtonClicked(CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsBlockingView);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.SessionsBlockingView);
        }

        #region timer and refresh

        private void automatedRefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void refreshBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            if (System.Threading.Thread.CurrentThread.Name == null) System.Threading.Thread.CurrentThread.Name = "SessionTraceWorker";

            e.Result = DoRefreshWork();

            BackgroundWorker backgroundWorker = sender as BackgroundWorker;

            if (backgroundWorker != null && backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void refreshBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || formClosing)
            {
                return;
            }
            else if (e.Error != null)
            {
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Appearance.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RefreshError;
                statusBar.Panels[(int)StatusPanel.RefreshStatus].Text = REFRESH_ERROR_MSG;
                refreshError = e.Error;
            }
            else
            {
                UpdateData(e.Result);
            }
        }

        #endregion

        #region toolstrip buttons

        private void startTraceToolStripButton_Click(object sender, EventArgs e)
        {
            StartTrace();
        }

        private void stopTraceToolStripButton_Click(object sender, EventArgs e)
        {
            StopTrace();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void clearTraceToolStripButton_Click(object sender, EventArgs e)
        {
            traceGridDataSource.Rows.Clear();
            statusBar.Panels[(int)StatusPanel.TraceItems].Text = "0 Items";
        }

        private void killSessionToolStripButton_Click(object sender, EventArgs e)
        {
            KillSession();
        }

        #endregion

        #region toolbars

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "toggleGroupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "showColumnChooserButton":
                    ShowColumnChooser();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
            }
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(traceGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #endregion

        #region grid

        private void traceGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader =
                        contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;
                    ((Infragistics.Win.UltraWinToolbars.StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof(RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        #endregion

        private void autoRefreshSpinner_ValueChanged(object sender, EventArgs e)
        {
            automatedRefreshTimer.Interval = (int)autoRefreshSpinner.Value * 1000;
        }

        private void SessionTraceDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            CancelRefresh();
            _traceOn = false;
            formClosing = true;
            DoRefreshWork();
            SaveSettings();
        }

        private void SessionTraceDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                RefreshView();
            }
        }

        private void SessionTraceDialog_Load(object sender, EventArgs e)
        {
            ApplySettings();
            if (!ApplicationModel.Default.AllInstances[instanceId].IsUserSysAdmin)
            {
                startTraceToolStripButton.Enabled = false;
                stopTraceToolStripButton.Enabled = false;
                contentPanel.Visible = false;
                SYSAdminWarningLabel.Visible = true;
            }

        }

        private void statusBar_PanelClick(object sender, Infragistics.Win.UltraWinStatusBar.PanelClickEventArgs e)
        {
            if (e.Panel.Index == (int)StatusPanel.RefreshStatus && refreshError != null)
            {
                ApplicationMessageBox.ShowError(this, "An error occurred while refreshing the active view.", refreshError, false);
            }
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.traceGrid);
        }
        #endregion
    }
}