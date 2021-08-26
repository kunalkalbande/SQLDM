using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Objects;
using ChartFX.WinForms;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Configuration;
using System.Threading;
using Idera.SQLdm.Common.Objects;
using System.Diagnostics;
using Idera.SQLdm.Common.UI.Dialogs;
using Infragistics.Win;
using Infragistics.Win.UltraWinGrid;
using ColumnHeader = Infragistics.Win.UltraWinGrid.ColumnHeader;
using Idera.SQLdm.DesktopClient.Controls;
using System.Globalization;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources
{
    internal partial class ResourcesWaitStats : ServerBaseView
    {
        #region Fields/Types

        private enum CategoryMode
        {
            All,
            Single
        }

        private enum WaitSumMode
        {
            Total,
            Signal,
            Resource
        }

        private static readonly object updateLock = new object();

        private bool chartRealTimeDataTablePrePopulated = false;
        private long otherInitialized = 0;
        private long ctorInitialized = 0;
        private readonly ServerSummaryHistoryData historyData;
        private Chart contextMenuSelectedChart = null;
        private Panel maxPanel;
        private Control focused = null;
        private DataTable chartDataTable;
        private DataTable chartRealTimeDataTable;
        private DataTable chartHistoricalDataTable;
        private DataTable gridWaitTypesDataTable;
        private WaitStatisticsSnapshot currentSnapshot = null;
        private Exception historyModeLoadError = null;
        private WaitSumMode waitSumMode;
        private string[] waitsSumModeMenuStrings;
        private UltraGridColumn selectedColumn = null;

        //private GridSettings lastMainGridSettings = null;

        private const string col_timestamp = "Timestamp";
        private const string col_ishistorical = "IsHistorical";
        private const string col_categoryid = "Category ID";
        private const string col_categoryName = "Category";
        private const string col_type = "Type";
        private const string col_definition = "Definition";
        private const string col_waitingTasksCountTotal = "Waiting Tasks Count Total";
        private const string col_waitTimeTotal = "Wait Time Total";
        private const string col_maxWaitTime = "Max Wait Time";
        private const string col_signalWaitTimeTotal = "Signal Wait Time Total";
        private const string col_resourceWaitTimeTotal = "Resource Wait Time Total";
        private const string col_waitingTasksCountDelta = "Waiting Tasks Count Delta";
        private const string col_waitTimeDelta = "Wait Time Delta";
        private const string col_signalWaitTimeDelta = "Signal Wait Time Delta";
        private const string col_resourceWaitTimeDelta = "Resource Wait Time Delta";
        private const string col_timeDelta = "Time Delta";
        private const string col_waitingTasksPerSecond = "Waiting Tasks Per Second";
        private const string col_totalWaitMillisecondsPerSecond = "Total Wait Milliseconds Per Second";
        private const string col_signalWaitMillisecondsPerSecond = "Signal Wait Milliseconds Per Second";
        private const string col_resourceWaitMillisecondsPerSecond = "Resource Wait Milliseconds Per Second";

        private Dictionary<string, WaitTypeInfo> waitTypesLookup;
                                                 // key-> wait_type, values: [category id, category name, wait defintion]

        private List<string> waitCategories;
        private bool waitTypesLookupInitialized;
        private DateTime? latestSnapshotDateTime;
        private string currentCategoryFilter;
        private bool updatingmenu;
        private OrderedSet<string> topXWaits;
        private DataGridViewTextBoxColumn textColumn;
        private Label Sql2kWarningLabel;

        private Color[] defaultLegendColors = new Color[]
            {
                Color.FromArgb(80, 124, 209),
                Color.FromArgb(57, 160, 48),
                Color.FromArgb(232, 155, 0),
                Color.FromArgb(229, 0, 145),
                Color.FromArgb(101, 0, 168),
                Color.FromArgb(165, 229, 122),
                Color.FromArgb(0, 160, 196),
                Color.FromArgb(28, 94, 85),
                Color.FromArgb(153, 0, 0),
                Color.FromArgb(255, 163, 142),
                Color.FromArgb(11, 37, 133),
                Color.FromArgb(197, 174, 250),
                Color.FromArgb(255, 200, 0),
                Color.FromArgb(173, 214, 255),
                Color.FromArgb(10, 50, 196),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(80, 124, 209),
                Color.FromArgb(57, 160, 48),
                Color.FromArgb(232, 155, 0),
                Color.FromArgb(229, 0, 145)
            };


        public event EventHandler GridGroupByBoxVisibleChanged;

        #endregion

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

        public bool GridGroupByBoxVisible
        {
            get { return !waitTypesGrid.DisplayLayout.GroupByBox.Hidden; }
            set
            {
                waitTypesGrid.DisplayLayout.GroupByBox.Hidden = !value;

                if (GridGroupByBoxVisibleChanged != null)
                {
                    GridGroupByBoxVisibleChanged(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        #region Constructor

        public ResourcesWaitStats(int instanceId, ServerSummaryHistoryData historyData) : base(instanceId)
        {
            this.historyData  = historyData;
            this.waitSumMode  = WaitSumMode.Resource;
            this.latestSnapshotDateTime = DateTime.MinValue;
            this.currentCategoryFilter = "all";
            this.topXWaits = new OrderedSet<string>();

            waitTypesLookup = ApplicationModel.Default.WaitTypes;
            waitCategories  = new List<string>();

            updatingmenu = true;
            InitializeComponent();
            //SQLDM - 26866 - Code changes to select Resource Waits as default option
            resourceToolStripMenuItem.Checked = true;
            resourceToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked; 

            ChartFxExtensions.SetContextMenu(waitsChart, toolbarsManager);

            updatingmenu = false;  
          
            waitTypesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.None;// Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;            

            this.waitsSumModeMenuStrings = new string[]
            {
                totalWaitTimeToolStripMenuItem.Text,
                signalToolStripMenuItem.Text,
                resourceToolStripMenuItem.Text                
            };

            maxPanel          = new Panel();
            maxPanel.Size     = tableLayoutPanel1.Size;
            maxPanel.Location = tableLayoutPanel1.Location;
            maxPanel.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right; 

            this.Controls.Add(maxPanel);
            this.Controls.SetChildIndex( maxPanel, 0 );
            maxPanel.SendToBack();

            this.Sql2kWarningLabel           = new Label();
            this.Sql2kWarningLabel.Dock      = DockStyle.Fill;
            this.Sql2kWarningLabel.Location  = new Point( 0, 0 );
            this.Sql2kWarningLabel.Name      = "Sql2kWarningLabel";
            this.Sql2kWarningLabel.Size      = new Size( 100, 100 );
            this.Sql2kWarningLabel.Text      = "This view is only supported on SQL Server 2005 or higher.";
            this.Sql2kWarningLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.Sql2kWarningLabel.Visible   = false;
            this.Controls.Add( this.Sql2kWarningLabel );
            this.Sql2kWarningLabel.BringToFront();

            this.textColumn = new DataGridViewTextBoxColumn();
            this.textColumn.HeaderText   = "Column3";
            this.textColumn.Name         = "textnolink";
            this.textColumn.ReadOnly     = true;
            this.textColumn.Resizable    = System.Windows.Forms.DataGridViewTriState.False;
            this.textColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            this.categoryLegend.Columns[0].Resizable = DataGridViewTriState.False;

            Settings.Default.PropertyChanged += Settings_PropertyChanged;

            InitializeDefaultSeriesColors();

            InitializeDataSources();     
            InitializeChart();                   

            Interlocked.Increment( ref ctorInitialized );
            AdaptFontSize();
            updateColor();
            SetGridTheme();

            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }        

        #endregion        

        #region Methods

        #region Initialization

        private void Initialize()
        {
            if( InvokeRequired )
            {
                Invoke( new MethodInvoker( Initialize ) );
                return;
            }

            InitializeWaitTypesGrid();
            InitializeChartDataTable();            
            IntializeMenu();            

            Interlocked.Increment( ref otherInitialized );
        }

        private void InitializeDefaultSeriesColors()
        {
            //defaultLegendColors = new Color[20];
            //waitsChart.RandomData.Points = 0;
            //waitsChart.RandomData.Series = 20;

            //for (int i = 0; i < defaultLegendColors.Length; i++)
            //{
            //    if (i >= waitsChart.Series.Count)
            //    {
            //        waitsChart.Series.Add(new SeriesAttributes());
            //    }
            //    defaultLegendColors[i] = waitsChart.Series[i].Color;
            //}

            //waitsChart.RandomData.Reset();
            //waitsChart.Series.Clear();
        }

        private void InitializeChart()
        {            
            waitsChart.DataSource = chartDataTable;

            waitsChart.Tag = "WaitsChart";
            waitsChart.Printer.Orientation = PageOrientation.Landscape;
            waitsChart.Printer.Compress    = true;
            waitsChart.Printer.ForceColors = true;
            waitsChart.Printer.Document.DocumentName = "Server Waits Statistics Chart";
            waitsChart.ToolTipFormat = "%s\n%v ms/s\n%x";

            waitsChart.AxisX.LabelsFormat.Format = ChartHelper.TimeChartAxisFormat; // SqlDM 10.2 (Anshul Aggarwal) : New History Browser
            waitsChart.AxisX.LabelsFormat.CustomFormat = ChartHelper.TimeChartCustomFormat;
            waitsChart.AxisY.Title.Text = "Milliseconds / Second";
            waitsChart.AxisY.DataFormat.Decimals = 2;
            waitsChart.Gallery = Gallery.Lines;

            waitsChart.SetMessageText( "NoData", "Please Wait" );
            InitalizeDrilldown(waitsChart);  //SQLdm 10.2 (Anshul Aggarwal) : Chart Drilldown functionality
        }

        private void InitializeDataSources()
        {      
            chartDataTable         = new DataTable();
            chartRealTimeDataTable = new DataTable();    
        
            // add placeholder values for data table
            chartDataTable.Columns.Add( col_timestamp, typeof( DateTime ) );

            // add columns to hold Wait type values
            chartRealTimeDataTable.Columns.Add( col_timestamp,              typeof( DateTime ) );
         
            chartRealTimeDataTable.Columns.Add( col_categoryid,             typeof( int ) );           
            chartRealTimeDataTable.Columns.Add( col_categoryName,           typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_type,                   typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_definition,             typeof( string ) );
            chartRealTimeDataTable.Columns.Add( col_waitingTasksCountTotal, typeof( long ) );
            chartRealTimeDataTable.Columns.Add( col_waitTimeTotal,          typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_maxWaitTime,            typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_signalWaitTimeTotal,    typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_resourceWaitTimeTotal,  typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_waitingTasksCountDelta, typeof( long ) );
            chartRealTimeDataTable.Columns.Add( col_waitTimeDelta,          typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_signalWaitTimeDelta,    typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_resourceWaitTimeDelta,  typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_timeDelta,              typeof( TimeSpan ) );
            chartRealTimeDataTable.Columns.Add( col_waitingTasksPerSecond,             typeof( double ) );
            chartRealTimeDataTable.Columns.Add( col_totalWaitMillisecondsPerSecond,    typeof( double ) );
            chartRealTimeDataTable.Columns.Add( col_signalWaitMillisecondsPerSecond,   typeof( double ) );
            chartRealTimeDataTable.Columns.Add( col_resourceWaitMillisecondsPerSecond, typeof( double ) );

            chartRealTimeDataTable.Columns[col_resourceWaitMillisecondsPerSecond].AllowDBNull = true;
            chartRealTimeDataTable.Columns.Add(col_ishistorical, typeof(bool));

            chartHistoricalDataTable = chartRealTimeDataTable.Clone();
        }

        private void InitializeChartDataTable()
        {       
            if( !waitTypesLookupInitialized )
                return;

            chartDataTable.Columns.Clear();

            chartDataTable.Columns.Add( col_timestamp, typeof( DateTime ) );

            if( currentCategoryFilter == "all" )
            {
                foreach( string category in waitCategories )
                    chartDataTable.Columns.Add( category, typeof( double ) );
            }
            else
            {
                foreach( string wait in topXWaits )
                    chartDataTable.Columns.Add( wait, typeof( double ) );
            }
        }

        private void IntializeMenu()
        {    
            if( !waitTypesLookupInitialized )
                return;

            if( InvokeRequired )
            {
                this.Invoke( new MethodInvoker( IntializeMenu ) );
                return;
            }

            waitsByCategoryToolStripMenuItem.DropDownItems.Clear();

            // loop through and get the categories only (a menu item for each)
            foreach( string category in waitCategories )
            {                
                ToolStripMenuItem menuItem = new ToolStripMenuItem();

                menuItem.CheckedChanged += new EventHandler( CategoryMenuItem_CheckedChanged );
                menuItem.CheckOnClick    = true;
                menuItem.Text            = category;

                waitsByCategoryToolStripMenuItem.DropDownItems.Add( menuItem );  
            }                        

            UpdateChartLegend();
            UpdateChartFieldMap();
        }

        private void UpdateChartFieldMap()
        {       
            if( InvokeRequired )
            {
                Invoke( new MethodInvoker( UpdateChartFieldMap ) );
                return;
            }

            if( !waitTypesLookupInitialized )
                return;            

            lock( updateLock )
            {
                InitializeChartDataTable();

                waitsChart.DataSourceSettings.Fields.Clear();

                List<FieldMap> maps = new List<FieldMap>();

                // setup the x axis value (timestamp)
                maps.Add( new FieldMap( col_timestamp, FieldUsage.XValue ) );

                if( currentCategoryFilter == "all" )
                {
                    // setup y axis values
                    foreach( string category in waitCategories )
                    {
                        FieldMap map = new FieldMap( category, FieldUsage.Value );
                        maps.Add( map );
                    }                    
                }
                else
                {                    
                    // setup y axis values
                    foreach( string wait in topXWaits )
                    {
                        FieldMap map = new FieldMap( wait, FieldUsage.Value );
                        maps.Add( map );
                    }
                }

                waitsChart.DataSourceSettings.Fields.AddRange( maps.ToArray() );
                waitsChart.DataSourceSettings.ReloadData();
            }
        }

        private void UpdateChartLegend()
        {  
            if( !waitTypesLookupInitialized || categoryLegend.ColumnCount < 2)
                return;

            int x = 0;

            // update the legend with the list of categories or top x waits for the given category
            if( currentCategoryFilter == "all" )
            {
                lblNoWaitData.Visible = false;

                // list all categories
                categoryLegend.Rows.Clear();
                categoryLegend.Columns.RemoveAt( 1 );
                categoryLegend.Columns.Add( this.text );

                int i, j = 0;

                foreach( string category in waitCategories )
                {
                    i = categoryLegend.Rows.Add( "", category );
                    j = categoryLegend.Rows.Add( "", "" ); // spacer

                    categoryLegend.Rows[i].Tag = x;
                    categoryLegend[0, i].Style.BackColor = GetColor( x++ ); // get colors from ????
                    categoryLegend.Rows[j].Height = 2; // spacer height                    
                }

                categoryLegend.ClearSelection();
            }
            else
            {                
                // add top x waits to the topXWaits set
                lock(updateLock)
                {
                    UpdateTopXWaits();
                }
                // loop through top x waits, and show them in the legend
                categoryLegend.Rows.Clear();
                categoryLegend.Columns.RemoveAt( 1 );
                categoryLegend.Columns.Add( this.textColumn );

                int i, j = 0;

                foreach( string wait in topXWaits )
                {
                    i = categoryLegend.Rows.Add( "", wait );
                    j = categoryLegend.Rows.Add( "", "" ); // spacer

                    categoryLegend.Rows[i].Tag = x;
                    categoryLegend[0, i].Style.BackColor = GetColor(x++); // get colors from ????
                    categoryLegend.Rows[j].Height = 2; // spacer height
                }

                categoryLegend.ClearSelection();

                lblNoWaitData.Visible = topXWaits.Count == 0;
            }
        }

        private void UpdateCategoryLegendColors()
        {  
            if( !waitTypesLookupInitialized )
                return;

            int x = 0;
            for( int i = 0; i < categoryLegend.Rows.Count; i++ )
            {
                if( (string)categoryLegend[1, i].Value == "" )
                    continue;

                categoryLegend[0, i].Style.BackColor = GetColor( x++ );
            }
        }

        private void InitializeWaitTypesGrid()
        {   
            if( !waitTypesLookupInitialized )
                return;

            gridWaitTypesDataTable = new DataTable();

            DataColumn c1 = new DataColumn( "Category",    typeof(string) );
            DataColumn c2 = new DataColumn( "Type",        typeof(string) );
            DataColumn c3 = new DataColumn( "Wait",        typeof(double) );
            DataColumn c4 = new DataColumn( "Total Wait",  typeof(double) );            
            DataColumn c5 = new DataColumn( "Description", typeof(string) );
            DataColumn c6 = new DataColumn( "Help",        typeof(string) );

            gridWaitTypesDataTable.Columns.Add( c1 );
            gridWaitTypesDataTable.Columns.Add( c2 );
            gridWaitTypesDataTable.Columns.Add( c3 );
            gridWaitTypesDataTable.Columns.Add( c4 );
            gridWaitTypesDataTable.Columns.Add( c5 );
            gridWaitTypesDataTable.Columns.Add( c6 );

            if( waitTypesLookup != null )
            {
                string helplink = null;

                foreach( string waittype in waitTypesLookup.Keys )
                {
                    DataRow row = gridWaitTypesDataTable.NewRow();   

                    row["Category"]    = waitTypesLookup[waittype].CategoryName;
                    row["Type"]        = waittype;
                    row["Wait"]        = 0;
                    row["Total Wait"]  = 0;
                    row["Description"] = waitTypesLookup[waittype].Description;

                    helplink = waitTypesLookup[waittype].HelpLink;
                    if(!string.IsNullOrEmpty(helplink))
                        row["Help"] = "Details";

                    gridWaitTypesDataTable.Rows.Add( row );
                }                
            }

            waitTypesGrid.DataSource = gridWaitTypesDataTable;
            waitTypesGrid.DisplayLayout.Bands[0].Columns[2].Format = "#######0.#####";
            waitTypesGrid.DisplayLayout.Bands[0].Columns[3].Format = "#######0.#####";
            waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Add( "Total Wait", true ); 

            UpdateGridColumns();
        }

        private void UpdateWaitTypesGrid()
        {     
            if( !waitTypesLookupInitialized )
                return;

            if( currentCategoryFilter == "all" )
            {
                waitTypesGrid.DataSource = gridWaitTypesDataTable;
            }
            else
            {
                DataView v = new DataView( gridWaitTypesDataTable );

                v.RowFilter = "Category = '" + currentCategoryFilter +"'";

                waitTypesGrid.DataSource = v;
            }
        }

        private void UpdateTopXWaits()
        {  
            Dictionary<string, double?> data = GetWaitValues(true);
            OrderedSet<Pair<double, string>> orderedWaits = new OrderedSet<Pair<double, string>>(new Comparison<Pair<double,string>>(delegate(Pair<double, string> a, Pair<double, string> b)
            {
                return -a.First.CompareTo(b.First);
            } ) );

            foreach( string wait in data.Keys )
            {
                if(string.IsNullOrEmpty(wait) || !waitTypesLookup.ContainsKey(wait)) 
                    continue;

                if(waitTypesLookup[wait].CategoryName != currentCategoryFilter)
                    continue;

                if( !data.ContainsKey( wait ) || !data[wait].HasValue || data[wait].Value == 0 )
                    continue;

                orderedWaits.Add( new Pair<double, string>( data[wait].Value, wait ) );
            }

            topXWaits.Clear();

            for( int i = 0; i < 10; i++ )
            {
                if( i == orderedWaits.Count )
                    break;

                topXWaits.Add( orderedWaits[i].Second );
            }
        }        

        private Color GetColor( int index )
        {
            Color result = Color.LightGray;
            try
            {
                if (index >= 0)
                {
                    if (waitsChart.Series == null || waitsChart.Series.Count == 0 || index >= waitsChart.Series.Count)
                    {
                        if (index < defaultLegendColors.Length)
                            result = defaultLegendColors[index];
                    }
                    else
                        result = waitsChart.Series[index].Color;
                }
            }
            catch
            {
                result = Color.LightGray;
            }

            //int alpha = 255;
            //if (waitsChart.AllSeries.Gallery == Gallery.Area)
            //    alpha = 175;

            //if (result.A != alpha)
            //    result = Color.FromArgb(alpha, result);

            return result;
        }

        #endregion

        #region Overrides (data refresh)
        
        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.ResourcesWaitsStatsView);
        }

        public override void RefreshView()
        {
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus( this.instanceId );

            if( status != null && status.InstanceVersion != null && status.InstanceVersion.Major <= 8 )
            {
                Sql2kWarningLabel.Visible = true;
                return;
            }

            if(HistoricalSnapshotDateTime == null || HistoricalSnapshotDateTime != currentHistoricalSnapshotDateTime 
                || HistoricalStartDateTime != currentHistoricalStartDateTime)
            {
                historyModeLoadError = null;
                base.RefreshView();
            }
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
                    while( Interlocked.Read( ref ctorInitialized ) == 0 ) {}

                    Initialize();

                    // wait for others to initialize
                    while( Interlocked.Read( ref otherInitialized ) == 0 ) {}
                }

                DateTime? historyDateTime = HistoricalSnapshotDateTime;

                if( historyDateTime == null )
                {
                    Log.Info( "Getting real-time snapshot" );
                    return GetRealTimeSnapshot(previousVisibleLimitInMinutes < currentRealTimeVisibleLimitInMinutes);
                }
                else
                {
                    Log.InfoFormat( "Populating historical snapshots (end={0}).", historyDateTime.Value );
                    if (ViewMode == ServerViewMode.Historical)
                    {
                        return GetHistoricalSnapshots(null, HistoricalSnapshotDateTime.Value, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes);
                    }
                    else
                    {
                        return GetHistoricalSnapshots(ApplicationModel.Default.HistoryTimeValue.StartDateTime,
                            ApplicationModel.Default.HistoryTimeValue.EndDateTime.Value, ApplicationModel.Default.HistoryTimeValue.CustomTimeMinutes);
                    }
                }
            }
        }

        public override void UpdateData( object data )
        {     
            lock( updateLock )
            {
                if( HistoricalSnapshotDateTime == null )
                {
                    if( data is WaitStatisticsSnapshot )
                    {
                        UpdateDataWithRealTimeSnapshot( data as WaitStatisticsSnapshot );
                    }
                    else
                    {
                        ApplicationController.Default.RefreshActiveView();
                        return;
                    }
                }
                else
                {
                    UpdateDataWithHistoricalSnapshots( data as DataTable );
                }
            }
        }

        private void UpdateChart()
        {         
            if( this.InvokeRequired )
            {
                this.Invoke( new MethodInvoker( UpdateChart ) );
                return;
            }

            if( HistoricalSnapshotDateTime == null )
            {
                operationalStatusPanel.Visible = false;
                maxPanel.Location = operationalStatusPanel.Location;
            }
            else
            {
                if( operationalStatusPanel.Visible )
                    maxPanel.Location = new Point( operationalStatusPanel.Location.X, operationalStatusPanel.Location.Y + operationalStatusPanel.Size.Height );
            }

            UpdateChartLegend();
            UpdateCategoryLegendColors();
            
            UpdateWaitTypesGridData();
            UpdateChartFieldMap();
            UpdateSeriesTransparency(waitsChart);

            UpdateRealTimeChart();
        }

        #region Real time updates

        private WaitStatisticsSnapshot GetRealTimeSnapshot(bool visibleMinutesIncreased)
        {   
            if (!chartRealTimeDataTablePrePopulated)
            {
                PrePopulateRealTimeDataTable();
                chartRealTimeDataTablePrePopulated = true;
            }
            else
            {
                BackfillScheduledRefreshData();

                // This will increase historical data if requird. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (visibleMinutesIncreased)
                    ForwardFillHistoricalData();

                // This will replace stale real-time data with historical. SqlDM 10.2 (Anshul Aggarwal) - New History Range Control
                if (Settings.Default.RealTimeChartHistoryLimitInMinutes < ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                    BackFillScheduledHistoricalData();
            }

            IManagementService          managementService = ManagementServiceHelper.GetDefaultService();
            WaitStatisticsConfiguration configuration     = new WaitStatisticsConfiguration( InstanceId, currentSnapshot );
            WaitStatisticsSnapshot      snapshot          = managementService.GetWaitStatistics( configuration );

            return snapshot;
        }

        private void PrePopulateRealTimeDataTable()
        {
            PopulateRealTimeSnapshots(null, DateTime.Now, ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes, true);
        }

        private void PopulateRealTimeSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes, bool clearData)
        {
            var data = RepositoryHelper.GetWaitStatistics(this.instanceId, endDateTime, minutes, startDateTime);
            UpdateChartData(data, clearData);
        }

        private void BackfillScheduledRefreshData()
        {
            var rtdt = this.chartRealTimeDataTable;
            var lastRowIndex = rtdt.Rows.Count - 1;
            if (lastRowIndex <= 0)
            {
                Log.Info("No data - doing prepopulate");
                // if no rows then prepopulate should do what we need
                PrePopulateRealTimeDataTable();
                return;
            }

            var now = DateTime.Now;
            var lastRow = rtdt.Rows[lastRowIndex];
            var lastDate = (DateTime)lastRow[col_timestamp];
            
            // SqlDM 10.2 (New History Browser) - If SD = 5mins and KD = 60min, then we need to reload entire data only if
            // we have no data for previous max(5,60) = 60 minutes. 
            var timeDiff = now - lastDate;
            if (timeDiff > TimeSpan.FromMinutes(ServerSummaryHistoryData.MaximumKeepData))
            {
                Log.InfoFormat("Last data point is from {0} ({1} old) - doing prepopulate to reload data", lastDate, timeDiff);
                // if last data point is older than our grooming period then prepopulate should work
                PrePopulateRealTimeDataTable();
                return;
            }

            var refreshInterval = TimeSpan.FromSeconds(Settings.Default.ForegroundRefreshIntervalInSeconds * 3);
            if (timeDiff <= refreshInterval || timeDiff.TotalMinutes < 1.0)
            {
                Log.VerboseFormat("Backfill skipped due to time difference between now and the last data point.  dif:{0}", timeDiff);
                return;
            }

            DataTable data = RepositoryHelper.GetWaitStatistics(this.instanceId, now, (int)timeDiff.TotalMinutes);
            MethodInvoker UICode = () => UpdateChartData(data, false);
            this.UIThread(UICode);
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
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
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
                    PopulateRealTimeSnapshots(startDateTime, endDateTime, null, false);
                }
            }
        }

        private void UpdateChartData(DataTable data, bool clearData)
        {
            if (data == null || data.Rows.Count == 0)
                return;

            string waittype = string.Empty;

            lock (updateLock)
            {
                chartRealTimeDataTable.BeginLoadData();
                if (clearData) chartRealTimeDataTable.Clear();
                int insertionIndex = -1;
                if (data.Rows.Count > 0 && !clearData)
                {
                    insertionIndex = ServerSummaryHistoryData.GetInsertionIndex(chartRealTimeDataTable, col_timestamp,
                        (DateTime)data.Rows[0][col_timestamp]);
                }
                foreach (DataRow datarow in data.Rows)
                {
                    DataRow row = chartRealTimeDataTable.NewRow();

                    waittype = (string)datarow[col_type];

                    row[col_timestamp] = datarow[col_timestamp];
                    row[col_ishistorical] = true;
                    row[col_type] = datarow[col_type];
                    row[col_waitingTasksCountTotal] = datarow[col_waitingTasksCountTotal];
                    row[col_waitTimeTotal] = datarow[col_waitTimeTotal];
                    row[col_maxWaitTime] = datarow[col_maxWaitTime];
                    row[col_signalWaitTimeTotal] = datarow[col_signalWaitTimeTotal];
                    row[col_resourceWaitTimeTotal] = datarow[col_resourceWaitTimeTotal];
                    row[col_waitingTasksPerSecond] = datarow[col_waitingTasksPerSecond];
                    row[col_totalWaitMillisecondsPerSecond] = datarow[col_totalWaitMillisecondsPerSecond];
                    row[col_signalWaitMillisecondsPerSecond] = datarow[col_signalWaitMillisecondsPerSecond];
                    row[col_resourceWaitMillisecondsPerSecond] = datarow[col_resourceWaitMillisecondsPerSecond];

                    if (waitTypesLookup != null && waitTypesLookup.ContainsKey(waittype))
                    {
                        row[col_categoryid] = waitTypesLookup[waittype].CategoryId;
                        row[col_categoryName] = waitTypesLookup[waittype].CategoryName;
                        //row[col_definition] = waitTypesLookup[waittype].Description;
                    }

                    if (insertionIndex >= 0)
                        chartRealTimeDataTable.Rows.InsertAt(row, insertionIndex++);
                    else
                        chartRealTimeDataTable.Rows.Add(row);
                }

                chartRealTimeDataTable.EndLoadData();
            }
        }

        private void UpdateDataWithRealTimeSnapshot( WaitStatisticsSnapshot snapshot )
        {     
            // do we have a snapshot
            if( snapshot == null )
            {
                Log.Info( "Snapshot returned as NULL" );
                return;
            }

            // was there an error
            if( snapshot.Error != null )
            {
                if( snapshot.ProductVersion != null && snapshot.ProductVersion.Major <= 8 )
                    Sql2kWarningLabel.Visible = true;

                ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now, snapshot.Error ) );

                return;
            }            
            
            Sql2kWarningLabel.Visible = false;
            latestSnapshotDateTime = snapshot.TimeStamp.HasValue ? snapshot.TimeStamp.Value.ToLocalTime() : snapshot.TimeStamp;

            Log.Info( string.Format("Snapshot returned {0} rows.", snapshot.Waits.Values.Count) );

            foreach( Wait wait in snapshot.Waits.Values )
            {
                DataRow row = chartRealTimeDataTable.NewRow();

                row[col_timestamp]    = snapshot.TimeStamp.Value.ToLocalTime();
                row[col_type]         = wait.WaitType;                
                if(wait.MaxWaitTime.HasValue)            row[col_maxWaitTime]            = wait.MaxWaitTime;                
                if(wait.ResourceWaitTimeDelta.HasValue)  row[col_resourceWaitTimeDelta]  = wait.ResourceWaitTimeDelta;
                if(wait.ResourceWaitTimeTotal.HasValue)  row[col_resourceWaitTimeTotal]  = wait.ResourceWaitTimeTotal;                
                if(wait.SignalWaitTimeDelta.HasValue)    row[col_signalWaitTimeDelta]    = wait.SignalWaitTimeDelta;
                if(wait.SignalWaitTimeTotal.HasValue)    row[col_signalWaitTimeTotal]    = wait.SignalWaitTimeTotal;
                if(wait.TimeDelta.HasValue)              row[col_timeDelta]              = wait.TimeDelta;                                                
                if(wait.WaitingTasksCountDelta.HasValue) row[col_waitingTasksCountDelta] = wait.WaitingTasksCountDelta;
                if(wait.WaitingTasksCountTotal.HasValue) row[col_waitingTasksCountTotal] = wait.WaitingTasksCountTotal;
                if(wait.WaitingTasksPerSecond.HasValue)  row[col_waitingTasksPerSecond]  = wait.WaitingTasksPerSecond;
                if(wait.WaitTimeDelta.HasValue)          row[col_waitTimeDelta]          = wait.WaitTimeDelta;
                if(wait.WaitTimeTotal.HasValue)          row[col_waitTimeTotal]          = wait.WaitTimeTotal;
                if(wait.TotalWaitMillisecondsPerSecond.HasValue)    row[col_totalWaitMillisecondsPerSecond]    = wait.TotalWaitMillisecondsPerSecond;
                if(wait.SignalWaitMillisecondsPerSecond.HasValue)   row[col_signalWaitMillisecondsPerSecond]   = wait.SignalWaitMillisecondsPerSecond;
                if(wait.ResourceWaitMillisecondsPerSecond.HasValue) row[col_resourceWaitMillisecondsPerSecond] = wait.ResourceWaitMillisecondsPerSecond;

                if( waitTypesLookup != null && waitTypesLookup.ContainsKey(wait.WaitType))
                {
                    row[col_categoryid]   = waitTypesLookup[wait.WaitType].CategoryId;
                    row[col_categoryName] = waitTypesLookup[wait.WaitType].CategoryName;
                    //row[col_definition]   = waitTypesLookup[wait.WaitType].Third;
                }

                chartRealTimeDataTable.Rows.Add( row );                
            }            

            currentSnapshot = snapshot;

            GroomHistoryData();    
            UpdateChart();                                            

            ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now ) );            
        }

        private void UpdateRealTimeChart()
        {   
            if( !waitTypesLookupInitialized )
                return;

            lock( updateLock )
            {
                DataTable d = chartRealTimeDataTable; // SHOULD DO IN ONE SPOT (HAVE SINGLE TABLE, SWITCH SOMEWHERE ELSE - EVENT HANDLER?)
                if( HistoricalSnapshotDateTime != null )
                    d = chartHistoricalDataTable;

                DataRow[] dataTable;
                if( HistoricalSnapshotDateTime == null )
                {
                    DateTime viewFilter = DateTime.Now.Subtract( TimeSpan.FromMinutes( ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes ) );
                    dataTable = d.Select( string.Format( "{0} > #{1}#", col_timestamp, viewFilter.ToString(CultureInfo.InvariantCulture)) );
                }
                else
                    dataTable = d.Select();

                if( currentCategoryFilter == "all" )
                {
                    // summarize the values for all wait types in all categories for every unique timestamp                

                    SortedDictionary<DateTime, Dictionary<string, double>> data = new SortedDictionary<DateTime, Dictionary<string, double>>();

                    DateTime timestamp = DateTime.MinValue;
                    string category = string.Empty;
                    double? value = null;
                    double? value1 = null;
                    double? value2 = null;

                    foreach( DataRow row in dataTable )
                    {
                        timestamp = ( DateTime )row[col_timestamp];
                        category  = "Other";

                        if( !( row[col_categoryName] is DBNull ) )
                            category = ( string )row[col_categoryName];


                        if( row[col_resourceWaitMillisecondsPerSecond] is DBNull )
                            continue;

                        value1 = ( double )row[col_resourceWaitMillisecondsPerSecond];
                        value2 = ( double )row[col_signalWaitMillisecondsPerSecond];

                        if( waitSumMode == WaitSumMode.Total )
                            value = value1 + value2;
                        else if( waitSumMode == WaitSumMode.Resource )
                            value = value1;
                        else if( waitSumMode == WaitSumMode.Signal )
                            value = value2;

                        if( !data.ContainsKey( timestamp ) )
                            data.Add( timestamp, new Dictionary<string, double>() );

                        if( !data[timestamp].ContainsKey( category ) )
                            data[timestamp].Add( category, value.Value );
                        else
                            data[timestamp][category] += value.Value;
                    }

                    waitsChart.SuspendLayout();
                    chartDataTable.BeginLoadData();
                    chartDataTable.Rows.Clear();

                    foreach( DateTime timestampkey in data.Keys )
                    {
                        DataRow row = chartDataTable.NewRow();
                        row[col_timestamp] = timestampkey;

                        foreach( string categorykey in data[timestampkey].Keys )
                        {
                            if( data[timestampkey].ContainsKey( categorykey ) )
                                row[categorykey] = data[timestampkey][categorykey];
                        }

                        chartDataTable.Rows.Add( row );
                    }

                    chartDataTable.EndLoadData();
                    waitsChart.ResumeLayout();
                }
                else
                {
                    SortedDictionary<DateTime, Dictionary<string, double>> data = new SortedDictionary<DateTime, Dictionary<string, double>>();

                    DateTime timestamp = DateTime.MinValue;
                    string type = string.Empty;
                    double? value = null;
                    double? value1 = null;
                    double? value2 = null;

                    foreach( DataRow row in dataTable )
                    {
                        timestamp = ( DateTime )row[col_timestamp];
                        type      = "Other";

                        if( !( row[col_type] is DBNull ) )
                            type = ( string )row[col_type];

                        if( !topXWaits.Contains( type ) )
                            continue;

                        if( row[col_resourceWaitMillisecondsPerSecond] is DBNull )
                            continue;

                        value1 = ( double )row[col_resourceWaitMillisecondsPerSecond];
                        value2 = ( double )row[col_signalWaitMillisecondsPerSecond];

                        if( waitSumMode == WaitSumMode.Total )
                            value = value1 + value2;
                        else if( waitSumMode == WaitSumMode.Resource )
                            value = value1;
                        else if( waitSumMode == WaitSumMode.Signal )
                            value = value2;

                        if( !data.ContainsKey( timestamp ) )
                            data.Add( timestamp, new Dictionary<string, double>() );

                        if( !data[timestamp].ContainsKey( type ) )
                            data[timestamp].Add( type, value.Value );
                        else
                            data[timestamp][type] += value.Value;
                    }

                    waitsChart.SuspendLayout();
                    chartDataTable.BeginLoadData();
                    chartDataTable.Rows.Clear();

                    foreach( DateTime timestampkey in data.Keys )
                    {
                        DataRow row = chartDataTable.NewRow();
                        row[col_timestamp] = timestampkey;

                        foreach( string typekey in data[timestampkey].Keys )
                        {
                            if( data[timestampkey].ContainsKey( typekey ) )
                            {
                                if( chartDataTable.Columns.Contains( typekey ) )
                                    row[typekey] = data[timestampkey][typekey];
                            }
                        }

                        chartDataTable.Rows.Add( row );
                    }

                    chartDataTable.EndLoadData();
                    waitsChart.ResumeLayout();
                }
            }

            if( chartDataTable.Rows.Count == 0 )
                waitsChart.SetMessageText( "NoData", "There is no data to show in this view." );

            waitsChart.DataSourceSettings.ReloadData();
        }

        private void GroomHistoryData()
        {     
            if (chartRealTimeDataTable != null)
            {
                var selectFilter = ServerSummaryHistoryData.GetGroomingFilter(col_timestamp);
                DataRow[] groomedRows = chartRealTimeDataTable.Select(selectFilter);
                foreach (DataRow row in groomedRows)
                {
                    row.Delete();
                }

                chartRealTimeDataTable.AcceptChanges();
            }
        }

        #endregion

        #region Historical updates

        private DataTable GetHistoricalSnapshots(DateTime? startDateTime, DateTime endDateTime, int? minutes)
        {        
            return RepositoryHelper.GetWaitStatistics( this.instanceId,endDateTime, minutes, startDateTime);
        }

        private void UpdateDataWithHistoricalSnapshots( DataTable data )
        {      
            ShowOperationalStatus(Properties.Resources.HistoryModeOperationalStatusLabel, Properties.Resources.StatusWarningSmall);
            currentHistoricalSnapshotDateTime = HistoricalSnapshotDateTime;
            currentHistoricalStartDateTime = HistoricalStartDateTime;
            string waittype = string.Empty;

            if( data == null )
                return;

            chartHistoricalDataTable.Clear();
            chartHistoricalDataTable.BeginLoadData();

            foreach( DataRow datarow in data.Rows )
            {
                DataRow row = chartHistoricalDataTable.NewRow();

                waittype = ( string )datarow[col_type];

                row[col_timestamp]                         = datarow[col_timestamp];
                row[col_type]                              = datarow[col_type];
                row[col_waitingTasksCountTotal]            = datarow[col_waitingTasksCountTotal];
                row[col_waitTimeTotal]                     = datarow[col_waitTimeTotal];
                row[col_maxWaitTime]                       = datarow[col_maxWaitTime];
                row[col_signalWaitTimeTotal]               = datarow[col_signalWaitTimeTotal];
                row[col_resourceWaitTimeTotal]             = datarow[col_resourceWaitTimeTotal];
                row[col_waitingTasksPerSecond]             = datarow[col_waitingTasksPerSecond];
                row[col_totalWaitMillisecondsPerSecond]    = datarow[col_totalWaitMillisecondsPerSecond];
                row[col_signalWaitMillisecondsPerSecond]   = datarow[col_signalWaitMillisecondsPerSecond];
                row[col_resourceWaitMillisecondsPerSecond] = datarow[col_resourceWaitMillisecondsPerSecond];

                if( waitTypesLookup != null && waitTypesLookup.ContainsKey( waittype ) )
                {
                    row[col_categoryid]   = waitTypesLookup[waittype].CategoryId;
                    row[col_categoryName] = waitTypesLookup[waittype].CategoryName;
                }

                chartHistoricalDataTable.Rows.Add( row );
            }

            chartHistoricalDataTable.EndLoadData();
            
            UpdateChart();            

            ApplicationController.Default.OnRefreshActiveViewCompleted( new RefreshActiveViewCompletedEventArgs( DateTime.Now ) );
        }

        #endregion       

        private void UpdateWaitTypesGridData()
        {         
            if( this.InvokeRequired )
            {
                this.Invoke( new MethodInvoker( UpdateWaitTypesGridData ) );
                return;
            }

            if( !waitTypesLookupInitialized )
                return;

            Dictionary<string, double?[]> data = GetCurrentWaitValues();

            string waitType;

            // loop through and udpate grid data table
            foreach( DataRow row in gridWaitTypesDataTable.Rows )
            {
                waitType = ( string )row["Type"];

                if( !data.ContainsKey( waitType ) )
                {
                    row["Wait"]       = DBNull.Value;
                    row["Total Wait"] = DBNull.Value;
                    continue;
                }

                row["Wait"]       = data[waitType][0];
                row["Total Wait"] = data[waitType][1];
            }

            waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.RefreshSort(false);
        }

        private Dictionary<string, double?[]> GetCurrentWaitValues()
        {         
            Dictionary<string, double?[]> data = new Dictionary<string, double?[]>(); // key=wait type, value=[wait value, total wait]
            string  waitType = string.Empty;
            double? value    = null;                
            double? value1   = null;                
            double? value2   = null;


            lock( updateLock )
            {
                DataTable dataTable = chartRealTimeDataTable;
                if( HistoricalSnapshotDateTime != null )
                    dataTable = chartHistoricalDataTable;

                DateTime timestamp = DateTime.MinValue;

                // get most recent values
                foreach( DataRow row in dataTable.Rows )
                {
                    if( row[col_resourceWaitMillisecondsPerSecond] is DBNull )
                        continue;

                    waitType = ( string )row[col_type];
                    value1   = ( double? )row[col_resourceWaitMillisecondsPerSecond];
                    value2   = ( double? )row[col_signalWaitMillisecondsPerSecond];

                    if( waitSumMode == WaitSumMode.Total )
                        value = value1 + value2;
                    else if( waitSumMode == WaitSumMode.Resource )
                        value = value1;
                    else if( waitSumMode == WaitSumMode.Signal )
                        value = value2;

                    timestamp = (DateTime)row[col_timestamp];

                    if( !data.ContainsKey( waitType ) )
                    {
                        double? currentValue = 0;

                        if (timestamp == latestSnapshotDateTime)
                            currentValue = value;

                        data.Add(waitType, new double?[] { currentValue, value });
                    }
                    else
                    {
                        data[waitType][1] += value; // always sum the total

                        // only sum wait value for current times
                        if( HistoricalSnapshotDateTime == null )
                        {
                            if (timestamp == latestSnapshotDateTime)
                            {
                                if (!data[waitType][0].HasValue)
                                    data[waitType][0] = 0;

                                data[waitType][0] += value;
                            }
                        }
                        else if(timestamp <= HistoricalSnapshotDateTime)
                        {
                            data[waitType][0] = value;
                        }
                    }
                }
            }

            return data;
        }

        private Dictionary<string, double?> GetWaitValues(bool includeAllTimes)
        {           
            Dictionary<string, double?> data = new Dictionary<string, double?>(); // key=wait type, value=wait value
            string  waitType = string.Empty;
            double? value    = null;                
            double? value1   = null;                
            double? value2   = null;


            lock( updateLock )
            {
                DataTable dataTable = chartRealTimeDataTable; // SHOULD DO IN ONE SPOT (HAVE SINGLE TABLE, SWITCH SOMEWHERE ELSE - EVENT HANDLER)
                if( HistoricalSnapshotDateTime != null )
                    dataTable = chartHistoricalDataTable;

                // get most recent values
                foreach( DataRow row in dataTable.Rows )
                {
                    if( HistoricalSnapshotDateTime == null )
                    {
                        if( !includeAllTimes && ( DateTime )row[col_timestamp] != latestSnapshotDateTime )
                            continue;
                    }

                    if( row[col_resourceWaitMillisecondsPerSecond] is DBNull )
                        continue;

                    waitType = ( string )row[col_type];
                    value1 = ( double? )row[col_resourceWaitMillisecondsPerSecond];
                    value2 = ( double? )row[col_signalWaitMillisecondsPerSecond];

                    if( waitSumMode == WaitSumMode.Total )
                        value = value1 + value2;
                    else if( waitSumMode == WaitSumMode.Resource )
                        value = value1;
                    else if( waitSumMode == WaitSumMode.Signal )
                        value = value2;

                    if( !data.ContainsKey( waitType ) )
                        data.Add( waitType, value );
                    else
                        data[waitType] += value;
                }
            }

            return data;
        }        

        public override void HandleBackgroundWorkerError( Exception e )
        {         
            if (HistoricalSnapshotDateTime != null)
            {
                historyModeLoadError = e;
                //historicalSnapshotStatusLinkLabel.Text = Properties.Resources.HistoryModeSnapshotLoadErrorViewLabel;
                //ServerDetailsView_Fill_Panel.Visible = false;
            }

            base.HandleBackgroundWorkerError( e );
        }        

        #endregion 

        #region Event Handlers

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "RealTimeChartVisibleLimitInMinutes":
                    UpdateChart();
                    break;
            }
        }
        
        private void MaximizeChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton)
        {  
            tableLayoutPanel1.Visible = false;
            tableLayoutPanel1.Controls.Remove(chartPanel);
            
            maximizeButton.Visible = false;
            restoreButton.Visible  = true;

            maxPanel.Controls.Add(chartPanel);

            // SqlDM 10.2 (Anshul Aggarwal) - SQLDM-27579 - Unable to restore the maximized graph in 'Resources->Server Waits' screen
            maxPanel.Size = tableLayoutPanel1.Size; 

            if ( HistoricalSnapshotDateTime != null )
                maxPanel.Location = tableLayoutPanel1.Location;
            else
                maxPanel.Location = operationalStatusPanel.Location;
        }

        private void RestoreChart(Panel chartPanel, ToolStripButton maximizeButton, ToolStripButton restoreButton, int column, int row)
        {         
            maxPanel.Controls.Remove(chartPanel);

            maximizeButton.Visible = true;
            restoreButton.Visible  = false;

            tableLayoutPanel1.Controls.Add(chartPanel);
            tableLayoutPanel1.SetCellPosition(chartPanel, new TableLayoutPanelCellPosition(column, row));
            tableLayoutPanel1.Visible = true;
        }

        private void WaitsByCategoryMaximizeButton_Click(object sender, EventArgs e)
        {         
            MaximizeChart(categoryChartPanel, waitsByCategoryMaximizeButton, waitsByCategoryMinimizeButton);
        }

        private void WaitsByCategoryRestoreButton_Click(object sender, EventArgs e)
        {         
            RestoreChart(categoryChartPanel, waitsByCategoryMaximizeButton, waitsByCategoryMinimizeButton, 0, 0);
        }

        private void waitTypeGridMinimizeButton_Click( object sender, EventArgs e )
        {         
            RestoreChart(waitTypeGridPanel, waitTypeGridMaximizeButton, waitTypeGridMinimizeButton, 0, 1);            
        }

        private void waitTypeGridMaximizeButton_Click( object sender, EventArgs e )
        {         
            MaximizeChart(waitTypeGridPanel, waitTypeGridMaximizeButton, waitTypeGridMinimizeButton);
        }

        private void totalWaitTimeToolStripMenuItem_Click( object sender, EventArgs e )
        {   
            if( updatingmenu )
                return;   

            waitSumMode = WaitSumMode.Total;
            
            lock( updateLock )
            {
                UpdateWaitSumMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }

        private void signalToolStripMenuItem_Click( object sender, EventArgs e )
        {        
            if( updatingmenu )
                return;   

            waitSumMode = WaitSumMode.Signal;
            
            lock( updateLock )
            {
                UpdateWaitSumMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }

        private void resourceToolStripMenuItem_Click( object sender, EventArgs e )
        {   
            if( updatingmenu )
                return;   

            waitSumMode = WaitSumMode.Resource;
            
            lock( updateLock )
            {
                UpdateWaitSumMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }

        private void UpdateWaitSumMenuItems()
        {         
            updatingmenu = true;

            totalWaitTimeToolStripMenuItem.Checked = waitSumMode == WaitSumMode.Total;
            signalToolStripMenuItem.Checked        = waitSumMode == WaitSumMode.Signal;
            resourceToolStripMenuItem.Checked      = waitSumMode == WaitSumMode.Resource;

            waitsSumModeMenuItem.Text = waitsSumModeMenuStrings[(int)waitSumMode];

            updatingmenu = false;
        }

        private void allWaitTypesMenuButton_Click( object sender, EventArgs e )
        {         
            if( updatingmenu )
                return;            
            
            // update the data filter
            currentCategoryFilter = "all";
            homeMenuToolstripButton.Enabled = false;

            // update menu item
            waitTypesCategoryToolstripMenuButton.Text = "All Wait Types";

            lock( updateLock )
            {
                UpdateCategoryMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }
        
        private void CategoryMenuItem_CheckedChanged( object sender, EventArgs e )
        {         
            if( updatingmenu )
                return;

            // update the data filter            
            currentCategoryFilter = ( ( ToolStripMenuItem )sender ).Text;
            homeMenuToolstripButton.Enabled = true;

            // update menu item
            waitTypesCategoryToolstripMenuButton.Text = currentCategoryFilter;

            lock( updateLock )
            {
                UpdateCategoryMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }

        private void categoryLegend_CellContentClick( object sender, DataGridViewCellEventArgs e )
        {        
            if( updatingmenu || currentCategoryFilter != "all" )
                return;

            categoryLegend.ClearSelection();

            if( e.ColumnIndex == 0 )
                return;

            // update the data filter
            currentCategoryFilter = (string)categoryLegend[e.ColumnIndex, e.RowIndex].Value;
            homeMenuToolstripButton.Enabled = true;

            // update menu item
            waitTypesCategoryToolstripMenuButton.Text = currentCategoryFilter;

            lock( updateLock )
            {
                UpdateCategoryMenuItems();
                UpdateWaitTypesGrid();
                UpdateChart();
            }
        }

        private void UpdateCategoryMenuItems()
        {         
            updatingmenu = true;

            // uncheck the other menu options
            allWaitTypesMenuButton.Checked = currentCategoryFilter == "all";
            foreach( ToolStripItem menuItem in waitsByCategoryToolStripMenuItem.DropDownItems )
                ( ( ToolStripMenuItem )( menuItem ) ).Checked = ( ( ToolStripMenuItem )( menuItem ) ).Text == currentCategoryFilter;

            updatingmenu = false;
        }

        private void homeMenuToolstripButton_Click( object sender, EventArgs e )
        {
            allWaitTypesMenuButton_Click( sender, e );
        }

        private void waitTypesGrid_ClickCell( object sender, Infragistics.Win.UltraWinGrid.ClickCellEventArgs e )
        {
            if( e.Cell.Column.Header.Caption != "Help" || !waitTypesLookupInitialized)
                return;

            string type = e.Cell.Row.Cells["Type"].Value as string;
            string help = waitTypesLookup[type].HelpLink;

            if( string.IsNullOrEmpty( help ) )
                return;

            if( help.StartsWith( "http" ) )
            {
                try
                {
                    Process.Start( help );
                }
                catch( Exception ex )
                {
                    ApplicationMessageBox.ShowError( this, "Failed to launch URL " + help, ex );
                }
            }
            else
            {
                ApplicationHelper.ShowHelpTopic( help );
            }

        }

        private void ResourcesWaitStats_Resize( object sender, EventArgs e )
        {
            UpdateGridColumns();
        }

        private void UpdateGridColumns()
        {
            if( !waitTypesLookupInitialized )
                return;

            waitTypesGrid.DisplayLayout.Bands[0].Columns["Help"].Width = 60;

            int a = waitTypesGrid.DisplayLayout.Bands[0].Columns["Category"].Width;
            int b = waitTypesGrid.DisplayLayout.Bands[0].Columns["Type"].Width;
            int c = waitTypesGrid.DisplayLayout.Bands[0].Columns["Wait"].Width;
            int d = waitTypesGrid.DisplayLayout.Bands[0].Columns["Total Wait"].Width;
            int e = waitTypesGrid.DisplayLayout.Bands[0].Columns["Help"].Width;
            
            int width = waitTypesGrid.Width - ( a + b + c + d + e ) - 20;

            if( width < 0 )
                width = 0;

            waitTypesGrid.DisplayLayout.Bands[0].Columns["Description"].Width = width;
        }

        private void graphAsLinesToolStripMenuItem_Click( object sender, EventArgs e )
        {
            waitsChart.AllSeries.Stacked = Stacked.No;
            waitsChart.Gallery = Gallery.Lines;

            if( waitsChart.DataSourceSettings.Fields.Count >= 1 )
                waitsChart.DataSourceSettings.Fields[0].Usage = FieldUsage.XValue;

            graphAsStackedBarsToolStripMenuItem.Checked = false;
            toolStripDropDownButtonGraphMode.Text = graphAsLinesToolStripMenuItem.Text;

            UpdateSeriesTransparency(waitsChart, 255);

            waitsChart.Refresh();
        }

        private void graphAsStackedBarsToolStripMenuItem_Click( object sender, EventArgs e )
        {
            waitsChart.AllSeries.Stacked = Stacked.Normal;
            waitsChart.Gallery = Gallery.Area;

            if( waitsChart.DataSourceSettings.Fields.Count >= 1 )
                waitsChart.DataSourceSettings.Fields[0].Usage = FieldUsage.Label;

            graphAsLinesToolStripMenuItem.Checked = false;
            toolStripDropDownButtonGraphMode.Text = graphAsStackedBarsToolStripMenuItem.Text;

            UpdateSeriesTransparency(waitsChart, 175);
            waitsChart.Refresh();
        }

        private void UpdateSeriesTransparency(Chart chart)
        {
            if (chart.AllSeries.Gallery == Gallery.Area)
                UpdateSeriesTransparency(chart, 175);
            else
                UpdateSeriesTransparency(chart, 255);
        }

        private void UpdateSeriesTransparency(Chart chart, int alpha)
        {
            if (chart.Series.Count == 0) return;
            ChartFxExtensions.SetAreaSeriesAlphaChannel(chart, alpha);
        }

        #endregion

        #region Splitter Focus

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

            return focusedControl ?? controls[0];
        }

        private void SplitContainer_MouseDown(object sender, MouseEventArgs e)
        {
            focused = GetFocusedControl(Controls);
        }

        private void SplitContainer_MouseUp(object sender, MouseEventArgs e)
        {
            if (focused != null)
            {
                focused.Focus();
                focused = null;
            }
        }

        #endregion        

        #region Context menu handlers

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {    
//            if (contextMenuSelectedChart == null)
//                return;

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
                case "sortAscendingButton":
                    SortSelectedColumnAscending();
                    break;
                case "sortDescendingButton":
                    SortSelectedColumnDescending();
                    break;
                case "groupByThisColumnButton":
                    GroupBySelectedColumn(((StateButtonTool)e.Tool).Checked);
                    break;
                case "groupByBoxButton":
                    ToggleGroupByBox();
                    break;
                case "removeThisColumnButton":
                    RemoveSelectedColumn();
                    break;
                case "columnChooserButton":
                    ShowColumnChooser();
                    break;
                case "collpaseAllGroupsButton":
                    CollapseAllGroups();
                    break;
                case "expandAllGroupsButton":
                    ExpandAllGroups();
                    break;
                case "printGridButton":
                    PrintGrid();
                    break;
                case "exportGridButton":
                    SaveGrid();
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
                    bool isGrouped = waitTypesGrid.Rows.Count > 0 && waitTypesGrid.Rows[0].IsGroupByRow;
                    ((PopupMenuTool)e.Tool).Tools["collapseAllGroupsButton"].SharedProps.Enabled =
                        ((PopupMenuTool)e.Tool).Tools["expandAllGroupsButton"].SharedProps.Enabled = isGrouped;
                    break;
                case "columnContextMenu":
                    int minCantForEnable = UltraGridHelper.GetNotHiddenColumns(waitTypesGrid);
                    bool enableTool = minCantForEnable > 1 ? true : false;

                    ((PopupMenuTool)e.Tool).Tools["removeThisColumnButton"].SharedProps.Enabled = enableTool;
                    break;
            }
        }

        private void ToggleChartToolbar(Chart chart, bool Visible)
        {         
            chart.ToolBar.Visible = Visible;
        }

        private string GetPrintTitle()
        {
            string printTitle = string.Empty;
            if (waitTypesCategoryToolstripMenuButton != null)
            {
                printTitle = string.Format("{0}\n{1}\\{2}",
                    "Server Waits",
                    waitTypesCategoryToolstripMenuButton.Text
                    , waitsSumModeMenuItem.Text);
            }
            return printTitle;
        }

        private void PrintChart(Chart chart)
        {
            bool legendBoxStatus = chart.LegendBox.Visible;
            chart.LegendBox.Visible = true;
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
            chart.LegendBox.Visible = legendBoxStatus;
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
            bool legendBoxStatus = chart.LegendBox.Visible;
            chart.LegendBox.Visible = true;
            string title = string.Empty;

            if (chart.Tag is ToolStripItem)
            {
                title = ((ToolStripItem)chart.Tag).Text;
            }
            else
            {
                title = GetPrintTitle();
            }

            ExportHelper.ChartHelper.ExportImageWithTitle(this, chart, title, ExportHelper.GetValidFileName(title, true));
            chart.LegendBox.Visible = legendBoxStatus;
        }

        #endregion                        

        #region Operational status

        private void ShowOperationalStatus(string message, Image icon)
        {
            operationalStatusImage.Image = icon;
            operationalStatusLabel.Text = string.Format(message, message);
            operationalStatusPanel.Visible = true;
        }

        private void SwitchToRealTimeMode()
        {
            operationalStatusPanel.Visible = false;
            //ShowChartStatusMessage("Loading...", true);
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
            operationalStatusLabel.BackColor = Color.FromArgb(212, 212, 212);
            operationalStatusImage.BackColor = Color.FromArgb(212, 212, 212);
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


        #endregion                        

        #region grid

        private void waitTypesGrid_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                UIElement selectedElement =
                    ((UltraGrid)sender).DisplayLayout.UIElement.ElementFromPoint(new Point(e.X, e.Y));
                object contextObject = selectedElement.GetContext(typeof(ColumnHeader));

                if (contextObject is ColumnHeader)
                {
                    ColumnHeader columnHeader = contextObject as ColumnHeader;
                    selectedColumn = columnHeader.Column;

                    ((StateButtonTool)toolbarsManager.Tools["groupByThisColumnButton"]).Checked = selectedColumn.IsGroupByColumn;
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

        private void SortSelectedColumnAscending()
        {
            if (selectedColumn != null)
            {

                waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, false);
            }
        }

        private void SortSelectedColumnDescending()
        {
            if (selectedColumn != null)
            {
                waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Clear();
                waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, true, false);
            }
        }

        private void GroupBySelectedColumn(bool GroupBy)
        {
            if (selectedColumn != null)
            {
                if (GroupBy)
                {
                    waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Add(selectedColumn, false, true);
                }
                else
                {
                    waitTypesGrid.DisplayLayout.Bands[0].SortedColumns.Remove(selectedColumn);
                }

                GridGroupByBoxVisible = waitTypesGrid.Rows[0].IsGroupByRow;
            }
        }

        private void ToggleGroupByBox()
        {
            GridGroupByBoxVisible = !GridGroupByBoxVisible;
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
            SimpleUltraGridColumnChooserDialog dialog = new SimpleUltraGridColumnChooserDialog(waitTypesGrid);
            dialog.Show(this);
        }

        private void CollapseAllGroups()
        {
            waitTypesGrid.Rows.CollapseAll(true);
        }

        private void ExpandAllGroups()
        {
            waitTypesGrid.Rows.ExpandAll(true);
        }

        private void PrintGrid()
        {
            ultraPrintPreviewDialog.Document = waitTypesGridPrintDocument;
            waitTypesGridPrintDocument.DefaultPageSettings.Landscape = true;
            waitTypesGridPrintDocument.Header.TextLeft =
                    string.Format("{0} - server waits as of {1}",
                                        ApplicationModel.Default.ActiveInstances[instanceId].InstanceName,
                                        DateTime.Now.ToString("G")
                                    );
            waitTypesGridPrintDocument.Footer.TextCenter = "Page [Page #]";

            ultraPrintPreviewDialog.ShowDialog();
        }

        private void SaveGrid()
        {
            waitTypesSaveFileDialog.DefaultExt = "xls";
            waitTypesSaveFileDialog.FileName = "ServerWaits";
            waitTypesSaveFileDialog.Filter = "Excel Workbook (*.xls)|*.xls";
            waitTypesSaveFileDialog.Title = "Save as Excel Spreadsheet";
            if (waitTypesSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    waitTypesGridExcelExporter.Export(waitTypesGrid, waitTypesSaveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    ApplicationMessageBox.ShowError(this, "Unable to export data", ex);
                }
            }
        }

        #endregion

        private int hilightRow = -1;
        private void categoryLegend_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
            {
                if (hilightRow != -1)
                    ResetHilight();
                return;
            }
            if (e.RowIndex != hilightRow)
                ResetHilight();

            HilightRow(categoryLegend.Rows[e.RowIndex]);
        }

        private void categoryLegend_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (hilightRow == -1) return;
            ResetHilight();
        }

        private void HilightRow(DataGridViewRow row)
        {
            if (row == null || !(row.Tag is Int32)) return;
            int index = (int)row.Tag;
            if (index < waitsChart.Series.Count)
            {
                try
                {
                    waitsChart.Highlight.HighlightItem(waitsChart.Series[index]);
                    hilightRow = index;
                }
                catch
                {
                    ResetHilight();
                }
            }
        }

        private void ResetHilight()
        {
            waitsChart.Highlight.ClearHighlight(this);
            hilightRow = -1;
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
            if(Settings.Default.ColorScheme == "Dark")
            {
                this.categoryLegend.BackgroundColor = ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor);
                this.categoryLegend.DefaultCellStyle.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor);
            }
            else
            {
                this.categoryLegend.BackgroundColor = Color.White;
                this.categoryLegend.DefaultCellStyle.BackColor = Color.White;
            }
            if (waitsChart != null)
                waitsChart.ForeColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ChartForeColor) : Color.Black;
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.waitTypesGrid);
        }
        #endregion
    }
}
