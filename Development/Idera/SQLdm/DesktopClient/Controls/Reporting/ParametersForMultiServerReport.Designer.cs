namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    partial class ParametersForMultiServerReport {
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
            this.serversLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.serversBox = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox();
            this.SuspendLayout();
            // 
            // serversLabel
            // 
            this.serversLabel.AutoSize = true;
            this.serversLabel.Location = new System.Drawing.Point(0, 4);
            this.serversLabel.Name = "serversLabel";
            this.serversLabel.Size = new System.Drawing.Size(46, 13);
            this.serversLabel.TabIndex = 0;
            this.serversLabel.Text = "Servers:";
            // 
            // serversBox
            // 
            this.serversBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.serversBox.FormattingEnabled = true;
            this.serversBox.Location = new System.Drawing.Point(52, 0);
            this.serversBox.Name = "serversBox";
            this.serversBox.Size = new System.Drawing.Size(174, 21);
            this.serversBox.TabIndex = 1;
            this.serversBox.DropDown += new System.EventHandler(this.serversBox_DropDown);
            // 
            // ParametersForMultiServerReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.serversBox);
            this.Controls.Add(this.serversLabel);
            this.Name = "ParametersForMultiServerReport";
            this.Size = new System.Drawing.Size(226, 21);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel serversLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox serversBox;
    }
}
