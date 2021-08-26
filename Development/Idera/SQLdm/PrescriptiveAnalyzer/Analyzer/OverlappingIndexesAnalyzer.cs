using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class OverlappingIndexesAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 12;
        private static Logger _logX = Logger.GetLogger("OverlappingIndexesAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public OverlappingIndexesAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("OverlappingIndexes analysis"); }


        private double _serverUpDays;

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.OverlappingIndexesMetrics == null) return;
            _serverUpDays = (null != sm.ServerPropertiesMetrics) ? (sm.ServerPropertiesMetrics.MinutesRunning / (60 * 24)) : 1.0;
            foreach (OverlappingIndexForDB metrics in sm.OverlappingIndexesMetrics.OverlappingIndexForDBs)
            {
                foreach (OverlappingIndex snap in metrics.OverlappingIndexList)
                {
                    Process(snap, conn);
                }
            }
        }


        private void Process(OverlappingIndex snap, System.Data.SqlClient.SqlConnection conn)
        {
            bool recommendDeleteDup = true;
            bool dupIndex = snap.DupIndex;
            string db = snap.DatabaseName;
            string schema = snap.schema;
            string table = snap.TableName;

            bool dupIndexUnique = snap.DupIndexUnique;
            bool indexUnique = snap.IndexUnique;
            bool dupIndexPrimaryKey = snap.DupIndexPrimaryKey;
            bool indexPrimaryKey = snap.IndexPrimaryKey;
            long indexId = snap.IndexId;
            long indexForeignKeys = snap.IndexForeignKeys;
            long dupIndexForeignKeys = snap.DupIndexForeignKeys;
            long indexKeySize = snap.IndexKeySize;
            long dupIndexKeySize = snap.DupIndexKeySize;

            //----------------------------------------------------------------
            // If both keys are used in a foreign key constraint, do not recommend to delete either.
            //
            if ((indexForeignKeys > 0) && (dupIndexForeignKeys > 0))
            {
                _logX.Debug(string.Format("Skipping dup index due to foreign keys (db:{0} schema:{1} table:{2})", db, schema, table));
                return;
            }

            //----------------------------------------------------------------
            // This should never happen but if by some chance it did, don't make the recommendation.
            //
            if (dupIndexPrimaryKey && indexPrimaryKey)
            {
                _logX.Debug(string.Format("Skipping dup index due to primary keys (db:{0} schema:{1} table:{2})", db, schema, table));
                return;
            }

            if (dupIndexPrimaryKey)
            {
                if (indexId > 1) // make sure this is not a clustered index.
                {
                    _logX.Debug(string.Format("Recommend deleting the first index since the dup is a primary key index (db:{0} schema:{1} table:{2})", db, schema, table));
                    recommendDeleteDup = false;
                }
                else
                {
                    _logX.Debug(string.Format("Skipping recommendation due to cluster matching non-cluster primary key (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
            }

            //-------------------------------------------------------------------------------
            //  If one index is unique and the other is not, recommend the non-unique index for delete
            //
            if (dupIndexUnique != indexUnique)
            {
                //--------------------------------------------------------------------------
                // Since the duplicate index is unique, try and recommend the first index.
                // 
                if (dupIndexUnique)
                {
                    if (indexId > 1) // make sure this is not a clustered index.
                    {
                        if (dupIndexForeignKeys > 0) // make sure there are not foreign key constraints
                        {
                            _logX.Debug(string.Format("Skipping dup index due to unique/non-unique with foreign keys (db:{0} schema:{1} table:{2})", db, schema, table));
                            return;
                        }
                        recommendDeleteDup = false;
                    }
                }
            }

            int includeColumnsLength = 0;
            int dupIncludeColumnsLength = 0;
            if (!dupIndex)
            {
                includeColumnsLength = (string.IsNullOrEmpty(snap.IndexIncludeCols)) ? 0 : snap.IndexIncludeCols.Split(' ').Length;
                dupIncludeColumnsLength = (string.IsNullOrEmpty(snap.DupIndexIncludeCols)) ? 0 : snap.DupIndexIncludeCols.Split(' ').Length;
            }
            if ((dupIndexKeySize > indexKeySize) || 
                ((dupIndexKeySize == indexKeySize) && (dupIncludeColumnsLength > includeColumnsLength)))
            {
                if (indexId > 1) // make sure this is not a clustered index.
                {
                    if (dupIndexForeignKeys > 0) // make sure there are not foreign key constraints
                    {
                        _logX.Debug(string.Format("Skipping dup index due to index key size with foreign keys (db:{0} schema:{1} table:{2})", db, schema, table));
                        return;
                    }
                    recommendDeleteDup = false;
                }
            }

            if (recommendDeleteDup)
            {
                if (dupIndexForeignKeys > 0)
                {
                    if (!indexUnique && (indexId > 1) && (indexKeySize <= dupIndexKeySize)) 
                    {
                        recommendDeleteDup = false;
                    }
                }
            }

            //--------------------------------------------------------------
            // At this point we should have determined if we want to recommend
            // the first or second duplicate index.  If the one we want to
            // recommend has foreign key constraints, skip the recommendation.
            //
            if (recommendDeleteDup)
            {
                if (dupIndexForeignKeys > 0)
                {
                    _logX.Debug(string.Format("Cannot delete duplicate due to foreign keys (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (dupIndexKeySize > indexKeySize)
                {
                    _logX.Debug(string.Format("Cannot delete partial duplicate due to key sizes (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (dupIndexPrimaryKey)
                {
                    _logX.Debug(string.Format("Cannot delete duplicate due to primary key index (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (dupIndexUnique && !indexUnique)
                {
                    _logX.Debug(string.Format("Cannot delete unique constraint duplicate index (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
            }
            else
            {
                if (indexForeignKeys > 0)
                {
                    _logX.Debug(string.Format("Cannot delete first duplicate due to foreign keys (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (indexKeySize > dupIndexKeySize)
                {
                    _logX.Debug(string.Format("Cannot delete first partial-duplicate due to key sizes (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (indexPrimaryKey)
                {
                    _logX.Debug(string.Format("Cannot delete first duplicate due to primary key index (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                if (indexUnique && !dupIndexUnique)
                {
                    _logX.Debug(string.Format("Cannot delete first unique constraint duplicate index (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
            }

            string keepIndexName = string.Empty;
            long keepIndexUsage = 0;
            long keepIndexUpdates = 0;

            string deleteIndexName = string.Empty;
            long deleteIndexUsage = 0;
            long deleteIndexUpdates = 0;

            if (recommendDeleteDup)
            {
                keepIndexName = snap.IndexName;
                keepIndexUsage = snap.IndexUsage;
                keepIndexUpdates = snap.IndexUpdates;

                deleteIndexName = snap.DupIndexName;
                deleteIndexUsage = snap.DupIndexUsage;
                deleteIndexUpdates = snap.DupIndexUpdates;
            }
            else
            {
                keepIndexName = snap.DupIndexName;
                keepIndexUsage = snap.DupIndexUsage;
                keepIndexUpdates = snap.DupIndexUpdates;

                deleteIndexName = snap.IndexName;
                deleteIndexUsage = snap.IndexUsage;
                deleteIndexUpdates = snap.IndexUpdates;
            }

            if (dupIndex)
            {
                //------------------------------------------------------------------
                // If the index is the clustered index, give a special recommendation
                // around deleting a non-clustered index that covers the same key 
                // columns as the clustered index.
                //
                if (1 == indexId)
                {
                    _logX.Debug(string.Format("NonClustered index matching clustered recommendation Keep:{0}  Delete:{1} (db:{2} schema:{3} table:{4})", keepIndexName, deleteIndexName, db, schema, table));
                    //------------------------------------------------------------------
                    // Only recommend the removal of the non-clustered index that matches
                    // the clustered index when the non-clustered index has not be used for reads.
                    //
                    if ((deleteIndexUsage > 0) || (0 == keepIndexUsage))
                    {
                        _logX.Debug(string.Format("NonClustered index matching clustered recommendation skipped due to index usage (Clustered Usage: {0}  Non-clustered Usage: {1})", keepIndexUsage, deleteIndexUsage));
                    }
                    else
                    {
                        AddRecommendation(new NonClusteredMatchingClusteredIndexRecommendation(
                                            conn,
                                            db,
                                            schema,
                                            table,
                                            _serverUpDays,
                                            keepIndexName,
                                            keepIndexUsage,
                                            keepIndexUpdates,
                                            deleteIndexName,
                                            deleteIndexUsage,
                                            deleteIndexUpdates
                                        ));
                    }
                    return;
                }
                _logX.Debug(string.Format("Duplicate index recommendation Keep:{0}  Delete:{1} (db:{2} schema:{3} table:{4})", keepIndexName, deleteIndexName, db, schema, table));
                AddRecommendation(new DuplicateIndexRecommendation(
                                        conn,
                                        db,
                                        schema,
                                        table,
                                        _serverUpDays,
                                        keepIndexName,
                                        keepIndexUsage,
                                        keepIndexUpdates,
                                        deleteIndexName,
                                        deleteIndexUsage,
                                        deleteIndexUpdates
                                    ));
            }
            else
            {
                string[] keepKeyColumns = null;
                string[] keepIncludeColumns = null;
                string[] overlapKeyColumns = null;
                string[] overlapIncludeColumns = null;
                if (recommendDeleteDup)
                {
                    keepKeyColumns = snap.IndexCols.Split(' ');
                    keepIncludeColumns = snap.IndexIncludeCols.Split(' ');
                    overlapKeyColumns = snap.DupIndexCols.Split(' ');
                    overlapIncludeColumns = snap.DupIndexIncludeCols.Split(' ');
                }
                else
                {
                    keepKeyColumns = snap.DupIndexCols.Split(' ');
                    keepIncludeColumns = snap.DupIndexIncludeCols.Split(' ');
                    overlapKeyColumns = snap.DupIndexCols.Split(' ');
                    overlapIncludeColumns = snap.IndexIncludeCols.Split(' ');
                }
                //-------------------------------------------------------------------------------------------------
                // If one of the indexes are unique don't give a recommendation if the keys do not match.
                //
                if ((indexUnique || dupIndexUnique) && !AllKeysMatch(overlapKeyColumns, keepKeyColumns))
                {
                    _logX.Debug(string.Format("Skip consolidate and overlapping recommendation due to unique indexes and mismatching key columns (db:{0} schema:{1} table:{2})", db, schema, table));
                    return;
                }
                //-----------------------------------------------------------------------------------------------
                // If the overlapped index is completely covered by the index we are keeping, make the recommendation
                // that the indexes overlap one another.
                //
                if (AllLeadKeysMatch(overlapKeyColumns, keepKeyColumns) && AllColumnsCovered(overlapIncludeColumns, keepKeyColumns, keepIncludeColumns))
                {
                    _logX.Debug(string.Format("Overlapping index recommendation Keep:{0}  Delete:{1} (db:{2} schema:{3} table:{4})", keepIndexName, deleteIndexName, db, schema, table));
                    AddRecommendation(new OverlappingIndexRecommendation(
                                            conn,
                                            db,
                                            schema,
                                            table,
                                            _serverUpDays,
                                            keepIndexName,
                                            keepIndexUsage,
                                            keepIndexUpdates,
                                            deleteIndexName,
                                            deleteIndexUsage,
                                            deleteIndexUpdates
                                        ));
                }
                else
                {
                    _logX.Debug(string.Format("Consolidate index recommendation Keep:{0}  Consolidate:{1} (db:{2} schema:{3} table:{4})", keepIndexName, deleteIndexName, db, schema, table));
                    AddRecommendation(new PartialDuplicateIndexRecommendation(
                                            conn,
                                            db,
                                            schema,
                                            table,
                                            _serverUpDays,
                                            keepIndexName,
                                            keepIndexUsage,
                                            keepIndexUpdates,
                                            deleteIndexName,
                                            deleteIndexUsage,
                                            deleteIndexUpdates
                                        ));
                }
            }
        }

        /// <summary>
        /// Verify that all of the cols are either key columns or include columns.
        /// </summary>
        /// <param name="cols">columns to verify for existence in either the key or include</param>
        /// <param name="keyCols">key columns.  These may have a sort indicator to be removed</param>
        /// <param name="includeCols">include columns</param>
        /// <returns></returns>
        private bool AllColumnsCovered(string[] cols, string[] keyCols, string[] includeCols)
        {
            return (AllColumnsCovered(cols, keyCols) || AllColumnsCovered(cols, includeCols));
        }

        /// <summary>
        /// Verify that the list of columns exist in the covered columns (order does not matter)
        /// </summary>
        /// <param name="cols">columns to verify existence</param>
        /// <param name="coveredCols">columns that are covered either by key or included</param>
        /// <returns>true if the cols are covered</returns>
        private bool AllColumnsCovered(string[] cols, string[] coveredCols)
        {
            if ((null == cols) || (null == coveredCols)) return (false);
            foreach (string c in cols) { if (!IsColumnCovered(c, coveredCols)) return (false); }
            return (true);
        }

        /// <summary>
        /// Verify that the columns is found in the covered columns.
        /// </summary>
        /// <param name="c">column to check for existence</param>
        /// <param name="coveredCols">covered columns</param>
        /// <returns>true if the column is found in the covered columns</returns>
        private bool IsColumnCovered(string c, string[] coveredCols)
        {
            if (null == coveredCols) return (false);
            foreach (string s in coveredCols) { if (IsColumnMatch(c, s)) return (true); }
            return (false);
        }

        /// <summary>
        /// Compare the two column strings to determine if they match.  Remove the sort indicator for this compare.
        /// </summary>
        /// <param name="c">column to compare</param>
        /// <param name="s">compare to without sort indicator</param>
        /// <returns>true if there is a match</returns>
        private bool IsColumnMatch(string c, string s)
        {
            if (string.IsNullOrEmpty(c)) return (true);
            if (string.IsNullOrEmpty(s)) return (false);
            return (0 == string.Compare(c.TrimEnd('A', 'D'), s.TrimEnd('A', 'D')));
        }

        /// <summary>
        /// Verify that all of the lead keys are in the keep keys array and in the exact same order.
        /// </summary>
        /// <param name="leadKeys">keys being tested to verify they exists in the keep keys</param>
        /// <param name="keepKeys">keys being kept</param>
        /// <returns>true if all lead keys are found in the keep keys and in the same order</returns>
        private bool AllLeadKeysMatch(string[] leadKeys, string[] keepKeys)
        {
            if ((null == leadKeys) || (null == keepKeys)) return (false);
            if (leadKeys.Length > keepKeys.Length) return (false);
            for (int n = 0; n < leadKeys.Length; ++n)
            {
                if (0 != string.Compare(leadKeys[n], keepKeys[n])) return (false);
            }
            return (true);
        }
        /// <summary>
        /// Verify that all of the lead keys and keep keys match and are in the exact same order.
        /// </summary>
        /// <param name="leadKeys">keys being tested to verify they exists in the keep keys</param>
        /// <param name="keepKeys">keys being kept</param>
        /// <returns>true if all lead keys are found in the keep keys and in the same order</returns>
        private bool AllKeysMatch(string[] leadKeys, string[] keepKeys)
        {
            if ((null == leadKeys) || (null == keepKeys)) return (false);
            if (leadKeys.Length != keepKeys.Length) return (false);
            for (int n = 0; n < leadKeys.Length; ++n)
            {
                if (0 != string.Compare(leadKeys[n], keepKeys[n])) return (false);
            }
            return (true);
        }
    }
}
