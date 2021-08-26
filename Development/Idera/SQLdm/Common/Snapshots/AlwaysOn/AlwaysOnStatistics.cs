//------------------------------------------------------------------------------
// <copyright file="AlwaysOnStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>

//------------------------------------------------------------------------------

using System;

namespace Idera.SQLdm.Common.Snapshots.AlwaysOn
{
    /// <summary>
    /// The AlwaysOn statistics information.
    /// </summary>
    [Serializable]
    public class AlwaysOnStatistics
    {
        private long alwaysOnStatisticsId = -1;
        private Guid replicaId = Guid.Empty;
        private Guid groupId = Guid.Empty;
        private long databaseId = -1;
        private bool isFailoverReady = false;
        private AlwaysOnSynchronizationState synchronizationDatabaseState = AlwaysOnSynchronizationState.None;
        private AlwaysOnSynchronizationHealth synchronizationDatabaseHealth = AlwaysOnSynchronizationHealth.None;
        private DatabaseStatusNoMaskBit databaseState = DatabaseStatusNoMaskBit.Offline;
        private bool isSuspended = false;
        private DateTime? lastHardenedTime = null;//new DateTime(1753, 1, 1);//Jan 1st, 1753 in the SQL server min date time
        private long logSendQueueSize;
        private long logSendRate;
        private long redoQueueSize;
        private long redoRate;
        private ReplicaRole replicaRole = ReplicaRole.None;
        private OperationalState operationalState = OperationalState.ReplicaIsNotLocal;
        private ConnectedState connectedState = ConnectedState.Disconnected;
        private AlwaysOnSynchronizationHealth synchronizationReplicaHealth = AlwaysOnSynchronizationHealth.None;
        private int lastConnectionErrorNumber = -1;
        private String lastConnectedErrorDescription = String.Empty;
        private DateTime? lastConnectErrorTimestamp = null;//new DateTime(1753, 1, 1);//Jan 1st, 1753 in the SQL server min date time
        private DateTime? utcCollectionDateTime = null; // SQLdm 10.2 (Anshul Aggarwal) - SQLDM-27488 - Databases>Availability Group screen not working as expected.
        private long estimatedDataLossTime;
        private int synchronizationPerformace = 0;
        private long fileStreamSendRate = 0;

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        private double? alwaysOnEstimatedDataLossTimeBaselineMean = null;
        private double? alwaysOnEstimatedDataLossTimeAsBaselinePerc = null;

        private double? alwaysOnEstimatedRecoveryTimeBaselineMean = null;
        private double? alwaysOnEstimatedRecoveryTimeAsBaselinePerc = null;

        private double? synchronizationPerformanceBaselineMean = null;
        private double? synchronizationPerformanceAsBaselinePerc = null;

        private double? logSendQueueSizeBaselineMean = null;
        private double? logSendQueueSizeAsBaselinePerc = null;

        private double? redoQueueSizeBaselineMean = null;
        private double? redoQueueSizeAsBaselinePerc = null;

        private double? redoRateBaselineMean = null;
        private double? redoRateAsBaselinePerc = null;
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline

        //START: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline
        public double? AlwaysOnEstimatedDataLossTimeBaselineMean
        {
            get { return alwaysOnEstimatedDataLossTimeBaselineMean; }
            set { alwaysOnEstimatedDataLossTimeBaselineMean = value; }
        }
        public double? AlwaysOnEstimatedDataLossTimeAsBaselinePerc
        {
            get { return alwaysOnEstimatedDataLossTimeAsBaselinePerc; }
            set { alwaysOnEstimatedDataLossTimeAsBaselinePerc = value; }
        }

        public double? AlwaysOnEstimatedRecoveryTimeBaselineMean
        {
            get { return alwaysOnEstimatedRecoveryTimeBaselineMean; }
            set { alwaysOnEstimatedRecoveryTimeBaselineMean = value; }
        }
        public double? AlwaysOnEstimatedRecoveryTimeAsBaselinePerc
        {
            get { return alwaysOnEstimatedRecoveryTimeAsBaselinePerc; }
            set { alwaysOnEstimatedRecoveryTimeAsBaselinePerc = value; }
        }

        public double? SynchronizationPerformanceBaselineMean
        {
            get { return synchronizationPerformanceBaselineMean; }
            set { synchronizationPerformanceBaselineMean = value; }
        }
        public double? SynchronizationPerformanceAsBaselinePerc
        {
            get { return synchronizationPerformanceAsBaselinePerc; }
            set { synchronizationPerformanceAsBaselinePerc = value; }
        }

