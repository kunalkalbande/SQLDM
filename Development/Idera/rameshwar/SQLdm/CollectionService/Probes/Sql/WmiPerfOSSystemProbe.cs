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
using System.Data;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    class WmiPerfOSSystemProbe : SqlIntervalProbe
    {
        #region fields

        private WmiPerfOSSystemSnapshot snapshot = null;
        private WmiConfiguration configuration = null;

        private string machineName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiPerfOSSystemProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public WmiPerfOSSystemProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo, cloudProviderId)
        {
            LOG = Logger.GetLogger("WmiPerfOSSystemProbe");
            snapshot = new WmiPerfOSSystemSnapshot(connectionInfo.InstanceName);
            this.configuration = configuration;
            this.cloudProviderId = cloudProviderId;
        }

        public WmiPerfOSSystemProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int interval, int max, int? cloudProviderId)
            : base(connectionInfo, interval, max, cloudProviderId)
        {
            LOG = Logger.GetLogger("WmiPerfOSSystemProbe");
            snapshot = new WmiPerfOSSystemSnapshot(connectionInfo.InstanceName);
            this.cloudProviderId = cloudProviderId;
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
            _wmicollector.Query = new WqlObjectQuery("select ProcessorQueueLength, ContextSwitchesPerSec, Frequency_Sys100NS, Timestamp_Sys100NS from Win32_PerfRawData_PerfOS_System");

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
            DataTable dt = new DataTable();
            dt.Columns.Add("ProcessorQueueLength", typeof(uint));
            dt.Columns.Add("ContextSwitchesPerSec", typeof(uint));
            dt.Columns.Add("Frequency_Sys100NS", typeof(ulong));
            dt.Columns.Add("Timestamp_Sys100NS", typeof(ulong));
                    
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
                            var processorQueueLength = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "ProcessorQueueLength");
                            var contextSwitchesPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "ContextSwitchesPerSec");
                            var frequency_Sys100NS = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Frequency_Sys100NS");
                            var timestamp_Sys100NS = WmiCollector.GetPropertyValueOrDefault<ulong>(mbo, "Timestamp_Sys100NS");
                            dt.Rows.Add(processorQueueLength, contextSwitchesPerSec, frequency_Sys100NS, timestamp_Sys100NS);
                        }
                        catch (Exception ex) { LOG.Error(ex); }
                    }
                }
            }
            if (IsInterval)
            {
                if (snapshot.ListWmiPerfOSSystem == null)
                    snapshot.ListWmiPerfOSSystem = new System.Collections.Generic.List<DataTable>();
                snapshot.ListWmiPerfOSSystem.Add(dt);
                ExecutionCount++;
            }
            else
                snapshot.WmiPerfOSSystem = dt;
            if (!IsInterval)
            {
                FireCompletion(snapshot, Result.Success);
                return;
            }
            Thread.Sleep(Interval * 1000);
            if (IsRunnable)
            {
                StartMachineNameCollector();
            }
            else
                FireCompletion(snapshot, Result.Success);
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }

        #endregion

    }
}
