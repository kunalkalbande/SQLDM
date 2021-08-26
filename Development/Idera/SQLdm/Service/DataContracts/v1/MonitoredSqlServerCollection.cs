using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [CollectionDataContract]    
    public class MonitoredSqlServerCollection: List<MonitoredSqlServer>
    {
        public MonitoredSqlServerCollection() : base() { }
        internal MonitoredSqlServerCollection(IEnumerable<MonitoredSqlServer> c)
            : base()
        {
            if (c == null) return;
            foreach (var i in c) this.Add(i);
        }
    }    
}
