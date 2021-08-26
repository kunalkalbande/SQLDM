using System;
using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Infragistics.Windows.Themes;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    // This is a debug aid used to show the contents of a DataTable.
    internal partial class DataTableDialog : Form {
        public DataTableDialog() {
            InitializeComponent();

            // Apply auto scale font size.
            AdaptFontSize();
            SetGridTheme();
            ThemeManager.CurrentThemeChanged += new EventHandler(OnCurrentThemeChanged);
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }

        void OnCurrentThemeChanged(object sender, EventArgs e)
        {
            SetGridTheme();
        }

        private void SetGridTheme()
        {
            // Update UltraGrid Theme
            var themeManager = new Controls.GridThemeManager();
            themeManager.updateGridTheme(this.ultraGrid);
        }
    }
}