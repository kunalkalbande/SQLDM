namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ReportInstructionsControl
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
            this.panel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.reportTitleLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel2 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.reportDescriptionLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.label3 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel4 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.reportInstructionsLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.reportTitleLabel);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(761, 47);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.SQLdmLogoHeader;
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(532, 45);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // reportTitleLabel
            // 
            this.reportTitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.reportTitleLabel.AutoEllipsis = true;
            this.reportTitleLabel.BackColor = System.Drawing.Color.White;
            this.reportTitleLabel.Font = new System.Drawing.Font("Arial", 14.25F, System.Drawing.FontStyle.Bold);
            this.reportTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(75)))), ((int)(((byte)(75)))));
            this.reportTitleLabel.Location = new System.Drawing.Point(411, 18);
            this.reportTitleLabel.Name = "reportTitleLabel";
            this.reportTitleLabel.Size = new System.Drawing.Size(350, 27);
            this.reportTitleLabel.TabIndex = 0;
            this.reportTitleLabel.Text = "< Report Title >";
            this.reportTitleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(235)))), ((int)(((byte)(234)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(761, 2);
            this.panel2.TabIndex = 0;
            // 
            // reportDescriptionLabel
            // 
            this.reportDescriptionLabel.AutoEllipsis = true;
            this.reportDescriptionLabel.BackColor = System.Drawing.Color.White;
            this.reportDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportDescriptionLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this.reportDescriptionLabel.Location = new System.Drawing.Point(5, 5);
            this.reportDescriptionLabel.Name = "reportDescriptionLabel";
            this.reportDescriptionLabel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.reportDescriptionLabel.Size = new System.Drawing.Size(756, 78);
            this.reportDescriptionLabel.TabIndex = 1;
            this.reportDescriptionLabel.Text = "< Report Description >";
            // 
            // label3
            // 
            this.label3.AutoEllipsis = true;
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F);
            this.label3.Location = new System.Drawing.Point(5, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(756, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Follow these steps to run this report:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel4
            // 
            this.panel4.AutoScroll = true;
            this.panel4.AutoSize = true;
            this.panel4.Controls.Add(this.reportInstructionsLabel);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.reportDescriptionLabel);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 77);
            this.panel4.Name = "panel4";
            this.panel4.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.panel4.Size = new System.Drawing.Size(761, 554);
            this.panel4.TabIndex = 3;
            // 
            // reportInstructionsLabel
            // 
            this.reportInstructionsLabel.BackColor = System.Drawing.Color.White;
            this.reportInstructionsLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportInstructionsLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportInstructionsLabel.Location = new System.Drawing.Point(5, 106);
            this.reportInstructionsLabel.Name = "reportInstructionsLabel";
            this.reportInstructionsLabel.Padding = new System.Windows.Forms.Padding(30, 10, 30, 0);
            this.reportInstructionsLabel.Size = new System.Drawing.Size(756, 448);
            this.reportInstructionsLabel.TabIndex = 3;
            this.reportInstructionsLabel.Text = "1. < Step 1 >\r\n2. < Step 2 >\r\n3. < Step 3 >";
            // 
            // ReportInstructionsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel1);
            this.Name = "ReportInstructionsControl";
            this.Size = new System.Drawing.Size(761, 631);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel2;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel reportTitleLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel reportDescriptionLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel label3;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  panel4;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel reportInstructionsLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
