using Idera.Newsfeed.Plugins.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Dialogs.Notification
{
    public partial class AlertRankValuesDialog : Form
    {
        public Int32 rankValue { get; set; }
        public string cmbRankValue { get; set; }
       
        public AlertRankValuesDialog()
        {
            InitializeComponent();
        }
       
        private void okButton_Click(object sender, EventArgs e)
        {
                rankValue = Convert.ToInt32(durationInMunitesSpinner.Value);
                cmbRankValue =Convert.ToString(CmbRankValue.SelectedIndex);
        }
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
        private void durationInMunitesSpinner_ValueChanged(object sender, EventArgs e)
        {
            NotificationRuleLabelGenerator.Rvalue = Convert.ToInt32(((NumericUpDown)sender).Value);
            rankValue = Convert.ToInt32(((NumericUpDown)sender).Value);
        }
        private void AlertRankValuesDialog_Load(object sender, EventArgs e)
        {

            if (Convert.ToInt32(rankValue) < 1)
            {
                rankValue = Convert.ToInt32(((AlertRankValuesDialog)sender).durationInMunitesSpinner.Value);
                durationInMunitesSpinner.Value = 15;
            }
            if (!string.IsNullOrEmpty(NotificationRuleLabelGenerator.RankSelectedValue))
            {
                durationInMunitesSpinner.Value = Convert.ToInt32(NotificationRuleLabelGenerator.RankSelectedValue);
            }


            Dictionary<int, string> comboSource = new Dictionary<int, string>();
            comboSource.Add(0, "greater than");
            comboSource.Add(1, "less than");
            comboSource.Add(2, "equal to");
            CmbRankValue.DataSource = new BindingSource(comboSource, null);
            CmbRankValue.DisplayMember = "Value";
            CmbRankValue.ValueMember = "Key";
            if (NotificationRuleLabelGenerator.SelectedCmbValues != null)
            {
                CmbRankValue.SelectedIndex = Convert.ToInt32(NotificationRuleLabelGenerator.SelectedCmbValues);
            }


        }

        private void CmbRankValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbRankValue =Convert.ToString(((ComboBox)sender).SelectedIndex);
        }
    }
}
