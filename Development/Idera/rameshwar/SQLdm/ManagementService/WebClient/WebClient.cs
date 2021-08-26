using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Text;
using System.Xml;
using Idera.SQLdm.Common.Auditing;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Data;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.Objects.ApplicationSecurity;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;
using Idera.SQLdm.ManagementService.Configuration;
using System.Data.SqlClient;
using System.Security.Principal;
using System.Data;
using Idera.SQLdm.Common;
using Idera.SQLdm.ManagementService.Helpers;
using Idera.SQLdm.ManagementService.Monitoring;
using Microsoft.ApplicationBlocks.Data;
using System.Data.SqlTypes;
using Idera.Newsfeed.Shared.Interface;
using System.Reflection;
using System.Security.Cryptography;
using Idera.Newsfeed.Shared.Security;
using Idera.SQLdm.ManagementService.Notification;
using Idera.SQLdm.Common.Notification;
using Idera.SQLdm.Common.Notification.Providers;
using Idera.Newsfeed.Shared.DataModel;
using BBS.TracerX;
using System.Threading;
using Wintellect.PowerCollections;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.Common.Objects;

namespace Idera.SQLdm.ManagementService.WebClient
{
    [Serializable]
    public class ServiceMethodAttribute : Attribute
    {
        public readonly string ServiceName;
        public readonly bool RequirePrincipal;

        public ServiceMethodAttribute(string serviceName, bool requirePrincipal)
        {
            ServiceName = serviceName;
            RequirePrincipal = requirePrincipal;
        }

        public ServiceMethodAttribute(string serviceName)
            : this(serviceName, false)
        {
        }
    }

    public class WebClient : MarshalByRefObject, IApplicationExec
    {
        private const string ERROR_NO_SERVICE = "Unable to connect to the {0}.\r\nTo trouble shoot:\r\n1.)Check to make sure the {0} is running.\r\n2.)Confirm that the configured communcation ports (5166 and 5167) are open in the firewall.\r\n3.)Finally, run the Management Service Configuration Wizard to confirm that the service is connected to the proper repository.";

        private static Logger Log;

        private static Dictionary<string, MethodInfo> serviceRegistry;
        private static RSACryptoServiceProvider serviceKey;
        private static PulseNotificationProviderInfo pulseProviderConfig;
        private static Application application;
        private static string applicationName;
        private static object ssync;
        private static Timer syncTimer;
        private static int DoingSynch = 0;
        
        static WebClient()
        {
            Log = Logger.GetLogger("WebClient");

            // create a registry of service names mapped to their implementation
            serviceRegistry = new Dictionary<string,MethodInfo>();
            foreach (MethodInfo method in typeof(WebClient).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                ServiceMethodAttribute sma = Attribute.GetCustomAttribute(method, typeof(ServiceMethodAttribute)) as ServiceMethodAttribute;
                if (sma != null)
                {
                    string name = sma.ServiceName;
                    if (!serviceRegistry.ContainsKey(name))
                        serviceRegistry.Add(name, method);
                }
            }
            serviceKey = new RSACryptoServiceProvider(1024);
            ssync = new object();
            applicationName = String.Format("SQLdm/{0}", Environment.MachineName);
        }

        internal static void Initialize()
        {
            
        }

        internal static Application Application 
        { 
            get { return GetApplication(); } 
        }

        internal static Application GetApplication()
        {
            Application app = null;

            if (application == null)
            {
                RefreshPulseRegistration(null);
            }

            return application;
        }


        internal static IPulseRemotingService GetPulseInterface(PulseNotificationProviderInfo info)
        {
            lock (ssync)
            {
                string url = String.Format("tcp://{0}:{1}/{2}", info.PulseServer, info.PulseServerPort, typeof(IPulseRemotingService).FullName);
                return RemotingHelper.GetObject<IPulseRemotingService>(url);
            }
        }

        /// <summary>
        /// Connect to pulse service and retrieve its version.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        internal static string ValidatePulseConnection(PulseNotificationProviderInfo info)
        {
            IPulseRemotingService pulse = GetPulseInterface(info);
            return pulse.GetVersion();
        }

        internal static void RefreshPulseRegistration(PulseNotificationProviderInfo info)
        {
            if (info == null)
            {
                info = GetPulseProviderInfo();
        
                if (info == null)
                    throw new ApplicationException("Newsfeed notification provider is not configured");
            }

            Application appl = null;
            try
            {
                IPulseRemotingService pulse = GetPulseInterface(info);

                // see if we can get our registration information
                appl = pulse.GetApplication<Application>(applicationName);
                if (appl == null)
                {
                    byte[] image;
                    GetDefaultApplicationConfiguration(out appl, out image);
                    appl = pulse.RegisterApplication(appl, image);
                }
                else
                {
                    // if already registered, use the existing info
                    appl = pulse.RegisterApplication(appl, null);
                }
            } 
            catch (Exception e)
            {
                if (appl == null)
                {
                    info = null;
                }
                throw;
            }

            lock (ssync)
            {
                application = appl;
                pulseProviderConfig = info;
                if (appl != null && info != null)
                    SynchronizeWithPulse();
            }
        }

        internal static int PostStatus(string sender, int senderType, string correlationId, Importance prevImportance, Importance newImportance, string metric, string text, string additionalInfo)
        {
            PulseNotificationProviderInfo info = GetPulseProviderInfo();
            if (info == null || application == null)
                throw new ApplicationException("Newsfeed notification provider is not configured");

            IPulseRemotingService service = GetPulseInterface(info);
           
            return service.AddStatusPost(application.Id, senderType , sender, correlationId, prevImportance, newImportance, metric, text, additionalInfo);
        }

        internal static PulseNotificationProviderInfo GetPulseProviderInfo()
        {
            PulseNotificationProviderInfo pnpi = null;
            foreach (NotificationProviderInfo npi in Management.Notification.GetNotificationProviders())
            {
                if (npi is PulseNotificationProviderInfo)
                {
                    pnpi = (PulseNotificationProviderInfo)npi;
                    break;
                }
            }
            return pnpi;
        }

        internal static List<User> GetUsers(PulseNotificationProviderInfo info)
        {
            IPulseRemotingService pulse = GetPulseInterface(info);
            return null;
        }

        internal static Application GetApplication(PulseNotificationProviderInfo info)
        {
            IPulseRemotingService pulse = GetPulseInterface(info);
            return pulse.GetApplication<Application>(applicationName);
        }

        //internal static void UpdateApplication(Application newApplication)
        //{
        //    UpdateDirectoryObject(newApplication);


        //    Application app = Application;
        //    if (app != null)
        //    {
        //        app.Name = newApplication.Name;
        //        app.ShortName = newApplication.ShortName;
        //        app.OwnerId = newApplication.OwnerId;
        //        app.Alias = newApplication.Alias;
        //    }
        //    else
        //    {
        //        application = newApplication;
        //    }
        //}

        //internal static void UpdateDirectoryObject(IDirectoryObject dirObject)
        //{
        //    IPulseRemotingService pulse = GetPulseInterface(GetPulseProviderInfo());    
        //    pulse.UpdateObject(dirObject);
        //}

        /// <summary>
        /// Register with the News Feed service and get our News Feed application id.
        /// </summary>
        internal static void GetDefaultApplicationConfiguration(out Application app, out byte[] image)
        {
            app = new Application();
            //SQLDM-30453.
            app.ApplicationType = "SQL diagnostic manager";
            app.EndPoint = new Uri(String.Format(@"tcp://{0}:{1}",
                                        Environment.MachineName,
                                        ManagementServiceConfiguration.ServicePort));

            app.Name = applicationName;
            //app.Alias = String.Format("SQLdm on {0}", Environment.MachineName);
            app.Alias = "SQL Diagnostic Manager";

            Assembly entry = Assembly.GetEntryAssembly();
            using (System.IO.Stream icostream = entry.GetManifestResourceStream("Idera.SQLdm.ManagementService.Resources.SQLdmLogo64.png"))
            {
                image = new byte[icostream.Length];
                icostream.Read(image, 0, image.Length);
            }
        }

        public object Execute(IIdentity identity, string service, params object[] args)
        {
            try
            {
                MethodInfo method;
                if (serviceRegistry.TryGetValue(service, out method))
                {
                    object[] parms = args;

                    ServiceMethodAttribute sma =
                        (ServiceMethodAttribute) Attribute.GetCustomAttribute(method, typeof (ServiceMethodAttribute));
                    if (sma != null && sma.RequirePrincipal)
                    {
                        int parmc = (args == null) ? 0 : args.Length;
                        parms = new object[parmc + 1];
                        parms[0] = identity;
                        if (parmc > 0)
                            Array.Copy(args, 0, parms, 1, parmc);
                    }
                    if (parms != null && parms.Length == 0)
                        parms = null;

                    return method.Invoke(this, parms);
                }
                
                throw new PulseApplicationException(PulseApplicationExceptionType.RequestedServiceNotFound, 
                                                    "WebClient - requested service not found: " + service);
            } 
            catch (Exception e)
            {
                Log.ErrorFormat("Error excuting service '{0}': {1}", service, e.ToString());

                Exception root = GetRootException(e);
                if (root != null)
                {
                    root = WrapException(root);
                    throw root;
                }

                throw new ApplicationException(e.Message);
            }
        }

