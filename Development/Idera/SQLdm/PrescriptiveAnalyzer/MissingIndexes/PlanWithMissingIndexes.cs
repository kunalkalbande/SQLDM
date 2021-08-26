using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class PlanWithMissingIndexes
    {
        private static int _staticCount = 0;

        private readonly List<MissingIndexGroupType> _missingIndexes = null;
        private readonly TraceEventStats _stats;
        private readonly int _id;

        public int ID { get { return (_id); } }
        public TraceEventStats Stats { get { return (_stats); } }
        public List<MissingIndexGroupType> MissingIndexes { get { return (_missingIndexes); } }

        private PlanWithMissingIndexes() { }
        internal PlanWithMissingIndexes(TraceEventStats stats, List<MissingIndexGroupType> missingIndexes)
        {
            _id = System.Threading.Interlocked.Increment(ref _staticCount);
            _stats = stats;
            _missingIndexes = missingIndexes;
        }
        public override int GetHashCode()
        {
            return (_id);
        }
        public override bool Equals(object obj)
        {
            PlanWithMissingIndexes a = obj as PlanWithMissingIndexes;
            if (null == a) return (false);
            return (_id.Equals(a.ID));
        }
    }
}
