using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{

    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - application response parameters
    [DataContract]
    public class Application
    {

         [DataMember]
         public int ApplicationNameID { get; set; }

         [DataMember]
         public string ApplicationName { get; set; }

    }
}
