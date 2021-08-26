using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using System.Globalization;
using System.Collections.Generic;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.Common.Events.AzureMonitor;
using Idera.SQLdm.Common.Events.AzureMonitor.Interfaces;
using Idera.SQLdm.Common.Data;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesSummaryView : ServerBaseView
    {
        private readonly ServerSummaryHistoryData historyData;
        //private readonly MonitoredSqlServerInstancePropertiesDialog monitoredSqlServerInstanceProperties;
        private DataTable currentDbSnapshotsDataTable;
        private Dictionary<String, DatabaseStatistics> dbstats;
        private static readonly object updateLock = new object();
        private Chart contextMenuSelectedChart = null;
        private Exception historyModeLoadError = null;
        private Dictionary<string, DatabaseStatistics> azureDbDetailDictionary;
        private String selectedCpuDb = null;
        private String selectedDb = null;
        private DateTime? dateTimeCpuChart = null;
        DataTable cpuTb = new DataTable();
        DataRow cpuDr;
        private TextItem<IAzureApplicationProfile> selectedProfile;

        public ResourcesSummaryView(int instanceId, ServerSummaryHistoryData historyData)
            : base(instanceId)
        {
            RefreshReportsProgress = true;
            this.historyData = historyData;

            InitializeComponent();
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                //this.tableLayoutPanel.Controls.Clear();
                //this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 0);
                //this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 1, 0);
                //this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 0, 1);
                //this.tableLayoutPanel.Controls.Add(this.sqlServerReadsWritesPanel, 1, 1);
                //this.tableLayoutPanel.RowCount = 2;
                //this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
                //this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
                SetAmazonRDSResourcesPanel();
            }

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId ||
                ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                SetAzureResourcesPanel();
            }

            ChartFxExtensions.SetContextMenu(cacheHitRatesChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(callRatesChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(cpuUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(diskUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(memoryUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(sqlServerReadsWritesChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(vmMemUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(hostMemUsageChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(VmAvaiableByteHyperVChart, toolbarsManager);
            ChartFxExtensions.SetContextMenu(HostAvaialbleByteHyperVChart, toolbarsManager);
            InitializeCharts();
            initlizeCpuTableColumns();
            Settings.Default.PropertyChanged += Settings_PropertyChanged;
            MonitoredSqlServerWrapper instanceObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ShowVMData(instanceObject.Instance.IsVirtualized);
            AdaptFontSize();
        }
        private void initlizeCpuTableColumns()
        {
            cpuTb.Columns.Add("Date", typeof(DateTime));
            cpuTb.Columns.Add("TotalCpuUsage", typeof(decimal));
        }
        private void addItemsToCpuMenu(ServerSummarySnapshots snapshots)
        {
            this.azureDbDetailDictionary = snapshots.ServerOverviewSnapshot.DbStatistics;
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            {
                List<String> dropdownItem = new List<String>();
                cpuDataBaseDropDownButton.DropDownItems.Clear();
                foreach (var dist in this.azureDbDetailDictionary)
                {
                    if (!string.Equals(dist.Key, "tempdb", StringComparison.CurrentCultureIgnoreCase))
                    {
                        String dbName = dist.Key;
                        ToolStripMenuItem item = new ToolStripMenuItem(dbName);
                        cpuDataBaseDropDownButton.DropDownItems.Add(item);
                        cpuDataBaseDropDownButton.Text = String.IsNullOrEmpty(selectedCpuDb) ? item.Text : selectedCpuDb;
                        selectedCpuDb = cpuDataBaseDropDownButton.Text;
                        item.Click += new EventHandler(cpu_database_clicked);
                    }
                }
            }
            else
            {
                cpuDataBaseDropDownButton.Visible = false;
            }
        }

        private void cpu_database_clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            cpuDataBaseDropDownButton.Text = item.Text;
            selectedCpuDb = item.Text;
            ConfigureCpuuChartDataSource(null);
        }

        private void addItemsToMenu(ServerSummarySnapshots snapshots)
        {
            dbstats = snapshots.ServerOverviewSnapshot.DbStatistics;
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            {
                List<String> dropdownItem = new List<String>();
                dataBaseDropDownButton.DropDownItems.Clear();
                foreach (var dist in dbstats)
                {
                    String dbName = dist.Key;
                    if (dbName != "master" && dbName != "tempdb")
                    {
                        ToolStripMenuItem item = new ToolStripMenuItem(dbName);
                        dataBaseDropDownButton.DropDownItems.Add(item);
                        dataBaseDropDownButton.Text = String.IsNullOrEmpty(selectedDb) ? item.Text : selectedDb;
                        selectedDb = dataBaseDropDownButton.Text;
                        item.Click += new EventHandler(database_clicked);
                    }
                }
            }
            else
            {
                dataBaseDropDownButton.Visible = false;
            }
        }

        private DataTable filterDataTable()
        {
            DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
            DataView dv = new DataView(currentDbSnapshotsDataTable);
            if (historyData.HistoricalSnapshotDateTime != null) // Ishistorical
            {
                dv.RowFilter = string.Format("DatabaseName = '{0}'", dataBaseDropDownButton.Text);
            }
            else
            {
                dv.RowFilter = string.Format("Date > #{0}# AND DatabaseName = '{1}'", viewFilter.ToString(CultureInfo.InvariantCulture), dataBaseDropDownButton.Text);
            }

            if (dv.Count > 0)
            {
                var limit = String.IsNullOrEmpty(dv[dv.Count - 1]["AzureCloudStorageLimit"].ToString()) ? "-" : dv[0]["AzureCloudStorageLimit"].ToString();
                memoryUsageChart.Titles[0].Text = "Storage Size Limit:" + limit + "GB";
                return dv.ToTable();
            }
            return null;
        }


        /// <summary>
        /// Start datetime for custom range, otherwise it is null.
        /// </summary>
        public override DateTime? HistoricalStartDateTime
        {
            get { return historyData.HistoricalStartDateTime; }
            set
            {
                if (historyData.HistoricalStartDateTime != value)
                {
                    historyData.HistoricalStartDateTime = value;
                    currentHistoricalStartDateTime = null;
                }
            }
        }

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historyData.HistoricalSnapshotDateTime; }
            set
            {
                historyData.HistoricalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }

        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChartDataFilter();
                    break;
            }
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesSummaryView);
        }

        #region Initialize Charts

        public void InitializeCharts()
        {
            InitializeCpuUsageChart();
            InitializeMemoryUsageChart();
            InitializeVmMemUsageChart();
            InitializeHostMemUsageChart();
            InitializeDiskUsageChart();
            InitializeCallRatesChart();
            InitializeCacheHitRatesChart();
            InitializeSqlServerReadsWritesChart();
            InitializeVmAvaiableByteHyperVChart();
            InitializeHostAvaialbleByteHyperVChart();
            ForceChartColors();
            drilldownProperties.ChartCursor = Cursors.Hand;
            InitalizeDrilldown(cpuUsageChart, memoryUsageChart, vmMemUsageChart, hostMemUsageChart, diskUsageChart,  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
                callRatesChart, cacheHitRatesChart, sqlServerReadsWritesChart, VmAvaiableByteHyperVChart, HostAvaialbleByteHyperVChart);
        }

        private void ForceChartColors()
        {
            cpuUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            callRatesChart.SetAreaSeriesAlphaChannel(175, 0);
            diskUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            hostMemUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            vmMemUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            memoryUsageChart.SetAreaSeriesAlphaChannel(175, 0);
            VmAvaiableByteHyperVChart.SetAreaSeriesAlphaChannel(175, 0);
            HostAvaialbleByteHyperVChart.SetAreaSeriesAlphaChannel(175, 0);
        }

        private void InitializeCpuUsageChart()
        {
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                cpuUsageChart.Tag = cpuStatusHeaderStripLabel;
                cpuUsageChart.Printer.Orientation = PageOrientation.Landscape;
                cpuUsageChart.Printer.Compress = true;
                cpuUsageChart.Printer.ForceColors = true;
                cpuUsageChart.Printer.Document.DocumentName = "CPU Usage Chart";
                cpuUsageChart.ToolBar.RemoveAt(0);
                cpuUsageChart.ToolTipFormat = "%s\n%v%%\n%x";


                if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId)
                {
                    FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                    FieldMap totalCpuUsageFieldMap = new FieldMap("TotalCpuUsage", FieldUsage.Value);
                    totalCpuUsageFieldMap.DisplayName = "SQL Server";
                    cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                         {
                                                                     dateFieldMap,
                                                                     totalCpuUsageFieldMap
                                                                         });
                    this.cpuUsageChart.AxisY.Title.Text = "VCORE Usage %";
                }
                else
                {
                    FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                    FieldMap totalCpuUsageFieldMap = new FieldMap("TotalCpuUsage", FieldUsage.Value);
                    totalCpuUsageFieldMap.DisplayName = "OS";
                    FieldMap sqlServerCpuUsageFieldMap = new FieldMap("SqlServerCpuUsage", FieldUsage.Value);
                    sqlServerCpuUsageFieldMap.DisplayName = "SQL Server";
                    FieldMap vmCpuUsageFieldMap = new FieldMap("vmCPUUsage", FieldUsage.Value);
                    vmCpuUsageFieldMap.DisplayName = "VM";
                    FieldMap esxCpuUsageFieldMap = new FieldMap("esxCPUUsage", FieldUsage.Value);
                    esxCpuUsageFieldMap.DisplayName = "Host";
                    cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                                            {
                                                                     dateFieldMap,
                                                                     vmCpuUsageFieldMap,
                                                                     esxCpuUsageFieldMap,
                                                                     sqlServerCpuUsageFieldMap,
                                                                     totalCpuUsageFieldMap
                                                                                            });
                }
                ChartFxExtensions.SetAreaSeriesAlphaChannel(cpuUsageChart, 175);
                cpuUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                cpuUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                cpuUsageChart.AxisY.Min = 0;
                cpuUsageChart.AxisY.Max = 100;
                cpuUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            }

            else
            {
                cpuUsageChart.Tag = cpuStatusHeaderStripLabel;
                cpuUsageChart.Printer.Orientation = PageOrientation.Landscape;
                cpuUsageChart.Printer.Compress = true;
                cpuUsageChart.Printer.ForceColors = true;
                cpuUsageChart.Printer.Document.DocumentName = "CPU Usage Chart";
                cpuUsageChart.ToolBar.RemoveAt(0);
                cpuUsageChart.ToolTipFormat = "%s\n%v%%\n%x";
                //String db = dataBaseDropDownButton.Text;
                cpuUsageChart.DataSourceSettings.Fields.Clear();


                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                FieldMap totalCpuUsageFieldMap = new FieldMap("AvgCpuPercent", FieldUsage.Value);
                totalCpuUsageFieldMap.DisplayName = "SQL Server";
                cpuUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                     {
                                                                     dateFieldMap,
                                                                     totalCpuUsageFieldMap
                                                                     });

                ChartFxExtensions.SetAreaSeriesAlphaChannel(cpuUsageChart, 175);

                cpuUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
                cpuUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
                cpuUsageChart.AxisY.Min = 0;
                cpuUsageChart.AxisY.Max = 100;
                // setMemDropDownSelection(sqlServerMenuItem);
                cpuUsageChart.DataSource = cpuTb;
            }
        }

        private void InitializeMemoryUsageChart()
        {
            var managedInstance = ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId;
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId
                && ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureManagedInstanceId)
            {
                memoryUsageChart.Tag = memoryUsageHeaderStripLabel.Text.Trim() + "-SQLServer";
                memoryUsageChart.Printer.Orientation = PageOrientation.Landscape;
                memoryUsageChart.Printer.Compress = true;
                memoryUsageChart.Printer.ForceColors = true;
                memoryUsageChart.Printer.Document.DocumentName = "Memory Usage Chart";
                memoryUsageChart.ToolBar.RemoveAt(0);
                memoryUsageChart.ToolTipFormat = "%s\n%v MB\n%x";
                memoryUsageChart.DataSourceSettings.Fields.Clear();
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                FieldMap sqlServerMemoryUsedFieldMap = new FieldMap("SqlServerMemoryUsed", FieldUsage.Value);
                sqlServerMemoryUsedFieldMap.DisplayName = "SQL Used";
                FieldMap sqlServerMemoryAllocatedFieldMap = new FieldMap("SqlServerMemoryAllocated", FieldUsage
                    .Value);
                sqlServerMemoryAllocatedFieldMap.DisplayName = "SQL Allocated";
                FieldMap totalMemoryUsedFieldMap = new FieldMap("OsMemoryUsed", FieldUsage.Value);
                totalMemoryUsedFieldMap.DisplayName = "Total Used";
                memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap,
                                                                        totalMemoryUsedFieldMap
                                                                        });


                memoryUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
            }
            else
            {
                //SQLDM 11 6.2.3

                memoryUsageChart.Tag = memoryUsageHeaderStripLabel.Text.Trim() + "-SQLServer";
                memoryUsageChart.Printer.Orientation = PageOrientation.Landscape;
                memoryUsageChart.Printer.Compress = true;
                memoryUsageChart.Printer.ForceColors = true;
                memoryUsageChart.Printer.Document.DocumentName = "Database Data Storage";
                memoryUsageChart.ToolBar.RemoveAt(0);
                memoryUsageChart.ToolTipFormat = "%s\n%v MB\n%x";
                memoryUsageChart.DataSourceSettings.Fields.Clear();
                FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
                FieldMap sqlServerMemoryUsedFieldMap, sqlServerMemoryAllocatedFieldMap;
                if (managedInstance)
                {
                    sqlServerMemoryUsedFieldMap = new FieldMap("SqlServerMemoryUsed", FieldUsage.Value);
                    sqlServerMemoryUsedFieldMap.DisplayName = "Used space";
                    sqlServerMemoryAllocatedFieldMap = new FieldMap("SqlServerMemoryAllocated", FieldUsage.Value);
                    sqlServerMemoryAllocatedFieldMap.DisplayName = "Allocated space";
                }
                else
                {
                    sqlServerMemoryUsedFieldMap = new FieldMap("AzureCloudUsedMemory", FieldUsage.Value);
                    sqlServerMemoryUsedFieldMap.DisplayName = "Used space";

                    sqlServerMemoryAllocatedFieldMap = new FieldMap("AzureCloudAllocatedMemory", FieldUsage.Value);
                    sqlServerMemoryAllocatedFieldMap.DisplayName = "Allocated space";
                }
                memoryUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                    {
                                                                        dateFieldMap,
                                                                        sqlServerMemoryUsedFieldMap,
                                                                        sqlServerMemoryAllocatedFieldMap
                                                                    });
                memoryUsageChart.Gallery = Gallery.Area;
                TitleDockable title1 = new TitleDockable();
                title1.Alignment = StringAlignment.Far;
                title1.Text = "Storage Size Limit:";
                memoryUsageChart.Titles.Add(title1);
                memoryUsageChart.AxisX.Title.Alignment = StringAlignment.Center;
                memoryUsageChart.AxisX.Title.LineAlignment = StringAlignment.Center;
                memoryUsageChart.AxisY.Title.Text = "MB";

                memoryUsageChart.DataSource = managedInstance ? historyData.RealTimeSnapshotsDataTable : currentDbSnapshotsDataTable;
            }
            ChartFxExtensions.SetAreaSeriesAlphaChannel(memoryUsageChart, 175);

            memoryUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            memoryUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;

            setMemDropDownSelection(sqlServerMenuItem);
        }

        private void InitializeVmMemUsageChart()
        {
            vmMemUsageChart.Tag = memoryUsageHeaderStripLabel.Text.Trim() + "-VM";
            vmMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            vmMemUsageChart.Printer.Compress = true;
            vmMemUsageChart.Printer.ForceColors = true;
            vmMemUsageChart.Printer.Document.DocumentName = "VM Memory Usage Chart";
            vmMemUsageChart.ToolBar.RemoveAt(0);
            vmMemUsageChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var vmMemActiveFieldMap = new FieldMap("vmMemActive", FieldUsage.Value);
            vmMemActiveFieldMap.DisplayName = "Active";
            var vmMemConsumedFieldMap = new FieldMap("vmMemConsumed", FieldUsage.Value);
            vmMemConsumedFieldMap.DisplayName = "Consumed";
            var vmMemGrantedFieldMap = new FieldMap("vmMemGranted", FieldUsage.Value);
            vmMemGrantedFieldMap.DisplayName = "Granted";
            var vmMemBalloonedFieldMap = new FieldMap("vmMemBallooned", FieldUsage.Value);
            vmMemBalloonedFieldMap.DisplayName = "Ballooned";
            var vmMemSwappedFieldMap = new FieldMap("vmMemSwapped", FieldUsage.Value);
            vmMemSwappedFieldMap.DisplayName = "Swapped";

            vmMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            vmMemActiveFieldMap,
                                                                            vmMemConsumedFieldMap,
                                                                            vmMemGrantedFieldMap,
                                                                            vmMemBalloonedFieldMap,
                                                                            vmMemSwappedFieldMap
                                                                        });

            vmMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            vmMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            vmMemUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeHostMemUsageChart()
        {
            hostMemUsageChart.Tag = memoryUsageHeaderStripLabel.Text.Trim() + "-Host";
            hostMemUsageChart.Printer.Orientation = PageOrientation.Landscape;
            hostMemUsageChart.Printer.Compress = true;
            hostMemUsageChart.Printer.ForceColors = true;
            hostMemUsageChart.Printer.Document.DocumentName = "Host Memory Usage Chart";
            hostMemUsageChart.ToolBar.RemoveAt(0);
            hostMemUsageChart.ToolTipFormat = "%s\n%v MB\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var hostMemActiveFieldMap = new FieldMap("esxMemActive", FieldUsage.Value);
            hostMemActiveFieldMap.DisplayName = "Active";
            var hostMemConsumedFieldMap = new FieldMap("esxMemConsumed", FieldUsage.Value);
            hostMemConsumedFieldMap.DisplayName = "Consumed";
            var hostMemGrantedFieldMap = new FieldMap("esxMemGranted", FieldUsage.Value);
            hostMemGrantedFieldMap.DisplayName = "Granted";
            var hostMemBalloonedFieldMap = new FieldMap("esxMemBallooned", FieldUsage.Value);
            hostMemBalloonedFieldMap.DisplayName = "Ballooned";
            var hostMemSwappedFieldMap = new FieldMap("esxMemSwapped", FieldUsage.Value);
            hostMemSwappedFieldMap.DisplayName = "Swapped";

            hostMemUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                        {
                                                                            dateFieldMap,
                                                                            hostMemActiveFieldMap,
                                                                            hostMemConsumedFieldMap,
                                                                            hostMemGrantedFieldMap,
                                                                            hostMemBalloonedFieldMap,
                                                                            hostMemSwappedFieldMap
                                                                        });

            hostMemUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            hostMemUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            hostMemUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeDiskUsageChart()
        {
            diskUsageChart.Tag = diskUsageHeaderStripLabel;
            diskUsageChart.Printer.Orientation = PageOrientation.Landscape;
            diskUsageChart.Printer.Compress = true;
            diskUsageChart.Printer.ForceColors = true;
            diskUsageChart.Printer.Document.DocumentName = "Disk Usage Chart";
            diskUsageChart.ToolBar.RemoveAt(0);
            diskUsageChart.ToolTipFormat = "%v%s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap diskTimeFieldMap = new FieldMap("PercentDiskTime", FieldUsage.Value);
            diskTimeFieldMap.DisplayName = "% Disk Time";

            diskUsageChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      diskTimeFieldMap
                                                                  });

            ChartFxExtensions.SetAreaSeriesAlphaChannel(diskUsageChart, 175);

            diskUsageChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            diskUsageChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            diskUsageChart.AxisY.Min = 0;
            diskUsageChart.AxisY.Max = 100;
            diskUsageChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeCallRatesChart()
        {
            callRatesChart.Tag = callRatesHeaderStripLabel;
            callRatesChart.Printer.Orientation = PageOrientation.Landscape;
            callRatesChart.Printer.Compress = true;
            callRatesChart.Printer.ForceColors = true;
            callRatesChart.Printer.Document.DocumentName = "Call Rates Chart";
            callRatesChart.ToolBar.RemoveAt(0);
            callRatesChart.ToolTipFormat = "%v %s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap batchesFieldMap = new FieldMap("CallRatesBatches", FieldUsage.Value);
            batchesFieldMap.DisplayName = "Batches";
            FieldMap compilesFieldMap = new FieldMap("CallRatesCompiles", FieldUsage.Value);
            compilesFieldMap.DisplayName = "Compiles";
            FieldMap recompilesFieldMap = new FieldMap("CallRatesReCompiles", FieldUsage.Value);
            recompilesFieldMap.DisplayName = "Re-Compiles";
            FieldMap transactionsFieldMap = new FieldMap("CallRatesTransactions", FieldUsage.Value);
            transactionsFieldMap.DisplayName = "Transactions";

            callRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                  {
                                                                      dateFieldMap,
                                                                      compilesFieldMap,
                                                                      recompilesFieldMap,
                                                                      batchesFieldMap,
                                                                      transactionsFieldMap
                                                                  });

            callRatesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            callRatesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            callRatesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeCacheHitRatesChart()
        {
            cacheHitRatesChart.Tag = cacheHitRatesHeaderStripLabel;
            cacheHitRatesChart.Printer.Orientation = PageOrientation.Landscape;
            cacheHitRatesChart.Printer.Compress = true;
            cacheHitRatesChart.Printer.ForceColors = true;
            cacheHitRatesChart.Printer.Document.DocumentName = "Cache Hit Rates Chart";
            cacheHitRatesChart.ToolBar.RemoveAt(0);
            cacheHitRatesChart.ToolTipFormat = "%s\n%v%%\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap bufferCacheFieldMap = new FieldMap("BufferCacheHitRatio", FieldUsage.Value);
            bufferCacheFieldMap.DisplayName = "Buffer Cache";
            FieldMap procedureCacheFieldMap = new FieldMap("ProcedureCacheHitRatio", FieldUsage.Value);
            procedureCacheFieldMap.DisplayName = "Procedure Cache";

            cacheHitRatesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                      {
                                                                          dateFieldMap,
                                                                          bufferCacheFieldMap,
                                                                          procedureCacheFieldMap
                                                                      });

            cacheHitRatesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            cacheHitRatesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            cacheHitRatesChart.AxisY.Min = 0;
            cacheHitRatesChart.AxisY.Max = 100;
            cacheHitRatesChart.AxisY.DataFormat.Decimals = 2;

            cacheHitRatesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeSqlServerReadsWritesChart()
        {
            sqlServerReadsWritesChart.Tag = sqlServerReadsWritesChartTypeButton;
            sqlServerReadsWritesChart.Printer.Orientation = PageOrientation.Landscape;
            sqlServerReadsWritesChart.Printer.Compress = true;
            sqlServerReadsWritesChart.Printer.ForceColors = true;
            sqlServerReadsWritesChart.Printer.Document.DocumentName = "SQL Server I/O Chart";
            sqlServerReadsWritesChart.ToolBar.RemoveAt(0);
            sqlServerReadsWritesChart.ToolTipFormat = "%v %s\n%x";

            FieldMap dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            FieldMap checkpointWritesFieldMap = new FieldMap("CheckpointWrites", FieldUsage.Value);
            checkpointWritesFieldMap.DisplayName = "Checkpoint Writes";
            FieldMap lazyWriterWritesFieldMap = new FieldMap("LazyWriterWrites", FieldUsage.Value);
            lazyWriterWritesFieldMap.DisplayName = "Lazy Writer Writes";
            FieldMap readAheadReadsFieldMap = new FieldMap("ReadAheadReads", FieldUsage.Value);
            readAheadReadsFieldMap.DisplayName = "Read Ahead Reads";
            FieldMap synchronousReadsFieldMap = new FieldMap("SynchronousReads", FieldUsage.Value);
            synchronousReadsFieldMap.DisplayName = "Page Reads";
            FieldMap synchronousWritesFieldMap = new FieldMap("SynchronousWrites", FieldUsage.Value);
            synchronousWritesFieldMap.DisplayName = "Page Writes";

            sqlServerReadsWritesChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                                             {
                                                                                 dateFieldMap,
                                                                                 checkpointWritesFieldMap,
                                                                                 lazyWriterWritesFieldMap,
                                                                                 readAheadReadsFieldMap,
                                                                                 synchronousReadsFieldMap,
                                                                                 synchronousWritesFieldMap
                                                                             });

            sqlServerReadsWritesChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            sqlServerReadsWritesChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            sqlServerReadsWritesChart.LegendBox.Width = 145;
            sqlServerReadsWritesChart.LegendBox.PlotAreaOnly = false;

            sqlServerReadsWritesChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        //Hyper V
        private void InitializeVmAvaiableByteHyperVChart()
        {
            VmAvaiableByteHyperVChart.Printer.Orientation = PageOrientation.Landscape;
            VmAvaiableByteHyperVChart.Printer.Compress = true;
            VmAvaiableByteHyperVChart.Printer.ForceColors = true;
            VmAvaiableByteHyperVChart.Printer.Document.DocumentName = "Memory Chart";
            VmAvaiableByteHyperVChart.ToolTipFormat = "VM %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var availByteFieldMap = new FieldMap("AvailableByteVm", FieldUsage.Value);
            availByteFieldMap.DisplayName = @"AvailableByte";

            VmAvaiableByteHyperVChart.DataSourceSettings.Fields.Clear();
            VmAvaiableByteHyperVChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   availByteFieldMap
                                                               });

            VmAvaiableByteHyperVChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            VmAvaiableByteHyperVChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            foreach (SeriesAttributes series in VmAvaiableByteHyperVChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
                series.Gallery = Gallery.Lines;
            }
            VmAvaiableByteHyperVChart.LegendBox.PlotAreaOnly = false;
            VmAvaiableByteHyperVChart.AxisY.Min = 0D;


            VmAvaiableByteHyperVChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }

        private void InitializeHostAvaialbleByteHyperVChart()
        {
            HostAvaialbleByteHyperVChart.Printer.Orientation = PageOrientation.Landscape;
            HostAvaialbleByteHyperVChart.Printer.Compress = true;
            HostAvaialbleByteHyperVChart.Printer.ForceColors = true;
            HostAvaialbleByteHyperVChart.Printer.Document.DocumentName = "Memory Chart";
            HostAvaialbleByteHyperVChart.ToolTipFormat = "Host %s\n%v MB\n%x";

            var dateFieldMap = new FieldMap("Date", FieldUsage.XValue);
            var availByteFieldMap = new FieldMap("AvailableByteHost", FieldUsage.Value);
            availByteFieldMap.DisplayName = @"AvailableByte";

            HostAvaialbleByteHyperVChart.DataSourceSettings.Fields.Clear();
            HostAvaialbleByteHyperVChart.DataSourceSettings.Fields.AddRange(new FieldMap[]
                                                               {
                                                                   dateFieldMap,
                                                                   availByteFieldMap
                                                               });

            HostAvaialbleByteHyperVChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            HostAvaialbleByteHyperVChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            foreach (SeriesAttributes series in HostAvaialbleByteHyperVChart.Series)
            {
                series.Color = Color.FromArgb(175, series.Color);
                series.Gallery = Gallery.Lines;
            }
            HostAvaialbleByteHyperVChart.LegendBox.PlotAreaOnly = false;
            HostAvaialbleByteHyperVChart.AxisY.Min = 0D;
            HostAvaialbleByteHyperVChart.DataSource = historyData.RealTimeSnapshotsDataTable;
        }


        private void ConfigureChartDataSource(DataTable dataSource)
        {
            // Update StorageLimit For Azure Managed Instance
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureManagedInstanceId && dataSource.Rows.Count > 0)
            {
                var limit = "" + dataSource.Rows[dataSource.Rows.Count - 1]["ManagedInstanceStorageLimit"];
                if (!String.IsNullOrEmpty(limit))
                    memoryUsageChart.Titles[0].Text = "Storage Size Limit:" + limit + "GB";
            }

            if (cpuUsageChart.DataSource == dataSource) return;

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
            {
                cpuUsageChart.DataSource = dataSource;
            }
            memoryUsageChart.DataSource = dataSource;
            diskUsageChart.DataSource = dataSource;
            callRatesChart.DataSource = dataSource;
            cacheHitRatesChart.DataSource = dataSource;
            sqlServerReadsWritesChart.DataSource = dataSource;
            vmMemUsageChart.DataSource = dataSource;
            hostMemUsageChart.DataSource = dataSource;
            VmAvaiableByteHyperVChart.DataSource = dataSource;
            HostAvaialbleByteHyperVChart.DataSource = dataSource;
            ForceChartColors();
            
        }

        private void ConfigureMemoryChartDataSource(ServerSummarySnapshots summarySnapshots = null)
        {
            if (summarySnapshots != null)
            {
                addItemsToMenu(summarySnapshots);
            }
            var filteredDataTable = filterDataTable();
            if (memoryUsageChart.DataSource == filteredDataTable) return;
            memoryUsageChart.DataSource = filteredDataTable;
        }

        private void ConfigureCpuuChartDataSource(ServerSummarySnapshots summarySnapshots = null)
        {
            if (summarySnapshots != null)
            {
                addItemsToCpuMenu(summarySnapshots);
            }
            var filteredDataTable = cpuFilterDataTable();
            if (cpuUsageChart.DataSource == filteredDataTable) return;
            if (filteredDataTable == null)
                cpuUsageChart.DataSource = cpuTb;
            else
                cpuUsageChart.DataSource = filteredDataTable;

        }

        #endregion

        #region Refresh Data

        public override void RefreshView()
        {
            // Allow refresh if in real-time mode or if in historical mode and last loaded historical snapshot is stale
            if (HistoricalSnapshotDateTime == null ||
                HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();
                //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
                if (!isUserSysAdmin)
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, false);
                else
                    ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, false);
            }
        }

        public override object DoRefreshWork(BackgroundWorker backgroundWorker)
        {
            var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
            currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
            if (HistoricalSnapshotDateTime == null)
            {
                if (!historyData.RealTimeSnapshotsPrePopulated)
                    historyData.PrePopulateRealTimeSnapshots(instanceId, this);
                else
                {
                    historyData.BackfillScheduledRefreshData(instanceId, this);

                    // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (previousVisibleLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.ForwardFillHistoricalData(instanceId, this);

                    // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                    if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                        historyData.BackFillScheduledHistoricalData(instanceId, this);
                }

                if (backgroundWorker.CancellationPending) return null;

                // this will refresh the current realtime data on entry and switching from history to realtime
                backgroundWorker.ReportProgress((int)Progress.Backfill, historyData);

                if (backgroundWorker.CancellationPending) return null;

                // go fetch the realtime snapshot
                return GetRealTimeSnapshots();
            }
            else
            {
                // If historical start/end did not change global, the snapshot is already populated.
                if (!historyData.HistoricalSnapshotsPopulated)
                    return historyData.PopulateHistoricalSnapshots(instanceId, this, backgroundWorker);
                else
                    return historyData.HistoricalSnapshots;
            }
        }
        private DataTable cpuFilterDataTable()        {            DateTime viewFilter =                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));            DataView dv = new DataView(currentDbSnapshotsDataTable);            if (historyData.HistoricalSnapshotDateTime != null) // Ishistorical
            {                dv.RowFilter = string.Format("DatabaseName = '{0}'", cpuDataBaseDropDownButton.Text);            }            else            {                dv.RowFilter = string.Format("Date > #{0}# AND DatabaseName = '{1}'", viewFilter.ToString(CultureInfo.InvariantCulture), cpuDataBaseDropDownButton.Text);            }            if (dv.Count > 0)            {                if (dv[dv.Count - 1]["DtuLimit"] != DBNull.Value)                    this.cpuUsageChart.AxisY.Title.Text = "DTU Usage %";                else                    this.cpuUsageChart.AxisY.Title.Text = "VCORE Usage %";                return dv.ToTable();            }            return null;        }

        protected override void UpdateProgress(object state, int progress)
        {
            using (Log.VerboseCall("UpdateProgress"))
            {
                switch (progress)
                {
                    case (int)Progress.Backfill:
                        UpdateChartDataFilter();
                        if (historyData.RealTimeSnapshotsDataTable.DefaultView.Count == 0) return;
                        operationalStatusPanel.Visible = false;
                        fillPanel.Visible = true;
                        if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                        {
                            ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        }
                        else
                        {
                            ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                            ConfigureMemoryChartDataSource();
                            ConfigureCpuuChartDataSource();
                        }
                        ChartsVisible(true);
                        break;
                }
            }
        }

        public override void HandleBackgroundWorkerError(Exception e)
        {
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                fillPanel.Visible = false;
            }

            base.HandleBackgroundWorkerError(e);
        }

        public ServerSummarySnapshots GetRealTimeSnapshots()
        {
            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            ServerOverviewConfiguration serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, historyData.LastServerOverviewSnapshot);
            if (serverOverviewSnapshotConfiguration.LastRefresh == null)
            {
                ServerSummarySnapshots summarySnapshots = managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
                if (summarySnapshots != null)
                {
                    serverOverviewSnapshotConfiguration = new ServerOverviewConfiguration(instanceId, summarySnapshots.ServerOverviewSnapshot);
                }
            }

            return managementService.GetServerSummary(serverOverviewSnapshotConfiguration);
        }

        public override void UpdateData(object data)
        {
            lock (updateLock)
            {
                if (historyData.HistoricalSnapshotDateTime == null)
                {
                    UpdateWithRealTimeSnapshots(data as ServerSummarySnapshots);
                }
                else
                {
                    UpdateWithHistoricalSnapshots(data as ServerSummarySnapshots);
                }
            }
            ShowVMData(historyData.IsVM);
        }

        private void UpdateWithRealTimeSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            operationalStatusPanel.Visible = false;
            fillPanel.Visible = true;

            if (summarySnapshots != null)
            {
                historyData.AddRealTimeSnapshots(summarySnapshots);

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    if (summarySnapshots.ServerOverviewSnapshot.Error == null)
                    {
                        if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                        {
                            ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                        }
                        else
                        {
                            ConfigureChartDataSource(historyData.RealTimeSnapshotsDataTable);
                            currentDbSnapshotsDataTable = historyData.CurrentDbSnapshotsDataTable;
                            ConfigureMemoryChartDataSource(summarySnapshots);
                            ConfigureCpuuChartDataSource(summarySnapshots);
                        }

                        if (summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                        {
                            memoryUsageChart.AxisY.Min = 0;
                            memoryUsageChart.AxisY.Max =
                                Convert.ToDouble(summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes);
                        }
                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig != null)
                        {
                            if (summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                            {
                                VmAvaiableByteHyperVChart.AxisY.Min = 0;
                                VmAvaiableByteHyperVChart.AxisY.Max =
                                    Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.MemUsage));
                            }

                            if (summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                            {
                                HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                                HostAvaialbleByteHyperVChart.AxisY.Max =
                                    Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.MemUsage));
                            }
                        }
                        UpdateChartDataFilter();
                        ChartsVisible(true);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                    }
                    else
                    {
                        ShowChartStatusMessage("No data available.", false);
                        ApplicationController.Default.OnRefreshActiveViewCompleted(
                            new RefreshActiveViewCompletedEventArgs(DateTime.Now, summarySnapshots.ServerOverviewSnapshot.Error));
                    }
                }
            }
        }

        private void UpdateWithHistoricalSnapshots(ServerSummarySnapshots summarySnapshots)
        {
            if (summarySnapshots != null)
            {
                fillPanel.Visible = true;

                if (summarySnapshots.ServerOverviewSnapshot != null)
                {
                    if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId != Common.Constants.MicrosoftAzureId)
                    {
                        ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable);
                    }
                    else
                    {
                        ConfigureChartDataSource(historyData.HistoricalSnapshotsDataTable);
                        currentDbSnapshotsDataTable = historyData.CurrentDbSnapshotsDataTable;
                        ConfigureMemoryChartDataSource(summarySnapshots);
                        ConfigureCpuuChartDataSource(summarySnapshots);
                    }

                    if (summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        memoryUsageChart.AxisY.Min = 0;
                        memoryUsageChart.AxisY.Max =
                            Convert.ToDouble(summarySnapshots.ServerOverviewSnapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes);
                    }

                    if (summarySnapshots.ServerOverviewSnapshot.VMConfig != null)
                    {
                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                        {
                            VmAvaiableByteHyperVChart.AxisY.Min = 0;
                            VmAvaiableByteHyperVChart.AxisY.Max =
                                Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.PerfStats.MemUsage));
                        }

                        if (summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes.HasValue)
                        {
                            HostAvaialbleByteHyperVChart.AxisY.Min = 0;
                            HostAvaialbleByteHyperVChart.AxisY.Max =
                                Convert.ToDouble((double)(summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes * 100) / (100 - summarySnapshots.ServerOverviewSnapshot.VMConfig.ESXHost.PerfStats.MemUsage));
                        }
                    }
                    ChartsVisible(true);
                    ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel,
                                          Properties.Resources.StatusWarningSmall);
                    currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
                    currentHistoricalStartDateTime = HistoricalStartDateTime;
                }
            }
            else
            {
                historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotDoesNotExistViewLabel;
                fillPanel.Visible = false;
            }

            ApplicationController.Default.OnRefreshActiveViewCompleted(
                    new RefreshActiveViewCompletedEventArgs(DateTime.Now));
        }

        private void UpdateChartDataFilter()
        {
            if (historyData.RealTimeSnapshotsDataTable != null)
            {
                DateTime viewFilter =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));

                historyData.RealTimeSnapshotsDataTable.DefaultView.RowFilter = string.Format("Date > #{0}#", viewFilter.ToString(CultureInfo.InvariantCulture));
                historyData.RealTimeSnapshotsDataTable.DefaultView.RowStateFilter = DataViewRowState.CurrentRows;
            }
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
            else if (chart.Tag is string)
                title = (string)chart.Tag;
            ExportHelper.ChartHelper.PrintChartWithTitle(this, chart, title, ultraPrintPreviewDialog);
        }

        private void SaveChartData(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            else if (chart.Tag is string)
                title = (string)chart.Tag;
            ExportHelper.ChartHelper.ExportToCsv(this, chart, ExportHelper.GetValidFileName(title, true));
        }

        private void SaveChartImage(Chart chart)
        {
            string title = string.Empty;
            if (chart.Tag is ToolStripItem)
                title = ((ToolStripItem)chart.Tag).Text;
            else if (chart.Tag is string)
                title = (string)chart.Tag;
            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {
            tableLayoutPanel.Visible = false;
            tableLayoutPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = false;
            restoreButton.Visible = true;
            chartContainerPanel.Controls.Add(chartPanel);
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton,
                                  int column, int row)
        {
            chartContainerPanel.Controls.Remove(chartPanel);
            maximizeButton.Visible = true;
            restoreButton.Visible = false;
            tableLayoutPanel.Controls.Add(chartPanel);
            tableLayoutPanel.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            tableLayoutPanel.Visible = true;

            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.AmazonRDSId)
            {
                SetAmazonRDSResourcesPanel();
            }
            if (ApplicationModel.Default.AllInstances[instanceId].CloudProviderId == Common.Constants.MicrosoftAzureId)
            {
                SetAzureResourcesPanel();
            }
        }
        public void SetAmazonRDSResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.sqlServerReadsWritesPanel, 1, 1);
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50.00F));
        }

        public void SetAzureResourcesPanel()
        {
            this.tableLayoutPanel.Controls.Clear();
            this.tableLayoutPanel.Controls.Add(this.sqlServerReadsWritesPanel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.cacheHitRatesPanel, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.memoryUsagePanel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.callRatesPanel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.cpuUsagePanel, 0, 0);
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.00F));
            this.tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 34.00F));
        }
        private void maximizeCpuUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cpuUsagePanel, maximizeCpuUsageChartButton, restoreCpuUsageChartButton);
        }

        private void restoreCpuUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cpuUsagePanel, maximizeCpuUsageChartButton, restoreCpuUsageChartButton, 0, 0);
        }

        private void maximizeCallRatesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(callRatesPanel, maximizeCallRatesChartButton, restoreCallRatesChartButton);
        }

        private void restoreCallRatesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(callRatesPanel, maximizeCallRatesChartButton, restoreCallRatesChartButton, 1, 0);
        }

        private void maximizeMemoryUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(memoryUsagePanel, maximizeMemoryUsageChartButton, restoreMemoryUsageChartButton);
        }

        private void restoreMemoryUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(memoryUsagePanel, maximizeMemoryUsageChartButton, restoreMemoryUsageChartButton, 0, 1);
        }

        private void maximizeCacheHitRatesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(cacheHitRatesPanel, maximizeCacheHitRatesChartButton, restoreCacheHitRatesChartButton);
        }

        private void restoreCacheHitRatesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(cacheHitRatesPanel, maximizeCacheHitRatesChartButton, restoreCacheHitRatesChartButton, 1, 1);
        }

        private void maximizeDiskUsageChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(diskUsagePanel, maximizeDiskUsageChartButton, restoreDiskUsageChartButton);
        }

        private void restoreDiskUsageChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(diskUsagePanel, maximizeDiskUsageChartButton, restoreDiskUsageChartButton, 0, 2);
        }

        private void maximizeSqlServerReadsWritesChartButton_Click(object sender, EventArgs e)
        {
            MaximizeChart(sqlServerReadsWritesPanel, maximizeSqlServerReadsWritesChartButton,
                          restoreSqlServerReadsWritesChartButton);
        }

        private void restoreSqlServerReadsWritesChartButton_Click(object sender, EventArgs e)
        {
            RestoreChart(sqlServerReadsWritesPanel, maximizeSqlServerReadsWritesChartButton,
                         restoreSqlServerReadsWritesChartButton, 1, 2);
        }

        #region Chart Click Events

        private void OnChartMouseClick(object sender, HitTestEventArgs e)
        {
            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other
                && !ChartHelper.IsMouseInsideChartArea(e))  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
            {
                // hit type of other means they clicked on the chart toolbar
                ServerViews targetView;
                if (sender == cpuUsageChart)
                    targetView = ServerViews.ResourcesCpu;
                else if (sender == callRatesChart)
                    targetView = ServerViews.ResourcesCpu;
                else if ((sender == memoryUsageChart) || (sender == vmMemUsageChart) || (sender == hostMemUsageChart) || (sender == VmAvaiableByteHyperVChart) || (sender == HostAvaialbleByteHyperVChart))
                    targetView = ServerViews.ResourcesMemory;
                else if (sender == cacheHitRatesChart)
                {
                    targetView = ServerViews.ResourcesMemory;
                    if (e.Series == 1)
                        targetView = ServerViews.ResourcesProcedureCache;
                }
                else if (sender == diskUsageChart)
                    targetView = ServerViews.ResourcesDisk;
                else if (sender == sqlServerReadsWritesChart)
                    targetView = ServerViews.ResourcesDisk;
                else
                    return;

                ApplicationController.Default.ShowServerView(instanceId, targetView);
            }
        }

        #endregion


        #endregion

        #region toolbar

        private void toolbarsManager_ToolClick(object sender, ToolClickEventArgs e)
        {
            if (contextMenuSelectedChart != null)
            {
                switch (e.Tool.Key)
                {
                    case "toggleChartToolbarButton":
                        ToggleChartToolbar(contextMenuSelectedChart, ((StateButtonTool)e.Tool).Checked);
                        break;
                    case "printChartButton":
                        PrintChart(contextMenuSelectedChart);
                        break;
                    case "exportChartDataButton":
                        SaveChartData(contextMenuSelectedChart);
                        break;
                    case "exportChartImageButton":
                        SaveChartImage(contextMenuSelectedChart);
                        break;
                }
            }
            contextMenuSelectedChart = null;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, BeforeToolDropdownEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "chartContextMenu":
                    Chart chart = (Chart)e.SourceControl;
                    ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
                    contextMenuSelectedChart = chart;
                    break;
            }
        }

        #endregion

        #region Operational Status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void operationalStatusLabel_MouseEnter(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void operationalStatusLabel_MouseLeave(object sender, EventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(211, 211, 211);
            operationalStatusImage.BackColor = Color.FromArgb(211, 211, 211);
        }

        private void operationalStatusLabel_MouseDown(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.White;
            operationalStatusLabel.BackColor = Color.FromArgb(251, 140, 60);
            operationalStatusImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void operationalStatusLabel_MouseUp(object sender, MouseEventArgs e)
        {
            operationalStatusLabel.ForeColor = Color.Black;
            operationalStatusLabel.BackColor = Color.FromArgb(255, 189, 105);
            operationalStatusImage.BackColor = Color.FromArgb(255, 189, 105);

            // Switching to real-time mode is the only supported opertional action at this time
            SwitchToRealTimeMode();
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            //SQLdm 10.1 (pulkit puri) -- SQLDM-25894 fix
            if (!isUserSysAdmin)
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.NO_DATA_AVAILABLE, true);
            else
                ShowChartStatusMessage(Idera.SQLdm.Common.Constants.LOADING, true);
            ApplicationController.Default.SetActiveViewToRealTimeMode();
        }

        private void ChartsVisible(bool value)
        {
            cpuUsageChart.Visible =
                callRatesChart.Visible =
                memoryUsageChart.Visible =
                cacheHitRatesChart.Visible =
                diskUsageChart.Visible =
                sqlServerReadsWritesChart.Visible = value;
        }

        private void ShowChartStatusMessage(string message, bool forceHide)
        {
            if (forceHide)
            {
                ChartsVisible(false);
            }

            cpuUsageChartStatusLabel.Text =
                callRatesChartStatusLabel.Text =
                memoryUsageChartStatusLabel.Text =
                cacheHitRatesChartStatusLabel.Text =
                diskUsageChartStatusLabel.Text =
                sqlServerReadsWritesChartStatusLabel.Text = message;
        }

        private void historicalSnapshotStatusLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (historyModeLoadError == null)
            {
                SwitchToRealTimeMode();
            }
            else
            {
                ApplicationMessageBox.ShowError(ParentForm,
                                                "An error occurred while loading a historical snapshot.",
                                                historyModeLoadError, false);
            }
        }

        private void ShowVMData(bool showVM)
        {
            if (showVM == memChartDropDownButton.Visible) return;

            memChartToolStripSeparator.Visible = memChartDropDownButton.Visible = showVM;
            foreach (SeriesAttributes series in cpuUsageChart.Series)
            {
                if ((series.Text == "VM") || (series.Text == "Host"))
                {
                    series.Visible = showVM;
                }
            }
        }

        private void setMemDropDownSelection(ToolStripMenuItem item)
        {
            memChartDropDownButton.Text = item.Text.Length > 60 ? string.Format("{0}...", item.Text.Substring(0, 60)) : item.Text;
            memChartDropDownButton.ToolTipText = item.Text;
            memChartDropDownButton.Tag = item.Text;

            //Hyper V
            MonitoredSqlServerStatus status = MonitoredSqlServerStatus.FromBackgroundRefresh(instanceId);
            string serverType = "Unknown";
            Boolean hyperVisibility = false;

            if (status != null && status.Instance.Instance.IsVirtualized)
            {
                serverType = status.Instance.Instance.VirtualizationConfiguration.VCServerType;
                hyperVisibility = serverType.Equals("HyperV") ? true : false;
            }

            if (item.Text == vmMemMenuItem.Text)
            {
                memoryUsageChart.Visible =
                    hostMemUsageChart.Visible =
                    HostAvaialbleByteHyperVChart.Visible = false;
                vmMemUsageChart.Visible = !hyperVisibility;
                VmAvaiableByteHyperVChart.Visible = hyperVisibility;
            }
            else if (item.Text == hostMemMenuItem.Text)
            {
                memoryUsageChart.Visible =
                    vmMemUsageChart.Visible =
                    VmAvaiableByteHyperVChart.Visible = false;
                hostMemUsageChart.Visible = !hyperVisibility;
                HostAvaialbleByteHyperVChart.Visible = hyperVisibility;
            }
            else
            {
                vmMemUsageChart.Visible =
                    hostMemUsageChart.Visible =
                    VmAvaiableByteHyperVChart.Visible =
                    HostAvaialbleByteHyperVChart.Visible = false;
                memoryUsageChart.Visible = true;
            }

        }
        #endregion

        private void memChartDropDownButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem is ToolStripMenuItem)
            {
                ToolStripMenuItem item = e.ClickedItem as ToolStripMenuItem;
                if (memChartDropDownButton.Tag.ToString() != item.Text)
                {
                    setMemDropDownSelection(item);
                }
            }
        }

        /// <summary>
        /// Adapts the size of the font for this control in case of OS font changes.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        private void database_clicked(object sender, EventArgs e)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Console.WriteLine(item.Text);
            dataBaseDropDownButton.Text = item.Text;
            selectedDb = item.Text;
            ConfigureMemoryChartDataSource(null);
        }

    }
}
