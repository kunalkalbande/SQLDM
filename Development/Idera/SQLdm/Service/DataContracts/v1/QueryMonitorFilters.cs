using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class QueryMonitorFilters
    {
        [DataMember]
        public string ApplicationIds { get; set; }

        [DataMember]
        public string DatabaseIds { get; set; }

        [DataMember]
        public string UserIds { get; set; }

        [DataMember]
        public string ClientIds { get; set; }

        [DataMember]
        public string SQLInclude { get; set; }

        [DataMember]
        public string SQLExclude { get; set; }

        [DataMember]
        public string AdvancedFilters { get; set; }

        [DataMember]
        public string StartTimestamp { get; set; }

        [DataMember]
        public string EndTimestamp { get; set; }

        [DataMember]
        public string StartIndex { get; set; }

        [DataMember]
        public string RowCount { get; set; }

        [DataMember]
        public string SortBy{ get; set; }

        [DataMember]
        public string SortOrder { get; set; }
        
        [DataMember]
        public string QuerySignatureId { get; set; }

        [DataMember]
        public string EventTypeId { get; set; }
    }
}
