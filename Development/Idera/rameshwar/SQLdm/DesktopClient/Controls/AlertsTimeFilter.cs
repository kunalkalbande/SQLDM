using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls
{
    public partial class AlertsTimeFilter : UserControl
    {
        private bool isLastN;

        public AlertsTimeFilter()
        {
            InitializeComponent();
            isLastN = true;
            showLastN();
        }

        private void showLastN()
        {
            _lnklbl_Specification.Text = "All Alerts for the last N days";
            _pnl_TimeSpan.Visible = false;
            _pnl_LastN.BringToFront();
        }

        private void showTimeSpan()
        {
            _lnklbl_Specification.Text = "All Alerts for the following time span";
            _pnl_TimeSpan.BringToFront();
        }

        private void _lnklbl_Specification_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            isLastN = !isLastN;
            if (isLastN)
            {
                showLastN();
            }
            else
            {
                showTimeSpan();
            }
        }
    }
}
