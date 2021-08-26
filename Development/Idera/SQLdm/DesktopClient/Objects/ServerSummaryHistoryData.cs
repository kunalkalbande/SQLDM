using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Baseline;
using Idera.SQLdm.Common.Helpers;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using System.Linq;

namespace Idera.SQLdm.DesktopClient.Objects
{

    internal sealed class ServerSummaryHistoryDataContainer
    {
        public MonitoredSqlServerStatus ServerStatus { get; set; }
        public DataTable OverviewStatistics { get; set; }
        public Pair<List<string>, DataTable> DiskDriveInfo { get; set; }
       
    }
    
    /// <summary>
    /// Stored data for multiple server views so history can be shared. Those 
    /// views include:
    /// 
    ///     - Server Summary
    ///     - Sessions Summary
    ///     - Resources Summary
    ///     - Resources CPU
    ///     - Resources Memory
    ///     - Resources Disk
    ///     - Services Summary (availability chart only)
    /// 
    /// </summary>
    public sealed class ServerSummaryHistoryData
    {
        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("ServerSummaryHistoryData");
        private Dictionary<DateTime, Dictionary<string, Dictionary<string, Object>>> AllAzureMetrics= new Dictionary<DateTime, Dictionary<string, Dictionary<string, object>>>();
        public readonly string DiskUsagePerDiskColumnPrefix = "DiskUsagePerDisk";
        public readonly string DiskQueueLengthPerDiskColumnPrefix = "DiskQueueLengthPerDisk";
        public readonly string DiskReadsPerSecPerDiskColumnPrefix = "DiskReadsPerSecPerDisk";
        public readonly string DiskTransfersPerSecPerDiskColumnPrefix = "DiskTransfersPerSecPerDisk";
        public readonly string DiskWritesPerSecPerDiskColumnPrefix = "DiskWritesPerSecPerDisk";
        public readonly string DiskTimePerReadPerDiskColumnPrefix = "DiskTimePerReadPerDisk";
        public readonly string DiskTimePerTransferPerDiskColumnPrefix = "DiskTimePerTransferPerDisk";
        public readonly string DiskTimePerWritePerDiskColumnPrefix = "DiskTimePerWritePerDisk";
        

        //6.2.4
        public readonly string DataIOUsage = "DataIOUsage";
        public readonly string LogIOUsage = "LogIOUsage";
        public readonly string DataIORate = "DataIORate";
        public readonly string LogIORate = "LogIORate";

        // SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. Baselines used by these charts are below.
        private const string DISK_BASELINE_OS_DISK_TIME = "OSDiskTimeBaseline";
        private const string DISK_BASELINE_HOST_DISK_READ = "HostDiskReadBaseline";
        private const string DISK_BASELINE_VM_DISK_READ = "VMDiskReadBaseline";
        private const string DISK_BASELINE_HOST_DISK_WRITE = "HostDiskWriteBaseline";
        private const string DISK_BASELINE_VM_DISK_WRITE = "VMDiskWriteBaseline";
        
        private const string DATE_COLUMN_NAME = "Date";
        private const string IS_BASELINE_COLUMN_NAME = "IsBaselineEntry"; 

        private const double MIN_BACKFILL_INTERVAL = 5.0;    //SqlDM 10.2 (Anshul Aggarwal) - Backfill only if missing atleast 5 minutes of data.
        private const int REFRESH_BACKFILL_MULTIPLIER = 3;

        private bool realTimeSnapshotsPrePopulated = false;
        private DataTable realTimeSnapshotsDataTable;
        private DataTable realTimeDiskDataTable;   // SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts.\
        private DataTable realTimeAzureDataTable; // 6.2.4
        private DataTable realTimeDbSnapshotsDataTable;
        private DataTable realTimeFileSnapshotsDataTable;
        private DataTable realTimeCustomCountersDataTable;
        private DataTable realTimeBaselinesDataTable;
        private DataTable historicalSnapshotsDataTable;
        private DataTable historicalDbSnapshotsDataTable;
        private DataTable historicalFileSnapshotsDataTable;
        private DataTable historicalCustomCountersDataTable;
        private DataTable historicalAzureDiskDataTable;//6.2.4
        private DataTable historicalDiskDataTable; // SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
        private ServerSummarySnapshots historicalSnapshots;
        private ServerOverview lastServerOverviewSnapshot;
        private CustomCounterCollectionSnapshot lastCustomCountersSnapshot;
        //private ServicesSnapshot lastServicesSnapshot;
        private DateTime? historicalSnapshotDateTime = null;
        private DateTime? historicalStartDateTime = null; // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
        private bool historicalSnapshotsPopulated = false;
        private MonitoredSqlServerStatus historicalSnapshotStatus = null;
        private readonly object updateLock = new object();
        private Dictionary<string, Pair<string, Type>> vmCounters = null;
        private DateTime EndTimeForBaselineStats;
        private bool isVM = false;
       
        //Const for column index for realTimeSnapshotDataTable
        private const int firstIndex = 0;
        private const int secondIndex = 1;

        internal event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public ServerSummaryHistoryData()
        {
            InitializeDataTable();
            //Settings.Default.PropertyChanged += Settings_PropertyChanged;
        }
        
        internal int InstanceId { get; set; } 

