using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Security.Principal;
using System.Xml;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Auditing;
using Microsoft.ApplicationBlocks.Data;

namespace Idera.SQLdm.Common.Objects.ApplicationSecurity
{
    /// <summary>
    /// Permissions supported by SQLdm.
    /// </summary>
    public enum PermissionType { View, ReadOnlyPlus , Modify, Administrator, None };//modified by Kunal(9/5/2017)

    /// <summary>
    /// Type of login.
    /// </summary>
    public enum LoginType
    {
        [Description("Unknown")]
        Unknown,
        [Description("Windows Authentication")]
        WindowsUser,
        [Description("Windows Authentication")]
        WindowsGroup,
        [Description("SQL Server Authentication")]
        SQLLogin
    };

    /// <summary>
    /// This class encapsulates basic monitored SQL Server information.
    /// </summary>
    [Serializable]
    public class Server : IComparable
    {
        #region members

        private int m_SQLServerID;
        private string m_InstanceName;
        private bool m_Active;
        private bool m_Deleted;

        #endregion

        #region ctors

        public Server(int sqlServerID, string instanceName, bool active, bool deleted)
        {
            m_SQLServerID = sqlServerID;
            m_InstanceName = instanceName;
            m_Active = active;
            m_Deleted = deleted;
        }

        #endregion

        #region properties

        public int SQLServerID
        {
            get { return m_SQLServerID; }
        }

        public string InstanceName
        {
            get { return m_InstanceName; }
        }

        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public bool Deleted
        {
            get { return m_Deleted; }
        }

        #endregion

        #region methods

        public override bool Equals(object rhs)
        {
            if (rhs == this) { return true; }

            Server other = rhs as Server;
            if (other == null) return false;

            return m_InstanceName.Equals(other.m_InstanceName);
        }

        public override int GetHashCode()
        {
            return m_InstanceName.GetHashCode();
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            Server other = rhs as Server;
            if (other == null) return 1;

            return m_InstanceName.CompareTo(other.m_InstanceName);
        }

        public override string ToString()
        {
            return m_InstanceName;
        }

        #endregion
    }

    /// <summary>
    /// This class encapsulates server permission
    /// </summary>
    [Serializable]
    public class ServerPermission : IComparable
    {
        #region members

        private Server m_Server;
        private PermissionType m_PermissionType;

        #endregion

        #region ctors

        public ServerPermission(int sqlServerID, string instanceName, bool active, bool deleted, PermissionType permissionType)
        {
            m_Server = new Server(sqlServerID, instanceName, active, deleted);
            m_PermissionType = permissionType;
        }

        #endregion

        #region properties

        public Server Server
        {
            get { return m_Server; }
        }

        public PermissionType PermissionType
        {
            get { return m_PermissionType; }
        }

        #endregion

        #region methods

        public void updateToHighestPermission(PermissionType pt)
        {
            // If the existing permission type is View and
            // input is Modify, then set to Modify.....etc.
            if ((int)m_PermissionType < (int)pt)
            {
                m_PermissionType = pt;
            }
        }

        public override bool Equals(object rhs)
        {
            if (rhs == this) { return true; }

            ServerPermission other = rhs as ServerPermission;
            if (other == null) return false;

            return m_Server.Equals(other.m_Server);
        }

        public override int GetHashCode()
        {
            return m_Server.GetHashCode();
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            ServerPermission other = rhs as ServerPermission;
            if (other == null) return 1;

            return m_Server.CompareTo(other);
        }

        public override string ToString()
        {
            return m_Server.InstanceName + ":" + m_PermissionType;
        }

        #endregion
    }

    [Serializable]
    public class PermissionDefinition : IComparable, ICloneable, IAuditable
    {
        #region members

        private int m_PermissionID;
        private bool m_System;
        private bool m_Enabled;
        private string m_Login;
        private LoginType m_LoginType;
        private PermissionType m_PermissionType;
        private List<Server> m_Servers = new List<Server>();
        private string m_Comment;
        private bool m_WebAppPermission;   // SQLdm 8.5 (Ankit Srivastava) -- Web Access 

        #endregion

        #region ctors

        public PermissionDefinition(
                int permissionID, 
                bool system,
                bool enabled,
                byte[] loginSID,
                string login, 
                LoginType loginType,
                PermissionType permissionType,
                int sqlServerID,
                string instanceName,
                bool active,
                bool deleted,
                string comment,
                bool webAppPermission
            )
        {
            m_PermissionID = permissionID;
            m_System = system;
            m_WebAppPermission = webAppPermission;
            m_Enabled = enabled;
            m_Login = login;
            m_LoginType = loginType;
            m_PermissionType = permissionType;
            AddInstance(sqlServerID, instanceName, active, deleted);
            m_Comment = comment;
        }

        public PermissionDefinition(
                int permissionID,
                bool system,
                bool enabled,
                byte[] loginSID,
                string login,
                LoginType loginType,
                PermissionType permissionType,
                string comment,
                bool webAppPermission
            )
        {
            m_PermissionID = permissionID;
            m_System = system;
            m_Enabled = enabled;
            m_WebAppPermission = webAppPermission;
            m_Login = login;
            m_LoginType = loginType;
            m_PermissionType = permissionType;
            m_Comment = comment;
        }

        #endregion

        #region properties

        [Auditable(false)]
        public int PermissionID
        {
            get { return m_PermissionID; }
        }

