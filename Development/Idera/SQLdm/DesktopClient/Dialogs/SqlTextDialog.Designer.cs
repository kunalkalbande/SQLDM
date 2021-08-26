namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class SqlTextDialog
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
            this.contentPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.sqlTextHeaderStrip = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.copyAllButton = new System.Windows.Forms.ToolStripButton();
            this.copySelectedButton = new System.Windows.Forms.ToolStripButton();
            this.sqlTextTextBox = new System.Windows.Forms.TextBox();
            this.diagnoseButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.contentPanel.SuspendLayout();
            this.sqlTextHeaderStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.contentPanel.BackColor2 = System.Drawing.Color.Empty;
            this.contentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.contentPanel.Controls.Add(this.diagnoseButton);
            this.contentPanel.Controls.Add(this.okButton);
            this.contentPanel.Controls.Add(this.sqlTextTextBox);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.GradientAngle = 90;
            this.contentPanel.Location = new System.Drawing.Point(0, 25);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(2);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.contentPanel.Size = new System.Drawing.Size(500, 461);
            this.contentPanel.TabIndex = 2;
            // 
            // sqlTextHeaderStrip
            // 
            this.sqlTextHeaderStrip.AutoSize = false;
            this.sqlTextHeaderStrip.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.sqlTextHeaderStrip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.sqlTextHeaderStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.sqlTextHeaderStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllButton,
            this.copySelectedButton});
            this.sqlTextHeaderStrip.Location = new System.Drawing.Point(0, 0);
            this.sqlTextHeaderStrip.Name = "sqlTextHeaderStrip";
            this.sqlTextHeaderStrip.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.sqlTextHeaderStrip.Size = new System.Drawing.Size(500, 25);
            this.sqlTextHeaderStrip.TabIndex = 0;
            this.sqlTextHeaderStrip.Text = "headerStrip1";
            // 
            // copyAllButton
            // 
            //this.copyAllButton.Checked = true;
            //this.copyAllButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.copyAllButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copyAllButton.ForeColor = System.Drawing.Color.Black;
            this.copyAllButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            this.copyAllButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.copyAllButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copyAllButton.Name = "copyAllButton";
            this.copyAllButton.Size = new System.Drawing.Size(72, 20);
            this.copyAllButton.Text = "Copy All";
            this.copyAllButton.ToolTipText = "Copy all text to the clipboard";
            this.copyAllButton.Click += new System.EventHandler(this.copyAllButton_Click);
            // 
            // copySelectedButton
            // 
            //this.copySelectedButton.Checked = true;
            //this.copySelectedButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.copySelectedButton.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.copySelectedButton.ForeColor = System.Drawing.Color.Black;
            this.copySelectedButton.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.copy;
            this.copySelectedButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.copySelectedButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.copySelectedButton.Name = "copySelectedButton";
            this.copySelectedButton.Size = new System.Drawing.Size(106, 20);
            this.copySelectedButton.Text = "Copy Selected";
            this.copySelectedButton.ToolTipText = "Copy selected text to the clipboard";
            this.copySelectedButton.Click += new System.EventHandler(this.copySelectedButton_Click);
            // 
            // sqlTextTextBox
            // 
            this.sqlTextTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sqlTextTextBox.BackColor = System.Drawing.Color.White;
            this.sqlTextTextBox.Location = new System.Drawing.Point(4, 4);
            this.sqlTextTextBox.Multiline = true;
            this.sqlTextTextBox.Name = "sqlTextTextBox";
            this.sqlTextTextBox.ReadOnly = true;
            this.sqlTextTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.sqlTextTextBox.Size = new System.Drawing.Size(493, 418);
            this.sqlTextTextBox.TabIndex = 1;
            this.sqlTextTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sqlTextTextBox_KeyUp);
            this.sqlTextTextBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.sqlTextTextBox_MouseUp);
            // 
            // diagnoseButton
            // 
            this.diagnoseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.diagnoseButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.diagnoseButton.Location = new System.Drawing.Point(340, 428);
            this.diagnoseButton.Name = "diagnoseButton";
            this.diagnoseButton.Size = new System.Drawing.Size(75, 23);
            this.diagnoseButton.TabIndex = 2;
            this.diagnoseButton.Text = "Diagnose Query";
            this.diagnoseButton.UseVisualStyleBackColor = true;
            this.diagnoseButton.Click += new System.EventHandler(this.diagnoseButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(415, 428);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Close";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // SqlTextDialog
            // 
            this.AcceptButton = this.diagnoseButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(500, 486);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.sqlTextHeaderStrip);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "SqlTextDialog";
            this.ShowIcon = false;
            this.Text = "SQL Text";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.sqlTextHeaderStrip.ResumeLayout(false);
            this.sqlTextHeaderStrip.PerformLayout();
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip sqlTextHeaderStrip;
        private System.Windows.Forms.ToolStripButton copyAllButton;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private System.Windows.Forms.ToolStripButton copySelectedButton;
        private System.Windows.Forms.Button diagnoseButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox sqlTextTextBox;
    }
}
