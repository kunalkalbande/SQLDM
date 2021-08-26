using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Analyzer;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.State;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Metrics
{
    public class SnapshotMetricOptions : BaseOptions
    {
        public int TotalMinutes { get; set; }
        public bool ProductionServer { get; set; }
        public bool OLTPServer { get; set; }

        public RealTimeRequest RealTime { get; set; }
        public bool IsRealTime { get { return (RealTimeRequest.None != RealTime); } }

        public SnapshotMetricOptions(AnalysisState state) : base(state) { }
    }
}
