namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class ToggleButton
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
            this.backgroundPanel = new Idera.SQLdm.DesktopClient.Controls.RoundedPanel();
            this.textLabel = new System.Windows.Forms.Label();
            this.toggleImage = new System.Windows.Forms.PictureBox();
            this.backgroundPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.toggleImage)).BeginInit();
            this.SuspendLayout();
            // 
            // backgroundPanel
            // 
            this.backgroundPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.backgroundPanel.Controls.Add(this.textLabel);
            this.backgroundPanel.Controls.Add(this.toggleImage);
            this.backgroundPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.backgroundPanel.FillColor = System.Drawing.Color.White;
            this.backgroundPanel.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(225)))), ((int)(((byte)(225)))), ((int)(((byte)(225)))));
            this.backgroundPanel.Location = new System.Drawing.Point(0, 0);
            this.backgroundPanel.Name = "backgroundPanel";
            this.backgroundPanel.Radius = 2F;
            this.backgroundPanel.Size = new System.Drawing.Size(68, 19);
            this.backgroundPanel.TabIndex = 1;
            // 
            // textLabel
            // 
            this.textLabel.BackColor = System.Drawing.Color.Transparent;
            this.textLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLabel.Location = new System.Drawing.Point(0, 0);
            this.textLabel.Name = "textLabel";
            this.textLabel.Size = new System.Drawing.Size(53, 19);
            this.textLabel.TabIndex = 0;
            this.textLabel.Text = "Text";
            this.textLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.textLabel.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.textLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.textLabel.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            // 
            // toggleImage
            // 
            this.toggleImage.BackColor = System.Drawing.Color.Transparent;
            this.toggleImage.Dock = System.Windows.Forms.DockStyle.Right;
            this.toggleImage.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.DownArrow_Gray_7w_4h;
            this.toggleImage.Location = new System.Drawing.Point(53, 0);
            this.toggleImage.Name = "toggleImage";
            this.toggleImage.Size = new System.Drawing.Size(15, 19);
            this.toggleImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.toggleImage.TabIndex = 1;
            this.toggleImage.TabStop = false;
            this.toggleImage.MouseLeave += new System.EventHandler(this.ChildControl_MouseLeave);
            this.toggleImage.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ChildControl_MouseClick);
            this.toggleImage.MouseEnter += new System.EventHandler(this.ChildControl_MouseEnter);
            // 
            // ToggleButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.backgroundPanel);
            this.Name = "ToggleButton";
            this.Size = new System.Drawing.Size(68, 19);
            this.backgroundPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.toggleImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RoundedPanel backgroundPanel;
        private System.Windows.Forms.PictureBox toggleImage;
        private System.Windows.Forms.Label textLabel;
    }
}
