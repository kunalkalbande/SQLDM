using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Services;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations
{
    [Serializable]
    public class CpuEncryptedVolumeRecommendation : CpuRecommendation
    {
        public UInt32 ProtectionStatus { get; private set; }
        public string DriveLetter { get; private set; }
        public bool SystemPageFile { get; private set; }
        public bool SqlDataFile { get; private set; }

        public CpuEncryptedVolumeRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.CpuEncryptedVolume, recProp)
        {
            ProtectionStatus = recProp.GetUInt32("ProtectionStatus");
            DriveLetter = recProp.GetString("DriveLetter");
            SystemPageFile = recProp.GetBool("SystemPageFile");
            SqlDataFile = recProp.GetBool("SqlDataFile");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("ProtectionStatus", ProtectionStatus.ToString());
            prop.Add("DriveLetter", DriveLetter.ToString());
            prop.Add("SystemPageFile", SystemPageFile.ToString());
            prop.Add("SqlDataFile", SqlDataFile.ToString());
            return prop;
        }

        public CpuEncryptedVolumeRecommendation(string drive, UInt32 protectionStatus, bool systemPageFile, bool sqlDataFile)
            : base(RecommendationType.CpuEncryptedVolume)
        {
            ProtectionStatus = protectionStatus;
            DriveLetter = drive;
            SystemPageFile = systemPageFile;
            SqlDataFile = sqlDataFile;
        }
    }
}
