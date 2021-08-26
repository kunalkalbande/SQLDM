using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    public partial class TemporaryTestDialog : Form {
        public TemporaryTestDialog() {            
            InitializeComponent();
            _curparms = new ParametersForMultiDatabaseReport();
            parametersForReporting1.SetVariablePart(_curparms);
        }

        UserControl _curparms;

        private void RunReportClicked(object sender, EventArgs e) {
            if (_curparms is ParametersForMultiTableReport) {
                _curparms = new ParametersForMultiDatabaseReport();
            } else if (_curparms is ParametersForMultiDatabaseReport) {
                _curparms = new ParametersForSingleServerReport();
            } else if (_curparms is ParametersForSingleServerReport) {
                _curparms = new ParametersForMultiServerReport();
            } else if (_curparms is ParametersForMultiServerReport) {
                _curparms = new ParametersForMultiTableReport();
            }

            parametersForReporting1.SetVariablePart(_curparms);
        }
    }
}