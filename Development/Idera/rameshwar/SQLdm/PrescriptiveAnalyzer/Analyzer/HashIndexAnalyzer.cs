using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdoctor.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    /// <summary>
    /// //SQLdm 10.0 - Srishti Purohit- New Recommendations - SDR-I25, SDR-I26, SDR-I28 Adding new analyzer 
    /// </summary>
    internal class HashIndexAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 19;
        private static Logger _logX = Logger.GetLogger("HashIndexAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        //const to check recomm conditions
        private const int minimumTotalBucketCount = 1000000;
        private const int emptyBucketPercentLimitForFewBucketsReco = 10;
        private const int emptyBucketPercentLimitForTooManyBucketsReco = 67;
        private const int emptyBucketPercentLimitForLargeDuplicateKeyReco = 10;
        private const int avgChainLengthLimit = 10;

        public HashIndexAnalyzer()
        {
            _id = id;
        }
        public override string GetDescription() { return ("HashIndex analysis"); }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.HashIndexMetrics == null || sm.HashIndexMetrics.HashIndexForDBs == null) return;
            foreach (HashIndexForDB metrics in sm.HashIndexMetrics.HashIndexForDBs)
            {
                foreach (HashIndex snap in metrics.HashIndexList)
                {
                    if (snap.EmptyBucketPercent < emptyBucketPercentLimitForFewBucketsReco && snap.total_bucket_count > minimumTotalBucketCount)
                        {
                            AddRecommendation(new HashIndexFewBucketsRecommendation(snap.DatabaseName, snap.SchemaName, snap.TableName, snap.IndexName, snap.total_bucket_count, snap.empty_bucket_count, snap.EmptyBucketPercent, snap.avg_chain_length, snap.max_chain_length));
                        }
                    if (snap.EmptyBucketPercent > emptyBucketPercentLimitForTooManyBucketsReco && snap.total_bucket_count > minimumTotalBucketCount)
                        {
                            AddRecommendation(new HashIndexTooManyBucketsRecommendation(snap.DatabaseName, snap.SchemaName, snap.TableName, snap.IndexName, snap.total_bucket_count, snap.empty_bucket_count, snap.EmptyBucketPercent, snap.avg_chain_length, snap.max_chain_length));
                        }
                    if (snap.EmptyBucketPercent > emptyBucketPercentLimitForLargeDuplicateKeyReco && snap.avg_chain_length > avgChainLengthLimit)
                        {
                            AddRecommendation(new HashIndexLargeDuplicateKeyRecommendation(snap.DatabaseName, snap.SchemaName, snap.TableName, snap.IndexName, snap.total_bucket_count, snap.empty_bucket_count, snap.EmptyBucketPercent, snap.avg_chain_length, snap.max_chain_length));
                        }
                }
                foreach (ScannedHashIndex snap in metrics.ScannedHashIndexList)
                {
                    AddRecommendation(new ScannedHashIndexRecommendation(snap.ScannedDatabaseName, snap.ScannedSchemaName, snap.ScannedTableName, snap.ScannedIndexName));
                }
            }
        }
    }
}
