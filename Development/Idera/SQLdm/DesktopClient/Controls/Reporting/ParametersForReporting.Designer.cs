namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class ParametersForReporting {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.runReportButton = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton();
            this.timeParameters1 = new Idera.SQLdm.DesktopClient.Controls.Reporting.TimeParameters();
            this.SuspendLayout();
            // 
            // runReportButton
            // 
            this.runReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.runReportButton.Location = new System.Drawing.Point(377, -1);
            this.runReportButton.Name = "runReportButton";
            this.runReportButton.Size = new System.Drawing.Size(75, 23);
            this.runReportButton.TabIndex = 1;
            this.runReportButton.Text = "Run Report";
            this.runReportButton.UseVisualStyleBackColor = true;
            // 
            // timeParameters1
            // 
            this.timeParameters1.Dock = System.Windows.Forms.DockStyle.Top;
            this.timeParameters1.Location = new System.Drawing.Point(0, 0);
            this.timeParameters1.Name = "timeParameters1";
            this.timeParameters1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.timeParameters1.Size = new System.Drawing.Size(451, 82);
            this.timeParameters1.TabIndex = 0;
            // 
            // ParametersForReporting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.runReportButton);
            this.Controls.Add(this.timeParameters1);
            this.Name = "ParametersForReporting";
            this.Size = new System.Drawing.Size(451, 217);
            this.ResumeLayout(false);

        }

        #endregion

        public Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomButton runReportButton;
        public TimeParameters timeParameters1;

    }
}