        public double? LogSendQueueSizeBaselineMean
        {
            get { return logSendQueueSizeBaselineMean; }
            set { logSendQueueSizeBaselineMean = value; }
        }
        public double? LogSendQueueSizeAsBaselinePerc
        {
            get { return logSendQueueSizeAsBaselinePerc; }
            set { logSendQueueSizeAsBaselinePerc = value; }
        }

        public double? RedoQueueSizeBaselineMean
        {
            get { return redoQueueSizeBaselineMean; }
            set { redoQueueSizeBaselineMean = value; }
        }
        public double? RedoQueueSizeAsBaselinePerc
        {
            get { return redoQueueSizeAsBaselinePerc; }
            set { redoQueueSizeAsBaselinePerc = value; }
        }

        public double? RedoRateBaselineMean
        {
            get { return redoRateBaselineMean; }
            set { redoRateBaselineMean = value; }
        }
        public double? RedoRateAsBaselinePerc
        {
            get { return redoRateAsBaselinePerc; }
            set { redoRateAsBaselinePerc = value; }
        }
        //END: SQLdm 10.0 (Tarun Sapra)- baseline mean and perc of baseline

        /// <summary>
        /// Indicates the estimated time in which the log records are being redone on a given 
        /// secondary database.
        /// </summary>
        private int estimatedRecoveryTime = -1;

        /// <summary>
        /// Identifier of the database within the availability group. This identifier is identical
        /// on every replica to which this database is joined.
        /// </summary>
        private Guid groupDatabaseId = Guid.Empty;

        public long AlwaysOnStatisticsId
        {
            get { return alwaysOnStatisticsId; }
            set { alwaysOnStatisticsId = value; }
        }

        public Guid ReplicaId
        {
            get { return replicaId; }
            set { replicaId = value; }
        }

        public Guid GroupId
        {
            get { return groupId; }
            set { groupId = value; }
        }

        public long DatabaseId
        {
            get { return databaseId; }
            set { databaseId = value; }
        }

        public bool IsFailoverReady
        {
            get { return isFailoverReady; }
            set { isFailoverReady = value; }
        }

        public AlwaysOnSynchronizationState SynchronizationDatabaseState
        {
            get { return synchronizationDatabaseState; }
            set { synchronizationDatabaseState = value; }
        }

        public AlwaysOnSynchronizationHealth SynchronizationDatabaseHealth
        {
            get { return synchronizationDatabaseHealth; }
            set { synchronizationDatabaseHealth = value; }
        }

        public DatabaseStatusNoMaskBit DatabaseState
        {
            get { return databaseState; }
            set { databaseState = value; }
        }

        public bool IsSuspended
        {
            get { return isSuspended; }
            set { isSuspended = value; }
        }

