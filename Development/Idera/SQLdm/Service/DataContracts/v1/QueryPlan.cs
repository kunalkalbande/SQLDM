using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
	// SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - Query plan response parameters
    [DataContract]
    public class QueryPlan
    {
        [DataMember]
        public int InstanceID { get; set; }

        [DataMember]
        public int SQLStatementID { get; set; }

        [DataMember]
        public int PlanID { get; set; }

        [DataMember]
        public string PlanXML { get; set; }

        [DataMember]
        public List<SQLQueryColumns> QueryColumns { get; set; }

		//SQLdm 10.0 - Display estimated query plan - added a flag to find if the query plan is actual or not
        [DataMember]
        public bool IsActualPlan { get; set; }

    }
}
