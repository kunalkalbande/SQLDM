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
    class WmiProcessorProbe : SqlIntervalProbe
    {
        #region fields

        private WmiProcessorSnapshot snapshot = null;
        private WmiConfiguration configuration = null;

        private string machineName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiProcessorProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public WmiProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo, cloudProviderId)
        {
            LOG = Logger.GetLogger("WmiProcessorProbe");
            this.cloudProviderId = cloudProviderId;
            snapshot = new WmiProcessorSnapshot(connectionInfo.InstanceName);
            this.configuration = configuration;            
        }

        public WmiProcessorProbe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int interval, int max, int? cloudProviderId)
            : base(connectionInfo, interval, max, cloudProviderId)
        {
            LOG = Logger.GetLogger("WmiProcessorProbe");
            snapshot = new WmiProcessorSnapshot(connectionInfo.InstanceName);
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

            // start the next probe 
            StartDirectCollector();
        }


        private void StartDirectCollector()
        {
            var opts = WmiCollector.CreateConnectionOptions(machineName, configuration, out impersonation);
            _wmicollector = new WmiCollector(machineName, opts, impersonation, true);//SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            _wmicollector.Query = new WqlObjectQuery("select AddressWidth, NumberOfLogicalProcessors, NumberOfCores, CurrentClockSpeed, MaxClockSpeed from Win32_Processor");

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
            dt.Columns.Add("NumberOfLogicalProcessors", typeof(uint));
            dt.Columns.Add("NumberOfCores", typeof(uint));
            dt.Columns.Add("CurrentClockSpeed", typeof(uint));
            dt.Columns.Add("MaxClockSpeed", typeof(uint));
            dt.Columns.Add("AddressWidth", typeof(uint));
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
                            var addressWidth = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "AddressWidth");
                            var numberOfLogicalProcessors = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "NumberOfLogicalProcessors");
                            var numberOfCores = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "NumberOfCores");
                            var currentClockSpeed = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "CurrentClockSpeed");
                            var maxClockSpeed = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "MaxClockSpeed");
                            dt.Rows.Add(numberOfLogicalProcessors, numberOfCores, currentClockSpeed, maxClockSpeed, addressWidth);
                        }
                        catch (Exception ex) { LOG.Error(ex); }
                    }
                 
                }
            }
            if (IsInterval)
            {
                if (snapshot.ListWmiProcessor == null)
                    snapshot.ListWmiProcessor = new System.Collections.Generic.List<DataTable>();
                snapshot.ListWmiProcessor.Add(dt);
                ExecutionCount++;
            }
            else
                snapshot.WmiProcessor = dt;
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
