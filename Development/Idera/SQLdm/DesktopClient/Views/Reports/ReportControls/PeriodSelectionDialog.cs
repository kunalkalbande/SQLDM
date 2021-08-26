using System;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Views.Reports.ReportControls
{
    public partial class PeriodSelectionDialog : Form
    {
        public PeriodSelectionDialog(DateTime minimumDate, DateTime startDate, DateTime endDate)
        {
            InitializeComponent();
            startRangePicker.CalendarInfo.MinDate =
                endRangePicker.CalendarInfo.MinDate = minimumDate;
            DateTime endOfToday = DateTime.Now.Date.Add(new TimeSpan(23, 59, 59));
            startRangePicker.CalendarInfo.MaxDate =
                endRangePicker.CalendarInfo.MaxDate = endOfToday;
            startRangePicker.Value = startDate > minimumDate ? startDate : minimumDate;
            endRangePicker.Value = endDate < endOfToday ? endDate : endOfToday;

            // Auto scale font size.
            AdaptFontSize();
        }

        public DateTime SelectedStartDate
        {
            get { return ((DateTime)startRangePicker.Value).Date; }
        }

        public DateTime SelectedEndDate
        {
            get { return ((DateTime) endRangePicker.Value).Date.Add(new TimeSpan(23, 59, 59)); }
        }

        private void startRangePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime startRange = ((DateTime)startRangePicker.Value).Date;
            DateTime endRange = ((DateTime)endRangePicker.Value).Date;

            if (startRange > endRange)
            {
                startRangePicker.Value = endRange;
            }
        }

        private void endRangePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime startRange = ((DateTime)startRangePicker.Value).Date;
            DateTime endRange = ((DateTime)endRangePicker.Value).Date;

            if (endRange < startRange)
            {
                endRangePicker.Value = startRange;
            }
        }

        private void okBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
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