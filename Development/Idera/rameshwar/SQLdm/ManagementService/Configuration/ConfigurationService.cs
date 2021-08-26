//------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.ManagementService.Configuration
{
    using System;
    using System.Data.SqlClient;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.ManagementService.Helpers;
    using Idera.SQLdm.ManagementService.Monitoring;

    public class ConfigurationService : MarshalByRefObject, IManagementServiceConfiguration
    {
        private static readonly BBS.TracerX.Logger Log = BBS.TracerX.Logger.GetLogger(typeof(ConfigurationService));

        public ConfigurationService()
        {
            using (Log.VerboseCall("ConfigurationService"))
            {
                Log.Verbose("Creating ConfigurationService object to service remote call");
            }
        }

        public ManagementServiceConfigurationMessage GetManagementServiceConfiguration()
        {
            ManagementServiceConfigurationMessage message = new ManagementServiceConfigurationMessage();
            ManagementServiceElement element = ManagementServiceConfiguration.GetManagementServiceElement();

            message.InstanceName = element.InstanceName;

            message.RepositoryHost = element.RepositoryServer;
            message.RepositoryDatabase = element.RepositoryDatabase;
            message.WindowsAuthentication = element.RepositoryWindowsAuthentication;
            message.RepositoryUsername = element.RepositoryUsername;
            message.RepositoryPassword = element.RepositoryPassword;

            message.ServicePort = element.ServicePort;

            return message;
        }

        public bool SetManagementServiceConfiguration(ManagementServiceConfigurationMessage config)
        {
            string instanceName = ManagementServiceConfiguration.InstanceName;

            SqlConnectionInfo connectInfo = new SqlConnectionInfo();
            connectInfo.InstanceName = config.RepositoryHost;
            connectInfo.DatabaseName = config.RepositoryDatabase;
            connectInfo.UseIntegratedSecurity = config.WindowsAuthentication;
            connectInfo.ApplicationName = Common.Constants.ManagementServiceConnectionStringApplicationName;
            if (!connectInfo.UseIntegratedSecurity)
            {
                connectInfo.UserName = config.RepositoryUsername;
                connectInfo.Password = config.RepositoryPassword;
            }

            ManagementServiceConfiguration.SetRepositoryConnectInfo(connectInfo);
            ManagementServiceConfiguration.SetServicePort(config.ServicePort);
            ManagementServiceConfiguration.Save();

            Management.AllowAutoRegistration = true;

            return true;
        }

        public string GetCommonAssemblyVersion()
        {
            return new CommonAssemblyInfo().GetCommonAssemblyVersion();
        }

        public string GetCommonAssemblyInformationVersion()
        {
            return new CommonAssemblyInfo().GetCommonAssemblyInformationVersion();
        }

        private ICollectionServiceConfiguration GetCollectionServiceInterface(Guid? collectionServiceID)
        {
            ICollectionServiceConfiguration service = null;

            Guid id = collectionServiceID == null
                          ? Management.CollectionServices.DefaultCollectionServiceID
                          : collectionServiceID.Value;

            if (id != Guid.Empty)
            {
                CollectionServiceContext context = Management.CollectionServices[id];
                if (context != null)
                    service = context.GetService<ICollectionServiceConfiguration>("Configuration");
            }
            return service;
        }

        public string GetCollectionServiceCommonAssemblyVersion(Guid? collectionServiceID)
        {
            string result = "0.0.0.0";
            ICollectionServiceConfiguration service = GetCollectionServiceInterface(collectionServiceID);
            if (service != null)
            {
                try
                {
                    result = service.GetCommonAssemblyVersion();
                }
                catch (Exception e)
                {
                    result = "Unknown: " + e.ToString();
                }
            }
            return result;
        }

        public string GetCollectionServiceCommonAssemblyInformationVersion(Guid? collectionServiceID)
        {
            string result = "0.0.0.0";
            ICollectionServiceConfiguration service = GetCollectionServiceInterface(collectionServiceID);
            if (service != null)
            {
                try
                {
                    result = service.GetCommonAssemblyInformationVersion();
                }
                catch (Exception e)
                {
                    result = "Unknown: " + e.ToString();
                }
            }
            return result;
        }


        public ManagementServiceStatus GetServiceStatus()
        {
            ManagementServiceStatus result = new ManagementServiceStatus();
            result.ManagementServiceID = Management.ManagementService.Id;
            result.InstanceName = new InstanceName(Management.ManagementService.MachineName, Management.ManagementService.InstanceName);
            result.ServicePort = ManagementServiceConfiguration.ServicePort;
            result.Status = SQLdmServiceStatus.Running;
            result.RepositoryHost = ManagementServiceConfiguration.RepositoryHost;
            result.RepositoryDatabase = ManagementServiceConfiguration.RepositoryDatabase;
            Guid defaultCSID = Management.CollectionServices.DefaultCollectionServiceID;
            result.DefaultCollectionServiceID = defaultCSID == Guid.Empty ? (Guid?)null : defaultCSID;

            try
            {
                TestRepositoryConnection(ManagementServiceConfiguration.ConnectionString);
                result.RepositoryConnectionTestResult = TestResult.Passed;
            } catch (Exception e)
            {
                result.RepositoryConnectionTestResult = TestResult.Failed;
                result.RepositoryConnectionTestException = e;
            }

            foreach (CollectionServiceInfo csi in Management.CollectionServices.GetCollectionServices())
            {
                CollectionServiceContext context = Management.CollectionServices[csi.Id];
                ICollectionServiceConfiguration icss = context.GetService<ICollectionServiceConfiguration>("Configuration");
                CollectionServiceStatus csstatus = null;
                try
                {
                    csstatus = icss.GetServiceStatus();
                } 
                catch (Exception e)
                {
                    csstatus = new CollectionServiceStatus();
                    csstatus.CollectionServiceConnectionException = e;
                    csstatus.CollectionServiceID = context.ServiceId;
                    csstatus.InstanceName = new InstanceName(context.CollectionService.MachineName, context.CollectionService.InstanceName);
                    csstatus.ServicePort = context.CollectionService.Port;
                }
                csstatus.LastHeartbeatReceived = context.LastHeartbeatReceived;
                csstatus.NextHeartbeatExpected = context.NextHeartbeatExpected;

                result.CollectionServices.Add(csstatus);
            }

            return result;
        }

        public void TestRepositoryConnection(SqlConnectionInfo connectionInfo)
        {
            TestRepositoryConnection(connectionInfo.ConnectionString);
        }

        private static void TestRepositoryConnection(string repositoryConnectionString)
        {
            if (String.IsNullOrEmpty(repositoryConnectionString))
            {
                throw new ManagementServiceException("Connection string is null or empty.");
            }
            SqlConnection connection = null;
            try
            {
                using (connection = new SqlConnection(repositoryConnectionString))
                {
                    if (!RepositoryHelper.IsValidRepository(connection))
                    {
                        throw new ManagementServiceException("Repository is not valid.");
                    }
                }
            }
            finally
            {
                if (connection != null)
                    connection.Dispose();
            }
        }

    }
}
