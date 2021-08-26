namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class ReportSelectionButton
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
            this.reportDescriptionLabel = new System.Windows.Forms.Label();
            this.reportTitleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // reportDescriptionLabel
            // 
            this.reportDescriptionLabel.AutoEllipsis = true;
            this.reportDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.reportDescriptionLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportDescriptionLabel.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportDescriptionLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this.reportDescriptionLabel.Location = new System.Drawing.Point(0, 16);
            this.reportDescriptionLabel.Name = "reportDescriptionLabel";
            this.reportDescriptionLabel.Size = new System.Drawing.Size(306, 37);
            this.reportDescriptionLabel.TabIndex = 2;
            this.reportDescriptionLabel.Text = "<description>";
            this.reportDescriptionLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.reportDescriptionLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            this.reportDescriptionLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            this.reportDescriptionLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.reportDescriptionLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
            this.reportDescriptionLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
            // 
            // reportTitleLabel
            // 
            this.reportTitleLabel.AutoSize = true;
            this.reportTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.reportTitleLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.reportTitleLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.reportTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.reportTitleLabel.Location = new System.Drawing.Point(0, 0);
            this.reportTitleLabel.Name = "reportTitleLabel";
            this.reportTitleLabel.Size = new System.Drawing.Size(69, 16);
            this.reportTitleLabel.TabIndex = 1;
            this.reportTitleLabel.Text = "<header>";
            this.reportTitleLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.reportTitleLabel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseDown);
            this.reportTitleLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            this.reportTitleLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.reportTitleLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseMove);
            this.reportTitleLabel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseUp);
            // 
            // ReportSelectionButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.reportDescriptionLabel);
            this.Controls.Add(this.reportTitleLabel);
            this.MinimumSize = new System.Drawing.Size(0, 40);
            this.Name = "ReportSelectionButton";
            this.Size = new System.Drawing.Size(306, 55);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label reportDescriptionLabel;
        private System.Windows.Forms.Label reportTitleLabel;
    }
}
