using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - clients response parameters
    [DataContract]
    public class Client
    {

         [DataMember]
         public int ClientID { get; set; }

         [DataMember]
         public string ClientName { get; set; }

    }
}
