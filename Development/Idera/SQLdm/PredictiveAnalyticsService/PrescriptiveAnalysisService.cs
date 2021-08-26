using System;
using System.Collections.Generic;
using Idera.SQLdm.PredictiveAnalyticsService.Helpers;
using Idera.SQLdm.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.AdHoc;
using Idera.SQLdm.Common.Services;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Values;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Configuration;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Snapshots;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer;

namespace Idera.SQLdm.PredictiveAnalyticsService
{

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New class
    /// </summary>
    public class PrescriptiveAnalysisService : MarshalByRefObject, IPrescriptiveAnalysisService
    {
        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("PrescriptiveAnalysisService");
        internal static PASqlConnectionInfo PASqlConnectionInfo { get; set; }
        internal static string ServerName { get; set; }

        private static void SetProperties(int monitoredSqlServerId)
        {
            MachineInfo machineInfo = DataHelper.GetMachineInfo(monitoredSqlServerId);
            ServerName = machineInfo.MachineName;
            PASqlConnectionInfo = new PASqlConnectionInfo()
            {
                ConnectionString = machineInfo.ConnectionString,
                IsAmazon = machineInfo.IsAmazon,
                IsAzure = machineInfo.IsAzure,
                IsLinux = machineInfo.IsLinux,
            };
        }

        private PAAnalysisConfiguration GetPAConfig(int monitoredSqlServerId, AnalysisConfiguration config)
        {
            // Block non-platform categories and place them in BlockedCategories
            config.BlockedCategories = config.BlockedCategories ?? new Dictionary<int, string>();
            var blockedCategories = DataHelper.GetBlockedCategoryListForTargetPlatform(monitoredSqlServerId) ?? new Dictionary<int, string>();

            foreach (var blockedCategory in blockedCategories)
            {
                if (!config.BlockedCategoryID.Contains(blockedCategory.Key))
                {
                    config.BlockedCategoryID.Add(blockedCategory.Key);
                }

                if (!config.BlockedCategories.ContainsKey(blockedCategory.Key))
                {
                    config.BlockedCategories.Add(blockedCategory.Key, blockedCategory.Value);
                }
            }

            PAAnalysisConfiguration paConfig = new PAAnalysisConfiguration(monitoredSqlServerId)
            {
                IncludeDatabaseName = config.IncludeDatabaseName,
                IncludeDatabase = config.IncludeDatabase,
                BlockedDatabases = config.BlockedDatabases,
                BlockedRecommendationID = config.BlockedRecommendationID,
                BlockedCategories = config.BlockedCategories,
                SqlServerID = config.SqlServerID,
                AnalysisDuration = config.AnalysisDuration,
                BlockedCategoryID = config.BlockedCategoryID,
                BlockedDatabaseIDList = config.BlockedDatabaseIDList,
                FilterApplication = config.FilterApplication,
                IsActive = config.IsActive,
                IsOLTP = config.IsOLTP,
                ProductionServer = config.ProductionServer,
                SchedulingStatus = config.SchedulingStatus,
                SelectedDays = config.SelectedDays,
                StartTime = config.StartTime,
                UseDefault = config.UseDefault
            };

            return paConfig;
        }

