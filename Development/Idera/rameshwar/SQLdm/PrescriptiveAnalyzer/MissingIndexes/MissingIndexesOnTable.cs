using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.ShowPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.ExecutionPlan;
using Idera.SQLdm.PrescriptiveAnalyzer.WorkLoad.Stats;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using BBS.TracerX;

namespace Idera.SQLdm.PrescriptiveAnalyzer.MissingIndexes
{
    internal class MissingIndexesOnTable
    {
        private static Logger _logX = Logger.GetLogger("MissingIndexesOnTable");

        private string _db;
        private string _schema;
        private string _table;
        private bool _saveStats = false;
        private int _traceDurationMintues = 0;
        private TraceEventStatsEqualityComparer _statsComparer = null;
        private List<MissingIndex> _missing = new List<MissingIndex>();
        private List<TraceEventStats> _eventStats = new List<TraceEventStats>();
        private SqlSystemObjectManager _ssom = null;

        public string FullTableName { get; private set; }
        public string Database { get { return(_db); } }
        public string Schema { get { return (_schema); } }
        public string Table { get { return (_table); } }

        private MissingIndexesOnTable() { }
        public MissingIndexesOnTable(SqlSystemObjectManager ssom, IEnumerable<TraceEventStats> eventStats, string db, string schema, string table, int traceDurationMintues)
        {
            _traceDurationMintues = traceDurationMintues;
            _ssom = ssom;
            Debug.Assert(null != eventStats, string.Format("Could not find associated trace events for table {0}.{1}.{2}", db, schema, table));
            if (null != eventStats) _eventStats.AddRange(eventStats);
            _saveStats = (_eventStats.Count <= 0);
            _db = db;
            _schema = schema;
            _table = table;
            FullTableName = SQLHelper.Bracket(_db, _schema, _table);
        }

        public void AddMissingIndex(TraceEventStats stats, ColumnGroupType[] cgts)
        {
            MissingIndex mi = new MissingIndex(cgts);
            if (!_missing.Contains(mi)) _missing.Add(mi);
            if (_saveStats)
            {
                if (null == _statsComparer) _statsComparer = new TraceEventStatsEqualityComparer();
                //if (!_eventStats.Contains(stats, _statsComparer)) _eventStats.Add(stats);
                bool res1 = false;
                foreach (TraceEventStats c in _eventStats)
                {
                    if (_statsComparer.Equals(stats, c))
                    {
                        res1 = true;
                    }
                }
                if (!res1) _eventStats.Add(stats);
            }
        }

