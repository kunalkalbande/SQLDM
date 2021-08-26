//------------------------------------------------------------------------------
// <copyright file="SqlConnectionInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Text;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Security.Encryption;
using System.Threading;

namespace Idera.SQLdm.Common.Configuration
{
    /// <summary>
    /// Stores connection info for connecting to a SQL Server.
    /// 
    /// Password is stored as plain-text.  It is encrypted when it is
    /// serialized or when being stored.
    /// </summary>
    [Serializable]
    public sealed class SqlConnectionInfo : ISerializable
    {
        #region constants

        private const string CipherInstanceName = "Idera.SQLdm.Common";
        private const string DefaultApplicationName = Constants.ConnectionStringApplicationNamePrefix;
        private const string DefaultDatabaseName = "master";

        #endregion

        #region fields

        private static int defaultConnectionTimeout = Constants.DefaultConnectionTimeout;

        private int connectionTimeout = DefaultConnectionTimeout;
        private string applicationName = DefaultApplicationName;
        private string instanceName;
        private string databaseName = DefaultDatabaseName;
        private bool useIntegratedSecurity = true;
        private string userName;
        private string password;
        private bool allowAsynchronousCommands = true;
        private bool encryptData;
        private bool trustServerCert;
        private string isMultiSubnetFailoverRequiredFlag = null;

        #endregion

        #region constructors

        public SqlConnectionInfo()
        {
        }

        private SqlConnectionInfo(SerializationInfo info, StreamingContext context)
        {
            
            connectionTimeout = info.GetInt32("timeout");
            applicationName = info.GetString("application");
            databaseName = info.GetString("databaseName");
            instanceName = info.GetString("instanceName");
            userName = info.GetString("userName");
            EncryptedPassword = info.GetString("password");
            useIntegratedSecurity = info.GetBoolean("integratedSecurity");
            allowAsynchronousCommands = info.GetBoolean("allowAsynchronousCommands");
            encryptData = info.GetBoolean("encryptData");
            trustServerCert = info.GetBoolean("trustServerCert");
        }

