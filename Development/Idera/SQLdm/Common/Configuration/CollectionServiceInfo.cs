//------------------------------------------------------------------------------
// <copyright file="CollectionServiceInfo.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Stores information about a specific Collection Service.
    /// </summary>
    [Serializable]
    public class CollectionServiceInfo : ISerializable
    {
        #region fields

        private ManagementServiceInfo managementService;

        private Guid id;
        private bool enabled;
        
        private string instanceName;
        private string machineName;
        private string address;
        private int port;
        private DateTime lastHeartbeatReceived = DateTime.MinValue;

        #endregion

        #region constructors

        /// <summary>
        /// Private default constructor for hibernate
        /// </summary>
        public CollectionServiceInfo() 
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:CollectionServiceInfo"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public CollectionServiceInfo(Guid id, bool enabled) 
        {
            this.id = id;
            this.enabled = enabled;
        }

        public CollectionServiceInfo(SerializationInfo info, StreamingContext context)
        {
            id = (Guid)info.GetValue("id", typeof(Guid));
            enabled = info.GetBoolean("enabled");
            managementService = (ManagementServiceInfo)info.GetValue("managementService", typeof(ManagementServiceInfo));
            instanceName = info.GetString("instanceName");
            machineName = info.GetString("machineName");
            address = info.GetString("address");
            port = info.GetInt32("port");
            lastHeartbeatReceived = info.GetDateTime("nextHeartbeat");
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The id.</value>

        public Guid Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:MonitoredObject"/> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        
        /// <summary>
        /// The correct method for associating a collection and management 
        /// service.  Sets the management service that this collection service 
        /// is assigned to.  This will also add this collection service to 
        /// this list of collection services in the management service.  
        /// The reverse (adding a collection service directly to the 
        /// management services list of collection services will not 
        /// maintain this relationship) is not supported. 
        /// </summary>
        public ManagementServiceInfo ManagementService
        {
            get { return managementService; }
            set
            {
                // remove us from the current management service
                if (managementService != null)
                    managementService.CollectionServices.Remove(this);
                // update the current setting
                managementService = value;
                // add us to the new management service
                if (managementService != null)
                    managementService.CollectionServices.Add(this);
            }
        }

        public DateTime LastHeartbeatReceived
        {
            get { return lastHeartbeatReceived; }
            set { lastHeartbeatReceived = value; }
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

        /// <summary>
        /// Gets or sets the name of the computer that the collection service instance 
        /// installed on.
        /// </summary>
        /// <value>The name.</value>
        public string MachineName
        {
            get { return machineName; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentNullException("machineName");
                machineName = value;
            }
        }


        /// <summary>
        /// Gets or sets the name of the collection service instance.
        /// </summary>
        /// <value>The name.</value>
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


        #endregion

        #region events

        #endregion

        #region methods

        public Uri ToUri()
        {
            return new UriBuilder("tcp", Address, Port).Uri;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("id", id);
            info.AddValue("enabled", enabled);
            info.AddValue("managementService", managementService);
            info.AddValue("instanceName", instanceName);
            info.AddValue("machineName", machineName);
            info.AddValue("address", address);
            info.AddValue("port", port);
            info.AddValue("nextHeartbeat", lastHeartbeatReceived);
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
