using System.Windows.Forms;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Views.Administration;
using Idera.SQLdm.DesktopClient.Views.Alerts;
using Idera.SQLdm.DesktopClient.Views.Tasks;
using Infragistics.Win.UltraWinExplorerBar;
using Idera.SQLdm.DesktopClient.Views.Pulse;

namespace Idera.SQLdm.DesktopClient.Controls.NavigationPane
{
    using Views.Reports;

    public partial class NavigationPaneControl : UserControl
    {
        private bool viewChanging;

        public NavigationPaneControl()
        {
            InitializeComponent();

            // Autoscale fontsize.
            AdaptFontSize();
        }

        public NavigationPanes SelectedPane
        {
            get { return (NavigationPanes) explorerBar.SelectedGroup.Index; }
            set { explorerBar.SelectedGroup = explorerBar.Groups[(int) value]; }
        }

        private void explorerBar_SelectedGroupChanged(object sender, GroupEventArgs e)
        {
            if (viewChanging)
                return;

            switch ((NavigationPanes) e.Group.Index)
            {
                case NavigationPanes.Servers:
                    serversNavigationPane.Focus();
                    serversNavigationPane.ShowViewForSelectedNode();
                    break;
                case NavigationPanes.Alerts:
                    alertsNavigationPane.Focus();
                    ApplicationController.Default.ShowAlertsView(StandardAlertsViews.ShowDefaultOrExisting);
                    break;
                case NavigationPanes.Pulse:
                    pulseNavigationPane.Focus();
                    ApplicationController.Default.ShowPulseView();
                    break;
                case NavigationPanes.Tasks:
                    tasksNavigationPane.Focus();
                    tasksNavigationPane.Visible = ApplicationModel.Default.IsTasksViewEnabled;
                    ApplicationController.Default.ShowTasksView(StandardTasksViews.ShowDefaultOrExisting);
                    break;
                case NavigationPanes.Reports:
                    reportsNavigationPane.Focus();
                    reportsNavigationPane.ValidateActivityVisibility();
                    ApplicationController.Default.ShowReportsView();
                    break;
                case NavigationPanes.Administration:
                    administrationNavigationPane.Focus();
                    ApplicationController.Default.ShowAdministrationView();
                    break;
            }
        }

        private void NavigationPaneControl_Load(object sender, System.EventArgs e)
        {
            if (DesignMode) return;
            UpdateUserTokenAttributes();
            ApplicationController.Default.ActiveViewChanged += ActiveViewChanged;
        }

        public void UpdateUserTokenAttributes()
        {
            explorerBar.Groups[(int)NavigationPanes.Administration].Visible = ApplicationModel.Default.UserToken.IsSQLdmAdministrator;
            alertsNavigationPane.UpdateUserTokenAttributes();
            tasksNavigationPane.UpdateUserTokenAttributes();
        }

        public void SetVisible(NavigationPanes pane, bool visible)
        {
            explorerBar.Groups[(int)pane].Visible = visible;
        }

        private void ActiveViewChanged(object sender, System.EventArgs e)
        {
            var pane = NavigationPanes.Servers;
            var activeView = ApplicationController.Default.ActiveView;

            if (activeView is TasksView)
                pane = NavigationPanes.Tasks;
            else if (activeView is PulseView)
                pane = NavigationPanes.Pulse;
            else if (activeView is AlertsView)
                pane = NavigationPanes.Alerts;
            else if (activeView is ReportsView)
                pane = NavigationPanes.Reports;
            else if (activeView is AdministrationView)
                pane = NavigationPanes.Administration;

            NavigationPanes currentPane = SelectedPane;
            if (pane != currentPane)
            {
                viewChanging = true;
                try
                {
                    SelectedPane = pane;
                } finally
                {
                    viewChanging = false;
                }
            }
        }

        /// <summary>
        /// Adapts the resolution for the fonts, based on the DPI applied for the operating system.
        /// </summary>
        private void AdaptFontSize()
        {
            AutoScaleFontHelper.Default.AutoScaleControl(this, AutoScaleFontHelper.ControlType.Container);
        }
    }

    public enum NavigationPanes
    {
        Servers,
        Alerts,
        Pulse,
        Tasks,
        Reports,
        Administration
    }
}
