using System;
using System.ComponentModel;
using System.Windows.Forms;

using Idera.Newsfeed.Plugins.Objects;
using Idera.Newsfeed.Plugins.UI;
using Idera.Newsfeed.Plugins.UI.Dialogs;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.DesktopClient.Controls;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.Common.Services;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Views.Pulse
{
    internal partial class PulseView : View
    {
        Label osUnsupportedLabel;
        private LinkLabel _newsfeedLinkLabelRequirements;
        private Panel osUnsupportedPanel;
        private static readonly BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);

        public PulseView()
        {
                System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                InitializeComponent();
                PulseController.Default.Initialize(this, Settings.Default.PulseAccountName, Settings.Default.PulseAccountKeepLoggedIn);
                PulseController.Default.MoreInfoRequested += PulseController_MoreInfoRequested;
                PulseController.Default.ApplicationActionRequested += PulseController_ApplicationActionRequested;
                Settings.Default.ActiveRepositoryConnectionChanged += Settings_ActiveRepositoryConnectionChanged;

                osUnsupportedPanel.BackColor = Color.White;
                osUnsupportedPanel.Visible = false;
                osUnsupportedPanel.Dock = DockStyle.Fill;
                osUnsupportedPanel.AutoSize = true;
                osUnsupportedPanel.Location = new System.Drawing.Point(0, 0);
                osUnsupportedPanel.Size = new System.Drawing.Size(578, 503);

                osUnsupportedLabel.Text = "IDERA Newsfeed is not supported on this operating system.";
                osUnsupportedLabel.BackColor = Color.White;
                osUnsupportedLabel.Visible = true;
                osUnsupportedLabel.AutoSize = true;
                osUnsupportedLabel.Location = new Point((this.Width / 2) - osUnsupportedLabel.Width, this.Height / 2);

                _newsfeedLinkLabelRequirements.AutoSize = true;
                _newsfeedLinkLabelRequirements.TabStop = true;
                _newsfeedLinkLabelRequirements.Text = "IDERA Newsfeed requirements";
                _newsfeedLinkLabelRequirements.Visible = true;
                _newsfeedLinkLabelRequirements.BackColor = Color.Transparent;
                _newsfeedLinkLabelRequirements.LinkClicked += new LinkLabelLinkClickedEventHandler(this.NewsfeedRequirements_LinkClicked);
                _newsfeedLinkLabelRequirements.Location = new Point(osUnsupportedLabel.Location.X + osUnsupportedLabel.PreferredWidth, this.Height / 2);

                osUnsupportedPanel.Controls.Add(osUnsupportedLabel);
                osUnsupportedPanel.Controls.Add(_newsfeedLinkLabelRequirements);
                this.Controls.Add(osUnsupportedPanel);
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by PulseView : {0}",stopWatch.ElapsedMilliseconds);
        }

        private void NewsfeedRequirements_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.SQLdmMobileNewsfeedRequirements);//SQLdm9.0 (Gaurav Karwal): changed to use idera wiki
        }

        public override void ShowHelp()
        {
            PulseController.Default.ShowHelp();
        }

        private void PulseController_MoreInfoRequested(object sender, HandledEventArgs e)
        {
            if (ApplicationController.Default.ActiveView != this || e.Handled) return;
            ShowPulseGettingStarted();
            e.Handled = true;
        }

        private void Settings_ActiveRepositoryConnectionChanged(object sender, EventArgs e)
        {
            ApplicationModel.Default.InitializePulse();
            ApplicationModel.Default.RefreshPulseConfiguration();
        }

        private void PulseController_ApplicationActionRequested(object sender, ApplicationActionRequestedEventArgs e)
        {
            if (e.Action == "$Metric")
            {
                int metricId;
                if (int.TryParse(e.ActionData.ToString(), out metricId))
                {
                    Metric metric = MetricDefinition.GetMetric(metricId);
                    switch (metric)
                    {
                        case Metric.Deadlock:
                            AlertFilter metricFilter = new AlertFilter();
                            metricFilter.Metric = Metric.Deadlock;
                            ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ByMetric, metricFilter);
                            break;
                        default:
                            ClickThroughHelper.NavigateToView(e.Server, metric, null);
                            break;
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(this, string.Format("The requested metric '{0}' has no associated view.", e.ActionData));
                }
            }
            else if (e.Action == "ConfigurePulseNotifications")
            {
                if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator)
                {
                    IManagementService managementService = ManagementServiceHelper.GetDefaultService(Settings.Default.ActiveRepositoryConnection.ConnectionInfo);

                    using (NotificationRulesDialog notificationRulesDialog = new NotificationRulesDialog(managementService))
                    {
                        notificationRulesDialog.ShowDialog(ParentForm);
                    }
                }
                else
                {
                    ApplicationMessageBox.ShowInfo(this, string.Format("You do not have permission to configure application notifications.  Contact your SQL Diagnostic Manager administrator to change notification settings for Newsfeed.", e.Action));
                }
            }
            else if(e.Action == "ShowHelp")
            {
                //[START] SQLdm 9.0 (Gaurav Karwal): force changing the url to wiki as data coming from newsfeed plugin.
                // Correcting the behavior: it was opposite, install link used to open requriements and vice-versa
                if ((string) e.ActionData == "SQLdm Mobile Requirements.htm") ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.SQLdmMobileNewsfeedInstallationRequirements);
                else if ((string)e.ActionData == "Install SQLdm Mobile.htm") ApplicationHelper.ShowHelpTopic(Idera.SQLdm.Common.HelpTopics.SQLdmMobileNewsfeedRequirements);
                else ApplicationHelper.ShowHelpTopic((string)e.ActionData);
                //ApplicationHelper.ShowHelpTopic((string) e.ActionData);
                // Correcting the behavior: it was opposite, install link used to open requriements and vice-versa
                //[END] SQLdm 9.0 (Gaurav Karwal): force changing the url to wiki as data coming from newsfeed plugin
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this, string.Format("The requested action '{0}' will not be tolerated.", e.Action));
            }
        }

        private void ShowPulseGettingStarted()
        {
            PulseGettingStartedDialog dialog = new PulseGettingStartedDialog(ManagementServiceHelper.ServerName);
            dialog.StartPosition = FormStartPosition.CenterParent;

            if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator && !ApplicationModel.Default.IsPulseConfigured)
            {
                dialog.AddConfigurationStep(new PulsePlatformConnectionConfigurationControl());
                dialog.AddConfigurationStep(new PulseNotificationsConfigurationControl());
                dialog.SetMode(GettingStartedMode.Administrator);
            }
            else if (ApplicationModel.Default.IsPulseConfigured)
            {
                dialog.SetMode(GettingStartedMode.SignUp);
            }
            else
            {
                dialog.SetMode(GettingStartedMode.LearnMoreOnly);
            }

            dialog.ShowDialog(this);
        }

        public void ShowPulseProfile(int ObjectId)
        {
            PulseController.Default.ShowProfileView(PulseSubViews.PF_Wall, ObjectId);
        }

        public override void RefreshView()
        {
            RefreshView(false);
        }

        public void RefreshView(bool isBackgroundRefresh)
        {
            Version osversion = Environment.OSVersion.Version;

            if (osversion.Major == 5 && osversion.Minor == 0)
            {
                osUnsupportedPanel.Visible = true;
                osUnsupportedPanel.BringToFront();
                return;
            }

            PulseController.Default.RefreshActiveView(isBackgroundRefresh);
        }
        
        private void PulseView_Resize(object sender, EventArgs e)
        {
            if (osUnsupportedLabel != null && _newsfeedLinkLabelRequirements != null)
            {
                if (this.Width > osUnsupportedLabel.Width + _newsfeedLinkLabelRequirements.Width)
                {
                    osUnsupportedLabel.Location =
                        new Point((this.Width/2) - (osUnsupportedLabel.Width + _newsfeedLinkLabelRequirements.Width)/2,
                                  this.Height/2);
                    _newsfeedLinkLabelRequirements.Location =
                        new Point(osUnsupportedLabel.Location.X + osUnsupportedLabel.Width,
                                  this.Height/2);
                }
            }
        }
    }
}

