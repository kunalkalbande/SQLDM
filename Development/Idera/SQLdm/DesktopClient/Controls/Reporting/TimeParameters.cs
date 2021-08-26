using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    /// <summary>
    /// This UserControl encapsulates the time parameters which are common to all four
    /// versions of the parameters pane in the Reporting view.
    /// </summary>
    public partial class TimeParameters : UserControl {
        public TimeParameters() {
            InitializeComponent();
            intervalBox.SelectedIndex = 1;
        }

        // Call this to restrict the available selections to those appropriate to
        // the table growth report.
        public void UseForTableGrowthReport() {
            intervalBox.Items.RemoveAt(4);
            intervalBox.Items.RemoveAt(1);
            intervalBox.Items.RemoveAt(0);
        }

        // Represents the first day/month displayed in the list of days/months.
        // Selected days/months are calculated from this and the selected indices.
        private DateTime _baseDate = DateTime.Today;
        private int[] _checkedDays;
        private string _dayList;

        private int[] _checkedMonths;
        private string _monthList;

        // When user changes interval, change which period radio buttons enabled.
        private void intervalBox_SelectedIndexChanged(object sender, EventArgs e) {
            // This little trick accounts for the fact that the first n items sometimes
            // get removed, and there are normally 5.
            int index = intervalBox.SelectedIndex + 5 - intervalBox.Items.Count;

            switch (index) {
                case 0: // minutes
                case 1: // hours
                    days.Enabled = true;
                    months.Enabled = false;
                    years.Enabled = false;
                    
                    days.Checked = true;
                    break;
                case 2: // days
                    days.Enabled = true;
                    months.Enabled = true;
                    years.Enabled = false;
                    
                    days.Checked = true;
                    break;
                case 3: // months
                    days.Enabled = false;
                    months.Enabled = true;
                    years.Enabled = true;
                    
                    months.Checked = true;
                    break;
                case 4: // years
                    days.Enabled = false;
                    months.Enabled = false;
                    years.Enabled = true;
                    
                    years.Checked = true;
                    break;
            }
        }

        // When user finishes seleting period items, creata a comma
        // separated list and display it in the periodBox control.
        void _periodList_Deactivate(object sender, EventArgs e) {
            CheckboxSelectionForm form = (CheckboxSelectionForm)sender;

            periodBox.Text = form.GetCommaSeparatedList();

            if (months.Checked) {
                _checkedMonths = form.GetCheckedIndices();
                _monthList = periodBox.Text;
            }

            if (days.Checked) {
                _checkedDays = form.GetCheckedIndices();
                _dayList = periodBox.Text;
            }

        }

        // When user checks a different period radio button, display
        // the last list of days/months the user selected for the period.
        private void PeriodCheckedChanged(object sender, EventArgs e) {
            if (years.Checked) {
                // Only one value is allowed in this case.
                periodBox.Text = Resources.Last365Days;
                periodBox.Enabled = false;
            } else if (months.Checked) {
                periodBox.Enabled = true;
                periodBox.Text = _monthList;
            } else if (days.Checked) {
                periodBox.Enabled = true;
                periodBox.Text = _dayList;
            }
        }

        // Show the days or months for the last year in a drop-down checked list box.
        private void periodList_DropDown(object sender, EventArgs e) {
            CheckboxSelectionForm periodList = new CheckboxSelectionForm();
            CheckedListBox.ObjectCollection items = periodList.TheListBox.Items;

            if (days.Checked) {
                // Show the last 365 days including today.
                for (int i = 0; i < 365; ++i) {
                    items.Add(_baseDate.AddDays(-i).ToString("dd MMMM yyyy"));
                }

                periodList.SetCheckedIndices(_checkedDays);
            } else {
                // Must be months.  Show the last 12 months including the current month.
                for (int i = 0; i < 12; ++i) {
                    items.Add(_baseDate.AddMonths(-i).ToString("MMMM yyyy"));
                }

                periodList.SetCheckedIndices(_checkedMonths);
            }

            periodList.Deactivate += new EventHandler(_periodList_Deactivate);
            periodList.ShowUnderControl(periodBox); // NOT modal.  We depend on the Deactivate event.
        }

    }
}
