using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Idera.SQLdm.Common.Snapshots;
using System.Windows.Data;
using System.Globalization;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    public class DashboardLayoutModel : INotifyPropertyChanged
    {
        private DashboardControl _control;
        private ServerSummaryDataModel _data;

        public event PropertyChangedEventHandler PropertyChanged;

		public DashboardLayoutModel()
		{
            Components = new ObservableCollection<DashboardComponent>();
            LoadRegisteredComponents();
            LoadTestData();
            LoadDefaultComponents();
		}
		
        public DashboardLayoutModel(DashboardControl control, ServerSummaryHistoryData history)
        {
            _control = control;
            _data = new ServerSummaryDataModel(history);
            
            Components = new ObservableCollection<DashboardComponent>();
            LoadRegisteredComponents();
            LoadDefaultComponents();
        }

        void LoadTestData()
        {
            _data = new ServerSummaryDataModel();
        }
 
        private void LoadRegisteredComponents()
        {
            RegisteredComponents = ChartComponents.GetRegisteredComponents();
        }

        public void LoadDefaultComponents()
        {
            Components.Add(new DashboardComponent() { Title = "CPU", ComponentKey = "CPUTrends", LogicalIndex = 0, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Network", ComponentKey = "Network", LogicalIndex = 1, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Memory", ComponentKey = "Memory", LogicalIndex = 2, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Caching", ComponentKey = "MemoryCache", LogicalIndex = 3, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Sessions", ComponentKey = "Sessions", LogicalIndex = 4, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Waits", ComponentKey = "Waits", LogicalIndex = 5, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Unknown", ComponentKey = "Waits", LogicalIndex = 6, Maximized = false, Model = DataModel });
            Components.Add(new DashboardComponent() { Title = "Unknown", ComponentKey = "Waits", LogicalIndex = 7, Maximized = false, Model = DataModel });            
        }

        internal void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Invalidate()
        {
                OnPropertyChanged("IsInEditMode");
                OnPropertyChanged("NumberColumns");
                OnPropertyChanged("NumberRows");
        }

        public ServerSummaryDataModel DataModel
        {
            get { return _data; }
        }

        private bool _isInEditMode = false;
        public bool IsInEditMode
        {
            get { return _isInEditMode;  }
            set
            {
                if (_isInEditMode == value) return;
                _isInEditMode = value;
                OnPropertyChanged("IsInEditMode");
            }
        }

        private int _numberColumns = 2;
        public int NumberColumns
        {
            get { return _numberColumns; }
            set
            {
                if (_numberColumns == value) return;
                _numberColumns = value;
                OnPropertyChanged("NumberColumns");
            }
        }

        private int _numberRows = 4;
        public int NumberRows
        {
            get { return _numberRows; }
            set
            {
                if (_numberRows == value) return;
                _numberRows = value;
                OnPropertyChanged("NumberRows");
            }
        }

        public ObservableCollection<DashboardComponent> Components { get; set; }
        public List<DashboardComponentMetaData> RegisteredComponents { get; set; }

        public void InsertColumn(int after)
        {
            NumberColumns++;
            _control.addColumnAfter(after);
        }

        public void InsertRow(int after)
        {
            NumberRows++;
            _control.addRowAfter(after);
        }

        public void DeleteColumn(int after)
        {
            _control.deleteColumn(after);
            NumberColumns--;
        }

        public void DeletetRow(int after)
        {
            _control.deleteRow(after);
            NumberRows--;
        }
    }

    public class DashboardComponent : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string  _title;
        private string  _componentKey;
        private int     _logicalIndex;
        private bool    _maximized;
        private ServerSummaryDataModel _model;

        public ServerSummaryDataModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        public string Title
        {
            get { return _title; }
            set 
            {
                if (String.Equals(value, _title)) return;
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public string ComponentKey
        {
            get { return _componentKey;  }
            set
            {
                if (_componentKey == value) return;
                _componentKey = value;
                OnPropertyChanged("ComponentKey");
            }
        }

        public int LogicalIndex
        {
            get { return _logicalIndex; }
            set 
            { 
                if (_logicalIndex == value) return;
                _logicalIndex = value;
                OnPropertyChanged("LogicalIndex");
            }
        }
        
        public bool Maximized
        {
            get { return _maximized; }
            set
            {
                if (_maximized == value) return;
                _maximized = value;
                OnPropertyChanged("Maximized");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DashboardComponentMetaData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name;
        private string _description;
        private string _templateKey;
        private string _imageKey;

        public string Key { get; set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (String.Equals(value, _name)) return;
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (String.Equals(value, _description)) return;
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        public string TemplateKey
        {
            get { return _templateKey; }
            set
            {
                if (String.Equals(value, _templateKey)) return;
                _templateKey = value;
                OnPropertyChanged("TemplateKey");
            }
        }

        public string ImageKey
        {
            get { return _imageKey; }
            set
            {
                if (String.Equals(value, _imageKey)) return;
                _imageKey = value;
                OnPropertyChanged("Image");
            }
        }

        public ImageSource Image
        {
            get
            {
                var image = ChartComponents.GetResource(ImageKey) as Image;
                if (image != null) return image.Source;
                return null;
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class DashboardDataModel
    {
        private ObservableCollection<ServerOverview> _chartData;

        public ObservableCollection<ServerOverview> ChartData
        {
            get { return _chartData; }
        }

        public DashboardDataModel()
        {
            _chartData = new ObservableCollection<ServerOverview>();

            Random rand = new Random(DateTime.Now.Millisecond);
            DateTime now = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var overview = new ServerOverview("Dumas");
          //      overview.TimeStamp = now.ToUniversalTime() + TimeSpan.FromMinutes(i * 6);
             //   overview.TimeDelta = new TimeSpan.FromMinutes(6);


  //              overview.ResponseTime = rand.Next(5000);
 //               overview.SystemProcesses.UserProcessesConsumingCpu = rand.Next(10);
 //               overview.SystemProcesses.CurrentUserProcesses = rand.Next(100);
 //               overview.SystemProcesses.BlockedProcesses = rand.Next(5);
 //               overview.SystemProcesses.CurrentSystemProcesses = rand.Next(10);
 //               overview.SystemProcesses.LeadBlockers = rand.Next(3);

                // CPU
   //             overview.OSMetricsStatistics.PercentDiskIdleTime = rand.NextDouble() * 100.0;
   //             overview.Statistics.CpuPercentage = (int)(rand.NextDouble() * 100.0);

//                overview.OSMetricsStatistics.PagesPersec = rand.NextDouble * 100.0;
//                overview.OSMetricsStatistics.PercentDiskTime = rand.NextDouble() * 100.0;


//                overview.Statistics.CheckpointPages = rand.Next(100);
//                overview.Statistics.LazyWrites = rand.Next(100);
//                overview.Statistics.ReadaheadPages = rand.Next(100);
//                overview.Statistics.PageReads = rand.Next(100);
//                overview.Statistics.PageWrites = rand.Next(100);


                _chartData.Add(overview);
            }
        }
    }

	public class DashboardDemoDataModel
    {
        private ObservableCollection<DashboardDemoDataOverview> _chartData;

        public ObservableCollection<DashboardDemoDataOverview> ChartData
        {
            get { return _chartData; }
        }

        public DashboardDemoDataModel()
        {
            _chartData = new ObservableCollection<DashboardDemoDataOverview>();

            Random rand = new Random(DateTime.Now.Millisecond);
            DateTime now = DateTime.Now;
            for (int i = 0; i < 10; i++)
            {
                var overview = new DashboardDemoDataOverview();
                overview.ServerName = "Dumas";
                overview.TimeStamp = now.ToUniversalTime() + TimeSpan.FromMinutes(i * 6);

  //              overview.ResponseTime = rand.Next(5000);
 //               overview.SystemProcesses.UserProcessesConsumingCpu = rand.Next(10);
 //               overview.SystemProcesses.CurrentUserProcesses = rand.Next(100);
 //               overview.SystemProcesses.BlockedProcesses = rand.Next(5);
 //               overview.SystemProcesses.CurrentSystemProcesses = rand.Next(10);
 //               overview.SystemProcesses.LeadBlockers = rand.Next(3);

                // CPU
   //             overview.OSMetricsStatistics.PercentDiskIdleTime = rand.NextDouble() * 100.0;
   //             overview.Statistics.CpuPercentage = (int)(rand.NextDouble() * 100.0);

//                overview.OSMetricsStatistics.PagesPersec = rand.NextDouble * 100.0;
//                overview.OSMetricsStatistics.PercentDiskTime = rand.NextDouble() * 100.0;

                overview.Statistics.CheckpointPages = rand.Next(100);
                overview.Statistics.LazyWrites = rand.Next(100);
                overview.Statistics.ReadaheadPages = rand.Next(100);
                overview.Statistics.PageReads = rand.Next(100);
                overview.Statistics.PageWrites = rand.Next(100);

                foreach (var drive in new string[] { "C:", "D:" })
                {
                    DashboardDemoDataDiskDriveStatistics d = new DashboardDemoDataDiskDriveStatistics();
                    d.DriveLetter = drive;
                    d.AvgDiskSecPerRead = TimeSpan.FromMilliseconds(rand.NextDouble() * 20);
                    d.AvgDiskSecPerWrite = TimeSpan.FromMilliseconds(rand.NextDouble() * 20);
                    d.AvgDiskSecPerTransfer = d.AvgDiskSecPerRead + d.AvgDiskSecPerWrite;
                    d.DiskReadsPerSec = rand.Next(500);
                    d.DiskWritesPerSec = rand.Next(200);
                    d.DiskTransfersPerSec = d.DiskReadsPerSec + d.DiskWritesPerSec;
                    overview.DiskDrives.Add(drive, d);
                }

                _chartData.Add(overview);
            }
        }
    }

	public class DashboardDemoDataOverview
	{
        public string ServerName { get; set; }
        public DateTime TimeStamp { get; set; }
        public DateTime TimeStampLocal { get { return TimeStamp.ToLocalTime(); } }
        public DashboardDemoDataStatistics Statistics { get; set; }
        public Dictionary<string,DashboardDemoDataDiskDriveStatistics> DiskDrives { get; set; }

        public DashboardDemoDataOverview()
        {
            Statistics = new DashboardDemoDataStatistics();
            DiskDrives = new Dictionary<string,DashboardDemoDataDiskDriveStatistics>();
        }
	}

    public class OverviewDiskDriveStatisticsIterator : IEnumerable<OverviewDiskDriveStatistics>
    {
        public ObservableCollection<DashboardDemoDataOverview> ChartData;
        public string DiskDriveLetter = "C:";

        public IEnumerator<OverviewDiskDriveStatistics> GetEnumerator()
        {
           IEnumerator<DashboardDemoDataOverview> enumerator = ChartData.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return new OverviewDiskDriveStatistics() { Overview = enumerator.Current, Disk = enumerator.Current.DiskDrives[DiskDriveLetter] };
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class OverviewDiskDriveStatisticsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<DashboardDemoDataOverview> input = value as ObservableCollection<DashboardDemoDataOverview>;
            if (input == null) return null;
            string key = parameter as string;
            return new OverviewDiskDriveStatisticsIterator() { ChartData = input, DiskDriveLetter = key };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           throw new NotImplementedException();
        }
    }



    public class OverviewDiskDriveStatistics
    {
        public DashboardDemoDataOverview Overview { get; set; }
        public DashboardDemoDataDiskDriveStatistics Disk { get; set; }
    }

    public class DashboardDemoDataStatistics
    {
        public long? CheckpointPages { get; set; }
        public long? LazyWrites { get; set; }
        public long? ReadaheadPages { get; set; }
        public long? PageReads { get; set; }
        public long? PageWrites { get; set; }
    } 


     
    public class DashboardDemoDataDiskDriveStatistics
    {
        public string DriveLetter { get; set; }
        public TimeSpan? AvgDiskSecPerRead { get; set; }
        public TimeSpan? AvgDiskSecPerTransfer { get; set; }
        public TimeSpan? AvgDiskSecPerWrite { get; set; }
        public double? DiskReadsPerSec { get; set; }
        public double? DiskTransfersPerSec { get; set; }
        public double? DiskWritesPerSec { get; set; }
    }

    public class SummarizedWaitData
    {
        public DateTime SnapshotDateTime { get; set; }
        public double ResourceWaits      { get; set; }
        public double SignalWaits        { get; set; }
        public double TotalWaits         { get { return ResourceWaits + SignalWaits; } }
    }

    public class CategorySummarizedWaitData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Category { get; set; }
        public SummarizedWaitData Data { get; set; }
        public ObservableCollection<SummarizedWaitData> History { get; private set; }
        public int Test { get; set; }

        public CategorySummarizedWaitData()
        {
            History = new ObservableCollection<SummarizedWaitData>();
            Test = DateTime.Now.Millisecond;
        }

        public void Update(SummarizedWaitData newData)
        {
            Data = newData;
            History.Add(newData);
            var cutoff = newData.SnapshotDateTime - TimeSpan.FromMinutes(30);               
            while (History[0].SnapshotDateTime < cutoff)
                History.RemoveAt(0);
        }
    }

    public class SummarizedWaitsDataModel
    {
        private Dictionary<string,CategorySummarizedWaitData> _map;
        private ObservableCollection<CategorySummarizedWaitData> _rows;
             
        public SummarizedWaitsDataModel()
        {
            _map = new Dictionary<string, CategorySummarizedWaitData>();
            _rows = new ObservableCollection<CategorySummarizedWaitData>();
                    
            var r = new Random(DateTime.Now.Millisecond);
            for (int i = 6; i >= 0; i--)
            {
                foreach (var category in new string[] { "Backup", "IO", "Lock", "Memory", "Page Latch", "Non Page Latch", "Other", "Log" })
                {
                    var data = new SummarizedWaitData()
                                    {
                                        ResourceWaits = r.Next(100, 500),
                                        SignalWaits = r.Next(50, 250),
                                        SnapshotDateTime = DateTime.Now - TimeSpan.FromMinutes(5*i)
                                    };

                    CategorySummarizedWaitData catdata;
                    if (!_map.TryGetValue(category, out catdata))
                    {
                        catdata = new CategorySummarizedWaitData();
                        catdata.Category = category;
                        catdata.Update(data);
                        _map.Add(category, catdata);
                        _rows.Add(catdata);
                    }
                    else
                    {
                        catdata.Update(data);
                    }
                }
            }
        }

        public ObservableCollection<CategorySummarizedWaitData> Current
        {
            get { return _rows; }
        }
    }
}
