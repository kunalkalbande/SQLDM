//------------------------------------------------------------------------------
// <copyright file="AlwaysOnAvailabilityGroupsSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Serialization;
using Idera.SQLdm.Common.Snapshots.AlwaysOn;
using System.Xml.Serialization;
using System.Data.SqlClient;
using System.Data;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// The snapshot for the 'AlwaysOn availability group', contains the topology information and
    /// the statistic information
    /// </summary>
    [Serializable]
    public sealed class AlwaysOnAvailabilityGroupsSnapshot : Snapshot
    {
        /// <summary>
        /// The availability groups available in the AlwaysOn instance.
        /// </summary>
        private Dictionary<Guid, AvailabilityGroup> availabilityGroups = new Dictionary<Guid, AvailabilityGroup>();

        /// <summary>
        /// The replicas statistic metrics for each replica available on the AlwayOn group.
        /// </summary>
        private List<AlwaysOnStatistics> replicaStatistics = new List<AlwaysOnStatistics>();

        #region constructors

        //Dummy constructor need to check if we need this later
        public AlwaysOnAvailabilityGroupsSnapshot()
        {
        }

        public AlwaysOnAvailabilityGroupsSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }
        /// <summary>
        /// Preparing historical snapshot from database - SQLDM 10.0.0(Ankit Nagpal)
        /// </summary>
        /// <param name="reader"></param>
        public AlwaysOnAvailabilityGroupsSnapshot(SqlDataReader reader) {
            //SQLDM - 26121 History view of Availability group not displaying correctly
            // Stores Latest UTC Collection Time to display latest snapshot only
            DateTime? recentUTCCollectionDateTime = null;
            DateTime? currentUTCCollectionDateTime = null;
            while (reader.Read()) {
                AlwaysOnStatistics stats = new AlwaysOnStatistics();
              

                if (reader["AlwaysOnStatisticsID"] != System.DBNull.Value) stats.AlwaysOnStatisticsId = (long)reader["AlwaysOnStatisticsID"];
                if (reader["ReplicaId"] != System.DBNull.Value) stats.ReplicaId = (Guid)reader["ReplicaId"];
                if (reader["GroupId"] != System.DBNull.Value) stats.GroupId = (Guid)reader["GroupId"];
                if (reader["DatabaseId"] != System.DBNull.Value) stats.DatabaseId = (int)reader["DatabaseId"];
                if (reader["IsFailoverReady"] != System.DBNull.Value) stats.IsFailoverReady = (bool)reader["IsFailoverReady"];
                if (reader["SynchronizationState"] != System.DBNull.Value) stats.SynchronizationDatabaseState = (AlwaysOnSynchronizationState)(byte)reader["SynchronizationState"];
                if (reader["SynchronizationHealth"] != System.DBNull.Value) stats.SynchronizationDatabaseHealth = (AlwaysOnSynchronizationHealth)(byte)reader["SynchronizationHealth"];
                if (reader["DatabaseState"] != System.DBNull.Value) stats.DatabaseState = (DatabaseStatusNoMaskBit)(byte)reader["DatabaseState"];
                if (reader["IsSuspended"] != System.DBNull.Value) stats.IsSuspended = (bool)reader["IsSuspended"];
                if (reader["LastHardenedTime"] != System.DBNull.Value) stats.LastHardenedTime = (DateTime)reader["LastHardenedTime"];
                if (reader["LogSendQueue"] != System.DBNull.Value) stats.LogSendQueueSize = (long)(double)reader["LogSendQueue"];
                if (reader["LogSendRate"] != System.DBNull.Value) stats.LogSendRate = (long)(double)reader["LogSendRate"];
                if (reader["RedoQueue"] != System.DBNull.Value) stats.RedoQueueSize = (long)(double)reader["RedoQueue"];
                if (reader["RedoRate"] != System.DBNull.Value) stats.RedoRate = (long)(double)reader["RedoRate"];
                if (reader["ReplicaRole"] != System.DBNull.Value) stats.ReplicaRole = (ReplicaRole)(int)reader["ReplicaRole"];
                if (reader["OperationalState"] != System.DBNull.Value) stats.OperationalState = (OperationalState)(byte)reader["OperationalState"];
                if (reader["ConnectedState"] != System.DBNull.Value) stats.ConnectedState = (ConnectedState)(byte)reader["ConnectedState"];
                if (reader["LastConnectErrorNumber"] != System.DBNull.Value) stats.LastConnectionErrorNumber = (int)reader["LastConnectErrorNumber"];
                if (reader["LastConnectErrorDescription"] != System.DBNull.Value) stats.LastConnectedErrorDescription = (string)reader["LastConnectErrorDescription"];
                if (reader["LastConnectErrorTimestamp"] != System.DBNull.Value) stats.LastConnectErrorTimestamp = (DateTime)reader["LastConnectErrorTimestamp"];
                if (reader["EstimatedDataLossTime"] != System.DBNull.Value) stats.EstimatedDataLossTime = (long)reader["EstimatedDataLossTime"];
                if (reader["SynchronizationPerformance"] != System.DBNull.Value) stats.SynchronizationPerformace = (int)reader["SynchronizationPerformance"];
                if (reader["FilestreamSendRate"] != System.DBNull.Value) stats.FileStreamSendRate = (long)reader["FilestreamSendRate"];
                if (reader["EstimatedRecoveryTime"] != System.DBNull.Value) stats.EstimatedRecoveryTime = (int)reader["EstimatedRecoveryTime"];
                if (reader["GroupDatabaseId"] != System.DBNull.Value) stats.GroupDatabaseId = (Guid)reader["GroupDatabaseId"];
                // Update Current UTC Collection Time
                currentUTCCollectionDateTime = null;
                // SQLdm 10.2 (Anshul Aggarwal) - SQLDM-27488 - Databases>Availability Group screen not working as expected.
                if (reader["UTCCollectionDateTime"] != System.DBNull.Value)
                {
                    currentUTCCollectionDateTime = stats.UTCCollectionDateTime = (DateTime)reader["UTCCollectionDateTime"];
                    // Latest Time updated only once to display most recent snapshot
                    if (recentUTCCollectionDateTime == null)
                    {
                        recentUTCCollectionDateTime = currentUTCCollectionDateTime;
                    }
                }

                replicaStatistics.Add(stats);
                // Insert Current Data only if its part of Most Recent Snapshot
                if (recentUTCCollectionDateTime == null || currentUTCCollectionDateTime == null || recentUTCCollectionDateTime != currentUTCCollectionDateTime)
                {
                   continue;
                }

                AlwaysOnDatabase db = new AlwaysOnDatabase();

                if (reader["AlwaysOnDatabasesID"] != System.DBNull.Value) db.AlwaysOnDatabaseId = (long)reader["AlwaysOnDatabasesID"];
                if (reader["ReplicaId"] != System.DBNull.Value) db.ReplicaId = (Guid)reader["ReplicaId"];
                if (reader["GroupId"] != System.DBNull.Value) db.GroupId = (Guid)reader["GroupId"];
                if (reader["GroupDatabaseId"] != System.DBNull.Value) db.GroupDatabaseId = (Guid)reader["GroupDatabaseId"];
                if (reader["DatabaseId"] != System.DBNull.Value) db.DatabaseId = (int)reader["DatabaseId"];
                if (reader["DatabaseName"] != System.DBNull.Value) db.DatabaseName = (string)reader["DatabaseName"];
                if (reader["ServerSourceName"] != System.DBNull.Value) db.ServerSourceName = (string)reader["ServerSourceName"];

                AvailabilityReplica replica = new AvailabilityReplica(stats.GroupId);

                if (reader["ReplicaId"] != System.DBNull.Value) replica.ReplicaId = (Guid)reader["ReplicaId"];
                if (reader["SQLServerID"] != System.DBNull.Value) replica.SqlServerId = (int)reader["SQLServerID"];
                if (reader["ReplicaName"] != System.DBNull.Value) replica.ReplicaName = (string)reader["ReplicaName"];
                if (reader["ServerSourceName"] != System.DBNull.Value) replica.ServerSourceName = (string)reader["ServerSourceName"];
                if (reader["FailoverMode"] != System.DBNull.Value) replica.FailoverMode = (FailOverMode)(int)reader["FailoverMode"];
                if (reader["AvailabilityMode"] != System.DBNull.Value) replica.AvailabilityMode = (AvailabilityMode)(int)reader["AvailabilityMode"];
                if (reader["PrimaryConnectionMode"] != System.DBNull.Value) replica.PrimaryConnectionMode = (PrimaryConnectionMode)(byte)reader["PrimaryConnectionMode"];
                if (reader["SecondaryConnectionMode"] != System.DBNull.Value) replica.SecondaryConnectionMode = (SecondaryConnectionMode)(byte)reader["SecondaryConnectionMode"];
                if (reader["GroupId"] != System.DBNull.Value) replica.GroupId = (Guid)reader["GroupId"];
                if (reader["ReplicaRole"] != System.DBNull.Value) replica.ReplicaRole = (ReplicaRole)(int)reader["ReplicaRole"];
                if (reader["SynchronizationHealthAvailabilityReplica"] != System.DBNull.Value) replica.SynchronizationDatabaseHealth = (AlwaysOnSynchronizationHealth)(byte)reader["SynchronizationHealthAvailabilityReplica"];
                if (reader["LogSendQueue"] != System.DBNull.Value) replica.LogSendQueueSize = (long)(double)reader["LogSendQueue"];
                if (reader["LogSendRate"] != System.DBNull.Value) replica.LogSendRate = (long)(double)reader["LogSendRate"];
                if (reader["RedoQueue"] != System.DBNull.Value) replica.RedoQueueSize = (long)(double)reader["RedoQueue"];
                if (reader["RedoRate"] != System.DBNull.Value) replica.RedoRate = (long)(double)reader["RedoRate"];

                AvailabilityGroup avg = new AvailabilityGroup();
                if (reader["GroupId"] != System.DBNull.Value) avg.GroupId = (Guid)reader["GroupId"];
                if (reader["GroupName"] != System.DBNull.Value) avg.GroupName = (string)reader["GroupName"];
                if (reader["ServerSourceName"] != System.DBNull.Value) avg.ServerSourceName = (string)reader["ServerSourceName"];
                if (reader["ListenerDnsName"] != System.DBNull.Value) avg.ListenerDnsName = (string)reader["ListenerDnsName"];
                if (reader["ListenerPort"] != System.DBNull.Value) avg.ListenerPort = (int)reader["ListenerPort"];
                if (reader["ListenerIpAddress"] != System.DBNull.Value) avg.ListenerIPAddress = (string)reader["ListenerIpAddress"];

                //If same AV GrpId
                if (availabilityGroups.ContainsKey(stats.GroupId)) {
                    //Check for same ReplicaId
                    if (availabilityGroups[stats.GroupId].Replicas.ContainsKey(stats.ReplicaId))
                    {
                        availabilityGroups[stats.GroupId].Replicas[stats.ReplicaId].AddDatabase(db);
                        
                    }
                    else {
                        replica.AddDatabase(db);
                        availabilityGroups[stats.GroupId].AddReplica(replica);
                    }
                }
                else {

                    replica.AddDatabase(db);
                    avg.AddReplica(replica);
                    availabilityGroups.Add(stats.GroupId, avg);
                   
                }

                
            }
            
            
            
        }
        #endregion

        /// <summary>
        /// The availability groups available in the AlwaysOn instance.
        /// </summary>
        [XmlIgnore]
        public IDictionary<Guid, AvailabilityGroup> AvailabilityGroups
        {
            get { return new Dictionary<Guid, AvailabilityGroup>(availabilityGroups); }
        }

        /// <summary>
        /// Returns a list that has all availability groups.
        /// </summary>
        public List<AvailabilityGroup> AvailabilityGroupItems
        {
            get
            {
                return new List<AvailabilityGroup>(availabilityGroups.Values);
            }
        }

        /// <summary>
        /// The replicas statistic metrics for each replica available on the AlwayOn group.
        /// </summary>
        public List<AlwaysOnStatistics> ReplicaStatistics
        {
            get
            {
                return new List<AlwaysOnStatistics>(replicaStatistics);
            }
        }

        public AvailabilityGroup GetAvailabilityGroup(Guid groupId)
        {
            return availabilityGroups[groupId];
        }

        public AvailabilityReplica GetAvailabilityReplica(Guid groupId, Guid replicaId)
        {
            AvailabilityReplica replica = null;

            if (availabilityGroups.ContainsKey(groupId))
            {
                var availabilityGroup = availabilityGroups[groupId];
                replica = availabilityGroup.Replicas[replicaId];
            }

            return replica;
        }

        public AlwaysOnDatabase GetDatabase(Guid groupId, Guid replicaId, long alwaysOnDatabaseId)
        {
            var replica = GetAvailabilityReplica(groupId, replicaId);
            AlwaysOnDatabase database = null;

            if (replica != null)
            {
                database = replica.Databases[alwaysOnDatabaseId];
            }

            return database;
        }


        /// <summary>
        /// Get the Statistics Details about the given database in the replica and group.
        /// </summary>
        public AlwaysOnStatistics GetReplicaStatistics(Guid groupId, Guid replicaId, long databaseId)
        {
            foreach (AlwaysOnStatistics statistics in replicaStatistics)
            {
                if (statistics.GroupId == groupId &&
                    statistics.ReplicaId == replicaId &&
                    statistics.DatabaseId == databaseId)
                    return statistics;
            }
            //If there are no matching statistics then we should reach the below code, Ideally we should never reach the below statement.
            return null;
        }

        /// <summary>
        /// Add a replica statistic in the snapshot.
        /// </summary>
        /// 
        /// <param name="replicaStatistic">The replica statistic to add in the snapshot.</param>
        public void AddReplicaStatistic(AlwaysOnStatistics replicaStatistic)
        {
            if (replicaStatistics == null)
            {
                return;
            }

            replicaStatistics.Add(replicaStatistic);
        }

        /// <summary>
        /// Add an availability group to the snapshot.
        /// </summary>
        /// 
        /// <param name="availabilityGroup">The availability group to add in the snapshot.</param>
        public void AddAvailabilityGroup(AvailabilityGroup availabilityGroup)
        {
            if (availabilityGroup == null)
            {
                return;
            }

            Guid groupId = availabilityGroup.GroupId;

            if (availabilityGroups.ContainsKey(groupId))
            {
                availabilityGroups[groupId] = availabilityGroup;
            }
            else
            {
                availabilityGroups.Add(groupId, availabilityGroup);
            }
        }

        /// <summary>
        /// Returns the XML representation for the 'AvailabilityGroup' items that has contained in the snapshot.
        /// </summary>
        /// <returns>The XML representation for the 'AvailabilityGroup' items that has contained in the snapshot.</returns>
        public Stream AvailabilityGroupItemsToXml()
        {
            return XmlObjectSerializer.Serialize(AvailabilityGroupItems);
        }
    }
}
