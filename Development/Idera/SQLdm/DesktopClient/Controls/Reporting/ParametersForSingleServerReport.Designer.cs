namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class ParametersForSingleServerReport {
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
            this.serverLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.serverBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.SuspendLayout();
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(0, 3);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(41, 13);
            this.serverLabel.TabIndex = 0;
            this.serverLabel.Text = "Server:";
            // 
            // serverBox
            // 
            this.serverBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serverBox.FormattingEnabled = true;
            this.serverBox.Location = new System.Drawing.Point(47, 0);
            this.serverBox.Name = "serverBox";
            this.serverBox.Size = new System.Drawing.Size(282, 21);
            this.serverBox.TabIndex = 1;
            // 
            // ParametersForSingleServerReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.serverBox);
            this.Controls.Add(this.serverLabel);
            this.Name = "ParametersForSingleServerReport";
            this.Size = new System.Drawing.Size(329, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serverLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox serverBox;
    }
}
