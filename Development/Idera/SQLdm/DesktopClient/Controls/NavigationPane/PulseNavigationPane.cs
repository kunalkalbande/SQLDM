using System.Windows.Forms;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    public partial class PulseNavigationPane : UserControl
    {
        public PulseNavigationPane()
        {
            InitializeComponent();

            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime)
                return;

            Idera.Newsfeed.Plugins.UI.PulseController.Default.SetNavigationContainer(this);
        }
    }
}
