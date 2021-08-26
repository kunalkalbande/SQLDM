namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SelectMonitoredServersDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblHeader = new System.Windows.Forms.Label();
            this.availableInstancesStatusLabel = new System.Windows.Forms.Label();
            this.loadingServersProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.addedInstancesListBox = new System.Windows.Forms.ListBox();
            this.removeInstancesButton = new System.Windows.Forms.Button();
            this.addInstancesButton = new System.Windows.Forms.Button();
            this.availableInstancesLabel = new System.Windows.Forms.Label();
            this.addedInstancesLabel = new System.Windows.Forms.Label();
            this.availableInstancesListBox = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.loadServersWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label1.Location = new System.Drawing.Point(0, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(644, 1);
            this.label1.TabIndex = 19;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.ControlDark;
            this.label2.Location = new System.Drawing.Point(-1, 475);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(644, 1);
            this.label2.TabIndex = 20;
            this.label2.Text = "label2";
            // 
            // lblHeader
            // 
            this.lblHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblHeader.Location = new System.Drawing.Point(16, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(590, 32);
            this.lblHeader.TabIndex = 21;
            this.lblHeader.Text = "Available Servers:";
            // 
            // availableInstancesStatusLabel
            // 
            this.availableInstancesStatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.availableInstancesStatusLabel.BackColor = System.Drawing.Color.White;
            this.availableInstancesStatusLabel.Enabled = false;
            this.availableInstancesStatusLabel.ForeColor = System.Drawing.SystemColors.GrayText;
            this.availableInstancesStatusLabel.Location = new System.Drawing.Point(17, 87);
            this.availableInstancesStatusLabel.Name = "availableInstancesStatusLabel";
            this.availableInstancesStatusLabel.Size = new System.Drawing.Size(259, 362);
            this.availableInstancesStatusLabel.TabIndex = 30;
            this.availableInstancesStatusLabel.Text = "< Unavailable >";
            this.availableInstancesStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.availableInstancesStatusLabel.Visible = false;
            // 
            // loadingServersProgressControl
            // 
            this.loadingServersProgressControl.Active = false;
            this.loadingServersProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.loadingServersProgressControl.BackColor = System.Drawing.Color.White;
            this.loadingServersProgressControl.Color = System.Drawing.Color.DarkGray;
            this.loadingServersProgressControl.InnerCircleRadius = 5;
            this.loadingServersProgressControl.Location = new System.Drawing.Point(17, 86);
            this.loadingServersProgressControl.Name = "loadingServersProgressControl";
            this.loadingServersProgressControl.NumberSpoke = 12;
            this.loadingServersProgressControl.OuterCircleRadius = 11;
            this.loadingServersProgressControl.RotationSpeed = 80;
            this.loadingServersProgressControl.Size = new System.Drawing.Size(259, 363);
            this.loadingServersProgressControl.SpokeThickness = 2;
            this.loadingServersProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.loadingServersProgressControl.TabIndex = 27;
            this.loadingServersProgressControl.TabStop = false;
            // 
            // addedInstancesListBox
            // 
            this.addedInstancesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addedInstancesListBox.DisplayMember = "Text";
            this.addedInstancesListBox.FormattingEnabled = true;
            this.addedInstancesListBox.Location = new System.Drawing.Point(367, 83);
            this.addedInstancesListBox.Name = "addedInstancesListBox";
            this.addedInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.addedInstancesListBox.Size = new System.Drawing.Size(257, 368);
            this.addedInstancesListBox.Sorted = true;
            this.addedInstancesListBox.TabIndex = 29;
            this.addedInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.addedInstancesListBox_SelectedIndexChanged);
            this.addedInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.addedInstancesListBox_MouseDoubleClick);
            // 
            // removeInstancesButton
            // 
            this.removeInstancesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.removeInstancesButton.Enabled = false;
            this.removeInstancesButton.Location = new System.Drawing.Point(286, 265);
            this.removeInstancesButton.Name = "removeInstancesButton";
            this.removeInstancesButton.Size = new System.Drawing.Size(75, 23);
            this.removeInstancesButton.TabIndex = 24;
            this.removeInstancesButton.Text = "< &Remove";
            this.removeInstancesButton.UseVisualStyleBackColor = true;
            this.removeInstancesButton.Click += new System.EventHandler(this.removeInstancesButton_Click);
            // 
            // addInstancesButton
            // 
            this.addInstancesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.addInstancesButton.Enabled = false;
            this.addInstancesButton.Location = new System.Drawing.Point(286, 236);
            this.addInstancesButton.Name = "addInstancesButton";
            this.addInstancesButton.Size = new System.Drawing.Size(75, 23);
            this.addInstancesButton.TabIndex = 23;
            this.addInstancesButton.Text = "&Add >";
            this.addInstancesButton.UseVisualStyleBackColor = true;
            this.addInstancesButton.Click += new System.EventHandler(this.addInstancesButton_Click);
            // 
            // availableInstancesLabel
            // 
            this.availableInstancesLabel.AutoSize = true;
            this.availableInstancesLabel.Location = new System.Drawing.Point(19, 65);
            this.availableInstancesLabel.Name = "availableInstancesLabel";
            this.availableInstancesLabel.Size = new System.Drawing.Size(92, 13);
            this.availableInstancesLabel.TabIndex = 25;
            this.availableInstancesLabel.Text = "Available Servers:";
            // 
            // addedInstancesLabel
            // 
            this.addedInstancesLabel.AutoSize = true;
            this.addedInstancesLabel.Location = new System.Drawing.Point(364, 65);
            this.addedInstancesLabel.Name = "addedInstancesLabel";
            this.addedInstancesLabel.Size = new System.Drawing.Size(97, 13);
            this.addedInstancesLabel.TabIndex = 26;
            this.addedInstancesLabel.Text = "Added Servers: {0}";
            // 
            // availableInstancesListBox
            // 
            this.availableInstancesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.availableInstancesListBox.DisplayMember = "Text";
            this.availableInstancesListBox.FormattingEnabled = true;
            this.availableInstancesListBox.HorizontalScrollbar = true;
            this.availableInstancesListBox.Location = new System.Drawing.Point(16, 83);
            this.availableInstancesListBox.Name = "availableInstancesListBox";
            this.availableInstancesListBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.availableInstancesListBox.Size = new System.Drawing.Size(264, 368);
            this.availableInstancesListBox.Sorted = true;
            this.availableInstancesListBox.TabIndex = 28;
            this.availableInstancesListBox.SelectedIndexChanged += new System.EventHandler(this.availableInstancesListBox_SelectedIndexChanged);
            this.availableInstancesListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableInstancesListBox_MouseDoubleClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(549, 498);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 31;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Enabled = false;
            this.btnOK.Location = new System.Drawing.Point(468, 498);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 32;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // loadServersWorker
            // 
            this.loadServersWorker.WorkerSupportsCancellation = true;
            this.loadServersWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.loadServersWorker_DoWork);
            this.loadServersWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.loadServersWorker_RunWorkerCompleted);
            // 
            // SelectMonitoredServersDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(642, 541);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.availableInstancesStatusLabel);
            this.Controls.Add(this.loadingServersProgressControl);
            this.Controls.Add(this.addedInstancesListBox);
            this.Controls.Add(this.removeInstancesButton);
            this.Controls.Add(this.addInstancesButton);
            this.Controls.Add(this.availableInstancesLabel);
            this.Controls.Add(this.addedInstancesLabel);
            this.Controls.Add(this.availableInstancesListBox);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.HelpButton = true;
            this.Name = "SelectMonitoredServersDialog";
            this.Text = "Select Monitored SQL Server Instances";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.SelectMonitoredServersDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.SelectMonitoredServersDialog_HelpRequested);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Label availableInstancesStatusLabel;
        private MRG.Controls.UI.LoadingCircle loadingServersProgressControl;
        private System.Windows.Forms.ListBox addedInstancesListBox;
        private System.Windows.Forms.Button removeInstancesButton;
        private System.Windows.Forms.Button addInstancesButton;
        private System.Windows.Forms.Label availableInstancesLabel;
        private System.Windows.Forms.Label addedInstancesLabel;
        private System.Windows.Forms.ListBox availableInstancesListBox;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.ComponentModel.BackgroundWorker loadServersWorker;


    }
}