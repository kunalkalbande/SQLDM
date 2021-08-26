using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using ChartFX.WinForms.Adornments;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Infragistics.Win;
using Infragistics.Win.Misc;
using Appearance=Infragistics.Win.Appearance;
using Resources=Idera.SQLdm.DesktopClient.Properties.Resources;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    using System.Collections.Generic;
    using Infragistics.Win.UltraWinToolTip;
    using Objects;
    using Common.Events;
    using Wintellect.PowerCollections;
    using Idera.SQLdm.DesktopClient.Helpers;
    using Idera.SQLdm.Common.Objects;
    using Infragistics.Windows.Themes;
    using Idera.SQLdm.DesktopClient.Properties;
    using Idera.SQLdm.Common.UI.Dialogs;

    internal class SqlServerInstanceThumbnail : IndexCardPanel
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SqlServerInstanceThumbnail");

        private int MarginWidth = 40;
        private const int MarginButtonsPaddingLeft = 5;
        private const int MarginButtonsPaddingBetween = 0;
        private int MarginButtonsSize = 30;
        private const int ContentAreaPadding = 7;

        /// <summary>
        /// The font size used to draw the header of the control.
        /// </summary>
        private float instanceNameFontSize = 10.0F;

        /// <summary>
        /// The font size used to draw the details of the control.
        /// </summary>
        private float metadataFontSize = 9.0F;

        private Pair<DataView, DataView> dataViews;
        private ServerVersion currentVersion = null;
        private int? currentResponseTime = null;
        private Int64? currentUserSessions = null;
        private int? currentCpuUsage = null;
        private Int64? currentMemoryUsagePercent = null;
        private Int64? currentPageReads = null;
        private Int64? currentPageWrites = null;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        private UltraToolTipManager toolTipManager;
        private UltraToolTipInfo statusToolTipInfo;
        private UltraToolTipInfo sessionsToolTipInfo;
        private UltraToolTipInfo queriesToolTipInfo;
        private UltraToolTipInfo resourcesToolTipInfo;
        private UltraToolTipInfo databasesToolTipInfo;
        private UltraToolTipInfo servicesToolTipInfo;
        private UltraToolTipInfo logsToolTipInfo;
        //For analsysi view 
        //10.0 SQLdm Srishti Purohit
        private UltraToolTipInfo analysisToolTipInfo;
        //For analsysi view 
        //10.0 SQLdm Srishti Purohit
        private UltraToolTipInfo launchSWAToolTipInfo;


        private UltraButton sessionsButton;
        private UltraButton queriesButton;
        private UltraButton resourcesButton;
        private UltraButton databasesButton;
        private UltraButton servicesButton;
        private UltraButton logsButton;
        //For analsysi view 
        //10.0 SQLdm Srishti Purohit
        private UltraButton analysisButton;
        //For analsysi view 
        //10.2 SQLdm Srishti Purohit
        private UltraButton launchSWAButton;
        private PictureBox statusPictureBox;
        private Chart chart;

        private Rectangle marginBounds;
        private Rectangle contentBounds;
        private Rectangle chartBounds;

        private MonitoredSqlServerStatus instanceStatus;
        private readonly MonitoredSqlServerWrapper instanceWrapper;
        private ThumbnailChartType chartType;
        private string SWaLaunchURL = null;

        public SqlServerInstanceThumbnail(int instanceId)
        {
            this.instanceId = instanceId;

            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.SupportsTransparentBackColor, true);
            //ValidateSWALaunch();
            InitializeComponent();
            //Saurabh, SQLDM - 30848 - UX - Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                ScaleControlsAsPerResolution();
            }
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.SqlServerInstanceThumbnail);
            instanceWrapper = ApplicationModel.Default.ActiveInstances[instanceId];
            instanceWrapper.Changed += instance_Changed;
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }
        public void ValidateSWALaunch()
        {
            try
            {
                if (ApplicationModel.Default.FocusObject != null && ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper && (ApplicationModel.Default.FocusObject as MonitoredSqlServerWrapper).Instance.InstanceName == ApplicationModel.Default.ActiveInstances[instanceId].InstanceName)
                {
                    ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
                    Common.Services.IManagementService managementService = ManagementServiceHelper.GetDefaultService(Properties.Settings.Default.ActiveRepositoryConnection.ConnectionInfo);
                    SWaLaunchURL = managementService.GetSWAWebURL(ApplicationModel.Default.ActiveInstances[instanceId].InstanceName);
                    //SQLdm 10.2.2--SQLDM-26966--Toggle visibility of launchSWAButton based on whether SWAURL is empty or not.
                    if (!string.IsNullOrEmpty(SWaLaunchURL))
                        launchSWAButton.Visible = true;
                    else
                        launchSWAButton.Visible = false;
                }
            }
            catch (Exception ex)
            {
                LOG.Error("Error while getting SWA launch URL : " + ex);
            }
        }


        /// <summary>
        /// The font size used to draw the header of the control.
        /// </summary>
        public float InstanceNameFontSize
        {
            get { return instanceNameFontSize; }
        }

        /// <summary>
        /// The font size used to draw the details of the control.
        /// </summary>
        public float MetadataFontSize
        {
            get { return metadataFontSize; }
        }

        public int InstanceId
        {
            get { return instanceId; }
        }

        public MonitoredSqlServerWrapper Instance
        {
            get { return instanceWrapper; }
        }

        public bool ChartVisible
        {
            get { return chart.Visible; }
        }

        public ThumbnailChartType ChartType
        {
            get { return chartType; }
        }

        public UltraToolTipManager ToolTipManager
        {
            get { return toolTipManager; }
            set
            {
                toolTipManager = value;

                toolTipManager.SetUltraToolTip(statusPictureBox, statusToolTipInfo);
                toolTipManager.SetUltraToolTip(sessionsButton, sessionsToolTipInfo);
                toolTipManager.SetUltraToolTip(queriesButton, queriesToolTipInfo);
                toolTipManager.SetUltraToolTip(resourcesButton, resourcesToolTipInfo);
                toolTipManager.SetUltraToolTip(databasesButton, databasesToolTipInfo);
                toolTipManager.SetUltraToolTip(servicesButton, servicesToolTipInfo);
                toolTipManager.SetUltraToolTip(logsButton, logsToolTipInfo);
                toolTipManager.SetUltraToolTip(analysisButton, analysisToolTipInfo);
                if (SWaLaunchURL != null)
                    toolTipManager.SetUltraToolTip(launchSWAButton, launchSWAToolTipInfo);
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (statusToolTipInfo != null)
                    statusToolTipInfo.Dispose();
                if (queriesToolTipInfo != null)
                    queriesToolTipInfo.Dispose();
                if (resourcesToolTipInfo != null)
                    resourcesToolTipInfo.Dispose();
                if (servicesToolTipInfo != null)
                    servicesToolTipInfo.Dispose();
                if (sessionsToolTipInfo != null)
                    sessionsToolTipInfo.Dispose();
                if (databasesToolTipInfo != null)
                    databasesToolTipInfo.Dispose();
                if (logsToolTipInfo != null)
                    logsToolTipInfo.Dispose();
                if (analysisToolTipInfo != null)
                    analysisToolTipInfo.Dispose();
                if (launchSWAToolTipInfo != null)
                    launchSWAToolTipInfo.Dispose();

                if (instanceWrapper != null)
                    instanceWrapper.Changed -= instance_Changed;

                if (components != null)
                    components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region InitializeComponent

        private void InitializeComponent()
        {
            SuspendLayout();
            Title = ApplicationModel.Default.ActiveInstances[instanceId].InstanceName;
            BackColor = Color.Transparent;
            Size = new Size(290, 280);
            Margin = new Padding(0);

            UpdateClientBounds();

            Point buttonLocation =
                new Point(marginBounds.X + MarginButtonsPaddingLeft, marginBounds.Y + MarginButtonsPaddingBetween);

            //
            // Common button appearance
            //
            Appearance buttonAppearance = new Appearance();
            buttonAppearance.ImageHAlign = HAlign.Center;
            buttonAppearance.ImageVAlign = VAlign.Middle;

            //
            // statusPictureBox
            //
            statusPictureBox = new PictureBox();
            statusPictureBox.Size = new Size(32, 32);
            statusPictureBox.Location = new Point(contentBounds.X + 1, contentBounds.Y + 6);
            statusPictureBox.Image = Resources.OK32x32;
            statusPictureBox.TabStop = false;

            //
            // sessionsButton
            //
            sessionsButton = new Controls.CustomControls.CustomWidgetUltraButton();
            sessionsButton.Appearance.ImageHAlign = HAlign.Center;
            sessionsButton.Appearance.ImageVAlign = VAlign.Middle;
            sessionsButton.Appearance.Image = Resources.SessionsThumbnail;
            sessionsButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            sessionsButton.Location = buttonLocation;
            sessionsButton.Name = "sessionsButton";
            sessionsButton.ShowFocusRect = false;
            sessionsButton.ShowOutline = false;
            sessionsButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            sessionsButton.ImageSize = new Size(24, 24);
            sessionsButton.UseAppStyling = false;
            sessionsButton.UseOsThemes = DefaultableBoolean.False;
            sessionsButton.Click += new EventHandler(sessionsButton_Click);

            //
            // queriesButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            queriesButton = new Controls.CustomControls.CustomWidgetUltraButton();
            queriesButton.Appearance.ImageHAlign = HAlign.Center;
            queriesButton.Appearance.ImageVAlign = VAlign.Middle;
            queriesButton.Appearance.Image = Resources.QueriesThumbnail;
            queriesButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            queriesButton.Location = buttonLocation;
            queriesButton.Name = "queriesButton";
            queriesButton.ShowFocusRect = false;
            queriesButton.ShowOutline = false;
            queriesButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            queriesButton.ImageSize = new Size(24, 24);
            queriesButton.UseAppStyling = false;
            queriesButton.UseOsThemes = DefaultableBoolean.False;
            queriesButton.Click += new EventHandler(queriesButton_Click);

            //
            // resourcesButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            resourcesButton = new Controls.CustomControls.CustomWidgetUltraButton();
            resourcesButton.Appearance.ImageHAlign = HAlign.Center;
            resourcesButton.Appearance.ImageVAlign = VAlign.Middle;
            resourcesButton.Appearance.Image = Resources.ResourcesThumbnail;
            resourcesButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            resourcesButton.Location = buttonLocation;
            resourcesButton.Name = "resourcesButton";
            resourcesButton.ShowFocusRect = false;
            resourcesButton.ShowOutline = false;
            resourcesButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            resourcesButton.ImageSize = new Size(24, 24);
            resourcesButton.UseAppStyling = false;
            resourcesButton.UseOsThemes = DefaultableBoolean.False;
            resourcesButton.Click += new EventHandler(resourcesButton_Click);

            //
            // databasesButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            databasesButton = new Controls.CustomControls.CustomWidgetUltraButton();
            databasesButton.Appearance.ImageHAlign = HAlign.Center;
            databasesButton.Appearance.ImageVAlign = VAlign.Middle;
            databasesButton.Appearance.Image = Resources.DatabasesThumbnail;
            databasesButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            databasesButton.Location = buttonLocation;
            databasesButton.Name = "databasesButton";
            databasesButton.ShowFocusRect = false;
            databasesButton.ShowOutline = false;
            databasesButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            databasesButton.ImageSize = new Size(24, 24);
            databasesButton.UseAppStyling = false;
            databasesButton.UseOsThemes = DefaultableBoolean.False;
            databasesButton.Click += new EventHandler(databasesButton_Click);

            //
            // servicesButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            servicesButton = new Controls.CustomControls.CustomWidgetUltraButton();
            servicesButton.Appearance.ImageHAlign = HAlign.Center;
            servicesButton.Appearance.ImageVAlign = VAlign.Middle;
            servicesButton.Appearance.Image = Resources.ServicesThumbnail;
            servicesButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            servicesButton.Location = buttonLocation;
            servicesButton.Name = "servicesButton";
            servicesButton.ShowFocusRect = false;
            servicesButton.ShowOutline = false;
            servicesButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            servicesButton.ImageSize = new Size(24, 24);
            servicesButton.UseAppStyling = false;
            servicesButton.UseOsThemes = DefaultableBoolean.False;
            servicesButton.Click += new EventHandler(servicesButton_Click);

            //
            // logsButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            logsButton = new Controls.CustomControls.CustomWidgetUltraButton();
            logsButton.Appearance.ImageHAlign = HAlign.Center;
            logsButton.Appearance.ImageVAlign = VAlign.Middle;
            logsButton.Appearance.Image = Resources.LogsThumbnail;
            logsButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            logsButton.Location = buttonLocation;
            logsButton.Name = "logsButton";
            logsButton.ShowFocusRect = false;
            logsButton.ShowOutline = false;
            logsButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            logsButton.ImageSize = new Size(24, 24);
            logsButton.UseAppStyling = false;
            logsButton.UseOsThemes = DefaultableBoolean.False;
            logsButton.Click += new EventHandler(logsButton_Click);
            //SQLdm 10.2.2--SQLDM-26966--Adding SWALaunch button at time of construction with false visibility
            // if (SWaLaunchURL != null)
            // {
            // //
            // // launchSWAButton
            // //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            launchSWAButton = new Controls.CustomControls.CustomWidgetUltraButton();
            launchSWAButton.Appearance.ImageHAlign = HAlign.Center;
            launchSWAButton.Appearance.ImageVAlign = VAlign.Middle;
            launchSWAButton.Appearance.Image = Resources.SWA_Icon24;
            launchSWAButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            launchSWAButton.Location = buttonLocation;
            launchSWAButton.Name = "launchSWAButton";
            launchSWAButton.ShowFocusRect = false;
            launchSWAButton.ShowOutline = false;
            launchSWAButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            launchSWAButton.ImageSize = new Size(24, 24);
            launchSWAButton.UseAppStyling = false;
            launchSWAButton.UseOsThemes = DefaultableBoolean.False;
            launchSWAButton.Click += new EventHandler(launchSWAButton_Click);
            launchSWAButton.Visible = false;
            //}
            //
            // analysisButton
            //
            buttonLocation.Y += MarginButtonsSize + MarginButtonsPaddingBetween;
            analysisButton = new Controls.CustomControls.CustomWidgetUltraButton();
            analysisButton.Appearance.ImageHAlign = HAlign.Center;
            analysisButton.Appearance.ImageVAlign = VAlign.Middle;
            analysisButton.Appearance.Image = Resources.Analyze32;
            analysisButton.ButtonStyle = UIElementButtonStyle.Office2007RibbonButton;
            analysisButton.Location = buttonLocation;
            analysisButton.Name = "analysisButton";
            analysisButton.ShowFocusRect = false;
            analysisButton.ShowOutline = false;
            analysisButton.Size = new Size(MarginButtonsSize, MarginButtonsSize);
            analysisButton.ImageSize = new Size(24, 24);
            analysisButton.UseAppStyling = false;
            analysisButton.UseOsThemes = DefaultableBoolean.False;
            analysisButton.Click += new EventHandler(analysisButton_Click);

            //
            // chart
            //
            chart = new Chart();
            chart.Visible = false;
            TitleDockable titleDockable = new TitleDockable();
            GradientBackground chartBackground = new GradientBackground(GradientType.Vertical);
            chartBackground.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            chart.Background = chartBackground;
            chart.Palette = "Schemes.Classic";
            chart.Border = new SimpleBorder(SimpleBorderType.None, Color.FromArgb(109, 125, 138));
            chart.AxisX.LabelsFormat.Format = AxisFormat.DateTime;
            chart.AxisX.LabelsFormat.CustomFormat = "h:mm";
            chart.Cursor = Cursors.Hand;
            chart.LegendBox.Visible = false;
            chart.ContextMenus = false;
            chart.Location = chartBounds.Location;
            chart.Size = chartBounds.Size;
            chart.PlotAreaMargin.Bottom = 1;
            chart.PlotAreaMargin.Left = 1;
            chart.PlotAreaMargin.Right = 8;
            titleDockable.Separation = 5;
            titleDockable.Text = "Response Time (ms)";
            chart.Titles.AddRange(new TitleDockable[] { titleDockable });
            chart.MouseClick += new HitTestEventHandler(OnChartMouseClick);
            chart.MouseDoubleClick += new HitTestEventHandler(chart_MouseDoubleClick);
            chart.MouseDown += new HitTestEventHandler(chart_MouseDown);

            statusToolTipInfo = new UltraToolTipInfo();
            sessionsToolTipInfo = new UltraToolTipInfo();
            queriesToolTipInfo = new UltraToolTipInfo();
            resourcesToolTipInfo = new UltraToolTipInfo();
            databasesToolTipInfo = new UltraToolTipInfo();
            servicesToolTipInfo = new UltraToolTipInfo();
            logsToolTipInfo = new UltraToolTipInfo();
            analysisToolTipInfo = new UltraToolTipInfo();
            launchSWAToolTipInfo = new UltraToolTipInfo();

            UpdateStatus(ApplicationModel.Default.GetInstanceStatus(instanceId));

            Controls.Add(chart);
            Controls.Add(statusPictureBox);
            Controls.Add(sessionsButton);
            Controls.Add(queriesButton);
            Controls.Add(resourcesButton);
            Controls.Add(databasesButton);
            Controls.Add(servicesButton);
            Controls.Add(logsButton);
            //SQLdm 10.2.2--SQLDM-26966--Adding SWALaunch button at time of construction with false visibility
            //if(SWaLaunchURL != null)
            Controls.Add(launchSWAButton);
            //show analysis if server version is greater than 2000
            //10.0 SQLdm Srishti Purohit
            MonitoredSqlServer instance = ApplicationModel.Default.ActiveInstances[instanceId];
            if (instance != null)
            {
                int sqlVersionMajor = 0;
                try
                {
                    if (instance.MostRecentSQLVersion != null)
                        sqlVersionMajor = instance.MostRecentSQLVersion.Major;
                    if (sqlVersionMajor < 9)
                    {
                        LOG.Info("Server Version : {0} does not support analysis view.", sqlVersionMajor);
                    }
                    else
                    {
                        Controls.Add(analysisButton);
                    }
                }
                catch (Exception ex)
                {
                    LOG.Error("Error in visibility set of Analyze button in all serever view. " + ex);
                }
            }

            ResumeLayout(false);
        }

        #endregion

        public void instance_Changed(object sender, MonitoredSqlServerChangedEventArgs e)
        {
            MonitoredSqlServerStatus status = ApplicationModel.Default.GetInstanceStatus(e.Instance.Id);
            if (status != null)
                UpdateStatus(status);

            Invalidate();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
                ApplicationController.Default.ShowServerView(instanceId);
            }

            base.OnMouseDoubleClick(e);
        }

        private void sessionsButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.SessionsSummary);
        }

        private void queriesButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Queries);
        }

        private void resourcesButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.ResourcesSummary);
        }

        private void databasesButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Databases);
        }

        private void servicesButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Services);
        }

        private void logsButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Logs);
        }

        private void analysisButton_Click(object sender, EventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId, ServerViews.Analysis);
        }
        private void launchSWAButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(SWaLaunchURL);
        }

        private void UpdateClientBounds()
        {
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
            {
                if (AutoScaleSizeHelper.isLargeSize)
                {
                    MarginWidth += 10;
                    MarginButtonsSize += 8;
                }
                if (AutoScaleSizeHelper.isXLargeSize)
                {
                    MarginWidth += 10;
                    MarginButtonsSize += 10;
                }
                if (AutoScaleSizeHelper.isXXLargeSize)
                {
                    MarginWidth += 10;
                    MarginButtonsSize += 12;
                }
            }

            marginBounds = contentAreaBounds;
            marginBounds.Width = MarginWidth;

            contentBounds = contentAreaBounds;
            contentBounds.X += MarginWidth + ContentAreaPadding;
            contentBounds.Y += ContentAreaPadding;
            contentBounds.Width -= MarginWidth + (ContentAreaPadding * 2);

            chartBounds = contentAreaBounds;
            chartBounds.X += MarginWidth;
            chartBounds.Width -= MarginWidth;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            // update tool tip text
            if (toolTipManager != null && instanceStatus != null)
            {
                statusToolTipInfo.ToolTipText = instanceStatus.ToolTip;
                statusToolTipInfo.Enabled = DefaultableBoolean.True;
                statusToolTipInfo.ToolTipTitle = instanceStatus.ToolTipHeading;
                statusToolTipInfo.ToolTipTitleAppearance.Image = instanceStatus.ToolTipHeadingImage;
                toolTipManager.SetUltraToolTip(statusPictureBox, statusToolTipInfo);

                SetToolTip(sessionsToolTipInfo, sessionsButton, MetricCategory.Sessions);
                SetToolTip(queriesToolTipInfo, queriesButton, MetricCategory.Queries);
                SetToolTip(resourcesToolTipInfo, resourcesButton, MetricCategory.Resources);
                SetToolTip(databasesToolTipInfo, databasesButton, MetricCategory.Databases);
                SetToolTip(servicesToolTipInfo, servicesButton, MetricCategory.Services);
                SetToolTip(logsToolTipInfo, logsButton, MetricCategory.Logs);
                SetToolTip(analysisToolTipInfo, analysisButton, MetricCategory.Analyze);
                if (SWaLaunchURL != null)
                {
                    launchSWAToolTipInfo.ToolTipText = "Launch instance in SWA";
                    toolTipManager.SetUltraToolTip(launchSWAButton, launchSWAToolTipInfo);
                }
            }


            base.OnMouseEnter(e);
        }

        private void SetToolTip(UltraToolTipInfo toolTipInfo, Control control, MetricCategory category)
        {
            ICollection<Issue> issues = instanceStatus[category];

            if (issues == null || issues.Count == 0)
            {
                toolTipInfo.ToolTipTitle = null;
                toolTipInfo.ResetToolTipTitleAppearance();
                toolTipInfo.ToolTipText = String.Format("{0}", category);
            }
            else
            {
                Issue[] issueArray = Algorithms.ToArray(issues);
                AppearanceBase titleAppearance = toolTipInfo.ToolTipTitleAppearance;

                switch (issueArray[0].Severity)
                {
                    case MonitoredState.Critical:
                        titleAppearance.Image = Resources.StatusCriticalSmall;
                        break;
                    case MonitoredState.Warning:
                        titleAppearance.Image = Resources.StatusWarningSmall;
                        break;
                    case MonitoredState.OK:
                        titleAppearance.Image = Resources.StatusOKSmall;
                        break;
                    default:
                        titleAppearance.Image = Resources.InformationSmall;
                        break;
                }

                toolTipInfo.ToolTipTitle = String.Format("{0} are {1}", category, issueArray[0].Severity);
                toolTipInfo.ToolTipText = instanceStatus.GetToolTip(category);
            }

            toolTipInfo.Enabled = DefaultableBoolean.Default;
            toolTipManager.SetUltraToolTip(control, toolTipInfo);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            UpdateClientBounds();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawImage(Resources.InstanceThumbnailMarginImage, marginBounds);
            DrawContent(e.Graphics);
            this.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.IndexCardPanelBackColor);
        }

        private void DrawContent(Graphics graphics)
        {
            bool IsInMaintenanceMode = instanceStatus != null && instanceStatus.IsInMaintenanceMode;

            if (graphics != null)
            {
                StringFormat stringFormat = new StringFormat();
                stringFormat.LineAlignment = StringAlignment.Center;
                stringFormat.FormatFlags |= StringFormatFlags.NoWrap;
                stringFormat.Trimming = StringTrimming.EllipsisCharacter;

                StringFormat stringFormat2 = new StringFormat(stringFormat);
                stringFormat2.Alignment = StringAlignment.Far;

                RectangleF instanceNameBounds = contentBounds;

                Brush instanceNameFontColor = Brushes.Black;
                Brush metadataFontColor = Brushes.Black;

                if (Settings.Default.ColorScheme == "Dark")
                {
                    instanceNameFontColor = new SolidBrush(ColorTranslator.FromHtml(DarkThemeColorConstants.ServerWidgetInstanceNameFontColor));
                    metadataFontColor = new SolidBrush(ColorTranslator.FromHtml(DarkThemeColorConstants.ServerWidgetMetadataFontColor));
                }

                using (Font instanceNameFont = new Font(Font.FontFamily, this.instanceNameFontSize, FontStyle.Bold, GraphicsUnit.Point, 0))
                {
                    instanceNameBounds.Offset(statusPictureBox.Width + ContentAreaPadding, 0);
                    instanceNameBounds.Height = instanceNameFont.GetHeight();
                    instanceNameBounds.Width -= (statusPictureBox.Width + ContentAreaPadding);
                    graphics.DrawString(Title, instanceNameFont, instanceNameFontColor, instanceNameBounds, stringFormat);
                }

                using (Font metadataFont = new Font(Font.FontFamily, this.metadataFontSize, FontStyle.Regular, GraphicsUnit.Point, 0))
                {
                    RectangleF metadataTextBounds = instanceNameBounds;
                    metadataTextBounds.Y += metadataTextBounds.Height;
                    metadataTextBounds.Height = metadataFont.GetHeight() + 5;

                    string text = String.Empty;
                    if (instanceWrapper.IsRefreshing)
                        text = "Refreshing...";
                    else
                    if (instanceStatus == null ||
                       (instanceStatus.Severity == MonitoredState.OK && !instanceStatus.IsRefreshAvailable))
                        text = "Initializing...";
                    else
                    if (currentVersion != null)
                        text = currentVersion.ToString();

                    graphics.DrawString(text,
                                        metadataFont,
                                        metadataFontColor,
                                        metadataTextBounds,
                                        stringFormat);

                    if (currentVersion != null)
                    {

                        metadataTextBounds.Y += metadataTextBounds.Height;
                        graphics.DrawString(
                            string.Format("Build {0}", currentVersion.Version),
                            metadataFont, metadataFontColor, metadataTextBounds,
                            stringFormat);

                        metadataTextBounds.Offset(-(statusPictureBox.Width + ContentAreaPadding), 0);
                        metadataTextBounds.Width = contentBounds.Width;

                        using (SolidBrush gridColor1 = new SolidBrush(Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ServerWidgetGridColor1) : Color.FromArgb(208, 216, 232)))
                        using (SolidBrush gridColor2 = new SolidBrush(Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.ServerWidgetGridColor2) : Color.FromArgb(233, 237, 244)))
                        {
                            RectangleF rowBounds = metadataTextBounds;
                            rowBounds.Y += rowBounds.Height + 7;
                            rowBounds.Height += 3;
                            graphics.FillRectangle(gridColor1, rowBounds);
                            metadataTextBounds = RectangleF.Inflate(rowBounds, -5, 0);

                            if (!IsInMaintenanceMode)
                            {
                                graphics.DrawString("Response Time:", metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat);
                                graphics.DrawString(currentResponseTime != null ? currentResponseTime + " ms" : @"N/A",
                                                    metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat2);
                            }
                            rowBounds.Y += rowBounds.Height;
                            graphics.FillRectangle(gridColor2, rowBounds);
                            metadataTextBounds = RectangleF.Inflate(rowBounds, -5, 0);
                            if (!IsInMaintenanceMode)
                            {
                                graphics.DrawString("User Sessions:", metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat);
                                graphics.DrawString(
                                    currentUserSessions != null ? currentUserSessions.ToString() : @"N/A",
                                    metadataFont, metadataFontColor, metadataTextBounds,
                                    stringFormat2);
                            }
                            rowBounds.Y += rowBounds.Height;
                            graphics.FillRectangle(gridColor1, rowBounds);
                            metadataTextBounds = RectangleF.Inflate(rowBounds, -5, 0);
                            if (!IsInMaintenanceMode)
                            {
                                graphics.DrawString("SQL CPU Usage:", metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat);
                                graphics.DrawString(currentCpuUsage != null ? currentCpuUsage + "%" : @"N/A",
                                                    metadataFont,
                                                    metadataFontColor, metadataTextBounds,
                                                    stringFormat2);
                            }
                            rowBounds.Y += rowBounds.Height;
                            graphics.FillRectangle(gridColor2, rowBounds);
                            metadataTextBounds = RectangleF.Inflate(rowBounds, -5, 0);
                            if (!IsInMaintenanceMode)
                            {
                                graphics.DrawString("SQL Memory Usage/Allocated:", metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat);
                                graphics.DrawString(
                                    currentMemoryUsagePercent != null ? currentMemoryUsagePercent + "%" : @"N/A",
                                    metadataFont, metadataFontColor, metadataTextBounds,
                                    stringFormat2);
                            }
                            rowBounds.Y += rowBounds.Height;
                            graphics.FillRectangle(gridColor1, rowBounds);
                            metadataTextBounds = RectangleF.Inflate(rowBounds, -5, 0);
                            if (!IsInMaintenanceMode)
                            {
                                graphics.DrawString("SQL Disk I/O:", metadataFont, metadataFontColor, metadataTextBounds,
                                                    stringFormat);

                                if (currentPageReads != null && currentPageWrites != null)
                                {
                                    graphics.DrawString(currentPageReads + @"/" + currentPageWrites, metadataFont,
                                                        metadataFontColor, metadataTextBounds,
                                                        stringFormat2);
                                }
                                else
                                {
                                    graphics.DrawString(@"N/A", metadataFont, metadataFontColor, metadataTextBounds,
                                                        stringFormat2);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void ShowChart(ThumbnailChartType type)
        {
            chartType = type;
            UpdateChart();
            chart.Visible = true;
        }

        public void HideChart()
        {
            chart.Visible = false;
        }

        #region Chart Click Events

        private void OnChartMouseClick(object sender, HitTestEventArgs e)
        {
            Selected = true;

            if (e.Button != MouseButtons.Right && e.HitType != HitType.Other)
            {
                // hit type of other means they clicked on the chart toolbar
                ServerViews targetView;
                switch (ChartType)
                {
                    case ThumbnailChartType.AvgCpuUsage:
                        targetView = ServerViews.ResourcesCpu;
                        break;
                    case ThumbnailChartType.MemoryUsage:
                        targetView = ServerViews.ResourcesMemory;
                        break;
                    case ThumbnailChartType.ResponseTime:
                        targetView = ServerViews.SessionsSummary;
                        break;
                    case ThumbnailChartType.SqlReadsWrites:
                        targetView = ServerViews.ResourcesDisk;
                        break;
                    case ThumbnailChartType.UserSessions:
                        targetView = ServerViews.SessionsDetails;
                        break;
                    default:
                        return;
                }
                ApplicationController.Default.ShowServerView(instanceId, targetView);
            }
        }

        #endregion

        void chart_MouseDown(object sender, HitTestEventArgs e)
        {
            OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta));
        }

        private void chart_MouseDoubleClick(object sender, HitTestEventArgs e)
        {
            ApplicationModel.Default.FocusObject = ApplicationModel.Default.ActiveInstances[instanceId];
            ApplicationController.Default.ShowServerView(instanceId);
        }

        public void UpdateStatus(MonitoredSqlServerStatus status)
        {
            // save a reference to the status for tooltips
            instanceStatus = status;

            // update images based on status
            Image statusImage = Resources.OK32x32;

            string imageKey = status != null ? status.ServerImageKey : "";
            switch (imageKey)
            {
                case "ServerMaintenanceMode":
                    statusImage = Resources.MaintenanceModeLarge;
                    break;
                case "ServerInformation":
                    statusImage = Resources.Information32x32;
                    break;
                case "ServerCritical":
                    statusImage = Resources.Critical32x32;
                    break;
                case "ServerWarning":
                    statusImage = Resources.Warning32x32;
                    break;
                case "ServerOK":
                    statusImage = Resources.OK32x32;
                    break;
            }
            statusPictureBox.Image = statusImage;
            try
            {
                statusPictureBox.BackColor = Color.Transparent;
            }
            catch(Exception ex)
            {
            }
            // update the image of the sessions button
            MonitoredState severity;
            statusImage = Resources.SessionsThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Sessions];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.SessionsCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.SessionsWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.SessionsInfoThumbnail;
                }
            }
            sessionsButton.Appearance.Image = statusImage;
            // update the image of the queries button
            statusImage = Resources.QueriesThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Queries];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.QueriesCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.QueriesWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.QueriesInfoThumbnail;
                }
            }
            queriesButton.Appearance.Image = statusImage;

            // update the image of the resources button
            statusImage = Resources.ResourcesThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Resources];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.ResourcesCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.ResourcesWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.ResourcesInfoThumbnail;
                }
            }
            resourcesButton.Appearance.Image = statusImage;

            // update the image of the databases button
            statusImage = Resources.DatabasesThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Databases];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.DatabasesCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.DatabasesWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.DatabasesInfoThumbnail;
                }
            }
            databasesButton.Appearance.Image = statusImage;

            // update the image of the services button
            statusImage = Resources.ServicesThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Services];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.ServicesCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.ServicesWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.ServicesInfoThumbnail;
                }
            }
            servicesButton.Appearance.Image = statusImage;

            //update the image of the logs button
            statusImage = Resources.LogsThumbnail;
            if (status != null)
            {
                ICollection<Issue> issues = instanceStatus[MetricCategory.Logs];
                if (issues != null && issues.Count > 0)
                {
                    Issue[] issueArray = Algorithms.ToArray(issues);
                    severity = issueArray[0].Severity;
                    if (severity == MonitoredState.Critical)
                        statusImage = Resources.LogsCriticalThumbnail;
                    else
                    if (severity == MonitoredState.Warning)
                        statusImage = Resources.LogsWarningThumbnail;
                    else
                    if (severity == MonitoredState.Informational)
                        statusImage = Resources.LogsInfoThumbnail;
                }
            }
            logsButton.Appearance.Image = statusImage;
        }

        public void UpdateData(Pair<DataView, DataView> data)
        {
            dataViews = data;

            UpdateLabels();
            UpdateChart();
        }

        private void UpdateLabels()
        {
            if (dataViews.First != null && dataViews.First.Count != 0)
            {
                string versionString = dataViews.First[dataViews.First.Count - 1]["ServerVersion"].ToString();

                try
                {
                    currentVersion = new ServerVersion(versionString);
                }
                catch (Exception e)
                {
                    currentVersion = new ServerVersion("0.0.0.0");
                    LOG.Error(string.Format("Unable to construct SQL Server version string: {0}", versionString), e);
                }
                //start : vineet 10.0.1 - defect fix, console hang
                int sqlVersionMajor = currentVersion.Major;
                if (sqlVersionMajor >= 9)
                {
                    if (!Controls.Contains(analysisButton))
                        Controls.Add(analysisButton);
                }
                //end : vineet 10.0.1 - defect fix, console hang

                // Current response time
                object responseTime = dataViews.First[dataViews.First.Count - 1]["ResponseTimeInMilliseconds"];
                if (responseTime != DBNull.Value)
                {
                    double responseOut;
                    if (double.TryParse(responseTime.ToString(), out responseOut))
                    {
                        currentResponseTime = (int)responseOut;
                    }
                    else
                    {
                        LOG.ErrorFormat("Error at UpdateLabels() invalid Convert.ToInt32() responseTime = {0}", responseTime);
                    }

                }
                else
                {
                    currentResponseTime = null;
                }

                // Current user sessions
                object userSessions = dataViews.First[dataViews.First.Count - 1]["UserProcesses"];
                if (userSessions != DBNull.Value)
                {
                    currentUserSessions = Convert.ToInt64(userSessions);
                }
                else
                {
                    currentUserSessions = null;
                }

                // Current CPU usage
                object cpuUsage = dataViews.First[dataViews.First.Count - 1]["CPUActivityPercentage"];
                if (cpuUsage != DBNull.Value)
                {
                    double cpuUsageOut;
                    if (double.TryParse(cpuUsage.ToString(), out cpuUsageOut))
                    {
                        currentCpuUsage = (int)cpuUsageOut;
                    }
                    else
                    {
                        LOG.ErrorFormat("Error at UpdateLabels() invalid Convert.ToInt32() cpuUsage = {0}", cpuUsage);
                    }

                }
                else
                {
                    currentCpuUsage = null;
                }

                // Current memory usage
                object memoryUsed = dataViews.First[dataViews.First.Count - 1]["SqlMemoryUsedInMegabytes"];
                object memoryAllocated = dataViews.First[dataViews.First.Count - 1]["SqlMemoryAllocatedInMegabytes"];
                if (memoryUsed != DBNull.Value && memoryAllocated != DBNull.Value && Convert.ToInt64(memoryAllocated) > 0)
                {
                    double memoryUsedPercent = ((double)Convert.ToInt64(memoryUsed) /
                                                (double)Convert.ToInt64(memoryAllocated)) * 100;
                    memoryUsedPercent = Math.Round(memoryUsedPercent);
                    double memoryUsedPercentOut;
                    if (double.TryParse(memoryUsedPercent.ToString(), out memoryUsedPercentOut))
                    {
                        currentMemoryUsagePercent = (long)memoryUsedPercentOut;
                        currentMemoryUsagePercent = currentMemoryUsagePercent > 100
                                                        ? 100
                                                        : currentMemoryUsagePercent;
                    }
                    else
                    {
                        LOG.ErrorFormat(
                            "Error at UpdateLabels() invalid Convert.ToInt32() memoryUsedPercentOut = {0}",
                            memoryUsedPercent);
                    }
                }
                else
                {
                    currentMemoryUsagePercent = null;
                }

                // Current SQL I/O
                object pageReads = dataViews.First[dataViews.First.Count - 1]["PageReads"];
                object pageWrites = dataViews.First[dataViews.First.Count - 1]["PageWrites"];
                if (pageReads != DBNull.Value && pageWrites != DBNull.Value)
                {
                    currentPageReads = Convert.ToInt64(pageReads);
                    currentPageWrites = Convert.ToInt64(pageWrites);
                }
                else
                {
                    currentPageReads = null;
                    currentPageWrites = null;
                }
            }
            else
            {
                currentVersion = null;
                currentResponseTime = null;
                currentUserSessions = null;
                currentCpuUsage = null;
                currentMemoryUsagePercent = null;
                currentPageReads = null;
                currentPageWrites = null;
            }

            Invalidate(false);
        }

        private void UpdateChart()
        {
            chart.Data.Clear();
            chart.DataSourceSettings.Fields.Clear();
            chart.AllSeries.Gallery = Gallery.Lines;

            bool IsInMaintenanceMode = instanceStatus != null && instanceStatus.IsInMaintenanceMode;

            if (dataViews.Second != null && dataViews.Second.Count != 0 && !IsInMaintenanceMode)
            {
                switch (chartType)
                {
                    case ThumbnailChartType.ResponseTime:
                        chart.Titles[0].Text = "Response Time (ms)";
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UTCCollectionDateTime", FieldUsage.XValue));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("ResponseTimeInMilliseconds", FieldUsage.Value));
                        chart.DataSource = dataViews.Second;
                        chart.Series[0].Text = "Response Time (ms)";
                        chart.Series[0].MarkerShape = MarkerShape.None;
                        chart.ToolTipFormat = "%v ms\n%x";
                        break;
                    case ThumbnailChartType.UserSessions:
                        chart.Titles[0].Text = "User Sessions";
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UTCCollectionDateTime", FieldUsage.XValue));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UserProcesses", FieldUsage.Value));
                        chart.DataSource = dataViews.Second;
                        chart.Series[0].Text = "User Sessions";
                        chart.Series[0].MarkerShape = MarkerShape.None;
                        chart.ToolTipFormat = "%v session(s)\n%x";
                        break;
                    case ThumbnailChartType.AvgCpuUsage:
                        chart.Titles[0].Text = "Avg. CPU Usage";
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UTCCollectionDateTime", FieldUsage.XValue));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("CPUActivityPercentage", FieldUsage.Value));
                        chart.DataSource = dataViews.Second;
                        chart.Series[0].Text = "Avg. CPU Usage";
                        chart.Series[0].MarkerShape = MarkerShape.None;
                        chart.ToolTipFormat = "%v%%\n%x";
                        break;
                    case ThumbnailChartType.MemoryUsage:
                        chart.Titles[0].Text = "SQL Memory Usage";
                        // TODO: This should be the same calculation for the summary view
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UTCCollectionDateTime", FieldUsage.XValue));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("SqlMemoryUsedInMegabytes", FieldUsage.Value));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("SqlMemoryAllocatedInMegabytes", FieldUsage.Value));
                        chart.AllSeries.Gallery = Gallery.Area;
                        chart.AllSeries.FillMode = FillMode.Gradient;
                        chart.DataSource = dataViews.Second;
                        chart.Series[0].Text = "SQL Memory Used";
                        chart.Series[1].Text = "SQL Memory Allocated";
                        chart.Series[0].MarkerShape = MarkerShape.None;
                        chart.Series[1].MarkerShape = MarkerShape.None;
                        chart.Series[0].AlternateColor = Color.Transparent;
                        chart.Series[1].AlternateColor = Color.Transparent;
                        chart.ToolTipFormat = "%v MB %s\n%x";
                        break;
                    case ThumbnailChartType.SqlReadsWrites:
                        chart.Titles[0].Text = "SQL I/O";
                        chart.DataSourceSettings.Fields.Add(new FieldMap("UTCCollectionDateTime", FieldUsage.XValue));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("PageReads", FieldUsage.Value));
                        chart.DataSourceSettings.Fields.Add(new FieldMap("PageWrites", FieldUsage.Value));
                        chart.DataSource = dataViews.Second;
                        chart.Series[0].Text = "Page Reads";
                        chart.Series[1].Text = "Page Writes";
                        chart.Series[0].MarkerShape = MarkerShape.None;
                        chart.Series[1].MarkerShape = MarkerShape.None;
                        chart.ToolTipFormat = "%v %s\n%x";
                        break;
                }
            }
        }

        /// <summary>
        /// Updates the base font sizes for title and details.
        /// </summary>
        /// <param name="instanceNameFontSize">Use to calculate the font size for headers.</param>
        /// <param name="metadataFontSize">Use to calculate the font size for the details.</param>
        public void UpdateFontSizes(float instanceNameFontSize, float metadataFontSize)
        {
            this.instanceNameFontSize = instanceNameFontSize;
            this.metadataFontSize = metadataFontSize;
        }

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ScaleControlsAsPerResolution()
        {
            AutoScaleSizeHelper.Default.AutoScaleControl(this.statusPictureBox, AutoScaleSizeHelper.ControlType.Control, new SizeF(1.5F, 1.5F));
            this.statusPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            this.statusPictureBox.Location = new Point(this.statusPictureBox.Location.X - 30, this.statusPictureBox.Location.Y - 15);
            AutoScaleSizeHelper.Default.AutoScaleControl(this, AutoScaleSizeHelper.ControlType.SqlServerInstanceThumbnail);
            if (AutoScaleSizeHelper.isLargeSize)
            {
                sessionsButton.ImageSize = new Size(sessionsButton.ImageSize.Width + 10, sessionsButton.ImageSize.Height + 8);
                queriesButton.ImageSize = new Size(queriesButton.ImageSize.Width + 10, queriesButton.ImageSize.Height + 8);
                resourcesButton.ImageSize = new Size(resourcesButton.ImageSize.Width + 10, resourcesButton.ImageSize.Height + 8);
                databasesButton.ImageSize = new Size(databasesButton.ImageSize.Width + 10, databasesButton.ImageSize.Height + 8);
                servicesButton.ImageSize = new Size(servicesButton.ImageSize.Width + 10, servicesButton.ImageSize.Height + 8);
                logsButton.ImageSize = new Size(logsButton.ImageSize.Width + 10, logsButton.ImageSize.Height + 8);
                analysisButton.ImageSize = new Size(analysisButton.ImageSize.Width + 10, analysisButton.ImageSize.Height + 8);
                launchSWAButton.ImageSize = new Size(launchSWAButton.ImageSize.Width + 10, launchSWAButton.ImageSize.Height + 8);
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                sessionsButton.ImageSize = new Size(sessionsButton.ImageSize.Width + 10, sessionsButton.ImageSize.Height + 10);
                queriesButton.ImageSize = new Size(queriesButton.ImageSize.Width + 10, queriesButton.ImageSize.Height + 10);
                resourcesButton.ImageSize = new Size(resourcesButton.ImageSize.Width + 10, resourcesButton.ImageSize.Height + 10);
                databasesButton.ImageSize = new Size(databasesButton.ImageSize.Width + 10, databasesButton.ImageSize.Height + 10);
                servicesButton.ImageSize = new Size(servicesButton.ImageSize.Width + 10, servicesButton.ImageSize.Height + 10);
                logsButton.ImageSize = new Size(logsButton.ImageSize.Width + 10, logsButton.ImageSize.Height + 10);
                analysisButton.ImageSize = new Size(analysisButton.ImageSize.Width + 10, analysisButton.ImageSize.Height + 10);
                launchSWAButton.ImageSize = new Size(launchSWAButton.ImageSize.Width + 10, launchSWAButton.ImageSize.Height + 10);
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                sessionsButton.ImageSize = new Size(sessionsButton.ImageSize.Width + 10, sessionsButton.ImageSize.Height + 12);
                queriesButton.ImageSize = new Size(queriesButton.ImageSize.Width + 10, queriesButton.ImageSize.Height + 12);
                resourcesButton.ImageSize = new Size(resourcesButton.ImageSize.Width + 10, resourcesButton.ImageSize.Height + 12);
                databasesButton.ImageSize = new Size(databasesButton.ImageSize.Width + 10, databasesButton.ImageSize.Height + 12);
                servicesButton.ImageSize = new Size(servicesButton.ImageSize.Width + 10, servicesButton.ImageSize.Height + 12);
                logsButton.ImageSize = new Size(logsButton.ImageSize.Width + 10, logsButton.ImageSize.Height + 12);
                analysisButton.ImageSize = new Size(analysisButton.ImageSize.Width + 10, analysisButton.ImageSize.Height + 12);
                launchSWAButton.ImageSize = new Size(launchSWAButton.ImageSize.Width + 10, launchSWAButton.ImageSize.Height + 12);
            }




        }



        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            UpdateStatus(ApplicationModel.Default.GetInstanceStatus(instanceId));
            Invalidate();
        }
     
    }
    internal enum ThumbnailChartType
    {
        ResponseTime,
        UserSessions,
        AvgCpuUsage,
        MemoryUsage,
        SqlReadsWrites
    }
}
