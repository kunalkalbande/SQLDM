using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{ 
    [Serializable]
    public class WaitStatsThreadPoolRecommendation : WaitStatsRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IMessageGenerator, IUndoMessageGenerator
    {
        public UInt64 CurrentMaxWorkerThreads { get; private set; }
        public UInt64 SuggestedMaxWorkerThreads { get; private set; }

        public WaitStatsThreadPoolRecommendation(AffectedBatches ab, UInt64 current, UInt64 suggested)
            : base(ab, RecommendationType.WaitStatsThreadPool)
        {
            CurrentMaxWorkerThreads = current;
            SuggestedMaxWorkerThreads = suggested;
        }

        public WaitStatsThreadPoolRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.WaitStatsThreadPool, recProp)
        {
            CurrentMaxWorkerThreads = recProp.GetUInt64("CurrentMaxWorkerThreads");
            SuggestedMaxWorkerThreads = recProp.GetUInt64("SuggestedMaxWorkerThreads");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("CurrentMaxWorkerThreads", CurrentMaxWorkerThreads.ToString());
            prop.Add("SuggestedMaxWorkerThreads", SuggestedMaxWorkerThreads.ToString());
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ConfigureMaxWorkerThreadsScriptGenerator(CurrentMaxWorkerThreads, SuggestedMaxWorkerThreads);
        }

        public bool IsScriptRunnable { get { return true; } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ConfigureMaxWorkerThreadsScriptGenerator(CurrentMaxWorkerThreads, SuggestedMaxWorkerThreads);
        }

        public bool IsUndoScriptRunnable { get { return true; } }

        new public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessageUndo);
            return messages;
        }

        new public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connection));
            messages.Add(Properties.Resources.RecommendationRequiresRestartMessage);
            return messages;
        }
    }
}
