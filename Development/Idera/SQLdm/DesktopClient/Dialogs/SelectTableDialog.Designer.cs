namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectTableDialog
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
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint8 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint4 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint5 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint7 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint6 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint1 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint2 = new Infragistics.Win.Layout.GridBagConstraint();
            Infragistics.Win.Layout.GridBagConstraint gridBagConstraint3 = new Infragistics.Win.Layout.GridBagConstraint();
            this.selectedTablesListBox = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.availableTablesListBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.availableTablesTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.ultraGridBagLayoutPanel1 = new Infragistics.Win.Misc.UltraGridBagLayoutPanel();
            this.availableTablesStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.availableTablesMessageLabel = new System.Windows.Forms.TextBox();
            this.databaseComboBox = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.panel5 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).BeginInit();
            this.ultraGridBagLayoutPanel1.SuspendLayout();
            this.availableTablesStackPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseComboBox)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectedTablesListBox
            // 
            this.selectedTablesListBox.FormattingEnabled = true;
            gridBagConstraint8.Anchor = Infragistics.Win.Layout.AnchorType.TopLeft;
            gridBagConstraint8.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint8.Insets.Top = 2;
            gridBagConstraint8.OriginX = 2;
            gridBagConstraint8.OriginY = 1;
            gridBagConstraint8.SpanY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.selectedTablesListBox, gridBagConstraint8);
            this.selectedTablesListBox.Location = new System.Drawing.Point(274, 21);
            this.selectedTablesListBox.Name = "selectedTablesListBox";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.selectedTablesListBox, new System.Drawing.Size(264, 251));
            this.selectedTablesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedTablesListBox.Size = new System.Drawing.Size(256, 329);
            this.selectedTablesListBox.Sorted = true;
            this.selectedTablesListBox.TabIndex = 5;
            this.selectedTablesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.selectedTablesListBox_MouseDoubleClick);
            this.selectedTablesListBox.SelectedIndexChanged += new System.EventHandler(this.selectedTablesListBox_SelectedIndexChanged);
            // 
            // label3
            // 
            gridBagConstraint4.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint4.OriginX = 2;
            gridBagConstraint4.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label3, gridBagConstraint4);
            this.label3.Location = new System.Drawing.Point(274, 0);
            this.label3.Name = "label3";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label3, new System.Drawing.Size(264, 15));
            this.label3.Size = new System.Drawing.Size(256, 19);
            this.label3.TabIndex = 6;
            this.label3.Text = "Selected Tables:";
            // 
            // removeButton
            // 
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(7, 94);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(67, 24);
            this.removeButton.TabIndex = 1;
            this.removeButton.Text = "< &Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(7, 64);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(67, 24);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "&Add >";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // availableTablesListBox
            // 
            this.availableTablesListBox.FormattingEnabled = true;
            this.availableTablesListBox.Location = new System.Drawing.Point(0, 0);
            this.availableTablesListBox.Name = "availableTablesListBox";
            this.availableTablesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableTablesListBox.Size = new System.Drawing.Size(195, 251);
            this.availableTablesListBox.Sorted = true;
            this.availableTablesListBox.TabIndex = 4;
            this.availableTablesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableTablesListBox_MouseDoubleClick);
            this.availableTablesListBox.SelectedIndexChanged += new System.EventHandler(this.availableTablesListBox_SelectedIndexChanged);
            // 
            // label2
            // 
            gridBagConstraint5.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint5.OriginX = 0;
            gridBagConstraint5.OriginY = 2;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label2, gridBagConstraint5);
            this.label2.Location = new System.Drawing.Point(0, 46);
            this.label2.Name = "label2";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label2, new System.Drawing.Size(184, 17));
            this.label2.Size = new System.Drawing.Size(195, 22);
            this.label2.TabIndex = 2;
            this.label2.Text = "Select a database to load it\'s tables:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // availableTablesTextBox
            // 
            this.availableTablesTextBox.ForeColor = System.Drawing.SystemColors.GrayText;
            gridBagConstraint7.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint7.OriginX = 0;
            gridBagConstraint7.OriginY = 1;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.availableTablesTextBox, gridBagConstraint7);
            this.availableTablesTextBox.Location = new System.Drawing.Point(0, 19);
            this.availableTablesTextBox.Name = "availableTablesTextBox";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.availableTablesTextBox, new System.Drawing.Size(184, 20));
            this.availableTablesTextBox.Size = new System.Drawing.Size(195, 20);
            this.availableTablesTextBox.TabIndex = 1;
            this.availableTablesTextBox.Text = "< Type comma separated names >";
            this.availableTablesTextBox.TextChanged += new System.EventHandler(this.availableTablesTextBox_TextChanged);
            this.availableTablesTextBox.Leave += new System.EventHandler(this.availableTablesTextBox_Leave);
            this.availableTablesTextBox.Enter += new System.EventHandler(this.availableTablesTextBox_Enter);
            // 
            // label1
            // 
            gridBagConstraint6.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint6.OriginX = 0;
            gridBagConstraint6.OriginY = 0;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.label1, gridBagConstraint6);
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.label1, new System.Drawing.Size(184, 15));
            this.label1.Size = new System.Drawing.Size(195, 19);
            this.label1.TabIndex = 0;
            this.label1.Text = "Available Tables:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(386, 370);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(467, 370);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ultraGridBagLayoutPanel1
            // 
            this.ultraGridBagLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.ultraGridBagLayoutPanel1.Controls.Add(this.availableTablesStackPanel);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.databaseComboBox);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.panel5);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label3);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.selectedTablesListBox);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label2);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.label1);
            this.ultraGridBagLayoutPanel1.Controls.Add(this.availableTablesTextBox);
            this.ultraGridBagLayoutPanel1.ExpandToFitHeight = true;
            this.ultraGridBagLayoutPanel1.ExpandToFitWidth = true;
            this.ultraGridBagLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.ultraGridBagLayoutPanel1.Name = "ultraGridBagLayoutPanel1";
            this.ultraGridBagLayoutPanel1.Size = new System.Drawing.Size(530, 355);
            this.ultraGridBagLayoutPanel1.TabIndex = 4;
            // 
            // availableTablesStackPanel
            // 
            this.availableTablesStackPanel.ActiveControl = this.availableTablesMessageLabel;
            this.availableTablesStackPanel.Controls.Add(this.availableTablesMessageLabel);
            this.availableTablesStackPanel.Controls.Add(this.availableTablesListBox);
            gridBagConstraint1.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint1.OriginX = 0;
            gridBagConstraint1.OriginY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.availableTablesStackPanel, gridBagConstraint1);
            this.availableTablesStackPanel.Location = new System.Drawing.Point(0, 96);
            this.availableTablesStackPanel.Name = "availableTablesStackPanel";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.availableTablesStackPanel, new System.Drawing.Size(200, 100));
            this.availableTablesStackPanel.Size = new System.Drawing.Size(195, 259);
            this.availableTablesStackPanel.TabIndex = 9;
            // 
            // availableTablesMessageLabel
            // 
            this.availableTablesMessageLabel.BackColor = System.Drawing.Color.White;
            this.availableTablesMessageLabel.Location = new System.Drawing.Point(0, 0);
            this.availableTablesMessageLabel.Multiline = true;
            this.availableTablesMessageLabel.Name = "availableTablesMessageLabel";
            this.availableTablesMessageLabel.ReadOnly = true;
            this.availableTablesMessageLabel.Size = new System.Drawing.Size(195, 259);
            this.availableTablesMessageLabel.TabIndex = 5;
            this.availableTablesMessageLabel.Text = "Please wait...";
            this.availableTablesMessageLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // databaseComboBox
            // 
            this.databaseComboBox.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            this.databaseComboBox.Enabled = false;
            gridBagConstraint2.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint2.OriginX = 0;
            gridBagConstraint2.OriginY = 3;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.databaseComboBox, gridBagConstraint2);
            this.databaseComboBox.Location = new System.Drawing.Point(0, 68);
            this.databaseComboBox.Name = "databaseComboBox";
            this.databaseComboBox.NullText = "Loading databases...";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.databaseComboBox, new System.Drawing.Size(144, 21));
            this.databaseComboBox.Size = new System.Drawing.Size(195, 21);
            this.databaseComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.databaseComboBox.TabIndex = 8;
            this.databaseComboBox.SelectionChanged += new System.EventHandler(this.databaseComboBox_SelectionChanged);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.addButton);
            this.panel5.Controls.Add(this.removeButton);
            gridBagConstraint3.Fill = Infragistics.Win.Layout.FillType.Both;
            gridBagConstraint3.OriginX = 1;
            gridBagConstraint3.OriginY = 4;
            this.ultraGridBagLayoutPanel1.SetGridBagConstraint(this.panel5, gridBagConstraint3);
            this.panel5.Location = new System.Drawing.Point(195, 96);
            this.panel5.Name = "panel5";
            this.ultraGridBagLayoutPanel1.SetPreferredSize(this.panel5, new System.Drawing.Size(82, 100));
            this.panel5.Size = new System.Drawing.Size(79, 259);
            this.panel5.TabIndex = 7;
            // 
            // SelectTableDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(554, 402);
            this.Controls.Add(this.ultraGridBagLayoutPanel1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectTableDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Excluded Tables";
            this.Load += new System.EventHandler(this.SelectTableDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ultraGridBagLayoutPanel1)).EndInit();
            this.ultraGridBagLayoutPanel1.ResumeLayout(false);
            this.ultraGridBagLayoutPanel1.PerformLayout();
            this.availableTablesStackPanel.ResumeLayout(false);
            this.availableTablesStackPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.databaseComboBox)).EndInit();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox availableTablesTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox availableTablesListBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.ListBox selectedTablesListBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Infragistics.Win.Misc.UltraGridBagLayoutPanel ultraGridBagLayoutPanel1;
        private System.Windows.Forms.Panel panel5;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor databaseComboBox;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel availableTablesStackPanel;
        private System.Windows.Forms.TextBox availableTablesMessageLabel;
    }
}