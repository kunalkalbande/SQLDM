using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class GettingStartedWizard : Form
    {
        private bool _showPulseView;

        public GettingStartedWizard()
        {
            InitializeComponent();
            Icon = Resources.AppIcon;
            Text = Application.ProductName;
            AdaptFontSize();
            this.Load += new EventHandler(GettingStartedWizard_OnLoad);//SqlDm10.2 (Tushar)--Fix for SQLDM-27157
        }

        //Start-SqlDM 10.2 (Tushar)--Fix for SQLDM-27157
        private void GettingStartedWizard_OnLoad(object sender, EventArgs e)
        {
            this.TopMost = false;
        }
        //End-SqlDM 10.2 (Tushar)--Fix for SQLDM-27157

        public bool ShowPulseView
        {
            get { return _showPulseView; }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void configureAlertsFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                using (AlertConfigurationDialog alertConfigDialog = new AlertConfigurationDialog(0, true))
                {
                    alertConfigDialog.ShowDialog(this);
                }
            }
            catch (Exception ex)
            {
                ApplicationMessageBox.ShowError(this,
                                                "Unable to retrieve the alert configuration from the SQLDM Repository.  Please resolve the following error and try again.",
                                                ex);
            }
        }

        private void addNewServersFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            using (ManageServersDialog manageServersDialog = new ManageServersDialog())
            {
                manageServersDialog.ShowDialog(this);
            }
        }

        private void GettingStartedWizard_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null)
            {
                e.Cancel = true;
            }

            ShowHelpTopic();
        }

        private void GettingStartedWizard_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null)
            {
                hlpevent.Handled = true;
            }

            ShowHelpTopic();
        }

        private void ShowHelpTopic()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.GettingStartedWithSQLdm);
        }

        private void visitTrialCenterFeatureButton_MouseClick(object sender, MouseEventArgs e)
        {
            ApplicationController.Default.LaunchTrialCenterUrl();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        //private void _pulseGetStartedButton_MouseClick(object sender, MouseEventArgs e)
        //{
        //    ShowPulseGettingStarted();
        //}

        //private void ShowPulseGettingStarted()
        //{
        //    PulseGettingStartedDialog dialog = new PulseGettingStartedDialog(ManagementServiceHelper.ServerName);
        //    dialog.ShowInTaskbar = true;

        //    if (ApplicationModel.Default.UserToken.IsSQLdmAdministrator && !ApplicationModel.Default.IsPulseConfigured)
        //    {
        //        dialog.AddConfigurationStep(new PulsePlatformConnectionConfigurationControl());
        //        dialog.AddConfigurationStep(new PulseNotificationsConfigurationControl());
        //        dialog.SetMode(GettingStartedMode.Administrator);
        //    }
        //    else if (ApplicationModel.Default.IsPulseConfigured)
        //    {
        //        dialog.SetMode(GettingStartedMode.SignUp);
        //    }
        //    else
        //    {
        //        dialog.SetMode(GettingStartedMode.LearnMoreOnly);
        //    }

        //    if (dialog.ShowDialog(this) == DialogResult.OK)
        //    {
        //        _showPulseView = true;
        //        Close();
        //    }
        //}
    }
}