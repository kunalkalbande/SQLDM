using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;

namespace Idera.SQLdm.DesktopClient.Views.Reports {
    // This is a debug aid used to show the contents of a DataTable.
    internal partial class DataTableDialog : Form {
        public DataTableDialog() {
            InitializeComponent();

            // Apply auto scale font size.
            AdaptFontSize();
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }
}