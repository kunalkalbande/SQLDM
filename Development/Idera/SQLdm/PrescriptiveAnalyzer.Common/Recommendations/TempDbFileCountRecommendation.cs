using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Idera.SQLdoctor.Common.Recommendations
{
    [Serializable]
    public class TempDbFileCountRecommendation : Recommendation, IProvideDatabase
    {
        public int ProcessorCount { get; internal set; }
        public int FileCount { get; internal set; }
        public string Database { get { return "tempdb"; } }

        internal TempDbFileCountRecommendation() : base(RecommendationType.DiskTempDbNotEnoughFiles) { }
    }
}
