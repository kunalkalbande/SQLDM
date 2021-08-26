using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using Idera.SQLdm.DesktopClient.Objects;
using System.Windows.Threading;
using System.Threading;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    public class ServerSummaryDataModel : INotifyPropertyChanged
    {
        private bool _isHistorical = false;
        private DataTableAdapter<OverviewDataAdapter> _realtimeOverviewAdapter;
        private ObservableCollection<KeyValuePair<string, string>> _serverProperties;
        private ServerSummaryHistoryData _historyData;
        private MonitoredSqlServerStatus _serverStatus;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool DummyData { get; set; }
        public bool IsHistorical
        {
            get { return _isHistorical; } 
            set 
            {
                if (_isHistorical == value) return;
                _isHistorical = value;
                ConfigureAdapters();
                OnPropertyChanged("IsHistorical");
            } 
        }

        public ServerSummaryDataModel()
        {
            DummyData = true;
            _historyData = new ServerSummaryHistoryData();
            _serverProperties = new ObservableCollection<KeyValuePair<string, string>>();
            DataTable realTimeData = _historyData.RealTimeSnapshotsDataTable;
            GenerateDummyData(realTimeData);
        }

        internal ServerSummaryDataModel(ServerSummaryHistoryData historyData)
        {
            _historyData = historyData;
            _historyData.PropertyChanged += HistoryData_PropertyChanged;
            IsHistorical = _historyData.HistoricalSnapshotDateTime.HasValue;
            _serverProperties = new ObservableCollection<KeyValuePair<string, string>>();
            LoadServerProperties();

            DummyData = false;
        }

        void HistoryData_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "LastServerOverviewSnapshot":
                    // reflect property changes as a result of setting a new last server overview in the history object.
                    OnPropertyChanged("LastSnapshotCurrentProcesses");
                    OnPropertyChanged("LastSnapshotCurrentUserProcesses");
                    OnPropertyChanged("LastSnapshotCurrentSystemProcesses");
                    OnPropertyChanged("LastSnapshotProcessorQueueLength");
                    OnPropertyChanged("LastSnapshotResponseTime");
                    break;
                case "LastServicesSnapshot":
                    break;
                case "HistoricalSnapshotDateTime":
                    IsHistorical = _historyData.HistoricalSnapshotDateTime.HasValue;
                    break;
            }
        }

        private void GenerateDummyData(DataTable realTimeData)
        {
            Random r = new Random(DateTime.Now.Millisecond);
            DateTime now = DateTime.Now;
            for (int i = 30; i >= 0; i-=6)
            {
                DataRow newRow = realTimeData.NewRow();
                newRow["Date"] = now - TimeSpan.FromMinutes(i);
                OverviewDataAdapter.GenerateTestRow(newRow, r);
                realTimeData.Rows.Add(newRow);
            }

            _serverStatus = MonitoredSqlServerStatus.CreateDesignTimeStatus();

            foreach (var key in new string[] { "Version", "Edition", "Running", "Clustered", "Processors", "Host", "Host OS", "Host Memory", "Databases", "Data Size", "Log Size" })
            {
                _serverProperties.Add(new KeyValuePair<string, string>(key, key + " value"));
            }
        }

        private void LoadServerProperties()
        {
            foreach (var key in new string[] { "Version","Edition","Running","Clustered","Processors","Host","Host OS","Host Memory","Databases","Data Size","Log Size" })
            {
                _serverProperties.Add(new KeyValuePair<string, string>(key, key + " value"));
            }
        }

        private void ConfigureAdapters()
        {
            _realtimeOverviewAdapter.Table = _isHistorical ? _historyData.HistoricalSnapshotsDataTable : _historyData.RealTimeSnapshotsDataTable;
        }

        public ServerSummaryHistoryData HistoryData { get { return _historyData; } }

        #region LastSnapshotProperties

        public int? LastSnapshotCurrentProcesses        
        { 
            get 
            { 
                if (HistoryData == null || HistoryData.LastServerOverviewSnapshot == null || HistoryData.LastServerOverviewSnapshot.SystemProcesses == null) return null;
                return HistoryData.LastServerOverviewSnapshot.SystemProcesses.CurrentProcesses; 
            } 
        }
        public int? LastSnapshotCurrentUserProcesses    
        { 
            get 
            { 
                if (HistoryData == null || HistoryData.LastServerOverviewSnapshot == null || HistoryData.LastServerOverviewSnapshot.SystemProcesses == null) return null;
                return HistoryData.LastServerOverviewSnapshot.SystemProcesses.CurrentUserProcesses; 
            } 
        }
        public int? LastSnapshotCurrentSystemProcesses
        {
            get
            {
                if (HistoryData == null || HistoryData.LastServerOverviewSnapshot == null || HistoryData.LastServerOverviewSnapshot.SystemProcesses == null) return null;
                return HistoryData.LastServerOverviewSnapshot.SystemProcesses.CurrentSystemProcesses;
            }
        }
        public double? LastSnapshotProcessorQueueLength
        {
            get
            {
                if (HistoryData == null || HistoryData.LastServerOverviewSnapshot == null || HistoryData.LastServerOverviewSnapshot.OSMetricsStatistics == null) return null;
                return HistoryData.LastServerOverviewSnapshot.OSMetricsStatistics.ProcessorQueueLength;
            }
        }
        public long LastSnapshotResponseTime
        {
            get
            {
                if (HistoryData == null || HistoryData.LastServerOverviewSnapshot == null) return 0;
                return HistoryData.LastServerOverviewSnapshot.ResponseTime;
            }
        }

        #endregion

        public DataTableAdapter<OverviewDataAdapter> RealtimeSnapshotData
        {
            get 
            {
                if (_realtimeOverviewAdapter == null) 
                    _realtimeOverviewAdapter = new DataTableAdapter<OverviewDataAdapter>(_historyData.RealTimeSnapshotsDataTable);
                return _realtimeOverviewAdapter;
            }
        }

        public ObservableCollection<KeyValuePair<string, string>> ServerProperties
        {
            get { return _serverProperties; }
        }

        public MonitoredSqlServerStatus ServerStatus
        {
            get { return _serverStatus; }
            set
            {
                if (_serverStatus == value) return;
                _serverStatus = value;
                OnPropertyChanged("ServerStatus");
            }
        }

        private string _serverStatusKey = "ServerInformation";
        public String ServerStatusKey
        { 
            get { return _serverStatusKey; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    value = "ServerInformation";
                if (_serverStatusKey.Equals(value)) return;
                _serverStatusKey = value;
                OnPropertyChanged("ServerStatus");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class DataTableAdapter<T> : INotifyCollectionChanged, INotifyPropertyChanged, IEnumerable<T> where T : DataRowAdapter, new()
    {
        private DataView _view;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public DataTableAdapter(DataTable table)
        {
            _view = table.DefaultView;
            _view.ListChanged += View_ListChanged;
        }

        public void FireCollectionChanged()
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public DataTable Table
        {
            get
            {
                if (_view == null) return null;
                return _view.Table;
            }
            set
            {
                if (value == null) return;
                if (_view != null) _view.ListChanged -= View_ListChanged;

                _view = value.DefaultView;
                _view.ListChanged += View_ListChanged;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            System.Collections.IEnumerator enumerator = _view.GetEnumerator();
            while (enumerator.MoveNext())
            {
                T item = new T();
                item.SetDataRow((DataRowView)enumerator.Current);
                yield return item;
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void View_ListChanged(object sender, ListChangedEventArgs e)
        {
            // do not reflect this event on a background thread
            if (Thread.CurrentThread.IsBackground) return;

            NotifyCollectionChangedEventArgs args = null;

            DataRowView row = null;
            if (e.PropertyDescriptor != null)
            {
                try { row = e.PropertyDescriptor.GetValue(sender) as DataRowView; }
                catch (Exception x) { }
            }

            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                case ListChangedType.ItemMoved:
                case ListChangedType.ItemChanged:
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                case ListChangedType.ItemDeleted:
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
                case ListChangedType.Reset:
                    args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                    break;
            }

            if (args != null)
                OnCollectionChanged(args);
        }

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (CollectionChanged != null)
                CollectionChanged(this, args);
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, args);
        }
    }

    public class DataRowAdapter
    {
        protected DataRowView _row;
        public DataRowAdapter()
        {
           
        }
        public void SetDataRow(DataRowView row)
        {
            _row = row;
        }

        protected T? GetColumn<T>(string name) where T : struct
        {
            if (_row.Row.IsNull(name)) return null;
            object value = _row[name];
            if (value is T)
                return (T)value;
            return (T)Convert.ChangeType(value, typeof(T));
        }

    }

    public class OverviewDataAdapter : DataRowAdapter
    {
        public DateTime? Date { get { return GetColumn<DateTime>("Date"); } }

        public long? CheckPointWrites { get { return GetColumn<long>("CheckpointWrites"); } }
        public long? LazyWriterWrites { get { return GetColumn<long>("LazyWriterWrites"); } }
        public long? ReadAheadPages { get { return GetColumn<long>("ReadAheadReads"); } }
        public long? PageReads { get { return GetColumn<long>("SynchronousReads"); } }
        public long? PageWrites { get { return GetColumn<long>("SynchronousWrites"); } }
        public long? ResponseTime { get { return GetColumn<long>("ResponseTime"); } }
        public int?  ActiveSessions { get { return GetColumn<int>("ActiveSessions"); } }
        public int?  UserSessions { get { return GetColumn<int>("UserSessions"); } }
        public int?  SystemSessions { get { return GetColumn<int>("SystemSessions"); } }
        public int?  IdleSessions { get { return GetColumn<int>("IdleSessions"); } }
        public int?  BlockedProcesses { get { return GetColumn<int>("BlockedProcesses"); } }
        public int?  LeadBlockers { get { return GetColumn<int>("LeadBlockers"); } }
        public long? TotalDeadlocks { get { return GetColumn<long>("Total Deadlocks"); } }
        public double? TotalCpuUsage { get { return GetColumn<double>("TotalCpuUsage"); } }
        //6.2.4 Disk Section
        public int? DataIOUsage { get { return GetColumn<int>("DataIOUsage"); } }
        public int? DataIORate { get { return GetColumn<int>("DataIORate"); } }
        public int? LogIOUsage { get { return GetColumn<int>("LogIOUsage"); } }
        public int? LogIORate { get { return GetColumn<int>("LogIORate"); } }
        //end
        public double? SqlServerCpuUsage { get { return GetColumn<double>("SqlServerCpuUsage"); } }
        public double? PagesPerSec { get { return GetColumn<double>("PagesPerSec"); } }
        public double? PercentDiskTime { get { return GetColumn<double>("PercentDiskTime"); } }
        public decimal? OsTotalPhysicalMemory { get { return GetColumn<decimal>("OsTotalPhysicalMemory"); } }
        public decimal? OsMemoryUsed { get { return GetColumn<decimal>("OsMemoryUsed"); } }
        public decimal? SqlServerMemoryAllocated { get { return GetColumn<decimal>("SqlServerMemoryAllocated"); } }
        public decimal? SqlServerMemoryUsed { get { return GetColumn<decimal>("SqlServerMemoryUsed"); } }
        public long? CallRatesBatches { get { return GetColumn<long>("CallRatesBatches"); } }
        public long? CallRatesCompiles { get { return GetColumn<long>("CallRatesCompiles"); } }
        public long? CallRatesReCompiles { get { return GetColumn<long>("CallRatesReCompiles"); } }
        public float? BufferCacheHitRatio { get { return GetColumn<float>("BufferCacheHitRatio"); } }
        public float? ProcedureCacheHitRatio { get { return GetColumn<float>("ProcedureCacheHitRatio"); } }
        public ulong? ProcessorQueueLength { get { return GetColumn<ulong>("ProcessorQueueLength"); } }
        public double? PercentPrivilegedTime { get { return GetColumn<double>("PercentPrivilegedTime"); } }
        public double? PercentUserTime { get { return GetColumn<double>("PercentUserTime"); } }
        public ulong? DiskQueueLength { get { return GetColumn<ulong>("DiskQueueLength"); } }
        public long? ReadWriteErrors { get { return GetColumn<long>("ReadWriteErrors"); } }
        public decimal? BufferCachePages { get { return GetColumn<decimal>("BufferCachePages"); } }
        public decimal? BufferCacheFreePages { get { return GetColumn<decimal>("BufferCacheFreePages"); } }
        public decimal? BufferCacheActivePages { get { return GetColumn<decimal>("BufferCacheActivePages"); } }
        public decimal? CommittedPages { get { return GetColumn<decimal>("CommittedPages"); } }
        public decimal? ProcedureCachePages { get { return GetColumn<decimal>("ProcedureCachePages"); } }
        public decimal? ProcedureCacheFreePages { get { return GetColumn<decimal>("ProcedureCacheFreePages"); } }
        public decimal? ProcedureCacheActivePages { get { return GetColumn<decimal>("ProcedureCacheActivePages"); } }
        public decimal? OptimizerMemory { get { return GetColumn<decimal>("OptimizerMemory"); } }
        public decimal? LockMemory { get { return GetColumn<decimal>("LockMemory"); } }
        public decimal? ConnectionMemory { get { return GetColumn<decimal>("ConnectionMemory"); } }
        public decimal? SortHashIndexMemory { get { return GetColumn<decimal>("SortHashIndexMemory"); } }
        public decimal? UsedDataMemory { get { return GetColumn<decimal>("UsedDataMemory"); } }
        public long? PageLifeExpectancy { get { return GetColumn<long>("PageLifeExpectancy"); } }

        public long? PacketsSent { get { return GetColumn<long>("PacketsSent"); } }
        public long? PacketsReceived { get { return GetColumn<long>("PacketsReceived"); } }
        public long? PacketErrors { get { return GetColumn<long>("PacketErrors"); } }


        public static void GenerateTestRow(DataRow row, Random r)
        {
            row["CheckpointWrites"] = r.Next(100);
            row["LazyWriterWrites"] = r.Next(100);
            row["ReadAheadReads"] = r.Next(100);
            row["SynchronousReads"] = r.Next(100);
            row["SynchronousWrites"] = r.Next(100);

            row["CallRatesBatches"] = r.Next(100);
            row["CallRatesCompiles"] = r.Next(100);
            row["CallRatesReCompiles"] = r.Next(100);

            row["Total Deadlocks"] = r.Next(10);
            row["LeadBlockers"] = r.Next(10);
            row["BlockedProcesses"] = r.Next(20);

            row["PacketsSent"] = r.Next(500);
            row["PacketsReceived"] = r.Next(100);
            row["PacketErrors"] = r.Next(2);

            row["TotalCpuUsage"] = r.NextDouble() * 100;
            row["SqlServerCpuUsage"] = Math.Min(r.NextDouble() * 100, (double)row["TotalCpuUsage"]);

            row["PagesPerSec"] = r.NextDouble() * 250;

            int totused = r.Next(1000);
            if (totused < 250) totused = 250;
            int sqlalloc = r.Next(totused);
            if (sqlalloc < 100) sqlalloc = 100;
            int sqlused = r.Next(sqlalloc);
            if (sqlused < 50) sqlused = 50;
            row["OsMemoryUsed"] = totused;
            row["SqlServerMemoryAllocated"] = sqlalloc;
            row["SqlServerMemoryUsed"] = sqlused;

            row["BufferCachePages"] = r.Next(2048);
            row["BufferCacheHitRatio"] = r.NextDouble() * 100;
            row["ProcedureCachePages"] = r.Next(1024);
            row["ProcedureCacheHitRatio"] = r.NextDouble() * 100;
            row["PageLifeExpectancy"] = r.Next(1000);
        }

    }
}