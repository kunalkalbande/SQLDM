using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1
{
    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - supported grouping response parameters
    [DataContract]
    public class SupportedGrouping
    {

        public SupportedGrouping(int groupId, string groupName)
        {
            this.GroupId = groupId;
            this.GroupName = groupName;
        }

        [DataMember]
        public int GroupId { get; set; }

        [DataMember]
        public string GroupName { get; set; }

    }
}
