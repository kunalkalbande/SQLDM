using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class CpuDatabaseEncryptionAlgorithmRecommendation : CpuRecommendation, IProvideDatabase
    {
        public string Database { get; private set; }
        public string Algorithm { get; private set; }
        public int KeyLength { get; private set; }

        public CpuDatabaseEncryptionAlgorithmRecommendation(string db, string algorithm, int keyLength)
            : base(RecommendationType.CpuDatabaseEncryptionAlgorithm)
        {
            Database = db;
            Algorithm = algorithm;
            KeyLength = keyLength;
        }
        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // High impact for RC2, DES or RC4
            // 
            if ((0 == string.Compare(Algorithm, "RC2", true)) ||
                (0 == string.Compare(Algorithm, "RC4", true)) ||
                (0 == string.Compare(Algorithm, "DES", true)))
            {
                return (HIGH_IMPACT);
            }
            return (base.AdjustImpactFactor(i));
        }
    }
}
