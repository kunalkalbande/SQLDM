//------------------------------------------------------------------------------
// <copyright file="CollectionServiceConfigurationMessage.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System.Runtime.Serialization;
    using System;

    [Serializable]
    public class CollectionServiceConfigurationMessage :ISerializable
    {
        public bool noConfiguration; 
        private string instanceName;
        private int    servicePort;
        private string managementHost;
        private int    managementPort;
        private int    heartbeatIntervalSeconds;

        public CollectionServiceConfigurationMessage()
        {
        }


        public CollectionServiceConfigurationMessage(SerializationInfo info, StreamingContext context)
        {
            noConfiguration = info.GetBoolean("noConfiguration");
            instanceName = info.GetString("instanceName");
            servicePort = info.GetInt32("servicePort");
            managementHost = info.GetString("managementHost");
            managementPort = info.GetInt32("managementPort");
            heartbeatIntervalSeconds = info.GetInt32("heartbeatInterval");  
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("noConfiguration", noConfiguration);
            info.AddValue("instanceName", instanceName);
            info.AddValue("servicePort", servicePort);
            info.AddValue("managementHost", managementHost);
            info.AddValue("managementPort", managementPort);
            info.AddValue("heartbeatInterval", heartbeatIntervalSeconds);
        }

        public bool HasNoConfiguration
        {
            get { return noConfiguration; }
            set { noConfiguration = value; }
        }

        public string InstanceName
        {
            get { return instanceName;  }
            set { instanceName = value; }
        }
        public int ServicePort
        {
            get { return servicePort;  }
            set { servicePort = value; }
        }
        public string ManagementHost
        {
            get { return managementHost; }
            set { managementHost = value; }
        }
        public int ManagementPort
        {
            get { return managementPort;  }
            set { managementPort = value; }
        }
        public int HeartbeatIntervalSeconds
        {
            get { return heartbeatIntervalSeconds;  }
            set { heartbeatIntervalSeconds = value; }
        }


    }
}
