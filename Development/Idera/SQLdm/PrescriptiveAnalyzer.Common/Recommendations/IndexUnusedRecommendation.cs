using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexUnusedRecommendation : IndexRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider,IUndoMessageGenerator
    {
        public readonly long Writes;
        public readonly double DaysNotUsed;
        public readonly double WritesPerSecond;

        public IndexUnusedRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.IndexUnused, recProp)
        {
            Writes = recProp.GetLong("Writes");
            DaysNotUsed = recProp.GetDouble("DaysNotUsed");
            WritesPerSecond = recProp.GetDouble("WritesPerSecond");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Writes", Writes.ToString());
            prop.Add("DaysNotUsed", DaysNotUsed.ToString());
            prop.Add("WritesPerSecond", WritesPerSecond.ToString());
            return prop;
        }

        public IndexUnusedRecommendation(SqlConnection conn, string db, string schema, string table, string name, long writes, double days)
            : base(RecommendationType.IndexUnused, db, schema, table, name)
        {
            TimeSpan ts = TimeSpan.FromDays(days);
            Writes = writes;
            DaysNotUsed = Math.Round(days, 2);
            if (0 == ts.TotalSeconds)
            {
                WritesPerSecond = 0;
            }
            else
            {
                WritesPerSecond = Math.Round(Writes / ts.TotalSeconds, 2);
            }
            RecommendationIndex.GetIndexProperties(conn);
        }

        public long DaysSinceUsed { get { return (long)DaysNotUsed; } }

        public override int AdjustConfidenceFactor(int i)
        {
            return base.AdjustConfidenceFactor(Convert.ToInt32(Math.Round(DaysSinceUsed * .3)));
        }

        public override int AdjustImpactFactor(int i)
        {
            if (DaysNotUsed > 0)
            {
                double writesPerDay = Writes / DaysNotUsed;
                if (writesPerDay > 1000000) return (LOW_IMPACT + 1);
                if (writesPerDay > 50000000) return (HIGH_IMPACT);
            }
            return base.AdjustImpactFactor(i);
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DropIndexScriptGenerator(this);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DropIndexScriptGenerator(this);
        }

        public bool IsUndoScriptRunnable
        {
            get
            {
                if (null == RecommendationIndex) return false;
                if (RecommendationIndex.HasErrors) return false;
                return true;
            }
        }

        public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationScriptRunDurationUndo);
            return messages;
        }
    }
}
