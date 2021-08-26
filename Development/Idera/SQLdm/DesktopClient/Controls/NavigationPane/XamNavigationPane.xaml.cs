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
using Idera.SQLdm.DesktopClient.Views.Tasks;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.DesktopClient.Views.Pulse;
using Idera.SQLdm.DesktopClient.Views.Reports;
using Idera.SQLdm.DesktopClient.Views.Administration;
using Idera.SQLdm.DesktopClient.Controls.Presentation;
using Idera.SQLdm.DesktopClient.Views.Servers.Server;
using Idera.SQLdm.DesktopClient.Views.Servers.ServerGroup;
using Idera.SQLdm.DesktopClient.Objects;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    public enum NavigationPaneAlignment
    {
        Vertical,
        Horizontal
    }
    /// <summary>
    /// Interaction logic for XamNavigationPane.xaml
    /// </summary>
    public partial class XamNavigationPane : UserControl, IThemeControl
    {
        private bool viewChanging;
        private ServersNavigationPane serversNavigationPane;
        private XamSecondaryNavigationPane secondaryNavigationPane;
        private AlertsNavigationPane alertsNavigationPane;

        public XamNavigationPane()
        {
            InitializeComponent();
            ApplicationController.Default.ActiveViewChanged += ActiveViewChanged;
            
        }

        public NavigationPaneAlignment NavigationPaneMenuAlignment { get; set; }

        public ServersNavigationPane ServersNavigationPane
        {
            get { return serversNavigationPane; }
            set { serversNavigationPane = value; }
        }
        public XamSecondaryNavigationPane SecondaryNavigationPane
        {
            get { return secondaryNavigationPane; }
            set { secondaryNavigationPane = value; }
        }
        public AlertsNavigationPane AlertsNavigationPane
        {
            get { return alertsNavigationPane; }
            set { alertsNavigationPane = value; }
        }
        
        public XamNavigationPanes SelectedPane { get; set; }


        public void ChangeGroup(XamNavigationPanes pane)
        {
            if (viewChanging)
                return;
            switch (pane)
            {
                case XamNavigationPanes.Servers:
                    serversNavigationPane.Focus();
                    serversNavigationPane.ShowViewForSelectedNode();
                    secondaryNavigationPane.Change(pane);
                    break;
                case XamNavigationPanes.Alerts:
                    alertsNavigationPane.Focus();
                    secondaryNavigationPane.Change(pane, 0);
                    break;
                case XamNavigationPanes.News:
                case XamNavigationPanes.Reports:
                case XamNavigationPanes.Tools:
                case XamNavigationPanes.Files:
                case XamNavigationPanes.Administration:
                    secondaryNavigationPane.Change(pane, 0);
                    break;
            }
        }

        bool firstViewChange = true;
        private void ActiveViewChanged(object sender, System.EventArgs e)
        {
            if (firstViewChange)
            {
                firstViewChange = false;
                return;
            }

            viewChanging = true;

            var activeView = ApplicationController.Default.ActiveView;

            if (activeView is ServerViewContainer)
            {
                topMenu.SelectedIndex = (int)XamNavigationPanes.Servers;
                secondaryNavigationPane.Change(XamNavigationPanes.Servers, 1);
            }
            else if (activeView is ServerGroupView)
            {
                topMenu.SelectedIndex = (int)XamNavigationPanes.Servers;
                secondaryNavigationPane.Change(XamNavigationPanes.Servers, 0);
            }


            viewChanging = false;
            
            
        }

        public void UpdateUserTokenAttributes()
        {
            //explorerBar.Groups[(int)NavigationPanes.Administration].Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            //alertsNavigationPane.UpdateUserTokenAttributes();
            //tasksNavigationPane.UpdateUserTokenAttributes();
        }

        public void UpdateTheme(ThemeName themeName)
        {
            serversNavigationPane.UpdateTheme(themeName);
        }


        public void ItemSelectetdChanged(XamNavigationPanes navType)
        {
            ChangeGroup(navType);

            switch (navType)
            {
                case XamNavigationPanes.Files:
                    break;
                case XamNavigationPanes.Reports:
                    ApplicationController.Default.ShowReportsView();
                    break;
                case XamNavigationPanes.Servers:
                    if (!(ApplicationModel.Default.FocusObject is MonitoredSqlServerWrapper || ApplicationModel.Default.FocusObject is Idera.SQLdm.Common.Objects.Tag || ApplicationModel.Default.FocusObject is UserView))
                        ApplicationController.Default.ShowActiveServersView();
                    break;
                case XamNavigationPanes.News:
                    ApplicationController.Default.ShowPulseView();
                    break;
                case XamNavigationPanes.Alerts:
                    ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ShowDefaultOrExisting);
                    break;
                case XamNavigationPanes.Tools:
                    break;
                case XamNavigationPanes.Administration:
                    ApplicationController.Default.ShowAdministrationView();
                    secondaryNavigationPane.administrationSecondaryMenu.SelectedIndex = 0;
                    break;
            }
        }

        public void TopMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext != null && this.DataContext is MainWindowViewModel)
            {
                ((MainWindowViewModel)this.DataContext).UpdatePredictiveAnalyticsState();

                ((MainWindowViewModel)this.DataContext).PrepareSnoozeAndUnsnoozeOperation();
            }
                       

            if (e.AddedItems[0] == null)
                return;

            TabItem item = e.AddedItems[0] as TabItem;
            
            if (item == null)
                return;

            string friendlyName = item.Name.Remove(item.Name.Length - 4, 4);
            XamNavigationPanes itemType = (XamNavigationPanes)Enum.Parse(typeof(XamNavigationPanes), friendlyName, true);

            ((MainWindowViewModel)this.DataContext).UpdateNavigataionPane(itemType);

            ((MainWindowViewModel)this.DataContext).UpdateSearchBarVisibility(itemType);

            ItemSelectetdChanged(itemType);

            e.Handled = true;
            
        }
    }

    public enum XamNavigationPanes
    {
        Servers,
        Reports,
        Alerts,
        Administration,
        News,
        Files,
        Tools,
    }
}
