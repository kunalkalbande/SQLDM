using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServerStatistics
    {
        [DataMember]
        public long cpuBusyDelta;
        [DataMember]
        public long idleTimeDelta;
        [DataMember]
        public long ioTimeDelta;
        [DataMember]
        public int timeTicks =  31250;

        [DataMember]
        public double cpuPercentage;

        [DataMember]
        public double idlePercentage;

        [DataMember]
        public double ioPercentage;

        [DataMember]
        public long packetsReceived;
        [DataMember]
        public long packetsSent;

        [DataMember]
        public long packetErrors;
        [DataMember]
        public long diskRead;
        [DataMember]
        public long diskWrite;
        [DataMember]
        public long diskErrors;

        [DataMember]
        public long totalConnections;

        [DataMember]
        public long batchRequests;

        [DataMember]
        public long checkpointPages;

        [DataMember]
        public long fullScans;

        [DataMember]
        public long lazyWrites;

        [DataMember]
        public long lockWaits;

        [DataMember]
        public long logFlushes;
        [DataMember]
        public long pageLifeExpectancy;
        [DataMember]
        public long pageLookups;
        [DataMember]
        public long pageReads;
        [DataMember]
        public long pageSplits;

        [DataMember]
        public long pageWrites;
        [DataMember]
        public long readaheadPages;
        [DataMember]
        public long sqlCompilations;
        [DataMember]
        public long sqlRecompilations;
        [DataMember]
        public long tableLockEscalations;
        [DataMember]
        public long workfilesCreated;

        [DataMember]
        public double BufferCacheHitRatio { get; set; }
        [DataMember]
        public double CacheHitRatio { get; set; }

        [DataMember]
        public long WorktablesCreated { get; set; }
        [DataMember]
        public long ReplicationUndistributed { get; set; }
        [DataMember]
        public long ReplicationSubscribed { get; set; }
        [DataMember]
        public long ReplicationUnsubscribed { get; set; }
        [DataMember]
        public double ReplicationLatencyInSeconds { get; set; }
        [DataMember]
        public decimal TempDBSize { get; set; }
        [DataMember]
        public double TempDBSizePercent { get; set; }

         [DataMember]
        public long OldestOpenTransactionsInMinutes { get; set; }

        [DataMember]
         public long PageLifeExpectancySeconds { get; set; }
        [DataMember]
        public decimal SqlMemoryUsed { get; set; }
    }    
}
