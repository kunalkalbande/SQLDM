using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.UI.Dialogs;
using dmResources = Idera.SQLdm.DesktopClient.Properties.Resources ;
using Infragistics.Win.UltraWinSchedule;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.Common.Objects;
using BBS.TracerX;

namespace Idera.SQLdm.DesktopClient.Views.Reports  {
    /// <summary>
    /// This is the dialog that is displayed when the user picks the "Custom" option
    /// for the duration/period in the report parameters pane.
    /// </summary>
    internal partial class ReportPeriodDialog : Dialogs.BaseDialog {
        /// <summary>
        /// The servers determine the minimum date that can be selected.
        /// </summary>
        public ReportPeriodDialog(List<MonitoredSqlServer> servers) {
            using (Log.DebugCall()) {
                InitializeComponent();
                SetMinDates(servers);
                ultraMonthViewSingle1.CalendarLook.SelectedDayAppearance.BackColor = Color.Orange;
                ultraMonthViewSingle1.CalendarInfo.AfterSelectedDateRangeChange += CalendarInfo_AfterSelectedDateRangeChange;

                // Start with the Days view.
                radioButton1_CheckedChanged(null, null);
            }

            // Apply auto scale font size.
            AdaptFontSize();
        }

        private static readonly Logger Log = Logger.GetLogger("ReportPeriodDialog");

        // The earliest registration date of the servers.
        private DateTime _firstDate; 

        // This is the basis for calculating which month or year
        // is selected.  For example, if the third month is
        // selected, it is _lastDate.AddMonths(-2).
        private DateTime _lastDate = DateTime.Today;

        // Set the earliest day, month, and year the user can select
        // based on the dates the servers were registered.
        private void SetMinDates(List<MonitoredSqlServer> servers) {
            _firstDate = servers[0].EarliestData;

            foreach (MonitoredSqlServer server in servers) {
                if (server.EarliestData < _firstDate) _firstDate = server.EarliestData;
            }
            
            _firstDate = _firstDate.ToLocalTime().Date;
            Log.Debug("_firstDate set to ", _firstDate);

            // days
            ultraMonthViewSingle1.CalendarInfo.MinDate = _firstDate;
            ultraMonthViewSingle1.CalendarInfo.MaxDate = _lastDate;

            // Months
            int firstMonth = _firstDate.Year * 12 + _firstDate.Month; // Absolute months since year 0000. 
            for (DateTime candidate = _lastDate;
                (candidate.Year * 12 + candidate.Month) >= firstMonth;
                candidate = candidate.AddMonths(-1)) {
                monthsListBox.Items.Add(candidate.ToString("MMMM yyyy"));
            }

            // Years
            for (int year = _lastDate.Year; year >= _firstDate.Year; --year) {
                yearsListBox.Items.Add(year);
            }

            // Range
            fromPicker.MinDate = _firstDate;
        }

        public static ReportPeriodType GetPeriodType(string str) {
            try {
                if (str.StartsWith(dmResources.PeriodTypeDays)) return ReportPeriodType.Days;
                if (str.StartsWith(dmResources.PeriodTypeMonths)) return ReportPeriodType.Months;
                if (str.StartsWith(dmResources.PeriodTypeYears)) return ReportPeriodType.Years;
                if (str.StartsWith(dmResources.PeriodTypeRange)) return ReportPeriodType.Range;
            } catch (Exception ex) {
                Log.Error("GetPeriodType failed", ex);
            }

            return ReportPeriodType.Invalid;
        }

        /// <summary>
        /// Get a string representing the user's selections.
        /// The returned string is displayed to the user and
        /// must be parseable by ParseString.
        /// </summary>
        public string GetString() {
            if (daysRadio.Checked)   return GetDaysString();
            if (monthsRadio.Checked) return GetMonthsString();
            if (yearsRadio.Checked)  return GetYearsString();
            if (rangeRadio.Checked)  return GetRangeString();
            throw new ApplicationException("None of the expected radio buttons are checked.");
        }

        /// <summary>
        /// Returns the list of dates corresponding to the current selection.
        /// </summary>
        /// <returns></returns>
        public List<DateRangeOffset> GetDates() {
            if (daysRadio.Checked) return GetDatesForDays();
            if (monthsRadio.Checked) return GetDatesForMonths();
            if (yearsRadio.Checked) return GetDatesForYears();
            if (rangeRadio.Checked) return GetDatesForRange();
            throw new ApplicationException("None of the expected radio buttons are checked.");
        }

