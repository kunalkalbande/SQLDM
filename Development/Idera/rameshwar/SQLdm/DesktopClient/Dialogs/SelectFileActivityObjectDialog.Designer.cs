namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectFileActivityObjectDialog
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
            this.buttonLayoutPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.contentLayoutPanel = new System.Windows.Forms.Panel();
            this.selectionTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.selectedGroupBox = new System.Windows.Forms.GroupBox();
            this.selectedObjectsListBox = new System.Windows.Forms.ListBox();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.addButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.availableGroupBox = new System.Windows.Forms.GroupBox();
            this.availableObjectsListBox = new System.Windows.Forms.ListBox();
            this.availableStatusLabel = new System.Windows.Forms.Label();
            this.buttonLayoutPanel.SuspendLayout();
            this.contentLayoutPanel.SuspendLayout();
            this.selectionTableLayoutPanel.SuspendLayout();
            this.selectedGroupBox.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.availableGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonLayoutPanel
            // 
            this.buttonLayoutPanel.Controls.Add(this.okButton);
            this.buttonLayoutPanel.Controls.Add(this.cancelButton);
            this.buttonLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonLayoutPanel.Location = new System.Drawing.Point(0, 324);
            this.buttonLayoutPanel.Name = "buttonLayoutPanel";
            this.buttonLayoutPanel.Size = new System.Drawing.Size(582, 38);
            this.buttonLayoutPanel.TabIndex = 0;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(414, 6);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(495, 6);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // infoLabel
            // 
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.ForeColor = System.Drawing.Color.Black;
            this.infoLabel.Location = new System.Drawing.Point(3, 2);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(572, 26);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "< info >";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // contentLayoutPanel
            // 
            this.contentLayoutPanel.Controls.Add(this.selectionTableLayoutPanel);
            this.contentLayoutPanel.Controls.Add(this.infoLabel);
            this.contentLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.contentLayoutPanel.Name = "contentLayoutPanel";
            this.contentLayoutPanel.Size = new System.Drawing.Size(582, 362);
            this.contentLayoutPanel.TabIndex = 1;
            // 
            // selectionTableLayoutPanel
            // 
            this.selectionTableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectionTableLayoutPanel.ColumnCount = 3;
            this.selectionTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.selectionTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.selectionTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.selectionTableLayoutPanel.Controls.Add(this.selectedGroupBox, 2, 0);
            this.selectionTableLayoutPanel.Controls.Add(this.buttonPanel, 1, 0);
            this.selectionTableLayoutPanel.Controls.Add(this.availableGroupBox, 0, 0);
            this.selectionTableLayoutPanel.Location = new System.Drawing.Point(6, 31);
            this.selectionTableLayoutPanel.Name = "selectionTableLayoutPanel";
            this.selectionTableLayoutPanel.RowCount = 1;
            this.selectionTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.selectionTableLayoutPanel.Size = new System.Drawing.Size(569, 287);
            this.selectionTableLayoutPanel.TabIndex = 5;
            // 
            // selectedGroupBox
            // 
            this.selectedGroupBox.Controls.Add(this.selectedObjectsListBox);
            this.selectedGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.selectedGroupBox.Location = new System.Drawing.Point(337, 3);
            this.selectedGroupBox.Name = "selectedGroupBox";
            this.selectedGroupBox.Size = new System.Drawing.Size(229, 281);
            this.selectedGroupBox.TabIndex = 4;
            this.selectedGroupBox.TabStop = false;
            this.selectedGroupBox.Text = "Selected Objects";
            // 
            // selectedObjectsListBox
            // 
            this.selectedObjectsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.selectedObjectsListBox.DisplayMember = "Text";
            this.selectedObjectsListBox.FormattingEnabled = true;
            this.selectedObjectsListBox.HorizontalScrollbar = true;
            this.selectedObjectsListBox.Location = new System.Drawing.Point(12, 25);
            this.selectedObjectsListBox.Name = "selectedObjectsListBox";
            this.selectedObjectsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.selectedObjectsListBox.Size = new System.Drawing.Size(201, 238);
            this.selectedObjectsListBox.Sorted = true;
            this.selectedObjectsListBox.TabIndex = 0;
            this.selectedObjectsListBox.SelectedValueChanged += new System.EventHandler(this.selectedObjectsListBox_SelectedValueChanged);
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.addButton);
            this.buttonPanel.Controls.Add(this.removeButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonPanel.Location = new System.Drawing.Point(237, 3);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(94, 281);
            this.buttonPanel.TabIndex = 6;
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(10, 75);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "&Add >";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(10, 102);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "< &Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // availableGroupBox
            // 
            this.availableGroupBox.Controls.Add(this.availableStatusLabel);
            this.availableGroupBox.Controls.Add(this.availableObjectsListBox);
            this.availableGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.availableGroupBox.Location = new System.Drawing.Point(3, 3);
            this.availableGroupBox.Name = "availableGroupBox";
            this.availableGroupBox.Size = new System.Drawing.Size(228, 281);
            this.availableGroupBox.TabIndex = 1;
            this.availableGroupBox.TabStop = false;
            this.availableGroupBox.Text = "Available Objects";
            // 
            // availableObjectsListBox
            // 
            this.availableObjectsListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableObjectsListBox.DisplayMember = "Text";
            this.availableObjectsListBox.FormattingEnabled = true;
            this.availableObjectsListBox.HorizontalScrollbar = true;
            this.availableObjectsListBox.Location = new System.Drawing.Point(12, 25);
            this.availableObjectsListBox.Name = "availableObjectsListBox";
            this.availableObjectsListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableObjectsListBox.Size = new System.Drawing.Size(201, 238);
            this.availableObjectsListBox.Sorted = true;
            this.availableObjectsListBox.TabIndex = 1;
            this.availableObjectsListBox.SelectedValueChanged += new System.EventHandler(this.availableObjectsListBox_SelectedValueChanged);
            // 
            // availableStatusLabel
            // 
            this.availableStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.availableStatusLabel.BackColor = System.Drawing.Color.White;
            this.availableStatusLabel.Enabled = false;
            this.availableStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableStatusLabel.Location = new System.Drawing.Point(12, 25);
            this.availableStatusLabel.Name = "availableStatusLabel";
            this.availableStatusLabel.Size = new System.Drawing.Size(201, 238);
            this.availableStatusLabel.TabIndex = 0;
            this.availableStatusLabel.Text = "< Unavailable >";
            this.availableStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.availableStatusLabel.Visible = false;
            // 
            // SelectFileActivityObjectDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(582, 362);
            this.Controls.Add(this.buttonLayoutPanel);
            this.Controls.Add(this.contentLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "SelectFileActivityObjectDialog";
            this.Text = "Select Object";
            this.buttonLayoutPanel.ResumeLayout(false);
            this.contentLayoutPanel.ResumeLayout(false);
            this.selectionTableLayoutPanel.ResumeLayout(false);
            this.selectedGroupBox.ResumeLayout(false);
            this.buttonPanel.ResumeLayout(false);
            this.availableGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel buttonLayoutPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Panel contentLayoutPanel;
        private System.Windows.Forms.GroupBox selectedGroupBox;
        private System.Windows.Forms.ListBox selectedObjectsListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.GroupBox availableGroupBox;
        private System.Windows.Forms.Label availableStatusLabel;
        private System.Windows.Forms.ListBox availableObjectsListBox;
        private System.Windows.Forms.TableLayoutPanel selectionTableLayoutPanel;
        private System.Windows.Forms.Panel buttonPanel;
    }
}