using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    [DataContract]
    public class Categories
    {
        [DataMember]
        public int MaxSeverity { get; set; }

        [DataMember]
        public string name { get; set; }        
    }
}
