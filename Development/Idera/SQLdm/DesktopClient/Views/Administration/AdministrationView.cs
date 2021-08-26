using System;
using System.Diagnostics;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Windows.Themes;
using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Views.Administration
{
    internal partial class AdministrationView : View
    {
        public enum AdministrationNode { Administration, ApplicationSecurity, CustomCounters, AuditedActions, ImportExport };//SQLdm 10.0 (Swati Gogia) added ImportExport to the enum

        private View applicationSecurityView = new ApplicationSecurityView();
        private View customCountersView = new CustomCountersView();
        private View auditedActions = new AuditedActionsView();
        private View importExport = new ImportExportView();//SQLdm 10.0 (Swati Gogia) added ImportExport 
        private AdministrationNode nodeShown = AdministrationNode.Administration;

        public AdministrationView()
        {
            InitializeComponent();

            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);

            // Autoscale font size.
            AdaptFontSize();
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
        }
        private void ScaleControlsAsPerResolution()
        {
            this.headerPanel.Size = new System.Drawing.Size(670, 75);
            this.pictureBox1.Size = new System.Drawing.Size(1332, 75);
            this.mainDdescriptionLabel.Location = new System.Drawing.Point(4, 76);
            this.mainDdescriptionLabel.AutoEllipsis = false;
            this.mainDdescriptionLabel.Size = new System.Drawing.Size(663, 65);
            this.dividerProgressBar2.Location = new System.Drawing.Point(185, 142);
            this.featureButtonAuditedActions.Location = new System.Drawing.Point(4, 366);
            this.featureButtonCustomCounter.Location = new System.Drawing.Point(3, 285);
            this.featureButtonImportExport.Location = new System.Drawing.Point(3, 448);
            this.featureButtonAppSecurity.Location = new System.Drawing.Point(3, 203);
        }
        public AdministrationNode NodeShown
        {
            get { return nodeShown; }
        }

        public void SetView(AdministrationNode node)
        {
            this.SuspendLayout();
            switch (node)
            {
                case AdministrationNode.Administration:
                    nodeShown = AdministrationNode.Administration;
                    headerStrip.HeaderImage = Idera.SQLdm.DesktopClient.Properties.Resources.AdministrationArea16x16;
                    titleLabel.Text = "Administration";
                    showAdminPanel();
                    break;
                case AdministrationNode.ApplicationSecurity:
                    nodeShown = AdministrationNode.ApplicationSecurity;
                    headerStrip.HeaderImage = Idera.SQLdm.DesktopClient.Properties.Resources.AppSecurity16x16;
                    titleLabel.Text = "Application Security";
                    showContentPanel(applicationSecurityView);
                    break;
                case AdministrationNode.CustomCounters:
                    nodeShown = AdministrationNode.CustomCounters;
                    headerStrip.HeaderImage = Idera.SQLdm.DesktopClient.Properties.Resources.CustomCounter16x16;
                    titleLabel.Text = "Custom Counters";
                    showContentPanel(customCountersView);
                    break;
                case AdministrationNode.AuditedActions:
                    nodeShown = AdministrationNode.AuditedActions;
                    headerStrip.HeaderImage = Idera.SQLdm.DesktopClient.Properties.Resources.ChangeLog16x16;
                    titleLabel.Text = "Change Log";
                    showContentPanel(auditedActions);
                    break;
                case AdministrationNode.ImportExport:
                    nodeShown = AdministrationNode.ImportExport;
                    headerStrip.HeaderImage = Idera.SQLdm.DesktopClient.Properties.Resources.ImportExport16x16;
                    titleLabel.Text = "Import Export Wizard";
                    showContentPanel(importExport);
                    break;
                default:
                    Debug.Assert(false, "Unknown administration node");
                    break;
            }
            this.ResumeLayout();
        }

        private void showAdminPanel()
        {
            adminPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            adminPanel.Visible = true;
            contentPanel.Visible = false;
        }

        private void showContentPanel(View v)
        {
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(v);
            contentPanel.Controls[0].Dock = System.Windows.Forms.DockStyle.Fill;
            contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            contentPanel.Visible = true;
            adminPanel.Visible = false;
        }

        public override void RefreshView()
        {
            if (contentPanel.Visible)
            {
                if (contentPanel.Controls.Count == 0)
                    SetView(AdministrationNode.Administration);
                else
                    ((View)contentPanel.Controls[0]).RefreshView();
            }
        }

        public override void ShowHelp()
        {
            if (contentPanel.Visible)
            {
                Debug.Assert(contentPanel.Controls.Count > 0);
                ((View)contentPanel.Controls[0]).ShowHelp();
            }
            else
            {
                Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.AdministrationView);
            }
        }

        private void featureButtonAppSecurity_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ApplicationController.Default.ShowAdministrationView(AdministrationNode.ApplicationSecurity);
        }

        private void featureButtonCustomCounter_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ApplicationController.Default.ShowAdministrationView(AdministrationNode.CustomCounters);
        }

        private void featureButtonImportExport_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ApplicationController.Default.ShowAdministrationView(AdministrationNode.ImportExport);
        }

        private void featureButtonAuditedActions_MouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            ApplicationController.Default.ShowAdministrationView(AdministrationNode.AuditedActions);
        }

        /// <summary>
        /// Auto scale the fontsize for the control, acording the current DPI resolution that has applied
        /// on the OS.
        /// </summary>
        protected void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            //START:DarkTheme 4.12 Icons Switching : Babita Manral
            if (Settings.Default.ColorScheme == "Dark")
            {
                this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
                this.pictureBox1.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.ReportColor);
            }
            else
            {
                this.pictureBox1.Image = global::Idera.SQLdm.DesktopClient.Properties.Resources.TodayPageHeader;
                this.pictureBox1.BackColor = Color.Transparent;
            }
            //END:DarkTheme 4.12 Icons Switching : Babita Manral
            this.featureButtonAppSecurity.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonCustomCounter.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonAuditedActions.HeaderColor = System.Drawing.Color.Red;
            this.featureButtonImportExport.HeaderColor = System.Drawing.Color.Red;
            Invalidate();
        }
    }
}

