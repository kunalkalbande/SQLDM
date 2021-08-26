using System.Windows.Controls;
using System.Windows.Input;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    /// <summary>
    /// Interaction logic for TileLayoutGridRowHeader.xaml
    /// </summary>
    public partial class TileLayoutGridRowHeader : UserControl
    {
        public TileLayoutGridRowHeader()
        {
            InitializeComponent();
        }

        private void addRowImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var panel = this.Parent as Panel;
            if (panel == null) return;
            var context = panel.DataContext as DashboardLayoutModel;
            if (context == null) return;
            context.InsertRow(Grid.GetRow(this));
        }

        private void deleteRowImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var panel = this.Parent as Panel;
            if (panel == null) return;
            var context = panel.DataContext as DashboardLayoutModel;
            if (context == null) return;
            context.DeletetRow(Grid.GetRow(this));
        }
    }
}