        [Auditable(false)]
        public bool System
        {
            get { return m_System; }
        }

        [Auditable("Enabled")]
        public bool Enabled
        {
            get { return m_Enabled; }
            set { m_Enabled = value; }
        }

        [Auditable("Web Application Access")]
        public bool WebAppPermission
        {
            get { return m_WebAppPermission; }
            set { m_WebAppPermission = value; }
        }

        [Auditable("Login")]
        public string Login
        {
            get { return m_Login; }
        }

        [Auditable("Login Type")]
        public LoginType LoginType
        {
            get { return m_LoginType; }
        }

        [Auditable("Permission")]
        public PermissionType PermissionType
        {
            get { return m_PermissionType; }
        }

        [Auditable("Instances")]
        public string Instances
        {
            get 
            {
                string ret = string.Empty;

                if (System || PermissionType == PermissionType.Administrator)
                {
                    ret = "< All >";
                }
                else if (m_Servers.Count == 0)
                {
                    ret = "< None >";
                }
                else
                {
                    int index = 0;
                    foreach (Server s in m_Servers)
                    {
                        ret += s.InstanceName;
                        if (++index < m_Servers.Count) { ret += ", "; }
                    }
                }
                
                return ret;
            }
        }

        [Auditable("Comment")]
        public string Comment
        {
            get { return m_Comment; }
            set { m_Comment = value; }
        }

        #endregion

        #region methods

        public void AddInstance(int sqlServerID, string instanceName, bool active, bool deleted)
        {
            m_Servers.Add(new Server(sqlServerID, instanceName, active, deleted));
        }

        public string LoginTypeAsString()
        {
            string ret = "Unknown";
            switch (m_LoginType)
            {
                case LoginType.WindowsGroup:
                    ret = "Windows Group";
                    break;

                case LoginType.WindowsUser:
                    ret = "Windows User";
                    break;

                case LoginType.SQLLogin:
                    ret = "SQL Login";
                    break;
            }
            return ret;
        }

        public List<Server> GetServerList()
        {
            return m_Servers;
        }

        public override bool Equals(object rhs)
        {
            if (rhs == this) { return true; }

            PermissionDefinition other = rhs as PermissionDefinition;
            if (other == null) return false;

            return m_PermissionID == other.m_PermissionID;
        }

        public override int GetHashCode()
        {
            return m_PermissionID;
        }

        public int CompareTo(object rhs)
        {
            if (this == rhs) return 0;

            PermissionDefinition other = rhs as PermissionDefinition;
            if (other == null) return 1;

            if (other.m_PermissionID > m_PermissionID) return -1;
            else if (other.m_PermissionID < m_PermissionID) return 1;
            else return 0;
        }

        #endregion

        public object Clone()
        {
            PermissionDefinition result = (PermissionDefinition)this.MemberwiseClone();
            result.m_Servers = new List<Server>(m_Servers);
            return result;
        }

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            var auditableProperties = PropertiesComparer.GetAuditableAttributes(this);
            var entity = DeployPropertiesData(auditableProperties);

            return entity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            var propertiesComparer = new PropertiesComparer();
            var auditableProperties = propertiesComparer.GetNewProperties(oldValue, this);
            var entity = DeployPropertiesData(auditableProperties);

            return entity;
        }

