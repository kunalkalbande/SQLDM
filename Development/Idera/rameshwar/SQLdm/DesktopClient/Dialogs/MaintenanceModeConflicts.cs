using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infragistics.Win.UltraWinGrid;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class MaintenanceModeConflicts : Form
    {
        public MaintenanceModeConflicts()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public UltraGrid GridView
        {
            get { return this.dataGridView1; }
        }
    }
}