        internal void Analyze(SqlConnection conn, HypotheticalIndexCleanupThread cleanup, List<IRecommendation> recs)
        {
            using (_logX.InfoCall(string.Format("Analyze {0}.{1}.{2}", _db, _schema, _table)))
            {
                SetConnectionDatabase(conn);
                int dbid = SQLHelper.GetDatabaseId(conn, _db);
                double tableUpdatesPerMin = GetTableUpdatesPerMin(conn, _db, _schema, _table);
                double tableUpdatesPerSec = tableUpdatesPerMin / 60;
                _logX.InfoFormat("Table {0}.{1}.{2} has {3} updates per sec.", _db, _schema, _table, tableUpdatesPerSec);
                if (tableUpdatesPerSec > Settings.Default.Max_TableUpdatesPerSecForMissingIndexRecommendation)
                {
                    _logX.InfoFormat("A new index will not be recommended for the table will because there are over {0} updates being performed per sec.", Settings.Default.Max_TableUpdatesPerSecForMissingIndexRecommendation);
                    return;
                }

                AutoPilotHelpers.Init(conn, dbid);
                Dictionary<int, ShowPlanXML> currentPlans = GetExecutionPlans(conn);
                Dictionary<int, ShowPlanXML> bestPlans = null;
                Dictionary<int, ShowPlanXML> tempPlans = null;
                double currentCost = GetTotalCost(currentPlans, Convert.ToUInt32(dbid));
                double lowestCost = currentCost;
                double tempCost = 0.0;
                HypotheticalIndexes bestIndexes = null;
                //----------------------------------------------------------------------------
                // Process verious new index recommendations to see which provides the best 
                // overall improvement in the execution plans.
                // 
                foreach (HypotheticalIndexes i in GetHypotheticalIndexes(conn))
                {
                    try
                    {
                        _logX.Debug("Analyzing Hypothetical Index: " + i.ToString());
                        i.AddIndexes(conn);
                        tempPlans = GetExecutionPlans(conn);
                        if (tempPlans.Count != currentPlans.Count) continue;
                        tempCost = GetTotalCost(tempPlans, Convert.ToUInt32(dbid));
                        if ((tempCost < lowestCost) ||
                            ((tempCost == lowestCost) && i.ContainsFewerColumns(bestIndexes)))
                        {
                            i.PurgeIndexes(tempPlans.Values, _ssom);
                            if (i.NeededIndexCount > 0)
                            {
                                lowestCost = tempCost;
                                bestPlans = tempPlans;
                                bestIndexes = i;
                                _logX.Debug("Lower Cost Hypothetical Index: " + i.ToString());
                            }
                        }
                    }
                    finally
                    {
                        i.ResetIndexes(conn);
                        cleanup.Add(i.GetDropAddedIndexes(conn));
                    }
                }
                //----------------------------------------------------------------------------
                // If we do not have any new indexes that will improve the execution plans, we
                // can exit at this point since we cannot make any recommendation.
                // 
                if (null == bestIndexes)
                {
                    _logX.Debug("No Hypothetical Indexes were found to improve performance.");
                    return;
                }
                //----------------------------------------------------------------------------
                // If we have more than one new index that will improve performance, try to 
                // consolidate it down to a single new index and recommend that index.
                // 
                if (bestIndexes.NeededIndexCount > 1)
                {
                    lowestCost = currentCost;
                    foreach (HypotheticalIndexes i in bestIndexes.GetConsolidatedIndexes())
                    {
                        try
                        {
                            _logX.Debug("Analyzing Consolidated Hypothetical Index: " + i.ToString());
                            i.AddIndexes(conn);
                            tempPlans = GetExecutionPlans(conn);
                            if (tempPlans.Count != currentPlans.Count) continue;
                            tempCost = GetTotalCost(tempPlans, Convert.ToUInt32(dbid));
                            if ((tempCost < lowestCost) ||
                                ((tempCost == lowestCost) && i.ContainsFewerColumns(bestIndexes)))
                            {
                                i.PurgeIndexes(tempPlans.Values, _ssom);
                                if (i.NeededIndexCount > 0)
                                {
                                    lowestCost = tempCost;
                                    bestPlans = tempPlans;
                                    bestIndexes = i;
                                    _logX.Debug("Lower Cost Consolidated Hypothetical Index: " + i.ToString());
                                }
                            }
                        }
                        finally
                        {
                            i.ResetIndexes(conn);
                            cleanup.Add(i.GetDropAddedIndexes(conn));
                        }
                    }
                }

                var indexes = bestIndexes.GetRecommendedIndexes(conn, new TableIndexes(conn, _db, _schema, _table));
                if (null != indexes)
                {
                    foreach (var ri in indexes)
                    {
                        _logX.Info("Adding missing index recommendation: " + ri.IndexName);
                    }

                }
                recs.Add(new MissingIndexRecommendation(_db,
                                                        _schema,
                                                        _table,
                                                        GetMissingIndexCost(Convert.ToUInt32(dbid), currentPlans, bestPlans),
                                                        indexes,
                                                        tableUpdatesPerSec,
                                                        tableUpdatesPerMin,
                                                        _traceDurationMintues,
                                                        GetTotalQueryDuration()
                                                        ));
            }
        }

        private UInt64 GetTotalQueryDuration()
        {
            if (null == _eventStats) return (0);
            UInt64 duration = 0;
            foreach (TraceEventStats s in _eventStats)
            {
                if (null == s) continue;
                duration += s.TotalDuration;
            }
            return (duration);
        }

        private double GetTableUpdatesPerMin(SqlConnection conn, string database, string schema, string table)
        {
            using (_logX.InfoCall("GetTableUpdatesPerMin for " + table))
            {
                try
                {
                    string sql = BatchFinder.GetTableUpdatesPerMin(new ServerVersion(conn.ServerVersion), database, schema, table);
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandType = CommandType.Text;
                        command.CommandTimeout = BatchConstants.DefaultCommandTimeout;
                        object o = command.ExecuteScalar();
                        if (null != o)
                        {
                            _logX.Info("GetTableUpdatesPerMin result: " + o.ToString());
                            return (Convert.ToDouble(o));
                        }
                        else
                        {
                            _logX.Info("GetTableUpdatesPerMin result is null");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "GetTableUpdatesPerMin Exception: ", ex);
                }
                return (0);
            }
        }