        private Exception WrapException(Exception root)
        {
            PulseApplicationExceptionType type = PulseApplicationExceptionType.Unknown;
            string messageText = String.Empty;

            if (root is Idera.SQLdm.Common.Services.ServiceCallProxy.ServiceCallException)
            {
                root = root.InnerException;
                type = PulseApplicationExceptionType.RemotingException;
                messageText = root.Message;
            }

            if (root is RemotingException)
            {
                type = PulseApplicationExceptionType.RemotingException;
                if (String.IsNullOrEmpty(messageText))
                    messageText = String.Format(ERROR_NO_SERVICE, "SQLDM Management Service");
            }
            else
            if (root is Win32Exception)
            {
                if (((Win32Exception)root).NativeErrorCode == 10061)
                    type = PulseApplicationExceptionType.RemotingException;
                if (String.IsNullOrEmpty(messageText))
                    messageText = String.Format(ERROR_NO_SERVICE, "SQLDM Management Service");
            }
            else
            if (root is ManagementServiceException)
            {
                type = PulseApplicationExceptionType.ManagementServiceException;
                if (String.IsNullOrEmpty(messageText))
                    messageText = "The SQLDM Management Service encountered an exception processing your request.";
            }
            else
            if (root is CollectionServiceException)
            {
                type = PulseApplicationExceptionType.CollectionServiceException;
                if (String.IsNullOrEmpty(messageText))
                    messageText = "The SQLDM Collection Service encountered an exception processing your request.";
            }
            else
            if (root != null && !root.GetType().IsSubclassOf(typeof(Exception)))
            {   // Collection Service probes can throw unsubclassed Exception objects
                type = PulseApplicationExceptionType.CollectionServiceException;
                messageText = root.Message;
                root = null;
            }

            Exception innerException = SanitizeException(root);
            if (String.IsNullOrEmpty(messageText))
                messageText = "The SQLDM Management Service encountered an exception processing your request.";

            throw new PulseApplicationException(type, messageText, innerException);
        }

        internal Exception GetRootException(Exception e)
        {
            Exception root = e;
            for (Exception x = e; x != null; x = x.InnerException)
            {
                if (x is TargetInvocationException)
                    continue;

                return x;
            }

            return root;
        }

        internal Exception SanitizeException(Exception e)
        {
            List<Exception> elist = new List<Exception>();
            for (Exception x = e; x != null; x = x.InnerException)
            {
                elist.Add(x);
            }
            elist.Reverse();

            ApplicationException root = null;
            foreach (Exception x in elist)
            {
                if (x is TargetInvocationException)
                    continue;

                if (x.GetType().FullName.StartsWith("System."))
                    return x;

                root = new ApplicationException(x.Message, root);
            }

            return root;
        }

        #region Discovery

        [ServiceMethod("get-services")]
        internal object GetServices()
        {
            List<string> result = new List<string>();
            foreach (string verb in serviceRegistry.Keys)
                result.Add(verb);
            return result;
        }

        //[ServiceMethod("get-applicationid")]
        //internal int? GetApplicationId()
        //{
        //    lock(ssync)
        //    {

        //    }
        //    return applicationId;
        //}

        #endregion

        #region Authenticate User

        [ServiceMethod("get-publickey")]
        internal string GetPublicKey()
        {
            return serviceKey.ToXmlString(false);
        }

        [ServiceMethod("authenticate-user")]
        internal SQLdmPrincipal AuthenticateSQLdmUser(string user, byte[] encpwd, bool windowsAuthentication)
        {
            byte[] decrypted = serviceKey.Decrypt(encpwd, true);
            string password = UnicodeEncoding.Default.GetString(decrypted);

            SQLdmIdentity identity = null;
            if (windowsAuthentication)
                identity = AuthenticateUsingWindowsCredentials(user, password);
            else
                identity = AuthenticateUsingSQLCredentials(user, password);

            return identity != null ? new SQLdmPrincipal(identity) : null;
        }

        private SQLdmIdentity AuthenticateUsingSQLCredentials(string user, string password)
        {
            SqlConnectionStringBuilder csbuilder = new SqlConnectionStringBuilder(ManagementServiceConfiguration.ConnectionString);
            csbuilder.IntegratedSecurity = false;
            csbuilder.UserID = user;
            csbuilder.Password = password;
            csbuilder.PersistSecurityInfo = false;
            csbuilder.Pooling = false;

            try
            {
                using (SqlConnection connection = new SqlConnection(csbuilder.ToString()))
                {
                    return ValidateConnection(connection, user, SQLdmAuthType.Sql);
                }
            }
            catch (Exception e)
            {
                // LOG
                throw;
            }
            return null;
        }

        private SQLdmIdentity AuthenticateUsingWindowsCredentials(string user, string password)
        {
            Exception innerException = null;
            SQLdmIdentity identity = null;

            string domain = null;
            string userid;

            string[] parts = user.Split('\\');
            switch (parts.Length)
            {
                case 0:
                case 1:
                    userid = user;
                    break;
                default:
                    domain = parts[0];
                    userid = parts[1];
                    for (int i = 2; i < parts.Length; i++)
                    {
                        userid = userid + "\\" + parts[i];
                    }
                    break;
            }

            //using (Impersonation ictx = new Impersonation(user, password))
            //{
            //    SqlConnectionStringBuilder csbuilder = new SqlConnectionStringBuilder(ManagementServiceConfiguration.ConnectionString);
            //    if (!csbuilder.IntegratedSecurity)
            //    {
            //        csbuilder.IntegratedSecurity = true;
            //        csbuilder.UserID = null;
            //        csbuilder.Password = null;
            //    }
            //    csbuilder.PersistSecurityInfo = false;
            //    csbuilder.Pooling = false;

            //    try
            //    {
            //        using (SqlConnection connection = new SqlConnection(csbuilder.ToString()))
            //        {
            //            identity = ValidateConnection(connection, csbuilder.UserID, SQLdmAuthType.Windows);
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        innerException = e;
            //    }
            //}

            using (ImpersonationContext ctx = new ImpersonationContext(domain, userid, password))
            {
                ctx.LogonUser();

                ctx.RunAs(delegate()
                {
                    SqlConnectionStringBuilder csbuilder = new SqlConnectionStringBuilder(ManagementServiceConfiguration.ConnectionString);
                    if (!csbuilder.IntegratedSecurity)
                    {
                        csbuilder.IntegratedSecurity = true;
                        //csbuilder.UserID = null;
                        //csbuilder.Password = null;
                    }
                    csbuilder.PersistSecurityInfo = false;
                    csbuilder.Pooling = false;

                    try
                    {
                        using (SqlConnection connection = new SqlConnection(csbuilder.ToString()))
                        {
                            identity = ValidateConnection(connection, userid, SQLdmAuthType.Windows);
                        }
                    }
                    catch (Exception e)
                    {
                        innerException = e;
                    }
                });
            }

            if (innerException != null)
                throw innerException;

            return identity;
        }

        [ServiceMethod("validate-user")]
        internal IPrincipal ValidateUser(string userName)
        {
            SqlConnectionStringBuilder csbuilder = new SqlConnectionStringBuilder(ManagementServiceConfiguration.ConnectionString);
            csbuilder.PersistSecurityInfo = false;
            csbuilder.Pooling = false;

            SQLdmAuthType authType = userName.Contains("\\") ? SQLdmAuthType.Windows : SQLdmAuthType.Sql;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("declare @sid varbinary(80)");
            sb.AppendFormat("select @sid = SUSER_SID('{0}', 0)", userName).AppendLine();
            sb.AppendLine("if @sid is not null");
            sb.AppendLine("begin");
            sb.AppendLine("declare @sql nvarchar(4000)");
            sb.AppendLine("declare @name nvarchar(256)");
            sb.AppendLine("select @name = SUSER_SNAME(@sid)");
            sb.AppendLine("select @sql = 'exec (''p_GetUserIdentifiers'') as LOGIN = ''' + @name + ''''");
            sb.AppendLine("execute sp_executesql @sql");
            sb.AppendLine("end");
            
            SQLdmIdentity id = null;
            try
            {
               
                using (SqlConnection connection = new SqlConnection(csbuilder.ToString()))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = sb.ToString();
                        cmd.CommandType = CommandType.Text;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {

                            id = new SQLdmIdentity(userName, authType, reader);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Log.WarnFormat("Exception trying to validate user ({0}) using sql impersonation", userName);
                throw;
            }
            return id != null ? RefreshIdentity(new SQLdmPrincipal(id)) : null;
        }

        private SQLdmIdentity ValidateConnection(SqlConnection connection, string name, SQLdmAuthType authType)
        {
            try
            {
                connection.Open();
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "p_GetUserIdentifiers";
                    cmd.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        return new SQLdmIdentity(name, authType, reader);
                    }
                }
            }
            catch (Exception e)
            {
                // LOG
                throw;
            }
        }

