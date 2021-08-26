using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// use [Database]
    /// update statistics [schema].[table]([stats]);
    /// </summary>
    [Serializable]
    public class OutOfDateStatsRecommendation : IndexRecommendation, IScriptGeneratorProvider 
    {
        public readonly UInt64 RowCount;
        public readonly UInt64 ModCount;
        public readonly UInt64 HoursSinceUpdated;
        public readonly DateTime StatsDate;

        public OutOfDateStatsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.OutOfDateStats, recProp)
        {
            RowCount = recProp.GetUInt64("RowCount");
            ModCount = recProp.GetUInt64("ModCount");
            HoursSinceUpdated = recProp.GetUInt64("HoursSinceUpdated");
            StatsDate = recProp.GetDateTime("StatsDate");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("RowCount", RowCount.ToString());
            prop.Add("ModCount", ModCount.ToString());
            prop.Add("HoursSinceUpdated", HoursSinceUpdated.ToString());
            prop.Add("StatsDate", StatsDate.ToString());
            return prop;
        }

        public OutOfDateStatsRecommendation(string db, string schema, string table, string name, UInt64 rowCount, UInt64 modCount, DateTime statsDate, UInt64 hoursSinceUpdated)
            : base(RecommendationType.OutOfDateStats, db, schema, table, name)
        {
            RowCount = rowCount;
            ModCount = modCount;
            StatsDate = statsDate;
            HoursSinceUpdated = hoursSinceUpdated;
        }
        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // Per Brett (PR DR372):
            // Impact is variable. Normally medium but can be high 
            //    if [rowmodctr] is greater than 5000 
            //      and [rowmodctr] is greater than [[rowcnt]/3] 
            //      and [StatsDate] is greater than 48 hours.
            // 
            if ((ModCount > 5000) && (ModCount > (RowCount/3)) && (HoursSinceUpdated > 48))
            {
                return (HIGH_IMPACT);
            }
            return (LOW_IMPACT + 1);
        }



        public IScriptGenerator GetScriptGenerator()
        {
            return new UpdateStatisticsScriptGenerator(this);
        }

        public bool IsScriptRunnable { get { return (true); } }
    }
}
