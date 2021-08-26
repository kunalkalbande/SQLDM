using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class AlertRecommendationOptionsDialog : Form
    {
        private const int DefaultWarningPercentage = 20;
        private const int DefaultCriticalPercentage = 30;

        public AlertRecommendationOptionsDialog()
        {
            InitializeComponent();
            warningPercentageOptionControl.Value = Settings.Default.AlertRecommendationWarningThesholdPercentage;
            criticalPercentageOptionControl.Value = Settings.Default.AlertRecommendationCriticalThesholdPercentage;
            AdaptFontSize();
        }

        private void restoreDefaultOptionsButton_Click(object sender, EventArgs e)
        {
            warningPercentageOptionControl.Value = DefaultWarningPercentage;
            criticalPercentageOptionControl.Value = DefaultCriticalPercentage;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (warningPercentageOptionControl.Value > criticalPercentageOptionControl.Value)
            {
                ApplicationMessageBox.ShowWarning(this,
                                                  "The Warning threshold recommendation percentage must be higher than that of the Critical percentage.");
                DialogResult = DialogResult.None;
            }
            else
            {
                Settings.Default.AlertRecommendationWarningThesholdPercentage =
                    Convert.ToInt32(warningPercentageOptionControl.Value);
                Settings.Default.AlertRecommendationCriticalThesholdPercentage =
                    Convert.ToInt32(criticalPercentageOptionControl.Value);
            }
        }

        private void AlertRecommendationOptionsDialog_HelpButtonClicked(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.AlertRecommendationsOptionsDialog);
        }

        private void AlertRecommendationOptionsDialog_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            if (hlpevent != null) hlpevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.AlertRecommendationsOptionsDialog);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}