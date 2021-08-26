using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

using BBS.TracerX;

using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Objects;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Views.Servers.Server.Overview
{
    internal class DashboardLayoutGalleryViewModel : INotifyPropertyChanged
    {
        private Logger Log = Logger.GetLogger("DashboardLayoutGalleryViewModel");
        private int instanceId;
        private ServerSummaryView4 _dashboardView;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Used for visual studio designer support.  
        /// Only add stuff to the default constructor if 
        /// it can stand on its own.
        /// </summary>
        public DashboardLayoutGalleryViewModel()
        {
            InitializeForDesigner();
        }

        internal DashboardLayoutGalleryViewModel(ServerSummaryView4 dashboardView)
        {
            this._dashboardView = dashboardView;
            this.instanceId = dashboardView.InstanceId;

            // this causes memory leaks which will need to be fixed before hooking it up again.
            //Settings.Default.PropertyChanged += SettingsPropertyChanged;

            Initialize();
        }

        //~DashboardLayoutGalleryViewModel()
        //{
        //    Dispose();
        //}

        //public void Dispose()
        //{
        //    Settings.Default.PropertyChanged -= SettingsPropertyChanged;
        //}

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            private set
            {
                _userName = value;
                FirePropertyChanged("UserName");
            }
        }
        private DashboardFilter _filter;
        public DashboardFilter Filter
        {
            get { return _filter; }
            private set
            {
                _filter = value;
                FirePropertyChanged("Filter");
            }
        }

        private List<DashboardLayoutSelector> _configurations;
        public List<DashboardLayoutSelector> Configurations
        {
            get { return _configurations; }
            private set
            {
                _configurations = value;
                FirePropertyChanged("Configurations");
            }
        }

        private int _selectedId;
        public int SelectedId
        {
            get { return _selectedId; }
            set
            {
                _selectedId = value;
                FirePropertyChanged("SelectedId");
                FirePropertyChanged("SelectionText");
            }
        }
        public string SelectionText
        {
            get
            {
                int idx = 1;
                foreach (DashboardLayoutSelector dashboard in Configurations)
                {
                    if (dashboard.DashboardLayoutID == SelectedId)
                        return string.Format("{0} of {1}", idx, Configurations.Count);
                    idx++;
                }
                return string.Format("{0} dashboards", Configurations.Count);
            }
        }

        private void InitializeForDesigner()
        {
            Configurations = new List<DashboardLayoutSelector>();
            DashboardLayoutSelector layout1 = new DashboardLayoutSelector("User", 1, "SQLdm Default", "SQL Server 2000", DashboardHelper.GetDefaultConfig(new ServerVersion("11.0.0.0"), instanceId), DateTime.Now, DateTime.Now, Properties.Resources.SQLdm_Default);
            DashboardLayoutSelector layout2 = new DashboardLayoutSelector("User", 2, "SQLdm Default", "SQL Server 2005 and later", DashboardHelper.GetDefaultConfig(new ServerVersion("11.0.0.0"), instanceId), DateTime.Now, DateTime.Now, Properties.Resources.SQLdm_Default_2);
            DashboardLayoutSelector layout3 = new DashboardLayoutSelector("User", 3, "SQLdm Default", "SQL Server 2012", DashboardHelper.GetDefaultConfig(new ServerVersion("11.0.0.0"), instanceId), DateTime.Now, DateTime.Now, Properties.Resources.SQLdm_Default_3);
            DashboardLayoutSelector layout4 = new DashboardLayoutSelector("User", 4, "Idera\\User", "My Dashboard", DashboardHelper.GetDefaultConfig(new ServerVersion("11.0.0.0"), instanceId), DateTime.Now, DateTime.Now, Properties.Resources.SQLdm_Default);
            DashboardLayoutSelector layout5 = new DashboardLayoutSelector("User", 5, "Idera\\User", "Test Servers", DashboardHelper.GetDefaultConfig(new ServerVersion("11.0.0.0"), instanceId), DateTime.Now, DateTime.Now, Properties.Resources.SQLdm_Default_2);
            Configurations.Add(layout1);
            Configurations.Add(layout2);
            Configurations.Add(layout3);
            Configurations.Add(layout4);
            Configurations.Add(layout5);
            SelectedId = 2;
        }

        public void Initialize()
        {
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;

            UserName = RepositoryHelper.GetRepositoryUser(connectionInfo);
            Filter = DashboardFilter.User;
            loadData(connectionInfo);
        }

        public void LoadData()
        {
            LoadData(Filter);
        }

        public void LoadData(DashboardFilter filter)
        {
            Filter = filter;
            SqlConnectionInfo connectionInfo = Settings.Default.ActiveRepositoryConnection.ConnectionInfo;
            loadData(connectionInfo);
        }

        private void loadData(SqlConnectionInfo connectionInfo)
        {
            try
            {
                Configurations = RepositoryHelper.GetDashboardLayouts(connectionInfo, UserName, Filter);
            }
            catch (Exception ex)
            {
                Log.Error("Error retrieving dashboard layout from repository. Using internal default instead.", ex);
                UserName = "Unknown";
                Configurations = new List<DashboardLayoutSelector>();
            }

            FirePropertyChanged("Configurations");
        }

        protected void FirePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    //switch (e.PropertyName)
        //    //{
        //    //    case "CollapseRibbon":
        //    //        break;
        //    //}
        //}

        // values are use in stored procedure p_GetDashboardLayouts and need to be kept in sync with it
        public enum DashboardFilter
        {
            Default = 0,
            User = 1,
            All
        }
    }
}
