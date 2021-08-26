using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Controls;
using Infragistics.Win.UltraWinGrid;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    public partial class MaintenanceModeConflicts : BaseDialog
    {
        public MaintenanceModeConflicts()
        {
            InitializeComponent();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new GridThemeManager();
            themeManager.updateGridTheme(this.dataGridView1);
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
