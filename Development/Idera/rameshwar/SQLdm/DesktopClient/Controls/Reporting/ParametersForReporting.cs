using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    public partial class ParametersForReporting : UserControl {
        public ParametersForReporting() {
            InitializeComponent();
        }

        private UserControl _currentControl;

        /// <summary>
        /// Put the specified UserControl under the TimeParameters.
        /// Remove the existing UserControl from that area and return
        /// it.  It could be null.
        /// </summary>
        public UserControl SetVariablePart(UserControl newControl) {
            UserControl oldControl = _currentControl;

            if (_currentControl != null) {  
                Controls.Remove(_currentControl);
            }

            newControl.Dock = DockStyle.Top;
            Controls.Add(newControl);
            Controls.SetChildIndex(newControl, 0);
            _currentControl = newControl;
            return oldControl;
        }

        // Allow cliets to subscribe to the "Run Report" button's click event.
        public event EventHandler RunClicked {
            add { runReportButton.Click += value; }
            remove { runReportButton.Click -= value; }
        }
    }
}
