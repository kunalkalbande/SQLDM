using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Collectors;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.TraceEvents;

namespace Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats
{
    public class TraceEventStats
    {
        private readonly double _estimatedCost = 0.0;
        private readonly TEBase _high = null;
        private readonly UInt64 _totalCount = UInt64.MinValue;
        private readonly UInt64 _totalDuration = UInt64.MinValue;
        private readonly TraceEventPlan _tep = null;
        private readonly ShowPlanXML _plan = null;
        public double EstimatedCost { get { return (_estimatedCost); } }
        public ShowPlanXML Plan { get { return (_plan); } }
        public Exception PlanError { get {return((null == _tep) ? null : _tep.Error);} }
        public string XmlPlan { get { return ((null == _tep) ? null : _tep.XmlPlan); } }
        public TEBase High { get { return (_high); } }
        public UInt64 TotalCount { get { return (_totalCount); } }
        public UInt64 TotalDuration { get { return (_totalDuration); } }
        public bool TraceCollectedEvent { get; private set; }
        private TraceEventStats() { }
        public TraceEventStats(SqlSystemObjectManager ssom, DataBucket d, TraceEventPlan tep) 
        {
            if (null == d) return;
            TraceCollectedEvent = d.TraceCollectedEvent;
            _high = d.HighDuration;
            _totalCount = d.TotalCount;
            _totalDuration = d.TotalDuration;
            _tep = tep;
            if (null != _tep)
            {
                _plan = _tep.Plan;
                if (null != _plan)
                {
                    ExecutionPlanCost cost = new ExecutionPlanCost(ssom);
                    cost.CalculateCost(_plan, tep.Event.DBID);
                    _estimatedCost = d.TotalCount * cost.TotalCost;
                }
            }
        }

        internal bool IsWorstTSQL()
        {
            return (!TraceCollectedEvent);
        }
    }
    public class TraceEventStatsEqualityComparer : IEqualityComparer<TraceEventStats>
    {
        public bool Equals(TraceEventStats x, TraceEventStats y)
        {
            return (x.High.TextNormalized.Equals(y.High.TextNormalized));
        }

        public int GetHashCode(TraceEventStats obj)
        {
            return (obj.High.TextNormalized.GetHashCode());
        }
    }
    public class TraceEventStatsCostComparer : IComparer<TraceEventStats>
    {
        public int Compare(TraceEventStats x, TraceEventStats y)
        {
            return (y.EstimatedCost.CompareTo(x.EstimatedCost));
        }
    }
}