        /// <summary>
        /// Return list of security identifiers for the given user using S4U.
        /// May not work when not running as a service or if the computer or service user
        /// is not in the trust realm for the users domain.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private SecurityIdentifier[] S4UGetAuthInfo(string user)
        {
            int i = user.IndexOf('\\');
            if (i > 0)
            {   // windows requires the user to be in name@domain format
                user = user.Substring(i + 1) + "@" + user.Substring(0, i);
            }

            List<SecurityIdentifier> ids = new List<SecurityIdentifier>();
            WindowsIdentity identity = new WindowsIdentity(user);
            ids.Add(identity.User);
            foreach (SecurityIdentifier id in identity.Groups)
                ids.Add(id);
            return ids.ToArray();
        }

        private SQLdmPrincipal RefreshIdentity(SQLdmPrincipal principal)
        {
            if (!principal.Identity.IsAuthenticated)
                return principal;
            if (principal.Identity.AuthenticationType != SQLdmAuthType.Windows.ToString())
                return principal;

            // try to use S4U to update the sids in the identity and return a new identity/principal
            try
            {
                SecurityIdentifier[] sids = S4UGetAuthInfo(principal.Identity.Name);
                SQLdmIdentity identity = new SQLdmIdentity((SQLdmIdentity)principal.Identity, sids);
                return new SQLdmPrincipal(identity);
            }
            catch (Exception e)
            {
                
            }

            return principal;
        }

        #endregion

        #region GetServers

        [ServiceMethod("get-servers", true)]
        internal DataTable GetAuthorizedServers(SQLdmIdentity identity)
        {
            SQLdmPrincipal current = RefreshIdentity(new SQLdmPrincipal(identity));

            string xml = current.GetRoleXml();

            return GetTable(ManagementServiceConfiguration.ConnectionString, "p_GetAuthorizedServers", xml);
        }

        #endregion

