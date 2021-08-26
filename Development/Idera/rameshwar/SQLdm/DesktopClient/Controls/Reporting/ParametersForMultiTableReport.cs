using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.Reporting {
    public partial class ParametersForMultiTableReport : UserControl {
        public ParametersForMultiTableReport() {
            InitializeComponent();
            _initialized = true;
        }

        private bool _initialized;
        private int[] _tableIndices;

        // Resize the databaseBox and serverBox as the container is
        // is resized, keeping their width's equal.
        protected override void OnResize(EventArgs e) {
            if (_initialized) {
                base.OnResize(e);
                databaseLabel.Left = this.Width / 2;
                databaseBox.Left = databaseLabel.Right;
                databaseBox.Width = this.Width - databaseBox.Left;
                serverBox.Width = databaseBox.Width;
            }
        }

        private void serverBox_SelectedIndexChanged(object sender, EventArgs e) {
            // TODO: Populate the databaseBox with the list of databases in
            // the selected server.
        }

        private void databaseBox_SelectedIndexChanged(object sender, EventArgs e) {
            // TODO: Populate the _tableList with the list of tables in the 
            // specified database.
        }

        // Display the list of tables for the user to select from.
        private void tablesBox_DropDown(object sender, EventArgs e) {
            // This is a form that looks like a CheckedListBox that "drops down".
            CheckboxSelectionForm tableList = new CheckboxSelectionForm();
            tableList.SetCheckedIndices(_tableIndices);
            tableList.Deactivate += new EventHandler(tableList_Deactivate);
            tableList.ShowUnderControl(tablesBox);
        }

        // When the user clicks outside of the list of tables,
        // put a comma-separated list of tables in the tablesBox.
        void tableList_Deactivate(object sender, EventArgs e) {
            CheckboxSelectionForm form = (CheckboxSelectionForm)sender;
            tablesBox.Text = form.GetCommaSeparatedList();
            _tableIndices = form.GetCheckedIndices();
        }
    }
}
