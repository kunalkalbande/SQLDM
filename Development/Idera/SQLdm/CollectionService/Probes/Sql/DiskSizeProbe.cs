
//------------------------------------------------------------------------------
// <copyright file="DiskSizeProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;
using System.Globalization;
using System.Threading;

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
    using System.Management;
    using System.Collections;
using Idera.SQLdm.CollectionService.Probes.Wmi;
    using System.Collections.Generic;


    /// <summary>
    /// SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements - New Probe class
    /// </summary>
    internal sealed class DiskSizeProbe : SqlBaseProbe
    {
        #region fields

        private DiskSizeDetails _diskSize = null;
        private DatabaseProbeConfiguration _dbConfig = null;
        private WmiConfiguration _wmiConfig = null;
        private WmiCollector wmiCollector;
        string _machineName;
        private DriveStatisticsWmiProbe _driveProbe;
        private object _driveProbeData;
        private DiskCollectionSettings _diskSettings;



        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskSizeSnapshot"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public DiskSizeProbe(SqlConnectionInfo connectionInfo, DatabaseProbeConfiguration config, WmiConfiguration wmiConfig, DiskCollectionSettings diskSettings, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("DiskSizeProbe");
            _diskSize = _diskSize?? new DiskSizeDetails(connectionInfo);
            _dbConfig = config;
            _wmiConfig = wmiConfig;
            _diskSettings = diskSettings;
            this.cloudProviderId = cloudProviderId;
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
            if (_dbConfig != null && _dbConfig.ReadyForCollection && cloudProviderId!= CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {

                StartMachineNameCollector();
            }
            else
            {
                FireCompletion(_diskSize, Result.Success);
            }
        }

        #region Direct WMI Collection

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for disk statistics which needs the machine name 
        /// </summary>
        private void StartMachineNameCollector()
        {
            // Passing CloudProvider information
            MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
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

            if (e.Result == Result.PermissionViolation)
            {
                this._diskSize.ProbeError = e.Snapshot.ProbeError;
                this.FireCompletion(this._diskSize, Result.PermissionViolation);
                ((IDisposable)sender).Dispose();
                return;
            }

            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    _machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe
            StartDiskDriveCollector();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
        /// <summary>
        /// When doing direct wmi, the database summary probe needs disk size information to use in expansion calculations.
        /// We have to collect the disk size info first and poke it into the temp table (#disk_drives) that the batch uses.
        /// </summary>
        private void StartDiskDriveCollector()
        {
            _driveProbe = new DriveStatisticsWmiProbe(_machineName, _wmiConfig, _diskSettings);
            _driveProbe.AutoDiscovery = true; // Enforcing it to discover all as per the requirement.
            _driveProbe.IncludeAll = true;
            _driveProbe.BeginProbe(OnDiskStatsComplete);
        }

        private void OnDiskStatsComplete(object sender, ProbeCompleteEventArgs args)
        {
            //replaced older object with the new object type
            var probedata = args.Data as DriveStatisticsWmiProbe.DriveStatisticsWmiDetails;
            if (probedata != null && probedata.DiskMap != null && probedata.DiskMap.Count > 0)
            {
                AddDiskStatistics(probedata.DiskMap.Values);
            }

            _driveProbeData = probedata;

            // start the next probe
            StartFileGroupCollector();
        }

        private void AddDiskStatistics(IEnumerable<DriveStatisticsWmiProbe.DiskStatistics> valueCollection)
        {
            var diskDrives = new Dictionary<string, DiskDriveStatistics>();
            var previousDiskDrives = new Dictionary<string, DiskDriveStatistics>();// = (_dbConfig.PreviousValues != null) ? _dbConfig.PreviousValues.DiskDrives : null;

            // blow out the mount points for each volume
            foreach (var disk in valueCollection)
            {
                if (disk.DriveType == 4 || disk.DriveType == 5) continue;

                foreach (var mount in disk.Paths)
                {
                    var dd = new DiskDriveStatistics();
                    dd.DriveName = mount;
                    dd.UnusedSizeKB = disk.FreeSpace/1024;
                    dd.TotalSizeKB = disk.TotalSize/1024;
                    dd.DiskReadsPerSecond = disk.DiskReadsPerSec;
                    dd.DiskWritesPerSecond = disk.DiskWritesPerSec;
                    

                    diskDrives.Add(dd.DriveName, dd);
                }
            }

            _diskSize.DiskDrives = diskDrives;
        }

        #endregion

        /// <summary>
        /// Define the Filegroup collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FileGroupCollector(SqlConnection conn, SqlCollector sdtCollector,ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildFileGroupCommand(conn, ver,_dbConfig, _wmiConfig, _diskSettings, _driveProbeData,cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FileGroupCallback));
        }

        /// <summary>
        /// Starts the File group collector.
        /// </summary>
        private void StartFileGroupCollector()
        {
            //Arg params for DiskSize2005
            var driveProbeDataParam = (DriveStatisticsWmiProbe.DriveStatisticsWmiDetails)_driveProbeData;
            bool isWMICallFailed = (driveProbeDataParam == null || driveProbeDataParam.DiskMap == null || driveProbeDataParam.DiskMap.Count == 0);

            StartGenericCollector(new Collector(FileGroupCollector), _diskSize, "StartFileGroupCollector", "File Group", FileGroupCallback, new object[] { isWMICallFailed });
        }

        /// <summary>
        /// Define the Filegroup callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FileGroupCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFileGroup(rd);
            }
            FireCompletion(_diskSize, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the Filegroup collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FileGroupCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(FileGroupCallback), _diskSize, "FileGroupCallback", "File Group",
                            sender, e);
        }

        /// <summary>
        /// Interpret Filegroup data
        /// </summary>
        private void InterpretFileGroup(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFileGroup"))
            {
                try
                {
                    var drives = _diskSize.DiskDrives;
                    if (drives == null || drives.Count == 0)
                    {
                        // SQLDM - 25664 (Varun Chopra) No Information Displayed in Resources / Disk Size
                        drives = InterpretDiskDrives(dataReader);
                        dataReader.NextResult();
                    }
                    else
                    {
                        if (dataReader.Read())
                           dataReader.NextResult();
                    }
                    while (dataReader.Read())
                    {
                        string driveName = Convert.ToString(dataReader["DriveName"]);
                        if (!String.IsNullOrEmpty(driveName) && drives.ContainsKey(driveName))
                        {
                            var drive = drives[driveName];

                            var AvailableSpace = dataReader["AvailableSpaceMB"] == DBNull.Value ? 0 : Convert.ToDouble(dataReader["AvailableSpaceMB"]);
                            var InitialSize = dataReader["InitialSizeMB"] == DBNull.Value ? 0 : Convert.ToDouble(dataReader["InitialSizeMB"]);
                            var UsedSpace = dataReader["SpaceUsedMB"] == DBNull.Value ? 0 : Convert.ToDouble(dataReader["SpaceUsedMB"]);
                            var isDataFile = dataReader["IsDataFile"] != DBNull.Value && Convert.ToBoolean(dataReader["IsDataFile"]);
                            if (isDataFile)
                            {
                                drive.SQLDataFreeMB = (drive.SQLDataFreeMB == null) ? AvailableSpace : (drive.SQLDataFreeMB + AvailableSpace);
                                drive.SQLDataUsedMB = (drive.SQLDataUsedMB == null) ? UsedSpace : (drive.SQLDataUsedMB + UsedSpace);
                            }
                            else
                                drive.SQLLogFileMB = (drive.SQLLogFileMB == null) ? InitialSize : (drive.SQLLogFileMB + InitialSize);

                            drives[driveName] = drive;

                            LOG.DebugFormat("DriveName: {0} Value - AvailableSpaceMB: {1} Value - InitialSizeMB: {2} Value - SpaceUsedMB: {3} Value - IsDataFile: {4} Value ",
                                               driveName, AvailableSpace, InitialSize, UsedSpace, isDataFile);

                        }
                        else
                        {
                            LOG.DebugFormat("The Drive name:  {0}  is null or empty", driveName);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(_diskSize, LOG, "Error interpreting Filegroup Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(_diskSize);
                }
            }
        }

        /// <summary>
        /// Interpret Disk Drives Information to get drive information from OLE
        /// </summary>
        /// <param name="dataReader">Data Reader</param>
        /// <returns>Dictionary with Disk Statistics Information</returns>
        /// <remarks>
        /// SQLDM - 25664 (Varun Chopra) No Information Displayed in Resources / Disk Size
        /// </remarks>
        private Dictionary<string, DiskDriveStatistics> InterpretDiskDrives(SqlDataReader dataReader)
        {
            Dictionary<string, DiskDriveStatistics> drives;
            using (LOG.DebugCall("InterpretDiskDrives"))
            {
                var diskDrives = new Dictionary<string, DiskDriveStatistics>();
                int colIndex;
                try
                {
                    while (dataReader.Read())
                    {
                        DiskDriveStatistics dds = new DiskDriveStatistics();

                        colIndex = dataReader.GetOrdinal("drive_letter");
                        if (!dataReader.IsDBNull(colIndex))
                        {
                            var driveName = dataReader.GetString(colIndex).Trim();
                            dds.DriveName = driveName;
                        }

                        colIndex = dataReader.GetOrdinal("unused_size");
                        if (!dataReader.IsDBNull(colIndex))
                        {
                            var unusedSizeBytes = dataReader.GetDecimal(colIndex);
                            dds.UnusedSizeKB = (double) (unusedSizeBytes / 1024);
                        }

                        colIndex = dataReader.GetOrdinal("total_size");
                        if (!dataReader.IsDBNull(colIndex))
                        {
                            var totalSizeBytes = dataReader.GetDecimal(colIndex);
                            dds.TotalSizeKB = (double) (totalSizeBytes / 1024);
                        }

                        colIndex = dataReader.GetOrdinal("DiskReadsPerSec");
                        if (!dataReader.IsDBNull(colIndex))
                        {
                            var diskReadsPerSec = double.Parse(dataReader.GetString(colIndex));
                            dds.DiskReadsPerSecond = diskReadsPerSec;
                        }
                        else
                        {
                            colIndex = dataReader.GetOrdinal("AvgDisksecPerRead");
                            if (!dataReader.IsDBNull(colIndex))
                            {
                                dds.DiskReadsPerSecond = double.Parse(dataReader.GetString(colIndex));
                            }
                            else
                            {
                                colIndex = dataReader.GetOrdinal("AvgDisksecPerRead_Base");
                                if (!dataReader.IsDBNull(colIndex))
                                {
                                    dds.DiskReadsPerSecond = double.Parse(dataReader.GetString(colIndex));
                                }
                            }
                        }

                        colIndex = dataReader.GetOrdinal("DiskWritesPerSec");
                        if (!dataReader.IsDBNull(colIndex))
                        {
                            var diskWritesPerSec = double.Parse(dataReader.GetString(colIndex));
                            dds.DiskWritesPerSecond = diskWritesPerSec;
                        }
                        else
                        {
                            colIndex = dataReader.GetOrdinal("AvgDisksecPerWrite");
                            if (!dataReader.IsDBNull(colIndex))
                            {
                                dds.DiskReadsPerSecond = double.Parse(dataReader.GetString(colIndex));
                            }
                            else
                            {
                                colIndex = dataReader.GetOrdinal("AvgDisksecPerWrite_Base");
                                if (!dataReader.IsDBNull(colIndex))
                                {
                                    dds.DiskReadsPerSecond = double.Parse(dataReader.GetString(colIndex));
                                }
                            }
                        }
                        diskDrives.Add(dds.DriveName, dds);
                    }
                }
                catch (Exception exception)
                {
                    LOG.ErrorFormat("Error in Disk Drives Callback InterpretDiskDrives: {0}",
                        exception);
                }
                finally
                {
                    drives = _diskSize.DiskDrives = diskDrives;
                }
            }
            return drives;
        }

        #endregion

        #region interface implementations

        #endregion

    }
}

