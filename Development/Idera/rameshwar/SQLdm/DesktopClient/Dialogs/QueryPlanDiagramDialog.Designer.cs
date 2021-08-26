using Idera.SQLdm.DesktopClient.Controls;
using System.Data;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class QueryPlanDiagramDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(QueryPlanDiagramDialog));
            
            this.QueryPlanTabControl = new System.Windows.Forms.TabControl();
            this.QueryPlanTab = new System.Windows.Forms.TabPage();
            this.zoomInBtn = new System.Windows.Forms.Button();
            this.zoomOutBtn = new System.Windows.Forms.Button();
            this.zoomToFitBtn = new System.Windows.Forms.Button();
            this.exportBtn = new System.Windows.Forms.Button();
            this.htmlDisplay = new System.Windows.Forms.WebBrowser();
            this.hiddenBrowser = new System.Windows.Forms.WebBrowser();
            this.xmlPlanTab = new System.Windows.Forms.TabPage();
            this.xmlExportBtn = new System.Windows.Forms.Button();
            this.SQLStatmentTab = new System.Windows.Forms.TabPage();
            this.sqlExportBtn = new System.Windows.Forms.Button();
            this.sqlEditor = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            this.xmlEditor = new ActiproSoftware.SyntaxEditor.SyntaxEditor();
            this.QueryColumns = new System.Windows.Forms.TabPage();
            this.dataSet1 = new System.Data.DataSet();
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("Band 0", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Database");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Schema");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn3 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Table");
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn4 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Column");
            ultraGridBand1.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.querySchema = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.QueryColumnBtn = new Button();
            this.DiagramExportFileDialog = new SaveFileDialog();
            this.CancelBtn = new System.Windows.Forms.Button();
            this.diagnoseQueryButton = new System.Windows.Forms.Button();
            this.QueryPlanTabControl.SuspendLayout();
            this.QueryPlanTab.SuspendLayout();
            this.xmlPlanTab.SuspendLayout();
            this.SQLStatmentTab.SuspendLayout();
            this.QueryColumns.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.querySchema)).BeginInit();
            this.SuspendLayout();
            // 
            // QueryPlanTabControl
            // 
            this.QueryPlanTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryPlanTabControl.Controls.Add(this.QueryPlanTab);
            this.QueryPlanTabControl.Controls.Add(this.xmlPlanTab);
            this.QueryPlanTabControl.Controls.Add(this.SQLStatmentTab);
            this.QueryPlanTabControl.Controls.Add(this.QueryColumns);
            this.QueryPlanTabControl.Location = new System.Drawing.Point(12, 12);
            this.QueryPlanTabControl.Margin = new System.Windows.Forms.Padding(19, 14, 12, 45);
            this.QueryPlanTabControl.Name = "QueryPlanTabControl";
            this.QueryPlanTabControl.SelectedIndex = 0;
            this.QueryPlanTabControl.Size = new System.Drawing.Size(894, 580);
            this.QueryPlanTabControl.TabIndex = 0;
            // 
            // QueryPlanTab
            // 
            this.QueryPlanTab.AutoScroll = true;
            this.QueryPlanTab.Controls.Add(this.zoomInBtn);
            this.QueryPlanTab.Controls.Add(this.zoomOutBtn);
            this.QueryPlanTab.Controls.Add(this.zoomToFitBtn);
            this.QueryPlanTab.Controls.Add(this.exportBtn);
            this.QueryPlanTab.Controls.Add(this.htmlDisplay);
            this.QueryPlanTab.Controls.Add(this.hiddenBrowser);
            this.QueryPlanTab.Location = new System.Drawing.Point(4, 24);
            this.QueryPlanTab.Margin = new System.Windows.Forms.Padding(19, 12, 12, 45);
            this.QueryPlanTab.Name = "QueryPlanTab";
            this.QueryPlanTab.Padding = new System.Windows.Forms.Padding(12, 45, 12, 12);
            this.QueryPlanTab.Size = new System.Drawing.Size(886, 552);
            this.QueryPlanTab.TabIndex = 0;
            this.QueryPlanTab.Text = "";
            this.QueryPlanTab.UseVisualStyleBackColor = true;
            // 
            // zoomInBtn
            // 
            this.zoomInBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomInBtn.Location = new System.Drawing.Point(535, 13);
            this.zoomInBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.zoomInBtn.Name = "zoomInBtn";
            this.zoomInBtn.Size = new System.Drawing.Size(80, 21);
            this.zoomInBtn.TabIndex = 1;
            this.zoomInBtn.Text = "Zoom In";
            this.zoomInBtn.UseVisualStyleBackColor = true;
            this.zoomInBtn.Click += new System.EventHandler(this.ZoomInBtn_Click);
            // 
            // zoomOutBtn
            // 
            this.zoomOutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomOutBtn.Location = new System.Drawing.Point(623, 13);
            this.zoomOutBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.zoomOutBtn.Name = "zoomOutBtn";
            this.zoomOutBtn.Size = new System.Drawing.Size(80, 21);
            this.zoomOutBtn.TabIndex = 2;
            this.zoomOutBtn.Text = "Zoom Out";
            this.zoomOutBtn.UseVisualStyleBackColor = true;
            this.zoomOutBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ZoomOutBtn_MouseClick);
            // 
            // zoomToFitBtn
            // 
            this.zoomToFitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.zoomToFitBtn.Location = new System.Drawing.Point(710, 13);
            this.zoomToFitBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.zoomToFitBtn.Name = "zoomToFitBtn";
            this.zoomToFitBtn.Size = new System.Drawing.Size(80, 21);
            this.zoomToFitBtn.TabIndex = 3;
            this.zoomToFitBtn.Text = "Zoom To Fit";
            this.zoomToFitBtn.UseVisualStyleBackColor = true;
            this.zoomToFitBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ZoomToFitBtn_MouseClick);
            // 
            // exportBtn
            // 
            this.exportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exportBtn.Location = new System.Drawing.Point(797, 13);
            this.exportBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.exportBtn.Name = "exportBtn";
            this.exportBtn.Size = new System.Drawing.Size(80, 21);
            this.exportBtn.TabIndex = 4;
            this.exportBtn.Text = "Export";
            this.exportBtn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.exportBtn.UseMnemonic = false;
            this.exportBtn.UseVisualStyleBackColor = false;
            this.exportBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.QueryDiagramExportBtn_MouseClick);
            // 
            // htmlDisplay
            // 
            this.htmlDisplay.AllowNavigation = false;
            this.htmlDisplay.AllowWebBrowserDrop = false;
            this.htmlDisplay.Dock = System.Windows.Forms.DockStyle.Fill;
            this.htmlDisplay.IsWebBrowserContextMenuEnabled = false;
            this.htmlDisplay.Location = new System.Drawing.Point(12, 45);
            this.htmlDisplay.Name = "htmlDisplay";
            this.htmlDisplay.ScriptErrorsSuppressed = false;
            this.htmlDisplay.Size = new System.Drawing.Size(862, 495);
            this.htmlDisplay.TabIndex = 5;
            this.htmlDisplay.ScrollBarsEnabled = true;
            // 
            // hiddenBrowser
            // 
            this.hiddenBrowser.Location = new System.Drawing.Point(446, 610);
            this.hiddenBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.hiddenBrowser.Name = "hiddenBrowser";
            this.hiddenBrowser.Size = new System.Drawing.Size(250, 250);
            this.hiddenBrowser.TabIndex = 2;
            this.hiddenBrowser.Visible = false;
            // 
            // xmlPlanTab
            // 
            this.xmlPlanTab.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.xmlPlanTab.Controls.Add(this.xmlExportBtn);
            this.xmlPlanTab.Controls.Add(this.xmlEditor);
            this.xmlPlanTab.Location = new System.Drawing.Point(4, 33);
            this.xmlPlanTab.Margin = new System.Windows.Forms.Padding(12, 45, 12, 12);
            this.xmlPlanTab.Name = "xmlPlanTab";
            this.xmlPlanTab.Padding = new System.Windows.Forms.Padding(12, 45, 12, 12);
            this.xmlPlanTab.Size = new System.Drawing.Size(886, 543);
            this.xmlPlanTab.TabIndex = 1;
            this.xmlPlanTab.Text = "";
            this.xmlPlanTab.UseVisualStyleBackColor = true;
            // 
            // xmlExportBtn
            // 
            this.xmlExportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.xmlExportBtn.Location = new System.Drawing.Point(795, 13);
            this.xmlExportBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.xmlExportBtn.Name = "xmlExportBtn";
            this.xmlExportBtn.Size = new System.Drawing.Size(80, 21);
            this.xmlExportBtn.TabIndex = 5;
            this.xmlExportBtn.Text = "Export";
            this.xmlExportBtn.UseVisualStyleBackColor = true;
            this.xmlExportBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.XmlExportBtn_MouseClick);
            // 
            // SQLStatmentTab
            // 
            this.SQLStatmentTab.AutoScrollMargin = new System.Drawing.Size(12, 12);
            this.SQLStatmentTab.Controls.Add(this.sqlExportBtn);
            this.SQLStatmentTab.Controls.Add(this.sqlEditor);
            this.SQLStatmentTab.Location = new System.Drawing.Point(4, 33);
            this.SQLStatmentTab.Margin = new System.Windows.Forms.Padding(12, 45, 12, 12);
            this.SQLStatmentTab.Name = "SQLStatmentTab";
            this.SQLStatmentTab.Padding = new System.Windows.Forms.Padding(12, 45, 12, 12);
            this.SQLStatmentTab.Size = new System.Drawing.Size(886, 543);
            this.SQLStatmentTab.TabIndex = 2;
            this.SQLStatmentTab.Text = "SQL Text";
            this.SQLStatmentTab.UseVisualStyleBackColor = true;
            // 
            // sqlExportBtn
            // 
            this.sqlExportBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlExportBtn.Location = new System.Drawing.Point(795, 13);
            this.sqlExportBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.sqlExportBtn.Name = "sqlExportBtn";
            this.sqlExportBtn.Size = new System.Drawing.Size(80, 21);
            this.sqlExportBtn.TabIndex = 5;
            this.sqlExportBtn.Text = "Export";
            this.sqlExportBtn.UseVisualStyleBackColor = true;
            this.sqlExportBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SqlExportBtn_MouseClick);
            
            ActiproSoftware.SyntaxEditor.Document sqlDocument = new ActiproSoftware.SyntaxEditor.Document();
            sqlDocument.ReadOnly = false;

            this.sqlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlEditor.Document = sqlDocument;
            this.sqlEditor.LineNumberMarginVisible = true;
            this.sqlEditor.Location = new System.Drawing.Point(36, 45);
            this.sqlEditor.Name = "sqlEditor";
            this.sqlEditor.Size = new System.Drawing.Size(838, 486);
            this.sqlEditor.TabIndex = 7;

            // 
            // QueryColumns
            // 
            this.QueryColumns.Controls.Add(this.querySchema);
            this.QueryColumns.Controls.Add(this.QueryColumnBtn);
            this.QueryColumns.Location = new System.Drawing.Point(4, 33);
            this.QueryColumns.Margin = new System.Windows.Forms.Padding(12);
            this.QueryColumns.Name = "QueryColumns";
            this.QueryColumns.Padding = new System.Windows.Forms.Padding(12, 45, 24, 12);
            this.QueryColumns.Size = new System.Drawing.Size(886, 543);
            this.QueryColumns.TabIndex = 3;
            this.QueryColumns.Text = "Query Columns";
            this.QueryColumns.UseVisualStyleBackColor = true;
            // 
            // querySchema
            // 
            this.querySchema.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                         | System.Windows.Forms.AnchorStyles.Left)
                         | System.Windows.Forms.AnchorStyles.Right)));
            
            ultraGridColumn1.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn1.Header.Caption = "";
            ultraGridColumn1.Header.VisiblePosition = 0;
            ultraGridColumn1.Width = 272;
            ultraGridColumn1.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            ultraGridColumn2.ExcludeFromColumnChooser = Infragistics.Win.UltraWinGrid.ExcludeFromColumnChooser.True;
            ultraGridColumn2.Header.VisiblePosition = 1;
            ultraGridColumn2.Header.Caption = "";
            ultraGridColumn2.Width = 195;
            ultraGridColumn2.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            ultraGridColumn3.Header.Caption = "";
            ultraGridColumn3.Header.VisiblePosition = 2;
            ultraGridColumn3.Width = 172;
            ultraGridColumn3.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            ultraGridColumn4.Header.VisiblePosition = 3;
            ultraGridColumn4.Header.Caption = "";
            ultraGridColumn4.Width = 221;
            ultraGridColumn4.CellAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2,
            ultraGridColumn3,
            ultraGridColumn4});
            this.querySchema.DisplayLayout.Override.HeaderAppearance.TextHAlign = Infragistics.Win.HAlign.Left;
            this.querySchema.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.querySchema.DisplayLayout.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.querySchema.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            this.querySchema.DisplayLayout.GroupByBox.Hidden = true;
            this.querySchema.DisplayLayout.MaxColScrollRegions = 1;
            this.querySchema.DisplayLayout.MaxRowScrollRegions = 10;
            this.querySchema.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ResizeAllColumns;
            this.querySchema.DisplayLayout.Bands[0].Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.querySchema.DisplayLayout.Bands[0].Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.querySchema.DisplayLayout.Bands[0].Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.querySchema.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.Solid;
            this.querySchema.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.Solid;
            this.querySchema.DisplayLayout.Override.CellPadding = 0;
            this.querySchema.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.querySchema.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            this.querySchema.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.querySchema.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.querySchema.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.querySchema.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Single;
            this.querySchema.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.querySchema.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.querySchema.DisplayLayout.UseFixedHeaders = false;
            this.querySchema.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.querySchema.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.querySchema.Font = new System.Drawing.Font("SegoeUI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.querySchema.Location = new System.Drawing.Point(0, 45);
            this.querySchema.Name = "tablesGrid";
            this.querySchema.Size = new System.Drawing.Size(870, 523);
            this.querySchema.TabIndex = 7;
            this.querySchema.DataSource = dataSet1;
            //
            // QueryColumnExportButton
            //
            this.QueryColumnBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.QueryColumnBtn.Location = new System.Drawing.Point(795, 13);
            this.QueryColumnBtn.Margin = new System.Windows.Forms.Padding(4, 12, 24, 12);
            this.QueryColumnBtn.Name = "QueryColumnBtn";
            this.QueryColumnBtn.Size = new System.Drawing.Size(80, 21);
            this.QueryColumnBtn.TabIndex = 5;
            this.QueryColumnBtn.Text = "Export";
            this.QueryColumnBtn.UseVisualStyleBackColor = true;
            this.QueryColumnBtn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.QueryExportColumnBtn_MouseClick);
            // 
            // DiagramExportFileDialog
            // 
            this.DiagramExportFileDialog.CheckFileExists = true;
            this.DiagramExportFileDialog.CreatePrompt = true;
            this.DiagramExportFileDialog.DefaultExt = "xls";
            this.DiagramExportFileDialog.FileName = "Query_Details";
            // 
            // CancelBtn
            // 
            this.CancelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelBtn.Location = new System.Drawing.Point(811, 610);
            this.CancelBtn.Margin = new System.Windows.Forms.Padding(12);
            this.CancelBtn.MinimumSize = new System.Drawing.Size(80, 21);
            this.CancelBtn.Name = "CancelBtn";
            this.CancelBtn.Size = new System.Drawing.Size(80, 21);
            this.CancelBtn.TabIndex = 1;
            this.CancelBtn.Text = "Cancel";
            this.CancelBtn.UseVisualStyleBackColor = true;
            this.CancelBtn.Click += new System.EventHandler(this.CancelBtn_Click);
            // 
            // diagnoseQueryButton
            // 
            this.diagnoseQueryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.diagnoseQueryButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.diagnoseQueryButton.Location = new System.Drawing.Point(688, 610);
            this.diagnoseQueryButton.Margin = new System.Windows.Forms.Padding(12);
            this.diagnoseQueryButton.MinimumSize = new System.Drawing.Size(80, 21);
            this.diagnoseQueryButton.Name = "diagnoseQueryButton";
            this.diagnoseQueryButton.Size = new System.Drawing.Size(112, 21);
            this.diagnoseQueryButton.TabIndex = 2;
            this.diagnoseQueryButton.Text = "Diagnose Query";
            this.diagnoseQueryButton.UseVisualStyleBackColor = true;
            this.diagnoseQueryButton.Visible = false;
            this.diagnoseQueryButton.Click += new System.EventHandler(this.diagnoseQueryButton_Click);

            ActiproSoftware.SyntaxEditor.Document xmlDocument = new ActiproSoftware.SyntaxEditor.Document();
            xmlDocument.ReadOnly = true;

            this.xmlEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.xmlEditor.Document = xmlDocument;
            this.xmlEditor.LineNumberMarginVisible = true;
            this.xmlEditor.Location = new System.Drawing.Point(36, 45);
            this.xmlEditor.Name = "xmlEditor";
            this.xmlEditor.Size = new System.Drawing.Size(838, 486);
            this.xmlEditor.TabIndex = 7;

            // 
            // QueryDiagram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelBtn;
            this.ClientSize = new System.Drawing.Size(918, 650);
            this.Controls.Add(this.CancelBtn);
            this.Controls.Add(this.diagnoseQueryButton);
            this.Controls.Add(this.QueryPlanTabControl);
            this.Font = new System.Drawing.Font("SegoeUI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("InstallationKit")));
            this.Location = new System.Drawing.Point(214, 79);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Query Diagram";
            this.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Query Details";
            this.QueryPlanTabControl.ResumeLayout(false);
            this.QueryPlanTab.ResumeLayout(false);
            this.xmlPlanTab.ResumeLayout(false);
            this.xmlPlanTab.PerformLayout();
            this.SQLStatmentTab.ResumeLayout(false);
            this.SQLStatmentTab.PerformLayout();
            this.QueryColumns.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.querySchema)).EndInit();
            this.ResumeLayout(false);
        }



        #endregion

        private System.Windows.Forms.TabControl QueryPlanTabControl;
        private System.Windows.Forms.TabPage QueryPlanTab;
        private System.Windows.Forms.TabPage xmlPlanTab;
        private System.Windows.Forms.TabPage SQLStatmentTab;
        private System.Windows.Forms.TabPage QueryColumns;
        private DataSet dataSet1;
        private System.Windows.Forms.Button zoomInBtn;
        private System.Windows.Forms.Button zoomOutBtn;
        private System.Windows.Forms.Button zoomToFitBtn;
        private System.Windows.Forms.Button exportBtn;
        private System.Windows.Forms.Button xmlExportBtn;
        private System.Windows.Forms.Button sqlExportBtn;
        private System.Windows.Forms.Button QueryColumnBtn;
        private SaveFileDialog DiagramExportFileDialog;
        private WebBrowser htmlDisplay;
        private WebBrowser hiddenBrowser;
        private Button CancelBtn;
        private Infragistics.Win.UltraWinGrid.UltraGrid querySchema;
        private ActiproSoftware.SyntaxEditor.SyntaxEditor xmlEditor;
        private ActiproSoftware.SyntaxEditor.SyntaxEditor sqlEditor;
        private Button diagnoseQueryButton;

    }
}