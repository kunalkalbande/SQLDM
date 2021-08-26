using System;
//using Idera.SQLdoctor.Common.ScriptGenerator;
using System.Collections.Generic;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using System.Data.SqlClient;
//using Idera.SQLdoctor.Common.Configuration;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class ServerConfigurationRecommendationNoScript : Recommendation
    {
        public readonly string Configuration;
        public readonly long CurrentValue;
        public readonly long DefaultValue;

        public ServerConfigurationRecommendationNoScript(RecommendationType type, string configuration, long currentValue, long defaultValue)
            : base(type)
        {
            Configuration = configuration;
            CurrentValue = currentValue;
            DefaultValue = defaultValue;
        }

        public ServerConfigurationRecommendationNoScript(RecommendationType type, RecommendationProperties recProp)
            : base(type, recProp)
        {
            Configuration = recProp.GetString("Configuration");
            CurrentValue = recProp.GetLong("CurrentValue");
            DefaultValue = recProp.GetLong("DefaultValue");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("Configuration", Configuration.ToString());
            prop.Add("CurrentValue", CurrentValue.ToString());
            prop.Add("DefaultValue", DefaultValue.ToString());
            return prop;
        }
    }

    [Serializable]
    public class ServerConfigurationRecommendation : ServerConfigurationRecommendationNoScript, IScriptGeneratorProvider, IUndoScriptGeneratorProvider, IMessageGenerator, IUndoMessageGenerator, ITransactionLessScript
    {
        public ServerConfigurationRecommendation(RecommendationType type, string configuration, long currentValue, long defaultValue)
            : base(type, configuration,currentValue,defaultValue)
        {
        }

        public ServerConfigurationRecommendation(RecommendationType type,RecommendationProperties recProp)
            : base(type, recProp)
        {
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            return prop;
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new ServerConfigurationScriptGenerator(Configuration,DefaultValue);
        }

        public bool IsScriptRunnable
        {
            get { return (true); }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new ServerConfigurationScriptGenerator(Configuration, CurrentValue);
        }

        public bool IsUndoScriptRunnable
        {
            get { return (true); }
        }

        new public List<string> GetUndoMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetUndoMessages(res, connection));
            if (RecommendationType.LockConfiguration == RecommendationType)
            {
                messages.Add(Properties.Resources.RecommendationRequiresRestartMessageUndo);
            }
            return messages;
        }

        new public List<string> GetMessages(RecommendationOptimizationStatus res, SqlConnection connection)
        {
            List<string> messages = new List<string>();
            messages.AddRange(base.GetMessages(res, connection));
            if (RecommendationType.LockConfiguration == RecommendationType)
            {
                messages.Add(Properties.Resources.RecommendationRequiresRestartMessage);
            }
            return messages;
        }

        /// <summary>
        /// Implemented ITransactionLessScript to support opti/undo scripts to run without transactions
        /// </summary>
        public bool IsScriptTransactionLess
        {
            get { return true; }
        }

        public bool IsUndoScriptTransactionLess
        {
            get { return true; }
        }
    }
}