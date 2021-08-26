namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class PleaseWaitDialog
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
            this.components = new System.ComponentModel.Container();
            this.ultraActivityIndicator1 = new Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator();
            this.message = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // ultraActivityIndicator1
            // 
            this.ultraActivityIndicator1.AnimationEnabled = true;
            this.ultraActivityIndicator1.CausesValidation = true;
            this.ultraActivityIndicator1.Location = new System.Drawing.Point(12, 58);
            this.ultraActivityIndicator1.Name = "ultraActivityIndicator1";
            this.ultraActivityIndicator1.Size = new System.Drawing.Size(398, 23);
            this.ultraActivityIndicator1.TabIndex = 0;
            this.ultraActivityIndicator1.TabStop = true;
            // 
            // message
            // 
            this.message.Location = new System.Drawing.Point(12, 32);
            this.message.Name = "message";
            this.message.Size = new System.Drawing.Size(398, 23);
            this.message.TabIndex = 1;
            this.message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer
            // 
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // PleaseWaitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 123);
            this.ControlBox = false;
            this.Controls.Add(this.message);
            this.Controls.Add(this.ultraActivityIndicator1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "PleaseWaitDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please Wait";
            this.Load += new System.EventHandler(this.PleaseWaitDialog_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private Infragistics.Win.UltraActivityIndicator.UltraActivityIndicator ultraActivityIndicator1;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel message;
        private System.Windows.Forms.Timer timer;
    }
}