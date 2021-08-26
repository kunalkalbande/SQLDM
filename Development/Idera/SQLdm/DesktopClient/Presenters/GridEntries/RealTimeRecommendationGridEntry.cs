using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.DesktopClient.Presenters.GridEntries
{
    internal class RealTimeRecommendationGridEntryCollection : List<RealTimeRecommendationGridEntry>
    {
        internal RealTimeRecommendationGridEntryCollection() : base() { }
        internal RealTimeRecommendationGridEntryCollection(IEnumerable<IRecommendation> recs)
            : base()
        {
            if (null == recs) return;
            foreach (IRecommendation r in recs)
            {
                Add(new RealTimeRecommendationGridEntry(r));
            }
        }
    }
    internal class RealTimeRecommendationGridEntry
    {
        public string Finding { get { return ((null == _r) ? "" : _r.FindingText); } }
        private IRecommendation _r;

        internal RealTimeRecommendationGridEntry() { }
        internal RealTimeRecommendationGridEntry(IRecommendation r) { _r = r; }

        internal IRecommendation GetRecommendation() { return (_r); }
    }
}
