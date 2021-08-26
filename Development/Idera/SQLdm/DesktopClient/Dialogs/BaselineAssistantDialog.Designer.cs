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
using Idera.SQLdm.DesktopClient.Controls.CustomControls;
using Infragistics.Windows.Themes;

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
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged1);

            this.SelectNumericMertic = new CustomComboBox();
            this.SelectNoOfWeeks = new CustomComboBox();
            this.ChooseMetricLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.ChooseWeekLabel = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Monday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Tuesday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Wednesday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Thrusday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Friday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Saturday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Sunday = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.Dummy = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel();
            this.chart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();
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
            this.Friday.AutoSize = true;
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
            setChartBackColor();
            
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
            //this.Controls.Add(this.Dummy);
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

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox SelectNumericMertic;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomComboBox SelectNoOfWeeks;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel ChooseMetricLabel;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel ChooseWeekLabel;

        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Monday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Tuesday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Wednesday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Thrusday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Friday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Saturday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Sunday;
        private Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomLabel Dummy;

        private ChartFX.WinForms.Chart chart = new Idera.SQLdm.DesktopClient.Controls.CustomControls.CustomChart();

        private void setChartBackColor()
        {
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.chart.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
            }
            else
            {
                this.chart.BackColor = Color.White;
            }
            
        }

        void OnCurrentThemeChanged1(object sender, EventArgs e)
        {
            Invalidate();
            setChartBackColor();
        }
    }
}