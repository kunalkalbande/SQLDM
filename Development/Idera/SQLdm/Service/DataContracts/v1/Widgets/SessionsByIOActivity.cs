using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Idera.SQLdm.Service.DataContracts.v1.Widgets
{
    //SQLdm 9.1 (Sanjali Makkar):  For Top X API : Sessions by I/O Activity
    [DataContract()]
    public class SessionsByIOActivity
    {
        [DataMember]
        public string InstanceName { get; set; }

        [DataMember]
        public long SessionID { get; set; }

        [DataMember]
        public DateTime UTCCollectionDateTime { get; set; }

        //Sessions will be compared on the basis of Physical IO
        [DataMember]
        public long? PhysicalIO { get; set; }

    }
}
