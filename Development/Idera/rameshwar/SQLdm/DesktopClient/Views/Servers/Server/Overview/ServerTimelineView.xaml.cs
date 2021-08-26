using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.Xml;
using Idera.SQLdm.Common.Objects;
using Infragistics.UltraChart.Resources.Appearance;
using Infragistics.Controls.Timelines;
using Idera.SQLdm.Common;
using System.Data;
using System.IO;
using Idera.SQLdm.Common.Events;
using Infragistics.Controls;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Infragistics.Controls.Menus;
using Infragistics.Windows;
using System.Diagnostics;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    /// <summary>
    /// Interaction logic for ServerTimelineView.xaml
    /// </summary>
    public partial class ServerTimelineView 
    {
        private DataTable data;
        private Dictionary<string, SeriesStyle> categorySeriesMap;
        private Dictionary<string, SuspendableObservableCollection<DateTimeEntry2>> categoryDataMap;
        private ActivityTimelineFilter  filter;
        private bool isInit;
        private DateTime lastLoadStartDate = DateTime.MinValue;
        private DateTime lastLoadEndDate   = DateTime.MinValue;
        private DateTime timeOfLastLoad    = DateTime.MinValue;
        private TimeSpan minLoadInterval   = new TimeSpan(0, 1, 0); // can't load any faster than 30 seconds
        private bool     updating          = false;
        AxisLabelConverter labelConverter = null;
        private bool isFirstRefresh = true;
        
        public ServerTimelineView()
        {
            InitializeComponent();            
            filter = new ActivityTimelineFilter();
            data   = new DataTable();
        }

        public ServerTimelineView(int instanceId) : base(instanceId)
        {
            filter = new ActivityTimelineFilter();
            data   = new DataTable();
                                                                                              
            isInit = true;
            InitializeComponent();
            InitSeriesCategoryMap();
            InitDataSource();                     
            InitTimeline();
            isInit = false;

            DataContext = this;            

            xamTimeline1.Loaded += new RoutedEventHandler(xamTimeline1_Loaded);
        }

        private void xamTimeline1_Loaded(object sender, RoutedEventArgs e)
        {
            if (xamTimeline1.Zoombar == null)
                return;

            xamTimeline1.Zoombar.ZoomChanged += new EventHandler<Infragistics.Controls.ZoomChangedEventArgs>(Zoombar_ZoomChanged);
            //xamTimeline1.MouseDoubleClick += new MouseButtonEventHandler(xamTimeline1_MouseDoubleClick);
        }        

        private void Zoombar_ZoomChanged(object sender, Infragistics.Controls.ZoomChangedEventArgs e)
        {
            if (labelConverter != null)
            {
                labelConverter.ZoomLevel = (sender as XamZoombar).Range.Scale;
                labelConverter.Range     = filter.Range;
            }
        }

        public override void RefreshView()
        {
            if (!isFirstRefresh)
                return;

            isFirstRefresh = false;
            object data = DoRefreshWork();
            UpdateData(data);
        }

        public void RefreshView(bool isBackgroundRefresh)
        {
            if (isBackgroundRefresh)
                return;

            object data = DoRefreshWork();
            UpdateData(data);        
        }

        public override object DoRefreshWork()
        {            
            MonitoredSqlServer server;
            
            if (ApplicationModel.Default.AllRegisteredInstances.TryGetValue(_instanceId, out server))
            {                
                // how long ago since we last loaded data
                TimeSpan timeSinceLastLoad = DateTime.Now - timeOfLastLoad;

                // can't refresh more often than the minimum interval (have to wait at least minLoadInterval long)
                if (timeSinceLastLoad < minLoadInterval || updating)
                    return null;

                // increment the dates to include "now"
                filter.UpdateDates();

                if (filter.BeginDate == lastLoadStartDate && filter.EndDate == lastLoadEndDate)
                    return null; // nothing to do if we already loaded                 

                data.Clear();

                RepositoryHelper.GetServerTimelineEvents(data, server.InstanceName, filter.BeginDate, filter.EndDate);

                if (data != null)
                {
                    lastLoadStartDate = filter.BeginDate;
                    lastLoadEndDate   = filter.EndDate;
                    timeOfLastLoad    = DateTime.Now;
                }

                return data;
            }

            return null;
        }

        public override void UpdateData(object obj)
        {
            if (data == null || obj == null)
            {
                ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));
                return;
            }

            updating = true;            

            if (((DateTimeAxis)xamTimeline1.Axis).Minimum != filter.BeginDate || ((DateTimeAxis)xamTimeline1.Axis).Maximum != filter.EndDate)
            {
                ((DateTimeAxis)xamTimeline1.Axis).Minimum = filter.BeginDate;
                ((DateTimeAxis)xamTimeline1.Axis).Maximum = filter.EndDate.AddMinutes(2);
            }

            SortedDictionary<DateTime, List<DateTimeEntry2>> count = new SortedDictionary<DateTime, List<DateTimeEntry2>>();
            Dictionary<string, Dictionary<DateTime, int>> categoryCounts = new Dictionary<string, Dictionary<DateTime, int>>();

            int    metric     = 0;
            string category   = string.Empty;
            int    stateEvent = 0;

            for (int i = 0; i < data.Rows.Count; i++)
            {
                DateTime date = ((DateTime)data.Rows[i]["UTCOccurrenceDateTime"]).ToLocalTime();

                if (!count.ContainsKey(date))
                    count.Add(date, new List<DateTimeEntry2>());
                
                DateTimeEntry2 entry = new DateTimeEntry2();

                metric     = (int)data.Rows[i]["Metric"]; 
                category   = (string)data.Rows[i]["Category"];
                stateEvent = (int)data.Rows[i]["StateEvent"];

                entry.Time          = date;
                entry.Title         = data.Rows[i]["Heading"];
                entry.AlertID       = (long)data.Rows[i]["AlertID"];
                entry.Tag           = new AlertCategory(metric, category, string.Empty);
                entry.TitleStyle    = GetCategoryEventStyle(category);
                entry.SeverityImage = GetSeverityImage(stateEvent);
                entry.MetricId      = metric;                      

                count[date].Add(entry);
                UpdateCounts(category, date, categoryCounts);
            }

            xamTimeline1.BeginInit();
            ClearDataSources();

            foreach (KeyValuePair<DateTime, List<DateTimeEntry2>> pair in count)
            {
                DateTime date = pair.Key;
                List<DateTimeEntry2> entries = pair.Value;

                for (int i = 0; i < entries.Count; i++)
                {
                    category = ((AlertCategory)entries[i].Tag).Category;
                    entries[i].Tag = entries[i].AlertID;
                    entries[i].ToolTip = string.Format("{0} alerts at {1}", GetCounts(category, date, categoryCounts), date);
                    AddToSeries(entries[i], category);
                }
            }                   

            UpdateDataSources();

            xamTimeline1.EndInit();     
            xamTimeline1.UpdateLayout();

            ApplicationController.Default.OnRefreshActiveViewCompleted(new RefreshActiveViewCompletedEventArgs(DateTime.Now));

            updating = false;
        }

        private void UpdateCounts(string category, DateTime date, Dictionary<string, Dictionary<DateTime, int>> categoryCounts)
        {
            if (!categoryCounts.ContainsKey("above")) categoryCounts.Add("above", new Dictionary<DateTime, int>());
            if (!categoryCounts.ContainsKey("below")) categoryCounts.Add("below", new Dictionary<DateTime, int>());
            if (!categoryCounts.ContainsKey("axis"))  categoryCounts.Add("axis", new Dictionary<DateTime, int>());

            string key = string.Empty;

            if (category == "Databases" || category == "Logs" || category == "Operational" || category == "Sessions")
                key = "below";
            else if (category == "Queries" || category == "Services")
                key = "axis";
            else
                key = "above";

            if (!categoryCounts[key].ContainsKey(date))
                categoryCounts[key].Add(date, 1);
            else
                categoryCounts[key][date] += 1;
        }

        private int GetCounts(string category, DateTime date, Dictionary<string, Dictionary<DateTime, int>> categoryCounts)
        {
            string key = string.Empty;

            if (category == "Databases" || category == "Logs" || category == "Operational" || category == "Sessions")
                key = "below";
            else if (category == "Queries" || category == "Services")
                key = "axis";
            else
                key = "above";

            // for now, just add them all up, rather than by category (leaving code above in case we change our minds)

            int count = 0;

            if (categoryCounts["above"].ContainsKey(date)) count += categoryCounts["above"][date];
            if (categoryCounts["axis"].ContainsKey(date))  count += categoryCounts["axis"][date];
            if (categoryCounts["below"].ContainsKey(date)) count += categoryCounts["below"][date];

            return count;
            //return categoryCounts[key][date];
        }

        private Style GetCategoryEventStyle(string category)
        {            
            Style style = null;

            style = TryFindResource("series_" + category + "_style") as Style;

            if (style == null)
                style = TryFindResource("series_Custom_style") as Style;

            return style;
        }

        private string GetSeverityImage(int stateEvent)
        {
            Transition transition = (Transition)stateEvent;

            switch (transition)
            {
                case Transition.OK_Warning:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToWarning.png";

                case Transition.OK_Info:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToInfo.png";

                case Transition.OK_Critical:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToCritical.png";

                case Transition.Info_OK:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToOK.png";

                case Transition.Info_Info:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RemainedInfo.png";

                case Transition.Info_Warning:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToWarning.png";

                case Transition.Info_Critical:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToCritical.png";

                case Transition.Warning_OK:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToOK.png";

                case Transition.Warning_Warning:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RemainedWarning.png";

                case Transition.Warning_Info:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToInfo.png";

                case Transition.Warning_Critical:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RaisedToCritical.png";

                case Transition.Critical_OK:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToOK.png";

                case Transition.Critical_Warning:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToWarning.png";

                case Transition.Critical_Info:
                    return "/SQLdmDesktopClient;component/Resources/16x16/LoweredToInfo.png";

                case Transition.Critical_Critical:
                    return "/SQLdmDesktopClient;component/Resources/16x16/RemainedCritical.png";              
            }
            
            return "/SQLdmDesktopClient;component/Resources/16x16/OK.png";
        }

        private void AddToSeries(DateTimeEntry2 entry, string category)
        {            
            SeriesStyle style = GetCategorySeriesStyle(category);

            if(categoryDataMap.ContainsKey(category))
                categoryDataMap[category].Add(entry);
            else
                categoryDataMap["Custom"].Add(entry);
        }

        private void UpdateDataSources()
        {
            foreach (string category in categorySeriesMap.Keys)
            {
                int seriesIdx = categorySeriesMap[category].SeriesIdx;

                xamTimeline1.Series[seriesIdx].DataMapping = "Time=Time;Duration=Duration;Title=Title;Details=Details;TitleStyle=TitleStyle;ToolTip=ToolTip;Tag=Tag;";
                xamTimeline1.Series[seriesIdx].DataSource = null;
                xamTimeline1.Series[seriesIdx].DataSource = categoryDataMap[category];
            }
        }

        private void ClearDataSources()
        {
            foreach (string category in categoryDataMap.Keys)
            {
                categoryDataMap[category].Clear();
                categoryDataMap[category].Suspended = true;
            }

            
        }

        private SeriesStyle GetCategorySeriesStyle(string category)
        {            
            if (categorySeriesMap.ContainsKey(category))
                return categorySeriesMap[category];

            return categorySeriesMap["Custom"];
        }

        private void UpdateFilter()
        {
            if (isInit)
                return;

            // get the filter values from the controls
        }

        private void InitDataSource()
        {
            data.Columns.Add("AlertID",      typeof(long)).AllowDBNull = false;
            data.Columns.Add("UTCOccurrenceDateTime", typeof(DateTime)).AllowDBNull = false;
            data.Columns.Add("DatabaseName", typeof(string)).AllowDBNull = true;
            data.Columns.Add("TableName",    typeof(string)).AllowDBNull = true;
            data.Columns.Add("Active",       typeof(bool)).AllowDBNull = true;
            data.Columns.Add("Category",     typeof(string)).AllowDBNull = true;
            data.Columns.Add("Metric",       typeof(int)).AllowDBNull = true;
            data.Columns.Add("Severity",     typeof(int)).AllowDBNull = true;
            data.Columns.Add("Rank",         typeof(int)).AllowDBNull = true;
            data.Columns.Add("StateEvent",   typeof(int)).AllowDBNull = true;
            data.Columns.Add("Value",        typeof(float)).AllowDBNull = true;
            data.Columns.Add("Heading",      typeof(string)).AllowDBNull = true;
        }

        private void InitTimeline()
        {                        
            DateTimeAxis axis = new DateTimeAxis();
            ///Setting initial unit type to hours , unit to 1.0 and correcting scroll scale
            //axis.Minimum   = DateTime.Now.AddDays(-1);
            axis.Minimum = DateTime.Now.Subtract(DateTime.Now.TimeOfDay);
            axis.Maximum   = DateTime.Now;
            axis.UnitType  = DateTimeUnitType.Hours;
            axis.AutoRange = false;
            axis.Unit = 1.0;
            axis.ScrollScale = 2 / DateTime.Now.TimeOfDay.TotalHours;
            axis.ScrollPosition = 1.0;
            axis.SelectedTime   = DateTime.Today;

            axis.ShowLabels         = false;
            axis.ShowMajorTickMarks = true;
            axis.ShowMinorTickMarks = false;

            xamTimeline1.PreviewAxis.ShowLabels = true;
            
            xamTimeline1.Axis = axis;

            Style customPointStyle = (Style)FindResource("series_Custom_pointstyle");

            foreach(string category in categorySeriesMap.Keys)
            {
                DateTimeSeries series = new DateTimeSeries();

                series.Title    = category;
                series.Position = categorySeriesMap[category].Position;
                series.Fill     = categorySeriesMap[category].Brush;
                
                Style pointStyle = TryFindResource("series_" + category + "_pointstyle") as Style;

                if (pointStyle != null)
                    series.EventPointStyle = pointStyle;
                else
                    series.EventPointStyle = customPointStyle;

                xamTimeline1.Series.Add(series);
            }
        }

        private void InitSeriesCategoryMap()
        {
            categorySeriesMap = new Dictionary<string, SeriesStyle>();

            categorySeriesMap.Add("MORE",           new SeriesStyle(0, Brushes.LightGray,     Position.BottomOrRight)); // FFD3D3D3
            categorySeriesMap.Add("Databases",      new SeriesStyle(1, Brushes.BlanchedAlmond, Position.BottomOrRight));    // FFFFEBCD - EBDDC6
            categorySeriesMap.Add("Logs",           new SeriesStyle(2, Brushes.BlueViolet,    Position.BottomOrRight));     // FF8A2BE2 - AD6BEB
            categorySeriesMap.Add("Operational",    new SeriesStyle(3, Brushes.Coral,         Position.BottomOrRight));     // FFFF7F50 - ffa483
            categorySeriesMap.Add("Queries",        new SeriesStyle(4, Brushes.Thistle,       Position.TopOrLeft));     // FFD8BFD8 - E0CCE0
            categorySeriesMap.Add("Resources",      new SeriesStyle(5, Brushes.ForestGreen,   Position.TopOrLeft)); // FF228B22 - 2cb42c
            categorySeriesMap.Add("Services",       new SeriesStyle(6, Brushes.PaleVioletRed, Position.TopOrLeft)); // FFDB7093 - e599b2
            categorySeriesMap.Add("Sessions",       new SeriesStyle(7, Brushes.LightGreen,    Position.BottomOrRight)); // FF90EE90 - bcf5bc
            categorySeriesMap.Add("Virtualization", new SeriesStyle(8, Brushes.Goldenrod,     Position.TopOrLeft)); // FFDAA520 - e4b849
            categorySeriesMap.Add("Custom",         new SeriesStyle(9, Brushes.SteelBlue,     Position.TopOrLeft));     // FF4682B4 - 7EA8CA

            categoryDataMap = new Dictionary<string, SuspendableObservableCollection<DateTimeEntry2>>();

            categoryDataMap.Add("MORE",           new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Databases",      new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Logs",           new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Operational",    new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Queries",        new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Resources",      new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Services",       new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Sessions",       new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Virtualization", new SuspendableObservableCollection<DateTimeEntry2>());
            categoryDataMap.Add("Custom",         new SuspendableObservableCollection<DateTimeEntry2>());
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ActivityTimelineview);
        }

        private bool IsBitSet(int flag, int bit)
        {
            return (flag & bit) == bit;
        }

        public void SetRange(TimelineFilterRange range, DateTime date)
        {
            filter.SetRange(range, date);
            labelConverter.Range = range;

            ResetLastLoad();
            RefreshView(false);

            double scale = 1.0;
            /*
            * Default Unit & Unit Type - SQLdm10.0(Ankit Nagpal)
            */
            switch (range)
            {
                /*
                 * Setting Unit & Unit Type for different options -  SQLdm10.0(Ankit Nagpal)
                 */
                case TimelineFilterRange.Today:
                    scale = 2 / DateTime.Now.TimeOfDay.TotalHours;
                    ((DateTimeAxis)xamTimeline1.Axis).Unit = 1;
                    ((DateTimeAxis)xamTimeline1.Axis).UnitType = DateTimeUnitType.Hours;
                    break;

                case TimelineFilterRange.Yesterday:
                    scale = 0.0833;
                    ((DateTimeAxis)xamTimeline1.Axis).Unit = 1;
                    ((DateTimeAxis)xamTimeline1.Axis).UnitType = DateTimeUnitType.Hours;
                    break;

                case TimelineFilterRange.Last7Days:
                    scale = 0.1428;
                    ((DateTimeAxis)xamTimeline1.Axis).Unit = 1;
                    ((DateTimeAxis)xamTimeline1.Axis).UnitType = DateTimeUnitType.Days;
                    break;

                case TimelineFilterRange.Last14Days:
                    scale = 0.0714;
                    ((DateTimeAxis)xamTimeline1.Axis).Unit = 1;
                    ((DateTimeAxis)xamTimeline1.Axis).UnitType = DateTimeUnitType.Days;
                    break;

                case TimelineFilterRange.Custom:
                    scale = 0.0833;
                    ((DateTimeAxis)xamTimeline1.Axis).Unit = 1;
                    ((DateTimeAxis)xamTimeline1.Axis).UnitType = DateTimeUnitType.Hours;
                    break;
            }
            ((DateTimeAxis)xamTimeline1.Axis).ScrollScale    = scale;
            ((DateTimeAxis)xamTimeline1.Axis).ScrollPosition = 1;
        }

        private void ResetLastLoad()
        {
            timeOfLastLoad = DateTime.MinValue;
        }

        private object prevclickobject = null;
        private int clickcount = 0;
        private DateTime prevClicktime = DateTime.MaxValue;

        private void XamEventTitle_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsEventDoubleClick(sender))
                return;

            Grid grid = sender as Grid;
            DependencyObject dp = VisualTreeHelper.GetParent(grid);

            while (dp != null && dp.GetType() != typeof(EventTitle))
                dp = VisualTreeHelper.GetParent(dp);

            if (dp.GetType() == typeof(EventTitle))
            {
                EventTitle    title = dp as EventTitle;
                DateTimeEntry entry = title.EventEntry as DateTimeEntry;

                long alertid = (long)entry.Tag;

                DrillThroughToAlerts(entry.Time, alertid);
            }            
        }

        private void XamEventPoint_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsEventDoubleClick(sender))
                return;

            Grid grid = sender as Grid;
            DependencyObject dp = VisualTreeHelper.GetParent(grid);

            while (dp != null && dp.GetType() != typeof(EventPoint))
                dp = VisualTreeHelper.GetParent(dp);

            if (dp.GetType() == typeof(EventPoint))
            {
                EventPoint point = dp as EventPoint;
                DateTimeEntry entry = point.EventEntry as DateTimeEntry;

                long alertid = (long)entry.Tag;

                DrillThroughToAlerts(entry.Time, alertid);
            }
        }

        private void XamMenuItem_Click(object sender, EventArgs e)
        {
            XamContextMenu menu = Utilities.GetAncestorFromType(sender as DependencyObject, typeof(XamContextMenu), true) as XamContextMenu;
            DateTimeEntry entry = ((menu.PlacementTargetResolved as Border).TemplatedParent as EventTitle).EventEntry as DateTimeEntry;

            long alertid = (long)entry.Tag;

            DrillThroughToAlerts(entry.Time, alertid);
        }

        private bool IsEventDoubleClick(object sender)
        {            
            TimeSpan delta = DateTime.Now - prevClicktime;
            bool isDoubleClick = false;
            
            if (prevclickobject == null)
            {
                clickcount = 1;

                isDoubleClick = false;
            }
            else if (prevclickobject == sender)
            {
                if (clickcount == 1)
                {
                    clickcount = 0;

                    if (delta.TotalSeconds < 1)
                        isDoubleClick = true;
                    else
                        isDoubleClick = false;
                }
                else
                {
                    clickcount = 1;
                    isDoubleClick = false;
                }
            }
            else if (prevclickobject != sender)
            {
                clickcount = 1;
                isDoubleClick =  false;
            }

            prevClicktime = DateTime.Now;
            prevclickobject = sender;

            return isDoubleClick;
        }

        private void DrillThroughToAlerts(DateTime date, long alertid)
        {
            int offset = 10;

            AlertFilter alertFilter = new AlertFilter();
            alertFilter.BeginDate   = date.AddSeconds(-offset);
            alertFilter.EndDate     = date.AddSeconds(offset);
            alertFilter.Instance    = ApplicationModel.Default.ActiveInstances[this.InstanceId].InstanceName;
            alertFilter.ForRealz    = true;
            alertFilter.AlertID     = alertid;

            ApplicationController.Default.ShowAlertsView(StandardAlertsViews.Custom, alertFilter);
        }

        #region Private classes

        private class ActivityTimelineFilter
        {            
            public DateTime BeginDate;
            public DateTime EndDate;
            public TimelineFilterRange Range = TimelineFilterRange.Today;

            public ActivityTimelineFilter()
            {
                EndDate    = DateTime.Now;
                BeginDate  = EndDate.AddDays(-7);
            }

            public void UpdateDates()
            {
                DateTime now = DateTime.Now;

                switch (Range)
                {
                    case TimelineFilterRange.Today:                        
                        BeginDate = now.Subtract(now.TimeOfDay); // start at midnight to include whole day
                        EndDate   = now;
                        break;

                    case TimelineFilterRange.Last7Days:
                        EndDate   = now;
                        BeginDate = now.AddDays(-7);
                        BeginDate = BeginDate.Subtract(BeginDate.TimeOfDay); // start at midnight to include whole day
                        break;

                    case TimelineFilterRange.Last14Days:
                        EndDate   = now;
                        BeginDate = now.AddDays(-14);
                        BeginDate = BeginDate.Subtract(BeginDate.TimeOfDay); // start at midnight to include whole day
                        break;

                    case TimelineFilterRange.Yesterday:
                    case TimelineFilterRange.Custom:
                        return;
                }                
            }            

            public void SetRange(TimelineFilterRange range, DateTime date)
            {
                Range = range;

                if (range == TimelineFilterRange.Custom)
                {
                    BeginDate = date.Subtract(date.TimeOfDay);
                    EndDate   = BeginDate.AddHours(24);
                }
                else if (range == TimelineFilterRange.Yesterday)
                {
                    DateTime today = DateTime.Now.Subtract(DateTime.Now.TimeOfDay);
                    BeginDate = today.AddDays(-1);
                    EndDate   = BeginDate.AddHours(24);
                }
                else
                {
                    UpdateDates();
                }
            }
        }

        private class AlertCategory
        {
            public int    Metric;
            public string Name;
            public string Category;

            public AlertCategory(int metric, string category, string name)
            {
                Metric   = metric;
                Category = category;
                Name     = name;                
            }
        }

        private class SeriesStyle
        {
            public Brush Brush;
            public Position Position;
            public int SeriesIdx;

            public SeriesStyle(int i, Brush b, Position p)
            {
                SeriesIdx = i;
                Brush = b;
                Position = p;
            }
        }

        #endregion                

        private void WpfServerBaseView_Loaded(object sender, RoutedEventArgs e)
        {
            labelConverter = this.Resources["converter"] as AxisLabelConverter;
        }

        private class DateTimeEntry2 : DateTimeEntry
        {
            private string imageString;
            public string SeverityImage
            {
                get { return imageString; }
                set { imageString = value; }
            }

            private int metricId;
            public int MetricId
            {
                get { return metricId; }
                set { metricId = value; }
            }

            private long alertId;
            public long AlertID
            {
                get { return alertId; }
                set { alertId = value; }
            }
        }
    }

    public enum TimelineFilterRange
    {
        Today,
        Yesterday,
        Last7Days,
        Last14Days,
        Custom
    }

    public class SuspendableObservableCollection<T> : ObservableCollection<T>
    {
        private bool suspended;

        public bool Suspended
        {
            get
            {
                return this.suspended;
            }
            set
            {
                this.suspended = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            if (!Suspended)
            {
                base.OnCollectionChanged(args);
            }
        }
    }

    public class AxisLabelConverter : System.Windows.Data.IValueConverter
    {
        public double ZoomLevel { get; set; }
        public TimelineFilterRange Range { get; set; }

        public AxisLabelConverter() { ZoomLevel = 1; }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DateTime time = (DateTime)value;            
            string formattedDate = time.Month.ToString() + "/" + time.Day.ToString();

            switch (Range)
            {
                case TimelineFilterRange.Today:
                    formattedDate = time.ToString("h tt");
                    break;

                case TimelineFilterRange.Yesterday:
                    formattedDate = time.ToString("h tt");
                    break;

                case TimelineFilterRange.Last7Days:
                    formattedDate = time.ToString("M");
                    break;

                case TimelineFilterRange.Last14Days:
                    formattedDate = time.ToString("M");
                    break;

                case TimelineFilterRange.Custom:
                    formattedDate = time.ToString("h tt");
                    break;
            }

            return formattedDate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
