using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Recommendations
{
    /// <summary>
    /// // SQLDoctor 3.5 - Praveen Suhalka - New Recommendations - SDR-I25 -Adding new recommendation class
    /// A memory-optimized hash index has too few buckets.
    /// </summary>
    [Serializable]
    public class HashIndexFewBucketsRecommendation : HashIndexRecommendation
    {

        public HashIndexFewBucketsRecommendation(string dbName, string schema, string tableName, string indexName, long totalBucketCount, long emptyBucketCount, double emptyBucketPercent, long avgChainLenght, long maxChainLength)
            : base(RecommendationType.HashIndexFewBuckets, dbName, schema, tableName, indexName, totalBucketCount, emptyBucketCount, emptyBucketPercent, avgChainLenght, maxChainLength)
        {
        }
        public HashIndexFewBucketsRecommendation(RecommendationProperties recProp)
            : base(RecommendationType.HashIndexFewBuckets, recProp)
        {
        }
    }
}