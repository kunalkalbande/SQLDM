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

namespace Idera.SQLdm.DesktopClient.Dialogs
{
    /// <summary>
    /// Interaction logic for XamSingleInputDialog.xaml
    /// </summary>
    public partial class XamSingleInputDialog : Window
    {
        public XamSingleInputDialog()
        {
            InitializeComponent();
        }

        public string ViewName
        {
            get { return viewName.Text; }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(ViewName))
                return;

            DialogResult = true;

            e.Handled = true;
        }
    }
}
