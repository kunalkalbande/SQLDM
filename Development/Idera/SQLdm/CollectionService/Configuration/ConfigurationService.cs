//------------------------------------------------------------------------------
// <copyright file="ConfigurationService.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace Idera.SQLdm.CollectionService.Configuration
{
    using Idera.SQLdm.Common.Services;
    using Idera.SQLdm.Common.Configuration;

    public class ConfigurationService : MarshalByRefObject, ICollectionServiceConfiguration
    {
        public CollectionServiceConfigurationMessage GetCollectionServiceConfiguration()
        {
            CollectionServiceConfigurationMessage message = new CollectionServiceConfigurationMessage();
            message.InstanceName = CollectionServiceConfiguration.InstanceName;
            message.ServicePort = CollectionServiceConfiguration.ServicePort;
            message.ManagementHost = CollectionServiceConfiguration.ManagementServiceAddress;
            message.ManagementPort = CollectionServiceConfiguration.ManagementServicePort;
            message.HeartbeatIntervalSeconds = (int)CollectionServiceConfiguration.HeartbeatInterval.TotalSeconds;

            return message;
        }

        public Result SetCollectionServiceConfiguration(CollectionServiceConfigurationMessage message)
        {
            string instanceName = CollectionServiceConfiguration.InstanceName;

            CollectionServiceConfiguration.SetServicePort(message.ServicePort);
            CollectionServiceConfiguration.SetManagementServiceAddress(message.ManagementHost, message.ManagementPort);
            CollectionServiceConfiguration.SetHeartbeatInterval(TimeSpan.FromSeconds(message.HeartbeatIntervalSeconds));

            // update the configutation
            CollectionServiceConfiguration.Save();

            return Result.Success;
        }

        public string GetCommonAssemblyVersion()
        {
            return new CommonAssemblyInfo().GetCommonAssemblyVersion();
        }

        public string GetCommonAssemblyInformationVersion()
        {
            return new CommonAssemblyInfo().GetCommonAssemblyInformationVersion();
        }


        public CollectionServiceStatus GetServiceStatus()
        {
            CollectionServiceStatus css = new CollectionServiceStatus();
            css.CollectionServiceID = CollectionServiceConfiguration.CollectionServiceId;
            css.InstanceName = new InstanceName(Environment.MachineName, CollectionServiceConfiguration.InstanceName);
            css.ServicePort = CollectionServiceConfiguration.ServicePort;
            css.Status = SQLdmServiceStatus.Running;

            css.ManagementServiceAddress = CollectionServiceConfiguration.ManagementServiceAddress;
            css.ManagementServicePort = CollectionServiceConfiguration.ManagementServicePort;

            IManagementService2 mgmtSvc = RemotingHelper.GetObject<IManagementService2>();
            try
            {
                if ("Hello".Equals(mgmtSvc.Echo("Hello")))
                    css.ManagementServiceTestResult = TestResult.Passed;
            }
            catch (Exception e)
            {
               css.ManagementServiceTestResult = TestResult.Failed;
               css.ManagementServiceTestException = e;
            }

            return css;
        }
    }
}
