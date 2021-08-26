using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    //SQLdm 9.1 (Sanjali Makkar) (Baseline Statistics) - Adding Baseline Statistics For Metric

    [DataContract]
    public class BaselineForMetric
    {
        //For the Time Bracket of T1-T2, output will be the baseline data at time T1
        //Start Time (T1) denotes the start of Time Bracket (T1-T2)
        [DataMember]
        public DateTime StartTime { get; set; }

        //End Time (T2) denotes the end of Time Bracket (T1-T2)
        [DataMember]
        public DateTime? EndTime { get; set; }
        
        //Value of the baseline at time T1
        [DataMember]
        public double Value { get; set; }
    }

}