        private void SetConnectionDatabase(SqlConnection conn)
        {
            try
            {
                conn.ChangeDatabase(_db);
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(_logX, string.Format("Failed to change to database {0}", _db), ex);
            }
        }

        private IEnumerable<MissingIndexCost> GetMissingIndexCost(UInt32 dbid, Dictionary<int, ShowPlanXML> currentPlans, Dictionary<int, ShowPlanXML> bestPlans)
        {
            List<MissingIndexCost> costs = new List<MissingIndexCost>();
            ExecutionPlanCost planCost = new ExecutionPlanCost(_ssom);
            double current;
            double best;
            ShowPlanXML bestPlan = null;
            foreach (KeyValuePair<int, ShowPlanXML> currentPlan in currentPlans)
            {
                if (bestPlans.TryGetValue(currentPlan.Key, out bestPlan))
                {
                    planCost.CalculateCost(currentPlan.Value, dbid);
                    current = planCost.TotalCost;
                    planCost.CalculateCost(bestPlan, dbid);
                    best = planCost.TotalCost;
                    costs.Add(new MissingIndexCost(GetExecutionCount(currentPlan.Key), GetBatch(currentPlan.Key), current, best));                    
                }
            }
            return (costs);
        }
        /// <summary>
        /// Calculate the total cost of all the given execution plans and take into consideration the number
        /// of times the associated batch was executed.
        /// </summary>
        /// <param name="plans"></param>
        /// <returns></returns>
        private double GetTotalCost(Dictionary<int, ShowPlanXML> plans, UInt32 dbid)
        {
            double totalCost = 0.0;
            ExecutionPlanCost planCost = new ExecutionPlanCost(_ssom);
            foreach (KeyValuePair<int, ShowPlanXML> plan in plans)
            {
                planCost.CalculateCost(plan.Value, dbid);
                totalCost += (planCost.TotalCost * GetCostMultiplier(plan.Key));
            }
            return (totalCost);
        }

        private IEnumerable<HypotheticalIndexes> GetHypotheticalIndexes(SqlConnection conn)
        {
            List<HypotheticalIndexes> hi = new List<HypotheticalIndexes>();
            //----------------------------------------------------------------------------
            // TODO: Add logic for combining index information for composite or covering indexes
            // 
            foreach (MissingIndex mi in _missing)
            {
                hi.Add(new HypotheticalIndexes(_db, _schema, _table, mi, true));
            }
            hi.Add(new HypotheticalIndexes(_db, _schema, _table, _missing));
            return (hi);
        }
        private UInt64 GetExecutionCount(int id)
        {
            if ((id >= 0) && (id < _eventStats.Count)) return (_eventStats[id].TotalCount);
            return (0);
        }
        private string GetBatch(int id)
        {
            if ((id >= 0) && (id < _eventStats.Count)) return (_eventStats[id].High.TextData);
            return (string.Empty);
        }
        /// <summary>
        /// Currently the cost multiplier will simply be the total number of times that the associated batch
        /// was executed.
        /// </summary>
        /// <param name="id">the index into the event stats list</param>
        /// <returns>the multiplier for calculating the total cost of the batch</returns>
        private UInt64 GetCostMultiplier(int id)
        {
            if ((id >= 0) && (id < _eventStats.Count)) return (_eventStats[id].TotalCount);
            return (1);
        }
        private Dictionary<int, ShowPlanXML> GetExecutionPlans(SqlConnection conn)
        {
            Dictionary<int, ShowPlanXML> results = new Dictionary<int, ShowPlanXML>();
            SqlDbNameManager dbNames = new SqlDbNameManager();
            string query = string.Empty;
            for (int n = 0; n < _eventStats.Count; ++n)
            {
                try
                {
                    query = _eventStats[n].High.TextData;
                    dbNames.UpdateDatabaseName(conn, _eventStats[n].High.DBID);
                    results.Add(n, ShowPlanUtils.GetPlan(AutoPilotHelpers.RunWithAutoPilot(conn, query)));
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, string.Format("MissingIndexesOnTable.GetExecutionPlans({0}) Exception: ", query), ex);
                }
            }
            SetConnectionDatabase(conn);
            return (results);
        }

        internal bool HasPartialMatch(DMVMissingIndex dmv)
        {
            if (null == _missing) return (false);
            foreach (MissingIndex i in _missing)
            {
                if (i.IsPartialMatch(dmv))
                {
                    return (true);
                }
            }
            return (false);
        }
    }

}