        /// <summary>
        /// Attempts to select the appropriate radio button and initialize
        /// the corresponding controls by parsing the specified string.
        /// This should succeed with any string returned by GetString(), but
        /// the user will have had a chance to modify the string first.
        /// This implementation gives up at the first sign of trouble.
        /// </summary>
        public void ParseString(string str) {
            try {
                switch (GetPeriodType(str)) {
                    case ReportPeriodType.Days:
                        ParseDaysString(str);
                        break;
                    case ReportPeriodType.Months:
                        ParseMonthsString(str);
                        break;
                    case ReportPeriodType.Years:
                        ParseYearsString(str);
                        break;
                    case ReportPeriodType.Range:
                        ParseRangeString(str);
                        break;
                }
            } catch (Exception ex) {
                Log.Error("ParseString failed.", ex);
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        #region Range

        private string GetRangeString() {
            string ret = dmResources.PeriodTypeRange;
            ret += " " + fromPicker.Value.ToShortDateString() + " - " + toPicker.Value.ToShortDateString();
            return ret;
        }

        private List<DateRangeOffset> GetDatesForRange() {
            List<DateRangeOffset> list = new List<DateRangeOffset>();
            DateRangeOffset.AddDateRange(list, fromPicker.Value.Date, toPicker.Value.Date.AddDays(1));
            return list;
        }

        private void ParseRangeString(string str) {
            // Format is like "Range: date1 - date2" where
            // the dates are in the ToShortDateString format.
            // We already know the string starts with "Range: " (localized).

            rangeRadio.Checked = true;
            str = str.Substring(dmResources.PeriodTypeRange.Length);
            string[] range = str.Split(new char[] { '-' });
            if (range.Length == 2) {
                string start = range[0].Trim();
                fromPicker.Value = DateTime.Parse(start);

                string end = range[1].Trim();
                toPicker.Value = DateTime.Parse(end);
            }
        }

        #endregion Range

        #region Days
        private string GetDaysString() {
            string ret = dmResources.PeriodTypeDays;
            foreach (DateRange range in ultraMonthViewSingle1.CalendarInfo.SelectedDateRanges) {
                if (range.Days.Count == 1) {
                    ret += " " + range.FirstDay.Date.ToShortDateString() + ",";
                } else if (range.Days.Count == 2) {
                    ret += " " + range.FirstDay.Date.ToShortDateString() + ", " + range.LastDay.Date.ToShortDateString() + ",";
                } else {
                    ret += " " + range.FirstDay.Date.ToShortDateString() + " - " + range.LastDay.Date.ToShortDateString() + ",";
                }
            }

            // Trim off the last ",".
            ret = ret.TrimEnd(new char[] { ',' });

            return ret;
        }

        private List<DateRangeOffset> GetDatesForDays() {
            List<DateRangeOffset> list = new List<DateRangeOffset>();

            foreach (DateRange range in ultraMonthViewSingle1.CalendarInfo.SelectedDateRanges) {
                // Add one day to the last day in each range to include all times of that day.
                DateRangeOffset.AddDateRange(list, range.FirstDay.Date, range.LastDay.Date.AddDays(1));
            }

            return list;
        }

        private void ParseDaysString(string str) {
            // Format is like "Days: date1, date2, date3 - date4" where
            // the dates are in the ToShortDateString format.
            // We already know the string starts with "Days: " (localized).
            // Start by stripping it off then splitting at commmas.

            daysRadio.Checked = true;
            str = str.Substring(dmResources.PeriodTypeDays.Length);
            string[] split = str.Split(new char[] { ',' }); // Split at commas.

            foreach (string s in split) {
                string sub = s.Trim();
                if (s != string.Empty) {
                    if (s.Contains("-")) {
                        // Looks like a range with start and end dates.
                        string[] range = s.Split(new char[] { '-' });
                        if (range.Length == 2) {
                            string start = range[0].Trim();
                            string end = range[1].Trim();
                            DateTime startDate = DateTime.Parse(start);
                            DateTime endDate = DateTime.Parse(end);
                            ultraMonthViewSingle1.CalendarInfo.SelectedDateRanges.Add(startDate, endDate);
                        }
                    } else {
                        DateTime date = DateTime.Parse(sub);
                        ultraMonthViewSingle1.CalendarInfo.SelectedDateRanges.Add(date);
                    }
                }
            }
        }
        #endregion Days

        #region Months

        // Constructs a displayable string representing the currently selected months.
        private string GetMonthsString() {
            string ret = dmResources.PeriodTypeMonths;

            foreach (int ndx in monthsListBox.SelectedIndices) {
                DateTime datetime = _lastDate.AddMonths(-ndx);
                ret += " " + datetime.ToString("M/yy") + ",";
            }

            // Trim off the last ",".
            ret = ret.TrimEnd(new char[] { ',' });

            return ret;
        }

        // Constructs a list of DateRangeOffsets for the currently selected months.
        // Contigous months are merged into one DateRangeOffset unless they include
        // a Daylight Savings Time change.
        private List<DateRangeOffset> GetDatesForMonths() {
            List<DateRangeOffset> list = new List<DateRangeOffset>();
            int startNdx = -2;

            // Track the first and last DateTimes in each contiguous range.
            // Initialize them to something to avoid compiler errors.
            DateTime first = DateTime.Now;
            DateTime last = first;

            // Remember the months are listed in reverse order, so the first
            // selected month is the last month in a range of selected months.

            foreach (int ndx in monthsListBox.SelectedIndices) {
                if (ndx == startNdx + 1) {
                    // Add to the current range by moving the start date back a month.
                    startNdx = ndx;
                    first = first.AddMonths(-1);
                } else {
                    // Starting a new contiguous range (and maybe ending one).
                    if (startNdx != -2) {
                        // Ending a contiguous range.
                        DateRangeOffset.AddDateRange(list, first, last);
                    }

                    startNdx = ndx;

                    // Set the end of the date range to the first day of the month after the
                    // selected month in order to include the entire selected month.
                    last = _lastDate.AddMonths(1-ndx);
                    last = last.AddDays(1 - last.Day);
                    first = last.AddMonths(-1); 
                }
            }

            Debug.Assert(startNdx != -2);
            
            // The last selected month always represents the end of a range.
            DateRangeOffset.AddDateRange(list, first, last);

            return list;
        }

        private void ParseMonthsString(string str) {
            // Format is like "Months: m/y, m/y, m/y - m/y" where
            // the month and year are two-digit numbers.
            // We already know the string starts with "Months: " (localized).
            // Start by stripping it off then splitting at commmas.

            monthsRadio.Checked = true;
            str = str.Substring(dmResources.PeriodTypeMonths.Length);
            string[] split = str.Split(new char[] { ',' }); // Split at commas.

            foreach (string s in split) {
                string sub = s.Trim();
                if (s != string.Empty) {
                    if (s.Contains("-")) {
                        // Looks like a range with start and end dates.
                        string[] range = s.Split(new char[] { '-' });
                        if (range.Length == 2) {
                            // Get the start and end dates. Use day-of-month = 1
                            // and time-of-day = midnight for all dates.
                            string start = range[0].Trim();
                            DateTime startDate = DateTime.ParseExact(start, "M/yy", null).Date;
                            startDate = startDate.AddDays(1 - startDate.Day);

                            string end = range[1].Trim();
                            DateTime endDate = DateTime.ParseExact(end, "M/yy", null).Date;
                            endDate = endDate.AddDays(1 - endDate.Day);

                            SelectMonthRange(startDate, endDate);
                        }
                    } else {
                        DateTime date = DateTime.ParseExact(sub, "M/yy", null).Date;
                        date = date.AddDays(1 - date.Day);
                        SelectMonthRange(date, date);
                    }
                }
            }
        }

        private void SelectMonthRange(DateTime startDate, DateTime endDate) {
            for (int i = 0; i < 12; ++i) {
                DateTime tempDate = _lastDate.AddMonths(-i).Date;
                tempDate = tempDate.AddDays(1 - tempDate.Day);
                if (tempDate >= startDate && tempDate <= endDate)
                    monthsListBox.SetSelected(i, true);
            }
        }

        #endregion Months

        #region Years

        private string GetYearsString() {
            string ret = dmResources.PeriodTypeYears;

            foreach (int year in yearsListBox.SelectedItems) {
                ret += string.Format(" {0},", year);
            }

            // Trim off the last ",".
            ret = ret.TrimEnd(new char[] { ',' });

            return ret;
        }
                
        // Constructs a list of DateRangeOffsets for the currently selected years.
        private List<DateRangeOffset> GetDatesForYears() {
            List<DateRangeOffset> list = new List<DateRangeOffset>();
            int startYear = -2;
            int endYear = -2;

            // Track the first and last DateTimes in each contiguous range.
            // Initialize them to something to avoid compiler errors.
            DateTime first = DateTime.Now;
            DateTime last = first;

            // The years are listed in reverse order, so the last year in a 
            // range is encountered first.

            foreach (int year in yearsListBox.SelectedItems) {
                if (year == endYear - 1) {
                    // Add to the current range by moving the start year back.
                    startYear = year;
                    first = new DateTime(year, 1, 1);
                } else {
                    // Starting a new contiguous range (and maybe ending one).
                    if (endYear != -2) {
                        // Ending a contiguous range.
                        DateRangeOffset.AddDateRange(list, first, last);
                    }

                    startYear = endYear = year;
                    // Set the last date to first day of next year to include the whole selected year.
                    last = new DateTime(year+1, 1, 1); 
                    first = new DateTime(year, 1, 1);
                }
            }

            Debug.Assert(endYear != -2);

            // The last selected year is always the end of a range.
            DateRangeOffset.AddDateRange(list, first, last);

            return list;
        }

        private void ParseYearsString(string str) {
            // Format is like "Years: 2001, 2002, 2004-2008" .
            // We already know the string starts with "Years: " (localized).
            // Start by stripping it off then splitting at commmas.

            yearsRadio.Checked = true;
            str = str.Substring(dmResources.PeriodTypeYears.Length);
            string[] split = str.Split(new char[] { ',' }); // Split at commas.

            foreach (string s in split) {
                string sub = s.Trim();
                if (s != string.Empty) {
                    if (s.Contains("-")) {
                        // Looks like a range with start and end years.
                        string[] range = s.Split(new char[] { '-' });
                        if (range.Length == 2) {
                            string start = range[0].Trim();
                            int startYear = Convert.ToInt32(start);

                            string end = range[1].Trim();
                            int endYear = Convert.ToInt32(end);

                            // Don't let the user make us loop from 1 to 999999999.
                            if (startYear <= endYear && startYear > 1980 && endYear < 2100) {
                                for (int i = startYear; i <= endYear; ++i) {
                                    int ndx = yearsListBox.FindStringExact(i.ToString());
                                    if (ndx != -1) {
                                        yearsListBox.SetSelected(ndx, true);
                                    }
                                }
                            }
                        }
                    } else {
                        int ndx = yearsListBox.FindStringExact(sub);
                        if (ndx != -1) {
                            yearsListBox.SetSelected(ndx, true);
                        }
                    }
                }
            }
        }

        #endregion Years

        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);

            // Determine if the ultraMonthViewSingle1 scroll bar should be visible.
            if (ultraMonthViewSingle1.FirstVisibleDay != null) {
                DateTime firstVisible = ultraMonthViewSingle1.FirstVisibleDay.Date;
                DateTime lastVisible = GetLastVisibleDay();

                if (firstVisible <= ultraMonthViewSingle1.CalendarInfo.MinDate.Date &&
                    lastVisible >= ultraMonthViewSingle1.CalendarInfo.MaxDate.Date) {
                    Log.Debug("Setting ultraMonthViewSingle1.ScrollbarVisible = false");
                    ultraMonthViewSingle1.ScrollbarVisible = false;
                }
            }
        }

