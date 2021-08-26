using System;
using System.Collections.Generic;
using System.ComponentModel;
//using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Objects;
using System.Threading;

using Idera.SQLdm.DesktopClient.Controls;
using Infragistics.Win.UltraWinTabControl;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs;
using System.Text.RegularExpressions;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common.Helpers;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesWaitStatsActive : ServerBaseView, IShowFilterDialog
    {
        private static readonly object updateLock = new object();

        // SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
      //  private const string ADVANCED_QUERY_VIEWS =
     //       "Click here to check out the Advanced Query Views in the SQLDM Web Console.";

        private const string PERMISSION_MESSAGE = "The SQL Diagnostic Manager monitoring account does not have sufficient privileges to execute this collector. Please review the product documentation for required permissions.";

        private enum ChartMode { Duration, Timeline }

        private ChartMode    chartMode;
        private readonly     ServerSummaryHistoryData historyData;        
        private DateTime?    latestSnapshotDateTime;        
        private DataTable    chartDataTable;
        private DataTable    chartRealTimeDataTable;
        private DataTable    chartHistoricalDataTable;        
        private Label        Sql2kWarningLabel;
        private Label        PermissionsWarningLabel;
        private string       groupby;
        private List<string> waitCategories;
        private bool         waitTypesLookupInitialized;
        private bool         overAxisX;
        private int          overSeries;
        private string       fulllabeltext;
        private bool         initTimelineColumns;
        private int          maxDataRows                        = 10;
        private bool         chartRealTimeDataTablePrePopulated = false;
        private bool         configurationLoaded                = false;
        //private Exception    historyModeLoadError               = null;
        private Chart        contextMenuSelectedChart           = null;
        private DateTime?    currentHistoricalSnapshotDateTime  = null;
        private object       currentSnapshot                    = null;
        private long         ctorInitialized                    = 0;
        private long         otherInitialized                   = 0;
        private int          maxLabelLength                     = 25; // maximum length of chart labels before truncating
        private Dictionary<string, string>       filters;
        private Dictionary<string, WaitTypeInfo> waitTypesLookup; // key-> wait_type, values: [category id, category name, wait defintion]        
        private Dictionary<string, string>       fullChartLabels; // key-> shortened label, value -> full label text
        private Dictionary<string, Color>        categoryColors;
        private Dictionary<string, string>       statementHashes;

        private const string col_timestamp    = "timestamp";
        private const string col_duration     = "duration";
        private const string col_sessionId    = "session id";
        private const string col_waitType     = "wait type";
        private const string col_waitCategory = "wait category";
        private const string col_hostName     = "host name";
        private const string col_programName  = "program name";
        private const string col_loginName    = "login name";
        private const string col_databaseName = "database name";
        private const string col_statementTxt = "statement txt";
        private const string col_statementFullTxt = "statement full txt";
        private const string col_ishistorical = "IsHistorical";

        private int activeWaitCollectorCollectionTimeSeconds = 30;
        private QueryWaitsFilter filter;
        AdvancedQueryFilterConfiguration serviceFilter;
        private bool XEEnabled = false;
        private bool QsEnabled = false;

        private static readonly Guid ClientsessionGuid = Guid.NewGuid();

        #region Properties

        public override DateTime? HistoricalSnapshotDateTime
        {
            get { return historyData.HistoricalSnapshotDateTime; }

            set
            {
                historyData.HistoricalSnapshotDateTime = value;
                currentHistoricalSnapshotDateTime = null;
            }
        }

        #endregion

        public ResourcesWaitStatsActive(int instanceId, ServerSummaryHistoryData historyData) : base(instanceId)
        {
            this.chartMode       = ChartMode.Timeline;
            this.historyData     = historyData;
            this.waitTypesLookup = ApplicationModel.Default.WaitTypes;
            this.waitCategories  = new List<string>();
            this.groupby         = col_statementTxt;
            this.filters         = new Dictionary<string, string>();
            this.fullChartLabels = new Dictionary<string, string>();
            this.categoryColors  = new Dictionary<string, Color>();
            this.filter          = new QueryWaitsFilter();
            this.serviceFilter   = new AdvancedQueryFilterConfiguration();
            this.statementHashes = new Dictionary<string, string>();

            InitializeComponent();

            this.Sql2kWarningLabel = new Label();
            this.Sql2kWarningLabel.Dock = DockStyle.Fill;
            this.Sql2kWarningLabel.Location = new Point(0, 0);
            this.Sql2kWarningLabel.Name = "Sql2kWarningLabel";
            this.Sql2kWarningLabel.Size = new Size(100, 100);
            this.Sql2kWarningLabel.Text = "This view is only supported on SQL Server 2005 or higher.";
            this.Sql2kWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Sql2kWarningLabel.Visible = false;
            this.Controls.Add(this.Sql2kWarningLabel);
            this.Sql2kWarningLabel.BringToFront();

            this.PermissionsWarningLabel = new Label();
            this.PermissionsWarningLabel.Dock = DockStyle.Fill;
            this.PermissionsWarningLabel.Location = new Point(0, 0);
            this.PermissionsWarningLabel.Name = "PermissionsWarningLabel";
            this.PermissionsWarningLabel.Size = new Size(100, 100);
            this.PermissionsWarningLabel.Text = PERMISSION_MESSAGE;
            this.PermissionsWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.PermissionsWarningLabel.Visible = false;
            this.Controls.Add(this.PermissionsWarningLabel);
            this.PermissionsWarningLabel.BringToFront();

            ChartFxExtensions.SetContextMenu(chart, toolbarsManager);

            InitializeChart();
            InitializeDataSources();
            //SQLdm 10.4 (Charles Schultz) Removal of advertisement for Web Console
            /*
            // START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
            advancedQueryViewLabel.Text = ADVANCED_QUERY_VIEWS;
            advancedQueryViewPanel.Visible = true;
            // END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client*/

            ultraTabStripControl1.Tabs[0].Tag = col_waitType;
            ultraTabStripControl1.Tabs[1].Tag = col_statementTxt;
            ultraTabStripControl1.Tabs[2].Tag = col_programName;
            ultraTabStripControl1.Tabs[3].Tag = col_databaseName;
            ultraTabStripControl1.Tabs[4].Tag = col_hostName;
            ultraTabStripControl1.Tabs[5].Tag = col_sessionId;
            ultraTabStripControl1.Tabs[6].Tag = col_loginName;

            filter.OnChanged += new EventHandler<EventArgs>( filter_OnChanged );
            breadCrumb1.OnTrailChanged += new EventHandler<Idera.SQLdm.DesktopClient.Controls.BreadCrumbTrailChangedEventArgs>( BreadCrumb1_OnTrailChanged );
            chart.GetAxisLabel += new AxisLabelEventHandler( chart_GetAxisLabel );
            chart.AxisX.Notify = true;
            Interlocked.Increment( ref ctorInitialized );
            AdaptFontSize();
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            updateColor();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        private void ScaleControlsAsPerResolution()
        {
            this.chart.LegendBox.AutoSize = true;
        }      
        #region Initialization

        private void InitializeChart()
        {
            chart.Tag                           = Common.Constants.QUERY_WAITS_CHART_NAME;
            chart.Printer.Orientation           = PageOrientation.Landscape;
            chart.Printer.Compress              = true;
            chart.Printer.ForceColors           = true;
            chart.Printer.Document.DocumentName = "Query Waits Statistics Chart";
            chart.ToolTipFormat                 = "%s\n%v ms/s";
            chart.AxisY.Title.Text              = "Total Wait Time (milliseconds/second)";
            chart.AxisX.Title.Text              = "";
            chart.Gallery                       = Gallery.Bar;
            chart.AxisX.Step                    = -1;
            chart.AxisX.LabelValue              = 1;
            chart.AxisX.Min                     = 0;
            UpdateChartAxisStyles();    // SqlDM 10.2 (Anshul Aggarwal) - Update chart axis styles
            chart.SetMessageText( "NoData", "Please Wait" );
            InitalizeDrilldown(chart);
        }

        private void InitializeDataSources()
        {
            chartDataTable = new DataTable();
            chartRealTimeDataTable = new DataTable();

            // add at least one column
            chartDataTable.Columns.Add( groupby, typeof( string ) );

            // add columns to hold Wait type values
            chartRealTimeDataTable.Columns.Add( col_timestamp,    typeof( DateTime ) );
            
            chartRealTimeDataTable.Columns.Add( col_duration,     typeof( decimal ) );//sqldm10.0 (Gaurav Karwal): changing the type to decimal
            chartRealTimeDataTable.Columns.Add( col_sessionId,    typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_waitType,     typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_waitCategory, typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_hostName,     typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_programName,  typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_loginName,    typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_databaseName, typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_statementTxt, typeof( string ) );
            chartRealTimeDataTable.Columns.Add(col_statementFullTxt, typeof(string));
            chartRealTimeDataTable.Columns.Add(col_ishistorical, typeof(bool));
            chartHistoricalDataTable = chartRealTimeDataTable.Clone();
            chart.DataSource         = chartDataTable;
            //SQLDM-30098 start
            chartRealTimeDataTable.DefaultView.Sort = col_timestamp + " ASC";
            //SQLDM-30098 end
        }  
      
        private void Initialize()
        {
            if( InvokeRequired )
            {
                Invoke( new MethodInvoker( Initialize ) );
                return;
            }
         
            InitializeChartColumns();
            InitializeChartFieldMap();
            InitializeCategoryColors();

            Interlocked.Increment( ref otherInitialized );
           // UpdateWebUiStatus();//SQLdm 9.0 (vineet kumar) -- Fixing DE44333
        }

        private void InitializeChartColumns()
        {
            chartDataTable.Columns.Clear();

            chartDataTable.Columns.Add( groupby, typeof( string ) );

            foreach( string category in waitCategories )
            {
                chartDataTable.Columns.Add( category, typeof( decimal ) );//SQLdm 10.0 (Gaurav Karwal): changed to handle decimal values from long
                chartDataTable.Columns[category].AllowDBNull = true;
            }
        }

        private void InitializeChartFieldMap()
        {
            chart.DataSourceSettings.Fields.Clear();
            chart.DataSourceSettings.Fields.Add( new FieldMap( groupby, FieldUsage.Label ) );

            foreach (string category in waitCategories)
            {
                FieldMap m = new FieldMap(category, FieldUsage.Value);
                chart.DataSourceSettings.Fields.Add(m);
            }

            chart.DataSourceSettings.ReloadData();
        }

        private void InitializeCategoryColors()
        {
            Color[] colors = new Color[]
            {
                Color.FromArgb(38, 100, 193),
                Color.FromArgb(199, 56, 0),
                Color.FromArgb(70, 177, 194),
                Color.FromArgb(118, 200, 45),
                Color.FromArgb(236, 179, 70),
                Color.FromArgb(186, 163, 240),
                Color.FromArgb(34, 137, 153),
                Color.FromArgb(250, 117, 80),
                Color.FromArgb(154, 206, 214),
                Color.FromArgb(30, 84, 92),
                Color.FromArgb(45, 118, 227),
                Color.FromArgb(205, 252, 162),
                Color.FromArgb(116, 38, 189),
                Color.FromArgb(154, 206, 214),
                Color.FromArgb(255, 231, 171),
                Color.FromArgb(215, 57, 214),
                Color.FromArgb(38, 100, 193),
                Color.FromArgb(199, 56, 0),
                Color.FromArgb(70, 177, 194),
            };

            for( int i = 0; i < waitCategories.Count; i++ )
            {
                if( i >= colors.Length )
                    break;

                categoryColors.Add( waitCategories[i], colors[i] );
            }
        }

        #endregion

        #region Updates

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesQueryWaitsView);
        }

        public override void RefreshView()
        {            
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus( this.instanceId );

            if( status != null && status.InstanceVersion != null && status.InstanceVersion.Major <= 8 )
            {
                Sql2kWarningLabel.Visible = true;
                return;// probably won't ever show for sql2k anyway
            }

            if (HistoricalSnapshotDateTime == null ||
             HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                //historyModeLoadError = null;
                base.RefreshView();
            }
            //else
            //    base.RefreshView();
        }

        public override object DoRefreshWork( BackgroundWorker backgroundWorker )
        {
            using( Log.InfoCall( "DoRefreshWork" ) )
            {
                var previousVisibleLimitInMinutes = this.currentRealTimeVisibleLimitInMinutes;
                currentRealTimeVisibleLimitInMinutes = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;
                if ( ApplicationModel.Default.WaitTypes != null && !waitTypesLookupInitialized )
                {
                    waitTypesLookup = ApplicationModel.Default.WaitTypes;
                    waitCategories.AddRange( ApplicationModel.Default.WaitCategories );
                    waitTypesLookupInitialized = true;

                    // wait for contstructor to finish (make sure it's done)
                    while( Interlocked.Read( ref ctorInitialized ) == 0 ) { }

                    Initialize();

                    // wait for others to initialize
                    while( Interlocked.Read( ref otherInitialized ) == 0 ) { }
                }

                DateTime? historyDateTime = HistoricalSnapshotDateTime;

                if( historyDateTime == null )
                {
                    Log.Info( "Getting real-time snapshot" );
                    return GetRealTimeSnapshot(previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes);
                }
                else
                {
                    Log.InfoFormat( "Populating historical snapshots (end={0}).", historyDateTime.Value);
                    return GetHistoricalSnapshots(historyDateTime.Value,
                        ViewMode == ServerViewMode.Historical ? ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes :
                        MathHelper.GetCeilMinutes(HistoricalStartDateTime, HistoricalSnapshotDateTime));
                }
            }
        }

        public override void UpdateData( object data )
        {
            lock( updateLock )
            {
                if( HistoricalSnapshotDateTime == null )
                {
                    if( data is ActiveWaitsSnapshot )
                        UpdateDataWithRealRealTimeSnapshot( data as ActiveWaitsSnapshot );
                }
                else
                {
                    UpdateDatawithHistoricalSnapshots( data as DataTable );
                }
                
                if(chartMode == ChartMode.Timeline) initTimelineColumns = true;
                
                UpdateChart();
                
                if (chartMode == ChartMode.Timeline) initTimelineColumns = false;
            }
        }

        #region Real time updates

        private ActiveWaitsSnapshot GetRealTimeSnapshot(bool visibleMinutesIncreased)
        {

            if (!chartRealTimeDataTablePrePopulated)
            {
                PrePopulateRealTimeDataTable();
                //PrePopulateWithRandomData();
                chartRealTimeDataTablePrePopulated = true;
            }
            else
            {
                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (visibleMinutesIncreased)
                    ForwardFillHistoricalData();

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();
            }

            IManagementService managementService = ManagementServiceHelper.GetDefaultService();
            ActiveWaitsConfiguration configuration = new ActiveWaitsConfiguration(InstanceId);

            //if the config file has not been loaded
            //if (!configurationLoaded)
            //{  //This really needs to happen every refresh
                LoadConfiguration();//load filters and XE state from repository
                activeWaitCollectorCollectionTimeSeconds = configuration.CollectionTimeSeconds;
                configurationLoaded = true;
            //}
            
            configuration.Enabled           = true;
            configuration.ClientSessionId   = ClientsessionGuid;
            configuration.RunTime           = new TimeSpan( 0, 1, 0 );

            configuration.WaitForPickupTime = new TimeSpan( 0, 2, 0 );
            
            //flag this config as containing the latest possible filter criteria (since we just fetched them this instant from repository)
            configuration.IsMaster = true;
            
            configuration.AdvancedConfiguration = serviceFilter;
            configuration.EnabledXe = XEEnabled;
            
            // SQLdm 10.4(Varun Chopra) query waits using Query Store
            configuration.EnabledQs = this.QsEnabled;

            ActiveWaitsSnapshot snapshot = managementService.GetActiveWaits( configuration );

            return snapshot;
        }
        /// <summary>
        /// Relevant Query Waits parameters must be fetched from the database to ensure the correct rules are observed.
        /// The query waits are continuously collected by the management service into waitStatisticsCollectors.
        /// It must make use of the continuouscollectioncontext for scheduled refreshed.
        /// For onDemand the config file must be populated here
        /// </summary>
        private void LoadConfiguration()
        {
            DataTable data = RepositoryHelper.GetQueryWaitsConfiguration( this.instanceId );

            if( data.Rows.Count == 0 )
                return;

            try
            {
                if( !( data.Rows[0][0] is DBNull ) )
                    serviceFilter = AdvancedQueryFilterConfiguration.DeserializeFromXml( ( string )data.Rows[0][0] );
                if (!(data.Rows[0][1] is DBNull))
                    XEEnabled = ((string) data.Rows[0][1]) == "True";
                if (!(data.Rows[0][2] is DBNull))
                    this.QsEnabled = ((string)data.Rows[0][2]) == "True";
            }
            catch ( Exception ex )
            {
                Log.Error( ex );
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Refreshes real-time data from the DB.
        /// </summary>
        private void PrePopulateRealTimeDataTable()
        {
            PopulateRealTimeDataTable(DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Populates real time datatable with select range data.
        /// </summary>
        private void PopulateRealTimeDataTable(DateTime endDateTime, int? minutes, bool clearData)
        {
            DataTable data = RepositoryHelper.GetQueryWaitStatistics(this.instanceId, endDateTime, minutes ?? 0);

            string waittype = string.Empty;

            if( data == null || data.Rows.Count == 0 )
                return;

            DateTime min = DateTime.MaxValue;
            DateTime max = DateTime.MinValue;

            lock( updateLock )
            {
                chartRealTimeDataTable.BeginLoadData();
                if (clearData) chartRealTimeDataTable.Clear();
                int insertionIndex = -1;
                if (data.Rows.Count > 0 && !clearData)
                {
                    insertionIndex = ServerSummaryHistoryData.GetInsertionIndex(chartRealTimeDataTable, col_timestamp,
                        (DateTime)data.Rows[0][col_timestamp]);
                }
                foreach ( DataRow datarow in data.Rows )
                {
                    DataRow newrow = chartRealTimeDataTable.NewRow();

                    newrow[col_timestamp]    = datarow[col_timestamp];
                    newrow[col_ishistorical] = true;
                    newrow[col_waitType]     = datarow[col_waitType];
                    newrow[col_duration]     = datarow[col_duration];
                    newrow[col_sessionId]    = datarow[col_sessionId];
                    newrow[col_hostName]     = datarow[col_hostName];
                    newrow[col_programName]  = MakeSafeChartLabel( ( string )datarow[col_programName] );
                    newrow[col_loginName]    = datarow[col_loginName];
                    newrow[col_databaseName] = MakeSafeChartLabel( ( string )datarow[col_databaseName] );
                    newrow[col_statementTxt] = MakeSafeChartLabel( ( string )datarow[col_statementTxt] );
                    newrow[col_statementFullTxt] = datarow[col_statementTxt];

                    if( waitTypesLookup != null && waitTypesLookup.ContainsKey( ( string )datarow[col_waitType] ) )
                        newrow[col_waitCategory] = waitTypesLookup[( string )datarow[col_waitType]].CategoryName;

                    if (insertionIndex >= 0)
                        chartRealTimeDataTable.Rows.InsertAt(newrow, insertionIndex++);
                    else
                        chartRealTimeDataTable.Rows.Add(newrow);

                    DumpRow( "PrePopulateRealTimeDataTable", newrow );
                }

                chartRealTimeDataTable.EndLoadData();
            }            
        }
        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Replaces realtime data that is now stale with data from repository.
        /// </summary>
        private void BackFillScheduledHistoricalData()
        {
            using (Log.InfoCall("BackFillScheduledHistoricalData"))
            {
                if (chartRealTimeDataTable != null &&
                    Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    DateTime startDateTime, endDateTime;
                    var backfillRequired = ServerSummaryHistoryData.GetBackFillHistoricalRange(chartRealTimeDataTable,
                        out startDateTime, out endDateTime, col_timestamp);
                    if (!backfillRequired)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1} of historical data",
                        startDateTime, endDateTime);
                    PopulateRealTimeDataTable(endDateTime, MathHelper.GetMinutes(startDateTime, endDateTime), false);
                }
            }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) : Fills History Data when scale increases
        /// </summary>
        private void ForwardFillHistoricalData()
        {
            using (Log.InfoCall("ForwardFillHistoricalData"))
            {
                if (chartRealTimeDataTable != null)
                {
                    DateTime startDateTime, endDateTime;
                    ServerSummaryHistoryData.GetForwardFillHistoricalRange(chartRealTimeDataTable,
                        out startDateTime, out endDateTime);
                    if (endDateTime <= startDateTime)
                        return;

                    Log.InfoFormat("Backfilling from {0} to {1}  of historical data", startDateTime, endDateTime);
                    PopulateRealTimeDataTable(endDateTime, MathHelper.GetMinutes(startDateTime, endDateTime), false);
                }
            }
        }

        /// <summary>
        /// Write the row details to the log for debugging
        /// </summary>
        /// <param name="header"></param>
        /// <param name="row"></param>
        private void DumpRow(string header, DataRow row)
        {
            StringBuilder b = new StringBuilder();

            b.Append( header );
            b.Append( ": [" );

            int numcols = row.Table.Columns.Count;

            for( int i = 0; i < numcols; i++ )
            {
                b.Append( row[i] );

                if( i < numcols - 1 )
                    b.Append( "," );
            }

            b.Append( "]" );

            Log.Debug( b.ToString() );
        }

        private void PrePopulateWithRandomData()
        {
            string[] sids           = new string[] { "session 1", "session 2", "session 3", "session 4", "session 5", "session 6", "session 7", "session 8", "session 9", "session 10", "session 11", "session 12" };
            int[]    durations      = new int[]    { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000 };
            string[] waitTypes      = new string[] { "IX_CPU", "PP_AU_BACKUP", "PREMTPIVE_CPU", "BACKUP_PS", "NN_WAIT", "SIGNAL_X_1", "BEEF_OS" };
            string[] hostNames      = new string[] { "qdavislaptop", "qdesktop", "dmlab", "host 4", "host 5", "host 6", "host 7", "host 8", "host 9", "host 10", "host 11", "host 12" };
            string[] programNames   = new string[] { "Visual Studio", "Enterprise Manager", "app 3", "app 4", "app 5", "app 6", "app 7", "app 8", "app 9", "app 10", "app 11", "app 12" };
            string[] loginNames     = new string[] { "qdavis", "testuser", "user 3", "user 4", "user 5", "user 6", "user 7", "user 8", "user 9", "user 10", "user 11" };
            string[] databaseNames  = new string[] { "AdventureWorks", "Northwind", "Pubs", "database 1", "database 2", "database 3", "database 4", "database 5", "database 6", "database 7", "database 8" };
            string[] statements     = new string[] { @"select 
	x1,
	x2,
	x3
from table", "select top 2 from table2", "insert into table values (v1, v2)", "update table set col1 = 2 where col2 = 3", "select a from b", "select b from c", "select c from d", "select d from e", "select e from f", "select f from g", "select g from h", "select a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p, q, r, s, t, u, v, w, x, y, z from alphabet", "select a, b, c, d, e from (select a1, b1, c1 from (select x, y, z from table1))", "select a, b, c, d, e from (select a1, b1, x1, y1 from tableZ))" };
            
            //int[] offsets = new int[] { 5, 10, 15, 20, 25, 6, 11, 16, 211, 26 };
            int[] offsets = new int[50];

            for( int i = 0; i < offsets.Length; i++ )
                offsets[i] = i;

            DateTime dt;
            long   duration;
            string session_id;
            string wait_type;
            string wait_category;
            string host_name;
            string program_name;            
            string login_name;
            string database_name;
            string statement_txt;

            chartRealTimeDataTable.BeginLoadData();

            for (int i = 0; i < 100; i++)
            {
                dt = DateTime.Now.AddMinutes(-offsets[i % (offsets.Length)]);

                session_id    = GetRandomElement<string>(sids);
                duration      = GetRandomElement<int>(durations);
                wait_type     = GetRandomElement<string>(waitTypes);
                wait_category = GetRandomElement<string>(waitCategories.ToArray());
                host_name     = GetRandomElement<string>(hostNames);
                program_name  = GetRandomElement<string>(programNames);
                login_name    = GetRandomElement<string>(loginNames);
                database_name = GetRandomElement<string>(databaseNames);
                statement_txt = GetRandomElement<string>(statements);

                DataRow r = chartRealTimeDataTable.NewRow();

                r[col_timestamp]    = dt;
                r[col_duration]     = 10000;
                r[col_sessionId]    = session_id;
                r[col_waitType]     = wait_type;
                r[col_waitCategory] = wait_category;
                r[col_hostName]     = host_name;
                r[col_programName]  = MakeSafeChartLabel(program_name);
                r[col_loginName]    = login_name;
                r[col_databaseName] = MakeSafeChartLabel( database_name );
                r[col_statementTxt] = MakeSafeChartLabel( statement_txt );
                r[col_statementFullTxt] = statement_txt;

                chartRealTimeDataTable.Rows.Add(r);
            }

            chartRealTimeDataTable.EndLoadData();
        }

        Random rand = new Random();
        private T GetRandomElement<T>(T[] array)
        {
            if (array.Length == 0)
                return default(T);

            int idx = rand.Next(array.Length);

            return array[idx];
        }

        /// <summary>
        /// Take the raw data from active waits, make sure the data is all safe for display in the chart
        /// Add a hash of the statementtxt to the staement hash dictionary
        /// </summary>
        /// <param name="snapshot"></param>
        private void UpdateDataWithRealRealTimeSnapshot(ActiveWaitsSnapshot snapshot)
        {
            if( snapshot == null )
            {
                Log.Info( "Snapshot returned as NULL" );
                return;
            }

            // was there an error
            if( snapshot.Error != null )
            {
                if (snapshot.ProductVersion != null && snapshot.ProductVersion.Major <= 8)
                {
                    Sql2kWarningLabel.Visible = true;
                }
                else if (snapshot.ProbeError != null)
                {
                    if (!PermissionsWarningLabel.Visible)
                    {
                        PermissionsWarningLabel.Visible = true;
                    }
                    if (PermissionsWarningLabel.Text != PERMISSION_MESSAGE)
                    {
                        PermissionsWarningLabel.Text = PERMISSION_MESSAGE;
                    }
                }

                Log.Error("UpdateDataFromRealtimeSnapshot encountered an error while obtaining file activity data.", snapshot.Error);
                ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now, snapshot.Error ) );
                return;
            }

            PermissionsWarningLabel.Visible = false;
            Sql2kWarningLabel.Visible = false;
            latestSnapshotDateTime = snapshot.TimeStamp.HasValue ? snapshot.TimeStamp.Value.ToLocalTime() : snapshot.TimeStamp;
            currentSnapshot        = snapshot;
            DataTable table        = snapshot.ActiveWaits;

            Log.Info( string.Format("Snapshot returned {0} rows.", table.Rows.Count) );

            // loop through the snapshot values and store them in the real time table
            foreach( DataRow snaprow in table.Rows )
            {
                DataRow newrow = chartRealTimeDataTable.NewRow();

                var sampleTime = (DateTime)snaprow["UTCCollectionDateTime"];

                int secondsPassedBucketStart = sampleTime.ToLocalTime().Second % activeWaitCollectorCollectionTimeSeconds;

                newrow[col_timestamp] = sampleTime.ToLocalTime().Add(new TimeSpan(0, 0, 0, activeWaitCollectorCollectionTimeSeconds - secondsPassedBucketStart)); // or snaprow[StatementUTCStartTime] converted to local
                
                newrow[col_waitType]     = snaprow["WaitType"];
                newrow[col_duration] = ((decimal)snaprow["WaitDuration"] > (activeWaitCollectorCollectionTimeSeconds * 1000) ? (activeWaitCollectorCollectionTimeSeconds * 1000) : (decimal)snaprow["WaitDuration"]) / activeWaitCollectorCollectionTimeSeconds; //wait duration cannot exceed the length of the bucket
                newrow[col_sessionId]    = snaprow["SessionID"];
                newrow[col_hostName]     = MakeSafeChartLabel(snaprow["HostName"] as string);
                newrow[col_programName]  = MakeSafeChartLabel(snaprow["ProgramName"] as string);
                newrow[col_loginName]    = MakeSafeChartLabel(snaprow["LoginName"] as string);
                newrow[col_databaseName] = MakeSafeChartLabel(snaprow["DatabaseName"] as string);
                newrow[col_statementTxt] = MakeSafeChartLabel(snaprow["StatementText"] as string);
                newrow[col_statementFullTxt] = snaprow["StatementText"];

                // store a hash of the full statement for drill down to the query monitor view
                string hash = SqlParsingHelper.GetSignatureHash((string)snaprow["StatementText"]);
                if (!statementHashes.ContainsKey((string)newrow[col_statementTxt]))
                    statementHashes.Add((string)newrow[col_statementTxt], hash);

                newrow[col_waitCategory] = "Other";
                if( waitTypesLookup != null && waitTypesLookup.ContainsKey( ( string )snaprow["WaitType"] ) )
                    newrow[col_waitCategory] = waitTypesLookup[( string )snaprow["WaitType"]].CategoryName;

                chartRealTimeDataTable.Rows.Add( newrow );

                DumpRow( "UpdateDataWithRealRealTimeSnapshot", newrow );
            }

            GroomHistoryData();

            // update complete
            ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now ) );
        }

        /// <summary>
        /// get the chart to reflect the latest changes
        /// </summary>
        private void UpdateChart()
        {
            //prevent cross thread exceptions if called on background thread
            if( InvokeRequired )
            {
                this.Invoke( new MethodInvoker( UpdateChart ) );
                return;
            }
            
            if(HistoricalSnapshotDateTime == null)
                operationalStatusPanel.Visible = false;

            if( chartMode == ChartMode.Duration )
                UpdateChartByDuration();
            else //timeline view
                UpdateChartByMode();            
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateChartByDuration()
        {
            DataTable sourceTable = GetCurrentDataTable();

            chart.SuspendLayout();
            chartDataTable.BeginLoadData();

            Sum( sourceTable, chartDataTable );
            SortChartData(chartDataTable);

            if( chartDataTable.Rows.Count == 0 )
                chart.SetMessageText( "NoData", "There is no data to show in this view." );

            chartDataTable.EndLoadData();
            chart.ResumeLayout();
            chart.DataSourceSettings.ReloadData();

            chart.AxisX.ResetScale();

            UpdateCategoryColors();
        }

        /// <summary>
        /// Update timeline mode
        /// </summary>
        private void UpdateChartByMode()
        {
            chart.SuspendLayout();
            chartDataTable.BeginLoadData();

            //chart data tabe is empty at this point (only a timestamp)
            SumTimeline( chartDataTable );

            FilterChartData( chartDataTable );

            if( chartDataTable.Rows.Count == 0 )
                chart.SetMessageText( "NoData", "There is no data to show in this view." );

            chartDataTable.EndLoadData();
            chart.ResumeLayout();
            chart.DataSourceSettings.ReloadData();
            
            chart.AxisX.ResetScale();

            chart.Refresh();
        }

        /// <summary>
        /// Remove all rows that sre older than the charthistorylimitinminutes
        /// </summary>
        private void GroomHistoryData()
        {            
            if (chartRealTimeDataTable != null)
            {
                Log.Debug( string.Format( "GroomHistoryData before groom: {0} rows", chartRealTimeDataTable.Rows.Count ) );

                DataRow[] groomedRows = chartRealTimeDataTable.Select(ServerSummaryHistoryData.GetGroomingFilter(col_timestamp));
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }

                chartRealTimeDataTable.AcceptChanges();

                Log.Debug( string.Format( "GroomHistoryData after groom: {0} rows", chartRealTimeDataTable.Rows.Count ) );
            }
        }

        #endregion

        #region Historical updates

        private object GetHistoricalSnapshots(DateTime endDateTime, int? minutes)
        {
            return RepositoryHelper.GetQueryWaitStatistics(this.instanceId, endDateTime, minutes ?? 0);
        }

        private void UpdateDatawithHistoricalSnapshots( DataTable data )
        {
            ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel, Properties.Resources.StatusWarningSmall);
            currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
            currentHistoricalStartDateTime = HistoricalStartDateTime;

            if( data == null )
                return;

            chartHistoricalDataTable.Clear();
            chartHistoricalDataTable.BeginLoadData();

            foreach( DataRow datarow in data.Rows )
            {
                DataRow newrow = chartHistoricalDataTable.NewRow();

                newrow[col_timestamp]    = datarow[col_timestamp];
                newrow[col_waitType]     = datarow[col_waitType];
                newrow[col_duration]     = datarow[col_duration];
                newrow[col_sessionId]    = datarow[col_sessionId];
                newrow[col_hostName]     = datarow[col_hostName];
                newrow[col_programName]  = MakeSafeChartLabel( datarow[col_programName] as string );
                newrow[col_loginName]    = datarow[col_loginName];
                newrow[col_databaseName] = MakeSafeChartLabel( datarow[col_databaseName] as string );
                newrow[col_statementTxt] = MakeSafeChartLabel( datarow[col_statementTxt] as string );
                newrow[col_statementFullTxt] = datarow[col_statementTxt];

                string hash = SqlParsingHelper.GetSignatureHash((string)datarow[col_statementTxt]);
                if (!statementHashes.ContainsKey((string)newrow[col_statementTxt]))
                    statementHashes.Add((string)newrow[col_statementTxt], hash);

                newrow[col_waitCategory] = "Other";
                if( waitTypesLookup != null && waitTypesLookup.ContainsKey( ( string )datarow[col_waitType] ) )
                    newrow[col_waitCategory] = waitTypesLookup[( string )datarow[col_waitType]].CategoryName;

                chartHistoricalDataTable.Rows.Add( newrow );
            }

            chartHistoricalDataTable.EndLoadData();

            ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now ) );
        }

        #endregion
        /// <summary>
        /// Sum by groupby
        /// Used by both modes
        /// </summary>
        /// <param name="sourceTable">This is the latest raw data for waits</param>
        /// <param name="targetTable">This is the table containing grouped data</param>
        private void Sum(DataTable sourceTable, DataTable targetTable)
        {
            //DateTime start;
            //DateTime end;

            if( !waitTypesLookupInitialized )
                return;

            Log.Debug( string.Format( "Sum: start: sourcetable has {0} rows", sourceTable.Rows.Count ) );

            //filters sourcetable
            FilterChartData( sourceTable );
            DataTable sortedSourceTable = sourceTable.DefaultView.ToTable();

            Log.Debug( string.Format( "Sum: after filter: {0} rows", sourceTable.Rows.Count ) );
            Log.Debug( string.Format( "Sum: sortedtable: {0} rows", sortedSourceTable.Rows.Count ) );

            StringBuilder filterString = new StringBuilder();

            foreach( string filter in filters.Keys )
                filterString.Append( string.Format( " and [{0}] = '{1}'", filter, MakeSqlSafe(filters[filter], false) ) );

            DataView  v = new DataView(sortedSourceTable);

            //start = DateTime.Now;

            //Debug.Print(start.ToString());
            //group by whatever is in groupby as well as wait_category
            DataTable t = v.ToTable( true, new string[] { groupby, col_waitCategory } ); // get all unique groupby values
            //end = DateTime.Now;
            
            //Debug.Print("start - {0}, end - {1}, duration - {2}", start.ToString(), end.ToString(), (end - start));

            Log.Debug( string.Format( "Sum: table t: groupby = {0}, col_waitcategory = {1}", groupby, col_waitCategory ) );
            Log.Debug( string.Format( "Sum: table t: {0} rows", t.Rows.Count ) );

            List<string> categories = new List<string>();
            List<string> groups     = new List<string>();

            //target tabe has not yet been touched
            // remove sort/filter
            targetTable.DefaultView.Sort = "";
            targetTable.DefaultView.RowFilter = "";

            // clear the target table
            targetTable.Clear();  
            targetTable.Columns.Clear();

            // add a column for the group
            targetTable.Columns.Add( groupby, typeof( string ) );

            // map the fields (only the categories we have)
            chart.DataSourceSettings.Fields.Clear();
            chart.DataSourceSettings.Fields.Add( new FieldMap( groupby, FieldUsage.Label ) );

            // add one column for each wait category (remember t is grouped by groupby,waitcategory)
            foreach (DataRow row in t.Rows)
            {
                if (row[col_waitCategory] is DBNull)
                {
                    continue;
                }
                var wait_category_data = (row[col_waitCategory] is DBNull) ? string.Empty : row[col_waitCategory];//added by Gaurav Karwal for handling if the wait category is null

                // add all of the new categories
                if (!targetTable.Columns.Contains((string)wait_category_data))
                {
                    targetTable.Columns.Add((string)wait_category_data, typeof(decimal)); //SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal
                    //save the category
                    categories.Add((string)wait_category_data);

                    Log.Debug(string.Format("Sum: targettable column added: {0}", (string)wait_category_data));

                    FieldMap m = new FieldMap( ( string )wait_category_data, FieldUsage.Value );
                    chart.DataSourceSettings.Fields.Add( m );
                }
                //save the group if it is not already in groups
                if( !groups.Contains( ( string )row[groupby].ToString() ) )
                    groups.Add( ( string )row[groupby].ToString() );
            }            

            // update the range of labels            
            chart.AxisX.Max = chart.AxisX.Step * groups.Count;

            // add a total column
            targetTable.Columns.Add("total", typeof(decimal)); //SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal

            // add one row for each groupby groups
            foreach (string group in groups)
            {
                DataRow r = targetTable.NewRow();

                r[groupby] = group;

                targetTable.Rows.Add(r);

                Log.Debug( string.Format( "Sum: targettable: added row for group = {0}", group ) );
            }
            //start = DateTime.Now;

            // for each groupby group, update each wait category column with durations for that category
            foreach( DataRow row in targetTable.Rows )
            {
                foreach( string category in categories )
                {                    
                    //object result = sourceTable.Compute( "Sum("+ col_duration +")", string.Format( "[{0}] = '{1}' and ["+ col_waitCategory +"] = '{2}' {3}", groupby, MakeSqlSafe((string)row[groupby]), category, filterString ) );                    
                    //object result = sortedSourceTable.Compute( "Sum("+ col_duration +")", string.Format( "[{0}] = '{1}' and ["+ col_waitCategory +"] = '{2}' {3}", groupby, MakeSqlSafe((string)row[groupby]), category, filterString ) );
                    
                    object result = sortedSourceTable.Compute("Sum(" + col_duration + ")", string.Format("[{0}] = '{1}' and [" + col_waitCategory + "] = '{2}' {3}", groupby, ((string)row[groupby]).Replace("'","''"), category, filterString));

                    Log.Debug( string.Format( "Sum: compute duration: {0}", string.Format( "[{0}] = '{1}' and [" + col_waitCategory + "] = '{2}' {3}", groupby, MakeSqlSafe( ( string )row[groupby],false ), category, filterString ) ) ); 

                    if( result is DBNull)
                    {
                        row[category] = DBNull.Value;
                        continue;
                    }

                    row[category] = (decimal)result; //SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal
                }
            }
            //end = DateTime.Now;
            //Debug.Print("computesum: start - {0}, end - {1}, duration - {2}", start.ToString(), end.ToString(), (end - start));

            // sum up the totals
            decimal total = 0; //SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal
            foreach (DataRow row in targetTable.Rows)
            {
                total = 0;

                foreach (string category in categories)
                {
                    if (row[category] is DBNull)
                        continue;

                    total += (decimal)row[category]; //SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal
                }

                row["total"] = total;
            }

            Log.Debug( string.Format( "Sum: start: sourcetable has {0} rows", sourceTable.Rows.Count ) );
            Log.Debug( string.Format( "Sum: start: targettable has {0} rows", targetTable.Rows.Count ) );
        }     
   
        private string MakeSafeChartLabel( string inputtext )
        {
            if (string.IsNullOrEmpty(inputtext))
                return "N/A";

            string shorttext = new String(inputtext.ToCharArray());

            // if short enough, then we return
            if( shorttext.Length < maxLabelLength )
                return shorttext;

            // remove non-printable whitesapce
            shorttext = Regex.Replace( shorttext, "[\x00-\x1f]", " " );
            shorttext = shorttext.Substring( 0, maxLabelLength - 3 ) + "...";

            string longtext = Regex.Replace( shorttext, "[\x00-\x1f]", " " ); // save long text with white space removed            
            string fulltext = Regex.Replace( inputtext, "\t", " " ); // remove tabs from input text

            // if we already have this text stored, then return it's key (shortened version)
            if( fullChartLabels.ContainsValue( fulltext ) )
            {
                foreach( string key in fullChartLabels.Keys )
                {
                    if( fullChartLabels[key] == fulltext )
                        return key;
                }
            }
            
            int i = 1;

            while( fullChartLabels.ContainsKey( shorttext ) )
                shorttext = string.Format( "{0}...{1}", longtext.Substring( 0, maxLabelLength - 4 ), i++ );

            fullChartLabels.Add( shorttext, fulltext );

            return shorttext;
        }
        /// <summary>
        /// Replace quote with quote quote and enclose square brackets in square brackets
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isForRowFilterLike">Row filter sql requires additional escaping</param>
        /// <returns></returns>
        private string MakeSqlSafe( string sql , bool isForRowFilterLike)
        {
            if (isForRowFilterLike)
            {
                sql = EscapeForLikeValue(sql);
            }
            else
            {
                sql = sql.Replace("'", "''");
            }
            return sql;
        }

        /// <summary>
        /// return true if the given index has enclosing square brackets
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool IsInSquares(string sql, int index)
        {
            int openingDepth = 0;
            int closingDepth = 0;

            if (index >= 1)
            {
                //look earlier in the string for extra opening parens
                //a previous opening paren increases the depth but only if there is a later closing paren
                for (int i = 0; i < index; i++)
                {
                    char c = sql[i];
                    if (c == '[') openingDepth++;
                    if (c == ']') if(openingDepth>0) openingDepth--;
                }
            }
            
            //look later in the string
            //only bother if there are NETT opening parens
            if (index < sql.Length && openingDepth > 0)
            {
                for (int i = index + 1; i < sql.Length; i++)
                {
                    char c = sql[i];
                    if (c == '[') if(closingDepth < 0) closingDepth++;
                    if (c == ']') closingDepth--;
                }
            }

            //+ve opening depth means more opens than closes NETT opens
            //-ve closing depth means more closes than opens NETT closes
            //if there were nett opens and nett closes
            return openingDepth > 0 && closingDepth < 0;
        }

        /// <summary>
        /// *%[] must be escaped when used in a like clause of a datarowfilter
        /// </summary>
        /// <param name="valueWithoutWildcards"></param>
        /// <returns></returns>
        public static string EscapeForLikeValue(string valueWithoutWildcards)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < valueWithoutWildcards.Length; i++)
            {
                char c = valueWithoutWildcards[i];
                bool isInParens = IsInSquares(valueWithoutWildcards, i);

                if (((c == '*' && isInParens) || (c == '*' && !isInParens && i > 0 && i < valueWithoutWildcards.Length-1))
                        || ((c == '%' && isInParens) || (c == '%' && !isInParens && i > 0 && i < valueWithoutWildcards.Length-1))
                        || c == '[' || c == ']')
                    sb.Append("[").Append(c).Append("]");
                else if (c == '\'')
                    sb.Append("''");
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// sort by total ASC, remove null totals and do a top N
        /// </summary>
        /// <param name="targetTable"></param>
        private void SortChartData(DataTable targetTable)
        {                   
            if( !waitTypesLookupInitialized )
                return;

            Log.Debug( string.Format( "SortChartData: start: targettable has {0} rows", targetTable.Rows.Count ) );
                        
            if( targetTable.Rows.Count <= maxDataRows )
            {
                targetTable.DefaultView.Sort      = "total ASC";
                targetTable.DefaultView.RowFilter = "total > 0";
            }
            else
            {
                DataTable t = targetTable.Copy();

                t.DefaultView.Sort      = "total ASC";
                t.DefaultView.RowFilter = "total > 0";

                targetTable.Clear();

                int topN = t.DefaultView.Count < maxDataRows ? t.DefaultView.Count : maxDataRows;
                int startingN = t.DefaultView.Count > maxDataRows ? t.DefaultView.Count - maxDataRows : 0;

                for( int i = 0; i < topN; i++ )
                    targetTable.ImportRow(t.DefaultView[startingN + i].Row);
            }

            Log.Debug( string.Format( "SortChartData: end: targettable has {0} rows", targetTable.Rows.Count ) );
        }

        /// <summary>
        /// Apply all filter criteria and return only data from the last half hour
        /// </summary>
        /// <param name="targetTable">This table will be filtered</param>
        private void FilterChartData( DataTable targetTable )
        {
            if( !waitTypesLookupInitialized )
                return;

            // SqlDM 10.2 (Anshul Aggarwal) - Filter query waits data as per the view mode.
            // SQLDM-27600 - History Range control_DC_Queries -> Query Waits: Selecting custom range in history range settings doesn't show query waits data
            DateTime viewFilter = DateTime.MinValue;
            if (ViewMode == ServerViewMode.RealTime)
                viewFilter = DateTime.Now.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
            else if (ViewMode == ServerViewMode.Historical)
                viewFilter = HistoricalSnapshotDateTime.Value.Subtract(TimeSpan.FromMinutes(ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes));
            else
                viewFilter = HistoricalStartDateTime.Value;


            Log.Debug( string.Format( "FilterChartData: viewfilter time > {0}", viewFilter));
            //SQLDM-19237: Changes to have # for the timestamp comparison
            targetTable.DefaultView.RowFilter = string.Format("timestamp > #{0}# {1}", viewFilter.ToString(CultureInfo.InvariantCulture), BuildFilterString( targetTable ));
        }

        private string BuildFilterString( DataTable targetTable )
        {
            StringBuilder text = new StringBuilder();

            try
            {
                if( !string.IsNullOrEmpty( filter.SqlUserNameIncludeFilter ) && targetTable.Columns.Contains( col_loginName ) ) 
                    text.Append( string.Format( " and [{0}] LIKE '{1}' ", col_loginName, MakeSqlSafe(filter.SqlUserNameIncludeFilter, true) ) );

                if( !string.IsNullOrEmpty( filter.SqlUserNameExcludeFilter ) && targetTable.Columns.Contains( col_loginName ) )
                    text.Append(string.Format(" and NOT ([{0}] LIKE '{1}') ", col_loginName, MakeSqlSafe(filter.SqlUserNameExcludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.ApplicationNameIncludeFilter ) && targetTable.Columns.Contains( col_programName ) )
                    text.Append(string.Format(" and [{0}] LIKE '{1}' ", col_programName, MakeSqlSafe(filter.ApplicationNameIncludeFilter, true)));
                
                if (filter.SqldmapplicationExcludeFilter && targetTable.Columns.Contains(col_programName))
                    text.Append(string.Format(" and [{0}] NOT LIKE '{1}%' ", col_programName, MakeSqlSafe(Common.Constants.ConnectionStringApplicationNamePrefix, true)));

                if( !string.IsNullOrEmpty( filter.ApplicationNameExcludeFilter ) && targetTable.Columns.Contains( col_programName ) )
                    text.Append(string.Format(" and NOT ([{0}] LIKE '{1}') ", col_programName, MakeSqlSafe(filter.ApplicationNameExcludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.DatabaseNameIncludeFilter ) && targetTable.Columns.Contains( col_databaseName ) )
                    text.Append(string.Format(" and [{0}] LIKE '{1}' ", col_databaseName, MakeSqlSafe(filter.DatabaseNameIncludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.DatabaseNameExcludeFilter ) && targetTable.Columns.Contains( col_databaseName ) )
                    text.Append(string.Format(" and ([{0}] NOT LIKE '{1}') ", col_databaseName, MakeSqlSafe(filter.DatabaseNameExcludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.ClientNameIncludeFilter ) && targetTable.Columns.Contains( col_hostName ) )
                    text.Append(string.Format(" and [{0}] LIKE '{1}' ", col_hostName, MakeSqlSafe(filter.ClientNameIncludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.ClientNameExcludeFilter ) && targetTable.Columns.Contains( col_hostName ) )
                    text.Append(string.Format(" and NOT ([{0}] LIKE '{1}') ", col_hostName, MakeSqlSafe(filter.ClientNameExcludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.SqlTextIncludeFilter ) && targetTable.Columns.Contains( col_statementFullTxt ) )
                    text.Append(string.Format(" and [{0}] LIKE '{1}' ", col_statementFullTxt, MakeSqlSafe(filter.SqlTextIncludeFilter, true)));

                if( !string.IsNullOrEmpty( filter.SqlTextExcludeFilter ) && targetTable.Columns.Contains( col_statementFullTxt ) ) 
                    text.Append(string.Format(" and NOT ([{0}] LIKE '{1}') ", col_statementFullTxt, MakeSqlSafe(filter.SqlTextExcludeFilter, true)));  
              
                Log.Debug( string.Format( "BuildFilterString: filter string = {0}", text.ToString() ) );
            }
            catch( Exception ex )
            {
                Log.Error( "Caught exception building filter string.", ex );
            }

            return text.ToString();
        }

        private void UpdateCategoryColors()
        {
            try
            {
                if( chart.Series == null || chart.Series.Count == 0 )
                    return;

                for( int i = 0; i < chart.Series.Count; i++ )
                {
                    if( categoryColors.ContainsKey( chart.Series[i].Text ) )
                        chart.Series[i].Color = categoryColors[chart.Series[i].Text];
                }
            }
            catch( Exception ex )
            {
                // nothing to do really
                Log.Error(ex );
            }
        }

        #endregion

        #region Event handlers

        private void ResourcesWaitStatsActive_Load(object sender, EventArgs e)
        {
            //breadCrumb1.AddCrumb( "Crumb 1" );
            //breadCrumb1.AddCrumb( "Crumb 2" );
            //breadCrumb1.AddCrumb( "Crumb 3" );
            //breadCrumb1.AddCrumb( "Crumb 4" );
            //breadCrumb1.AddCrumb( "Crumb 5" );

            // first breadcrumb is the server
            MonitoredSqlServerWrapper server = ApplicationModel.Default.ActiveInstances[this.instanceId];

            if( server != null )
                breadCrumb1.AddCrumb( server.InstanceName );            

            /*
             * 
             * chart data table will  
             * 
             * */

            /*
            chart.Series.Clear();

            chart.AxisX.Min = 0;
            chart.AxisX.Max = 100;
            chart.AxisX.Step = 20;

            chart.AxisX.Labels[0] = "Query 1";
            chart.AxisX.Labels[1] = "Query 2";
            chart.AxisX.Labels[2] = "Query 3";
            chart.AxisX.Labels[3] = "Query 4";
            chart.AxisX.Labels[4] = "Query 5";

            chart.AxisX.LabelValue = 20;
            */
        }

        private void BreadCrumb1_OnTrailChanged( object sender, BreadCrumbTrailChangedEventArgs e )
        {
            filters.Clear();

            foreach( Crumb crumb in e.Crumbs )
            {
                if( string.IsNullOrEmpty(crumb.Tag) )
                    continue;

                filters.Add( crumb.Tag, crumb.CrumbName );
            }

            UpdateTabs( e.Crumbs );

            if(chartMode == ChartMode.Timeline)
                initTimelineColumns = true;

            UpdateChart();

            if(chartMode == ChartMode.Timeline)
                initTimelineColumns = false;
        }

        private void breadCrumb1_SizeChanged( object sender, EventArgs e )
        {
            // move the top of the graph tabs to be just below the bread crumb
        }

        private void chart_MouseClick(object sender, ChartFX.WinForms.HitTestEventArgs e)
        {            
            if( e.HitType == HitType.Axis && e.Object is AxisX )
                HandleAxisLabelClick( e );

            if( e.HitType == HitType.LegendBox && e.Object is LegendBox )
                HandleLegendLabelClick( e );
        }

        private void HandleAxisLabelClick( ChartFX.WinForms.HitTestEventArgs e )
        {
            if( chartMode == ChartMode.Timeline )
                return;

            if( e.Button == MouseButtons.Right )
                HandleAxisLabelRightClick( e );
            else
                HandleAxisLabelLeftClick( e );   
        }

        private void HandleAxisLabelLeftClick( ChartFX.WinForms.HitTestEventArgs e )
        {
            if( GetNumTabsVisible() == 1 )
                return;

            // scale
            double x = e.Value;

            // find label value that is closest to clicked axis value -- the closest is the "clicked" label
            int labelindex = ( int )x;

            if (labelindex > chart.AxisX.Labels.Count)
                labelindex = chart.AxisX.Labels.Count - 1;   
         
            string filtervalue = chart.AxisX.Labels[labelindex];

            // now, add the clicked link to the filters list
            AddFilter( groupby, filtervalue );

            // pass the full text to the bread crumb (for popup)
            string fulltext = fullChartLabels.ContainsKey( filtervalue ) ? fullChartLabels[filtervalue] : filtervalue;

            // update breadcrumb
            breadCrumb1.AddCrumb( filtervalue, groupby, fulltext );

            // switch groupby back to statements
            groupby = col_waitType;

            // update tabs
            ultraTabStripControl1.Tabs[ultraTabStripControl1.SelectedTab.Index].Visible = false;
            SelectFirstVisibleTab();
            
            // update chart
            UpdateChart();
        }

        private void HandleAxisLabelRightClick( ChartFX.WinForms.HitTestEventArgs e )
        {
            // scale
            double x = e.Value;

            // find label value that is closest to clicked axis value -- the closest is the "clicked" label
            int labelindex = ( int )x;

            if (labelindex > chart.AxisX.Labels.Count)
                labelindex = chart.AxisX.Labels.Count - 1;   
         
            string label = chart.AxisX.Labels[labelindex];

            // need to know which full label to get from the dictionary
            fulllabeltext = label;
        }

        private void chart_GetAxisLabel( object sender, AxisLabelEventArgs e )
        {
            //if( e.Axis is AxisY )
            //    return;

            ////e.Text = MakeSafeChartLabel( e.Text );
        }        

        private int GetNumTabsVisible()
        {
            int n = 0;

            for( int i = 0; i < ultraTabStripControl1.Tabs.Count; i++ )
                n += ultraTabStripControl1.Tabs[i].Visible ? 1 : 0;

            return n;
        }

        private void SelectFirstVisibleTab()
        {
            for( int i = ultraTabStripControl1.Tabs.Count-1; i >= 0 ; i-- )
            {
                if( ultraTabStripControl1.Tabs[i].Visible )
                    ultraTabStripControl1.SelectedTab = ultraTabStripControl1.Tabs[i];
            }
        }

        private void HandleLegendLabelClick( ChartFX.WinForms.HitTestEventArgs e )
        {
            Console.WriteLine(e.Object);

            Dictionary<string, string> topics = new Dictionary<string, string>();

            topics.Add( "Backup",             HelpTopics.ResourcesQueryWaitsviewBackup );
            topics.Add( "I/O",                HelpTopics.ResourcesQueryWaitsviewIO );
            topics.Add( "Lock",               HelpTopics.ResourcesQueryWaitsviewLock );
            topics.Add( "Memory",             HelpTopics.ResourcesQueryWaitsviewMemory );
            topics.Add( "Non-I/O Page Latch", HelpTopics.ResourcesQueryWaitsviewNonIOPageLatch );
            topics.Add( "Non-Page Latch",     HelpTopics.ResourcesQueryWaitsviewNonPageLatch );
            topics.Add( "Other",              HelpTopics.ResourcesQueryWaitsviewOther );
            topics.Add( "Transaction Log",    HelpTopics.ResourcesQueryWaitsviewTransactionLog );

            try
            {
                if( chart.Series == null )
                    return;

                string topic = chart.Series[e.Series].Text;

                // show help topic
                if( topics.ContainsKey( topic ) )
                    ApplicationHelper.ShowHelpTopic( topics[topic]);
                else
                    ApplicationHelper.ShowHelpTopic( HelpTopics.ResourcesQueryWaitsView );
            }
            catch( Exception ex )
            {
                Log.Error( "Caught exception handling legend box label click.", ex );
            }
        }

        private void AddFilter( string name, string value )
        {
            if( filters.ContainsKey( name ) )
                filters[name] = value;
            else
                filters.Add( name, value );
        }

        private void RemoveFilter( string name )
        {
            if( filters.ContainsKey( name ) )
                filters.Remove( name );
        }

        private void UpdateTabs( IList<Crumb> crumbs )
        {
            string filter;
            bool found;

            // iterate through the tabs, and check to see if there is a crumb for it.  if so, we don't show that tab, otherwise, we do
            foreach( UltraTab tab in ultraTabStripControl1.Tabs )
            {
                filter = tab.Tag as string;
                found  = false;

                foreach( Crumb crumb in crumbs )
                {
                    if( crumb.Tag == filter )
                    {
                        found = true;
                        break;
                    }
                }

                tab.Visible = !found;
            }
        }

        private void ultraTabStripControl1_SelectedTabChanged( object sender, Infragistics.Win.UltraWinTabControl.SelectedTabChangedEventArgs e )
        {
            // change the group by
            switch( e.Tab.Text )
            {
                case "Databases":

                    groupby = col_databaseName;                    
                    break;

                case "Clients":

                    groupby = col_hostName;
                    break;

                case "Users":

                    groupby = col_loginName;
                    break;

                case "Applications":

                    groupby = col_programName;
                    break;

                case "Sessions":

                    groupby = col_sessionId;
                    break;

                case "Statements":

                    groupby = col_statementTxt;
                    break;

                case "Waits":

                    groupby = col_waitType;
                    break;
            }

            initTimelineColumns = true;
            UpdateChart();
            initTimelineColumns = false;
        }
        
        private void chart_MouseMove( object sender, HitTestEventArgs e )
        {
            //if( chartMode == ChartMode.Timeline )
            //    return;

            try
            {
                HitTestEventArgs e2 = chart.HitTest( e.X, e.Y );
                overAxisX = e2.HitType == HitType.Axis && e2.Object is AxisX;

                overSeries = e2.Series;

                if (chartMode == ChartMode.Duration)
                {
                    if (overAxisX)
                        Cursor = Cursors.Hand;
                    else if (!overAxisX)
                        Cursor = Cursors.Default;
                }
                else if (Cursor == Cursors.Hand)
                    Cursor = Cursors.Default;
            }
            catch( Exception)
            {
                // ?
            }
        }

        private void chart_GetTip( object sender, GetTipEventArgs e )
        {
            if( e.HitType != HitType.Axis || !(e.Object is AxisX) )
                return;

            e.Text = "";
        }

        #endregion

        #region Toolbar handlers

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            if (contextMenuSelectedChart == null)
                return;

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
                case "viewFullGroupByTextButton":
                    ViewFullGroupByText();
                    break;
                case "showQueryHistoryButton":
                    ShowQueryHistory();
                    break;
            }

            contextMenuSelectedChart = null;
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {            
            if (e.Tool.Key != "chartContextMenu")
                return;
            
            // if we are over the axis, and are in duration mode, show only the view text option
            toolbarsManager.Tools["toggleChartToolbarButton"].SharedProps.Visible  = !overAxisX;
            toolbarsManager.Tools["printChartButton"].SharedProps.Visible          = !overAxisX;
            toolbarsManager.Tools["exportChartDataButton"].SharedProps.Visible     = !overAxisX;
            toolbarsManager.Tools["exportChartImageButton"].SharedProps.Visible    = !overAxisX;
            toolbarsManager.Tools["viewFullGroupByTextButton"].SharedProps.Visible = overAxisX;
            toolbarsManager.Tools["showQueryHistoryButton"].SharedProps.Visible    = overAxisX && groupby == col_statementTxt;

            if( chartMode == ChartMode.Timeline )
            {
                toolbarsManager.Tools["viewFullGroupByTextButton"].SharedProps.Visible = overSeries >= 0;
                toolbarsManager.Tools["showQueryHistoryButton"].SharedProps.Visible    = overSeries >= 0 && groupby == col_statementTxt;
            }

            Chart chart = (Chart)e.SourceControl;
            ((StateButtonTool)((PopupMenuTool)e.Tool).Tools["toggleChartToolbarButton"]).InitializeChecked(chart.ToolBar.Visible);
            contextMenuSelectedChart = chart;
        }

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {
            chart.ToolBar.Visible = Visible;
        }

        private string GetPrintTitle()
        {
            string printTitle = string.Empty;
            if (ultraTabStripControl1 != null && ultraTabStripControl1.SelectedTab != null)
            {
                printTitle = ultraTabStripControl1.SelectedTab.Text;
            }
            return printTitle;
        }

        private void PrintChart(Chart chart)
        {
            string title = string.Empty;

            if (chart.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)chart.Tag).Text;
             }
            else
            {
                title = GetPrintTitle(); 
            }

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
            {
                title = ((ToolStripItem) chart.Tag).Text;
            }
            else
            {
                title = GetPrintTitle();
            }

            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
        }

        private void ViewFullGroupByText()
        {
            string text = string.Empty;

            if( chartMode == ChartMode.Duration )
            {
                if( fulllabeltext == null )
                    return;

                text = fulllabeltext;    
            
                if( fullChartLabels.ContainsKey( fulllabeltext ) )
                    text = fullChartLabels[fulllabeltext];
            }
            else
            {
                if( overSeries < 0 )
                    return;

                if( chart.Series == null )
                    return;

                fulllabeltext = chart.Series[overSeries].Text;

                if( fullChartLabels.ContainsKey( fulllabeltext ) )
                    text = fullChartLabels[fulllabeltext];
                else
                    text = fulllabeltext;
            }

            text = Regex.Replace( text, "\t", " " );

            if( !string.IsNullOrEmpty( text ) )
                ApplicationMessageBox.ShowMessage( text );
        }

        private void ShowQueryHistory()
        {
            string hash = "";

            if (chartMode == ChartMode.Duration)
            {
                if (fulllabeltext == null)
                {
                    Log.Debug("ShowQueryHistory: fulllabeltext was null.  Cannot navigate to Query History.");
                    return;
                }                
            }
            else
            {
                if (chart.Series == null)
                {
                    Log.Debug("ShowQueryHistory: chart.Series was null.  Cannot navigate to Query History.");
                    return;
                }

                fulllabeltext = chart.Series[overSeries].Text;
            }

            if (statementHashes.ContainsKey(fulllabeltext))
                hash = statementHashes[fulllabeltext];

            ApplicationController.Default.ShowServerView(this.instanceId, ServerViews.QueriesHistory, hash);

        }

        #endregion                                

        #region Operational status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image   = icon;
            operationalStatusLabel.Text    = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            ApplicationController.Default.SetActiveViewToRealTimeMode();
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

            SwitchToRealTimeMode();
        }


        #endregion  

        /// <summary>
        /// group by the selected characteristic
        /// for each time that is present.
        /// Only used in timeline view
        /// </summary>
        /// <param name="targetTable">passed in empty. table ultimately containing grouped data</param>
        private void SumTimeline(DataTable targetTable)
        {
            //DateTime start;
            //DateTime end;

            //gets the data in almost raw form.
            //some small changes to the timestamp have taken place but there is a 1:1 relationship to raw rows still
            DataTable data = GetCurrentDataTable();  

            // remove sort/filter
            targetTable.DefaultView.Sort = "";
            targetTable.DefaultView.RowFilter = "";

            // clear out the data
            targetTable.Clear();

            // update the columns if necessary
            if( targetTable.Columns[0].DataType != typeof( DateTime ) || initTimelineColumns )
            {
                DataTable oldtable = new DataTable();

                // Need to re sum and re sort the data
                // data contains the latest data, old table contains the aggregated data
                Sum( data, oldtable );
                
                //Sorted, deNulled and top N'ed
                SortChartData( oldtable );

                targetTable.Columns.Clear();

                // add timestamp column
                targetTable.Columns.Add( "timestamp", typeof( DateTime ) );

                // get the new columns (one for each groupby value)
                for( int i = 0; i < oldtable.DefaultView.Count; i++ )
                {
                    DataColumn column = new DataColumn((string)oldtable.DefaultView[i][groupby], typeof(decimal));//SQLdm 10.0 (Gaurav Karwal): changed the datatype to decimal
                    column.AllowDBNull = true;
                    column.DefaultValue = 0;
                    if (string.IsNullOrEmpty(column.Caption))
                    {
                        continue;
                    }
                    targetTable.Columns.Add( column );
                }
            }
            // Targettable now has all of the required columns, one column per category

            // get a list of all the unique timestamps
            DataTable timestampsTable = data.DefaultView.ToTable(true, new string[] { "timestamp" });            
            DateTime  timestamp;

            //data.PrimaryKey = new DataColumn[] { data.Columns[groupby] };
            //DataTable primary = data.DefaultView.RowFilter( string.Format("Convert(timestamp, 'System.String') = '{0}'", timestamp));
            DateTime donePrimary;
            DateTime startPrimary;
            TimeSpan primaryDuration = new TimeSpan(0);

            //start = DateTime.Now;
            // for each timestamp, for each column, we sum up the totals for that time
            foreach (DataRow timerow in timestampsTable.Rows)
            {
                DataRow r = targetTable.NewRow();

                timestamp = (DateTime)timerow["timestamp"];
                r["timestamp"] = timestamp;
                startPrimary = DateTime.Now;
                //SQLDM - 19237 - Here CultureInfo.InvariantCulture on view filter is not required as we are trying to compare the string format dates here.
                var primary = new DataView(data, string.Format("Convert(timestamp, 'System.String') = '{0}'", timestamp), "timestamp Asc", DataViewRowState.CurrentRows);
                donePrimary = DateTime.Now;
                primaryDuration = primaryDuration + (donePrimary - startPrimary);

                for (int i = 1; i < targetTable.Columns.Count; i++)
                {
                    //DataTable secondary = primary.ToTable().DefaultView.FindRows();
                    object total = primary.ToTable().Compute("Sum(duration)",
                                                         string.Format("[{0}] = '{1}'", groupby, MakeSqlSafe(targetTable.Columns[i].Caption, false)));
//                    object total = primary.ToTable().Compute("Sum(duration)",
//string.Format("[{0}] = '{1}'", groupby, targetTable.Columns[i].Caption.Replace("'","''")));

                    //sum the column
                    //object total = data.Compute("Sum(duration)", string.Format("[{0}] = '{1}' and Convert(timestamp, 'System.String') = '{2}'", groupby, MakeSqlSafe( targetTable.Columns[i].Caption ), timestamp));

                    if (total is DBNull || total == null)
                        continue;

                    r[targetTable.Columns[i].Caption] = (decimal)total;  //SQLdm 10.0 (Gaurav Karwal): changing to decimal to handle values coming from procedure
                }

                targetTable.Rows.Add(r);
            }
            //end = DateTime.Now;
            
            //Debug.Print("computesum(duration): start - {0}, end - {1}, duration - {2}", start.ToString(), end.ToString(), (end - start));
            //Debug.Print(string.Format("Primary took a total of {0} ms", primaryDuration.ToString()));

            chart.DataSourceSettings.Fields.Clear();
            chart.DataSourceSettings.Fields.Add(new FieldMap("timestamp", FieldUsage.Label));

            for (int i = 1; i < targetTable.Columns.Count; i++)
                chart.DataSourceSettings.Fields.Add(new FieldMap(targetTable.Columns[i].Caption, FieldUsage.Value));
        }

        private void queryWaitsByDurationToolStripMenuItem_Click( object sender, EventArgs e )
        {
            toolStripSplitButton1.Text = queryWaitsByDurationToolStripMenuItem.Text;
            this.toolStripSplitButton1.ToolTipText = queryWaitsByDurationToolStripMenuItem.Text;
            queryWaitsOverTimeToolStripMenuItem.Checked = false;            
            chartMode = ChartMode.Duration;
            HookUnhookDrilldownHandlers(drilldownProperties, false); // SqlDM 10.2(Anshul Aggarwal) - Remove drilldown functionality from duration chart
            UpdateChartMode();
        }

        private void queryWaitsOverTimeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            toolStripSplitButton1.Text = queryWaitsOverTimeToolStripMenuItem.Text;
            this.toolStripSplitButton1.ToolTipText = queryWaitsOverTimeToolStripMenuItem.Text;
            queryWaitsByDurationToolStripMenuItem.Checked = false;
            chartMode = ChartMode.Timeline;
            HookUnhookDrilldownHandlers(drilldownProperties, true); // SqlDM 10.2(Anshul Aggarwal) - Add drilldown functionality to timeline chart
            UpdateChartMode();
        }

        private void UpdateChartMode()
        {
            UpdateChartAxisStyles();
            UpdateChart();
        }

        /// <summary>
        /// Updates chart axis styles.
        /// </summary>
        private void UpdateChartAxisStyles()
        {
            if (chartMode == ChartMode.Duration)
            {
                chart.AxisX.LabelsFormat.Format = AxisFormat.None;
                chart.Gallery                   = Gallery.Gantt;
                chart.AxisX.Step                = 1;
                chart.AxisX.Staggered           = false;
                chart.AxisX.LabelAngle          = 0;

                chart.AxisX.Font      = new System.Drawing.Font( "Microsoft Sans Serif", 8F, ( ( System.Drawing.FontStyle )( ( System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline ) ) ) );
                chart.AxisX.TextColor = System.Drawing.Color.FromArgb( ( ( int )( ( ( byte )( 45 ) ) ) ), ( ( int )( ( ( byte )( 118 ) ) ) ), ( ( int )( ( ( byte )( 227 ) ) ) ) );

                //InitializeChartFieldMap();                                             
            }
            else
            {                
                chart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat;
                chart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormatWithYear;
                chart.AllSeries.Stacked         = Stacked.Normal;
                chart.Gallery                   = Gallery.Bar;
                chart.AxisX.Step                = -1;
                chart.AxisX.LabelAngle          = 30;                
                chart.AxisX.Staggered           = false;

                chart.AxisX.Font      = new System.Drawing.Font( "Microsoft Sans Serif", 8F, ( ( System.Drawing.FontStyle )( ( System.Drawing.FontStyle.Regular ) ) ) );
                chart.AxisX.TextColor = Color.Black;
            }

        }
        /// <summary>
        /// Get the Historical or real-time snapshot for the selected time
        /// </summary>
        /// <returns></returns>
        private DataTable GetCurrentDataTable()
        {
            if( HistoricalSnapshotDateTime != null )
                return chartHistoricalDataTable;
            else
                return chartRealTimeDataTable;
        }

        public void ShowFilter()
        {
            QueryWaitsFilter newfilter = new QueryWaitsFilter();
            filter.MaxRows = maxDataRows;

            newfilter.UpdateValues( filter );

            GenericFilterDialog dialog = new GenericFilterDialog( newfilter );
            DialogResult       result  = dialog.ShowDialog( this );

            if( result == DialogResult.OK )
            {
                maxDataRows = newfilter.MaxRows;

                filter.UpdateValues( newfilter );

                if (HistoricalSnapshotDateTime != null)
                    UpdateChart();

                ApplicationController.Default.ActiveView.CancelRefresh();
                ApplicationController.Default.RefreshActiveView();
            }
        }
        public bool ConfigurationDialogVisible { get; private set; }
        public void ShowConfigureDialog()
        {
            using( MonitoredSqlServerInstancePropertiesDialog dialog = new MonitoredSqlServerInstancePropertiesDialog( this.instanceId ) )
            {
                dialog.SelectedPropertyPage = MonitoredSqlServerInstancePropertiesDialogPropertyPages.WaitStatistics;
                try
                {
                    ConfigurationDialogVisible = true;
                    dialog.ShowDialog(this);
                }
                catch(Exception ex)
                {
                    Log.Error(ex);
                }
                ConfigurationDialogVisible = false;
            }
        }

        private void filter_OnChanged( object sender, EventArgs e )
        {
            // if the updates result in a non default filter, then we need a new message for the chart
            if( filter.HasDefaultValues() )
                chart.SetMessageText( "NoData", "Please Wait" );
            else
                chart.SetMessageText( "NoData", "No data matches filter" );
        }

        private void chart_MouseDown(object sender, HitTestEventArgs e)
        {
            if (e.Button == MouseButtons.Left) return;

            if (e.HitType == HitType.Axis && e.Object is AxisX)
                HandleAxisLabelClick(e);
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
            updateColor();
        }

        void updateColor()
        {
            if(chart != null)
                chart.ForeColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ChartForeColor) : Color.Black;
        }

        // START : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
        /// <summary>
        /// SQLdm 10.4 (Charles Schultz) - Removal of Web Console Advertisements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        /*
        private void advancedQueryViewLabel_MouseEnter(object sender, EventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(255, 189, 105);
            advancedQueryViewImage.BackColor = Color.FromArgb(255, 189, 105);
        }

        private void advancedQueryViewLabel_MouseLeave(object sender, EventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(135, 206, 250);
            advancedQueryViewImage.BackColor = Color.FromArgb(135, 206, 250);
        }

        private void advancedQueryViewLabel_MouseDown(object sender, MouseEventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.White;
            advancedQueryViewLabel.BackColor = Color.FromArgb(251, 140, 60);
            advancedQueryViewImage.BackColor = Color.FromArgb(251, 140, 60);
        }

        private void advancedQueryViewLabel_MouseUp(object sender, MouseEventArgs e)
        {
            advancedQueryViewLabel.ForeColor = Color.Black;
            advancedQueryViewLabel.BackColor = Color.FromArgb(255, 189, 105);
            advancedQueryViewImage.BackColor = Color.FromArgb(255, 189, 105);

            IManagementService managementService =
                ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            string cwfWebUrl = managementService.GetCWFWebURL();

            //string cwfWebUrl = "http://localhost:9279/webui";  // TODO : replace this with mngt service function to read cwf web url

            if (cwfWebUrl != null)
            {
                try
                {
                    System.Diagnostics.Process.Start(cwfWebUrl);
                }
                catch (Exception exception)
                {
                    Log.Info("Not able to open URL : " + cwfWebUrl + " => " + exception.InnerException);
                }
            }
            else
            {
                Log.Info("CWF Web URL is null !!");
            }
        }
        // END : SQLdm 9.0 (Abhishek Joshi) -Advertise Web Console Query Views in the SQLdm Desktop Client
        //START : SQLdm 9.0 (vineet kumar) -- Fixing DE44333
        private void UpdateWebUiStatus()
        {
            IManagementService managementService =
               ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
            string cwfWebUrl = managementService.GetCWFWebURL();
            if (string.IsNullOrEmpty(cwfWebUrl))
            {
                advancedQueryViewPanel.Visible = false;
            }
            else
                advancedQueryViewPanel.Visible = true;
        }
        //END : SQLdm 9.0 (vineet kumar) -- Fixing DE44333
        */
    }
}
