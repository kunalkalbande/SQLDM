using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.Tiles;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    /// <summary>
    /// Interaction logic for TileLayoutCustomHeader.xaml
    /// </summary>
    public partial class TileLayoutCustomHeader : UserControl
    {
        public TileLayoutCustomHeader()
        {
            InitializeComponent();
        }

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as Tile;
            if (parent == null) return;
            var idp = parent.Content as IDashboardProperties;
            if (idp != null && idp.HasDashboardProperties)
                idp.IsDashboardPropertiesVisible = !idp.IsDashboardPropertiesVisible;
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
           var parent = Parent as Tile;
            if (parent == null) return;

            parent.Content = null;
            ((TileLayoutCustomHeader)parent.Header).Text.Text = "";
        }
    }
}