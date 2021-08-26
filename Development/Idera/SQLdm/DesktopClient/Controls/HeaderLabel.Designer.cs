namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class HeaderLabel
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
            this.headerText = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.SuspendLayout();
            // 
            // headerText
            // 
            this.headerText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerText.AutoEllipsis = true;
            this.headerText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.headerText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(89)))), ((int)(((byte)(89)))), ((int)(((byte)(89)))));
            this.headerText.Location = new System.Drawing.Point(5, 2);
            this.headerText.Name = "headerText";
            this.headerText.Size = new System.Drawing.Size(274, 15);
            this.headerText.TabIndex = 0;
            this.headerText.Text = "<header>";
            this.headerText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HeaderLabel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.headerText);
            this.Name = "HeaderLabel";
            this.Size = new System.Drawing.Size(280, 20);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel headerText;
    }
}
