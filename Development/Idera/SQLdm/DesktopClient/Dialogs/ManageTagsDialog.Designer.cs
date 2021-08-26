using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ManageTagsDialog
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
            bool isDarkThemeSelected = Properties.Settings.Default.ColorScheme == "Dark";
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ManageTagsDialog));
            this.doneButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.getTagsWorker = new System.ComponentModel.BackgroundWorker();
            this.removeTagsWorker = new System.ComponentModel.BackgroundWorker();
            this.office2007PropertyPage1 = new Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage(isDarkThemeSelected);
            this.statusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.manageTagsMainContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.tagsListView = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.manageTagsButtonsContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.editButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.removeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.addButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.bottomNoteContainer = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.label1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.office2007PropertyPage1.ContentPanel.SuspendLayout();
            this.manageTagsMainContainer.SuspendLayout();
            this.manageTagsButtonsContainer.SuspendLayout();
            this.bottomNoteContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // doneButton
            // 
            this.doneButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.doneButton.AutoEllipsis = true;
            this.doneButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.doneButton.Location = new System.Drawing.Point(479, 473);
            this.doneButton.Name = "doneButton";
            this.doneButton.Size = new System.Drawing.Size(75, 23);
            this.doneButton.TabIndex = 0;
            this.doneButton.Text = "Done";
            this.doneButton.UseVisualStyleBackColor = true;
            // 
            // getTagsWorker
            // 
            this.getTagsWorker.WorkerSupportsCancellation = true;
            this.getTagsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.getTagsWorker_DoWork);
            this.getTagsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.getTagsWorker_RunWorkerCompleted);
            // 
            // removeTagsWorker
            // 
            this.removeTagsWorker.WorkerSupportsCancellation = true;
            this.removeTagsWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.removeTagsWorker_DoWork);
            this.removeTagsWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.removeTagsWorker_RunWorkerCompleted);
            // 
            // office2007PropertyPage1
            // 
            this.office2007PropertyPage1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.office2007PropertyPage1.BackColor = System.Drawing.Color.White;
            this.office2007PropertyPage1.BorderWidth = 1;
            // 
            // 
            // 
            this.office2007PropertyPage1.ContentPanel.BackColor2 = System.Drawing.Color.White;
            this.office2007PropertyPage1.ContentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.statusLabel);
            this.office2007PropertyPage1.ContentPanel.Controls.Add(this.manageTagsMainContainer);
            this.office2007PropertyPage1.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.office2007PropertyPage1.ContentPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.office2007PropertyPage1.ContentPanel.Location = new System.Drawing.Point(1, 56);
            this.office2007PropertyPage1.ContentPanel.Name = "ContentPanel";
            this.office2007PropertyPage1.ContentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.office2007PropertyPage1.ContentPanel.ShowBorder = false;
            this.office2007PropertyPage1.ContentPanel.Size = new System.Drawing.Size(540, 398);
            this.office2007PropertyPage1.ContentPanel.TabIndex = 1;
            this.office2007PropertyPage1.HeaderImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.Tag32x32;
            this.office2007PropertyPage1.Location = new System.Drawing.Point(12, 12);
            this.office2007PropertyPage1.Name = "office2007PropertyPage1";
            this.office2007PropertyPage1.Size = new System.Drawing.Size(542, 455);
            this.office2007PropertyPage1.TabIndex = 1;
            this.office2007PropertyPage1.Text = "Manage tags associated with SQLDM servers, custom counters and permissions.";
            // 
            // statusLabel
            // 
            this.statusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.statusLabel.Location = new System.Drawing.Point(17, 187);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(506, 16);
            this.statusLabel.TabIndex = 9;
            this.statusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // manageTagsMainContainer
            // 
            this.manageTagsMainContainer.ColumnCount = 3;
            this.manageTagsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.manageTagsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.manageTagsMainContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.manageTagsMainContainer.Controls.Add(this.label3, 0, 0);
            this.manageTagsMainContainer.Controls.Add(this.label4, 1, 1);
            this.manageTagsMainContainer.Controls.Add(this.tagsListView, 1, 2);
            this.manageTagsMainContainer.Controls.Add(this.manageTagsButtonsContainer, 1, 3);
            this.manageTagsMainContainer.Controls.Add(this.bottomNoteContainer, 1, 4);
            this.manageTagsMainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manageTagsMainContainer.Location = new System.Drawing.Point(1, 1);
            this.manageTagsMainContainer.Name = "manageTagsMainContainer";
            this.manageTagsMainContainer.RowCount = 5;
            this.manageTagsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.manageTagsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.manageTagsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.manageTagsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.manageTagsMainContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.manageTagsMainContainer.Size = new System.Drawing.Size(538, 396);
            this.manageTagsMainContainer.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.manageTagsMainContainer.SetColumnSpan(this.label3, 3);
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(532, 2);
            this.label3.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoEllipsis = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(13, 7);
            this.label4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(512, 65);
            this.label4.TabIndex = 8;
            this.label4.Text = resources.GetString("label4.Text");
            // 
            // tagsListView
            // 
            this.tagsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4});
            this.tagsListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tagsListView.FullRowSelect = true;
            this.tagsListView.HideSelection = false;
            this.tagsListView.Location = new System.Drawing.Point(13, 80);
            this.tagsListView.Name = "tagsListView";
            this.tagsListView.Size = new System.Drawing.Size(512, 241);
            this.tagsListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.tagsListView.TabIndex = 0;
            this.tagsListView.UseCompatibleStateImageBehavior = false;
            this.tagsListView.View = System.Windows.Forms.View.Details;
            this.tagsListView.SelectedIndexChanged += new System.EventHandler(this.tagsListView_SelectedIndexChanged);
            this.tagsListView.DoubleClick += new System.EventHandler(this.tagsListView_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Tag";
            this.columnHeader1.Width = 212;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Servers";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 100;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Custom Counters";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Permissions";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 100;
            // 
            // manageTagsButtonsContainer
            // 
            this.manageTagsButtonsContainer.Controls.Add(this.editButton);
            this.manageTagsButtonsContainer.Controls.Add(this.removeButton);
            this.manageTagsButtonsContainer.Controls.Add(this.addButton);
            this.manageTagsButtonsContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.manageTagsButtonsContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.manageTagsButtonsContainer.Location = new System.Drawing.Point(13, 327);
            this.manageTagsButtonsContainer.Name = "manageTagsButtonsContainer";
            this.manageTagsButtonsContainer.Size = new System.Drawing.Size(512, 35);
            this.manageTagsButtonsContainer.TabIndex = 2;
            // 
            // editButton
            // 
            this.editButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.editButton.AutoEllipsis = true;
            this.editButton.BackColor = System.Drawing.SystemColors.Control;
            this.editButton.Enabled = false;
            this.editButton.Location = new System.Drawing.Point(434, 3);
            this.editButton.Name = "editButton";
            this.editButton.Size = new System.Drawing.Size(75, 25);
            this.editButton.TabIndex = 2;
            this.editButton.Text = "Edit";
            this.editButton.UseVisualStyleBackColor = false;
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.AutoEllipsis = true;
            this.removeButton.BackColor = System.Drawing.SystemColors.Control;
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(353, 3);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(75, 25);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "Remove";
            this.removeButton.UseVisualStyleBackColor = false;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.AutoEllipsis = true;
            this.addButton.BackColor = System.Drawing.SystemColors.Control;
            this.addButton.Location = new System.Drawing.Point(272, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 25);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // bottomNoteContainer
            // 
            this.bottomNoteContainer.Controls.Add(this.label1);
            this.bottomNoteContainer.Controls.Add(this.label2);
            this.bottomNoteContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bottomNoteContainer.Location = new System.Drawing.Point(13, 368);
            this.bottomNoteContainer.Name = "bottomNoteContainer";
            this.bottomNoteContainer.Size = new System.Drawing.Size(512, 25);
            this.bottomNoteContainer.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Note:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(47, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(294, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Removing a tag will not delete items associated with that tag.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ManageTagsDialog
            // 
            this.AcceptButton = this.doneButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(566, 508);
            this.Controls.Add(this.office2007PropertyPage1);
            this.Controls.Add(this.doneButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageTagsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Tags";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.ManageTagsDialog_HelpButtonClicked);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ManageTagsDialog_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ManageTagsDialog_HelpRequested);
            this.office2007PropertyPage1.ContentPanel.ResumeLayout(false);
            this.manageTagsMainContainer.ResumeLayout(false);
            this.manageTagsButtonsContainer.ResumeLayout(false);
            this.bottomNoteContainer.ResumeLayout(false);
            this.bottomNoteContainer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton doneButton;
        private Idera.SQLdm.DesktopClient.Controls.Office2007PropertyPage office2007PropertyPage1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton removeButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton editButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton addButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomListView tagsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel statusLabel;
        private System.ComponentModel.BackgroundWorker getTagsWorker;
        private System.ComponentModel.BackgroundWorker removeTagsWorker;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel manageTagsButtonsContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel bottomNoteContainer;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel manageTagsMainContainer;
    }
}