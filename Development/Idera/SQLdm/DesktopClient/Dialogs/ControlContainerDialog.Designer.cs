namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class ControlContainerDialog
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
            this.controlContainerPanel = new Idera.SQLdm.DesktopClient.Controls.GradientPanel();
            this.SuspendLayout();
            // 
            // controlContainerPanel
            // 
            this.controlContainerPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.controlContainerPanel.BackColor2 = System.Drawing.Color.White;
            this.controlContainerPanel.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(140)))), ((int)(((byte)(140)))));
            this.controlContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.controlContainerPanel.FillStyle = Idera.SQLdm.DesktopClient.Controls.GradientPanelFillStyle.Solid;
            this.controlContainerPanel.Location = new System.Drawing.Point(0, 0);
            this.controlContainerPanel.Name = "controlContainerPanel";
            this.controlContainerPanel.Padding = new System.Windows.Forms.Padding(1);
            this.controlContainerPanel.Size = new System.Drawing.Size(271, 416);
            this.controlContainerPanel.TabIndex = 0;
            // 
            // ControlContainerDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(271, 416);
            this.Controls.Add(this.controlContainerPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ControlContainerDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ControlContainerDialog";
            this.Deactivate += new System.EventHandler(this.ControlContainerDialog_Deactivate);
            this.ResumeLayout(false);

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.GradientPanel controlContainerPanel;

    }
}