using System;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Dialogs.Notification;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Win.AppStyling;
using Idera.SQLdm.DesktopClient.Views.Servers.Server.Resources;
using Idera.SQLdm.DesktopClient.Controls;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Dialogs
{


    public enum ConsoleAlertNotificationDisplayOptions
    {
        AlwaysShow,
        ShowOnlyOnStateTransition,
        NeverShow
    }

    public partial class ConsoleOptionsDialog : BaseDialog
    {
        private int currentRealTimeChartsHistoryLimit;
        private int currentRealTimeChartsVisibleLimit;
        private String previousTheme;

        public ConsoleOptionsDialog()
        {
            this.DialogHeader = "Console Options";
            InitializeComponent();
            SetPropertiesTheme();
            AdaptFontSize();
            previousTheme = Settings.Default.ColorScheme;
            //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
            if (AutoScaleSizeHelper.isScalingRequired)
                ScaleControlsAsPerResolution();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
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
            styleSelectionComboBox.SelectedItem = styleSelectionComboBox.Items.ValueList.FindByDataValue(style);
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

            MainWindow mainWindow = new MainWindow();
            Settings.Default.ColorScheme = (string)styleSelectionComboBox.SelectedItem.DataValue;
            if(previousTheme == Settings.Default.ColorScheme)
            {
                return;
            }
            previousTheme = Settings.Default.ColorScheme;
            if (Settings.Default.ColorScheme == ThemeName.Light.ToString())
            {
                mainWindow.MenuLight_Click();
            }
            else if (Settings.Default.ColorScheme == ThemeName.Dark.ToString())
            {
                mainWindow.MenuDark_Click();
            }
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

        //Saurabh - SQLDM-30848 - UX-Modernization, PRD 4.2
        private void ScaleControlsAsPerResolution()
        {
            if (AutoScaleSizeHelper.isLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel3, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.2F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel2, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.5F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel1, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.2F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel1, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.5F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel2, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.5F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel3, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.5F, 1.0F), false);
                this.noteLabel.Width += 200;
                //this.notificationPreviewButton.Anchor = AnchorStyles.Right;
                this.notificationPreviewButton.Size = new System.Drawing.Size(100, 40);
                this.realtimeChartsHistoryLimitComboBox.Width += 50;
                return;
            }
            if (AutoScaleSizeHelper.isXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel3, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.5F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel2, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.75F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel1, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.25F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel1, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.75F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel2, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.5F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel3, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.5F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.notificationPreviewButton, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.5F, 1.25F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.backgroundRefreshSpinner, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.foregroundRefreshSpinner, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.styleSelectionComboBox, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.realtimeChartsHistoryLimitComboBox, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                this.noteLabel.Width += 300;
                this.notificationPreviewButton.Size = new System.Drawing.Size(120, 50);
                this.realtimeChartsHistoryLimitComboBox.Width += 50;
                //this.notificationPreviewButton.Anchor = AnchorStyles.Right;
                //this.notificationPreviewButton.Width -= 600;
                //this.notificationPreviewButton.Height -= 50;
                return;
            }
            if (AutoScaleSizeHelper.isXXLargeSize)
            {
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel3, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.75F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel2, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.75F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.tableLayoutPanel1, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.0F, 1.33F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel1, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.85F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel2, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.75F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.panel3, AutoScaleSizeHelper.ControlType.Form, new System.Drawing.SizeF(1.75F, 1.0F), false);
                AutoScaleSizeHelper.Default.AutoScaleControl(this.notificationPreviewButton, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.65F, 1.25F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.backgroundRefreshSpinner, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.foregroundRefreshSpinner, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.styleSelectionComboBox, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                AutoScaleSizeHelper.Default.AutoScaleControl(this.realtimeChartsHistoryLimitComboBox, AutoScaleSizeHelper.ControlType.Control, new System.Drawing.SizeF(1.75F, 1.0F));
                this.noteLabel.Width += 400;
                this.notificationPreviewButton.Size = new System.Drawing.Size(140, 60);
                this.realtimeChartsHistoryLimitComboBox.Width += 50;
                //this.notificationPreviewButton.Anchor = AnchorStyles.Right;
                //this.notificationPreviewButton.Width -= 800;
                //this.notificationPreviewButton.Height -= 50;
                return;
            }
        }
    }
}
