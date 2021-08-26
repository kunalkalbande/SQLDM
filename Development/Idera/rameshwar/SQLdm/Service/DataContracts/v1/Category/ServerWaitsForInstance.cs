using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class ServerWaitsForInstance
    {                
        [DataMember]
        public string Category { get; set; }
        
        [DataMember]
        public string WaitType { get; set; }

        [DataMember]
        public Int64 WaitingTasks { get; set; }
        
        [DataMember]
        public Statistics statistics { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string HelpLink { get; set; }

        [DataContract]
        public class Statistics
        {
            [DataMember]
            public WaitDescription TotalWaitInMils { get; set; }

            [DataMember]
            public WaitDescription SignalWaitInMils { get; set; }

            [DataMember]
            public WaitDescription ResourceWaitInMils { get; set; }
        }

        [DataContract]
        public class WaitDescription
        {
            [DataMember]
            public string Wait { get; set; }

            [DataMember]
            public string TotalWait { get; set; }
        }
    }    
}
