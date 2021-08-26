//------------------------------------------------------------------------------
// <copyright file="SQLdmDriveInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;

namespace Idera.SQLdm.PowerShell.Objects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Management.Automation;
    using System.Net;
    using System.Runtime.Remoting;
    using System.Runtime.Remoting.Channels;
    using System.Runtime.Remoting.Channels.Tcp;
    using Idera.SQLdm.Common.Objects;
    using Idera.SQLdm.Common.Objects.ApplicationSecurity;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.PowerShell.Helpers;

    public class SQLdmDriveInfo : PSDriveInfo
    {
        private static bool connectedToAssemblyResolveEvent = false;

        private object sync = new object();
        private SqlConnectionStringBuilder connectionString;
        private string instanceName;
        private string database;

        // application security configuration
        private Configuration appSecurityConfig;
        private DateTime appSecurityLoadDateTime;
        private bool appSecurityDirty;

        private UserToken userToken;
        private bool userTokenDirty = true;
        private DateTime userTokenRetrieved;

        // tag configuration
        private Dictionary<int, Tag> tags;
        private Dictionary<string, Tag> tagsByName;
        private bool tagsDirty = true;
        private DateTime tagsRetrieved;

        public SQLdmDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, string instanceName, string database) 
            : base(name,provider,root,description, credential)
        {
            this.instanceName = instanceName;
            this.database = database;
        }

        public SQLdmDriveInfo(PSDriveInfo driveInfo) : base(driveInfo)
        {
            Validate();
            
            // only configure remoting one time
            if (!connectedToAssemblyResolveEvent)
                ConfigureRemoting();
        }

        internal Configuration GetAppSecurityConfiguration()
        {
            if (appSecurityConfig == null)
            {
                appSecurityConfig = new Configuration();
                appSecurityDirty = true;
            }
            if (appSecurityDirty || appSecurityLoadDateTime - DateTime.Now > TimeSpan.FromMinutes(1))
            {
                appSecurityConfig.Refresh(RepositoryConnectionString);
                appSecurityLoadDateTime = DateTime.Now;
            }
            return appSecurityConfig;
        }

        internal void SetSecurityConfigurationDirty()
        {
            appSecurityDirty = true;
        }

        internal UserToken UserToken
        {
            get
            {
                if (userToken == null)
                {
                    userToken = new UserToken();
                    userTokenDirty = true;
                }
                if (userTokenDirty || userTokenRetrieved - DateTime.Now > TimeSpan.FromMinutes(1))
                {
                    userToken.Refresh(RepositoryConnectionString);
                    userTokenRetrieved = DateTime.Now;
                }
                return userToken;
            }
        }

        internal void SetAppSecurityEnabled(SQLdmDriveInfo drive, bool newEnabled)
        {
           SetContextDataChangeLog(drive);
            
            if (newEnabled)
                DataHelper.ManagementService.EnableSecurity();
            else
                DataHelper.ManagementService.DisableSecurity();

            userTokenDirty = true;
        }

        internal void SetTagsDirty()
        {
            lock (sync)
            {
                tagsDirty = true;
            }
        }

        internal Dictionary<int,Tag> Tags
        {
            get
            {
                lock (sync)
                {
                    if (tags == null)
                    {
                        tags = new Dictionary<int, Tag>();
                        tagsDirty = true;
                    }
                    if (tagsDirty || tagsRetrieved - DateTime.Now > TimeSpan.FromMinutes(1))
                    {
                        tags = Helper.GetTags(this);
                        tagsRetrieved = DateTime.Now;
                        tagsByName = null;
                    }
                    return tags;
                }
            }
        }

        internal Dictionary<string,Tag> TagsByName
        {
            get
            {
                lock (sync)
                {
                    Dictionary<int, Tag> tagsById = Tags;
                    if (tagsByName == null)
                    {
                        tagsByName = new Dictionary<string, Tag>();
                        foreach (Tag tag in tagsById.Values)
                        {   // add names lowercased so we can do case-insensitive lookups
                            tagsByName.Add(tag.Name.ToLower(), tag);                            
                        }
                    }
                    return tagsByName;
                }
            }
        }
        
        public string RepositoryServer
        {
            get { return instanceName; }
        }

        public string RepositoryDatabase
        {
            get { return database; }
        }

        internal string RepositoryConnectionString
        {
            get { return connectionString.ToString(); }
        }
        
        /// <summary>
        /// Validate the connection to the SQLdm repository.
        /// </summary>
        internal void Validate()
        {
            lock(sync)
            {
                connectionString = new SqlConnectionStringBuilder();
                bool trusted = true;

                if (Credential.UserName != null)
                {
                    NetworkCredential creds = (NetworkCredential)Credential;
                    if (creds != null)
                    {
                        connectionString.UserID = creds.UserName;
                        connectionString.Password = creds.Password;
                        //trusted = String.IsNullOrEmpty(connectionString.UserID);
                    }
                }
                connectionString.IntegratedSecurity = trusted;

//                String[] parts = SQLdmProvider.NormalizePath(Root).Split('\\');
//                int chunk = 0;
//                if (parts.Length >= 3)
//                {
//                        string server = parts[0];
//                        if (String.Equals("localhost", server, StringComparison.CurrentCultureIgnoreCase) ||
//                            String.Equals("local", server, StringComparison.CurrentCultureIgnoreCase) ||
//                            String.Equals(".", server, StringComparison.CurrentCultureIgnoreCase) ||
//                            String.Equals("My Computer", server, StringComparison.CurrentCultureIgnoreCase))
//                        {
//                            server = Environment.MachineName;
//                        }
//                        
//                        if (!String.Equals("DEFAULT",parts[1],StringComparison.CurrentCultureIgnoreCase))
//                            server = String.Format("{0}\\{1}", server, parts[1]);
//                        connectionString.DataSource = server;
//
//                        connectionString.InitialCatalog = parts[2];
//                }

                connectionString.DataSource = instanceName;
                connectionString.InitialCatalog = database;

                Debug.Print("Drive connection string: {0}", connectionString);
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
                                int port = (int) dataReader["Port"];

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

        /// <summary>
        /// Close connection to the repository.
        /// </summary>
        internal void Close()
        {
            DataHelper.ManagementServiceURL = null;
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
                connectedToAssemblyResolveEvent = true;
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Type type = typeof(MonitoredSqlServer);
            if (args.Name.StartsWith("Idera.SQLdm.Common"))
                return type.Assembly;
            return null;
        }

        

        /// <summary>
        /// Set contextData for use in management service for changeLog
        /// </summary>
        /// <param name="drive"></param>
        internal void SetContextDataChangeLog(SQLdmDriveInfo drive)
        {
             string user;

            if (String.IsNullOrEmpty(drive.Credential.UserName))
            {
                user = AuditingEngine.GetWorkstationUser();
            }
            else
            {
                user = drive.Credential.UserName.StartsWith("\\") ? drive.Credential.UserName.Remove(0, 1) : drive.Credential.UserName;
            }

            AuditingEngine.SetContextData(user);
        }
    }
}
