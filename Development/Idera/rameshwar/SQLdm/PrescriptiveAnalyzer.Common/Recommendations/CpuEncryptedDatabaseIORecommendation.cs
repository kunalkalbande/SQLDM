using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Idera.SQLdoctor.Common.Services;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class CpuEncryptedDatabaseIORecommendation : CpuRecommendation
    {
        public List<string> EncryptedDatabases { get; private set; }
        public double EncryptedIOPercent { get; private set; }
        public UInt64 EncryptedIO { get; private set; }
        public UInt64 TotalIO { get; private set; }
        public string EncryptedIOPercentString { get { return (string.Format("{0:0.#}", EncryptedIOPercent)); } }

        public CpuEncryptedDatabaseIORecommendation(IEnumerable<string> encryptedDatabases, double encryptedIOPercent, UInt64 encryptedIO, UInt64 totalIO)
            : base(RecommendationType.CpuEncryptedDatabaseIO)
        {
            EncryptedDatabases = new List<string>();
            if (null != encryptedDatabases) EncryptedDatabases.AddRange(encryptedDatabases);
            EncryptedIOPercent = encryptedIOPercent;
            EncryptedIO = encryptedIO;
            TotalIO = totalIO;
        }
        public override int AdjustImpactFactor(int i)
        {
            //----------------------------------------------------------------------------
            // If the encrypted io is over 50% of the total io, up the impact to HIGH.
            // 
            if (EncryptedIOPercent > 50)
            {
                return (HIGH_IMPACT);
            }
            return (base.AdjustImpactFactor(i));
        }
    }
}
