using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BBS.TracerX;
using Idera.SQLdm.Common.Snapshots;
using Wintellect.PowerCollections;
using System.Data;
using System.Text.RegularExpressions;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources;
using System.Threading;

namespace Idera.SQLdm.DesktopClient.Controls
{
    
    [ToolboxBitmap(typeof(Panel))]
    public partial class FileActivityPanel : Panel
    {
        #region Fields

        private int instanceId;
        private const string readsPerSecondText  = "Reads/s:";
        private const string writesPerSecondText = "Writes/s:";
        private static readonly Logger Log = Logger.GetLogger("FileActivityPanel");
        private SizeF writeSizeLabel;
        private SizeF readSizeLabel;

        private HScrollBar disksScrollBar;
        private Dictionary<string, Pair<VScrollBar, bool>> fileScrollBars; // bool -> intended visible state of scrollbar

        private Dictionary<string, object>             cache;
        private Dictionary<string, SortedList<DateTime, double?[]>> othersCache; // cache of others histories
        private Dictionary<string, double[]>           othersCacheHistoryMax; // max for the current others cache histories
        private Dictionary<string, double[]>           cacheHistoryMax; // max values for history data for each object (key is disk or file key)
        private Dictionary<string, string>             maxFileCache; // max file per drive,key => drivekey, value=> filekey of max drive (drive with highest io)
        private Dictionary<string, FileActivityFile>   othersFileCache;
        private Dictionary<string, int>                scrollDeltas; // offsets for each scrollbar (key = diskkey)
        private Dictionary<string, ScrollInfo>         scrollInfos; // rectangles and type for each scrollbar (key = diskkey)
        private FileActivityFilter                     filter;
        private SortedList<string, OrderedSet<string>> filteredCacheKeys;

        private Rectangle disksGroupRect; // for the rectangle holding all disks
        private Rectangle viewRect;
        private Dictionary<string, Rectangle> diskRectangles; // for disks
        private Dictionary<string, Rectangle> fileRectangles; // for files
        private Dictionary<string, Rectangle> toggleHotSpots; // locations where double clicking expands a file 
        private Dictionary<string, Rectangle> expandoHotSpots; // locations of the expand/collapse arrows
        private Dictionary<string, bool>      fileExpandedStates;
        private List<TooltipInfo>             toolTips;
        private TooltipInfo                   currentTooltip;

        // maps filekey to drivekey in cache (so we can get to the drive from the file) [filepath->driveletter, filepath->driveletter,...]
        private Dictionary<string, string> fileDiskMap;

        private const int diskUiWidth           = 250;
        private const int diskUiHeaderHeight    = 50;
        private const int diskUiMargin          = 5;
        private const int fileUiMargin          = 5;
        private const int fileUiExpandedHeight  = 300;
        private const int fileUiCollapsedHeight = 50;

        private SolidBrush controlBackColorBrush;
        private SolidBrush diskBackColorBrush;
        private SolidBrush fileBackColorBrush;
        private SolidBrush fileBackColorMaxBrush;
        private SolidBrush fileOthersBackColorBrush;
        private SolidBrush fileIconBackColorBrush;
        private SolidBrush fileDarkTextBrush;
        private SolidBrush fileEmptyActivityBrush;
        private SolidBrush fileReadActivityBrush;
        private SolidBrush fileWriteActivityBrush;
        private SolidBrush fileChartBackColorBrush;
        private SolidBrush histogramBotBackBrush;
        private Pen        outlinePen;
        private Pen        histogramDividerPen;
        private Pen        readHistogramPen;
        private Pen        writeHistorgramPen;
        private Pen        readLinePen;
        private Pen        writeLinePen;

        private SolidBrush[] readActivityBrushes;
        private SolidBrush[] writeActivityBrushes;

        private Dictionary<FileActivityFileType, Image> fileTypeImages;
        private ContextMenu menu;
        private ToolTip tooltip;
        private Point   mouseClickLocation;
        private Point   mouseMovePrevLocation;
        private string  mouseMoveDiskkey;
        private bool    leftMouseDown;

        private FileActivitySort sort;
        private FileActivitySortDirection sortDirection;
        private Sorter sorter;

        private int    currentFlashStep;
        private Thread flashThread;
        private List<Pair<Rectangle, bool>> activityLights;
        private readonly object paintlock = new object();

        #endregion

        #region Properties

        public int InstanceId
        {
            get { return instanceId; }
            set { instanceId = value; }
        }

        internal FileActivitySort SortType
        {
            get { return sort; }
            set { sort = value; }
        }

        internal FileActivitySortDirection SortDirection
        {
            get { return sortDirection; }
            set { sortDirection = value; }
        }

        #endregion

        #region Contstructor

        public FileActivityPanel()
        {
            SetStyle
                (
                    ControlStyles.UserPaint |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.ResizeRedraw |
                    ControlStyles.OptimizedDoubleBuffer, true
                );

            sort          = FileActivitySort.Filename;
            sortDirection = FileActivitySortDirection.Down;
            sorter        = new Sorter(this);

            currentFlashStep = 0; // of 11
            activityLights   = new List<Pair<Rectangle, bool>>();
            flashThread      = new Thread(new ThreadStart(ActivityFlashThreadWorker));            
            flashThread.Name = "File Activity Panel UI Animation Thread";
            flashThread.IsBackground = true;

            tooltip = new ToolTip();

            // setup the colors            
            controlBackColorBrush    = new SolidBrush(Color.FromArgb(235, 235, 235));
            diskBackColorBrush       = new SolidBrush(Color.FromArgb(247, 247, 247));   
            fileBackColorBrush       = new SolidBrush(Color.FromArgb(233, 233, 233));
            fileBackColorMaxBrush    = new SolidBrush(Color.FromArgb(255, 229, 147));
            fileOthersBackColorBrush = new SolidBrush(Color.FromArgb(231, 232, 233));
            fileIconBackColorBrush   = new SolidBrush(Color.FromArgb(203, 203, 203));
            fileDarkTextBrush        = new SolidBrush(Color.FromArgb(203, 203, 203));
            fileEmptyActivityBrush   = new SolidBrush(Color.FromArgb(166, 166, 166));
            fileReadActivityBrush    = new SolidBrush(Color.FromArgb(57, 160, 48));
            fileWriteActivityBrush   = new SolidBrush(Color.FromArgb(80, 124, 209));
            fileChartBackColorBrush  = new SolidBrush(Color.WhiteSmoke);
            histogramBotBackBrush = new SolidBrush(Color.WhiteSmoke);     
            outlinePen               = new Pen(Color.FromArgb(203, 203, 203));
            histogramDividerPen      = new Pen(Color.FromArgb(233, 233, 233), 4);
            readHistogramPen         = new Pen(Color.FromArgb(57, 160, 48));
            writeHistorgramPen       = new Pen(Color.FromArgb(80, 124, 209));
            readLinePen              = new Pen(Color.FromArgb(57, 160, 48), 2);
            writeLinePen             = new Pen(Color.FromArgb(80, 124, 209), 2);

            readActivityBrushes = new SolidBrush[11];
            readActivityBrushes[0]  = new SolidBrush(Color.FromArgb(57, 160, 48));
            readActivityBrushes[1]  = new SolidBrush(ControlPaint.Light(readActivityBrushes[0].Color, 0.20f));
            readActivityBrushes[2]  = new SolidBrush(ControlPaint.Light(readActivityBrushes[1].Color, 0.20f));
            readActivityBrushes[3]  = new SolidBrush(ControlPaint.Light(readActivityBrushes[2].Color, 0.20f));
            readActivityBrushes[4]  = new SolidBrush(ControlPaint.Light(readActivityBrushes[3].Color, 0.20f));
            readActivityBrushes[5]  = new SolidBrush(ControlPaint.Light(readActivityBrushes[4].Color, 0.20f));
            readActivityBrushes[6]  = (SolidBrush)readActivityBrushes[4].Clone();
            readActivityBrushes[7] =  (SolidBrush)readActivityBrushes[3].Clone();
            readActivityBrushes[8] =  (SolidBrush)readActivityBrushes[2].Clone();
            readActivityBrushes[9] =  (SolidBrush)readActivityBrushes[1].Clone();
            readActivityBrushes[10] = (SolidBrush)readActivityBrushes[0].Clone();

            writeActivityBrushes = new SolidBrush[11];
            writeActivityBrushes[0] = new SolidBrush(Color.FromArgb(80, 124, 209));
            writeActivityBrushes[1] = new SolidBrush(ControlPaint.Light(writeActivityBrushes[0].Color, 0.20f));
            writeActivityBrushes[2] = new SolidBrush(ControlPaint.Light(writeActivityBrushes[1].Color, 0.20f));
            writeActivityBrushes[3] = new SolidBrush(ControlPaint.Light(writeActivityBrushes[2].Color, 0.20f));
            writeActivityBrushes[4] = new SolidBrush(ControlPaint.Light(writeActivityBrushes[3].Color, 0.20f));
            writeActivityBrushes[5] = new SolidBrush(ControlPaint.Light(writeActivityBrushes[4].Color, 0.20f));
            writeActivityBrushes[6] = (SolidBrush)writeActivityBrushes[4].Clone();
            writeActivityBrushes[7] = (SolidBrush)writeActivityBrushes[3].Clone();
            writeActivityBrushes[8] = (SolidBrush)writeActivityBrushes[2].Clone();
            writeActivityBrushes[9] = (SolidBrush)writeActivityBrushes[1].Clone();
            writeActivityBrushes[10] = (SolidBrush)writeActivityBrushes[0].Clone();

            // setup data objects
            cache              = new Dictionary<string, object>();
            othersCache        = new Dictionary<string, SortedList<DateTime, double?[]>>();
            cacheHistoryMax    = new Dictionary<string, double[]>();
            othersCacheHistoryMax = new Dictionary<string, double[]>();
            othersFileCache    = new Dictionary<string, FileActivityFile>();
            maxFileCache       = new Dictionary<string, string>();
            scrollInfos        = new Dictionary<string, ScrollInfo>();
            scrollDeltas       = new Dictionary<string, int>();
            fileDiskMap        = new Dictionary<string, string>();
            filteredCacheKeys  = new SortedList<string, OrderedSet<string>>();
            diskRectangles     = new Dictionary<string, Rectangle>();
            fileRectangles     = new Dictionary<string, Rectangle>();
            toggleHotSpots     = new Dictionary<string, Rectangle>();
            expandoHotSpots    = new Dictionary<string, Rectangle>();
            fileExpandedStates = new Dictionary<string, bool>();
            toolTips           = new List<TooltipInfo>();
            disksGroupRect     = new Rectangle();
            viewRect           = new Rectangle();
            fileScrollBars     = new Dictionary<string, Pair<VScrollBar, bool>>();
            mouseMoveDiskkey   = string.Empty;
            currentTooltip     = new TooltipInfo();

            // setup scroll bar the disks
            disksScrollBar         = new HScrollBar();
            disksScrollBar.Dock    = DockStyle.Bottom;
            disksScrollBar.Visible = false;
            disksScrollBar.Scroll += new ScrollEventHandler(DisksGroup_Scroll);

            this.Controls.Add(disksScrollBar);

            // setup the file type images array
            fileTypeImages = new Dictionary<FileActivityFileType, Image>();
            fileTypeImages.Add(FileActivityFileType.Data,    Properties.Resources.Database32);
            fileTypeImages.Add(FileActivityFileType.Log,     Properties.Resources.DatabaseIndex32);
            fileTypeImages.Add(FileActivityFileType.Unknown, Properties.Resources.DatabaseIndex32);

            // setup context menu
            menu = new ContextMenu();

            MenuItem expandAllDbFiles   = new MenuItem("Expand All [] Files",   new EventHandler(HandleMenuExpandAllDbFiles));
            MenuItem expandAllFiles     = new MenuItem("Expand All Files",      new EventHandler(HandleMenuExpandAllFiles));
            MenuItem collapseAllDbFiles = new MenuItem("Collapse All [] Files", new EventHandler(HandleMenuCollapseAllDbFiles));
            MenuItem collapseAllFiles   = new MenuItem("Collapse All Files",    new EventHandler(HandleMenuCollapseAllFiles));
            MenuItem navToFiles         = new MenuItem("Show Disks View",       new EventHandler(HandleShowDisksView));
            MenuItem navToDisks         = new MenuItem("Show Disk Files View",  new EventHandler(HandleShowDiskFilesView));

            MenuItem s1 = new MenuItem("-");
            MenuItem s2 = new MenuItem("-");

            menu.MenuItems.AddRange(new MenuItem[] { expandAllDbFiles, expandAllFiles, s1, collapseAllDbFiles, collapseAllFiles, s2, navToFiles, navToDisks });
                       
            // setup event handlers
            this.MouseDoubleClick += new MouseEventHandler(FileActivityPanel_MouseDoubleClick);
            this.MouseClick       += new MouseEventHandler(FileActivityPanel_MouseClick);
            this.MouseMove        += new MouseEventHandler(FileActivityPanel_MouseMove);
            this.MouseDown        += new MouseEventHandler(FileActivityPanel_MouseDown);
            this.MouseUp          += new MouseEventHandler(FileActivityPanel_MouseUp);
            this.VisibleChanged   += new EventHandler(FileActivityPanel_VisibleChanged);
            

            InitializeComponent();

            flashThread.Start();
        }        

