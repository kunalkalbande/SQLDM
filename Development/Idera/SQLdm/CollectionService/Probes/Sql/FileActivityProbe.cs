//------------------------------------------------------------------------------
// <copyright file="FileActivityProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.CollectionService.Probes.Wmi;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;
    using System.Collections.Generic;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class FileActivityProbe : SqlBaseProbe
    {
        #region fields

        private FileActivitySnapshot fileActivity = null;
        private FileActivityConfiguration config = null;
        private DiskCollectionSettings diskSettings;
        private TimeSpan? UTCdelta = null;

        private WmiConfiguration wmiConfig;
        private string machineName;
        private DriveStatisticsWmiProbe driveProbe;

        private Dictionary<string, FileActivityFile> fileMap;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileActivityProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public FileActivityProbe(SqlConnectionInfo connectionInfo, FileActivityConfiguration config, WmiConfiguration wmiConfiguration, DiskCollectionSettings diskSettings,int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("FileActivityProbe");
            this.cloudProviderId = cloudProviderId;
            fileActivity = new FileActivitySnapshot(connectionInfo);
            this.config = config;
            this.diskSettings = diskSettings;
            this.wmiConfig = wmiConfiguration;
        }

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                StartFileActivityCollector();
            }
            else
            {
                FireCompletion(fileActivity, Result.Success);
            }
        }

        /// <summary>
        /// Define the FileActivity collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FileActivityCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildFileActivityCommand(conn, ver, wmiConfig, diskSettings);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FileActivityCallback));
        }

        /// <summary>
        /// Starts the File Activity collector.
        /// </summary>
        private void StartFileActivityCollector()
        {
            StartGenericCollector(new Collector(FileActivityCollector), fileActivity, "StartFileActivityCollector", "File Activity", null, new object[] { });
        }

        /// <summary>
        /// Define the FileActivity callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FileActivityCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFileActivity(rd);
            }

            if (wmiConfig.DirectWmiEnabled)
                StartMachineNameCollector();//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe before calling the DiskDriveColector
            else
                FireCompletion(fileActivity, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FileActivity collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FileActivityCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FileActivityCallback), fileActivity, "FileActivityCallback", "File Activity",
                            sender, e);
        }

        /// <summary>
        /// Interpret FileActivity data
        /// </summary>
        private void InterpretFileActivity(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFileActivity"))
            {
                var directWmi = wmiConfig.DirectWmiEnabled;
                try
                {
                    if (config.PreviousValues != null)
                    {

                        if (config.PreviousValues.ServerStartupTime != fileActivity.ServerStartupTime)
                        {    
                            config.PreviousValues = null;
                        }
                        else
                        {
                            UTCdelta = fileActivity.TimeStamp - config.PreviousValues.TimeStamp;
                        }
                    }

                    dataReader.Read();
                    if (!dataReader.IsDBNull(0))
                    {
                        if (directWmi)
                        {
                            //machineName = dataReader.GetString(0);//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get this value
                            fileActivity.OsStatisticAvailability = "service unavailable";
                        }
                        else
                            fileActivity.OsStatisticAvailability = dataReader.GetString(0);
                    }

                    if (!directWmi)
                    {
                        dataReader.NextResult();
                        ReadDisks(dataReader);
                        CalculateDiskDrives();
                        dataReader.NextResult();
                        ReadFiles(dataReader);
                    }
                    else
                    {
                        dataReader.NextResult();
                        ReadFilesDirectWmi(dataReader);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(fileActivity, LOG, "Error interpreting File Activity Collector: {0}", e, false);
                    GenericFailureDelegate(fileActivity);
                }
            }
        }

        private void ReadDisks(SqlDataReader dr)
        {
            try
            {
                while (dr.Read())
                {
                    if (!dr.IsDBNull(0))
                    {
                        FileActivityDisk drive = new FileActivityDisk();
                        drive.DriveLetter = dr.GetString(0);
                        if (!dr.IsDBNull(1)) drive.DiskReadsPerSec_Raw = double.Parse(dr.GetString(1));
                        if (!dr.IsDBNull(2)) drive.DiskWritesPerSec_Raw = double.Parse(dr.GetString(2));
                        if (!dr.IsDBNull(3)) drive.DiskTransfersPerSec_Raw = double.Parse(dr.GetString(3));
                        if (!dr.IsDBNull(4)) drive.Frequency_Perftime = double.Parse(dr.GetString(4));
                        if (!dr.IsDBNull(5)) drive.TimeStamp_PerfTime = double.Parse(dr.GetString(5));
                        fileActivity.Drives.Add(drive.DriveLetter,drive);   
                        
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(fileActivity, LOG, "Error reading disks: {0}", e,
                                                    false);
                GenericFailureDelegate(fileActivity);
            }
        }

        internal void CalculateDiskDrives()
        {
            try
            {
                if (config != null && config.PreviousValues != null && config.PreviousValues.Drives != null)
                {
                    foreach (FileActivityDisk dd in fileActivity.Drives.Values)
                    {
                        if (config.PreviousValues.Drives.ContainsKey(dd.DriveLetter))
                        {
                            double? delta; // throw away
                            double? perftimeDelta;
                            bool breakOut;

                            perftimeDelta = OSMetrics.Calculate_Timer_Delta(
                                config.PreviousValues.Drives[dd.DriveLetter].TimeStamp_PerfTime,
                                dd.TimeStamp_PerfTime);

                            breakOut = !(perftimeDelta.HasValue && perftimeDelta.Value > 0);

                            if (!breakOut)
                            {
                                if (!UTCdelta.HasValue ||
                                    (perftimeDelta / dd.Frequency_Perftime) >= UTCdelta.Value.TotalSeconds * 1.25)
                                {
                                    breakOut = true;
                                }
                            }

                            if (!breakOut)
                            {


                                dd.DiskReadsPerSec =
                                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(
                                        config.PreviousValues.Drives[dd.DriveLetter].DiskReadsPerSec_Raw,
                                        dd.DiskReadsPerSec_Raw,
                                        out delta,
                                        perftimeDelta,
                                        dd.Frequency_Perftime);

                                dd.DiskWritesPerSec =
                                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(
                                        config.PreviousValues.Drives[dd.DriveLetter].DiskWritesPerSec_Raw,
                                        dd.DiskWritesPerSec_Raw,
                                        out delta,
                                        perftimeDelta,
                                        dd.Frequency_Perftime);

                                dd.DiskTransfersPerSec =
                                    OSMetrics.Calculate_PERF_COUNTER_COUNTER(
                                      config.PreviousValues.Drives[dd.DriveLetter].DiskTransfersPerSec_Raw,
                                      dd.DiskTransfersPerSec_Raw,
                                      out delta,
                                      perftimeDelta,
                                      dd.Frequency_Perftime);

                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(fileActivity, LOG, "Error interpreting disks: {0}", e,
                                                    false);
                GenericFailureDelegate(fileActivity);
            }
        }

        private void ReadFilesDirectWmi(SqlDataReader dr)
        {
            try
            {
                fileMap = new Dictionary<string, FileActivityFile>();
                while (dr.Read())
                {
                    if (!dr.IsDBNull(6)) // skip null file path
                    {
                        var file = new FileActivityFile();
                        if (!dr.IsDBNull(0)) file.DatabaseName = dr.GetString(0);
                        if (!dr.IsDBNull(1)) file.ReadsRaw = dr.GetDecimal(1);
                        if (!dr.IsDBNull(2)) file.WritesRaw = dr.GetDecimal(2);
                        if (!dr.IsDBNull(3)) file.DriveName = dr.GetString(3);
                        if (!dr.IsDBNull(4)) file.Filename = dr.GetString(4);
                        if (!dr.IsDBNull(5)) file.FileType = (FileActivityFileType)dr.GetInt32(5);
                        file.Filepath = dr.GetString(6);

                        if (!fileMap.ContainsKey(file.Filepath))
                            fileMap.Add(file.Filepath, file);
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(fileActivity, LOG, "Error reading files: {0}", e, false);
                GenericFailureDelegate(fileActivity);
            }
        }

        private void ReadFiles(SqlDataReader dr)
        {
            try
            {
                while (dr.Read())
                {
                    if (!dr.IsDBNull(3) && !dr.IsDBNull(6))
                    {
                        string drive = dr.GetString(3);
                        if (fileActivity.Drives.ContainsKey(drive))
                        {
                            FileActivityFile file = new FileActivityFile();
                            if (!dr.IsDBNull(0)) file.DatabaseName = dr.GetString(0);
                            if (!dr.IsDBNull(1)) file.ReadsRaw = dr.GetDecimal(1);
                            if (!dr.IsDBNull(2)) file.WritesRaw = dr.GetDecimal(2);
                            if (!dr.IsDBNull(4)) file.Filename = dr.GetString(4);
                            if (!dr.IsDBNull(5)) file.FileType = (FileActivityFileType) dr.GetInt32(5);
                            file.Filepath = dr.GetString(6);
                            file.DriveName = drive;
                            CalculateFile(drive, file);
                            if (!fileActivity.Drives[drive].Files.ContainsKey(file.Filepath))
							{
                                fileActivity.Drives[drive].Files.Add(file.Filepath,file);
							}
                            else
                            {
                                LOG.ErrorFormat("An error has occurred while adding {0} (this Key already exists) on drive {1}.", file.Filepath, drive);
                            }
                        }
                    }

                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(fileActivity, LOG, "Error reading files: {0}", e,
                                                    false);
                GenericFailureDelegate(fileActivity);
            }
        }

        private void CalculateFile(string drive , FileActivityFile file)
        {
            if (config != null 
                && config.PreviousValues != null 
                && config.PreviousValues.Drives != null 
                && config.PreviousValues.Drives.ContainsKey(drive) 
                && config.PreviousValues.Drives[drive].Files != null
                && config.PreviousValues.Drives[drive].Files.ContainsKey(file.Filepath)
                && UTCdelta.HasValue
                )
            {
                FileActivityFile prevActivity = config.PreviousValues.Drives[drive].Files[file.Filepath];
                if (file.ReadsRaw.HasValue && prevActivity.ReadsRaw.HasValue && file.ReadsRaw.Value >= prevActivity.ReadsRaw.Value)
                {
                    file.ReadsPerSec =
                        (double?)
                        ((file.ReadsRaw.Value - prevActivity.ReadsRaw.Value)/
                         (decimal) UTCdelta.Value.TotalSeconds);
                }

                if (file.WritesRaw.HasValue && prevActivity.WritesRaw.HasValue && file.WritesRaw.Value >= prevActivity.WritesRaw.Value)
                {
                    file.WritesPerSec =
                        (double?)
                        ((file.WritesRaw.Value - prevActivity.WritesRaw.Value)/
                         (decimal) UTCdelta.Value.TotalSeconds);
                }

            }            
        }

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for disk statistics which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo,cloudProviderId);
            machineProbe.BeginProbe(MachineNameCallback);
        }

        /// <summary>
        /// Machine Name Collector Call back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MachineNameCallback(object sender, SnapshotCompleteEventArgs e)
        {
            // This means we cancelled out the probe
            if (e.Result == Result.Failure)
                return;
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe
            StartDriveStatisticsProbe();
        }
		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        private void StartDriveStatisticsProbe()
        {
            driveProbe = new DriveStatisticsWmiProbe(machineName, wmiConfig, diskSettings);
            driveProbe.AutoDiscovery = diskSettings.AutoDiscover;
            if (!diskSettings.AutoDiscover && diskSettings.Drives != null && diskSettings.Drives.Length > 0)
            {
                driveProbe.IncludeAll = false;
                driveProbe.DriveList.AddRange(diskSettings.Drives);
            }
            driveProbe.BeginProbe(OnDiskStatsComplete);
        }

        private void OnDiskStatsComplete(object sender, ProbeCompleteEventArgs args)
        {
            if (driveProbe.WMIStatisticsAvailable)
            {
                fileActivity.OsStatisticAvailability = "available";
            }
            else if (driveProbe.WMIServiceTimedout)
            {
                fileActivity.OsStatisticAvailability = "service timedout";
            }

            //START SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type
            var probedata = args.Data as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;
            if (probedata != null && probedata.DiskMap != null && probedata.DiskMap.Count > 0)
            {
                AddDiskStatistics(probedata.DiskMap.Values);
            }
            //END SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - replaced older object with the new object type

            FireCompletion(fileActivity, args.Result);
        }

        private void AddDiskStatistics(IEnumerable<DriveStatisticsWmiProbe.DiskStatistics> valueCollection)
        {
            // blow out the mount points for each volume
            foreach (var disk in valueCollection)
            {
                // skip drive types 4 () & 5
                if (disk.DriveType == 4 || disk.DriveType == 5) continue;

                foreach (var mount in disk.Paths)
                {
                    var volume = new FileActivityDisk();
                    volume.DriveLetter = mount;
                    volume.DiskReadsPerSec_Raw = disk.DiskReadsPerSec;
                    volume.DiskWritesPerSec_Raw = disk.DiskWritesPerSec;
                    volume.DiskTransfersPerSec_Raw = disk.DiskTransfersPerSec;
                    volume.Frequency_Perftime = disk.Frequency_Perftime;
                    volume.TimeStamp_PerfTime = disk.TimeStamp_PerfTime;
                    fileActivity.Drives.Add(volume.DriveLetter, volume);
                }
            }

            CalculateDiskDrives();

            MapFilesToDisks();
        }

        private void MapFilesToDisks()
        {
            var driveList = new List<string>(fileActivity.Drives.Keys);
            driveList.Sort(DriveComparison);

            foreach (var file in fileMap.Values)
            {
                var drive = MatchDrive(driveList, file);
                if (String.IsNullOrEmpty(drive)) continue;
                file.DriveName = drive;
                CalculateFile(drive, file);
                if (!fileActivity.Drives[drive].Files.ContainsKey(file.Filepath))
                {
                    fileActivity.Drives[drive].Files.Add(file.Filepath, file);
                }
            }
        }

        private int DriveComparison(string left, string right)
        {
            return string.Compare(left, right, true);
        }

        private string MatchDrive(List<string> driveList, FileActivityFile file)
        {
            var filepath = file.Filepath;
            for (var i = driveList.Count - 1; i >= 0; i--)
            {
                if (filepath.StartsWith(driveList[i],StringComparison.CurrentCultureIgnoreCase))
                    return driveList[i];
            }

            // file does not match a mount point
            return null;
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
