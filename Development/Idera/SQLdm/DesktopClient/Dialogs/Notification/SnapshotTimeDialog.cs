using Idera.SQLdm.Common;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;
    using Idera.SQLdm.Common.Notification;
    using System.ComponentModel;
    using Idera.SQLdm.DesktopClient.Helpers;

    public partial class SnapshotTimeDialog : Form
    {
        private string timeFormat;
        private SnapshotTimeRule rule;


        public SnapshotTimeDialog()
        {
            InitializeComponent();
            AdaptFontSize();
        }

        public SnapshotTimeRule SnapshotTimeRule
        {
            get { return rule; }
            set { rule = value; }
        }

        private void SnapshotTimeDialog_Load(object sender, EventArgs e)
        {
            timeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern;
            timeFormat = timeFormat.Replace(":s", string.Empty);
            timeFormat = timeFormat.Replace("s", string.Empty);

            PopulateStartTimeCombo();
            PopulateEndTimeCombo();

            cboStartTime.Text = DateTime.Today.Add(rule.StartTime).ToString(timeFormat);
            cboEndTime.Text = DateTime.Today.Add(rule.EndTime).ToString(timeFormat);

            chkUseDOW.Checked = rule.AnyDays;
            if (rule.AnyDays)
            {
                chkMonday.Checked = rule.Monday;
                chkTuesday.Checked = rule.Tuesday;
                chkWednesday.Checked = rule.Wednesday;
                chkThursday.Checked = rule.Thursday;
                chkFriday.Checked = rule.Friday;
                chkSaturday.Checked = rule.Saturday;
                chkSunday.Checked = rule.Sunday;
            }
            dowPanel.Enabled = chkUseDOW.Checked;
        }

        private void PopulateStartTimeCombo()
		{
			this.cboStartTime.Items.Clear();

            DateTime theDate = DateTime.Today;
			for ( int i = 0; i < 48; i ++ )
			{
                this.cboStartTime.Items.Add(theDate.ToString(timeFormat));
				theDate = theDate.Add( new TimeSpan(0, 30, 0) );
			}
		}

        /// <summary>
		/// Populates the ComboBox that contains the values for the appointment's end time.
		/// </summary>
		private void PopulateEndTimeCombo()
		{
			this.cboEndTime.Items.Clear();

            DateTime start = DateTime.Today;

			for ( int i = 0; i < 49; i ++ )
			{
				TimeSpan timeSpan = new TimeSpan(0, i * 30, 0);
				DateTime theDate = start.Add( timeSpan );
				string itemText = theDate.ToString(timeFormat);
				this.cboEndTime.Items.Add( itemText );
			}
		}

        private void OnCheckStateChanged(object sender, EventArgs e)
        {
            if (sender == chkUseDOW)
                dowPanel.Enabled = chkUseDOW.Checked;
            UpdateButtons();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            try
            {
                DateTime dt = DateTime.Parse(cboStartTime.Text);
                rule.StartTime = dt.TimeOfDay;
            }
            catch (Exception e1)
            {
                this.DialogResult = DialogResult.None;
                errorProvider.SetError(cboStartTime, e1.Message);
                return;
            }
            try
            {
                DateTime dt = DateTime.Parse(cboEndTime.Text);
                rule.EndTime = dt.TimeOfDay;
            }
            catch (Exception e2)
            {
                this.DialogResult = DialogResult.None;
                errorProvider.SetError(cboEndTime, e2.Message);
                return;
            }

            // update the rule with the current values

            bool useDOW = chkUseDOW.Checked;

            rule.Monday = useDOW && chkMonday.Checked;
            rule.Tuesday = useDOW && chkTuesday.Checked;
            rule.Wednesday = useDOW && chkWednesday.Checked;
            rule.Thursday = useDOW && chkThursday.Checked;
            rule.Friday = useDOW && chkFriday.Checked;
            rule.Saturday = useDOW && chkSaturday.Checked;
            rule.Sunday = useDOW && chkSunday.Checked;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            bool isOK = false;

            bool dodays = chkUseDOW.Checked;
            if (dodays)
            {
                if (!chkMonday.Checked &&
                    !chkTuesday.Checked &&
                    !chkWednesday.Checked &&
                    !chkThursday.Checked &&
                    !chkFriday.Checked && 
                    !chkSaturday.Checked &&
                    !chkSunday.Checked)
                {
                    dodays = false;
                } 
            }

            try
            {
                DateTime dt = DateTime.Parse(cboStartTime.Text);
                TimeSpan st = dt.TimeOfDay;
                dt = DateTime.Parse(cboEndTime.Text);
                TimeSpan et = dt.TimeOfDay;
                if (st.Hours != et.Hours || st.Minutes != et.Minutes)
                    isOK = true;

            }
            catch
            {
            }

            btnOK.Enabled = isOK || dodays;
        }

        protected override void OnHelpButtonClicked(CancelEventArgs e) {
            if (e != null) e.Cancel = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
        }

        protected override void OnHelpRequested(HelpEventArgs hevent) {
            if (hevent != null) hevent.Handled = true;
            Idera.SQLdm.DesktopClient.Helpers.ApplicationHelper.ShowHelpTopic(HelpTopics.NewNotificationRule);
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