//------------------------------------------------------------------------------
// <copyright file="ManagementServiceInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    public class ManagementServiceInfo : ISerializable
    {
        private Guid id;
        private string machineName;
        private string instanceName;
        private string address;
        private int port;
        private Guid? defaultCollectionServiceID;

        private IList<CollectionServiceInfo> collectionServices;

        public ManagementServiceInfo()
        {
            collectionServices = new List<CollectionServiceInfo>(5);
        }

        public ManagementServiceInfo(Guid id, string machineName, string instanceName)
        {
            this.id = id;
            this.machineName = machineName;
            this.instanceName = instanceName;
        }

        private ManagementServiceInfo(SerializationInfo info, StreamingContext context)
        {
            id = (Guid)info.GetValue("id", typeof(Guid));
            machineName = info.GetString("machineName");
            instanceName = info.GetString("instanceName");
            address = info.GetString("address");
            port = info.GetInt32("port");
            collectionServices = (IList<CollectionServiceInfo>)info.GetValue("collectionServices", typeof(List<CollectionServiceInfo>));
        }

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        public string MachineName
        {
            get { return machineName; }
            set { machineName = value; }
        }

        public string InstanceName
        {
            get { return instanceName; }
            set { instanceName = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public Guid? DefaultCollectionServiceID
        {
            get { return defaultCollectionServiceID;  }
            set { defaultCollectionServiceID = value; }
        }

        /// <summary>
        /// Array of collection services managed by this management service.
        /// </summary>
        public IList<CollectionServiceInfo> CollectionServices
        {
            get { return collectionServices; }
            set { collectionServices = value; }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("machineName", machineName);
            info.AddValue("instanceName", instanceName);
            info.AddValue("address", address);
            info.AddValue("port", port);
            info.AddValue("collectionServices", collectionServices);
        }
    }
}
