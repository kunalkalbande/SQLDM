using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.CollectionService.Probes
{
    public interface IIntervalProbe
    {
        DateTime NextTimeToExecute { get; }
        int Interval { get; }
        int MaxCount { get;  }
        bool IsInterval { get; }
    }
}
