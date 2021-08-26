using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class NonClusteredMatchingClusteredIndexRecommendation : IndexRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IUndoMessageGenerator
    {
        public string ClusteredIndexName { get {return(IndexName);} }
        public readonly long ClusteredUsage;
        public readonly long ClusteredUpdates;

        public readonly string NonClusteredIndexName;
        public readonly long NonClusteredUsage;
        public readonly long NonClusteredUpdates;

        public readonly double ServerUpDays;

        public double UpdatesPerDay { get { return (Math.Round(ServerUpDays > 0 ? NonClusteredUpdates / ServerUpDays : NonClusteredUpdates, 1)); } }
        public double UsagePerDay { get { return (Math.Round(ServerUpDays > 0 ? NonClusteredUsage / ServerUpDays : NonClusteredUsage, 1)); } }

        public double NonClusteredUpdatesPerDay { get { return (UpdatesPerDay); } }
        public double NonClusteredUsagePerDay { get { return (UsagePerDay); } }

        public double ClusteredUpdatesPerDay { get { return (Math.Round(ServerUpDays > 0 ? ClusteredUpdates / ServerUpDays : ClusteredUpdates, 1)); } }
        public double ClusteredUsagePerDay { get { return (Math.Round(ServerUpDays > 0 ? ClusteredUsage / ServerUpDays : ClusteredUsage, 1)); } }

        public NonClusteredMatchingClusteredIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.NonClusteredMatchingClusteredIndex, recProp)
        {
            ServerUpDays = recProp.GetDouble("ServerUpDays");
            ClusteredUsage = recProp.GetLong("ClusteredUsage");
            ClusteredUpdates = recProp.GetLong("ClusteredUpdates");
            NonClusteredIndexName = recProp.GetString("NonClusteredIndexName");
            NonClusteredUsage = recProp.GetLong("NonClusteredUsage");
            NonClusteredUpdates = recProp.GetLong("NonClusteredUpdates");
            RecommendationIndex.IndexName = NonClusteredIndexName;
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ServerUpDays", ServerUpDays.ToString());
            prop.Add("ClusteredUsage", ClusteredUsage.ToString());
            prop.Add("ClusteredUpdates", ClusteredUpdates.ToString());
            prop.Add("NonClusteredIndexName", NonClusteredIndexName.ToString());
            prop.Add("NonClusteredUsage", NonClusteredUsage.ToString());
            prop.Add("NonClusteredUpdates", NonClusteredUpdates.ToString());
            return prop;
        }

        public NonClusteredMatchingClusteredIndexRecommendation(SqlConnection conn,
                                                        string db, 
                                                        string schema, 
                                                        string table,
                                                        double serverUpDays,
                                                        string clusteredIndexName, 
                                                        long clusteredUsage,
                                                        long clusteredUpdates,
                                                        string nonClusteredIndexName, 
                                                        long nonClusteredUsage,
                                                        long nonClusteredUpdates
                                                        )
            : base(RecommendationType.NonClusteredMatchingClusteredIndex, db, schema, table, clusteredIndexName)
        {
            ServerUpDays = serverUpDays;
            ClusteredUsage = clusteredUsage;
            ClusteredUpdates = clusteredUpdates;

            NonClusteredIndexName = nonClusteredIndexName;
            NonClusteredUsage = nonClusteredUsage;
            NonClusteredUpdates = nonClusteredUpdates;
            RecommendationIndex.IndexName = nonClusteredIndexName; // Point the index object to the duplicate before calling GetIndexProperties
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

        public IScriptGenerator GetScriptGenerator()
        {
            return new DropIndexScriptGenerator(this, NonClusteredIndexName);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DropIndexScriptGenerator(this, NonClusteredIndexName);
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
