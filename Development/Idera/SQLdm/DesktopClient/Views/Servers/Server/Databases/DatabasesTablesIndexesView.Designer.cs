using System;
using System.Drawing;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Databases
{
    partial class DatabasesTablesIndexesView
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
            ChartFX.WinForms.Adornments.GradientBackground gradientBackground1 = new ChartFX.WinForms.Adornments.GradientBackground();
            Infragistics.Win.Appearance appearance20 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn21 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Object", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn22 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type", -1, 622182797);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn23 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Action", -1, 622542329);
            Infragistics.Win.Appearance appearance21 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance22 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance23 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance24 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Controls.CustomControls.CustomValueList(622182797);
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem12 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem13 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem14 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem15 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem16 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem17 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem18 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem19 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem20 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem21 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem22 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem23 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem24 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList2 = new Controls.CustomControls.CustomValueList(622542329);
            Infragistics.Win.ValueListItem valueListItem25 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem26 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem27 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem28 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem29 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem30 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem31 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand2 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn24 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Index", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn25 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Clustered", -1, 632140422);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn26 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Unique", -1, 632140422);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn27 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Size (MB)");
            Infragistics.Win.Appearance appearance31 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn28 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rows");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn29 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Statistics Update");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn30 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rows Modified Since Last Update");
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn31 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Fill Factor");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn32 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Levels");
            Infragistics.Win.Appearance appearance35 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance36 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance37 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance38 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance39 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance40 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance41 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance42 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance43 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance44 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Controls.CustomControls.CustomValueList(632140422);
            Infragistics.Win.ValueListItem valueListItem32 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem33 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance45 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand3 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn33 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column(s)", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn34 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Average Length");
            Infragistics.Win.Appearance appearance46 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn35 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Average Row Hits");
            Infragistics.Win.Appearance appearance47 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn36 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Row Hits %");
            Infragistics.Win.Appearance appearance48 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance49 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance50 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance51 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance52 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance53 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance54 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance55 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance56 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance57 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance58 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand4 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn37 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Value", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn38 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rows With Same Key");
            Infragistics.Win.Appearance appearance59 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance60 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance61 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance62 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance63 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance64 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance65 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance66 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance67 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance68 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("columnContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Controls.CustomControls.CustomButtonTool("showColumnChooserButton");
            Infragistics.Win.Appearance appearance70 = new Infragistics.Win.Appearance();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DatabasesTablesIndexesView));
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Controls.CustomControls.CustomButtonTool("toggleGroupByBoxButton");
            Infragistics.Win.Appearance appearance71 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Controls.CustomControls.CustomButtonTool("sortAscendingButton");
            Infragistics.Win.Appearance appearance72 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Controls.CustomControls.CustomButtonTool("sortDescendingButton");
            Infragistics.Win.Appearance appearance73 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Controls.CustomControls.CustomButtonTool("removeThisColumnButton");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("groupByThisColumnButton", "");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool3 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.Appearance appearance74 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.Appearance appearance75 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.Appearance appearance76 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.Appearance appearance77 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.Appearance appearance78 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("chartContextMenu");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool4 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("toggleChartToolbarButton", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Controls.CustomControls.CustomButtonTool("printChartButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Controls.CustomControls.CustomButtonTool("exportChartImageButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Controls.CustomControls.CustomButtonTool("exportChartDataButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool19 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool20 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool21 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridDataContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Controls.CustomControls.CustomButtonTool("updateTableStatisticsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool24 = new Controls.CustomControls.CustomButtonTool("rebuildTableIndexesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool29 = new Controls.CustomControls.CustomButtonTool("updateTableStatisticsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool30 = new Controls.CustomControls.CustomButtonTool("rebuildTableIndexesButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool31 = new Controls.CustomControls.CustomButtonTool("collapseAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Controls.CustomControls.CustomButtonTool("expandAllGroupsButton");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool5 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("gridNoGroupsContextMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool33 = new Controls.CustomControls.CustomButtonTool("printGridButton");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool34 = new Controls.CustomControls.CustomButtonTool("exportGridButton");
            appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand5 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Object ID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Qualified Table Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Type", -1, 551181110, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Table");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Owner/Schema");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Data Size (MB)");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Index Size (MB)");
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Text Size (MB)");
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Database Size");
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn11 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Fragmentation (Quiet Time)");
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn12 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Last Fragmentation Collection");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn13 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("% Fragmentation (Current)");
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn14 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Fragmentation Status");
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn15 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Rows");
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn16 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("File Group");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn17 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Date Created");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn18 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Pinned", -1, 551429563);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn19 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("BCP Table Locking", -1, 551429563);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn20 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Full-Text Catalog");
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance16 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance17 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance18 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance19 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList4 = new Controls.CustomControls.CustomValueList(551181110);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueList valueList5 = new Controls.CustomControls.CustomValueList(551429563);
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab(true);
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            Infragistics.Win.UltraWinTabControl.UltraTab ultraTab4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTab();
            this.sizeTabPageControl = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.tableSizeChart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
            this.dependenciesTabPageControl = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.dependenciesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.dependenciesReferencedByViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.dependenciesReferencesViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.indexesTabPageControl = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.indexesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.indexStatisticsTabPageControl = new Infragistics.Win.UltraWinTabControl.UltraTabPageControl();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.indexStatisticsColumnsGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.indexStatisticsDataDistributionGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.indexStatisticsIndexSelectionComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.indexStatisticsDataDistributionViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.indexStatisticsColumnsViewRadioButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton();
            this.toolbarsManager = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this.DatabasesTablesIndexesView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.refreshDatabasesButton = new Infragistics.Win.Misc.UltraButton();
            this.splitContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomSplitContainer();
            this.tablesGrid = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.tablesGridStatusMessage = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableDetailsTabControl = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraTabControl();
            this.ultraTabSharedControlsPage1 = new Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage();
            this.tableDetailsStatusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableDetailsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.tableDetailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.hideTableDetailsButton = new System.Windows.Forms.ToolStripButton();
            this.availableDatabasesComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.refreshAvailableDatabasesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.refreshTableDetailsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.refreshIndexStatisticsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.rebuildIndexesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.updateStatisticsBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.historicalSnapshotStatusLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.sizeTabPageControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeChart)).BeginInit();
            this.dependenciesTabPageControl.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dependenciesGrid)).BeginInit();
            this.panel1.SuspendLayout();
            this.indexesTabPageControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexesGrid)).BeginInit();
            this.indexStatisticsTabPageControl.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsColumnsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsDataDistributionGrid)).BeginInit();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsIndexSelectionComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).BeginInit();
            this.DatabasesTablesIndexesView_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tablesGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetailsTabControl)).BeginInit();
            this.tableDetailsTabControl.SuspendLayout();
            this.tableDetailsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableDatabasesComboBox)).BeginInit();
            this.SuspendLayout();
            // 
            // sizeTabPageControl
            // 
            this.sizeTabPageControl.Controls.Add(this.tableSizeChart);
            this.sizeTabPageControl.Location = new System.Drawing.Point(1, 23);
            this.sizeTabPageControl.Name = "sizeTabPageControl";
            this.sizeTabPageControl.Size = new System.Drawing.Size(685, 210);
            // 
            // tableSizeChart
            // 
            this.tableSizeChart.AllSeries.BarShape = ChartFX.WinForms.BarShape.Cylinder;
            this.tableSizeChart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Bar;
            this.tableSizeChart.AllSeries.PointLabels.Visible = true;
            this.tableSizeChart.AxisX.Visible = false;
            this.tableSizeChart.AxisY.Title.Text = "KB";
            gradientBackground1.ColorFrom = System.Drawing.Color.FromArgb(((int)(((byte)(210)))), ((int)(((byte)(210)))), ((int)(((byte)(210)))));
            this.tableSizeChart.Background = gradientBackground1;
            this.tableSizeChart.Border = new ChartFX.WinForms.Adornments.SimpleBorder(ChartFX.WinForms.Adornments.SimpleBorderType.None, System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(125)))), ((int)(((byte)(138))))));
            this.tableSizeChart.ContextMenus = false;
            this.toolbarsManager.SetContextMenuUltra(this.tableSizeChart, "chartContextMenu");
            this.tableSizeChart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableSizeChart.LegendBox.PlotAreaOnly = false;
            this.tableSizeChart.Location = new System.Drawing.Point(0, 0);
            this.tableSizeChart.Name = "tableSizeChart";
            this.tableSizeChart.Palette = "Schemes.Classic";
            this.tableSizeChart.PlotAreaColor = System.Drawing.Color.White;
            this.tableSizeChart.Size = new System.Drawing.Size(685, 210);
            this.tableSizeChart.TabIndex = 0;
            this.tableSizeChart.View3D.Enabled = true;
            this.tableSizeChart.Resize += new System.EventHandler(this.tableSizeChart_Resize);
            // 
            // dependenciesTabPageControl
            // 
            this.dependenciesTabPageControl.Controls.Add(this.panel2);
            this.dependenciesTabPageControl.Controls.Add(this.panel1);
            this.dependenciesTabPageControl.Location = new System.Drawing.Point(-10000, -10000);
            this.dependenciesTabPageControl.Name = "dependenciesTabPageControl";
            this.dependenciesTabPageControl.Size = new System.Drawing.Size(685, 210);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.panel2.Controls.Add(this.dependenciesGrid);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 30);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.panel2.Size = new System.Drawing.Size(685, 180);
            this.panel2.TabIndex = 6;
            // 
            // dependenciesGrid
            // 
            appearance20.BackColor = System.Drawing.SystemColors.Window;
            appearance20.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.dependenciesGrid.DisplayLayout.Appearance = appearance20;
            this.dependenciesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn21.Header.VisiblePosition = 0;
            ultraGridColumn21.Width = 273;
            ultraGridColumn22.Header.VisiblePosition = 1;
            ultraGridColumn22.Width = 190;
            ultraGridColumn23.Header.VisiblePosition = 2;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn21,
            ultraGridColumn22,
            ultraGridColumn23});
            this.dependenciesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.dependenciesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.dependenciesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance21.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance21.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.dependenciesGrid.DisplayLayout.GroupByBox.Appearance = appearance21;
            appearance22.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dependenciesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance22;
            this.dependenciesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.dependenciesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance23.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance23.BackColor2 = System.Drawing.SystemColors.Control;
            appearance23.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance23.ForeColor = System.Drawing.SystemColors.GrayText;
            this.dependenciesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance23;
            this.dependenciesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.dependenciesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.dependenciesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.dependenciesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.dependenciesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.dependenciesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.dependenciesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance24.BackColor = System.Drawing.SystemColors.Window;
            this.dependenciesGrid.DisplayLayout.Override.CardAreaAppearance = appearance24;
            appearance25.BorderColor = System.Drawing.Color.Silver;
            appearance25.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.dependenciesGrid.DisplayLayout.Override.CellAppearance = appearance25;
            this.dependenciesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.dependenciesGrid.DisplayLayout.Override.CellPadding = 0;
            this.dependenciesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.dependenciesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance26.BackColor = System.Drawing.SystemColors.Control;
            appearance26.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance26.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance26.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance26.BorderColor = System.Drawing.SystemColors.Window;
            this.dependenciesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance26;
            this.dependenciesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance27.TextHAlignAsString = "Left";
            this.dependenciesGrid.DisplayLayout.Override.HeaderAppearance = appearance27;
            this.dependenciesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance28.BackColor = System.Drawing.SystemColors.Window;
            appearance28.BorderColor = System.Drawing.Color.Silver;
            this.dependenciesGrid.DisplayLayout.Override.RowAppearance = appearance28;
            this.dependenciesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.dependenciesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.dependenciesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.dependenciesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.dependenciesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance29.BackColor = System.Drawing.SystemColors.ControlLight;
            this.dependenciesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance29;
            this.dependenciesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.dependenciesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.dependenciesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList1.Key = "TypeValueList";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem5.DataValue = 0;
            valueListItem5.DisplayText = "After Trigger";
            valueListItem6.DataValue = 1;
            valueListItem6.DisplayText = "Check Constraint";
            valueListItem7.DataValue = 2;
            valueListItem7.DisplayText = "Default";
            valueListItem8.DataValue = 3;
            valueListItem8.DisplayText = "First Delete Trigger";
            valueListItem9.DataValue = 4;
            valueListItem9.DisplayText = "First Insert Trigger";
            valueListItem10.DataValue = 5;
            valueListItem10.DisplayText = "First Update Trigger";
            valueListItem11.DataValue = 6;
            valueListItem11.DisplayText = "Function";
            valueListItem12.DataValue = 7;
            valueListItem12.DisplayText = "Inline Table Function";
            valueListItem13.DataValue = 8;
            valueListItem13.DisplayText = "Instead Of Trigger";
            valueListItem14.DataValue = 9;
            valueListItem14.DisplayText = "Last Delete Trigger";
            valueListItem15.DataValue = 10;
            valueListItem15.DisplayText = "Last Insert Trigger";
            valueListItem16.DataValue = 11;
            valueListItem16.DisplayText = "Last Update Trigger";
            valueListItem17.DataValue = 12;
            valueListItem17.DisplayText = "Primary Key";
            valueListItem18.DataValue = 13;
            valueListItem18.DisplayText = "Startup Procedure";
            valueListItem19.DataValue = 14;
            valueListItem19.DisplayText = "Stored Procedure";
            valueListItem20.DataValue = 15;
            valueListItem20.DisplayText = "Foreign Key";
            valueListItem21.DataValue = 16;
            valueListItem21.DisplayText = "Table Function";
            valueListItem22.DataValue = 17;
            valueListItem22.DisplayText = "Trigger";
            valueListItem23.DataValue = 18;
            valueListItem23.DisplayText = "View";
            valueListItem24.DataValue = 19;
            valueListItem24.DisplayText = "Unknown";
            valueList1.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem5,
            valueListItem6,
            valueListItem7,
            valueListItem8,
            valueListItem9,
            valueListItem10,
            valueListItem11,
            valueListItem12,
            valueListItem13,
            valueListItem14,
            valueListItem15,
            valueListItem16,
            valueListItem17,
            valueListItem18,
            valueListItem19,
            valueListItem20,
            valueListItem21,
            valueListItem22,
            valueListItem23,
            valueListItem24});
            valueList2.Key = "ActionValueList";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem25.DataValue = 0;
            valueListItem25.DisplayText = "Integrity";
            valueListItem26.DataValue = 1;
            valueListItem26.DisplayText = "Read";
            valueListItem27.DataValue = 2;
            valueListItem27.DisplayText = "Select All";
            valueListItem28.DataValue = 3;
            valueListItem28.DisplayText = "Update";
            valueListItem29.DataValue = 4;
            valueListItem29.DisplayText = "Update Select All";
            valueListItem30.DataValue = 5;
            valueListItem30.DisplayText = "None";
            valueListItem31.DataValue = 5;
            valueListItem31.DisplayText = "Unknown";
            valueList2.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem25,
            valueListItem26,
            valueListItem27,
            valueListItem28,
            valueListItem29,
            valueListItem30,
            valueListItem31});
            this.dependenciesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2});
            this.dependenciesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.dependenciesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.dependenciesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dependenciesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dependenciesGrid.Location = new System.Drawing.Point(0, 1);
            this.dependenciesGrid.Name = "dependenciesGrid";
            this.dependenciesGrid.Size = new System.Drawing.Size(685, 179);
            this.dependenciesGrid.TabIndex = 4;
            this.dependenciesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dependenciesGrid_MouseDown);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.dependenciesReferencedByViewRadioButton);
            this.panel1.Controls.Add(this.dependenciesReferencesViewRadioButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(685, 30);
            this.panel1.TabIndex = 5;
            // 
            // dependenciesReferencedByViewRadioButton
            // 
            this.dependenciesReferencedByViewRadioButton.AutoSize = true;
            this.dependenciesReferencedByViewRadioButton.Location = new System.Drawing.Point(105, 7);
            this.dependenciesReferencedByViewRadioButton.Name = "dependenciesReferencedByViewRadioButton";
            this.dependenciesReferencedByViewRadioButton.Size = new System.Drawing.Size(96, 17);
            this.dependenciesReferencedByViewRadioButton.TabIndex = 1;
            this.dependenciesReferencedByViewRadioButton.Text = "Referenced By";
            this.dependenciesReferencedByViewRadioButton.UseVisualStyleBackColor = true;
            // 
            // dependenciesReferencesViewRadioButton
            // 
            this.dependenciesReferencesViewRadioButton.AutoSize = true;
            this.dependenciesReferencesViewRadioButton.Checked = true;
            this.dependenciesReferencesViewRadioButton.Location = new System.Drawing.Point(6, 7);
            this.dependenciesReferencesViewRadioButton.Name = "dependenciesReferencesViewRadioButton";
            this.dependenciesReferencesViewRadioButton.Size = new System.Drawing.Size(80, 17);
            this.dependenciesReferencesViewRadioButton.TabIndex = 0;
            this.dependenciesReferencesViewRadioButton.TabStop = true;
            this.dependenciesReferencesViewRadioButton.Text = "References";
            this.dependenciesReferencesViewRadioButton.UseVisualStyleBackColor = true;
            this.dependenciesReferencesViewRadioButton.CheckedChanged += new System.EventHandler(this.dependenciesReferencesViewRadioButton_CheckedChanged);
            // 
            // indexesTabPageControl
            // 
            this.indexesTabPageControl.Controls.Add(this.indexesGrid);
            this.indexesTabPageControl.Location = new System.Drawing.Point(-10000, -10000);
            this.indexesTabPageControl.Name = "indexesTabPageControl";
            this.indexesTabPageControl.Size = new System.Drawing.Size(685, 210);
            // 
            // indexesGrid
            // 
            appearance30.BackColor = System.Drawing.SystemColors.Window;
            appearance30.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.indexesGrid.DisplayLayout.Appearance = appearance30;
            this.indexesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn24.Header.Fixed = true;
            ultraGridColumn24.Header.VisiblePosition = 0;
            ultraGridColumn25.Header.VisiblePosition = 1;
            ultraGridColumn26.Header.VisiblePosition = 2;
            appearance31.TextHAlignAsString = "Right";
            ultraGridColumn27.CellAppearance = appearance31;
            ultraGridColumn27.Format = "N2";
            ultraGridColumn27.Header.VisiblePosition = 3;
            appearance32.TextHAlignAsString = "Right";
            ultraGridColumn28.CellAppearance = appearance32;
            ultraGridColumn28.Header.VisiblePosition = 4;
            ultraGridColumn28.Hidden = true;
            ultraGridColumn29.Format = "G";
            ultraGridColumn29.Header.VisiblePosition = 5;
            appearance33.TextHAlignAsString = "Right";
            ultraGridColumn30.CellAppearance = appearance33;
            ultraGridColumn30.Header.VisiblePosition = 6;
            appearance34.TextHAlignAsString = "Right";
            ultraGridColumn31.CellAppearance = appearance34;
            ultraGridColumn31.Header.VisiblePosition = 7;
            appearance35.TextHAlignAsString = "Right";
            ultraGridColumn32.CellAppearance = appearance35;
            ultraGridColumn32.Header.Caption = "Depth";
            ultraGridColumn32.Header.VisiblePosition = 8;
            ultraGridBand2.Columns.AddRange(new object[] {
            ultraGridColumn24,
            ultraGridColumn25,
            ultraGridColumn26,
            ultraGridColumn27,
            ultraGridColumn28,
            ultraGridColumn29,
            ultraGridColumn30,
            ultraGridColumn31,
            ultraGridColumn32});
            this.indexesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand2);
            this.indexesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.indexesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance36.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance36.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.indexesGrid.DisplayLayout.GroupByBox.Appearance = appearance36;
            appearance37.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance37;
            this.indexesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance38.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance38.BackColor2 = System.Drawing.SystemColors.Control;
            appearance38.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance38.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance38;
            this.indexesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.indexesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.indexesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.indexesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.indexesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.indexesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance39.BackColor = System.Drawing.SystemColors.Window;
            this.indexesGrid.DisplayLayout.Override.CardAreaAppearance = appearance39;
            appearance40.BorderColor = System.Drawing.Color.Silver;
            appearance40.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.indexesGrid.DisplayLayout.Override.CellAppearance = appearance40;
            this.indexesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.indexesGrid.DisplayLayout.Override.CellPadding = 0;
            this.indexesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.indexesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance41.BackColor = System.Drawing.SystemColors.Control;
            appearance41.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance41.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance41.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance41.BorderColor = System.Drawing.SystemColors.Window;
            this.indexesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance41;
            this.indexesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance42.TextHAlignAsString = "Left";
            this.indexesGrid.DisplayLayout.Override.HeaderAppearance = appearance42;
            this.indexesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance43.BackColor = System.Drawing.SystemColors.Window;
            appearance43.BorderColor = System.Drawing.Color.Silver;
            this.indexesGrid.DisplayLayout.Override.RowAppearance = appearance43;
            this.indexesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.indexesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance44.BackColor = System.Drawing.SystemColors.ControlLight;
            this.indexesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance44;
            this.indexesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.indexesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.indexesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList3.Key = "BooleanValueList";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem32.DataValue = true;
            valueListItem32.DisplayText = "True";
            valueListItem33.DataValue = false;
            valueListItem33.DisplayText = "False";
            valueList3.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem32,
            valueListItem33});
            this.indexesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList3});
            this.indexesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.indexesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.indexesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indexesGrid.Location = new System.Drawing.Point(0, 0);
            this.indexesGrid.Name = "indexesGrid";
            this.indexesGrid.Size = new System.Drawing.Size(685, 210);
            this.indexesGrid.TabIndex = 5;
            this.indexesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.indexesGrid_MouseDown);
            // 
            // indexStatisticsTabPageControl
            // 
            this.indexStatisticsTabPageControl.Controls.Add(this.panel3);
            this.indexStatisticsTabPageControl.Controls.Add(this.panel4);
            this.indexStatisticsTabPageControl.Location = new System.Drawing.Point(-10000, -10000);
            this.indexStatisticsTabPageControl.Name = "indexStatisticsTabPageControl";
            this.indexStatisticsTabPageControl.Size = new System.Drawing.Size(685, 210);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.panel3.Controls.Add(this.indexStatisticsColumnsGrid);
            this.panel3.Controls.Add(this.indexStatisticsDataDistributionGrid);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 30);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this.panel3.Size = new System.Drawing.Size(685, 180);
            this.panel3.TabIndex = 8;
            // 
            // indexStatisticsColumnsGrid
            // 
            appearance45.BackColor = System.Drawing.SystemColors.Window;
            appearance45.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.indexStatisticsColumnsGrid.DisplayLayout.Appearance = appearance45;
            this.indexStatisticsColumnsGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn33.Header.Fixed = true;
            ultraGridColumn33.Header.VisiblePosition = 0;
            ultraGridColumn33.Width = 290;
            appearance46.TextHAlignAsString = "Right";
            ultraGridColumn34.CellAppearance = appearance46;
            ultraGridColumn34.Format = "N2";
            ultraGridColumn34.Header.VisiblePosition = 1;
            ultraGridColumn34.Width = 201;
            appearance47.TextHAlignAsString = "Right";
            ultraGridColumn35.CellAppearance = appearance47;
            ultraGridColumn35.Format = "N2";
            ultraGridColumn35.Header.VisiblePosition = 2;
            appearance48.TextHAlignAsString = "Right";
            ultraGridColumn36.CellAppearance = appearance48;
            ultraGridColumn36.Format = "0.00\\%";
            ultraGridColumn36.Header.VisiblePosition = 3;
            ultraGridBand3.Columns.AddRange(new object[] {
            ultraGridColumn33,
            ultraGridColumn34,
            ultraGridColumn35,
            ultraGridColumn36});
            this.indexStatisticsColumnsGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand3);
            this.indexStatisticsColumnsGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.indexStatisticsColumnsGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance49.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance49.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.indexStatisticsColumnsGrid.DisplayLayout.GroupByBox.Appearance = appearance49;
            appearance50.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexStatisticsColumnsGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance50;
            this.indexStatisticsColumnsGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexStatisticsColumnsGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance51.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance51.BackColor2 = System.Drawing.SystemColors.Control;
            appearance51.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance51.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexStatisticsColumnsGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance51;
            this.indexStatisticsColumnsGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.indexStatisticsColumnsGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance52.BackColor = System.Drawing.SystemColors.Window;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.CardAreaAppearance = appearance52;
            appearance53.BorderColor = System.Drawing.Color.Silver;
            appearance53.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.CellAppearance = appearance53;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.CellPadding = 0;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance54.BackColor = System.Drawing.SystemColors.Control;
            appearance54.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance54.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance54.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance54.BorderColor = System.Drawing.SystemColors.Window;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.GroupByRowAppearance = appearance54;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance55.TextHAlignAsString = "Left";
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.HeaderAppearance = appearance55;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance56.BackColor = System.Drawing.SystemColors.Window;
            appearance56.BorderColor = System.Drawing.Color.Silver;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.RowAppearance = appearance56;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance57.BackColor = System.Drawing.SystemColors.ControlLight;
            this.indexStatisticsColumnsGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance57;
            this.indexStatisticsColumnsGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.indexStatisticsColumnsGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.indexStatisticsColumnsGrid.DisplayLayout.UseFixedHeaders = true;
            this.indexStatisticsColumnsGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.indexStatisticsColumnsGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.indexStatisticsColumnsGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexStatisticsColumnsGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indexStatisticsColumnsGrid.Location = new System.Drawing.Point(0, 1);
            this.indexStatisticsColumnsGrid.Name = "indexStatisticsColumnsGrid";
            this.indexStatisticsColumnsGrid.Size = new System.Drawing.Size(685, 179);
            this.indexStatisticsColumnsGrid.TabIndex = 4;
            this.indexStatisticsColumnsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.indexStatisticsColumnsGrid_MouseDown);
            // 
            // indexStatisticsDataDistributionGrid
            // 
            appearance58.BackColor = System.Drawing.SystemColors.Window;
            appearance58.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Appearance = appearance58;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn37.Header.VisiblePosition = 0;
            ultraGridColumn37.Width = 332;
            appearance59.TextHAlignAsString = "Right";
            ultraGridColumn38.CellAppearance = appearance59;
            ultraGridColumn38.Format = "N0";
            ultraGridColumn38.Header.VisiblePosition = 1;
            ultraGridBand4.Columns.AddRange(new object[] {
            ultraGridColumn37,
            ultraGridColumn38});
            this.indexStatisticsDataDistributionGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand4);
            this.indexStatisticsDataDistributionGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance60.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance60.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance60.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance60.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.indexStatisticsDataDistributionGrid.DisplayLayout.GroupByBox.Appearance = appearance60;
            appearance61.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance61;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance62.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance62.BackColor2 = System.Drawing.SystemColors.Control;
            appearance62.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance62.ForeColor = System.Drawing.SystemColors.GrayText;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance62;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance63.BackColor = System.Drawing.SystemColors.Window;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.CardAreaAppearance = appearance63;
            appearance64.BorderColor = System.Drawing.Color.Silver;
            appearance64.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.CellAppearance = appearance64;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.CellPadding = 0;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance65.BackColor = System.Drawing.SystemColors.Control;
            appearance65.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance65.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance65.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance65.BorderColor = System.Drawing.SystemColors.Window;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.GroupByRowAppearance = appearance65;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance66.TextHAlignAsString = "Left";
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.HeaderAppearance = appearance66;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance67.BackColor = System.Drawing.SystemColors.Window;
            appearance67.BorderColor = System.Drawing.Color.Silver;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.RowAppearance = appearance67;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance68.BackColor = System.Drawing.SystemColors.ControlLight;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance68;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.UseFixedHeaders = true;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.indexStatisticsDataDistributionGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.indexStatisticsDataDistributionGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.indexStatisticsDataDistributionGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.indexStatisticsDataDistributionGrid.Location = new System.Drawing.Point(0, 1);
            this.indexStatisticsDataDistributionGrid.Name = "indexStatisticsDataDistributionGrid";
            this.indexStatisticsDataDistributionGrid.Size = new System.Drawing.Size(685, 179);
            this.indexStatisticsDataDistributionGrid.TabIndex = 5;
            this.indexStatisticsDataDistributionGrid.Visible = false;
            this.indexStatisticsDataDistributionGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.indexStatisticsDataDistributionGrid_MouseDown);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.indexStatisticsIndexSelectionComboBox);
            this.panel4.Controls.Add(this.indexStatisticsDataDistributionViewRadioButton);
            this.panel4.Controls.Add(this.indexStatisticsColumnsViewRadioButton);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(685, 30);
            this.panel4.TabIndex = 7;
            // 
            // indexStatisticsIndexSelectionComboBox
            // 
            this.indexStatisticsIndexSelectionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.indexStatisticsIndexSelectionComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.indexStatisticsIndexSelectionComboBox.Location = new System.Drawing.Point(3, 5);
            this.indexStatisticsIndexSelectionComboBox.Name = "indexStatisticsIndexSelectionComboBox";
            this.indexStatisticsIndexSelectionComboBox.Size = new System.Drawing.Size(459, 21);
            this.indexStatisticsIndexSelectionComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.indexStatisticsIndexSelectionComboBox.TabIndex = 2;
            this.indexStatisticsIndexSelectionComboBox.SelectionChanged += new System.EventHandler(this.indexStatisticsIndexSelectionComboBox_SelectionChanged);
            // 
            // indexStatisticsDataDistributionViewRadioButton
            // 
            this.indexStatisticsDataDistributionViewRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.indexStatisticsDataDistributionViewRadioButton.AutoSize = true;
            this.indexStatisticsDataDistributionViewRadioButton.Location = new System.Drawing.Point(579, 7);
            this.indexStatisticsDataDistributionViewRadioButton.Name = "indexStatisticsDataDistributionViewRadioButton";
            this.indexStatisticsDataDistributionViewRadioButton.Size = new System.Drawing.Size(103, 17);
            this.indexStatisticsDataDistributionViewRadioButton.TabIndex = 1;
            this.indexStatisticsDataDistributionViewRadioButton.Text = "Data Distribution";
            this.indexStatisticsDataDistributionViewRadioButton.UseVisualStyleBackColor = true;
            // 
            // indexStatisticsColumnsViewRadioButton
            // 
            this.indexStatisticsColumnsViewRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.indexStatisticsColumnsViewRadioButton.AutoSize = true;
            this.indexStatisticsColumnsViewRadioButton.Checked = true;
            this.indexStatisticsColumnsViewRadioButton.Location = new System.Drawing.Point(490, 7);
            this.indexStatisticsColumnsViewRadioButton.Name = "indexStatisticsColumnsViewRadioButton";
            this.indexStatisticsColumnsViewRadioButton.Size = new System.Drawing.Size(65, 17);
            this.indexStatisticsColumnsViewRadioButton.TabIndex = 0;
            this.indexStatisticsColumnsViewRadioButton.TabStop = true;
            this.indexStatisticsColumnsViewRadioButton.Text = "Columns";
            this.indexStatisticsColumnsViewRadioButton.UseVisualStyleBackColor = true;
            this.indexStatisticsColumnsViewRadioButton.CheckedChanged += new System.EventHandler(this.indexStatisticsColumnsViewRadioButton_CheckedChanged);
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
            appearance70.Image = ((object)(resources.GetObject("appearance70.Image")));
            buttonTool6.SharedPropsInternal.AppearancesSmall.Appearance = appearance70;
            buttonTool6.SharedPropsInternal.Caption = "Column Chooser";
            appearance71.Image = ((object)(resources.GetObject("appearance71.Image")));
            buttonTool7.SharedPropsInternal.AppearancesSmall.Appearance = appearance71;
            buttonTool7.SharedPropsInternal.Caption = "Group By Box";
            appearance72.Image = ((object)(resources.GetObject("appearance72.Image")));
            buttonTool8.SharedPropsInternal.AppearancesSmall.Appearance = appearance72;
            buttonTool8.SharedPropsInternal.Caption = "Sort Ascending";
            appearance73.Image = ((object)(resources.GetObject("appearance73.Image")));
            buttonTool9.SharedPropsInternal.AppearancesSmall.Appearance = appearance73;
            buttonTool9.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool10.SharedPropsInternal.Caption = "Remove This Column";
            stateButtonTool2.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            stateButtonTool3.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            stateButtonTool3.SharedPropsInternal.Caption = "Toolbar";
            appearance74.Image = ((object)(resources.GetObject("appearance74.Image")));
            buttonTool11.SharedPropsInternal.AppearancesSmall.Appearance = appearance74;
            buttonTool11.SharedPropsInternal.Caption = "Print";
            appearance75.Image = ((object)(resources.GetObject("appearance75.Image")));
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance75;
            buttonTool12.SharedPropsInternal.Caption = "Print";
            appearance76.Image = ((object)(resources.GetObject("appearance76.Image")));
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance76;
            buttonTool13.SharedPropsInternal.Caption = "Export to Excel";
            appearance77.Image = ((object)(resources.GetObject("appearance77.Image")));
            buttonTool14.SharedPropsInternal.AppearancesSmall.Appearance = appearance77;
            buttonTool14.SharedPropsInternal.Caption = "Export to Excel (csv)";
            appearance78.Image = ((object)(resources.GetObject("appearance78.Image")));
            buttonTool15.SharedPropsInternal.AppearancesSmall.Appearance = appearance78;
            buttonTool15.SharedPropsInternal.Caption = "Save Image";
            popupMenuTool2.SharedPropsInternal.Caption = "chartContextMenu";
            stateButtonTool4.MenuDisplayStyle = Infragistics.Win.UltraWinToolbars.StateButtonMenuDisplayStyle.DisplayCheckmark;
            buttonTool16.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            stateButtonTool4,
            buttonTool16,
            buttonTool17,
            buttonTool18});
            popupMenuTool3.SharedPropsInternal.Caption = "gridContextMenu";
            buttonTool21.InstanceProps.IsFirstInGroup = true;
            popupMenuTool3.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool19,
            buttonTool20,
            buttonTool21,
            buttonTool22});
            popupMenuTool4.SharedPropsInternal.Caption = "gridDataContextMenu";
            buttonTool25.InstanceProps.IsFirstInGroup = true;
            buttonTool27.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool23,
            buttonTool24,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            buttonTool28});
            buttonTool29.SharedPropsInternal.Caption = "Update Statistics";
            buttonTool30.SharedPropsInternal.Caption = "Rebuild Indexes";
            buttonTool31.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool32.SharedPropsInternal.Caption = "Expand All Groups";
            popupMenuTool5.SharedPropsInternal.Caption = "gridNoGroupsContextMenu";
            popupMenuTool5.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool33,
            buttonTool34});
            this.toolbarsManager.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool1,
            buttonTool6,
            buttonTool7,
            buttonTool8,
            buttonTool9,
            buttonTool10,
            stateButtonTool2,
            stateButtonTool3,
            buttonTool11,
            buttonTool12,
            buttonTool13,
            buttonTool14,
            buttonTool15,
            popupMenuTool2,
            popupMenuTool3,
            popupMenuTool4,
            buttonTool29,
            buttonTool30,
            buttonTool31,
            buttonTool32,
            popupMenuTool5});
            this.toolbarsManager.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.toolbarsManager_BeforeToolDropdown);
            this.toolbarsManager.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.toolbarsManager_ToolClick);
            // 
            // DatabasesTablesIndexesView_Fill_Panel
            // 
            this.DatabasesTablesIndexesView_Fill_Panel.Controls.Add(this.refreshDatabasesButton);
            this.DatabasesTablesIndexesView_Fill_Panel.Controls.Add(this.splitContainer);
            this.DatabasesTablesIndexesView_Fill_Panel.Controls.Add(this.availableDatabasesComboBox);
            this.DatabasesTablesIndexesView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.DatabasesTablesIndexesView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DatabasesTablesIndexesView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.DatabasesTablesIndexesView_Fill_Panel.Name = "DatabasesTablesIndexesView_Fill_Panel";
            this.DatabasesTablesIndexesView_Fill_Panel.Size = new System.Drawing.Size(689, 517);
            this.DatabasesTablesIndexesView_Fill_Panel.TabIndex = 8;
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
            this.refreshDatabasesButton.TabIndex = 5;
            this.refreshDatabasesButton.Click += new System.EventHandler(this.refreshDatabasesButton_Click);
            this.refreshDatabasesButton.MouseEnterElement += new UIElementEventHandler(mouseEnter_refreshDatabasesButton);
            this.refreshDatabasesButton.MouseLeaveElement += new UIElementEventHandler(mouseLeave_refreshDatabasesButton);
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            // 
            // splitContainer
            // 
            this.splitContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer.Location = new System.Drawing.Point(0, 28);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(203)))), ((int)(((byte)(203)))), ((int)(((byte)(203)))));
            this.splitContainer.Panel1.Controls.Add(this.tablesGrid);
            this.splitContainer.Panel1.Controls.Add(this.tablesGridStatusMessage);
            this.splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.splitContainer.Panel2.Controls.Add(this.tableDetailsTabControl);
            this.splitContainer.Panel2.Controls.Add(this.tableDetailsStatusLabel);
            this.splitContainer.Panel2.Controls.Add(this.tableDetailsHeaderStrip);
            this.splitContainer.Size = new System.Drawing.Size(689, 489);
            this.splitContainer.SplitterDistance = 230;
            this.splitContainer.TabIndex = 0;
            this.splitContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseDown);
            this.splitContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.splitContainer_MouseUp);
            // 
            // tablesGrid
            // 
            appearance2.BackColor = System.Drawing.SystemColors.Window;
            appearance2.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.tablesGrid.DisplayLayout.Appearance = appearance2;
            this.tablesGrid.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.Header.Fixed = true;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.Header.Fixed = true;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn4.Header.VisiblePosition = 5;
            ultraGridColumn4.Hidden = true;
            ultraGridColumn5.Header.VisiblePosition = 3;
            ultraGridColumn5.Hidden = true;
            ultraGridColumn6.Header.VisiblePosition = 4;
            appearance3.TextHAlignAsString = "Right";
            ultraGridColumn7.CellAppearance = appearance3;
            ultraGridColumn7.Format = "N2";
            ultraGridColumn7.Header.VisiblePosition = 10;
            appearance4.TextHAlignAsString = "Right";
            ultraGridColumn8.CellAppearance = appearance4;
            ultraGridColumn8.Format = "N2";
            ultraGridColumn8.Header.VisiblePosition = 11;
            appearance5.TextHAlignAsString = "Right";
            ultraGridColumn9.CellAppearance = appearance5;
            ultraGridColumn9.Format = "N2";
            ultraGridColumn9.Header.VisiblePosition = 12;
            appearance6.TextHAlignAsString = "Right";
            ultraGridColumn10.CellAppearance = appearance6;
            ultraGridColumn10.Format = "0.00%";
            ultraGridColumn10.Header.VisiblePosition = 9;
            appearance7.TextHAlignAsString = "Right";
            ultraGridColumn11.CellAppearance = appearance7;
            ultraGridColumn11.Format = "0.00%";
            ultraGridColumn11.Header.VisiblePosition = 6;
            ultraGridColumn11.NullText = "N/A";
            ultraGridColumn12.Format = "G";
            ultraGridColumn12.Header.VisiblePosition = 15;
            ultraGridColumn12.NullText = "Never";
            appearance8.TextHAlignAsString = "Right";
            ultraGridColumn13.CellAppearance = appearance8;
            ultraGridColumn13.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn13.Format = "0.00%";
            ultraGridColumn13.Header.VisiblePosition = 7;
            ultraGridColumn13.Hidden = true;
            appearance9.TextHAlignAsString = "Right";
            ultraGridColumn14.CellAppearance = appearance9;
            ultraGridColumn14.Header.Caption = "% Fragmentation (Current)";
            ultraGridColumn14.Header.VisiblePosition = 8;
            ultraGridColumn14.Hidden = true;
            appearance10.TextHAlignAsString = "Right";
            ultraGridColumn15.CellAppearance = appearance10;
            ultraGridColumn15.Header.VisiblePosition = 13;
            ultraGridColumn16.Header.VisiblePosition = 14;
            ultraGridColumn17.Format = "G";
            ultraGridColumn17.Header.VisiblePosition = 16;
            ultraGridColumn18.Header.VisiblePosition = 17;
            ultraGridColumn19.Header.VisiblePosition = 18;
            ultraGridColumn20.Header.VisiblePosition = 19;
            ultraGridColumn20.NullText = "";
            ultraGridBand5.Columns.AddRange(new object[] {
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
            ultraGridColumn20});
            this.tablesGrid.DisplayLayout.BandsSerializer.Add(ultraGridBand5);
            this.tablesGrid.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.tablesGrid.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance11.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.tablesGrid.DisplayLayout.GroupByBox.Appearance = appearance11;
            appearance12.ForeColor = System.Drawing.SystemColors.GrayText;
            this.tablesGrid.DisplayLayout.GroupByBox.BandLabelAppearance = appearance12;
            this.tablesGrid.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.tablesGrid.DisplayLayout.GroupByBox.Hidden = true;
            appearance13.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance13.BackColor2 = System.Drawing.SystemColors.Control;
            appearance13.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance13.ForeColor = System.Drawing.SystemColors.GrayText;
            this.tablesGrid.DisplayLayout.GroupByBox.PromptAppearance = appearance13;
            this.tablesGrid.DisplayLayout.MaxColScrollRegions = 1;
            this.tablesGrid.DisplayLayout.MaxRowScrollRegions = 1;
            this.tablesGrid.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.tablesGrid.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.tablesGrid.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.tablesGrid.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.tablesGrid.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance14.BackColor = System.Drawing.SystemColors.Window;
            this.tablesGrid.DisplayLayout.Override.CardAreaAppearance = appearance14;
            appearance15.BorderColor = System.Drawing.Color.Silver;
            appearance15.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.tablesGrid.DisplayLayout.Override.CellAppearance = appearance15;
            this.tablesGrid.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.tablesGrid.DisplayLayout.Override.CellPadding = 0;
            this.tablesGrid.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.tablesGrid.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance16.BackColor = System.Drawing.SystemColors.Control;
            appearance16.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance16.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance16.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance16.BorderColor = System.Drawing.SystemColors.Window;
            this.tablesGrid.DisplayLayout.Override.GroupByRowAppearance = appearance16;
            this.tablesGrid.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance17.TextHAlignAsString = "Left";
            this.tablesGrid.DisplayLayout.Override.HeaderAppearance = appearance17;
            this.tablesGrid.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            appearance18.BackColor = System.Drawing.SystemColors.Window;
            appearance18.BorderColor = System.Drawing.Color.Silver;
            this.tablesGrid.DisplayLayout.Override.RowAppearance = appearance18;
            this.tablesGrid.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.tablesGrid.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.tablesGrid.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.tablesGrid.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.tablesGrid.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance19.BackColor = System.Drawing.SystemColors.ControlLight;
            this.tablesGrid.DisplayLayout.Override.TemplateAddRowAppearance = appearance19;
            this.tablesGrid.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.tablesGrid.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.tablesGrid.DisplayLayout.UseFixedHeaders = true;
            valueList4.Key = "TableType";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = true;
            valueListItem1.DisplayText = "System";
            valueListItem2.DataValue = false;
            valueListItem2.DisplayText = "User";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            valueList5.Key = "BooleanType";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem3.DataValue = true;
            valueListItem3.DisplayText = "True";
            valueListItem4.DataValue = false;
            valueListItem4.DisplayText = "False";
            valueList5.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem4});
            this.tablesGrid.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList4,
            valueList5});
            this.tablesGrid.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.tablesGrid.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.tablesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tablesGrid.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tablesGrid.Location = new System.Drawing.Point(0, 1);
            this.tablesGrid.Name = "tablesGrid";
            this.tablesGrid.Size = new System.Drawing.Size(689, 228);
            this.tablesGrid.TabIndex = 3;
            this.tablesGrid.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.tablesGrid_AfterSelectChange);
            this.tablesGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tablesGrid_MouseDown);
            // 
            // tablesGridStatusMessage
            // 
            this.tablesGridStatusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tablesGridStatusMessage.BackColor = System.Drawing.Color.White;
            this.tablesGridStatusMessage.Location = new System.Drawing.Point(0, 24);
            this.tablesGridStatusMessage.Name = "tablesGridStatusMessage";
            this.tablesGridStatusMessage.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.tablesGridStatusMessage.Size = new System.Drawing.Size(689, 205);
            this.tablesGridStatusMessage.TabIndex = 4;
            this.tablesGridStatusMessage.Text = "< status message >";
            this.tablesGridStatusMessage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.tablesGridStatusMessage.Visible = false;
            // 
            // tableDetailsTabControl
            // 
            this.tableDetailsTabControl.Controls.Add(this.ultraTabSharedControlsPage1);
            this.tableDetailsTabControl.Controls.Add(this.sizeTabPageControl);
            this.tableDetailsTabControl.Controls.Add(this.dependenciesTabPageControl);
            this.tableDetailsTabControl.Controls.Add(this.indexesTabPageControl);
            this.tableDetailsTabControl.Controls.Add(this.indexStatisticsTabPageControl);
            this.tableDetailsTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableDetailsTabControl.Location = new System.Drawing.Point(0, 19);
            this.tableDetailsTabControl.Name = "tableDetailsTabControl";
            this.tableDetailsTabControl.SharedControlsPage = this.ultraTabSharedControlsPage1;
            this.tableDetailsTabControl.Size = new System.Drawing.Size(689, 236);
            this.tableDetailsTabControl.TabIndex = 0;
            ultraTab1.TabPage = this.sizeTabPageControl;
            ultraTab1.Text = "Size";
            ultraTab2.TabPage = this.dependenciesTabPageControl;
            ultraTab2.Text = "Dependencies";
            ultraTab3.TabPage = this.indexesTabPageControl;
            ultraTab3.Text = "Indexes";
            ultraTab4.TabPage = this.indexStatisticsTabPageControl;
            ultraTab4.Text = "Index Statistics";
            this.tableDetailsTabControl.Tabs.AddRange(new Infragistics.Win.UltraWinTabControl.UltraTab[] {
            ultraTab1,
            ultraTab2,
            ultraTab3,
            ultraTab4});
            this.tableDetailsTabControl.Visible = false;
            // 
            // ultraTabSharedControlsPage1
            // 
            this.ultraTabSharedControlsPage1.Location = new System.Drawing.Point(-10000, -10000);
            this.ultraTabSharedControlsPage1.Name = "ultraTabSharedControlsPage1";
            this.ultraTabSharedControlsPage1.Size = new System.Drawing.Size(685, 210);
            // 
            // tableDetailsStatusLabel
            // 
            this.tableDetailsStatusLabel.BackColor = System.Drawing.Color.White;
            this.tableDetailsStatusLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableDetailsStatusLabel.Location = new System.Drawing.Point(0, 19);
            this.tableDetailsStatusLabel.Name = "tableDetailsStatusLabel";
            this.tableDetailsStatusLabel.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.tableDetailsStatusLabel.Size = new System.Drawing.Size(689, 236);
            this.tableDetailsStatusLabel.TabIndex = 11;
            this.tableDetailsStatusLabel.Text = "< status message >";
            this.tableDetailsStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableDetailsHeaderStrip
            // 
            this.tableDetailsHeaderStrip.AutoSize = false;
            this.tableDetailsHeaderStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.tableDetailsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tableDetailsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.tableDetailsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.tableDetailsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tableDetailsHeaderStrip.HotTrackEnabled = false;
            this.tableDetailsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tableDetailsHeaderStripLabel,
            this.hideTableDetailsButton});
            this.tableDetailsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.tableDetailsHeaderStrip.Name = "tableDetailsHeaderStrip";
            this.tableDetailsHeaderStrip.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.tableDetailsHeaderStrip.Size = new System.Drawing.Size(689, 19);
            this.tableDetailsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.tableDetailsHeaderStrip.TabIndex = 10;
            // 
            // tableDetailsHeaderStripLabel
            // 
            this.tableDetailsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tableDetailsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.tableDetailsHeaderStripLabel.Name = "tableDetailsHeaderStripLabel";
            this.tableDetailsHeaderStripLabel.Size = new System.Drawing.Size(80, 16);
            this.tableDetailsHeaderStripLabel.Text = "Table Details";
            // 
            // hideTableDetailsButton
            // 
            this.hideTableDetailsButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.hideTableDetailsButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(172)))), ((int)(((byte)(172)))), ((int)(((byte)(172)))));
            this.hideTableDetailsButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.hideTableDetailsButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Office2007Close;
            this.hideTableDetailsButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.hideTableDetailsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.hideTableDetailsButton.Name = "hideTableDetailsButton";
            this.hideTableDetailsButton.Size = new System.Drawing.Size(23, 16);
            this.hideTableDetailsButton.ToolTipText = "Close";
            this.hideTableDetailsButton.Click += new System.EventHandler(this.hideTableDetailsButton_Click);
            // 
            // availableDatabasesComboBox
            // 
            this.availableDatabasesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.availableDatabasesComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.availableDatabasesComboBox.Location = new System.Drawing.Point(3, 4);
            this.availableDatabasesComboBox.Name = "availableDatabasesComboBox";
            this.availableDatabasesComboBox.Size = new System.Drawing.Size(657, 21);
            this.availableDatabasesComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.availableDatabasesComboBox.TabIndex = 0;
            this.availableDatabasesComboBox.SelectionChanged += new System.EventHandler(this.availableDatabasesComboBox_SelectionChanged);
            // 
            // refreshAvailableDatabasesBackgroundWorker
            // 
            this.refreshAvailableDatabasesBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshAvailableDatabasesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshAvailableDatabasesBackgroundWorker_DoWork);
            this.refreshAvailableDatabasesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshAvailableDatabasesBackgroundWorker_RunWorkerCompleted);
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "";
            // 
            // refreshTableDetailsBackgroundWorker
            // 
            this.refreshTableDetailsBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshTableDetailsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshTableDetailsBackgroundWorker_DoWork);
            this.refreshTableDetailsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshTableDetailsBackgroundWorker_RunWorkerCompleted);
            // 
            // refreshIndexStatisticsBackgroundWorker
            // 
            this.refreshIndexStatisticsBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshIndexStatisticsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshIndexStatisticsBackgroundWorker_DoWork);
            this.refreshIndexStatisticsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshIndexStatisticsBackgroundWorker_RunWorkerCompleted);
            // 
            // rebuildIndexesBackgroundWorker
            // 
            this.rebuildIndexesBackgroundWorker.WorkerReportsProgress = true;
            this.rebuildIndexesBackgroundWorker.WorkerSupportsCancellation = true;
            this.rebuildIndexesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.rebuildIndexesBackgroundWorker_DoWork);
            this.rebuildIndexesBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.rebuildIndexesBackgroundWorker_ProgressChanged);
            this.rebuildIndexesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.rebuildIndexesBackgroundWorker_RunWorkerCompleted);
            // 
            // updateStatisticsBackgroundWorker
            // 
            this.updateStatisticsBackgroundWorker.WorkerSupportsCancellation = true;
            this.updateStatisticsBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.updateStatisticsBackgroundWorker_DoWork);
            this.updateStatisticsBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.updateStatisticsBackgroundWorker_RunWorkerCompleted);
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
            // DatabasesTablesIndexesView
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.Controls.Add(this.DatabasesTablesIndexesView_Fill_Panel);
            this.Controls.Add(this.historicalSnapshotStatusLinkLabel);
            this.Name = "DatabasesTablesIndexesView";
            this.Size = new System.Drawing.Size(689, 517);
            this.Load += new System.EventHandler(this.DatabasesTablesIndexesView_Load);
            this.sizeTabPageControl.ResumeLayout(false);
            this.sizeTabPageControl.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tableSizeChart)).EndInit();
            this.dependenciesTabPageControl.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dependenciesGrid)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.indexesTabPageControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.indexesGrid)).EndInit();
            this.indexStatisticsTabPageControl.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsColumnsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsDataDistributionGrid)).EndInit();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.indexStatisticsIndexSelectionComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.toolbarsManager)).EndInit();
            this.DatabasesTablesIndexesView_Fill_Panel.ResumeLayout(false);
            this.DatabasesTablesIndexesView_Fill_Panel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tablesGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tableDetailsTabControl)).EndInit();
            this.tableDetailsTabControl.ResumeLayout(false);
            this.tableDetailsHeaderStrip.ResumeLayout(false);
            this.tableDetailsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.availableDatabasesComboBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.ContextMenuManager toolbarsManager;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  DatabasesTablesIndexesView_Fill_Panel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor availableDatabasesComboBox;
        private Infragistics.Win.UltraWinTabControl.UltraTabControl tableDetailsTabControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabSharedControlsPage ultraTabSharedControlsPage1;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl sizeTabPageControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl dependenciesTabPageControl;
        private Infragistics.Win.Misc.UltraButton refreshDatabasesButton;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl indexesTabPageControl;
        private Infragistics.Win.UltraWinTabControl.UltraTabPageControl indexStatisticsTabPageControl;
        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip tableDetailsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel tableDetailsHeaderStripLabel;
        private System.Windows.Forms.ToolStripButton hideTableDetailsButton;
        private System.ComponentModel.BackgroundWorker refreshAvailableDatabasesBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel tableDetailsStatusLabel;
        private Infragistics.Win.UltraWinGrid.UltraGrid tablesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel tablesGridStatusMessage;
        private ChartFX.WinForms.Chart tableSizeChart;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Infragistics.Win.UltraWinGrid.UltraGrid dependenciesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton dependenciesReferencedByViewRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton dependenciesReferencesViewRadioButton;
        private Infragistics.Win.UltraWinGrid.UltraGrid indexesGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Infragistics.Win.UltraWinGrid.UltraGrid indexStatisticsColumnsGrid;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton indexStatisticsDataDistributionViewRadioButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomRadioButton indexStatisticsColumnsViewRadioButton;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor indexStatisticsIndexSelectionComboBox;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private System.ComponentModel.BackgroundWorker refreshTableDetailsBackgroundWorker;
        private System.ComponentModel.BackgroundWorker refreshIndexStatisticsBackgroundWorker;
        private Infragistics.Win.UltraWinGrid.UltraGrid indexStatisticsDataDistributionGrid;
        private System.ComponentModel.BackgroundWorker rebuildIndexesBackgroundWorker;
        private System.ComponentModel.BackgroundWorker updateStatisticsBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel historicalSnapshotStatusLinkLabel;
        private Infragistics.Win.Appearance appearance1;
    }
}
