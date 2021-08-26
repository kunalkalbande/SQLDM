namespace Idera.SQLdm.DesktopClient.Controls
{
    partial class BreadCrumb
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )        
        {            
            if( disposing )
            {
                if (hotFont     != null) hotFont.Dispose();
                if (coldFont    != null) coldFont.Dispose();
                if (dividerFont != null) dividerFont.Dispose();

                if(components != null)
                    components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.flowLayoutPanel1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip( this.components );
            this.toolStripMenuItem1 = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point( 0, 0 );
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size( 448, 102 );
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange( new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1} );
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size( 153, 48 );
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.Information16x16;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size( 152, 22 );
            this.toolStripMenuItem1.Text = "View Text";
            this.toolStripMenuItem1.Click += new System.EventHandler( this.toolStripMenuItem1_Click );
            // 
            // BreadCrumb
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add( this.flowLayoutPanel1 );
            this.Name = "BreadCrumb";
            this.Size = new System.Drawing.Size( 448, 102 );
            this.Load += new System.EventHandler( this.BreadCrumb_Load );
            this.contextMenuStrip1.ResumeLayout( false );
            this.ResumeLayout( false );

        }

        #endregion

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomFlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
    }
}
