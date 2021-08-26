using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Wintellect.PowerCollections;
using System.Runtime.Serialization;
using PluginCommon;

namespace Idera.SQLdm.Service.DataContracts.v1
{
     [DataContract]
    public class ServerSummaryContainer
    {
      
        [DataMember]
        public MonitoredSqlServer ServerStatus { get; set; }

        //public DataTable OverviewStatistics { get; set; }

        [DataMember]
        public ServerOverview Overview {get; set;}
        //public Pair<List<string>, DataTable> DiskDriveInfo { get; set; }

        //SQLdm 10.2 (Anshika Sharma) : Added to Show CategoryWaiseMaxAlerts
        [DataMember]
        public AlertWiseMaxSeverity AlertCategoryWiseMaxSeverity { get; set; }

        [DataMember]
        public Product Product { get; set; }

    }

    [DataContract]
    public class ServerSummaryContainerV2
    {

        [DataMember]
        public List<ServerSummaryContainer> ServerSummaryContainerList { get; set; }

        [DataMember]
        public int totalResults { get; set; }


    }
}
