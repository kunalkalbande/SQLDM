using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinGrid;
using Microsoft.ReportingServices.ReportRendering;

namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    partial class ServerGroupPropertiesView
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
            if (disposing)
            {
                // unhook event handlers to allow view to dispose
                ApplicationModel.Default.ActiveInstances.Changed -= ActiveInstances_Changed;
            }

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
            Color backColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridBackColor) : System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203))))); ;
            Color foreColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridForeColor) : Color.Black;
            Color activeBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridActiveBackColor) : Color.White;
            Color hoverBackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.UltraGridHoverBackColor) : Color.White;
            Color borderColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.IndexCardPanelBorderColor) : Color.Gray;
            this.components = new System.ComponentModel.Container();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("MonitoredSQLServerConfigurationDisplayWrapper", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn41 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLServerID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn42 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("HasChanges", -1, 972251);
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn43 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PreferredClusterNode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn44 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ScheduledCollectionInterval");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn45 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReorganizationMinimumTableSizeKB");
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn46 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReplicationMonitoringDisabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn47 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ExtendedHistoryCollectionDisabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn48 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ServerPingInterval");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn49 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseStatisticsInterval");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn50 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MostRecentSQLVersion");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn51 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MostRecentSQLEdition");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn52 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("JobAlerts", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn53 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ErrorlogAlerts", -1, 195913626);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn54 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AuthenticationMode", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn55 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActiveClusterNode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn56 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InstanceName");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn57 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MaintenanceMode");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn58 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WmiConfiguration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn59 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryMonitorConfiguration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn60 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryMonitorEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn61 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActivityMonitorEnabled");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn62 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryMonitorFilterTypes");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn63 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("QueryMonitorAdvancedFilters");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn64 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DeadlockMonitoring");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn65 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LastGrowthStatisticsRunTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn66 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("LastReorgStatisticsRunTime");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn67 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("GrowthStatisticsDisplay");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn68 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ReorgStatisticsDisplay");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn69 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableStatisticsExcludedDatabasesCount");
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn70 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("TableStatisticsExcludedDatabasesString");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn71 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomCountersCount");
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn72 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("CustomCountersList");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn73 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DiskCollectionSettings");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn74 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("InputBufferLimiter");
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn75 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActiveWaitsConfiguration");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn76 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActiveWaitsFilters");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn77 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("VCenter");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn78 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("VMHost");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn79 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BaselineTimePeriod");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn80 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BaselineDateRange");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumnAnalysisTime = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AnalysisScheduledTimePeriod");
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(195913626);
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(972251);
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("instanceContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("refreshInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("deleteInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("showInstancePropertiesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("refreshInstanceButton");
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("openInstanceButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("deleteInstanceButton");
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("showInstancePropertiesButton");
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            this.ServerGroupPropertiesView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.splitContainer1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.propertiesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.columnHelp = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.serverValidRange = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.columnLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.undoButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.saveButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.lblNoSqlservers = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.monitoredSqlServerConfigurationBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.ServerGroupPropertiesView_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.propertiesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.monitoredSqlServerConfigurationBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.SuspendLayout();
            // 
            // ServerGroupPropertiesView_Fill_Panel
            // 
            this.ServerGroupPropertiesView_Fill_Panel.Controls.Add(this.splitContainer1);
            this.ServerGroupPropertiesView_Fill_Panel.Controls.Add(this.lblNoSqlservers);
            this.ServerGroupPropertiesView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerGroupPropertiesView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerGroupPropertiesView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ServerGroupPropertiesView_Fill_Panel.Name = "ServerGroupPropertiesView_Fill_Panel";
            this.ServerGroupPropertiesView_Fill_Panel.Size = new System.Drawing.Size(453, 433);
            this.ServerGroupPropertiesView_Fill_Panel.TabIndex = 0;
            this.ServerGroupPropertiesView_Fill_Panel.BackColor = backColor;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.BackColor = backColor;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.propertiesGrid);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = backColor; //System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer1.Panel2.Controls.Add(this.columnHelp);
            this.splitContainer1.Panel2.Controls.Add(this.serverValidRange);
            this.splitContainer1.Panel2.Controls.Add(this.columnLabel);
            this.splitContainer1.Panel2.Controls.Add(this.undoButton);
            this.splitContainer1.Panel2.Controls.Add(this.saveButton);
            this.splitContainer1.Size = new System.Drawing.Size(453, 433);
            this.splitContainer1.SplitterDistance = 300;
            this.splitContainer1.TabIndex = 1;
            // 
            // propertiesGrid
            // 
            this.propertiesGrid.DataSource = this.monitoredSqlServerConfigurationBindingSource;
            appearance3.BackColor = backColor; //System.Drawing.SystemColors.Window;
            appearance3.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.propertiesGrid.DisplayLayout.Appearance = appearance3;
            ultraGridColumn41.Header.Caption = "SQL Server ID";
            ultraGridColumn41.Header.VisiblePosition = 2;
            ultraGridColumn41.Hidden = true;
            appearance35.ImageHAlign = Infragistics.Win.HAlign.Center;
            appearance35.ImageVAlign = Infragistics.Win.VAlign.Middle;
            ultraGridColumn42.CellAppearance = appearance35;
            ultraGridColumn42.Header.Caption = "Changed?";
            ultraGridColumn42.Header.Fixed = true;
            ultraGridColumn42.Header.VisiblePosition = 0;
            ultraGridColumn43.Header.Caption = "Preferred Cluster Node";
            ultraGridColumn43.Header.VisiblePosition = 11;
            ultraGridColumn43.Nullable = Nullable.EmptyString;
            ultraGridColumn43.RowLayoutColumnInfo.OriginX = 47;
            ultraGridColumn43.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn43.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(195, 0);
            ultraGridColumn43.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn43.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn44.Header.Caption = "Alert Refresh";
            ultraGridColumn44.Header.VisiblePosition = 7;
            ultraGridColumn44.RowLayoutColumnInfo.OriginX = 6;
            ultraGridColumn44.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn44.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn44.RowLayoutColumnInfo.SpanY = 2;
            appearance36.TextHAlignAsString = "Right";
            ultraGridColumn45.CellAppearance = appearance36;
            ultraGridColumn45.Header.Caption = "Fragmentation Min. Table Size (KB)";
            ultraGridColumn45.Header.VisiblePosition = 21;
            ultraGridColumn46.Header.Caption = "Replication Monitoring Disabled";
            ultraGridColumn46.Header.VisiblePosition = 36;
            ultraGridColumn46.RowLayoutColumnInfo.OriginX = 43;
            ultraGridColumn46.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn46.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn46.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn47.Header.Caption = "Session History Browser Collection Disabled";
            ultraGridColumn47.Header.VisiblePosition = 37;
            ultraGridColumn47.RowLayoutColumnInfo.OriginX = 41;
            ultraGridColumn47.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn47.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn47.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn48.Header.Caption = "Check Server Accessibility";
            ultraGridColumn48.Header.VisiblePosition = 6;
            ultraGridColumn48.RowLayoutColumnInfo.OriginX = 4;
            ultraGridColumn48.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn48.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn48.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn49.Header.Caption = "Database Statistics Refresh";
            ultraGridColumn49.Header.VisiblePosition = 8;
            ultraGridColumn50.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn50.Header.Caption = "SQL Version";
            ultraGridColumn50.Header.VisiblePosition = 3;
            ultraGridColumn51.Header.Caption = "SQL Edition";
            ultraGridColumn51.Header.VisiblePosition = 4;
            ultraGridColumn52.Header.Caption = "Job Alerting Enabled";
            ultraGridColumn52.Header.VisiblePosition = 38;
            ultraGridColumn53.Header.Caption = "Error Log Alerting Enabled";
            ultraGridColumn53.Header.VisiblePosition = 39;
            ultraGridColumn54.Header.Caption = "Authentication Mode";
            ultraGridColumn54.Header.VisiblePosition = 5;
            ultraGridColumn54.RowLayoutColumnInfo.OriginX = 2;
            ultraGridColumn54.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn54.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn54.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn55.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn55.Header.Caption = "Active Cluster Node";
            ultraGridColumn55.Header.VisiblePosition = 10;
            ultraGridColumn55.RowLayoutColumnInfo.OriginX = 45;
            ultraGridColumn55.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn55.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(189, 0);
            ultraGridColumn55.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn55.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn56.CellActivation = Infragistics.Win.UltraWinGrid.Activation.NoEdit;
            ultraGridColumn56.Header.Caption = "Server Name";
            ultraGridColumn56.Header.Fixed = true;
            ultraGridColumn56.Header.VisiblePosition = 1;
            ultraGridColumn56.RowLayoutColumnInfo.OriginX = 0;
            ultraGridColumn56.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn56.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn56.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn57.CellActivation = Infragistics.Win.UltraWinGrid.Activation.ActivateOnly;
            ultraGridColumn57.Header.Caption = "Maintenance Mode";
            ultraGridColumn57.Header.VisiblePosition = 9;
            ultraGridColumn57.RowLayoutColumnInfo.OriginX = 8;
            ultraGridColumn57.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn57.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(204, 0);
            ultraGridColumn57.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn57.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn58.Header.Caption = "OS Metrics Collection";
            ultraGridColumn58.Header.VisiblePosition = 35;
            ultraGridColumn58.RowLayoutColumnInfo.OriginX = 39;
            ultraGridColumn58.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn58.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn58.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn59.Header.Caption = "Query Monitor Configuration";
            ultraGridColumn59.Header.VisiblePosition = 20;
            ultraGridColumn59.Hidden = true;
            ultraGridColumn59.RowLayoutColumnInfo.OriginX = 10;
            ultraGridColumn59.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn59.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(455, 0);
            ultraGridColumn59.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn59.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn60.Header.Caption = "Query Monitor Enabled";
            ultraGridColumn60.Header.VisiblePosition = 15;
            ultraGridColumn60.RowLayoutColumnInfo.OriginX = 16;
            ultraGridColumn60.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn60.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn60.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn61.Header.Caption = "Activity Monitor Enabled";
            ultraGridColumn61.Header.VisiblePosition = 16;
            ultraGridColumn62.Header.Caption = "Query Monitor Thresholds";
            ultraGridColumn62.Header.VisiblePosition = 18;
            ultraGridColumn62.RowLayoutColumnInfo.OriginX = 20;
            ultraGridColumn62.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn62.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(297, 0);
            ultraGridColumn62.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn62.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn63.Header.Caption = "Query Monitor Advanced Filters";
            ultraGridColumn63.Header.VisiblePosition = 19;
            ultraGridColumn63.RowLayoutColumnInfo.OriginX = 24;
            ultraGridColumn63.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn63.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(488, 0);
            ultraGridColumn63.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn63.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn64.Header.Caption = "Deadlock Monitoring";
            ultraGridColumn64.Header.VisiblePosition = 17;
            ultraGridColumn65.Format = "g";
            ultraGridColumn65.Header.Caption = "Last Table Growth Collection";
            ultraGridColumn65.Header.VisiblePosition = 27;
            ultraGridColumn65.RowLayoutColumnInfo.OriginX = 12;
            ultraGridColumn65.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn65.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn65.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn66.Format = "g";
            ultraGridColumn66.Header.Caption = "Last Fragmentation Collection";
            ultraGridColumn66.Header.VisiblePosition = 23;
            ultraGridColumn66.RowLayoutColumnInfo.OriginX = 18;
            ultraGridColumn66.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn66.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn66.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn67.Header.Caption = "Table Growth Collection";
            ultraGridColumn67.Header.VisiblePosition = 24;
            ultraGridColumn67.RowLayoutColumnInfo.OriginX = 14;
            ultraGridColumn67.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn67.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn67.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn68.Header.Caption = "Table Fragmentation Collection";
            ultraGridColumn68.Header.VisiblePosition = 22;
            ultraGridColumn68.RowLayoutColumnInfo.OriginX = 22;
            ultraGridColumn68.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn68.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn68.RowLayoutColumnInfo.SpanY = 2;
            appearance37.TextHAlignAsString = "Right";
            ultraGridColumn69.CellAppearance = appearance37;
            ultraGridColumn69.Header.Caption = "Table Statistics Excluded Databases (Count)";
            ultraGridColumn69.Header.VisiblePosition = 26;
            ultraGridColumn69.Hidden = true;
            ultraGridColumn69.RowLayoutColumnInfo.OriginX = 28;
            ultraGridColumn69.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn69.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn69.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn70.Header.Caption = "Table Statistics Excluded Databases (List)";
            ultraGridColumn70.Header.VisiblePosition = 25;
            ultraGridColumn70.RowLayoutColumnInfo.OriginX = 30;
            ultraGridColumn70.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn70.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(511, 0);
            ultraGridColumn70.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn70.RowLayoutColumnInfo.SpanY = 2;
            appearance38.TextHAlignAsString = "Right";
            ultraGridColumn71.CellAppearance = appearance38;
            ultraGridColumn71.Header.Caption = "Linked Custom Counters (Count)";
            ultraGridColumn71.Header.VisiblePosition = 29;
            ultraGridColumn71.Hidden = true;
            ultraGridColumn71.RowLayoutColumnInfo.OriginX = 49;
            ultraGridColumn71.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn71.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn71.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn72.Header.Caption = "Linked Custom Counters (List)";
            ultraGridColumn72.Header.VisiblePosition = 28;
            ultraGridColumn72.RowLayoutColumnInfo.OriginX = 34;
            ultraGridColumn72.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn72.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(406, 0);
            ultraGridColumn72.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn72.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn73.Header.Caption = "Disk Collection Settings";
            ultraGridColumn73.Header.VisiblePosition = 14;
            ultraGridColumn73.RowLayoutColumnInfo.OriginX = 36;
            ultraGridColumn73.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn73.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(246, 0);
            ultraGridColumn73.RowLayoutColumnInfo.SpanX = 3;
            ultraGridColumn73.RowLayoutColumnInfo.SpanY = 2;
            appearance39.TextHAlignAsString = "Right";
            ultraGridColumn74.CellAppearance = appearance39;
            ultraGridColumn74.Header.Caption = "Input Buffer Limiter";
            ultraGridColumn74.Header.VisiblePosition = 32;
            ultraGridColumn75.Header.Caption = "Query Waits";
            ultraGridColumn75.Header.VisiblePosition = 30;
            ultraGridColumn75.RowLayoutColumnInfo.OriginX = 32;
            ultraGridColumn75.RowLayoutColumnInfo.OriginY = 0;
            ultraGridColumn75.RowLayoutColumnInfo.PreferredCellSize = new System.Drawing.Size(253, 0);
            ultraGridColumn75.RowLayoutColumnInfo.SpanX = 2;
            ultraGridColumn75.RowLayoutColumnInfo.SpanY = 2;
            ultraGridColumn76.Header.Caption = "Query Waits Filters";
            ultraGridColumn76.Header.VisiblePosition = 31;
            ultraGridColumn77.Header.Caption = "Virtualization Host Name";
            ultraGridColumn77.Header.VisiblePosition = 13;
            ultraGridColumn78.Header.Caption = "VM Name";
            ultraGridColumn78.Header.VisiblePosition = 12;
            ultraGridColumn79.Header.Caption = "Baseline Time Period";
            ultraGridColumn79.Header.VisiblePosition = 33;
            ultraGridColumn80.Header.Caption = "Baseline Date Range";
            ultraGridColumn80.Header.VisiblePosition = 34;
            ultraGridColumnAnalysisTime.Header.Caption = "Analysis Time Period";
            //ultraGridColumn80.Header.VisiblePosition = 34;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn41,
            ultraGridColumn42,
            ultraGridColumn43,
            ultraGridColumn44,
            ultraGridColumn45,
            ultraGridColumn46,
            ultraGridColumn47,
            ultraGridColumn48,
            ultraGridColumn49,
            ultraGridColumn50,
            ultraGridColumn51,
            ultraGridColumn52,
            ultraGridColumn53,
            ultraGridColumn54,
            ultraGridColumn55,
            ultraGridColumn56,
            ultraGridColumn57,
            ultraGridColumn58,
            ultraGridColumn59,
            ultraGridColumn60,
            ultraGridColumn61,
            ultraGridColumn62,
            ultraGridColumn63,
            ultraGridColumn64,
            ultraGridColumn65,
            ultraGridColumn66,
            ultraGridColumn67,
            ultraGridColumn68,
            ultraGridColumn69,
            ultraGridColumn70,
            ultraGridColumn71,
            ultraGridColumn72,
            ultraGridColumn73,
            ultraGridColumn74,
            ultraGridColumn75,
            ultraGridColumn76,
            ultraGridColumn77,
            ultraGridColumn78,
            ultraGridColumn79,
            ultraGridColumn80,
            ultraGridColumnAnalysisTime});
            appearance40.BackColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            ultraGridBand1.Override.CellAppearance = appearance40;
            appearance41.BackColorDisabled = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            ultraGridBand1.Override.RowAppearance = appearance41;
            this.propertiesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.propertiesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.propertiesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance8.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.propertiesGrid.DisplayLayout.GroupByBox.Appearance = appearance8;
            appearance9.ForeColor = System.Drawing.SystemColors.GrayText;
            this.propertiesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance9;
            this.propertiesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.propertiesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance10.BackColor2 = System.Drawing.SystemColors.Control;
            appearance10.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance10.ForeColor = System.Drawing.SystemColors.GrayText;
            this.propertiesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance10;
            this.propertiesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.propertiesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.propertiesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.propertiesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.propertiesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.True;
            this.propertiesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.propertiesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance11.BackColor = backColor; //System.Drawing.SystemColors.Window;
            this.propertiesGrid.DisplayLayout.Override.CardAreaAppearance = appearance11;
            appearance12.BorderColor = System.Drawing.Color.Silver;
            appearance12.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.propertiesGrid.DisplayLayout.Override.CellAppearance = appearance12;
            this.propertiesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.Edit;
            this.propertiesGrid.DisplayLayout.Override.CellMultiLine = Infragistics.Win.DefaultableBoolean.False;
            this.propertiesGrid.DisplayLayout.Override.CellPadding = 0;
            this.propertiesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.propertiesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            this.propertiesGrid.Scale(1.01f);
            appearance22.BackColor = System.Drawing.SystemColors.Control;
            appearance22.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance22.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance22.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance22.BorderColor = System.Drawing.SystemColors.Window;
            this.propertiesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance22;
            this.propertiesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance23.TextHAlignAsString = "Left";
            this.propertiesGrid.DisplayLayout.Override.HeaderAppearance = appearance23;
            this.propertiesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.propertiesGrid.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            appearance24.BorderColor = System.Drawing.Color.Silver;
            this.propertiesGrid.DisplayLayout.Override.RowAppearance = appearance24;
            this.propertiesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.propertiesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.propertiesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.propertiesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.propertiesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance25.BackColor = System.Drawing.SystemColors.ControlLight;
            this.propertiesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance25;
            this.propertiesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.propertiesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.propertiesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "yesNoValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance26.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueList2.Appearance = appearance26;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "hasChangesValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            this.propertiesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2});
            this.propertiesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.propertiesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.propertiesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertiesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.propertiesGrid.Location = new System.Drawing.Point(0, 0);
            this.propertiesGrid.Name = "propertiesGrid";
            this.propertiesGrid.Size = new System.Drawing.Size(453, 300);
            this.propertiesGrid.TabIndex = 0;
            this.propertiesGrid.AfterCellUpdate += new Infragistics.Win.UltraWinGrid.CellEventHandler(this.propertiesGrid_AfterCellUpdate);
            this.propertiesGrid.InitializeLayout += new Infragistics.Win.UltraWinGrid.InitializeLayoutEventHandler(this.propertiesGrid_InitializeLayout);
            this.propertiesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.propertiesGrid_AfterSelectChange);
            this.propertiesGrid.BeforeCellUpdate += new Infragistics.Win.UltraWinGrid.BeforeCellUpdateEventHandler(this.propertiesGrid_BeforeCellUpdate);
            this.propertiesGrid.ClickCell += new Infragistics.Win.UltraWinGrid.ClickCellEventHandler(this.propertiesGrid_ClickCell);
            this.propertiesGrid.DoubleClickCell += new Infragistics.Win.UltraWinGrid.DoubleClickCellEventHandler(this.propertiesGrid_DoubleClickCell);
            this.propertiesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.propertiesGrid_MouseDown);
            // 
            // columnHelp
            // 
            this.columnHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.columnHelp.BackColor = backColor; //System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.columnHelp.ForeColor = foreColor;
            this.columnHelp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
           
            this.columnHelp.Location = new System.Drawing.Point(10, 31);
            //this.columnHelp.Multiline = true;
            this.columnHelp.Name = "columnHelp";
            //this.columnHelp.ReadOnly = true;
            this.columnHelp.Size = new System.Drawing.Size(315, 98);
            this.columnHelp.TabIndex = 2;
            this.columnHelp.Text = "Click on a cell above for detailed help on that configuration option.";
            // 
            // serverValidRange
            // 
            this.serverValidRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.serverValidRange.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.serverValidRange.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.serverValidRange.Location = new System.Drawing.Point(10, 31);
            //this.serverValidRange.Multiline = true;
            this.serverValidRange.Name = "serverValidRange";
            //this.serverValidRange.ReadOnly = true;
            this.serverValidRange.Size = new System.Drawing.Size(331, 22);
            this.serverValidRange.TabIndex = 5;
            // 
            // columnLabel
            // 
            this.columnLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.columnLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.columnLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.columnLabel.Location = new System.Drawing.Point(5, 8);
            //this.columnLabel.Multiline = true;
            this.columnLabel.Name = "columnLabel";
            //this.columnLabel.ReadOnly = true;
            this.columnLabel.Size = new System.Drawing.Size(339, 17);
            this.columnLabel.TabIndex = 4;
            this.columnLabel.Text = "Server Properties";
            // 
            // undoButton
            // 
            this.undoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.undoButton.Enabled = false;
            this.undoButton.Location = new System.Drawing.Point(356, 36);
            this.undoButton.Name = "undoButton";
            this.undoButton.Size = new System.Drawing.Size(94, 22);
            this.undoButton.TabIndex = 1;
            this.undoButton.Text = "Reset Changes";
            this.undoButton.UseVisualStyleBackColor = true;
            this.undoButton.Click += new System.EventHandler(this.undoButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(356, 3);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(94, 22);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "Save Changes ";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // lblNoSqlservers
            // 
            this.lblNoSqlservers.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblNoSqlservers.Location = new System.Drawing.Point(0, 0);
            this.lblNoSqlservers.Name = "lblNoSqlservers";
            this.lblNoSqlservers.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.lblNoSqlservers.Size = new System.Drawing.Size(453, 433);
            this.lblNoSqlservers.TabIndex = 2;
            this.lblNoSqlservers.Text = "< status label >";
            this.lblNoSqlservers.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Server Group";
            this.ultraGridPrintDocument.Grid = this.propertiesGrid;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // monitoredSqlServerConfigurationBindingSource
            // 
            this.monitoredSqlServerConfigurationBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Objects.MonitoredSQLServerConfigurationDisplayWrapper);
            // 
            // _ServerGroupPropertiesView_Toolbars_Dock_Area_Left
            // 
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.Name = "_ServerGroupPropertiesView_Toolbars_Dock_Area_Left";
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 433);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left.ToolbarsManager = this.toolbarsManager;
            // 
            // toolbarsManager
            // 
            this.toolbarsManager.DesignerFlags = 1;
            this.toolbarsManager.DockWithinContainer = this;
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
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            popupMenuTool2.SharedPropsInternal.Caption = "instanceContextMenu";
            buttonTool11.InstanceProps.IsFirstInGroup = true;
            buttonTool12.InstanceProps.IsFirstInGroup = true;
            buttonTool13.InstanceProps.IsFirstInGroup = true;
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            buttonTool17.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool10,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16,
            buttonTool17});
            appearance17.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            buttonTool18.SharedPropsInternal.AppearancesSmall.Appearance = appearance17;
            buttonTool18.SharedPropsInternal.Caption = "Refresh Alerts";
            buttonTool19.SharedPropsInternal.Caption = "Open";
            appearance18.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Delete;
            buttonTool20.SharedPropsInternal.AppearancesSmall.Appearance = appearance18;
            buttonTool20.SharedPropsInternal.Caption = "Delete";
            buttonTool20.SharedPropsInternal.Shortcut = System.Windows.Forms.Shortcut.Del;
            appearance19.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Properties;
            buttonTool21.SharedPropsInternal.AppearancesSmall.Appearance = appearance19;
            buttonTool21.SharedPropsInternal.Caption = "Properties...";
            buttonTool22.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool25.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26});
            appearance20.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool27.SharedPropsInternal.AppearancesSmall.Appearance = appearance20;
            buttonTool27.SharedPropsInternal.Caption = "Export To Excel";
            appearance21.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool28.SharedPropsInternal.AppearancesSmall.Appearance = appearance21;
            buttonTool28.SharedPropsInternal.Caption = "Print";
            buttonTool29.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool30.SharedPropsInternal.Caption = "Expand All Groups";
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            popupMenuTool2,
            buttonTool18,
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22,
            stateButtonTool2,
            popupMenuTool3,
            buttonTool27,
            buttonTool28,
            buttonTool29,
            buttonTool30});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // _ServerGroupPropertiesView_Toolbars_Dock_Area_Right
            // 
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(453, 0);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.Name = "_ServerGroupPropertiesView_Toolbars_Dock_Area_Right";
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 433);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServerGroupPropertiesView_Toolbars_Dock_Area_Top
            // 
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.Name = "_ServerGroupPropertiesView_Toolbars_Dock_Area_Top";
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(453, 0);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top.ToolbarsManager = this.toolbarsManager;
            // 
            // _ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom
            // 
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.BackColor = backColor; //System.Drawing.SystemColors.Control;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 433);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.Name = "_ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom";
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(453, 0);
            this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.toolbarsManager;
            // 
            // ServerGroupPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ServerGroupPropertiesView_Fill_Panel);
            this.Controls.Add(this._ServerGroupPropertiesView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._ServerGroupPropertiesView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._ServerGroupPropertiesView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom);
            this.Name = "ServerGroupPropertiesView";
            this.Size = new System.Drawing.Size(453, 433);
            this.Load += new System.EventHandler(this.ServerGroupDetailsView_Load);
            this.ServerGroupPropertiesView_Fill_Panel.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.propertiesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.monitoredSqlServerConfigurationBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  ServerGroupPropertiesView_Fill_Panel;
        private Infragistics.Win.UltraWinGrid.UltraGrid propertiesGrid;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.BindingSource monitoredSqlServerConfigurationBindingSource;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton saveButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton undoButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel columnHelp;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel columnLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serverValidRange;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerGroupPropertiesView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerGroupPropertiesView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerGroupPropertiesView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _ServerGroupPropertiesView_Toolbars_Dock_Area_Bottom;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel lblNoSqlservers;
    }
}
