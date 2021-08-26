namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class TemporaryTestDialog {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.parametersForReporting1 = new Idera.SQLdm.DesktopClient.Controls.Reporting.ParametersForReporting();
            this.SuspendLayout();
            // 
            // parametersForReporting1
            // 
            this.parametersForReporting1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parametersForReporting1.Location = new System.Drawing.Point(5, 5);
            this.parametersForReporting1.Name = "parametersForReporting1";
            this.parametersForReporting1.Size = new System.Drawing.Size(529, 135);
            this.parametersForReporting1.TabIndex = 0;
            this.parametersForReporting1.RunClicked += new System.EventHandler(this.RunReportClicked);
            // 
            // TemporaryTestDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(539, 145);
            this.Controls.Add(this.parametersForReporting1);
            this.Name = "TemporaryTestDialog";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Text = "TemporaryTestDialog";
            this.ResumeLayout(false);

        }

        #endregion

        private ParametersForReporting parametersForReporting1;

    }
}