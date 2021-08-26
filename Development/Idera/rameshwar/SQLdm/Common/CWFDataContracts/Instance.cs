using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    public class Instance
    {
        
        public string Name { get; set; }

        
        public DateTime? UtcFirstSeen { get; set; }

        
        public string UtcLastSeen { get; set; }

        
        public string Version { get; set; }

        
        public string Edition { get; set; }

        
        public string Owner { get; set; }

        
        public string Location { get; set; }

        public InstanceStatus InstanceStatus { get; set; }

        public string Comments { get; set; }

        public Products Products { get; set; }
    }
}
