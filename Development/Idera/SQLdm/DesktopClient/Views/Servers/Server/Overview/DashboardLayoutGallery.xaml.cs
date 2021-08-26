using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.UI.Dialogs;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;
using Infragistics.Windows.Controls;
using Infragistics.Windows.DataPresenter;
using DashboardFilter = Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview.DashboardLayoutGalleryViewModel.DashboardFilter;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    /// <summary>
    /// Interaction logic for DashboardLayoutGallery.xaml
    /// </summary>
    public partial class DashboardLayoutGallery : WpfServerDesignBaseView
    {
        #region fields

        private int instanceId;
        private string userName;
        private DashboardFilter filter;

        private DashboardLayoutGalleryViewModel viewModel
        {
            get
            {
                return (DataContext is DashboardLayoutGalleryViewModel)
                           ? DataContext as DashboardLayoutGalleryViewModel
                           : null;
            }
        }

        private bool changingView = false;

        #endregion

        #region constructors

        public DashboardLayoutGallery()
        {
            InitializeComponent();
        }

        public DashboardLayoutGallery(int instanceId)
        {
            InitializeComponent();

            this.instanceId = instanceId;
            Initialize();
        }

        #endregion

        #region properties

        public event EventHandler SelectionChanged;
        public event EventHandler FilterChanged;

        public bool DeleteAllowed
        {
            get
            {
                if (dashboardLayoutXamDataCarousel.ActiveRecord is DataRecord)
                {
                    DataRecord record = dashboardLayoutXamDataCarousel.ActiveRecord as DataRecord;
                    if (record.DataItem is DashboardLayoutSelector)
                    {
                        DashboardLayoutSelector selector = record.DataItem as DashboardLayoutSelector;

                        return selector.CanDelete;
                    }
                }
                return false;
            }
        }

        internal DashboardFilter Filter
        {
            get { return filter; }
            set
            {
                this.filter = value;
                SetFilter(filter);
                if (FilterChanged != null)
                {
                    FilterChanged(this, EventArgs.Empty);
                }
            }
        }

        public int SelectedDashboardLayoutID
        {
            get { return viewModel.SelectedId; }
        }

        #endregion

        private void Initialize()
        {
            // don't let this fail since it is called from the constructor
            try
            {
                SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
                userName = RepositoryHelper.GetRepositoryUser(connectionInfo);
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving username from the repository.",
                          ex);
                userName = "Unknown";
            }
        }

        public override void ShowHelp()
        {
            ApplicationHelper.ShowHelpTopic(HelpTopics.SelectDashboardLayoutView);
        }

        public override void SetArgument(object argument)
        {
            if (argument is int)
            {
                int dashboardLayoutId = (int)argument;

                foreach(DashboardLayoutSelector dashboard in dashboardLayoutXamDataCarousel.DataSource)
                {
                    if (dashboard.DashboardLayoutID == dashboardLayoutId)
                    {
                        viewModel.SelectedId = dashboardLayoutId;
                        dashboardLayoutXamDataCarousel.ActiveDataItem = dashboard;
                        break;
                    }
                }
            }
        }

        public override void SaveSettings()
        {
            
        }

        private void SetFilter(DashboardFilter filter)
        {
            viewModel.LoadData(filter);
            dashboardLayoutXamDataCarousel.DataSource = viewModel.Configurations;
        }

        public void DeleteDashboard()
        {
            if (viewModel.SelectedId != 0)
            {
                DashboardLayoutSelector dashboard = (DashboardLayoutSelector)((DataRecord)dashboardLayoutXamDataCarousel.ActiveRecord).DataItem;
                if (dashboard.IsSystem)
                {
                    ApplicationMessageBox.ShowError(this.GetWinformWindow(), "SQLdm default dashboards cannot be deleted.");
                    return;
                }
                if (dashboard.Owner != userName)
                {
                    ApplicationMessageBox.ShowError(this.GetWinformWindow(), "You must be the owner of a dashboard in order to delete it.");
                    return;
                }
                if (DialogResult.Yes ==
                    ApplicationMessageBox.ShowQuestion(this.GetWinformWindow(),
                                                       string.Format("Are you sure you want to delete dashboard '{0}'?", dashboard.Name)))
                {
                    try
                    {
                        RepositoryHelper.DeleteDashboardLayout(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                            viewModel.SelectedId);

                        ApplicationMessageBox.ShowInfo(this.GetWinformWindow(),
                                                       string.Format("Dashboard '{0}' has been deleted.", dashboard.Name));

                        viewModel.LoadData();
                        dashboardLayoutXamDataCarousel.DataSource = viewModel.Configurations;
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this.GetWinformWindow(), "Error deleting dashboard.", ex );
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this.GetWinformWindow(), "There is no dashboard selected. Select a dashboard before attempting to delete it.");
            }
        }

        public void SetAsGlobalDefault()
        {
            if (viewModel.SelectedId != 0)
            {
                string name = ((DashboardLayoutSelector)((DataRecord)dashboardLayoutXamDataCarousel.ActiveRecord).DataItem).Name;
                if (DialogResult.Yes ==
                    ApplicationMessageBox.ShowQuestion(this.GetWinformWindow(),
                                                       string.Format("Are you sure you want use layout '{0}' as the default for all servers with no selection?", name)))
                {
                    try
                    {
                        RepositoryHelper.SetDefaultDashboardLayout(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                            userName,
                            null,
                            viewModel.SelectedId);

                        ApplicationMessageBox.ShowInfo(this.GetWinformWindow(),
                                                       string.Format("Layout '{0}' is now the default for all servers with no selection.", name));

                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this.GetWinformWindow(), "Error setting default dashboard layout.", ex);
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this.GetWinformWindow(), "There is no layout selected. Select a layout before attempting to use it.");
            }
        }

        public void SetAsServerDefault()
        {
            if (viewModel.SelectedId != 0)
            {
                string name = ((DashboardLayoutSelector)((DataRecord)dashboardLayoutXamDataCarousel.ActiveRecord).DataItem).Name;
                if (DialogResult.Yes ==
                    ApplicationMessageBox.ShowQuestion(this.GetWinformWindow(),
                                                       string.Format("Are you sure you want use layout '{0}' as the default for this server?", name)))
                {
                    try
                    {
                        RepositoryHelper.SetDefaultDashboardLayout(
                            Settings.Default.ActiveRepositoryConnection.ConnectionInfo,
                            userName,
                            instanceId,
                            viewModel.SelectedId);

                        ApplicationMessageBox.ShowInfo(this.GetWinformWindow(),
                                                       string.Format("Layout '{0}' is now the default for this server.", name));
                    }
                    catch (Exception ex)
                    {
                        ApplicationMessageBox.ShowError(this.GetWinformWindow(), "Error setting server default dashboard layout.", ex);
                    }
                }
            }
            else
            {
                ApplicationMessageBox.ShowInfo(this.GetWinformWindow(), "There is no layout selected. Select a layout before attempting to use it.");
            }
        }

        private void WpfServerBaseView_Loaded(object sender, RoutedEventArgs e)
        {
            dashboardLayoutXamDataCarousel.DataSource = ((DashboardLayoutGalleryViewModel)DataContext).Configurations;
        }

        private void dashboardLayoutXamDataCarousel_RecordsInViewChanged(object sender, Infragistics.Windows.DataPresenter.Events.RecordsInViewChangedEventArgs e)
        {
            Record[] records = dashboardLayoutXamDataCarousel.GetRecordsInView(false);
            // horizontalOffset is the index of the first item in view
            int horizontalOffset = Convert.ToInt32(dashboardLayoutXamDataCarousel.ScrollInfo.HorizontalOffset);
            int centerPosition = (horizontalOffset + (records.Length / 2)) % dashboardLayoutXamDataCarousel.Records.Count;
            foreach(Record record in records)
            {
                if (record.Index == centerPosition)
                {
                    changingView = true;
                    record.IsActive = true;
                    return;
                }
            }
        }

        private void dashboardLayoutXamDataCarousel_RecordActivated(object sender, Infragistics.Windows.DataPresenter.Events.RecordActivatedEventArgs e)
        {
            if (IsDisposed)
                return;

            int id = 0;
            if (e.Record is DataRecord)
            {
                DataRecord record = e.Record as DataRecord;
                if (record.DataItem is DashboardLayoutSelector)
                {
                    DashboardLayoutSelector selector = record.DataItem as DashboardLayoutSelector;

                    id = selector.DashboardLayoutID;
                }
            }

            if (!changingView && id != viewModel.SelectedId)
            {
                double horizontalOffset = e.Record.Index == 0
                                              ? dashboardLayoutXamDataCarousel.Records.Count - 1
                                              : e.Record.Index - 1;
                dashboardLayoutXamDataCarousel.ScrollInfo.SetHorizontalOffset(horizontalOffset);
            }
            viewModel.SelectedId = id;

            if (SelectionChanged != null)
            {
                SelectionChanged(this, new EventArgs());
            }
            changingView = false;
        }
    }
}