        #endregion

        #region Event Handlers

        private void FileActivityPanel_MouseClick(object sender, MouseEventArgs e)
        {
            mouseClickLocation = e.Location;

            if (e.Button == MouseButtons.Right)
            {
                FileActivityFile file = GetFileFromPoint(e.Location);

                if (file != null)
                {
                    menu.MenuItems[0].Text = string.Format("Expand All [{0}] Files", file.DatabaseName);
                    menu.MenuItems[3].Text = string.Format("Collapse All [{0}] Files", file.DatabaseName);

                    ToggleStates states = GetToggleStates(LookupDriveKey(file.Filepath), file.DatabaseName);

                    menu.MenuItems[0].Enabled = !states.IsDbExpanded;
                    menu.MenuItems[1].Enabled = !states.IsAllExpanded;
                    menu.MenuItems[3].Enabled = !states.IsDbCollapsed;
                    menu.MenuItems[4].Enabled = !states.IsAllCollapsed;

                    menu.Show(this, e.Location);
                }
            }
            else
            {
                bool found = false;

                // loop to see if an expando hot spot was clicked
                foreach (string filekey in expandoHotSpots.Keys)
                {
                    Rectangle rect = expandoHotSpots[filekey];

                    if (rect.Contains(e.Location))
                    {
                        FileActivityFile file = GetCurrentFileCache(filekey);

                        ToggleFileExpanded(filekey);
                        found = true;
                        break;
                    }
                }

                if (found)
                    Invalidate();
            }
        }

        private void FileActivityPanel_MouseDown(object sender, MouseEventArgs e)
        {
            leftMouseDown    = e.Button == MouseButtons.Left;
            mouseMoveDiskkey = string.Empty;
            mouseMovePrevLocation = e.Location;

            // get the disk we've clicked the scroll thumb on
            foreach (string diskkey in scrollInfos.Keys)
            {
                ScrollInfo scrollInfo = scrollInfos[diskkey];

                if (scrollInfo.rects[2].Contains(e.Location))
                {
                    mouseMoveDiskkey = diskkey;
                    break;
                }
            }            
        }

        private void FileActivityPanel_MouseUp(object sender, MouseEventArgs e)
        {
            leftMouseDown    = false;
            mouseMoveDiskkey = string.Empty;

            // find which scrollbar rectangle has been clicked
            foreach (string diskkey in scrollInfos.Keys)
            {
                ScrollInfo scrollInfo = scrollInfos[diskkey];

                if (scrollInfo.rects[0].Contains(mouseClickLocation))
                {
                    HandleScrollButtonClick(diskkey, true, scrollInfo.rects[0]);
                    break;
                }

                if (scrollInfo.rects[1].Contains(mouseClickLocation))
                {
                    HandleScrollButtonClick(diskkey, false, scrollInfo.rects[1]);
                    break;
                }

                if (scrollInfo.rects[4].Contains(mouseClickLocation))
                {
                    HandleScrollLargeChangeClick(diskkey, scrollInfo, true);
                    break;
                }

                if (scrollInfo.rects[5].Contains(mouseClickLocation))
                {
                    HandleScrollLargeChangeClick(diskkey, scrollInfo, false);
                    break;
                }
            }
        }

        private void FileActivityPanel_MouseMove(object sender, MouseEventArgs e)
        {          
            if (!leftMouseDown || string.IsNullOrEmpty(mouseMoveDiskkey))
            {
                for (int i = 0; i < toolTips.Count; i++)
                {
                    if (toolTips[i].Rect.Contains(e.Location))
                    {
                        if (currentTooltip.Text == toolTips[i].Text)
                            return;
                        else if (currentTooltip.Text != toolTips[i].Text)
                            ClearTooltip();

                        currentTooltip.Rect = toolTips[i].Rect;
                        currentTooltip.Text = toolTips[i].Text;

                        mouseMovePrevLocation = e.Location;
                        DrawTooltip();                        
                        return;
                    }
                }

                // found no tooltip, clear the old one
                ClearTooltip();

                return;
            }

            int delta = GetScrollDelta(mouseMoveDiskkey);
            ScrollInfo scrollInfo = GetScrollInfo(mouseMoveDiskkey);

            delta += e.Location.Y - mouseMovePrevLocation.Y;
            
            int min = scrollInfo.rects[0].Bottom;
            int max = scrollInfo.rects[1].Top;
            int h   = scrollInfo.rects[2].Height;
            int offset = min + delta;

            if (offset >= min && offset <= (max-h))
                SetScrollDelta(mouseMoveDiskkey, delta);

            mouseMovePrevLocation = e.Location;            

            Invalidate();
        }

        private void HandleScrollButtonClick(string diskkey, bool isUp, Rectangle rect)
        {
            // scroll by x
            int delta  = GetScrollDelta(diskkey);
            int deltaX = isUp ? delta - 10 : delta + 10;

            ScrollInfo scrollInfo = GetScrollInfo(diskkey);

            int min = scrollInfo.rects[0].Bottom;
            int max = scrollInfo.rects[1].Top;
            int h   = scrollInfo.rects[2].Height;
            int offset = min + deltaX;


            if (offset < min)
            {
                for (int i = 1; i < 10; i++)
                {
                    deltaX = isUp ? deltaX + 1 : deltaX - 1;
                    offset = min + deltaX;
                    if (offset >= min)
                        break;
                }
            }

            if (offset > (max - h))
            {
                for (int i = 1; i < 10; i++)
                {
                    deltaX = isUp ? deltaX + 1 : deltaX - 1;
                    offset = min + deltaX;
                    if (offset <= (max - h))
                        break;
                }
            }

            if (offset >= min && offset <= (max - h))
                SetScrollDelta(diskkey, deltaX);

            Invalidate();
        }

        private void HandleScrollLargeChangeClick(string diskkey, ScrollInfo scrollInfo, bool isUp)
        {
            int gutterHeight = isUp ? scrollInfo.rects[4].Height : scrollInfo.rects[5].Height;
            int thumbheight  = scrollInfo.rects[2].Height;
            int deltaCurrent = GetScrollDelta(diskkey);
            int deltaOffset  = isUp ? deltaCurrent - thumbheight : deltaCurrent + thumbheight;

            if (gutterHeight < thumbheight)
                deltaOffset = isUp ? deltaCurrent - gutterHeight/2 : deltaCurrent + gutterHeight/2;

            int min = scrollInfo.rects[0].Bottom;
            int max = scrollInfo.rects[1].Top;
            int h   = scrollInfo.rects[2].Height;
            int offset = min + deltaOffset;

            if (offset >= min && offset <= (max - h))
                SetScrollDelta(diskkey, deltaOffset);

            Invalidate();
        }

        private FileActivityFile GetFileFromPoint(Point point)
        {
            foreach (string filekey in toggleHotSpots.Keys)
            {
                Rectangle rect = toggleHotSpots[filekey];

                if (rect.Contains(mouseClickLocation))
                {
                    FileActivityFile file = GetCurrentFileCache(filekey);

                    return file;
                }
            }

            return null;
        }

        private void HandleMenuExpandAllDbFiles(object sender, EventArgs e)
        {
            FileActivityFile file = GetFileFromPoint(mouseClickLocation);

            if (file == null)
                return;

            string filekey = GetFilekey(file);
            
            // expand all the files for this database
            SetAllFilesExpanded(LookupDriveKey(filekey), file.DatabaseName, true);            
            Invalidate();
        }

