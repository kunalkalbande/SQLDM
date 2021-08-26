using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseNoRecentLogBackupRecommendation : DatabaseRecommendation
    {
        private Int64 _logBackupAge;
        private string _recoveryMode;
        public string RecoveryMode { get { return _recoveryMode; } }
        public Int64 Age { get { return _logBackupAge; } }
        public DatabaseNoRecentLogBackupRecommendation(string database, Int64 age, string recoveryMode)
            : base(RecommendationType.DatabaseNoRecentLogBackup, database)
        {
            _logBackupAge = age;
            _recoveryMode = recoveryMode;
        }

        public DatabaseNoRecentLogBackupRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseNoRecentLogBackup, recProp)
        {
            _logBackupAge = recProp.GetInt("_logBackupAge");
            _recoveryMode = recProp.GetString("_recoveryMode");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("_logBackupAge", _logBackupAge.ToString());
            prop.Add("_recoveryMode", _recoveryMode.ToString());
            return prop;
        }
    }
}