        private AuditableEntity DeployPropertiesData(List<PropertiesComparer.PropertiesData> propertiesDatas)
        {
            AuditableEntity entity = new AuditableEntity();

            foreach (var property in propertiesDatas)
            {
                if ("Comment".Equals(property.Name) && String.IsNullOrEmpty(property.Value))
                {
                    property.Value = "None";
                }

                entity.AddMetadataProperty(property.Name, property.Value);
            }

            return entity;
        }
    }

    [Serializable]
    public class Configuration
    {
        #region members

        // Security enabled column.
        private const int ColIsSecurityEnabled = 0;

        // Permission columns
        private const int ColPermissionID = 0;
        private const int ColSystem = 1;
        private const int ColEnabled = 2;
        private const int ColLoginSID = 3;
        private const int ColLogin = 4;
        private const int ColLoginType = 5;
        private const int ColPermissionType = 6;
        private const int ColSQLServerID = 7;
        private const int ColInstanceName = 8;
        private const int ColActive = 9;
        private const int ColDeleted = 10;
        private const int ColComment = 11;
        private const int ColWebAppPermission = 12;

        // Members
        private bool m_IsSecurityEnabled = false;
        private Dictionary<int,PermissionDefinition> m_Permissions = new Dictionary<int,PermissionDefinition>();

        #endregion

        #region ctors

        public Configuration()
        {
        }

        #endregion

        #region properties

        public bool IsSecurityEnabled
        {
            get { return m_IsSecurityEnabled; }
        }

        public Dictionary<int,PermissionDefinition> Permissions
        {
            get { return m_Permissions; }
        }

        #endregion

        #region methods

        public void Refresh(string connectionString)
        {
            // Clear members.
            m_IsSecurityEnabled = false;
            m_Permissions.Clear();

            // Refresh permissions.
            int sysPermissionID = -1;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_GetPermissions"))
            {
                // Read the security enabled flag.
                if (reader.Read())
                {
                    m_IsSecurityEnabled = reader.IsDBNull(ColIsSecurityEnabled) ? false : reader.GetSqlBoolean(ColIsSecurityEnabled).Value;
                }

                // Read permissions.
                if (IsSecurityEnabled && reader.NextResult())
                {
                    while (reader.Read())
                    {
                        // Read permission ID.   If the permission id is -1, it means its a system
                        // permission.  So use a different index that keeps decreasing.
                        SqlInt32 permissionID = !reader.IsDBNull(ColPermissionID)? reader.GetInt32(ColPermissionID):SqlInt32.Null;
                        int id = permissionID.Value == -1 ? sysPermissionID-- : permissionID.Value;

                        // If permission exists, then add the monitored server instance to the 
                        // permission.   Else create a new permission and add to dictionary.
                        PermissionDefinition permission = null;
                        if (m_Permissions.TryGetValue(id, out permission))
                        {
                            // Read server id and instance name, and add to the list of instances assigned to
                            // the permission.
                            SqlInt32 sqlServerID = reader.IsDBNull(ColSQLServerID) ? SqlInt32.Null : reader.GetInt32(ColSQLServerID);
                            SqlString instanceName = reader.IsDBNull(ColInstanceName) ? SqlString.Null : reader.GetSqlString(ColInstanceName);
                            SqlBoolean active = reader.IsDBNull(ColActive) ? SqlBoolean.Null : reader.GetSqlBoolean(ColActive);
                            SqlBoolean deleted = reader.IsDBNull(ColDeleted) ? SqlBoolean.Null : reader.GetSqlBoolean(ColDeleted);
                            if (!sqlServerID.IsNull && !instanceName.IsNull && !active.IsNull && !deleted.IsNull)
                            {
                                permission.AddInstance(sqlServerID.Value, instanceName.Value, active.Value, deleted.Value);
                            }
                        }
                        else
                        {
                            // Read the fields and create permission object.
                            SqlInt32 system = reader.GetInt32(ColSystem);
                            SqlInt32 enabled = reader.GetSqlInt32(ColEnabled);
                            SqlBinary loginSID = reader.GetSqlBinary(ColLoginSID);
                            SqlString login = reader.IsDBNull(ColLogin) ? SqlString.Null : reader.GetSqlString(ColLogin);
                            SqlInt32 loginType = reader.GetSqlInt32(ColLoginType);
                            SqlInt32 permissionType = reader.GetSqlInt32(ColPermissionType);
                            SqlInt32 sqlServerID = reader.IsDBNull(ColSQLServerID) ? SqlInt32.Null : reader.GetSqlInt32(ColSQLServerID);
                            SqlString instanceName = reader.IsDBNull(ColInstanceName) ? SqlString.Null : reader.GetSqlString(ColInstanceName);
                            SqlBoolean active = reader.IsDBNull(ColActive) ? SqlBoolean.Null : reader.GetSqlBoolean(ColActive);
                            SqlBoolean deleted = reader.IsDBNull(ColDeleted) ? SqlBoolean.Null : reader.GetSqlBoolean(ColDeleted);
                            SqlString comment = reader.IsDBNull(ColComment) ? SqlString.Null : reader.GetSqlString(ColComment);
                            SqlInt32 webAppPermission = reader.IsDBNull(ColWebAppPermission) ? SqlInt32.Zero : reader.GetSqlInt32(ColWebAppPermission);
                            
                            if (!sqlServerID.IsNull && !instanceName.IsNull && !active.IsNull)
                            {
                                permission = new PermissionDefinition(id, system.Value == 1, enabled.Value == 1, loginSID.Value,
                                                                login.IsNull ? "" : login.Value,
                                                                    (LoginType)loginType.Value, (PermissionType)permissionType.Value,
                                                                        sqlServerID.Value, instanceName.Value, active.Value, deleted.Value,
                                                                            comment.Value, webAppPermission.Value == 1);
                            }
                            else
                            {
                                permission = new PermissionDefinition(
                                                    id, 
                                                    system.Value == 1, 
                                                    enabled.Value == 1, 
                                                    loginSID.Value,
                                                    login.IsNull ? String.Empty : login.Value,
                                                    (LoginType)loginType.Value, 
                                                    (PermissionType)permissionType.Value,
                                                    comment.IsNull ? String.Empty : comment.Value,
                                                    webAppPermission.Value == 1);
                            }

                            // Add to permissions collection.
                            m_Permissions.Add(id, permission);
                        }
                    }
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// This class encapsulates permissions assigned to a SQLdm user.   
    /// </summary>
    [Serializable]
    public class UserToken
    {
        #region members

        // flag columns
        private const int ColFlagIsSecurityEnabled = 0;
        private const int ColFlagIsSysadmin = 1;
        private const int ColFlagIsSQLdmAdministrator = 2;

        // assigned server columns
        private const int ColSQLServerID = 0;
        private const int ColInstanceName = 1;
        private const int ColActive = 2;
        private const int ColDeleted = 3;
        private const int ColPermissionType = 4;

        private bool m_IsSecurityEnabled = true;
        private bool m_IsSysadmin = true;
        private bool m_IsSQLdmAdministrator = false;
        private byte[] m_SID = null;//SQLdm 10.0 (Gaurav Karwal): The SID to be used in rest api
        private Dictionary<int, ServerPermission> m_ServersDictionaryById = new Dictionary<int, ServerPermission>();
        private Dictionary<string, ServerPermission> m_ServersDictionaryByName = new Dictionary<string, ServerPermission>();
        private List<ServerPermission> m_ActiveServers = new List<ServerPermission>();

        #endregion

        #region ctors

        public UserToken()
        {
        }

        #endregion

        #region properties

        public bool IsSecurityEnabled
        {
            get 
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSecurityEnabled;
                }
                return flag;
            }
        }

        //[START] SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api
        public string UserSID {
            get 
            {
                return new SecurityIdentifier(m_SID,0).Value;
            }
        }
        //[END] SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api

        public bool IsSysadmin
        {
            get 
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSysadmin;
                }
                return flag;
            }
        }

        public bool IsSQLdmAdministrator
        {
            get 
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSQLdmAdministrator;
                }
                return flag;
            }
        }

        public bool IsAnySQLdmPermissionAssigned
        {
            get 
            {
                bool flag = false;
                lock (this)
                {
                    flag = m_IsSQLdmAdministrator || m_ServersDictionaryById.Count > 0;
                }
                return flag; 
            }
        }

        public IEnumerable<ServerPermission> AssignedServers
        {
            get 
            {
                ServerPermission[] array = null;
                lock(this)
                {
                    array = new ServerPermission[m_ServersDictionaryById.Count];
                    m_ServersDictionaryById.Values.CopyTo(array, 0);
                }
                return array;
            }
        }

        public IEnumerable<ServerPermission> ActiveAssignedServers
        {
            get 
            {
                ServerPermission[] array = null;
                lock (this)
                {
                    array = new ServerPermission[m_ServersDictionaryById.Count];
                    m_ActiveServers.CopyTo(array, 0);
                }
                return array;
            }
        }

        public ICollection<int> ActiveAssignedServerIds
        {
            get { return m_ServersDictionaryById.Keys; }
        }

        public string ActiveAssignedServersXmlFilter
        {
            get
            {
                // Create xml doc with servers list.
                XmlDocument xmlDoc = new XmlDocument();
                XmlElement rootElement = xmlDoc.CreateElement("Servers");
                xmlDoc.AppendChild(rootElement);

                lock (this)
                {
                    foreach (ServerPermission sp in m_ActiveServers)
                    {
                        XmlElement e = xmlDoc.CreateElement("Server");
                        e.SetAttribute("InstanceName", sp.Server.InstanceName);
                        xmlDoc.FirstChild.AppendChild(e);
                    }
                }

                return xmlDoc.InnerXml;
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Refresh by connecting to the repository and determining permissions.
        /// </summary>
        /// <param name="connectionString"></param>
        public void Refresh(string connectionString)
        {
            lock (this)
            {
                // Clear internal fields.
                m_IsSecurityEnabled = m_IsSysadmin = m_IsSQLdmAdministrator = false;
                m_ServersDictionaryById.Clear();
                m_ServersDictionaryByName.Clear();
                m_ActiveServers.Clear();

                // Get credentials information.
                bool isIntegratedSecurity = false;
                string sqlLogin = "";
                byte[] sidByteArray = null;
                getCredentials(connectionString, out isIntegratedSecurity, out sqlLogin, out sidByteArray);

                m_SID = sidByteArray;//SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api

                // Create parameters for user token SP.
                SqlParameter[] arParms = new SqlParameter[3];
                arParms[0] = new SqlParameter("@IsSQLLogin", SqlDbType.Bit);
                arParms[0].Direction = ParameterDirection.Input;
                arParms[0].Value = !isIntegratedSecurity;
                
                arParms[1] = new SqlParameter("@SQLLoginName", SqlDbType.NVarChar);
                arParms[1].Direction = ParameterDirection.Input;
                arParms[1].Value = sqlLogin;
                
                arParms[2] = new SqlParameter("@WindowsSID", SqlDbType.Binary);
                arParms[2].Direction = ParameterDirection.Input;
                arParms[2].Value = sidByteArray;

                // Get user token from repository and fill the fields.
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_GetUserToken", arParms))
                {
                    // Read the security enabled, sysadmin and sqldm admin flags.
                    if (reader.Read())
                    {
                        m_IsSecurityEnabled = reader.IsDBNull(ColFlagIsSecurityEnabled) ? false : reader.GetSqlBoolean(ColFlagIsSecurityEnabled).Value;
                        m_IsSysadmin = reader.IsDBNull(ColFlagIsSysadmin) ? false : reader.GetSqlBoolean(ColFlagIsSysadmin).Value;
                        m_IsSQLdmAdministrator = reader.IsDBNull(ColFlagIsSQLdmAdministrator) ? false : reader.GetSqlBoolean(ColFlagIsSQLdmAdministrator).Value;
                    }

                    // Read assigned servers.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            // Get server ID and permission type.
                            int sqlServerID = reader.GetSqlInt32(ColSQLServerID).Value;
                            PermissionType permissionType = (PermissionType)reader.GetSqlInt32(ColPermissionType).Value;

                            // If server already in assigned server update its permission,
                            // Else add a new server permission.
                            ServerPermission serverPermission = null;
                            if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                            {
                                serverPermission.updateToHighestPermission(permissionType);
                            }
                            else
                            {
                                string instanceName = reader.GetSqlString(ColInstanceName).Value;
                                bool active = reader.GetSqlBoolean(ColActive).Value;
                                bool deleted = reader.GetSqlBoolean(ColDeleted).Value;
                                serverPermission = new ServerPermission(sqlServerID, instanceName, active, deleted, permissionType);
                                m_ServersDictionaryById.Add(sqlServerID, serverPermission);
                                m_ServersDictionaryByName.Add(instanceName, serverPermission);
                            }

                            // If server is active, add to active servers list.
                            if (serverPermission.Server.Active)
                            {
                                m_ActiveServers.Add(serverPermission);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Refresh by connecting to the repository and determining permissions.
        /// </summary>
        /// <param name="connectionString"></param>
        public void RefreshForWebUI(bool isIntegratedSecurity, string loginName, string connectionString)
        {
            lock (this)
            {
                // Clear internal fields.
                m_IsSecurityEnabled = m_IsSysadmin = m_IsSQLdmAdministrator = false;
                m_ServersDictionaryById.Clear();
                m_ServersDictionaryByName.Clear();
                m_ActiveServers.Clear();

                // Get credentials information.
                //bool isIntegratedSecurity = false;
                string sqlLogin = "";
                byte[] sidByteArray = null;
                getCredentials(connectionString, out isIntegratedSecurity, out sqlLogin, out sidByteArray);

                m_SID = sidByteArray;//SQLdm 10.0 (Gaurav Karwal): getting the SID to be used in rest api

                // Create parameters for user token SP.
                SqlParameter[] arParms = new SqlParameter[2];
                arParms[0] = new SqlParameter("@IsSQLLogin", SqlDbType.Bit);
                arParms[0].Direction = ParameterDirection.Input;
                arParms[0].Value = !isIntegratedSecurity;

                arParms[1] = new SqlParameter("@SQLLoginName", SqlDbType.NVarChar);
                arParms[1].Direction = ParameterDirection.Input;
                arParms[1].Value = loginName;

                //arParms[2] = new SqlParameter("@WindowsSID", SqlDbType.Binary);
                //arParms[2].Direction = ParameterDirection.Input;
                //arParms[2].Value = sidByteArray;

                // Get user token from repository and fill the fields.
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_GetUserTokenForWeb", arParms))
                {
                    // Read the security enabled, sysadmin and sqldm admin flags.
                    if (reader.Read())
                    {
                        m_IsSecurityEnabled = reader.IsDBNull(ColFlagIsSecurityEnabled) ? false : reader.GetSqlBoolean(ColFlagIsSecurityEnabled).Value;
                        m_IsSysadmin = reader.IsDBNull(ColFlagIsSysadmin) ? false : reader.GetSqlBoolean(ColFlagIsSysadmin).Value;
                        m_IsSQLdmAdministrator = reader.IsDBNull(ColFlagIsSQLdmAdministrator) ? false : reader.GetSqlBoolean(ColFlagIsSQLdmAdministrator).Value;
                    }

                    // Read assigned servers.
                    if (reader.NextResult())
                    {
                        while (reader.Read())
                        {
                            // Get server ID and permission type.
                            int sqlServerID = reader.GetSqlInt32(ColSQLServerID).Value;
                            PermissionType permissionType = (PermissionType)reader.GetSqlInt32(ColPermissionType).Value;

                            // If server already in assigned server update its permission,
                            // Else add a new server permission.
                            ServerPermission serverPermission = null;
                            if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                            {
                                serverPermission.updateToHighestPermission(permissionType);
                            }
                            else
                            {
                                string instanceName = reader.GetSqlString(ColInstanceName).Value;
                                bool active = reader.GetSqlBoolean(ColActive).Value;
                                bool deleted = reader.GetSqlBoolean(ColDeleted).Value;
                                serverPermission = new ServerPermission(sqlServerID, instanceName, active, deleted, permissionType);
                                m_ServersDictionaryById.Add(sqlServerID, serverPermission);
                                m_ServersDictionaryByName.Add(instanceName, serverPermission);
                            }

                            // If server is active, add to active servers list.
                            if (serverPermission.Server.Active && serverPermission.PermissionType >= PermissionType.Modify)
                            {
                                m_ActiveServers.Add(serverPermission);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Returns the permission assigned for the server.
        /// </summary>
        /// <param name="sqlServerID"></param>
        /// <returns></returns>
        public PermissionType GetServerPermission(int sqlServerID)
        {
            PermissionType pt = PermissionType.None;

            lock (this)
            {
                // If security is enabled get permission assigned to the server
                // else return admin permission.
                if (IsSecurityEnabled)
                {
                    ServerPermission serverPermission = null;
                    if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                    {
                        pt = serverPermission.PermissionType;
                    }
                }
                else
                {
                    pt = PermissionType.Administrator;
                }
            }

            return pt;
        }

        // SQLDM-28765 - Operator Security Role Changes
        /// <summary>
        /// checks if  the permission assigned for the server is greater than readonlyplus
        /// </summary>
        /// <param name="sqlServerIdList"></param>
        /// <returns></returns>
        public bool IsHasModifyPermission(IList<int> sqlServerIdList)
        {
            bool hasPermisionOverAnyServer = false;

            lock (this)
            {
                if (IsSecurityEnabled)
                {
                    foreach (var sqlServerId in sqlServerIdList)
                    {
                        ServerPermission serverPermission = null;
                        if (m_ServersDictionaryById.TryGetValue(sqlServerId, out serverPermission))
                        {

                            if (serverPermission.PermissionType >= PermissionType.Modify)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    hasPermisionOverAnyServer = true;
                }
            }

            return hasPermisionOverAnyServer;
        }

        /// <summary>
        /// Returns the permission assigned for the server.
        /// </summary>
        /// <param name="sqlServerIdList"></param>
        /// <returns></returns>
        public bool IsHasModifyServerPermission(IList<int> sqlServerIdList)
        {
            bool hasPermisionOverAnyServer = false;

            lock (this)
            {
                if (IsSecurityEnabled)
                {
                    foreach (var sqlServerId in sqlServerIdList)
                    {
                        ServerPermission serverPermission = null;
                        if (m_ServersDictionaryById.TryGetValue(sqlServerId, out serverPermission))
                        {
                            //Operator Security Role Changes - 10.3
                            if (serverPermission.PermissionType >= PermissionType.Modify||serverPermission.PermissionType==PermissionType.ReadOnlyPlus)
                            {
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    hasPermisionOverAnyServer = true;
                }
            }

            return hasPermisionOverAnyServer;
        }

        public PermissionType GetServerPermission(string server)
        {
            PermissionType pt = PermissionType.None;

            lock (this)
            {
                // If security is enabled get permission assigned to the server
                // else return admin permission.
                if (IsSecurityEnabled)
                {
                    ServerPermission serverPermission = null;
                    if (m_ServersDictionaryByName.TryGetValue(server, out serverPermission))
                    {
                        pt = serverPermission.PermissionType;
                    }
                }
                else
                {
                    pt = PermissionType.Administrator;
                }
            }

            return pt;
        }

        /// <summary>
        /// Is the connected user still SQLdm Administrator if the specified
        /// permission is deleted or disabled.
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="permissionID"></param>
        /// <returns></returns>
        public static bool IsUserAdministratorWithoutPermission(
                string connectionString, 
                int permissionID
            )
        {
            // Get credentials information.
            bool isIntegratedSecurity = false;
            string sqlLogin = "";
            byte[] sidByteArray = null;
            getCredentials(connectionString, out isIntegratedSecurity, out sqlLogin, out sidByteArray);

            // Create parameters for user token SP.
            SqlParameter[] arParms = new SqlParameter[4];
            arParms[0] = new SqlParameter("@IsSQLLogin", SqlDbType.Bit);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = !isIntegratedSecurity;

            arParms[1] = new SqlParameter("@SQLLoginName", SqlDbType.NVarChar);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = sqlLogin;

            arParms[2] = new SqlParameter("@WindowsSID", SqlDbType.Binary);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = sidByteArray;

            arParms[3] = new SqlParameter("@PermissionID", SqlDbType.Int);
            arParms[3].Direction = ParameterDirection.Input;
            arParms[3].Value = permissionID;

            // Check if the user has admin permission after deleting/disabling the permission.
            bool isAdmin = false;
            using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, "p_IsUserAdministrator", arParms))
            {
                // Read the security enabled, sysadmin and sqldm admin flags.
                if (reader.Read())
                {
                    isAdmin = reader.IsDBNull(0) ? false : reader.GetSqlBoolean(0).Value;
                }
            }

            return isAdmin;
        }

        /// <summary>
        /// Use this method to add a server to the user token, typically done
        /// when a new server is added in the UI.   The server is added as active
        /// with admin permissions.
        /// </summary>
        /// <param name="sqlServerID"></param>
        /// <param name="instanceName"></param>
        public void AddServer(
                int sqlServerID,
                string instanceName
            )
        {
            lock (this)
            {
                // If security is enabled add server to the dictionary and list.
                if (IsSecurityEnabled)
                {
                    if (!m_ServersDictionaryById.ContainsKey(sqlServerID))
                    {
                        ServerPermission sp = new ServerPermission(sqlServerID, instanceName, true, false, PermissionType.Administrator);
                        m_ServersDictionaryById.Add(sqlServerID, sp);
                        m_ActiveServers.Add(sp);
                    }
                }
            }
        }

        public void DeleteServer(
                int sqlServerID
            )
        {
            lock (this)
            {
                // If security is enabled delete the server from dictionary and list.
                if (IsSecurityEnabled)
                {
                    ServerPermission serverPermission = null;
                    if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                    {
                        m_ActiveServers.Remove(serverPermission);
                        m_ServersDictionaryById.Remove(sqlServerID);
                    }
                }
            }
        }

        public void DeactivateServer(
                int sqlServerID
            )
        {
            lock (this)
            {
                // If security is enabled deactivate the server and remove from active list.
                if (IsSecurityEnabled)
                {
                    ServerPermission serverPermission = null;
                    if (m_ServersDictionaryById.TryGetValue(sqlServerID, out serverPermission))
                    {
                        m_ActiveServers.Remove(serverPermission);
                        serverPermission.Server.Active = false;
                    }
                }
            }
        }

        private static void getCredentials(
                string connectionString, 
                out bool isIntegratedSecurity,
                out string sqlLogin,
                out byte[] sidByteArray
            )
        {
            // Init return.
            isIntegratedSecurity = false;
            sqlLogin = "";
            sidByteArray = new byte[85];

            // Get credential information from connection string if using SQL login
            // get name of login.   Else get the windows user sid byte array of the
            // currently connected user.
            SqlConnectionStringBuilder scsb = new SqlConnectionStringBuilder(connectionString);
            isIntegratedSecurity = scsb.IntegratedSecurity;
            if (isIntegratedSecurity)
            {
                WindowsIdentity.GetCurrent().User.GetBinaryForm(sidByteArray, 0);
                sqlLogin = scsb.UserID;
            }
            else
            {
                sqlLogin = scsb.UserID;
            }
        }

        public static string CreateServerXmlFilter(string instanceName)
        {
            // Create xml doc with servers list.
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("Servers");
            xmlDoc.AppendChild(rootElement);

            XmlElement e = xmlDoc.CreateElement("Server");
            e.SetAttribute("InstanceName", instanceName);
            xmlDoc.FirstChild.AppendChild(e);

            return xmlDoc.InnerXml;
        }

        #endregion
    }

    public class SecurityManagement
    {
        private const int SidBufferSize = 85;

        private static BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger("SecurityManagement");

        public static void EnableSecurity(string connectionString)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "p_AppSecEnable");
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when enabling application security", ex);
                throw (ex);
            }
        }

        public static void DisableSecurity(string connectionString)
        {
            try
            {
                SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, "p_AppSecDisable");
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when disabling application security", ex);
                throw (ex);
            }
        }

        public static Configuration GetSecurityConfiguration(string connectionString)
        {
            Configuration config = new Configuration();
            try
            {
                config.Refresh(connectionString);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when retrieving configuration", ex);
                throw (ex);
            }
            return config;
        }


        public static bool DoesLoginExist(
                string connectionString, 
                string login
            )
        {
            bool ret = false;
            string query = "select sid from master..syslogins where name = '";
            query += login;
            query += "'";
            try
            {
                using (SqlDataReader reader = SqlHelper.ExecuteReader(connectionString, CommandType.Text, query))
                {
                    ret = reader.HasRows;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered when checking if login exists", ex);
                throw ex;
            }
            return ret;
        }

        public static void AddPermission(
                string connnectionString, 
                string sqldmDatabase,
                string login, 
                bool isSqlLogin, 
                string password, 
                PermissionType permission, 
                IEnumerable<int> tags,
                IEnumerable<int> servers,
                string comment,
                bool webAppPermission
            )
        {
            // Create login
            byte[] sid = null;
            createLogin(connnectionString, sqldmDatabase, login, isSqlLogin, password, out sid);
            if (sid == null)
            {
                throw new Exception("Unable to use an existing or create a new login.");
            }

            // Create permission.
            createPermission(connnectionString, sid, permission, tags, servers, comment, webAppPermission);
        }

        private static void createLogin(
                string connnectionString, 
                string sqldmDatabase,
                string login, 
                bool isSqlLogin, 
                string password,
                out byte[] sid
            )
        {
            // If Windows login, capitalize the domain name, else
            // do nothing.
            string caseAdjustedLogin = login;
            if(!isSqlLogin)
            {
                // If no account return.
                if(!string.IsNullOrEmpty(login)) 
                {
                    // Get domain and user based on single whack location.
                    int index = login.IndexOf(@"\");
                    if (index != -1)
                    {
                        string domain = login.Substring(0, index);
                        string user = login.Substring(index + 1, login.Length - index - 1);
                        caseAdjustedLogin = domain.ToUpper() + @"\" + user;
                    }
                    else
                    {
                        caseAdjustedLogin = login;
                    }
                }
            }

            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[5];
            arParms[0] = new SqlParameter("@Login", SqlDbType.NVarChar);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = caseAdjustedLogin;

            arParms[1] = new SqlParameter("@IsSQLLogin", SqlDbType.Bit);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = isSqlLogin;

            arParms[2] = new SqlParameter("@Password", SqlDbType.NVarChar);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = password;

            arParms[3] = new SqlParameter("@Database", SqlDbType.NVarChar);
            arParms[3].Direction = ParameterDirection.Input;
            arParms[3].Value = sqldmDatabase;

            arParms[4] = new SqlParameter("@LoginSID", SqlDbType.VarBinary, SidBufferSize);
            arParms[4].Direction = ParameterDirection.Output;

            // Call SP to create login and get SID of login
            try
            {
                SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_AddLogin", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while creating login", ex);
                throw ex;
            }

            // Get the login SID.
            if (arParms[4].Value is byte[])
            {
                sid = (byte[])arParms[4].Value;
            }
            else
            {
                Log.Error("@LoginSID is not byte[]");
                throw new Exception("LoginSID is not byte[]");
            }
        }

        private static void createPermission(
                string connnectionString,
                byte[] loginSID,
                PermissionType permission,
                IEnumerable<int> tags,
                IEnumerable<int> servers,
                string comment,
                bool webAppPermission
            )
        {
            // Create xml for with tags list
            XmlDocument tagsXmlDoc = new XmlDocument();
            XmlElement rootElement = tagsXmlDoc.CreateElement("Tags");
            tagsXmlDoc.AppendChild(rootElement);
            if (tags != null)
            {
                foreach (int i in tags)
                {
                    XmlElement e = tagsXmlDoc.CreateElement("Tag");
                    e.SetAttribute("TagId", i.ToString());
                    tagsXmlDoc.FirstChild.AppendChild(e);
                }
            }

            // Create xml doc with servers list.
            XmlDocument serversXmlDoc = new XmlDocument();
            rootElement = serversXmlDoc.CreateElement("Servers");
            serversXmlDoc.AppendChild(rootElement);
            if (servers != null)
            {
                foreach (int i in servers)
                {
                    XmlElement e = serversXmlDoc.CreateElement("Server");
                    e.SetAttribute("SQLServerID", i.ToString());
                    serversXmlDoc.FirstChild.AppendChild(e);
                }
            }

            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[6];
            arParms[0] = new SqlParameter("@LoginSID", SqlDbType.VarBinary, SidBufferSize);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = loginSID;

            arParms[1] = new SqlParameter("@Permission", SqlDbType.Int);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = permission;

            arParms[2] = new SqlParameter("@TagsXML", SqlDbType.NText);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = tagsXmlDoc.InnerXml;

            arParms[3] = new SqlParameter("@ServerXML", SqlDbType.NText);
            arParms[3].Direction = ParameterDirection.Input;
            arParms[3].Value = serversXmlDoc.InnerXml;

            arParms[4] = new SqlParameter("@Comment", SqlDbType.NVarChar);
            arParms[4].Direction = ParameterDirection.Input;
            arParms[4].Value = comment;

            arParms[5] = new SqlParameter("@WebAppPermission", SqlDbType.Bit);
            arParms[5].Direction = ParameterDirection.Input;
            arParms[5].Value = webAppPermission;

            // Call SP to create permission
            try
            {
                int test = SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_AddPermission", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while creating permission", ex);
                throw ex;
            }
        }

        public static void EditPermission(
                string connnectionString,
                int permissionID,
                bool enabled,
                PermissionType permission,
                IEnumerable<int> tags,
                IEnumerable<int> servers,
                string comment,
                bool webAppPermission
            )
        {
            // Create xml for with tags list
            XmlDocument tagsXmlDoc = new XmlDocument();
            XmlElement rootElement = tagsXmlDoc.CreateElement("Tags");
            tagsXmlDoc.AppendChild(rootElement);
            if (tags != null)
            {
                foreach (int i in tags)
                {
                    XmlElement e = tagsXmlDoc.CreateElement("Tag");
                    e.SetAttribute("TagId", i.ToString());
                    tagsXmlDoc.FirstChild.AppendChild(e);
                }
            }

            // Create xml doc with servers list.
            XmlDocument serversXmlDoc = new XmlDocument();
            rootElement = serversXmlDoc.CreateElement("Servers");
            serversXmlDoc.AppendChild(rootElement);
            if (servers != null)
            {
                foreach (int i in servers)
                {
                    XmlElement e = serversXmlDoc.CreateElement("Server");
                    e.SetAttribute("SQLServerID", i.ToString());
                    serversXmlDoc.FirstChild.AppendChild(e);
                }
            }

            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[7];
            arParms[0] = new SqlParameter("@PermissionID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = permissionID;

            arParms[1] = new SqlParameter("@Permission", SqlDbType.Int);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = permission;

            arParms[2] = new SqlParameter("@Enabled", SqlDbType.Bit);
            arParms[2].Direction = ParameterDirection.Input;
            arParms[2].Value = enabled;

            arParms[3] = new SqlParameter("@TagsXML", SqlDbType.NText);
            arParms[3].Direction = ParameterDirection.Input;
            arParms[3].Value = tagsXmlDoc.InnerXml;

            arParms[4] = new SqlParameter("@ServerXML", SqlDbType.NText);
            arParms[4].Direction = ParameterDirection.Input;
            arParms[4].Value = serversXmlDoc.InnerXml;

            arParms[5] = new SqlParameter("@Comment", SqlDbType.NVarChar);
            arParms[5].Direction = ParameterDirection.Input;
            arParms[5].Value = comment;

            arParms[6] = new SqlParameter("@WebAppPermission", SqlDbType.Bit);
            arParms[6].Direction = ParameterDirection.Input;
            arParms[6].Value = webAppPermission;

            // Call SP to update permission
            try
            {
                SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_UpdatePermission", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while updating permission", ex);
                throw ex;
            }
        }

        public static void SetPermissionStatus(
                string connnectionString,
                int permissionID,
                bool flag
            )
        {
            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[2];
            arParms[0] = new SqlParameter("@PermissionID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = permissionID;

            arParms[1] = new SqlParameter("@Enabled", SqlDbType.Bit);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = flag;

            // Call SP to update permission status
            try
            {
                SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_UpdatePermissionStatus", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while updating permission status", ex);
                throw ex;
            }
        }

        public static void SetWebAccessStatus(
                string connnectionString,
                int permissionID,
                bool flag
            )
        {
            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[2];
            arParms[0] = new SqlParameter("@PermissionID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = permissionID;

            arParms[1] = new SqlParameter("@WebAppPermission", SqlDbType.Bit);
            arParms[1].Direction = ParameterDirection.Input;
            arParms[1].Value = flag;

            // Call SP to update permission status
            try
            {
                SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_UpdateWebAppPermissionStatus", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while updating web application access status", ex);
                throw ex;
            }
        }



        public static void DeletePermission(
                string connnectionString,
                int permissionID
            )
        {
            // Create SP params.
            SqlParameter[] arParms = new SqlParameter[1];
            arParms[0] = new SqlParameter("@PermissionID", SqlDbType.Int);
            arParms[0].Direction = ParameterDirection.Input;
            arParms[0].Value = permissionID;

            // Call SP to delete permission
            try
            {
                SqlHelper.ExecuteNonQuery(connnectionString, CommandType.StoredProcedure, "p_DeletePermission", arParms);
            }
            catch (Exception ex)
            {
                Log.Error("Exception encountered while deleting permission", ex);
                throw ex;
            }
        }
    }
}

