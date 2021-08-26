using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management;
using System.Text;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Sql;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Probes.Wmi
{
    internal class OSStatisticsWmiMiniProbe : WmiMiniProbe
    {
        private readonly static Logger LOG;
        
        public UInt64? TotalPhysicalMemory;
        public UInt64? AvailableBytes;
        public UInt64? PagesPerSec;
        public UInt64? Frequency_PerfTime;
        public UInt64? Timestamp_PerfTime;
        public UInt64? Timestamp_Sys100NS;
        public UInt64? PercentProcessorTime;
        public UInt64? PercentSQLProcessorTime;
        public UInt64? PercentPrivilegedTime;
        public UInt64? PercentUserTime;
        public UInt64? ProcessorQueueLength;
        public UInt64? PercentDiskIdleTime;
        public UInt64? PercentDiskIdleTime_Base;
        public UInt64? AvgDiskQueueLength;
        public DateTime? ServerTimeStamp;

        public bool WMIStatisticsAvailable;

        public int SqlProcessID;

        //OsStatisticAvailability possible texts
        private const string ServiceTimedOutText = "service timedout";
        private const string ServiceUnavailableText = "service unavailable";

        static OSStatisticsWmiMiniProbe()
        {
            LOG = Logger.GetLogger("OSStatisticsWmiMiniProbe");
        }

        public OSStatisticsWmiMiniProbe(string machineName, WmiConfiguration wmiConfig, int SqlProcessID) : base(machineName, wmiConfig, LOG)
        {
            WMIStatisticsAvailable = false;
            this.SqlProcessID = SqlProcessID;
        }

        protected override void Start()
        {
            StartComputerSystemCollector();
        }

        private void StartComputerSystemCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            _collector.BeginCollection(ComputerSystemCallback, InterpretObject, null);
        }

        private void ComputerSystemCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("ComputerSystemCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            TotalPhysicalMemory = WmiCollector.GetPropertyValue<UInt64>(mo,"TotalPhysicalMemory");
                        }
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);
                    
                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartOsMemoryCollector();
            }
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }

        private void StartOsMemoryCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT AvailableBytes,PagesPersec,Frequency_PerfTime,Timestamp_PerfTime,Timestamp_Sys100NS FROM Win32_PerfRawData_PerfOS_Memory");
            _collector.BeginCollection(OsMemoryCallback, InterpretObject, null);
        }

        private void OsMemoryCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("OsMemoryCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            AvailableBytes = WmiCollector.GetPropertyValue<UInt64>(mo,"AvailableBytes");
                            PagesPerSec = WmiCollector.GetPropertyValue<UInt32>(mo,"PagesPersec");
                            Frequency_PerfTime = WmiCollector.GetPropertyValue<UInt64>(mo,"Frequency_PerfTime");
                            Timestamp_PerfTime = WmiCollector.GetPropertyValue<UInt64>(mo,"Timestamp_PerfTime");
                            Timestamp_Sys100NS = WmiCollector.GetPropertyValue<UInt64>(mo,"Timestamp_Sys100NS");
                        }
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);

                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartOsProcessorCollector();
            }
        }

        private void StartOsProcessorCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT PercentProcessorTime,PercentPrivilegedTime,PercentUserTime FROM Win32_PerfRawData_PerfOS_Processor WHERE Name = '_Total'");
            _collector.BeginCollection(OsProcessorCallback, InterpretObject, null);
        }

        private void OsProcessorCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("OsProcessorCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            PercentProcessorTime = WmiCollector.GetPropertyValue<UInt64>(mo,"PercentProcessorTime");
                            PercentPrivilegedTime = WmiCollector.GetPropertyValue<UInt64>(mo,"PercentPrivilegedTime");
                            PercentUserTime = WmiCollector.GetPropertyValue<UInt64>(mo,"PercentUserTime");
                        }
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);

                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartOSystemCollector();
            }
        }

        private void StartOSystemCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT ProcessorQueueLength FROM Win32_PerfRawData_PerfOS_System");
            _collector.BeginCollection(OsSystemCallback, InterpretObject, null);
        }

        private void OsSystemCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("OsSystemCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            ProcessorQueueLength = WmiCollector.GetPropertyValue<UInt32>(mo,"ProcessorQueueLength");
                        }

                        _collector.Results.Clear();
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);

                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartPerfDiskCollector();
            }
        }

        private void StartPerfDiskCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT * FROM Win32_PerfRawData_PerfDisk_PhysicalDisk WHERE Name = '_Total'");
            _collector.BeginCollection(OsPerfDiskCallback, InterpretObject, null);
        }

        private void OsPerfDiskCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("OsPerfDiskCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            PercentDiskIdleTime = WmiCollector.GetPropertyValue<UInt64>(mo,"PercentIdleTime");
                            PercentDiskIdleTime_Base = WmiCollector.GetPropertyValue<UInt64>(mo, "PercentIdleTime_Base");
                            AvgDiskQueueLength = WmiCollector.GetPropertyValue<UInt64>(mo,"AvgDiskQueueLength");           
                        }
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);

                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartSQLCpuCollector();
            }
        }

        private void StartSQLCpuCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery(string.Format("SELECT PercentProcessorTime FROM win32_PerfRawData_PerfProc_Process WHERE IDProcess={0}", SqlProcessID));
            _collector.BeginCollection(SQLCpuCallback, InterpretObject, null);
        }

        private void SQLCpuCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("SQLCpuCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var mo = result[0] as ManagementBaseObject;
                        if (mo != null)
                        {
                            PercentSQLProcessorTime = WmiCollector.GetPropertyValue<UInt64>(mo, "PercentProcessorTime");
                        }
                    }
                }
                else
                {
                    LogException(LOG, e.Exception);
        
                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                WMIServiceTimedout = _collector.WMITimedout;
                WMIStatisticsAvailable = WMIServiceTimedout ? false : true;
                FireCompletion(this, e.Result);
            }
        }


        internal OSMetrics ReadOSMetrics(OSMetrics previousRefresh, Snapshot logTarget, BBS.TracerX.Logger LOG)
        {
            OSMetrics refresh = new OSMetrics();
            try
            {
                bool logTargetIsNull = (logTarget == null || logTarget.TimeStamp == null);
                DateTime? inputUTCTimestamp = logTargetIsNull ? new DateTime() : logTarget.TimeStamp;
                if (ServerTimeStamp.HasValue) inputUTCTimestamp = ServerTimeStamp.Value;

                if (WMIStatisticsAvailable)
                {
                    refresh = OSMetrics.CookCounters(
                        TotalPhysicalMemory,
                        AvailableBytes,
                        PagesPerSec,
                        PercentProcessorTime,
                        PercentPrivilegedTime,
                        PercentUserTime,
                        ProcessorQueueLength,
                        PercentDiskIdleTime,
                        AvgDiskQueueLength,
                        Timestamp_Sys100NS,
                        Timestamp_PerfTime,
                        PercentDiskIdleTime_Base,
                        Frequency_PerfTime,
                        PercentSQLProcessorTime,
                        inputUTCTimestamp,
                        previousRefresh);

                    if (refresh.DiffFromExpectedTimeStampError > 0)
                    {
                        LOG.Warn("OS Metrics detected an invalid value for " + refresh.DiffFromExpectedTimeStampName +
                                 " and disposed of related metrics.  TimeStamp_Sys100NS_Delta: " +
                                 refresh.TimeStamp_Sys100NS_Delta +
                                 " Difference from Expected: " + refresh.DiffFromExpectedTimeStampError);
                    }
                }
                else if(WMIServiceTimedout)
                {
                    refresh.OsStatisticAvailability = ServiceTimedOutText;
                }
                else
                {
                    refresh.OsStatisticAvailability = ServiceUnavailableText;
                }
            }
            catch (Exception exception)
            {
                ProbeHelpers.LogAndAttachToSnapshot(logTarget, LOG, "Read OS Metrics failed: {0}", exception, false);
            }

            return refresh;
        }
    }
}
