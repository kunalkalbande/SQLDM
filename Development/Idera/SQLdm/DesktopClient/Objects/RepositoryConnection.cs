using System;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.DesktopClient.Helpers;
using Idera.SQLdm.DesktopClient.Properties;

namespace Idera.SQLdm.DesktopClient.Objects
{
    [Serializable]
    internal sealed class RepositoryConnection
    {
        private SqlConnectionInfo connectionInfo;
        private bool savePassword = false;
        private readonly UserViewCollection userViews = new UserViewCollection();
        private static BBS.TracerX.Logger StartUpTimeLog = BBS.TracerX.Logger.GetLogger(TextConstants.StartUpTimeLogName);
        [NonSerialized]
        private RepositoryInfo repositoryInfo;

        public RepositoryConnection() : this(new SqlConnectionInfo())
        {
        }

        public RepositoryConnection(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            this.connectionInfo = connectionInfo;

            AddDefaultServerGroups();
        }

        public string ServerName
        {
            get { return connectionInfo.InstanceName; }
            set { connectionInfo.InstanceName = value; }
        }

        public string DatabaseName
        {
            get { return connectionInfo.DatabaseName; }
            set { connectionInfo.DatabaseName = value; }
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        public UserViewCollection UserViews
        {
            get { return userViews; }
        }

        public RepositoryInfo RepositoryInfo
        {
            get { return repositoryInfo; }
        }

        /// <summary>
        /// This property only applies to connections using SQL Server authentication.
        /// </summary>
        public bool SavePassword
        {
            get { return savePassword; }
            set { savePassword = value; }
        }

        private void AddDefaultServerGroups()
        {
            SearchUserView criticalUsers = SearchUserView.CreateView(SearchUserView.CriticalUserViewID);
            userViews.Add(criticalUsers);

            SearchUserView warningUsers = SearchUserView.CreateView(SearchUserView.WarningUserViewID);
            userViews.Add(warningUsers);

            SearchUserView okUsers = SearchUserView.CreateView(SearchUserView.OKUserViewID);
            userViews.Add(okUsers);

            SearchUserView maintModeUsers = SearchUserView.CreateView(SearchUserView.MaintenanceModeUserViewID);
            userViews.Add(maintModeUsers);
        }

        /// <summary>
        /// This method connects to the associated SQL Server and refreshes the repository 
        /// information.
        /// </summary>
        public void RefreshRepositoryInfo()
        {
                var stopWatch = new System.Diagnostics.Stopwatch();
                stopWatch.Start();
                repositoryInfo = RepositoryHelper.GetRepositoryInfo(connectionInfo);
                ApplicationModel.Default.RefershUserSessionSettings(); //SqlDM 10.2 (Anshul Aggarwal) - Persist User Settings
                stopWatch.Stop();
                StartUpTimeLog.DebugFormat("Time taken by  RefreshRepositoryInfo :{0}", stopWatch.ElapsedMilliseconds);
        }
    }
}
