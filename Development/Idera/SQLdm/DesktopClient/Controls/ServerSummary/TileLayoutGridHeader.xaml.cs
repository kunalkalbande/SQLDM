using System.Windows.Controls;
using System.Windows.Input;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    /// <summary>
    /// Interaction logic for TileLayoutGridHeader.xaml
    /// </summary>
    public partial class TileLayoutGridHeader : UserControl
    {
        public TileLayoutGridHeader()
        {
            InitializeComponent();
        }

        private void addColumnImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var panel = this.Parent as Panel;
            if (panel == null) return;
            var context = panel.DataContext as DashboardLayoutModel;
            if (context == null) return;
            context.InsertColumn(Grid.GetColumn(this));
        }

        private void deleteColumnImage_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var panel = this.Parent as Panel;
            if (panel == null) return;
            var context = panel.DataContext as DashboardLayoutModel;
            if (context == null) return;
            context.DeleteColumn(Grid.GetColumn(this));
        }
    }
}