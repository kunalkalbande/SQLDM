using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class IndexUnderutilizedRecommendation : IndexRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IUndoMessageGenerator
    {
        public readonly long Reads;
        public readonly long Writes;
        public readonly double WritesPerSecond;
        public readonly decimal WritesPerRead;
        public readonly double DaysNotUsed;

        public IndexUnderutilizedRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.IndexUnderutilized, recProp)
        {
            Reads = recProp.GetLong("Reads");
            Writes = recProp.GetLong("Writes");
            WritesPerSecond = recProp.GetDouble("WritesPerSecond");
            WritesPerRead = recProp.GetDecimal("WritesPerRead");
            DaysNotUsed = recProp.GetDouble("DaysNotUsed");
        }

        public IndexUnderutilizedRecommendation(SqlConnection conn, string db, string schema, string table, string name, long reads, long writes, double writesPerSec, decimal writesPerRead, double days)
            : base(RecommendationType.IndexUnderutilized, db, schema, table, name)
        {
            Reads = reads;
            Writes = writes;
            WritesPerRead = Math.Round(writesPerRead, 2);
            WritesPerSecond = Math.Round(writesPerSec, 2);
            DaysNotUsed = Math.Round(days, 2);
            RecommendationIndex.GetIndexProperties(conn);
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Reads", Reads.ToString());
            prop.Add("Writes", Writes.ToString());
            prop.Add("WritesPerSecond", WritesPerSecond.ToString());
            prop.Add("WritesPerRead", WritesPerRead.ToString());
            prop.Add("DaysNotUsed", DaysNotUsed.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // Per Brett:
            //                 
            //   Low - updates per sec >=2 <=25/sec ave; 
            //   Med - updates per sec > 25; 
            //   There is no high impact for this recomendation as no existing index (no matter how bad) can single-handedly drag down a server
            // 
            //if (WritesPerSecond  > 25) return (LOW_IMPACT + 1);

            //----------------------------------------------------------------------------
            // (Revised) Per Brett:
            //           
            //  The impact factor and confidence factor are variable. The impact is low but 
            //  it's medium if the index table is greater than 1 milion user updates per 24h 
            //  and it is high if the index table is greater than 50 million user updates per 24h. 
            //  The confidence starts at 0 + 3% per 24/h that SQL Server has been up. 1 month up = 100% confidence. 
            //

            if (DaysNotUsed > 0)
            {
                double writesPerDay = Writes / DaysNotUsed;
                if (writesPerDay > 1000000) return (LOW_IMPACT + 1);
                if (writesPerDay > 50000000) return (HIGH_IMPACT);
            }
            return base.AdjustImpactFactor(i);
        }

        public override int AdjustConfidenceFactor(int i)
        {
            return base.AdjustConfidenceFactor(Convert.ToInt32(Math.Round(DaysNotUsed * .3)));
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
