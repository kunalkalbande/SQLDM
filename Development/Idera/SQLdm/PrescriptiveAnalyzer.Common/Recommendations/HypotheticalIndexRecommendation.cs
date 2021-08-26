using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class HypotheticalIndexRecommendation : IndexRecommendation, IScriptGeneratorProvider
    {
        public HypotheticalIndexRecommendation(string db, string schema, string table, string name)
            : base(RecommendationType.HypotheticalIndex, db, schema, table, name)
        {
        }

        public HypotheticalIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.HypotheticalIndex, recProp)
        {
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DropIndexScriptGenerator(this);
        }

        public bool IsScriptRunnable { get { return true; } }

    }
}
