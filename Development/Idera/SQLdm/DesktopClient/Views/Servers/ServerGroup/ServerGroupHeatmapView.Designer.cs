namespace Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup
{
    partial class ServerGroupHeatMapView
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
            this.flowLayoutPanel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel(); //changed from flowlayout to table layout
            this.ServerGroupHeatMapView_Fill_Panel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel ();
            this.ServerGroupHeatMapView_Fill_Panel.SuspendLayout();
            this.SuspendLayout();
            //
            // noInstancesLabel
            //
            this.NoInstancesLabel = new System.Windows.Forms.Label();
            this.NoInstancesLabel.Text = "No instances to show in this view";
            this.NoInstancesLabel.Location = new System.Drawing.Point(300, 5);
            this.NoInstancesLabel.Size = new System.Drawing.Size(300,20);
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel.Location = new System.Drawing.Point(10, 10);
            this.flowLayoutPanel.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.flowLayoutPanel.Size = new System.Drawing.Size(940, 940);
            this.flowLayoutPanel.TabIndex = 0;
            this.flowLayoutPanel.Visible = false;
            //this.flowLayoutPanel.Click += ServerGroupHeatMapView_Click;
            // 
            // ServerGroupHeatMapView_Fill_Panel
            // 
            this.ServerGroupHeatMapView_Fill_Panel.AutoScroll = true;
            this.ServerGroupHeatMapView_Fill_Panel.Controls.Add(this.flowLayoutPanel);
            this.ServerGroupHeatMapView_Fill_Panel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ServerGroupHeatMapView_Fill_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerGroupHeatMapView_Fill_Panel.Location = new System.Drawing.Point(0, 0);
            this.ServerGroupHeatMapView_Fill_Panel.Margin = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.ServerGroupHeatMapView_Fill_Panel.Name = "ServerGroupHeatMapView_Fill_Panel";
            this.ServerGroupHeatMapView_Fill_Panel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 10);
            this.ServerGroupHeatMapView_Fill_Panel.Size = new System.Drawing.Size(960, 960);
            this.ServerGroupHeatMapView_Fill_Panel.TabIndex = 0;
            // 
            // ServerGroupHeatMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.ServerGroupHeatMapView_Fill_Panel);
            this.Margin = new System.Windows.Forms.Padding(14, 14, 14, 14);
            this.Name = "ServerGroupHeatMapView";
            this.Size = new System.Drawing.Size(960, 960);
            this.ServerGroupHeatMapView_Fill_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

       


        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomTableLayoutPanel flowLayoutPanel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomPanel  ServerGroupHeatMapView_Fill_Panel;
        private System.Windows.Forms.Label NoInstancesLabel;

        
        #endregion
    }
}
