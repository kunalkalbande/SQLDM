using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - users response parameters
    [DataContract]
    public class User
    {

         [DataMember]
         public int UserID { get; set; }

         [DataMember]
         public string UserName { get; set; }

    }
}