        private void HandleMenuExpandAllFiles(object sender, EventArgs e)
        {
            FileActivityFile file = GetFileFromPoint(mouseClickLocation);

            if (file == null)
                return;

            string filekey = GetFilekey(file);

            // expand all the files for this database
            SetAllFilesExpanded(LookupDriveKey(filekey), string.Empty, true);
            Invalidate();
        }

        private void HandleMenuCollapseAllDbFiles(object sender, EventArgs e)
        {
            FileActivityFile file = GetFileFromPoint(mouseClickLocation);

            if (file == null)
                return;

            string filekey = GetFilekey(file);
            string diskkey = LookupDriveKey(filekey);

            // expand all the files for this database
            SetAllFilesExpanded(diskkey, file.DatabaseName, false);

            // handle the case in which the scroll delta needs to be corrected
            CorrectForScrollbarOvershoot(diskkey);

            Invalidate();
        }

        private void HandleMenuCollapseAllFiles(object sender, EventArgs e)
        {
            FileActivityFile file = GetFileFromPoint(mouseClickLocation);

            if (file == null)
                return;

            string filekey = GetFilekey(file);
            string diskkey = LookupDriveKey(filekey);

            // expand all the files for this database
            SetAllFilesExpanded(diskkey, string.Empty, false);

            // handle the case in which the scroll delta needs to be corrected
            CorrectForScrollbarOvershoot(diskkey);

            Invalidate();
        }

        private void CorrectForScrollbarOvershoot(string diskkey)
        {
            ScrollInfo scrollInfo = GetScrollInfo(diskkey);
            int delta  = GetScrollDelta(diskkey);
            int min    = scrollInfo.rects[0].Bottom;
            int max    = scrollInfo.rects[1].Top;
            int h      = scrollInfo.rects[2].Height;
            int offset = min + delta;

            if (offset > (max - h))
                SetScrollDelta(diskkey, max - h);
        }

        private void HandleShowDisksView(object sender, EventArgs e)
        {
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesDisk);
        }

        private void HandleShowDiskFilesView(object sender, EventArgs e)
        {
            FileActivityFile file = GetFileFromPoint(mouseClickLocation);

            if (file == null)
            {
                ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesFiles);
                return;
            }

