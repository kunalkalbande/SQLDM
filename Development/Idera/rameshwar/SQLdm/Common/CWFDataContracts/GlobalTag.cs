using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    [Serializable]
    [DataContract]    
    public class GlobalTag
    {
        
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int ID { get; set; }

        [DataMember]
        public List<string> Instances { get; set; }

   
    }
}
