using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.AppStyling;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources;

namespace Idera.SQLdm.DesktopClient.Dialogs
{


    public enum ConsoleAlertNotificationDisplayOptions
    {
        AlwaysShow,
        ShowOnlyOnStateTransition,
        NeverShow
    }

    public partial class ConsoleOptionsDialog : Form
    {
        private int currentRealTimeChartsHistoryLimit;
        private int currentRealTimeChartsVisibleLimit;

        public ConsoleOptionsDialog()
        {
            InitializeComponent();
            AdaptFontSize();
        }

        private void notificationPreviewButton_Click(object sender, EventArgs e)
        {
            NotificationPopupWindow window = new NotificationPopupWindow();
            window.SetMessage("This is a preview of a Console Alert.");
            window.Show();
        }

        private void ConsoleOptionsDialog_Load(object sender, EventArgs e)
        {
            foregroundRefreshSpinner.Value = Settings.Default.ForegroundRefreshIntervalInSeconds;
            backgroundRefreshSpinner.Value = Settings.Default.BackgroundRefreshIntervalInMinutes;
            hideWhenMinimizedBox.Checked = Settings.Default.HideConsoleWhenMinimized;
            ///Persisting enable checkbox - Ankit Nagpal SQLdm 10.0
			baselineEnableBox.Checked = Settings.Default.EnableBaseline;
            realtimeChartsHistoryLimitComboBox.SelectedItem =
                realtimeChartsHistoryLimitComboBox.Items.ValueList.FindByDataValue(
                    Settings.Default.RealTimeChartHistoryLimitInMinutes);
            currentRealTimeChartsHistoryLimit = Settings.Default.RealTimeChartHistoryLimitInMinutes;

            currentRealTimeChartsVisibleLimit = ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes;

            AlertRowLimitSpinner.Value = Settings.Default.AlertRowLimit;

            ConsoleAlertNotificationDisplayOptions option = ConsoleAlertNotificationDisplayOptions.AlwaysShow;
            bool showPopupServerOk = false;
            bool showPopupCriticalOnly = false;
            bool showPopupAlways = true;

            try
            {
                option = (ConsoleAlertNotificationDisplayOptions)Settings.Default.ConsoleAlertNotificationDisplayOption;
                showPopupServerOk = Settings.Default.PopupShowPopupServerInfoWarCrit;
                showPopupCriticalOnly = Settings.Default.PopupShowCriticalServersOnly;
                showPopupAlways = Settings.Default.PopupShowAllServerStatuses;
                enableSoundForCriticalRadio.Checked = Settings.Default.PopupEnableCriticalAlertSound;
                enableSoundForAllRadio.Checked = !enableSoundForCriticalRadio.Checked;
            }
            finally
            {
                switch (option)
                {
                    case ConsoleAlertNotificationDisplayOptions.AlwaysShow:
                        consoleAlertNotificationOptionsAlwaysShow.Checked = true;
                        break;
                    case ConsoleAlertNotificationDisplayOptions.ShowOnlyOnStateTransition:
                        consoleAlertNotificationOptionsShowOnlyOnStateTransition.Checked = true;
                        break;
                    case ConsoleAlertNotificationDisplayOptions.NeverShow:
                        consoleAlertNotificationOptionsNeverShow.Checked = true;
                        break;
                }
                consoleAlertPopupShowInfoWarCritical.Checked = showPopupServerOk;
                consoleAlertPopupShowCriticalAlertOnly.Checked = showPopupCriticalOnly;
                consoleAlertPopupShowAllAlertStatuses.Checked = showPopupAlways;
            }

            string style = Settings.Default.ColorScheme;
            if (String.IsNullOrEmpty(style))
                style = "Office2007Black.isl";
            styleSelectionComboBox.SelectedItem =
                styleSelectionComboBox.Items.ValueList.FindByDataValue(style);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Settings.Default.HideConsoleWhenMinimized = hideWhenMinimizedBox.Checked;
            /// Ankit Nagpal SQLdm 10.0
			Settings.Default.EnableBaseline = baselineEnableBox.Checked;
            Settings.Default.ForegroundRefreshIntervalInSeconds = (int)foregroundRefreshSpinner.Value;
            Settings.Default.BackgroundRefreshIntervalInMinutes = (int)backgroundRefreshSpinner.Value;
            Settings.Default.RealTimeChartHistoryLimitInMinutes =
                (int)realtimeChartsHistoryLimitComboBox.SelectedItem.DataValue;
            Settings.Default.AlertRowLimit = (int)AlertRowLimitSpinner.Value;
            Settings.Default.PopupShowPopupServerInfoWarCrit = this.consoleAlertPopupShowInfoWarCritical.Checked;
            Settings.Default.PopupShowCriticalServersOnly = this.consoleAlertPopupShowCriticalAlertOnly.Checked;
            Settings.Default.PopupShowAllServerStatuses = this.consoleAlertPopupShowAllAlertStatuses.Checked;
            Settings.Default.PopupEnableCriticalAlertSound = this.enableSoundForCriticalRadio.Checked;
            if (consoleAlertNotificationOptionsAlwaysShow.Checked)
            {
                Settings.Default.ConsoleAlertNotificationDisplayOption =
                    Convert.ToInt32(consoleAlertNotificationOptionsAlwaysShow.Tag);
            }
            else if (consoleAlertNotificationOptionsShowOnlyOnStateTransition.Checked)
            {
                Settings.Default.ConsoleAlertNotificationDisplayOption =
                    Convert.ToInt32(consoleAlertNotificationOptionsShowOnlyOnStateTransition.Tag);
            }
            else if (consoleAlertNotificationOptionsNeverShow.Checked)
            {
                Settings.Default.ConsoleAlertNotificationDisplayOption =
                    Convert.ToInt32(consoleAlertNotificationOptionsNeverShow.Tag);
            }

            Settings.Default.ColorScheme = (string)styleSelectionComboBox.SelectedItem.DataValue;
        }

        private void realtimeChartsHistoryLimitComboBox_SelectionChanged(object sender, EventArgs e)
        {
            currentRealTimeChartsHistoryLimit = (int)realtimeChartsHistoryLimitComboBox.SelectedItem.DataValue;
        }

        protected override void OnHelpButtonClicked(System.ComponentModel.CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConsoleOptions);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.ConsoleOptions);
        }

        private void consoleAlertNotificationOptionsNeverShow_CheckedChanged(object sender, EventArgs e)
        {
            consoleAlertPopupShowCriticalAlertOnly.Enabled = !consoleAlertNotificationOptionsNeverShow.Checked;
            consoleAlertPopupShowInfoWarCritical.Enabled = !consoleAlertNotificationOptionsNeverShow.Checked;
            consoleAlertPopupShowAllAlertStatuses.Enabled = !consoleAlertNotificationOptionsNeverShow.Checked;
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
