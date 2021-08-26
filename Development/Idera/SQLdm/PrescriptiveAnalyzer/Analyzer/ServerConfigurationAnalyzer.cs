using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Wintellect.PowerCollections;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    class ServerConfigurationAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 15;
        private static Logger _logX = Logger.GetLogger("ServerAnalyzer");
        protected override Logger GetLogger() { return (_logX); }
        private const int maxServerMemoryLimit = 64;
        private const int sizeFactor = 16;
        private const int dbCompatibilityForSQL2016 = 130;
        private const int dbCompatibilityForSQL2014 = 120;
        private const int avgBufferPoolExtIOLimit = 10;

        public ServerConfigurationAnalyzer()
        {
            _id = id;
        }


        public override string GetDescription() { return ("Server configuration analysis"); }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze"))
            {
                ServerVersion ver;
                using (conn)
                {
                    conn.Open();
                    ver = new ServerVersion(conn.ServerVersion);
                }
                base.Analyze(sm, conn);
                AnalyzeConfigurations(sm.ServerConfigurationMetrics.ConfigurationSettings, ver);
                AnalyzeSecurity(sm.ServerConfigurationMetrics.SecuritySettings);
                AnalyzeVulnerableLogins(sm);
                AnalyzeDeprecatedAgentTokenJobs(sm);
                AnalyzeTraceFlag4199(sm, ver);//SQLdm10.0 -Srishti Purohit - new recommendations (SDR-Q37, SDR-Q38)
                AnalyzeBufferPoolExt(sm, ver);//SQLdm10.0 -Srishti Purohit - new recommendations (SDR-M31, SDR-M32)
                AnalyzeResourceGovernerPool(sm, ver); //SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-D23)
                AnalyzeCardinalityEstimatorUsed(sm, ver); //SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-Q44, SDR-Q45)
                AnalyzeAvailabilityGroupDBFailOver(sm, ver); //SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-R8)
            }
        }
        /// <summary>
        /// //SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-R8)
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ver"></param>
        private void AnalyzeAvailabilityGroupDBFailOver(SnapshotMetrics sm, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeAvailabilityGroupDBFailOver"))
            {
                if (ver.Major >= 13)
                {
                    foreach (string name in sm.ServerConfigurationMetrics.AvailabilityGroups.Keys)
                    {
                        bool dbFailOver = sm.ServerConfigurationMetrics.AvailabilityGroups[name];
                        if (!dbFailOver)
                        {
                            AddRecommendation(new AvailabilityGroupNotEnabledForFailoverRecommendation(name));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// //SQLdm10.0 -Srishti Purohit -  new recommendations (SDR-Q44, SDR-Q45)
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="ver"></param>
        private void AnalyzeCardinalityEstimatorUsed(SnapshotMetrics sm, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeCardinalityEstimatorUsed"))
            {
                if (ver.Major >= 12)
                {
                    Dictionary<string, int> DbWithCompatibilityLowerThan120 = FindDbsWithLowerCompatibility(sm, dbCompatibilityForSQL2014);
                    if (DbWithCompatibilityLowerThan120.Count > 0)
                    {
                        AddRecommendation(new NewCardinalityEstimatorNotBeingUsedRecommendation(DbWithCompatibilityLowerThan120));
                    }
                    if (sm.ServerConfigurationMetrics.TraceFlag2312 && sm.ServerConfigurationMetrics.TraceFlag9481)
                    {
                        AddRecommendation(new BothNewAndOldCardinalityEstimatorInUseRecommendation(DbWithCompatibilityLowerThan120));
                    }
                }
            }
        }
        /// <summary>
        /// //SQLDm 10.0 - Srishti Purohit -  new recommendations (SDR-D23)
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="ver"></param>
        private void AnalyzeResourceGovernerPool(SnapshotMetrics sm, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeResourceGovernerPool"))
            {
                if (sm.ServerConfigurationMetrics.Edition.ToLower().Contains("enterprise") && sm.ServerConfigurationMetrics.IsResourceGovernerEnable)
                {
                    if (sm.ServerConfigurationMetrics.ResourcePoolNameList.Count > 0)
                    {
                        AddRecommendation(new ResourceGovernerIOStallRecommendation(sm.ServerConfigurationMetrics.ResourcePoolNameList));
                    }
                }
            }
        }
        /// <summary>
        /// //SQLDm 10.0 - Srishti Purohit -  new recommendations (SDR-M31, SDR-M32, SDR-M33)
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="ver"></param>
        private void AnalyzeBufferPoolExt(SnapshotMetrics sm, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeBufferPoolExt"))
            {
                if (ver.Major >= 12)
                {
                    string bufferFilePath = sm.ServerConfigurationMetrics.BufferPoolExtFilePath;
                    long maxServerMemory = sm.ServerConfigurationMetrics.MaxServerMemorySizeMB;
                    long bufferPoolExtSize = sm.ServerConfigurationMetrics.BufferPoolExtSizeKB;
                    // max server memory is greater than 64 gb
                    if (bufferPoolExtSize > 0 && (maxServerMemory / 1024) > maxServerMemoryLimit)
                    {
                        AddRecommendation(new BufferPoolExtensionNotUsefulRecommendation(bufferFilePath, bufferPoolExtSize));
                    }
                    // buffer pool ext size is more than 16 times of max server memory
                    if ((bufferPoolExtSize / 1024) > sizeFactor * (maxServerMemory))
                    {
                        AddRecommendation(new LargeBufferPoolExtensionSizeRecommendation(bufferFilePath, bufferPoolExtSize));
                    }

                    if (sm.BufferPoolExtIOMetrics.AverageValue > avgBufferPoolExtIOLimit || sm.BufferPoolExtIOMetrics.IsBufferPoolExtIOSpikeExist)
                    {
                        AddRecommendation(new BufferPoolExtensionHighIORecommendation());
                    }
                }
            }
        }

        /// <summary>
        /// //SQLDm 10.0 - Srishti Purohit-  new recommendations
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="ver"></param>
        private void AnalyzeTraceFlag4199(SnapshotMetrics sm, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeTraceFlag4199"))
            {
                if (ver.Major >= 13)
                {
                    if (sm.ServerConfigurationMetrics.TraceFlag4199)
                    {
                        Dictionary<string, int> DbWithCompatibilityLowerThan130 = FindDbsWithLowerCompatibility(sm, dbCompatibilityForSQL2016);

                        if (DbWithCompatibilityLowerThan130.Count > 0)
                        {
                            AddRecommendation(new Flag4199LowCompatibleRecommendation(DbWithCompatibilityLowerThan130));
                        }
                        else
                        {
                            AddRecommendation(new Flag4199AllDbCompatibleRecommendation());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// //SQLDm 10.0 - Srishti Purohit-  new recommendations
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private bool isBlockedDatabase(SnapshotMetrics sm, string db)
        {
            bool result = false;
            foreach (string DB in sm.Options.BlockedDatabases)
            {
                if (DB == db)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        /// //SQLDm 10.0 - Srishti Purohit -  new recommendations 
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        private Dictionary<string, int> FindDbsWithLowerCompatibility(SnapshotMetrics sm, int limit)
        {
            Dictionary<string, int> DbWithCompatibilityLowerThanLimit = new Dictionary<string, int>();
            foreach (string db in sm.ServerConfigurationMetrics.Compatibility.Keys)
            {
                if (sm.Options.IsDatabaseBlocked(db)) { continue; }
                else
                {
                    if (sm.ServerConfigurationMetrics.Compatibility[db] < limit)
                    {
                        DbWithCompatibilityLowerThanLimit.Add(db, sm.ServerConfigurationMetrics.Compatibility[db]);
                    }
                }
            }
            return DbWithCompatibilityLowerThanLimit;
        }

        private void AnalyzeConfigurations(DataTable dt, ServerVersion ver)
        {
            using (_logX.DebugCall("AnalyzeConfigurations"))
            {
                if (dt == null)
                {
                    _logX.Warn("The datatable passed is null. Recommendations not generated for AnalyzeConfigurations.");
                    return;
                }
                Dictionary<string, Pair<RecommendationType, int>> defaultConfigurationValues = new Dictionary
                    <string, Pair<RecommendationType, int>>
                {
                        {"max worker threads", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,0)},
                        {"query wait (s)", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,-1)},
                        {"max full-text crawl range", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,4)},
                        {"access check cache bucket count", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,0)},
                        {"access check cache quota", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,0)},
                        {"Ad Hoc Distributed Queries", new Pair<RecommendationType,int>(RecommendationType.ServerConfiguration,0)},
                        {"blocked process threshold",new Pair<RecommendationType,int>(RecommendationType.BlockedProcessThreshold,5)},
                        {"blocked process threshold (s)",new Pair<RecommendationType,int>(RecommendationType.BlockedProcessThreshold,5)},
                        {"locks",new Pair<RecommendationType,int>(RecommendationType.LockConfiguration,0)},
                        {"default trace enabled",new Pair<RecommendationType,int>(RecommendationType.DefaultTraceEnabled,1)},
                        {"disallow results from triggers",new Pair<RecommendationType,int>(RecommendationType.DisallowResultsFromTriggers,1)},
                        {"clr enabled",new Pair<RecommendationType,int>(RecommendationType.CLREnabled,0)},
                        {"McAfeeBufferOverflow", new Pair<RecommendationType, int>(RecommendationType.McAfeeBufferOverflow,0)},
                        {"network packet size (B)", new Pair<RecommendationType,int>(RecommendationType.NetworkPacketSize,4096)},

                    };
                DataTableReader dr = new DataTableReader(dt);
                while (dr.Read())
                {
                    string configName = DataHelper.ToString(dr, "name");
                    long runningValue = DataHelper.ToLong(dr, "value_in_use");
                    long configValue = DataHelper.ToLong(dr, "value");
                    bool isDynamic = DataHelper.ToBoolean(dr, "is_dynamic");
                    if (defaultConfigurationValues.ContainsKey(configName))
                    {
                        switch (defaultConfigurationValues[configName].First)
                        {
                            case RecommendationType.McAfeeBufferOverflow:
                                AddRecommendation(new Recommendation(RecommendationType.McAfeeBufferOverflow));
                                continue;
                            case RecommendationType.BlockedProcessThreshold:
                                // Blocked Process Threshold of 0 or > 5 is OK 
                                if (runningValue != 0 && runningValue < defaultConfigurationValues[configName].Second)
                                {
                                    AddRecommendation(new ServerConfigurationRecommendation(defaultConfigurationValues[configName].First, configName, runningValue, defaultConfigurationValues[configName].Second));
                                }
                                continue;
                            case RecommendationType.NetworkPacketSize:
                                // Network packet size between 4096 and 8060 is OK
                                if (runningValue < defaultConfigurationValues[configName].Second || runningValue > 8060)
                                {
                                    AddRecommendation(new ServerConfigurationRecommendation(defaultConfigurationValues[configName].First, configName, runningValue, defaultConfigurationValues[configName].Second));
                                }
                                continue;
                            case RecommendationType.LockConfiguration:
                                // lock config has been deprecated in SS 2008 http://msdn.microsoft.com/en-us/library/ms143729(SQL.100).aspx
                                if (ver.Major >= (int)SSVer_e.SS_2008) { continue; }
                                break;
                        }
                        if (defaultConfigurationValues[configName].Second != runningValue)
                        {
                            AddRecommendation(new ServerConfigurationRecommendation(defaultConfigurationValues[configName].First, configName, runningValue, defaultConfigurationValues[configName].Second));
                        }
                    }
                    if (!isDynamic && runningValue != configValue)
                    {
                        AddRecommendation(new ServerConfigurationRecommendationNoScript(RecommendationType.ServerConfigurationRestartRequired, configName, runningValue, configValue));
                    }
                }
            }
        }

        private void AnalyzeSecurity(DataTable dt)
        {
            using (_logX.DebugCall("AnalyzeSecurity"))
            {
                if (dt == null)
                {
                    _logX.Warn("The datatable passed is null. Recommendations not generated for AnalyzeSecurity");
                    return;
                }
                Dictionary<string, Pair<RecommendationType, int>> expectedSecuritySettings = new Dictionary
                    <string, Pair<RecommendationType, int>>
                {
                        {"IsIntegratedSecurityOnly", new Pair<RecommendationType,int>(RecommendationType.MixedModeAuthentication,1)},
                        {"BuiltinAdministratorIsSysadmin", new Pair<RecommendationType,int>(RecommendationType.BuiltinAdministratorIsSysadmin,0)},
                        {"PublicEnabledSqlAgentProxyAccount", new Pair<RecommendationType,int>(RecommendationType.PublicEnabledSqlAgentProxyAccount,0)},
                };
                DataTableReader dr = new DataTableReader(dt);
                while (dr.Read())
                {
                    string secName = DataHelper.ToString(dr, "name");
                    long secValue = DataHelper.ToLong(dr, "value");
                    if (secName != "Unable to access")
                    {
                        if (expectedSecuritySettings.ContainsKey(secName))
                        {
                            switch (expectedSecuritySettings[secName].First)
                            {
                                case RecommendationType.MixedModeAuthentication:
                                    if (expectedSecuritySettings[secName].Second != secValue)
                                    {
                                        AddRecommendation(new MixedModeAuthenticationRecommendation());
                                    }
                                    continue;
                                case RecommendationType.BuiltinAdministratorIsSysadmin:
                                case RecommendationType.PublicEnabledSqlAgentProxyAccount:
                                    if (expectedSecuritySettings[secName].Second != secValue)
                                    {
                                        AddRecommendation(new Recommendation(expectedSecuritySettings[secName].First));
                                    }
                                    continue;
                            }
                        }
                    }
                }
            }
        }


        private void AnalyzeVulnerableLogins(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeVulnerableLogins"))
            {
                foreach (Triple<string, bool, bool> login in sm.ServerConfigurationMetrics.VulnerableLogins)
                {
                    AddRecommendation(new VulnerableSqlLoginRecommendation(login.First, login.Second, login.Third));
                }
            }
        }

        private void AnalyzeDeprecatedAgentTokenJobs(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("AnalyzeDeprecatedAgentTokenJobs"))
            {
                foreach (Pair<string, string> instance in sm.ServerConfigurationMetrics.DeprecatedAgentTokenJobs)
                {
                    AddRecommendation(new DeprecatedAgentTokenInUseRecommendation(instance.First, instance.Second));
                }
            }
        }


    }
}
