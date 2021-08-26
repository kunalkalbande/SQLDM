namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Dashboard
{
    partial class AnalysisViewControl
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
            this.panelAnalysisView = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panelAnalysisView
            // 
            this.panelAnalysisView.AutoSize = false;
            this.panelAnalysisView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAnalysisView.Location = new System.Drawing.Point(0, 0);
            this.panelAnalysisView.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.panelAnalysisView.Name = "panelAnalysisView";
            this.panelAnalysisView.Size = new System.Drawing.Size(284, 261);
            this.panelAnalysisView.TabIndex = 0;
            this.panelAnalysisView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelAnalysisView_MouseClick);
            // 
            // AnalysisViewControl
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.panelAnalysisView);
            this.Name = "AnalysisViewControl";
            this.Caption = "Analysis Results View";
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.AutoSize = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Panel panelAnalysisView;
    }
}
