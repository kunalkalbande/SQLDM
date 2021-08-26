using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Category
{
    [DataContract]
    public class FileDrivesForInstance
    {
        [DataMember]
        public string DriveName { get; set; }             
    }
}
