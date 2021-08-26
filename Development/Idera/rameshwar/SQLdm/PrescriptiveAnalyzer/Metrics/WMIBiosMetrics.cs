using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class WMIBiosMetrics
    {
        public bool IsVMWare { get; set; }
        public bool IsHyperV { get; set; }
        public bool IsXen { get; set; }

        public bool IsVirtualMachine { get; set; }
    }
}
