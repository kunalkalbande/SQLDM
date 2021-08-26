using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class MissingIndexRecommendation : MissingIndexBaseRecommendation, IProvideAffectedBatches
    {
        private readonly List<MissingIndexCost> _costs = new List<MissingIndexCost>();

        public ICollection<MissingIndexCost> Costs { get { return (_costs); } }
        public int AffectedBatchesCount { get { return (null == _costs ? 0 : _costs.Count); } }
        private int _traceDurationMinutes = 0;
        private UInt64 _totalQueryDurationMicroSec = 0;

        public MissingIndexRecommendation(string db, string schema, string table, IEnumerable<MissingIndexCost> costs, IEnumerable<RecommendedIndex> recommendedIndexes, double tableUpdatesPerSec, double tableUpdatesPerMin, int traceDurationMinutes, UInt64 totalQueryDurationMS)
            : base(RecommendationType.MissingIndex, db, schema, table, recommendedIndexes, tableUpdatesPerSec, tableUpdatesPerMin)
        {
            _traceDurationMinutes = traceDurationMinutes;
            _totalQueryDurationMicroSec = totalQueryDurationMS;
            if (null != costs) _costs.AddRange(costs);
        }

        public MissingIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.MissingIndex, recProp)
        {
            _costs = recProp.GetMissingIndexCost("_costs");
            _traceDurationMinutes = recProp.GetInt("_traceDurationMinutes");
            _totalQueryDurationMicroSec = recProp.GetUInt64("_totalQueryDurationMicroSec");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("_costs", RecommendationProperties.GetXml<List<MissingIndexCost>>(_costs));
            prop.Add("_traceDurationMinutes", _traceDurationMinutes.ToString());
            prop.Add("_totalQueryDurationMicroSec", _totalQueryDurationMicroSec.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            try
            {
                //----------------------------------------------------------------------------
                // If the total query duration for the tsql related to this recommendation is
                // over 5% of the total analysis duration, this should be a high-impact recommendation.
                // 
                UInt64 durationMicroSec = ((Convert.ToUInt64(_traceDurationMinutes) * 60) * 1000000);
                if ((durationMicroSec * .05) <= _totalQueryDurationMicroSec) return (HIGH_IMPACT);
            }
            catch { }
            return (LOW_IMPACT + 1);
        }

        public AffectedBatches GetAffectedBatches()
        {
            AffectedBatches ab = new AffectedBatches();
            if (null == Costs) return (ab);
            foreach (MissingIndexCost mic in Costs)
            {
                ab.Add(new AffectedBatch(mic.ToString(), mic.Batch));
            }
            return (ab);
        }
    }
}
