using Idera.SQLdm.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Repository
{
    [Serializable]
    internal sealed class RepositoryConnection
    {
        private SqlConnectionInfo connectionInfo;
        private bool savePassword = false;        

        [NonSerialized]
        private RepositoryInfo repositoryInfo;

        public RepositoryConnection()
            : this(new SqlConnectionInfo())
        {
        }

        public RepositoryConnection(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            this.connectionInfo = connectionInfo;

            //AddDefaultServerGroups();
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

        /// <summary>
        /// This method connects to the associated SQL Server and refreshes the repository 
        /// information.
        /// </summary>
        public void RefreshRepositoryInfo()
        {
            repositoryInfo = RepositoryHelper.GetRepositoryInfo(connectionInfo);
        }
    }
}