        // This horrible code exists because Infragistics did not provide a LastVisibleDay property.
        private DateTime GetLastVisibleDay() {
            //    If the specified control is null, being disposed of, 
            //    or has been disposed of, return null 
            //     
            if (ultraMonthViewSingle1 == null ||
                 ultraMonthViewSingle1.Disposing ||
                 ultraMonthViewSingle1.IsDisposed)
                return ultraMonthViewSingle1.FirstVisibleDay.Date;

            DateTime lastDay;

            //    If there is only one visible day of the week, we can determine 
            //    the last visible day by adding weeks to the FirstVisibleDay‘s date 
            //    The number of weeks we add is the value of the VisibleWeeks 
            //    property minus one. 
            if (GetNumberOfVisibleDaysOfWeek() == 1) {
                //    If there is only one visible week, and only one visible day, 
                //    we know that the last visible day is the same as the first, 
                //    so we can early out by returning the FirstVisibleDay. 
                if (ultraMonthViewSingle1.VisibleWeeks == 1)
                    return ultraMonthViewSingle1.FirstVisibleDay.Date;

                //    Add weeks to the date of the FirstVisibleDay 
                lastDay = ultraMonthViewSingle1.FirstVisibleDay.Date.AddDays((ultraMonthViewSingle1.VisibleWeeks - 1) * 7);
                return lastDay;
            }

            //    Get the date of the first visible day displayed by the control 
            DateTime firstDay = ultraMonthViewSingle1.FirstVisibleDay.Date;

            //    Determine the DayOfWeek of the last visible day of the week displayed by the control 
            // 
            //    Start with the day that is exactly one week from the first visible day, 
            //    then go backwards until we hit a day whose corresponding DayOfWeek 
            //    object is visible 
            lastDay = firstDay.AddDays(7.0F);
            for (int i = 1; i < 7; i++) {
                lastDay = lastDay.AddDays(-1);
                if (ultraMonthViewSingle1.CalendarInfo.DaysOfWeek[lastDay.DayOfWeek].Visible)
                    break;
            }

            //    Now we have the date of the last visible day in the same week, 
            //    as the first visible day, so now we need to add weeks to it to get 
            //    the very last day displayed by the control. 
            lastDay = lastDay.AddDays((ultraMonthViewSingle1.VisibleWeeks - 1) * 7);
            
            return lastDay;
        }

