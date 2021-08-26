using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public enum DatabaseFileActivityType { Normal, Max }
    public enum DatabaseFileCategory { Normal, Other }

    public partial class DatabaseFileActivity : UserControl
    {
        private FileActivityFile fileInfo;
        private Dictionary<FileActivityFileType, Image> fileActivityImageMap;
        private List<double> readData;
        private List<double> writeData;
        private double totalDriveReadsPerSecond;
        private double totalDriveWritesPerSecond;

        private string READSFORMAT  = "{0:####.##}";
        private string WRITESFORMAT = "{0:####.##}";

        private bool isExpanded;
        private DatabaseFileActivityType activityType;
        private DatabaseFileCategory category;
        private ToolTip tooltip;

        public bool IsExpanded
        {            
            get { return isExpanded; }
            set { isExpanded = value; UpdateControls(); Notify(OnToggled); }
        }

        public DatabaseFileActivityType ActivityType
        {
            get { return activityType; }
            set { activityType = value; UpdateControls(); }
        }

        public DatabaseFileCategory Category
        {
            get { return category; }
            set { category = value; }
        }

        public FileActivityFile FileInfo
        {
            get { return fileInfo; }
            set { this.fileInfo = value; UpdateControls(); }
        }       

        public event EventHandler OnToggled;
        public event EventHandler OnExpandAllDb;
        public event EventHandler OnExpandAllFiles;
        public event EventHandler OnCollapseAllDb;
        public event EventHandler OnCollapseAllFiles;
        public event EventHandler OnNavigateToDisksView;
        public event EventHandler OnNavigateToDatabaseFilesView;

        public event EventHandler<GetToggleStateArgs> OnRequestToggleState;

        public DatabaseFileActivity(FileActivityFile fileInfo)
        {            
            this.fileInfo  = fileInfo;
            this.readData  = new List<double>();
            this.writeData = new List<double>();
            this.activityType = DatabaseFileActivityType.Normal;
            this.category  = DatabaseFileCategory.Normal;
            this.tooltip   = new ToolTip();

            InitializeComponent();

            chart1.PlotAreaMargin.Bottom = -1;
            chart1.PlotAreaMargin.Top    = -1;
            chart1.PlotAreaMargin.Left   = -1;
            chart1.PlotAreaMargin.Right  = -1;     

            this.fileActivityImageMap = new Dictionary<FileActivityFileType, Image>();
            this.fileActivityImageMap.Add(FileActivityFileType.Data,    Properties.Resources.Database);
            this.fileActivityImageMap.Add(FileActivityFileType.Log,     Properties.Resources.Logs);
            this.fileActivityImageMap.Add(FileActivityFileType.Unknown, Properties.Resources.Logs);

            // can show that it's offline if the status is offline
            // null IO data is "inaccessible" for databsae
        }
        
        private void UpdateControls()
        {
            if (this.InvokeRequired)
            {
                Invoke(new MethodInvoker(UpdateControls));
                return;
            }

            // get the reads and writes
            double readsPerSecond  = fileInfo.ReadsPerSec.HasValue  ? fileInfo.ReadsPerSec.Value  : double.NaN;
            double writesPerSecond = fileInfo.WritesPerSec.HasValue ? fileInfo.WritesPerSec.Value : double.NaN;

            if (readsPerSecond < 0.01)
                readsPerSecond = 0;

            if (writesPerSecond < 0.01)
                writesPerSecond = 0;

            // set the tooltips
            tooltip.SetToolTip(databaseFileName, fileInfo.Filepath);
            tooltip.SetToolTip(databaseName, fileInfo.DatabaseName);
            tooltip.SetToolTip(readActivityIndicator,  double.IsNaN(readsPerSecond)  || readsPerSecond  == 0 ? "No activity" : readsPerSecond.ToString("####.##  reads/sec"));
            tooltip.SetToolTip(writeActivityIndicator, double.IsNaN(writesPerSecond) || writesPerSecond == 0 ? "No activity" : writesPerSecond.ToString("####.## writes/sec"));

            // set the text fields
            databaseFileName.Text = fileInfo.Filename;
            databaseName.Text     = fileInfo.DatabaseName;    
      
            if(double.IsNaN(readsPerSecond)  || readsPerSecond  == 0)
                readActivity.Text = "None";
            else
                readActivity.Text = string.Format(READSFORMAT, readsPerSecond);

            if (double.IsNaN(writesPerSecond) || writesPerSecond == 0)
                writeActivity.Text = "None";
            else
                writeActivity.Text  = string.Format(WRITESFORMAT, writesPerSecond);

            // set the images
            fileTypeIndicator.BackgroundImage = fileActivityImageMap[fileInfo.FileType];
            readActivityIndicator.Image    = double.IsNaN(readsPerSecond)  || readsPerSecond  == 0 ? null : Properties.Resources.DiskDriveReadActivity_12x12_pulsing;
            writeActivityIndicator.Image   = double.IsNaN(writesPerSecond) || writesPerSecond == 0 ? null : Properties.Resources.DiskDriveWriteActivity_12x12_pulsing;

            // update the mini chart
            miniActivityChartGroup.Visible = !IsExpanded;
            miniChartReadsBar.Width  = (int)((miniActivityChartGroup.Width / 2) * (readsPerSecond / totalDriveReadsPerSecond));
            miniChartWritesBar.Width = (int)((miniActivityChartGroup.Width / 2) * (writesPerSecond / totalDriveWritesPerSecond)); 

            // trends
            int readTrend  = GetReadTrend();
            int writeTrend = GetWriteTrend();

            if (readTrend == 0 || double.IsNaN(readsPerSecond) || readsPerSecond == 0)
                pbReadTrend.Visible = false;
            else
            {
                pbReadTrend.Visible = true;
                pbReadTrend.Image = readTrend > 0 ? Properties.Resources.arrow_up_green_16x16_plain : Properties.Resources.arrow_down_green_16x16_plain;
            }

            if (writeTrend == 0 || double.IsNaN(writesPerSecond) || writesPerSecond == 0)
                pbWriteTrend.Visible = false;
            else
            {
                pbWriteTrend.Visible = true;
                pbWriteTrend.Image = writeTrend > 0 ? Properties.Resources.arrow_up_blue_16x16_plain : Properties.Resources.arrow_down_blue_16x16_plain;
            }

            // update the big chart
            int numpoints = readData.Count > writeData.Count ? readData.Count : writeData.Count;
            
            chart1.Data.Y.Clear();
            chart1.Data.X.Clear();
            chart1.Data.Clear();

            chart1.Data.Points = numpoints;
            chart1.Data.Series = 2;
            chart1.Series[0].Text = "Writes per Second";
            chart1.Series[1].Text = "Reads per Second";

            if (!HasAllZeros(readData))
            {
                for (int j = 0; j < readData.Count; j++)
                {
                    chart1.Data.Y[1, j] = readData[j];
                    chart1.Data.X[1, j] = j;
                }
            }

            if (!HasAllZeros(writeData))
            {
                for (int j = 0; j < writeData.Count; j++)
                {
                    chart1.Data.Y[0, j] = writeData[j];
                    chart1.Data.X[0, j] = j;
                }
            }

            // draw the background
            if (this.activityType == DatabaseFileActivityType.Normal)
            {
                this.BackgroundImage = Properties.Resources.background_gradient_blue;
            }
            else if (activityType == DatabaseFileActivityType.Max)
            {
                this.BackgroundImage = Properties.Resources.background_gradient_gold;
            }
            
            if (activityType != DatabaseFileActivityType.Max && category == DatabaseFileCategory.Other)
            {
                this.BackgroundImage = Properties.Resources.background_gradient_gray;
            }

            // handle expanded/collapsed
            int h = isExpanded ? 290 : 50;
            if (this.Height != h)
                this.Height = h;
        }

        private bool HasAllZeros(List<double> list)
        {
            // empty
            if (list.Count == 0)
                return true;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] > 0)
                    return false;
            }

            return true;
        }

        public void SetTotals(double? totalReads, double? totalWrites)
        {
            totalDriveReadsPerSecond  = totalReads.HasValue  ? totalReads.Value  : double.NaN;
            totalDriveWritesPerSecond = totalWrites.HasValue ? totalWrites.Value : double.NaN;
            UpdateControls();
        }

        public void SetData(List<double> readData, List<double> writeData)
        {
            this.readData.Clear();
            this.writeData.Clear();
            this.readData.AddRange(readData);
            this.writeData.AddRange(writeData);
            UpdateControls();
        }

        private void DatabaseFileActivity_Load(object sender, EventArgs e)
        {
            chart1.Data.Series = 2;
            chart1.Series[0].Text = "Writes per Second";
            chart1.Series[1].Text = "Reads per Second";
            chart1.ToolTipFormat = "%v %s"; // %v%% Disk Busy on Drive %s\n%x

            UpdateControls();
        }

        private void DatabaseFileActivity_VisibleChanged(object sender, EventArgs e)
        {
            UpdateControls();
        }

        private void Toggle_DoubleClick(object sender, EventArgs e)
        {
            // resize
            isExpanded = !isExpanded;

            // update 
            UpdateControls();

            // notify
            Notify(OnToggled);
        }

        private void toolbarsManager_BeforeToolDropdown(object sender, Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventArgs e)
        {            
            toolbarsManager.Tools["expandAllDb"].SharedProps.Caption   = "Expand All " + fileInfo.DatabaseName + " Files";
            toolbarsManager.Tools["collapseAllDb"].SharedProps.Caption = "Collapse All " + fileInfo.DatabaseName + " Files";            

            if (OnRequestToggleState != null)
            {
                GetToggleStateArgs args = new GetToggleStateArgs();
                OnRequestToggleState(this, args);

                toolbarsManager.Tools["expandAllDb"].SharedProps.Enabled      = !args.IsDbExpanded;
                toolbarsManager.Tools["expandAllFiles"].SharedProps.Enabled   = !args.IsAllExpanded;
                toolbarsManager.Tools["collapseAllDb"].SharedProps.Enabled    = !args.IsDbCollapsed;
                toolbarsManager.Tools["collapseAllFiles"].SharedProps.Enabled = !args.IsAllCollapsed;
            }
        }

        private void toolbarsManager_ToolClick(object sender, Infragistics.Win.UltraWinToolbars.ToolClickEventArgs e)
        {
            switch (e.Tool.Key)
            {
                case "expandAllDb":
                    Notify(OnExpandAllDb);
                    break;
                case "expandAllFiles":
                    Notify(OnExpandAllFiles);
                    break;
                case "collapseAllDb":
                    Notify(OnCollapseAllDb);
                    break;
                case "collapseAllFiles":
                    Notify(OnCollapseAllFiles);
                    break;
                case "navigateToDisksView":
                    Notify(OnNavigateToDisksView);
                    break;
                case "navigateToDatabaseFilesView":
                    Notify(OnNavigateToDatabaseFilesView);
                    break;
            }
        }

        private void Notify(EventHandler handler)
        {
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private int GetReadTrend()
        {
            if (readData.Count < 2)
                return 0;

            if ((readData[0] - readData[1]) == 0)
                return 0;

            return readData[0] - readData[1] > 0 ? 1 : -1;
        }

        private int GetWriteTrend()
        {
            if (writeData.Count < 2)
                return 0;

            if ((writeData[0] - writeData[1]) == 0)
                return 0;

            return writeData[0] - writeData[1] > 0 ? 1 : -1;
        }
    }

    public class GetToggleStateArgs : EventArgs
    {
        public bool IsDbExpanded;
        public bool IsAllExpanded;
        public bool IsDbCollapsed;
        public bool IsAllCollapsed;
    }
}
