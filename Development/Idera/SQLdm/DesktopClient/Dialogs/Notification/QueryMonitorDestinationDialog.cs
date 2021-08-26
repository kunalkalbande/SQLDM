using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    using System;
    using System.Windows.Forms;

    public partial class QueryMonitorDestinationDialog : BaseDialog
    {
        private int durationInMinutes = -1;


        public QueryMonitorDestinationDialog()
        {
            this.DialogHeader = "Query Monitor Settings";
            InitializeComponent();
            AdaptFontSize();
        }

        public int QMDurationInMinutes
        {
            get
            {
                return durationInMinutes;
            }
            set
            {
                durationInMinutes = value;
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (enableForeverButton.Checked)
                durationInMinutes = 0;
            else
            {
                durationInMinutes = (int) durationInMunitesSpinner.Value;
            }
        }

        private void QueryMonitorDestinationDialog_Load(object sender, EventArgs e)
        {
            if (durationInMinutes < 1)
            {
                durationInMunitesSpinner.Value = 15;
                enableForeverButton.Checked = true;
            }
            else
            {
                durationInMunitesSpinner.Value = durationInMinutes;
                enableForeverButton.Checked = false;
            }
            enableLimitedButton.Checked = !enableForeverButton.Checked;
        }

        private void enableLimitedButton_CheckedChanged(object sender, EventArgs e)
        {
            bool durationSpinnerEnabled = enableLimitedButton.Checked;
            durationInMunitesSpinner.Enabled = durationSpinnerEnabled;
            label1.Enabled = durationSpinnerEnabled;
            label4.Enabled = durationSpinnerEnabled;
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