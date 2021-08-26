﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Management;
using System.Text;
using System.Threading;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.CollectionService.Probes.Wmi;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    class WmiVolumeProbe: SqlBaseProbe
    {
        #region fields

        private WmiVolumeSnapshot snapshot = null;
        private WmiConfiguration configuration = null;

        private string machineName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiVolumeProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        public WmiVolumeProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("WmiVolumeProbe");
            snapshot = new WmiVolumeSnapshot(connectionInfo.InstanceName);
            this.configuration = configuration;
            this.cloudProviderId = cloudProviderId;
        }

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            StartMachineNameCollector();
        }


        /// <summary>
        /// Starts the Machine Name collector.
        /// before the WMI probe for Service Status which needs the machine name 
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
            {
                //Vineet DM10.0.2 -- Fixing analysis issue, it was not sending firecomplete in this block. Fixed the same.
                if (snapshot != null)
                    snapshot.SetError("Error executing inner collector (Machine Name Collector): ", e.Snapshot == null ? new Exception("Machine Name Snapshot Was NUll") : e.Snapshot.Error);
                FireCompletion(snapshot, Result.Failure);
            }
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartDirectCollector();
        }


        private void StartDirectCollector()
        {
            var opts = WmiCollector.CreateConnectionOptions(machineName, configuration, out impersonation);
            _wmicollector = new WmiCollector(machineName, opts, impersonation, true);//Start: SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            _wmicollector.Query = new WqlObjectQuery("select Name, BlockSize, FileSystem from Win32_Volume");

            try
            {
                if (impersonation != null)
                {
                    // login user and get windows identity
                    if (!impersonation.IsLoggedOn)
                        impersonation.LogonUser();
                }

                _wmicollector.BeginCollection(DirectWmiCallback, InterpretObject, null);
            }
            catch (Exception e)
            {
                snapshot.SetError("Error validating WMI connection: ", e);
                FireCompletion(snapshot, Result.Failure);
            }
        }

        private void DirectWmiCallback(object sender, CollectorCompleteEventArgs e)
        {

            LOG.VerboseFormat("WmiVolume collector ran in {0} milliseconds.", e.ElapsedMilliseconds);

            if (e.Result != Result.Success)
            {
                snapshot.SetError("Error validating WMI connection: ", e.Exception);
            }
            else
            {
                var result = e.Value as IList;
                if (result != null && result.Count > 0)
                {
                    foreach (ManagementBaseObject mbo in result)
                    {
                        try
                        {
                            var name = WmiCollector.GetReferencePropertyValue<string>(mbo, "Name");
                            if (String.IsNullOrEmpty(name)) continue;
                            var filesystem = WmiCollector.GetReferencePropertyValue<string>(mbo, "FileSystem");
                            var blockSize = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "BlockSize");
                            snapshot.WmiVolume.Rows.Add(name, filesystem, blockSize);
                        }
                        catch (Exception ex)
                        {
                            LOG.Error(ex);
                        }
                    }
                }
            }

            FireCompletion(snapshot, e.Result);
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }

        #endregion

    }
}
