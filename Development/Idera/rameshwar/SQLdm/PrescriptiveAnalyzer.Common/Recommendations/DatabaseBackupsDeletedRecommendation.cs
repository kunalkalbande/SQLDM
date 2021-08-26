using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseBackupsDeletedRecommendation : DatabaseRecommendation
    {
        public string BackupFile { get; private set; }
        public string FileName { get { return BackupFile; } }
        public DatabaseBackupsDeletedRecommendation(string database, string backupFile)
            : base(RecommendationType.DatabaseBackupsDeleted, database)
        {
            BackupFile = backupFile;
        }

        public DatabaseBackupsDeletedRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseBackupsDeleted, recProp)
        {
            BackupFile = recProp.GetString("BackupFile");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("BackupFile", BackupFile.ToString());
            return prop;
        }
    }
}
