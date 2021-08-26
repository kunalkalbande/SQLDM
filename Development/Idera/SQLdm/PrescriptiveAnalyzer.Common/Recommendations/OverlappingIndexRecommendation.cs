﻿using System;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class OverlappingIndexRecommendation : IndexRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IUndoMessageGenerator
    {
        public readonly long Usage;
        public readonly long Updates;
        public readonly string OverlappingIndexName;
        public readonly long DupUsage;
        public readonly long DupUpdates;
        public readonly double ServerUpDays;

        public double UpdatesPerDay { get { return (Math.Round(ServerUpDays > 0 ? DupUpdates / ServerUpDays : DupUpdates, 1)); } }
        public OverlappingIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.OverlappingIndexes, recProp)
        {
            ServerUpDays = recProp.GetDouble("ServerUpDays");
            Usage = recProp.GetLong("Usage");
            Updates = recProp.GetLong("Updates");
            OverlappingIndexName = recProp.GetString("OverlappingIndexName");
            DupUsage = recProp.GetLong("DupUsage");
            DupUpdates = recProp.GetLong("DupUpdates");
            RecommendationIndex.IndexName = recProp.GetString("dupName");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ServerUpDays", ServerUpDays.ToString());
            prop.Add("Usage", Usage.ToString());
            prop.Add("Updates", Updates.ToString());
            prop.Add("OverlappingIndexName", OverlappingIndexName.ToString());
            prop.Add("DupUsage", DupUsage.ToString());
            prop.Add("DupUpdates", DupUpdates.ToString());
            prop.Add("dupName", RecommendationIndex.IndexName);
            return prop;
        }

        public OverlappingIndexRecommendation(SqlConnection conn,
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
            : base(RecommendationType.OverlappingIndexes, db, schema, table, name)
        {
            ServerUpDays = serverUpDays;
            Usage = usage;
            Updates = updates;
            OverlappingIndexName = dupName;
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

        public IScriptGenerator GetScriptGenerator()
        {
            return new DropIndexScriptGenerator(this, OverlappingIndexName);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DropIndexScriptGenerator(this, OverlappingIndexName);
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
