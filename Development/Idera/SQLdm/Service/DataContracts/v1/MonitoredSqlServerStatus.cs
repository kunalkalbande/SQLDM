using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class MonitoredSqlServerStatus
    {
        [DataMember]
        public string Severity;

        [DataMember]
        public MonitoredSqlServerCollection SqlServerCollection { 
            get 
            { 
                return new MonitoredSqlServerCollection(this.monitoredSqlServerList); 
            } 
        }

        public IList<MonitoredSqlServer> monitoredSqlServerList;

        public MonitoredSqlServerStatus()
        {
            this.monitoredSqlServerList = new List<MonitoredSqlServer>();
        }
    }
}
