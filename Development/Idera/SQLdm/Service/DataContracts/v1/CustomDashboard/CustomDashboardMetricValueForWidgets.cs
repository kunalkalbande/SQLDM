using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.CustomDashboard
{

    /// <summary>
    /// SQLdm 10.0 (srishti purohit) : Class to give data contract for Custom Dashboard- Metric Value functionality
    /// </summary>
    /// 
    [DataContract]
    public class MetricValueforCustomDashboard
    {
        #region fields


        // Properties
        [DataMember]
        public long CustomDashboardId { get; set; }
        [DataMember]
        public List<MetricValuesforInstance> MetricValuesforInstance { get; set; }
        [DataMember]
        public long WidgetID { get; set; }

        [DataMember]
        public int ServerID { get; set; }

        [DataMember]
        public string InstanceName { get; set; }
        #endregion
    }

    [DataContract]
    public class MetricValuesforInstance
    {
        public MetricValuesforInstance(DateTime collectTime, double metricValue)
        {
            CollectionTime = collectTime;
            MetricValue = metricValue;
        }
        [DataMember]
        public DateTime CollectionTime { get; set; }

        [DataMember]
        public double MetricValue { get; set; }
    }
        
}

