using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdm.Service.Core.Enums
{
    // SQLdm 9.0 (Abhishek Joshi) -- WebUI Query View Filter - purpose of this enum is to assign sequential metric IDs for all the supported metrics
    internal enum MetricType
    {
        DurationInMilliSeconds = 1,
        CPUTimeInMilliSeconds = 2,
        Reads = 3,
        Writes = 4,
        InputOutput = 5,
        BlockingDurationInMilliSeconds = 6,
        WaitDurationInMilliSeconds = 7,
        Deadlocks = 8
    }
}
