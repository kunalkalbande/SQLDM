using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers
{
    public class RecommendationCountHelper
    {
        private Dictionary<RecommendationType, int> _countPerType = new Dictionary<RecommendationType, int>();
        private int _max = 0;

        private RecommendationCountHelper() { }
        public RecommendationCountHelper(int max) { _max = max; }
        public int Add(IRecommendation r)
        {
            if (null == r) return 0;
            return (Add(r.RecommendationType));
        }
        public int Add(RecommendationType rt)
        {
            int count = 0;
            _countPerType.TryGetValue(rt, out count);
            _countPerType[rt] = ++count;
            return (count);
        }
        public int Count(RecommendationType rt)
        {
            int count = 0;
            _countPerType.TryGetValue(rt, out count);
            return (count);
        }
        public bool Allow(IRecommendation r)
        {
            if (null == r) return false;
            return (Allow(r.RecommendationType));
        }
        public bool Allow(RecommendationType rt)
        {
            if (_max <= 0) return (true);
            int count = 0;
            if (_countPerType.TryGetValue(rt, out count))
            {
                return (count < _max);
            }
            return (true);
        }
    }
}
