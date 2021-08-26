namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class PulseNotificationsConfigurationControl
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
            this._notificationRulesListBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckedListBox1();
            this._addRuleButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._noRulesLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._statusPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this._statusLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this._statusCircle = new MRG.Controls.UI.LoadingCircle();
            this._initializeWorker = new System.ComponentModel.BackgroundWorker();
            this._selectAllCheckBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox();
            this._editRuleButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._deleteRuleButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this._statusPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _notificationRulesListBox
            // 
            this._notificationRulesListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._notificationRulesListBox.CheckOnClick = true;
            this._notificationRulesListBox.Font = new System.Drawing.Font("Arial", 9F);
            this._notificationRulesListBox.FormattingEnabled = true;
            this._notificationRulesListBox.Location = new System.Drawing.Point(25, 31);
            this._notificationRulesListBox.Name = "_notificationRulesListBox";
            this._notificationRulesListBox.Size = new System.Drawing.Size(588, 324);
            this._notificationRulesListBox.Sorted = true;
            this._notificationRulesListBox.TabIndex = 0;
            this._notificationRulesListBox.SelectedIndexChanged += new System.EventHandler(this._notificationRulesListBox_SelectedIndexChanged);
            this._notificationRulesListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this._notificationRulesListBox_ItemCheck);
            // 
            // _addRuleButton
            // 
            this._addRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._addRuleButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._addRuleButton.Location = new System.Drawing.Point(25, 360);
            this._addRuleButton.Name = "_addRuleButton";
            this._addRuleButton.Size = new System.Drawing.Size(146, 27);
            this._addRuleButton.TabIndex = 2;
            this._addRuleButton.Text = "Add Notification Rule";
            this._addRuleButton.UseVisualStyleBackColor = true;
            this._addRuleButton.Click += new System.EventHandler(this._addRuleButton_Click);
            // 
            // _noRulesLabel
            // 
            this._noRulesLabel.AutoSize = true;
            this._noRulesLabel.BackColor = System.Drawing.Color.White;
            this._noRulesLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._noRulesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this._noRulesLabel.Location = new System.Drawing.Point(201, 35);
            this._noRulesLabel.Name = "_noRulesLabel";
            this._noRulesLabel.Size = new System.Drawing.Size(236, 15);
            this._noRulesLabel.TabIndex = 11;
            this._noRulesLabel.Text = "There are no notification rules at this time.";
            this._noRulesLabel.Visible = false;
            // 
            // _statusPanel
            // 
            this._statusPanel.BackColor = System.Drawing.Color.White;
            this._statusPanel.Controls.Add(this._statusLabel);
            this._statusPanel.Controls.Add(this._statusCircle);
            this._statusPanel.Location = new System.Drawing.Point(218, 140);
            this._statusPanel.Name = "_statusPanel";
            this._statusPanel.Size = new System.Drawing.Size(200, 67);
            this._statusPanel.TabIndex = 12;
            this._statusPanel.Visible = false;
            // 
            // _statusLabel
            // 
            this._statusLabel.AutoSize = true;
            this._statusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this._statusLabel.Location = new System.Drawing.Point(70, 26);
            this._statusLabel.Name = "_statusLabel";
            this._statusLabel.Size = new System.Drawing.Size(54, 13);
            this._statusLabel.TabIndex = 1;
            this._statusLabel.Text = Idera.SQLdm.Common.Constants.LOADING;
            // 
            // _statusCircle
            // 
            this._statusCircle.Active = false;
            this._statusCircle.Color = System.Drawing.Color.DarkGray;
            this._statusCircle.InnerCircleRadius = 8;
            this._statusCircle.Location = new System.Drawing.Point(36, 22);
            this._statusCircle.Name = "_statusCircle";
            this._statusCircle.NumberSpoke = 24;
            this._statusCircle.OuterCircleRadius = 9;
            this._statusCircle.RotationSpeed = 40;
            this._statusCircle.Size = new System.Drawing.Size(32, 23);
            this._statusCircle.SpokeThickness = 4;
            this._statusCircle.StylePreset = MRG.Controls.UI.LoadingCircle.StylePresets.IE7;
            this._statusCircle.TabIndex = 0;
            // 
            // _initializeWorker
            // 
            this._initializeWorker.WorkerSupportsCancellation = true;
            this._initializeWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this._initializeWorker_DoWork);
            this._initializeWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this._initializeWorker_RunWorkerCompleted);
            // 
            // _selectAllCheckBox
            // 
            this._selectAllCheckBox.AutoSize = true;
            this._selectAllCheckBox.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._selectAllCheckBox.ForeColor = System.Drawing.Color.Black;
            this._selectAllCheckBox.Location = new System.Drawing.Point(28, 10);
            this._selectAllCheckBox.Name = "_selectAllCheckBox";
            this._selectAllCheckBox.Size = new System.Drawing.Size(75, 19);
            this._selectAllCheckBox.TabIndex = 13;
            this._selectAllCheckBox.Text = "Select All";
            this._selectAllCheckBox.UseVisualStyleBackColor = true;
            this._selectAllCheckBox.CheckedChanged += new System.EventHandler(this._selectAllCheckBox_CheckedChanged);
            // 
            // _editRuleButton
            // 
            this._editRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._editRuleButton.Enabled = false;
            this._editRuleButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._editRuleButton.Location = new System.Drawing.Point(177, 360);
            this._editRuleButton.Name = "_editRuleButton";
            this._editRuleButton.Size = new System.Drawing.Size(75, 27);
            this._editRuleButton.TabIndex = 14;
            this._editRuleButton.Text = "Edit";
            this._editRuleButton.UseVisualStyleBackColor = true;
            this._editRuleButton.Click += new System.EventHandler(this._editRuleButton_Click);
            // 
            // _deleteRuleButton
            // 
            this._deleteRuleButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._deleteRuleButton.Enabled = false;
            this._deleteRuleButton.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._deleteRuleButton.Location = new System.Drawing.Point(258, 360);
            this._deleteRuleButton.Name = "_deleteRuleButton";
            this._deleteRuleButton.Size = new System.Drawing.Size(75, 27);
            this._deleteRuleButton.TabIndex = 15;
            this._deleteRuleButton.Text = "Delete";
            this._deleteRuleButton.UseVisualStyleBackColor = true;
            this._deleteRuleButton.Click += new System.EventHandler(this._deleteRuleButton_Click);
            // 
            // PulseNotificationsConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this._deleteRuleButton);
            this.Controls.Add(this._editRuleButton);
            this.Controls.Add(this._selectAllCheckBox);
            this.Controls.Add(this._statusPanel);
            this.Controls.Add(this._noRulesLabel);
            this.Controls.Add(this._addRuleButton);
            this.Controls.Add(this._notificationRulesListBox);
            this.Name = "PulseNotificationsConfigurationControl";
            this.Size = new System.Drawing.Size(637, 403);
            this._statusPanel.ResumeLayout(false);
            this._statusPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox _notificationRulesListBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _addRuleButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _noRulesLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  _statusPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel _statusLabel;
        private MRG.Controls.UI.LoadingCircle _statusCircle;
        private System.ComponentModel.BackgroundWorker _initializeWorker;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomCheckBox _selectAllCheckBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _editRuleButton;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton _deleteRuleButton;

    }
}
