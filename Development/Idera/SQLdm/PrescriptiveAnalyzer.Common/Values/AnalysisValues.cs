using System;
using System.Collections.Generic;
using System.Text;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Values
{
    [Serializable]
    public class AnalysisValues
    {
        public SnapshotValues SnapshotValues { get; set; }
        public SnapshotMetrics SnapshotMetrics { get; set; }

        public bool RanWorkloadAnalysis { get; set; }
        public bool RanSnapshotAnalysis { get; set; }
        public bool RanDBObjectAnalysis { get; set; }

        public bool AdHocBatchAnalysis { get; set; }

        public string TargetCategory { get; set; }
        private List<string> _targetCategories;
        public List<string> TargetCategories
        {
            get
            {
                if (null == _targetCategories) return (null);
                return (new List<string>(_targetCategories));
            }
            set
            {
                if (null == value) { _targetCategories = null; return; }
                _targetCategories = new List<string>(value);
            }
        }
    }
}
