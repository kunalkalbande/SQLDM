using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary
{
    /// <summary>
    /// Interaction logic for ServerSummaryContent.xaml
    /// </summary>
    public partial class ServerSummaryContent : UserControl
    {
        public ServerSummaryContent()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            dashboardControl.Model = e.NewValue as DashboardLayoutModel;
        }
    }
}
