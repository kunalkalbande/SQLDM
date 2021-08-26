using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    // SQLdm 9.0 (Abhishek Joshi) --WebUI Query Monitoring -adding the query columns
    [DataContract]
    public class SQLQueryColumns
    {
        [DataMember]
        public string Database { get; set; }

        [DataMember]
        public string Table { get; set; }

        [DataMember]
        public string Schema { get; set; }

        [DataMember]
        public string Column { get; set; }
    }
}