        public bool RealTimeSnapshotsPrePopulated
        {
            get
            {
                if (!realTimeSnapshotsPrePopulated)
                    return realTimeSnapshotsPrePopulated;

                int lastx = realTimeSnapshotsDataTable.Rows.Count - 1;
                if (lastx >= 0)
                {
                    DataRow last = realTimeSnapshotsDataTable.Rows[lastx];
                    if (!last.IsNull(0))
                    {
                        DateTime tod = (DateTime)last[0];
                        if (tod < DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)))
                            realTimeSnapshotsPrePopulated = false;
                    }
                }
                return realTimeSnapshotsPrePopulated;
            }
        }

        public DataTable RealTimeSnapshotsDataTable
        {
            get { return realTimeSnapshotsDataTable; }
        }

        public DataTable RealTimeDbSnapshotsDataTable
        {
            get { return realTimeDbSnapshotsDataTable; }
        }

        public DataTable RealTimeFileSnapshotsDataTable
        {
            get { return realTimeFileSnapshotsDataTable; }
        }

        public DataTable RealTimeCustomCountersDataTable
        {
            get { return realTimeCustomCountersDataTable; }
        }

        public DataTable RealTimeBaselinesDataTable
        {
            get { return realTimeBaselinesDataTable; }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
        /// </summary>
        public DataTable RealTimeDiskDataTable
        {
            get { return realTimeDiskDataTable; }
        }


        //6.2.4
        public DataTable RealTimeAzureDataTable
        {
            get { return realTimeAzureDataTable; }
        }

        public DataTable HistoricalSnapshotsDataTable
        {
            get { return historicalSnapshotsDataTable; }
        }

        public DataTable HistoricalDbSnapshotsDataTable
        {
            get { return historicalDbSnapshotsDataTable; }
        }

        public DataTable HistoricalFileSnapshotsDataTable
        {
            get { return historicalFileSnapshotsDataTable; }
        }

        public DataTable HistoricalCustomCountersDataTable
        {
            get { return historicalCustomCountersDataTable; }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
        /// </summary>
        public DataTable HistoricalDiskDataTable
        {
            get { return historicalDiskDataTable; }
        }


        //6.2.4
        public DataTable HistoricalAzureDiskDataTable
        {
            get { return historicalAzureDiskDataTable; }
        }

        public ServerSummarySnapshots HistoricalSnapshots
        {
            get { return historicalSnapshots; }
        }

        public object SyncRoot
        {
            get { return updateLock; }
        }

        public ServerOverview LastServerOverviewSnapshot
        {
            get { return lastServerOverviewSnapshot; }
            private set
            {
                lastServerOverviewSnapshot = value;
                OnPropertyChanged("LastServerOverviewSnapshot");
            }
        }

        public ServerOverview CurrentServerOverviewSnapshot
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalSnapshots.ServerOverviewSnapshot;
                    return null;
                }
                return lastServerOverviewSnapshot;
            }
        }

        public CustomCounterCollectionSnapshot LastCustomCountersSnapshot
        {
            get { return lastCustomCountersSnapshot; }
            private set
            {
                lastCustomCountersSnapshot = value;
                OnPropertyChanged("LastCustomCountersSnapshot");
            }
        }

        public CustomCounterCollectionSnapshot CurrentCustomCounterSnapshot
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalSnapshots.CustomCounterCollectionSnapshot;
                    return null;
                }
                return lastCustomCountersSnapshot;
            }
        }

        public DataTable CurrentSnapshotsDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalSnapshotsDataTable;
                    return null;
                }
                return realTimeSnapshotsDataTable;
            }
        }

        public DataTable CurrentDbSnapshotsDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalDbSnapshotsDataTable;
                    return null;
                }
                return realTimeDbSnapshotsDataTable;
            }
        }

        public DataTable CurrentFileSnapshotsDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalFileSnapshotsDataTable;
                    return null;
                }
                return realTimeFileSnapshotsDataTable;
            }
        }

        public DataTable CurrentCustomCounterSnapshotsDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalCustomCountersDataTable;
                    return null;
                }
                return realTimeCustomCountersDataTable;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
        /// </summary>
        public DataTable CurrentDiskDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalDiskDataTable;
                    return null;
                }
                return realTimeDiskDataTable;
            }
        }
        /// <summary>
        /// 6.2.4 Azure Disk Data Table
        /// </summary>
        public DataTable CurrentAzureDiskDataTable
        {
            get
            {
                if (historicalSnapshotDateTime.HasValue)
                {
                    if (historicalSnapshots != null) return historicalAzureDiskDataTable;
                    return null;
                }
                return realTimeAzureDataTable;
            }
        }

        /// <summary>
        /// Start datetime for custom range, otherwise null.
        /// </summary>
        public DateTime? HistoricalStartDateTime
        {
            get { return historicalStartDateTime; }
            set
            {
                if (historicalStartDateTime != value)
                {
                    historicalStartDateTime = value;
                    historicalSnapshotsPopulated = false;
                }
            }
        }

        public DateTime? HistoricalSnapshotDateTime
        {
            get { return historicalSnapshotDateTime; }
            set
            {
                if (historicalSnapshotDateTime != value)
                {
                    historicalSnapshotDateTime = value;
                    historicalSnapshotsPopulated = false;
                    OnPropertyChanged("HistoricalSnapshotDateTime");
                }
            }
        }

        public bool HistoricalSnapshotsPopulated
        {
            get { return historicalSnapshotsPopulated; }
        }

        public MonitoredSqlServerStatus HistoricalSnapshotStatus
        {
            get { return historicalSnapshotStatus; }
        }

        public bool IsVM
        {
            get { return isVM; }
        }

        /// <summary>
        /// Gets the max minutes for which data needs to be kept in UI.
        /// </summary>
        public static int MaximumKeepData
        {
            get
            {
                return Math.Max(Settings.Default.RealTimeChartHistoryLimitInMinutes,
                    ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
            }
        }

        #region Helper Methods

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
        /// </summary>
        public static bool isBaselinesUsedForDiskCharts(string name)
        {
            return name == DISK_BASELINE_OS_DISK_TIME || name == DISK_BASELINE_HOST_DISK_READ || name == DISK_BASELINE_VM_DISK_READ ||
                name == DISK_BASELINE_HOST_DISK_WRITE || name == DISK_BASELINE_VM_DISK_WRITE;
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Get start and end datetimes for filling increased historical data range.
        /// </summary>
        public static void GetForwardFillHistoricalRange(DataTable table, out DateTime startDateTime, out DateTime endDateTime)
        {
            using (Log.InfoCall("GetForwardFillHistoricalRange"))
            {
                endDateTime = DateTime.Now;
                startDateTime = endDateTime.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                if (table != null && table.Rows.Count > 0)
                {
                    DataRow last = table.Rows[0];
                    if (!last.IsNull(0))
                    {
                        var date = last[0] as DateTime?;
                        if (date != null)
                            endDateTime = date.Value.Subtract(TimeSpan.FromSeconds(1));
                    }
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Get start and end datetimes for backfilling historical to replace stale realtime data.
        /// </summary>
        public static bool GetBackFillHistoricalRange(DataTable table, out DateTime startDateTime, out DateTime endDateTime,
            string dateColumn = DATE_COLUMN_NAME)
        {
            using (Log.InfoCall("GetBackFillHistoricalRange"))
            {
                endDateTime = DateTime.Now.Subtract(
                        TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes))
                    .Subtract(TimeSpan.FromSeconds(1));
                int minutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes -
                              Settings.Default.RealTimeChartHistoryLimitInMinutes;
                startDateTime = endDateTime.Subtract(TimeSpan.FromMinutes(minutes));

                // Use existing data to reduce data to be fetched
                 //SQLDM - 19237 - Anshul Aggarwal - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
				string selectFilter = string.Format(dateColumn + " <= #{0}# AND IsHistorical = true", endDateTime.ToString(CultureInfo.InvariantCulture));
                if (table.Columns.Contains(IS_BASELINE_COLUMN_NAME))
                    selectFilter += " AND (" + IS_BASELINE_COLUMN_NAME + " is null)";
                var data = table.Select(selectFilter);
                if (data != null && data.Length > 0)
                {
                    DataRow last = data[data.Length - 1];
                    if (!last.IsNull(0))
                    {
                        var date = last[0] as DateTime?;
                        if (date != null)
                            startDateTime = date.Value.AddSeconds(1);
                    }
                }

                // Frequency of DB calls to replace stale real-time data is controlled by refresh-interval and min-backfill-interval.
                // If time interval of data to be replaced is either less than REFRESH_BACKFILL_MULTIPLIER * ForegroundRefreshIntervalInSeconds
                // or min-backfill-interval (currently 1 min), we skip the DB call for now.
                var timeDiff = endDateTime - startDateTime;
                var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * REFRESH_BACKFILL_MULTIPLIER);
                if (endDateTime <= startDateTime || timeDiff <= refreshInterval || timeDiff.TotalMinutes < MIN_BACKFILL_INTERVAL)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Gets filter for grooming data
        /// </summary>
        public static string GetGroomingFilter(string dateColumn)
        {
            using (Log.InfoCall("GetGroomingFilter"))
            {
                DateTime groomThreshold =
                    DateTime.Now.Subtract(TimeSpan.FromMinutes(Settings.Default.RealTimeChartHistoryLimitInMinutes));
                 //SQLDM - 19237 - Anshul Aggarwal - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
				string selectFilter = string.Format(dateColumn + " < #{0}#", groomThreshold.ToString(CultureInfo.InvariantCulture));

                // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                // If realtime  historical mix data, then need to filter out based on IsHistorical flag.
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes <
                    ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime groomThresholdHistorical =
                        DateTime.Now.Subtract(
                            TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
                    selectFilter =
                        string.Format(
                            "(" + dateColumn + " < #{0}# AND (IsHistorical is null OR IsHistorical = false)) OR (" + dateColumn +
                            " < #{1}# AND IsHistorical = true)",
                            groomThreshold.ToString(CultureInfo.InvariantCulture),
                            groomThresholdHistorical.ToString(CultureInfo.InvariantCulture));
                }

                return selectFilter;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Gets insertion index for raw data based on exisiting data
        /// </summary>
        public static int GetInsertionIndex(DataTable savedHistoryDataTable, string savedDateColumn, DataTable rawHistoryStatisticsDataTable,
            string rawDateColumn)
        {
            using (Log.InfoCall("GetInsertionIndex"))
            {
                if (savedHistoryDataTable == null || rawHistoryStatisticsDataTable == null
                    || savedHistoryDataTable.Rows.Count == 0 || rawHistoryStatisticsDataTable.Rows.Count == 0)
                    return -1;

                var time = rawHistoryStatisticsDataTable.Rows[0][rawDateColumn] as DateTime?;
                if (time != null)
                    return GetIndexOfNextRow(savedHistoryDataTable, savedDateColumn, time.Value);

                return -1;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Gets insertion index for raw data based on exisiting data
        /// </summary>
        public static int GetInsertionIndex(DataTable savedHistoryDataTable, string savedDateColumn, DateTime time)
        {
            using (Log.InfoCall("GetInsertionIndex"))
            {
                if (savedHistoryDataTable == null || savedHistoryDataTable.Rows.Count == 0)
                    return -1;
                
                return GetIndexOfNextRow(savedHistoryDataTable, savedDateColumn, time);
            }
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Gets insertion index for next time.
        /// </summary>
        private static int GetIndexOfNextRow(DataTable savedHistoryDataTable, string savedDateColumn, DateTime time)
        {
            using (Log.InfoCall("GetInsertionIndex"))
            {
				 //SQLDM - 19237 - Anshul Aggarwal - Adding # to the date value for applying the filter. Also appying the CultureInfo.InvariantCulture on view filter
                var rowFilter = savedDateColumn + " <= #" + time.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture) + "#";
                if (savedHistoryDataTable.Columns.Contains(IS_BASELINE_COLUMN_NAME))
                    rowFilter += " AND (" + IS_BASELINE_COLUMN_NAME + " is null)";
                var rows = savedHistoryDataTable.Select(rowFilter);
                if (rows != null && rows.Length > 0)
                {
                    return savedHistoryDataTable.Rows.IndexOf(rows[rows.Length - 1]) + 1;
                }
                return 0;
            }
        }

        #endregion
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartHistoryLimitInMinutes":
                    GroomRealTimeSnapshots();
                    break;
            }
        }

        private void InitializeDataTable()
        {
            realTimeSnapshotsDataTable = new DataTable();
            realTimeSnapshotsDataTable.Columns.Add("Date", typeof(DateTime));

            #region Server Overview Columns

            //
            // Sessions 
            // 
            realTimeSnapshotsDataTable.Columns.Add("ResponseTime", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("ActiveSessions", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("UserSessions", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("SystemSessions", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("IdleSessions", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("BlockedProcesses", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("LeadBlockers", typeof(int));
            RealTimeSnapshotsDataTable.Columns.Add("Total Deadlocks", typeof(long));
            //
            // Resources
            // 
            realTimeSnapshotsDataTable.Columns.Add("TotalCpuUsage", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SqlServerCpuUsage", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("PagesPerSec", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("PercentDiskTime", typeof(double));
            //start
            //6.2.4 Disk Section
            realTimeSnapshotsDataTable.Columns.Add("DataIOUsage", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("DataIORate", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("LogIOUsage", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("LogIORate", typeof(decimal));
            //end
            //
            // Locks - Requests
            //
            realTimeSnapshotsDataTable.Columns.Add("Requests - AllocUnit", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Application", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Database", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Extent", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - File", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - HoBT", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Key", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Latch", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Metadata", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Object", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Page", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - RID", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Requests - Table", typeof(long));

            //
            // Locks - Average Wait Time
            //
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - AllocUnit", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Application", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Database", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Extent", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - File", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - HoBT", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Key", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Latch", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Metadata", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Object", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Page", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - RID", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Average Wait Time - Table", typeof(double));

            //
            // Locks - Deadlocks
            //
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - AllocUnit", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Application", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Database", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Extent", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - File", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - HoBT", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Key", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Latch", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Metadata", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Object", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Page", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - RID", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Deadlocks - Table", typeof(long));

            //
            // Locks - Timeouts
            //
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - AllocUnit", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Application", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Database", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Extent", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - File", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - HoBT", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Key", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Latch", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Metadata", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Object", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Page", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - RID", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Timeouts - Table", typeof(long));

            //
            // Locks - Waits
            //
            realTimeSnapshotsDataTable.Columns.Add("Waits - AllocUnit", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Application", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Database", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Extent", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - File", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - HoBT", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Key", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Latch", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Metadata", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Object", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Page", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - RID", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("Waits - Table", typeof(long));

            //
            // Locks - Wait Time
            //
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - AllocUnit", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Application", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Database", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Extent", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - File", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - HoBT", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Key", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Latch", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Metadata", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Object", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Page", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - RID", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("Wait Time - Table", typeof(double));

            realTimeSnapshotsDataTable.Columns.Add("Wait Time - KeyRID", typeof(double));
            realTimeSnapshotsDataTable.Columns["Wait Time - KeyRID"].Expression = "[Wait Time - Key] + [Wait Time - RID]";

            #endregion

            #region Resource Columns

            //realTimeSnapshotsDataTable.Columns.Add("TotalCpuUsage", typeof(double));
            //realTimeSnapshotsDataTable.Columns.Add("SqlServerCpuUsage", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OsTotalPhysicalMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("OsMemoryUsed", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("SqlServerMemoryAllocated", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("SqlServerMemoryUsed", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("ManagedInstanceStorageLimit", typeof(decimal));
            //realTimeSnapshotsDataTable.Columns.Add("PercentDiskTime", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("CallRatesBatches", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("CallRatesTransactions", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("CallRatesCompiles", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("CallRatesReCompiles", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("BufferCacheHitRatio", typeof(float));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCacheHitRatio", typeof(float));
            realTimeSnapshotsDataTable.Columns.Add("CheckpointWrites", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("LazyWriterWrites", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("ReadAheadReads", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("SynchronousReads", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("SynchronousWrites", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("ProcessorQueueLength", typeof(ulong));
            realTimeSnapshotsDataTable.Columns.Add("PercentPrivilegedTime", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("PercentUserTime", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("DiskQueueLength", typeof(ulong));
            realTimeSnapshotsDataTable.Columns.Add("ReadWriteErrors", typeof(long));
            //realTimeSnapshotsDataTable.Columns.Add("PagesPerSec", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("BufferCachePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("BufferCacheFreePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("BufferCacheActivePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("CommittedPages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCachePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCacheFreePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCacheActivePages", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("OptimizerMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("LockMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("ConnectionMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("SortHashIndexMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("UsedDataMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("LockOptimizerConnectionSortHashIndexMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("PageLifeExpectancy", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("AzureCloudMetrics", typeof(Dictionary<string, Dictionary<string, object>>));

            // Network
            realTimeSnapshotsDataTable.Columns.Add("PacketsSent", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("PacketsReceived", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("PacketErrors", typeof(long));

            realTimeSnapshotsDataTable.Columns.Add("PacketsSentPerSecond", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("PacketsReceivedPerSecond", typeof(long));
            realTimeSnapshotsDataTable.Columns.Add("PacketErrorsPerSecond", typeof(long));
            
            //START: SQLdm 10.0 (Tarun Sapra) - Baseline columns
            //baselines
            realTimeSnapshotsDataTable.Columns.Add("OSUserTimeBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OSPrivilegeTimeBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OSProcessorQueueLengthBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OSProcessorTimeBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OSPagingBaseline", typeof(double));
           
            realTimeSnapshotsDataTable.Columns.Add("PageLifeExpectancyBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCacheHitRatioBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SQLServerMemoryUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("ProcedureCacheSizeBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SQLMemoryAllocatedBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SQLServerMemoryUsedBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("VMDiskUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("HostDiskUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("HostMemoryUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("VmMemoryUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("HostCPUUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("VmCPUUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SqlServerCPUUsageBaseline", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("OSAverageDiskQueueLengthBaseline", typeof(double));//SQLdm 10.0 (Tarun Sapra)- DE45623: 'OS Average Disk Queue Length' baseline graph value is not matching           

            realTimeSnapshotsDataTable.Columns.Add("AzureCloudAllocatedMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("AzureCloudUsedMemory", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("AzureCloudStorageLimit", typeof(decimal));
            //END: SQLdm 10.0 (Tarun Sapra) - Baseline columns
            //sqldm 11.1 5.4.1
                                            
            realTimeSnapshotsDataTable.Columns.Add("ReadThroughput", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("WriteThroughput", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("SwapUsage", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("ReadLatency", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("WriteLatency", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("CPUCreditBalance", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("CPUCreditUsage", typeof(double));
            realTimeSnapshotsDataTable.Columns.Add("DiskQueueDepth", typeof(double));

            #endregion

            #region Services Columns

            // NOTE: These columns are added dynamically the first time the snapshot is collected

            #endregion


            #region Tempdb

            realTimeSnapshotsDataTable.Columns.Add("TempdbUserObjectsMegabytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbInternalObjectsMegabytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbVersionStoreMegabytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbMixedExtentsMegabytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbUnallocatedSpaceMegabytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbGAMWaitTime", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("tempdbPFSWaitTime", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TempdbSGAMWaitTime", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("VersionStoreCleanupKilobytes", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("VersionStoreGenerationKilobytes", typeof(decimal));

            #endregion

            #region Waits

            realTimeSnapshotsDataTable.Columns.Add("IOWaits", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("LockWaits", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("MemoryWaits", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("TransactionLogWaits", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("OtherWaits", typeof(decimal));
            realTimeSnapshotsDataTable.Columns.Add("SignalWaits", typeof(decimal));

            #endregion

            #region Virtualization Columns

            LoadVmDictionary();

            foreach (string metric in vmCounters.Keys)
            {
                realTimeSnapshotsDataTable.Columns.Add(metric, vmCounters[metric].Second);
            }

            #endregion

            #region Databases

            realTimeDbSnapshotsDataTable = new DataTable();
            realTimeDbSnapshotsDataTable.Columns.Add("Date", typeof(DateTime));
           
            realTimeDbSnapshotsDataTable.Columns.Add("DatabaseName", typeof(string));
            realTimeDbSnapshotsDataTable.Columns.Add("DatabaseStatus", typeof(Common.Snapshots.DatabaseStatus));
            realTimeDbSnapshotsDataTable.Columns.Add("TransactionsPerSec", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("LogFlushWaitsPerSec", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("LogFlushesPerSec", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("LogKilobytesFlushedPerSec", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("LogCacheReadsPerSec", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("LogCacheHitRatio", typeof(long));
            realTimeDbSnapshotsDataTable.Columns.Add("NumberReadsPerSec", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("NumberWritesPerSec", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("BytesReadPerSec", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("BytesWrittenPerSec", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("IoStallMSPerSec", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("IsHistorical", typeof(bool)); // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
            realTimeDbSnapshotsDataTable.DefaultView.Sort = "Date";
            realTimeDbSnapshotsDataTable.Columns.Add("AvgCpuPercent", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("AvgDataIoPercent", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("AvgLogWritePercent", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("DtuLimit", typeof(long));
            
            // 6.2.3
            realTimeDbSnapshotsDataTable.Columns.Add("AzureCloudAllocatedMemory", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("AzureCloudUsedMemory", typeof(decimal));
            realTimeDbSnapshotsDataTable.Columns.Add("AzureCloudStorageLimit", typeof(decimal));

            //Elastic Pool Support
            realTimeDbSnapshotsDataTable.Columns.Add("ElasticPool", typeof(string));

           
            #endregion

            #region Files

            realTimeFileSnapshotsDataTable = new DataTable();
            realTimeFileSnapshotsDataTable.Columns.Add("Date", typeof(DateTime));
        
            realTimeFileSnapshotsDataTable.Columns.Add("FileName", typeof(string));
            realTimeFileSnapshotsDataTable.Columns.Add("FileType", typeof(string));
            realTimeFileSnapshotsDataTable.Columns.Add("FilePath", typeof(string));
            realTimeFileSnapshotsDataTable.Columns.Add("DriveName", typeof(string));
            realTimeFileSnapshotsDataTable.Columns.Add("DatabaseName", typeof(string));
            realTimeFileSnapshotsDataTable.Columns.Add("ReadsPerSecond", typeof(double));
            realTimeFileSnapshotsDataTable.Columns.Add("WritesPerSecond", typeof(double));
            realTimeFileSnapshotsDataTable.Columns.Add("TransfersPerSecond", typeof(double));
            realTimeFileSnapshotsDataTable.Columns.Add("IsHistorical", typeof(bool)); // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
            realTimeFileSnapshotsDataTable.DefaultView.Sort = "Date";
            
            

            #endregion

            #region Custom Counters

            realTimeCustomCountersDataTable = new DataTable();
            realTimeCustomCountersDataTable.Columns.Add("Date", typeof(DateTime));
          
            realTimeCustomCountersDataTable.Columns.Add("MetricID", typeof(int));
            realTimeCustomCountersDataTable.Columns.Add("RawValue", typeof(decimal));
            realTimeCustomCountersDataTable.Columns.Add("DeltaValue", typeof(decimal));
            realTimeCustomCountersDataTable.Columns.Add("ScaledRawValue", typeof(decimal));
            realTimeCustomCountersDataTable.Columns.Add("ScaledDeltaValue", typeof(decimal));
            realTimeCustomCountersDataTable.Columns.Add("DisplayValue", typeof(decimal));
            realTimeCustomCountersDataTable.Columns.Add("IsHistorical", typeof(bool));  // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
            #endregion

            #region Disk
            
            // SqlDM 10.2 (Anshul Aggarwal) - Added new disk datatable for disk charts. 
           
            realTimeDiskDataTable = new DataTable();
            realTimeDiskDataTable.Columns.Add("Date", typeof(DateTime));
            realTimeDiskDataTable.Columns.Add(DISK_BASELINE_OS_DISK_TIME, typeof(double));
            realTimeDiskDataTable.Columns.Add(DISK_BASELINE_HOST_DISK_READ, typeof(double));
            realTimeDiskDataTable.Columns.Add(DISK_BASELINE_VM_DISK_READ, typeof(double));
            realTimeDiskDataTable.Columns.Add(DISK_BASELINE_HOST_DISK_WRITE, typeof(double));
            realTimeDiskDataTable.Columns.Add(DISK_BASELINE_VM_DISK_WRITE, typeof(double));

            realTimeDiskDataTable.Columns.Add("Junk", typeof(int));
            realTimeDiskDataTable.Columns.Add("IsHistorical", typeof(bool));  // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
            realTimeDiskDataTable.Columns.Add(IS_BASELINE_COLUMN_NAME, typeof(bool));   // SqlDM 10.2 (Anshul Aggarwal) - New History Browser


            //6.2.4
            realTimeAzureDataTable = new DataTable();
            realTimeAzureDataTable.Columns.Add("Date", typeof(DateTime));
            realTimeAzureDataTable.Columns.Add(DataIOUsage, typeof(double));
            realTimeAzureDataTable.Columns.Add(LogIOUsage, typeof(double));
            realTimeAzureDataTable.Columns.Add(DataIORate, typeof(double));
            realTimeAzureDataTable.Columns.Add(LogIORate, typeof(double));
            realTimeAzureDataTable.Columns.Add("IsHistorical", typeof(bool));
            realTimeAzureDataTable.Columns.Add("AzureIOMetrics", typeof(List<Dictionary<String, String>>));



            #endregion



            // For binding charts that have no valid column
            realTimeSnapshotsDataTable.Columns.Add("Junk", typeof(int));
            realTimeSnapshotsDataTable.Columns.Add("IsHistorical", typeof(bool));   // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
            realTimeSnapshotsDataTable.Columns.Add(IS_BASELINE_COLUMN_NAME, typeof(bool));   // SqlDM 10.2 (Anshul Aggarwal) - New History Browser

            historicalSnapshotsDataTable = realTimeSnapshotsDataTable.Clone();
            historicalAzureDiskDataTable = realTimeAzureDataTable.Clone();//6.2.4
            historicalDbSnapshotsDataTable = realTimeDbSnapshotsDataTable.Clone();
            historicalFileSnapshotsDataTable = realTimeFileSnapshotsDataTable.Clone();
            historicalCustomCountersDataTable = realTimeCustomCountersDataTable.Clone();
            historicalDiskDataTable = realTimeDiskDataTable.Clone();
        }

        /// <summary>
        /// Loads the vmCounters dictionary
        /// </summary>
        private void LoadVmDictionary()
        {
            vmCounters = new Dictionary<string, Pair<string, Type>>();
            vmCounters.Add("vmCPULimit", new Pair<string, Type>("vCPU Speed Limit", typeof(long)));
            vmCounters.Add("vmCPUReserve", new Pair<string, Type>("vCPU Reserved Speed", typeof(long)));
            vmCounters.Add("vmMemSize", new Pair<string, Type>("VM Memory Size (MB)", typeof(long)));
            vmCounters.Add("vmMemLimit", new Pair<string, Type>("VM Memory Limit (MB)", typeof(long)));
            vmCounters.Add("vmMemReserve", new Pair<string, Type>("VM Memory Reserved (MB)", typeof(long)));
            vmCounters.Add("esxCPUMHz", new Pair<string, Type>("Host Processor Speed", typeof(int)));
            vmCounters.Add("esxNumCPUCores", new Pair<string, Type>("Host Physical Processors", typeof(short)));
            vmCounters.Add("esxNumCPUThreads", new Pair<string, Type>("Host Logical Processors", typeof(short)));
            vmCounters.Add("esxMemSize", new Pair<string, Type>("Host Physical Memory (MB)", typeof(long)));
            vmCounters.Add("esxNumNICs", new Pair<string, Type>("Host NICs", typeof(int)));
            vmCounters.Add("esxBootTime", new Pair<string, Type>("Host Host Boot Time", typeof(DateTime)));
            vmCounters.Add("vmBootTime", new Pair<string, Type>("VM Boot Time", typeof(DateTime)));
            vmCounters.Add("vmCPUUsage", new Pair<string, Type>("vCPU Usage (Percent)", typeof(decimal)));
            vmCounters.Add("vmCPUUsageMHz", new Pair<string, Type>("vCPU Usage (MHz)", typeof(int)));
            vmCounters.Add("vmCPUReady", new Pair<string, Type>("vCPU Ready Time (ms)", typeof(long)));
            vmCounters.Add("vmCPUSwapWait", new Pair<string, Type>("vCPU Swap Wait Time (ms)", typeof(long)));
            vmCounters.Add("vmMemSwapInRate", new Pair<string, Type>("VM Memory Swap In Rate (KB/s)", typeof(long)));
            vmCounters.Add("vmMemSwapOutRate", new Pair<string, Type>("VM Memory Swap Out Rate (KB/s)", typeof(long)));
            vmCounters.Add("vmMemSwapped", new Pair<string, Type>("VM Memory Swapped (MB)", typeof(long)));
            vmCounters.Add("vmMemActive", new Pair<string, Type>("VM Active Memory (MB)", typeof(long)));
            vmCounters.Add("vmMemConsumed", new Pair<string, Type>("VM Consumed Memory (MB)", typeof(long)));
            vmCounters.Add("vmMemGranted", new Pair<string, Type>("VM Memory Granted (MB)", typeof(long)));
            vmCounters.Add("vmMemBallooned", new Pair<string, Type>("VM Ballooned Memory (MB)", typeof(long)));
            vmCounters.Add("vmMemUsage", new Pair<string, Type>("VM Memory Usage (Percent)", typeof(decimal)));
            vmCounters.Add("vmDiskRead", new Pair<string, Type>("VM Disk Reads (KB/s)", typeof(long)));
            vmCounters.Add("vmDiskWrite", new Pair<string, Type>("VM Disk Writes (KB/s)", typeof(long)));
            vmCounters.Add("vmDiskUsage", new Pair<string, Type>("VM Disk Usage (KB/s)", typeof(long)));
            vmCounters.Add("vmNetUsage", new Pair<string, Type>("VM Network Usage (KB/s)", typeof(long)));
            vmCounters.Add("vmNetTransmitted", new Pair<string, Type>("VM Network Transmitted (KB/s)", typeof(long)));
            vmCounters.Add("vmNetReceived", new Pair<string, Type>("VM Network Received (KB/s)", typeof(long)));
            vmCounters.Add("esxCPUUsage", new Pair<string, Type>("Host CPU Usage (Percent)", typeof(decimal)));
            vmCounters.Add("esxCPUUsageMHz", new Pair<string, Type>("Host CPU Usage (MHz)", typeof(int)));
            vmCounters.Add("esxMemSwapInRate", new Pair<string, Type>("Host Memory Swap In Rate (KB/s)", typeof(long)));
            vmCounters.Add("esxMemSwapOutRate", new Pair<string, Type>("Host Memory Swap Out Rate (KB/s)", typeof(long)));
            vmCounters.Add("esxMemActive", new Pair<string, Type>("Host Active Memory (MB)", typeof(long)));
            vmCounters.Add("esxMemConsumed", new Pair<string, Type>("Host Consumed Memory (MB)", typeof(long)));
            vmCounters.Add("esxMemGranted", new Pair<string, Type>("Host Memory Granted (MB)", typeof(long)));
            vmCounters.Add("esxMemBallooned", new Pair<string, Type>("Host Ballooned Memory (MB)", typeof(long)));
            vmCounters.Add("esxMemUsage", new Pair<string, Type>("Host Memory Usage (Percent)", typeof(decimal)));
            vmCounters.Add("esxDiskRead", new Pair<string, Type>("Host Disk Reads (KB/s)", typeof(long)));
            vmCounters.Add("esxDiskWrite", new Pair<string, Type>("Host Disk Writes (KB/s)", typeof(long)));
            vmCounters.Add("esxDiskUsage", new Pair<string, Type>("Host Disk Usage (KB/s)", typeof(long)));
            vmCounters.Add("esxDeviceLatency", new Pair<string, Type>("Host Disk Device Latency (ms)", typeof(long)));
            vmCounters.Add("esxKernelLatency", new Pair<string, Type>("Host Disk Kernel Latency (ms)", typeof(long)));
            vmCounters.Add("esxQueueLatency", new Pair<string, Type>("Host Disk Queue Latency (ms)", typeof(long)));
            vmCounters.Add("esxTotalLatency", new Pair<string, Type>("Host Disk Total Latency (ms)", typeof(long)));
            vmCounters.Add("esxNetUsage", new Pair<string, Type>("Host Network Usage (KB/s)", typeof(long)));
            vmCounters.Add("esxNetTransmitted", new Pair<string, Type>("Host Network Transmitted (KB/s)", typeof(long)));
            vmCounters.Add("esxNetReceived", new Pair<string, Type>("Host Network Received (KB/s)", typeof(long)));
            vmCounters.Add("PagePerSecVM", new Pair<string, Type>("Page Per Sec VM", typeof(long)));
            vmCounters.Add("PagePerSecHost", new Pair<string, Type>("Page Per Sec Host", typeof(long)));
            vmCounters.Add("AvailableByteVm", new Pair<string, Type>("Avaiaable Byte VM", typeof(long)));
            vmCounters.Add("AvailableByteHost", new Pair<string, Type>("Avaiaable Byte Host", typeof(long)));
        }

        /// <summary>
        /// Adds a column for each service collected in the Services snapshot.
        /// </summary>
        /// <param name="snapshot"></param>
        private void InitializeServicesColumns(ServicesSnapshot snapshot)
        {
            if (realTimeSnapshotsDataTable != null && snapshot != null && snapshot.Error == null)
            {
                foreach (ServiceName service in snapshot.ServiceDetails.Keys)
                {
                    if (!realTimeSnapshotsDataTable.Columns.Contains(ApplicationHelper.GetEnumDescription(service)))
                    {
                        realTimeSnapshotsDataTable.Columns.Add(ApplicationHelper.GetEnumDescription(service), typeof(int));
                    }
                }
            }
        }

        //START: SQLdm 10.0 (Tarun Sapra) - This function deletes all the data related to the baselines
        private void DeleteBaselinesData(DataTable snapshotDataTable, DataTable diskDataTable)
        {
            foreach (string baselineName in Enum.GetNames(typeof(BaselineNameAsPerMetricID)))
            {
                // SqlDM 10.2 (Anshul Aggarwal) - Baselines are now present in either snapshot or disk datatables.
                var table = isBaselinesUsedForDiskCharts(baselineName) ? diskDataTable : snapshotDataTable;
                if (table != null)
                {
                    string selectFilter = string.Format("{0} >= 0", baselineName);
                    DataRow[] baselineRows = table.Select(selectFilter);
                    foreach (DataRow row in baselineRows)
                    {
                        row.Delete();
                    }
                    table.AcceptChanges();
                }
            }
        }
        //END: SQLdm 10.0 (Tarun Sapra) - This function deletes all the data related to the baselines

        private void GroomRealTimeSnapshots()
        {
            string selectFilter = GetGroomingFilter("Date");
            if (realTimeSnapshotsDataTable != null)
            {
                DataRow[] groomedRows = realTimeSnapshotsDataTable.Select(selectFilter);

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeSnapshotsDataTable.AcceptChanges();
            }

            if (realTimeDbSnapshotsDataTable != null)
            {
                DataRow[] groomedRows = realTimeDbSnapshotsDataTable.Select(selectFilter);

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeDbSnapshotsDataTable.AcceptChanges();

            }

            if (realTimeFileSnapshotsDataTable != null)
            {
                DataRow[] groomedRows = realTimeFileSnapshotsDataTable.Select(selectFilter);

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeFileSnapshotsDataTable.AcceptChanges();

            }

            if (realTimeCustomCountersDataTable != null)
            {
                DataRow[] groomedRows = realTimeCustomCountersDataTable.Select(selectFilter);

                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeCustomCountersDataTable.AcceptChanges();

            }

            if (realTimeDiskDataTable != null)
            {
                DataRow[] groomedRows = realTimeDiskDataTable.Select(selectFilter);
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeDiskDataTable.AcceptChanges();
            }

            //6.2.4
            if (realTimeAzureDataTable != null)
            {
                DataRow[] groomedRows = realTimeAzureDataTable.Select(selectFilter);
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }
                realTimeAzureDataTable.AcceptChanges();
            }
        }

        public void AddRealTimeSnapshots(ServerSummarySnapshots serverSummarySnapshots)
        {
            if (serverSummarySnapshots != null)
            {
                lock (updateLock)
                {
                    DataRow newRow = realTimeSnapshotsDataTable.NewRow();
                    newRow["Date"] = DateTime.Now;
                    newRow["IsHistorical"] = false; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                    AddServerOverviewSnapshot(serverSummarySnapshots.ServerOverviewSnapshot, newRow);
                    AddResourcesSnapshot(serverSummarySnapshots.ServerOverviewSnapshot, newRow);
                    AddDatabaseStatisticsSnapshots(serverSummarySnapshots.ServerOverviewSnapshot.DbStatistics, realTimeDbSnapshotsDataTable, (DateTime)newRow["Date"], serverSummarySnapshots.ServerOverviewSnapshot.TimeDelta);
                    AddFileActivitySnapshots(serverSummarySnapshots.ServerOverviewSnapshot.FileActivity, realTimeFileSnapshotsDataTable, (DateTime) newRow["Date"]);
                    AddCustomCountersSnapshots(serverSummarySnapshots.CustomCounterCollectionSnapshot, realTimeCustomCountersDataTable, (DateTime)newRow["Date"]);
                    AddDiskSnapshot(serverSummarySnapshots.ServerOverviewSnapshot, realTimeDiskDataTable, (DateTime)newRow["Date"]);   // SqlDM 10.2 (Anshul Aggarwal) - New disk table for disk charts.
                    //6.2.4
                    AddAzureDiskSanpshot(serverSummarySnapshots.ServerOverviewSnapshot.AzureIOMetrics, realTimeAzureDataTable, (DateTime)newRow["Date"]);
                    realTimeSnapshotsDataTable.Rows.Add(newRow);
                    
                    GroomRealTimeSnapshots();

                    // SQLDM-27575: adding the old changes to fix the odd behavior related left gaps
                    DataTable baselinesDataTable = new DataTable();
                    if (realTimeSnapshotsDataTable.Rows.Count > 1)
                    {
                        var startDate = Convert.ToDateTime(realTimeSnapshotsDataTable.Rows[0]["Date"], CultureInfo.InvariantCulture);
                        var endDate = Convert.ToDateTime(realTimeSnapshotsDataTable.Rows[realTimeSnapshotsDataTable.Rows.Count - 1]["Date"], CultureInfo.InvariantCulture);
                        Log.Info("Fetching baseline details between : {0} and {1}.", startDate, endDate);

                        baselinesDataTable = RepositoryHelper.FillBaselineData(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        InstanceId,
                        endDate.ToUniversalTime(),
                        (int)(endDate.ToUniversalTime() - startDate.ToUniversalTime()).TotalSeconds,
                        (DateTime.Now.Date - DateTime.Now.ToUniversalTime().Date).TotalDays);
                    }
                    else
                    {
                        //START: SQLdm 10.0 (Tarun Sapra) - Gets all the baselines data and insert that into the data table as per the metricId in data table
                        baselinesDataTable = RepositoryHelper.FillBaselineData(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        InstanceId,
                        DateTime.Now.ToUniversalTime(),
                        ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes * 60 - 10,
                        (DateTime.Now.Date - DateTime.Now.ToUniversalTime().Date).TotalDays);
                    }
                    
                    //SQLdm 10.1 (srishti purohit) -- Moving below delete function to reduce the gap between data removed and data reloaded
                    DeleteBaselinesData(realTimeSnapshotsDataTable, realTimeDiskDataTable);//SQLdm 10.0 (Tarun Sapra) - Delete baselines data in the current snapshot
                    PopulateBaselinesDataTableWithHistory(realTimeSnapshotsDataTable, realTimeDiskDataTable, baselinesDataTable);
                    //END: SQLdm 10.0 (Tarun Sapra) - Gets all the baselines data and insert that into the data table as per the metricId in data table
                }
            }
        }

        private void AddServerOverviewSnapshot(ServerOverview snapshot, DataRow dataRow)
        {
            try
            {
                if (snapshot != null && snapshot.Error == null && dataRow != null)
                {
                    dataRow["ResponseTime"] = snapshot.ResponseTime;
                    dataRow["AzureCloudMetrics"] = snapshot.AzureCloudMetrics;

                    if (snapshot.SystemProcesses.ActiveProcesses.HasValue)
                    {
                        dataRow["ActiveSessions"] = snapshot.SystemProcesses.ActiveProcesses.Value;

                        if (snapshot.SystemProcesses.CurrentUserProcesses.HasValue)
                        {
                            dataRow["IdleSessions"] = snapshot.SystemProcesses.CurrentUserProcesses -
                                                      snapshot.SystemProcesses.ActiveProcesses.Value;
                        }
                        else
                        {
                            dataRow["IdleSessions"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dataRow["ActiveSessions"] = DBNull.Value;
                        dataRow["IdleSessions"] = DBNull.Value;
                    }

                    if (snapshot.SystemProcesses.CurrentSystemProcesses.HasValue)
                    {
                        dataRow["SystemSessions"] = snapshot.SystemProcesses.CurrentSystemProcesses.Value;
                    }
                    else
                    {
                        dataRow["SystemSessions"] = DBNull.Value;
                    }

                    if (snapshot.SystemProcesses.LeadBlockers.HasValue)
                    {
                        dataRow["LeadBlockers"] = snapshot.SystemProcesses.LeadBlockers.Value;
                    }
                    else
                    {
                        dataRow["LeadBlockers"] = DBNull.Value;
                    }

                    if (snapshot.SystemProcesses.BlockedProcesses.HasValue)
                    {
                        dataRow["BlockedProcesses"] = snapshot.SystemProcesses.BlockedProcesses.Value;
                    }
                    else
                    {
                        dataRow["BlockedProcesses"] = DBNull.Value;
                    }

                    if (snapshot.LockCounters.TotalCounters.Deadlocks.HasValue)
                    {
                        dataRow["Total Deadlocks"] = snapshot.LockCounters.TotalCounters.Deadlocks;
                    }
                    else
                    {
                        dataRow["Total Deadlocks"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.PercentProcessorTime.HasValue)
                    {
                        dataRow["TotalCpuUsage"] = snapshot.OSMetricsStatistics.PercentProcessorTime;
                    }
                    else
                    {
                        dataRow["TotalCpuUsage"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.CpuPercentage.HasValue)
                    {
                        dataRow["SqlServerCpuUsage"] = snapshot.Statistics.CpuPercentage;
                    }
                    else
                    {
                        dataRow["SqlServerCpuUsage"] = DBNull.Value;
                    }

                     if (snapshot.OSMetricsStatistics.PagesPersec.HasValue || snapshot.PagesPersec.HasValue)
                    {
                        dataRow["PagesPerSec"] = snapshot.OSMetricsStatistics.PagesPersec ?? snapshot.PagesPersec;
                    }
                    else
                    {
                        dataRow["PagesPerSec"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.PercentDiskTime.HasValue)
                    {
                        dataRow["PercentDiskTime"] = snapshot.OSMetricsStatistics.PercentDiskTime.Value;
                    }
                    else
                    {
                        dataRow["PercentDiskTime"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketsSent.HasValue)
                    {
                        dataRow["PacketsSent"] = snapshot.Statistics.PacketsSent.Value;
                    }
                    else
                    {
                        dataRow["PacketsSent"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketsReceived.HasValue)
                    {
                        dataRow["PacketsReceived"] = snapshot.Statistics.PacketsReceived.Value;
                    }
                    else
                    {
                        dataRow["PacketsReceived"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketErrors.HasValue)
                    {
                        dataRow["PacketErrors"] = snapshot.Statistics.PacketErrors.Value;
                    }
                    else
                    {
                        dataRow["PacketErrors"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketsSent.HasValue && snapshot.TimeDelta.HasValue)
                    {
                        dataRow["PacketsSentPerSecond"] = snapshot.Statistics.PacketsSent.Value / snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["PacketsSentPerSecond"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketsReceived.HasValue && snapshot.TimeDelta.HasValue)
                    {
                        dataRow["PacketsReceivedPerSecond"] = snapshot.Statistics.PacketsReceived.Value / snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["PacketsReceivedPerSecond"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PacketErrors.HasValue && snapshot.TimeDelta.HasValue)
                    {
                        dataRow["PacketErrorsPerSecond"] = snapshot.Statistics.PacketErrors.Value / snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["PacketErrorsPerSecond"] = DBNull.Value;
                    }

                    if (snapshot.ManagedInstanceStorageLimit > 0)
                    {
                        dataRow["ManagedInstanceStorageLimit"] = snapshot.ManagedInstanceStorageLimit;
                    }
                    else
                    {
                        dataRow["ManagedInstanceStorageLimit"] = DBNull.Value;
                    }

                    if (snapshot.VMConfig != null)
                    {
                        isVM = true;

                        ////VM Num CPUs
                        //string metric = "vmNumCPUs";
                        //if (snapshot.VMConfig.NumCPUs != null)
                        //    dataRow[metric] = snapshot.VMConfig.NumCPUs;
                        //else
                        //    dataRow[metric] = DBNull.Value;

                        // VM CPU Limit
                        string metric = "vmCPULimit";
                        if (snapshot.VMConfig.CPULimit != null)
                            dataRow[metric] = snapshot.VMConfig.CPULimit;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM CPU Reserve
                        metric = "vmCPUReserve";
                        if (snapshot.VMConfig.CPUReserve != null)
                            dataRow[metric] = snapshot.VMConfig.CPUReserve;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Memory Size
                        metric = "vmMemSize";
                        if (snapshot.VMConfig.MemSize != null && snapshot.VMConfig.MemSize.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.MemSize.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Memory limit
                        metric = "vmMemLimit";
                        if (snapshot.VMConfig.MemLimit != null && snapshot.VMConfig.MemLimit.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.MemLimit.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Memory Reserve
                        metric = "vmMemReserve";
                        if (snapshot.VMConfig.MemReserve != null && snapshot.VMConfig.MemReserve.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.MemReserve.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX CPU MHz
                        metric = "esxCPUMHz";
                        if (snapshot.VMConfig.ESXHost.CPUMHz != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.CPUMHz;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Num Physical CPUs
                        metric = "esxNumCPUCores";
                        if (snapshot.VMConfig.ESXHost.NumCPUCores != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.NumCPUCores;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Num Logical CPUs
                        metric = "esxNumCPUThreads";
                        if (snapshot.VMConfig.ESXHost.NumCPUThreads != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.NumCPUThreads;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Memory Size
                        metric = "esxMemSize";
                        if (snapshot.VMConfig.ESXHost.MemSize != null && snapshot.VMConfig.ESXHost.MemSize.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.MemSize.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Number of NICs
                        metric = "esxNumNICs";
                        if (snapshot.VMConfig.ESXHost.NumNICs != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.NumNICs;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Boot Time
                        metric = "esxBootTime";
                        if (snapshot.VMConfig.ESXHost.BootTime != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.BootTime;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Boot Time
                        metric = "vmBootTime";
                        if (snapshot.VMConfig.BootTime != null)
                            dataRow[metric] = snapshot.VMConfig.BootTime;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM CPU Usage
                        metric = "vmCPUUsage";
                        if (snapshot.VMConfig.PerfStats.CpuUsage != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.CpuUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM CPU Usage MHz
                        metric = "vmCPUUsageMHz";
                        if (snapshot.VMConfig.PerfStats.CpuUsageMHz != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.CpuUsageMHz;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Ready Time
                        metric = "vmCPUReady";
                        if (snapshot.VMConfig.PerfStats.CpuReady != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.CpuReady;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM CPU Swap Wait
                        metric = "vmCPUSwapWait";
                        if (snapshot.VMConfig.PerfStats.CpuSwapWait != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.CpuSwapWait;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Swap In Rate
                        metric = "vmMemSwapInRate";
                        if (snapshot.VMConfig.PerfStats.MemSwapInRate != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemSwapInRate;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Swap Out Rate
                        metric = "vmMemSwapOutRate";
                        if (snapshot.VMConfig.PerfStats.MemSwapOutRate != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemSwapOutRate;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Swapped
                        metric = "vmMemSwapped";
                        if (snapshot.VMConfig.PerfStats.MemSwapped != null && snapshot.VMConfig.PerfStats.MemSwapped.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemSwapped.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Active
                        metric = "vmMemActive";
                        if (snapshot.VMConfig.PerfStats.MemActive != null && snapshot.VMConfig.PerfStats.MemActive.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemActive.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Consumed
                        metric = "vmMemConsumed";
                        if (snapshot.VMConfig.PerfStats.MemConsumed != null && snapshot.VMConfig.PerfStats.MemConsumed.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemConsumed.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Granted
                        metric = "vmMemGranted";
                        if (snapshot.VMConfig.PerfStats.MemGranted != null && snapshot.VMConfig.PerfStats.MemGranted.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemGranted.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Ballooned
                        metric = "vmMemBallooned";
                        if (snapshot.VMConfig.PerfStats.MemBallooned != null && snapshot.VMConfig.PerfStats.MemBallooned.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemBallooned.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Mem Usage
                        metric = "vmMemUsage";
                        if (snapshot.VMConfig.PerfStats.MemUsage != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.MemUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Disk Read
                        metric = "vmDiskRead";
                        if (snapshot.VMConfig.PerfStats.DiskRead != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.DiskRead;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Disk Write
                        metric = "vmDiskWrite";
                        if (snapshot.VMConfig.PerfStats.DiskWrite != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.DiskWrite;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Disk Usage
                        metric = "vmDiskUsage";
                        if (snapshot.VMConfig.PerfStats.DiskUsage != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.DiskUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Net Usage
                        metric = "vmNetUsage";
                        if (snapshot.VMConfig.PerfStats.NetUsage != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.NetUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Net Transmitted
                        metric = "vmNetTransmitted";
                        if (snapshot.VMConfig.PerfStats.NetTransmitted != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.NetTransmitted;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM Net Received
                        metric = "vmNetReceived";
                        if (snapshot.VMConfig.PerfStats.NetReceived != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.NetReceived;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX CPu Usage
                        metric = "esxCPUUsage";
                        if (snapshot.VMConfig.ESXHost.PerfStats.CpuUsage != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.CpuUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX CPu Usage MHz
                        metric = "esxCPUUsageMHz";
                        if (snapshot.VMConfig.ESXHost.PerfStats.CpuUsageMHz != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.CpuUsageMHz;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Swap In Rate
                        metric = "esxMemSwapInRate";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemSwapInRate != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemSwapInRate;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Swap Out Rate
                        metric = "esxMemSwapOutRate";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemSwapOutRate != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemSwapOutRate;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Active
                        metric = "esxMemActive";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemActive != null && snapshot.VMConfig.ESXHost.PerfStats.MemActive.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemActive.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Consumed
                        metric = "esxMemConsumed";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemConsumed != null && snapshot.VMConfig.ESXHost.PerfStats.MemConsumed.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemConsumed.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Granted
                        metric = "esxMemGranted";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemGranted != null && snapshot.VMConfig.ESXHost.PerfStats.MemGranted.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemGranted.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Ballooned
                        metric = "esxMemBallooned";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemBallooned != null && snapshot.VMConfig.ESXHost.PerfStats.MemBallooned.Megabytes.HasValue)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemBallooned.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Mem Usage
                        metric = "esxMemUsage";
                        if (snapshot.VMConfig.ESXHost.PerfStats.MemUsage != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.MemUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Read
                        metric = "esxDiskRead";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskRead != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskRead;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Write
                        metric = "esxDiskWrite";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskWrite != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskWrite;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Usage
                        metric = "esxDiskUsage";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskUsage != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Device Latency
                        metric = "esxDeviceLatency";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskDeviceLatency != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskDeviceLatency;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Kernel Latency
                        metric = "esxKernelLatency";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskKernelLatency != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskKernelLatency;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Queue Latency
                        metric = "esxQueueLatency";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskQueueLatency != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskQueueLatency;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Disk Total Latency
                        metric = "esxTotalLatency";
                        if (snapshot.VMConfig.ESXHost.PerfStats.DiskTotalLatency != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.DiskTotalLatency;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Net Usage
                        metric = "esxNetUsage";
                        if (snapshot.VMConfig.ESXHost.PerfStats.NetUsage != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.NetUsage;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Net Transmitted
                        metric = "esxNetTransmitted";
                        if (snapshot.VMConfig.ESXHost.PerfStats.NetTransmitted != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.NetTransmitted;
                        else
                            dataRow[metric] = DBNull.Value;

                        // ESX Net Received
                        metric = "esxNetReceived";
                        if (snapshot.VMConfig.ESXHost.PerfStats.NetReceived != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.NetReceived;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM page per sec
                        metric = "PagePerSecVM";
                        if (snapshot.VMConfig.PerfStats.PagePerSec != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.PagePerSec;
                        else
                            dataRow[metric] = DBNull.Value;

                        // Host Page Per se
                        metric = "PagePerSecHost";
                        if (snapshot.VMConfig.ESXHost.PerfStats.PagePerSec != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.PagePerSec;
                        else
                            dataRow[metric] = DBNull.Value;

                        // VM available Byte HyperV
                        metric = "AvailableByteVm";
                        if (snapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes != null)
                            dataRow[metric] = snapshot.VMConfig.PerfStats.AvaialableByteHyperV.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;

                        // Hostavailable Byte HyperV
                        metric = "AvailableByteHost";
                        if (snapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes != null)
                            dataRow[metric] = snapshot.VMConfig.ESXHost.PerfStats.AvaialableByteHyperV.Megabytes;
                        else
                            dataRow[metric] = DBNull.Value;
                    }
                    else
                    {
                        //anil data is not collecting but still machine in VM
                       // isVM = false;
                        Log.DebugFormat(">>>>> {0} is not virtualized <<<<<", snapshot.ServerName);
                    }


                    AddLockStatisticsSnapshot(snapshot.LockCounters, dataRow);

                    AddTempdbStatisticsSnapshot(snapshot.TempdbStatistics, dataRow);

                    LastServerOverviewSnapshot = snapshot;
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the server overview snapshot.", e);
            }
        }

        private void AddDatabaseStatisticsSnapshots(Dictionary<String, DatabaseStatistics> dbStatistics, DataTable dt, DateTime date, TimeSpan? timeDelta)
        {
            if (dbStatistics != null && dt != null)
            {
                try
                {
                    dt.BeginLoadData();
                    foreach (DatabaseStatistics dbstats in dbStatistics.Values)
                    {
                        DataRow newRow = dt.NewRow();
                        bool validTimeDelta = timeDelta != null && timeDelta.Value.TotalSeconds > 0;
                        newRow["DatabaseName"] = dbstats.Name;
                        newRow["Date"] = date;
                        newRow["IsHistorical"] = false; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                        newRow["DatabaseStatus"] = dbstats.Status;
                        if (dbstats.Transactions.HasValue && validTimeDelta)
                        {
                            newRow["TransactionsPerSec"] = dbstats.Transactions / timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.LogFlushWaits.HasValue && validTimeDelta)
                        {
                            newRow["LogFlushWaitsPerSec"] = dbstats.LogFlushWaits / timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.LogFlushes.HasValue && validTimeDelta)
                        {
                            newRow["LogFlushesPerSec"] = dbstats.LogFlushes / timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.LogSizeFlushed != null && dbstats.LogSizeFlushed.Kilobytes.HasValue && validTimeDelta)
                        {
                            newRow["LogKilobytesFlushedPerSec"] = dbstats.LogSizeFlushed.Kilobytes / (decimal)timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.LogCacheReads.HasValue && validTimeDelta)
                        {
                            newRow["LogCacheReadsPerSec"] = dbstats.LogCacheReads / timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.LogCacheHitRatio.HasValue)
                        {
                            newRow["LogCacheHitRatio"] = dbstats.LogCacheHitRatio;
                        }

                        if (dbstats.Reads.HasValue && validTimeDelta)
                        {
                            newRow["NumberReadsPerSec"] = dbstats.Reads / (decimal)timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.Writes.HasValue && validTimeDelta)
                        {
                            newRow["NumberWritesPerSec"] = dbstats.Writes / (decimal)timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.BytesRead.HasValue && validTimeDelta)
                        {
                            newRow["BytesReadPerSec"] = dbstats.BytesRead / (decimal)timeDelta.Value.TotalSeconds;
                        }

                        if (dbstats.BytesWritten.HasValue && validTimeDelta)
                        {
                            newRow["BytesWrittenPerSec"] = dbstats.BytesWritten / (decimal)timeDelta.Value.TotalSeconds;
                        }
                        if (dbstats.IoStallMs.HasValue && validTimeDelta)
                        {
                            newRow["IoStallMSPerSec"] = dbstats.IoStallMs / (decimal)timeDelta.Value.TotalSeconds;
                        }
                        if (dbstats.AzureDbDetail != null)
                        {

                            if (dbstats.AzureDbDetail.AvgDataIoPercent.HasValue && validTimeDelta)
                            {
                                newRow["AvgDataIoPercent"] = dbstats.AzureDbDetail.AvgDataIoPercent;
                            }
                            if (dbstats.AzureDbDetail.AvgLogWritePercent.HasValue && validTimeDelta)
                            {
                                newRow["AvgLogWritePercent"] = dbstats.AzureDbDetail.AvgLogWritePercent;
                            }
                            if (dbstats.AzureDbDetail.DtuLimit.HasValue && validTimeDelta)
                            {
                                newRow["DtuLimit"] = dbstats.AzureDbDetail.DtuLimit;
                            }
                            if (dbstats.AzureDbDetail.AvgCpuPercent.HasValue && validTimeDelta)
                            {
                                if (dbstats.AzureDbDetail.DtuLimit.HasValue)
                                {
                                    decimal? max = dbstats.AzureDbDetail.AvgCpuPercent > dbstats.AzureDbDetail.AvgDataIoPercent ?
                                         dbstats.AzureDbDetail.AvgCpuPercent : dbstats.AzureDbDetail.AvgDataIoPercent;
                                    max = max > dbstats.AzureDbDetail.AvgLogWritePercent ? max : dbstats.AzureDbDetail.AvgLogWritePercent;
                                    newRow["AvgCpuPercent"] = max;
                                }
                                else
                                    newRow["AvgCpuPercent"] = dbstats.AzureDbDetail.AvgCpuPercent;
                            }
                        }


                        // Start - 6.2.3
                        if (dbstats.AzureCloudAllocatedMemory.HasValue)
                        {
                            newRow["AzureCloudAllocatedMemory"] = dbstats.AzureCloudAllocatedMemory;
                        }

                        if (dbstats.AzureCloudUsedMemory.HasValue)
                        {
                            newRow["AzureCloudUsedMemory"] = dbstats.AzureCloudUsedMemory;
                        }

                        if (dbstats.AzureCloudStorageLimit.HasValue)
                        {
                            newRow["AzureCloudStorageLimit"] = dbstats.AzureCloudStorageLimit;
                        }
                        // End - 6.2.3

                        //Elastic pool Support
                        if (dbstats.ElasticPool!=null)
                        {
                            newRow["ElasticPool"] = dbstats.ElasticPool;
                        }

                        dt.Rows.Add(newRow);
                    }
                }
                finally
                {
                    dt.EndLoadData();
                }
            }
        }

        private void AddFileActivitySnapshots(Dictionary<String, FileActivityFile> fileActivity, DataTable dt, DateTime date)
        {
            if (fileActivity != null && dt != null)
            {
                try
                {
                    dt.BeginLoadData();
                    foreach (FileActivityFile file in fileActivity.Values)
                    {
                        DataRow newRow = dt.NewRow();
                        newRow["Date"] = date;
                        newRow["IsHistorical"] = false; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                        newRow["FileName"] = file.Filename;
                        newRow["FileType"] = file.FileType;
                        newRow["FilePath"] = file.Filepath;
                        newRow["DriveName"] = file.DriveName;
                        newRow["DatabaseName"] = file.DatabaseName;
                        if (file.ReadsPerSec.HasValue)
                        {
                            newRow["ReadsPerSecond"] = file.ReadsPerSec;
                        }
                        if (file.WritesPerSec.HasValue)
                        {
                            newRow["WritesPerSecond"] = file.WritesPerSec;
                        }
                        if (file.TransfersPerSec.HasValue)
                        {
                            newRow["TransfersPerSecond"] = file.TransfersPerSec;
                        }

                        dt.Rows.Add(newRow);
                    }
                }
                finally
                {
                    dt.EndLoadData();
                }
            }
        }

        private void AddCustomCountersSnapshots(CustomCounterCollectionSnapshot counters, DataTable dt, DateTime date)
        {
            if (counters != null &&
                !counters.CollectionFailed &&
                dt != null)
            {
                try
                {
                    dt.BeginLoadData();
                    foreach (var counter in counters.CustomCounterList.Values)
                    {
                        if (counter.CollectionFailed || counter.Error != null)
                            continue;

                        DataRow newRow = dt.NewRow();
                        newRow["Date"] = date;
                        newRow["IsHistorical"] = false; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                        newRow["MetricID"] = counter.Definition.MetricID;
                        try
                        {
                            decimal? displayValue = null;
                            if (counter.RawValue.HasValue)
                            {
                                newRow["RawValue"] = counter.RawValue;
                                if (counter.Definition.CalculationType == CalculationType.Value)
                                {
                                    displayValue = Convert.ToDecimal(counter.Definition.Scale) * counter.RawValue;
                                }
                            }
                            if (counter.DeltaValue.HasValue)
                            {
                                newRow["DeltaValue"] = counter.DeltaValue;
                                if (counter.Definition.CalculationType == CalculationType.Delta &&
                                    counter.TimeDelta.HasValue && counter.TimeDelta.Value.TotalSeconds > 0)
                                {
                                    displayValue = Convert.ToDecimal(counter.Definition.Scale) *
                                                   (counter.DeltaValue / (decimal) counter.TimeDelta.Value.TotalSeconds);
                                }
                            }

                            if (displayValue.HasValue)
                            {
                                newRow["DisplayValue"] = displayValue.Value;
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.ErrorFormat("Error scaling custom counter {0} values. {1}",
                                    counter.Definition.CounterName, ex);
                        }

                        dt.Rows.Add(newRow);
                    }

                    LastCustomCountersSnapshot = counters;
                }
                finally
                {
                    dt.EndLoadData();
                }
            }
        }

        private void AddLockStatisticsSnapshot(LockStatistics snapshot, DataRow dataRow)
        {
            try
            {
                if (snapshot != null && dataRow != null)
                {
                    if (snapshot.AllocUnitCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - AllocUnit"] =
                            snapshot.AllocUnitCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Application"] =
                            snapshot.ApplicationCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Database"] =
                            snapshot.DatabaseCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Extent"] =
                            snapshot.ExtentCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - File"] =
                            snapshot.FileCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - HoBT"] =
                            snapshot.HeapCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Key"] =
                            snapshot.KeyCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Latch"] =
                            snapshot.LatchCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Metadata"] =
                            snapshot.MetadataCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Object"] =
                            snapshot.ObjectCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Page"] =
                            snapshot.PageCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - RID"] =
                            snapshot.RidCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.AverageWaitTime.HasValue)
                    {
                        dataRow["Average Wait Time - Table"] =
                            snapshot.TableCounters.AverageWaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Average Wait Time - Table"] = DBNull.Value;
                    }

                    if (snapshot.AllocUnitCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - AllocUnit"] = snapshot.AllocUnitCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Application"] = snapshot.ApplicationCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Database"] = snapshot.DatabaseCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Extent"] = snapshot.ExtentCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - File"] = snapshot.FileCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - HoBT"] = snapshot.HeapCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Key"] = snapshot.KeyCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Latch"] = snapshot.LatchCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Metadata"] = snapshot.MetadataCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Object"] = snapshot.ObjectCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Page"] = snapshot.PageCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - RID"] = snapshot.RidCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.Deadlocks.HasValue)
                    {
                        dataRow["Deadlocks - Table"] = snapshot.TableCounters.Deadlocks.Value;
                    }
                    else
                    {
                        dataRow["Deadlocks - Table"] = DBNull.Value;
                    }

                    if (snapshot.TotalCounters.Deadlocks.HasValue)
                    {
                        dataRow["Total Deadlocks"] = snapshot.TotalCounters.Deadlocks;
                    }
                    else
                    {
                        dataRow["Total Deadlocks"] = DBNull.Value;
                    }

                    if (snapshot.AllocUnitCounters.Requests.HasValue)
                    {
                        dataRow["Requests - AllocUnit"] = snapshot.AllocUnitCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Application"] = snapshot.ApplicationCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Database"] = snapshot.DatabaseCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Extent"] = snapshot.ExtentCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.Requests.HasValue)
                    {
                        dataRow["Requests - File"] = snapshot.FileCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.Requests.HasValue)
                    {
                        dataRow["Requests - HoBT"] = snapshot.HeapCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Key"] = snapshot.KeyCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Latch"] = snapshot.LatchCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Metadata"] = snapshot.MetadataCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Object"] = snapshot.ObjectCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Page"] = snapshot.PageCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.Requests.HasValue)
                    {
                        dataRow["Requests - RID"] = snapshot.RidCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.Requests.HasValue)
                    {
                        dataRow["Requests - Table"] = snapshot.TableCounters.Requests.Value;
                    }
                    else
                    {
                        dataRow["Requests - Table"] = DBNull.Value;
                    }

                    if (snapshot.AllocUnitCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - AllocUnit"] = snapshot.AllocUnitCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Application"] = snapshot.ApplicationCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Database"] = snapshot.DatabaseCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Extent"] = snapshot.ExtentCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - File"] = snapshot.FileCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - HoBT"] = snapshot.HeapCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Key"] = snapshot.KeyCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Latch"] = snapshot.LatchCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Metadata"] = snapshot.MetadataCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Object"] = snapshot.ObjectCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Page"] = snapshot.PageCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - RID"] = snapshot.RidCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.Timeouts.HasValue)
                    {
                        dataRow["Timeouts - Table"] = snapshot.TableCounters.Timeouts.Value;
                    }
                    else
                    {
                        dataRow["Timeouts - Table"] = DBNull.Value;
                    }

                    if (snapshot.AllocUnitCounters.Waits.HasValue)
                    {
                        dataRow["Waits - AllocUnit"] = snapshot.AllocUnitCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Application"] = snapshot.ApplicationCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Database"] = snapshot.DatabaseCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Extent"] = snapshot.ExtentCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.Waits.HasValue)
                    {
                        dataRow["Waits - File"] = snapshot.FileCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.Waits.HasValue)
                    {
                        dataRow["Waits - HoBT"] = snapshot.HeapCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Key"] = snapshot.KeyCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Latch"] = snapshot.LatchCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Metadata"] = snapshot.MetadataCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Object"] = snapshot.ObjectCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Page"] = snapshot.PageCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.Waits.HasValue)
                    {
                        dataRow["Waits - RID"] = snapshot.RidCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.Waits.HasValue)
                    {
                        dataRow["Waits - Table"] = snapshot.TableCounters.Waits.Value;
                    }
                    else
                    {
                        dataRow["Waits - Table"] = DBNull.Value;
                    }

                    if (snapshot.AllocUnitCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - AllocUnit"] = snapshot.AllocUnitCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - AllocUnit"] = DBNull.Value;
                    }

                    if (snapshot.ApplicationCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Application"] =
                            snapshot.ApplicationCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Application"] = DBNull.Value;
                    }

                    if (snapshot.DatabaseCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Database"] = snapshot.DatabaseCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Database"] = DBNull.Value;
                    }

                    if (snapshot.ExtentCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Extent"] = snapshot.ExtentCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Extent"] = DBNull.Value;
                    }

                    if (snapshot.FileCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - File"] = snapshot.FileCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - File"] = DBNull.Value;
                    }

                    if (snapshot.HeapCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - HoBT"] = snapshot.HeapCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - HoBT"] = DBNull.Value;
                    }

                    if (snapshot.KeyCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Key"] = snapshot.KeyCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Key"] = DBNull.Value;
                    }

                    if (snapshot.LatchCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Latch"] = snapshot.LatchCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Latch"] = DBNull.Value;
                    }

                    if (snapshot.MetadataCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Metadata"] = snapshot.MetadataCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Metadata"] = DBNull.Value;
                    }

                    if (snapshot.ObjectCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Object"] = snapshot.ObjectCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Object"] = DBNull.Value;
                    }

                    if (snapshot.PageCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Page"] = snapshot.PageCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Page"] = DBNull.Value;
                    }

                    if (snapshot.RidCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - RID"] = snapshot.RidCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - RID"] = DBNull.Value;
                    }

                    if (snapshot.TableCounters.WaitTime.HasValue)
                    {
                        dataRow["Wait Time - Table"] = snapshot.TableCounters.WaitTime.Value.TotalMilliseconds;
                    }
                    else
                    {
                        dataRow["Wait Time - Table"] = DBNull.Value;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the lock statistics snapshot.", e);
            }
        }

        private void AddTempdbStatisticsSnapshot(TempdbSummaryStatistics snapshot, DataRow dataRow)
        {
            try
            {
                if (snapshot != null && dataRow != null)
                {
                    dataRow["TempdbGAMWaitTime"] =
                        snapshot.TempdbGAMWaitTime.TotalMilliseconds;

                    dataRow["TempdbPFSWaitTime"] =
                        snapshot.TempdbPFSWaitTime.TotalMilliseconds;

                    dataRow["TempdbSGAMWaitTime"] =
                        snapshot.TempdbSGAMWaitTime.TotalMilliseconds;

                    if (snapshot.VersionStoreCleanupKilobytes.HasValue)
                    {
                        dataRow["VersionStoreCleanupKilobytes"] =
                            snapshot.VersionStoreCleanupKilobytes.Value;
                    }
                    else
                    {
                        dataRow["VersionStoreCleanupKilobytes"] = DBNull.Value;
                    }

                    if (snapshot.VersionStoreGenerationKilobytes.HasValue)
                    {
                        dataRow["VersionStoreGenerationKilobytes"] =
                            snapshot.VersionStoreGenerationKilobytes.Value;
                    }
                    else
                    {
                        dataRow["VersionStoreGenerationKilobytes"] = DBNull.Value;
                    }

                    if (snapshot.UserObjectsMegabytes.HasValue)
                    {
                        dataRow["TempdbUserObjectsMegabytes"] =
                            snapshot.UserObjectsMegabytes.Value;
                    }
                    else
                    {
                        dataRow["TempdbUserObjectsMegabytes"] = DBNull.Value;
                    }

                    if (snapshot.InternalObjectsMegabytes.HasValue)
                    {
                        dataRow["TempdbInternalObjectsMegabytes"] =
                            snapshot.InternalObjectsMegabytes.Value;
                    }
                    else
                    {
                        dataRow["TempdbInternalObjectsMegabytes"] = DBNull.Value;
                    }

                    if (snapshot.VersionStoreMegabytes.HasValue)
                    {
                        dataRow["TempdbVersionStoreMegabytes"] =
                            snapshot.VersionStoreMegabytes.Value;
                    }
                    else
                    {
                        dataRow["TempdbVersionStoreMegabytes"] = DBNull.Value;
                    }

                    if (snapshot.MixedExtentsMegabytes.HasValue)
                    {
                        dataRow["TempdbMixedExtentsMegabytes"] =
                            snapshot.MixedExtentsMegabytes.Value;
                    }
                    else
                    {
                        dataRow["TempdbMixedExtentsMegabytes"] = DBNull.Value;
                    }

                    if (snapshot.UnallocatedSpaceMegabytes.HasValue)
                    {
                        dataRow["TempdbUnallocatedSpaceMegabytes"] =
                            snapshot.UnallocatedSpaceMegabytes.Value;
                    }
                    else
                    {
                        dataRow["TempdbUnallocatedSpaceMegabytes"] = DBNull.Value;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the tempdb statistics snapshot.", e);
            }
        }

        private void AddResourcesSnapshot(ServerOverview snapshot, DataRow dataRow)
        {
            try
            {
                if (snapshot != null && snapshot.Error == null && dataRow != null)
                {
                    #region CPU Data

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.BatchRequests.HasValue)
                    {
                        dataRow["CallRatesBatches"] = snapshot.Statistics.BatchRequests /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["CallRatesBatches"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.Transactions.HasValue)
                    {
                        dataRow["CallRatesTransactions"] = snapshot.Statistics.Transactions /
                                                         snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["CallRatesTransactions"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.SqlCompilations.HasValue)
                    {
                        dataRow["CallRatesCompiles"] = snapshot.Statistics.SqlCompilations /
                                                       snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["CallRatesCompiles"] = DBNull.Value;
                    }
                    if (snapshot.AWSCloudMetrics.Count() > 0)
                    {
                        dataRow["CPUCreditBalance"] = snapshot.AWSCloudMetrics["CPUCreditBalance"];//snapshot.CPUCreditBalance.HasValue ? snapshot.CPUCreditBalance.Value : 0;
                        dataRow["CPUCreditUsage"] = snapshot.AWSCloudMetrics["CPUCreditUsage"];//snapshot.CPUCreditUsage.HasValue ? snapshot.CPUCreditUsage.Value : 0;
                        dataRow["WriteThroughput"] = snapshot.AWSCloudMetrics["WriteThroughput"];//snapshot.WriteThroughput.HasValue ? snapshot.WriteThroughput.Value : 0;
                        dataRow["WriteLatency"] = snapshot.AWSCloudMetrics["WriteLatency"];//snapshot.WriteLatency.HasValue ? snapshot.WriteLatency.Value : 0;
                        dataRow["SwapUsage"] = snapshot.AWSCloudMetrics["SwapUsage"];//snapshot.SwapUsage.HasValue ? snapshot.SwapUsage.Value : 0;
                        dataRow["ReadLatency"] = snapshot.AWSCloudMetrics["ReadLatency"];//snapshot.ReadLatency.HasValue ? snapshot.ReadLatency.Value : 0;
                        dataRow["ReadThroughput"] = snapshot.AWSCloudMetrics["ReadThroughput"];//snapshot.ReadThroughput.HasValue ? snapshot.ReadThroughput.Value : 0;
                        dataRow["DiskQueueDepth"] = snapshot.AWSCloudMetrics["DiskQueueDepth"];//snapshot.DiskQueueDepth.HasValue ? snapshot.DiskQueueDepth.Value : 0;
                    }
                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.SqlRecompilations.HasValue)
                    {
                        dataRow["CallRatesReCompiles"] = snapshot.Statistics.SqlRecompilations /
                                                         snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["CallRatesReCompiles"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.ProcessorQueueLength.HasValue)
                    {
                        dataRow["ProcessorQueueLength"] = snapshot.OSMetricsStatistics.ProcessorQueueLength;
                    }
                    else
                    {
                        dataRow["ProcessorQueueLength"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.PercentPrivilegedTime.HasValue)
                    {
                        dataRow["PercentPrivilegedTime"] = snapshot.OSMetricsStatistics.PercentPrivilegedTime;
                    }
                    else
                    {
                        dataRow["PercentPrivilegedTime"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.PercentUserTime.HasValue)
                    {
                        dataRow["PercentUserTime"] = snapshot.OSMetricsStatistics.PercentUserTime;
                    }
                    else
                    {
                        dataRow["PercentUserTime"] = DBNull.Value;
                    }

                    #endregion

                    #region Memory Data

                    if (snapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes.HasValue)
                    {
                        dataRow["OsTotalPhysicalMemory"] = snapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes;

                        if (snapshot.OSMetricsStatistics.AvailableBytes.Megabytes.HasValue)
                        {
                            dataRow["OsMemoryUsed"] = snapshot.OSMetricsStatistics.TotalPhysicalMemory.Megabytes -
                                                      snapshot.OSMetricsStatistics.AvailableBytes.Megabytes;
                        }
                        else
                        {
                            dataRow["OsMemoryUsed"] = DBNull.Value;
                        }
                    }
                    else
                    {
                        dataRow["OsTotalPhysicalMemory"] = DBNull.Value;
                        dataRow["OsMemoryUsed"] = DBNull.Value;
                    }

                    if (snapshot.TargetServerMemory.Megabytes.HasValue)
                    {
                        dataRow["SqlServerMemoryAllocated"] = snapshot.TargetServerMemory.Megabytes;
                    }
                    else
                    {
                        dataRow["SqlServerMemoryAllocated"] = DBNull.Value;
                    }

                    if (snapshot.TotalServerMemory.Megabytes.HasValue)
                    {
                        dataRow["SqlServerMemoryUsed"] = snapshot.TotalServerMemory.Megabytes;
                    }
                    else
                    {
                        dataRow["SqlServerMemoryUsed"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.BufferCacheHitRatio.HasValue)
                    {
                        dataRow["BufferCacheHitRatio"] = snapshot.Statistics.BufferCacheHitRatio;
                    }
                    else
                    {
                        dataRow["BufferCacheHitRatio"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.CacheHitRatio.HasValue)
                    {
                        dataRow["ProcedureCacheHitRatio"] = snapshot.Statistics.CacheHitRatio;
                    }
                    else
                    {
                        dataRow["ProcedureCacheHitRatio"] = DBNull.Value;
                    }

                    //if (snapshot.OSMetricsStatistics.PagesPersec.HasValue)
                    //{
                    //    dataRow["PagesPerSec"] = snapshot.OSMetricsStatistics.PagesPersec;
                    //}
                    //else
                    //{
                    //    dataRow["PagesPerSec"] = DBNull.Value;
                    //}

                    if (snapshot.MemoryStatistics.BufferCachePages.Megabytes.HasValue)
                    {
                        dataRow["BufferCachePages"] = snapshot.MemoryStatistics.BufferCachePages.Megabytes;
                    }
                    else
                    {
                        dataRow["BufferCachePages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.FreePages.Megabytes.HasValue)
                    {
                        dataRow["BufferCacheFreePages"] = snapshot.MemoryStatistics.FreePages.Megabytes;
                    }
                    else
                    {
                        dataRow["BufferCacheFreePages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.BufferCachePages.Megabytes.HasValue &&
                        snapshot.MemoryStatistics.FreePages.Megabytes.HasValue)
                    {
                        dataRow["BufferCacheActivePages"] = snapshot.MemoryStatistics.BufferCachePages.Megabytes -
                                                            snapshot.MemoryStatistics.FreePages.Megabytes;
                    }
                    else
                    {
                        dataRow["BufferCacheActivePages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.CommittedPages.Megabytes.HasValue)
                    {
                        dataRow["CommittedPages"] = snapshot.MemoryStatistics.CommittedPages.Megabytes;
                    }
                    else
                    {
                        dataRow["CommittedPages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.CachePages.Megabytes.HasValue)
                    {
                        dataRow["ProcedureCachePages"] = snapshot.MemoryStatistics.CachePages.Megabytes;
                    }
                    else
                    {
                        dataRow["ProcedureCachePages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.FreeCachePages.Megabytes.HasValue)
                    {
                        dataRow["ProcedureCacheFreePages"] = snapshot.MemoryStatistics.FreeCachePages.Megabytes;
                    }
                    else
                    {
                        dataRow["ProcedureCacheFreePages"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.CachePages.Megabytes.HasValue &&
                        snapshot.MemoryStatistics.FreeCachePages.Megabytes.HasValue)
                    {
                        dataRow["ProcedureCacheActivePages"] = snapshot.MemoryStatistics.CachePages.Megabytes -
                                                               snapshot.MemoryStatistics.FreeCachePages.Megabytes;
                    }
                    else
                    {
                        dataRow["ProcedureCacheActivePages"] = DBNull.Value;
                    }

                    bool otherSet = false;
                    decimal other = 0;

                    if (snapshot.MemoryStatistics.OptimizerMemory.Megabytes.HasValue)
                    {
                        dataRow["OptimizerMemory"] = snapshot.MemoryStatistics.OptimizerMemory.Megabytes;
                        other = snapshot.MemoryStatistics.OptimizerMemory.Megabytes.Value;
                        otherSet = true;
                    }
                    else
                    {
                        dataRow["OptimizerMemory"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.LockMemory.Megabytes.HasValue)
                    {
                        dataRow["LockMemory"] = snapshot.MemoryStatistics.LockMemory.Megabytes;
                        other += snapshot.MemoryStatistics.LockMemory.Megabytes.Value;
                        otherSet = true;
                    }
                    else
                    {
                        dataRow["LockMemory"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.ConnectionMemory.Megabytes.HasValue)
                    {
                        dataRow["ConnectionMemory"] = snapshot.MemoryStatistics.ConnectionMemory.Megabytes;
                        other += snapshot.MemoryStatistics.ConnectionMemory.Megabytes.Value;
                        otherSet = true;
                    }
                    else
                    {
                        dataRow["ConnectionMemory"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.GrantedWorkspaceMemory.Megabytes.HasValue)
                    {
                        dataRow["SortHashIndexMemory"] = snapshot.MemoryStatistics.GrantedWorkspaceMemory.Megabytes;
                        other += snapshot.MemoryStatistics.GrantedWorkspaceMemory.Megabytes.Value;
                        otherSet = true;
                    }
                    else
                    {
                        dataRow["SortHashIndexMemory"] = DBNull.Value;
                    }

                    if (snapshot.MemoryStatistics.CommittedPages.Megabytes.HasValue)
                    {
                        dataRow["UsedDataMemory"] = snapshot.MemoryStatistics.CommittedPages.Megabytes;
                    }
                    else
                    {
                        dataRow["UsedDataMemory"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.PageLifeExpectancySeconds.HasValue)
                    {
                        dataRow["PageLifeExpectancy"] = snapshot.Statistics.PageLifeExpectancySeconds;
                    }
                    else
                    {
                        dataRow["PageLifeExpectancy"] = DBNull.Value;
                    }

                    if (otherSet)
                    {
                        dataRow["LockOptimizerConnectionSortHashIndexMemory"] = other;
                    }
                    else
                    {
                        dataRow["LockOptimizerConnectionSortHashIndexMemory"] = DBNull.Value;

                    }

                    #endregion

                    #region Disk Data

                    //if (snapshot.OSMetricsStatistics.PercentDiskTime.HasValue)
                    //{
                    //    dataRow["PercentDiskTime"] = snapshot.OSMetricsStatistics.PercentDiskTime;
                    //}
                    //else
                    //{
                    //    dataRow["PercentDiskTime"] = DBNull.Value;
                    //}

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.CheckpointPages.HasValue)
                    {
                        dataRow["CheckpointWrites"] = snapshot.Statistics.CheckpointPages /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["CheckpointWrites"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.LazyWrites.HasValue)
                    {
                        dataRow["LazyWriterWrites"] = snapshot.Statistics.LazyWrites /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["LazyWriterWrites"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.ReadaheadPages.HasValue)
                    {
                        dataRow["ReadAheadReads"] = snapshot.Statistics.ReadaheadPages /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["ReadAheadReads"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.PageReads.HasValue)
                    {
                        dataRow["SynchronousReads"] = snapshot.Statistics.PageReads /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["SynchronousReads"] = DBNull.Value;
                    }

                    if (snapshot.TimeDelta.HasValue &&
                        snapshot.TimeDelta.Value.TotalSeconds > 0 &&
                        snapshot.Statistics.PageWrites.HasValue)
                    {
                        dataRow["SynchronousWrites"] = snapshot.Statistics.PageWrites /
                                                      snapshot.TimeDelta.Value.TotalSeconds;
                    }
                    else
                    {
                        dataRow["SynchronousWrites"] = DBNull.Value;
                    }

                    if (snapshot.OSMetricsStatistics.AvgDiskQueueLength.HasValue)
                    {
                        dataRow["DiskQueueLength"] = snapshot.OSMetricsStatistics.AvgDiskQueueLength;
                    }
                    else
                    {
                        dataRow["DiskQueueLength"] = DBNull.Value;
                    }

                    if (snapshot.Statistics.DiskErrors.HasValue)
                    {
                        dataRow["ReadWriteErrors"] = snapshot.Statistics.DiskErrors;
                    }
                    else
                    {
                        dataRow["ReadWriteErrors"] = DBNull.Value;
                    }
                    
                    #endregion

                    #region Waits
                    snapshot.CalculateWaitStatisticsSummary(ApplicationModel.Default.WaitTypes);
                    if (snapshot.WaitStatsSummary != null)
                    {
                        dataRow["IOWaits"] = snapshot.WaitStatsSummary.IOWaits;
                        dataRow["LockWaits"] = snapshot.WaitStatsSummary.LockWaits;
                        dataRow["MemoryWaits"] = snapshot.WaitStatsSummary.MemoryWaits;
                        dataRow["TransactionLogWaits"] = snapshot.WaitStatsSummary.TransactionLogWaits;
                        dataRow["OtherWaits"] = snapshot.WaitStatsSummary.OtherWaits;
                        dataRow["SignalWaits"] = snapshot.WaitStatsSummary.SignalWaits;
                    }

                    #endregion
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the resources snapshot.", e);
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Used new disk table for disk charts
        /// </summary>
        private void AddDiskSnapshot(ServerOverview snapshot, DataTable table, DateTime date)
        {
            try
            {
                if (snapshot != null && snapshot.Error == null && table != null && snapshot.DiskDrives.Count > 0)
                {
                    var dataRow = table.NewRow();
                    dataRow["Date"] = date;
                    dataRow["IsHistorical"] = false; // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                    foreach (DiskDrive diskDrive in snapshot.DiskDrives.Values)
                    {
                        string diskUsagePerDiskColumnName = DiskUsagePerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskUsagePerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskUsagePerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.DiskBusyPercent.HasValue)
                        {
                            dataRow[diskUsagePerDiskColumnName] = diskDrive.DiskBusyPercent;
                        }
                        else
                        {
                            dataRow[diskUsagePerDiskColumnName] = DBNull.Value;
                        }

                        string diskQueueLengthPerDiskColumnName = DiskQueueLengthPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskQueueLengthPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskQueueLengthPerDiskColumnName, typeof(ulong));
                        }

                        if (diskDrive.AverageDiskQueueLength.HasValue)
                        {
                            dataRow[diskQueueLengthPerDiskColumnName] = diskDrive.AverageDiskQueueLength;
                        }
                        else
                        {
                            dataRow[diskQueueLengthPerDiskColumnName] = DBNull.Value;
                        }

                        string diskReadsPerSecPerDiskColumnName = DiskReadsPerSecPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskReadsPerSecPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskReadsPerSecPerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.DiskReadsPerSec.HasValue)
                        {
                            dataRow[diskReadsPerSecPerDiskColumnName] = diskDrive.DiskReadsPerSec;
                        }
                        else
                        {
                            dataRow[diskReadsPerSecPerDiskColumnName] = DBNull.Value;
                        }

                        string diskTransfersPerSecPerDiskColumnName = DiskTransfersPerSecPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskTransfersPerSecPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskTransfersPerSecPerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.DiskTransfersPerSec.HasValue)
                        {
                            dataRow[diskTransfersPerSecPerDiskColumnName] = diskDrive.DiskTransfersPerSec;
                        }
                        else
                        {
                            dataRow[diskTransfersPerSecPerDiskColumnName] = DBNull.Value;
                        }

                        string diskWritesPerSecPerDiskColumnName = DiskWritesPerSecPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskWritesPerSecPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskWritesPerSecPerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.DiskWritesPerSec.HasValue)
                        {
                            dataRow[diskWritesPerSecPerDiskColumnName] = diskDrive.DiskWritesPerSec;
                        }
                        else
                        {
                            dataRow[diskWritesPerSecPerDiskColumnName] = DBNull.Value;
                        }

                        string diskTimePerReadPerDiskColumnName = DiskTimePerReadPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskTimePerReadPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskTimePerReadPerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.AvgDiskSecPerRead.HasValue)
                        {
                            dataRow[diskTimePerReadPerDiskColumnName] = diskDrive.AvgDiskSecPerRead.Value.TotalMilliseconds;
                        }
                        else
                        {
                            dataRow[diskTimePerReadPerDiskColumnName] = DBNull.Value;
                        }

                        string diskTimePerTransferPerDiskColumnName = DiskTimePerTransferPerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskTimePerTransferPerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskTimePerTransferPerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.AvgDiskSecPerTransfer.HasValue)
                        {
                            dataRow[diskTimePerTransferPerDiskColumnName] = diskDrive.AvgDiskSecPerTransfer.Value.TotalMilliseconds;
                        }
                        else
                        {
                            dataRow[diskTimePerTransferPerDiskColumnName] = DBNull.Value;
                        }

                        string diskTimePerWritePerDiskColumnName = DiskTimePerWritePerDiskColumnPrefix + diskDrive.DriveLetter;

                        if (!realTimeDiskDataTable.Columns.Contains(diskTimePerWritePerDiskColumnName))
                        {
                            realTimeDiskDataTable.Columns.Add(diskTimePerWritePerDiskColumnName, typeof(double));
                        }

                        if (diskDrive.AvgDiskSecPerWrite.HasValue)
                        {
                            dataRow[diskTimePerWritePerDiskColumnName] = diskDrive.AvgDiskSecPerWrite.Value.TotalMilliseconds;
                        }
                        else
                        {
                            dataRow[diskTimePerWritePerDiskColumnName] = DBNull.Value;
                        }
                    }
                    table.Rows.Add(dataRow);
                }
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the disk snapshot.", e);
            }
        }

        //6.2.4
        private void AddAzureDiskSanpshot(List<Dictionary<String, String>> azureIOMetrics, DataTable table, DateTime date)
        {
            try
            {
                var dataRow = table.NewRow();
                dataRow["Date"] = date;

                if (azureIOMetrics != null && azureIOMetrics.Count > 0)
                {
                    double dataIOUsage = 0;
                    double logIOUsage = 0;
                    double dataIORate = 0;
                    double logIORate = 0;

                    String dataIOUsageStr;
                    String logIOUsageStr;
                    String dataIORateStr;
                    String logIORateStr;

                    foreach (var azureIoMetric in azureIOMetrics)
                    {
                        var initial = azureIoMetric;
                        initial.TryGetValue("dataIOUsage", out dataIOUsageStr);
                        initial.TryGetValue("logIOUsage", out logIOUsageStr);
                        initial.TryGetValue("dataIORate", out dataIORateStr);
                        initial.TryGetValue("logIORate", out logIORateStr);

                        dataIOUsage += Double.Parse(dataIOUsageStr);
                        logIOUsage += Double.Parse(logIOUsageStr);
                        dataIORate += Double.Parse(dataIORateStr);
                        logIORate += Double.Parse(logIORateStr);

                    }

                    dataRow["DataIOUsage"] = dataIOUsage;
                    dataRow["LogIOUsage"] = logIOUsage;
                    dataRow["DataIORate"] = dataIORate;
                    dataRow["LogIORate"] = logIORate;

                }
                else
                {
                    dataRow["DataIOUsage"] = DBNull.Value;
                    dataRow["LogIOUsage"] = DBNull.Value;
                    dataRow["DataIORate"] = DBNull.Value;
                    dataRow["LogIORate"] = DBNull.Value;
                }

                table.Rows.Add(dataRow);
            }
            catch (Exception e)
            {
                Log.Error("An error occurred while adding the Azure disk snapshot.", e);
            }
        }

        public void BackfillScheduledRefreshData(int instanceId, Control foregroundControl)
        {
            if (realTimeSnapshotsDataTable != null)
            {
                var rtdt = this.RealTimeSnapshotsDataTable;
                var lastRowIndex = rtdt.Rows.Count - 1;
                if (lastRowIndex <= 0)
                {
                    Log.Info("No data - doing prepopulate");
                    // if no rows then prepopulate should do what we need
                    PrePopulateRealTimeSnapshots(instanceId, foregroundControl);
                    return;
                }

                var now = DateTime.Now;
                var lastRow = rtdt.Rows[lastRowIndex];
                //Handling exception when (DateTime)lastRow["Date"] gives DBnull
                //10.0 SQLdm srishti purohit
                var lastDate = DateTime.MinValue;
                if(lastRow["Date"] != null)
                    DateTime.TryParse(lastRow["Date"].ToString(), out lastDate);
                // SqlDM 10.2 (New History Browser) - If SD = 5mins and KD = 60min, then we need to reload entire data only if
                // we have no data for previous max(5,60) = 60 minutes. 
                var timeDiff = now - lastDate;
                if (timeDiff > TimeSpan.FromMinutes(MaximumKeepData)) 
                {
                    Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                    // if last data point is older than our grooming period then prepopulate should work
                    PrePopulateRealTimeSnapshots(instanceId, foregroundControl);
                    return;
                }

                var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
                if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
                {
                    Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                    return;
                }

                Log.InfoFormat("Last data point is from {0} - backfilling {1} minutes of data", lastDate, (int)(timeDiff.TotalMinutes));
                lock (updateLock)
                {
                    var serverSummary = RepositoryHelper.GetServerSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                          instanceId, now, (int)timeDiff.TotalMinutes);

                    var dbStatistics = RepositoryHelper.GetDatabaseCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                             instanceId, now, (int)timeDiff.TotalMinutes);

                    var fileActivity = RepositoryHelper.GetFileActivity(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                         instanceId, now, (int)timeDiff.TotalMinutes);

                    var customCounters = RepositoryHelper.GetCustomCounterStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                        instanceId, now, (int)timeDiff.TotalMinutes);
                    
                    if (foregroundControl != null && foregroundControl.InvokeRequired)
                    {
                        MethodInvoker methodInvoker = () => PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters, false);
                        foregroundControl.Invoke(methodInvoker);
                    }
                    else
                        PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters, false);

                    realTimeSnapshotsPrePopulated = true;
                }
            }
        }

        public void PrePopulateRealTimeSnapshots(int instanceId, Control foregroundControl)
        {
            if (realTimeSnapshotsDataTable != null)
            {
                lock (updateLock)
                {
                    DateTime now = DateTime.Now;
                    EndTimeForBaselineStats = now;

                    ServerSummaryHistoryDataContainer serverSummary =
                        RepositoryHelper.GetServerSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                          instanceId, now,
                                                          ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

                    DataTable dbStatistics = RepositoryHelper.GetDatabaseCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                         instanceId, now,
                                                         ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

                    DataTable fileActivity = RepositoryHelper.GetFileActivity(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                         instanceId, now,
                                                         ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    
                    DataTable customCounters = RepositoryHelper.GetCustomCounterStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                                                         instanceId, now,
                                                         ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);

                   
                    if (foregroundControl != null && foregroundControl.InvokeRequired)
                    {
                        MethodInvoker methodInvoker = delegate { PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters); };
                        foregroundControl.Invoke(methodInvoker);
                    }
                    else
                        PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters);

                    realTimeSnapshotsPrePopulated = true;
                }
            }
        }

        void PrePopulateRealTimeSnapshotsDelegate(ServerSummaryHistoryDataContainer serverSummary, DataTable dbStatistics, DataTable fileActivity, DataTable customCounters, bool clear = true)
        {
            try
            {
                realTimeDbSnapshotsDataTable.BeginLoadData();
                if (clear) realTimeDbSnapshotsDataTable.Clear();
                PopulateDbDataTableWithHistory(realTimeDbSnapshotsDataTable, dbStatistics);
                realTimeSnapshotsDataTable.BeginLoadData();
                realTimeDiskDataTable.BeginLoadData(); // SqlDM 10.2 (Anshul Aggarwal) - New disk table for disk charts.
                if (clear)
                {
                    realTimeSnapshotsDataTable.Clear();
                    realTimeDiskDataTable.Clear();
                }

               
                    PopulateDataTableWithHistory(realTimeSnapshotsDataTable, serverSummary.OverviewStatistics);

             
                PopulateDiskDataTableWithHistory(realTimeDiskDataTable, serverSummary.DiskDriveInfo);

                //6.2.4
                realTimeAzureDataTable.BeginLoadData();
                if (clear) {
                    realTimeAzureDataTable.Clear();
                }

                PopulateAzureDiskDataTableWithHistory(realTimeAzureDataTable, serverSummary.OverviewStatistics);
              
                
            }
            finally
            {
                realTimeDbSnapshotsDataTable.EndLoadData();
                realTimeSnapshotsDataTable.EndLoadData();
                realTimeDiskDataTable.EndLoadData();
                //6.2.4
                realTimeAzureDataTable.EndLoadData();
            }

            try
            {
                realTimeFileSnapshotsDataTable.BeginLoadData();
                if (clear) realTimeFileSnapshotsDataTable.Clear();
                PopulateFilesDataTableWithHistory(realTimeFileSnapshotsDataTable, fileActivity);
            }
            finally
            {
                realTimeFileSnapshotsDataTable.EndLoadData();
            }

            if (customCounters != null)
            {
                try
                {
                    realTimeCustomCountersDataTable.BeginLoadData();
                    if (clear) realTimeCustomCountersDataTable.Clear();
                    PopulateCustomCountersDataTableWithHistory(realTimeCustomCountersDataTable, customCounters);
                }
                finally
                {
                    realTimeCustomCountersDataTable.EndLoadData();
                }
            }
        }

        static DateTime TruncateToSecond(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Kind);
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Replaces realtime data that is now stale with data from repository.
        /// </summary>
        public void BackFillScheduledHistoricalData(int instanceId, Control foregroundControl)
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (realTimeSnapshotsDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = GetBackFillHistoricalRange(realTimeSnapshotsDataTable, out startDateTime, out endDateTime);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(instanceId, foregroundControl, startDateTime, endDateTime);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        public void ForwardFillHistoricalData(int instanceId, Control foregroundControl)
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (realTimeSnapshotsDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    GetForwardFillHistoricalRange(realTimeSnapshotsDataTable, out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;
                    
                    Log.InfoFormat("Backfilling from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateRealTimeSnapshots(instanceId, foregroundControl, startDateTime, endDateTime);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Pouplates real-time data from repository with custom start-end times.
        /// </summary>
        private void PopulateRealTimeSnapshots(int instanceId, Control foregroundControl, DateTime startDateTime, DateTime endDateTime)
        {
            lock (updateLock)
            {
                var serverSummary = RepositoryHelper.GetServerSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                instanceId, endDateTime, null, startDateTime);

                var dbStatistics = RepositoryHelper.GetDatabaseCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                instanceId, endDateTime, null, startDateTime);

                var fileActivity = RepositoryHelper.GetFileActivity(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                instanceId, endDateTime, null, startDateTime);

                var customCounters = RepositoryHelper.GetCustomCounterStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                instanceId, endDateTime, null, startDateTime);
               
                if (foregroundControl != null && foregroundControl.InvokeRequired)
                {
                    MethodInvoker methodInvoker = () => PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters, false);
                    foregroundControl.Invoke(methodInvoker);
                }
                else
                    PrePopulateRealTimeSnapshotsDelegate(serverSummary, dbStatistics, fileActivity, customCounters, false);
            }
         }

        public ServerSummarySnapshots PopulateHistoricalSnapshots(int instanceId, System.Windows.Forms.Control foregroundControl, BackgroundWorker worker)
        {
            using (Log.VerboseCall("PopulateHistoricalSnapshots"))
            {
                if (historicalSnapshotsDataTable != null && HistoricalSnapshotDateTime.HasValue)
                {
                    return GetHisoricalSnapshots(instanceId, foregroundControl, worker, HistoricalStartDateTime, HistoricalSnapshotDateTime.Value,
                        HistoricalStartDateTime.HasValue ? MathHelper.GetCeilMinutes(HistoricalStartDateTime, HistoricalSnapshotDateTime) : 
                        ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                }
                return null;
            }
        }

         /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Populates historical data with custom start-end times.
        /// </summary>
       private ServerSummarySnapshots GetHisoricalSnapshots(int instanceId, System.Windows.Forms.Control foregroundControl, BackgroundWorker worker,
            DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {
            ServerSummarySnapshots serverSummarySnapshots = null;
            //Sqldm-28694 start
            int curr_seconds=endDateTime.Second;
            //changing seconds to 59 so that we can get the snapshot of same time at two different servers
          endDateTime=  endDateTime.AddSeconds(59 - curr_seconds);
        //sqldm-28694 end
            lock (updateLock)
            {
                ServerSummaryHistoryDataContainer serverSummary = RepositoryHelper.GetServerSummary(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        instanceId, endDateTime, minutes, startDateTime);
                DataTable dbStatistics = RepositoryHelper.GetDatabaseCounters(Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        instanceId, endDateTime, minutes, startDateTime);
                DataTable fileActivity = RepositoryHelper.GetFileActivity(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId, 
                    endDateTime, minutes, startDateTime);
                DataTable customCounters = RepositoryHelper.GetCustomCounterStatistics(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId
                    , endDateTime, minutes, startDateTime);

                // SQLdm 10.0 (Tarun Sapra) - Gets all the baselines data and insert that into the data table as per the metricId in data table
                // SQLDM-27575: adding the old changes to fix the odd behavior related left gaps
                DataTable baselinesDataTable = new DataTable();
                if (serverSummary.OverviewStatistics != null && serverSummary.OverviewStatistics.Rows.Count > 1)
                {
                    if (serverSummary.OverviewStatistics.Columns.Contains("CollectionDateTime"))
                    {
                        var startDate = Convert.ToDateTime(serverSummary.OverviewStatistics.Rows[firstIndex]["CollectionDateTime"], CultureInfo.InvariantCulture);
                        var endDate = Convert.ToDateTime(serverSummary.OverviewStatistics.Rows[serverSummary.OverviewStatistics.Rows.Count - secondIndex]["CollectionDateTime"], CultureInfo.InvariantCulture);
                        Log.InfoFormat("Fetching baseline details between : {0} and {1}.", startDate , endDate);

                        baselinesDataTable = RepositoryHelper.FillBaselineData(
                        Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                        InstanceId,
                        endDate.ToUniversalTime(),
                        (int)(endDate.ToUniversalTime() - startDate.ToUniversalTime()).TotalSeconds,
                        (DateTime.Now.Date - DateTime.Now.ToUniversalTime().Date).TotalDays);
                    }
                    else
                    {
                        Log.ErrorFormat("In savedHistoryDataTable table column 'Date' not found.");
                    }
                }
                else
                {
                    var date = endDateTime.ToUniversalTime();
                    if (minutes == null)
                        minutes = MathHelper.GetMinutes(startDateTime, endDateTime) ?? 0;
                    int seconds = minutes.Value * 60 - 10;

                    baselinesDataTable = RepositoryHelper.FillBaselineData(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                    InstanceId, date, seconds, (endDateTime - endDateTime.ToUniversalTime()).TotalDays);
                }
                
                historicalSnapshotStatus = serverSummary.ServerStatus;
                // iterate over each row and deserialize all the lock statistics into a new column
                DataColumn lso = serverSummary.OverviewStatistics.Columns.Add("LockStatisticsObject", typeof(LockStatistics));
                if (worker == null || !worker.CancellationPending)
                {
                    foreach (DataRow row in serverSummary.OverviewStatistics.Rows)
                    {
                        object value = row["LockStatistics"];
                        if (value != DBNull.Value)
                        {
                            LockStatistics lockStatistics = Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>((byte[])value);
                            row[lso] = (lockStatistics != null) ? (object)lockStatistics : DBNull.Value;
                        }
                        else
                        row[lso] = DBNull.Value;
                    }
                }

                bool isCustomRange = startDateTime != null;
                if (worker == null || !worker.CancellationPending)
                {
                    if (foregroundControl != null && foregroundControl.InvokeRequired)
                    {
                        System.Windows.Forms.MethodInvoker methodInvoker = delegate
                        {
                            PopulateHistoricalDelegate(serverSummary, dbStatistics, fileActivity, customCounters, ref serverSummarySnapshots,
                                baselinesDataTable, instanceId, isCustomRange);
                        };
                        foregroundControl.Invoke(methodInvoker);
                    }
                    else
                        PopulateHistoricalDelegate(serverSummary, dbStatistics, fileActivity, customCounters, ref serverSummarySnapshots,
                            baselinesDataTable, instanceId, isCustomRange);
                }
            }
            
            
            historicalSnapshots = serverSummarySnapshots;
            return serverSummarySnapshots;
        }

        void PopulateHistoricalDelegate(ServerSummaryHistoryDataContainer serverSummary, DataTable dbStatistics, DataTable fileActivity, DataTable customCounters, 
            ref ServerSummarySnapshots serverSummarySnapshots, DataTable baselinesDataTable, int? instanceId, bool isCustom)
        {           
            using (Log.VerboseCall("PopulateHistoricalDelegate"))
            {
                try
                {
                    historicalSnapshotsDataTable.BeginLoadData();
                    historicalSnapshotsDataTable.Clear();

                    historicalDbSnapshotsDataTable.BeginLoadData();
                    historicalDbSnapshotsDataTable.Clear();

                    historicalFileSnapshotsDataTable.BeginLoadData();
                    historicalFileSnapshotsDataTable.Clear();

                    historicalCustomCountersDataTable.BeginLoadData();
                    historicalCustomCountersDataTable.Clear();

                    historicalDiskDataTable.BeginLoadData();  // SqlDM 10.2 (Anshul Aggarwal) - New disk table for disk charts.
                    historicalDiskDataTable.Clear();

                    //6.2.4
                    historicalAzureDiskDataTable.BeginLoadData();
                    historicalAzureDiskDataTable.Clear();

                    int? cloudProviderId = null;
                    
                    //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
                    if(instanceId != null)
                        cloudProviderId = RepositoryHelper.GetCloudProviderIdForMonitoredServer(Settings.Default.ActiveRepositoryConnection.ConnectionInfo, instanceId.Value);
                    
                    if (historicalSnapshotStatus != null || cloudProviderId != null)
                    {
                        //SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
                        if (cloudProviderId != null)                        
                            historicalSnapshotStatus = new MonitoredSqlServerStatus();                                                    

                        historicalSnapshotStatus.IsHistoricalStatus = true;

                        if (serverSummary.OverviewStatistics != null &&
                            serverSummary.OverviewStatistics.Rows.Count > 0)
                        {

                            PopulateDbDataTableWithHistory(historicalDbSnapshotsDataTable, dbStatistics);
                            PopulateDataTableWithHistory(historicalSnapshotsDataTable, serverSummary.OverviewStatistics);

                            // SqlDM 10.2 (Anshul Aggarwal) - New disk table for disk charts.
                            PopulateDiskDataTableWithHistory(historicalDiskDataTable, serverSummary.DiskDriveInfo);

                            

                            // SqlDM 10.2 (Anshul Aggarwal) - Populate baselines
                            PopulateBaselinesDataTableWithHistory(historicalSnapshotsDataTable, historicalDiskDataTable, baselinesDataTable);


                            PopulateFilesDataTableWithHistory(historicalFileSnapshotsDataTable, fileActivity);

                            PopulateCustomCountersDataTableWithHistory(historicalCustomCountersDataTable, customCounters);

                            DateTime lastSnapshotDateTime = (DateTime)serverSummary.OverviewStatistics.Rows[serverSummary.OverviewStatistics.Rows.Count - 1]["CollectionDateTime"];
                            string select = String.Format("UTCCollectionDateTime = #{0}#", lastSnapshotDateTime.ToString("o"));
                            DataRow[] mostRecentDbRows = dbStatistics.Select(select);
                            serverSummarySnapshots = ConstructServerSummarySnapshots(
                                    serverSummary.OverviewStatistics.Rows[serverSummary.OverviewStatistics.Rows.Count - 1],
                                    serverSummary.DiskDriveInfo.First, mostRecentDbRows);
                            
                            historicalSnapshotsPopulated = true;
                            //sqldm-28694 start

                            
                                ServerViewContainer serverView = (ServerViewContainer)ApplicationController.Default.ActiveView;

                                Controls.HistoryBrowserPane historyBrowserPane = serverView.historyBrowserControl.GetHistoryBrowserPane();

                                //Loading historical snapshot of a particular day
                                Infragistics.Win.UltraWinTree.UltraTreeNode historicalSnapshot = historyBrowserPane.LoadSnapshot(historicalSnapshotDateTime);
                                Infragistics.Win.UltraWinTree.UltraTree historicalSnapshotsTree = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTree();

                            KeyValuePair<DateTime, MonitoredSqlServerStatus>? snapshotInfo = null, snapshotInfoTemp;
                                Infragistics.Win.AppearanceBase appearance = null;
                                if (historicalSnapshot != null && historicalSnapshot.Nodes != null)
                                {
                                    historicalSnapshotsTree.Nodes.Add(historicalSnapshot);
                                    //Iterating over spanshot tree to get the keyvaluepairof required dateTime
                                    foreach (Infragistics.Win.UltraWinTree.UltraTreeNode node in historicalSnapshotsTree.Nodes[0].Nodes)
                                    {
                                        snapshotInfoTemp = (KeyValuePair<DateTime, MonitoredSqlServerStatus>)node.Tag;

                                        if (lastSnapshotDateTime.Equals((snapshotInfoTemp.Value.Key)))
                                        {
                                            snapshotInfo = (KeyValuePair<DateTime, MonitoredSqlServerStatus>)node.Tag;
                                            appearance = node.Override.NodeAppearance;
                                            break;
                                        }
                                    }
                                    //If lastsnapshot is empty , then select the first snapshot from tree.
                                    if (snapshotInfo == null)
                                    {

                                        if (historicalSnapshotsTree.Nodes[0].Nodes != null)
                                        {
                                            try
                                            {
                                                appearance = historicalSnapshot.Nodes[0].Nodes[0].Override.NodeAppearance;
                                            }
                                            catch (Exception ex)
                                            {
                                                Log.Warn(ex);
                                            }
                                            snapshotInfo = (KeyValuePair<DateTime, MonitoredSqlServerStatus>)historicalSnapshotsTree.Nodes[0].Nodes[0].Tag;
                                        }
                                    }
                                    historyBrowserPane.RecentlyViewed(snapshotInfo, appearance);
                                    serverView.HistoricalSnapshotDateTime = snapshotInfo.Value.Key;
                                }
                            //sqldm 28694 end
                            //6.2.4
                            PopulateAzureDiskDataTableWithHistory(historicalAzureDiskDataTable, serverSummary.OverviewStatistics);
                        }
                        }
                    
                }
                finally
                {
                    historicalSnapshotsDataTable.EndLoadData();
                    historicalDbSnapshotsDataTable.EndLoadData();
                    historicalFileSnapshotsDataTable.EndLoadData();
                    historicalCustomCountersDataTable.EndLoadData();
                    historicalDiskDataTable.EndLoadData();
                    historicalAzureDiskDataTable.EndLoadData();//6.2.4
                }
            }
        }

        
        private void PopulateDataTableWithHistory(DataTable savedHistoryDataTable, DataTable rawHistoryStatisticsDataTable)
        {
            using (Log.InfoCall("PopulateDataTableWithHistory"))
            {
                bool vm = false;
                if (savedHistoryDataTable != null && rawHistoryStatisticsDataTable != null)
                {
                    int insertionIndex = GetInsertionIndex(savedHistoryDataTable, "Date", rawHistoryStatisticsDataTable, "CollectionDateTime");
                    foreach (DataRow rawRow in rawHistoryStatisticsDataTable.Rows)
                    {
                        object value;
                        DataRow newRow = savedHistoryDataTable.NewRow();

                        double? timeDeltaInSeconds = null;
                        value = rawRow["TimeDeltaInSeconds"];
                        if (value != DBNull.Value)
                        {
                            timeDeltaInSeconds = (double)value;
                        }

                        newRow["Date"] = rawRow["CollectionDateTime"];
                        newRow["IsHistorical"] = true;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                        newRow["ResponseTime"] = rawRow["ResponseTimeInMilliseconds"];
                        newRow["UserSessions"] = rawRow["UserProcesses"];
                        newRow["ActiveSessions"] = rawRow["ActiveProcesses"];
                        newRow["SystemSessions"] = rawRow["SystemProcesses"];

                        if (newRow["UserSessions"] != DBNull.Value && newRow["ActiveSessions"] != DBNull.Value)
                        {
                            newRow["IdleSessions"] = (int)newRow["UserSessions"] -
                                                     (int)newRow["ActiveSessions"];
                        }

                        newRow["BlockedProcesses"] = rawRow["BlockedProcesses"];
                        newRow["LeadBlockers"] = rawRow["LeadBlockers"];
                        newRow["TotalCpuUsage"] = rawRow["ProcessorTimePercent"];
                        newRow["SqlServerCpuUsage"] = rawRow["CPUActivityPercentage"];
                        newRow["PagesPerSec"] = rawRow["PagesPerSecond"];
                        newRow["PercentDiskTime"] = rawRow["DiskTimePercent"];

                        long? osTotalMemory = null;
                        value = rawRow["OSTotalPhysicalMemoryInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            osTotalMemory = (long)value / 1024;
                            newRow["OsTotalPhysicalMemory"] = osTotalMemory;
                        }

                        value = rawRow["OSAvailableMemoryInKilobytes"];
                        if (value != DBNull.Value && osTotalMemory != null)
                        {
                            long osAvailableMemory = (long)value / 1024;

                            newRow["OsMemoryUsed"] = osTotalMemory - osAvailableMemory;
                        }

                        value = rawRow["SqlMemoryAllocatedInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["SqlServerMemoryAllocated"] = (long)value / 1024;
                        }

                        value = rawRow["SqlMemoryUsedInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["SqlServerMemoryUsed"] = (long)value / 1024;
                        }

                        value = rawRow["ManagedInstanceStorageLimit"];
                        if (value != DBNull.Value)
                        {
                            newRow["ManagedInstanceStorageLimit"] = (decimal)value;
                        }


                        value = rawRow["Transactions"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                        {
                            newRow["CallRatesTransactions"] = (long)value / timeDeltaInSeconds;
                        }

                        value = rawRow["Batches"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                        {
                            newRow["CallRatesBatches"] = (long)value / timeDeltaInSeconds;
                        }

                        value = rawRow["SqlCompilations"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                        {
                            newRow["CallRatesCompiles"] = (long)value / timeDeltaInSeconds;
                        }
                      
                        newRow["CPUCreditBalance"]= rawRow["CPUCreditBalance"];
                        newRow["CPUCreditUsage"]=rawRow["CPUCreditUsage"];
                        newRow["WriteThroughput"] =rawRow["WriteThroughput"];
                        newRow["ReadThroughput"] = rawRow["ReadThroughput"];
                        newRow["SwapUsage"] = rawRow["SwapUsage"];
                        newRow["ReadLatency"] = rawRow["ReadLatency"];
                        newRow["WriteLatency"] = rawRow["WriteLatency"];
                        newRow["DiskQueueDepth"] = rawRow["DiskQueueDepth"];

                        value = rawRow["SqlRecompilations"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                        {
                            newRow["CallRatesReCompiles"] = (long)value / timeDeltaInSeconds;
                        }

                        newRow["BufferCacheHitRatio"] = rawRow["BufferCacheHitRatioPercentage"];
                        newRow["ProcedureCacheHitRatio"] = rawRow["ProcedureCacheHitRatioPercentage"];

                        value = rawRow["CheckpointWrites"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            newRow["CheckpointWrites"] = (long)value / timeDeltaInSeconds;
                        else
                            newRow["CheckpointWrites"] = DBNull.Value;

                        value = rawRow["LazyWriterWrites"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            newRow["LazyWriterWrites"] = (long)value / timeDeltaInSeconds;
                        else
                            newRow["LazyWriterWrites"] = DBNull.Value;

                        value = rawRow["ReadAheadPages"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            newRow["ReadAheadReads"] = (long)value / timeDeltaInSeconds;
                        else
                            newRow["ReadAheadReads"] = DBNull.Value;

                        value = rawRow["PageReads"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            newRow["SynchronousReads"] = (long)value / timeDeltaInSeconds;
                        else
                            newRow["SynchronousReads"] = DBNull.Value;

                        value = rawRow["PageWrites"];
                        if (value != DBNull.Value && timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            newRow["SynchronousWrites"] = (long)value / timeDeltaInSeconds;
                        else
                            newRow["SynchronousWrites"] = DBNull.Value;

                        newRow["ProcessorQueueLength"] = rawRow["ProcessorQueueLength"];
                        newRow["PercentPrivilegedTime"] = rawRow["PrivilegedTimePercent"];
                        newRow["PercentUserTime"] = rawRow["UserTimePercent"];
                        newRow["DiskQueueLength"] = rawRow["DiskQueueLength"];
                        newRow["ReadWriteErrors"] = rawRow["PageErrors"];

                        value = rawRow["BufferCacheSizeInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["BufferCachePages"] = (long)value / 1024;
                        }

                        value = rawRow["FreePagesInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["BufferCacheFreePages"] = (long)value / 1024;
                        }

                        if (newRow["BufferCachePages"] != DBNull.Value && newRow["BufferCacheFreePages"] != DBNull.Value)
                        {
                            newRow["BufferCacheActivePages"] = (decimal)newRow["BufferCachePages"] -
                                                               (decimal)newRow["BufferCacheFreePages"];
                        }

                        value = rawRow["CommittedInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["CommittedPages"] = (long)value / 1024;
                        }

                        value = rawRow["ProcedureCacheSizeInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["ProcedureCachePages"] = value;
                            newRow["ProcedureCachePages"] = (decimal)newRow["ProcedureCachePages"] / 1024;
                        }

                        value = rawRow["FreeCachePagesInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["ProcedureCacheFreePages"] = value;
                            newRow["ProcedureCacheFreePages"] = (decimal)newRow["ProcedureCacheFreePages"] / 1024;
                        }

                        if (newRow["ProcedureCachePages"] != DBNull.Value &&
                            newRow["ProcedureCacheFreePages"] != DBNull.Value)
                        {
                            newRow["ProcedureCacheActivePages"] = (decimal)newRow["ProcedureCachePages"] -
                                                                  (decimal)newRow["ProcedureCacheFreePages"];
                        }

                        value = rawRow["OptimizerMemoryInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["OptimizerMemory"] = (long)value / 1024;
                        }

                        value = rawRow["LockMemoryInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["LockMemory"] = (long)value / 1024;
                        }

                        value = rawRow["ConnectionMemoryInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["ConnectionMemory"] = (long)value / 1024;
                        }

                        value = rawRow["GrantedWorkspaceMemoryInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["SortHashIndexMemory"] = (long)value / 1024;
                        }

                        value = rawRow["CommittedInKilobytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["UsedDataMemory"] = (long)value / 1024;
                        }

                        value = rawRow["PageLifeExpectancy"];
                        if (value != DBNull.Value)
                        {
                            newRow["PageLifeExpectancy"] = (long)value;
                        }

                        value = rawRow["PacketErrors"];
                        if (value != DBNull.Value)
                        {
                            newRow["PacketErrors"] = (long)value;
                            if (timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            {
                                newRow["PacketErrorsPerSecond"] = (long)value / timeDeltaInSeconds;
                            }
                        }

                        value = rawRow["PacketsReceived"];
                        if (value != DBNull.Value)
                        {
                            newRow["PacketsReceived"] = (long)value;
                            if (timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            {
                                newRow["PacketsReceivedPerSecond"] = (long)value / timeDeltaInSeconds;
                            }
                        }

                        value = rawRow["PacketsSent"];
                        if (value != DBNull.Value)
                        {
                            newRow["PacketsSent"] = (long)value;
                            if (timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0)
                            {
                                newRow["PacketsSentPerSecond"] = (long)value / timeDeltaInSeconds;
                            }
                        }




                        value = rawRow["I/O"];
                        if (value != DBNull.Value)
                        {
                            newRow["IOWaits"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["Lock"];
                        if (value != DBNull.Value)
                        {
                            newRow["LockWaits"] = Convert.ToDecimal(value);
                        }


                        value = rawRow["Memory"];
                        if (value != DBNull.Value)
                        {
                            newRow["MemoryWaits"] = Convert.ToDecimal(value);
                        }


                        value = rawRow["Transaction Log"];
                        if (value != DBNull.Value)
                        {
                            newRow["TransactionLogWaits"] = Convert.ToDecimal(value);
                        }


                        value = rawRow["Other"];
                        if (value != DBNull.Value)
                        {
                            newRow["OtherWaits"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["Signal"];
                        if (value != DBNull.Value)
                        {
                            newRow["SignalWaits"] = Convert.ToDecimal(value);
                        }

                        if (rawRow.Table.Columns.Contains("LockStatisticsObject"))
                        {
                            value = rawRow["LockStatisticsObject"];
                            if (value != DBNull.Value)
                            {
                                AddLockStatisticsSnapshot((LockStatistics)value, newRow);
                            }
                        }
                        else
                        {
                            value = rawRow["LockStatistics"];
                            if (value != DBNull.Value)
                            {
                                LockStatistics lockStatistics = Serialized<LockStatistics>.DeserializeCompressed<LockStatistics>((byte[])value);
                                if (lockStatistics != null)
                                    AddLockStatisticsSnapshot(lockStatistics, newRow);
                            }
                        }

                        // Virtualization Data
                        value = rawRow["vmUUID"];
                        if (value != DBNull.Value)
                        {
                            vm = true;

                            foreach (string metric in vmCounters.Keys)
                            {
                                if (!rawRow.Table.Columns.Contains(metric)) continue;
                                value = rawRow[metric];

                                if (value != DBNull.Value)
                                {
                                    if (metric.EndsWith("MemSwapped") || metric.EndsWith("MemActive") ||
                                        metric.EndsWith("MemConsumed") || metric.EndsWith("MemGranted") ||
                                        metric.EndsWith("MemBallooned"))
                                    {
                                        value = (new FileSize(Convert.ToDecimal(value))).Megabytes;
                                    }

                                    if (vmCounters[metric].Second.Name == "Int32")
                                        newRow[metric] = Convert.ToInt32(value);
                                    else if (vmCounters[metric].Second.Name == "Int64")
                                        newRow[metric] = Convert.ToInt64(value);
                                    else if (vmCounters[metric].Second.Name == "Decimal")
                                        newRow[metric] = Convert.ToDecimal(value);
                                    else if (vmCounters[metric].Second.Name == "DateTime")
                                        newRow[metric] = Convert.ToDateTime(value);
                                    else if (vmCounters[metric].Second.Name == "Int16")
                                        newRow[metric] = Convert.ToInt16(value);
                                    else
                                        newRow[metric] = DBNull.Value;
                                }
                                else
                                {
                                    DateTime missingCollection = (DateTime)newRow["Date"];
                                    Log.DebugFormat("[ServerSummaryHistoryData] Missing info in the repository for {0} at {1} ", metric,
                                                    missingCollection.ToString());
                                }
                            }
                        }

                        //Tempdb data
                        value = rawRow["TempdbUserObjectsInMegabytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["TempdbUserObjectsMegabytes"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["TempdbInternalObjectsInMegabytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["TempdbInternalObjectsMegabytes"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["TempdbVersionStoreInMegabytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["TempdbVersionStoreMegabytes"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["TempdbMixedExtentsInMegabytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["TempdbMixedExtentsMegabytes"] = Convert.ToDecimal(value);
                        }

                        value = rawRow["TempdbUnallocatedSpaceInMegabytes"];
                        if (value != DBNull.Value)
                        {
                            newRow["TempdbUnallocatedSpaceMegabytes"] = Convert.ToDecimal(value);
                        }
                        if (AllAzureMetrics.ContainsKey((DateTime)rawRow["CollectionDateTime"]))
                        {
                            newRow["AzureCloudMetrics"] = AllAzureMetrics[(DateTime)rawRow["CollectionDateTime"]];
                        }
                         

                        // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
                        // Insert new data in correct index.
                        if (insertionIndex >= 0)
                            savedHistoryDataTable.Rows.InsertAt(newRow, insertionIndex++);
                        else
                            savedHistoryDataTable.Rows.Add(newRow);
                    }
                    isVM = vm;
                    
                }
            }
        }
        private void PopulateDataTableWithHistoryAWS(DataTable savedHistoryDataTable, DataTable rawHistoryStatisticsDataTable)
        {
            using (Log.InfoCall("PopulateDataTableWithHistoryAWS"))
            {
                bool vm = false;
                if (savedHistoryDataTable != null && rawHistoryStatisticsDataTable != null)
                {
                    int insertionIndex = GetInsertionIndex(savedHistoryDataTable, "Date", rawHistoryStatisticsDataTable, "CollectionDateTime");
                    foreach (DataRow rawRow in rawHistoryStatisticsDataTable.Rows)
                    {
                        object value;
                        DataRow newRow = savedHistoryDataTable.NewRow();

                        double? timeDeltaInSeconds = null;
                        value = rawRow["TimeDeltaInSeconds"];
                        if (value != DBNull.Value)
                        {
                            timeDeltaInSeconds = (double)value;
                        }

                        newRow["Date"] = rawRow["CollectionDateTime"];
                        newRow["IsHistorical"] = true;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser


                        newRow["CPUCreditBalance"] = rawRow["CPUCreditBalance"];
                        newRow["CPUCreditUsage"] = rawRow["CPUCreditUsage"];
                        newRow["WriteThroughput"] = rawRow["WriteThroughput"];
                        newRow["ReadThroughput"] = rawRow["ReadThroughput"];
                        newRow["SwapUsage"] = rawRow["SwapUsage"];
                        newRow["ReadLatency"] = rawRow["ReadLatency"];
                        newRow["WriteLatency"] = rawRow["WriteLatency"];
                        newRow["DiskQueueDepth"] = rawRow["DiskQueueDepth"];


                    }
                }
            }
        }
        private void PopulateDbDataTableWithHistory(DataTable savedHistoryDataTable, DataTable rawHistoryStatisticsDataTable)
        {
            int insertionIndex = GetInsertionIndex(savedHistoryDataTable, "Date", rawHistoryStatisticsDataTable, "UTCCollectionDateTime");
            
            
            foreach (DataRow rawRow in rawHistoryStatisticsDataTable.Rows)
            { 
                Dictionary<string, object> temp = new Dictionary<string, object>();
                object value;
                DataRow newRow = savedHistoryDataTable.NewRow();

                decimal? timeDeltaInSeconds = null;
                value = rawRow["TimeDeltaInSeconds"];
                if (value != DBNull.Value)
                {
                    timeDeltaInSeconds = Convert.ToDecimal(value);
                }

                bool validTimeDelta = timeDeltaInSeconds.HasValue && timeDeltaInSeconds > 0;
                
                if (!AllAzureMetrics.ContainsKey((DateTime)rawRow["UTCCollectionDateTime"]))
                    AllAzureMetrics[(DateTime)rawRow["UTCCollectionDateTime"]] = new Dictionary<string, Dictionary<string, object>>();

                temp["AverageDataIOPercent"]= rawRow["AverageDataIO"]; 
                temp["AverageLogWritePercent"] = rawRow["AverageLogIO"];
                temp["MaxSessionPercent"] = rawRow["MaxSession"];
                temp["MaxWorkerPercent"] = rawRow["MaxWorker"];
                temp["DatabaseAverageMemoryUsagePercent"] = rawRow["DatabaseAverageMemoryUsage"];
                temp["InMemoryStorageUsagePercent"] = rawRow["InMemoryStorageUsage"];
                bool hasNotEmptyValues = temp.Any(pair => pair.Value == DBNull.Value);

                if (!hasNotEmptyValues)
                {
                    AllAzureMetrics[(DateTime)rawRow["UTCCollectionDateTime"]][(string)rawRow["DatabaseName"]] = temp;
                }

                newRow["Date"] = rawRow["UTCCollectionDateTime"];
                newRow["IsHistorical"] = true;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                newRow["DatabaseName"] = rawRow["DatabaseName"];
                newRow["DatabaseStatus"] = (Common.Snapshots.DatabaseStatus)rawRow["DatabaseStatus"];
                value = rawRow["Transactions"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["TransactionsPerSec"] = (long)value / timeDeltaInSeconds;
                }

                value = rawRow["LogFlushWaits"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["LogFlushWaitsPerSec"] = (long)value / timeDeltaInSeconds;
                }

                value = rawRow["LogFlushes"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["LogFlushesPerSec"] = (long)value / timeDeltaInSeconds;
                }

                value = rawRow["LogKilobytesFlushed"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["LogKilobytesFlushedPerSec"] = (long)value / timeDeltaInSeconds;
                }

                value = rawRow["LogCacheReads"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["LogCacheReadsPerSec"] = (long)value / timeDeltaInSeconds;
                }

                newRow["LogCacheHitRatio"] = rawRow["LogCacheHitRatio"];

                value = rawRow["NumberReads"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["NumberReadsPerSec"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }

                value = rawRow["NumberWrites"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["NumberWritesPerSec"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }

                value = rawRow["BytesRead"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["BytesReadPerSec"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }

                value = rawRow["BytesWritten"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["BytesWrittenPerSec"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }

                value = rawRow["IoStallMS"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["IoStallMSPerSec"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }
                // cpu usage chart data
               
                Decimal? avgCpuPercent = null;
                Decimal? avgDataIoPercent = null;
                Decimal? avgLogWritePercent = null;
                
                value = rawRow["AvgDataIoPercent"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    avgDataIoPercent = Convert.ToDecimal(value) / timeDeltaInSeconds;
                    newRow["AvgDataIoPercent"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }
                value = rawRow["AvgLogWritePercent"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    avgLogWritePercent = Convert.ToDecimal(value) / timeDeltaInSeconds;
                    newRow["AvgLogWritePercent"] = Convert.ToDecimal(value) / timeDeltaInSeconds;
                }
                value = rawRow["DtuLimit"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    newRow["DtuLimit"] = Convert.ToDecimal(value) / timeDeltaInSeconds; ;
                }
                value = rawRow["AvgCpuPercent"];
                if (value != DBNull.Value && validTimeDelta)
                {
                    avgCpuPercent = Convert.ToDecimal(value) / timeDeltaInSeconds;
                    if (value != DBNull.Value)
                    {
                        if (avgDataIoPercent != null && avgLogWritePercent != null)
                        {
                            decimal? max = avgCpuPercent > avgDataIoPercent ? avgCpuPercent : avgDataIoPercent;
                            max = max > avgLogWritePercent ? max : avgLogWritePercent;
                            newRow["AvgCpuPercent"] = max;
                        }
                    }
                    else
                        newRow["AvgCpuPercent"] = Convert.ToDecimal(value) / timeDeltaInSeconds; 
                }
                //// SqlDM 10.2 (Anshul Aggarwal) - New History Browser
                // Insert new data in correct index.


                // 6.2.3
                value = rawRow["AzureCloudAllocatedMemory"];
                if(value != DBNull.Value)
                {
                    newRow["AzureCloudAllocatedMemory"] = Convert.ToDecimal(value);
                }

                value = rawRow["AzureCloudUsedMemory"];
                if (value != DBNull.Value)
                {
                    newRow["AzureCloudUsedMemory"] = Convert.ToDecimal(value);
                }

                value = rawRow["AzureCloudStorageLimit"];
                if (value != DBNull.Value)
                {
                    newRow["AzureCloudStorageLimit"] = Convert.ToDecimal(value);
                }

                //Elastic Pool Support
                value = rawRow["ElasticPool"];
                if (value != DBNull.Value)
                {
                    newRow["ElasticPool"] = Convert.ToString(value);
                }

                if (insertionIndex >= 0)
                    savedHistoryDataTable.Rows.InsertAt(newRow, insertionIndex++);
                else
                    savedHistoryDataTable.Rows.Add(newRow);
            }
        }

        private void PopulateFilesDataTableWithHistory(DataTable filesDataTable, DataTable rawHistoryStatisticsDataTable)
        {
            int insertionIndex = GetInsertionIndex(filesDataTable, "Date", rawHistoryStatisticsDataTable, "Date");
            foreach (DataRow rawRow in rawHistoryStatisticsDataTable.Rows)
            {
                var copy = rawRow.ItemArray.Clone() as object[];
                if (copy != null)
                {
                    var newRow = filesDataTable.NewRow();
                    newRow.ItemArray = copy;
                    newRow["IsHistorical"] = true;

                    // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
                    // Insert new data in correct index.
                    if (insertionIndex >= 0)
                        filesDataTable.Rows.InsertAt(newRow, insertionIndex++);
                    else
                        filesDataTable.Rows.Add(newRow);
                }
            }
        }

        private void PopulateCustomCountersDataTableWithHistory(DataTable savedHistoryDataTable, DataTable rawHistoryStatisticsDataTable)
        {
            int insertionIndex = GetInsertionIndex(savedHistoryDataTable, "Date", rawHistoryStatisticsDataTable, "CollectionDateTime");
            foreach (DataRow rawRow in rawHistoryStatisticsDataTable.Rows)
            {
                DataRow newRow = savedHistoryDataTable.NewRow();
                newRow["Date"] = rawRow["CollectionDateTime"];
                newRow["IsHistorical"] = true;  // SqlDM 10.2(Anshul Aggarwal) : New History Browser
                newRow["MetricID"] = rawRow["MetricID"];
                if (rawRow["RawValue"] != DBNull.Value)
                {
                    newRow["RawValue"] = rawRow["RawValue"];
                }
                if (rawRow["DeltaValue"] != DBNull.Value)
                {
                    newRow["DeltaValue"] = rawRow["DeltaValue"];
                }
                if (rawRow["DisplayValue"] != DBNull.Value)
                {
                    newRow["DisplayValue"] = rawRow["DisplayValue"];
                }

                // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
                // Insert new data in correct index.
                if (insertionIndex >= 0)
                    savedHistoryDataTable.Rows.InsertAt(newRow, insertionIndex++);
                else
                    savedHistoryDataTable.Rows.Add(newRow);
            }
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Populate baselines data.
        /// </summary>
        private void PopulateBaselinesDataTableWithHistory(DataTable snapshotDataTable , DataTable diskTable, DataTable baselinesDataTable)
        {
            using (Log.InfoCall("PopulateBaselinesDataTableWithHistory"))
            {
                if (snapshotDataTable != null && diskTable != null && baselinesDataTable != null)
                {
                    foreach (DataRow rawRow in baselinesDataTable.Rows)
                    {
                        BaselineNameAsPerMetricID enumBaselineName;
                        string columnName;
                        int metricId = Convert.ToInt32(rawRow["MetricID"]);
                        object value;

                        if (metricId == -1003 || metricId == -71 || metricId == -70)//convert kb to mb
                        {
                            enumBaselineName = (BaselineNameAsPerMetricID)Convert.ToInt32(rawRow["MetricID"]);
                            columnName = enumBaselineName.ToString();
                            value = Convert.ToInt64(rawRow["Value"]) >> 10;//right shift by 10 places .. i.e divide by 1024
                        }
                        else
                        {
                            enumBaselineName = (BaselineNameAsPerMetricID)Convert.ToInt32(rawRow["MetricID"]);
                            columnName = enumBaselineName.ToString();
                            value = rawRow["Value"];
                        }
                        var table = isBaselinesUsedForDiskCharts(columnName) ? diskTable : snapshotDataTable;
                        var newBaselineRow = table.NewRow();
                        //SQLDM - 19237 - Code change to get the proper date format to avoid code exceptions when the date format is changed in the region settings of control panel.
                        newBaselineRow["Date"] = (DateTime)rawRow["Date"];
                        newBaselineRow[columnName] = value;
                        newBaselineRow[IS_BASELINE_COLUMN_NAME] = true;
                        table.Rows.Add(newBaselineRow);
                    }
                    //END: SQLdm 10.0 (Tarun Sapra) - Gets all the baselines data and insert that into the data table as per the metricId in data table
                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Populate new disk table for disk charts
        /// </summary>
        private void PopulateDiskDataTableWithHistory(DataTable savedHistoryDataTable, Pair<List<string>, DataTable> diskDriveInfo)
        {
            using (Log.InfoCall("PopulateDiskDataTableWithHistory"))
            {
                if (savedHistoryDataTable != null)
                {
                    if (diskDriveInfo.First != null)
                    {
                        foreach (string diskDrive in diskDriveInfo.First)
                        {
                            string diskUsagePerDiskColumnName = DiskUsagePerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskUsagePerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskUsagePerDiskColumnName, typeof(double));
                            }

                            string diskQueueLengthPerDiskColumnName = DiskQueueLengthPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskQueueLengthPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskQueueLengthPerDiskColumnName, typeof(ulong));
                            }

                            string diskReadsPerSecPerDiskColumnName = DiskReadsPerSecPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskReadsPerSecPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskReadsPerSecPerDiskColumnName, typeof(double));
                            }

                            string diskTransfersPerSecPerDiskColumnName = DiskTransfersPerSecPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskTransfersPerSecPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskTransfersPerSecPerDiskColumnName, typeof(double));
                            }

                            string diskWritesPerSecPerDiskColumnName = DiskWritesPerSecPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskWritesPerSecPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskWritesPerSecPerDiskColumnName, typeof(double));
                            }

                            string diskTimePerReadPerDiskColumnName = DiskTimePerReadPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskTimePerReadPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskTimePerReadPerDiskColumnName, typeof(double));
                            }

                            string diskTimePerTransferPerDiskColumnName = DiskTimePerTransferPerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskTimePerTransferPerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskTimePerTransferPerDiskColumnName, typeof(double));
                            }

                            string diskTimePerWritePerDiskColumnName = DiskTimePerWritePerDiskColumnPrefix + diskDrive;

                            if (!savedHistoryDataTable.Columns.Contains(diskTimePerWritePerDiskColumnName))
                            {
                                savedHistoryDataTable.Columns.Add(diskTimePerWritePerDiskColumnName, typeof(double));
                            }
                        }
                    }
                    
                    if (diskDriveInfo.Second != null && diskDriveInfo.Second.Rows.Count > 0)
                    {
                        int insertionIndex = GetInsertionIndex(savedHistoryDataTable, "Date", diskDriveInfo.Second, "CollectionDateTime");
                        DataRow newRow = null;
                        DateTime? prevDate = null;
                        foreach (DataRow diskDriveRow in diskDriveInfo.Second.Rows)
                        {
                            var currentDate = diskDriveRow["CollectionDateTime"] as DateTime?;
                            if (currentDate == null)
                                return;

                            bool isNewRow = false;
                            if(newRow == null || currentDate != prevDate)
                            {
                                isNewRow = true;
                                newRow = savedHistoryDataTable.NewRow();
                                newRow["Date"] = currentDate;
                                newRow["IsHistorical"] = true;
                                prevDate = currentDate;
                            }
                            
                            newRow[DiskUsagePerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["DiskBusyPercent"];
                            newRow[DiskQueueLengthPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["AverageDiskQueueLength"];
                            newRow[DiskReadsPerSecPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["DiskReadsPerSecond"];
                            newRow[DiskTransfersPerSecPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["DiskTransfersPerSecond"];
                            newRow[DiskWritesPerSecPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["DiskWritesPerSecond"];
                            newRow[DiskTimePerReadPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["AverageDiskMillisecondsPerRead"];
                            newRow[DiskTimePerTransferPerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["AverageDiskMillisecondsPerTransfer"];
                            newRow[DiskTimePerWritePerDiskColumnPrefix + diskDriveRow["DriveName"]] =
                                diskDriveRow["AverageDiskMillisecondsPerWrite"];

                            // SqlDM 10.2 (Anshul Aggarwal) - New History Browser
                            // Insert new data in correct index.
                            if (!isNewRow && diskDriveInfo.Second.Rows[diskDriveInfo.Second.Rows.Count - 1] != newRow)
                                continue;

                            if (insertionIndex >= 0)
                                savedHistoryDataTable.Rows.InsertAt(newRow, insertionIndex++);
                            else
                                savedHistoryDataTable.Rows.Add(newRow);
                        }
                    }
                }
            }
        }

        //6.2.4
        private void PopulateAzureDiskDataTableWithHistory(DataTable historicalAzureDiskDataTable, DataTable overviewStatistics)
        {
            using (Log.InfoCall("PopulateDataTableWithHistory"))
            {
                if (historicalAzureDiskDataTable != null && overviewStatistics != null)
                {
                    int insertionIndex = GetInsertionIndex(historicalAzureDiskDataTable, "Date", overviewStatistics, "CollectionDateTime");
                    foreach (DataRow rawRow in overviewStatistics.Rows)
                    {
                        object value;
                        DataRow newRow = historicalAzureDiskDataTable.NewRow();

                        double? timeDeltaInSeconds = null;
                        value = rawRow["TimeDeltaInSeconds"];
                        if (value != DBNull.Value)
                        {
                            timeDeltaInSeconds = (double)value;
                        }

                        newRow["Date"] = rawRow["CollectionDateTime"];
                        newRow["IsHistorical"] = true; 
                        newRow["DataIOUsage"] = rawRow["DataIOUsage"];
                        newRow["LogIOUsage"] = rawRow["LogIOUsage"];
                        newRow["DataIORate"] = rawRow["DataIORate"];
                        newRow["LogIORate"] = rawRow["LogIORate"];

                       
                        if (insertionIndex >= 0)
                            historicalAzureDiskDataTable.Rows.InsertAt(newRow, insertionIndex++);
                        else
                            historicalAzureDiskDataTable.Rows.Add(newRow);
                    }
                }
            }
        }



        private static ServerSummarySnapshots ConstructServerSummarySnapshots(DataRow serverStatisticsDataRow, List<string> diskDrives, DataRow[] dbStatisticsDataRows)
        {
            ServerSummarySnapshots serverSummarySnapshots = null;

            if (serverStatisticsDataRow != null)
            {
                string instanceName = Convert.ToString(serverStatisticsDataRow["InstanceName"]);
                ServerOverview serverOverview = new ServerOverview(instanceName, serverStatisticsDataRow, diskDrives, dbStatisticsDataRows);
                //ResourceSnapshot resourceSnapshot = new ResourceSnapshot(instanceName, diskDrives);
                //serverSummarySnapshots = new ServerSummarySnapshots(serverOverview, resourceSnapshot, null);
                //serverSummarySnapshots = new ServerSummarySnapshots(serverOverview, resourceSnapshot);
                serverSummarySnapshots = new ServerSummarySnapshots(serverOverview);
            }

            return serverSummarySnapshots;
        }
    }
}
