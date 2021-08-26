using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - supported metrics response parameters
     [DataContract]
    public class SupportedMetric
    {

        public SupportedMetric(int metricId, string metricName)
        {
            this.MetricId = metricId;
            this.MetricName = metricName;
        }

         [DataMember]
         public int MetricId { get; set; }

         [DataMember]
         public string MetricName { get; set; }

    }
}
