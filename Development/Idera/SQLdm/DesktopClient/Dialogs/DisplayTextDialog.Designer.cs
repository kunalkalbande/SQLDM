namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class DisplayTextDialog
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
            this.headerStrip1 = new Idera.SQLdm.DesktopClient.Controls.HeaderStrip();
            this.copyAllButton = new System.Windows.Forms.ToolStripButton();
            this.copySelectedButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.closeButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.textBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox();
            this.contentPanel.SuspendLayout();
            this.headerStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.contentPanel.BackColor2 = System.Drawing.Color.Empty;
            this.contentPanel.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.contentPanel.Controls.Add(this.textBox);
            this.contentPanel.Controls.Add(this.panel1);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.GradientAngle = 90;
            this.contentPanel.Location = new System.Drawing.Point(0, 25);
            this.contentPanel.Margin = new System.Windows.Forms.Padding(2);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Padding = new System.Windows.Forms.Padding(1);
            this.contentPanel.Size = new System.Drawing.Size(554, 117);
            this.contentPanel.TabIndex = 1;
            // 
            // headerStrip1
            // 
            this.headerStrip1.AutoSize = false;
            this.headerStrip1.Font = new System.Drawing.Font("Arial", 12.75F, System.Drawing.FontStyle.Bold);
            this.headerStrip1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.headerStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyAllButton,
            this.copySelectedButton});
            this.headerStrip1.Location = new System.Drawing.Point(0, 0);
            this.headerStrip1.Name = "headerStrip1";
            this.headerStrip1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.headerStrip1.Size = new System.Drawing.Size(554, 25);
            this.headerStrip1.TabIndex = 0;
            this.headerStrip1.Text = "headerStrip1";
            // 
            // copyAllButton
            // 
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(213)))), ((int)(((byte)(213)))), ((int)(((byte)(213)))));
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(1, 81);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(552, 35);
            this.panel1.TabIndex = 0;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(466, 6);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 0;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.White;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Location = new System.Drawing.Point(1, 1);
            this.textBox.Multiline = true;
            this.textBox.Name = "textBox";
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Size = new System.Drawing.Size(552, 80);
            this.textBox.TabIndex = 1;
            this.textBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyUp);
            this.textBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.textBox_MouseUp);
            // 
            // DisplayTextDialog
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(554, 142);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.headerStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "DisplayTextDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DisplayTextDialog";
            this.contentPanel.ResumeLayout(false);
            this.contentPanel.PerformLayout();
            this.headerStrip1.ResumeLayout(false);
            this.headerStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.HeaderStrip headerStrip1;
        private System.Windows.Forms.ToolStripButton copyAllButton;
        private System.Windows.Forms.ToolStripButton copySelectedButton;
        private Idera.SQLdm.DesktopClient.Controls.GradientPanel contentPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTextBox textBox;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton closeButton;
    }
}