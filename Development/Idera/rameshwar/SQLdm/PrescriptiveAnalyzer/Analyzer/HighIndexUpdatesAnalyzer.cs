using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using System.Data.SqlClient;
using BBS.TracerX;
//using Idera.SQLdoctor.AnalysisEngine.Batches;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class HighIndexUpdatesAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 6;
        private static Logger _logX = Logger.GetLogger("HighIndexUpdatesAnalyzer");
        private double daysSinceServerStarted = 0;
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("HighIndexUpdates analysis"); }

        public HighIndexUpdatesAnalyzer()
        {
            _id = id;
        }

        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            if (sm == null) return;
            if (sm.HighIndexUpdatesMetrics == null) return;
            if (sm.ServerPropertiesMetrics != null)
            {
                daysSinceServerStarted = sm.ServerPropertiesMetrics.MinutesRunning / (60 * 24);
            }
            foreach (HighIndexUpdatesForDB metrics in sm.HighIndexUpdatesMetrics.HighIndexUpdatesForDBs)
            {
                foreach (HighIndexUpdates snap in metrics.HighIndexUpdatesList)
                {
                    if (string.IsNullOrEmpty(snap.DatabaseName)) { continue; }

                    ProcessRow(snap, conn);
                }
            }
        }

        private void ProcessRow(HighIndexUpdates snap, System.Data.SqlClient.SqlConnection conn)
        {
            long reads = snap.UserReads;
            long writes = snap.UserWrites;
            long daysTable = snap.DaysSinceTableCreate;
            string db = snap.DatabaseName;
            string schema = snap.schema;
            string table = snap.TableName;
            string indexName = snap.IndexName;
            double days = daysSinceServerStarted;
            //----------------------------------------------------------------------------
            // If the table was created after the server started, use the days since the
            // table was created.
            // 
            if (daysTable < days)
            {
                _logX.DebugFormat("Table {0} has been created since server started.  ServerStartedDays:{1}  TableCreatedDays:{2}", table, days, daysTable);
                days = daysTable;
            }
            if (days <= 6)
            {
                _logX.DebugFormat("HighIndexUpdatesAnalyzer skipped due to days up less than 7 for db:{0} schema:{1} table:{2} index:{3}", db, schema, table, indexName);
                return;
            }

            if (reads > 0)
            {
                //----------------------------------------------------------------------------
                // Per Brett:
                //                 
                // Dont show if number of updates < 2/sec average over the sample period or longer; 
                //   Low - updates per sec >=2 <=25/sec ave; 
                //   Med - updates per sec > 25; 
                //   There is no high impact for this recomendation as no existing index (no matter how bad) can single-handedly drag down a server
                // 
                // Confidence is variable too:
                //   5% confidence if its a clustered index as no DBA can easily change this.
                //   30% confidence if not a clustered index as we dont know if removing this index is going to destroy the run times of queries or stop a query which names the index from running completly
                // 
                TimeSpan ts = TimeSpan.FromDays(days);
                if (0 == ts.TotalSeconds)
                {
                    _logX.DebugFormat("IndexUnderutilizedRecommendation skipped due to total seconds being zero for db:{0} schema:{1} table:{2} index:{3}", db, schema, table, indexName);
                    return;
                }
                double writesPerSec = (writes / ts.TotalSeconds);
                if (writesPerSec < 2)
                {
                    _logX.DebugFormat("IndexUnderutilizedRecommendation skipped due to writes per second being less than 2 for db:{0} schema:{1} table:{2} index:{3}", db, schema, table, indexName);
                    return;
                }
                AddRecommendation(new IndexUnderutilizedRecommendation(
                                        conn,
                                        db,
                                        schema,
                                        table,
                                        indexName,
                                        reads,
                                        writes,
                                        writesPerSec,
                                        snap.WritesPerRead,
                                        daysSinceServerStarted
                                    ));
            }
            else
            {
                //----------------------------------------------------------------------------
                // Per Brett (PR DR311):
                //   I have changed the tree after looking at this recomendation. The decision points changed are:
                //     1. Server must have been running 48 hours or more:
                //     2. Confidence starts much lower and goes up in smaller increments. The effect is that we are only 100% confident after at least 31 days have passed (a month-end) and the index still remains unused.
                //     3. The impact limits have been changed too. See below
                //     4. Note that the number of actions on the table needs to be shown.
                // 
                if (days >= 2)
                {
                    AddRecommendation(new IndexUnusedRecommendation(
                                            conn,
                                            db,
                                            schema,
                                            table,
                                            indexName,
                                            writes,
                                            daysSinceServerStarted
                                        ));
                }
                else
                {
                    _logX.DebugFormat("IndexUnusedRecommendation skipped due to days being less than 2 for db:{0} schema:{1} table:{2} index:{3}", db, schema, table, indexName);
                }
            }
        }
    }
}
