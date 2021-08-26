using System;
using System.Drawing;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    partial class DatabasesConfigurationView
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
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DatabaseName", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Collation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Recovery", -1, 98659907);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Compatibility", -1, 774349954);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAnsiNullDefault", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAnsiNullsEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAnsiPaddingEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAnsiWarningsEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsArithmeticAbortEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAutoClose", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAutoCreateStatistics", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAutoShrink", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAutoUpdateStatistics", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsCloseCursorsOnCommitEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsFulltextEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsInStandBy", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsLocalCursorsDefault", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsMergePublished", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsNullConcat", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsNumericRoundAbortEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsParameterizationForced", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsQuotedIdentifiersEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsPublished", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsRecursiveTriggersEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsSubscribed", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsSyncWithBackup", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsTornPageDetectionEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Status");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Updateability");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("UserAccess");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Version");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsDbChainingOn", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsDateCorrelationOn", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsVardecimalEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("PageVerifyOption");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsAutoUpdateStatsAsyncOn", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsBrokerEnabled", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("IsTrustworthy", -1, 575079454);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn39 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SnapshotIsolationState");
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(575079454);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(98659907);
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(774349954);
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.DatabasesConfigurationView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.refreshDatabasesButton = new Infragistics.Win.Misc.UltraButton();
            this.databasesFilterComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.configurationGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.configurationGridStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.DatabasesConfigurationView_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databasesFilterComboBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).BeginInit();
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
            appearance11.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance11;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance12.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance12;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance13;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance14;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance15;
            buttonTool11.SharedPropsInternal.Caption = "Print";
            appearance16.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance16;
            buttonTool12.SharedPropsInternal.Caption = "Export to Excel";
            popupMenuTool2.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool15.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool13,
            buttonTool14,
            buttonTool15,
            buttonTool16});
            buttonTool17.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool18.SharedPropsInternal.Caption = "Expand All Groups";
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
            buttonTool17,
            buttonTool18});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // DatabasesConfigurationView_Fill_Panel
            // 
            this.DatabasesConfigurationView_Fill_Panel.Controls.Add(this.refreshDatabasesButton);
            this.DatabasesConfigurationView_Fill_Panel.Controls.Add(this.databasesFilterComboBox);
            this.DatabasesConfigurationView_Fill_Panel.Controls.Add(this.panel1);
            this.DatabasesConfigurationView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DatabasesConfigurationView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabasesConfigurationView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.DatabasesConfigurationView_Fill_Panel.Name = "DatabasesConfigurationView_Fill_Panel";
            this.DatabasesConfigurationView_Fill_Panel.Size = new System.Drawing.Size(689, 517);
            this.DatabasesConfigurationView_Fill_Panel.TabIndex = 8;
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
            if (!refreshDatabasesButton.Enabled)
                appearance1.Image = Helpers.ImageUtils.ChangeOpacity(global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh, 0.50F);
            else
                appearance1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ToolbarRefresh;
            appearance1.ImageHAlign = Infragistics.Win.HAlign.Center;
            this.refreshDatabasesButton.Appearance = appearance1;
            this.refreshDatabasesButton.Location = new System.Drawing.Point(663, 3);
            this.refreshDatabasesButton.Name = "refreshDatabasesButton";
            this.refreshDatabasesButton.ShowFocusRect = false;
            this.refreshDatabasesButton.ShowOutline = false;
            this.refreshDatabasesButton.Size = new System.Drawing.Size(23, 23);
            this.refreshDatabasesButton.TabIndex = 16;
            this.refreshDatabasesButton.Click += new System.EventHandler(this.refreshDatabasesButton_Click);
            this.refreshDatabasesButton.MouseEnterElement += new UIElementEventHandler(mouseEnter_refreshDatabasesButton);
            this.refreshDatabasesButton.MouseLeaveElement += new UIElementEventHandler(mouseLeave_refreshDatabasesButton);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
            // 
            // databasesFilterComboBox
            // 
            this.databasesFilterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.databasesFilterComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.databasesFilterComboBox.Location = new System.Drawing.Point(3, 4);
            this.databasesFilterComboBox.Name = "databasesFilterComboBox";
            this.databasesFilterComboBox.Size = new System.Drawing.Size(657, 21);
            this.databasesFilterComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.databasesFilterComboBox.TabIndex = 11;
            this.databasesFilterComboBox.SelectionChanged += new System.EventHandler(this.databasesFilterComboBox_SelectionChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.panel1.Controls.Add(this.configurationGrid);
            this.panel1.Controls.Add(this.configurationGridStatusLabel);
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.panel1.Size = new System.Drawing.Size(689, 489);
            this.panel1.TabIndex = 6;
            // 
            // configurationGrid
            // 
            appearance17.BackColor = System.Drawing.SystemColors.Window;
            appearance17.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.configurationGrid.DisplayLayout.Appearance = appearance17;
            this.configurationGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.Caption = "Database Name";
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.Caption = "Recovery model";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.Caption = "Compatibility level";
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn5.Header.Caption = "ANSI NULL Default";
            ultraGridColumn5.Header.VisiblePosition = 4;
            ultraGridColumn6.Header.Caption = "ANSI NULLS Enabled";
            ultraGridColumn6.Header.VisiblePosition = 5;
            ultraGridColumn7.Header.Caption = "ANSI Padding Enabled";
            ultraGridColumn7.Header.VisiblePosition = 6;
            ultraGridColumn8.Header.Caption = "ANSI Warnings Enabled";
            ultraGridColumn8.Header.VisiblePosition = 7;
            ultraGridColumn9.Header.Caption = "Arithmetic Abort Enabled";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn10.Header.Caption = "Auto Close";
            ultraGridColumn10.Header.VisiblePosition = 9;
            ultraGridColumn11.Header.Caption = "Auto Create Statistics";
            ultraGridColumn11.Header.VisiblePosition = 10;
            ultraGridColumn12.Header.Caption = "Auto Shrink";
            ultraGridColumn12.Header.VisiblePosition = 11;
            ultraGridColumn13.Header.Caption = "Auto Update Statistics";
            ultraGridColumn13.Header.VisiblePosition = 12;
            ultraGridColumn14.Header.Caption = "Close Cursors On Commit Enabled";
            ultraGridColumn14.Header.VisiblePosition = 15;
            ultraGridColumn15.Header.Caption = "Fulltext Enabled";
            ultraGridColumn15.Header.VisiblePosition = 18;
            ultraGridColumn16.Header.Caption = "In Standby";
            ultraGridColumn16.Header.VisiblePosition = 19;
            ultraGridColumn17.Header.Caption = "Local Cursors Default";
            ultraGridColumn17.Header.VisiblePosition = 20;
            ultraGridColumn18.Header.Caption = "Merge Published";
            ultraGridColumn18.Header.VisiblePosition = 21;
            ultraGridColumn19.Header.Caption = "NULL Concat";
            ultraGridColumn19.Header.VisiblePosition = 22;
            ultraGridColumn20.Header.Caption = "Numeric Round Abort Enabled";
            ultraGridColumn20.Header.VisiblePosition = 23;
            ultraGridColumn21.Header.Caption = "Parameterization";
            ultraGridColumn21.Header.VisiblePosition = 25;
            ultraGridColumn22.Header.Caption = "Quoted Identifiers Enabled";
            ultraGridColumn22.Header.VisiblePosition = 27;
            ultraGridColumn23.Header.Caption = "Published";
            ultraGridColumn23.Header.VisiblePosition = 26;
            ultraGridColumn24.Header.Caption = "Recursive Triggers Enabled";
            ultraGridColumn24.Header.VisiblePosition = 28;
            ultraGridColumn25.Header.Caption = "Subscribed";
            ultraGridColumn25.Header.VisiblePosition = 30;
            ultraGridColumn26.Header.Caption = "Sync With Backup";
            ultraGridColumn26.Header.VisiblePosition = 31;
            ultraGridColumn27.Header.Caption = "Torn Page Detection Enabled";
            ultraGridColumn27.Header.VisiblePosition = 32;
            ultraGridColumn28.Header.VisiblePosition = 34;
            ultraGridColumn29.Header.Caption = "Read-Only";
            ultraGridColumn29.Header.VisiblePosition = 35;
            ultraGridColumn30.Header.Caption = "Restrict Access";
            ultraGridColumn30.Header.VisiblePosition = 36;
            ultraGridColumn31.Header.VisiblePosition = 38;
            ultraGridColumn32.Header.Caption = "Cross-database Ownership Chaining Enabled";
            ultraGridColumn32.Header.VisiblePosition = 16;
            ultraGridColumn33.Header.Caption = "Date Correlation Optimization Enabled";
            ultraGridColumn33.Header.VisiblePosition = 17;
            ultraGridColumn34.Header.Caption = "VarDecimal Storage Format Enabled";
            ultraGridColumn34.Header.VisiblePosition = 37;
            ultraGridColumn35.Header.Caption = "Page Verify";
            ultraGridColumn35.Header.VisiblePosition = 24;
            ultraGridColumn36.Header.Caption = "Auto Update Statistics Asynchronously";
            ultraGridColumn36.Header.VisiblePosition = 13;
            ultraGridColumn37.Header.Caption = "Broker Enabled";
            ultraGridColumn37.Header.VisiblePosition = 14;
            ultraGridColumn38.Header.Caption = "Trustworthy";
            ultraGridColumn38.Header.VisiblePosition = 33;
            ultraGridColumn39.Header.Caption = "Snapshot Isolation State";
            ultraGridColumn39.Header.VisiblePosition = 29;
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
            ultraGridColumn12,
            ultraGridColumn13,
            ultraGridColumn14,
            ultraGridColumn15,
            ultraGridColumn16,
            ultraGridColumn17,
            ultraGridColumn18,
            ultraGridColumn19,
            ultraGridColumn20,
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23,
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32,
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36,
            ultraGridColumn37,
            ultraGridColumn38,
            ultraGridColumn39});
            this.configurationGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.configurationGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.configurationGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.configurationGrid.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.configurationGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.configurationGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.configurationGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.configurationGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.configurationGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.configurationGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.configurationGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.configurationGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.configurationGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.configurationGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.configurationGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.configurationGrid.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.configurationGrid.DisplayLayout.Override.CellAppearance = appearance6;
            this.configurationGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.configurationGrid.DisplayLayout.Override.CellPadding = 0;
            this.configurationGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.configurationGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.configurationGrid.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.configurationGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Left";
            this.configurationGrid.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.configurationGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.configurationGrid.DisplayLayout.Override.RowAppearance = appearance9;
            this.configurationGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.configurationGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.configurationGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.configurationGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.configurationGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.configurationGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.configurationGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.configurationGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.configurationGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "ConfigurationOptionState";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = true;
            valueListItem1.DisplayText = "True";
            valueListItem2.DataValue = false;
            valueListItem2.DisplayText = "False";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            valueList2.Key = "recoveryModelValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.Key = "compatibilityLevelValueList";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.SortStyle = Infragistics.Win.ValueListSortStyle.AscendingByValue;
            this.configurationGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3});
            this.configurationGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.configurationGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.configurationGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.configurationGrid.Location = new System.Drawing.Point(0, 1);
            this.configurationGrid.Name = "configurationGrid";
            this.configurationGrid.Size = new System.Drawing.Size(689, 488);
            this.configurationGrid.TabIndex = 4;
            this.configurationGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.configurationsGrid_MouseDown);
            // 
            // configurationGridStatusLabel
            // 
            this.configurationGridStatusLabel.BackColor = System.Drawing.Color.White;
            this.configurationGridStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.configurationGridStatusLabel.Location = new System.Drawing.Point(0, 1);
            this.configurationGridStatusLabel.Name = "configurationGridStatusLabel";
            this.configurationGridStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.configurationGridStatusLabel.Size = new System.Drawing.Size(689, 488);
            this.configurationGridStatusLabel.TabIndex = 5;
            this.configurationGridStatusLabel.Text = "< Status Message >";
            this.configurationGridStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Databases Configurations";
            this.ultraGridPrintDocument.Grid = this.configurationGrid;
            // 
            // historicalSnapshotStatusLinkLabel
            // 
            this.historicalSnapshotStatusLinkLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historicalSnapshotStatusLinkLabel.Location = new System.Drawing.Point(0, 0);
            this.historicalSnapshotStatusLinkLabel.Name = "historicalSnapshotStatusLinkLabel";
            this.historicalSnapshotStatusLinkLabel.Size = new System.Drawing.Size(689, 517);
            this.historicalSnapshotStatusLinkLabel.TabIndex = 32;
            this.historicalSnapshotStatusLinkLabel.TabStop = true;
            this.historicalSnapshotStatusLinkLabel.Text = "This view does not support historical mode. Click here to switch to real-time mod" +
    "e.";
            this.historicalSnapshotStatusLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.historicalSnapshotStatusLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.historicalSnapshotStatusLinkLabel_LinkClicked);
            // 
            // DatabasesConfigurationView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.DatabasesConfigurationView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "DatabasesConfigurationView";
            this.Size = new System.Drawing.Size(689, 517);
            this.Load += new System.EventHandler(this.DatabasesConfigurationView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.DatabasesConfigurationView_Fill_Panel.ResumeLayout(false);
            this.DatabasesConfigurationView_Fill_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databasesFilterComboBox)).EndInit();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.configurationGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  DatabasesConfigurationView_Fill_Panel;
        private Infragistics.Win.UltraWinGrid.UltraGrid configurationGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel configurationGridStatusLabel;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor databasesFilterComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Infragistics.Win.Misc.UltraButton refreshDatabasesButton;
        private Infragistics.Win.Appearance appearance1;
    }
}
