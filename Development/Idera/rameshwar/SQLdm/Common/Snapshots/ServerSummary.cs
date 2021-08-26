//------------------------------------------------------------------------------
// <copyright file="ServerSummary.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Server Summary object, representing a standard server overview on-demand resultset
    /// </summary>
    /// <preliminary/>
    [Serializable]
    public class ServerSummary : Snapshot
    {
        #region fields

        private ServiceState?               _agentStatus = null;
        private ServiceState?               _dtcStatus = null;
        private ServiceState?               _fullTextSearchStatus = null;
        private bool?                       _isClustered = null;
        private string                      _language = null;
        private ServerLoginConfiguration    _loginConfiguration = null;
        private long?                       _openTransactions = null;
        private FileSize                    _physicalMemory = null;
        private int?                        _processorCount = null;
        private string                      _replicationDistributor = null;
        private ReplicationState?           _replicationStatus = null;
        private TimeSpan?                   _responseTime = null;
        private DateTime?                   _runningSince = null;
        private ServiceState?               _serverStatus = null;
        private int?                        _affinityMask = null;
        private bool?                       _lightweightPooling = null;
        private bool?                       _priorityBoost = null;
        private int?                        _processorsUsed = null;
        private bool?                       _setWorkingSetSize = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the ServerSummary class with all 
        /// collected data.
        /// </summary>
        public ServerSummary(
            string serverName,
            ServiceState? agentStatus,
            ServiceState? dtcStatus,
            ServiceState? fullTextSearchStatus,
            bool? isClustered,
            string language,
            ServerLoginConfiguration loginConfiguration,
            long? openTransactions,
            FileSize physicalMemory,
            int? processorCount,
            string replicationDistributor,
            ReplicationState? replicationStatus,
            TimeSpan? responseTime,
            DateTime? runningSince,
            ServiceState? serverStatus,
            bool? lightweightPooling,
            bool? priorityBoost,
            int? processorsUsed,
            bool? setWorkingSetSize)
            : base(serverName)
        {
            if (serverName == null || serverName.Length == 0) throw new ArgumentNullException("serverName");

            _agentStatus = agentStatus;
           _dtcStatus = dtcStatus;
           _fullTextSearchStatus = fullTextSearchStatus;
           _isClustered = isClustered;
           _language = language;
           _loginConfiguration = loginConfiguration;
           _openTransactions = openTransactions;
           _physicalMemory = physicalMemory;
           _processorCount = processorCount;
           _replicationDistributor = replicationDistributor;
           _replicationStatus = replicationStatus;
           _responseTime = responseTime;
           _runningSince = runningSince;
           _serverStatus = serverStatus;
           _lightweightPooling = lightweightPooling;
           _priorityBoost = priorityBoost;
           _processorsUsed = processorsUsed;
           _setWorkingSetSize = setWorkingSetSize;
        }


        public ServerSummary()
        {
            
        }
        #endregion

        #region properties

        /// <summary>
        /// Gets the sampled SQL Server Agent status.
        /// </summary>
        public ServiceState? AgentStatus
        {
            get { return _agentStatus; }
        }

        ///// <summary>
        ///// Gets the configuration options for the sampled SQL Server.
        ///// </summary>
        //public ServerConfigurationOptions ConfigurationOptions
        //{
        //    get { return _configurationOptions; }
        //}

        /// <summary>
        /// Gets the distributed transaction coordinator status.
        /// </summary>
        public ServiceState? DtcStatus
        {
            get { return _dtcStatus; }
        }

        /// <summary>
        /// Gets the status of the full text search service.
        /// </summary>
        public ServiceState? FullTextSearchStatus
        {
            get { return _fullTextSearchStatus; }
        }

        /// <summary>
        /// Gets whether the sampled SQL Server is clustered.
        /// </summary>
        public bool? IsClustered
        {
            get { return _isClustered; }
        }

        /// <summary>
        /// Gets the language version of SQL Server.
        /// </summary>
        public string Language
        {
            get { return _language; }
        }

        /// <summary>
        /// Gets the login configuration of the sampled SQL Server.
        /// </summary>
        public ServerLoginConfiguration LoginConfiguration
        {
            get { return _loginConfiguration; }
        }

        /// <summary>
        /// Gets the number of open transactions on the SQL Server.
        /// </summary>
        public long? OpenTransactions
        {
            get { return _openTransactions; }
        }

        ///// <summary>
        ///// Gets the performance counters values for the sampled SQL Server.
        ///// </summary>
        //public ServerPerformanceCounters? PerformanceCounters
        //{
        //    get { return _performanceCounters; }
        //}

        ///// <summary>
        ///// Gets the procedure cache health of the sampled SQL Server.
        ///// </summary>
        //public ServerProcedureCache? ProcedureCache
        //{
        //    get { return _procedureCache; }
        //}


        /// <summary>
        /// Gets the amount of RAM installed on the computer running SQL Server.
        /// </summary>
        public FileSize PhysicalMemory
        {
            get { return _physicalMemory; }
        }

        /// <summary>
        /// Gets the number of processors in the computer running SQL Server.
        /// </summary>
        public int? ProcessorCount
        {
            get { return _processorCount; }
        }

        ///// <summary>
        ///// Gets the SQL Server product version.
        ///// </summary>
        //public ServerVersion? ProductVersion
        //{
        //    get { return _productVersion; }
        //}

        /// <summary>
        /// Gets the name of the distributor to which this SQL Server publishes/subscribes.
        /// </summary>
        public string ReplicationDistributor
        {
            get { return _replicationDistributor; }
        }

        /// <summary>
        /// Gets the replication status of the SQL Server.
        /// </summary>
        public ReplicationState? ReplicationStatus
        {
            get { return _replicationStatus; }
        }

        /// <summary>
        /// Gets the response time of the SQL Server.
        /// </summary>
        public TimeSpan? ResponseTime
        {
            get { return _responseTime; }
        }

        /// <summary>
        /// Gets the datetime the SQL Server was last started.
        /// </summary>
        public DateTime? RunningSince
        {
            get { return _runningSince; }
        }

        /// <summary>
        /// Gets the status of the sampled SQL Server.
        /// </summary>
        public ServiceState? ServerStatus
        {
            get { return _serverStatus; }
        }

        ///// <summary>
        ///// Gets the system processes overview for the sampled SQL Server.
        ///// </summary>
        //public ServerSystemProcesses SystemProcesses
        //{
        //    get { return _systemProcesses; }
        //}

        ///// <summary>
        ///// Gets the throughput for the sampled SQL Server.
        ///// </summary>
        //public ServerThroughput Throughput
        //{
        //    get { return _throughput; }
        //}

        /// <summary>
        /// Gets a bit mask representing which processors in a symmetric multi processor (SMP)
        /// environment SQL Server will use.
        /// </summary>
        public int? AffinityMask
        {
            get { return _affinityMask; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server is using fiber mode scheduling for 
        /// thread context switching.
        /// </summary>
        public bool? LightWeightPooling
        {
            get { return _lightweightPooling; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server is running at a higher scheduling 
        /// priority that other processes on the same computer.
        /// </summary>
        public bool? PriorityBoost
        {
            get { return _priorityBoost; }
        }

        /// <summary>
        /// Gets the total number of processors used based on the affinity mask.
        /// </summary>
        public int? ProcessorsUsed
        {
            get { return _processorsUsed; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server uses a reserved physical memory space.
        /// </summary>
        public bool? SetWorkingSetSize
        {
            get { return _setWorkingSetSize; }
        }
        
        #endregion

        #region events

        #endregion

        #region methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("Server Summary - " + System.Environment.NewLine);
            sb.AppendLine("AgentStatus: " + AgentStatus.ToString());
            sb.AppendLine("DtcStatus: " + DtcStatus.ToString());
            sb.AppendLine("FullTextSearchStatus: " + FullTextSearchStatus.ToString());
            sb.AppendLine("IsClustered: " + IsClustered.ToString());
            sb.AppendLine("Language: " + Language.ToString());
            sb.AppendLine("LoginConfiguration: " + (LoginConfiguration != null ? LoginConfiguration.ToString(): "NULL"));
            sb.AppendLine("OpenTransactions: " + OpenTransactions.ToString());
            sb.AppendLine("PhysicalMemory: " + PhysicalMemory.ToString());
            sb.AppendLine("ProcessorCount: " + ProcessorCount.ToString());
            sb.AppendLine("ReplicationDistributor: " + ReplicationDistributor.ToString());
            sb.AppendLine("ResponseTime: " + ResponseTime.ToString());
            sb.AppendLine("RunningSince: " + RunningSince.ToString());
            sb.AppendLine("ServerStatus: " + ServerStatus.ToString());
            return sb.ToString();
        }
        
        
        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