        public Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetPrescriptiveAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config)
        {
            using (LOG.InfoCall("GetPrescriptiveAnalysisResult"))
            {
                try
                {
                    DateTime analysisStartTime = DateTime.Now;

                    SetProperties(monitoredSqlServerId);

                    SnapshotValues previousSnapshot = DataHelper.GetPreviousSnapshotValuesForPrescriptiveAnalysis(monitoredSqlServerId);
                    // PrescriptiveAnalyticsSnapshot pa = DataHelper.GetPrescriptiveAnalysisSnapshots(monitoredSqlServerId, config);
                    PAPrescriptiveAnalyticsSnapshot paSnapshot = DataHelper.GetPAPrescriptiveAnalysisSnapshots(monitoredSqlServerId, config);
                    // PAPrescriptiveAnalyticsSnapshot paSnapshot = new PAPrescriptiveAnalyticsSnapshot();
                    paSnapshot.ConnectionString = DataHelper.GetConnectionStringForServer(monitoredSqlServerId);
                    paSnapshot.MachineName = DataHelper.GetMachineName(monitoredSqlServerId);

                    if (previousSnapshot == null)
                        previousSnapshot = new SnapshotValues(null);

                    RecommendationEngine rec = new RecommendationEngine(GetPAConfig(monitoredSqlServerId, config), paSnapshot, previousSnapshot);
                    rec.Run();

                    rec.Results.Type = PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType.Default;
                    rec.Results.AnalysisStartTime = analysisStartTime;

                    // changes for making recommendations target specific
                    var recommendationsForPlatform = DataHelper.GetRecommendationListForTargetPlatform(monitoredSqlServerId);
                    int recommendationCount = 0;
                    
                    for (var i = 0; i < rec.Recommendations.Count; i++)
                    {
                        if (recommendationsForPlatform.Contains(rec.Recommendations[i].ID))
                        {
                            recommendationCount++;
                        }
                        else
                        {
                            LOG.Info("Recommendation {0} might be applicable for server id {1}", rec.Recommendations[i].ID, monitoredSqlServerId);
                            rec.Recommendations.Remove(rec.Recommendations[i]);
                            i--;
                        }
                    }

                    rec.Results.TotalRecommendationCount = recommendationCount;
                    return rec.Results;

                }
                catch (Exception exception)
                {
                    LOG.Error("Caught Exception in GetPrescriptiveAnalysisResult", exception);
                    var res = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result();
                    res.Error = exception;
                    return res;
                }
            }
        }

        public void GetPrescriptiveAnalysisResult(int monitoredSqlServerId)
        {
            using (LOG.InfoCall("GetPrescriptiveAnalysisResult"))
            {
                try
                {
                    DateTime analysisStartTime = DateTime.Now;
                    ScheduledPrescriptiveAnalysisConfiguration config = DataHelper.GetConfiguration(monitoredSqlServerId);

                    if (config.maintenanceMode == true || config.serverVersion == null || config.serverVersion.Major < 9 || config.analysisConfig.SchedulingStatus == false)
                    {

                        if (config.maintenanceMode == true)
                        {
                            LOG.Info(String.Format("Scheduled analysis skipped for server:{0} as server is in maintenance mode", monitoredSqlServerId));
                        }
                        else if (config.serverVersion == null || config.serverVersion.Major < 9)
                        {
                            LOG.Info(String.Format("Scheduled analysis skipped for server:{0} as server version is not found or less than 2005 which does not support analysis", monitoredSqlServerId));
                        }
                        else if (config.analysisConfig.SchedulingStatus == false)
                        {
                            LOG.Info(String.Format("Scheduled analysis skipped for server:{0} as scheduling is disabled for server", monitoredSqlServerId));
                        }
                    }
                    else
                    {
                        SetProperties(monitoredSqlServerId);

                        SnapshotValues previousSnapshotValue = DataHelper.GetPreviousSnapshotValuesForPrescriptiveAnalysis(monitoredSqlServerId);

                        PAPrescriptiveAnalyticsSnapshot paSnapshot = DataHelper.GetPAPrescriptiveAnalysisSnapshots(monitoredSqlServerId, config.analysisConfig);
                        // PrescriptiveAnalyticsSnapshot pa = DataHelper.GetPrescriptiveAnalysisSnapshots(monitoredSqlServerId, config.analysisConfig);
                        // PAPrescriptiveAnalyticsSnapshot paSnapshot = new PAPrescriptiveAnalyticsSnapshot();
                        paSnapshot.ConnectionString = DataHelper.GetConnectionStringForServer(monitoredSqlServerId);
                        paSnapshot.MachineName = DataHelper.GetMachineName(monitoredSqlServerId);

                        RecommendationEngine rec = new RecommendationEngine(GetPAConfig(monitoredSqlServerId, config.analysisConfig), paSnapshot, previousSnapshotValue);
                        rec.Run();

                        rec.Results.Type = PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType.ScheduledDefault;
                        rec.Results.AnalysisStartTime = analysisStartTime;
                        DataHelper.SaveRecommendations(rec.Results, monitoredSqlServerId);
                    }
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                }
            }
        }

