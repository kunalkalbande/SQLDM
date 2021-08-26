using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
     [DataContract]
    public class Metric
    {

        //START: SQLdm 10.0 :(Gaurav Karwal): adding the property to tell if the metric is numeric

         //private IDictionary<int, Type> _nonNumericMetrics = null;
            

         //public Metric(ref IDictionary<int, Type> nonNumericMetrics) 
         //{
         //    if (nonNumericMetrics != null) 
         //    {
         //        _nonNumericMetrics = nonNumericMetrics;
         //    }
         //}

        //SQLdm 10.0 :(srishti purohit) Fixing issue with MetricNumeric property 
         [DataMember]
         public bool IsMetricNumeric
         {
             get;
             set;
         }

         //END: SQLdm 10.0 :(Gaurav Karwal): adding the property to tell if the metric is numeric

         [DataMember]
         public int MetricId { get; set; }

         [DataMember]
         public int Rank { get; set; }

         [DataMember]
         public string MetricCategory { get; set; }
         
         [DataMember]
         public string Name { get; set; }
         
         [DataMember]
         public string Description { get; set; }
         
         [DataMember]
         public string DefaultInfoValue { get; set; }
                   
         [DataMember]
         public int MinValue { get; set; }
          
         [DataMember]
         public long MaxValue { get; set; }          
          
         [DataMember]
         public long DefaultWarningThreshold { get; set; }
         
         [DataMember]
         public long DefaultCriticalThreshold { get; set; }

         [DataMember]
         public string WarningThreshold { get; set; }

         [DataMember]
         public string CriticalThreshold { get; set; }

    }
}