            ApplicationController.Default.ShowServerView(instanceId, ServerViews.DatabasesFiles, file.DatabaseName);
        }

        private void FileActivityPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bool found = false;

            // loop through the toggle hotspots and see which would we clicked inside of
            foreach (string filekey in toggleHotSpots.Keys)
            {
                Rectangle rect = toggleHotSpots[filekey];

                if (rect.Contains(e.Location))
                {
                    FileActivityFile file = GetCurrentFileCache(filekey);

                    ToggleFileExpanded(filekey);
                    found = true;
                    break;
                }
            }

            if(found)
                Invalidate();
        }

        private void FileActivityPanel_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            Invalidate();
        }

        private void DisksGroup_Scroll(object sender, ScrollEventArgs e)
        {            
            // redraw
            Invalidate();
        }

        private void ActivityFlashThreadWorker()
        {
            while (true)
            {
                int current = Interlocked.Increment(ref currentFlashStep);

                if (current > 10)
                    Interlocked.Add(ref currentFlashStep, -10);

                DrawActivityLights();

                Thread.Sleep(160);
            }
        }        

        #endregion

        #region Data methods

        public void SetFilter(FileActivityFilter filter)
        {
            this.filter = filter.Clone();

            othersCache.Clear();
            othersCacheHistoryMax.Clear();
            othersFileCache.Clear();
            maxFileCache.Clear();

            UpdateFilteredData();
        }

        private void UpdateFilteredData()
        {
            // clear keys, iterate over cache and get all that match filter
            filteredCacheKeys.Clear();

            object data = null;

            foreach (string key in cache.Keys)
            {
                data = cache[key];

                if (!(data is FileActivityFile))
                    continue;

                FileActivityFile file = (FileActivityFile)data;
                
                if (filter.DoesFileMatchFilter(file))
                {
                    string drivekey = LookupDriveKey(file.Filepath);
                    string filekey  = file.Filepath;

                    if (!filter.Drives.Contains(drivekey))
                        continue;

                    if (!filteredCacheKeys.ContainsKey(drivekey))
                        filteredCacheKeys.Add(drivekey, new OrderedSet<string>(sorter));

                    if(!filteredCacheKeys.ContainsKey(filekey))
                        filteredCacheKeys[drivekey].Add(filekey);
                }
                else
                {
                    Debug.Print("Filtered out {0}", file.Filepath);
                }
            }

            if (filter.IncludeOthers)
            {
                foreach (string diskkey in filter.Drives)
                {
                    if (!filteredCacheKeys.ContainsKey(diskkey))                        
                        filteredCacheKeys.Add(diskkey, new OrderedSet<string>());

                    filteredCacheKeys[diskkey].Add("others:" + diskkey);
                    AddFileDiskMapping(diskkey, "others:" + diskkey);
                }
            }
        }

        public void TrimData(int minutesToRetain)
        {            
            DateTime cutoff = DateTime.Now.ToUniversalTime().AddMinutes(-minutesToRetain);

            foreach (string key in cache.Keys)
            {
                if (!key.StartsWith("file:history:") && !key.StartsWith("drive:history:"))
                    continue;

                SortedList<DateTime, double?[]> history = (SortedList<DateTime, double?[]>)cache[key];
                List<DateTime> keysToRemove = new List<DateTime>();

                foreach (DateTime historykey in history.Keys)
                {
                    if (historykey < cutoff)
                        keysToRemove.Add(historykey);
                }

                foreach (DateTime keyToRemove in keysToRemove)
                    history.Remove(keyToRemove);
            }
        }
        
        /// <summary>
        /// clears out any previous data and updates internal data with table data
        /// </summary>
        /// <param name="table"></param>
        public void SetData(DataTable table)
        {
            // clear out any other data
            cache.Clear();
            othersCache.Clear();
            othersCacheHistoryMax.Clear();
            othersFileCache.Clear();
            maxFileCache.Clear();
            fileDiskMap.Clear();
            cacheHistoryMax.Clear();

            DateTime timestamp         = DateTime.Now;
            string drivename           = string.Empty;
            string databasename        = string.Empty;
            string filename            = string.Empty;
            int    filetype            = 0;
            string filepath            = string.Empty;
            long?   diskreadspersecond  = null;
            long?   diskwritespersecond = null;
            double filereadspersecond  = 0;
            double filewritespersecond = 0;

            Dictionary<DateTime, FileActivitySnapshot> snapshots = new Dictionary<DateTime, FileActivitySnapshot>();

            foreach (DataRow row in table.Rows)
            {
                timestamp           = GetAs<DateTime>(row, "timestamp");
                drivename           = GetAs<string>(row, "drive name");
                databasename        = GetAs<string>(row, "database name");
                filename            = GetAs<string>(row, "file name");
                filetype            = GetAs<int>(row, "file type");
                filepath            = GetAs<string>(row, "file path");
                if (!(row["disk reads per second"] is DBNull))
                    diskreadspersecond  = GetAs<long>(row, "disk reads per second");
                if (!(row["disk writes per second"] is DBNull))
                    diskwritespersecond = GetAs<long>(row, "disk writes per second");
                filereadspersecond  = GetAs<double>(row, "file reads per second");
                filewritespersecond = GetAs<double>(row, "file writes per second");
                // disktransfers... change repository code...

                FileActivitySnapshot snapshot = null;
                FileActivityDisk     disk     = null;
                FileActivityFile     file     = null;

                if(!snapshots.ContainsKey(timestamp))
                    snapshots.Add(timestamp, new FileActivitySnapshot(new SqlConnectionInfo()));

                snapshot = snapshots[timestamp];

                if (!snapshot.Drives.ContainsKey(drivename))
                    snapshot.Drives.Add(drivename, new FileActivityDisk());

                disk = snapshot.Drives[drivename];

                disk.DriveLetter      = drivename;
                disk.DiskReadsPerSec  = diskreadspersecond;
                disk.DiskWritesPerSec = diskwritespersecond;

                if (!disk.Files.ContainsKey(filepath))
                    disk.Files.Add(filepath, new FileActivityFile());

                file = disk.Files[filepath];

                file.DatabaseName = databasename;
                file.DriveName    = drivename;
                file.Filename     = filename;
                file.Filepath     = filepath;
                file.FileType     = (FileActivityFileType)filetype;
                file.ReadsPerSec  = filereadspersecond;
                file.WritesPerSec = filewritespersecond;
            }

            //foreach (FileActivitySnapshot snapshot in snapshots.Values)
            //    UpdateData(snapshot, -1);            

            foreach (DateTime timeStamp in snapshots.Keys)
                UpdateData(snapshots[timeStamp], -1,timeStamp);            

            UpdateFilteredData();
        }

        private T GetAs<T>(DataRow row, string column)
        {
            if (row[column] is DBNull)
                return default(T);

            return (T)row[column];
        }

        public void UpdateData(FileActivitySnapshot snapshot, int minutesToRetain)
        {
            UpdateData(snapshot, minutesToRetain, null);
        }

        /// <summary>
        /// saves data as it is appended 
        /// </summary>
        /// <param name="snapshot"></param>
        public void UpdateData(FileActivitySnapshot snapshot, int minutesToRetain, DateTime? inputTimeStamp)
        {
            DateTime timestamp = inputTimeStamp.HasValue ? inputTimeStamp.Value : 
                (snapshot.TimeStamp.HasValue ? snapshot.TimeStamp.Value : DateTime.Now);// snapshot.TimeStampLocal.HasValue ? snapshot.TimeStampLocal.Value : DateTime.Now;

            foreach (FileActivityDisk drive in snapshot.Drives.Values)
            {
                if (drive.Files.Count == 0)
                    continue;

                SaveToCache(timestamp, drive, drive.DriveLetter, drive.DiskReadsPerSec, drive.DiskWritesPerSec);

                // save the data for each file for this drive
                foreach (FileActivityFile file in drive.Files.Values)
                {
                    file.DriveName = drive.DriveLetter;

                    SaveToCache(timestamp, file, file.Filepath, file.ReadsPerSec, file.WritesPerSec);
                    
                    AddFileDiskMapping(drive.DriveLetter, file.Filepath);
                }
            }

            othersCache.Clear();
            othersCacheHistoryMax.Clear();
            othersFileCache.Clear();
            maxFileCache.Clear();

            if(minutesToRetain > 0)
                TrimData(minutesToRetain);

            UpdateFilteredData();
        }        

        private void SaveToCache(DateTime timestamp, object data, string key, double? readsPerSec, double? writesPerSec)
        {
            string type       = data is FileActivityDisk ? "drive" : "file";
            string currentkey = type + ":current:" + key.ToLower();
            string historykey = type + ":history:" + key.ToLower();

            // store the current data
            if (!cache.ContainsKey(currentkey))
                cache.Add(currentkey, data);
            else
                cache[currentkey] = data;

            // add the current data to the list of historical values
            SortedList<DateTime, double?[]> history;            
            if (!cache.ContainsKey(historykey))
            {
                history = new SortedList<DateTime, double?[]>();
                cache.Add(historykey, history);
            }
            else
            {
                history = (SortedList<DateTime, double?[]>)cache[historykey];
            }

            // timestamp for this key shouldn't exist, but make sure
            if (!history.ContainsKey(timestamp))
                history.Add(timestamp, new double?[]{ readsPerSec, writesPerSec });

            // update max cache
            double[] max = GetCacheHistoryMax(key);

            max[0] = readsPerSec.HasValue  && readsPerSec.Value  > max[0] ? readsPerSec.Value  : max[0];
            max[1] = writesPerSec.HasValue && writesPerSec.Value > max[1] ? writesPerSec.Value : max[1];
        }

        private FileActivityDisk GetCurrentDiskCache(string key)
        {
            return GetFromCache<FileActivityDisk, FileActivityDisk>(key, "current");
        }

        private FileActivityFile GetCurrentFileCache(string key)
        {
            if (key.StartsWith("others:"))
                return GetCurrentOthersFileCache(key.Substring(7));

            return GetFromCache<FileActivityFile, FileActivityFile>(key, "current");
        }


        private FileActivityFile GetCurrentOthersFileCache(string diskkey)
        {
            if (othersFileCache.ContainsKey(diskkey))
                return othersFileCache[diskkey];

            FileActivityFile others = new FileActivityFile();

            others.IsOtherFiles = true;
            others.DriveName = diskkey;
            others.Filename = filter.IsSetByUser ? "Database Files + Remaining Disk Files" : "Remaining Disk Files";
            others.DatabaseName = filter.IsSetByUser ? "All other database files and all other files on " + diskkey : "All other files on " + diskkey;
            others.Filepath = filter.IsSetByUser ? "All other files not matching the filter plus all other files on " + diskkey : "All other files on " + diskkey;
            others.FileType = FileActivityFileType.Unknown;

            double?[] data = CalculateOthersCurrentIO(diskkey, "others:" + diskkey);

            others.ReadsPerSec = data[0];
            others.WritesPerSec = data[1];

            othersFileCache.Add(diskkey, others);

            return others;
        }


        private double?[] CalculateOthersCurrentIO(string diskkey, string otherskey)
        {
            double?[] data = new double?[2];

            OrderedSet<string> filekeys = GetFilteredFileKeys(diskkey);
            FileActivityDisk   disk     = GetCurrentDiskCache(diskkey);

            if (disk == null || filekeys == null || filekeys.Count == 0)
                return data;

            double readsum  = 0;
            double writesum = 0;

            for (int i = 0; i < filekeys.Count; i++)
            {
                if (filekeys[i] == otherskey)
                    continue;

                FileActivityFile file = GetCurrentFileCache(filekeys[i]);

                if (file.ReadsPerSec.HasValue)
                    readsum += file.ReadsPerSec.Value;

                if (file.WritesPerSec.HasValue)
                    writesum += file.WritesPerSec.Value;
            }

            // Not sure why we weren't subtracting if it was 0
            //if (readsum  > 0 && disk.DiskReadsPerSec.HasValue)  data[0] = disk.DiskReadsPerSec.Value  - readsum;
            //if (writesum > 0 && disk.DiskWritesPerSec.HasValue) data[1] = disk.DiskWritesPerSec.Value - writesum;

            if (disk.DiskReadsPerSec.HasValue) data[0] = disk.DiskReadsPerSec.Value > readsum ? disk.DiskReadsPerSec.Value - readsum : 0;
            if (disk.DiskWritesPerSec.HasValue) data[1] = disk.DiskWritesPerSec.Value > writesum ? disk.DiskWritesPerSec.Value - writesum : 0;

            return data;
        }

        private SortedList<DateTime, double?[]> GetHistoryDiskCache(string key)
        {
            return GetFromCache<FileActivityDisk, SortedList<DateTime, double?[]>>(key, "history");
        }

        private SortedList<DateTime, double?[]> GetHistoryFileCache(string key)
        {
            if (key.StartsWith("others:"))
                return GetHistoryOthersFileCache(key.Substring(7));

            return GetFromCache<FileActivityFile, SortedList<DateTime, double?[]>>(key, "history");
        }

        private SortedList<DateTime, double?[]> GetHistoryOthersFileCache(string diskkey)
        {
            string otherskey = "others:" + diskkey;

            if (othersCache.ContainsKey(diskkey))
                return othersCache[diskkey];

            SortedList<DateTime, double?[]> history     = new SortedList<DateTime, double?[]>();
            SortedList<DateTime, double?[]> diskhistory = GetHistoryDiskCache(diskkey);
            OrderedSet<string>              filekeys    = GetFilteredFileKeys(diskkey);
            SortedList<DateTime, double?[]> filehistory = null;

            double?[] filedata  = null;
            double?[] otherdata = null;

            for (int i = 0; i < filekeys.Count; i++)
            {
                if (filekeys[i] == otherskey)
                    continue;

                filehistory = GetHistoryFileCache(filekeys[i]);

                if (filehistory == null || filehistory.Count == 0)
                    continue;

                for (int j = 0; j < filehistory.Keys.Count; j++)
                {
                    if (!history.ContainsKey(filehistory.Keys[j]))
                        history.Add(filehistory.Keys[j], new double?[2]{ 0, 0 });

                    filedata  = filehistory[filehistory.Keys[j]];
                    otherdata = history[filehistory.Keys[j]];

                    if (filedata[0].HasValue) otherdata[0] += filedata[0].Value;
                    if (filedata[1].HasValue) otherdata[1] += filedata[1].Value;
                }
            }

            double?[] diskdata = null;

            // loop through and subtract file values from disk to get "others"
            foreach (DateTime timekey in history.Keys)
            {
                if (!diskhistory.ContainsKey(timekey))
                    continue;

                diskdata = diskhistory[timekey];
                filedata = history[timekey];

                // calculate the reads
                if (diskdata[0].HasValue && filedata[0].HasValue)
                    filedata[0] = diskdata[0] > filedata[0] ? diskdata[0] - filedata[0] : 0;
                else if (diskdata[0].HasValue)
                    filedata[0] = diskdata[0];
                else
                    filedata[0] = null; // even if we have file data, we don't have "other" data without a disk value

                // calculate the writes
                if (diskdata[1].HasValue && filedata[1].HasValue)
                    filedata[1] = diskdata[1] > filedata[1] ? diskdata[1] - filedata[1] : 0;
                else if (diskdata[1].HasValue)
                    filedata[1] = diskdata[1];
                else
                    filedata[1] = null; // even if we have file data, we don't have "other" data without a disk value
            }

            othersCache.Add(diskkey, history);
            return history;
        }

        private K GetFromCache<T, K>(string key, string range)
        {
            string type    = typeof(T) == typeof(FileActivityDisk) ? "drive" : "file";
            string fullkey = string.Format("{0}:{1}:{2}", type, range, key).ToLower();

            if (cache.ContainsKey(fullkey))
                return (K)cache[fullkey];

            return default(K);
        }

        private double[] GetCacheHistoryMax(string key)
        {
            if (key.StartsWith("others:"))
                return GetOthersCacheHistoryMax(key.Substring(7));

            if (!cacheHistoryMax.ContainsKey(key))
                cacheHistoryMax.Add(key, new double[] { 0, 0 });

            return cacheHistoryMax[key];
        }

        private double[] GetOthersCacheHistoryMax(string diskkey)
        {
            if (othersCacheHistoryMax.ContainsKey(diskkey))
                return othersCacheHistoryMax[diskkey];

            double[] max = new double[] { 0, 0 };

            SortedList<DateTime, double?[]> history = GetHistoryOthersFileCache(diskkey);
            double?[] data = null;

            for (int i = 0; i < history.Values.Count; i++)
            {
                data = history.Values[i];

                max[0] = data[0].HasValue && data[0].Value > max[0] ? data[0].Value : max[0];
                max[1] = data[1].HasValue && data[1].Value > max[1] ? data[1].Value : max[1];   
            }            

            othersCacheHistoryMax.Add(diskkey, max);
            return max;
        }

        private void AddFileDiskMapping(string driveLetter, string filepath)
        {
            if (!fileDiskMap.ContainsKey(filepath))
                fileDiskMap.Add(filepath, driveLetter);
            else
                fileDiskMap[filepath] = driveLetter;
        }

        private string LookupDriveKey(string filekey)
        {
            if (fileDiskMap.ContainsKey(filekey))
                return fileDiskMap[filekey];

            return string.Empty;
        }

        private OrderedSet<string> GetFilteredFileKeys(string drivekey)
        {
            if (filteredCacheKeys.ContainsKey(drivekey))
                return filteredCacheKeys[drivekey];

            return new OrderedSet<string>();
        }

        private Rectangle GetFilteredFileRectangle(string filekey)
        {
            if (fileRectangles.ContainsKey(filekey))
                return fileRectangles[filekey];

            return Rectangle.Empty;
        }

        private void SetAllFilesExpanded(string drivekey, string dbname, bool isExpanded)
        {
            if (string.IsNullOrEmpty(drivekey))
                return;

            OrderedSet<string> filekeys = GetFilteredFileKeys(drivekey);

            for (int i = 0; i < filekeys.Count; i++)
            {                
                if (string.IsNullOrEmpty(dbname))
                    SetFileExpanded(filekeys[i], isExpanded);
                else
                {
                    FileActivityFile file = GetCurrentFileCache(filekeys[i]);

                    if (file != null && file.DatabaseName == dbname)
                        SetFileExpanded(filekeys[i], isExpanded);
                }
            }
        }

        private void SetFileExpanded(string filekey, bool isExpanded)
        {
            if (!fileExpandedStates.ContainsKey(filekey))
                fileExpandedStates.Add(filekey, isExpanded);
            else
                fileExpandedStates[filekey] = isExpanded;
        }

        private void ToggleFileExpanded(string filekey)
        {
            if (!fileExpandedStates.ContainsKey(filekey))
                fileExpandedStates.Add(filekey, true);
            else
                fileExpandedStates[filekey] = !fileExpandedStates[filekey];
        }        

        private bool IsFileExpanded(string filekey)
        {
            if (!fileExpandedStates.ContainsKey(filekey))
                fileExpandedStates.Add(filekey, false);

            return fileExpandedStates[filekey];
        }

        private ToggleStates GetToggleStates(string drivekey, string dbname)
        {
            if (string.IsNullOrEmpty(drivekey))
                return new ToggleStates();

            int dbExpandedCount   = 0;
            int allExpandedCount  = 0;
            int dbCollapsedCount  = 0;
            int allCollapsedCount = 0;

            ToggleStates state = new ToggleStates();
            OrderedSet<string> filekeys = GetFilteredFileKeys(drivekey);

            bool isExpanded = false;

            for (int i = 0; i < filekeys.Count; i++)
            {
                isExpanded = IsFileExpanded(filekeys[i]);

                allCollapsedCount += isExpanded ? 0 : 1;
                allExpandedCount  += isExpanded ? 1 : 0;

                if (!string.IsNullOrEmpty(dbname))
                {
                    FileActivityFile file = GetCurrentFileCache(filekeys[i]);

                    if (file != null && file.DatabaseName == dbname)
                    {
                        dbCollapsedCount += isExpanded ? 0 : 1;
                        dbExpandedCount  += isExpanded ? 1 : 0;
                    }
                }
            }

            state.IsDbExpanded   = dbCollapsedCount  == 0;
            state.IsAllExpanded  = allCollapsedCount == 0;
            state.IsDbCollapsed  = dbExpandedCount   == 0;
            state.IsAllCollapsed = allExpandedCount  == 0;

            return state;
        }

        private void AddToggleHotSpot(string filekey, Rectangle rect)
        {
            if (!toggleHotSpots.ContainsKey(filekey))
                toggleHotSpots.Add(filekey, rect);
            else
                toggleHotSpots[filekey] = rect;
        }

        private void AddExpandoHotSpot(string filekey, Rectangle rect)
        {
            if (!expandoHotSpots.ContainsKey(filekey))
                expandoHotSpots.Add(filekey, rect);
            else
                expandoHotSpots[filekey] = rect;
        }

        private string GetMaxFile(string diskkey)
        {
            if (string.IsNullOrEmpty(diskkey))
                return string.Empty;

            if (maxFileCache.ContainsKey(diskkey))
                return maxFileCache[diskkey];

            OrderedSet<string> filekeys = GetFilteredFileKeys(diskkey);

            string filekeymax = string.Empty;
            double max        = 0;
            double total      = 0;
            SortedList<DateTime, double?[]> history = null;
            double?[] data = null;

            for (int i = 0; i < filekeys.Count; i++)
            {
                history = GetHistoryFileCache(filekeys[i]);

                if (history == null || history.Count == 0)
                    continue;

                data  = history[history.Keys[history.Keys.Count-1]]; // most recent values
                total = 0;

                if (data[0].HasValue) total += data[0].Value;
                if (data[1].HasValue) total += data[1].Value;

                if (total > max && total > 0)
                {
                    max = total;
                    filekeymax = filekeys[i];
                }
            }

            maxFileCache.Add(diskkey, filekeymax);

            return filekeymax;
        }

        private int[] GetTrends(string filekey)
        {
            if (string.IsNullOrEmpty(filekey))
                return new int[] { 0, 0 };

            SortedList<DateTime, double?[]> history = GetHistoryFileCache(filekey);

            if (history.Count < 2)
                return new int[] { 0, 0 };

            int[] trends = new int[]{ -2, -2 };

            int readIdx  = 0;
            int writeIdx = 1;

            double?[] a = history.Values[history.Count - 1];
            double?[] b = history.Values[history.Count - 2];

            if (!a[readIdx].HasValue  || !b[readIdx].HasValue)  trends[readIdx]  = 0;
            if (!a[writeIdx].HasValue || !b[writeIdx].HasValue) trends[writeIdx] = 0;                       

            if (trends[readIdx] == -2)
            {
                double rdiff = a[readIdx].Value - b[readIdx].Value;

                if (rdiff == 0)
                    trends[readIdx] = 0;
                else
                    trends[readIdx] = rdiff > 0 ? 1 : -1;
            }

            if (trends[writeIdx] == -2)
            {
                double wdiff = a[writeIdx].Value - b[writeIdx].Value;

                if (wdiff == 0)
                    trends[writeIdx] = 0;
                else
                    trends[writeIdx] = wdiff > 0 ? 1 : -1;
            }

            return trends;
        }

        private void SetScrollDelta(string diskkey, int delta)
        {
            if (!scrollDeltas.ContainsKey(diskkey))
                scrollDeltas.Add(diskkey, delta);
            else
                scrollDeltas[diskkey] = delta;
        }

        private int GetScrollDelta(string diskkey)
        {
            if (scrollDeltas.ContainsKey(diskkey))
                return scrollDeltas[diskkey];

            return 0;
        }

        private ScrollInfo GetScrollInfo(string diskkey)
        {
            if (!scrollInfos.ContainsKey(diskkey))
                scrollInfos.Add(diskkey, new ScrollInfo());

            return scrollInfos[diskkey];
        }

        private void SaveTooltip(string text, Rectangle rect)
        {
            TooltipInfo info = new TooltipInfo();
            info.Rect = rect;
            info.Text = text;
            toolTips.Add(info);
        }

        private void SaveActivityLigt(Rectangle rect, bool isRead)
        {
            activityLights.Add(new Pair<Rectangle, bool>(rect, isRead));
        }

        private string GetFilekey(FileActivityFile file)
        {
            if(file.IsOtherFiles)
                return string.Format("others:" + file.DriveName);
            
            return file.Filepath;
        }

        #endregion

        #region Drawing methods

        private void UpdateRectangles(Rectangle clipRect)
        {
            // start fresh
            diskRectangles.Clear();
            fileRectangles.Clear();
            toolTips.Clear();
            activityLights.Clear();

            // update disks group rectangle (the extent of all disks rectangles)
            disksGroupRect.Y = 0;
            disksGroupRect.X = 0;
            disksGroupRect.Height = clipRect.Height;
            disksGroupRect.Width  = (filteredCacheKeys.Count * diskUiWidth) + (filteredCacheKeys.Count * diskUiMargin) + diskUiMargin;
            
            // update the view rectangle (it's our view relative to the disks group rectangle)
            viewRect.Y      = 0;
            viewRect.X      = disksScrollBar.Value;
            viewRect.Width  = clipRect.Width;
            viewRect.Height = clipRect.Height;
            
            // update the drives
            UpdateDriveRectangles();
        }

        private void UpdateDriveRectangles()
        {
            string diskkey = string.Empty;

            for (int i = 0; i < filteredCacheKeys.Keys.Count; i++)
            {
                diskkey = filteredCacheKeys.Keys[i];

                Rectangle d = new Rectangle();

                d.Width  = diskUiWidth;
                d.Height = disksGroupRect.Height - (2 * diskUiMargin);
                d.Y      = diskUiMargin;
                d.X      = (i * diskUiWidth) + ((i + 1) * diskUiMargin);
               
                // save to the disk rectangles list
                diskRectangles.Add(diskkey, d);

                // now, get the filekeys for this disk and create the file rectangles
                OrderedSet<string> filekeys = filteredCacheKeys[diskkey];
                int y = fileUiMargin + diskUiHeaderHeight;

                // one rectangle for each file(path)
                for (int j = 0; j < filekeys.Count; j++)
                {
                    Rectangle f = new Rectangle();

                    f.Width  = diskUiWidth - (2 * fileUiMargin);
                    f.Height = IsFileExpanded(filekeys[j]) ? fileUiExpandedHeight : fileUiCollapsedHeight;
                    f.X      = d.X + fileUiMargin;
                    f.Y      = y;

                    fileRectangles.Add(filekeys[j], f);
                
                    // update the y location (can't just multiply since some files may be expanded/collapsed)
                    y += f.Height + fileUiMargin;                    
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.Visible)
                return;

            lock (paintlock)
            {
                Graphics  graphics = e.Graphics;
                Rectangle clipRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width, ClientRectangle.Height - 17);

                writeSizeLabel = graphics.MeasureString(writesPerSecondText, this.Font);
                readSizeLabel  = graphics.MeasureString(readsPerSecondText, this.Font);

                // get the rectangles for use to paint inside of
                UpdateRectangles(clipRect);

                // check if we need to show scroll bars for the drives
                disksScrollBar.Minimum = 0;
                disksScrollBar.Maximum = disksGroupRect.Width;
                disksScrollBar.LargeChange = viewRect.Width;
                disksScrollBar.Visible = disksGroupRect.Width > clipRect.Width;

                // draw the background
                graphics.FillRectangle(controlBackColorBrush, clipRect);

                // draw drives and files
                DrawDrives(graphics);
            }            
        }

        private void DrawDrives(Graphics graphics)
        {            
            Dictionary<string, Rectangle>.Enumerator e = diskRectangles.GetEnumerator();
            
            int    left               = 0;
            int    headerTitleYOffset = 0;
            int    headerDataYOffset  = 0;
            int    headerDataXOffset  = 0;
            int    scrollDelta        = 0;
            int    filesHeight        = 0;
            string reads              = string.Empty;
            string writes             = string.Empty;
            string transfers          = string.Empty;            
            bool   showScrollbar      = false;                        
            bool   isLeftAligned      = false;
            bool   isRightAligned     = false;
            bool   isTopAligned       = false;
            bool   isMaxfile          = false;
            double viewRatio          = 0;
            Rectangle lastRect           = Rectangle.Empty;
            Rectangle filesClipRectangle = Rectangle.Empty;
            Rectangle intersectRect      = Rectangle.Empty;

            // we only know the hot spots after rendering, so start over
            toggleHotSpots.Clear();
            expandoHotSpots.Clear();

            // loop through all the drive rectangles
            while (e.MoveNext())
            {
                Rectangle diskRect = e.Current.Value;
                
                if (!viewRect.IntersectsWith(diskRect))
                    continue;

                // get the data for this disk
                FileActivityDisk disk = GetCurrentDiskCache(e.Current.Key);

                if (disk == null)
                    continue;
                
                // create the rectangle for the disk
                Rectangle offsetRect = diskRect;
                offsetRect.Intersect(viewRect);
                
                isLeftAligned  = diskRect.X == offsetRect.X;
                isRightAligned = (diskRect.X + diskRect.Width) == (offsetRect.X + offsetRect.Width);

                offsetRect.X -= viewRect.X;                

                // init header values
                left               = isLeftAligned ? offsetRect.X + 5 : offsetRect.X - (diskRect.Width - offsetRect.Width) + 5;
                headerTitleYOffset = 10; // top of title text
                headerDataYOffset  = headerTitleYOffset + 20; // top of reads/writes per second text
                headerDataXOffset  = 130; // left side of writes/sec text

                // draw the background
                graphics.FillRectangle(diskBackColorBrush, offsetRect);

                // draw the header
                TextRenderer.DrawText(graphics, "Drive: " + disk.DriveLetter, this.Font, new Rectangle(left, diskRect.Top + headerTitleYOffset, diskRect.Width, 20), Color.Black, TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping);
                SaveTooltip("Drive: " + disk.DriveLetter, new Rectangle(left, diskRect.Top + headerTitleYOffset, diskRect.Width, 25));

                reads = !disk.DiskReadsPerSec.HasValue
                            ? "Reads/Sec: None"
                            : disk.DiskReadsPerSec >= 1000
                                  ? string.Format("Reads/Sec: {0:####0}", disk.DiskReadsPerSec)
                                  : string.Format("Reads/Sec: {0:####0.##}", disk.DiskReadsPerSec);
                writes = !disk.DiskWritesPerSec.HasValue
                            ? "Writes/Sec: None"
                            : disk.DiskWritesPerSec >= 1000
                                  ? string.Format("Writes/Sec: {0:####0}", disk.DiskWritesPerSec)
                                  : string.Format("Writes/Sec: {0:####0.##}", disk.DiskWritesPerSec);
                //transfers = disk.DiskTransfersPerSec.HasValue ? string.Format("Transfers/Sec: {0:####0.##}", disk.DiskTransfersPerSec) : "Transfers/Sec: None";

                if (disk.DiskReadsPerSec.HasValue && disk.DiskReadsPerSec.Value == 0)         reads     = "Reads/Sec: None";
                if (disk.DiskWritesPerSec.HasValue && disk.DiskWritesPerSec.Value == 0) writes = "Writes/Sec: None";
                if (disk.DiskTransfersPerSec.HasValue && disk.DiskTransfersPerSec.Value == 0) transfers = "Transfers/Sec: None";
                
                graphics.DrawString(reads,     this.Font, Brushes.Black, left,                     diskRect.Top + headerDataYOffset);
                graphics.DrawString(writes,    this.Font, Brushes.Black, left + headerDataXOffset, diskRect.Top + headerDataYOffset);
                //graphics.DrawString(transfers, this.Font, Brushes.Black, left + headerDataXOffset + 130, diskRect.Top + headerDataYOffset);

                // get the filtered files for this disk
                OrderedSet<string> filekeys = GetFilteredFileKeys(disk.DriveLetter);

                // handle some scroll bar drawing
                lastRect      = GetFilteredFileRectangle(filekeys[filekeys.Count - 1]);                
                showScrollbar = isRightAligned && ((lastRect.Bottom + fileUiMargin) > offsetRect.Height);
                scrollDelta   = GetScrollDelta(e.Current.Key);                

                filesClipRectangle = new Rectangle(offsetRect.Left, 55, offsetRect.Width, offsetRect.Height - 53);
                filesHeight        = lastRect.Bottom + fileUiMargin;
                viewRatio          = (double)filesClipRectangle.Height / (double)filesHeight;
                int maxscrollHeight = 0;
                double scrollPixels = 0;

                if (showScrollbar)
                {
                    maxscrollHeight = DrawScrollbar(graphics, e.Current.Key, offsetRect, ref scrollDelta, viewRatio);
                    scrollPixels = (double)(filesHeight - (filesClipRectangle.Height + (fileUiMargin + diskUiHeaderHeight))) / (double)maxscrollHeight;

                    scrollPixels += ((diskRect.Height) * filesHeight / 200000) / (double)maxscrollHeight;
                }

                // draw the file rectangles
                for (int i = 0; i < filekeys.Count; i++)
                {                    
                    Rectangle fileRect = GetFilteredFileRectangle(filekeys[i]);                    

                    intersectRect = fileRect;
                    intersectRect.IntersectsWith(offsetRect);
                    isTopAligned = fileRect.Y == offsetRect.Y;                    
                    intersectRect.X -= viewRect.X;

                    if (showScrollbar)
                    {
                        intersectRect.Width -= 16;
                        intersectRect.Y     -= (int)(scrollDelta * scrollPixels);//(int)((double)scrollDelta / viewRatio);                    
                    }

                    if (!offsetRect.IntersectsWith(intersectRect))
                        continue;

                    isMaxfile = GetMaxFile(e.Current.Key) == filekeys[i];

                    // draw file details
                    Region clip = graphics.Clip;
                    graphics.SetClip(filesClipRectangle);                    
                    DrawFile(graphics, intersectRect, filekeys[i], isMaxfile);
                    graphics.Clip = clip;
                }

                // draw the disk outline
                graphics.DrawRectangle(outlinePen, offsetRect);
            }            
        }

        private int DrawScrollbar(Graphics graphics, string diskkey, Rectangle offsetRect, ref int scrollDelta, double viewRatio)
        {
            int t = offsetRect.Top + 50;
            int h = (offsetRect.Height - 56) + fileUiMargin;
            int l = (offsetRect.Left + offsetRect.Width) - 18;
            int w = 18;

            int maxScrollHeight = 0;
            int thumbOffset     = t + 16 + scrollDelta;

            Rectangle scrollRect  = new Rectangle(l, t, w, h);
            Rectangle arrowUpRect = new Rectangle(l, t, 16, 15);
            Rectangle arrowDnRect = new Rectangle(l, scrollRect.Bottom - 14, 16, 14);

            int thumbHeight = (int)((scrollRect.Height - (arrowDnRect.Height + arrowUpRect.Height)) * viewRatio);

            if (thumbHeight < 4)
                thumbHeight = 4;

            if (thumbOffset < arrowUpRect.Bottom)
                thumbOffset = arrowUpRect.Bottom;

            if (thumbOffset + thumbHeight > arrowDnRect.Top)
            {
                thumbOffset = arrowDnRect.Top - thumbHeight;
                scrollDelta = thumbOffset - t - 16;
                SetScrollDelta(diskkey, scrollDelta);
            }

            Rectangle thumbRect          = new Rectangle(l+1, thumbOffset, 15, thumbHeight);
            Rectangle thumbGrip          = new Rectangle(thumbRect.Left + 2, thumbRect.Top + (thumbRect.Height / 2) - 6, 11, 12);
            Rectangle thumbRectCapTop    = new Rectangle(thumbRect.Left, thumbRect.Top, thumbRect.Width, 6);
            Rectangle thumbRectCapBottom = new Rectangle(thumbRect.Left, thumbRect.Bottom-6, thumbRect.Width, 6);

            bool drawcaps = thumbRect.Height > 12;

            graphics.DrawImage(Properties.Resources.scrollbar_thumb_background, scrollRect);
            graphics.DrawImage(Properties.Resources.scrollbar_arrowup,    arrowUpRect);
            graphics.DrawImage(Properties.Resources.scrollbar_arrowdown,  arrowDnRect);
            graphics.DrawImage(Properties.Resources.scrollbar_thumb_box,  thumbRect);

            if (drawcaps)
            {
                graphics.DrawImage(Properties.Resources.scrollbar_thumb_grip, thumbGrip);
                graphics.DrawImage(Properties.Resources.scrollbar_thumb_box_endcap_top, thumbRectCapTop);
                graphics.DrawImage(Properties.Resources.scrollbar_thumb_box_endcap_bottom, thumbRectCapBottom);
            }
            else
                graphics.DrawRectangle(Pens.DarkGray, thumbRect);

            ScrollInfo scrollInfo = GetScrollInfo(diskkey);

            scrollInfo.rects[0] = arrowUpRect;
            scrollInfo.rects[1] = arrowDnRect;
            scrollInfo.rects[2] = thumbRect;
            scrollInfo.rects[3] = new Rectangle(l, t, w, arrowDnRect.Top - t);
            scrollInfo.rects[4] = new Rectangle(l, arrowUpRect.Bottom, scrollRect.Width, thumbRect.Top   - arrowUpRect.Bottom);
            scrollInfo.rects[5] = new Rectangle(l, thumbRect.Bottom,   scrollRect.Width, arrowDnRect.Top - thumbRect.Bottom);

            maxScrollHeight = arrowDnRect.Top - thumbHeight - arrowUpRect.Bottom;

            return maxScrollHeight;
        }

        private void DrawFile(Graphics graphics, Rectangle fileRect, string filekey, bool isMaxFile)
        {            
            FileActivityFile file = GetCurrentFileCache(filekey);

            if (file == null)
                return;

            // store the clickable area
            AddToggleHotSpot(filekey, new Rectangle(fileRect.X, fileRect.Y, fileRect.Width, fileRect.Height));

            // setup the variables for rendering (shortcuts, offsets, etc)
            int x0        = fileRect.Left;
            int y0        = fileRect.Top;
            int r0        = fileRect.Right;
            int topline   = y0 + 2;
            int toplineX  = y0 + 40;
            int margin    = 4;
            int dbTypeIconWidth   = 32;            
            int activityIconWidth = 16;

            // draw file background
            SolidBrush backbrush = file.IsOtherFiles ? fileOthersBackColorBrush : fileBackColorBrush;
            if (isMaxFile) backbrush = fileBackColorMaxBrush;
            graphics.FillRectangle(backbrush, fileRect);

            // draw the icon
            graphics.FillRectangle(fileIconBackColorBrush, new Rectangle(x0+2, y0+2, 32, 32));

            if(file.IsOtherFiles)
                graphics.DrawImage(Properties.Resources.Documents_32, x0 + 2, y0 + 2);
            else
                graphics.DrawImage(fileTypeImages[file.FileType], x0+2, y0+2);

            // draw the logical filename
            TextRenderer.DrawText(graphics, file.Filename, this.Font, new Rectangle(x0 + dbTypeIconWidth + margin, topline, fileRect.Width-74, 20), Color.Black, TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping);

            // draw the database name            
            TextRenderer.DrawText(graphics, file.DatabaseName, this.Font, new Rectangle(x0 + dbTypeIconWidth + margin, y0 + 20, fileRect.Width - 74, 20), file.IsOtherFiles ? Color.DarkGray : Color.SteelBlue, TextFormatFlags.EndEllipsis | TextFormatFlags.PreserveGraphicsClipping);

            // smooth the activity lights
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // draw the activity lights
            SolidBrush readBrush  = file.ReadsPerSec.HasValue  ? fileReadActivityBrush  : fileEmptyActivityBrush;
            SolidBrush writeBrush = file.WritesPerSec.HasValue ? fileWriteActivityBrush : fileEmptyActivityBrush;

            if (file.ReadsPerSec.HasValue && file.ReadsPerSec.Value == 0)   readBrush  = fileEmptyActivityBrush;
            if (file.WritesPerSec.HasValue && file.WritesPerSec.Value == 0) writeBrush = fileEmptyActivityBrush;

            Rectangle readLightRect  = new Rectangle(r0 - (2 * activityIconWidth) - (2 * margin), topline, 16, 16);
            Rectangle writeLightRect = new Rectangle(r0 - activityIconWidth - margin,             topline, 16, 16);

            graphics.FillEllipse(readBrush,  readLightRect);
            graphics.FillEllipse(writeBrush, writeLightRect);

            // save tooltips for the lights
            string readActivityTip = "No activity";
            if (file.ReadsPerSec.HasValue && file.ReadsPerSec.Value > 0)
            {
                if (file.ReadsPerSec.Value > 1 && file.ReadsPerSec.Value < 1.01)
                {
                    readActivityTip = string.Format("{0:####0.##} read per second", file.ReadsPerSec.Value);
                }
                else if (file.ReadsPerSec.Value < 0.01)
                {
                    readActivityTip = "Less than 0.01 read per second";
                }
                else
                {
                    readActivityTip = string.Format("{0:####0.##} reads per second", file.ReadsPerSec.Value);
                }
            }
            string writeActivityTip = "No activity";
            if (file.WritesPerSec.HasValue && file.WritesPerSec.Value > 0)
            {
                if (file.WritesPerSec.Value > 1 && file.WritesPerSec.Value < 1.01)
                {
                    writeActivityTip = string.Format("{0:####0.##} write per second", file.WritesPerSec.Value);
                }
                else if (file.WritesPerSec.Value < 0.01)
                {
                    writeActivityTip = "Less than 0.01 write per second";
                }
                else
                {
                    writeActivityTip = string.Format("{0:####0.##} writes per second", file.WritesPerSec.Value);
                }
            }

            // save tooltips
            SaveTooltip(file.Filepath,     new Rectangle(x0 + dbTypeIconWidth + margin, topline, fileRect.Width - 80, 15));
            SaveTooltip(file.DatabaseName, new Rectangle(x0 + dbTypeIconWidth + margin, y0 + 19, fileRect.Width - 80, 15));
            SaveTooltip(readActivityTip,   readLightRect);
            SaveTooltip(writeActivityTip,  writeLightRect);

            // save the activity light rects
            if (file.ReadsPerSec.HasValue  && file.ReadsPerSec.Value  > 0 && readLightRect.Top  > 50 && readLightRect.Bottom  < graphics.ClipBounds.Bottom) SaveActivityLigt(readLightRect,  true);
            if (file.WritesPerSec.HasValue && file.WritesPerSec.Value > 0 && writeLightRect.Top > 50 && writeLightRect.Bottom < graphics.ClipBounds.Bottom) SaveActivityLigt(writeLightRect, false);            

            // draw the file expand/collapse icon
            Rectangle toggleRect   = new Rectangle(writeLightRect.Left + 4, writeLightRect.Bottom + 6, 8, 6);
            Point[]   togglePoints = IsFileExpanded(filekey)
                ? 
                    // arrow up
                    new Point[] 
                    { 
                        new Point(toggleRect.Left,  toggleRect.Bottom), 
                        new Point(toggleRect.Right, toggleRect.Bottom), 
                        new Point(toggleRect.Left + toggleRect.Width/2, toggleRect.Top) 
                    }                    
                :
                    // arrow down
                    new Point[] 
                    { 
                        new Point(toggleRect.Left,  toggleRect.Top), 
                        new Point(toggleRect.Right, toggleRect.Top), 
                        new Point(toggleRect.Left + toggleRect.Width/2, toggleRect.Bottom) 
                    }
                ;

            graphics.FillPolygon(file.IsOtherFiles ? Brushes.DarkGray : Brushes.SteelBlue, togglePoints);
            AddExpandoHotSpot(filekey, toggleRect);

            // set smoothing back to default
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;

            // if collapsed, then draw a mini histogram
            if (!IsFileExpanded(filekey))
            {
                int x  = x0 + 1;
                int y  = toplineX - margin;
                int h  = 13;
                int w  = fileRect.Width - 2;
                int w2 = w / 2;
                
                graphics.FillRectangle(histogramBotBackBrush, new Rectangle(x, y, w, h));

                Rectangle readHistoRect  = new Rectangle(x,          y, w2-2, h);
                Rectangle writeHistoRect = new Rectangle(x + w2 + 2, y, w2-2, h);

                DrawHistogram(graphics, readHistoRect,  filekey, true);
                DrawHistogram(graphics, writeHistoRect, filekey, false);

                // draw divider between histograms
                graphics.DrawLine(histogramDividerPen, x + w2, y, x + w2, y + h);

                Rectangle readHistoTipRect = readHistoRect;
                Rectangle writeHistoTipRect = writeHistoRect;

                readHistoTipRect.Inflate(-2, -2);
                writeHistoTipRect.Inflate(-2, -2);

                SaveTooltip("Reads Per Second", readHistoTipRect);
                SaveTooltip("Writes Per Second", writeHistoTipRect);
            }
            else // else, draw full graph and text labels for activity
            {
                // draw the chart
                Rectangle chartRect = new Rectangle(x0 + margin, toplineX, fileRect.Width - (2 * margin), 230);
                graphics.FillRectangle(fileChartBackColorBrush, chartRect);
                graphics.DrawRectangle(Pens.LightGray, chartRect);

                DrawChartData(graphics, chartRect, filekey, true);  // draw read data
                DrawChartData(graphics, chartRect, filekey, false); // draw write data

                // draw activity labels
                string readTextValue  = !file.ReadsPerSec.HasValue ? "None" :
                                        file.ReadsPerSec >= 1000
                                            ? string.Format("{0:####0}", file.ReadsPerSec.Value)
                                            : string.Format("{0:####0.##}", file.ReadsPerSec.Value);
                string writeTextValue = !file.WritesPerSec.HasValue ? "None" :
                                        file.WritesPerSec >= 1000
                                            ? string.Format("{0:####0}", file.WritesPerSec.Value)
                                            : string.Format("{0:####0.##}", file.WritesPerSec.Value);

                if (file.ReadsPerSec.HasValue  && file.ReadsPerSec.Value == 0)  readTextValue  = "None";
                if (file.WritesPerSec.HasValue && file.WritesPerSec.Value == 0) writeTextValue = "None";

                // draw activity labels and trend arrows
                int[] trends = GetTrends(filekey);

                SizeF readSizeValue  = graphics.MeasureString(readTextValue, this.Font);
                SizeF writeSizeValue = graphics.MeasureString(writeTextValue, this.Font);

                int readValueOffset  = trends[0] == 0 ? 0 : 16;
                int writeValueOffset = trends[1] == 0 ? 0 : 16;

                int top = chartRect.Top + chartRect.Height + margin;

                // labels
                graphics.DrawString(readsPerSecondText, this.Font, Brushes.Black, chartRect.Left, top);
                graphics.DrawString(readTextValue, this.Font, Brushes.Black, chartRect.Left + readValueOffset + readSizeLabel.Width, top);

                float writeLeft = r0 - writeSizeLabel.Width - writeValueOffset - writeSizeValue.Width - margin;

                // value text
                graphics.DrawString(writesPerSecondText, this.Font, Brushes.Black, writeLeft, top);
                graphics.DrawString(writeTextValue, this.Font, Brushes.Black, r0 - writeSizeValue.Width - margin, top);

                // trend arrows
                Image readArrow  = trends[0] > 0 ? Properties.Resources.arrow_up_green_16x16_plain  : Properties.Resources.arrow_down_green_16x16_plain;
                Image writeArrow = trends[1] > 0 ? Properties.Resources.arrow_up_blue_16x16_plain : Properties.Resources.arrow_down_blue_16x16_plain;
                                
                if (trends[0] != 0) graphics.DrawImage(readArrow,  chartRect.Left + readSizeLabel.Width, top, 16, 16);
                if (trends[1] != 0) graphics.DrawImage(writeArrow, writeLeft + writeSizeLabel.Width, top, 16, 16);                              
            }
        }

        private void DrawHistogram(Graphics graphics, Rectangle rect, string filekey, bool isRead)
        {
            // get the data history
            SortedList<DateTime, double?[]> history = GetHistoryFileCache(filekey);
            double[] max = GetCacheHistoryMax(filekey);
            int      idx = isRead ? 0 : 1;

            if (history == null || max[idx] == 0)
                return;

            float y0 = rect.Bottom; 
            float y1 = 0f;
            float h  = rect.Height - 1;
            float x  = rect.Right  - 2;

            double?[] data = null;
            Pen pen = isRead ? readHistogramPen : writeHistorgramPen;

            for (int i = history.Count - 1; i >= 0; i--)
            {
                data = history.Values[i];

                if (x < rect.Left)
                    break;

                if (data[idx].HasValue)
                {
                    y1 = 0;
                    if(max[idx] > 0)
                        y1 = (float)(h * (data[idx].Value / max[idx]));
                    if (y1 > h || y1 < 0)
                    {
                        Log.ErrorFormat(
                        "Method: DrawHistogram cache history max ({0}) is lower than current data value ({1}) at key ({2}) in filekey ({3}) with y1 = {4} at index {5}",
                        max[idx], data[idx].Value, history.Keys[i], filekey, y1, i);
                    }
                    graphics.DrawLine(pen, x, y0, x, y0 - y1);
                }
                else
                    graphics.DrawLine(pen, x, y0, x, y0); // zero height line for empty values

                // move left
                x -= 2;
            }
        }

        private void DrawChartData(Graphics graphics, Rectangle rect, string filekey, bool isRead)
        {
            // get the data history
            SortedList<DateTime, double?[]> history = GetHistoryFileCache(filekey);
            double[] max = GetCacheHistoryMax(filekey);
            int idx = isRead ? 0 : 1;

            if (history == null || max[idx] == 0)
                return;

            int   y0 = rect.Bottom - 3;
            float y1 = 0f;
            float h  = rect.Height - 6;
            int   x  = rect.Right;
            int   markerWidth     = 6;
            int   markerWidthHalf = 3;

            double?[] data = null;

            SolidBrush  brush  = isRead ? fileReadActivityBrush : fileWriteActivityBrush;
            Pen         pen    = isRead ? readLinePen           : writeLinePen;
            List<Point> points = new List<Point>();

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            for (int i = history.Count - 1; i >= 0; i--)
            {
                data = history.Values[i];

                if (x < rect.Left)
                    break;

                if (data[idx].HasValue)
                {
                    y1 = 0;
                    if(max[idx] > 0)
                        y1 = (float)(h * (data[idx].Value / max[idx]));
                    if (y1 > h || y1 < 0)
                    {
                        Log.ErrorFormat(
                        "Method: DrawHistogram cache history max ({0}) is lower than current data value ({1}) at key ({2}) in filekey ({3}) with y1 = {4} at index {5}",
                        max[idx], data[idx].Value, history.Keys[i], filekey, y1, i);
                    }
                    points.Add(new Point(x, y0 - (int)y1));

                    Rectangle marker = new Rectangle(x - markerWidthHalf, (y0 - (int)y1) - markerWidthHalf, markerWidth, markerWidth);
                    graphics.FillEllipse(brush, marker);

                    SaveTooltip(string.Format("{0:####0.##} {1} per second at {2}", data[idx].Value, isRead ? "reads" : "writes", history.Keys[i].ToLocalTime().ToString()), new Rectangle(x, y0 - (int)y1, 10, 10));
                }
                else
                    points.Add(new Point(x, rect.Bottom));


                x -= 10;
            }

            for (int i = 0; i < points.Count - 1; i++)
                graphics.DrawLine(pen, points[i], points[i + 1]);

            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.Default;
        }

        private void DrawTooltip()
        {            
            Graphics graphics = this.CreateGraphics();

            SizeF     size    = graphics.MeasureString(currentTooltip.Text, this.Font);
            Rectangle tipRect = new Rectangle(mouseMovePrevLocation.X, mouseMovePrevLocation.Y-(int)size.Height, (int)size.Width, (int)size.Height);

            tipRect.Inflate(4, 4);

            graphics.FillRectangle(Brushes.LightYellow, tipRect);
            graphics.DrawRectangle(Pens.Black,  tipRect);
            graphics.DrawString(currentTooltip.Text, this.Font, Brushes.Black, tipRect.X+ 4, tipRect.Y + 4);

            currentTooltip.Rect = tipRect;
        }

        private void ClearTooltip()
        {
            Graphics graphics = this.CreateGraphics();

            if (string.IsNullOrEmpty(currentTooltip.Text))
                return;

            currentTooltip.Text = string.Empty;

            currentTooltip.Rect.Inflate(100, 100); // make sure we repaint any smears

            Invalidate(currentTooltip.Rect);
        }

        private void DrawActivityLights()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(DrawActivityLights));
                return;
            }

            if (this.IsDisposed)
                return;

            int step = currentFlashStep;

            if (step > 10) 
                step = 0;

            lock (paintlock)
            {
                if (!this.Visible || activityLights.Count == 0)
                    return;

                using (Graphics graphics = this.CreateGraphics())
                {
                    for (int i = 0; i < activityLights.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(currentTooltip.Text) &&
                            currentTooltip.Rect.IntersectsWith(activityLights[i].First))
                            continue;

                        if (activityLights[i].Second == true)
                            graphics.FillEllipse(readActivityBrushes[step], activityLights[i].First);
                        else
                            graphics.FillEllipse(writeActivityBrushes[step], activityLights[i].First);
                    }
                }
            }
        }

        #endregion

        #region Private types

        private struct ToggleStates
        {
            public bool IsDbExpanded;
            public bool IsAllExpanded;
            public bool IsDbCollapsed;
            public bool IsAllCollapsed;
        }

        private class TooltipInfo
        {
            public Rectangle Rect;
            public string Text;
        }

        private class ScrollInfo
        {
            public Rectangle[] rects;

            public ScrollInfo()
            {
                rects = new Rectangle[]
                {
                    new Rectangle(), // up
                    new Rectangle(), // down
                    new Rectangle(), // thumb
                    new Rectangle(), // gutter
                    new Rectangle(), // top gutter
                    new Rectangle()  // bottom gutter
                };
            }
        }

        private class Sorter : IComparer<string>
        {
            FileActivityPanel parent;

            public Sorter(FileActivityPanel parent)
            {
                this.parent = parent;
            }

            #region IComparer<string> Members

            public int Compare(string x, string y)
            {
                FileActivityFile a = parent.GetCurrentFileCache(x);
                FileActivityFile b = parent.GetCurrentFileCache(y);

                // QA has databases where the filename is a single space, which is trimmed to null
                // This only seems to cause a problem when sorting
                if (a.Filename == null)
                    a.Filename = " ";
                if (b.Filename == null)
                    b.Filepath = " ";
                
                double a1;
                double b1;

                if (a.IsOtherFiles) return 1;
                if (b.IsOtherFiles) return -1;

                int d = parent.sortDirection == FileActivitySortDirection.Down ? 1 : -1;

                if (parent.sort == FileActivitySort.Databasename)
                {
                    if(a.DatabaseName == b.DatabaseName)
                        return d * a.Filename.CompareTo(b.Filename);

                    return d * a.DatabaseName.CompareTo(b.DatabaseName);
                }

                if (parent.sort == FileActivitySort.Reads)
                {
                    //if (!a.ReadsPerSec.HasValue) return d;
                    //if (!b.ReadsPerSec.HasValue) return d;

                    //if (a.ReadsPerSec.Value == b.ReadsPerSec.Value)
                    //    return d * a.Filename.CompareTo(b.Filename);

                    //return d * a.ReadsPerSec.Value.CompareTo(b.ReadsPerSec.Value);
                    a1 = (a.ReadsPerSec.HasValue) ? a.ReadsPerSec.Value : 0;
                    b1 = (b.ReadsPerSec.HasValue) ? b.ReadsPerSec.Value : 0;

                    if (a1 == b1)
                        return d * a.Filename.CompareTo(b.Filename);

                    return d * a1.CompareTo(b1);
                }

                if (parent.sort == FileActivitySort.Writes)
                {
                    //if (!a.WritesPerSec.HasValue) return d;
                    //if (!b.WritesPerSec.HasValue) return d;

                    //if (a.WritesPerSec.Value == b.WritesPerSec.Value)
                    //    return d * a.Filename.CompareTo(b.Filename);

                    //return d * a.WritesPerSec.Value.CompareTo(b.WritesPerSec.Value);

                    a1 = (a.WritesPerSec.HasValue) ? a.WritesPerSec.Value : 0;
                    b1 = (b.WritesPerSec.HasValue) ? b.WritesPerSec.Value : 0;

                    if (a1 == b1)
                        return d * a.Filename.CompareTo(b.Filename);

                    return d * a1.CompareTo(b1);
                }

                return d * a.Filename.CompareTo(b.Filename);
            }

            #endregion
        }

        #endregion
    }

    public class FileActivityFilter
    {
        public List<string> Drives;
        public List<string> Databases;
        public List<string> Filetypes;
        public string Filename;
        public string Filepath;
        public bool IsSetByUser; // set to true when navigating from another view, or when the user sets a value, then don't change on refreshes if true
        public bool IncludeOthers = true;

        public FileActivityFilter()
        {
            Drives = new List<string>();
            Databases = new List<string>();
            Filetypes = new List<string>();
            Filename = string.Empty;
            Filepath = string.Empty;
            IsSetByUser = false;
        }

        public void Clear()
        {
            IsSetByUser = false;
            Drives.Clear();
            Databases.Clear();
            Filetypes.Clear();
            Filename = string.Empty;
            Filepath = string.Empty;
        }

        public bool DoesFileMatchFilter(FileActivityFile file)
        {
            if (!string.IsNullOrEmpty(Filename))
            {

                string filenamelike = "^(" + Regex.Escape(Filename).Replace("%", ".*") + ")$";

                if (!Regex.IsMatch(file.Filename, filenamelike, RegexOptions.IgnoreCase))
                    return false;
            }

            if (!string.IsNullOrEmpty(Filepath))
            {
                string filepathlike = "^(" + Regex.Escape(Filepath).Replace("%", ".*") + ")$";

                if (!Regex.IsMatch(file.Filepath, filepathlike, RegexOptions.IgnoreCase))
                    return false;
            }

            if (!Filetypes.Contains(file.FileType.ToString()))
                return false;

            if (!Databases.Contains(file.DatabaseName))
                return false;

            return true;
        }

        public bool IsOnlyOthers()
        {
            return IncludeOthers && Filetypes.Count == 0;
        }

        public void AddFileType(string fileType)
        {
            if (!Filetypes.Contains(fileType))
                Filetypes.Add(fileType);
        }

        public void RemoveFileType(string fileType)
        {
            if (Filetypes.Contains(fileType))
                Filetypes.Remove(fileType);
        }

        public FileActivityFilter Clone()
        {
            FileActivityFilter f = new FileActivityFilter();

            f.Databases.AddRange(this.Databases);
            f.Drives.AddRange(this.Drives);
            f.Filetypes.AddRange(this.Filetypes);
            f.Filename      = this.Filename;
            f.Filepath      = this.Filepath;            
            f.IncludeOthers = this.IncludeOthers;
            f.IsSetByUser   = this.IsSetByUser;

            return f;
        }
    }
}
