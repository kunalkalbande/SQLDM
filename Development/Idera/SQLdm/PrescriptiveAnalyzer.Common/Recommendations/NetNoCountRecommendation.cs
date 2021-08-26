using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    /// <summary>
    /// Network recommendation report #79b
    /// </summary>
    [Serializable]
    public class NetNoCountRecommendation : NetworkRecommendation, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IMessageGenerator
    {
        public UInt32 StmtsWithRows { get; set; }
        public UInt32 StmtsWithNoCountOn { get; set; }
        public UInt64 BatchReqSec { get; set; }

        public NetNoCountRecommendation() : base(RecommendationType.NetNoCount, "SDR-N7") { }

        public NetNoCountRecommendation(UInt32 stmtsWithRows, UInt32 stmtsWithNoCountOn, UInt64 batchReqSec)
            : this()
        {
            StmtsWithRows = stmtsWithRows;
            StmtsWithNoCountOn = stmtsWithNoCountOn;
            BatchReqSec = batchReqSec;
        }

        public NetNoCountRecommendation(RecommendationProperties recProp) :
            base(RecommendationType.NetNoCount, "SDR-N7", recProp)
        {
            StmtsWithRows = recProp.GetUInt32("StmtsWithRows");
            StmtsWithNoCountOn = recProp.GetUInt32("StmtsWithNoCountOn");
            BatchReqSec = recProp.GetUInt64("BatchReqSec");
        }


        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("StmtsWithRows", StmtsWithRows.ToString());
            prop.Add("StmtsWithNoCountOn", StmtsWithNoCountOn.ToString());
            prop.Add("BatchReqSec", BatchReqSec.ToString());
            return prop;
        }

        public override int AdjustImpactFactor(int i)
        {
            double noCount = (StmtsWithNoCountOn * 100.0) / StmtsWithRows;
            if (BatchReqSec > 500)
            {
                if (noCount < 50.0) return ((noCount < 30.0) ? HIGH_IMPACT : LOW_IMPACT + 1);
            }
            else
            {
                if (noCount < 50.0) return (LOW_IMPACT + 1);
            }
            return (base.AdjustImpactFactor(i));
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new SetNoCountOnScriptGenerator();
        }

        public bool IsScriptRunnable { get { return (true); } }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new SetNoCountOnScriptGenerator();
        }

        public bool IsUndoScriptRunnable { get { return (true); } }

        List<string> IMessageGenerator.GetMessages(RecommendationOptimizationStatus res, SqlConnection connectionInfo)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connectionInfo));
            messages.Add(Properties.Resources.RecommendationCanBreakADO);
            return messages;
        }
    }
}
