using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ChartFX.WinForms;
using Wintellect.PowerCollections;

using Idera.SQLdm.Common.Services;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win.UltraWinDataSource;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Win;
using System.Drawing.Drawing2D;
using Idera.SQLdm.Common;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.Common.UI.Dialogs;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    internal partial class DatabasesControl : DashboardControl
    {
        private enum dbChartType
        {
            [Description("Transactions/sec")]
            Transactions,
            [Description("Log Flushes/sec")]
            LogFlushes,
            [Description("Reads/sec")]
            Reads,
            [Description("Writes/sec")]
            Writes,
            [Description("I/O Stall ms/sec")]
            IoStall
        }

        private enum dbDataColumn
        {
            [Description("TransactionsPerSec")]
            Transactions,
            [Description("LogFlushesPerSec")]
            LogFlushes,
            [Description("NumberReadsPerSec")]
            Reads,
            [Description("NumberWritesPerSec")]
            Writes,
            [Description("IoStallMSPerSec")]
            IoStall
        }

        private const string OPTIONTYPE_CHARTTYPE = @"ChartType";

        private const string chartTitleFormat = "{0} {1}";
        private Control contextControl = null;

        private int selectedType = -1;
        private List<string> selectedDatabases = new List<string>();
        private Dictionary<string, Dictionary<string, DataRowView>> selectedElasticPools = new Dictionary<string, Dictionary<string, DataRowView>>();

        private DataTable top5DataTable;

        public DatabasesControl() : base(DashboardPanel.Databases)
        {
            InitializeComponent();
            ChartFxExtensions.SetContextMenu(databaseChart, toolbarsManager);

            helpTopic = HelpTopics.ServerDashboardViewDatabasesPanel;
        }

        internal override void Initialize(ServerBaseView baseView, ServerSummaryHistoryData history)
        {
            base.Initialize(baseView, history);

            headerSelectTypeDropDownButton.DropDownItems.Clear();

            foreach (dbChartType type in Enum.GetValues(typeof(dbChartType)))
            {
                ToolStripMenuItem chartTypeItem = new ToolStripMenuItem(ApplicationHelper.GetEnumDescription(type), null, chartTypeSelectToolStripMenuItem_Click);
                chartTypeItem.Tag = (int)type;
                headerSelectTypeDropDownButton.DropDownItems.Add(chartTypeItem);
            }

            if (selectedType < 0)
            {
                selectedType = (int) headerSelectTypeDropDownButton.DropDownItems[0].Tag;
            }
            if (Enum.IsDefined(typeof(dbChartType), selectedType))
            {
                headerSelectTypeDropDownButton.Text = headerSelectTypeDropDownButton.DropDownItems[selectedType].Text;
            }
            headerSelectTypeSeparator.Visible =
                headerSelectTypeDropDownButton.Visible = true;

            InitializeDatabasesGrid();

            CreateChartDataSource();

            InitializeCharts();
        }

        public override void SetOptions(List<DashboardPanelOption> options)
        {
            foreach (var option in options)
            {
                if (option.Type == OPTIONTYPE_CHARTTYPE)
                {
                    foreach (string value in option.Values)
                    {
                        dbChartType chartType;
                        if (dbChartType.TryParse(value, true, out chartType))
                        {
                            selectedType = (int)chartType;
                            UpdateConfigOptions();
                            break;
                        }
                    }
                }
            }
        }

        private void UpdateConfigOptions()
        {
            List<string> options = new List<string> {Enum.GetName(typeof (dbChartType), selectedType)};
            DashboardPanelConfiguration.SetOptions(new List<DashboardPanelOption> { new DashboardPanelOption(OPTIONTYPE_CHARTTYPE, options) });
        }

        private void chartTypeSelectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                headerSelectTypeDropDownButton.Text = item.Text;
                if (item.Tag != null && item.Tag is int)
                {
                    selectedType = (int)item.Tag;
                    UpdateConfigOptions();
                    ConfigureDataSource();
                }
            }
        }

        private void CreateChartDataSource()
        {

            top5DataTable = new DataTable();
            top5DataTable.Columns.Add("Date", typeof(DateTime));

            top5DataTable.Columns.Add("TransactionsPerSec1", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec1", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec1", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec1", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec1", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec2", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec2", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec2", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec2", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec2", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec3", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec3", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec3", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec3", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec3", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec4", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec4", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec4", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec4", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec4", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec5", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec5", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec5", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec5", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec5", typeof(decimal));


            // Additional columns for Elastic Pools-Nikhil
            top5DataTable.Columns.Add("TransactionsPerSec6", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec6", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec6", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec6", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec6", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec7", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec7", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec7", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec7", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec7", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec8", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec8", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec8", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec8", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec8", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec9", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec9", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec9", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec9", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec9", typeof(decimal));
            top5DataTable.Columns.Add("TransactionsPerSec10", typeof(long));
            top5DataTable.Columns.Add("LogFlushesPerSec10", typeof(long));
            top5DataTable.Columns.Add("NumberReadsPerSec10", typeof(decimal));
            top5DataTable.Columns.Add("NumberWritesPerSec10", typeof(decimal));
            top5DataTable.Columns.Add("IoStallMSPerSec10", typeof(decimal));
            top5DataTable.PrimaryKey = new DataColumn[] { top5DataTable.Columns["DatabaseName"] };
            /* Elastic Pool Support-Nikhil
               Removal of Date as Primary Key-For Elastic Pools-we have to use same snapshot twice for below things-
               A-For the regular database -example qe-db at 11:45 pm
               B-For elastic Pool which have testdb(11:43pm) at and qe-db(11:45pm) and we have to use latest of these both dates(11:45pm) in the elastic pool row  which is causing same date to be used twice.
             */
            top5DataTable.DefaultView.Sort = "Date";
        }

        private void InitializeCharts()
        {
            InitializeDatabasesChart();
            ConfigureDatabasesChart();
            InitalizeDrilldown(databaseChart); //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeDatabasesGrid()
        {
            topDatabasesGrid.DrawFilter = new PercentageBackgroundDrawFilter();
        }

        private void ConfigureDatabasesGrid()
        {
            using (Log.VerboseCall("ConfigureDatabasesGrid"))
            {
                topDatabasesGrid.SuspendLayout();

                string selectedCol = ApplicationHelper.GetEnumDescription((dbDataColumn) selectedType);
                // creating a dataview on the passed table causes index corruption and index out of bounds exceptions
                // when it collides with the background refresh, so use a copy of the table

                if (historyData == null || historyData.CurrentDbSnapshotsDataTable == null || historyData.CurrentDbSnapshotsDataTable.DefaultView == null)
                    return;

                DataView sourceView = historyData.CurrentDbSnapshotsDataTable.DefaultView;
                DataTable lastSnapshot = sourceView.Table.Clone();
                
                // go backwards through the rows
                if (sourceView.Count > 0)
                {
                    lastSnapshot.BeginLoadData();
                    DataRowView lastRow = sourceView[sourceView.Count - 1];
                    DateTime match = (DateTime)lastRow["Date"];
                    selectedElasticPools.Clear();
                    for (int i = sourceView.Count - 1; i >= 0; i--)
                    {
                        DataRowView row = sourceView[i];
                        if (match.Equals(row["Date"]))
                        {
                            lastSnapshot.ImportRow(row.Row);
                            // Start: Elastic Pool Support-Nikhil
                            if (row["ElasticPool"] != string.Empty && row["ElasticPool"] != null)
                            {
                                if (selectedElasticPools.ContainsKey(Convert.ToString(row["ElasticPool"])))
                                {
                                    if (selectedElasticPools[Convert.ToString(row["ElasticPool"])].ContainsKey(Convert.ToString(row["DatabaseName"])))
                                        selectedElasticPools[Convert.ToString(row["ElasticPool"])][Convert.ToString(row["DatabaseName"])] = row;
                                    else
                                    {
                                        selectedElasticPools[Convert.ToString(row["ElasticPool"])].Add(Convert.ToString(row["DatabaseName"]), row);
                                    }
                                }
                                else if (Convert.ToString(row["ElasticPool"]) != string.Empty && Convert.ToString(row["ElasticPool"]) != "")
                                {
                                    selectedElasticPools.Add(Convert.ToString(row["ElasticPool"]), (new Dictionary<string, DataRowView>() { }));
                                    selectedElasticPools[Convert.ToString(row["ElasticPool"])].Add(Convert.ToString(row["DatabaseName"]), row);
                                }
                            }
                        }
                        else
                            break;
                    }

                    /*Processing Elastic pools in the Databases 
                      The database grid is binded with rows and showing specific columns of rows on UI,the columns are  
                      A-DatabaseNames
                      B-ValueBars.
                      The new rows created for elastic pools have Database name=EalsticPoolName and Values as aggregates.
                    */
                    foreach (var elasticPool in selectedElasticPools)
                    {
                        var desRow = lastSnapshot.NewRow();
                        var sourceRow = elasticPool.Value.Values.FirstOrDefault().Row;
                        desRow.ItemArray = sourceRow.ItemArray.Clone() as object[];
                        desRow["DatabaseName"] = Convert.ToString(elasticPool.Key);

                        long aggregateLong = 0;
                        decimal aggregateDecimal = 0;
                        foreach (var databaseRow in elasticPool.Value)
                        {
                            var row = databaseRow.Value;
                            if(selectedCol.Equals("TransactionsPerSec")||selectedCol.Equals("LogFlushesPerSec"))
                            {
                                long val;
                                if (long.TryParse(row[selectedCol].ToString(), out val))
                                {
                                    aggregateLong =aggregateLong + val;
                                }
                            }
                            else
                            {
                                decimal decimalVal;
                                if (decimal.TryParse(row[selectedCol].ToString(), out decimalVal))
                                {
                                    aggregateDecimal = aggregateDecimal + decimalVal;
                                }

                            }

                        }
                        if (selectedCol.Equals("TransactionsPerSec") || selectedCol.Equals("LogFlushesPerSec"))
                        {
                            desRow[selectedCol] = aggregateLong;
                        }
                        else
                            desRow[selectedCol] = aggregateDecimal;
                        lastSnapshot.Rows.Add(desRow);
                    }
                    // End: Elastic Pool Support-Nikhil
                    lastSnapshot.EndLoadData();
                }

                // sort descending by selected column
                lastSnapshot.DefaultView.Sort = string.Format("{0} desc", selectedCol);

                topDatabasesGrid.SetDataBinding(lastSnapshot.DefaultView, null);

                foreach (UltraGridColumn col in topDatabasesGrid.DisplayLayout.Bands[0].Columns)
                {
                    if (col.Key == "DatabaseName" || col.Key == "ValueBar" || col.Key == selectedCol)
                    {
                        col.Hidden = false;
                    }
                    else
                    {
                        col.Hidden = true;
                    }
                }

                topDatabasesGrid.DisplayLayout.Bands[0].Columns[selectedCol].PerformAutoResize(PerformAutoSizeType.AllRowsInBand);
                topDatabasesGrid.DisplayLayout.Bands[0].Columns["DatabaseName"]
                    .Width = 166 - topDatabasesGrid.DisplayLayout.Bands[0].Columns[selectedCol].Width;

                double topValue = 1;
                if (topDatabasesGrid.Rows.Count > 0)
                {
                    if (topDatabasesGrid.Rows[0].Cells[selectedCol].Value != DBNull.Value)
                    {
                        topValue = Convert.ToDouble(topDatabasesGrid.Rows[0].Cells[selectedCol].Value);
                        if (topValue == 0)
                        {
                            topValue = 1;
                        }
                    }
                    foreach (UltraGridRow row in topDatabasesGrid.Rows)
                    {
                        row.Cells["ValueBar"].Value = row.Cells[selectedCol].Value == DBNull.Value
                                                          ? 0
                                                          : Math.Round(
                                                              Convert.ToDouble(row.Cells[selectedCol].Value) / topValue, 2);
                        if (row.Cells[selectedCol].Value == DBNull.Value)
                        {
                            row.Cells[selectedCol].ToolTipText = "N/A";
                            row.CellAppearance.BackColor = Color.WhiteSmoke;
                        }
                    }

                    selectedDatabases.Clear();
                    for (int idx = 0; idx < Math.Min(5, topDatabasesGrid.DisplayLayout.Rows.Count); idx++)
                    {
                        string dbName = topDatabasesGrid.DisplayLayout.Rows[idx].Cells["DatabaseName"].Text;
                        if (dbName != string.Empty)
                            selectedDatabases.Add(dbName);
                    }
                }

                topDatabasesGrid.ResumeLayout();
            }
        }

        private void InitializeDatabasesChart()
        {
            databaseChart.Printer.Orientation = PageOrientation.Landscape;
            databaseChart.Printer.Compress = true;
            databaseChart.Printer.ForceColors = true;
            databaseChart.Printer.Document.DocumentName = "Database Trends Chart";
            databaseChart.ToolTipFormat = string.Format("%s\n%v {0}\n%x", headerSelectTypeDropDownButton.Text);
        }

        private void ConfigureDatabasesChart()
        {
            databaseChart.SuspendLayout();
            databaseChart.DataSourceSettings.Fields.Clear();
            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            dateFieldMap.DisplayName = "Date";
            databaseChart.DataSourceSettings.Fields.Add(dateFieldMap);

            string selectedCol = ApplicationHelper.GetEnumDescription((dbDataColumn)selectedType);
            if (selectedDatabases.Count > 0)
            {
                for (int idx = 1; idx <= selectedDatabases.Count; idx++)
                {
                    string database = selectedDatabases[idx - 1];
                    FieldMap fieldMap = new FieldMap(selectedCol + idx, FieldUsage.Value);
                    fieldMap.DisplayName = database;
                    databaseChart.DataSourceSettings.Fields.Add(fieldMap);
                }

                databaseChart.DataSourceSettings.ReloadData();
                databaseChart_Resize(databaseChart, new EventArgs());
                int showDecimals = 0;
                if (selectedType == (int)dbDataColumn.Reads || selectedType == (int)dbDataColumn.Writes)
                {
                    showDecimals = 2;
                }
                databaseChart.AxisY.DataFormat.Decimals = showDecimals;
                databaseChart.ToolTipFormat = string.Format("%s\n%v {0}\n%x", headerSelectTypeDropDownButton.Text);
                databaseChart.Invalidate();
            }
            databaseChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            databaseChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            databaseChart.ResumeLayout();
        }

        public override void ApplySettings()
        {
            //if (Enum.IsDefined(typeof(dbChartType), Settings.Default.DashboardDatabasesControlSelectedChart))
            //{
            //    foreach (ToolStripMenuItem item in headerSelectTypeDropDownButton.DropDownItems)
            //    {
            //        if (item.Tag != null && item.Tag is int)
            //        {
            //            if ((int)item.Tag == Settings.Default.DashboardDatabasesControlSelectedChart)
            //            {
            //                selectedType = Settings.Default.DashboardDatabasesControlSelectedChart;
            //                headerSelectTypeDropDownButton.Text = headerSelectTypeDropDownButton.DropDownItems[selectedType].Text;
            //                ConfigureDataSource();
            //                break;
            //            }
            //        }
            //    }
            //}
        }

        internal override void ConfigureDataSource()
        {
            using (Log.VerboseCall("ConfigureDataSource"))
            {

                // update the grid
                ConfigureDatabasesGrid();

                try
                {
                    top5DataTable.BeginLoadData();
                    top5DataTable.Rows.Clear();

                    // SQLDM 10.2 (Anshul Aggarwal) - Added null checks
                    // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                    DataTable historyDataTable = historyData != null ? historyData.CurrentDbSnapshotsDataTable : null;
                    if (historyDataTable != null && selectedDatabases.Count > 0)
                    {
                        // update the databases chart
                        string selectedCol = ApplicationHelper.GetEnumDescription((dbDataColumn)selectedType);
                        StringBuilder dbs = new StringBuilder();
                        foreach (string db in selectedDatabases)
                        {
                            if (dbs.Length > 0)
                                dbs.Append(",");
                            dbs.AppendFormat("'{0}'", db.Replace("'", "''"));
                        }

                        string viewFilter = string.Format("DatabaseName in ({0})", dbs);
                        if (historyData.HistoricalSnapshotDateTime == null)
                        {
                            DateTime dateLimit = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                            viewFilter = String.Format("Date > #{0:o}# AND {1}", dateLimit, viewFilter);
                        }

                        DataView currentData = new DataView(historyDataTable,  // SQLDM-27456 - History Range control_DC: Multiple errors displayed when custom range is used
                                                            viewFilter,
                                                            "Date",
                                                            DataViewRowState.CurrentRows);

                        DateTime top5RowDate = DateTime.MinValue;
                        DataRow top5Row = null;
                        Dictionary<string, HashSet<string>> poolAggregate = new Dictionary<string, HashSet<string>>() ;

                        // Pre-Processing Elastic Pool Information for Databases:
                        foreach (DataRowView row in currentData)
                        {
                            if (row["ElasticPool"] != string.Empty && row["ElasticPool"] != null && Convert.ToString(row["ElasticPool"]) != "" &&
                                row["DatabaseName"] != string.Empty && row["DatabaseName"] != null && Convert.ToString(row["DatabaseName"]) != "")
                            {


                                if (poolAggregate.ContainsKey(Convert.ToString(row["ElasticPool"])))
                                {
                                    if (!poolAggregate[Convert.ToString(row["ElasticPool"])].Contains(Convert.ToString(row["DatabaseName"])))
                                    {
                                        poolAggregate[Convert.ToString(row["ElasticPool"])].Add(Convert.ToString(row["DatabaseName"]));
                                        poolAggregate[Convert.ToString(row["ElasticPool"])].Add("DbTest");
                                    }

                                }
                                else
                                {
                                    poolAggregate.Add(Convert.ToString(row["ElasticPool"]), new HashSet<string>() { });
                                    poolAggregate[Convert.ToString(row["ElasticPool"])].Add(Convert.ToString(row["DatabaseName"]));
                                    poolAggregate[Convert.ToString(row["ElasticPool"])].Add("DbTest");
                                }
                            
                            }
                        }
                        long TransactionsPerSec = 0, LogFlushesPerSec = 0;
                        decimal NumberReadsPerSec = 0, NumberWritesPerSec = 0, IoStallMSPerSec = 0;
                        foreach (DataRowView row in currentData)
                        {
                            DateTime date = (DateTime) row["Date"];
                            if (top5Row == null || date != top5RowDate)
                            {
                                top5Row = top5DataTable.NewRow();
                                top5Row["Date"] = date;
                                top5DataTable.Rows.Add(top5Row);
                                top5RowDate = date;

                                TransactionsPerSec =LogFlushesPerSec = 0;
                                NumberReadsPerSec = NumberWritesPerSec = IoStallMSPerSec = 0;
                            }

                            int dbIndex = selectedDatabases.IndexOf(row["DatabaseName"].ToString());

                            if (dbIndex == -1)
                                continue;

                            dbIndex += 1;
                            
                            foreach (dbDataColumn col in Enum.GetValues(typeof (dbDataColumn)))
                            {
                                string columnName = ApplicationHelper.GetEnumDescription(col);
                                top5Row[columnName + dbIndex] = row[columnName];
                            }
                            // Start: Elastic Pool Support:Nikhil
                            if (poolAggregate.ContainsKey(row["ElasticPool"].ToString()))

                            {
                                if (poolAggregate[Convert.ToString(row["ElasticPool"])].Contains(Convert.ToString(row["DatabaseName"])))
                                {
                                    int dbindex = selectedDatabases.IndexOf(Convert.ToString(row["DatabaseName"])) + 1;
                                    if (!(top5Row["TransactionsPerSec" + dbindex] is DBNull))
                                        TransactionsPerSec += long.Parse(top5Row["TransactionsPerSec" + dbindex].ToString());

                                    if (!(top5Row["LogFlushesPerSec" + dbindex] is DBNull))
                                        LogFlushesPerSec += long.Parse(top5Row["LogFlushesPerSec" + dbindex].ToString());

                                    if (!(top5Row["NumberReadsPerSec" + dbindex] is DBNull))
                                        NumberReadsPerSec += decimal.Parse(top5Row["NumberReadsPerSec" + dbindex].ToString());

                                    if (!(top5Row["NumberWritesPerSec" + dbindex] is DBNull))
                                        NumberWritesPerSec += decimal.Parse(top5Row["NumberWritesPerSec" + dbindex].ToString());

                                    if (!(top5Row["IoStallMSPerSec" + dbindex] is DBNull))
                                        IoStallMSPerSec += decimal.Parse(top5Row["IoStallMSPerSec" + dbindex].ToString());
                                }
                             }


                            int poolIndex = selectedDatabases.IndexOf(Convert.ToString(row["ElasticPool"]));
                            if (poolIndex == -1)
                                continue;
                            poolIndex++;

                            foreach (dbDataColumn col in Enum.GetValues(typeof(dbDataColumn)))
                                {
                                    string columnName = ApplicationHelper.GetEnumDescription(col);
                                    switch (columnName)
                                    {
                                        case "TransactionsPerSec":
                                            top5Row[columnName + poolIndex] = TransactionsPerSec;
                                            break;
                                        case "LogFlushesPerSec":
                                            top5Row[columnName + poolIndex] = LogFlushesPerSec;
                                            break;
                                        case "NumberReadsPerSec":
                                            top5Row[columnName + poolIndex] = NumberReadsPerSec;
                                            break;
                                        case "NumberWritesPerSec":
                                            top5Row[columnName + poolIndex] = NumberWritesPerSec;
                                            break;
                                        case "IoStallMSPerSec":
                                            top5Row[columnName + poolIndex] = IoStallMSPerSec;
                                            break;
                                        default:
                                            break;
                                    }

                                }
                                // End: Elastic Pool Support:Nikhil

                            }

                    }
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "An error occurrred determining the top 5 databases", ex);
                }
                finally
                {
                    top5DataTable.EndLoadData();
                }
                databaseChart.DataSource = top5DataTable;
                ChartFxExtensions.SetAxisXTimeScale(databaseChart, 2);
                ConfigureDatabasesChart();

                Invalidate();
            }
        }

        #region toolbar

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            if (designMode)
            {
                e.Cancel = true;
                return;
            }

            if (e.SourceControl is Chart)
            {
                contextControl = e.SourceControl;
            }
            else if (e.SourceControl is UltraGrid)
            {
                contextControl = e.SourceControl;
            }
        }

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "configureAlertsButton":
                    ConfigureControlAlerts(contextControl);
                    break;
                case "showHelpButton":
                    ShowControlHelp(contextControl);
                    break;
                case "showDetailsButton":
                    ShowControlDetails(contextControl);
                    break;
                case "printChartButton":
                    PrintChart(contextControl);
                    break;
                case "exportChartDataButton":
                    SaveChartData(contextControl);
                    break;
                case "exportChartImageButton":
                    SaveChartImage(contextControl);
                    break;
            }
            contextControl = null;
        }

        #endregion

        #region Chart Actions

        private void ConfigureControlAlerts(Control targetControl)
        {
            if (targetControl != null)
            {
                //try
                //{
                //    AlertConfigurationDialog dialog = new AlertConfigurationDialog(instanceId, false);

                //    if (targetControl == throughputChart)
                //    {
                //        dialog.Select(Metric.ServerResponseTime);
                //    }
                //    else if (targetControl == responseTimeGauge)
                //    {
                //        dialog.Select(Metric.ServerResponseTime);
                //    }

                //    dialog.ShowDialog(ParentForm);
                //}
                //catch (Exception ex)
                //{
                //    ApplicationMessageBox.ShowError(this,
                //                                    "Unable to retrieve the alert configuration from the SQLdm Repository.  Please resolve the following error and try again.",
                //                                    ex);
                //}
            }
        }

        private void ShowControlHelp(Control targetControl)
        {
            string topic = string.Empty;

            switch ((dbChartType)selectedType)
            {
                case dbChartType.Transactions:
                    topic = HelpTopics.ServerDashboardViewDatabasesPanelTrans;
                    break;
                case dbChartType.LogFlushes:
                    topic = HelpTopics.ServerDashboardViewDatabasesPanelLogFlushes;
                    break;
                case dbChartType.Reads:
                    topic = HelpTopics.ServerDashboardViewDatabasesPanelReads;
                    break;
                case dbChartType.Writes:
                    topic = HelpTopics.ServerDashboardViewDatabasesPanelWrites;
                    break;
                case dbChartType.IoStall:
                    topic = HelpTopics.ServerDashboardViewDatabasesPanelIoStall;
                    break;
            }

            ShowHelp(topic);
        }

        private void ShowControlDetails(Control targetControl)
        {
            if (targetControl != null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesSummary);
            }
        }

        private void PrintChart(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
            }
        }

        private void SaveChartData(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
            }
        }

        private void SaveChartImage(Control control)
        {
            if (control is Chart)
            {
                Chart chart = (Chart)control;
                string title = string.Format(chartTitleFormat, this.Caption, chart.Tag.ToString());
                ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
            }
        }

        #endregion

        #region Chart Click Events

        private void chart_MouseClick(object sender, HitTestEventArgs e)
        {
            if (designMode)
            {
                DashboardControl_MouseClick(sender, e);
                return;
            }

            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ShowControlDetails((Chart)sender);
            }
        }

        #endregion

        private void databaseChart_Resize(object sender, EventArgs e)
        {
            Chart chart = (Chart)sender;
            int maxLegendWidth = chart.Width / 3;

            chart.LegendBox.AutoSize = true;
            chart.UpdateSizeNow();
            if (chart.LegendBox.Width > maxLegendWidth)
            {
                chart.LegendBox.Width = maxLegendWidth;
                chart.Invalidate();
            }
        }

        private void topDatabasesGrid_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            UltraGridBand band = topDatabasesGrid.DisplayLayout.Bands[0];

            band.Columns["DatabaseName"].CellAppearance.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
        }
    }

    internal sealed class PercentageBackgroundDrawFilter : IUIElementDrawFilter
    {
        private readonly IUIElementDrawFilter previousFilter;

        internal PercentageBackgroundDrawFilter()
        {
            this.previousFilter = null;
        }

        internal PercentageBackgroundDrawFilter(IUIElementDrawFilter previousFilter)
        {
            this.previousFilter = previousFilter;    
        }

        public bool DrawElement(DrawPhase drawPhase, ref UIElementDrawParams drawParams)
        {
            if (drawPhase == DrawPhase.BeforeDrawFocus ||
                drawPhase == DrawPhase.BeforeDrawForeground)
            {
                return true;
            }

            if (drawPhase == DrawPhase.BeforeDrawBackColor)
            {
                if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
                {
                    CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                    if (cell.Column.Key == "ValueBar")
                    {
                        Rectangle rect = (Rectangle)drawParams.Element.RectInsideBorders;
                        using (Brush brush = new SolidBrush(Color.White))
                        {
                            drawParams.Graphics.FillRectangle(brush, rect);
                        }

                        rect.Width = Convert.ToInt32(rect.Width * Convert.ToDouble(cell.Cell.Value));
                        if (rect.Width > 0)
                        {
                            //drawParams.Graphics.FillRectangle(drawParams.BackBrush, rect);
                            using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, cell.Column.CellAppearance.BackColor, cell.Column.CellAppearance.BackColor2, LinearGradientMode.Vertical))
                            {
                                drawParams.Graphics.FillRectangle(brush, rect);
                            }
                        }
                    }
                }
                return true;
            }

            if (previousFilter != null)
                return previousFilter.DrawElement(drawPhase, ref drawParams);

            return false;
        }

        public DrawPhase GetPhasesToFilter(ref UIElementDrawParams drawParams)
        {
            DrawPhase result = DrawPhase.BeforeDrawFocus;

            if (drawParams.Element is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextUIElement && drawParams.Element.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (drawParams.Element is EditorWithTextDisplayTextUIElement && drawParams.Element.Parent.Parent is CellUIElement)
            {
                CellUIElement cell = (CellUIElement)drawParams.Element.Parent.Parent;
                if (cell.Column.Key == "ValueBar")
                    result = DrawPhase.BeforeDrawBackColor | DrawPhase.BeforeDrawForeground;
            }

            if (previousFilter != null)
                result |= previousFilter.GetPhasesToFilter(ref drawParams);

            return result;
        }
    }
}
