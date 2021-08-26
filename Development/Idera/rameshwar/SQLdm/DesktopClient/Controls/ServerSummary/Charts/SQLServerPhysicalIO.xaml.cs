using System;
using System.Collections.Generic;
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

namespace Idera.SQLdm.DesktopClient.Controls.ServerSummary.Charts
{
	/// <summary>
	/// Interaction logic for SQLServerPhysicalIO.xaml
	/// </summary>
	public partial class SQLServerPhysicalIO : UserControl
	{
		public SQLServerPhysicalIO()
		{
			this.InitializeComponent();
		}

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
	}
}