        private int GetNumberOfVisibleDaysOfWeek() {
            //-------------------------------------------------------------------------------- 
            // 
            //    Returns the number of visible days of the week displayed by the control 
            // 
            //-------------------------------------------------------------------------------- 

            //    If the specified control is null, being disposed of, 
            //    or has been disposed of, return 0 
            //     
            if (ultraMonthViewSingle1 == null ||
                 ultraMonthViewSingle1.Disposing ||
                 ultraMonthViewSingle1.IsDisposed)
                return 0;

            //    Now iterate the CalendarInfo object‘s DaysOfWeek collection, 
            //    and increment a counter each time we hit a DayOfWeek 
            //    object whose Visible property is true. 
            int count = 0;
            foreach (Infragistics.Win.UltraWinSchedule.DayOfWeek dayOfWeek in ultraMonthViewSingle1.CalendarInfo.DaysOfWeek) {
                if (dayOfWeek.Visible)
                    count++;
            }

            //    Return the accumulated total of visible DayOfWeek objects 
            return count;
        }

        private void UpdateOkButton() {
            if (daysRadio.Checked) {
                button1.Enabled = ultraMonthViewSingle1.CalendarInfo.SelectedDateRanges.Count > 0;
            } else if (monthsRadio.Checked) {
                button1.Enabled = monthsListBox.SelectedIndex != -1;
            } else if (yearsRadio.Checked) {
                button1.Enabled = yearsListBox.SelectedIndex != -1;
            } else if (rangeRadio.Checked) {
                button1.Enabled = true;
            } else {
                throw new ApplicationException("None of the expected radio buttons are checked.");
            }
        }

        #region Events

        private void radioButton1_CheckedChanged(object sender, EventArgs e) {
            monthsListBox.Visible = monthsRadio.Checked;
            ultraMonthViewSingle1.Visible = daysRadio.Checked;
            daysTipLabel.Visible = daysRadio.Checked;
            yearsListBox.Visible = yearsRadio.Checked;
            rangePanel.Visible = rangeRadio.Checked;

            UpdateOkButton();
        }

        private void button1_Click(object sender, EventArgs e) {
            if (rangeRadio.Checked) {
                if (fromPicker.Value > toPicker.Value) {
                    // Show error message and don't close the dialog.
                    ApplicationMessageBox.ShowWarning(this, dmResources.DateRangeInverted);
                    DialogResult = DialogResult.None;
                }
            }
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            ApplicationHelper.ShowHelpTopic(HelpTopics.ReportFilters);
        }

        private void AnyListBox_SelectedIndexChanged(object sender, EventArgs e) {
            UpdateOkButton();
        }

        void CalendarInfo_AfterSelectedDateRangeChange(object sender, EventArgs e) {
            UpdateOkButton();
        }

        #endregion
    }

    internal enum ReportPeriodType
    {
        Invalid,
        Days,
        Months,
        Years,
        Range
    }
}