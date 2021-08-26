namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using Idera.SQLdm.Common.Objects;

    partial class SelectJobDialog
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
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tableLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.loadButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.serverComboBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomUltraComboEditor();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.contentStackPanel = new Idera.SQLdm.Common.UI.Controls.StackLayoutPanel();
            this.instructionContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.noJobsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.informationBox1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox();
            this.jobContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.jobListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this.waitingContentPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.jobNameLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.jobPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.tableLayoutPanel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serverComboBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.panel2.SuspendLayout();
            this.contentStackPanel.SuspendLayout();
            this.instructionContentPanel.SuspendLayout();
            this.jobContentPanel.SuspendLayout();
            this.waitingContentPanel.SuspendLayout();
            this.jobPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Instance:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.loadButton, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.serverComboBox, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(420, 38);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // loadButton
            // 
            this.loadButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.loadButton.AutoEllipsis = true;
            this.loadButton.AutoSize = true;
            this.loadButton.Location = new System.Drawing.Point(360, 7);
            this.loadButton.Name = "loadButton";
            this.loadButton.Size = new System.Drawing.Size(57, 23);
            this.loadButton.TabIndex = 3;
            this.loadButton.Text = "Load";
            this.loadButton.UseVisualStyleBackColor = true;
            this.loadButton.Click += new System.EventHandler(this.loadButton_Click);
            // 
            // serverComboBox
            // 
            this.serverComboBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.serverComboBox.DataSource = this.bindingSource;
            this.serverComboBox.DisplayMember = "InstanceName";
            this.serverComboBox.LimitToList = true;
            this.serverComboBox.Location = new System.Drawing.Point(203, 8);
            this.serverComboBox.Name = "serverComboBox";
            this.serverComboBox.NullText = "< Select an instance >";
            this.serverComboBox.Size = new System.Drawing.Size(290, 21);
            this.serverComboBox.SortStyle = Infragistics.Win.ValueListSortStyle.Ascending;
            this.serverComboBox.TabIndex = 2;
            this.serverComboBox.ValueMember = "Id";
            this.serverComboBox.SelectionChanged += new System.EventHandler(this.serverComboBox_SelectionChanged);
            // 
            // bindingSource
            // 
            this.bindingSource.AllowNew = false;
            this.bindingSource.DataSource = typeof(Idera.SQLdm.Common.Objects.MonitoredSqlServer);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cancelButton);
            this.panel2.Controls.Add(this.okButton);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(7, 297);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(420, 45);
            this.panel2.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.AutoSize = true;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(338, 14);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.AutoSize = true;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(257, 14);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // contentStackPanel
            // 
            this.contentStackPanel.ActiveControl = this.instructionContentPanel;
            this.contentStackPanel.Controls.Add(this.instructionContentPanel);
            this.contentStackPanel.Controls.Add(this.jobContentPanel);
            this.contentStackPanel.Controls.Add(this.waitingContentPanel);
            this.contentStackPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentStackPanel.Location = new System.Drawing.Point(7, 76);
            this.contentStackPanel.Name = "contentStackPanel";
            this.contentStackPanel.Size = new System.Drawing.Size(420, 221);
            this.contentStackPanel.TabIndex = 4;
            // 
            // instructionContentPanel
            // 
            this.instructionContentPanel.Controls.Add(this.noJobsLabel);
            this.instructionContentPanel.Controls.Add(this.informationBox1);
            this.instructionContentPanel.Location = new System.Drawing.Point(0, 0);
            this.instructionContentPanel.Name = "instructionContentPanel";
            this.instructionContentPanel.Padding = new System.Windows.Forms.Padding(7);
            this.instructionContentPanel.Size = new System.Drawing.Size(420, 221);
            this.instructionContentPanel.TabIndex = 2;
            // 
            // noJobsLabel
            // 
            this.noJobsLabel.AutoEllipsis = true;
            this.noJobsLabel.BackColor = System.Drawing.SystemColors.Control;
            this.noJobsLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.noJobsLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noJobsLabel.ForeColor = System.Drawing.Color.Red;
            this.noJobsLabel.Location = new System.Drawing.Point(7, 7);
            this.noJobsLabel.Name = "noJobsLabel";
            this.noJobsLabel.Size = new System.Drawing.Size(406, 65);
            this.noJobsLabel.TabIndex = 1;
            this.noJobsLabel.Text = "No job steps found on \'{0}\'.";
            this.noJobsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.noJobsLabel.Visible = false;
            // 
            // informationBox1
            // 
            this.informationBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.informationBox1.Location = new System.Drawing.Point(13, 74);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(397, 139);
            this.informationBox1.TabIndex = 0;
            this.informationBox1.Text = "Select an instance from the list and click the load button to retrieve job inform" +
                "ation.\r\n";
            // 
            // jobContentPanel
            // 
            this.jobContentPanel.Controls.Add(this.jobListBox);
            this.jobContentPanel.Location = new System.Drawing.Point(0, 0);
            this.jobContentPanel.Name = "jobContentPanel";
            this.jobContentPanel.Size = new System.Drawing.Size(322, 216);
            this.jobContentPanel.TabIndex = 1;
            // 
            // jobListBox
            // 
            this.jobListBox.CheckOnClick = true;
            this.jobListBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.jobListBox.FormattingEnabled = true;
            this.jobListBox.Location = new System.Drawing.Point(0, 0);
            this.jobListBox.Name = "jobListBox";
            this.jobListBox.Size = new System.Drawing.Size(322, 216);
            this.jobListBox.Sorted = true;
            this.jobListBox.TabIndex = 1;
            this.jobListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.jobListBox_ItemCheck);
            // 
            // waitingContentPanel
            // 
            this.waitingContentPanel.Controls.Add(this.label2);
            this.waitingContentPanel.Controls.Add(this.progressBar1);
            this.waitingContentPanel.Location = new System.Drawing.Point(0, 0);
            this.waitingContentPanel.MinimumSize = new System.Drawing.Size(300, 60);
            this.waitingContentPanel.Name = "waitingContentPanel";
            this.waitingContentPanel.Size = new System.Drawing.Size(336, 234);
            this.waitingContentPanel.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(16, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(306, 19);
            this.label2.TabIndex = 1;
            this.label2.Text = "Please wait...";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Enabled = false;
            this.progressBar1.Location = new System.Drawing.Point(16, 92);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(307, 21);
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 0;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.WorkerSupportsCancellation = true;
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Job name:";
            // 
            // jobNameLabel
            // 
            this.jobNameLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.jobNameLabel.Location = new System.Drawing.Point(68, 9);
            this.jobNameLabel.Name = "jobNameLabel";
            this.jobNameLabel.ReadOnly = true;
            this.jobNameLabel.Size = new System.Drawing.Size(345, 20);
            this.jobNameLabel.TabIndex = 3;
            this.jobNameLabel.Text = "{0}";
            // 
            // jobPanel
            // 
            this.jobPanel.Controls.Add(this.tableLayoutPanel2);
            this.jobPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.jobPanel.Location = new System.Drawing.Point(7, 38);
            this.jobPanel.Name = "jobPanel";
            this.jobPanel.Size = new System.Drawing.Size(420, 38);
            this.jobPanel.TabIndex = 5;
            this.jobPanel.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tableLayoutPanel1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(7, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(420, 38);
            this.panel1.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.jobNameLabel, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(420, 38);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // SelectJobDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(434, 342);
            this.Controls.Add(this.contentStackPanel);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.jobPanel);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelectJobDialog";
            this.Padding = new System.Windows.Forms.Padding(7, 0, 7, 0);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Job";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SelectJobDialog_FormClosing);
            this.Load += new System.EventHandler(this.SelectJobDialog_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.serverComboBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.contentStackPanel.ResumeLayout(false);
            this.instructionContentPanel.ResumeLayout(false);
            this.jobContentPanel.ResumeLayout(false);
            this.waitingContentPanel.ResumeLayout(false);
            this.jobPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor serverComboBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.Common.UI.Controls.StackLayoutPanel contentStackPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  waitingContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.BindingSource bindingSource;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private System.Windows.Forms.CheckedListBox jobListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  instructionContentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomInformationBox informationBox1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton loadButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel noJobsLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox jobNameLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  jobPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel tableLayoutPanel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
    }
}