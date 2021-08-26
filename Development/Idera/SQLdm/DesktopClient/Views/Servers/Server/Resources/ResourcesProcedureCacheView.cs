using System;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win.UltraWinToolbars;
using ColumnHeader=Infragistics.Win.UltraWinGrid.ColumnHeader;
using Constants=Idera.SQLdm.Common.Constants;
using System.Globalization;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesProcedureCacheView : ServerBaseView
    {
        private const string FORMAT_CHARTTYPE = "Object Types: {0}";
        private DateTime? historicalSnapshotDateTime;
        private DataTable historyDataTable;
        private bool initialized = false;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Control focused = null;
        private UltraGridColumn selectedColumn = null;

        //last Settings values used to determine if changed for saving when leaving
        private int lastMainSplitterDistance = 0;
        private int lastQuerySplitterDistance = 0;
        private GridSettings lastMainGridSettings = null;
        private bool lastChartsVisible = true;
        private string lastChartType = string.Empty;


        public event EventHandler ChartsVisibleChanged;
        public event EventHandler GridGroupByBoxVisibleChanged;

        public ResourcesProcedureCacheView(int instanceId) : base(instanceId)
        {
            InitializeComponent();

            ChartFxExtensions.SetContextMenu(cacheSizeChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(objectTypesChart, toolbarsManager);

            cacheObjectsGrid.DrawFilter = new HideFocusRectangleDrawFilter();
            historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeUnsupportedViewLabel;

            InitializeHistoryDataTable();
            InitializeCharts();

            // Autoscale font size.
            AdaptFontSize();

            Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
        }


        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set { historicalSnapshotDateTime = value; }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomHistoryData();
                    break;
            }
        }

        public override void ShowHelp()
        {
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesProcedureCacheView);
        }

        public override void ApplySettings()
        {
            // Fixed panel is second panel, so restore size of second panel
            lastMainSplitterDistance = splitContainer.Height - Settings.Default.ResourcesProcedureCacheViewMainSplitter;
            if (lastMainSplitterDistance > 0)
            {
                splitContainer.SplitterDistance = lastMainSplitterDistance;
            }
            else
            {
                lastMainSplitterDistance = splitContainer.Height - splitContainer.SplitterDistance;
            }

            // Fixed panel is second panel, so restore size of second panel
            lastQuerySplitterDistance = splitContainer2.Height - Settings.Default.ResourcesProcedureCacheViewQuerySplitter;
            if (lastQuerySplitterDistance > 0)
            {
                splitContainer2.SplitterDistance = lastQuerySplitterDistance;
            }
            else
            {
                lastQuerySplitterDistance = splitContainer2.Height - splitContainer2.SplitterDistance;
            }

            if (Settings.Default.ResourcesProcedureCacheViewMainGrid != null)
            {
                lastMainGridSettings = Settings.Default.ResourcesProcedureCacheViewMainGrid;
                GridSettings.ApplySettingsToGrid(lastMainGridSettings, cacheObjectsGrid);
                // force a change so ribbon stays in sync
                GridGroupByBoxVisible = GridGroupByBoxVisible;
            }

            lastChartsVisible =
                ChartsVisible = Settings.Default.ResourcesProcedureCacheViewChartsVisible;

            string chartType = Settings.Default.ResourcesProcedureCacheViewChartType;
            if (chartType != null && chartType.Length > 0)
            {
                foreach (ToolStripItem item in objectTypesHeaderStripLabel.DropDownItems)
                {
                    if (item.Text == chartType)
                    {
                        ConfigureObjectTypesChart(chartType);
                        lastChartType = chartType;
                        break;
                    }
                }
            }
        }

        public override void SaveSettings()
        {
            GridSettings mainGridSettings = GridSettings.GetSettings(cacheObjectsGrid);
            // save all settings only if anything has changed
            if (lastMainSplitterDistance != splitContainer.Height - splitContainer.SplitterDistance
                || !mainGridSettings.Equals(lastMainGridSettings)
                || lastChartsVisible != ChartsVisible
                || string.Format(FORMAT_CHARTTYPE, lastChartType) != objectTypesHeaderStripLabel.Text)

            {
                // Fixed panel is second panel, so save size of second panel
                lastMainSplitterDistance =
                    Settings.Default.ResourcesProcedureCacheViewMainSplitter = splitContainer.Height - splitContainer.SplitterDistance;
                // Fixed panel is second panel, so save size of second panel
                lastQuerySplitterDistance =
                    Settings.Default.ResourcesProcedureCacheViewQuerySplitter = splitContainer2.Height - splitContainer2.SplitterDistance;
                lastMainGridSettings =
                    Settings.Default.ResourcesProcedureCacheViewMainGrid = mainGridSettings;
                lastChartsVisible =
                    Settings.Default.ResourcesProcedureCacheViewChartsVisible = ChartsVisible;
                lastChartType =
                    Settings.Default.ResourcesProcedureCacheViewChartType = objectTypesHeaderStripLabel.Text.Substring(FORMAT_CHARTTYPE.Length - 3);
            }
        }

        #region Properties

        public bool ChartsVisible
        {
            get { return !splitContainer.Panel2Collapsed; }
            set
            {
                splitContainer.Panel2Collapsed = !value;
                RestoreChart();

                if (ChartsVisibleChanged != null)
                {
                    ChartsVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        public bool GridGroupByBoxVisible
        {
            get { return !cacheObjectsGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                cacheObjectsGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region History DataTable

        private void InitializeHistoryDataTable()
        {
            historyDataTable = new DataTable();
            historyDataTable.Columns.Add("Date", typeof (DateTime));

            //
            // Size
            //
            historyDataTable.Columns.Add("Adhoc Query - Size", typeof (decimal));
            historyDataTable.Columns.Add("Check Constraint - Size", typeof (decimal));
            historyDataTable.Columns.Add("Default Constraint - Size", typeof (decimal));
            historyDataTable.Columns.Add("Extended Procedure - Size", typeof (decimal));
            historyDataTable.Columns.Add("Prepared Statement - Size", typeof (decimal));
            historyDataTable.Columns.Add("Replication Procedure - Size", typeof (decimal));
            historyDataTable.Columns.Add("Rule - Size", typeof (decimal));
            historyDataTable.Columns.Add("Stored Procedure - Size", typeof (decimal));
            historyDataTable.Columns.Add("System Table - Size", typeof (decimal));
            historyDataTable.Columns.Add("Trigger - Size", typeof (decimal));
            historyDataTable.Columns.Add("User Table - Size", typeof (decimal));
            historyDataTable.Columns.Add("View - Size", typeof (decimal));

            //
            // Hit Ratio
            //
            historyDataTable.Columns.Add("Adhoc Query - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Check Constraint - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Default Constraint - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Extended Procedure - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Prepared Statement - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Replication Procedure - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Rule - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Stored Procedure - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("System Table - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("Trigger - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("User Table - Hit Ratio", typeof (double));
            historyDataTable.Columns.Add("View - Hit Ratio", typeof (double));

            //
            // Object Count
            //
            historyDataTable.Columns.Add("Adhoc Query - Object Count", typeof (long));
            historyDataTable.Columns.Add("Check Constraint - Object Count", typeof (long));
            historyDataTable.Columns.Add("Default Constraint - Object Count", typeof (long));
            historyDataTable.Columns.Add("Extended Procedure - Object Count", typeof (long));
            historyDataTable.Columns.Add("Prepared Statement - Object Count", typeof (long));
            historyDataTable.Columns.Add("Replication Procedure - Object Count", typeof (long));
            historyDataTable.Columns.Add("Rule - Object Count", typeof (long));
            historyDataTable.Columns.Add("Stored Procedure - Object Count", typeof (long));
            historyDataTable.Columns.Add("System Table - Object Count", typeof (long));
            historyDataTable.Columns.Add("Trigger - Object Count", typeof (long));
            historyDataTable.Columns.Add("User Table - Object Count", typeof (long));
            historyDataTable.Columns.Add("View - Object Count", typeof (long));

            //
            // Use Rate
            //
            historyDataTable.Columns.Add("Adhoc Query - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Check Constraint - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Default Constraint - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Extended Procedure - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Prepared Statement - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Replication Procedure - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Rule - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Stored Procedure - Use Rate", typeof (long));
            historyDataTable.Columns.Add("System Table - Use Rate", typeof (long));
            historyDataTable.Columns.Add("Trigger - Use Rate", typeof (long));
            historyDataTable.Columns.Add("User Table - Use Rate", typeof (long));
            historyDataTable.Columns.Add("View - Use Rate", typeof (long));
        }

        private void AddHistoryData(ProcedureCache snapshot)
        {
            if (snapshot != null && snapshot.Error == null)
            {
                DataRow newRow = historyDataTable.NewRow();
                newRow["Date"] = DateTime.Now;

                //
                // Size
                //
                AddHistorySize(snapshot, newRow, "adhoc", "Adhoc Query - Size");
                AddHistorySize(snapshot, newRow, "check", "Check Constraint - Size");
                AddHistorySize(snapshot, newRow, "default", "Default Constraint - Size");
                AddHistorySize(snapshot, newRow, "extended procedure", "Extended Procedure - Size");
                AddHistorySize(snapshot, newRow, "prepared", "Prepared Statement - Size");
                AddHistorySize(snapshot, newRow, "replproc", "Replication Procedure - Size");
                AddHistorySize(snapshot, newRow, "rule", "Rule - Size");
                AddHistorySize(snapshot, newRow, "proc", "Stored Procedure - Size");
                AddHistorySize(snapshot, newRow, "systab", "System Table - Size");
                AddHistorySize(snapshot, newRow, "trigger", "Trigger - Size");
                AddHistorySize(snapshot, newRow, "usrtab", "User Table - Size");
                AddHistorySize(snapshot, newRow, "view", "View - Size");

                //
                // Hit Ratio
                //
                AddHistoryHitRatio(snapshot, newRow, "adhoc", "Adhoc Query - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "check", "Check Constraint - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "default", "Default Constraint - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "extended procedure", "Extended Procedure - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "prepared", "Prepared Statement - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "replproc", "Replication Procedure - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "rule", "Rule - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "proc", "Stored Procedure - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "systab", "System Table - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "trigger", "Trigger - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "usrtab", "User Table - Hit Ratio");
                AddHistoryHitRatio(snapshot, newRow, "view", "View - Hit Ratio");

                //
                // Object Count
                //
                AddHistoryObjectCount(snapshot, newRow, "adhoc", "Adhoc Query - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "check", "Check Constraint - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "default", "Default Constraint - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "extended procedure", "Extended Procedure - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "prepared", "Prepared Statement - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "replproc", "Replication Procedure - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "rule", "Rule - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "proc", "Stored Procedure - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "systab", "System Table - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "trigger", "Trigger - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "usrtab", "User Table - Object Count");
                AddHistoryObjectCount(snapshot, newRow, "view", "View - Object Count");

                //
                // Use Rate
                //
                AddHistoryUseRate(snapshot, newRow, "adhoc", "Adhoc Query - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "check", "Check Constraint - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "default", "Default Constraint - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "extended procedure", "Extended Procedure - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "prepared", "Prepared Statement - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "replproc", "Replication Procedure - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "rule", "Rule - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "proc", "Stored Procedure - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "systab", "System Table - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "trigger", "Trigger - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "usrtab", "User Table - Use Rate");
                AddHistoryUseRate(snapshot, newRow, "view", "View - Use Rate");

                historyDataTable.Rows.Add(newRow);
                GroomHistoryData();
            }
        }

        private void AddHistorySize(ProcedureCache snapshot, DataRow newRow, string objectType, string columnName)
        {
            if (snapshot != null && newRow != null)
            {
                ProcedureCacheObjectType objectTypeData;

                if (snapshot.ObjectTypes.TryGetValue(objectType, out objectTypeData))
                {
                    if (objectTypeData.Size != null && objectTypeData.Size.Megabytes.HasValue)
                    {
                        newRow[columnName] = objectTypeData.Size.Megabytes;
                    }
                    else
                    {
                        newRow[columnName] = DBNull.Value;
                    }
                }
                else
                {
                    newRow[columnName] = DBNull.Value;
                }
            }
        }

        private void AddHistoryHitRatio(ProcedureCache snapshot, DataRow newRow, string objectType, string columnName)
        {
            if (snapshot != null && newRow != null)
            {
                ProcedureCacheObjectType objectTypeData;

                if (snapshot.ObjectTypes.TryGetValue(objectType, out objectTypeData))
                {
                    if (objectTypeData.HitRatio.HasValue)
                    {
                        newRow[columnName] = objectTypeData.HitRatio;
                    }
                    else
                    {
                        newRow[columnName] = DBNull.Value;
                    }
                }
                else
                {
                    newRow[columnName] = DBNull.Value;
                }
            }
        }

        private void AddHistoryObjectCount(ProcedureCache snapshot, DataRow newRow, string objectType, string columnName)
        {
            if (snapshot != null && newRow != null)
            {
                ProcedureCacheObjectType objectTypeData;

                if (snapshot.ObjectTypes.TryGetValue(objectType, out objectTypeData))
                {
                    if (objectTypeData.ObjectCount.HasValue)
                    {
                        newRow[columnName] = objectTypeData.ObjectCount;
                    }
                    else
                    {
                        newRow[columnName] = DBNull.Value;
                    }
                }
                else
                {
                    newRow[columnName] = DBNull.Value;
                }
            }
        }

        private void AddHistoryUseRate(ProcedureCache snapshot, DataRow newRow, string objectType, string columnName)
        {
            if (snapshot != null && newRow != null)
            {
                ProcedureCacheObjectType objectTypeData;

                if (snapshot.ObjectTypes.TryGetValue(objectType, out objectTypeData))
                {
                    if (objectTypeData.UseCount.HasValue)
                    {
                        newRow[columnName] = objectTypeData.UseCount;
                    }
                    else
                    {
                        newRow[columnName] = DBNull.Value;
                    }
                }
                else
                {
                    newRow[columnName] = DBNull.Value;
                }
            }
        }

        private void GroomHistoryData()
        {
            if (historyDataTable != null)
            {
                DateTime groomThreshold =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));

                DataRow[] groomedRows = historyDataTable.Select(string.Format("Date < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture))); // SQLDM-19237, Tolga K

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                historyDataTable.AcceptChanges();
            }
        }

        #endregion

        #region Initialize Charts

        public void InitializeCharts()
        {
            InitializeObjectTypesChart();
            InitializeCacheSizeChart();
        }

        private void InitializeCacheSizeChart()
        {
            cacheSizeChart.Tag = cacheSizeHeaderStripLabel;
            cacheSizeChart.Printer.Orientation = PageOrientation.Landscape;
            cacheSizeChart.Printer.Compress = true;
            cacheSizeChart.Printer.ForceColors = true;
            cacheSizeChart.Printer.Document.DocumentName = "Cache Size Chart";
            cacheSizeChart.ToolBar.RemoveAt(0);
            cacheSizeChart.ToolTipFormat = "%s\n%v MB\n%x";
            cacheSizeChart.AxisY.DataFormat.Decimals = 2;

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap adhocQueryFieldMap = new FieldMap("Adhoc Query - Size", FieldUsage.Value);
            adhocQueryFieldMap.DisplayName = "Adhoc Query";
            FieldMap checkConstraintFieldMap = new FieldMap("Check Constraint - Size", FieldUsage.Value);
            checkConstraintFieldMap.DisplayName = "Check Constraint";
            FieldMap defaultConstraintFieldMap = new FieldMap("Default Constraint - Size", FieldUsage.Value);
            defaultConstraintFieldMap.DisplayName = "Default Constraint";
            FieldMap extendedProcedureFieldMap = new FieldMap("Extended Procedure - Size", FieldUsage.Value);
            extendedProcedureFieldMap.DisplayName = "Extended Procedure";
            FieldMap preparedStatementFieldMap = new FieldMap("Prepared Statement - Size", FieldUsage.Value);
            preparedStatementFieldMap.DisplayName = "Prepared Statement";
            FieldMap replicationProcedureFieldMap = new FieldMap("Replication Procedure - Size", FieldUsage.Value);
            replicationProcedureFieldMap.DisplayName = "Replication Procedure";
            FieldMap ruleFieldMap = new FieldMap("Rule - Size", FieldUsage.Value);
            ruleFieldMap.DisplayName = "Rule";
            FieldMap storedProcedureFieldMap = new FieldMap("Stored Procedure - Size", FieldUsage.Value);
            storedProcedureFieldMap.DisplayName = "Stored Procedure";
            FieldMap systemTableFieldMap = new FieldMap("System Table - Size", FieldUsage.Value);
            systemTableFieldMap.DisplayName = "System Table";
            FieldMap triggerFieldMap = new FieldMap("Trigger - Size", FieldUsage.Value);
            triggerFieldMap.DisplayName = "Trigger";
            FieldMap userTableFieldMap = new FieldMap("User Table - Size", FieldUsage.Value);
            userTableFieldMap.DisplayName = "User Table";
            FieldMap viewFieldMap = new FieldMap("View - Size", FieldUsage.Value);
            viewFieldMap.DisplayName = "View";

            cacheSizeChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      viewFieldMap,
                                                                      userTableFieldMap,
                                                                      triggerFieldMap,
                                                                      systemTableFieldMap,
                                                                      storedProcedureFieldMap,
                                                                      ruleFieldMap,
                                                                      replicationProcedureFieldMap,
                                                                      preparedStatementFieldMap,
                                                                      extendedProcedureFieldMap,
                                                                      defaultConstraintFieldMap,
                                                                      checkConstraintFieldMap,
                                                                      adhocQueryFieldMap
                                                                  });

            Color tempColor0 = cacheSizeChart.Series[0].Color;
            Color tempColor1 = cacheSizeChart.Series[1].Color;
            Color tempColor2 = cacheSizeChart.Series[2].Color;
            Color tempColor3 = cacheSizeChart.Series[3].Color;
            Color tempColor4 = cacheSizeChart.Series[4].Color;
            Color tempColor5 = cacheSizeChart.Series[5].Color;
            cacheSizeChart.Series[0].Color = cacheSizeChart.Series[11].Color;
            cacheSizeChart.Series[1].Color = cacheSizeChart.Series[10].Color;
            cacheSizeChart.Series[2].Color = cacheSizeChart.Series[9].Color;
            cacheSizeChart.Series[3].Color = cacheSizeChart.Series[8].Color;
            cacheSizeChart.Series[4].Color = cacheSizeChart.Series[7].Color;
            cacheSizeChart.Series[5].Color = cacheSizeChart.Series[6].Color;
            cacheSizeChart.Series[6].Color = tempColor5;
            cacheSizeChart.Series[7].Color = tempColor4;
            cacheSizeChart.Series[8].Color = tempColor3;
            cacheSizeChart.Series[9].Color = tempColor2;
            cacheSizeChart.Series[10].Color = tempColor1;
            cacheSizeChart.Series[11].Color = tempColor0;
            ChartFxExtensions.SetAreaSeriesAlphaChannel(cacheSizeChart, 175);
            cacheSizeChart.AxisX.LabelsFormat.Format = AxisFormat.Time;
            cacheSizeChart.LegendBox.Width = 155;
            cacheSizeChart.LegendBox.PlotAreaOnly = false;
            cacheSizeChart.DataSource = historyDataTable;
            
        }

        private void InitializeObjectTypesChart()
        {
            objectTypesChart.Tag = objectTypesHeaderStripLabel;
            objectTypesChart.Printer.Orientation = PageOrientation.Landscape;
            objectTypesChart.Printer.Compress = true;
            objectTypesChart.Printer.ForceColors = true;
            objectTypesChart.Printer.Document.DocumentName = "Object Types Chart";
            objectTypesChart.ToolBar.RemoveAt(0);

            objectTypesChart.AxisX.LabelsFormat.Format = AxisFormat.Time;
            objectTypesChart.LegendBox.Width = 155;
            objectTypesChart.LegendBox.PlotAreaOnly = false;
            objectTypesChart.DataSource = historyDataTable;

            ConfigureObjectTypesHitRatioChart();
        }

        private void ConfigureObjectTypesChart(string chartType)
        {
            objectTypesHeaderStripLabel.Text = string.Format(FORMAT_CHARTTYPE, chartType);
            if (chartType == "Hit Ratio")
            {
                ConfigureObjectTypesHitRatioChart();
            }
            else if (chartType == "Object Count")
            {
                ConfigureObjectTypesObjectCountChart();
            }
            else if (chartType == "Use Rate")
            {
                ConfigureObjectTypesUseRateChart();
            }
        }

        private void ConfigureObjectTypesHitRatioChart()
        {
            objectTypesChart.DataSourceSettings.Fields.Clear();

            objectTypesChart.AxisY.Title.Text = "%";
            objectTypesChart.AxisY.Min = 0;
            objectTypesChart.AxisY.Max = 100;
            objectTypesChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap adhocQueryFieldMap = new FieldMap("Adhoc Query - Hit Ratio", FieldUsage.Value);
            adhocQueryFieldMap.DisplayName = "Adhoc Query";
            FieldMap checkConstraintFieldMap = new FieldMap("Check Constraint - Hit Ratio", FieldUsage.Value);
            checkConstraintFieldMap.DisplayName = "Check Constraint";
            FieldMap defaultConstraintFieldMap = new FieldMap("Default Constraint - Hit Ratio", FieldUsage.Value);
            defaultConstraintFieldMap.DisplayName = "Default Constraint";
            FieldMap extendedProcedureFieldMap = new FieldMap("Extended Procedure - Hit Ratio", FieldUsage.Value);
            extendedProcedureFieldMap.DisplayName = "Extended Procedure";
            FieldMap preparedStatementFieldMap = new FieldMap("Prepared Statement - Hit Ratio", FieldUsage.Value);
            preparedStatementFieldMap.DisplayName = "Prepared Statement";
            FieldMap replicationProcedureFieldMap = new FieldMap("Replication Procedure - Hit Ratio", FieldUsage.Value);
            replicationProcedureFieldMap.DisplayName = "Replication Procedure";
            FieldMap ruleFieldMap = new FieldMap("Rule - Hit Ratio", FieldUsage.Value);
            ruleFieldMap.DisplayName = "Rule";
            FieldMap storedProcedureFieldMap = new FieldMap("Stored Procedure - Hit Ratio", FieldUsage.Value);
            storedProcedureFieldMap.DisplayName = "Stored Procedure";
            FieldMap systemTableFieldMap = new FieldMap("System Table - Hit Ratio", FieldUsage.Value);
            systemTableFieldMap.DisplayName = "System Table";
            FieldMap triggerFieldMap = new FieldMap("Trigger - Hit Ratio", FieldUsage.Value);
            triggerFieldMap.DisplayName = "Trigger";
            FieldMap userTableFieldMap = new FieldMap("User Table - Hit Ratio", FieldUsage.Value);
            userTableFieldMap.DisplayName = "User Table";
            FieldMap viewFieldMap = new FieldMap("View - Hit Ratio", FieldUsage.Value);
            viewFieldMap.DisplayName = "View";

            objectTypesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        adhocQueryFieldMap,
                                                                        checkConstraintFieldMap,
                                                                        defaultConstraintFieldMap,
                                                                        extendedProcedureFieldMap,
                                                                        preparedStatementFieldMap,
                                                                        replicationProcedureFieldMap,
                                                                        ruleFieldMap,
                                                                        storedProcedureFieldMap,
                                                                        systemTableFieldMap,
                                                                        triggerFieldMap,
                                                                        userTableFieldMap,
                                                                        viewFieldMap
                                                                    });

            objectTypesChart.DataSourceSettings.ReloadData();
        }

        private void ConfigureObjectTypesObjectCountChart()
        {
            objectTypesChart.DataSourceSettings.Fields.Clear();

            objectTypesChart.AxisY.Title.Text = String.Empty;
            objectTypesChart.AxisY.AutoScale = true;
            objectTypesChart.ToolTipFormat = "%s\n%v objects\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap adhocQueryFieldMap = new FieldMap("Adhoc Query - Object Count", FieldUsage.Value);
            adhocQueryFieldMap.DisplayName = "Adhoc Query";
            FieldMap checkConstraintFieldMap = new FieldMap("Check Constraint - Object Count", FieldUsage.Value);
            checkConstraintFieldMap.DisplayName = "Check Constraint";
            FieldMap defaultConstraintFieldMap = new FieldMap("Default Constraint - Object Count", FieldUsage.Value);
            defaultConstraintFieldMap.DisplayName = "Default Constraint";
            FieldMap extendedProcedureFieldMap = new FieldMap("Extended Procedure - Object Count", FieldUsage.Value);
            extendedProcedureFieldMap.DisplayName = "Extended Procedure";
            FieldMap preparedStatementFieldMap = new FieldMap("Prepared Statement - Object Count", FieldUsage.Value);
            preparedStatementFieldMap.DisplayName = "Prepared Statement";
            FieldMap replicationProcedureFieldMap =
                new FieldMap("Replication Procedure - Object Count", FieldUsage.Value);
            replicationProcedureFieldMap.DisplayName = "Replication Procedure";
            FieldMap ruleFieldMap = new FieldMap("Rule - Object Count", FieldUsage.Value);
            ruleFieldMap.DisplayName = "Rule";
            FieldMap storedProcedureFieldMap = new FieldMap("Stored Procedure - Object Count", FieldUsage.Value);
            storedProcedureFieldMap.DisplayName = "Stored Procedure";
            FieldMap systemTableFieldMap = new FieldMap("System Table - Object Count", FieldUsage.Value);
            systemTableFieldMap.DisplayName = "System Table";
            FieldMap triggerFieldMap = new FieldMap("Trigger - Object Count", FieldUsage.Value);
            triggerFieldMap.DisplayName = "Trigger";
            FieldMap userTableFieldMap = new FieldMap("User Table - Object Count", FieldUsage.Value);
            userTableFieldMap.DisplayName = "User Table";
            FieldMap viewFieldMap = new FieldMap("View - Object Count", FieldUsage.Value);
            viewFieldMap.DisplayName = "View";

            objectTypesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        adhocQueryFieldMap,
                                                                        checkConstraintFieldMap,
                                                                        defaultConstraintFieldMap,
                                                                        extendedProcedureFieldMap,
                                                                        preparedStatementFieldMap,
                                                                        replicationProcedureFieldMap,
                                                                        ruleFieldMap,
                                                                        storedProcedureFieldMap,
                                                                        systemTableFieldMap,
                                                                        triggerFieldMap,
                                                                        userTableFieldMap,
                                                                        viewFieldMap
                                                                    });

            objectTypesChart.DataSourceSettings.ReloadData();
        }

        private void ConfigureObjectTypesUseRateChart()
        {
            objectTypesChart.DataSourceSettings.Fields.Clear();

            objectTypesChart.AxisY.Title.Text = String.Empty;
            objectTypesChart.AxisY.AutoScale = true;
            objectTypesChart.ToolTipFormat = "%s\n%v uses\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap adhocQueryFieldMap = new FieldMap("Adhoc Query - Use Rate", FieldUsage.Value);
            adhocQueryFieldMap.DisplayName = "Adhoc Query";
            FieldMap checkConstraintFieldMap = new FieldMap("Check Constraint - Use Rate", FieldUsage.Value);
            checkConstraintFieldMap.DisplayName = "Check Constraint";
            FieldMap defaultConstraintFieldMap = new FieldMap("Default Constraint - Use Rate", FieldUsage.Value);
            defaultConstraintFieldMap.DisplayName = "Default Constraint";
            FieldMap extendedProcedureFieldMap = new FieldMap("Extended Procedure - Use Rate", FieldUsage.Value);
            extendedProcedureFieldMap.DisplayName = "Extended Procedure";
            FieldMap preparedStatementFieldMap = new FieldMap("Prepared Statement - Use Rate", FieldUsage.Value);
            preparedStatementFieldMap.DisplayName = "Prepared Statement";
            FieldMap replicationProcedureFieldMap = new FieldMap("Replication Procedure - Use Rate", FieldUsage.Value);
            replicationProcedureFieldMap.DisplayName = "Replication Procedure";
            FieldMap ruleFieldMap = new FieldMap("Rule - Use Rate", FieldUsage.Value);
            ruleFieldMap.DisplayName = "Rule";
            FieldMap storedProcedureFieldMap = new FieldMap("Stored Procedure - Use Rate", FieldUsage.Value);
            storedProcedureFieldMap.DisplayName = "Stored Procedure";
            FieldMap systemTableFieldMap = new FieldMap("System Table - Use Rate", FieldUsage.Value);
            systemTableFieldMap.DisplayName = "System Table";
            FieldMap triggerFieldMap = new FieldMap("Trigger - Use Rate", FieldUsage.Value);
            triggerFieldMap.DisplayName = "Trigger";
            FieldMap userTableFieldMap = new FieldMap("User Table - Use Rate", FieldUsage.Value);
            userTableFieldMap.DisplayName = "User Table";
            FieldMap viewFieldMap = new FieldMap("View - Use Rate", FieldUsage.Value);
            viewFieldMap.DisplayName = "View";

            objectTypesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        adhocQueryFieldMap,
                                                                        checkConstraintFieldMap,
                                                                        defaultConstraintFieldMap,
                                                                        extendedProcedureFieldMap,
                                                                        preparedStatementFieldMap,
                                                                        replicationProcedureFieldMap,
                                                                        ruleFieldMap,
                                                                        storedProcedureFieldMap,
                                                                        systemTableFieldMap,
                                                                        triggerFieldMap,
                                                                        userTableFieldMap,
                                                                        viewFieldMap
                                                                    });

            objectTypesChart.DataSourceSettings.ReloadData();
        }

        private void objectTypesHitRatioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureObjectTypesChart("Hit Ratio");
        }

        private void objectTypesObjectCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureObjectTypesChart("Object Count");
        }

        private void objectTypesUseRateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigureObjectTypesChart("Use Rate");
        }

        #endregion

        #region Refresh View

        public override void RefreshView()
        {
            if (HistoricalSnapshotDateTime == null)
            {
                ResourcesProcedureCacheView_Fill_Panel.Visible = true;
                cacheObjectsGridStatusLabel.Text =
                    cacheSizeChartStatusLabel.Text =
                    objectTypesChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
                base.RefreshView();
            }
            else
            {
                ResourcesProcedureCacheView_Fill_Panel.Visible = false;
                ApplicationController.Default.OnRefreshActiveViewCompleted(
                        new RefreshActiveViewCompletedEventArgs(DateTime.Now));
            }
        }

        public override object DoRefreshWork()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            ProcedureCacheConfiguration configuration = new ProcedureCacheConfiguration(instanceId);
            return (Snapshot)managementService.GetProcedureCache(configuration);
        }

        public override void UpdateData(object data)
        {
            if (data != null && data is ProcedureCache)
            {
                lock (updateLock)
                {
                    ProcedureCache snapshot = data as ProcedureCache;

                    if (snapshot.Error == null)
                    {
                        if (snapshot.ObjectList != null && snapshot.ObjectList.Rows.Count > 0)
                        {
                            GridSettings gridSettings;
                            if (!initialized && lastMainGridSettings != null)
                            {
                                gridSettings = lastMainGridSettings;
                                initialized = true;
                            }
                            else
                            {
                                gridSettings = GridSettings.GetSettings(cacheObjectsGrid);
                            }

                            cacheObjectsGrid.DataSource = snapshot.ObjectList;

                            //reapply grid settings on every refresh because datasource is changed
                            GridSettings.ApplySettingsToGrid(gridSettings, cacheObjectsGrid);

                            splitContainer2.Visible = true;
                            ApplicationController.Default.SetCustomStatus(
                                String.Format("Procedure Cache: {0} Item{1}",
                                        snapshot.ObjectList.Rows.Count,
                                        snapshot.ObjectList.Rows.Count == 1 ? string.Empty : "s")
                                );
                        }
                        else
                        {
                            cacheObjectsGridStatusLabel.Text = "There are no items to show in this view.";
                            splitContainer2.Visible = false;
                            cacheObjectsGrid.DataSource = null;
                            ApplicationController.Default.ClearCustomStatus();
                        }

                        AddHistoryData(snapshot);
                        UpdateChartDataFilter();

                        cacheSizeChart.Visible = true;
                        objectTypesChart.Visible = true;

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        cacheObjectsGridStatusLabel.Text =
                            cacheSizeChartStatusLabel.Text =
                            objectTypesChartStatusLabel.Text = "Unable to update data for this view.";
                        ApplicationController.Default.ClearCustomStatus();

                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, snapshot.Error));
                    }
                }
            }
        }

        private void UpdateChartDataFilter()
        {
            if (historyDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                historyDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
        }

        #endregion

        #region Splitter Focus

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

        private void splitContainer2_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void splitContainer2_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        private static Control GetFocusedControl(ControlCollection controls)
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

        #endregion

        #region Charts

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private void PrintChart(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            splitContainer.Visible = false;
            chartsLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            ResourcesProcedureCacheView_Fill_Panel.Controls.Add(chartPanel);
        }

        private void RestoreChart()
        {
            if (restoreCacheSizeChartButton.Visible)
            {
                RestoreChart(cacheSizePanel, maximizeCacheSizeChartButton, restoreCacheSizeChartButton, 0, 0);
            }
            else if (restoreObjectTypesChartButton.Visible)
            {
                RestoreChart(objectTypesPanel, maximizeObjectTypesChartButton, restoreObjectTypesChartButton, 1, 0);
            }
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            ResourcesProcedureCacheView_Fill_Panel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            chartsLayoutPanel.Controls.Add(chartPanel);
            chartsLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            splitContainer.Visible = true;
        }

        private void maximizeCacheSizeChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cacheSizePanel, maximizeCacheSizeChartButton, restoreCacheSizeChartButton);
        }

        private void restoreCacheSizeChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cacheSizePanel, maximizeCacheSizeChartButton, restoreCacheSizeChartButton, 0, 0);
        }

        private void maximizeObjectTypesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(objectTypesPanel, maximizeObjectTypesChartButton, restoreObjectTypesChartButton);
        }

        private void restoreObjectTypesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(objectTypesPanel, maximizeObjectTypesChartButton, restoreObjectTypesChartButton, 1, 0);
        }

        #endregion

        #region Context Menus

        private void cacheObjectsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof (ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
                    toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "columnContextMenu");
                }
                else
                {
                    contextObject = selectedElement.GetAncestor(typeof (RowUIElement));

                    if (contextObject is RowUIElement)
                    {
                        RowUIElement row = contextObject as RowUIElement;
                        row.Row.Selected = true;

                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridDataContextMenu");
                    }
                    else
                    {
                        toolbarsManager.SetContextMenuUltra(((UltraGrid)sender), "gridContextMenu");
                    }
                }
            }
        }

        #region toolbar

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
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
                case "viewQueryHistoryButton":
                    ShowQueryHistoryView();
                    break;
                case "clearCacheButton":
                    ClearCache();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
                    break;
                case "collapseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                case "toggleChartToolbarButton":
                    if (contextMenuSelectedChart != null)
                    {
                        ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                    }
                    break;
                case "printChartButton":
                    if (contextMenuSelectedChart != null)
                    {
                        PrintChart(contextMenuSelectedChart);
                    }
                    break;
                case "exportChartDataButton":
                    if (contextMenuSelectedChart != null)
                    {
                        SaveChartData(contextMenuSelectedChart);
                    }
                    break;
                case "exportChartImageButton":
                    if (contextMenuSelectedChart != null)
                    {
                        SaveChartImage(contextMenuSelectedChart);
                    }
                    break;
            }
            contextMenuSelectedChart = null;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
                case "gridContextMenu":
                    ((PopupMenuTool)e.Tool).Tools["clearCacheButton"].SharedProps.Visible =
                        ApplicationModel.Default.UserToken.GetServerPermission(instanceId) >= PermissionType.Modify;
                    break;
            }

            if (e.Tool.Key == "gridDataContextMenu" || e.Tool.Key == "gridContextMenu")
            {
                bool isGrouped = cacheObjectsGrid.Rows.Count > 0 && cacheObjectsGrid.Rows[0].IsGroupByRow;
                ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                    ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
            }
            if (e.Tool.Key == "columnContextMenu")
            {
                int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(cacheObjectsGrid);
                bool enableTool = minCantForEnable > 1 ? true : false;

                ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
            }
        }

        #endregion

        #region grid

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = ultraGridPrintDocument;
            ultraGridPrintDocument.DefaultPageSettings.Landscape = true;
            ultraGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - procedure cache as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            ultraGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            saveFileDialog.DefaultExt = "xls";
            saveFileDialog.FileName = "ProcedureCache";
            saveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            saveFileDialog.Title = "Save as Excel Spreadsheet";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ultraGridExcelExporter.Export(cacheObjectsGrid, saveFileDialog.FileName);
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
                cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    cacheObjectsGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
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

        private void CollapseAllGroups()
        {
            cacheObjectsGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            cacheObjectsGrid.Rows.ExpandAll(true);
        }

        private void ShowColumnChooser()
        {
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(cacheObjectsGrid);
            dialog.Show(this);
        }

        #endregion

        private void ShowQueryHistoryView()
        {
            if (sqlTextBox.Text.Length > 0)
            {
                string SqlHash = SqlParsingHelper.GetSignatureHash(sqlTextBox.Text);
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.QueriesHistory, SqlHash);

            }
        }

        public void ClearCache()
        {
            DialogResult result =
                ApplicationMessageBox.ShowQuestion(ParentForm,
                                                   "Clearing the Procedure Cache can cause performance problems for the target instance because commonly used procedures will no longer be stored in cache. Would you like to continue?");

            if (result == DialogResult.Yes)
            {
                try
                {
                    IManagementService managementService =
                        ManagementServiceHelper.GetDefaultService(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    AuditingEngine.SetContextData(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo.ActiveRepositoryUser);

                    Snapshot snapshot =
                        managementService.SendFreeProcedureCache(new FreeProcedureCacheConfiguration(instanceId));

                    if (snapshot.Error == null)
                    {
                        ApplicationController.Default.RefreshActiveView();
                    }
                    else
                    {
                        ApplicationMessageBox.ShowError(this, "An error occurred while clearing the Procedure Cache.",
                                                        snapshot.Error);
                    }
                }
                catch (Exception e)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurred while clearing the Procedure Cache.", e);
                }
            }
        }

        #endregion

        private void ResourcesProcedureCacheView_Load(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void cacheObjectsGrid_AfterSelectChange(object sender, AfterSelectChangeEventArgs e)
        {
            if (cacheObjectsGrid.Selected.Rows.Count == 1 && cacheObjectsGrid.Selected.Rows[0].Cells != null)
            {
                sqlTextBox.Text = cacheObjectsGrid.Selected.Rows[0].Cells["Command"].Value as string;
            }
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        #region Autoscale font size

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {            
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        #endregion
    }
}
