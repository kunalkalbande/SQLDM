//------------------------------------------------------------------------------
// <copyright file="AvailabilityReplica.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using ReplicaRoleEnum = Idera.SQLdm.Common.Snapshots.ReplicaRole;

namespace Idera.SQLdm.Common.Snapshots.AlwaysOn
{
    /// <summary>
    /// The availability replica information.
    /// </summary>
    [Serializable]
    public class AvailabilityReplica
    {
        /// <summary>
        /// The replica Id.
        /// </summary>
        private Guid replicaId = Guid.Empty;

        private long sqlServerId = -1;

        private String replicaName = String.Empty;

        /// <summary>
        /// The Cluster Server Name from which is collected the Replica information.
        /// </summary>
        private String serverSourceName = String.Empty;

        private FailOverMode failoverMode = FailOverMode.None;

        private AvailabilityMode availabilityMode = AvailabilityMode.None;

        private PrimaryConnectionMode primaryConnectionMode = PrimaryConnectionMode.None;

        private SecondaryConnectionMode secondaryConnectionMode = SecondaryConnectionMode.None;

        private Guid groupId = Guid.Empty;

        private ReplicaRole replicaRole = ReplicaRole.None;
        private AlwaysOnSynchronizationHealth synchronizationDatabaseHealth = AlwaysOnSynchronizationHealth.None;
        private long logSendQueueSize;
        private long logSendRate;
        private long redoQueueSize;
        private long redoRate;

        private Dictionary<long, AlwaysOnDatabase> databases = new Dictionary<long, AlwaysOnDatabase>();

        public AvailabilityReplica(Guid groupId)
        {
            this.groupId = groupId;
        }

        public AvailabilityReplica()
        {
            // No op
        }

        [XmlElement]
        public Guid ReplicaId
        {
            get { return replicaId; }
            set { replicaId = value; }
        }

        [XmlElement]
        public long SqlServerId
        {
            get { return sqlServerId; }
            set { sqlServerId = value; }
        }

        [XmlElement]
        public String ReplicaName
        {
            get { return replicaName; }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    replicaName = value;
                }
            }
        }

        [XmlIgnore]
        public FailOverMode FailoverMode
        {
            get { return failoverMode; }
            set { failoverMode = value; }
        }

        [XmlElement]
        public int FailoverModeIntValue
        { 
            get
            {
                return (int) failoverMode;
            }
            set
            {
                // No op. Use to expose the property to serialize.
            }
        }

        [XmlIgnore]
        public AvailabilityMode AvailabilityMode
        {
            get { return availabilityMode; }
            set { availabilityMode = value; }
        }

        [XmlElement]
        public int AvailabilityModeIntValue
        {
            get
            {
                return (int) availabilityMode;
            }

            set
            {
                // No op. Use to expose the property to serialize.
            }
        }

        [XmlIgnore]
        public PrimaryConnectionMode PrimaryConnectionMode
        {
            get { return primaryConnectionMode;}
            set { primaryConnectionMode = value; }
        }

        [XmlElement]
        public int PrimaryConnectionModeIntValue
        {
            get
            {
                return (int) primaryConnectionMode;
            }

            set
            {
                // No op. Use to expose the property to serialize.
            }
        }

        [XmlIgnore]
        public SecondaryConnectionMode SecondaryConnectionMode
        {
            get { return secondaryConnectionMode; }
            set { secondaryConnectionMode = value; }
        }

        [XmlElement]
        public int SecondaryConnectionModeIntValue
        {
            get
            {
                return (int) secondaryConnectionMode;
            }

            set
            {
                // No op. Use to expose the property to serialize.
            }
        }

        public ReplicaRole ReplicaRole
        {
            get { return replicaRole; }
            set { replicaRole = value; }
        }

        public AlwaysOnSynchronizationHealth SynchronizationDatabaseHealth
        {
            get { return synchronizationDatabaseHealth; }
            set { synchronizationDatabaseHealth = value; }
        }

        public long LogSendQueueSize
        {
            get { return logSendQueueSize; }
            set { logSendQueueSize = value; }
        }

        public long LogSendRate
        {
            get { return logSendRate; }
            set { logSendRate = value; }
        }

        public long RedoQueueSize
        {
            get { return redoQueueSize; }
            set { redoQueueSize = value; }
        }

        public long RedoRate
        {
            get { return redoRate; }
            set { redoRate = value; }
        }

        public List<AlwaysOnDatabase> DatabaseItems
        {
            get
            {
                return new List<AlwaysOnDatabase>(databases.Values);
            }
            set
            {
                // No op. Defined in order to serialize this class.
            }
        }

        [XmlIgnore]
        public IDictionary<long, AlwaysOnDatabase> Databases
        {
            get { return new Dictionary<long, AlwaysOnDatabase>(databases); }
        }

        [XmlElement]
        public Guid GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        /// <summary>
        /// Exposes the ReplicaRole property in its 'Integer' representation. Indicates the 'Role'
        /// for the 'availability replica'.
        /// </summary>
        [XmlElement]
        public int ReplicaRoleIntValue
        {
            get { return (int) replicaRole; }
            set { replicaRole = (ReplicaRoleEnum)value; }
        }

        /// <summary>
        /// The Cluster Server Name from which is collected the Replica information.
        /// </summary>
        [XmlElement]
        public String ServerSourceName
        {
            get { return serverSourceName; }
            set { serverSourceName = value; }
        }

        public void AddDatabase(AlwaysOnDatabase alwaysOnDatabase)
        {
            if (alwaysOnDatabase == null)
            {
                return;
            }

            long databaseId = alwaysOnDatabase.DatabaseId;

            if (databases.ContainsKey(databaseId))
            {
                databases[databaseId] = alwaysOnDatabase;
            }
            else
            {
                databases.Add(databaseId, alwaysOnDatabase);
            }
        }

        internal static FailOverMode ConvertToFailoverMode(int mode)
        {
            switch ((FailOverMode)mode)
            {
                case FailOverMode.None:
                    return FailOverMode.None;
                case FailOverMode.Manual:
                    return FailOverMode.Manual;
                case FailOverMode.Automatic:
                    return FailOverMode.Automatic;
                default:
                    return FailOverMode.None;
            }
        }

        internal static AvailabilityMode ConvertToAvailabilityMode(int mode)
        {
            switch ((AvailabilityMode)mode)
            {
                case AvailabilityMode.None:
                    return AvailabilityMode.None;
                case AvailabilityMode.Asynchronous:
                    return AvailabilityMode.Asynchronous;
                case AvailabilityMode.Synchronous:
                    return AvailabilityMode.Synchronous;
                default:
                    return AvailabilityMode.None;
            }
        }
    }
}
