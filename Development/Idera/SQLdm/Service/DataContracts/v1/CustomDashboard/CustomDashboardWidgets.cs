using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;


namespace Idera.SQLdm.Service.DataContracts.v1.CustomDashboard
{
    /// <summary>
    /// SQLdm 10.0 (srishti purohit) : Class to give data contract for Custom Dashboard- Widget functionality
    /// </summary>
    /// 
    [DataContract]
    public class CustomDashboardWidgets
    {
        #region fields

        [DataMember]
        public Int64 WidgetID
        {
            get;
            set;
        }

        [DataMember]
        public int relatedCustomDashboardID
        {
            get;
            set;
        }

        [DataMember]
        public string WidgetName
        {
            get;
            set;
        }

        [DataMember]
        public Int64 WidgetTypeID
        {
            get;
            set;
        }

        [DataMember]
        public Int64 MetricID
        {
            get;
            set;
        }

        [DataMember]
        public Int64 Match
        {
            get;
            set;
        }

        [DataMember]
        public List<int> TagId
        {
            get;
            set;
        }

        [DataMember]
        public List<int> sqlServerId
        {
            get;
            set;
        }

        //SQLdm 10.0 srishti purohit -Created Record Time and updated time will not be response 
        public Nullable<DateTime> RecordCreatedTimestamp { get; set; }

        public Nullable<DateTime> RecordUpdateDateTimestamp { get; set; }

        #endregion

    }
}
