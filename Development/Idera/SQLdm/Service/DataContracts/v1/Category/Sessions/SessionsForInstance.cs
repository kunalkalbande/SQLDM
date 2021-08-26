using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category.Sessions
{
    [DataContract]
    public class SessionsForInstance
    {
        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataMember]
        public Connection connection  { get; set; }
    
        [DataMember]
        public Usage usage  { get; set; }
    
        [DataMember]
        public LockInformation lockInformation { get; set; }
                
        [DataMember]
        public TempDBUsage tempDBUsage  { get; set; }
    }

    [DataContract]
    public class Connection
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Host { get; set; }

        [DataMember]
        public bool IsUserSession { get; set; }

        [DataMember]
        public bool IsSystemSession { get; set; }

        [DataMember]
        public string Database { get; set; }

        [DataMember]
        public string Status { get; set; }
        
        [DataMember]
        public string Command { get; set; }
        
        [DataMember]
        public string Application { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string NetLibrary { get; set; }

        [DataMember]
        public string NetworkAddress { get; set; }

        [DataMember]
        public string User { get; set; }

        [DataMember]
        public long ExecutionContext { get; set; }

        [DataMember]
        public string TransactionIsolationLevel { get; set; }
    }

    [DataContract]
    public class Usage
    {
        [DataMember]
        public long? OpenTransactions { get; set; }

        [DataMember]
        public long Cpu { get; set; }

        [DataMember]
        public long CpuDelta { get; set; }

        [DataMember]
        public long? PhysicalIO { get; set; }

        [DataMember]
        public string Memory { get; set; }
        
        [DataMember]
        public DateTime LoginTime { get; set; }

        [DataMember]
        public DateTime LastActivity { get; set; }
    }

    [DataContract]
    public class LockInformation
    {
        [DataMember]
        public long WaitTime { get; set; }
        
        [DataMember]
        public string WaitType { get; set; }
        
        [DataMember]
        public string Resource { get; set; }
        
        [DataMember]
        public int? BlockedBy { get; set; }
        
        [DataMember]
        public bool Blocking { get; set; }
        
        [DataMember]
        public int BlockedCount  { get; set; }
    }

    [DataContract]
    public class TempDBUsage
    {        
        [DataMember]
        public long VersionStoreElapsedSeconds { get; set; }
        
        [DataMember]
        public SpaceUsed spaceUsed { get; set; }

        [DataContract]
        public class SpaceUsed
        {
            [DataMember]
            public string SessionUser { get; set; }

            [DataMember]
            public string TaskUser { get; set; }

            [DataMember]
            public string SessionInterval { get; set; }

            [DataMember]
            public string TaskInterval { get; set; }
        }
    }
}
