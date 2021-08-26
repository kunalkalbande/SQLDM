using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Text;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Configuration;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.Common.Attributes;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.CollectionService.Probes.Wmi
{
    internal class DriveStatisticsWmiProbe : WmiMiniProbe
    {
        private readonly static Logger LOG;

        [Auditable("Enabled auto discovery of disk drives")]
        public bool AutoDiscovery { get; set; }
        public bool IncludeAll { get; set; }
        public List<string> DriveList { get; set; }


        //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Removed diskmap and added driveStatisticsProbeDetails to store the probe result and added _returnAll field
        private DriveStatisticsWmiDetails driveStatisticsProbeDetails = new DriveStatisticsWmiDetails();

        private bool _returnAll = false;
        
        private int _cookedCounterValue;
        
        public bool ReturnAll
        {
            set { _returnAll = value; }
        }
        //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Removed diskmap and added driveStatisticsProbeDetails to store the probe result and added _returnAll field

        public bool WMIStatisticsAvailable;

        static DriveStatisticsWmiProbe()
        {
            LOG = Logger.GetLogger("DriveStatisticsWmiProbe");
        }

        public DriveStatisticsWmiProbe(string machineName, WmiConfiguration wmiConfig, DiskCollectionSettings diskSettings)
            : base(machineName, wmiConfig, LOG)
        {
            driveStatisticsProbeDetails.DiskMap = new Dictionary<string, DiskStatistics>();//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
            AutoDiscovery = true;
            IncludeAll = true;
            WMIStatisticsAvailable = false;
            DriveList = new List<string>();
            _cookedCounterValue = (CollectionServiceConfiguration.GetCollectionServiceElement().CookedDiskDriveWaitTimeInSeconds > 0 || (diskSettings != null && diskSettings.CookedDiskDriveWaitTimeInSeconds>0)) ? 1: 0;
        }

        protected override void Start()
        {
            if (AutoDiscovery)
                StartMountPointCollector();
            else
                StartVolumeCollector();
        }

        private void StartLogicalDiskCollector()
        {
            _collector.Results.Clear();
            _collector.Query =
                new WqlObjectQuery("SELECT Name,DeviceID,DriveType,FileSystem,Size,FreeSpace FROM Win32_LogicalDisk WHERE DriveType <> 4 AND DriveType <> 5");
            _collector.BeginCollection(LogicalDiskCallback, InterpretObject, null);
        }

        private void LogicalDiskCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("LogicalDiskCallback"))
            {
                if (e.Result == Result.Success)
                {

                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        foreach (ManagementBaseObject mbo in result)
                        {
                            var deviceId = WmiCollector.GetReferencePropertyValue<string>(mbo, "DeviceID");
                            if (String.IsNullOrEmpty(deviceId)) continue;

                            var driveType = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "DriveType");
                            if (driveType == 0) continue;

                            DiskStatistics stats = null;
                            if (!driveStatisticsProbeDetails.DiskMap.TryGetValue(deviceId, out stats)) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                            {
                                stats = new DiskStatistics(deviceId);
                                driveStatisticsProbeDetails.DiskMap.Add(deviceId, stats); //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                                var name = WmiCollector.GetReferencePropertyValue<string>(mbo, "Name");
                                if (String.IsNullOrEmpty(name)) continue;
                                stats.Paths.Add(name.TrimEnd(':', '\\'));
                            }

                            stats.DriveType = driveType;
                            stats.TotalSize = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Size");
                            stats.FreeSpace = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "FreeSpace");
                            stats.FileSystem = WmiCollector.GetReferencePropertyValue<string>(mbo, "FileSystem") ?? String.Empty;
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

                StartRawPerfDiskCollector();
            }
        }

        private void StartMountPointCollector()
        {
            _collector.Query = new WqlObjectQuery("SELECT * FROM Win32_MountPoint");
            _collector.BeginCollection(MountPointCallback, InterpretObject, null);
        }

        private void MountPointCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("MountPointCallback"))
            {
                if (e.Result == Result.Success)
                {
                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var resultCount = result.Count;
                        for (var index = 0; index < result.Count; index++)
                        {
                            var mbo = (ManagementBaseObject)result[index];
                            var path = mbo["Directory"].ToString();
                            int i = path.IndexOf("=") + 1;
                            if (i > 0) path = path.Substring(i + 1).Trim('\"');
                            path = path.TrimEnd('\\', ':').Replace("\\\\", "\\");

                            var deviceId = mbo["Volume"].ToString();
                            i = deviceId.IndexOf("=") + 1;
                            if (i > 0) deviceId = deviceId.Substring(i + 1).Trim('\"');
                            deviceId = deviceId.Replace("\\\\", "\\");

                            DiskStatistics stats = null;
                            if (!this.driveStatisticsProbeDetails.DiskMap.TryGetValue(deviceId, out stats)
                            ) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                            {
                                stats = new DiskStatistics(deviceId);
                                this.driveStatisticsProbeDetails.DiskMap
                                    .Add(
                                        deviceId,
                                        stats); //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                            }
                            stats.Paths.Add(path);
                        }
                    }
                }
                else
                {
                    if (!(e.Exception is ManagementException && ((ManagementException)e.Exception).ErrorCode == ManagementStatus.InvalidClass))
                        LogException(LOG, e.Exception); // just log exception and allow the next collector to run

                    if (!_collector.CanContinue)
                    {
                        LogException(LOG, e.Exception);
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                }

                StartVolumeCollector();
            }
        }

        private void StartVolumeCollector()
        {
            _collector.Results.Clear();
            _collector.Query = new WqlObjectQuery("SELECT Name,DeviceID,DriveType,FileSystem,Capacity,FreeSpace FROM Win32_Volume WHERE DriveType <> 4 AND DriveType <> 5");
            _collector.BeginCollection(VolumeCallback, InterpretObject, null);
        }

        private void VolumeCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("VolumeCallback"))
            {
                if (e.Result == Result.Success)
                {

                    var result = e.Value as IList;
                    if (result != null && result.Count > 0)
                    {
                        var resultCount = result.Count;
                        for (var index = 0; index < resultCount; index++)
                        {
                            var mbo = (ManagementBaseObject)result[index];
                            var deviceId = WmiCollector.GetReferencePropertyValue<string>(mbo, "DeviceID");
                            if (String.IsNullOrEmpty(deviceId)) continue;
                            var name = WmiCollector.GetReferencePropertyValue<string>(mbo, "Name");
                            if (String.IsNullOrEmpty(name) || name.Equals(deviceId)) continue;
                            var driveType = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "DriveType");
                            if (driveType == 0) continue;

                            DiskStatistics stats = null;
                            if (!this.driveStatisticsProbeDetails.DiskMap.TryGetValue(deviceId, out stats)
                            ) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                            {
                                stats = new DiskStatistics(deviceId);
                                this.driveStatisticsProbeDetails.DiskMap
                                    .Add(
                                        deviceId,
                                        stats); //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                                stats.Paths.Add(name.TrimEnd(':', '\\'));
                            }

                            stats.DriveType = driveType;

                            var ulongval = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Capacity");
                            if (ulongval != default(ulong)) stats.TotalSize = ulongval;

                            ulongval = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "FreeSpace");
                            if (ulongval != default(ulong)) stats.FreeSpace = ulongval;

                            var filesystem = WmiCollector.GetReferencePropertyValue<string>(mbo, "FileSystem");
                            if (!String.IsNullOrEmpty(filesystem)) stats.FileSystem = filesystem;
                        }
                    }
                }
                else
                {
                    if (e.Exception is ManagementException && ((ManagementException)e.Exception).ErrorCode == ManagementStatus.InvalidClass)
                    {
                        // OS could be so old it doesn't support WIN32_Volume
                        StartLogicalDiskCollector();
                        return;
                    }
                    
                    if (!_collector.CanContinue)
                    {
                        FireCompletion(e.Exception, e.Result);
                        return;
                    }
                    LogException(LOG, e.Exception);
                }

                StartRawPerfDiskCollector();
            }
        }

        void FilterLogicalDisks()
        {
            var kill = new List<string>();

            // see if this disk is in the include map if not and if returnAll is false then remove it
            if (!(AutoDiscovery || DriveList.Count == 0) && !_returnAll) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Avoid filtering in case of returnAll=true;
            {
                var lowerDriveNames = new List<string>(DriveList.Count);
                foreach (var drive in DriveList)
                {
                    lowerDriveNames.Add(drive.TrimEnd(':', '\\').ToLower());
                }

                foreach (var disk in driveStatisticsProbeDetails.DiskMap) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                {
                    var shouldkill = true;
                    foreach (var path in lowerDriveNames)
                    {
                        if (!disk.Value.ContainsPath(path)) continue;

                        shouldkill = false;
                        break;
                    }
                    if (shouldkill)
                        kill.Add(disk.Key);
                }
                foreach (var key in kill)
                {
                    driveStatisticsProbeDetails.DiskMap.Remove(key);//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                }
            }
        }


        /// <summary>
        /// //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - new method to GET SQL DISK FILTER STRING 
        /// </summary>
        /// <returns></returns>
        private string GetDiskFilter()
        {
            var diskFilter = new StringBuilder();

            if (driveStatisticsProbeDetails.DiskMap.Count == 0 || AutoDiscovery)
            {
                return null;
            }
            else
            {

                diskFilter.Append(" ( ");
                var mark = diskFilter.Length;
                // build a list of disk names to collect stats for selected drives
                foreach (var drive in DriveList)
                {
                    var mount = drive.TrimEnd(':', '\\').ToUpper();
                    if (mark != diskFilter.Length)
                        diskFilter.Append(" OR ");
                    diskFilter.AppendFormat(" upper(drive_letter) ='{0}' ", mount.ToUpper());
                }

                if (mark == diskFilter.Length)
                    diskFilter.Remove(mark - 3, 3);
                else
                    diskFilter.Append(" ) ");

               
            }

            return diskFilter.ToString();
        }

        private void StartRawPerfDiskCollector()
        {
            FilterLogicalDisks();

            // use select * to work around windows 2008 f%$k up
            var wql = new StringBuilder("SELECT * FROM Win32_PerfRawData_PerfDisk_LogicalDisk");

            // build a list of disk names to collect stats for
            // list will either come from collected mount points or from configuration or all 
            if ((driveStatisticsProbeDetails.DiskMap.Count == 0 || AutoDiscovery) || _returnAll) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Avoid filtering in case of returnAll=true;
            {
                _collector.Query = new WqlObjectQuery(wql.ToString());
                if (_returnAll)//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Set the diskSqlfiter incase of returnAll=true
                    driveStatisticsProbeDetails.DiskSqlFilter = GetDiskFilter();
            }
            else
            {
                wql.Append(" WHERE ");
                var mark = wql.Length;


                foreach (var disk in driveStatisticsProbeDetails.DiskMap.Values) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                {
                    if (disk.DriveType == 4 || disk.DriveType == 5) continue;
                    if (disk.FileSystem != null) continue;

                    foreach (var mount in disk.Paths)
                    {
                        var wmount = (mount.Length == 1) ? mount + ":" : mount.Replace("\\", "\\\\");
                        if (mark != wql.Length)
                            wql.Append(" or ");
                        wql.AppendFormat("Name='{0}'", wmount);

                    }
                }
                if (mark == wql.Length)
                    wql.Remove(mark - 7, 7);

                
                _collector.Query = new WqlObjectQuery(wql.ToString());
            }
            _collector.Results.Clear();
            _collector.BeginCollection(RawPerfDiskCallback, InterpretRawPerfDiskObject, null);
        }
        
        private void StartPerfFormattedPerfDiskCollector()
        {
            // use select * to work around windows 2008 f%$k up
            var wql = new StringBuilder("SELECT * FROM Win32_PerfFormattedData_PerfDisk_LogicalDisk");

            // build a list of disk names to collect stats for
            // list will either come from collected mount points or from configuration or all 
            if ((driveStatisticsProbeDetails.DiskMap.Count == 0 || AutoDiscovery) || _returnAll) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Avoid filtering in case of returnAll=true;
            {
                _collector.Query = new WqlObjectQuery(wql.ToString());
                if (_returnAll)//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Set the diskSqlfiter incase of returnAll=true
                    driveStatisticsProbeDetails.DiskSqlFilter = GetDiskFilter();
            }
            else
            {
                wql.Append(" WHERE ");
                var mark = wql.Length;


                foreach (var disk in driveStatisticsProbeDetails.DiskMap.Values) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                {
                    if (disk.DriveType == 4 || disk.DriveType == 5) continue;
                    if (disk.FileSystem != null) continue;

                    foreach (var mount in disk.Paths)
                    {
                        var wmount = (mount.Length == 1) ? mount + ":" : mount.Replace("\\", "\\\\");
                        if (mark != wql.Length)
                            wql.Append(" or ");
                        wql.AppendFormat("Name='{0}'", wmount);

                    }
                }
                if (mark == wql.Length)
                    wql.Remove(mark - 7, 7);


                _collector.Query = new WqlObjectQuery(wql.ToString());
            }
            _collector.Results.Clear();
            _collector.BeginCollection(PerfFormattedPerfDiskCallback, InterpretPerfFormattedPerfDiskObject, null);
        }

        private void PerfFormattedPerfDiskCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("RawPerfDiskCallback"))
            {
                _collector.Results.Clear();
                WMIServiceTimedout = _collector.WMITimedout;
                WMIStatisticsAvailable = !WMIServiceTimedout && e.Result == Result.Success;
                FireCompletion(driveStatisticsProbeDetails, e.Result, e.Exception);//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails as a result instead of _diskmap
            }
        }

        private object InterpretPerfFormattedPerfDiskObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            DiskStatistics result = null;

            var name = (string)newObject["Name"];
            var lowname = name.TrimEnd(':', '\\').ToLower();
            // find the stat object in the disk map))
            foreach (var disk in driveStatisticsProbeDetails.DiskMap.Values) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
            {
                if (!disk.ContainsPath(lowname)) continue;
                result = disk;
                break;
            }

            if (result == null)
            {
                return null;
            }
            result.PercentFreeSpace = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject, "PercentFreeSpace");
            result.PercentIdleTime = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject, "PercentIdleTime");
            result.PercentIdleTime_Base = -1;
            result.TotalSize = (100 * result.FreeSpace) / result.PercentFreeSpace;

            return result;
        }

        private void RawPerfDiskCallback(object sender, CollectorCompleteEventArgs e)
        {
            using (LOG.VerboseCall("RawPerfDiskCallback"))
            {
                _collector.Results.Clear();

                List<string> lowerPathNames = null;
                var deleteme = new List<string>();
                foreach (var entry in driveStatisticsProbeDetails.DiskMap) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                {
                    if ((!AutoDiscovery && DriveList.Count > 0) && !_returnAll)//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Avoid filtering in case of returnAll=true;
                    {
                        if (entry.Value.Paths.Count == 1)
                        {
                            var path = entry.Value.Paths[0].ToUpper();
                            if (!Algorithms.Exists<string>(DriveList, t => String.Equals(t.TrimEnd(':','\\').ToUpper(), path)))
                            {
                                deleteme.Add(entry.Key);
                            }
                        }
                        else
                        {
                            if (lowerPathNames == null)
                            {
                                lowerPathNames = new List<string>();
                                foreach (var drive in DriveList)
                                    lowerPathNames.Add(drive.ToLower());
                            }
                            if (!entry.Value.Prune(lowerPathNames))
                            {
                                deleteme.Add(entry.Key);
                            }
                        }
                    }
                    else
                        if (entry.Value.DriveType == 4 || entry.Value.DriveType == 5)
                            deleteme.Add(entry.Key);
                        else
                            if (String.IsNullOrEmpty(entry.Value.FileSystem))
                                deleteme.Add(entry.Key);
                }

                var b = new StringBuilder();
                foreach (var entryKey in deleteme)
                {
                    if (b.Length > 0)
                        b.Append(",");
                    b.Append(entryKey);
                    driveStatisticsProbeDetails.DiskMap.Remove(entryKey); //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                }
                if (b.Length > 0)
                    LOG.Verbose("Filtering disks from list: " + b.ToString());

                WMIServiceTimedout = _collector.WMITimedout;
                WMIStatisticsAvailable = WMIServiceTimedout ? false : e.Result == Result.Success;
                if (_cookedCounterValue > 0)
                {
                    StartPerfFormattedPerfDiskCollector();
                }
                else
                {
                    FireCompletion(driveStatisticsProbeDetails, e.Result, e.Exception);//SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails as a result instead of _diskmap
                }
            }
        }

        private object InterpretRawPerfDiskObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            DiskStatistics result = null;

            var name = (string)newObject["Name"];
            var lowname = name.TrimEnd(':','\\').ToLower();
            // find the stat object in the disk map))
            foreach (var disk in driveStatisticsProbeDetails.DiskMap.Values) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
            {
                if (!disk.ContainsPath(lowname)) continue;
                result = disk;
                break;
            }

            if (result == null && IncludeAll)
            {
                // use the logical drive name as the device id
                //Added the try get value check to resolve rally issue DE40261. Aditya Shukla SQLdm 8.6.
                if (!driveStatisticsProbeDetails.DiskMap.TryGetValue(name, out result)) //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                {
                    result = new DiskStatistics(name);
                    result.Paths.Add(name.TrimEnd(':'));
                    driveStatisticsProbeDetails.DiskMap.Add(name, result);  //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - driveStatisticsProbeDetails.DiskMap instead of _diskmap
                }
            }

            if (result != null)
            {
                result.AvgDiskQueueLength = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject,"AvgDiskQueueLength");
                result.AvgDisksecPerRead = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerRead");
                result.AvgDisksecPerRead_Base = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerRead_Base");
                result.AvgDisksecPerTransfer = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerTransfer");
                result.AvgDisksecPerTransfer_Base = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerTransfer_Base");
                result.AvgDisksecPerWrite = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerWrite");
                result.AvgDisksecPerWrite_Base = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"AvgDisksecPerWrite_Base");
                result.DiskReadsPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"DiskReadsPerSec");
                result.DiskTransfersPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"DiskTransfersPerSec");
                result.DiskWritesPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"DiskWritesPerSec");
                result.FreeMegabytes = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"FreeMegabytes");
                result.Frequency_Perftime = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject,"Frequency_Perftime");
                result.PercentFreeSpace_Base = WmiCollector.GetPropertyValueOrDefault<uint>(newObject,"PercentFreeSpace_Base");
                result.PercentIdleTime = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject,"PercentIdleTime");
                result.PercentIdleTime_Base = WmiCollector.GetPropertyValueOrDefault<int>(newObject,"PercentIdleTime_Base");
                result.TimeStamp_PerfTime = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject,"TimeStamp_PerfTime");
                result.TimeStamp_Sys100NS = WmiCollector.GetPropertyValueOrDefault<ulong>(newObject,"TimeStamp_Sys100NS");
            }

            return result;
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }

        internal class DiskStatistics
        {
            public DiskStatistics(string deviceId)
            {
                DeviceId = deviceId;
                Paths = new List<string>();
            }

            public string DeviceId { get; set; }
            public UInt32 DriveType { get; set; }
            public List<String> Paths { get; private set; }

            public UInt64 TotalSize { get; set; }
            public UInt64 FreeSpace { get; set; }

            public UInt64 AvgDiskQueueLength { get; set; }
            public UInt32 AvgDisksecPerRead { get; set; }
            public UInt32 AvgDisksecPerRead_Base { get; set; }
            public UInt32 AvgDisksecPerTransfer { get; set; }
            public UInt32 AvgDisksecPerTransfer_Base { get; set; }
            public UInt32 AvgDisksecPerWrite { get; set; }
            public UInt32 AvgDisksecPerWrite_Base { get; set; }
            public UInt32 DiskReadsPerSec { get; set; }
            public UInt32 DiskTransfersPerSec { get; set; }
            public UInt32 DiskWritesPerSec { get; set; }
            public UInt32 FreeMegabytes { get; set; }
            public UInt64 Frequency_Perftime { get; set; }
            public UInt32 PercentFreeSpace_Base { get; set; }
            public UInt64 PercentIdleTime { get; set; }
            public int PercentIdleTime_Base { get; set; }
            public UInt64 TimeStamp_PerfTime { get; set; }
            public UInt64 TimeStamp_Sys100NS { get; set; }
            public string FileSystem { get; set; }
            public ulong PercentFreeSpace { get; set; }


            public bool ContainsPath(string lowerPathName)
            {
                foreach (var path in Paths)
                {
                    if (path.ToLower() == lowerPathName)
                        return true;
                }
                return false;
            }

            public bool Prune(IList<string> lowerPathNames)
            {
                var newList = new List<string>();
                foreach (var path in Paths)
                {
                    if (lowerPathNames.Contains(path.ToLower()))
                    {
                        newList.Add(path);
                    }
                }

                if (newList.Count > 0)
                {   // replace the list
                    Paths = newList;
                    return true;
                }
                // return false with original list
                return false;
            }
        }

        /// <summary>
        /// //SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - Added new class to Store sqlDiskFilter and DiskMap as a result fo driveStatisticsProbe
        /// </summary>
        internal class DriveStatisticsWmiDetails
        {
            private Dictionary<string, DiskStatistics> _diskMap;

            internal Dictionary<string, DiskStatistics> DiskMap
            {
                get { return _diskMap; }
                set { _diskMap = value; }
            }

            private string _diskSqlFilter;

            public string DiskSqlFilter
            {
                get { return _diskSqlFilter; }
                set { _diskSqlFilter = value; }
            }
        }
    }
}
