//SQLdm 10.0 (Tarun Sapra) - Displaying a dialog for the baseline definition assistant 

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using ChartFX.WinForms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.UltraWinToolbars;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    partial class BaselineAssistantDialog
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
            this.SelectNumericMertic = new System.Windows.Forms.ComboBox();
            this.SelectNoOfWeeks = new System.Windows.Forms.ComboBox();
            this.ChooseMetricLabel = new System.Windows.Forms.Label();
            this.ChooseWeekLabel = new System.Windows.Forms.Label();
            this.Monday = new System.Windows.Forms.Label();
            this.Tuesday = new System.Windows.Forms.Label();
            this.Wednesday = new System.Windows.Forms.Label();
            this.Thrusday = new System.Windows.Forms.Label();
            this.Friday = new System.Windows.Forms.Label();
            this.Saturday = new System.Windows.Forms.Label();
            this.Sunday = new System.Windows.Forms.Label();
            this.Dummy = new System.Windows.Forms.Label();
            this.chart = new ChartFX.WinForms.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // SelectNumericMertic
            // 
            this.SelectNumericMertic.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectNumericMertic.FormattingEnabled = true;
            this.SelectNumericMertic.Location = new System.Drawing.Point(12, 32);
            this.SelectNumericMertic.Name = "SelectNumericMertic";
            this.SelectNumericMertic.Size = new System.Drawing.Size(280, 21);
            this.SelectNumericMertic.TabIndex = 0;
            this.SelectNumericMertic.SelectedIndexChanged += new System.EventHandler(this.DropDownSelectionChanged);
            // 
            // SelectNoOfWeeks
            // 
            this.SelectNoOfWeeks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectNoOfWeeks.FormattingEnabled = true;
            this.SelectNoOfWeeks.Items.AddRange(new object[] {
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8});
            this.SelectNoOfWeeks.Location = new System.Drawing.Point(12, 82);
            this.SelectNoOfWeeks.Name = "SelectNoOfWeeks";
            this.SelectNoOfWeeks.Size = new System.Drawing.Size(121, 21);
            this.SelectNoOfWeeks.TabIndex = 0;
            this.SelectNoOfWeeks.SelectedIndexChanged += new System.EventHandler(this.DropDownSelectionChanged);
            // 
            // ChooseMetricLabel
            // 
            this.ChooseMetricLabel.AutoSize = true;
            this.ChooseMetricLabel.Location = new System.Drawing.Point(12, 13);
            this.ChooseMetricLabel.Name = "ChooseMetricLabel";
            this.ChooseMetricLabel.Size = new System.Drawing.Size(75, 13);
            this.ChooseMetricLabel.TabIndex = 1;
            this.ChooseMetricLabel.Text = "Choose Metric";
            // 
            // ChooseWeekLabel
            // 
            this.ChooseWeekLabel.AutoSize = true;
            this.ChooseWeekLabel.Location = new System.Drawing.Point(12, 62);
            this.ChooseWeekLabel.Name = "ChooseWeekLabel";
            this.ChooseWeekLabel.Size = new System.Drawing.Size(111, 13);
            this.ChooseWeekLabel.TabIndex = 1;
            this.ChooseWeekLabel.Text = "Choose No Of Weeks";
            // 
            // Monday
            // 
            this.Monday.AutoSize = true;
            this.Monday.BackColor = System.Drawing.Color.White;
            this.Monday.Location = new System.Drawing.Point(425, 353);
            this.Monday.Name = "Monday";
            this.Monday.Size = new System.Drawing.Size(0, 13);
            this.Monday.TabIndex = 1;
            // 
            // Tuesday
            // 
            this.Tuesday.AutoSize = true;
            this.Tuesday.BackColor = System.Drawing.Color.White;
            this.Tuesday.Location = new System.Drawing.Point(500, 353);
            this.Tuesday.Name = "Tuesday";
            this.Tuesday.Size = new System.Drawing.Size(0, 13);
            this.Tuesday.TabIndex = 1;
            // 
            // Wednesday
            // 
            this.Wednesday.AutoSize = true;
            this.Wednesday.BackColor = System.Drawing.Color.White;
            this.Wednesday.Location = new System.Drawing.Point(585, 353);
            this.Wednesday.Name = "Wednesday";
            this.Wednesday.Size = new System.Drawing.Size(0, 13);
            this.Wednesday.TabIndex = 1;
            // 
            // Thrusday
            // 
            this.Thrusday.AutoSize = true;
            this.Thrusday.BackColor = System.Drawing.Color.White;
            this.Thrusday.Location = new System.Drawing.Point(680, 353);
            this.Thrusday.Name = "Thrusday";
            this.Thrusday.Size = new System.Drawing.Size(0, 13);
            this.Thrusday.TabIndex = 1;
            // 
            // Friday
            // 
            this.Friday.BackColor = System.Drawing.Color.White;
            this.Friday.Location = new System.Drawing.Point(760, 353);
            this.Friday.Name = "Friday";
            this.Friday.Size = new System.Drawing.Size(50, 13);
            this.Friday.TabIndex = 1;
            // 
            // Saturday
            // 
            this.Saturday.AutoSize = true;
            this.Saturday.BackColor = System.Drawing.Color.White;
            this.Saturday.Location = new System.Drawing.Point(830, 353);
            this.Saturday.Name = "Saturday";
            this.Saturday.Size = new System.Drawing.Size(0, 13);
            this.Saturday.TabIndex = 1;
            // 
            // Sunday
            // 
            this.Sunday.AutoSize = true;
            this.Sunday.BackColor = System.Drawing.Color.White;
            this.Sunday.Location = new System.Drawing.Point(910, 353);
            this.Sunday.Name = "Sunday";
            this.Sunday.Size = new System.Drawing.Size(0, 13);
            this.Sunday.TabIndex = 1;
            // 
            // Dummy
            // 
            this.Dummy.BackColor = System.Drawing.Color.White;
            this.Dummy.Location = new System.Drawing.Point(935, 353);
            this.Dummy.Name = "Dummy";
            this.Dummy.Size = new System.Drawing.Size(100, 13);
            this.Dummy.TabIndex = 1;
            this.Dummy.Text = "           ";
            // 
            // chart
            // 
            this.chart.AllSeries.Gallery = ChartFX.WinForms.Gallery.Scatter;
            this.chart.AxisX.Title.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chart.AxisX.Visible = false;
            this.chart.AxisY.Title.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.chart.Location = new System.Drawing.Point(310, 32);
            this.chart.Name = "chart";
            this.chart.Size = new System.Drawing.Size(770, 350);
            this.chart.TabIndex = 2;
            // 
            // BaselineAssistantDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 400);
            this.Controls.Add(this.Monday);
            this.Controls.Add(this.Tuesday);
            this.Controls.Add(this.Wednesday);
            this.Controls.Add(this.Thrusday);
            this.Controls.Add(this.Friday);
            this.Controls.Add(this.Saturday);
            this.Controls.Add(this.Sunday);
            this.Controls.Add(this.Dummy);
            this.Controls.Add(this.ChooseMetricLabel);
            this.Controls.Add(this.SelectNumericMertic);
            this.Controls.Add(this.SelectNoOfWeeks);
            this.Controls.Add(this.ChooseWeekLabel);
            this.Controls.Add(this.chart);
            this.HelpButton = true;
            this.Name = "BaselineAssistantDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Baseline Definition Visualizer";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.BaselineAssistantDialog_HelpButtonClicked);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.BaselineAssistantDialog_HelpRequested);
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox SelectNumericMertic;
        private System.Windows.Forms.ComboBox SelectNoOfWeeks;
        private System.Windows.Forms.Label ChooseMetricLabel;
        private System.Windows.Forms.Label ChooseWeekLabel;

        private System.Windows.Forms.Label Monday;
        private System.Windows.Forms.Label Tuesday;
        private System.Windows.Forms.Label Wednesday;
        private System.Windows.Forms.Label Thrusday;
        private System.Windows.Forms.Label Friday;
        private System.Windows.Forms.Label Saturday;
        private System.Windows.Forms.Label Sunday;
        private System.Windows.Forms.Label Dummy;

        private ChartFX.WinForms.Chart chart = new ChartFX.WinForms.Chart();
    }
}