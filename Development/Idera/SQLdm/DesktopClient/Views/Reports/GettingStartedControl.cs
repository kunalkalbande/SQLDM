//-----------------------------------------------------------------------
// <copyright file="GettingStartedControl.cs" company="Idera">
//     Copyright (c) Idera, Inc. 2003-2015  All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Dialogs;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Reports
{
    public partial class GettingStartedControl : UserControl
    {
        private enum DiscoverPanels
        {
            Servers,
            Databases,
            Tables
        }

        private enum MonitorPanels
        {
            Servers, 
            Virtualization,
            SqlActivity
        }

        private DiscoverPanels activeDiscoverPanel = DiscoverPanels.Servers;
        private MonitorPanels activeMonitorPanel = MonitorPanels.Servers;
        private readonly Font analyzeLabelMouseEnterFont = new Font("Arial", 9F, FontStyle.Bold | FontStyle.Underline);
        private readonly Font analyzeLabelNormalFont = new Font("Arial", 9F, FontStyle.Bold);
        private readonly Color analyzeLabelSelectedFillColor = Color.FromArgb(228, 228, 228);
        private const int TodayPageHeaderGrowFactor = 18;

        public Color backColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
        public Color foreColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.Color.White : System.Drawing.Color.Black;
        public Color panelbackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.PanelBackColor) : System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(244)))));

        public GettingStartedControl()
        {
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
           
            InitializeComponent();

            backColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
            foreColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.Color.White : System.Drawing.Color.Black;
            panelbackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.PanelBackColor) : System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(244)))));

            SetBgColor();

            //Set "Activity" tab visible in false
            this.sqldmActivityLabel.Visible = false;
            UpdateUserToken();
            ApplicationController.Default.BackgroundRefreshCompleted += BackgroundRefreshCompleted;
            //Verify is the user is an administrator to let him see and use the Change log report
            VerifyIfAdminUser();

            //AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
            if (AutoScaleSizeHelper.scalingFactor == 1.25)
                ScaleControlsAsPerResolution25();
            if (AutoScaleSizeHelper.scalingFactor > 1.25)
                ScaleControlsAsPerResolution25andAbove();
        }
        private void ScaleControlsAsPerResolution25()
        {
            this.dynamicTableLayoutPanel.MinimumSize = new System.Drawing.Size(180, 700);
            this.label1.Size = new System.Drawing.Size(250, 28);
            this.label1.Font = new System.Drawing.Font("Verdana", 13.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Size = new System.Drawing.Size(200, 28);
            this.panel2.Size = new System.Drawing.Size(200, 70);

            this.dynamicTableLayoutPanel2.MinimumSize = new System.Drawing.Size(180, 700);
            this.label13.Size = new System.Drawing.Size(250, 28);
            this.label13.Font = new System.Drawing.Font("Verdana", 13.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Size = new System.Drawing.Size(200, 28);
            this.panel23.Size = new System.Drawing.Size(200, 70);

            this.dynamicTableLayoutPanel3.MinimumSize = new System.Drawing.Size(180, 700);
            this.label14.Size = new System.Drawing.Size(250, 28);
            this.label14.Font = new System.Drawing.Font("Verdana", 13.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Size = new System.Drawing.Size(200, 28);
            this.panel24.Size = new System.Drawing.Size(200, 70);
        }

        

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            backColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.ColorTranslator.FromHtml("#012A4F") : System.Drawing.Color.White;
            foreColor = Settings.Default.ColorScheme == "Dark" ? System.Drawing.Color.White : System.Drawing.Color.Black;
            panelbackColor = Settings.Default.ColorScheme == "Dark" ? ColorTranslator.FromHtml(DarkThemeColorConstants.PanelBackColor) : System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(239)))), ((int)(((byte)(244)))));
            SetBgColor();
        }

        private void SetBgColor()
        {
            this.BackColor = backColor;
            this.monitorTab.BackColor = backColor;
            this.analyzeTab.BackColor = backColor;
            this.planTab.BackColor = backColor;

            this.discoverPanel.BackColor = backColor;
            this.tableLayoutPanel1.BackColor = backColor;
            this.tableLayoutPanel2.BackColor = backColor;
            this.tableLayoutPanel3.BackColor = backColor;
            this.dynamicTableLayoutPanel.BackColor = backColor;
            this.dynamicTableLayoutPanel2.BackColor = backColor;
            this.dynamicTableLayoutPanel3.BackColor = backColor;

            this.analyzeServersButtonPanel.BackColor = backColor;
            this.analyzeServersLabel.BackColor = backColor;
            this.analyzeServersLabel.ForeColor = foreColor;

            this.headerPanel.BackColor = panelbackColor;
            this.headerPanel2.BackColor = panelbackColor;
            this.headerPanel3.BackColor = panelbackColor;
            this.headerPanel4.BackColor = panelbackColor;

        }

        private void ScaleControlsAsPerResolution25andAbove()
        {
            this.dynamicTableLayoutPanel.MinimumSize = new System.Drawing.Size(300, 1400);
            this.dynamicTableLayoutPanel.Location = new System.Drawing.Point(25, 180);
            this.dynamicTableLayoutPanel2.MinimumSize = new System.Drawing.Size(300, 1400);
            this.dynamicTableLayoutPanel2.Location = new System.Drawing.Point(35, 180);
            this.dynamicTableLayoutPanel3.MinimumSize = new System.Drawing.Size(300, 1400);
            this.dynamicTableLayoutPanel3.Location = new System.Drawing.Point(30, 180);

            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label2.Size = new System.Drawing.Size(350, 35);
            this.label1.Size = new System.Drawing.Size(350, 50);

            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label23.Size = new System.Drawing.Size(350, 35);
            this.label13.Size = new System.Drawing.Size(350, 50);


            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label24.Size = new System.Drawing.Size(350, 35);
            this.label14.Size = new System.Drawing.Size(350, 50);

            this.panel2.Size = new System.Drawing.Size(350, 130);
            this.panel24.Size = new System.Drawing.Size(350, 130);
            this.panel23.Size = new System.Drawing.Size(350, 130);

            this.headerPanel3.Size = new System.Drawing.Size(3000, 60);
            this.headerPanel2.Size = new System.Drawing.Size(3000, 60);
            this.headerPanel4.Size = new System.Drawing.Size(3000, 60);
            this.tabControl1.Padding = new Point((int)((float)15*AutoScaleSizeHelper.scalingFactor), (int)((float)5* AutoScaleSizeHelper.scalingFactor));

        }
        /// <summary>
        /// Verify if the user that are accessing to the Report tab is Administrator, 
        /// and if it is an administrator the method will put visible the "Activity" tab.
        /// </summary>
        private void VerifyIfAdminUser()
        {
            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
            {
                sqldmActivityLabel.Visible = true;
            }
        }

        private void BackgroundRefreshCompleted(object sender, BackgroundRefreshCompleteEventArgs e)
        {
            UpdateUserToken();
        }

        private void UpdateUserToken()
        {
            deployPanel.Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
        }

        private void reportButton_Load(object sender, EventArgs e)
        {
            if (sender is ReportSelectionButton)
            {
                ReportSelectionButton reportButton = (ReportSelectionButton)sender;
                string reportTypeString = reportButton.Tag as string;

                if (Enum.IsDefined(typeof(ReportTypes), reportTypeString))
                {
                    ReportTypes reportType = (ReportTypes) Enum.Parse(typeof (ReportTypes), reportTypeString, true);
                    reportButton.Title = ReportsHelper.GetReportTitle(reportType);
                    reportButton.Description = ReportsHelper.GetReportShortDescription(reportType);
                }
                reportButton.Invalidate(false);
            }
        }

        private void reportButton_MouseClick(object sender, MouseEventArgs e)
        {
            if (sender is ReportSelectionButton)
            {
                ReportSelectionButton reportButton = (ReportSelectionButton)sender;
                string reportType = reportButton.Tag as string;

                if (Enum.IsDefined(typeof(ReportTypes), reportType))
                {
                    ApplicationController.Default.ShowReportsView(
                        (ReportTypes)Enum.Parse(typeof(ReportTypes), reportType, true));
                }
            }
        }

        private void SwitchActiveDiscoverPanel(DiscoverPanels newActiveDiscoverPanel)
        {
            if (activeDiscoverPanel != newActiveDiscoverPanel)
            {
                activeDiscoverPanel = newActiveDiscoverPanel;
                analyzeServersButtonPanel.FillColor = activeDiscoverPanel == DiscoverPanels.Servers
                                                          ? analyzeLabelSelectedFillColor
                                                          : Color.White;
                analyzeDatabasesButtonPanel.FillColor = activeDiscoverPanel == DiscoverPanels.Databases
                                                          ? analyzeLabelSelectedFillColor
                                                          : Color.White;
                analyzeResourcesButtonPanel.FillColor = activeDiscoverPanel == DiscoverPanels.Tables
                                                          ? analyzeLabelSelectedFillColor
                                                          : Color.White;
                analyzeServersReportsPanel.Visible = activeDiscoverPanel == DiscoverPanels.Servers;
                analyzeDatabasesReportsPanel.Visible = activeDiscoverPanel == DiscoverPanels.Databases;
                analyzeResourcesReportsPanel.Visible = activeDiscoverPanel == DiscoverPanels.Tables;
            }
        }

        private void SwitchActiveMonitorPanel(MonitorPanels newActiveMonitorPanel)
        {
            if (activeMonitorPanel != newActiveMonitorPanel)
            {
                activeMonitorPanel = newActiveMonitorPanel;
                monitorServersButtonPanel.FillColor = activeMonitorPanel == MonitorPanels.Servers
                                                          ? analyzeLabelSelectedFillColor
                                                          : Color.White;
                monitorVirtualizationButtonPanel.FillColor = activeMonitorPanel == MonitorPanels.Virtualization
                                                                 ? analyzeLabelSelectedFillColor
                                                                 : Color.White;
                monitorSqldmActivityButtonPanel.FillColor = activeMonitorPanel == MonitorPanels.SqlActivity
                                                                 ? analyzeLabelSelectedFillColor
                                                                 : Color.White;
                monitorServerReportsPanel.Visible = activeMonitorPanel == MonitorPanels.Servers;
                monitorVirtualizationReportsPanel.Visible = activeMonitorPanel == MonitorPanels.Virtualization;
                monitorSqldmActivityReportsPanel.Visible = activeMonitorPanel == MonitorPanels.SqlActivity;
            }

            //if (newActiveMonitorPanel == MonitorPanels.Virtualization) Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.ReportsVirtualizationSection); //SQldm9.0 (Gaurav Karwal): for opening the help links on virtualization area in reports
        }

        private void deployReportsLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ICollection<Common.Objects.CustomReport> reports =
                Helpers.RepositoryHelper.GetCustomReportsList(
                    Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

            DeployReportsWizard wizard = new DeployReportsWizard(reports);

            wizard.ShowDialog(ParentForm);
        }

        private void analyzeLabel_MouseEnter(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                ((Label)sender).Font = analyzeLabelMouseEnterFont;
            }
        }

        private void analyzeLabel_MouseLeave(object sender, EventArgs e)
        {
            if (sender is Label)
            {
                ((Label)sender).Font = analyzeLabelNormalFont;
            }
        }

        private void analyzeServersLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveDiscoverPanel(DiscoverPanels.Servers);
        }

        private void analyzeDatabasesLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveDiscoverPanel(DiscoverPanels.Databases);
        }

        private void analyzeTablesLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveDiscoverPanel(DiscoverPanels.Tables);
        }

        private void monitorServersLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveMonitorPanel(MonitorPanels.Servers);
        }

        private void monitorVirtualizationLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveMonitorPanel(MonitorPanels.Virtualization);
        }

        private void monitorSqlActivityLabel_MouseClick(object sender, MouseEventArgs e)
        {
            SwitchActiveMonitorPanel(MonitorPanels.SqlActivity);
        }

        /// <summary>
        /// Occurs before a form is displayed for the first time.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GettingStartedControlLoad(object sender, EventArgs e)
        {
            // Adjust the canvas size for the header panel.
            //Size headerPanelSize = headerPanel.Size;
            //Size backgroundImageSize = headerPanel.BackgroundImage.Size;
            //headerPanel.Size = new Size(headerPanelSize.Width, backgroundImageSize.Height);
            //Invalidate();
        }
    }
}
