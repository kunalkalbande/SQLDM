using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Values;
using Idera.SQLdoctor.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Services
{
    public interface IOptimizationEngine
    {
        void Cancel();
        void Wait();
        bool IsRunning();
    }
}
