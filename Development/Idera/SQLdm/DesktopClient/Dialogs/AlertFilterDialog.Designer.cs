namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class AlertFilterDialog
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
            this.okButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.cancelButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.excludePanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.excludeFilterTextBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.appendExcludeFiltersButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.panel3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.refreshProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.refreshLinkLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel();
            this.filterOptionsListView = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.refreshBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.panel1.SuspendLayout();
            this.excludePanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(325, 2);
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
            this.cancelButton.Location = new System.Drawing.Point(406, 2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 357);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 37);
            this.panel1.TabIndex = 2;
            // 
            // excludePanel
            // 
            this.excludePanel.Controls.Add(this.excludeFilterTextBox);
            this.excludePanel.Controls.Add(this.appendExcludeFiltersButton);
            this.excludePanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.excludePanel.Location = new System.Drawing.Point(0, 327);
            this.excludePanel.Name = "excludePanel";
            this.excludePanel.Size = new System.Drawing.Size(493, 30);
            this.excludePanel.TabIndex = 3;
            // 
            // excludeFilterTextBox
            // 
            this.excludeFilterTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.excludeFilterTextBox.Location = new System.Drawing.Point(93, 6);
            this.excludeFilterTextBox.Multiline = true;
            this.excludeFilterTextBox.Name = "excludeFilterTextBox";
            this.excludeFilterTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.excludeFilterTextBox.Size = new System.Drawing.Size(388, 20);
            this.excludeFilterTextBox.TabIndex = 1;
            this.excludeFilterTextBox.TextChanged += new System.EventHandler(this.excludeFilterTextBox_TextChanged);
            this.excludeFilterTextBox.Resize += new System.EventHandler(this.excludeFilterTextBox_Resize);
            // 
            // appendExcludeFiltersButton
            // 
            this.appendExcludeFiltersButton.Location = new System.Drawing.Point(12, 4);
            this.appendExcludeFiltersButton.Name = "appendExcludeFiltersButton";
            this.appendExcludeFiltersButton.Size = new System.Drawing.Size(75, 23);
            this.appendExcludeFiltersButton.TabIndex = 0;
            this.appendExcludeFiltersButton.Text = "Exclude ->";
            this.appendExcludeFiltersButton.UseVisualStyleBackColor = true;
            this.appendExcludeFiltersButton.Click += new System.EventHandler(this.appendExcludeFiltersButton_Click);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.refreshLinkLabel);
            this.panel3.Controls.Add(this.filterOptionsListView);
            this.panel3.Controls.Add(this.refreshProgressControl);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(493, 327);
            this.panel3.TabIndex = 4;
            // 
            // refreshProgressControl
            // 
            this.refreshProgressControl.Active = false;
            this.refreshProgressControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshProgressControl.BackColor = System.Drawing.Color.White;
            this.refreshProgressControl.Color = System.Drawing.Color.DarkGray;
            this.refreshProgressControl.InnerCircleRadius = 8;
            this.refreshProgressControl.Location = new System.Drawing.Point(13, 13);
            this.refreshProgressControl.Name = "refreshProgressControl";
            this.refreshProgressControl.NumberSpoke = 10;
            this.refreshProgressControl.OuterCircleRadius = 12;
            this.refreshProgressControl.RotationSpeed = 80;
            this.refreshProgressControl.Size = new System.Drawing.Size(467, 313);
            this.refreshProgressControl.SpokeThickness = 4;
            this.refreshProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.refreshProgressControl.TabIndex = 2;
            this.refreshProgressControl.Visible = false;
            // 
            // refreshLinkLabel
            // 
            this.refreshLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshLinkLabel.BackColor = System.Drawing.Color.White;
            this.refreshLinkLabel.Location = new System.Drawing.Point(13, 13);
            this.refreshLinkLabel.Name = "refreshLinkLabel";
            this.refreshLinkLabel.Size = new System.Drawing.Size(467, 313);
            this.refreshLinkLabel.TabIndex = 1;
            this.refreshLinkLabel.TabStop = true;
            this.refreshLinkLabel.Text = "Click here to refresh";
            this.refreshLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.refreshLinkLabel.Visible = false;
            this.refreshLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.refreshLinkLabel_LinkClicked);
            // 
            // filterOptionsListView
            // 
            this.filterOptionsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterOptionsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.filterOptionsListView.FullRowSelect = true;
            this.filterOptionsListView.HideSelection = false;
            this.filterOptionsListView.Location = new System.Drawing.Point(12, 12);
            this.filterOptionsListView.Name = "filterOptionsListView";
            this.filterOptionsListView.Size = new System.Drawing.Size(469, 315);
            this.filterOptionsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.filterOptionsListView.TabIndex = 0;
            this.filterOptionsListView.UseCompatibleStateImageBehavior = false;
            this.filterOptionsListView.View = System.Windows.Forms.View.Details;
            this.filterOptionsListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.filterOptionsListView_MouseDoubleClick);
            this.filterOptionsListView.Resize += new System.EventHandler(this.filterOptionsListView_Resize);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Name";
            this.columnHeader1.Width = 237;
            // 
            // refreshBackgroundWorker
            // 
            this.refreshBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshBackgroundWorker_DoWork);
            this.refreshBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshBackgroundWorker_RunWorkerCompleted);
            // 
            // AlertFilterDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.MinimumSize = new System.Drawing.Size(350, 350);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(493, 394);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.excludePanel);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AlertFilterDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select {0} to Filter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AlertFilterDialog_FormClosing);
            this.panel1.ResumeLayout(false);
            this.excludePanel.ResumeLayout(false);
            this.excludePanel.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton okButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton cancelButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  excludePanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox excludeFilterTextBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton appendExcludeFiltersButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListView filterOptionsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.ComponentModel.BackgroundWorker refreshBackgroundWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLinkLabel refreshLinkLabel;
        private MRG.Controls.UI.LoadingCircle refreshProgressControl;
    }
}