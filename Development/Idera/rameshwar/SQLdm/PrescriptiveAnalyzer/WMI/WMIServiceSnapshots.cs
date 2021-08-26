using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.AnalysisEngine.Snapshot.WMI
{
    internal class WMIServiceSnapshots : List<WMIService>
    {
        internal bool IsRunning
        {
            get
            {
                if (this.Count <= 0) return (false);
                return (this.Last().IsRunning);
            }
        }

        internal string Name
        {
            get
            {
                if (this.Count <= 0) return (string.Empty);
                return (this[0].Name);
            }
        }
    }
}
