using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration
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
        private bool multipleActiveResultSets = false;
        private bool encryptData;
        private bool trustServerCert;

        private bool useRemoteWmi = false;

        private string connectionString = "";

        public WmiConnectionInfo WmiConnectionInfo { get; set; }

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
            InstanceName = info.GetString("instanceName");
            userName = info.GetString("userName");
            EncryptedPassword = info.GetString("password");
            useIntegratedSecurity = info.GetBoolean("integratedSecurity");
            allowAsynchronousCommands = info.GetBoolean("allowAsynchronousCommands");
            multipleActiveResultSets = info.GetBoolean("multipleActiveResultSets");
            encryptData = info.GetBoolean("encryptData");
            trustServerCert = info.GetBoolean("trustServerCert");
            try
            {
                useRemoteWmi = info.GetBoolean("useRemoteWmi");
                WmiConnectionInfo = info.GetValue("WmiConnectionInfo", typeof(WmiConnectionInfo)) as WmiConnectionInfo;
            }
            catch { }
        }

        /// <summary>
        /// Initializes a new instance of the SqlConnectionInfo class using
        /// the specified wmi connection information.
        /// </summary>
        /// <param name="wmiInfo">The wmi connection information.</param>
        public SqlConnectionInfo(WmiConnectionInfo wmiInfo)
        {
            WmiConnectionInfo = wmiInfo;
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
        public string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        /// <summary>
        /// Gets the connection string used when the connection is established with the instance of SQL Server.
        /// </summary>
        public string ConnectionString
        {
            get 
            {
                if (connectionString == string.Empty)
                    return GetConnectionString();
                else
                    return connectionString;
            
            }
            set { connectionString = value; }
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
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the password used to establish a connection.
        /// </summary>
        /// <value>The password.</value>
        [XmlIgnore]
        public string Password
        {
            set { password = value; }
            internal get { return password; }
        }

        [XmlIgnore]
        public bool IsPasswordSet
        {
            get { return !String.IsNullOrEmpty(password); }
        }

        /// <summary>
        /// Gets or sets the name of the instance of SQL Server.
        /// </summary>
        /// <value>The instanceName.</value>
        public string InstanceName
        {
            get { return (String.IsNullOrEmpty(instanceName) ? instanceName : instanceName.ToUpper()); }
            set
            {
                if (String.IsNullOrEmpty(value)) throw new ArgumentNullException("instanceName");
                instanceName = value.ToUpper();
            }
        }

        /// <summary>
        /// Gets or sets whether Windows integrated security is used.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use integrated security]; otherwise, <c>false</c>.
        /// </value>
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
        /// Gets or sets a value indicating whether [use multipleActiveResultSets].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use multipleActiveResultSets]; otherwise, <c>false</c>.
        /// </value>
        public bool MultipleActiveResultSets
        {
            get { return multipleActiveResultSets; }
            set { multipleActiveResultSets = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether remote wmi should be used.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [UseRemoteWmi]; otherwise, <c>false</c>.
        /// </value>
        public bool UseRemoteWmi
        {
            get { return useRemoteWmi; }
            set { useRemoteWmi = value; }
        }

        /// <summary>
        /// Get or set a value that causes the client network provider to use SSL 
        /// when connecting to a server.
        /// </summary>
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
            clone.MultipleActiveResultSets = MultipleActiveResultSets;
            clone.EncryptData = EncryptData;
            clone.TrustServerCertificate = TrustServerCertificate;
            clone.ConnectionString = ConnectionString;

            if (!UseIntegratedSecurity)
            {
                clone.UseIntegratedSecurity = UseIntegratedSecurity;
                clone.UserName = UserName;
                clone.Password = Password;
            }

            clone.UseRemoteWmi = UseRemoteWmi;
            if (null != WmiConnectionInfo) clone.WmiConnectionInfo = WmiConnectionInfo.Clone();

            return clone;
        }

        /// <summary>
        /// Get a SqlConnection to the server specified in this connection configuration.
        /// Note that it's up to the caller to open, close and dispose of the connection.
        /// </summary>
        /// <returns>A SQL connection.</returns>
        public SqlConnection GetConnection()
        {
            if (connectionString == string.Empty)
                return new SqlConnection(GetConnectionString());
            else
                return new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection(string applicationName)
        {
            this.applicationName = applicationName;
            return new SqlConnection(GetConnectionString());
        }

        public string GetConnectionString()
        {
            return GetConnectionString(true);
        }

        /// <summary>
        /// Provides a SQL Server connection string representing the collection target.
        /// </summary>
        /// <returns>The connection string.</returns>
        public string GetConnectionString(bool includePassword)
        {
            var connectionString = new SqlConnectionStringBuilder();

            connectionString.ApplicationName = ApplicationName;
            connectionString.DataSource = InstanceName;
            connectionString.InitialCatalog = DatabaseName;

            if (UseIntegratedSecurity)
            {
                connectionString.IntegratedSecurity = true;
            }
            else
            {
                connectionString.UserID = UserName;

                if (includePassword)
                {
                    connectionString.Password = Password;
                }
            }

            connectionString.ConnectTimeout = ConnectionTimeout;
            connectionString.AsynchronousProcessing = AllowAsynchronousCommands;
            connectionString.MultipleActiveResultSets = MultipleActiveResultSets;
            connectionString.Encrypt = encryptData;

            if (encryptData)
            {
                connectionString.TrustServerCertificate = trustServerCert;
            }

            return connectionString.ToString();
        }

        // Added to support logging.
        public override string ToString()
        {
            return string.Format("Instance = '{0}', Database = '{1}'", InstanceName, DatabaseName);
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <returns></returns>
        public ServerVersion GetProductVersion()
        {
            using (SqlConnection connection = GetConnection())
            {
                connection.Open();
                return GetProductVersion(connection);
            }
        }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <param name="conn">The connection.</param>
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
            info.AddValue("instanceName", InstanceName);
            info.AddValue("userName", userName);
            info.AddValue("password", EncryptedPassword);
            info.AddValue("integratedSecurity", useIntegratedSecurity);
            info.AddValue("allowAsynchronousCommands", allowAsynchronousCommands);
            info.AddValue("multipleActiveResultSets", multipleActiveResultSets);
            info.AddValue("encryptData", encryptData);
            info.AddValue("trustServerCert", trustServerCert);

            info.AddValue("useRemoteWmi", useRemoteWmi);
            info.AddValue("WmiConnectionInfo", WmiConnectionInfo);
        }

        public string EncryptedPassword
        {
            get
            {
                if (String.IsNullOrEmpty(password))
                    return null;
                
                byte[] bytes = Encoding.Unicode.GetBytes(password);
                byte[] protectedPassword = ProtectedData.Protect(bytes, null, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(protectedPassword);
            }

            set
            {
                if (String.IsNullOrEmpty(value))
                    password = String.Empty;
                else
                {
                    byte[] bytes = Convert.FromBase64String(value);
                    byte[] decrypted = ProtectedData.Unprotect(bytes, null, DataProtectionScope.CurrentUser);
                    password = Encoding.Unicode.GetString(decrypted);
                }
            }
        }

        public static SqlConnectionInfo DeserializeXml(XmlNode element)
        {
            if (element == null) return null;

            SqlConnectionInfo connection = null;

            try
            {
                var connectionInfoNode = element["WmiConnectionInfo"];
                if (null != connectionInfoNode) connection = new SqlConnectionInfo(WmiConnectionInfo.DeserializeXml(connectionInfoNode));
            }
            catch{}
            if (null == connection) connection = new SqlConnectionInfo();

            var attribute = GetXmlAttributeValue(element, "Instance");
            if (attribute != null) connection.InstanceName = attribute.Value;

            attribute = GetXmlAttributeValue(element, "Database");
            if (attribute != null) connection.DatabaseName = attribute.Value;

            try
            {
                attribute = GetXmlAttributeValue(element, "IntegratedSecurity");
                if (attribute != null) connection.UseIntegratedSecurity = Convert.ToBoolean(attribute.Value);
            }
            catch { }

            if (!connection.UseIntegratedSecurity)
            {
                attribute = GetXmlAttributeValue(element, "UserName");
                if (attribute != null) connection.UserName = attribute.Value;

                try
                {
                    attribute = GetXmlAttributeValue(element, "Password");
                    if (attribute != null) connection.EncryptedPassword = attribute.Value;
                }
                catch (Exception ex) 
                { 
                    ExceptionLogger.Log("Failed to deserialize password! (If accessing with a different user, the user based decryption may be failing!)", ex);
                }
            }

            try
            {
                attribute = GetXmlAttributeValue(element, "AllowAsyncCommands");
                if (attribute != null) connection.AllowAsynchronousCommands = Convert.ToBoolean(attribute.Value);
            }
            catch { }

            try
            {
                attribute = GetXmlAttributeValue(element, "TrustServerCertificate");
                if (attribute != null) connection.TrustServerCertificate = Convert.ToBoolean(attribute.Value);
            }
            catch { }

            try
            {
                attribute = GetXmlAttributeValue(element, "ConnectionTimeout");
                if (attribute != null) connection.ConnectionTimeout = Convert.ToInt32(attribute.Value);
            }
            catch { }

            try
            {
                attribute = GetXmlAttributeValue(element, "UseRemoteWmi");
                if (attribute != null) connection.UseRemoteWmi = Convert.ToBoolean(attribute.Value);
            }
            catch { }

            return connection;
        }

        private static XmlAttribute GetXmlAttributeValue(XmlNode element, string attributeName)
        {
            if (element == null || string.IsNullOrEmpty(attributeName)) return null;

            try
            {
                return element.Attributes[attributeName];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XmlElement SerializeXml(XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
            {
                throw new ArgumentNullException("xmlDocument");
            }

            var element = xmlDocument.CreateElement("ConnectionInfo");

            if (null != WmiConnectionInfo) element.AppendChild(WmiConnectionInfo.SerializeXml(xmlDocument));

            var attribute = xmlDocument.CreateAttribute("Instance");
            attribute.Value = InstanceName;
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("Database");
            attribute.Value = DatabaseName;
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("IntegratedSecurity");
            attribute.Value = UseIntegratedSecurity.ToString();
            element.Attributes.Append(attribute);

            if (!UseIntegratedSecurity)
            {
                attribute = xmlDocument.CreateAttribute("UserName");
                attribute.Value = UserName;
                element.Attributes.Append(attribute);

                attribute = xmlDocument.CreateAttribute("Password");
                attribute.Value = EncryptedPassword;
                element.Attributes.Append(attribute);
            }

            attribute = xmlDocument.CreateAttribute("AllowAsyncCommands");
            attribute.Value = AllowAsynchronousCommands.ToString();
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("EncryptData");
            attribute.Value = EncryptData.ToString();
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("TrustServerCertificate");
            attribute.Value = TrustServerCertificate.ToString();
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("ConnectionTimeout");
            attribute.Value = ConnectionTimeout.ToString();
            element.Attributes.Append(attribute);

            attribute = xmlDocument.CreateAttribute("UseRemoteWmi");
            attribute.Value = UseRemoteWmi.ToString();
            element.Attributes.Append(attribute);

            return element;
        }
    }
}
