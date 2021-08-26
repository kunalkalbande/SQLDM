using System;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    partial class ServerUptime
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerUptime));
            this.panel1.SuspendLayout();
            this.filterPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Size = new System.Drawing.Size(752, 125);
            // 
            // reportViewer
            // 
            this.reportViewer.Size = new System.Drawing.Size(752, 456);
            // 
            // filterPanel
            // 
            this.filterPanel.Size = new System.Drawing.Size(752, 125);
           
            this.filterPanel.Controls.SetChildIndex(this.periodLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.periodCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.sampleSizeCombo, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeInfoLabel, 0);
            this.filterPanel.Controls.SetChildIndex(this.rangeLabel, 0);            
            // 
            // periodLabel
            // 
            this.periodLabel.Location = new System.Drawing.Point(10, 37);
            // 
            // sampleLabel
            // 
            this.sampleLabel.Location = new System.Drawing.Point(408, 37);
            this.sampleLabel.Visible = false;
            
            // 
            // periodCombo
            // 
            this.periodCombo.Location = new System.Drawing.Point(58, 34);
            this.periodCombo.Size = new System.Drawing.Size(300, 21);
            this.periodCombo.TabIndex = 1;
            //this.periodCombo.SelectedIndexChanged += new System.EventHandler(this.periodCombo_SelectedIndexChanged);
            // 
            // sampleSizeCombo
            // 
            this.sampleSizeCombo.Location = new System.Drawing.Point(459, 34);
            this.sampleSizeCombo.Visible = false;
            
            // 
            // reportInstructionsControl
            // 
            this.reportInstructionsControl.ReportInstructions = resources.GetString("reportInstructionsControl.ReportInstructions");
            this.reportInstructionsControl.Size = new System.Drawing.Size(752, 456);
            // 
            // rangeLabel
            // 
            this.rangeLabel.Location = new System.Drawing.Point(58, 61);
            this.rangeLabel.Size = new System.Drawing.Size(300, 21);
            // 
            // rangeInfoLabel
            // 
            this.rangeInfoLabel.Location = new System.Drawing.Point(10, 65);
            //// 
            //// instanceLabel
            //// 
            this.instanceLabel.Location = new System.Drawing.Point(10, 10);
            //// 
            //// tagsLabel
            //// 
            this.tagsLabel.Location = new System.Drawing.Point(10, 10);
            this.tagsLabel.Visible = false;
            //// 
            //// instanceCombo
            //// 
            ////
            this.instanceCombo.Location = new System.Drawing.Point(58, 7);
            this.instanceCombo.Size = new System.Drawing.Size(300, 21);
            this.instanceCombo.TabIndex = 2;
            this.instanceCombo.SelectedIndexChanged += serverNameChanged;
            //// 
            //// tagsComboBox
            //// 
            this.tagsComboBox.Location = new System.Drawing.Point(58, 7);
            this.tagsComboBox.Size = new System.Drawing.Size(300, 21);
            this.tagsComboBox.TabIndex = 0;
            this.tagsComboBox.Visible = false;
            // 
            // ServerUptime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Name = "ServerUptime";
            this.panel1.ResumeLayout(false);
            this.filterPanel.ResumeLayout(false);
            this.filterPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        private void serverNameChanged(object sender, EventArgs e)
        {
            if(instanceCombo.SelectedIndex > 0)
                serverName = instanceCombo.Items[instanceCombo.SelectedIndex].ToString();
        }

        #endregion
    }
}