        #region DataTable Helpers
        public static DataTable GetTable(string connectionString, string spName,
                                 params object[] parameterValues)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionInfo");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlDataReader dataReader = SqlHelper.ExecuteReader(connection, spName, parameterValues))
                {
                    return GetTable(dataReader);
                }
            }
        }

        internal static DataTable GetTable(SqlDataReader dataReader)
        {
            return GetTable(dataReader, true);
        }

        internal static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime)
        {
            return GetTable(dataReader, convertDatesToLocalTime, null);
        }

        internal static DataTable GetTable(SqlDataReader dataReader, bool convertDatesToLocalTime, bool? isColumnReadOnly)
        {
            if (dataReader == null)
            {
                return null;
            }

            List<int> dateColumns = new List<int>();
            DataTable schemaTable = dataReader.GetSchemaTable();
            DataTable dataTable = new DataTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                if (!dataTable.Columns.Contains(schemaTable.Rows[i]["ColumnName"].ToString()))
                {
                    DataColumn dataColumn = new DataColumn();
                    dataColumn.ColumnName = schemaTable.Rows[i]["ColumnName"].ToString();
                    dataColumn.Unique = Convert.ToBoolean(schemaTable.Rows[i]["IsUnique"]);
                    dataColumn.AllowDBNull = Convert.ToBoolean(schemaTable.Rows[i]["AllowDBNull"]);
                    dataColumn.ReadOnly = isColumnReadOnly.HasValue
                                              ? isColumnReadOnly.Value
                                              : Convert.ToBoolean(schemaTable.Rows[i]["IsReadOnly"]);
                    dataColumn.DataType = schemaTable.Rows[i]["DataType"] as Type;
                    dataTable.Columns.Add(dataColumn);

                    if (convertDatesToLocalTime && dataColumn.DataType == typeof(DateTime))
                        dateColumns.Add(i);
                }
            }

            object[] itemArray = new object[dataReader.FieldCount];

            dataTable.BeginLoadData();
            while (dataReader.Read())
            {
                try
                {
                    dataReader.GetValues(itemArray);
                }
                catch (OverflowException oe)
                {
                    SafeLoadRow(dataReader, itemArray, dataTable);
                }
                if (dateColumns.Count > 0)
                {
                    foreach (int columnIndex in dateColumns)
                    {
                        if (itemArray[columnIndex] != DBNull.Value)
                        {
                            itemArray[columnIndex] = ((DateTime)itemArray[columnIndex]).ToLocalTime();
                        }
                    }
                }

                dataTable.LoadDataRow(itemArray, true);
            }
            dataTable.EndLoadData();

            return dataTable;
        }

        public static void SafeLoadRow(SqlDataReader reader, object[] itemArray, DataTable dataTable)
        {

            int n = itemArray.Length;
            for (int i = 0; i < n; i++)
            {
                try
                {
                    if (dataTable.Columns[i].DataType == typeof(decimal))
                    {
                        SqlDecimal sqlDecimal = reader.GetSqlDecimal(i);
                        itemArray[i] = sqlDecimal.IsNull ? (object)DBNull.Value : Convert.ToDecimal(sqlDecimal.ToDouble());
                    }
                    else
                        itemArray[i] = reader.GetValue(i);
                }
                catch (OverflowException e)
                {
                    itemArray[i] = DBNull.Value;
                }
            }
        }

        #endregion

        [ServiceMethod("get-applicationid")]
        internal int GetPulseApplicationId()
        {
            if (Application == null)
                throw new ApplicationException("SQL Diagnostic Manager is not configured for the IDERA Newsfeed");
            return WebClient.Application.Id;
        }

        [ServiceMethod("get-pulsename")]
        internal string GetPulseServerName()
        {
            PulseNotificationProviderInfo info = GetPulseProviderInfo();
            if (info == null)
                return null;

            return info.PulseServer;
        }

        [ServiceMethod("ping")]
        internal string Ping()
        {
            return "Pong";
        }
        
        [ServiceMethod("get-sessionmetrics")]
        internal DataTable GetSessionMetrics(string instanceName, int historyInMinutes)
        {
            int sqlServerId = GetInstanceId(instanceName);
            Sessions helper = new Sessions();
            return helper.GetSessionMetrics(sqlServerId, TimeSpan.FromMinutes(historyInMinutes));
        }

        [ServiceMethod("get-sessionsummary")]
        internal DataTable GetSessionSummary(string instanceName, string searchTerm)
        {
            int sqlServerId = GetInstanceId(instanceName);
            SessionSummaryConfiguration configuration = new SessionSummaryConfiguration(sqlServerId);
            configuration.SearchTerm = searchTerm;
            Sessions helper = new Sessions();
            return helper.GetSessionSummary(configuration);
        }

        [ServiceMethod("get-sessions")]
        internal DataTable GetSessions(string instanceName, string searchTerm, bool activeOnly, bool blockingOnly, bool blockedOnly, bool topCpu, bool topIo, bool topMemory, bool topWait)
        {
            int sqlServerId = GetInstanceId(instanceName);
            SessionsConfiguration configuration = new SessionsConfiguration(sqlServerId, false, false, activeOnly, blockedOnly, false, blockingOnly, false, false, false, false, null, string.Empty, string.Empty, string.Empty, string.Empty);
            if (topCpu)
            {
                configuration.TopCpu = true;
                configuration.TopLimit = 20;
            }
            else if (topIo)
            {
                configuration.TopIo = true;
                configuration.TopLimit = 20;
            }
            else if (topMemory)
            {
                configuration.TopMemory = true;
                configuration.TopLimit = 20;
            }
            else if (topWait)
            {
                configuration.TopWait = true;
                configuration.TopLimit = 20;
            }

            configuration.ExcludeSystemProcesses = activeOnly;

            configuration.InputBufferLimiter = 250;
            configuration.SearchTerm = searchTerm;
            Sessions helper = new Sessions();
            return helper.GetSessions(configuration, false);
        }

        [ServiceMethod("get-sessiondetails")]
        internal DataTable GetSessionDetails(string instanceName, int spid)
        {
            int sqlServerId = GetInstanceId(instanceName);
            SessionsConfiguration configuration = new SessionsConfiguration(sqlServerId, false, false, false, false, false, false, false, false, false, false, null, string.Empty, string.Empty, string.Empty, string.Empty);
            configuration.Spid = spid;
            configuration.InputBufferLimiter = 250;
            Sessions helper = new Sessions();
            return helper.GetSessions(configuration, false);
        }

        [ServiceMethod("get-jobs")]
        internal DataTable GetJobs(string instanceName, bool showRunningOnly, bool showFailedOnly)
        {
            int sqlServerId = GetInstanceId(instanceName);
            Jobs helper = new Jobs();

            AgentJobSummaryConfiguration.JobSummaryFilterType filter = AgentJobSummaryConfiguration.JobSummaryFilterType.All;
            if (showFailedOnly) filter = AgentJobSummaryConfiguration.JobSummaryFilterType.Failed;
            if (showRunningOnly) filter = AgentJobSummaryConfiguration.JobSummaryFilterType.Running;

            return helper.GetJobs(sqlServerId, filter, null);
        }

        [ServiceMethod("get-jobhistory")]
        internal DataTable GetJobHistory(string instanceName, Guid jobId, bool failedOnly, int maxRows)
        {
            int sqlServerId = GetInstanceId(instanceName);
            Jobs helper = new Jobs();
            return helper.GetJobHistory(sqlServerId, jobId, failedOnly, maxRows);
        }

        [ServiceMethod("get-databases")]
        internal DataTable GetDatbases(string instanceName, bool includeSystemDatabases)
        {
            int sqlServerId = GetInstanceId(instanceName);
            Databases helper = new Databases();
            return helper.GetDatbases(sqlServerId, includeSystemDatabases);
        }

        [ServiceMethod("get-databasefileinfo")]
        internal DataTable GetDatbaseFileInfo(string instanceName, string databaseName)
        {
            int sqlServerId = GetInstanceId(instanceName);
            Databases helper = new Databases();
            return helper.GetDatbaseFileInfo(sqlServerId, databaseName);
        }

        [ServiceMethod("get-serverstatus")]
        internal DataSet GetServerStatus()
        {
            DataSet result = new DataSet("SQLdm Server Status");

            DataTable assets = result.Tables.Add("Assets");
            DataColumn parent = assets.Columns.Add("SQLServerID", typeof (int));
            assets.Columns.Add("InstanceName", typeof (string));
            assets.Columns.Add("ServerVersion", typeof (string));
            assets.Columns.Add("ServerEdition", typeof (string));
            assets.Columns.Add("ActiveWarningAlerts", typeof (int));
            assets.Columns.Add("ActiveCriticalAlerts", typeof (int));
            assets.Columns.Add("MaintenanceModeEnabled", typeof (bool));
            assets.Columns.Add("LastScheduledCollectionTime", typeof (DateTime));

            DataTable alerts = result.Tables.Add("Alerts");
            DataColumn child = alerts.Columns.Add("SQLServerID", typeof (int));
            alerts.Columns.Add("Time", typeof (DateTime));
            alerts.Columns.Add("Metric", typeof (int));
            alerts.Columns.Add("Severity", typeof (int));
            alerts.Columns.Add("Rank", typeof (int));
            alerts.Columns.Add("Subject", typeof (string));
            alerts.Columns.Add("Category", typeof (string));

            // Create DataRelation.
            DataRelation relation = new DataRelation("Assets-Alerts", parent, child);
            // Add the relation to the DataSet.
            result.Relations.Add(relation);

            result.RemotingFormat = SerializationFormat.Binary;
            assets.RemotingFormat = SerializationFormat.Binary;
            alerts.RemotingFormat = SerializationFormat.Binary;

            string statusDocument = Management.ScheduledCollection.MonitoredSQLServerStatusDocument;
            XmlDocument document = new XmlDocument();
            document.LoadXml(statusDocument);

            int i;
            DateTime d;

            // serverStatus has same schema as statusDocument
            foreach (XmlNode serverNode in document.DocumentElement.SelectNodes("/Servers/Server"))
            {
                DataRow asset = assets.NewRow();
                foreach (DataColumn column in assets.Columns)
                {
                    XmlAttribute attribute = serverNode.Attributes[column.ColumnName];
                    if (attribute == null)
                        continue;

                    Type columnType = column.DataType;
                    if (columnType == typeof (string))
                    {
                        asset[column] = attribute.Value;
                        continue;
                    }
                    if (columnType == typeof (int))
                    {
                        if (Int32.TryParse(attribute.Value, out i))
                            asset[column] = i;
                        continue;
                    }
                    if (columnType == typeof (bool))
                    {
                        asset[column] = ("1".Equals(attribute.Value));
                        continue;
                    }
                    if (columnType == typeof (DateTime))
                    {
                        string timeString = attribute.Value;
                        if (!String.IsNullOrEmpty(timeString) && DateTime.TryParse(timeString, out d))
                            asset[column] = d;
                        continue;
                    }
                }

                if (asset["InstanceName"] == null || asset["InstanceName"] == DBNull.Value)
                    continue;

                assets.Rows.Add(asset);

                OrderedSet<Issue> topIssues = new OrderedSet<Issue>();

                foreach (XmlNode category in serverNode.ChildNodes)
                {
                    if (!category.HasChildNodes || category.Attributes == null)
                        continue;

                    string catname = category.Attributes["Name"].Value;

                    foreach (XmlNode statusNode in category.ChildNodes)
                    {
                        if (!statusNode.Name.Equals("State"))
                            continue;

                        MonitoredState issueSeverity = MonitoredState.None;
                        Metric metric = Metric.ProductVersion;
                        int rank = 0;
                        string subject = null;
                        DateTime occurenceTime = default(DateTime);
                        foreach (XmlAttribute attribute in statusNode.Attributes)
                        {
                            switch (attribute.Name)
                            {
                                case "Rank":
                                    Int32.TryParse(attribute.Value, out rank);
                                    break;
                                case "Severity":
                                    byte sev = 0;
                                    if (Byte.TryParse(attribute.Value, out sev))
                                    {
                                        issueSeverity = (MonitoredState) Enum.ToObject(typeof (MonitoredState), sev);
                                    }
                                    break;
                                case "Metric":
                                    int imetric = 0;
                                    if (Int32.TryParse(attribute.Value, out imetric))
                                    {
                                        metric = (Metric) Enum.ToObject(typeof (Metric), imetric);
                                    }
                                    break;
                                case "Subject":
                                    subject = attribute.Value;
                                    break;
                                case "OccurenceTime":
                                    string timeString = attribute.Value;
                                    if (String.IsNullOrEmpty(timeString))
                                        occurenceTime = default(DateTime);
                                    else
                                    {
                                        try
                                        {
                                            occurenceTime = DateTime.Parse(timeString).ToLocalTime();
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    break;
                            }
                        }

                        Issue issue = new Issue(metric, issueSeverity, rank, catname, subject, occurenceTime);
                        topIssues.Add(issue);
                    }
                }

                for (int x = 0; x < 3 && x < topIssues.Count; x++)
                {
                    Issue issue = topIssues[x];
                    DataRow alert = alerts.NewRow();

                    alert["SQLServerID"] = asset["SQLServerID"];
                    alert["Time"] = issue.OccurenceTime;
                    alert["Metric"] = (int) issue.Metric;
                    alert["Severity"] = (int) issue.Severity;
                    alert["Rank"] = issue.Rank;
                    alert["Category"] = issue.Category;
                    alert["Subject"] = issue.Subject;
                    alerts.Rows.Add(alert);
                }
            }
            return result;
        }

        internal static int GetInstanceId(string instanceName)
        {
            MonitoredSqlServerState state = Management.ScheduledCollection.GetCachedMonitoredSqlServer(instanceName, StringComparison.CurrentCultureIgnoreCase);
            if (state != null)
                return state.WrappedServer.Id;
            
            return RepositoryHelper.GetMonitoredSqlServer(ManagementServiceConfiguration.ConnectionString, instanceName);
        }

        [ServiceMethod("get-serveroverview")]
        internal DataSet GetServerOverview(String instanceName, DataTable configTable)
        {
            int sqlServerId = 0;
            ServerOverviewConfiguration configuration;

            if (configTable == null || configTable.Columns.Count == 0)
            {
                configTable = new DataTable("Config");
                configTable.Columns.Add("SQLServerID", typeof(int));
                configTable.Columns.Add("clientSessionID", typeof(Guid));
                configTable.Columns.Add("lastRefresh", typeof(DateTime));
                configTable.Columns.Add("serverStartupTime", typeof(DateTime));
                configTable.Columns.Add("previousOSMetrics", typeof(byte[]));
                configTable.Columns.Add("previousServerStatistics", typeof(byte[]));
                configTable.Columns.Add("previousLockStatistics", typeof(byte[]));
                configTable.Columns.Add("previousDiskDrives", typeof(byte[]));
                configTable.Columns.Add("previousDbStatistics", typeof(byte[]));
                configTable.RemotingFormat = SerializationFormat.Binary;
            }
            
            if (configTable.Rows.Count == 0)
            {
                sqlServerId = GetInstanceId(instanceName);
                configuration = new ServerOverviewConfiguration(sqlServerId);
            }
            else
            {
                configuration = GetConfiguration(configTable);
                sqlServerId = configuration.MonitoredServerId;
            }

            ServerOverview overview = null;

            List<ServerOverview> history = LoadChartHistory(sqlServerId, null, null);
            if (history != null && history.Count > 0)
            {
                overview = history[0];
            }
            else
            {   
                // only do this if we can't get the overview from the database
                ManagementService service = new ManagementService();
                if (configuration.ServerStartupTime == null)
                {
                    ServerOverview first = service.GetServerOverview(configuration);
                    configuration = new ServerOverviewConfiguration(sqlServerId, first);
                }
                // get the realtime snapshot
                overview = service.GetServerOverview(configuration);
            }

            DataSet result = ServerDetails.RollupServerStatus(sqlServerId, overview);
             
            ServerOverviewConfiguration nextConfig = new ServerOverviewConfiguration(sqlServerId, overview);
            configTable = configTable.Clone();
            result.Tables.Add(configTable);
            DataRow newConfigRow = configTable.NewRow();
            newConfigRow["SQLServerID"] = sqlServerId;
            newConfigRow["clientSessionID"] = nextConfig.ClientSessionId;
            newConfigRow["lastRefresh"] = nextConfig.LastRefresh.HasValue ? (object)nextConfig.LastRefresh : DBNull.Value;
            newConfigRow["serverStartupTime"] = nextConfig.ServerStartupTime.HasValue ? (object)nextConfig.ServerStartupTime : DBNull.Value;
            newConfigRow["previousOSMetrics"] = GetSerialized(nextConfig.PreviousOSMetrics);
            newConfigRow["previousServerStatistics"] = GetSerialized(nextConfig.PreviousServerStatistics);
            newConfigRow["previousLockStatistics"] = GetSerialized(nextConfig.PreviousLockStatistics);
            newConfigRow["previousDiskDrives"] = GetSerialized(nextConfig.PreviousDiskDrives);
            newConfigRow["previousDbStatistics"] = GetSerialized(nextConfig.PreviousDbStatistics);
            configTable.Rows.Add(newConfigRow);

            return result;
        }

        private ServerOverviewConfiguration GetConfiguration(DataTable configTable)
        {
            DataRow configRow = configTable.Rows[0];
            int sqlServerId = (int)configRow["SQLServerID"];
            ServerOverviewConfiguration configuration = new ServerOverviewConfiguration(sqlServerId);

            object value = configRow["clientSessionID"];
            if (HasValue(value))
                configuration.ClientSessionId = (Guid)value;

            value = configRow["lastRefresh"];
            if (HasValue(value))
                configuration.LastRefresh = (DateTime)value;

            value = configRow["serverStartupTime"];
            if (HasValue(value))
                configuration.ServerStartupTime = (DateTime)value;

            configuration.PreviousOSMetrics = GetValue<OSMetrics>(configRow["previousOSMetrics"]);
            configuration.PreviousServerStatistics = GetValue<ServerStatistics>(configRow["previousServerStatistics"]);
            configuration.PreviousLockStatistics = GetValue<LockStatistics>(configRow["previousLockStatistics"]);
            configuration.PreviousDiskDrives = GetValue<Dictionary<string, DiskDrive>>(configRow["previousDiskDrives"]);
            configuration.PreviousDbStatistics = GetValue<Dictionary<string, DatabaseStatistics>>(configRow["previousDbStatistics"]);
            
            return configuration;
        }

        [ServiceMethod("get-metrichistory")]
        internal DataSet GetMetricHistory(String instanceName, TimeSpan historyPeriod, DataTable overviewConfig)
        {
            ServerOverview newest = null;
            int instanceId = GetInstanceId(instanceName);

            IList<MetricThresholdEntry> thresholdList = RepositoryHelper.GetMetricThresholds(ManagementServiceConfiguration.ConnectionString, instanceId);
            Dictionary<Pair<int, string>, MetricThresholdEntry> thresholds = new Dictionary<Pair<int, string>, MetricThresholdEntry>();
            foreach (MetricThresholdEntry entry in thresholdList)
            {
                Pair<int, string> pair = new Pair<int, string>(entry.MetricID, entry.MetricInstanceName);
                thresholds.Add(pair, entry);
            }

            DataSet result = new DataSet("MetricHistory");
            result.RemotingFormat = SerializationFormat.Binary;

            DataTable details = ServerDetails.CreateServerDetailsTable();
            details.RemotingFormat = SerializationFormat.Binary;

            result.Tables.Add(details);

            // always include the config table
            DataTable configTable = new DataTable("Config");
            configTable.Columns.Add("SQLServerID", typeof(int));
            configTable.Columns.Add("clientSessionID", typeof(Guid));
            configTable.Columns.Add("lastRefresh", typeof(DateTime));
            configTable.Columns.Add("serverStartupTime", typeof(DateTime));
            configTable.Columns.Add("previousOSMetrics", typeof(byte[]));
            configTable.Columns.Add("previousServerStatistics", typeof(byte[]));
            configTable.Columns.Add("previousLockStatistics", typeof(byte[]));
            configTable.Columns.Add("previousDiskDrives", typeof(byte[]));
            configTable.Columns.Add("previousDbStatistics", typeof(byte[]));
            configTable.RemotingFormat = SerializationFormat.Binary;
            result.Tables.Add(configTable);

            if (historyPeriod.TotalSeconds > 0)
            {
                ServerDetails.GetMetricGraphData(instanceId, historyPeriod, details, thresholds, out newest);
                ServerDetails.GetDatabaseGraphData(instanceId, historyPeriod, details, thresholds);
            }
            
            if (overviewConfig != null)
            {
                ManagementService service = new ManagementService();

                ServerOverviewConfiguration configuration = null;
                if (overviewConfig.Rows.Count > 0)
                    configuration = GetConfiguration(overviewConfig);
                else
                {
                    if (newest != null)
                        configuration = new ServerOverviewConfiguration(instanceId, newest);
                    else
                    {
                        configuration = new ServerOverviewConfiguration(instanceId);
                        newest = service.GetServerOverview(configuration);
                        configuration = new ServerOverviewConfiguration(instanceId, newest);
                    }
                }

                // get the realtime snapshot
                newest = service.GetServerOverview(configuration);
                if (newest != null)
                {
                    // calc the waits summary first
                    newest.CalculateWaitStatisticsSummary(ManagementService.GetWaitTypes());

                    ServerDetails.SetMetricGraphRows(newest, details, thresholds);
                }
            }

            if (newest != null)
            {

                ServerOverviewConfiguration nextConfig = new ServerOverviewConfiguration(instanceId, newest);
                DataRow newConfigRow = configTable.NewRow();
                newConfigRow["SQLServerID"] = instanceId;
                newConfigRow["clientSessionID"] = nextConfig.ClientSessionId;
                newConfigRow["lastRefresh"] = nextConfig.LastRefresh.HasValue ? (object)nextConfig.LastRefresh : DBNull.Value;
                newConfigRow["serverStartupTime"] = nextConfig.ServerStartupTime.HasValue ? (object)nextConfig.ServerStartupTime : DBNull.Value;
                newConfigRow["previousOSMetrics"] = GetSerialized(nextConfig.PreviousOSMetrics);
                newConfigRow["previousServerStatistics"] = GetSerialized(nextConfig.PreviousServerStatistics);
                newConfigRow["previousLockStatistics"] = GetSerialized(nextConfig.PreviousLockStatistics);
                newConfigRow["previousDiskDrives"] = GetSerialized(nextConfig.PreviousDiskDrives);
                newConfigRow["previousDbStatistics"] = GetSerialized(nextConfig.PreviousDbStatistics);

                configTable.Rows.Add(newConfigRow);
            }

            return result;
        }

        private const string GetServerSummaryStoredProcedure = "p_GetServerSummary";
        private List<ServerOverview> LoadChartHistory(int monitoredServerId, DateTime? snapshotDateTime, int? historyInMinutes)
        {
            List<ServerOverview> result = new List<ServerOverview>();
            Dictionary<DateTime,ServerOverview> overviewMap = new Dictionary<DateTime, ServerOverview>();
            using (SqlConnection connection = ManagementServiceConfiguration.GetRepositoryConnection())
            {
                using (SqlDataReader dataReader =
                    SqlHelper.ExecuteReader(connection, 
                                            GetServerSummaryStoredProcedure, 
                                            monitoredServerId,
                                            snapshotDateTime.HasValue ? (object)snapshotDateTime.Value.ToUniversalTime() : null, 
                                            historyInMinutes.HasValue ? (object)historyInMinutes.Value : null))
                {
                    // read past historical server status data
                    dataReader.Read();
                    dataReader.NextResult();

                    DataTable overviewStatistics = GetTable(dataReader, false);
                    foreach (DataRow row in overviewStatistics.Rows)
                    {
                        ServerOverview overview = new ServerOverview((string)row["InstanceName"], row, new string[0], new DataRow[0]); 
                        overviewMap.Add(overview.ServerTimeUTC.Value, overview);
                        result.Add(overview);
                    }

                    if (dataReader.NextResult() && dataReader.NextResult())
                    {
                        ServerOverview overview = null;               
                        while (dataReader.Read())
                        {
                            DateTime serverTimeUTC = dataReader.GetDateTime(0);
                            if (overview == null || overview.ServerTimeUTC != serverTimeUTC)
                            {
                                if (!overviewMap.TryGetValue(serverTimeUTC, out overview))
                                    continue;
                            }

                            DiskDrive dd = new DiskDrive();
                            if (!dataReader.IsDBNull(1)) dd.DriveLetter = dataReader.GetString(1).Trim();
                            if (!dataReader.IsDBNull(2)) dd.UnusedSize.Kilobytes = dataReader.GetDecimal(2);
                            if (!dataReader.IsDBNull(3)) dd.TotalSize.Kilobytes = dataReader.GetDecimal(3);                                
                            if (!dataReader.IsDBNull(4)) dd.DiskIdlePercent = dataReader.GetInt64(4);
                            if (!dataReader.IsDBNull(5)) dd.AverageDiskQueueLength = dataReader.GetInt64(5);
                            if (!dataReader.IsDBNull(6)) dd.AvgDiskSecPerRead = TimeSpan.FromMilliseconds(dataReader.GetInt64(6));
                            if (!dataReader.IsDBNull(7)) dd.AvgDiskSecPerTransfer = TimeSpan.FromMilliseconds(dataReader.GetInt64(7));
                            if (!dataReader.IsDBNull(8)) dd.AvgDiskSecPerWrite = TimeSpan.FromMilliseconds(dataReader.GetInt64(8));
                            if (!dataReader.IsDBNull(9)) dd.DiskReadsPerSec = dataReader.GetInt64(9);
                            if (!dataReader.IsDBNull(10)) dd.DiskTransfersPerSec = dataReader.GetInt64(10);
                            if (!dataReader.IsDBNull(11)) dd.DiskWritesPerSec = dataReader.GetInt64(11);

                            overview.DiskDrives.Add(dd.DriveLetter, dd);
                        }
                    }
                }
            }
            return result;
        }

        private static void LoadTable(DataTable table, object obj)
        {
            int rowIndex = table.Rows.Count;

            DataRow dummyRow = table.NewRow();
            table.Rows.Add(dummyRow);

            foreach (PropertyInfo propertyInfo in obj.GetType().GetProperties())
            {
                try
                {
                    Type propertyType = propertyInfo.PropertyType;
                    if (propertyType.IsPrimitive)
                    {
                        DataColumn column = GetColumn(table, propertyInfo, null);
                        table.Rows[rowIndex][column] = propertyInfo.GetValue(obj, null);
                    }
                    else if (propertyType == typeof (FileSize))
                    {
                        DataColumn column = GetColumn(table, propertyInfo, null);
                        FileSize fs = propertyInfo.GetValue(obj, null) as FileSize;
                        if (fs != null && fs.Bytes.HasValue)
                            table.Rows[rowIndex][column] = Convert.ToInt64(fs.Bytes.Value);
                        else
                            table.Rows[rowIndex][column] = DBNull.Value;
                    }
                    else if (propertyType.IsGenericType)
                    {
                        Type genericType = propertyType.GetGenericTypeDefinition();
                        if (genericType == typeof (Nullable<>))
                        {
                            Type[] generics = propertyType.GetGenericArguments();
                            if (generics[0].IsPrimitive || generics[0].IsEnum || generics[0] == typeof(DateTime))
                            {
                                object value = DBNull.Value;
                                object nullableValue = propertyInfo.GetValue(obj, null);
                                if (nullableValue != null)
                                {
                                    if ((bool) propertyType.GetProperty("HasValue").GetValue(nullableValue, null))
                                        value = propertyType.GetProperty("Value").GetValue(nullableValue, null);
                                }

                                if (generics[0].IsEnum)
                                {
                                    generics[0] = Enum.GetUnderlyingType(generics[0]);
                                    if (value != null)
                                        value = Convert.ChangeType(value, generics[0]);
                                }

                                DataColumn column = GetColumn(table, propertyInfo, generics[0]);
                                table.Rows[rowIndex][column] = value;
                            }
                        }
                    }
                } 
                catch (Exception exception)
                {
                    Log.Error(exception);    
                }
            }
        }

        private static DataColumn GetColumn(DataTable table, PropertyInfo property, Type dataType)
        {
            string name = property.Name;
            if (!table.Columns.Contains(name))
            {
                if (dataType == null)
                    dataType = property.PropertyType;
                
                if (dataType == typeof(FileSize))
                    dataType = typeof (long);

                return table.Columns.Add(name, dataType);
            }

            return table.Columns[name];
        }

        private static bool HasValue(object value)
        {
            return value != null && !(value is DBNull);
        }

        private static T GetValue<T>(object value) where T : class
        {
            if (value == null || value == DBNull.Value)
                return null;

            if (value is byte[])
                return Serialized<object>.DeserializeCompressed<T>((byte[]) value);
            
            return (T)value;
        }

        private static byte[] GetSerialized(object value)
        {
            if (value == null || value == DBNull.Value)
                return null;

            return Serialized<object>.SerializeCompressed<object>(value);
        }

        [ServiceMethod("send-startagent")]
        internal object[] SendStartAgent(String instanceName)
        {
            int instanceId = GetInstanceId(instanceName);
            ServiceControlConfiguration configuration = new ServiceControlConfiguration(instanceId, ServiceName.Agent, ServiceControlConfiguration.ServiceControlAction.Start);

            Serialized<Snapshot> result = ManagementService.SendServerAction<ServiceControlConfiguration, Snapshot>(configuration);
            Snapshot snapshot = result;

            // exception logic
            if (snapshot.Error != null)
            {
                object[] ea = new object[1];
                ea[0] = GetRootException(snapshot.Error);
                if (ea[0] == null)
                    ea[0] = new ApplicationException(snapshot.Error.Message);

                return ea;
            }

            return new object[0];
        }

        [ServiceMethod("send-stopagent")]
        internal object[] SendStopAgent(String instanceName)
        {
            int instanceId = GetInstanceId(instanceName);
            ServiceControlConfiguration configuration = new ServiceControlConfiguration(instanceId, ServiceName.Agent, ServiceControlConfiguration.ServiceControlAction.Stop);

            Serialized<Snapshot> result = ManagementService.SendServerAction<ServiceControlConfiguration, Snapshot>(configuration);
            Snapshot snapshot = result;

            // exception logic
            if (snapshot.Error != null)
            {
                object[] ea = new object[1];
                ea[0] = GetRootException(snapshot.Error);
                if (ea[0] == null)
                    ea[0] = new ApplicationException(snapshot.Error.Message);

                return ea;
            }

            return new object[0];
        }

        [ServiceMethod("send-killsession")]
        internal object[] SendKillSession(String instanceName, int spid)
        {
            int instanceId = GetInstanceId(instanceName);
            KillSessionConfiguration configuration = new KillSessionConfiguration(instanceId, spid);
            Serialized<Snapshot> result = ManagementService.SendServerAction<KillSessionConfiguration, Snapshot>(configuration);
            Snapshot snapshot = result;

            // exception logic
            if (snapshot.Error != null)
            {
                object[] ea = new object[1];
                ea[0] = GetRootException(snapshot.Error);
                if (ea[0] == null)
                    ea[0] = new ApplicationException(snapshot.Error.Message);

                return ea;
            }

            return new object[0];
        }

        [ServiceMethod("send-adhocquery")]
        internal object[] SendAdhocQuery(String instanceName, string sql, bool returnData)
        {
            int instanceId = GetInstanceId(instanceName);
            AdhocQueryConfiguration configuration = new AdhocQueryConfiguration(instanceId, sql, returnData);
            Serialized<AdhocQuerySnapshot> result = ManagementService.SendServerAction<AdhocQueryConfiguration, AdhocQuerySnapshot>(configuration);

            AdhocQuerySnapshot snapshot = result;
            if (snapshot.Error == null)
            {
                object[] ra = new object[4];
                ra[0] = snapshot.Duration;
                ra[1] = snapshot.RowsAffected;
                ra[2] = snapshot.RowSetCount;
                ra[3] = snapshot.DataSet;
                return ra;
            }

            // exception logic
            object[] ea = new object[1];
            ea[0] = GetRootException(snapshot.Error);
            if (ea[0] == null)
                ea[0] = new ApplicationException(snapshot.Error.Message);

            return ea;
        }

        [ServiceMethod("get-databaselist")]
        internal IDictionary<string,bool> GetDatabaseList(String instanceName, bool includeSystem, bool includeUser)
        {
            int instanceId = GetInstanceId(instanceName);
            ManagementService service = new ManagementService();
            return service.GetDatabases(instanceId, includeSystem, includeUser);
        }

        [ServiceMethod("send-jobstart")]
        internal object[] SendStartJob(String instanceName, string jobName, string jobStep)
        {
            int instanceId = GetInstanceId(instanceName);
            JobControlConfiguration configuration = new JobControlConfiguration(instanceId, jobName, jobStep, JobControlAction.Start);
            
            Serialized<Snapshot> result = ManagementService.SendServerAction<JobControlConfiguration, Snapshot>(configuration);
            Snapshot snapshot = result;

            // exception logic
            if (snapshot.Error != null)
            {
                object[] ea = new object[1];
                ea[0] = GetRootException(snapshot.Error);
                if (ea[0] == null)
                    ea[0] = new ApplicationException(snapshot.Error.Message);

                return ea;
            }
            return new object[0];
        }

        [ServiceMethod("send-jobstop")]
        internal object[] SendStopJob(String instanceName, string jobName)
        {
            int instanceId = GetInstanceId(instanceName);
            JobControlConfiguration configuration = new JobControlConfiguration(instanceId, jobName, JobControlAction.Stop);

            Serialized<Snapshot> result = ManagementService.SendServerAction<JobControlConfiguration, Snapshot>(configuration);
            Snapshot snapshot = result;

            // exception logic
            if (snapshot.Error != null)
            {
                object[] ea = new object[1];
                ea[0] = GetRootException(snapshot.Error);
                if (ea[0] == null)
                    ea[0] = new ApplicationException(snapshot.Error.Message);

                return ea;
            }
            return new object[0];
        }

        [ServiceMethod("get-activealerts")]
        internal DataTable GetActiveAlerts(String instanceName,
                                           String databaseName,
                                           int? maxRows
                                           )
        {                
            return RepositoryHelper.GetActiveAlerts(ManagementServiceConfiguration.ConnectionString, instanceName, databaseName, maxRows);            
        }

        [ServiceMethod("send-scheduledrefresh")]
        internal object ForceScheduledRefresh(String instanceName)
        {
            int instanceId = GetInstanceId(instanceName);
            Management.QueueDelegate(delegate()
            {
                try
                {
                    ManagementService service = new ManagementService();
                    service.ForceScheduledRefresh(instanceId);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            });

            return null;
        }

        [ServiceMethod("configure-authprovider")]
        internal object ConfigurePulseNotificationProvider(string pulseServerName)
        {
            PulseNotificationProviderInfo pnpi = null;
            NotificationManager manager = Management.Notification;
            foreach (NotificationProviderInfo npi in manager.GetNotificationProviders())
            {
                pnpi = npi as PulseNotificationProviderInfo;
                if (pnpi != null)
                    break;
            }

            if (pnpi != null)
            {
                //if (0 == String.Compare(pulseServerName, pnpi.PulseServer, true))
                //    return false;

                pnpi.PulseServer = pulseServerName;
                pnpi.Enabled = true;
                manager.UpdateNotificationProvider(pnpi, false);
            }
            else
            {
                pnpi = new PulseNotificationProviderInfo(true);
                pnpi.Id = NotificationProviderInfo.GetDefaultId<PulseNotificationProviderInfo>().Value;
                pnpi.PulseServer = pulseServerName;
                pnpi.PulseServerPort = 5168;
                pnpi.Description = pulseServerName + " Newsfeed Service";
                manager.AddNotificationProvider(pnpi, false);

                try
                {
                    NotificationRule defaultNotificationRule = new NotificationRule();
                    defaultNotificationRule.Description = "Newsfeed Status Updates";
                    defaultNotificationRule.Destinations.Add(new PulseDestination());
                    MetricDefinitions metricDefinitions = Management.GetMetricDefinitions();
                    List<int> metricIds = new List<int>();

                    foreach (int metricId in metricDefinitions.GetMetricDefinitionKeys())
                    {
                        switch (metricId)
                        {
                            case (int)Metric.IndexRowHits:          // visual only
                            case (int)Metric.FullTextRefreshHours:  // visual only
                            case (int)Metric.MaintenanceMode:       // no notifications
                            case (int)Metric.Operational:           // no notifications 
                                continue;
                        }

                        metricIds.Add(metricId);
                    }

                    defaultNotificationRule.MetricIDs = metricIds;
                    manager.AddNotificationRule(defaultNotificationRule);
                }
                catch (Exception e)
                {
                    Log.Error("An error occurred while attempting to add the default 'Newsfeed Status Updates' notification rule.", e);
                }
            }

            return true;
        }

        [ServiceMethod("delete-pulseconfiguration")]
        internal object DeletePulseNotificationProvider(string pulseServerName)
        {
            using (Log.InfoCall("DeletePulseNotificationProvider"))
            {
                PulseNotificationProviderInfo pnpi = null;
                NotificationManager manager = Management.Notification;
                foreach (NotificationProviderInfo npi in manager.GetNotificationProviders())
                {
                    pnpi = npi as PulseNotificationProviderInfo;
                    if (pnpi != null)
                        break;
                }

                if (pnpi == null)
                    return true;

                // clean up all the action rules referencing this provider
                foreach (NotificationRule rule in manager.GetNotificationRules())
                {
                    bool delete = rule.Destinations.Count == 1;
                    NotificationDestinationInfo pulseDestination = null;
                    foreach (NotificationDestinationInfo ndi in rule.Destinations)
                    {
                        if (ndi.ProviderID == pnpi.Id)
                        {
                            pulseDestination = ndi;
                            break;
                        }
                    }
                    // no pulse destination - go check next rule
                    if (pulseDestination == null)
                        continue;
                    try
                    {
                        if (delete)
                        {
                            // only one destination - delete the rule
                            manager.DeleteNotificationRule(rule.Id);
                        }
                        else
                        {
                            // remove the destination - update the rule
                            rule.Destinations.Remove(pulseDestination);
                            manager.SaveNotificationRule(rule);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error("Error deleting action or removing Newsfeed destination from rule.  ", e);
                    }
                }

                lock (ssync)
                {
                    pulseProviderConfig = null;
                    application = null;
                }

                manager.DeleteNotificationProvider(pnpi.Id);

                return true;
            }
        }

        #region Integration

        internal static void SafeKickoffAssetSynchronization(object state)
        {
            try
            {
                KickoffAssetSynchronization(state);
            } catch (Exception)
            {
                /* */
            }
        }

        internal static void KickoffAssetSynchronization(object state)
        {
            lock (ssync)
            {
                if (pulseProviderConfig == null || application == null)
                    return;

                if (DoingSynch > 0)
                    return;

                DoingSynch++;

                if (syncTimer != null)
                    syncTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            try
            {
                SynchronizeWithPulse();
            }
            catch (Exception e)
            {
                Log.Error("Error sending Newsfeed synchronization: ", e);
            }
            finally
            {
                lock (ssync)
                {
                    TimeSpan interval = --DoingSynch == 0 ? TimeSpan.FromHours(1) : TimeSpan.FromSeconds(30);

                    if (syncTimer == null)
                        syncTimer = new Timer(KickoffAssetSynchronization, 
                                              null,
                                              (int) interval.TotalMilliseconds, 
                                              Timeout.Infinite);
                    else
                        syncTimer.Change((int)interval.TotalMilliseconds, Timeout.Infinite);
                }
            }
        }

        private static void SynchronizeWithPulse()
        {
            using (Log.InfoCall("SynchronizeWithPulse"))
            {
                if (pulseProviderConfig == null)
                {
                    Log.Warn("Newsfeed provider configuration is not set - skipping synchronize.");
                    return;
                }
                if (application == null)
                {
                    Log.Warn("Newsfeed application id is unknown - skipping synchronize.");
                    return;
                }

                Common.Objects.ApplicationSecurity.Configuration config = null;
                Dictionary<string, List<PermissionDefinition>> pxref =
                    new Dictionary<string, List<PermissionDefinition>>();
                try
                {
                    List<Triple<int, string, bool>> instances =
                        RepositoryHelper.GetMonitoredSqlServerNames(
                            ManagementServiceConfiguration.GetRepositoryConnection(), null, true);

                    config = new Common.Objects.ApplicationSecurity.Configuration();
                    config.Refresh(ManagementServiceConfiguration.ConnectionString);
                    // remap permissions
                    foreach (PermissionDefinition permission in config.Permissions.Values)
                    {
                        if (!permission.Enabled)
                            continue;

                        if (permission.System || permission.PermissionType == PermissionType.Administrator)
                        {   
                            // admins get entries for all servers
                            foreach (Triple<int, string, bool> instance in instances)
                            {
                                List<PermissionDefinition> permList;
                                if (!pxref.TryGetValue(instance.Second, out permList))
                                {
                                    permList = new List<PermissionDefinition>();
                                    pxref.Add(instance.Second, permList);
                                }
                                permList.Add(permission);
                            }
                        }
                        else
                        {
                            // everyone else get entries just for their assigned servers
                            foreach (Server server in permission.GetServerList())
                            {
                                List<PermissionDefinition> permList;
                                if (!pxref.TryGetValue(server.InstanceName, out permList))
                                {
                                    permList = new List<PermissionDefinition>();
                                    pxref.Add(server.InstanceName, permList);
                                }
                                permList.Add(permission);
                            }
                        }
                    }

                    StringBuilder xmldoc = new StringBuilder();
                    using (XmlWriter writer = XmlWriter.Create(xmldoc))
                    {
                        writer.WriteStartDocument();
                        writer.WriteStartElement("Application");
                        writer.WriteAttributeString("Secure", config.IsSecurityEnabled.ToString());

                        foreach (Triple<int, string, bool> instance in instances)
                        {
                            writer.WriteStartElement("Server");
                            writer.WriteAttributeString("serverId", instance.First.ToString());
                            writer.WriteValue(instance.Second);

                            if (config.IsSecurityEnabled)
                            {
                                List<PermissionDefinition> permissions;
                                if (pxref.TryGetValue(instance.Second, out permissions))
                                {
                                    permissions.Sort(WebClient.PermissionSortComparison);
                                    PermissionDefinition last = null;
                                    foreach (PermissionDefinition permission in permissions)
                                    {
                                        if (last == null || !last.Login.Equals(permission.Login))
                                        {
                                            writer.WriteStartElement("Permission");
                                            writer.WriteAttributeString("Rights", ((int) permission.PermissionType).ToString());
                                            writer.WriteValue(permission.Login);
                                            writer.WriteEndElement();
                                        }
                                        last = permission;
                                    }
                                }
                            }
                            writer.WriteEndElement();
                        }
                    }

                    Log.InfoFormat("Sending asset list to Newsfeed @ {0}:{1}", pulseProviderConfig.PulseServer, pulseProviderConfig.PulseServerPort);
                    IPulseRemotingService pulse = GetPulseInterface(pulseProviderConfig);
                    pulse.ApplicationSyncAssets(application.Id, xmldoc.ToString());
                }
                catch (Exception e)
                {
                    Log.Error("Exception trying to send asset list to Newsfeed: ", e);
                }


                // get a list of servers
                //List<string> allServerNames = Management.ScheduledCollection.GetServerNames();
                //Dictionary<string, ServerAcl> acls = null;
                //Common.Objects.ApplicationSecurity.Configuration config = null;
                //try
                //{
                //    config = new Common.Objects.ApplicationSecurity.Configuration();
                //    config.Refresh(ManagementServiceConfiguration.ConnectionString);
                //    if (config.IsSecurityEnabled) 
                //    {
                //        acls = new Dictionary<string, ServerAcl>();
                //        List<string> selectedServerNames = new List<string>();

                //        foreach (Common.Objects.ApplicationSecurity.PermissionDefinition pd in config.Permissions.Values)
                //        {
                //            if (!pd.Enabled)
                //                continue;



                //            List<string> servers;
                //            if (pd.System || pd.PermissionType == PermissionType.Administrator)
                //            {
                //                servers = allServerNames;
                //            }
                //            else
                //            {
                //                servers = selectedServerNames;
                //                selectedServerNames.Clear();
                //                foreach (Common.Objects.ApplicationSecurity.Server instance in pd.GetServerList())
                //                {
                //                    servers.Add(instance.InstanceName);
                //                }
                //            }
                //            foreach (string server in servers)
                //            {
                //                ServerAcl acl = null;
                //                if (!acls.TryGetValue(server, out acl))
                //                {
                //                    acl = new ServerAcl(server);
                //                    acls.Add(server, acl);
                //                }
                //                ServerAclEntry acle = new ServerAclEntry();
                //                acle.LoginName = pd.Login;
                //                acle.Permission = (Permission) (int) pd.PermissionType;
                //                acl.AddAclEntry(acle);
                //            }
                //        }
                //    }

                //    Log.InfoFormat("Sending asset list to Newsfeed @ {0}:{1}", pulseProviderConfig.PulseServer, pulseProviderConfig.PulseServerPort);
                //    IPulseRemotingService pulse = GetPulseInterface(pulseProviderConfig);
                //    pulse.ApplicationSyncAssets(application.Id, allServerNames, acls);
                //}
                //catch (Exception e)
                //{
                //    Log.Error("Exception trying to send asset list to Newsfeed: ", e);
                //}
            }
        }

        public static int PermissionSortComparison(PermissionDefinition left, PermissionDefinition right)
        {
            int rc;

            if (left != null)
            {
                if (right == null) return 1;
                rc = left.Login.CompareTo(right.Login);
                if (rc == 0)
                    rc = right.PermissionType.CompareTo(left.PermissionType);
            }
            else
            {
                rc = right == null ? 0 : -1;
            }

            return rc;
        }

        #endregion

        public class Issue : IComparable
        {
            public readonly Metric Metric;
            public readonly MonitoredState Severity;
            public readonly string Category;
            public readonly string Subject;
            public readonly int Rank;
            private DateTime occurenceTime = DateTime.MinValue;

            internal Issue(Metric metric, MonitoredState severity, int rank, string category, string subject, DateTime occurenceTime)
            {
                Metric = metric;
                Severity = severity;
                Rank = rank;
                Category = category ?? "";
                Subject = subject ?? "";
                OccurenceTime = occurenceTime;
            }

            public DateTime OccurenceTime
            {
                get { return occurenceTime; }
                set { occurenceTime = value; }
            }

            public int CompareTo(object obj)
            {
                if (!(obj is Issue))
                    throw new ArgumentException("obj is null or an incompatable type");

                Issue other = (Issue)obj;

                int rc = -Severity.CompareTo(other.Severity);
                if (rc == 0)
                {
                    rc = Rank.CompareTo(other.Rank);
                    if (rc == 0)
                    {
                        rc = -OccurenceTime.CompareTo(other.OccurenceTime);
                        if (rc == 0)
                        {
                            rc = Metric.CompareTo(other.Metric);
                            if (rc == 0)
                            {
                                if (Subject == null)
                                {
                                    rc = (other.Subject == null) ? 0 : -1;
                                }
                                else if (other.Subject == null)
                                    rc = 1;

                                rc = Subject.CompareTo(other.Subject);
                            }
                        }
                    }
                }
                return rc;
            }

            public override bool Equals(object obj)
            {
                return this.CompareTo(obj) == 0;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public static bool operator !=(Issue left, Issue right)
            {
                return left.CompareTo(right) != 0;
            }

            public static bool operator ==(Issue left, Issue right)
            {
                return left.CompareTo(right) == 0;
            }
        }

        #region Query Templates

        [ServiceMethod("get-querytemplates")]
        internal DataTable GetQueryTemplates()
        {
            return GetTable(ManagementServiceConfiguration.ConnectionString, "p_GetRunQueryScripts");
        }

        [ServiceMethod("delete-querytemplate")]
        internal void DeleteQueryTemplate(int templateId)
        {
            using (Log.DebugCall())
            {
                using (SqlConnection connection = new SqlConnection(ManagementServiceConfiguration.ConnectionString))
                {
                    SqlHelper.ExecuteNonQuery(connection, "p_DeleteRunQueryScript", templateId);
                }
            }
        }

        [ServiceMethod("save-querytemplate")]
        internal void SaveQueryTemplate(int templateId, string name, string query)
        {
            using (Log.DebugCall())
            {
                using (SqlConnection connection = new SqlConnection(ManagementServiceConfiguration.ConnectionString))
                {
                    SqlHelper.ExecuteNonQuery(connection, "p_UpdateRunQueryScript", templateId, name, query);
                }
            }
        }

        [ServiceMethod("save-querytemplate-new")]
        internal int SaveQueryTemplate(string name, string query)
        {
            using (Log.DebugCall())
            {
                using (SqlConnection connection = new SqlConnection(ManagementServiceConfiguration.ConnectionString))
                {
                    return (int)SqlHelper.ExecuteScalar(connection, "p_InsertRunQueryScript", name, query);
                }
            }
        }

        #endregion
    }
}
