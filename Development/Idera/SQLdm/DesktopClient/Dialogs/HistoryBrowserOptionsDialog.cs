using System;
using System.Windows.Forms;
using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using System.ComponentModel;
using Idera.SQLdm.Common;
using Idera.SQLdm.DesktopClient.Properties;
using System.Drawing;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class HistoryBrowserOptionsDialog : BaseDialog
    {
        #region Constants

        private const string ERROR_FUTURE_TIME = "Future date and time is not allowed.";
        private const string ERROR_START_MORE_END = "Start time should be less than end time.";
        private const string ERROR_LIMIT = "Maximum of 1 year data can be selected.";
        private const int DEFAULT_COMBO_INDEX = 2;
        private const int DEFAULT_MAX_DAYS_ALLOWED = 366;
        private const int CUSTOM_OPTION_DATA_VALUE = 0;
        private const string DATE_CUSTOM_FORMAT = "MM/dd/yyyy"; // [SQLDM-27479] - Use MM/dd/yyyy format in 'History Browser Range' window
        private const string TIME_CUSTOM_FORMAT = "hh:mm tt"; // [SQLDM-27479] - Use MM/dd/yyyy format in 'History Browser Range' window

        #endregion

        public HistoryBrowserOptionsDialog()
        {
            this.DialogHeader = "History Browser Range";
            InitializeComponent();
            AdaptFontSize();
            RefreshScale();
            UpdateTheme();
        }
    
        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Initializes dialog using given settings.
        /// </summary>
        public void RefreshDialog()
       {
            beginDateDateTimePicker.ValueChanged -= DateTimePicker_ValueChanged;
            beginTimeDateTimePicker.ValueChanged -= DateTimePicker_ValueChanged;
            endDateDateTimePicker.ValueChanged -= DateTimePicker_ValueChanged;
            endTimeDateTimePicker.ValueChanged -= DateTimePicker_ValueChanged;
            
            endDateDateTimePicker.Value = beginDateDateTimePicker.Value = beginTimeDateTimePicker.Value = endTimeDateTimePicker.Value = DateTime.Now;
            var history = ApplicationModel.Default.HistoryTimeValue;
            if (history != null)
            {
                if (history.ViewMode == ServerViewMode.Custom)
                {
                    realtimeChartsHistoryLimitComboBox.SelectedIndex = realtimeChartsHistoryLimitComboBox.Items.Count - 1;
                }
                else
                {
                    RefreshScale();
                }
                    
                if (history.StartDateTime != null && history.EndDateTime != null)
                {
                    beginDateDateTimePicker.Value = beginTimeDateTimePicker.Value = history.StartDateTime.Value;
                    endTimeDateTimePicker.Value = endDateDateTimePicker.Value = history.EndDateTime.Value;
                    
                }
            }
            
            ValidateStartEndFilter(false);
            beginDateDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            beginTimeDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            endDateDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            endTimeDateTimePicker.ValueChanged += DateTimePicker_ValueChanged;
            UpdateTheme();
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Changes global state
        /// </summary>
        private void okButton_Click(object sender, EventArgs e)
        {
            int? dataValue = realtimeChartsHistoryLimitComboBox.SelectedItem.DataValue as int?;
            if (dataValue == null)
                    return;
            
            if (dataValue.Value == CUSTOM_OPTION_DATA_VALUE)
            {
                ApplicationModel.Default.HistoryTimeValue.SetCustomRange(
                    beginDateDateTimePicker.Value.Date.Add((beginTimeDateTimePicker.Value.TimeOfDay)),
                    endDateDateTimePicker.Value.Date.Add((endTimeDateTimePicker.Value.TimeOfDay)));
            }
            else
            {
                ApplicationModel.Default.HistoryTimeValue.SetVisibleMinutes(dataValue.Value);
            }
            
            ApplicationController.Default.PersistUserSettings(); // Persist user settings on background thread.
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Triggers custom dates validation on change.
        /// </summary>
        private void DateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            ValidateStartEndFilter();
        }

        /// <summary>
        ///  SqlDM 10.2 (Anshul Aggarwal) - Sets initial scale based on Settings value;
        /// </summary>
        private void RefreshScale()
        {
            realtimeChartsHistoryLimitComboBox.SelectedIndex = DEFAULT_COMBO_INDEX; //Default to 1 day.
            foreach (var item in realtimeChartsHistoryLimitComboBox.Items)
            {
                var data = item.DataValue as int?;
                if (data.HasValue && data.Value == ApplicationModel.Default.HistoryTimeValue.RealTimeMinutes)
                {
                    realtimeChartsHistoryLimitComboBox.SelectedItem = item;
                    break;

                }
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Enables/disables start-end dates based on custom selection.
        /// </summary>
        private void realtimeChartsHistoryLimitComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (realtimeChartsHistoryLimitComboBox.SelectedItem != null && (int)realtimeChartsHistoryLimitComboBox.SelectedItem.DataValue == CUSTOM_OPTION_DATA_VALUE)
            {
                beginDateDateTimePicker.Enabled = true;
                beginTimeDateTimePicker.Enabled = true;
                endDateDateTimePicker.Enabled = true;
                endTimeDateTimePicker.Enabled = true;
                ValidateStartEndFilter();
            }
            else
            {
                okButton.Enabled = true;
                beginDateDateTimePicker.Enabled = false;
                beginTimeDateTimePicker.Enabled = false;
                endDateDateTimePicker.Enabled = false;
                endTimeDateTimePicker.Enabled = false;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) : New History Browser
        /// Shows error message popup to user after validing custom start-end dates
        /// <param name="showMessage">Show error message popup to user</param>
        /// </summary>
        private void ValidateStartEndFilter(bool showMessage = true)
        {
            okButton.Enabled = true;
            var error = GetValidationError();
            if (error != null)
            {
                if (showMessage)
                    ApplicationMessageBox.ShowInfo(this, error);
                okButton.Enabled = false;
            }
        }

        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) : New History Browser
        /// Validates custom scale dates
        /// </summary>
        private string GetValidationError()
        {
            if (realtimeChartsHistoryLimitComboBox.SelectedItem != null && (int)realtimeChartsHistoryLimitComboBox.SelectedItem.DataValue == CUSTOM_OPTION_DATA_VALUE)
            {
                var startDate = beginDateDateTimePicker.Value.Date.Add((beginTimeDateTimePicker.Value.TimeOfDay));
                var endDate = endDateDateTimePicker.Value.Date.Add((endTimeDateTimePicker.Value.TimeOfDay));
                if (startDate > DateTime.Now || endDate > DateTime.Now)
                {
                    return ERROR_FUTURE_TIME;
                }
                else if (endDate < startDate)
                {
                    return ERROR_START_MORE_END;
                }
                else if (endDate.Subtract(startDate).TotalDays > DEFAULT_MAX_DAYS_ALLOWED)
                {
                    return ERROR_LIMIT;
                }
            }
            
            return null;
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        
        /// <summary>
        /// SqlDM 10.2 (Anshul Aggarwal) - Opens help link page for histor browser console options.
        /// </summary>
        private void HistoryBrowserOptionsDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.HistoryBrowserOptionsDialogHelp);
        }

        public void UpdateTheme()
        {
            if(Settings.Default.ColorScheme == "Dark")
            {
                Color backcolor = ColorTranslator.FromHtml(DarkThemeColorConstants.BackColor);
                Color forecolor = ColorTranslator.FromHtml(DarkThemeColorConstants.ForeColor);
                Color buttonBackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
                this.BackColor = backcolor;
                this.okButton.BackColor = buttonBackColor;
                this.okButton.FlatAppearance.BorderColor = buttonBackColor;
                this.okButton.FlatStyle = FlatStyle.Flat;
                this.okButton.ForeColor = forecolor;
                this.okButton.MouseEnter += new EventHandler(this.ok_button_MouseEnter);
                this.okButton.MouseLeave += new EventHandler(this.ok_button_MouseLeave);

                this.cancelButton.BackColor = buttonBackColor;
                this.cancelButton.FlatAppearance.BorderColor = buttonBackColor;
                this.cancelButton.FlatStyle = FlatStyle.Flat;
                this.cancelButton.ForeColor = forecolor;
                this.cancelButton.MouseEnter += new EventHandler(this.cancel_button_MouseEnter);
                this.cancelButton.MouseLeave += new EventHandler(this.cancel_button_MouseLeave);

            }
            else
            {
                this.BackColor = SystemColors.Window;
                this.okButton.BackColor = SystemColors.ButtonFace;
                this.okButton.FlatStyle = FlatStyle.System;
                this.okButton.TextAlign = ContentAlignment.MiddleCenter;

                this.cancelButton.BackColor = SystemColors.ButtonFace;
                this.cancelButton.FlatStyle = FlatStyle.System;
                this.cancelButton.TextAlign = ContentAlignment.MiddleCenter;
            }
        }


        private void ok_button_MouseEnter(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.okButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.HoveredActionButtonBackColor);
        }

        private void ok_button_MouseLeave(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.okButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
        }

        private void cancel_button_MouseEnter(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.cancelButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.HoveredActionButtonBackColor);
        }

        private void cancel_button_MouseLeave(object sender, EventArgs e)
        {
            OnMouseHover(e);
            this.cancelButton.BackColor = ColorTranslator.FromHtml(DarkThemeColorConstants.EnabledActionButtonBackColor);
        }
    }
}