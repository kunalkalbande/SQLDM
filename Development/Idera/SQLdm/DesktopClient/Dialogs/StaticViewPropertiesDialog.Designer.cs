namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class StaticViewPropertiesDialog
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
            this.viewNameTextBox = new System.Windows.Forms.TextBox();
            this.viewNameLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.monitoredInstancesListBox = new System.Windows.Forms.ListBox();
            this.monitoredInstancesLabel = new System.Windows.Forms.Label();
            this.divider1 = new System.Windows.Forms.Label();
            this.viewInstancesLabel = new System.Windows.Forms.Label();
            this.viewInstancesListBox = new System.Windows.Forms.ListBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.addRemovePanel = new System.Windows.Forms.Panel();
            this.monitoredInstancesPanel = new System.Windows.Forms.Panel();
            this.monitoredInstancesStatusLabel = new System.Windows.Forms.Label();
            this.loadMonitoredInstancesBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1.SuspendLayout();
            this.addRemovePanel.SuspendLayout();
            this.monitoredInstancesPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // viewNameTextBox
            // 
            this.viewNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.viewNameTextBox.Location = new System.Drawing.Point(15, 25);
            this.viewNameTextBox.Name = "viewNameTextBox";
            this.viewNameTextBox.Size = new System.Drawing.Size(473, 20);
            this.viewNameTextBox.TabIndex = 0;
            // 
            // viewNameLabel
            // 
            this.viewNameLabel.AutoSize = true;
            this.viewNameLabel.Location = new System.Drawing.Point(15, 9);
            this.viewNameLabel.Name = "viewNameLabel";
            this.viewNameLabel.Size = new System.Drawing.Size(64, 13);
            this.viewNameLabel.TabIndex = 1;
            this.viewNameLabel.Text = "View Name:";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(332, 409);
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
            this.cancelButton.Location = new System.Drawing.Point(413, 409);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // monitoredInstancesListBox
            // 
            this.monitoredInstancesListBox.DisplayMember = "InstanceName";
            this.monitoredInstancesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitoredInstancesListBox.FormattingEnabled = true;
            this.monitoredInstancesListBox.HorizontalScrollbar = true;
            this.monitoredInstancesListBox.Location = new System.Drawing.Point(0, 0);
            this.monitoredInstancesListBox.Name = "monitoredInstancesListBox";
            this.monitoredInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.monitoredInstancesListBox.Size = new System.Drawing.Size(182, 316);
            this.monitoredInstancesListBox.Sorted = true;
            this.monitoredInstancesListBox.TabIndex = 1;
            this.monitoredInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.monitoredInstancesListBox_MouseDoubleClick);
            this.monitoredInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.monitoredInstancesListBox_SelectedIndexChanged);
            // 
            // monitoredInstancesLabel
            // 
            this.monitoredInstancesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.monitoredInstancesLabel.AutoSize = true;
            this.monitoredInstancesLabel.Location = new System.Drawing.Point(3, 7);
            this.monitoredInstancesLabel.Name = "monitoredInstancesLabel";
            this.monitoredInstancesLabel.Size = new System.Drawing.Size(106, 13);
            this.monitoredInstancesLabel.TabIndex = 7;
            this.monitoredInstancesLabel.Text = "Monitored Instances:";
            // 
            // divider1
            // 
            this.divider1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.divider1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.divider1.Location = new System.Drawing.Point(12, 399);
            this.divider1.Name = "divider1";
            this.divider1.Size = new System.Drawing.Size(476, 2);
            this.divider1.TabIndex = 8;
            // 
            // viewInstancesLabel
            // 
            this.viewInstancesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.viewInstancesLabel.AutoSize = true;
            this.viewInstancesLabel.Location = new System.Drawing.Point(291, 7);
            this.viewInstancesLabel.Name = "viewInstancesLabel";
            this.viewInstancesLabel.Size = new System.Drawing.Size(82, 13);
            this.viewInstancesLabel.TabIndex = 10;
            this.viewInstancesLabel.Text = "View Instances:";
            // 
            // viewInstancesListBox
            // 
            this.viewInstancesListBox.DisplayMember = "InstanceName";
            this.viewInstancesListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.viewInstancesListBox.FormattingEnabled = true;
            this.viewInstancesListBox.HorizontalScrollbar = true;
            this.viewInstancesListBox.Location = new System.Drawing.Point(291, 23);
            this.viewInstancesListBox.Name = "viewInstancesListBox";
            this.viewInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.viewInstancesListBox.Size = new System.Drawing.Size(182, 316);
            this.viewInstancesListBox.Sorted = true;
            this.viewInstancesListBox.TabIndex = 2;
            this.viewInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.groupInstancesListBox_MouseDoubleClick);
            this.viewInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.groupInstancesListBox_SelectedIndexChanged);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(10, 173);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 23);
            this.removeButton.TabIndex = 1;
            this.removeButton.Text = "< Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(10, 144);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "Add >";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.monitoredInstancesLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.viewInstancesLabel, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.viewInstancesListBox, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.addRemovePanel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.monitoredInstancesPanel, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 51);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(476, 345);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // addRemovePanel
            // 
            this.addRemovePanel.Controls.Add(this.removeButton);
            this.addRemovePanel.Controls.Add(this.addButton);
            this.addRemovePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.addRemovePanel.Location = new System.Drawing.Point(191, 3);
            this.addRemovePanel.Name = "addRemovePanel";
            this.tableLayoutPanel1.SetRowSpan(this.addRemovePanel, 2);
            this.addRemovePanel.Size = new System.Drawing.Size(94, 339);
            this.addRemovePanel.TabIndex = 1;
            // 
            // monitoredInstancesPanel
            // 
            this.monitoredInstancesPanel.Controls.Add(this.monitoredInstancesStatusLabel);
            this.monitoredInstancesPanel.Controls.Add(this.monitoredInstancesListBox);
            this.monitoredInstancesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.monitoredInstancesPanel.Location = new System.Drawing.Point(3, 23);
            this.monitoredInstancesPanel.Name = "monitoredInstancesPanel";
            this.monitoredInstancesPanel.Size = new System.Drawing.Size(182, 319);
            this.monitoredInstancesPanel.TabIndex = 0;
            // 
            // monitoredInstancesStatusLabel
            // 
            this.monitoredInstancesStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.monitoredInstancesStatusLabel.BackColor = System.Drawing.Color.White;
            this.monitoredInstancesStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.monitoredInstancesStatusLabel.Location = new System.Drawing.Point(2, 1);
            this.monitoredInstancesStatusLabel.Name = "monitoredInstancesStatusLabel";
            this.monitoredInstancesStatusLabel.Size = new System.Drawing.Size(177, 26);
            this.monitoredInstancesStatusLabel.TabIndex = 7;
            this.monitoredInstancesStatusLabel.Text = "< Unavailable >";
            this.monitoredInstancesStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.monitoredInstancesStatusLabel.Visible = false;
            // 
            // loadMonitoredInstancesBackgroundWorker
            // 
            this.loadMonitoredInstancesBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadMonitoredInstancesBackgroundWorker_DoWork);
            this.loadMonitoredInstancesBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadMonitoredInstancesBackgroundWorker_RunWorkerCompleted);
            // 
            // StaticViewPropertiesDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(500, 444);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.divider1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.viewNameLabel);
            this.Controls.Add(this.viewNameTextBox);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(430, 390);
            this.Name = "StaticViewPropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "My View - {0}";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.addRemovePanel.ResumeLayout(false);
            this.monitoredInstancesPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox viewNameTextBox;
        private System.Windows.Forms.Label viewNameLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListBox monitoredInstancesListBox;
        private System.Windows.Forms.Label monitoredInstancesLabel;
        private System.Windows.Forms.Label divider1;
        private System.Windows.Forms.Label viewInstancesLabel;
        private System.Windows.Forms.ListBox viewInstancesListBox;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel addRemovePanel;
        private System.ComponentModel.BackgroundWorker loadMonitoredInstancesBackgroundWorker;
        private System.Windows.Forms.Panel monitoredInstancesPanel;
        private System.Windows.Forms.Label monitoredInstancesStatusLabel;
    }
}