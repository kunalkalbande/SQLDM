namespace Idera.SQLdm.ImportWizard
{
    partial class ImportStatusForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportStatusForm));
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this._notifyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._mi_Cancel = new System.Windows.Forms.ToolStripMenuItem();
            this._mi_ToolStripSep = new System.Windows.Forms.ToolStripSeparator();
            this._mi_Help = new System.Windows.Forms.ToolStripMenuItem();
            this._mi_OpenImportStatus = new System.Windows.Forms.ToolStripMenuItem();
            this._btn_Cancel = new System.Windows.Forms.Button();
            this._btn_HideAndNotify = new System.Windows.Forms.Button();
            this._lbl_ImportStatus = new System.Windows.Forms.Label();
            this._infbx = new Divelements.WizardFramework.InformationBox();
            this._prgsBar = new System.Windows.Forms.ProgressBar();
            this._lbl_OverallProgress = new System.Windows.Forms.Label();
            this._rtb_ImportStatus = new System.Windows.Forms.RichTextBox();
            this._lbl_Sep = new System.Windows.Forms.Label();
            this._btn_Close = new System.Windows.Forms.Button();
            this._btn_Help = new System.Windows.Forms.Button();
            this._grpbx = new System.Windows.Forms.GroupBox();
            this._notifyContextMenu.SuspendLayout();
            this._grpbx.SuspendLayout();
            this.SuspendLayout();
            // 
            // _notifyIcon
            // 
            this._notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this._notifyIcon.BalloonTipText = "Double-click to see SQL diagnostic manager Import Status";
            this._notifyIcon.ContextMenuStrip = this._notifyContextMenu;
            this._notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("_notifyIcon.Icon")));
            this._notifyIcon.Text = "SQL diagnostic manager Import Wizard Status";
            this._notifyIcon.Visible = true;
            this._notifyIcon.DoubleClick += new System.EventHandler(this._notifyIcon_DoubleClick);
            this._notifyIcon.BalloonTipClicked += new System.EventHandler(this._notifyIcon_BalloonTipClicked);
            // 
            // _notifyContextMenu
            // 
            this._notifyContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._mi_Cancel,
            this._mi_ToolStripSep,
            this._mi_Help,
            this._mi_OpenImportStatus});
            this._notifyContextMenu.Name = "_notifyContextMenu";
            this._notifyContextMenu.ShowImageMargin = false;
            this._notifyContextMenu.Size = new System.Drawing.Size(354, 76);
            // 
            // _mi_Cancel
            // 
            this._mi_Cancel.Name = "_mi_Cancel";
            this._mi_Cancel.ShortcutKeyDisplayString = "";
            this._mi_Cancel.Size = new System.Drawing.Size(353, 22);
            this._mi_Cancel.Text = "&Cancel";
            this._mi_Cancel.Click += new System.EventHandler(this._mi_Cancel_Click);
            // 
            // _mi_ToolStripSep
            // 
            this._mi_ToolStripSep.Name = "_mi_ToolStripSep";
            this._mi_ToolStripSep.Size = new System.Drawing.Size(350, 6);
            // 
            // _mi_Help
            // 
            this._mi_Help.Name = "_mi_Help";
            this._mi_Help.Size = new System.Drawing.Size(353, 22);
            this._mi_Help.Text = "&Help";
            this._mi_Help.Click += new System.EventHandler(this._mi_Help_Click);
            // 
            // _mi_OpenImportStatus
            // 
            this._mi_OpenImportStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._mi_OpenImportStatus.Name = "_mi_OpenImportStatus";
            this._mi_OpenImportStatus.ShortcutKeyDisplayString = "";
            this._mi_OpenImportStatus.Size = new System.Drawing.Size(353, 22);
            this._mi_OpenImportStatus.Text = "&Open SQL diagnostic manager Import Wizard Status";
            this._mi_OpenImportStatus.Click += new System.EventHandler(this._mi_OpenImportStatus_Click);
            // 
            // _btn_Cancel
            // 
            this._btn_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_Cancel.Location = new System.Drawing.Point(393, 373);
            this._btn_Cancel.Name = "_btn_Cancel";
            this._btn_Cancel.Size = new System.Drawing.Size(75, 23);
            this._btn_Cancel.TabIndex = 3;
            this._btn_Cancel.Text = "&Cancel";
            this._btn_Cancel.UseVisualStyleBackColor = true;
            this._btn_Cancel.Click += new System.EventHandler(this._btn_Cancel_Click);
            // 
            // _btn_HideAndNotify
            // 
            this._btn_HideAndNotify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._btn_HideAndNotify.Location = new System.Drawing.Point(12, 373);
            this._btn_HideAndNotify.Name = "_btn_HideAndNotify";
            this._btn_HideAndNotify.Size = new System.Drawing.Size(176, 23);
            this._btn_HideAndNotify.TabIndex = 0;
            this._btn_HideAndNotify.Text = "Hide and &Notify When Complete";
            this._btn_HideAndNotify.UseVisualStyleBackColor = true;
            this._btn_HideAndNotify.Click += new System.EventHandler(this._btn_HideAndNotify_Click);
            // 
            // _lbl_ImportStatus
            // 
            this._lbl_ImportStatus.AutoSize = true;
            this._lbl_ImportStatus.Location = new System.Drawing.Point(6, 16);
            this._lbl_ImportStatus.Name = "_lbl_ImportStatus";
            this._lbl_ImportStatus.Size = new System.Drawing.Size(72, 13);
            this._lbl_ImportStatus.TabIndex = 2;
            this._lbl_ImportStatus.Text = "Import Status:";
            // 
            // _infbx
            // 
            this._infbx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._infbx.Location = new System.Drawing.Point(12, 12);
            this._infbx.Name = "_infbx";
            this._infbx.Size = new System.Drawing.Size(537, 43);
            this._infbx.TabIndex = 3;
            this._infbx.TabStop = false;
            this._infbx.Text = resources.GetString("_infbx.Text");
            // 
            // _prgsBar
            // 
            this._prgsBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._prgsBar.Location = new System.Drawing.Point(6, 243);
            this._prgsBar.Name = "_prgsBar";
            this._prgsBar.Size = new System.Drawing.Size(525, 23);
            this._prgsBar.TabIndex = 5;
            // 
            // _lbl_OverallProgress
            // 
            this._lbl_OverallProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lbl_OverallProgress.AutoSize = true;
            this._lbl_OverallProgress.Location = new System.Drawing.Point(6, 227);
            this._lbl_OverallProgress.Name = "_lbl_OverallProgress";
            this._lbl_OverallProgress.Size = new System.Drawing.Size(87, 13);
            this._lbl_OverallProgress.TabIndex = 6;
            this._lbl_OverallProgress.Text = "Overall Progress:";
            // 
            // _rtb_ImportStatus
            // 
            this._rtb_ImportStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._rtb_ImportStatus.BackColor = System.Drawing.SystemColors.Control;
            this._rtb_ImportStatus.Location = new System.Drawing.Point(6, 32);
            this._rtb_ImportStatus.Name = "_rtb_ImportStatus";
            this._rtb_ImportStatus.ReadOnly = true;
            this._rtb_ImportStatus.Size = new System.Drawing.Size(525, 181);
            this._rtb_ImportStatus.TabIndex = 7;
            this._rtb_ImportStatus.TabStop = false;
            this._rtb_ImportStatus.Text = "";
            // 
            // _lbl_Sep
            // 
            this._lbl_Sep.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._lbl_Sep.BackColor = System.Drawing.SystemColors.ControlText;
            this._lbl_Sep.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this._lbl_Sep.Location = new System.Drawing.Point(-1, 360);
            this._lbl_Sep.Name = "_lbl_Sep";
            this._lbl_Sep.Size = new System.Drawing.Size(565, 1);
            this._lbl_Sep.TabIndex = 0;
            // 
            // _btn_Close
            // 
            this._btn_Close.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_Close.Location = new System.Drawing.Point(312, 373);
            this._btn_Close.Name = "_btn_Close";
            this._btn_Close.Size = new System.Drawing.Size(75, 23);
            this._btn_Close.TabIndex = 8;
            this._btn_Close.Text = "&Close";
            this._btn_Close.UseVisualStyleBackColor = true;
            this._btn_Close.Click += new System.EventHandler(this._btn_Close_Click);
            // 
            // _btn_Help
            // 
            this._btn_Help.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btn_Help.Location = new System.Drawing.Point(474, 373);
            this._btn_Help.Name = "_btn_Help";
            this._btn_Help.Size = new System.Drawing.Size(75, 23);
            this._btn_Help.TabIndex = 9;
            this._btn_Help.Text = "&Help";
            this._btn_Help.UseVisualStyleBackColor = true;
            this._btn_Help.Click += new System.EventHandler(this._btn_Help_Click);
            // 
            // _grpbx
            // 
            this._grpbx.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._grpbx.Controls.Add(this._lbl_ImportStatus);
            this._grpbx.Controls.Add(this._rtb_ImportStatus);
            this._grpbx.Controls.Add(this._lbl_OverallProgress);
            this._grpbx.Controls.Add(this._prgsBar);
            this._grpbx.Location = new System.Drawing.Point(12, 61);
            this._grpbx.Name = "_grpbx";
            this._grpbx.Size = new System.Drawing.Size(537, 284);
            this._grpbx.TabIndex = 10;
            this._grpbx.TabStop = false;
            // 
            // ImportStatusForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(561, 408);
            this.Controls.Add(this._btn_Help);
            this.Controls.Add(this._lbl_Sep);
            this.Controls.Add(this._btn_Close);
            this.Controls.Add(this._infbx);
            this.Controls.Add(this._btn_HideAndNotify);
            this.Controls.Add(this._btn_Cancel);
            this.Controls.Add(this._grpbx);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportStatusForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Idera SQL diagnostic manager Import Wizard Status";
            this.VisibleChanged += new System.EventHandler(this.ImportStatusForm_VisibleChanged);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImportStatusForm_FormClosing);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ImportStatusForm_HelpRequested);
            this._notifyContextMenu.ResumeLayout(false);
            this._grpbx.ResumeLayout(false);
            this._grpbx.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.Button _btn_Cancel;
        private System.Windows.Forms.Button _btn_HideAndNotify;
        private System.Windows.Forms.ContextMenuStrip _notifyContextMenu;
        private System.Windows.Forms.ToolStripMenuItem _mi_Cancel;
        private System.Windows.Forms.ToolStripSeparator _mi_ToolStripSep;
        private System.Windows.Forms.ToolStripMenuItem _mi_OpenImportStatus;
        private System.Windows.Forms.Label _lbl_ImportStatus;
        private Divelements.WizardFramework.InformationBox _infbx;
        private System.Windows.Forms.ProgressBar _prgsBar;
        private System.Windows.Forms.Label _lbl_OverallProgress;
        private System.Windows.Forms.RichTextBox _rtb_ImportStatus;
        private System.Windows.Forms.Label _lbl_Sep;
        private System.Windows.Forms.Button _btn_Close;
        private System.Windows.Forms.Button _btn_Help;
        private System.Windows.Forms.ToolStripMenuItem _mi_Help;
        private System.Windows.Forms.GroupBox _grpbx;
    }
}