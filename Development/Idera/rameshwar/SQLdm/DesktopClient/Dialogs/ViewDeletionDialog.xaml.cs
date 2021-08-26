using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class ViewDeletionDialog : Window
    {
        public ViewDeletionDialog()
        {
            InitializeComponent();
        }

        public void PopulateViewNameComboBox(List<string> viewNames)
        {
            viewNamesCombobox.ItemsSource = viewNames;
            viewNamesCombobox.SelectedIndex = 0;
        }

        public string SelectedViewName
        {
            get { return viewNamesCombobox.SelectedItem as string; }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            e.Handled = true;
        }
    }
}
