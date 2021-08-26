using System;
using System.Collections.Generic;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Configuration;
using System.Data.SqlClient;
using Idera.SQLdm.PrescriptiveAnalyzer.Metrics;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Helpers;
using Idera.SQLdm.PrescriptiveAnalyzer.Common;
using Idera.SQLdm.PrescriptiveAnalyzer.Common.Recommendations;
using Idera.SQLdm.PrescriptiveAnalyzer.Properties;
using Idera.SQLdm.PrescriptiveAnalyzer.SQL;
using Idera.SQLdm.PrescriptiveAnalyzer.Batches;

namespace Idera.SQLdm.PrescriptiveAnalyzer.Analyzer
{
    internal class MemoryAnalyzer : AbstractAnalyzer
    {
        private const Int32 id = 9;
        private List<string> _processThatShouldNotBeRunning = new List<string> { 
                                                                "sidebar.exe",
                                                                "skype.exe",
                                                                "iexplore.exe",
                                                                "tweetdeck.exe",
                                                                };
        private List<string> _processThatShouldBeSkipped = new List<string> { 
                                                                "system idle process", 
                                                                "system", 
                                                                "winlogon", 
                                                                "lsass", 
                                                                "csrsssmss"
                                                                };
        private List<string> _servicesThatShouldNotBeRunning = new List<string> { 
                                                                "alerter",
                                                                "browser",
                                                                "clipsrv",
                                                                "dhcp server",
                                                                "messenger",
                                                                "spooler",
                                                                "wsearch",
                                                                "tapisrv",
                                                                "tabletinputservices",
                                                                "termservice",
                                                                "sysmain",
                                                                "snmptrap",
                                                                "fax",
                                                                "net dde",
                                                                "trkwks",
                                                                "ersrv",
                                                                "licenseservice",
                                                                "tlntsvr",
                                                                "audiosrv",
                                                                "smtp", 
                                                                "ftp*", 
                                                                };
        private static Logger _logX = Logger.GetLogger("MemoryAnalyzer");
        protected override Logger GetLogger() { return (_logX); }
        private ServerVersion _ver = null;
        private string[] _startupOptions = null;
        private bool _lockPagesInMemory = false;
        private bool _3GB = false;
        private bool _PAE = false;
        private bool _AWE = false;
        private UInt64 _totalPhysicalMemory = 0;
        private string _USERVA = string.Empty;

        public MemoryAnalyzer()
        {
            _id = id;
        }

        public override string GetDescription() { return ("Memory analysis"); }

        public override void Analyze(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("MemoryAnalyzer.Analyze"))
            {
                if (conn.State != System.Data.ConnectionState.Open)
                    conn.Open();
                base.Analyze(sm, conn);

                _ver = new ServerVersion(conn.ServerVersion);
                _startupOptions = GetStartOptions(conn);
                if (null != _startupOptions)
                {
                    //_3GB = _startupOptions.Contains("3GB");
                    _3GB = false;
                    for (int index = 0; index < _startupOptions.Length; index++)
                    {
                        if (_startupOptions[index] == "3GB")
                        { _3GB = true; break; }
                    }

                    //_PAE = _startupOptions.Contains("PAE");
                    _PAE = false;
                    for (int index = 0; index < _startupOptions.Length; index++)
                    {
                        if (_startupOptions[index] == "PAE")
                        { _PAE = true; break; }
                    }

                    foreach (string s in _startupOptions)
                    {
                        if (null != s) if (s.StartsWith("USERVA")) { _USERVA = s; break; }
                    }
                }
                //if (IsValid(sc.ServerPropertiesCollector))
                //{
                    _totalPhysicalMemory = sm.ServerPropertiesMetrics.PhysicalMemory;
                    _AWE = sm.ServerPropertiesMetrics.AWEEnabled;
                    _logX.DebugFormat("ServerProperties - TotalPhysicalMemory:{0}  AWE:{1}", _totalPhysicalMemory, _AWE);
                //}
                _lockPagesInMemory = IsLockedPagesInMemory(conn);
                _logX.DebugFormat("3GB:{0}  PAE:{1}  AWE:{2}  LockPagesInMemory:{3}  USERVA:{4}", _3GB, _PAE, _AWE, _lockPagesInMemory, _USERVA);

                //if (IsValid(sc.WMIPhysicalMemoryCollector))
                //{
                    _logX.DebugFormat("PhysicalMemoryCollector - TotalPhysicalMemory:{0}", sm.WMIPhysicalMemoryMetrics.TotalCapacity);
                    _totalPhysicalMemory = Math.Max(_totalPhysicalMemory, sm.WMIPhysicalMemoryMetrics.TotalCapacity);
                //}
                _logX.DebugFormat("TotalPhysicalMemory:{0}  ({1})", _totalPhysicalMemory, FormatHelper.FormatBytes(_totalPhysicalMemory));

                if (Is32BitOS(sm, conn)) { Analyze32BitOS(sm, conn); } else { Analyze64BitOS(sm, conn); }

                AnalyzeMinServerMemory(sm, conn);
                AnalyzeMultipleInstances(sm, conn);
                AnalyzeRunningProcesses(sm, conn);
                AnalyzeDomainController(sm, conn);
                AnalyzeFileSharing(sm, conn);
                AnalyzeDefaultFillFactor(sm, conn);
                AnalyzePageSplits(sm, conn);
                AnalyzePhysicalMemoryChange(sm, conn);
                if (!AnalyzeMaxServerMemoryChange(sm, conn)) { AnalyzeMaxServerMemory(sm, conn); }
                AnalyzeMaxUserConnections(sm, conn);
                AnalyzeIndexCreation(sm, conn);
                AnalyzePerformanceOfServices(sm, conn);
                AnalyzePerformanceOfSystemCache(sm, conn);
                AnalyzeAdHocWorkloads(sm, conn);
                AnalyzeThemes(sm, conn);
                AnalyzePageFaults(sm, conn);
                AnalyzePageCompression(sm, conn);
                if (conn.State == System.Data.ConnectionState.Open)
                    conn.Close();
            }
        }

