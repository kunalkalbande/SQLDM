using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    // SQLdm 9.0 (Abhishek Joshi) --class to serve as a key while aggregating data in buckets

    public class BucketKeyForGraphData
    { 
        public string GroupByID { get; set; }

        public string GroupByName { get; set; }

        public string BucketStartDateTime { get; set; }

        public string BucketEndDateTime { get; set; }
    }
}
