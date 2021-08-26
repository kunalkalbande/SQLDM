using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class PartialDuplicateIndexRecommendation : IndexRecommendation
    {
        public readonly long Usage;
        public readonly long Updates;
        public readonly string RedundantIndexName;
        public readonly long DupUsage;
        public readonly long DupUpdates;
        public readonly double ServerUpDays;

        public double UpdatesPerDay { get { return (Math.Round(ServerUpDays > 0 ? DupUpdates / ServerUpDays : DupUpdates, 1)); } }

        public PartialDuplicateIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.PartialDuplicateIndex, recProp)
        {
            Usage = recProp.GetLong("Usage");
            Updates = recProp.GetLong("Updates");
            RedundantIndexName = recProp.GetString("RedundantIndexName");
            DupUsage = recProp.GetLong("DupUsage");
            DupUpdates = recProp.GetLong("DupUpdates");
            ServerUpDays = recProp.GetDouble("ServerUpDays");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Usage", Usage.ToString());
            prop.Add("Updates", Updates.ToString());
            prop.Add("RedundantIndexName", RedundantIndexName.ToString());
            prop.Add("DupUsage", DupUsage.ToString());
            prop.Add("DupUpdates", DupUpdates.ToString());
            prop.Add("ServerUpDays", ServerUpDays.ToString());
            return prop;
        }

        public PartialDuplicateIndexRecommendation(SqlConnection conn,
                                                        string db, 
                                                        string schema, 
                                                        string table,
                                                        double serverUpDays,
                                                        string name,
                                                        long usage,
                                                        long updates,
                                                        string dupName,
                                                        long dupUsage,
                                                        long dupUpdates
                                                        )
            : base(RecommendationType.PartialDuplicateIndex, db, schema, table, name)
        {
            ServerUpDays = serverUpDays;
            Usage = usage;
            Updates = updates;
            RedundantIndexName = dupName;
            DupUsage = dupUsage;
            DupUpdates = dupUpdates;
            RecommendationIndex.IndexName = dupName; // Point the index object to the duplicate before calling GetIndexProperties
            RecommendationIndex.GetIndexProperties(conn);
        }

        public override int AdjustImpactFactor(int i)
        {
            double u = UpdatesPerDay;
            if (u > 1000000)
            {
                if (u > 50000000) return (HIGH_IMPACT);
                return (LOW_IMPACT + 1);
            }
            return base.AdjustImpactFactor(i);
        }

        //public override int AdjustImpactFactor(int i)
        //{
        //    if (DupUpdates > 20000000)
        //    {
        //        if (DupUpdates > 100000000) return (HIGH_IMPACT);
        //        return (LOW_IMPACT + 1);
        //    }
        //    return base.AdjustImpactFactor(i);
        //}

    }
}
