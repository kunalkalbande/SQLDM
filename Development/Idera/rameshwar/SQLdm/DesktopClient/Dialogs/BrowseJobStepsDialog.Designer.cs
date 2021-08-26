using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class BrowseJobStepsDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.filterOptionsTreeView = new System.Windows.Forms.TreeView();
            this.refreshBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.refreshProgressControl = new MRG.Controls.UI.LoadingCircle();
            this.refreshLinkLabel = new System.Windows.Forms.LinkLabel();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Location = new System.Drawing.Point(0, 357);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(493, 37);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.filterOptionsTreeView);
            this.panel2.Controls.Add(this.refreshLinkLabel);
            this.panel2.Controls.Add(this.refreshProgressControl);
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(493, 357);
            this.panel2.TabIndex = 3;
            // 
            // filterOptionsTreeView
            // 
            this.filterOptionsTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.filterOptionsTreeView.FullRowSelect = true;
            this.filterOptionsTreeView.Location = new System.Drawing.Point(12, 12);
            this.filterOptionsTreeView.Name = "filterOptionsTreeView";
            this.filterOptionsTreeView.Size = new System.Drawing.Size(469, 333);
            this.filterOptionsTreeView.TabIndex = 0;
            this.filterOptionsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.filterOptionsTreeView_AfterSelect);
            // 
            // refreshBackgroundWorker
            // 
            this.refreshBackgroundWorker.WorkerSupportsCancellation = true;
            this.refreshBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.refreshBackgroundWorker_DoWork);
            this.refreshBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.refreshBackgroundWorker_RunWorkerCompleted);
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
            this.refreshProgressControl.Location = new System.Drawing.Point(12, 12);
            this.refreshProgressControl.Name = "refreshProgressControl";
            this.refreshProgressControl.NumberSpoke = 10;
            this.refreshProgressControl.OuterCircleRadius = 12;
            this.refreshProgressControl.RotationSpeed = 80;
            this.refreshProgressControl.Size = new System.Drawing.Size(469, 333);
            this.refreshProgressControl.SpokeThickness = 4;
            this.refreshProgressControl.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.MacOSX;
            this.refreshProgressControl.TabIndex = 3;
            this.refreshProgressControl.Visible = false;
            // 
            // refreshLinkLabel
            // 
            this.refreshLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.refreshLinkLabel.BackColor = System.Drawing.Color.White;
            this.refreshLinkLabel.Location = new System.Drawing.Point(12, 12);
            this.refreshLinkLabel.Name = "refreshLinkLabel";
            this.refreshLinkLabel.Size = new System.Drawing.Size(469, 333);
            this.refreshLinkLabel.TabIndex = 4;
            this.refreshLinkLabel.TabStop = true;
            this.refreshLinkLabel.Text = "Click here to refresh";
            this.refreshLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.refreshLinkLabel.Visible = false;
            this.refreshLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.refreshLinkLabel_LinkClicked);
            // 
            // BrowseJobStepsDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(493, 394);
            this.FormBorderStyle=FormBorderStyle.FixedDialog;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BrowseJobStepsDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "{0} - Select job step to filter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BrowseJobStepsDialog_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TreeView filterOptionsTreeView;
        private System.ComponentModel.BackgroundWorker refreshBackgroundWorker;
        private MRG.Controls.UI.LoadingCircle refreshProgressControl;
        private System.Windows.Forms.LinkLabel refreshLinkLabel;
    }
}