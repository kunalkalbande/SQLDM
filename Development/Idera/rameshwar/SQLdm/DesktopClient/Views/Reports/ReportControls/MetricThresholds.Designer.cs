namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
   partial class MetricThresholds
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
         this.panel1.SuspendLayout();
         this.filterPanel.SuspendLayout();
         this.SuspendLayout();
         // 
         // panel1
         // 
         this.panel1.Size = new System.Drawing.Size(752, 67);
         // 
         // reportViewer
         // 
         this.reportViewer.Size = new System.Drawing.Size(752, 537);
         // 
         // filterPanel
         // 
         this.filterPanel.Size = new System.Drawing.Size(752, 67);
         // 
         // tagsLabel
         // 
         this.tagsLabel.Location = new System.Drawing.Point(342, 69);
         // 
         // instanceLabel
         // 
         this.instanceLabel.Location = new System.Drawing.Point(10, 10);
         // 
         // instanceCombo
         // 
         this.instanceCombo.Location = new System.Drawing.Point(70, 7);
         this.instanceCombo.Size = new System.Drawing.Size(300, 21);
         // 
         // tagsComboBox
         // 
         this.tagsComboBox.Location = new System.Drawing.Point(390, 66);
         // 
         // reportInstructionsControl
         // 
         this.reportInstructionsControl.ReportInstructions = "1. Select a SQL Server on which to report.\r\n2. Click Run Report.";
         this.reportInstructionsControl.Size = new System.Drawing.Size(752, 537);
         // 
         // MetricThresholds
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.Name = "MetricThresholds";
         this.panel1.ResumeLayout(false);
         this.filterPanel.ResumeLayout(false);
         this.filterPanel.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion
   }
}