        public DateTime? LastHardenedTime
        {
            get { return lastHardenedTime; }
            set { lastHardenedTime = value; }
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

        public long RedoRate
        {
            get { return redoRate; }
            set { redoRate = value; }
        }

        public AlwaysOnSynchronizationHealth SynchronizationReplicaHealth
        {
            get { return synchronizationReplicaHealth; }
            set { synchronizationReplicaHealth = value; }
        }

        public int LastConnectionErrorNumber
        {
            get { return lastConnectionErrorNumber; }
            set { lastConnectionErrorNumber = value; }
        }

        public string LastConnectedErrorDescription
        {
            get { return lastConnectedErrorDescription; }
            set { lastConnectedErrorDescription = value; }
        }

        public int SynchronizationPerformace
        {
            get { return synchronizationPerformace; }
            set { synchronizationPerformace = value; }
        }

        public long FileStreamSendRate
        {
            get { return fileStreamSendRate; }
            set { fileStreamSendRate = value; }
        }

        public long RedoQueueSize
        {
            get { return redoQueueSize; }
            set { redoQueueSize = value; }
        }

        public ReplicaRole ReplicaRole
        {
            get { return replicaRole; }
            set { replicaRole = value; }
        }

        public OperationalState OperationalState
        {
            get { return operationalState; }
            set { operationalState = value; }
        }

        public ConnectedState ConnectedState
        {
            get { return connectedState; }
            set { connectedState = value; }
        }

        public DateTime? LastConnectErrorTimestamp
        {
            get { return lastConnectErrorTimestamp; }
            set { lastConnectErrorTimestamp = value; }
        }

        /// <summary>
        /// SQLdm 10.2 (Anshul Aggarwal) - SQLDM-27488 Databases>Availability Group screen not working as expected.
        /// Gives UTC Collection DateTime for the AlwaysOn statistic.
        /// </summary>
        public DateTime? UTCCollectionDateTime
        {
            get { return utcCollectionDateTime; }
            set { utcCollectionDateTime = value; }
        }

        public long EstimatedDataLossTime
        {
            get { return estimatedDataLossTime; }
            set { estimatedDataLossTime = value; }
        }

        /// <summary>
        /// The Estimated Recovery Time. Indicates the estimated time in which the log records are
        /// being redone on a given secondary database.
        /// </summary>
        public int EstimatedRecoveryTime
        {
            get { return estimatedRecoveryTime; }
            set { estimatedRecoveryTime = value; }
        }

        /// <summary>
        /// Identifier of the database within the availability group. This identifier is identical
        /// on every replica to which this database is joined.
        /// </summary>
        public Guid GroupDatabaseId
        {
            get { return groupDatabaseId; }
            set { groupDatabaseId = value; }
        }

        internal static DatabaseStatusNoMaskBit ConvertToDatabaseStatusNoMaskBit(int mode)
        {
            switch ((DatabaseStatusNoMaskBit)mode)
            {
                case DatabaseStatusNoMaskBit.Online:
                    return DatabaseStatusNoMaskBit.Online;
                case DatabaseStatusNoMaskBit.Restoring:
                    return DatabaseStatusNoMaskBit.Restoring;
                case DatabaseStatusNoMaskBit.Recovering:
                    return DatabaseStatusNoMaskBit.Recovering;
                case DatabaseStatusNoMaskBit.RecoveryPending:
                    return DatabaseStatusNoMaskBit.RecoveryPending;
                case DatabaseStatusNoMaskBit.Suspect:
                    return DatabaseStatusNoMaskBit.Suspect;
                case DatabaseStatusNoMaskBit.Emergency:
                    return DatabaseStatusNoMaskBit.Emergency;
                case DatabaseStatusNoMaskBit.Offline:
                    return DatabaseStatusNoMaskBit.Offline;
                default:
                    return DatabaseStatusNoMaskBit.Offline;
            }
        }

        internal static AlwaysOnSynchronizationHealth ConvertToAlwaysOnSynchronizationHealth(int mode)
        {
            switch ((AlwaysOnSynchronizationHealth)mode)
            {
                case AlwaysOnSynchronizationHealth.None:
                    return AlwaysOnSynchronizationHealth.None;
                case AlwaysOnSynchronizationHealth.NotHealthy:
                    return AlwaysOnSynchronizationHealth.NotHealthy;
                case AlwaysOnSynchronizationHealth.PartiallyHealthy:
                    return AlwaysOnSynchronizationHealth.PartiallyHealthy;
                case AlwaysOnSynchronizationHealth.Healthy:
                    return AlwaysOnSynchronizationHealth.Healthy;
                default:
                    return AlwaysOnSynchronizationHealth.None;
            }
        }

        internal static ReplicaRole ConvertToReplicaRole(int mode)
        {
            switch ((ReplicaRole)mode)
            {
                case ReplicaRole.None:
                    return ReplicaRole.None;
                case ReplicaRole.Resolving:
                    return ReplicaRole.Resolving;
                case ReplicaRole.Primary:
                    return ReplicaRole.Primary;
                case ReplicaRole.Secondary:
                    return ReplicaRole.Secondary;
                default:
                    return ReplicaRole.None;
            }
        }

        internal static OperationalState ConvertToOperationalState(int mode)
        {
            switch ((OperationalState)mode)
            {
                case OperationalState.ReplicaIsNotLocal:
                    return OperationalState.ReplicaIsNotLocal;
                case OperationalState.PendingFailover:
                    return OperationalState.PendingFailover;
                case OperationalState.Pending:
                    return OperationalState.Pending;
                case OperationalState.Online:
                    return OperationalState.Online;
                case OperationalState.Offline:
                    return OperationalState.Offline;
                case OperationalState.Failed:
                    return OperationalState.Failed;
                case OperationalState.FailedNoQuorum:
                    return OperationalState.FailedNoQuorum;
                default:
                    return OperationalState.ReplicaIsNotLocal;
            }
        }

        internal static AlwaysOnSynchronizationState ConvertToAlwaysOnSynchronizationState(int mode)
        {
            switch ((AlwaysOnSynchronizationState)mode)
            {
                case AlwaysOnSynchronizationState.None:
                    return AlwaysOnSynchronizationState.None;
                case AlwaysOnSynchronizationState.NotSynchronizing:
                    return AlwaysOnSynchronizationState.NotSynchronizing;
                case AlwaysOnSynchronizationState.Synchronizing:
                    return AlwaysOnSynchronizationState.Synchronizing;
                case AlwaysOnSynchronizationState.Synchronized:
                    return AlwaysOnSynchronizationState.Synchronized;
                case AlwaysOnSynchronizationState.Reverting:
                    return AlwaysOnSynchronizationState.Reverting;
                case AlwaysOnSynchronizationState.Initializing:
                    return AlwaysOnSynchronizationState.Initializing;
                default:
                    return AlwaysOnSynchronizationState.None;
            }
        }

    }
}
