using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Core.Enums
{
    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - purpose of this enum is to assign sequential group IDs for all the supported groups
    internal enum GroupType
    {
        Application = 1,
        Database = 2,
        User = 3,
        Client = 4,
        QuerySignature = 5,
        QueryStatement = 6
    }
}
