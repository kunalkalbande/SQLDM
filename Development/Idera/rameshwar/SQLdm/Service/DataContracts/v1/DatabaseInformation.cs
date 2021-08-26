using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - database response parameters
    [DataContract]
    public class DatabaseInformation
    {

         [DataMember]
         public int DatabaseID { get; set; }

         [DataMember]
         public string DatabaseName { get; set; }

         [DataMember]
         public bool IsSystemDatabase { get; set; }

    }
}
