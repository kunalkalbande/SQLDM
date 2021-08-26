using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class BaseMetrics
    {
        bool _isDataValid = true;
        public bool IsDataValid
        {
            get { return _isDataValid; }
            set { _isDataValid = value; }
        }

        public virtual void AddSnapshot(Idera.SQLdm.Common.Snapshots.PrescriptiveAnalyticsSnapshot snapshot)
        {
        }
    }
}
