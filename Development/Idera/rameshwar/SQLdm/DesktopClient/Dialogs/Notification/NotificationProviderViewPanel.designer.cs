using Idera.SQLdm.Common.UI.Controls;
namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    partial class NotificationProviderViewPanel
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
            Infragistics.Win.UltraWinGrid.UltraGridBand ultraGridBand1 = new Infragistics.Win.UltraWinGrid.UltraGridBand("NotificationProviderWrapper", -1);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn1 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Name", -1, null, 0, Infragistics.Win.UltraWinGrid.SortIndicator.Ascending, false);
            Infragistics.Win.UltraWinGrid.UltraGridColumn ultraGridColumn2 = new Infragistics.Win.UltraWinGrid.UltraGridColumn("Enabled");
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance2 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance3 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance4 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance5 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance6 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance7 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance8 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance9 = new Infragistics.Win.Appearance();
            Infragistics.Win.Appearance appearance10 = new Infragistics.Win.Appearance();
            this.addButton = new System.Windows.Forms.Button();
            this.editButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.stackLayoutPanel1 = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.providerListView = new Infragistics.Win.UltraWinGrid.UltraGrid();
            this.noProvidersLabel = new System.Windows.Forms.Label();
            this.providerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.stackLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.providerListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addButton.Location = new System.Drawing.Point(3, 8);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.editButton.Location = new System.Drawing.Point(85, 8);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 23);
            this.editButton.TabIndex = 3;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = true;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeButton.Location = new System.Drawing.Point(170, 8);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 4;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.removeButton);
            this.panel1.Controls.Add(this.editButton);
            this.panel1.Controls.Add(this.addButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 450);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(444, 34);
            this.panel1.TabIndex = 6;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.stackLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(450, 487);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // stackLayoutPanel1
            // 
            this.stackLayoutPanel1.ActiveControl = null;
            this.stackLayoutPanel1.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.stackLayoutPanel1.Controls.Add(this.providerListView);
            this.stackLayoutPanel1.Controls.Add(this.noProvidersLabel);
            this.stackLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.stackLayoutPanel1.Name = "stackLayoutPanel1";
            this.stackLayoutPanel1.Padding = new System.Windows.Forms.Padding(1);
            this.stackLayoutPanel1.Size = new System.Drawing.Size(444, 441);
            this.stackLayoutPanel1.TabIndex = 5;
            // 
            // providerListView
            // 
            this.providerListView.DataSource = this.providerBindingSource;
            appearance1.BackColor = System.Drawing.SystemColors.Window;
            appearance1.BorderColor = System.Drawing.SystemColors.InactiveCaption;
            this.providerListView.DisplayLayout.Appearance = appearance1;
            this.providerListView.DisplayLayout.AutoFitStyle = Infragistics.Win.UltraWinGrid.AutoFitStyle.ExtendLastColumn;
            ultraGridColumn1.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn1.Header.VisiblePosition = 1;
            ultraGridColumn2.AutoCompleteMode = Infragistics.Win.AutoCompleteMode.Append;
            ultraGridColumn2.Header.VisiblePosition = 0;
            ultraGridBand1.Columns.AddRange(new object[] {
            ultraGridColumn1,
            ultraGridColumn2});
            this.providerListView.DisplayLayout.BandsSerializer.Add(ultraGridBand1);
            this.providerListView.DisplayLayout.CaptionVisible = Infragistics.Win.DefaultableBoolean.False;
            appearance2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(227)))), ((int)(((byte)(239)))), ((int)(((byte)(255)))));
            appearance2.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance2.BackGradientStyle = Infragistics.Win.GradientStyle.None;
            appearance2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(101)))), ((int)(((byte)(147)))), ((int)(((byte)(207)))));
            this.providerListView.DisplayLayout.GroupByBox.Appearance = appearance2;
            appearance3.ForeColor = System.Drawing.SystemColors.GrayText;
            this.providerListView.DisplayLayout.GroupByBox.BandLabelAppearance = appearance3;
            this.providerListView.DisplayLayout.GroupByBox.BorderStyle = Infragistics.Win.UIElementBorderStyle.Solid;
            this.providerListView.DisplayLayout.GroupByBox.Hidden = true;
            appearance4.BackColor = System.Drawing.SystemColors.ControlLightLight;
            appearance4.BackColor2 = System.Drawing.SystemColors.Control;
            appearance4.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance4.ForeColor = System.Drawing.SystemColors.GrayText;
            this.providerListView.DisplayLayout.GroupByBox.PromptAppearance = appearance4;
            this.providerListView.DisplayLayout.MaxColScrollRegions = 1;
            this.providerListView.DisplayLayout.MaxRowScrollRegions = 1;
            this.providerListView.DisplayLayout.Override.AllowAddNew = Infragistics.Win.UltraWinGrid.AllowAddNew.No;
            this.providerListView.DisplayLayout.Override.AllowDelete = Infragistics.Win.DefaultableBoolean.False;
            this.providerListView.DisplayLayout.Override.AllowUpdate = Infragistics.Win.DefaultableBoolean.False;
            this.providerListView.DisplayLayout.Override.BorderStyleCell = Infragistics.Win.UIElementBorderStyle.None;
            this.providerListView.DisplayLayout.Override.BorderStyleRow = Infragistics.Win.UIElementBorderStyle.None;
            appearance5.BackColor = System.Drawing.SystemColors.Window;
            this.providerListView.DisplayLayout.Override.CardAreaAppearance = appearance5;
            appearance6.BorderColor = System.Drawing.Color.Silver;
            appearance6.TextTrimming = Infragistics.Win.TextTrimming.EllipsisCharacter;
            this.providerListView.DisplayLayout.Override.CellAppearance = appearance6;
            this.providerListView.DisplayLayout.Override.CellClickAction = Infragistics.Win.UltraWinGrid.CellClickAction.RowSelect;
            this.providerListView.DisplayLayout.Override.CellPadding = 0;
            this.providerListView.DisplayLayout.Override.FixedHeaderIndicator = Infragistics.Win.UltraWinGrid.FixedHeaderIndicator.None;
            this.providerListView.DisplayLayout.Override.GroupByColumnsHidden = Infragistics.Win.DefaultableBoolean.False;
            appearance7.BackColor = System.Drawing.SystemColors.Control;
            appearance7.BackColor2 = System.Drawing.SystemColors.ControlDark;
            appearance7.BackGradientAlignment = Infragistics.Win.GradientAlignment.Element;
            appearance7.BackGradientStyle = Infragistics.Win.GradientStyle.Horizontal;
            appearance7.BorderColor = System.Drawing.SystemColors.Window;
            this.providerListView.DisplayLayout.Override.GroupByRowAppearance = appearance7;
            this.providerListView.DisplayLayout.Override.GroupByRowInitialExpansionState = Infragistics.Win.UltraWinGrid.GroupByRowInitialExpansionState.Expanded;
            appearance8.TextHAlignAsString = "Left";
            this.providerListView.DisplayLayout.Override.HeaderAppearance = appearance8;
            this.providerListView.DisplayLayout.Override.HeaderClickAction = Infragistics.Win.UltraWinGrid.HeaderClickAction.SortSingle;
            appearance9.BackColor = System.Drawing.SystemColors.Window;
            appearance9.BorderColor = System.Drawing.Color.Silver;
            this.providerListView.DisplayLayout.Override.RowAppearance = appearance9;
            this.providerListView.DisplayLayout.Override.RowSelectors = Infragistics.Win.DefaultableBoolean.False;
            this.providerListView.DisplayLayout.Override.SelectTypeCell = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.providerListView.DisplayLayout.Override.SelectTypeCol = Infragistics.Win.UltraWinGrid.SelectType.None;
            this.providerListView.DisplayLayout.Override.SelectTypeRow = Infragistics.Win.UltraWinGrid.SelectType.Extended;
            appearance10.BackColor = System.Drawing.SystemColors.ControlLight;
            this.providerListView.DisplayLayout.Override.TemplateAddRowAppearance = appearance10;
            this.providerListView.DisplayLayout.ScrollBounds = Infragistics.Win.UltraWinGrid.ScrollBounds.ScrollToFill;
            this.providerListView.DisplayLayout.ScrollStyle = Infragistics.Win.UltraWinGrid.ScrollStyle.Immediate;
            this.providerListView.DisplayLayout.UseFixedHeaders = true;
            this.providerListView.DisplayLayout.ViewStyle = Infragistics.Win.UltraWinGrid.ViewStyle.SingleBand;
            this.providerListView.DisplayLayout.ViewStyleBand = Infragistics.Win.UltraWinGrid.ViewStyleBand.OutlookGroupBy;
            this.providerListView.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.providerListView.Location = new System.Drawing.Point(1, 1);
            this.providerListView.Name = "providerListView";
            this.providerListView.Size = new System.Drawing.Size(448, 315);
            this.providerListView.TabIndex = 6;
            this.providerListView.UpdateMode = Infragistics.Win.UltraWinGrid.UpdateMode.OnCellChangeOrLostFocus;
            this.providerListView.Visible = false;
            this.providerListView.AfterSelectChange += new Infragistics.Win.UltraWinGrid.AfterSelectChangeEventHandler(this.providerListView_AfterSelectChange);
            this.providerListView.DoubleClickRow += new Infragistics.Win.UltraWinGrid.DoubleClickRowEventHandler(this.providerListView_DoubleClickRow);
            this.providerListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.providerListView_MouseClick);
            // 
            // noProvidersLabel
            // 
            this.noProvidersLabel.BackColor = System.Drawing.SystemColors.Window;
            this.noProvidersLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.noProvidersLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.noProvidersLabel.Location = new System.Drawing.Point(1, 1);
            this.noProvidersLabel.Name = "noProvidersLabel";
            this.noProvidersLabel.Size = new System.Drawing.Size(442, 439);
            this.noProvidersLabel.TabIndex = 2;
            this.noProvidersLabel.Text = "There are no {0}notification providers defined.  Click the Add button to create a" +
                " new instance.";
            this.noProvidersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // providerBindingSource
            // 
            this.providerBindingSource.DataSource = typeof(Idera.SQLdm.DesktopClient.Dialogs.Notification.NotificationProviderWrapper);
            // 
            // NotificationProviderViewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NotificationProviderViewPanel";
            this.Size = new System.Drawing.Size(450, 487);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.stackLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.providerListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.providerBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button removeButton;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel stackLayoutPanel1;
        private Infragistics.Win.UltraWinGrid.UltraGrid providerListView;
        private System.Windows.Forms.BindingSource providerBindingSource;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label noProvidersLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
