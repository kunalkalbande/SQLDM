using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    public partial class ParametersForMultiServerReport : UserControl {
        public ParametersForMultiServerReport() {
            InitializeComponent();
        }

        private int[] _serverIndices;

        // Display the list of servers for the user to select from.
        private void serversBox_DropDown(object sender, EventArgs e) {
            // This is a form that looks like a CheckedListBox that "drops down".
            CheckboxSelectionForm serverList = new CheckboxSelectionForm();
            serverList.SetCheckedIndices(_serverIndices);
            serverList.Deactivate += new EventHandler(List_Deactivate);
            serverList.ShowUnderControl(serversBox);
        }

        // When the user clicks outside of the list of servers,
        // put a comma-separated list of tables in the serversBox.
        void List_Deactivate(object sender, EventArgs e) {
            CheckboxSelectionForm form = (CheckboxSelectionForm)sender;
            serversBox.Text = form.GetCommaSeparatedList();
            _serverIndices = form.GetCheckedIndices();
        }

    }
}
