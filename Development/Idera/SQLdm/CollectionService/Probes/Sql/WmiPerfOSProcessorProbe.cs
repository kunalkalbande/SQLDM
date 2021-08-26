using System;
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
    class WmiPerfOSProcessorProbe: SqlBaseProbe
    {
        #region fields

        private WmiPerfOSProcessorSnapshot snapshot = null;
        private WmiConfiguration configuration = null;

        private string machineName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiPerfOSProcessorProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public WmiPerfOSProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            LOG = Logger.GetLogger("WmiPerfOSProcessorProbe");
            snapshot = new WmiPerfOSProcessorSnapshot(connectionInfo.InstanceName);
            this.configuration = configuration;            
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

            // start the next probe 
            StartDirectCollector();
        }


        private void StartDirectCollector()
        {
            var opts = WmiCollector.CreateConnectionOptions(machineName, configuration, out impersonation);
            _wmicollector = new WmiCollector(machineName, opts, impersonation, true);//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            _wmicollector.Query = new WqlObjectQuery("select Name, InterruptsPerSec, PercentProcessorTime, PercentPrivilegedTime, PercentInterruptTime, Frequency_Sys100NS, Timestamp_Sys100NS from Win32_PerfRawData_PerfOS_Processor");

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

            LOG.VerboseFormat("Service collector ran in {0} milliseconds.", e.ElapsedMilliseconds);

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
                            var interruptsPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "InterruptsPerSec");
                            var percentProcessorTime = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "PercentProcessorTime");
                            var percentPrivilegedTime = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "PercentPrivilegedTime");
                            var percentInterruptTime = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "PercentInterruptTime");
                            var frequency_Sys100NS = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Frequency_Sys100NS");
                            var timestamp_Sys100NS = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Timestamp_Sys100NS");
                            snapshot.WmiPerfOSProcessor.Rows.Add(name, interruptsPerSec, percentProcessorTime, percentPrivilegedTime, percentInterruptTime, frequency_Sys100NS, timestamp_Sys100NS);
                        }
                        catch (Exception ex) { LOG.Error(ex); }
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
