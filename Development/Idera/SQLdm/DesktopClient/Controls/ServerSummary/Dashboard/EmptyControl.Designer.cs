namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class EmptyControl
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
            this.designLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.SuspendLayout();
            // 
            // designLabel
            // 
            this.designLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.designLabel.Location = new System.Drawing.Point(0, 0);
            this.designLabel.Name = "designLabel";
            this.designLabel.Size = new System.Drawing.Size(600, 184);
            this.designLabel.TabIndex = 1;
            this.designLabel.Text = "Drag a panel from the panel gallery and drop it here";
            this.designLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // EmptyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Caption = "";
            this.Controls.Add(this.designLabel);
            this.Name = "EmptyControl";
            this.Controls.SetChildIndex(this.designLabel, 0);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel designLabel;
    }
}
