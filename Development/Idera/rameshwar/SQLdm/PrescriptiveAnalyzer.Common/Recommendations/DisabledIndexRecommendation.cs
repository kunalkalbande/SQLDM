using System;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DisabledIndexRecommendation : IndexRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IUndoMessageGenerator
    {
        public DisabledIndexRecommendation(SqlConnection conn, string db, string schema, string table, string name)
            : base(RecommendationType.DisabledIndex, db, schema, table, name)
        {
            RecommendationIndex.GetIndexProperties(conn);
        }
        public DisabledIndexRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DisabledIndex, recProp)
        {
            
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
