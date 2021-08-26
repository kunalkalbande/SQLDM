using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class TempdbSummaryStatistics
    {
        [DataMember]
        public string userObjectsMegabytes;
        [DataMember]
        public string internalObjectsMegabytes;
        [DataMember]
        public string versionStoreMegabytes;
        [DataMember]
        public string mixedExtentsMegabytes;
        [DataMember]
        public string unallocatedSpaceMegabytes;

        [DataMember]
        public string filesize;
    }
    
}
