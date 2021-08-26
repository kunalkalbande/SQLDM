using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class MetricSeverityDialog : Form
    {
        public Int32 metricSheverityValue { get; set; }
        public MetricSeverityDialog()
        {
            InitializeComponent();
        }
        private void MetricSeverityDialog_Load(object sender, EventArgs e)
        {
            int MetricSeverityValue = 0;
            if(NotificationRuleLabelGenerator.SelectedSeverityValue == 0)
            {
                MetricSeverityValue = 1;
            }
            else
            {
                MetricSeverityValue = NotificationRuleLabelGenerator.SelectedSeverityValue;
            }
            SeveritydurationInMunitesSpinner.Value = MetricSeverityValue;
        }
        private void SeveritydurationInMunitesSpinnerr_ValueChanged(object sender, EventArgs e)
        {
            NotificationRuleLabelGenerator.SelectedSeverityValue = Convert.ToInt32(((NumericUpDown)sender).Value);
            metricSheverityValue = Convert.ToInt32(((NumericUpDown)sender).Value);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            metricSheverityValue = Convert.ToInt32(SeveritydurationInMunitesSpinner.Value);
        }

        
    }
}
