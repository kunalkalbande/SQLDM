namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class FailureIsCriticalOptionDialog
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.informationBox1 = new Divelements.WizardFramework.InformationBox();
            this.generateAlertButton = new System.Windows.Forms.RadioButton();
            this.skipAlertButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(178, 107);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(259, 107);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // informationBox1
            // 
            this.informationBox1.Location = new System.Drawing.Point(5, 12);
            this.informationBox1.Name = "informationBox1";
            this.informationBox1.Size = new System.Drawing.Size(329, 41);
            this.informationBox1.TabIndex = 2;
            this.informationBox1.Text = "If the value for the custom counter fails to be collected, would you like to gene" +
                "rate this alert in a critical state.";
            // 
            // generateAlertButton
            // 
            this.generateAlertButton.AutoSize = true;
            this.generateAlertButton.Checked = true;
            this.generateAlertButton.Location = new System.Drawing.Point(36, 47);
            this.generateAlertButton.Name = "generateAlertButton";
            this.generateAlertButton.Size = new System.Drawing.Size(92, 17);
            this.generateAlertButton.TabIndex = 3;
            this.generateAlertButton.TabStop = true;
            this.generateAlertButton.Text = "Generate alert";
            this.generateAlertButton.UseVisualStyleBackColor = true;
            // 
            // skipAlertButton
            // 
            this.skipAlertButton.AutoSize = true;
            this.skipAlertButton.Location = new System.Drawing.Point(36, 70);
            this.skipAlertButton.Name = "skipAlertButton";
            this.skipAlertButton.Size = new System.Drawing.Size(125, 17);
            this.skipAlertButton.TabIndex = 4;
            this.skipAlertButton.Text = "Do not generate alert";
            this.skipAlertButton.UseVisualStyleBackColor = true;
            // 
            // FailureIsCriticalOptionDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(346, 142);
            this.Controls.Add(this.skipAlertButton);
            this.Controls.Add(this.generateAlertButton);
            this.Controls.Add(this.informationBox1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FailureIsCriticalOptionDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generate Alert";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Divelements.WizardFramework.InformationBox informationBox1;
        private System.Windows.Forms.RadioButton generateAlertButton;
        private System.Windows.Forms.RadioButton skipAlertButton;
    }
}