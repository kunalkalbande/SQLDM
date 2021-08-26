using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    partial class AuditedActionsView
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
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("AuditableEventID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("ActionID");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Action");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("DateTime", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Descending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn5 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Workstation");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn6 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("WorkstationUser");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn7 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("SQLUser");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn8 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn9 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("MetaData");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn10 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Header");
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance11 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList1 = new Infragistics.Win.ValueList(29722407);
            Infragistics.Win.ValueList valueList2 = new Infragistics.Win.ValueList(29767626);
            Infragistics.Win.Appearance appearance12 = new Infragistics.Win.Appearance();
            Infragistics.Win.ValueList valueList3 = new Infragistics.Win.ValueList(17251657);
            Infragistics.Win.ValueList valueList4 = new Infragistics.Win.ValueList(84474157);
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn1 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("AuditableEventID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn2 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("ActionID");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn3 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Action");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn4 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("DateTime");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn5 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Workstation");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn6 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("WorkstationUser");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn7 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("SQLUser");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn8 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Name");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn9 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("MetaData");
            Infragistics.Win.UltraWinDataSource.UltraDataColumn ultraDataColumn10 = new Infragistics.Win.UltraWinDataSource.UltraDataColumn("Header");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool3 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("GridRowMenu");
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool1 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("ColumnContext");
            Infragistics.Win.UltraWinToolbars.RibbonTab ribbonTab1 = new Infragistics.Win.UltraWinToolbars.RibbonTab("ribbon1");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup1 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup1");
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool1 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("TimeButton");
            Infragistics.Win.UltraWinToolbars.RibbonGroup ribbonGroup2 = new Infragistics.Win.UltraWinToolbars.RibbonGroup("ribbonGroup2");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool16 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ExpandAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool17 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CollapseAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool18 = new Infragistics.Win.UltraWinToolbars.ButtonTool("GroupByBoxMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool2 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ribbonButtonPrint");
            Infragistics.Win.Appearance appearance15 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool5 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ribbonButtonPdf");
            Infragistics.Win.Appearance appearance14 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool6 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ribbonButtonExcel");
            Infragistics.Win.Appearance appearance13 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool2 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("GridRowMenu");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool4 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CollapseAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool14 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ExpandAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool9 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PrintMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool7 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ExcelMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool1 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CopyToClipboardMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool8 = new Infragistics.Win.UltraWinToolbars.ButtonTool("Print");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool10 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PrintMenuOption");
            Infragistics.Win.Appearance appearance26 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool12 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ExcelMenuOption");
            Infragistics.Win.Appearance appearance27 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool13 = new Infragistics.Win.UltraWinToolbars.ButtonTool("PDFMenuOption");
            Infragistics.Win.Appearance appearance28 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool3 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CopyToClipboardMenuOption");
            Infragistics.Win.Appearance appearance25 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupControlContainerTool popupControlContainerTool2 = new Infragistics.Win.UltraWinToolbars.PopupControlContainerTool("PopupControlContainerTool1");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool11 = new Infragistics.Win.UltraWinToolbars.ButtonTool("CollapseAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool15 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ExpandAllGroupsMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool22 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SortAscendingMenuOption");
            Infragistics.Win.Appearance appearance29 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool23 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SortDescendingMenuOption");
            Infragistics.Win.Appearance appearance30 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool25 = new Infragistics.Win.UltraWinToolbars.ButtonTool("GroupByBoxMenuOption");
            Infragistics.Win.Appearance appearance32 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool26 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RemoveThisColumnMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool27 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ColumnChooserMenuOption");
            Infragistics.Win.Appearance appearance33 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.PopupMenuTool popupMenuTool4 = new Infragistics.Win.UltraWinToolbars.PopupMenuTool("ColumnContext");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool28 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SortAscendingMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool32 = new Infragistics.Win.UltraWinToolbars.ButtonTool("SortDescendingMenuOption");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool1 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("GroupByThisColumnMenuOption", "");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool35 = new Infragistics.Win.UltraWinToolbars.ButtonTool("GroupByBoxMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool36 = new Infragistics.Win.UltraWinToolbars.ButtonTool("RemoveThisColumnMenuOption");
            Infragistics.Win.UltraWinToolbars.ButtonTool buttonTool37 = new Infragistics.Win.UltraWinToolbars.ButtonTool("ColumnChooserMenuOption");
            Infragistics.Win.UltraWinToolbars.StateButtonTool stateButtonTool2 = new Infragistics.Win.UltraWinToolbars.StateButtonTool("GroupByThisColumnMenuOption", "");
            Infragistics.Win.Appearance appearance34 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinToolbars.ComboBoxTool comboBoxTool2 = new Infragistics.Win.UltraWinToolbars.ComboBoxTool("TimeButton");
            Infragistics.Win.ValueList valueList5 = new Infragistics.Win.ValueList(0);
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.UltraWinToolbars.LabelTool labelTool2 = new Infragistics.Win.UltraWinToolbars.LabelTool("DateRangeMenuLabel");
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.AuditedActionsView_Fill_Panel = new Infragistics.Win.Misc.UltraPanel();
            this.activeAlerts_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.ultraGridAuditActions = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.auditedActionsViewDataSource = new Idera.SQLdm.DesktopClient.Views.Administration.AuditedActionsViewDataSource();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelAction = new System.Windows.Forms.Label();
            this.labelDate = new System.Windows.Forms.Label();
            this.labelWorkastation = new System.Windows.Forms.Label();
            this.labelWorkstationUser = new System.Windows.Forms.Label();
            this.labelSQLUser = new System.Windows.Forms.Label();
            this.labelName = new System.Windows.Forms.Label();
            this.textBoxBody = new System.Windows.Forms.TextBox();
            this.detailsHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.detailsHeaderStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.labelNoRowSelected = new System.Windows.Forms.Label();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.contextMenuManager1 = new Idera.SQLdm.DesktopClient.Controls.ContextMenuManager(this.components);
            this._AuditedActionsView_Toolbars_Dock_Area_Left = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_Right = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_Top = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom = new Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.ultraGridExcelExporter = new Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter(this.components);
            this.ultraGridPrintDocument = new Infragistics.Win.UltraWinGrid.UltraGridPrintDocument(this.components);
            this.ultraPrintPreviewDialog = new Infragistics.Win.Printing.UltraPrintPreviewDialog(this.components);
            this.AuditedActionsView_Fill_Panel.ClientArea.SuspendLayout();
            this.AuditedActionsView_Fill_Panel.SuspendLayout();
            this.activeAlerts_Fill_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridAuditActions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditedActionsViewDataSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.tableLayoutMain.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.detailsHeaderStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contextMenuManager1)).BeginInit();
            this.SuspendLayout();
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_1_Left
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.BackColor = System.Drawing.Color.White;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.Location = new System.Drawing.Point(0, 50);
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.Name = "_AuditedActionsView_Toolbars_Dock_Area_1_Left";
            this._AuditedActionsView_Toolbars_Dock_Area_1_Left.Size = new System.Drawing.Size(0, 479);
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_1_Right
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.BackColor = System.Drawing.Color.White;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.Location = new System.Drawing.Point(814, 50);
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.Name = "_AuditedActionsView_Toolbars_Dock_Area_1_Right";
            this._AuditedActionsView_Toolbars_Dock_Area_1_Right.Size = new System.Drawing.Size(0, 479);
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_1_Top
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.BackColor = System.Drawing.Color.White;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.Location = new System.Drawing.Point(0, 0);
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.Name = "_AuditedActionsView_Toolbars_Dock_Area_1_Top";
            this._AuditedActionsView_Toolbars_Dock_Area_1_Top.Size = new System.Drawing.Size(814, 25);
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_1_Bottom
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.BackColor = System.Drawing.Color.White;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.Location = new System.Drawing.Point(0, 529);
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.Name = "_AuditedActionsView_Toolbars_Dock_Area_1_Bottom";
            this._AuditedActionsView_Toolbars_Dock_Area_1_Bottom.Size = new System.Drawing.Size(814, 0);
            // 
            // AuditedActionsView_Fill_Panel
            // 
            // 
            // AuditedActionsView_Fill_Panel.ClientArea
            // 
            this.AuditedActionsView_Fill_Panel.ClientArea.Controls.Add(this.activeAlerts_Fill_Panel);
            this.AuditedActionsView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.AuditedActionsView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AuditedActionsView_Fill_Panel.Location = new System.Drawing.Point(0, 127);
            this.AuditedActionsView_Fill_Panel.Name = "AuditedActionsView_Fill_Panel";
            this.AuditedActionsView_Fill_Panel.Size = new System.Drawing.Size(814, 402);
            this.AuditedActionsView_Fill_Panel.TabIndex = 0;
            // 
            // activeAlerts_Fill_Panel
            // 
            this.activeAlerts_Fill_Panel.BackColor = System.Drawing.SystemColors.Control;
            this.activeAlerts_Fill_Panel.BackColor2 = System.Drawing.SystemColors.Control;
            this.activeAlerts_Fill_Panel.BorderColor = System.Drawing.Color.White;
            this.activeAlerts_Fill_Panel.BorderWidth = 0;
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left);
            this.activeAlerts_Fill_Panel.Controls.Add(this.splitContainer);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top);
            this.activeAlerts_Fill_Panel.Controls.Add(this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom);
            this.activeAlerts_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.activeAlerts_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.activeAlerts_Fill_Panel.Name = "activeAlerts_Fill_Panel";
            this.activeAlerts_Fill_Panel.Size = new System.Drawing.Size(814, 402);
            this.activeAlerts_Fill_Panel.TabIndex = 4;
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 0);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 402);
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.ultraGridAuditActions);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.panel2);
            this.splitContainer.Size = new System.Drawing.Size(814, 402);
            this.splitContainer.SplitterDistance = 174;
            this.splitContainer.TabIndex = 3;
            // 
            // ultraGridAuditActions
            // 
            this.ultraGridAuditActions.DataMember = "Band 0";
            this.ultraGridAuditActions.DataSource = this.auditedActionsViewDataSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.ultraGridAuditActions.DisplayLayout.Appearance = appearance1;
            this.ultraGridAuditActions.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Hidden = true;
            ultraGridColumn2.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Hidden = true;
            ultraGridColumn3.Header.VisiblePosition = 3;
            ultraGridColumn3.Width = 237;
            ultraGridColumn4.Format = "G";
            ultraGridColumn4.GroupByMode = Infragistics.Win.UltraWinGrid.GroupByMode.OutlookDate;
            ultraGridColumn4.Header.Caption = "Date";
            ultraGridColumn4.Header.VisiblePosition = 2;
            ultraGridColumn4.Style = Infragistics.Win.UltraWinGrid.ColumnStyle.DateTime;
            ultraGridColumn4.Width = 145;
            ultraGridColumn5.Header.VisiblePosition = 5;
            ultraGridColumn5.Width = 131;
            ultraGridColumn6.Header.Caption = "Workstation User";
            ultraGridColumn6.Header.VisiblePosition = 6;
            ultraGridColumn6.Width = 190;
            ultraGridColumn7.Header.Caption = "Repository User";
            ultraGridColumn7.Header.VisiblePosition = 7;
            ultraGridColumn7.Width = 113;
            ultraGridColumn8.Header.Caption = "Object Name";
            ultraGridColumn8.Header.VisiblePosition = 4;
            ultraGridColumn8.Width = 154;
            ultraGridColumn9.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn9.Header.Caption = "Action Details";
            ultraGridColumn9.Header.VisiblePosition = 8;
            ultraGridColumn9.Hidden = true;
            ultraGridColumn10.Header.Caption = "Change Summary";
            ultraGridColumn10.Header.VisiblePosition = 9;
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
            ultraGridColumn10});
            this.ultraGridAuditActions.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.ultraGridAuditActions.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.None;
            this.ultraGridAuditActions.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            appearance3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.Appearance = appearance3;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.BandLabelAppearance = appearance4;
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.Hidden = true;
            appearance5.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance5.BackColor2 = System.Drawing.SystemColors.Control;
            appearance5.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance5.ForeColor = System.Drawing.SystemColors.GrayText;
            this.ultraGridAuditActions.DisplayLayout.GroupByBox.PromptAppearance = appearance5;
            this.ultraGridAuditActions.DisplayLayout.LoadStyle = Infragistics.Win.UltraWinGrid.LoadStyle.LoadOnDemand;
            this.ultraGridAuditActions.DisplayLayout.MaxColScrollRegions = 1;
            this.ultraGridAuditActions.DisplayLayout.MaxRowScrollRegions = 1;
            this.ultraGridAuditActions.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.ultraGridAuditActions.DisplayLayout.Override.AllowColSizing = Infragistics.Win.UltraWinGrid.AllowColSizing.Free;
            this.ultraGridAuditActions.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridAuditActions.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridAuditActions.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.ultraGridAuditActions.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            appearance6.BackColor = System.Drawing.SystemColors.Window;
            this.ultraGridAuditActions.DisplayLayout.Override.CardAreaAppearance = appearance6;
            appearance7.BorderColor = System.Drawing.Color.Silver;
            appearance7.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.ultraGridAuditActions.DisplayLayout.Override.CellAppearance = appearance7;
            this.ultraGridAuditActions.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.ultraGridAuditActions.DisplayLayout.Override.CellPadding = 0;
            this.ultraGridAuditActions.DisplayLayout.Override.DefaultRowHeight = 20;
            this.ultraGridAuditActions.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.ultraGridAuditActions.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance8.BackColor = System.Drawing.SystemColors.Control;
            appearance8.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance8.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance8.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance8.BorderColor = System.Drawing.SystemColors.Window;
            this.ultraGridAuditActions.DisplayLayout.Override.GroupByRowAppearance = appearance8;
            this.ultraGridAuditActions.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Collapsed;
            appearance9.TextHAlignAsString = "Left";
            this.ultraGridAuditActions.DisplayLayout.Override.HeaderAppearance = appearance9;
            this.ultraGridAuditActions.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortMulti;
            this.ultraGridAuditActions.DisplayLayout.Override.MaxSelectedRows = 1;
            appearance10.BackColor = System.Drawing.SystemColors.Window;
            appearance10.BorderColor = System.Drawing.Color.Silver;
            this.ultraGridAuditActions.DisplayLayout.Override.RowAppearance = appearance10;
            this.ultraGridAuditActions.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.ultraGridAuditActions.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridAuditActions.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridAuditActions.DisplayLayout.Override.SelectTypeGroupByRow = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.ultraGridAuditActions.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            appearance11.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ultraGridAuditActions.DisplayLayout.Override.TemplateAddRowAppearance = appearance11;
            this.ultraGridAuditActions.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.ultraGridAuditActions.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.ultraGridAuditActions.DisplayLayout.UseFixedHeaders = true;
            valueList1.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList1.Key = "Metrics";
            valueList1.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            appearance12.ImageHAlign = Infragistics.Win.HAlign.Center;
            valueList2.Appearance = appearance12;
            valueList2.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.Picture;
            valueList2.Key = "Severity";
            valueList2.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList2.SortStyle = Infragistics.Win.ValueListSortStyle.DescendingByValue;
            valueList3.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayTextAndPicture;
            valueList3.Key = "Transitions";
            valueList3.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueList3.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            valueList4.DisplayStyle = Infragistics.Win.ValueListDisplayStyle.DisplayText;
            valueList4.Key = "BooleanYesNo";
            valueList4.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem1.DataValue = false;
            valueListItem1.DisplayText = "No";
            valueListItem2.DataValue = true;
            valueListItem2.DisplayText = "Yes";
            valueList4.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2});
            this.ultraGridAuditActions.DisplayLayout.ValueLists.AddRange(new Infragistics.Win.ValueList[] {
            valueList1,
            valueList2,
            valueList3,
            valueList4});
            this.ultraGridAuditActions.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.ultraGridAuditActions.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.ultraGridAuditActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ultraGridAuditActions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ultraGridAuditActions.Location = new System.Drawing.Point(0, 0);
            this.ultraGridAuditActions.Name = "ultraGridAuditActions";
            this.ultraGridAuditActions.Size = new System.Drawing.Size(814, 174);
            this.ultraGridAuditActions.TabIndex = 1;
            this.ultraGridAuditActions.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.ultraGridAuditActions_AfterSelectChange);
            this.ultraGridAuditActions.AfterSortChange += new Infragistics.Win.UltraWinGrid.BandEventHandler(this.ultraGridAuditActions_AfterSortChange);
            this.ultraGridAuditActions.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ultraGridAuditActions_MouseDown);
            // 
            // auditedActionsViewDataSource
            // 
            this.auditedActionsViewDataSource.AllowAdd = false;
            this.auditedActionsViewDataSource.AllowDelete = false;
            ultraDataColumn1.DataType = typeof(long);
            ultraDataColumn2.DataType = typeof(int);
            ultraDataColumn4.DataType = typeof(System.DateTime);
            this.auditedActionsViewDataSource.Band.Columns.AddRange(new object[] {
            ultraDataColumn1,
            ultraDataColumn2,
            ultraDataColumn3,
            ultraDataColumn4,
            ultraDataColumn5,
            ultraDataColumn6,
            ultraDataColumn7,
            ultraDataColumn8,
            ultraDataColumn9,
            ultraDataColumn10});
            this.auditedActionsViewDataSource.KeyIndex = 0;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.panel2.Controls.Add(this.tableLayoutMain);
            this.panel2.Controls.Add(this.detailsHeaderStrip);
            this.panel2.Controls.Add(this.labelNoRowSelected);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(814, 224);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutMain
            // 
            this.tableLayoutMain.AutoSize = true;
            this.tableLayoutMain.ColumnCount = 2;
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutMain.Controls.Add(this.tableLayoutPanel1, 0, 0);
            this.tableLayoutMain.Controls.Add(this.textBoxBody, 0, 1);
            this.tableLayoutMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutMain.Location = new System.Drawing.Point(0, 19);
            this.tableLayoutMain.Name = "tableLayoutMain";
            this.tableLayoutMain.RowCount = 3;
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutMain.Size = new System.Drawing.Size(814, 205);
            this.tableLayoutMain.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutMain.SetColumnSpan(this.tableLayoutPanel1, 2);
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 270F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label6, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelAction, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelDate, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.labelWorkastation, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelWorkstationUser, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelSQLUser, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelName, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(808, 60);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Action:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoEllipsis = true;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Workstation:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoEllipsis = true;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 43);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Workstation User:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoEllipsis = true;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(371, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Date:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoEllipsis = true;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(371, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Repository User:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoEllipsis = true;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(371, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 5;
            this.label6.Text = "Object Name:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelAction
            // 
            this.labelAction.AutoEllipsis = true;
            this.labelAction.AutoSize = true;
            this.labelAction.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.Action", true));
            this.labelAction.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelAction.Location = new System.Drawing.Point(101, 0);
            this.labelAction.Name = "labelAction";
            this.labelAction.Size = new System.Drawing.Size(264, 20);
            this.labelAction.TabIndex = 6;
            this.labelAction.Text = "<unknown>";
            this.labelAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelDate
            // 
            this.labelDate.AutoEllipsis = true;
            this.labelDate.AutoSize = true;
            this.labelDate.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.DateTime", true));
            this.labelDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelDate.Location = new System.Drawing.Point(462, 0);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(343, 20);
            this.labelDate.TabIndex = 7;
            this.labelDate.Text = "<unknown>";
            this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelWorkastation
            // 
            this.labelWorkastation.AutoEllipsis = true;
            this.labelWorkastation.AutoSize = true;
            this.labelWorkastation.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.Workstation", true));
            this.labelWorkastation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWorkastation.Location = new System.Drawing.Point(101, 20);
            this.labelWorkastation.Name = "labelWorkastation";
            this.labelWorkastation.Size = new System.Drawing.Size(264, 20);
            this.labelWorkastation.TabIndex = 8;
            this.labelWorkastation.Text = "<unknown>";
            this.labelWorkastation.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelWorkstationUser
            // 
            this.labelWorkstationUser.AutoEllipsis = true;
            this.labelWorkstationUser.AutoSize = true;
            this.labelWorkstationUser.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.WorkstationUser", true));
            this.labelWorkstationUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelWorkstationUser.Location = new System.Drawing.Point(101, 40);
            this.labelWorkstationUser.Name = "labelWorkstationUser";
            this.labelWorkstationUser.Size = new System.Drawing.Size(264, 20);
            this.labelWorkstationUser.TabIndex = 9;
            this.labelWorkstationUser.Text = "<unknown>";
            this.labelWorkstationUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelSQLUser
            // 
            this.labelSQLUser.AutoEllipsis = true;
            this.labelSQLUser.AutoSize = true;
            this.labelSQLUser.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.SQLUser", true));
            this.labelSQLUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelSQLUser.Location = new System.Drawing.Point(462, 20);
            this.labelSQLUser.Name = "labelSQLUser";
            this.labelSQLUser.Size = new System.Drawing.Size(343, 20);
            this.labelSQLUser.TabIndex = 10;
            this.labelSQLUser.Text = "<unknown>";
            this.labelSQLUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelName
            // 
            this.labelName.AutoEllipsis = true;
            this.labelName.AutoSize = true;
            this.labelName.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.Name", true));
            this.labelName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelName.Location = new System.Drawing.Point(462, 40);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(343, 20);
            this.labelName.TabIndex = 11;
            this.labelName.Text = "<unknown>";
            this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBoxBody
            // 
            this.textBoxBody.BackColor = System.Drawing.Color.White;
            this.textBoxBody.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutMain.SetColumnSpan(this.textBoxBody, 2);
            this.textBoxBody.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.auditedActionsViewDataSource, "Band 0.MetaData", true));
            this.textBoxBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxBody.Location = new System.Drawing.Point(3, 69);
            this.textBoxBody.Multiline = true;
            this.textBoxBody.Name = "textBoxBody";
            this.textBoxBody.ReadOnly = true;
            this.textBoxBody.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxBody.Size = new System.Drawing.Size(808, 113);
            this.textBoxBody.TabIndex = 1;
            // 
            // detailsHeaderStrip
            // 
            this.detailsHeaderStrip.AutoSize = false;
            this.detailsHeaderStrip.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.detailsHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.detailsHeaderStrip.GripMargin = new System.Windows.Forms.Padding(0);
            this.detailsHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.detailsHeaderStrip.HotTrackEnabled = false;
            this.detailsHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.detailsHeaderStripLabel});
            this.detailsHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.detailsHeaderStrip.Name = "detailsHeaderStrip";
            this.detailsHeaderStrip.Padding = new System.Windows.Forms.Padding(0);
            this.detailsHeaderStrip.Size = new System.Drawing.Size(814, 19);
            this.detailsHeaderStrip.Style = Idera.SQLdm.DesktopClient.Controls.HeaderStripStyle.Small;
            this.detailsHeaderStrip.TabIndex = 0;
            this.detailsHeaderStrip.Text = "Details";
            // 
            // detailsHeaderStripLabel
            // 
            this.detailsHeaderStripLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.detailsHeaderStripLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.detailsHeaderStripLabel.Name = "detailsHeaderStripLabel";
            this.detailsHeaderStripLabel.Size = new System.Drawing.Size(46, 16);
            this.detailsHeaderStripLabel.Text = "Details";
            // 
            // labelNoRowSelected
            // 
            this.labelNoRowSelected.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNoRowSelected.Location = new System.Drawing.Point(0, 0);
            this.labelNoRowSelected.Name = "labelNoRowSelected";
            this.labelNoRowSelected.Size = new System.Drawing.Size(814, 224);
            this.labelNoRowSelected.TabIndex = 2;
            this.labelNoRowSelected.Text = "Select an action to view its details.";
            this.labelNoRowSelected.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(814, 0);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 402);
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(814, 0);
            // 
            // _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom
            // 
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.SystemColors.Control;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 402);
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Name = "_activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom";
            this._activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(814, 0);
            // 
            // contextMenuManager1
            // 
            this.contextMenuManager1.DesignerFlags = 1;
            this.contextMenuManager1.DockWithinContainer = this;
            popupMenuTool3.InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
            this.contextMenuManager1.MiniToolbar.NonInheritedTools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            popupMenuTool3,
            popupMenuTool1});
            this.contextMenuManager1.Office2007UICompatibility = false;
            this.contextMenuManager1.Ribbon.FileMenuStyle = Infragistics.Win.UltraWinToolbars.FileMenuStyle.None;
            ribbonTab1.Caption = "Home";
            ribbonGroup1.Caption = "Filters";
            ribbonGroup1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            comboBoxTool1});
            ribbonGroup2.Caption = "Groups";
            ribbonGroup2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool16,
            buttonTool17,
            buttonTool18});
            ribbonTab1.Groups.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonGroup[] {
            ribbonGroup1,
            ribbonGroup2});
            this.contextMenuManager1.Ribbon.NonInheritedRibbonTabs.AddRange(new Infragistics.Win.UltraWinToolbars.RibbonTab[] {
            ribbonTab1});
            this.contextMenuManager1.Ribbon.QuickAccessToolbar.Visible = false;
            this.contextMenuManager1.Ribbon.Visible = true;
            appearance15.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool2.SharedPropsInternal.AppearancesLarge.Appearance = appearance15;
            buttonTool2.SharedPropsInternal.Caption = "Print";
            buttonTool2.SharedPropsInternal.CustomizerCaption = "Print";
            appearance14.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool5.SharedPropsInternal.AppearancesLarge.Appearance = appearance14;
            buttonTool5.SharedPropsInternal.Caption = "PDF";
            buttonTool5.SharedPropsInternal.CustomizerCaption = "PDF";
            appearance13.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool6.SharedPropsInternal.AppearancesLarge.Appearance = appearance13;
            buttonTool6.SharedPropsInternal.Caption = "Excel";
            buttonTool6.SharedPropsInternal.CustomizerCaption = "Excel";
            popupMenuTool2.InstanceProps.Visible = Infragistics.Win.DefaultableBoolean.False;
            popupMenuTool2.SharedPropsInternal.Caption = "GridRowMenu";
            popupMenuTool2.SharedPropsInternal.CustomizerCaption = "GridRowMenu";
            buttonTool4.InstanceProps.IsFirstInGroup = true;
            buttonTool9.InstanceProps.IsFirstInGroup = true;
            popupMenuTool2.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool4,
            buttonTool14,
            buttonTool9,
            buttonTool7,
            buttonTool1});
            buttonTool8.SharedPropsInternal.Caption = "Print";
            buttonTool8.SharedPropsInternal.CustomizerCaption = "Print";
            appearance26.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Print16x16;
            buttonTool10.SharedPropsInternal.AppearancesSmall.Appearance = appearance26;
            buttonTool10.SharedPropsInternal.Caption = "Print";
            buttonTool10.SharedPropsInternal.CustomizerCaption = "PrintMenuOption";
            buttonTool10.SharedPropsInternal.DisplayStyle = Infragistics.Win.UltraWinToolbars.ToolDisplayStyle.DefaultForToolType;
            appearance27.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool12.SharedPropsInternal.AppearancesSmall.Appearance = appearance27;
            buttonTool12.SharedPropsInternal.Caption = "Export to Excel";
            buttonTool12.SharedPropsInternal.CustomizerCaption = "ExcelMenuOption";
            appearance28.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Export16x16;
            buttonTool13.SharedPropsInternal.AppearancesSmall.Appearance = appearance28;
            buttonTool13.SharedPropsInternal.Caption = "Export to PDF";
            buttonTool13.SharedPropsInternal.CustomizerCaption = "PDFMenuOption";
            appearance25.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            buttonTool3.SharedPropsInternal.AppearancesSmall.Appearance = appearance25;
            buttonTool3.SharedPropsInternal.Caption = "Copy To Clipboard";
            buttonTool3.SharedPropsInternal.CustomizerCaption = "CopyToClipboardMenuOption";
            popupControlContainerTool2.SharedPropsInternal.Caption = "PopupControlContainerTool1";
            buttonTool11.SharedPropsInternal.Caption = "Collapse All Groups";
            buttonTool11.SharedPropsInternal.CustomizerCaption = "CollapseAllGroupsMenuOption";
            buttonTool15.SharedPropsInternal.Caption = "Expand All Groups";
            buttonTool15.SharedPropsInternal.CustomizerCaption = "ExpandAllGroupsMenuOption";
            appearance29.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortAscending;
            buttonTool22.SharedPropsInternal.AppearancesSmall.Appearance = appearance29;
            buttonTool22.SharedPropsInternal.Caption = "Sort Ascending";
            buttonTool22.SharedPropsInternal.CustomizerCaption = "SortAscendingMenuOption";
            appearance30.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SortDescending;
            buttonTool23.SharedPropsInternal.AppearancesSmall.Appearance = appearance30;
            buttonTool23.SharedPropsInternal.Caption = "Sort Descending";
            buttonTool23.SharedPropsInternal.CustomizerCaption = "SortDescendingMenuOption";
            appearance32.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByBox;
            buttonTool25.SharedPropsInternal.AppearancesSmall.Appearance = appearance32;
            buttonTool25.SharedPropsInternal.Caption = "Group By Box";
            buttonTool25.SharedPropsInternal.CustomizerCaption = "GroupByBoxMenuOption";
            buttonTool26.SharedPropsInternal.Caption = "Remove This Column";
            buttonTool26.SharedPropsInternal.CustomizerCaption = "RemoveThisColumnMenuOption";
            appearance33.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.ColumnChooser;
            buttonTool27.SharedPropsInternal.AppearancesSmall.Appearance = appearance33;
            buttonTool27.SharedPropsInternal.Caption = "Column Chooser";
            buttonTool27.SharedPropsInternal.CustomizerCaption = "ColumnChooserMenuOption";
            popupMenuTool4.SharedPropsInternal.Caption = "ColumnContext";
            popupMenuTool4.SharedPropsInternal.CustomizerCaption = "ColumnContext";
            buttonTool37.InstanceProps.IsFirstInGroup = true;
            popupMenuTool4.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool28,
            buttonTool32,
            stateButtonTool1,
            buttonTool35,
            buttonTool36,
            buttonTool37});
            appearance34.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.GroupByThisColumn;
            stateButtonTool2.SharedPropsInternal.AppearancesSmall.Appearance = appearance34;
            stateButtonTool2.SharedPropsInternal.Caption = "Group By This Column";
            stateButtonTool2.SharedPropsInternal.CustomizerCaption = "GroupByThisColumnMenuOption";
            comboBoxTool2.SharedPropsInternal.Caption = "Period:";
            comboBoxTool2.SharedPropsInternal.CustomizerCaption = "TimeButton";
            valueList5.PreferredDropDownSize = new System.Drawing.Size(0, 0);
            valueListItem3.DataValue = "ValueListItem0";
            valueListItem3.DisplayText = "Anytime";
            valueListItem4.DataValue = "ValueListItem1";
            valueListItem4.DisplayText = "Today";
            valueListItem5.DataValue = "ValueListItem2";
            valueListItem5.DisplayText = "Last 7 Days";
            valueListItem6.DataValue = "ValueListItem3";
            valueListItem6.DisplayText = "Last 30 Days";
            valueListItem7.DataValue = "ValueListItem4";
            valueListItem7.DisplayText = "Custom Range";
            valueList5.ValueListItems.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem3,
            valueListItem4,
            valueListItem5,
            valueListItem6,
            valueListItem7});
            comboBoxTool2.ValueList = valueList5;
            labelTool2.SharedPropsInternal.Caption = "<Date Range>";
            labelTool2.SharedPropsInternal.CustomizerCaption = "DateRangeMenuLabel";
            this.contextMenuManager1.Tools.AddRange(new Infragistics.Win.UltraWinToolbars.ToolBase[] {
            buttonTool2,
            buttonTool5,
            buttonTool6,
            popupMenuTool2,
            buttonTool8,
            buttonTool10,
            buttonTool12,
            buttonTool13,
            buttonTool3,
            popupControlContainerTool2,
            buttonTool11,
            buttonTool15,
            buttonTool22,
            buttonTool23,
            buttonTool25,
            buttonTool26,
            buttonTool27,
            popupMenuTool4,
            stateButtonTool2,
            comboBoxTool2,
            labelTool2});
            this.contextMenuManager1.AfterToolCloseup += new Infragistics.Win.UltraWinToolbars.ToolDropdownEventHandler(this.contextMenuManager1_AfterToolCloseup);
            this.contextMenuManager1.BeforeToolDropdown += new Infragistics.Win.UltraWinToolbars.BeforeToolDropdownEventHandler(this.contextMenuManager1_BeforeToolDropdown);
            this.contextMenuManager1.ToolClick += new Infragistics.Win.UltraWinToolbars.ToolClickEventHandler(this.contextMenuManager1_ToolClick);
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_Left
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_Left.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_Left.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._AuditedActionsView_Toolbars_Dock_Area_Left.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Left;
            this._AuditedActionsView_Toolbars_Dock_Area_Left.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_Left.Location = new System.Drawing.Point(0, 127);
            this._AuditedActionsView_Toolbars_Dock_Area_Left.Name = "_AuditedActionsView_Toolbars_Dock_Area_Left";
            this._AuditedActionsView_Toolbars_Dock_Area_Left.Size = new System.Drawing.Size(0, 402);
            this._AuditedActionsView_Toolbars_Dock_Area_Left.ToolbarsManager = this.contextMenuManager1;
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_Right
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_Right.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_Right.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._AuditedActionsView_Toolbars_Dock_Area_Right.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Right;
            this._AuditedActionsView_Toolbars_Dock_Area_Right.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_Right.Location = new System.Drawing.Point(814, 127);
            this._AuditedActionsView_Toolbars_Dock_Area_Right.Name = "_AuditedActionsView_Toolbars_Dock_Area_Right";
            this._AuditedActionsView_Toolbars_Dock_Area_Right.Size = new System.Drawing.Size(0, 402);
            this._AuditedActionsView_Toolbars_Dock_Area_Right.ToolbarsManager = this.contextMenuManager1;
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_Top
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_Top.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_Top.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._AuditedActionsView_Toolbars_Dock_Area_Top.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Top;
            this._AuditedActionsView_Toolbars_Dock_Area_Top.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_Top.Location = new System.Drawing.Point(0, 0);
            this._AuditedActionsView_Toolbars_Dock_Area_Top.Name = "_AuditedActionsView_Toolbars_Dock_Area_Top";
            this._AuditedActionsView_Toolbars_Dock_Area_Top.Size = new System.Drawing.Size(814, 127);
            this._AuditedActionsView_Toolbars_Dock_Area_Top.ToolbarsManager = this.contextMenuManager1;
            // 
            // _AuditedActionsView_Toolbars_Dock_Area_Bottom
            // 
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(219)))), ((int)(((byte)(255)))));
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.DockedPosition = Infragistics.Win.UltraWinToolbars.DockedPosition.Bottom;
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.ForeColor = System.Drawing.SystemColors.ControlText;
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.Location = new System.Drawing.Point(0, 529);
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.Name = "_AuditedActionsView_Toolbars_Dock_Area_Bottom";
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.Size = new System.Drawing.Size(814, 0);
            this._AuditedActionsView_Toolbars_Dock_Area_Bottom.ToolbarsManager = this.contextMenuManager1;
            // 
            // ultraGridPrintDocument
            // 
            this.ultraGridPrintDocument.DocumentName = "Audited Events";
            this.ultraGridPrintDocument.Grid = this.ultraGridAuditActions;
            // 
            // ultraPrintPreviewDialog
            // 
            this.ultraPrintPreviewDialog.Document = this.ultraGridPrintDocument;
            this.ultraPrintPreviewDialog.Name = "ultraPrintPreviewDialog";
            // 
            // AuditedActionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.AuditedActionsView_Fill_Panel);
            this.Controls.Add(this._AuditedActionsView_Toolbars_Dock_Area_Left);
            this.Controls.Add(this._AuditedActionsView_Toolbars_Dock_Area_Right);
            this.Controls.Add(this._AuditedActionsView_Toolbars_Dock_Area_Top);
            this.Controls.Add(this._AuditedActionsView_Toolbars_Dock_Area_Bottom);
            this.Name = "AuditedActionsView";
            this.Size = new System.Drawing.Size(814, 529);
            this.AuditedActionsView_Fill_Panel.ClientArea.ResumeLayout(false);
            this.AuditedActionsView_Fill_Panel.ResumeLayout(false);
            this.activeAlerts_Fill_Panel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridAuditActions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.auditedActionsViewDataSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tableLayoutMain.ResumeLayout(false);
            this.tableLayoutMain.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.detailsHeaderStrip.ResumeLayout(false);
            this.detailsHeaderStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contextMenuManager1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_1_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_1_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_1_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_1_Bottom;
        private Infragistics.Win.Misc.UltraPanel AuditedActionsView_Fill_Panel;
        private Controls.ContextMenuManager contextMenuManager1;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _AuditedActionsView_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private Infragistics.Win.UltraWinGrid.ExcelExport.UltraGridExcelExporter ultraGridExcelExporter;
        private Infragistics.Win.UltraWinGrid.UltraGridPrintDocument ultraGridPrintDocument;
        private Infragistics.Win.Printing.UltraPrintPreviewDialog ultraPrintPreviewDialog;
        private System.Windows.Forms.Panel panel2;
        private Controls.HeaderStrip detailsHeaderStrip;
        private System.Windows.Forms.ToolStripLabel detailsHeaderStripLabel;
        private System.Windows.Forms.SplitContainer splitContainer;
        private Controls.GradientPanel activeAlerts_Fill_Panel;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Left;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Right;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Top;
        private Infragistics.Win.UltraWinToolbars.UltraToolbarsDockArea _activeAlerts_Fill_Panel_Toolbars_Dock_Area_Bottom;
        private System.Windows.Forms.TableLayoutPanel tableLayoutMain;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelAction;
        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.Label labelWorkastation;
        private System.Windows.Forms.Label labelWorkstationUser;
        private System.Windows.Forms.Label labelSQLUser;
        private System.Windows.Forms.Label labelName;
        private AuditedActionsViewDataSource auditedActionsViewDataSource;
        private System.Windows.Forms.TextBox textBoxBody;
        private System.Windows.Forms.Label labelNoRowSelected;
        private UltraGrid ultraGridAuditActions;
    }
}
