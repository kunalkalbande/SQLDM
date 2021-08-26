using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Idera.SQLdoctor.Common.Services
{
    [Serializable]
    public enum Category
    {
        [Description("Workload Capture")]
        WorkloadCapture,
        [Description("Index")]
        WorkloadIndexAnalysis,
        [Description("Best Practices")]
        WorkloadBestPractices,
        [Description("CPU")]
        Processor,
        Memory,
        Network,
        [Description("Index")]
        Index,
        IO
    }
}
