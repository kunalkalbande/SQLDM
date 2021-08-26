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
    class WmiTCPv6Probe: SqlBaseProbe
    {
        #region fields

        private WmiTCPv6Snapshot snapshot = null;
        private WmiConfiguration configuration = null;

        private string machineName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiTCPv6Probe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Cloud Provider server information</param>
        public WmiTCPv6Probe(SqlConnectionInfo connectionInfo, WmiConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("WmiTCPv6Probe");
            snapshot = new WmiTCPv6Snapshot(connectionInfo.InstanceName);
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
            LOG.Info("TCPv6Probe's machinnamecallback");
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
            LOG.Info("TCPv6Probe's starting direct collector");
            var opts = WmiCollector.CreateConnectionOptions(machineName, configuration, out impersonation);
            _wmicollector = new WmiCollector(machineName, opts, impersonation, true); //Start: SQLdm 10.0.2 -Praveen Suhalka - RunAnalysis issue fix
            _wmicollector.Query = new WqlObjectQuery("select SegmentsPersec, SegmentsRetransmittedPerSec from Win32_PerfRawData_Tcpip_TCPv6");

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
            LOG.Info("TCPv6Probe's starting direct wmi callback");

            LOG.VerboseFormat("wmiTCPv6 collector ran in {0} milliseconds.", e.ElapsedMilliseconds);

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
                            var segmentsPersec = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "SegmentsPersec");
                            var segmentsRetransmittedPerSec = WmiCollector.GetPropertyValueOrDefault<uint>(mbo, "SegmentsRetransmittedPerSec");
                            snapshot.WmiTCP.Rows.Add(segmentsPersec, segmentsRetransmittedPerSec);
                        }
                        catch (Exception ex)
                        { LOG.Info("TCPv6Probe's starting direct wmi callback Error Occurred"); 
                            LOG.Error(ex); }
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
