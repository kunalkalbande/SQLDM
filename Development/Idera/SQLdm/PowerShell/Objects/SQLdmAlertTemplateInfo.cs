using Idera.SQLdm.Common.Objects;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Thresholds;
using Idera.SQLdm.PowerShell.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;

namespace Idera.SQLdm.PowerShell.Objects
{
    public class SQLdmAlertTemplateInfo
    {
        private static bool connectedToAssemblyResolveEvent = false;

        private object sync = new object();

        public SQLdmAlertTemplateInfo(SqlConnectionStringBuilder connectionString)
        {
            Validate(connectionString);

            // only configure remoting one time
            if (!connectedToAssemblyResolveEvent)
                ConfigureRemoting();
        }

        internal void Validate(SqlConnectionStringBuilder connectionString)
        {
            lock (sync)
            {
                using (SqlConnection connection = new SqlConnection(connectionString.ToString()))
                {
                    connection.Open();
                    Debug.Print("Got connection to {0} version={1}", connection.ConnectionString, connection.ServerVersion);

                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "p_GetDefaultManagementService";
                        command.CommandType = System.Data.CommandType.StoredProcedure;
                        Debug.Print("About to execute reader to get default management service");
                        using (SqlDataReader dataReader = command.ExecuteReader())
                        {
                            if (dataReader.Read())
                            {
                                string machineName = dataReader["MachineName"] as string;
                                string instance = dataReader["InstanceName"] as string;
                                string address = dataReader["Address"] as string;
                                int port = (int)dataReader["Port"];

                                DataHelper.ManagementServiceURL = new Uri(String.Format("tcp://{0}:{1}/Management", address, port));
                                Debug.Print("Management Service at {0}", DataHelper.ManagementServiceURL);
                            }
                            else
                                throw new ApplicationException("Default management service not found.");
                        }
                    }
                }
                if (!connectedToAssemblyResolveEvent)
                    ConfigureRemoting();
            }
        }

        internal void ConfigureRemoting()
        {
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch
            {
                if (RemotingConfiguration.CustomErrorsMode != CustomErrorsModes.Off)
                    Debug.Print("Unable to set remoting custom errors to off.");
            }
            // register a server channel
            IDictionary properties = new System.Collections.Specialized.ListDictionary();

            // register a client channel
            properties = new System.Collections.Specialized.ListDictionary();
            properties["name"] = "tcp-client";
            properties["impersonationLevel"] = "None";
            properties["impersonate"] = false;
            properties["secure"] = false;

            BinaryClientFormatterSinkProvider clientSinkProvider = new BinaryClientFormatterSinkProvider();

            TcpClientChannel tcpClientChannel = new TcpClientChannel(properties, clientSinkProvider);
            try
            {
                ChannelServices.RegisterChannel(tcpClientChannel, false);
            }
            catch (Exception e)
            {
                // someone else may have already registered a client channel - hopefully we can just use it
                if (ChannelServices.RegisteredChannels.Length > 0)
                    Debug.Print("TCP Channel is already registered.");
                else
                    Debug.Print("Unable to register TCP Channel to communicate with the SQLDM Management Service.");
            }
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Debug.Print("Registered channel: " + channel.ChannelName);
            }
            if (!connectedToAssemblyResolveEvent)
            {
                Debug.Print("Connecting to assembly resolve event");
                Debug.Print("Common assembly: " + typeof(MonitoredSqlServer).AssemblyQualifiedName);
                Debug.Print("Vim 25 Service: " + typeof(Vim25Api.VirtualMachinePowerState).AssemblyQualifiedName);
                connectedToAssemblyResolveEvent = true;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Type type = typeof(MonitoredSqlServer);
            Type vimType = typeof(Vim25Api.VirtualMachinePowerState);
            if (args.Name.StartsWith("Idera.SQLdm.Common"))
                return type.Assembly;
            else if (args.Name.StartsWith("Vim"))
                return vimType.Assembly;
            return null;
        }

        public int GetAlertTemplateById(SqlConnectionStringBuilder connectionString, string TemplateName)
        {
            return Helper.GetAlertTemplateById(connectionString, TemplateName);
        }

        public Dictionary<int, string> GetInstanceId(SqlConnectionStringBuilder connectionString, List<string> instanceName)
        {
            return Helper.GetInstanceId(connectionString, instanceName);
        }

        public Dictionary<int, string> GetInstanceByTags(SqlConnectionStringBuilder connectionString, List<string> tagsName)
        {
            return Helper.GetInstanceByTags(connectionString, tagsName);
        }

        public ICollection<Tag> GetTags(SqlConnectionStringBuilder connectionString)
        {
            return Helper.GetTags(connectionString);
        }

        public static List<MonitoredSqlServer> GetMonitoredSqlServers(string repositoryConnectionString, Guid? collectionServiceId, bool activeOnly)
        {
            if (repositoryConnectionString == null)
            {
                throw new ArgumentNullException("repositoryConnectionString");
            }

            using (SqlConnection repositoryConnection = new SqlConnection(repositoryConnectionString))
            {
                repositoryConnection.Open();
                return GetMonitoredSqlServers(repositoryConnectionString, collectionServiceId, activeOnly);
            }
        }
    }
}
