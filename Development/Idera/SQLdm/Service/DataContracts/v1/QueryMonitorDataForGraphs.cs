using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    // SQLdm 9.0 (Abhishek Joshi) --Query Monitoring -response for Graph representation using Query Monitor Statistics

    [DataContract]
    public class QueryMonitorDataForGraphs
    {
        [DataMember]
        public string GroupByID { get; set; }

        [DataMember]
        public string GroupByName { get; set; }

        [DataMember]
        public string BucketType { get; set; }

        [DataMember]
        public Int64 YAxisValue { get; set; }

        [DataMember]
        public string BucketStartDateTime { get; set; }

        [DataMember]
        public string BucketEndDateTime { get; set; }
        
    }
}
