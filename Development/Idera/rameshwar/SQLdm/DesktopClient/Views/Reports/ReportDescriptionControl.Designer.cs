namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    partial class ReportDescriptionControl
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
            this.headerPanel = new System.Windows.Forms.Panel();
            this.headerLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.reportDescriptionLabel = new System.Windows.Forms.Label();
            this.gettingStartedLabel = new System.Windows.Forms.Label();
            this.dividerProgressBar1 = new Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar();
            this.headerPanel.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(191)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.headerPanel.BackgroundImage = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
            this.headerPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.headerPanel.Controls.Add(this.headerLabel);
            this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.headerPanel.Location = new System.Drawing.Point(0, 0);
            this.headerPanel.Name = "headerPanel";
            this.headerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.headerPanel.Size = new System.Drawing.Size(651, 73);
            this.headerPanel.TabIndex = 5;
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel.BackColor = System.Drawing.Color.Transparent;
            this.headerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(66)))), ((int)(((byte)(139)))));
            this.headerLabel.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.headerLabel.Location = new System.Drawing.Point(275, 32);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(375, 37);
            this.headerLabel.TabIndex = 0;
            this.headerLabel.Text = "< Report Title >";
            this.headerLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 123F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.reportDescriptionLabel, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.gettingStartedLabel, 1, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 82);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(651, 458);
            this.tableLayoutPanel.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Description:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Getting Started:";
            // 
            // reportDescriptionLabel
            // 
            this.reportDescriptionLabel.AutoEllipsis = true;
            this.reportDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.reportDescriptionLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportDescriptionLabel.Location = new System.Drawing.Point(126, 0);
            this.reportDescriptionLabel.Name = "reportDescriptionLabel";
            this.reportDescriptionLabel.Size = new System.Drawing.Size(522, 60);
            this.reportDescriptionLabel.TabIndex = 4;
            this.reportDescriptionLabel.Text = "< Report description >";
            // 
            // gettingStartedLabel
            // 
            this.gettingStartedLabel.AutoEllipsis = true;
            this.gettingStartedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gettingStartedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gettingStartedLabel.Location = new System.Drawing.Point(126, 60);
            this.gettingStartedLabel.Name = "gettingStartedLabel";
            this.gettingStartedLabel.Size = new System.Drawing.Size(522, 398);
            this.gettingStartedLabel.TabIndex = 5;
            this.gettingStartedLabel.Text = "< List steps to create the report >";
            // 
            // dividerProgressBar1
            // 
            this.dividerProgressBar1.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(135)))), ((int)(((byte)(45)))));
            this.dividerProgressBar1.Color2 = System.Drawing.Color.White;
            this.dividerProgressBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dividerProgressBar1.Location = new System.Drawing.Point(0, 73);
            this.dividerProgressBar1.Name = "dividerProgressBar1";
            this.dividerProgressBar1.Size = new System.Drawing.Size(651, 3);
            this.dividerProgressBar1.Speed = 15;
            this.dividerProgressBar1.Step = 5F;
            this.dividerProgressBar1.TabIndex = 7;
            // 
            // ReportDescriptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel);
            this.Controls.Add(this.dividerProgressBar1);
            this.Controls.Add(this.headerPanel);
            this.Name = "ReportDescriptionControl";
            this.Size = new System.Drawing.Size(651, 540);
            this.headerPanel.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.InfiniteProgressBar dividerProgressBar1;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label reportDescriptionLabel;
        private System.Windows.Forms.Label gettingStartedLabel;
    }
}
