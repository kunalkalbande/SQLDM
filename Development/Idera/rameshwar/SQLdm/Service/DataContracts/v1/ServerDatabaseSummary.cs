using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class ServerDatabaseSummary 
    {
        [DataMember]
        public int databaseCount;
        [DataMember]
        public int dataFileCount;
        [DataMember]
        public int logFileCount;
        [DataMember]
        public decimal dataFileSpaceAllocated;
        [DataMember]
        public decimal dataFileSpaceUsed;
        [DataMember]
        public decimal logFileSpaceAllocated;
        [DataMember]
        public decimal logFileSpaceUsed;
    }
}
