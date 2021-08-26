using System;
using System.Drawing;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    partial class DatabasesAlwaysOnView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            ///Operational Status Panel required for history mode -  SQLDM 10.0.0 Ankit Nagpal
            this.operationalStatusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.operationalStatusImage = new System.Windows.Forms.PictureBox();
            this.operationalStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            appearance1 = new Infragistics.Win.Appearance();
            appearance2 = new Infragistics.Win.Appearance();
            appearance3 = new Infragistics.Win.Appearance();
            appearance4 = new Infragistics.Win.Appearance();
            appearance5 = new Infragistics.Win.Appearance();
            appearance6 = new Infragistics.Win.Appearance();
            appearance7 = new Infragistics.Win.Appearance();
            appearance8 = new Infragistics.Win.Appearance();
            appearance9 = new Infragistics.Win.Appearance();
            appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(1609175141);
            appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Group Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Group ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Replica Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Replica ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Failover Mode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Availability mode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Replica Role");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Synchronization Health");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Redo Queue (KB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Redo Rate (KB/s)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Send Queue (KB)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Log Send Rate (KB/s)");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Listener DNS Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Listener IP Address");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Listener Port");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Failover Readiness");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Synchronization Database Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Suspended Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Hardened Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Operational Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Connection Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Connect Error #");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Connect Error Description");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Connect Error Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Synchronization Performance");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Estimated Data Loss");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Estimated Recovery Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("FileStream Send Rate");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(238621516);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(239127688);
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(243636016);
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground2 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("detailsGridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabasesAlwaysOnView));
            this.actionProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.propertiesTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.transactionLogsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.DatabasesAlwaysOn_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.refreshDatabasesButton = new Infragistics.Win.Misc.UltraButton();
            this.availabilityGroupsComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.contentContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.alwaysOnAvailabilityGroupsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.databasesGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.alwaysOnRatePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alwaysOnRateChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.alwaysOnRateChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.alwaysOnRateChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.alwaysOnRateHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.maximizeAlwaysOnRateChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreAlwaysOnRateChartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.alwaysOnSizePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.alwaysOnSizeChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel();
            this.alwaysOnSizeChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.alwaysOnSizeChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.alwaysOnSizeHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.maximizeAlwaysOnSizeChartButton = new System.Windows.Forms.ToolStripButton();
            this.restoreAlwaysOnSizeChartButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.alwaysOnStatisticsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.disksTabPage = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.diskUsagePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.currentDiskUsageChartContainerPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.diskUsageChartStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.refreshAlwaysOnDetailsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.propertiesTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.transactionLogsGrid)).BeginInit();
            this.DatabasesAlwaysOn_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availabilityGroupsComboBox)).BeginInit();
            this.contentContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnAvailabilityGroupsGrid)).BeginInit();
            this.tableLayoutPanel.SuspendLayout();
            this.alwaysOnRatePanel.SuspendLayout();
            this.alwaysOnRateChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnRateChart)).BeginInit();
            this.alwaysOnRateHeaderStrip.SuspendLayout();
            this.alwaysOnSizePanel.SuspendLayout();
            this.alwaysOnSizeChartContainerPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnSizeChart)).BeginInit();
            this.alwaysOnSizeHeaderStrip.SuspendLayout();
            this.disksTabPage.SuspendLayout();
            this.diskUsagePanel.SuspendLayout();
            this.currentDiskUsageChartContainerPanel.SuspendLayout();
            this.operationalStatusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // actionProgressControl
            // 
            this.actionProgressControl.Active = false;
            this.actionProgressControl.BackColor = System.Drawing.Color.White;
            this.actionProgressControl.Color = System.Drawing.Color.DarkGray;
            this.actionProgressControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actionProgressControl.InnerCircleRadius = 5;
            this.actionProgressControl.Location = new System.Drawing.Point(0, 1);
            this.actionProgressControl.Name = "actionProgressControl";
            this.actionProgressControl.NumberSpoke = 12;
            this.actionProgressControl.OuterCircleRadius = 11;
            this.actionProgressControl.RotationSpeed = 100;
            this.actionProgressControl.Size = new System.Drawing.Size(689, 197);
            this.actionProgressControl.SpokeThickness = 2;
            this.actionProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.actionProgressControl.TabIndex = 0;
            this.actionProgressControl.Visible = false;
            // 
            // propertiesTabPage
            // 
            this.propertiesTabPage.Controls.Add(this.transactionLogsGrid);
            this.propertiesTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.propertiesTabPage.Name = "propertiesTabPage";
            this.propertiesTabPage.Size = new System.Drawing.Size(685, 270);
            // 
            // transactionLogsGrid
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.transactionLogsGrid.DisplayLayout.Appearance = appearance1;
            this.transactionLogsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            this.transactionLogsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.transactionLogsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.transactionLogsGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.transactionLogsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.transactionLogsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.transactionLogsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.transactionLogsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.transactionLogsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.transactionLogsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.transactionLogsGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.transactionLogsGrid.DisplayLayout.Override.CellAppearance = appearance6;
            this.transactionLogsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.transactionLogsGrid.DisplayLayout.Override.CellPadding = 0;
            this.transactionLogsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.transactionLogsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Left";
            this.transactionLogsGrid.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.transactionLogsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.transactionLogsGrid.DisplayLayout.Override.RowAppearance = appearance9;
            this.transactionLogsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.transactionLogsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.transactionLogsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.transactionLogsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.transactionLogsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.transactionLogsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "autoGrowthValueList";
            this.transactionLogsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1});
            this.transactionLogsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.transactionLogsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.transactionLogsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.transactionLogsGrid.Location = new System.Drawing.Point(0, 0);
            this.transactionLogsGrid.Name = "transactionLogsGrid";
            this.transactionLogsGrid.Size = new System.Drawing.Size(685, 270);
            this.transactionLogsGrid.TabIndex = 9;
            // 
            // DatabasesAlwaysOn_Fill_Panel
            // 
            this.DatabasesAlwaysOn_Fill_Panel.Controls.Add(this.refreshDatabasesButton);
            this.DatabasesAlwaysOn_Fill_Panel.Controls.Add(this.availabilityGroupsComboBox);
            this.DatabasesAlwaysOn_Fill_Panel.Controls.Add(this.contentContainerPanel);
            this.DatabasesAlwaysOn_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DatabasesAlwaysOn_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabasesAlwaysOn_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.DatabasesAlwaysOn_Fill_Panel.Name = "DatabasesAlwaysOn_Fill_Panel";
            this.DatabasesAlwaysOn_Fill_Panel.Size = new System.Drawing.Size(689, 517);
            this.DatabasesAlwaysOn_Fill_Panel.TabIndex = 10;
            // 
            // refreshDatabasesButton
            //
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.refreshDatabasesButton.UseAppStyling = false;
                this.refreshDatabasesButton.UseOsThemes = DefaultableBoolean.False;
                this.refreshDatabasesButton.ButtonStyle = UIElementButtonStyle.FlatBorderless;
            }
            else
            {
                this.refreshDatabasesButton.UseAppStyling = true;
            }
            this.refreshDatabasesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            if(!refreshDatabasesButton.Enabled)
                appearance11.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
            else
                appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            appearance11.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.refreshDatabasesButton.Appearance = appearance11;
            this.refreshDatabasesButton.Location = new System.Drawing.Point(663, 3);
            this.refreshDatabasesButton.Name = "refreshDatabasesButton";
            this.refreshDatabasesButton.ShowFocusRect = false;
            this.refreshDatabasesButton.ShowOutline = false;
            this.refreshDatabasesButton.Size = new System.Drawing.Size(23, 23);
            this.refreshDatabasesButton.TabIndex = 11;
            this.refreshDatabasesButton.MouseEnterElement += new UIElementEventHandler(mouseEnter_refreshDatabasesButton);
            this.refreshDatabasesButton.MouseLeaveElement += new UIElementEventHandler(mouseLeave_refreshDatabasesButton);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            //Event Methods
            

            // 
            // availabilityGroupsComboBox
            // 
            this.availabilityGroupsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availabilityGroupsComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.availabilityGroupsComboBox.Location = new System.Drawing.Point(3, 4);
            this.availabilityGroupsComboBox.Name = "availabilityGroupsComboBox";
            this.availabilityGroupsComboBox.Size = new System.Drawing.Size(657, 21);
            this.availabilityGroupsComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.availabilityGroupsComboBox.TabIndex = 10;
            this.availabilityGroupsComboBox.SelectionChanged += new System.EventHandler(this.availabilityGroupsComboBox_SelectionChanged);
            // 
            // contentContainerPanel
            // 
            this.contentContainerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.contentContainerPanel.Controls.Add(this.splitContainer1);
            this.contentContainerPanel.Location = new System.Drawing.Point(0, 31);
            this.contentContainerPanel.Name = "contentContainerPanel";
            this.contentContainerPanel.Size = new System.Drawing.Size(689, 486);
            this.contentContainerPanel.TabIndex = 8;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.splitContainer1.Panel1.Controls.Add(this.alwaysOnAvailabilityGroupsGrid);
            this.splitContainer1.Panel1.Controls.Add(this.databasesGridStatusLabel);
            this.splitContainer1.Panel1.Controls.Add(this.actionProgressControl);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel);
            this.splitContainer1.Panel2.Controls.Add(this.alwaysOnStatisticsStatusLabel);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.splitContainer1.Size = new System.Drawing.Size(689, 486);
            this.splitContainer1.SplitterDistance = 199;
            this.splitContainer1.TabIndex = 0;
            // 
            // alwaysOnAvailabilityGroupsGrid
            // 
            appearance12.BackColor = System.Drawing.SystemColors.Window;
            appearance12.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Appearance = appearance12;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn20.Header.VisiblePosition = 2;
            ultraGridColumn17.Header.VisiblePosition = 3;
            ultraGridColumn17.Hidden = true;
            ultraGridColumn21.Header.VisiblePosition = 4;
            ultraGridColumn18.Header.VisiblePosition = 5;
            ultraGridColumn18.Hidden = true;
            appearance13.ImageHAlign = Infragistics.Win.HAlign.Center;
            ultraGridColumn15.Header.Appearance = appearance13;
            ultraGridColumn15.Header.Fixed = true;
            ultraGridColumn15.Header.VisiblePosition = 0;
            ultraGridColumn15.Width = 120;
            ultraGridColumn26.Header.VisiblePosition = 6;
            ultraGridColumn22.Header.VisiblePosition = 7;
            ultraGridColumn33.Header.VisiblePosition = 8;
            ultraGridColumn1.Header.VisiblePosition = 9;
            ultraGridColumn2.Header.VisiblePosition = 10;
            ultraGridColumn38.Header.VisiblePosition = 11;
            ultraGridColumn3.Header.VisiblePosition = 12;
            ultraGridColumn36.Header.VisiblePosition = 13;
            ultraGridColumn24.Header.VisiblePosition = 14;
            ultraGridColumn27.Header.VisiblePosition = 15;
            ultraGridColumn30.Header.VisiblePosition = 16;
            ultraGridColumn30.Hidden = true;
            ultraGridColumn4.Header.VisiblePosition = 1;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 17;
            ultraGridColumn6.Header.VisiblePosition = 18;
            ultraGridColumn7.Header.VisiblePosition = 19;
            ultraGridColumn8.Header.VisiblePosition = 20;
            ultraGridColumn9.Header.VisiblePosition = 21;
            ultraGridColumn10.Header.VisiblePosition = 22;
            ultraGridColumn11.Header.VisiblePosition = 23;
            ultraGridColumn12.Header.VisiblePosition = 24;
            ultraGridColumn12.Hidden = true;
            ultraGridColumn13.Header.VisiblePosition = 25;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn14.Header.VisiblePosition = 26;
            ultraGridColumn14.Hidden = true;
            ultraGridColumn16.Header.VisiblePosition = 27;
            ultraGridColumn19.Header.VisiblePosition = 28;
            ultraGridColumn25.Header.VisiblePosition = 29;
            ultraGridColumn23.Header.VisiblePosition = 30;
            ultraGridColumn23.Hidden = true;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn20,
            ultraGridColumn17,
            ultraGridColumn21,
            ultraGridColumn18,
            ultraGridColumn15,
            ultraGridColumn26,
            ultraGridColumn22,
            ultraGridColumn33,
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn38,
            ultraGridColumn3,
            ultraGridColumn36,
            ultraGridColumn24,
            ultraGridColumn27,
            ultraGridColumn30,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn16,
            ultraGridColumn19,
            ultraGridColumn25,
            ultraGridColumn23});
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.CardAreaAppearance = appearance17;
            appearance18.BorderColor = System.Drawing.Color.Silver;
            appearance18.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.CellAppearance = appearance18;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.CellPadding = 0;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.SystemColors.Control;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.BorderColor = System.Drawing.SystemColors.Window;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance19;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance20.TextHAlignAsString = "Left";
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.HeaderAppearance = appearance20;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.RowAppearance = appearance21;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance22.BackColor = System.Drawing.SystemColors.ControlLight;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance22;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.UseFixedHeaders = true;
            valueList2.Key = "mirrorOperationalStatus";
            valueListItem1.DataValue = -1;
            valueListItem1.DisplayText = " Not set";
            valueListItem2.DataValue = 0;
            valueListItem2.DisplayText = "Failed Over";
            valueListItem3.DataValue = 1;
            valueListItem3.DisplayText = "Normal";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            valueList3.Key = "mirroringState";
            valueListItem4.DataValue = 0;
            valueListItem4.DisplayText = "Suspended";
            valueListItem5.DataValue = 1;
            valueListItem5.DisplayText = "Disconnected";
            valueListItem6.DataValue = 2;
            valueListItem6.DisplayText = "Synchronizing";
            valueListItem7.DataValue = 3;
            valueListItem7.DisplayText = "Pending Failover";
            valueListItem8.DataValue = 4;
            valueListItem8.DisplayText = "Synchronized";
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5,
            valueListItem6,
            valueListItem7,
            valueListItem8});
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList4.Key = "witnessConnectionState";
            valueListItem9.DataValue = 0;
            valueListItem9.DisplayText = "No Witness";
            valueListItem10.DataValue = 1;
            valueListItem10.DisplayText = "Connected";
            valueListItem11.DataValue = 2;
            valueListItem11.DisplayText = "Disconnected";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem9,
            valueListItem10,
            valueListItem11});
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList2,
            valueList3,
            valueList4});
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.alwaysOnAvailabilityGroupsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.alwaysOnAvailabilityGroupsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnAvailabilityGroupsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alwaysOnAvailabilityGroupsGrid.Location = new System.Drawing.Point(0, 1);
            this.alwaysOnAvailabilityGroupsGrid.Name = "alwaysOnAvailabilityGroupsGrid";
            this.alwaysOnAvailabilityGroupsGrid.Size = new System.Drawing.Size(689, 197);
            this.alwaysOnAvailabilityGroupsGrid.TabIndex = 7;
            this.alwaysOnAvailabilityGroupsGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.alwaysOnAvailabilityGroupsGrid_InitializeLayout);
            this.alwaysOnAvailabilityGroupsGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.alwaysOnAvailabilityGroupsGrid_AfterSelectChange);
            this.alwaysOnAvailabilityGroupsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.alwaysOnAvailabilityGroupsGrid_MouseDown);
            // 
            // databasesGridStatusLabel
            // 
            this.databasesGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.databasesGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.databasesGridStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.databasesGridStatusLabel.Name = "databasesGridStatusLabel";
            this.databasesGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.databasesGridStatusLabel.Size = new System.Drawing.Size(689, 197);
            this.databasesGridStatusLabel.TabIndex = 6;
            this.databasesGridStatusLabel.Text = "< Status Message >";
            this.databasesGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.alwaysOnRatePanel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.alwaysOnSizePanel, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 1);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(689, 282);
            this.tableLayoutPanel.TabIndex = 9;
            // 
            // alwaysOnRatePanel
            // 
            this.alwaysOnRatePanel.Controls.Add(this.alwaysOnRateChartContainerPanel);
            this.alwaysOnRatePanel.Controls.Add(this.alwaysOnRateHeaderStrip);
            this.alwaysOnRatePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnRatePanel.Location = new System.Drawing.Point(347, 3);
            this.alwaysOnRatePanel.Name = "alwaysOnRatePanel";
            this.alwaysOnRatePanel.Size = new System.Drawing.Size(339, 276);
            this.alwaysOnRatePanel.TabIndex = 4;
            // 
            // alwaysOnRateChartContainerPanel
            // 
            this.alwaysOnRateChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.alwaysOnRateChartContainerPanel.Controls.Add(this.alwaysOnRateChart);
            this.alwaysOnRateChartContainerPanel.Controls.Add(this.alwaysOnRateChartStatusLabel);
            this.alwaysOnRateChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnRateChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.alwaysOnRateChartContainerPanel.Name = "alwaysOnRateChartContainerPanel";
            this.alwaysOnRateChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.alwaysOnRateChartContainerPanel.Size = new System.Drawing.Size(339, 257);
            this.alwaysOnRateChartContainerPanel.TabIndex = 12;
            // 
            // alwaysOnRateChart
            // 
            this.alwaysOnRateChart.AllSeries.MarkerSize = ((short)(2));
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.alwaysOnRateChart.Background = gradientBackground1;
            this.alwaysOnRateChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.alwaysOnRateChart.ContextMenus = false;
            this.alwaysOnRateChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.alwaysOnRateChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnRateChart.LegendBox.LineSpacing = 1D;
            this.alwaysOnRateChart.Location = new System.Drawing.Point(1, 0);
            this.alwaysOnRateChart.Name = "alwaysOnRateChart";
            this.alwaysOnRateChart.Palette = "Schemes.Classic";
            this.alwaysOnRateChart.PlotAreaColor = System.Drawing.Color.White;
            this.alwaysOnRateChart.RandomData.Series = 2;
            this.alwaysOnRateChart.Size = new System.Drawing.Size(337, 256);
            this.alwaysOnRateChart.TabIndex = 13;
            // 
            // alwaysOnRateChartStatusLabel
            // 
            this.alwaysOnRateChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.alwaysOnRateChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnRateChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.alwaysOnRateChartStatusLabel.Name = "alwaysOnRateChartStatusLabel";
            this.alwaysOnRateChartStatusLabel.Size = new System.Drawing.Size(337, 256);
            this.alwaysOnRateChartStatusLabel.TabIndex = 12;
            this.alwaysOnRateChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.alwaysOnRateChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // alwaysOnRateHeaderStrip
            // 
            this.alwaysOnRateHeaderStrip.AutoSize = false;
            this.alwaysOnRateHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.alwaysOnRateHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.alwaysOnRateHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.alwaysOnRateHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.alwaysOnRateHeaderStrip.HotTrackEnabled = false;
            this.alwaysOnRateHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maximizeAlwaysOnRateChartButton,
            this.restoreAlwaysOnRateChartButton,
            this.toolStripLabel1});
            this.alwaysOnRateHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.alwaysOnRateHeaderStrip.Name = "alwaysOnRateHeaderStrip";
            this.alwaysOnRateHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.alwaysOnRateHeaderStrip.Size = new System.Drawing.Size(339, 19);
            this.alwaysOnRateHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.alwaysOnRateHeaderStrip.TabIndex = 10;
            // 
            // maximizeAlwaysOnRateChartButton
            // 
            this.maximizeAlwaysOnRateChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeAlwaysOnRateChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeAlwaysOnRateChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeAlwaysOnRateChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeAlwaysOnRateChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeAlwaysOnRateChartButton.Name = "maximizeAlwaysOnRateChartButton";
            this.maximizeAlwaysOnRateChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeAlwaysOnRateChartButton.ToolTipText = "Maximize";
            this.maximizeAlwaysOnRateChartButton.Click += new System.EventHandler(this.maximizeAlwaysOnRateChartButton_Click);
            // 
            // restoreAlwaysOnRateChartButton
            // 
            this.restoreAlwaysOnRateChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreAlwaysOnRateChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreAlwaysOnRateChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreAlwaysOnRateChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreAlwaysOnRateChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreAlwaysOnRateChartButton.Name = "restoreAlwaysOnRateChartButton";
            this.restoreAlwaysOnRateChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreAlwaysOnRateChartButton.Text = "Restore";
            this.restoreAlwaysOnRateChartButton.Visible = false;
            this.restoreAlwaysOnRateChartButton.Click += new System.EventHandler(this.restoreAlwaysOnRateChartButton_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(180, 16);
            this.toolStripLabel1.Text = "Transfer Rates (Redo and Log)";
            // 
            // alwaysOnSizePanel
            // 
            this.alwaysOnSizePanel.Controls.Add(this.alwaysOnSizeChartContainerPanel);
            this.alwaysOnSizePanel.Controls.Add(this.alwaysOnSizeHeaderStrip);
            this.alwaysOnSizePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnSizePanel.Location = new System.Drawing.Point(3, 3);
            this.alwaysOnSizePanel.Name = "alwaysOnSizePanel";
            this.alwaysOnSizePanel.Size = new System.Drawing.Size(338, 276);
            this.alwaysOnSizePanel.TabIndex = 3;
            // 
            // alwaysOnSizeChartContainerPanel
            // 
            this.alwaysOnSizeChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(202)))), ((int)(((byte)(202)))), ((int)(((byte)(202)))));
            this.alwaysOnSizeChartContainerPanel.Controls.Add(this.alwaysOnSizeChart);
            this.alwaysOnSizeChartContainerPanel.Controls.Add(this.alwaysOnSizeChartStatusLabel);
            this.alwaysOnSizeChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnSizeChartContainerPanel.Location = new System.Drawing.Point(0, 19);
            this.alwaysOnSizeChartContainerPanel.Name = "alwaysOnSizeChartContainerPanel";
            this.alwaysOnSizeChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.alwaysOnSizeChartContainerPanel.Size = new System.Drawing.Size(338, 257);
            this.alwaysOnSizeChartContainerPanel.TabIndex = 12;
            // 
            // alwaysOnSizeChart
            // 
            this.alwaysOnSizeChart.AllSeries.BarShape = ChartFX.WinForms.BarShape.Cylinder;
            this.alwaysOnSizeChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Gantt;
            this.alwaysOnSizeChart.AllSeries.Stacked = ChartFX.WinForms.Stacked.Normal;
            gradientBackground2.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.alwaysOnSizeChart.Background = gradientBackground2;
            this.alwaysOnSizeChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.alwaysOnSizeChart.ContextMenus = false;
            this.alwaysOnSizeChart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.alwaysOnSizeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnSizeChart.LegendBox.PlotAreaOnly = false;
            this.alwaysOnSizeChart.Location = new System.Drawing.Point(1, 0);
            this.alwaysOnSizeChart.Name = "alwaysOnSizeChart";
            this.alwaysOnSizeChart.Palette = "Schemes.Classic";
            this.alwaysOnSizeChart.PlotAreaColor = System.Drawing.Color.White;
            this.alwaysOnSizeChart.RandomData.Series = 2;
            this.alwaysOnSizeChart.Size = new System.Drawing.Size(336, 256);
            this.alwaysOnSizeChart.TabIndex = 13;
            // 
            // alwaysOnSizeChartStatusLabel
            // 
            this.alwaysOnSizeChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.alwaysOnSizeChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnSizeChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.alwaysOnSizeChartStatusLabel.Name = "alwaysOnSizeChartStatusLabel";
            this.alwaysOnSizeChartStatusLabel.Size = new System.Drawing.Size(336, 256);
            this.alwaysOnSizeChartStatusLabel.TabIndex = 12;
            this.alwaysOnSizeChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.alwaysOnSizeChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // alwaysOnSizeHeaderStrip
            // 
            this.alwaysOnSizeHeaderStrip.AutoSize = false;
            this.alwaysOnSizeHeaderStrip.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.alwaysOnSizeHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.alwaysOnSizeHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.alwaysOnSizeHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.alwaysOnSizeHeaderStrip.HotTrackEnabled = false;
            this.alwaysOnSizeHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maximizeAlwaysOnSizeChartButton,
            this.restoreAlwaysOnSizeChartButton,
            this.toolStripLabel2});
            this.alwaysOnSizeHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.alwaysOnSizeHeaderStrip.Name = "alwaysOnSizeHeaderStrip";
            this.alwaysOnSizeHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.alwaysOnSizeHeaderStrip.Size = new System.Drawing.Size(338, 19);
            this.alwaysOnSizeHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.alwaysOnSizeHeaderStrip.TabIndex = 10;
            // 
            // maximizeAlwaysOnSizeChartButton
            // 
            this.maximizeAlwaysOnSizeChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.maximizeAlwaysOnSizeChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.maximizeAlwaysOnSizeChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Maximize;
            this.maximizeAlwaysOnSizeChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.maximizeAlwaysOnSizeChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.maximizeAlwaysOnSizeChartButton.Name = "maximizeAlwaysOnSizeChartButton";
            this.maximizeAlwaysOnSizeChartButton.Size = new System.Drawing.Size(23, 16);
            this.maximizeAlwaysOnSizeChartButton.ToolTipText = "Maximize";
            this.maximizeAlwaysOnSizeChartButton.Click += new System.EventHandler(this.maximizeAlwaysOnSizeChartButton_Click);
            // 
            // restoreAlwaysOnSizeChartButton
            // 
            this.restoreAlwaysOnSizeChartButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.restoreAlwaysOnSizeChartButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.restoreAlwaysOnSizeChartButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.RestoreDown;
            this.restoreAlwaysOnSizeChartButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.restoreAlwaysOnSizeChartButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.restoreAlwaysOnSizeChartButton.Name = "restoreAlwaysOnSizeChartButton";
            this.restoreAlwaysOnSizeChartButton.Size = new System.Drawing.Size(23, 16);
            this.restoreAlwaysOnSizeChartButton.Text = "Restore";
            this.restoreAlwaysOnSizeChartButton.Visible = false;
            this.restoreAlwaysOnSizeChartButton.Click += new System.EventHandler(this.restoreAlwaysOnSizeChartButton_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(158, 16);
            this.toolStripLabel2.Text = "Queue Size (Redo and Log)";
            // 
            // alwaysOnStatisticsStatusLabel
            // 
            this.alwaysOnStatisticsStatusLabel.BackColor = System.Drawing.Color.White;
            this.alwaysOnStatisticsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alwaysOnStatisticsStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.alwaysOnStatisticsStatusLabel.Name = "alwaysOnStatisticsStatusLabel";
            this.alwaysOnStatisticsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.alwaysOnStatisticsStatusLabel.Size = new System.Drawing.Size(689, 282);
            this.alwaysOnStatisticsStatusLabel.TabIndex = 8;
            this.alwaysOnStatisticsStatusLabel.Text = "< Status Message >";
            this.alwaysOnStatisticsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // disksTabPage
            // 
            this.disksTabPage.Controls.Add(this.diskUsagePanel);
            this.disksTabPage.Location = new System.Drawing.Point(-10000, -10000);
            this.disksTabPage.Name = "disksTabPage";
            this.disksTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.disksTabPage.Size = new System.Drawing.Size(685, 270);
            // 
            // diskUsagePanel
            // 
            this.diskUsagePanel.Controls.Add(this.currentDiskUsageChartContainerPanel);
            this.diskUsagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskUsagePanel.Location = new System.Drawing.Point(3, 3);
            this.diskUsagePanel.Name = "diskUsagePanel";
            this.diskUsagePanel.Size = new System.Drawing.Size(679, 264);
            this.diskUsagePanel.TabIndex = 4;
            // 
            // currentDiskUsageChartContainerPanel
            // 
            this.currentDiskUsageChartContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(173)))), ((int)(((byte)(209)))), ((int)(((byte)(255)))));
            this.currentDiskUsageChartContainerPanel.Controls.Add(this.diskUsageChartStatusLabel);
            this.currentDiskUsageChartContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.currentDiskUsageChartContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.currentDiskUsageChartContainerPanel.Name = "currentDiskUsageChartContainerPanel";
            this.currentDiskUsageChartContainerPanel.Padding = new System.Windows.Forms.Padding(1, 0, 1, 1);
            this.currentDiskUsageChartContainerPanel.Size = new System.Drawing.Size(679, 264);
            this.currentDiskUsageChartContainerPanel.TabIndex = 12;
            // 
            // diskUsageChartStatusLabel
            // 
            this.diskUsageChartStatusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            this.diskUsageChartStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diskUsageChartStatusLabel.Location = new System.Drawing.Point(1, 0);
            this.diskUsageChartStatusLabel.Name = "diskUsageChartStatusLabel";
            this.diskUsageChartStatusLabel.Size = new System.Drawing.Size(677, 263);
            this.diskUsageChartStatusLabel.TabIndex = 12;
            this.diskUsageChartStatusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.diskUsageChartStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "detailsGridContextMenu";
            buttonTool24.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool24,
            buttonTool25});
            appearance23.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool1.SharedPropsInternal.Caption = "Sort Ascending";
            appearance24.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool2.SharedPropsInternal.Caption = "Sort Descending";
            popupMenuTool2.SharedPropsInternal.Caption = "columnsContextMenu";
            buttonTool20.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool5,
            buttonTool6,
            stateButtonTool3,
            buttonTool33,
            buttonTool20,
            buttonTool21});
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool27.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool27,
            buttonTool28});
            appearance25.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance25;
            buttonTool11.SharedPropsInternal.Caption = "Print Grid";
            appearance26.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance26;
            buttonTool12.SharedPropsInternal.Caption = "Export Grid";
            buttonTool13.SharedPropsInternal.Caption = "Remove This Column";
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance27;
            buttonTool14.SharedPropsInternal.Caption = "Column Chooser";
            appearance28.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance28;
            buttonTool15.SharedPropsInternal.Caption = "Group By Box";
            appearance29.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByThisColumn;
            stateButtonTool1.SharedPropsInternal.AppearancesSmall.Appearance = appearance29;
            stateButtonTool1.SharedPropsInternal.Caption = "Group By This Column";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool1,
            buttonTool2,
            popupMenuTool2,
            popupMenuTool3,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            stateButtonTool1});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // operationalStatusPanel - SQLDM 10.0.0 Ankit Nagpal
            // 
            this.operationalStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.operationalStatusPanel.Controls.Add(this.operationalStatusImage);
            this.operationalStatusPanel.Controls.Add(this.operationalStatusLabel);
            this.operationalStatusPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.operationalStatusPanel.Location = new System.Drawing.Point(0, 0);
            this.operationalStatusPanel.Name = "operationalStatusPanel";
            this.operationalStatusPanel.Size = new System.Drawing.Size(748, 27);
            this.operationalStatusPanel.TabIndex = 17;
            this.operationalStatusPanel.Visible = false;
            // 
            // operationalStatusImage
            // 
            this.operationalStatusImage.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusImage.Image = ((System.Drawing.Image)(resources.GetObject("operationalStatusImage.Image")));
            this.operationalStatusImage.Location = new System.Drawing.Point(7, 5);
            this.operationalStatusImage.Name = "operationalStatusImage";
            this.operationalStatusImage.Size = new System.Drawing.Size(16, 16);
            this.operationalStatusImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.operationalStatusImage.TabIndex = 3;
            this.operationalStatusImage.TabStop = false;
            // 
            // operationalStatusLabel
            // 
            this.operationalStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.operationalStatusLabel.BackColor = System.Drawing.Color.LightGray;
            this.operationalStatusLabel.ForeColor = System.Drawing.Color.Black;
            this.operationalStatusLabel.Location = new System.Drawing.Point(4, 3);
            this.operationalStatusLabel.Name = "operationalStatusLabel";
            this.operationalStatusLabel.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.operationalStatusLabel.Size = new System.Drawing.Size(740, 21);
            this.operationalStatusLabel.TabIndex = 2;
            this.operationalStatusLabel.Text = "< status message >";
            this.operationalStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.operationalStatusLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseDown);
            this.operationalStatusLabel.MouseEnter += new System.EventHandler(this.operationalStatusLabel_MouseEnter);
            this.operationalStatusLabel.MouseLeave += new System.EventHandler(this.operationalStatusLabel_MouseLeave);
            this.operationalStatusLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.operationalStatusLabel_MouseUp);            // 
            // refreshAlwaysOnDetailsBackgroundWorker
            // 
            this.refreshAlwaysOnDetailsBackgroundWorker.WorkerSupportsCancellation = true;
            // 
            // DatabasesAlwaysOnView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.DatabasesAlwaysOn_Fill_Panel);
            this.Controls.Add(this.operationalStatusPanel);
            this.Name = "DatabasesAlwaysOnView";
            this.Size = new System.Drawing.Size(689, 517);
            this.Load += new System.EventHandler(this.DatabasesAlwaysOnView_Load);
            this.propertiesTabPage.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.transactionLogsGrid)).EndInit();
            this.DatabasesAlwaysOn_Fill_Panel.ResumeLayout(false);
            this.DatabasesAlwaysOn_Fill_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availabilityGroupsComboBox)).EndInit();
            this.contentContainerPanel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnAvailabilityGroupsGrid)).EndInit();
            this.tableLayoutPanel.ResumeLayout(false);
            this.alwaysOnRatePanel.ResumeLayout(false);
            this.alwaysOnRateChartContainerPanel.ResumeLayout(false);
            this.alwaysOnRateChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnRateChart)).EndInit();
            this.alwaysOnRateHeaderStrip.ResumeLayout(false);
            this.alwaysOnRateHeaderStrip.PerformLayout();
            this.alwaysOnSizePanel.ResumeLayout(false);
            this.alwaysOnSizeChartContainerPanel.ResumeLayout(false);
            this.alwaysOnSizeChartContainerPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alwaysOnSizeChart)).EndInit();
            this.alwaysOnSizeHeaderStrip.ResumeLayout(false);
            this.alwaysOnSizeHeaderStrip.PerformLayout();
            this.disksTabPage.ResumeLayout(false);
            this.diskUsagePanel.ResumeLayout(false);
            this.currentDiskUsageChartContainerPanel.ResumeLayout(false);
            this.operationalStatusPanel.ResumeLayout(false);
            this.operationalStatusPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.operationalStatusImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  DatabasesAlwaysOn_Fill_Panel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  contentContainerPanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Infragistics.Win.UltraWinGrid.UltraGrid alwaysOnAvailabilityGroupsGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel databasesGridStatusLabel;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl disksTabPage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  diskUsagePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  currentDiskUsageChartContainerPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel diskUsageChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel alwaysOnStatisticsStatusLabel;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl propertiesTabPage;
        private Infragistics.Win.UltraWinGrid.UltraGrid transactionLogsGrid;
        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private MRG.Controls.UI.LoadingCircle actionProgressControl;
        private Infragistics.Win.Misc.UltraButton refreshDatabasesButton;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor availabilityGroupsComboBox;
        private System.ComponentModel.BackgroundWorker refreshAlwaysOnDetailsBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  alwaysOnRatePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  alwaysOnRateChartContainerPanel;
        private ChartFX.WinForms.Chart alwaysOnRateChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel alwaysOnRateChartStatusLabel;
        private Controls.HeaderStrip alwaysOnRateHeaderStrip;
        private System.Windows.Forms.ToolStripButton maximizeAlwaysOnRateChartButton;
        private System.Windows.Forms.ToolStripButton restoreAlwaysOnRateChartButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  alwaysOnSizePanel;
        private Controls.HeaderStrip alwaysOnSizeHeaderStrip;
        private System.Windows.Forms.ToolStripButton maximizeAlwaysOnSizeChartButton;
        private System.Windows.Forms.ToolStripButton restoreAlwaysOnSizeChartButton;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  alwaysOnSizeChartContainerPanel;
        private ChartFX.WinForms.Chart alwaysOnSizeChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel alwaysOnSizeChartStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  operationalStatusPanel;
        private System.Windows.Forms.PictureBox operationalStatusImage;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel operationalStatusLabel;
        Infragistics.Win.Appearance appearance1;
        Infragistics.Win.Appearance appearance2;
        Infragistics.Win.Appearance appearance3;
        Infragistics.Win.Appearance appearance4;
        Infragistics.Win.Appearance appearance5;
        Infragistics.Win.Appearance appearance6;
        Infragistics.Win.Appearance appearance7;
        Infragistics.Win.Appearance appearance8;
        Infragistics.Win.Appearance appearance9;
        Infragistics.Win.Appearance appearance10;
        Infragistics.Win.ValueList valueList1;
        Infragistics.Win.Appearance appearance11;
        Infragistics.Win.Appearance appearance12;
    }
}
