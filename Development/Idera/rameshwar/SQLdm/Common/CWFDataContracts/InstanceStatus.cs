using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Common.CWFDataContracts
{
    public enum InstanceStatus
    {
        
        Discovered = 0,
        
        Managed = 1,
        
        Unmanaged = 2,
        
        Ignored = 3,
        
        Archived = 4,
        
        Unsupported = 5
    }
}