        public Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetAdHocBatchAnalysisResult(int monitoredSqlServerId, string queryText, string database, AnalysisConfiguration config)
        {
            using (LOG.InfoCall("GetAdHocBatchAnalysisResult"))
            {
                try
                {
                    DateTime analysisStartTime = DateTime.Now;
                    PAAdHocBatches batches = new PAAdHocBatches();
                    Dictionary<int, string> databases = DataHelper.GetDatabasesForServer(monitoredSqlServerId);
                    uint dbid = 0;
                    foreach (int id in databases.Keys)
                    {
                        if (databases[id].Trim().ToLower() == database.Trim().ToLower())
                        {
                            dbid = (uint)id;
                            break;
                        }
                    }
                    PAAdHocBatch batch = new PAAdHocBatch(queryText, 1, dbid);
                    batches.Add(batch);

                    SnapshotValues previousSnapshotValue = DataHelper.GetPreviousSnapshotValuesForPrescriptiveAnalysis(monitoredSqlServerId);

                    PAPrescriptiveAnalyticsSnapshot paSnapshot = new PAPrescriptiveAnalyticsSnapshot();
                    paSnapshot.ConnectionString = DataHelper.GetConnectionStringForServer(monitoredSqlServerId);
                    paSnapshot.MachineName = DataHelper.GetMachineName(monitoredSqlServerId);

                    RecommendationEngine rec = new RecommendationEngine(GetPAConfig(monitoredSqlServerId, config), paSnapshot, previousSnapshotValue);
                    rec.RunAdHocBatchAnalysis(batches);

                    rec.Results.Type = PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType.AdHocBatch;
                    rec.Results.AnalysisStartTime = analysisStartTime;
                    return rec.Results;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    var res = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result();
                    res.Error = exception;
                    return res;
                }
            }
        }

        public PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result GetWorkLoadAnalysisResult(int monitoredSqlServerId, AnalysisConfiguration config, ActiveWaitsConfiguration waitConfig, QueryMonitorConfiguration queryConfig)
        {
            using (LOG.InfoCall("GetWorkLoadAnalysisResult"))
            {
                try
                {
                    DateTime analysisStartTime = DateTime.Now;
                    SnapshotValues previousSnapshotValue = DataHelper.GetPreviousSnapshotValuesForPrescriptiveAnalysis(monitoredSqlServerId);

                    PAPrescriptiveAnalyticsSnapshot paSnapshot = new PAPrescriptiveAnalyticsSnapshot();
                    paSnapshot.ConnectionString = DataHelper.GetConnectionStringForServer(monitoredSqlServerId);
                    paSnapshot.MachineName = DataHelper.GetMachineName(monitoredSqlServerId);

                    RecommendationEngine rec = new RecommendationEngine(GetPAConfig(monitoredSqlServerId, config), paSnapshot, previousSnapshotValue);
                    
                    var paWaitConfig = new PAActiveWaitsConfiguration(monitoredSqlServerId);
                    var paQueryConfig = new PAQueryMonitorConfiguration(
                                                        false, false, false, false, TimeSpan.FromMilliseconds(500), TimeSpan.Zero,
                                                        0, 0, new PAFileSize(1024), 2, 1000,
                                                        new PAAdvancedQueryMonitorConfiguration(), false, true
                                                    );

                    rec.RunWorkLoadAnalysis(paWaitConfig, paQueryConfig);

                    rec.Results.Type = PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.AnalysisType.WorkLoad;
                    rec.Results.AnalysisStartTime = analysisStartTime;
                    return rec.Results;
                }
                catch (Exception exception)
                {
                    LOG.Error(exception);
                    var res = new PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations.Result();
                    res.Error = exception;
                    return res;
                }
            }
        }

        public void SetScheduleTasks(List<PAScheduledTask> tasks)
        {
            using (LOG.InfoCall("SetScheduleTasks"))
            {
                MainService.SetScheduledTask(tasks);
            }
        }
    }
}
