using System;
using System.Collections.Generic;
using System.Text;
using TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Objects;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class DeadlockAnalyzer : AbstractAnalyzer
    {
        private static Logger _logX = Logger.GetLogger("DeadlockAnalyzer");
        protected override Logger GetLogger() { return (_logX); }

        public override string GetDescription() { return ("Deadlocked process analysis"); }
        public override void Analyze(SnapshotMetrics sm, System.Data.SqlClient.SqlConnection conn)
        {
            using (_logX.DebugCall("DeadlockAnalyzer.Analyze"))
            {
                base.Analyze(sm, conn);

                if (String.IsNullOrEmpty(sm.DeadlockMetrics.TraceFile))
                {
                    DeadlockTraceFlagsRecommendation recommendation = new DeadlockTraceFlagsRecommendation();
                    AddRecommendation(recommendation);

                }
                else
                    if (sm.DeadlockMetrics.DeadlockCount > 0)
                    {
                        foreach (var deadlock in sm.DeadlockMetrics.Deadlocks)
                        {
                            DeadlockRecommendation recommendation = new DeadlockRecommendation();
                            recommendation.EventSequence = deadlock.EventSequence;
                            recommendation.TimeStamp = deadlock.StartTime;
                            recommendation.XDL = deadlock.TextData;

                            UpdateDeadlockData(sm.Options, recommendation);

                            AddRecommendation(recommendation);
                        }
                    }
            }
        }

        private void UpdateDeadlockData(BaseOptions options, DeadlockRecommendation recommendation)
        {
            CheckCancel();

            DeadlockData deadlockData = DeadlockData.FromXDL(recommendation.XDL);
            //var deadlock = deadlockData.Items.First();
            var deadlock = deadlockData.Items[0];
            if (deadlock != null)
            {
                string victimid = deadlock.DeadlockVictim;
                DeadlockProcess victim = null;
                if (!String.IsNullOrEmpty(victimid))
                {
                    try
                    {
                        for (int index = 0; index < deadlock.ProcessList.Length; index++)
                        {
                            if (deadlock.ProcessList[index].Id == victimid)
                            {
                                victim = deadlock.ProcessList[index];
                            }
                        }
                        //victim = deadlock.ProcessList.First(item => victimid.Equals(item.Id));
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.Log(_logX, string.Format("Unable to find deadlock process for victim id '{0}'", victimid), e);
                    }
                }

                if (victim == null)
                {
                    try
                    {
                        //victim = deadlock.ProcessList.First();
                        victim = deadlock.ProcessList[0];
                    }
                    catch (Exception e)
                    {
                        ExceptionLogger.Log(_logX, "No processes in deadlock graph", e);
                    }
                }

                if (victim != null)
                {
                    recommendation.ApplicationName = victim.clientapp;
                    recommendation.HostName = victim.hostname;
                    recommendation.UserName = victim.loginname;
                    string cdb = victim.currentdb;
                    uint dbid;
                    if (UInt32.TryParse(cdb, out dbid))
                    {
                        recommendation.DatabaseId = (int)dbid;
                        recommendation.Database = options.GetDatabaseName(dbid);
                    }
                }
            }
        }
    }
}
