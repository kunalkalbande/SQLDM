//------------------------------------------------------------------------------
// <copyright file="MonitoredServer.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.Runtime.Serialization;
    using Idera.SQLdm.Common.Objects;

    /// <summary>
    /// Contains information needed to connect to a monitored server.
    /// </summary>
    [Serializable]
    public class MonitoredServerInfo : MonitoredServer
    {
        #region fields

        private Guid collectionServiceId;
        
        private DateTime registeredDate;
        private SqlConnectionInfo connectionInfo;
        private TimeSpan normalCollectionInterval = TimeSpan.FromMinutes(6);
        private bool inBlackout;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServerInfo"/> class.
        /// </summary>
        public MonitoredServerInfo()
        {
        }

        public MonitoredServerInfo(MonitoredSqlServer server) : this(server.Id, server.ConnectionInfo.InstanceName, server.IsActive) {
            CollectionServiceId = server.CollectionServiceId;
            ConnectionInfo = server.ConnectionInfo;
            NormalCollectionInterval = server.ScheduledCollectionInterval;
            InBlackout = server.MaintenanceModeEnabled;
            RegisteredDate = server.RegisteredDate;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServerInfo"/> class.
        /// </summary>
        /// <param name="monitoredServer">The monitored server.</param>
        protected MonitoredServerInfo(MonitoredServerInfo monitoredServer)
            : base(monitoredServer)
        {
            RegisteredDate = monitoredServer.RegisteredDate;
            ConnectionInfo = monitoredServer.ConnectionInfo;
            NormalCollectionInterval = monitoredServer.NormalCollectionInterval;
            InBlackout = monitoredServer.InBlackout;
        }

        public MonitoredServerInfo(int id, string name, bool enabled)
            : base(id, name, enabled)
        {
            RegisteredDate = DateTime.Now;
            ConnectionInfo = new SqlConnectionInfo(name);
            InBlackout = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MonitoredServerInfo"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        public MonitoredServerInfo(int id, string name, bool enabled, string username, string password)//, int normalCollectionInterval)
            : base(id, name, enabled)
        {
            RegisteredDate = DateTime.Now;

            if (String.IsNullOrEmpty(username) && String.IsNullOrEmpty(password))
                ConnectionInfo = new SqlConnectionInfo(Name);
            else
                ConnectionInfo = new SqlConnectionInfo(Name, username, password);
            NormalCollectionInterval = TimeSpan.FromMinutes(6);
            InBlackout = false;
        }

        protected MonitoredServerInfo(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {        
            registeredDate = info.GetDateTime("registeredDate");
            connectionInfo = (SqlConnectionInfo)info.GetValue("connectionInfo", typeof(SqlConnectionInfo));
            normalCollectionInterval = TimeSpan.FromSeconds(info.GetInt32("collectionInterval"));
            inBlackout = info.GetBoolean("inBlackout");
            collectionServiceId = (Guid) info.GetValue("collectionServiceId", typeof (Guid));
        }
        
        #endregion

        #region properties

        public Guid CollectionServiceId
        {
            get { return collectionServiceId; }
            set { collectionServiceId = value; }
        }
        
        /// <summary>
        /// Gets or sets the registered date.
        /// </summary>
        /// <value>The registered date.</value>
        public DateTime RegisteredDate
        {
            get { return registeredDate; }
            set { registeredDate = value; }
        }

        /// <summary>
        /// Gets or sets the connection info.
        /// </summary>
        /// <value>The connection info.</value>
        public SqlConnectionInfo ConnectionInfo
        {
            get { return connectionInfo; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("connectionInfo");
                connectionInfo = value;
            }
        }

        /// <summary>
        /// Gets or sets the normal collection interval in minutes.
        /// </summary>
        /// <value>The normal collection interval.</value>
        public TimeSpan NormalCollectionInterval
        {
            get { return normalCollectionInterval; }
            set { normalCollectionInterval = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [in blackout].
        /// </summary>
        /// <value><c>true</c> if [in blackout]; otherwise, <c>false</c>.</value>
        public bool InBlackout
        {
            get { return inBlackout; }
            set { inBlackout = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("registeredDate", registeredDate);
            info.AddValue("connectionInfo", connectionInfo);
            info.AddValue("collectionInterval", normalCollectionInterval.Seconds);
            info.AddValue("inBlackout", inBlackout);
            info.AddValue("collectionServiceId", collectionServiceId);
        }
        
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

        #region IComparable<MonitoredServer> Members

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
        /// </returns>
        public int CompareTo(MonitoredServer other)
        {
            return Id.CompareTo(other.Id);
        }

        #endregion
    }
}
