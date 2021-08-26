using System;
using System.Collections.Generic;
using System.Text;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ScriptGenerator;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdoctor.Common.Recommendations
{
    /// <summary>
    /// // SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-I25,SDR-26, 27 and SDR-I28 -Adding new recommendation class
    /// </summary>
    [Serializable]
    public class HashIndexRecommendation : IndexRecommendation
    {
        public long TotalBucketCount;
        public long EmptyBucketCount;
        public double EmptyBucketPercent;
        public long AvgChainLength;
        public long MaxChainLength;


        public HashIndexRecommendation(RecommendationType rt, string dbName, string schema, string tableName, string indexName, long totalBucketCount, long emptyBucketCount, double emptyBucketPercent, long avgChainLenght, long maxChainLength)
            : base(rt, dbName, schema, tableName, indexName)
        {
            TotalBucketCount = totalBucketCount;
            EmptyBucketCount = emptyBucketCount;
            EmptyBucketPercent = emptyBucketPercent;
            AvgChainLength = avgChainLenght;
            MaxChainLength = maxChainLength;
        }
        public HashIndexRecommendation(RecommendationType rt, RecommendationProperties recProp)
            : base(rt, recProp)
        {
            TotalBucketCount = recProp.GetLong("total_bucket_count");
            EmptyBucketCount = recProp.GetLong("empty_bucket_count");
            EmptyBucketPercent = recProp.GetDouble("EmptyBucketPercent");
            AvgChainLength = recProp.GetLong("avg_chain_length");
            MaxChainLength = recProp.GetLong("max_chain_length");
        }

        public override Dictionary<string, string> GetProperties()
        {
            var prop = base.GetProperties();
            prop.Add("total_bucket_count", TotalBucketCount.ToString());
            prop.Add("empty_bucket_count", EmptyBucketCount.ToString());
            prop.Add("EmptyBucketPercent", EmptyBucketPercent.ToString());
            prop.Add("avg_chain_length", AvgChainLength.ToString());
            prop.Add("max_chain_length", MaxChainLength.ToString());
            return prop;
        }
    }
}