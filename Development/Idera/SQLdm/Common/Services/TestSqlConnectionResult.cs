using System;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Services
{
    [Serializable]
    public class TestSqlConnectionResult
    {
        private SqlConnectionInfo connectionInfo;
        private bool succeeded;
        private string serverVersion = String.Empty;
        private Exception error = null;
        private bool isAdmin;

        private TestSqlConnectionResult(SqlConnectionInfo connectionInfo)
        {
            if (connectionInfo == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }
            
            this.connectionInfo = connectionInfo;    
        }

        public TestSqlConnectionResult(SqlConnectionInfo connectionInfo, string serverVersion)
            : this (connectionInfo)
        {
            this.succeeded = true;
            this.serverVersion = serverVersion;
        }

        public TestSqlConnectionResult(SqlConnectionInfo connectionInfo, string serverVersion,bool isAdmin)
            : this(connectionInfo)
        {
            this.succeeded = true;
            this.serverVersion = serverVersion;
            this.isAdmin = isAdmin;

        }

        public TestSqlConnectionResult(SqlConnectionInfo connectionInfo, Exception error)
            : this(connectionInfo)
        {
            this.succeeded = false;
            this.error = error;
        }

        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
        }

        public bool Succeeded
        {
            get { return succeeded; }
        }

        public string ServerVersion
        {
            get { return serverVersion; }
        }

        public bool IsAdmin
        {
            get { return isAdmin; }
        }

        public Exception Error
        {
            get { return error; }
        }
    }
}