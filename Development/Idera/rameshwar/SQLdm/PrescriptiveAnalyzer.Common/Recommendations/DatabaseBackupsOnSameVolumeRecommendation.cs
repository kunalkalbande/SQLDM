using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class DatabaseBackupsOnSameVolumeRecommendation : DatabaseRecommendation
    {
        public string BackupFile { get; private set; }
        public string DataFile { get; private set; }
        public DatabaseBackupsOnSameVolumeRecommendation(string database, string backupFile, string dataFile)
            : base(RecommendationType.DatabaseBackupsOnSameVolume, database)
        {
            BackupFile = backupFile;
            DataFile = dataFile;
        }

        public DatabaseBackupsOnSameVolumeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.DatabaseBackupsOnSameVolume, recProp)
        {
            BackupFile = recProp.GetString("BackupFile");
            DataFile = recProp.GetString("DataFile");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("BackupFile", BackupFile.ToString());
            prop.Add("DataFile", DataFile.ToString());
            return prop;
        }
    }
}
