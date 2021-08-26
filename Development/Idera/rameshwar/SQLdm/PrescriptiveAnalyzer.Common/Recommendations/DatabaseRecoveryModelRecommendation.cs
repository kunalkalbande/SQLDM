using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseRecoveryModelRecommendation : DatabaseRecommendation
    {
        public string RecoveryModel { get; private set; }
        public DatabaseRecoveryModelRecommendation(string database, string recoveryModel)
            : base(RecommendationType.DatabaseRecoveryModel, database)
        {
            RecoveryModel = recoveryModel;
        }

        public DatabaseRecoveryModelRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseRecoveryModel, recProp)
        {
            RecoveryModel = recProp.GetString("RecoveryModel");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("RecoveryModel", RecoveryModel.ToString());
            return prop;
        }
    }
}
