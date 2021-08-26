using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Services
{
    partial class ServicesSqlAgentJobsView
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        /// 
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("startJobButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("stopJobButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("startJobButton");
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("stopJobButton");
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("viewMessageButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("jobHistoryContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("viewMessageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("JobId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Category");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled", -1, 1218384219);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Scheduled", -1, 1218384219);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status", -1, 2099348547);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Run Outcome", -1, 2099358610);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Run Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Run Duration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Next Run Time");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Owner");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Description");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(2099348547);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(2099358610);
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(1218384219);
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Job Executions", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("JobId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Job Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Outcome", -1, -2117334295);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Retries");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Execution Steps");
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Execution Steps", 0);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExecutionInstanceId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceId");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Step ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Step Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Outcome", -1, -2117334295);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Duration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Retries");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Message");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(-2117334295);
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServicesSqlAgentJobsView));
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.ServicesSqlAgentJobsView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.jobSummaryGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.jobSummaryGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobHistoryGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.jobHistoryGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobHistoryHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.hideJobHistoryButton = new System.Windows.Forms.ToolStripButton();
            this.jobHistoryHeaderStripDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.jobHistoryAllToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.jobHistoryFailedToolStripMenuItem = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.ServicesSqlAgentJobsView_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.jobSummaryGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobHistoryGrid)).BeginInit();
            this.jobHistoryHeaderStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 0;
            this.toolbarsManager.ShowFullMenusDelay = 500;
            popupMenuTool1.SharedPropsInternal.Caption = "columnContextMenu";
            stateButtonTool1.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            popupMenuTool1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool1,
            buttonTool2,
            stateButtonTool1,
            buttonTool3,
            buttonTool4,
            buttonTool5});
            appearance23.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance23;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance24.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance24;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance25.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance25;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance26.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance26;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance27;
            buttonTool11.SharedPropsInternal.Caption = "Print";
            appearance28.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance28;
            buttonTool12.SharedPropsInternal.Caption = "Export to Excel";
            popupMenuTool2.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16});
            popupMenuTool3.SharedPropsInternal.Caption = "gridDataContextMenu";
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            buttonTool19.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool25,
            buttonTool26,
            buttonTool17,
            buttonTool18,
            buttonTool19,
            buttonTool20});
            buttonTool21.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool22.SharedPropsInternal.Caption = "Expand All Groups";
            appearance39.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StartTrace;
            buttonTool23.SharedPropsInternal.AppearancesSmall.Appearance = appearance39;
            buttonTool23.SharedPropsInternal.Caption = "Start Job";
            appearance40.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.StopTrace;
            buttonTool24.SharedPropsInternal.AppearancesSmall.Appearance = appearance40;
            buttonTool24.SharedPropsInternal.Caption = "Stop Job";
            buttonTool27.SharedPropsInternal.Caption = "View Message";
            buttonTool27.SharedPropsInternal.CustomizerCaption = "View Message";
            popupMenuTool4.SharedPropsInternal.Caption = "jobHistoryContextMenu";
            buttonTool28.InstanceProps.IsFirstInGroup = true;
            buttonTool31.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool30,
            buttonTool28,
            buttonTool29,
            buttonTool31,
            buttonTool32});
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            buttonTool11,
            buttonTool12,
            popupMenuTool2,
            popupMenuTool3,
            buttonTool21,
            buttonTool22,
            buttonTool23,
            buttonTool24,
            buttonTool27,
            popupMenuTool4});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // ServicesSqlAgentJobsView_Fill_Panel
            // 
            this.ServicesSqlAgentJobsView_Fill_Panel.Controls.Add(this.splitContainer);
            this.ServicesSqlAgentJobsView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServicesSqlAgentJobsView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServicesSqlAgentJobsView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ServicesSqlAgentJobsView_Fill_Panel.Name = "ServicesSqlAgentJobsView_Fill_Panel";
            this.ServicesSqlAgentJobsView_Fill_Panel.Size = new System.Drawing.Size(708, 553);
            this.ServicesSqlAgentJobsView_Fill_Panel.TabIndex = 8;
            // 
            // splitContainer
            // 
            this.splitContainer.DataBindings.Add(new System.Windows.Forms.Binding("SplitterDistance", global::Idera.SQLdm.DesktopClient.Properties.Settings.Default, "ServicesAgentJobsViewMainSplitter", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer.Panel1.Controls.Add(this.jobSummaryGrid);
            this.splitContainer.Panel1.Controls.Add(this.jobSummaryGridStatusLabel);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.jobHistoryGrid);
            this.splitContainer.Panel2.Controls.Add(this.jobHistoryGridStatusLabel);
            this.splitContainer.Panel2.Controls.Add(this.jobHistoryHeaderStrip);
            this.splitContainer.Size = new System.Drawing.Size(708, 553);
            this.splitContainer.SplitterDistance = 234;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // jobSummaryGrid
            // 
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.jobSummaryGrid.DisplayLayout.Appearance = appearance1;
            this.jobSummaryGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.Fixed = true;
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn8.Format = "G";
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn10.Format = "G";
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4,
            ultraGridColumn5,
            ultraGridColumn6,
            ultraGridColumn7,
            ultraGridColumn8,
            ultraGridColumn9,
            ultraGridColumn10,
            ultraGridColumn11,
            ultraGridColumn12});
            this.jobSummaryGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.jobSummaryGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.jobSummaryGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.jobSummaryGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.jobSummaryGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.jobSummaryGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.jobSummaryGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.jobSummaryGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.jobSummaryGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.jobSummaryGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.jobSummaryGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.jobSummaryGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.jobSummaryGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.jobSummaryGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.jobSummaryGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.jobSummaryGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.jobSummaryGrid.DisplayLayout.Override.CellAppearance = appearance6;
            this.jobSummaryGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.jobSummaryGrid.DisplayLayout.Override.CellPadding = 0;
            this.jobSummaryGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.jobSummaryGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.jobSummaryGrid.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.jobSummaryGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Left";
            this.jobSummaryGrid.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.jobSummaryGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.jobSummaryGrid.DisplayLayout.Override.RowAppearance = appearance9;
            this.jobSummaryGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.jobSummaryGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobSummaryGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobSummaryGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobSummaryGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.jobSummaryGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.jobSummaryGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.jobSummaryGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.jobSummaryGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "statusValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.Key = "outcomeValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.Key = "yesNoValueList";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.jobSummaryGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.jobSummaryGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.jobSummaryGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.jobSummaryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobSummaryGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jobSummaryGrid.Location = new System.Drawing.Point(0, 0);
            this.jobSummaryGrid.Name = "jobSummaryGrid";
            this.jobSummaryGrid.Size = new System.Drawing.Size(708, 233);
            this.jobSummaryGrid.TabIndex = 4;
            this.jobSummaryGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.jobSummaryGrid_AfterSelectChange);
            this.jobSummaryGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jobSummaryGrid_MouseDown);
            // 
            // jobSummaryGridStatusLabel
            // 
            this.jobSummaryGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.jobSummaryGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobSummaryGridStatusLabel.Location = new System.Drawing.Point(0, 0);
            this.jobSummaryGridStatusLabel.Name = "jobSummaryGridStatusLabel";
            this.jobSummaryGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.jobSummaryGridStatusLabel.Size = new System.Drawing.Size(708, 233);
            this.jobSummaryGridStatusLabel.TabIndex = 7;
            this.jobSummaryGridStatusLabel.Text = "< Status Message >";
            this.jobSummaryGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // jobHistoryGrid
            // 
            appearance11.BackColor = System.Drawing.SystemColors.Window;
            appearance11.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.jobHistoryGrid.DisplayLayout.Appearance = appearance11;
            this.jobHistoryGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn13.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn13.Header.VisiblePosition = 1;
            ultraGridColumn13.Hidden = true;
            ultraGridColumn14.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn14.Header.VisiblePosition = 2;
            ultraGridColumn14.Hidden = true;
            ultraGridColumn15.ColSpan = ((short)(2));
            ultraGridColumn15.Format = "";
            ultraGridColumn15.Header.Fixed = true;
            ultraGridColumn15.Header.VisiblePosition = 0;
            ultraGridColumn15.Width = 218;
            ultraGridColumn16.Format = "G";
            ultraGridColumn16.Header.VisiblePosition = 3;
            ultraGridColumn17.Header.VisiblePosition = 4;
            ultraGridColumn17.Width = 87;
            ultraGridColumn18.Header.VisiblePosition = 5;
            appearance12.TextHAlignAsString = "Right";
            ultraGridColumn19.CellAppearance = appearance12;
            ultraGridColumn19.Header.VisiblePosition = 6;
            ultraGridColumn19.Width = 45;
            ultraGridColumn20.Header.VisiblePosition = 7;
            ultraGridColumn21.Header.VisiblePosition = 8;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21});
            ultraGridColumn22.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn22.Header.VisiblePosition = 0;
            ultraGridColumn22.Hidden = true;
            ultraGridColumn23.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn23.Header.VisiblePosition = 1;
            ultraGridColumn23.Hidden = true;
            ultraGridColumn24.Header.VisiblePosition = 2;
            ultraGridColumn24.MinWidth = 45;
            ultraGridColumn24.Width = 45;
            ultraGridColumn25.Header.VisiblePosition = 3;
            ultraGridColumn25.Width = 154;
            ultraGridColumn26.Format = "G";
            ultraGridColumn26.Header.VisiblePosition = 4;
            ultraGridColumn27.Header.VisiblePosition = 5;
            ultraGridColumn27.Width = 87;
            ultraGridColumn28.Header.VisiblePosition = 6;
            appearance13.TextHAlignAsString = "Right";
            ultraGridColumn29.CellAppearance = appearance13;
            ultraGridColumn29.Format = "N0";
            ultraGridColumn29.Header.VisiblePosition = 7;
            ultraGridColumn29.Width = 45;
            ultraGridColumn30.Header.VisiblePosition = 8;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30});
            this.jobHistoryGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.jobHistoryGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.jobHistoryGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.jobHistoryGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance14.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance14.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.jobHistoryGrid.DisplayLayout.GroupByBox.Appearance = appearance14;
            appearance15.ForeColor = System.Drawing.SystemColors.GrayText;
            this.jobHistoryGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance15;
            this.jobHistoryGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.jobHistoryGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance16.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance16.BackColor2 = System.Drawing.SystemColors.Control;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.ForeColor = System.Drawing.SystemColors.GrayText;
            this.jobHistoryGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance16;
            this.jobHistoryGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.jobHistoryGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.jobHistoryGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.jobHistoryGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.jobHistoryGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.jobHistoryGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.jobHistoryGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            this.jobHistoryGrid.DisplayLayout.Override.CardAreaAppearance = appearance17;
            appearance18.BorderColor = System.Drawing.Color.Silver;
            appearance18.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.jobHistoryGrid.DisplayLayout.Override.CellAppearance = appearance18;
            this.jobHistoryGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.jobHistoryGrid.DisplayLayout.Override.CellPadding = 0;
            this.jobHistoryGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.jobHistoryGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance19.BackColor = System.Drawing.SystemColors.Control;
            appearance19.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance19.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance19.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance19.BorderColor = System.Drawing.SystemColors.Window;
            this.jobHistoryGrid.DisplayLayout.Override.GroupByRowAppearance = appearance19;
            this.jobHistoryGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance20.TextHAlignAsString = "Left";
            this.jobHistoryGrid.DisplayLayout.Override.HeaderAppearance = appearance20;
            this.jobHistoryGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance21.BackColor = System.Drawing.SystemColors.Window;
            appearance21.BorderColor = System.Drawing.Color.Silver;
            this.jobHistoryGrid.DisplayLayout.Override.RowAppearance = appearance21;
            this.jobHistoryGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.jobHistoryGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobHistoryGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobHistoryGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.jobHistoryGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance22.BackColor = System.Drawing.SystemColors.ControlLight;
            this.jobHistoryGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance22;
            this.jobHistoryGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.jobHistoryGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.jobHistoryGrid.DisplayLayout.UseFixedHeaders = true;
            valueList4.Key = "outcomeValueList";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.jobHistoryGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList4});
            this.jobHistoryGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.jobHistoryGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobHistoryGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jobHistoryGrid.Location = new System.Drawing.Point(0, 19);
            this.jobHistoryGrid.Name = "jobHistoryGrid";
            this.jobHistoryGrid.Size = new System.Drawing.Size(708, 296);
            this.jobHistoryGrid.TabIndex = 12;
            this.jobHistoryGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.jobHistoryGrid_MouseDown);
            // 
            // jobHistoryGridStatusLabel
            // 
            this.jobHistoryGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.jobHistoryGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobHistoryGridStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.jobHistoryGridStatusLabel.Name = "jobHistoryGridStatusLabel";
            this.jobHistoryGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.jobHistoryGridStatusLabel.Size = new System.Drawing.Size(708, 296);
            this.jobHistoryGridStatusLabel.TabIndex = 13;
            this.jobHistoryGridStatusLabel.Text = "< Status Message >";
            this.jobHistoryGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // jobHistoryHeaderStrip
            // 
            this.jobHistoryHeaderStrip.AutoSize = false;
            this.jobHistoryHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.jobHistoryHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.jobHistoryHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.jobHistoryHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.jobHistoryHeaderStrip.HotTrackEnabled = false;
            this.jobHistoryHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideJobHistoryButton,
            this.jobHistoryHeaderStripDropDownButton});
            this.jobHistoryHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.jobHistoryHeaderStrip.Name = "jobHistoryHeaderStrip";
            this.jobHistoryHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.jobHistoryHeaderStrip.Size = new System.Drawing.Size(708, 19);
            this.jobHistoryHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.jobHistoryHeaderStrip.TabIndex = 11;
            // 
            // hideJobHistoryButton
            // 
            this.hideJobHistoryButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideJobHistoryButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideJobHistoryButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.hideJobHistoryButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.hideJobHistoryButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideJobHistoryButton.Name = "hideJobHistoryButton";
            this.hideJobHistoryButton.Size = new System.Drawing.Size(23, 16);
            this.hideJobHistoryButton.ToolTipText = "Maximize";
            this.hideJobHistoryButton.Click += new System.EventHandler(this.hideJobHistoryButton_Click);
            // 
            // jobHistoryHeaderStripDropDownButton
            // 
            this.jobHistoryHeaderStripDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.jobHistoryHeaderStripDropDownButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.jobHistoryAllToolStripMenuItem,
            this.jobHistoryFailedToolStripMenuItem});
            this.jobHistoryHeaderStripDropDownButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.jobHistoryHeaderStripDropDownButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.jobHistoryHeaderStripDropDownButton.Image = ((System.Drawing.Image)(resources.GetObject("jobHistoryHeaderStripDropDownButton.Image")));
            this.jobHistoryHeaderStripDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.jobHistoryHeaderStripDropDownButton.Name = "jobHistoryHeaderStripDropDownButton";
            this.jobHistoryHeaderStripDropDownButton.Size = new System.Drawing.Size(104, 16);
            this.jobHistoryHeaderStripDropDownButton.Text = "Job History: All";
            // 
            // jobHistoryAllToolStripMenuItem
            // 
            this.jobHistoryAllToolStripMenuItem.Name = "jobHistoryAllToolStripMenuItem";
            this.jobHistoryAllToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.jobHistoryAllToolStripMenuItem.Text = "All";
            this.jobHistoryAllToolStripMenuItem.Click += new System.EventHandler(this.jobHistoryAllToolStripMenuItem_Click);
            // 
            // jobHistoryFailedToolStripMenuItem
            // 
            this.jobHistoryFailedToolStripMenuItem.Name = "jobHistoryFailedToolStripMenuItem";
            this.jobHistoryFailedToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
            this.jobHistoryFailedToolStripMenuItem.Text = "Failed";
            this.jobHistoryFailedToolStripMenuItem.Click += new System.EventHandler(this.jobHistoryFailedToolStripMenuItem_Click);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "SQL Agent Jobs";
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(708, 553);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 31;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
    "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // ServicesSqlAgentJobsView
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ServicesSqlAgentJobsView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "ServicesSqlAgentJobsView";
            this.Size = new System.Drawing.Size(708, 553);
            this.Load += new System.EventHandler(this.ServicesSqlAgentJobsView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ServicesSqlAgentJobsView_Fill_Panel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.jobSummaryGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.jobHistoryGrid)).EndInit();
            this.jobHistoryHeaderStrip.ResumeLayout(false);
            this.jobHistoryHeaderStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  ServicesSqlAgentJobsView_Fill_Panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinGrid.UltraGrid jobSummaryGrid;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip jobHistoryHeaderStrip;
        private System.Windows.Forms.ToolStripButton hideJobHistoryButton;
        private Infragistics.Win.UltraWinGrid.UltraGrid jobHistoryGrid;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel jobSummaryGridStatusLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel jobHistoryGridStatusLabel;
        private System.Windows.Forms.ToolStripDropDownButton jobHistoryHeaderStripDropDownButton;
        private System.Windows.Forms.ToolStripMenuItem jobHistoryAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem jobHistoryFailedToolStripMenuItem;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
    }
}
