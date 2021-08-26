using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    /// <summary>
    /// SQLdm10.2(srishti purohit) - Class to returned from GetQueryWaitStatisticsForInstanceOverview API for instance overview page in web UI
    /// SQLdm-28027 defect fix
    /// </summary>
    [DataContract]
    public class QueryWaitStatisticsForInstanceOverview
    {
        private string waitCategory = "";
        [DataMember]
        public DateTime StatementUTCStartTime { get; set; }
        
        [DataMember]
        public int WaitCategoryID { get; set; }

        [DataMember]
        public string WaitCategory
        {
            get { return waitCategory; }
            set { waitCategory = value; }
        }

        [DataMember]
        public decimal WaitDurationPerSecond { get; set; }        
    }
}
