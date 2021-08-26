using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Database does not have auto create or update stats on
    /// </summary>
    [Serializable]
    public class AutoStatsRecommendation : Recommendation, IScriptGeneratorProvider, IProvideDatabase, IUndoScriptGeneratorProvider
    {
        public readonly string _database;
        public readonly bool CreateStats;
        public readonly bool UpdateStats;
        public string Database { get { return _database; } }

        public AutoStatsRecommendation(RecommendationProperties lstProperties)
            : base(RecommendationType.AutoStats, lstProperties)
        {
            _database = lstProperties.GetString("Database");
            CreateStats = lstProperties.GetBool("CreateStats");
            UpdateStats = lstProperties.GetBool("UpdateStats");
        }

        public AutoStatsRecommendation(string db, bool createStats, bool updateStats)
            : base(RecommendationType.AutoStats)
        {
            _database = db;
            CreateStats = createStats;
            UpdateStats = updateStats;
        }

        public override Dictionary<string, string> GetProperties()
        {
            Dictionary<string, string> lstProperties = base.GetProperties();
            lstProperties.Add("Database", Database);
            lstProperties.Add("CreateStats", FormatHelper.FormatBoolToString(CreateStats));
            lstProperties.Add("UpdateStats", FormatHelper.FormatBoolToString(UpdateStats));
            return lstProperties;
        }
        public IScriptGenerator GetScriptGenerator()
        {
            return new AutoStatisticsScriptGenerator(Database, CreateStats, UpdateStats);
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new AutoStatisticsScriptGenerator(Database, CreateStats, UpdateStats);
        }

        public bool IsUndoScriptRunnable { get { return (true); } }
    }
}
