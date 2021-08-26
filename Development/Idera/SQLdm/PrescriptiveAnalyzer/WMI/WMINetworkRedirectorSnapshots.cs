using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMINetworkRedirectorSnapshots : List<WMINetworkRedirector>
    {
        public UInt32 NetworkErrorsPerSec 
        {
            get
            {
                if (this.Count <= 1) return (0);
                return (this.Last().NetworkErrorsPerSec - this[0].NetworkErrorsPerSec);
            }
        }
    }
}