        /// <summary>
        /// Initializes a new instance of the ServerConnection class using
        /// the specified server name and default Windows Integrated security.
        /// </summary>
        /// <param name="instanceName">The instanceName.</param>
        public SqlConnectionInfo(string instanceName)
        {
            InstanceName = instanceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlConnectionInfo"/> class.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="databaseName">The database name.</param>
        public SqlConnectionInfo(string instanceName, string databaseName)
            : this(instanceName)
        {
            DatabaseName = databaseName;
        }

        /// <summary>
        /// Initializes a new instance of the ServerConnection class using
        /// the specified computer name and sets up SQL Server authentication using
        /// the specifed username and password.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password to use for SQL Server authentication.</param>
        public SqlConnectionInfo(string instanceName, string userName, string password)
            : this(instanceName)
        {
            useIntegratedSecurity = false;
            UserName = userName;
            Password = password;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SqlConnectionInfo"/> class.
        /// </summary>
        /// <param name="instanceName">The instance name.</param>
        /// <param name="databaseName">The database name.</param>
        /// <param name="userName">The username.</param>
        /// <param name="password">The password.</param>
        public SqlConnectionInfo(string instanceName, string databaseName, string userName, string password)
            : this(instanceName, userName, password)
        {
            DatabaseName = databaseName;
        }

        #endregion

        #region properties

        [Auditable("Active Repository User")]
        public string ActiveRepositoryUser
        {
            get
            {
                String FullUserNameTemplate = "{0}\\{1}";
                string workStationUser = String.Format(FullUserNameTemplate, Environment.UserDomainName, Environment.UserName);

                return UseIntegratedSecurity ? workStationUser : UserName;
            }
        }

        public static int DefaultConnectionTimeout
        {
            get { return defaultConnectionTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("DefaultConnectionTimeout");

                defaultConnectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of seconds before a connection times out.
        /// </summary>
        [Auditable(false)]
        public int ConnectionTimeout
        {
            get { return connectionTimeout; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("connectionTimeout");

                connectionTimeout = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        [AuditableAttribute(false)]
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Gets the connection string used when the connection is established with the instance of SQL Server.
        /// </summary>
        [AuditableAttribute(false)]
        public string ConnectionString
        {
            get { return GetConnectionString(); }
        }

        /// <summary>
        /// Gets or sets the default database name.
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets a string that represents the login name to use for
        /// SQL Server authentication mode.
        /// </summary>
        [Auditable("Changed user name to")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the password used to establish a connection.
        /// </summary>
        /// <value>The password.</value>
        [AuditableAttribute(true,true)]
        public string Password
        {
            set { password = value; }
            get { return password; }
        }

        /// <summary>
        /// Gets or sets the name of the instance of SQL Server.
        /// </summary>
        /// <value>The instanceName.</value>
        public string InstanceName
        {
            get { return instanceName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("instanceName");
                instanceName = value;
            }
        }

        /// <summary>
        /// Gets or sets whether Windows integrated security is used.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use integrated security]; otherwise, <c>false</c>.
        /// </value>
        [Auditable("Windows Integrated Authentication")]
        public bool UseIntegratedSecurity
        {
            get { return useIntegratedSecurity; }
            set { useIntegratedSecurity = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use asynchronous processing].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use asynchronous processing]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowAsynchronousCommands
        {
            get { return allowAsynchronousCommands; }
            set { allowAsynchronousCommands = value; }
        }

        /// <summary>
        /// Get or set a value that causes the client network provider to use SSL 
        /// when connecting to a server.
        /// </summary>
        [Auditable("Encrypted Connection with SSL")]
        public bool EncryptData
        {
            get { return encryptData; }
            set { encryptData = value; }
        }

        /// <summary>
        /// Get or set a value that causes the client network provider to skip (trust) 
        /// the certificate used when an SSL connection is negotiated between the client
        /// and the server.  Setting this to true will allow use of self-signed certificates.
        /// </summary>
        [Auditable("Trust Server Certificate Encryption")]
        public bool TrustServerCertificate
        {
            get { return trustServerCert; }
            set { trustServerCert = value; }
        }


        #endregion

        #region methods

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public SqlConnectionInfo Clone()
        {
            SqlConnectionInfo clone = new SqlConnectionInfo(InstanceName);
            clone.ApplicationName = ApplicationName;
            clone.DatabaseName = DatabaseName;
            clone.ConnectionTimeout = ConnectionTimeout;
            clone.AllowAsynchronousCommands = AllowAsynchronousCommands;
            clone.EncryptData = EncryptData;
            clone.TrustServerCertificate = TrustServerCertificate;

            if (!UseIntegratedSecurity)
            {
                clone.UseIntegratedSecurity = UseIntegratedSecurity;
                clone.UserName = UserName;
                clone.Password = Password;
            }

            return clone;
        }

        /// <summary>
        /// Get a SqlConnection to the server specified in this connection configuration.
        /// Note that it's up to the caller to open, close and dispose of the connection.
        /// </summary>
        /// <returns>A SQL connection.</returns>
        public SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
        //sqldm-30013 start
        public SqlConnection GetConnectionDatabase(String dbname)
        {
            return new SqlConnection(GetConnectionString(dbname));
        }

        public SqlConnection GetConnection(string applicationName, string database)
        {
            if (database == null)
            {
                return GetConnection(applicationName);
            }
            this.applicationName = applicationName;
            return new SqlConnection(GetConnectionString(database));
        }

        //sqldm-30013 end
        public SqlConnection GetConnection(string applicationName)
        {
            this.applicationName = applicationName;
            return new SqlConnection(GetConnectionString());
        }
		
		//START SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding new method which return connection with increased timeout
        public SqlConnection GetQueryMonitorConnection(string applicationName)
        {
            this.applicationName = applicationName;
            return new SqlConnection(GetQueryMonitorConnectionString());
        }

        private string GetQueryMonitorConnectionString()
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder(GetConnectionString());
            connectionString.ConnectTimeout = Constants.DefaultQueryMonitorConnectionTimeout;
            return connectionString.ConnectionString;
        }
		//END SQLdm 9.1 (Ankit Srivastava ) --Rally Defect DE43572 -- Adding new method which return connection with increased timeout
        /// <summary>
        /// Provides a SQL Server connection string representing the collection target.
        /// </summary>
        /// <returns>The connection string.</returns>
        private string GetConnectionString(String dbname=null)
        {
            SqlConnectionStringBuilder connectionString = new SqlConnectionStringBuilder();

            // Application name
            connectionString.ApplicationName = ApplicationName;

            // Server name
            connectionString.DataSource = InstanceName;

            // Database name
            if (dbname == null)//sqldm-30013 // making individual connection to db
            {                         
                connectionString.InitialCatalog = DatabaseName;
            }
            else
            {
                connectionString.InitialCatalog = dbname;
            }

            // Authentication information
            if (UseIntegratedSecurity)
            {
                connectionString.IntegratedSecurity = true;    
            }
            else
            {
                connectionString.UserID = UserName;
                connectionString.Password = Password;
            }

            // Connection Timeout
            connectionString.ConnectTimeout = ConnectionTimeout;

            //Asynchronous Command Support
            connectionString.AsynchronousProcessing = AllowAsynchronousCommands;

            //SSL Support
            connectionString.Encrypt = encryptData;
            if (encryptData)
            {
                connectionString.TrustServerCertificate = trustServerCert;    
            }

            if (isMultiSubnetFailoverRequiredFlag == null)
            {
                try
                {
                    isMultiSubnetFailoverRequired(connectionString.ConnectionString);
                }
                catch (Exception ex)
                {
                    
                }
                if (isMultiSubnetFailoverRequiredFlag == "1")
                {
                    connectionString.ConnectionString += ";MultiSubnetFailover=True;";
                }
            }
           
            return connectionString.ConnectionString;
        }

        private void isMultiSubnetFailoverRequired(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand sqlcommand = new SqlCommand("SELECT SERVERPROPERTY ('IsHadrEnabled');", connection))
                {
                    using (SqlDataReader dataReader = sqlcommand.ExecuteReader())
                    {
                        while (dataReader.Read())
                        {
                            string value = dataReader[0].ToString();
                            isMultiSubnetFailoverRequiredFlag = value;
                        }
                    }
                }

                //SQLDM-29229. Handle Multisubnetfailover Flag for AG's without Listeners.
                 using (SqlCommand sqlcommand = new SqlCommand("Select * From sys.availability_group_listeners;", connection))
                {
                    using (SqlDataReader dataReader = sqlcommand.ExecuteReader())
                    {
                        // If the Data is null i.e., there are no listseners in the AG, change the isMultiSubnetFailoverRequiredFlag to null.
                        if (dataReader == null || dataReader.HasRows == false)
                        {
                            isMultiSubnetFailoverRequiredFlag = null;
                        }
                    }
                }
                

            }
        }

        // Added to support logging.
        public override string ToString() {
            return string.Format("Instance = '{0}', Database = '{1}'", InstanceName, DatabaseName);
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <returns></returns>
        public ServerVersion GetProductVersion()
        {
            using (SqlConnection conn = GetConnection())
            {
                return GetProductVersion(conn);
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <param name="conn">The conn.</param>
        /// <returns></returns>
        public ServerVersion GetProductVersion(SqlConnection conn)
        {
            return new ServerVersion(conn.ServerVersion);
        }

        #endregion

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("timeout", connectionTimeout);
            info.AddValue("application", applicationName);
            info.AddValue("databaseName", databaseName);
            info.AddValue("instanceName", instanceName);
            info.AddValue("userName", userName);
            info.AddValue("password", EncryptedPassword);
            info.AddValue("integratedSecurity", useIntegratedSecurity);
            info.AddValue("allowAsynchronousCommands", allowAsynchronousCommands);
            info.AddValue("encryptData", encryptData);
            info.AddValue("trustServerCert", trustServerCert);
        }

        [AuditableAttribute(false)]
        public string EncryptedPassword
        {
            get
            {
                // this will produce a different value every time
                return Cipher.EncryptPassword(CipherInstanceName, password);
            }

            set { password = Cipher.DecryptPassword(CipherInstanceName, value); }
        }
    }
}
