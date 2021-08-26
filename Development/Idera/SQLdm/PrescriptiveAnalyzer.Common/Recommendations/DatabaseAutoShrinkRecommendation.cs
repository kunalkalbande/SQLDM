using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseAutoShrinkRecommendation : Recommendation, IProvideDatabase, IScriptGeneratorProvider, IUndoScriptGeneratorProvider
    {
        public string Database { get { return DatabaseName; } }
        public string DatabaseName { get; set; }
        public bool ProductionServer { get; set; }

        public DatabaseAutoShrinkRecommendation(bool productionServer)
            : base(RecommendationType.AutoShrinkEnabled)
        {
        }

        public DatabaseAutoShrinkRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.AutoShrinkEnabled, recProp)
        {
            ProductionServer = recProp.GetBool("ProductionServer");
            DatabaseName = recProp.GetString("DatabaseName");
        }


        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("DatabaseName", DatabaseName.ToString());
            prop.Add("ProductionServer", ProductionServer.ToString());
            return prop;
        }


        public override int AdjustImpactFactor(int i)
        {
            if (ProductionServer)
                return HIGH_IMPACT;

            return base.AdjustImpactFactor(i);
        }

        public IScriptGenerator GetScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database, "AUTO_SHRINK", "OFF WITH NO_WAIT");
        }

        public bool IsScriptRunnable
        {
            get { return true; }
        }

        public IUndoScriptGenerator GetUndoScriptGenerator()
        {
            return new DatabaseConfigurationScriptGenerator(Database, "AUTO_SHRINK", "ON WITH NO_WAIT");
        }

        public bool IsUndoScriptRunnable
        {
            get { return true; }
        }
    }
}
