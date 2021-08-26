using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Recommendations
{
    /// <summary>
    /// // SQLDoctor 3.5 - Praveen Suhalka - New Recommendations - SDR-I26 -Adding new recommendation class
    /// There are many duplicate key values in a memory-optimized hash index.
    /// </summary>
    [Serializable]
    public class HashIndexLargeDuplicateKeyRecommendation : HashIndexRecommendation
    {

        public HashIndexLargeDuplicateKeyRecommendation(string dbName, string schema, string tableName, string indexName, long totalBucketCount, long emptyBucketCount, double emptyBucketPercent, long avgChainLenght, long maxChainLength)
            : base(RecommendationType.HashIndexLargeDuplicateKey, dbName, schema, tableName, indexName, totalBucketCount, emptyBucketCount, emptyBucketPercent, avgChainLenght, maxChainLength)
        {
        }
        public HashIndexLargeDuplicateKeyRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.HashIndexLargeDuplicateKey, recProp)
        {
        }
    }
}