        private void AnalyzeDomainController(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeDomainController"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIComputerSystemMetrics) { _logX.Debug("null WMIComputerSystemCollector"); return; }
                //if (!IsValid(sc.WMIComputerSystemCollector)) return;
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer"); return; }
                if (sm.WMIComputerSystemMetrics.IsBackupDomainController || sm.WMIComputerSystemMetrics.IsPrimaryDomainController)
                {
                    _logX.DebugFormat("A production SQL Server should not be used as a {0} domain controller.", sm.WMIComputerSystemMetrics.IsPrimaryDomainController ? "primary" : "backup");
                    AddRecommendation(new MemDomainControllerRecommendation());
                }
            }
        }

        private void AnalyzePageCompression(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePageCompression"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIPerfOSProcessorMetrics) { _logX.Debug("null WMIPerfOSProcessorCollector"); return; }
                //if (!IsValid(sc.WMIPerfOSProcessorCollector)) return;
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (GetRecommendationCount() > 0) { _logX.Debug("exit due to recommendations already existing."); return; }
                if (!IsUnderMemoryPressure(sm, conn)) { _logX.Debug("exit due to not being under memory pressure."); return; }
                int processorTime = sm.WMIPerfOSProcessorMetrics.GetAvgCpu(sm.ServerPropertiesMetrics.AffinityMask);
                _logX.DebugFormat("ProcessorTimePercent:{0}  AffinityMask:{1}  Mem_CPUForPageCompression:{2}", processorTime, sm.ServerPropertiesMetrics.AffinityMask, Settings.Default.Mem_CPUForPageCompression);
                if (processorTime < Settings.Default.Mem_CPUForPageCompression)
                {
                    _logX.Debug("Page compression recommendations should be checked.");
                    _common.PageCompression = true;
                }
            }
        }

        private void AnalyzeThemes(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeThemes"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer"); return; }
                _common.ThemesService = true;
            }
        }

        private void AnalyzePageFaults(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePageFaults"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIProcessMetrics) { _logX.Debug("null WMIProcessCollector"); return; }
                //if (!IsValid(sc.WMIProcessCollector)) return;
                if (null == sm.WMIPerfOSMemoryMetrics) { _logX.Debug("null WMIPerfOSMemoryCollector"); return; }
                //if (!IsValid(sc.WMIPerfOSMemoryCollector)) return;
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer server"); return; }
                if (GetRecommendationCount() > 0) { _logX.Debug("exit due to recommendations already existing."); return; }
                _logX.DebugFormat("PagesPerSec:{0}  High:{1}", sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond, Settings.Default.Mem_HighPageFaultsPerSec);
                if (sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond > Settings.Default.Mem_HighPageFaultsPerSec)
                {
                    List<WMIProcess> processes = new List<WMIProcess>(sm.WMIProcessMetrics.Processes);
                    processes.Remove(sm.WMIProcessMetrics.GetProcessById(sm.ServerPropertiesMetrics.ServerProcessID));
                    processes.RemoveAll(delegate(WMIProcess p)
                    {
                        if (null == p) return (true);
                        if (string.IsNullOrEmpty(p.Name)) return (true);
                        return (_processThatShouldBeSkipped.Contains(p.Name.ToLower()));
                    });
                    if (processes.Count > 1) { processes.Sort(delegate(WMIProcess p1, WMIProcess p2) { return (p1.WorkingSetSize.CompareTo(p2.WorkingSetSize)); }); }
                    if (processes.Count > 10) { processes.RemoveRange(10, processes.Count - 10); }
                    //IEnumerable<string> names = from p in processes select FormatNameArg(p.Name, p.SvcHostArgs);
                    List<string> names = new List<string>();
                    foreach (WMIProcess p in processes)
                    {
                        names.Add(FormatNameArg(p.Name, p.SvcHostArgs));
                    }

                    if (null == names) { _logX.Debug("exit due to no names."); return; }
                    _logX.DebugFormat("Add recommendation showing the top 10 processes by memory usage ({0}).", names);
                    AddRecommendation(new MemTop10ProcessesRecommendation(sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond, names));
                }
            }
        }

        private string FormatNameArg(string name, string arg)
        {
            if (string.IsNullOrEmpty(arg)) return (name);
            return (string.Format("{0} {1}", name, arg));
        }

        private void AnalyzeAdHocWorkloads(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeAdHocWorkloads"))
            {
                if (null == sm) { _logX.Debug("null SnapshotMetrics"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesMetrics"); return; }
                if (_ver.Major < (int)SSVer_e.SS_2008) { _logX.DebugFormat("Optimize for AdHoc not supported for {0}", _ver.ToString()); return; }
                if (sm.ServerPropertiesMetrics.OptimizeForAdHocWorkloads) { _logX.Debug("Workload already optimized for ad hoc queries"); return; }
                object o = SQLHelper.GetScalarResult(BatchFinder.GetBatch("GetAdhocCachedPlanBytes", _ver), conn);
                if (DataHelper.IsNull(o)) { _logX.Debug("Could not get ad hoc plan cache bytes"); return; }
                try
                {
                    UInt64 bytes = Convert.ToUInt64(o);
                    UInt64 highBytes = ((UInt64)Settings.Default.Mem_HighAdhocCacheSizeMB) << 20;
                    _logX.DebugFormat("Adhoc cache bytes:{0}  High:{1}", bytes, highBytes);
                    if (bytes > highBytes)
                    {
                        _logX.Debug("Add optimize for adhoc workloads recommendation.");
                        AddRecommendation(new MemOptimizeForAdhocRecommendation(bytes, highBytes));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AnalyzeAdHocWorkloads Exception:", ex);
                }
            }
        }

        /// <summary>
        /// Information supporting this flag being used to determine if 'performance of system cache' is set
        /// instead of performance of programs.
        /// http://secretdiaryofhan.wordpress.com/2007/04/23/data-collector-and-windows-system-cache/
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="conn"></param>
        private void AnalyzePerformanceOfSystemCache(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePerformanceOfSystemCache"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                _logX.DebugFormat("WindowsVersion:{0}", sm.ServerPropertiesMetrics.WindowsVersion);
                if (sm.ServerPropertiesMetrics.IsWindows2008_Or_Newer)
                {
                    _logX.DebugFormat("Skipping Windows 2008 and newer (ver:{0})", sm.ServerPropertiesMetrics.WindowsVersionNum);
                    return;
                }

                CheckCancel();
                object o = SQLHelper.RegRead(conn, "HKEY_LOCAL_Machine", @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\", "LargeSystemCache");
                if (null == o) { _logX.Debug("null LargeSystemCache"); return; }
                _logX.DebugFormat("LargeSystemCache:{0}", o.ToString());
                try
                {
                    if (1 == Convert.ToUInt32(o))
                    {
                        //_logX.Debug("Add performance of cache recommendation.");
                        _logX.Debug("Skipped add performance of cache recommendation.");
                        //AddRecommendation(new MemPerfOfCacheRecommendation(sc.Options.ProductionServer));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AnalyzePerformanceOfSystemCache Exception: ", ex);
                }
            }
        }

        /// <summary>
        /// Check if the server is configured for background service performance.
        /// System\Performance Settings\Advanced\Processor Scheduling
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="conn"></param>
        private void AnalyzePerformanceOfServices(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePerformanceOfServices"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }

                CheckCancel();
                object o = SQLHelper.RegRead(conn, "HKEY_LOCAL_Machine", @"SYSTEM\CurrentControlSet\Control\PriorityControl\", "Win32PrioritySeparation");
                if (null == o) { _logX.Debug("null Win32PrioritySeparation"); return; }
                _logX.DebugFormat("Win32PrioritySeparation:{0}", o.ToString());
                try
                {
                    UInt32 priSep = Convert.ToUInt32(o);
                    //----------------------------------------------------------------------------
                    // The 2 bit should be the values for 'best performance of background services'
                    //
                    //  info from: http://redmondmag.com/Articles/2002/04/01/Unleash-The-Beast.aspx?Page=2
                    //
                    //  Highest Bits Interval 
                    //    00 or 11 Shorter Intervals (Windows 2000/XP Professional); Longer Intervals (Windows 2000/2002 Server) 
                    //    01 Longer Intervals 
                    //    10 Shorter Intervals 
                    //  Middle Bits Fixed or variable 
                    //    00 or 11 Variable length (Pro); Fixed length (Server) 
                    //    01 Variable Length 
                    //    10 Fixed Length 
                    //  Lowest Bits Ratio of foreground to background threads 
                    //    00 Equal and fixed. This value also overrides the Fixed/Variable value to Fixed. 
                    //    01 2:1. Foreground processes receive twice the processor time of background processes. 
                    //    10 or 11 3:1. Foreground processes receive three times the processor time of background processes. 
                    //
                    //----------------------------------------------------------------------------
                    //  http://technet.microsoft.com/en-us/library/cc976120.aspx
                    //  based on the above information from technet, this flag has different meanings on Server vs
                    //  Pro versions of windows.  If the value is 2, we will assume that it is set correctly since
                    //  they should be running the server version of windows.
                    //
                    //  Please note that the same value is interpreted differently on a computer running Windows 2000 Professional 
                    //  than on one running Windows 2000 Server.
                    //  
                    //  For example, on a computer running Windows 2000 Professional, the default value, 2 (000010), 
                    //  specifies shorter, variable intervals, in which foreground threads get three times the processor time as 
                    //  background threads.
                    //  
                    //  On a computer running Windows 2000 Server, the same default value, 2 (000010), specifies longer, fixed intervals, 
                    //  in which foreground and background threads get the same amount of processor time each time they run.
                    //  
                    //  These strategies optimize foreground processes on a workstation, and they accommodate the needs of processor-intensive 
                    //  services on a server.
                    //  
                    if (2 == priSep)
                    {
                        _logX.Debug("The default setting is in use for this OS (Pro gives priority to foreground services and Server gives equal priority), skipping recommendation.");
                    }
                    else if (38 == priSep)
                    {
                        _logX.Debug("Add recommendation due to best performance being given to foreground applications.  'best performance of background services' should instead be used.");
                        AddRecommendation(new MemPerfOfServicesRecommendation(sm.Options.ProductionServer));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AnalyzePerformanceOfServices Exception: ", ex);
                }
            }
        }

        private void AnalyzeIndexCreation(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeIndexCreation"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                _logX.DebugFormat("IndexCreationMemory:{0} ", sm.ServerPropertiesMetrics.IndexCreationMemory);
                if (0 != sm.ServerPropertiesMetrics.IndexCreationMemory)
                {
                    _logX.Debug("Add index creation memory recommendation.");
                    AddRecommendation(new MemIndexCreationRecommendation(sm.ServerPropertiesMetrics.IndexCreationMemory));
                }
            }
        }

        private void AnalyzeMaxUserConnections(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeMaxUserConnections"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                _logX.DebugFormat("MaxUserConnections:{0}  High:{1}", sm.ServerPropertiesMetrics.MaxUserConnections, Settings.Default.Mem_MaxUserConnections);
                if (sm.ServerPropertiesMetrics.MaxUserConnections > (UInt64)Settings.Default.Mem_MaxUserConnections)
                {
                    _logX.Debug("Add max user connection recommendation.");
                    AddRecommendation(new MemUserConnectionsRecommendation(sm.ServerPropertiesMetrics.MaxUserConnections));
                }
            }
        }

        private bool AnalyzeMaxServerMemoryChange(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeMaxServerMemoryChange"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (false); }
                if (null == sm.Previous) { _logX.Debug("null Previous"); return (false); }
                if (null == sm.Current) { _logX.Debug("null Current"); return (false); }
                _logX.DebugFormat("MaxServerMemory - Current:{0}  Previous:{1}", sm.Current.MaxServerMemory, sm.Previous.MaxServerMemory);
                CheckCancel();
                if (!IsUnderMemoryPressure(sm, conn)) { _logX.Debug("not under memory pressure"); return (false); }
                if (sm.Current.MaxServerMemory >= sm.Previous.MaxServerMemory) { _logX.Debug("max server memory has not be reduced"); return (false); }
                double suggestedMaxServerMemory = DetermineMinSettingForMaxServerMemory(sm);
                _logX.DebugFormat("SuggestedMaxServerMemory:{0}", suggestedMaxServerMemory);

                //--------------------------------------------------------------------------
                // Removed for pr DR1135 - Modified logic to just check that the max server memory
                // has been reduced and that the server is under memory pressure.
                //
                //if (0.0 == suggestedMaxServerMemory) { _logX.Debug("could not determine the suggested max server memory"); return (false); }

                double gigsMaxServer = (sm.ServerPropertiesMetrics.MaxServerMemory / (double)(1 << 10));
                _logX.DebugFormat("MaxServerMemory:{0}GB", gigsMaxServer);
                _logX.DebugFormat("Suggested max of {0} with current max {1}.  Recommendation added due to max server memory being reduced!", suggestedMaxServerMemory, gigsMaxServer);
                AddRecommendation(new MemMaxServerMemoryRecommendation(suggestedMaxServerMemory, gigsMaxServer));
                return (true);

                //--------------------------------------------------------------------------
                // Removed for pr DR1135 - Modified logic to just check that the max server memory
                // has been reduced and that the server is under memory pressure.
                //
                //if ((suggestedMaxServerMemory < (gigsMaxServer - 0.1)) || (suggestedMaxServerMemory > (gigsMaxServer + 0.1)))
                //{
                //    _logX.DebugFormat("Suggested max of {0} is not within +-0.1 of the current max {1}.  Recommendation added!", suggestedMaxServerMemory, gigsMaxServer);
                //    AddRecommendation(new MemMaxServerMemoryRecommendation(suggestedMaxServerMemory, gigsMaxServer));
                //    return (true);
                //}
                //return (false);
            }
        }
        private void AnalyzePhysicalMemoryChange(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePhysicalMemoryChange"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Previous) { _logX.Debug("null Previous"); return; }
                if (null == sm.Current) { _logX.Debug("null Current"); return; }
                _logX.DebugFormat("TotalMemory - Current:{0}  Previous:{1}", sm.Current.TotalPhysicalMemory, sm.Previous.TotalPhysicalMemory);
                if (sm.Current.TotalPhysicalMemory < sm.Previous.TotalPhysicalMemory)
                {
                    _logX.Debug("Memory decreased recommendation added.");
                    AddRecommendation(new MemDecreasedRecommendation(sm.Current.TotalPhysicalMemory, sm.Previous.TotalPhysicalMemory));
                }
            }
        }

        private void AnalyzePageSplits(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzePageSplits"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return; }
                //if (!IsValid(sc.SampledServerResourcesCollector)) return;
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                _logX.DebugFormat("PageSplits:{0}  PagesAllocated:{1}  Mem_HighPageSplitsToAllocationRatio:{2}", sm.SampledServerResourcesMetrics.PageSplitsSec, sm.SampledServerResourcesMetrics.PagesAllocatedSec, Settings.Default.Mem_HighPageSplitsToAllocationRatio);
                if (sm.SampledServerResourcesMetrics.PageSplitsSec <=
                    (sm.SampledServerResourcesMetrics.PagesAllocatedSec * Settings.Default.Mem_HighPageSplitsToAllocationRatio))
                {
                    return;
                }
                if (!IsUnderMemoryPressure(sm, conn))
                {
                    _logX.DebugFormat("DefaultFillFactor:{0}  Equal:{1}  Over:{2}", sm.ServerPropertiesMetrics.DefaultFillFactor, Settings.Default.Mem_DefaultFillFactorEqual, Settings.Default.Mem_DefaultFillFactorOver);
                    if ((Settings.Default.Mem_DefaultFillFactorEqual == sm.ServerPropertiesMetrics.DefaultFillFactor) ||
                        (sm.ServerPropertiesMetrics.DefaultFillFactor > Settings.Default.Mem_DefaultFillFactorOver))
                    {
                        _logX.DebugFormat("Add recommendation to decrease default fill factor (fill factor over {0}%).", Settings.Default.Mem_DefaultFillFactorOver);
                        AddRecommendation(new MemDecreaseDefaultFillFactorRecommendation(sm.ServerPropertiesMetrics.DefaultFillFactor, sm.SampledServerResourcesMetrics.PageSplitsSec, sm.SampledServerResourcesMetrics.PagesAllocatedSec));
                    }
                }
            }
        }
        private void AnalyzeDefaultFillFactor(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeDefaultFillFactor"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return; }
                //if (!IsValid(sc.SampledServerResourcesCollector)) return;
                _logX.DebugFormat("DefaultFillFactor:{0}  Equal:{1}  Over:{2}", sm.ServerPropertiesMetrics.DefaultFillFactor, Settings.Default.Mem_DefaultFillFactorEqual, Settings.Default.Mem_DefaultFillFactorOver);
                if ((Settings.Default.Mem_DefaultFillFactorEqual == sm.ServerPropertiesMetrics.DefaultFillFactor) ||
                    (sm.SampledServerResourcesMetrics.DefaultFillFactor > Settings.Default.Mem_DefaultFillFactorOver))
                {
                    return;
                }
                _logX.DebugFormat("PageSplits:{0}  Low:{1}", sm.SampledServerResourcesMetrics.PageSplitsSec, Settings.Default.Mem_PageSplitsLow);
                if (sm.SampledServerResourcesMetrics.PageSplitsSec > (UInt64)Settings.Default.Mem_PageSplitsLow) { return; }
                if (IsUnderMemoryPressure(sm, conn))
                {
                    _logX.Debug("Add recommendation to increase default fill factor.");
                    AddRecommendation(new MemIncreaseDefaultFillFactorRecommendation(sm.ServerPropertiesMetrics.DefaultFillFactor, sm.SampledServerResourcesMetrics.PageSplitsSec));
                }
                else
                {
                    _logX.DebugFormat("DefaultFillFactor:{0}  Low:{1}", sm.ServerPropertiesMetrics.DefaultFillFactor, Settings.Default.Mem_DefaultFillFactorLow);
                    if (sm.ServerPropertiesMetrics.DefaultFillFactor < Settings.Default.Mem_DefaultFillFactorLow)
                    {
                        _logX.Debug("Skip add of recommendation to adjust default fill factor.");
                        //_logX.Debug("Add recommendation to adjust default fill factor.");
                        //AddRecommendation(new MemAdjustDefaultFillFactorRecommendation(sc.ServerPropertiesCollector.DefaultFillFactor, sc.SampledServerResourcesCollector.PageSplitsSec));
                    }
                }
            }
        }

        /// <summary>
        /// The use of LargeSystemCache for this check is based on:
        /// http://technet.microsoft.com/en-us/library/cc938576.aspx
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="conn"></param>
        private void AnalyzeFileSharing(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeFileSharing"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer server"); return; }

                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                _logX.DebugFormat("WindowsVersion:{0}", sm.ServerPropertiesMetrics.WindowsVersion);
                if (sm.ServerPropertiesMetrics.IsWindows2008_Or_Newer)
                {
                    _logX.DebugFormat("Skipping Windows 2008 and newer (ver:{0})", sm.ServerPropertiesMetrics.WindowsVersionNum);
                    return;
                }

                object o = SQLHelper.RegRead(conn, "HKEY_LOCAL_Machine", @"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management\", "LargeSystemCache");
                if (null == o) { _logX.Debug("null LargeSystemCache"); return; }
                try
                {
                    if (1 == Convert.ToUInt32(o))
                    {
                        _logX.Debug("Add recommendation due to 'Maximize data throughput for file sharing'");
                        AddRecommendation(new MemFileSharingRecommendation());
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.Log(_logX, "AnalyzeFileSharing Exception: ", ex);
                }
            }
        }

        private void AnalyzeRunningProcesses(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeRunningProcesses"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIProcessMetrics) { _logX.Debug("null WMIProcessCollector"); return; }
                //if (!IsValid(sc.WMIProcessCollector)) return;
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a ProductionServer server"); return; }
                _processThatShouldNotBeRunning.Sort();

                //HashSet<string> processes = new HashSet<string>();
                List<string> processesLst = new List<string>();
                foreach (WMIProcess p in sm.WMIProcessMetrics.Processes)
                {
                    if (null == p) continue;
                    if (string.IsNullOrEmpty(p.Name)) continue;
                    if (_processThatShouldNotBeRunning.BinarySearch(p.Name.ToLower()) >= 0)
                    {
                        //processes.Add(p.Name);
                        if (!processesLst.Contains(p.Name))
                        {
                            processesLst.Add(p.Name);
                        }
                        _logX.DebugFormat("Adding recommendation due to process {0}", p.Name);
                    }
                }
                //----------------------------------------------------------------------------
                // Check if any of the services are running.
                // 
                //if (IsValid(sc.WMIServiceCollector))
                //{
                    foreach (string svc in _servicesThatShouldNotBeRunning)
                    {
                        if (svc.EndsWith("*"))
                        {
                            foreach (WMIService w in sm.WMIServiceMetrics.ServicesThatStartWith(svc.TrimEnd('*')))
                            {
                                if (null == w) continue;
                                if (string.IsNullOrEmpty(w.Name)) continue;
                                if (w.IsRunning)
                                {
                                    //processes.Add(w.Name);
                                    if (!processesLst.Contains(w.Name))
                                    {
                                        processesLst.Add(w.Name);
                                    }
                                    _logX.DebugFormat("Adding recommendation due to service {0}", w.Name);
                                }
                            }
                        }
                        else if (sm.WMIServiceMetrics.IsRunning(svc))
                        {
                            //processes.Add(svc);
                            if (!processesLst.Contains(svc))
                            {
                                processesLst.Add(svc);
                            }
                            _logX.DebugFormat("Adding recommendation due to service {0}", svc);
                        }
                    }
                //}
                    if (processesLst.Count > 0) AddRecommendation(new MemRunningProcessRecommendation(processesLst));
            }
        }

        private void AnalyzeMultipleInstances(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeMultipleInstances"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.Options) { _logX.Debug("null Options"); return; }
                if (!sm.Options.ProductionServer) { _logX.Debug("not a Production server"); return; }
                _logX.Debug("Perform multiple instance check.");
                _common.MultipleInstances = true;
            }
        }

        private void AnalyzeMinServerMemory(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeMinServerMemory"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                if (null == sm.WMIPerfOSMemoryMetrics) { _logX.Debug("null WMIPerfOSMemoryCollector"); return; }
                //if (!IsValid(sc.WMIPerfOSMemoryCollector)) return;
                _logX.DebugFormat("MinServerMemory:{0}  MinutesRunning:{1}", sm.ServerPropertiesMetrics.MinServerMemory, sm.ServerPropertiesMetrics.MinutesRunning);
                if (0 == sm.ServerPropertiesMetrics.MinServerMemory) { _logX.Debug("exit AnalyzeMinServerMemory due to default of zero."); return; }
                _logX.DebugFormat("Memory PagesPerSec:{0}  Mem_MinMemory_PagesPerSec:{1}", sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond, Settings.Default.Mem_MinMemory_PagesPerSec);
                if (sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond <= Settings.Default.Mem_MinMemory_PagesPerSec) { _logX.Debug("exit AnalyzeMinServerMemory due to pages per second."); return; }

                _logX.DebugFormat("PhysicalMemory in bytes: {0}", _totalPhysicalMemory);
                double gigsPhysical = (_totalPhysicalMemory / (double)(1 << 30));
                double gigsMinServer = (sm.ServerPropertiesMetrics.MinServerMemory / (double)(1 << 10));
                _logX.DebugFormat("PhysicalMemory in gigs: {0}  MinServer in gigs: {1}", gigsPhysical, gigsMinServer);
                if (gigsPhysical <= 4)
                {
                    if ((gigsPhysical - gigsMinServer) < 1)
                    {
                        AddMemoryPagingRecommendation(sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond);
                    }
                }
                else if ((gigsPhysical > 4) && (gigsPhysical <= 16))
                {
                    if ((gigsPhysical - gigsMinServer) < 2)
                    {
                        AddMemoryPagingRecommendation(sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond);
                    }
                }
                else if (gigsPhysical > 16)
                {
                    if ((gigsPhysical - gigsMinServer) < 3)
                    {
                        AddMemoryPagingRecommendation(sm.WMIPerfOSMemoryMetrics.AvgPagesPerSecond);
                    }
                }
            }
        }
        private void AddMemoryPagingRecommendation(double pagesPerSec)
        {
            using (_logX.DebugCall(string.Format("AddMemoryPagingRecommendation({0})", pagesPerSec)))
            {
                AddRecommendation(new MemPagingRecommendation(pagesPerSec));
            }
        }
        private void AnalyzeMaxServerMemory(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("AnalyzeMaxServerMemory"))
            {
                CheckCancel();
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sc.ServerPropertiesCollector)) return;
                _logX.DebugFormat("MaxServerMemory:{0}  MinutesRunning:{1}", sm.ServerPropertiesMetrics.MaxServerMemory, sm.ServerPropertiesMetrics.MinutesRunning);
                //----------------------------------------------------------------------------
                // check to see if the max server is set to the max signed int value (2147483647)
                // 
                if (Int32.MaxValue == sm.ServerPropertiesMetrics.MaxServerMemory) { return; }

                //----------------------------------------------------------------------------
                // Since the user has constrained the memory usage of sql server, test to see
                // if the server is experiencing any issues if it has been up over 30 minutes.
                // 
                if (sm.ServerPropertiesMetrics.MinutesRunning <= 30) { return; }
                if (IsUnderMemoryPressure(sm, conn))
                {
                    double suggestedMaxServerMemory = DetermineMinSettingForMaxServerMemory(sm);
                    if (suggestedMaxServerMemory > 0.0)
                    {
                        double gigsMaxServer = (sm.ServerPropertiesMetrics.MaxServerMemory / (double)(1 << 10));
                        _logX.DebugFormat("MaxServerMemory:{0}GB", gigsMaxServer);
                        //-----------------------------------------------------------------
                        // Try to account for rounding on this recommendation.  If the suggestion is so
                        // close to what is already set, don't give the recommendation.
                        //
                        if (gigsMaxServer < (suggestedMaxServerMemory - 0.05))
                        {
                            AddMemoryStarvationRecommendation(suggestedMaxServerMemory, gigsMaxServer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Determine what the max server memory should be based on the physical memory of the server.
        /// </summary>
        /// <param name="sc"></param>
        /// <returns>suggested max server memory</returns>
        private double DetermineMinSettingForMaxServerMemory(SnapshotMetrics sm)
        {
            using (_logX.DebugCall("DetermineMinSettingForMaxServerMemory"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (0.0); }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return (0.0); }
                //if (!IsValid(sc.ServerPropertiesCollector)) return (0.0);
                _logX.DebugFormat("PhysicalMemory in bytes: {0}", _totalPhysicalMemory);
                double gigsPhysical = (_totalPhysicalMemory / (double)(1 << 30));
                double temp = 0.0;
                _logX.DebugFormat("PhysicalMemory: {0}GB", gigsPhysical);

                if (gigsPhysical < 4) { temp = 1.1; }
                else if ((gigsPhysical >= 4) && (gigsPhysical <= 16)) { temp = 2.1; }
                else if (gigsPhysical > 16) { temp = (gigsPhysical / 4); }

                if ((temp > 0.0) && (temp < gigsPhysical))
                {
                    temp = (gigsPhysical - temp);
                }
                _logX.DebugFormat("Suggested min setting for max server memory: {0}", temp);
                return (temp);
            }
        }

        private void AddMemoryStarvationRecommendation(double suggestedMaxMem, double maxServerMem)
        {
            using (_logX.DebugCall(string.Format("AddMemoryStarvationRecommendation({0}, {1})", suggestedMaxMem, maxServerMem)))
            {
                AddRecommendation(new MemStarvationRecommendation(suggestedMaxMem, maxServerMem));
            }
        }

        private bool IsPLEUnderThreshold(IEnumerable<NUMANodeCounters> ncs)
        {
            int PLEfor4GB = Settings.Default.Mem_Pressure_PageLife;

            foreach (NUMANodeCounters nc in ncs)
            {
                // http://www.sqlskills.com/blogs/jonathan/finding-what-queries-in-the-plan-cache-use-a-specific-index/
                // TargetPages are in units of 8KB. Adjust PLE value based on memory available for the node
                // 1 << 22 == number of KBs in 4GBs
                ulong expectedPLE = (ulong)Math.Max(PLEfor4GB * ((double)nc.TargetPages * 8.0 / (1 << 22)), PLEfor4GB);
                _logX.DebugFormat("PageLifeExpectancy:{0}  at Node: {1} UnderPressureAt:{2}", nc.PageLifeExpectancy, nc.NodeName, expectedPLE);
                if (nc.PageLifeExpectancy < expectedPLE)
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsUnderMemoryPressure(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("IsUnderMemoryPressure"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (false); }
                if (null == sm.SampledServerResourcesMetrics) { _logX.Debug("null SampledServerResourcesCollector"); return (false); }
                //if (!IsValid(sc.SampledServerResourcesCollector)) return (false);
                _logX.DebugFormat("BufferCacheHitRatio:{0}  UnderPressureAt:{1}", sm.SampledServerResourcesMetrics.LastBufferCacheHitRatio, Settings.Default.Mem_Pressure_BufferCacheHitRatio);
                if (sm.SampledServerResourcesMetrics.LastBufferCacheHitRatio < Settings.Default.Mem_Pressure_BufferCacheHitRatio) { _logX.Debug("Under memory pressure"); return (true); }

                if (IsPLEUnderThreshold(sm.NUMANodeCountersMetrics.NumaNodeCountersList)) { _logX.Debug("Under memory pressure"); return (true); }

                _logX.DebugFormat("MemoryGrantsPending:{0}  UnderPressureAt:{1}", sm.SampledServerResourcesMetrics.MemoryGrantsPending, Settings.Default.Mem_Pressure_MemoryGrantsPending);
                if (sm.SampledServerResourcesMetrics.MemoryGrantsPending > (UInt64)Settings.Default.Mem_Pressure_MemoryGrantsPending) { _logX.Debug("Under memory pressure"); return (true); }
                _logX.Debug("Not under memory pressure");
                return (false);
            }
        }

        private void Analyze32BitOS(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitOS"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.ServerPropertiesMetrics) { _logX.Debug("null ServerPropertiesCollector"); return; }
                //if (!IsValid(sm.ServerPropertiesMetrics)) return;
                _logX.DebugFormat("PhysicalMemory in bytes: {0}", _totalPhysicalMemory);
                double gigs = (_totalPhysicalMemory / (double)(1 << 30));
                _logX.DebugFormat("PhysicalMemory in gigs: {0}", gigs);
                gigs = Math.Round(gigs, 1);
                _logX.DebugFormat("PhysicalMemory in gigs (Rounded): {0}", gigs);
                //-------------------------------------------------------
                // SQL Server 2012 and newer do not support AWE on 32-bit systems.
                //
                string sqlVersionString = SQLHelper.GetSqlVersionString(conn);
                if (SQLHelper.GetSqlVersion(sqlVersionString) > 10)
                {
                    if (4 <= gigs)
                    {
                        _logX.Debug("Added 4GB+ recommendation for SQL Server 2012.");
                        AddRecommendation(new Mem32bit4gSS2012Recommendation(Convert.ToUInt64(Math.Round(gigs, 0))));
                    }
                    else if ((gigs > 3) && (gigs < 4))
                    {
                        Analyze32BitWith3to4GB(sm, conn);
                    }
                    else
                    {
                        Analyze32BitWith3GBorLess(sm, conn);
                    }
                }
                else
                {
                    if (gigs >= 16)
                    {
                        Analyze32BitWith16GB(sm, conn);
                    }
                    else if ((gigs > 4) && (gigs < 16))
                    {
                        Analyze32BitWith4to16GB(sm, conn);
                    }
                    else if (4 == gigs)
                    {
                        Analyze32BitWith4GB(sm, conn);
                    }
                    else if ((gigs > 3) && (gigs < 4))
                    {
                        Analyze32BitWith3to4GB(sm, conn);
                    }
                    else
                    {
                        Analyze32BitWith3GBorLess(sm, conn);
                    }
                }
            }

        }

        private void Analyze32BitWith3GBorLess(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitWith3GBorLess"))
            {
                //if (!_3GB && string.IsNullOrEmpty(_USERVA) && !_PAE && !_AWE)
                //--------------------------------------------------------
                //  Change for DR1156 to not check for a userva value.
                //  This was based on SQLskills review by Jonathan Kehayias.
                //
                if (!_3GB && !_PAE && !_AWE)
                {
                    return;
                }
                _logX.Debug("Added 3GB or less recommendation.    (Report #6)");
                AddRecommendation(new Mem32bit3gOrLessRecommendation());
            }
        }

        private void Analyze32BitWith3to4GB(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitWith3to4GB"))
            {
                //if (_3GB && _USERVA.Contains("1.1") && !_PAE && !_AWE)
                //--------------------------------------------------------
                //  Change for DR790 to not check for constant userva value since the
                // value would depend on the current available memory.
                //
                //if (_3GB && !string.IsNullOrEmpty(_USERVA) && !_PAE && !_AWE)
                //--------------------------------------------------------
                //  Change for DR1156 to not check for a userva value.
                //  This was based on SQLskills review by Jonathan Kehayias.
                //
                if (_3GB && !_PAE && !_AWE)
                {
                    return;
                }
                _logX.Debug("Added 3 to 4GB recommendation.    (Report #5)");
                AddRecommendation(new Mem32bit3to4gRecommendation(_totalPhysicalMemory));
            }
        }

        private void Analyze32BitWith4GB(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitWith4GB"))
            {
                if (_3GB && !_PAE && !_AWE)
                {
                    return;
                }
                _logX.Debug("Added 4GB recommendation.    (Report #4)");
                AddRecommendation(new Mem32bit4gRecommendation());
            }
        }

        private void Analyze32BitWith4to16GB(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitWith4to16GB"))
            {
                if (_3GB && _PAE && _AWE && _lockPagesInMemory)
                {
                    return;
                }
                _logX.Debug("Added 4GB to 16GB recommendation.    (Report #3)");
                AddRecommendation(new Mem32bit4to16gRecommendation());
            }
        }

        private void Analyze32BitWith16GB(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze32BitWith16GB"))
            {
                if (!_3GB && _PAE && _AWE && _lockPagesInMemory)
                {
                    return;
                }
                _logX.Debug("Added 16GB recommendation.    (Report #2)");
                AddRecommendation(new Mem32bit16gRecommendation());
            }
        }

        private string[] GetStartOptions(SqlConnection conn)
        {
            using (_logX.DebugCall("GetStartOptions"))
            {
                CheckCancel();
                string s = SQLHelper.RegRead(conn, "HKEY_LOCAL_Machine", @"SYSTEM\CurrentControlSet\Control\", "SystemStartOptions") as string;
                if (null == s) return (null);
                string[] split = s.ToUpper().Replace(@"\", "").Replace(@"/", "").Split(' ');
                _logX.Debug("Options:", split);
                return (split);
            }
        }

        private void Analyze64BitOS(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Analyze64BitOS"))
            {
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return; }
                if (null == sm.WMIProcessorMetrics) { _logX.Debug("null WMIProcessorCollector"); return; }
                //if (!IsValid(sc.WMIProcessorCollector)) return;
                if (sm.WMIProcessorMetrics.Is64Bit)
                {
                    if (!_lockPagesInMemory)
                    {
                        // Lock pages is supported all builds of enterprise and developer editions
                        bool lockPagesSupported = (sm.ServerPropertiesMetrics.ServerEdition.ToLower().Contains("developer")) || (sm.ServerPropertiesMetrics.ServerEdition.ToLower().Contains("enterprise"));

                        if (sm.ServerPropertiesMetrics.ServerEdition.ToLower().Contains("standard"))
                        {
                            SQLHelper.CheckConnection(conn);
                            ServerVersion serverVersion = new ServerVersion(conn.ServerVersion);

                            if (serverVersion.Major == 9)
                            {
                                lockPagesSupported = (serverVersion.Build >= 4226); // lock pages supported in 2005 standard SP3 with CU 4
                            }
                            else if (serverVersion.Major == 10)
                            {
                                if (serverVersion.Minor == 0)
                                    lockPagesSupported = (serverVersion.Build >= 2714); // lock pages supported in 2008 standard SP1 with CU 2
                                else
                                    lockPagesSupported = (serverVersion.Minor == 50); // Lock pages is supported all builds SQL 2008 R2
                            }
                        }

                        if (lockPagesSupported)
                        {
                            _logX.Info("Add locked pages in memory recommendation.  (Report #1)");
                            AddRecommendation(new Mem64bitLockPagesRecommendation());
                        }
                    }
                }
            }

        }

        private bool IsLockedPagesInMemory(SqlConnection conn)
        {
            CheckCancel();

            object o = SQLHelper.GetScalarResult(BatchFinder.GetBatch("GetLockedPageKB", _ver), conn);
            if (null == o) return (false);
            try
            {
                return (0 != Convert.ToUInt64(o));
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(_logX, "IsLockedPagesInMemory() Exception:", ex);
            }
           
            return (false);
         
        }

        public static bool Is32BitOS(SnapshotMetrics sm, SqlConnection conn)
        {
            using (_logX.DebugCall("Is32BitOS"))
            {
                CheckCancel(sm);
                if (null == sm) { _logX.Debug("null SnapshotCollector"); return (false); }
                if (null == conn) { _logX.Debug("null Connection"); return (false); }
                string version = SQLHelper.GetSqlVersionString(conn).ToUpper();
                if (version.Contains("X64") || version.Contains("64-BIT") || version.Contains("64BIT") || version.Contains("64 BIT"))
                {
                    return (false);
                }
                _logX.Debug("The server appears to be running a 32-bit OS.");

                object o = SQLHelper.RegRead(conn, "HKEY_LOCAL_Machine", @"SYSTEM\CurrentControlSet\Control\Session Manager\Environment\", "PROCESSOR_ARCHITECTURE");
                if (null == o) { _logX.Debug("null PROCESSOR_ARCHITECTURE"); return (true); }
                string pArch = o.ToString();
                _logX.DebugFormat("PROCESSOR_ARCHITECTURE:{0}", pArch);
                bool b64 = o.ToString().Contains("64");
                if (b64) { _logX.Debug("The server is running a 64-bit OS."); return (false); }
                return (true);
            }
        }
    }
}
