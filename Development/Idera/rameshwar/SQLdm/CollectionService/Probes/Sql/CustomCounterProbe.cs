//------------------------------------------------------------------------------
// <copyright file="CustomCounterProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Idera.SQLdm.CollectionService.Monitoring;
using Idera.SQLdm.Common.Events;
using Idera.SQLdm.Common.VMware;
using Idera.SQLdm.Services.Common.Probes.Azure;
using Vim25Api;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Management;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Snapshots;
    using Common.Services;

    using Wintellect.PowerCollections;
    using Idera.SQLdm.CollectionService.Probes.Wmi;
    using Idera.SQLdm.Common.HyperV;
    using Common.Events.AzureMonitor;


    /// <summary>
    /// Custom Counter Collection
    /// </summary>
    internal sealed class CustomCounterProbe : SqlBaseProbe, IOnDemandProbe
    {
        #region fields
        private static Dictionary<Pair<string, string>, object> vmcache = new Dictionary<Pair<string, string>, object>();

        private CustomCounterCollectionSnapshot returnSnapshot = null;
        private List<CustomCounterSnapshot> counterList = new List<CustomCounterSnapshot>();
        private Stopwatch sw = new Stopwatch();
        private int index = -1;

        private IOnDemandContext context;
        private TimeSpan timeout = TimeSpan.FromSeconds(80);
        private Timer collectionTimer;
        private Timer cancelTimer;
        private SqlCollector currentCollector;
        private bool hasExited = false;

        private VirtualizationConfiguration _virtualizationConfiguration;
        private ServiceUtil _vmServices;
        private ManagedObjectReference _vmMoRef;
        private ManagedObjectReference _esxMoRef;
//        private PerfMetricId[] _vmMetricIds;
//        private PerfMetricId[] _esxMetricIds;

        private WmiConfiguration wmiConfig;
        private string machineName;
        private WmiCollector wmiCollector;
        private ImpersonationContext impersonation;

        private TimeSpan _scheduledCollectionInterval;
        #endregion

        #region constructors


        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCounterProbe"/> class.
        /// </summary>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public CustomCounterProbe(SqlConnectionInfo connectionInfo, List<CustomCounterConfiguration> configs, TimeSpan timeout, WmiConfiguration wmiConfiguration, VirtualizationConfiguration virtualizationConfiguration, int? cloudProviderId, TimeSpan scheduledCollectionInterval)
            : base(connectionInfo)
        {
            this._scheduledCollectionInterval = scheduledCollectionInterval;
            // Subtract 5 seconds for cleanup if the timeout is greater than 10 seconds
            // These numbers are arbitrary and to prevent the context from timing out before the probe except in rapid refreshes
            this.timeout = timeout.TotalSeconds > 10 ? timeout.Subtract(TimeSpan.FromSeconds(5)) : timeout;
            
            LOG = Logger.GetLogger("CustomCounterProbe");
            // Skip permissions for CloudProviders
            this.cloudProviderId = cloudProviderId;
            returnSnapshot = new CustomCounterCollectionSnapshot(connectionInfo);

            foreach (CustomCounterConfiguration config in configs)
            {
                counterList.Add(InitializeCounter(config));
            }
            

            this.wmiConfig = wmiConfiguration;

            connectionInfo.ApplicationName = Constants.CustomCounterConnectionStringApplicationName;

            _virtualizationConfiguration = virtualizationConfiguration;
        }

        private CustomCounterSnapshot InitializeCounter(CustomCounterConfiguration config)
        {
            CustomCounterDefinition def;
            
            // Look up the definition if it's not a test
            if (config.CounterDefinition == null || config.MetricID >= 0)
            {
                def = Collection.GetCustomCounter(config.MetricID);
            }
            else
            {
                def = config.CounterDefinition;
            }

            // If previous snapshot exists, make sure it didn't fail and has the same definition
            if (config.PreviousSnapshot != null
                && !config.PreviousSnapshot.CollectionFailed
                && config.PreviousSnapshot.Definition.EqualsMinusEnabled(def))
            {
                return new CustomCounterSnapshot(connectionInfo, config.PreviousSnapshot);
            }
            else
            {
                if (def != null)
                {
                    return
                        new CustomCounterSnapshot(connectionInfo, def);
                }
            }

            return null;
        }

        #endregion

        #region properties

        private bool IsCancelled
        {
            get
            {
                try
                {
                    return context != null ? context.IsCancelled : false;
                }
                catch (Exception)
                {
                    //Catch any of various remoting exceptions
                    return true;
                }
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (counterList == null || counterList.Count == 0)
            {
                lock (returnSnapshot)
                {
                    hasExited = true;
                    returnSnapshot.LoadDictionaryFromList(counterList);
                    ProbeHelpers.LogAndAttachToSnapshot(returnSnapshot, LOG,
                                                        "Insufficient arguments to start a custom counter collection",
                                                        false);
                    FireCompletion(returnSnapshot, Result.Failure);
                }
            }

            foreach(CustomCounterSnapshot customCounter in counterList)
            {
                if (customCounter.Definition == null)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(customCounter, LOG, "Insufficient arguments to start a custom counter collection", false);
                }
            }

            sw.Start();

            // Tolga K - to fix memory leak begins
            if (collectionTimer != null)
            {
                collectionTimer.Dispose();
            }

            if (cancelTimer != null)
            {
                cancelTimer.Dispose();
            }
            // Tolga K - to fix memory leak ends

            collectionTimer = new Timer(
                        new TimerCallback(ExitCollection),
                        null,
                        timeout,
                        TimeSpan.FromMilliseconds(-1));


            cancelTimer = new Timer(
                new TimerCallback(CheckCancelled),
                null,
                30000,
                30000);

            CollectNext();
        }

        private void CheckCancelled(object state)
        {
                if (hasExited)
                {
                    if (cancelTimer != null)
                    {
                        cancelTimer.Dispose();
                        cancelTimer = null;
                    }
                    return;
                }

                if (IsCancelled)
                {
                    LOG.Verbose(
                    String.Format("Cancelling custom counter probe for server {0}",
                                  returnSnapshot.ServerName));
                    ExitCollection(null);
                }
        }

        private void CollectNext()
        {
            using (LOG.VerboseCall("CollectNext"))
            {
                try
                {
                    collect_next:
                    
                    // If this has already timed out but we somehow came here, exit immediately
                    if (hasExited)
                        return;


                    bool cancelled = IsCancelled;

                    // Cancel after the timeout even if the context has not cancelled
                    // Especially for scheduled refresh
                    if (sw.Elapsed.TotalSeconds >= timeout.TotalSeconds)
                    {
                        cancelled = true;
                    }

                    if (!cancelled)
                    {
                        if (index < counterList.Count)
                        {
                            index++;

                            //Just move on if there is already something wrong with the counter
                            while (index < counterList.Count && counterList[index].CollectionFailed)
                            {
                                index++;
                            }

                            if (index >= counterList.Count)
                            {
                                ExitCollection(null);
                                return;
                            }

                            if (counterList[index] != null && counterList[index].Definition != null)
                            {
                                switch (counterList[index].Definition.MetricType)
                                {
                                    case MetricType.WMI:
                                        LOG.Verbose(String.Format("Starting WMI counter with ID {0}",counterList[index].Definition.MetricID));
                                        StartCustomCounterOSCollector();
                                        return;
                                    case MetricType.SQLStatement:
                                        LOG.Verbose(String.Format("Starting SQL Statement counter with ID {0}",counterList[index].Definition.MetricID));
                                        StartCustomCounterSQLStatementCollector();
                                        return;
                                    case MetricType.SQLCounter:
                                        LOG.Verbose(String.Format("Starting SQL counter with ID {0}", counterList[index].Definition.MetricID));
                                        StartCustomCounterSQLCollector();
                                        return;
                                    case MetricType.VMCounter:
                                        CollectVmCustomCounter();
                                        goto collect_next;
                                    case MetricType.AzureCounter:
                                        LOG.Verbose(string.Format("Starting Azure counter with ID {0}",
                                            counterList[index].Definition.MetricID));
                                        CollectCustomCounterAzureCollector();
                                        return;
                                }
                            }
                        }
                        else
                        {
                            LOG.Verbose("Exiting custom counter - reached end of counter list");
                            ExitCollection(null);
                            return;
                        }
                    }
                    else
                    {
                        LOG.Verbose("Exiting custom counter - collection was cancelled.");
                        ExitCollection(null);
                        return;
                    }
                }
                catch (Exception e)
                {
                    lock (returnSnapshot)
                    {
                        LOG.ErrorFormat("Error CustomCounterProbe CollectNext, index: {0}, counterList:{1} Detailed exception message: {2}",index,counterList.Count,e);
                        ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG,
                                                            "An unhandled error occurred while collecting custom counters", e,
                                                            false);
                        ExitCollection(null);
                        return;
                    }
                }
            }
        }

        private void CollectCustomCounterAzureCollector()
        {
            var azureMonitorStopwatch = new Stopwatch();
            azureMonitorStopwatch.Start();
            var definition = counterList[index].Definition;
            
            // Collect Custom Counter Data using Metric name and resource URI
            var resourceUri = definition.ServerType;
            // use the profile id
            var profileId = definition.ProfileId;
            
            // To get from the counter definition
            var monitorManagementConfiguration = new MonitorManagementConfiguration
            {
                MonitorParameters = new AzureMonitorParameters
                {
                    Resource = new AzureResource
                    {
                        Uri = resourceUri
                    }
                }
            };
            try
            {
                if (profileId == 0)
                {
                    counterList[index]
                        .SetError(
                            "The Custom Counter must be linked to Application Profile to access the Azure Monitor API.",
                            new ArgumentNullException("profileId"));
                    azureMonitorStopwatch.Stop();
                    counterList[index].RunTime = TimeSpan.FromMilliseconds(azureMonitorStopwatch.ElapsedMilliseconds);
                }
                else
                {
                    var mgmtSvc = RemotingHelper.GetObject<IManagementService>();
                    monitorManagementConfiguration.Profile = mgmtSvc.GetAzureProfile(profileId, resourceUri);
                    var azureMonitorClient = new AzureManagementClient
                    {
                        Configuration = monitorManagementConfiguration
                    };

                    // Calculate the closest time for the metric depending on the scheduled collection interval
                    var closestTime = ProbeHelpers.GetClosestTime(azureMonitorClient, _scheduledCollectionInterval,
                        definition.CounterName);

                    // Get Metric Name from Metric Display name
                    var metricName = ProbeHelpers.GetMetricName(azureMonitorClient, definition.CounterName);

                    monitorManagementConfiguration.MonitorParameters.MetricName = metricName;
                    LOG.Info("Closes Time Interval selected (in minutes) is: " + closestTime.TotalMinutes);
                    // Update the interval to the closest time
                    monitorManagementConfiguration.MonitorParameters.Interval = closestTime;
                    monitorManagementConfiguration.MonitorParameters.Timespan = string.Format("{0:O}/{1:O}",
                        DateTime.UtcNow.Subtract(closestTime),
                        DateTime.UtcNow);


                    var response = azureMonitorClient.GetMetrics(monitorManagementConfiguration.MonitorParameters)
                        .GetAwaiter()
                        .GetResult();
                    azureMonitorStopwatch.Stop();
                    counterList[index].RunTime = TimeSpan.FromMilliseconds(azureMonitorStopwatch.ElapsedMilliseconds);
                    var firstResponse = response.Value.FirstOrDefault();
                    if (firstResponse != null && firstResponse.Timeseries != null)
                    {
                        var timeSeries = firstResponse.Timeseries.FirstOrDefault();
                        if (timeSeries != null && timeSeries.Data != null)
                        {
                            var data = timeSeries.Data.FirstOrDefault();
                            if (data != null)
                            {
                                counterList[index].RawValue = (decimal?) data.Average;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                counterList[index].SetError("Problem in fetching the Custom Counter value from the Azure Monitor API. The collection service returned the following error: {0}", exception);
                azureMonitorStopwatch.Stop();
                counterList[index].RunTime = TimeSpan.FromMilliseconds(azureMonitorStopwatch.ElapsedMilliseconds);
            }
            CollectNext();
        }

        private void ExitCollection(object state)
        {
            if (cancelTimer != null)
            {
                cancelTimer.Dispose();
                cancelTimer = null;
            }

            if (collectionTimer != null)
            {
                collectionTimer.Dispose();
                collectionTimer = null;
            }

            if (currentCollector != null)
            {
                currentCollector.Dispose();
                currentCollector = null;
            }

            if (impersonation != null)
            {
                impersonation.Dispose();
                impersonation = null;
            }

            if (_vmServices != null)
            {
                try
                {
                    if (_vmServices.ConnectState == Common.VMware.ConnectState.Connected)
                        _vmServices.Disconnect();
                } 
                catch (Exception)
                {
                    /* */
                }
                _vmMoRef = _esxMoRef = null;
                _vmServices = null;
            }

            if (hasExited)
                return;

            LOG.Debug(String.Format("Exiting custom counter collection after {0} seconds", sw.Elapsed.TotalSeconds));

            //Cancel any outstanding collections
            if (currentCollector != null)
                currentCollector.Dispose();

            LOG.Verbose(String.Format("ExitCollection: index:{0}, counterList length:{1}", index, counterList.Count));
            // Set anything that has not completed as timed out
            for (int i = index; i < counterList.Count; i++)
            {
                counterList[i].SetError(
                    "Custom counter collection was cancelled or timed out before this counter could be attempted.",
                    new Exception(
                        "Custom counter collection was cancelled or timed out before this counter could be attempted."));
            }
            lock (returnSnapshot)
            {
                hasExited = true;
                returnSnapshot.LoadDictionaryFromList(counterList);
                FireCompletion(returnSnapshot, Result.Success);
            }
        }

        /// <summary>
        /// Starts the Machine Name collector.
        /// The WMI probe for disk statistics needs the machine name and needs to run prior to 
        /// the database summary probe.
        /// </summary>
        private void StartMachineNameCollector()
        {
            using (LOG.VerboseCall("CustomCounterProbe:StartMachineNameCollector"))
            {
                LOG.Verbose(String.Format("Starting StartMachineNameCollector: index:{0}, counterList length:{1}", index, counterList.Count));
                // Use Machine Name Probe since CustomCounterOs Permissions are checked with Generic Collector Way
                // Passing CloudProvider information
                MachineNameProbe machineProbe = new MachineNameProbe(connectionInfo, cloudProviderId);
                machineProbe.BeginProbe(MachineNameCallback);
            }
            
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
                var machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (machineSnapshot != null)
                    machineName = machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartWmiCounterCollector();
        }

        private void StartWmiCounterCollector()
        {
            // Initialize the WMI collector.
            var opts = WmiCollector.CreateConnectionOptions(machineName, wmiConfig, out impersonation);
            wmiCollector = new WmiCollector(machineName, opts, impersonation);

            if (impersonation != null && !impersonation.IsLoggedOn)
            {
                // get user identitity
                impersonation.LogonUser();
            }

            var def = counterList[index].Definition;

            var query = String.Format("SELECT {0} from {1}", def.CounterName, def.ObjectName);
            if (!String.IsNullOrEmpty(def.InstanceName))
                query += " WHERE " + def.InstanceName;

            wmiCollector.Query = new WqlObjectQuery(query);
            wmiCollector.Results.Clear();
            wmiCollector.BeginCollection(DirectWmiCallback, InterpretObject, null);
        }

        private void DirectWmiCallback(object sender, CollectorCompleteEventArgs e)
        {
            LOG.VerboseFormat("Service collector ran in {0} milliseconds.", e.ElapsedMilliseconds);
            Exception excp = null;
            if (e.Result == Result.Success)
            {
                var result = e.Value as IList;
                if (result != null && result.Count > 0)
                {
                    var item = result[0] as ManagementBaseObject;
                    if (item != null)
                    {
                        var propertyName = counterList[index].Definition.CounterName;
                        var value = item.GetPropertyValue(propertyName);
                        // values are returned as strings - Convert.ToDecimal handles numeric values but not boolean strings
                        if (value is string)
                        {
                            // handle string to bool conversion
                            if (String.Equals((string) value, Boolean.TrueString,
                                              StringComparison.InvariantCultureIgnoreCase))
                                value = 1.0m;
                            else if (String.Equals((string) value, Boolean.FalseString,
                                                   StringComparison.InvariantCultureIgnoreCase))
                                value = 0.0m;
                        }
                        if (value != null)
                            counterList[index].RawValue = Convert.ToDecimal(value);
                        else
                        {
                            var def = counterList[index].Definition;
                            excp = WmiCollector.CreateManagementException(ManagementStatus.InvalidProperty,
                                                                          String.Format("Unable to find property {0} on object {1}.", def.CounterName, def.CounterName), 
                                                                              null);
                        }
                    }
                    else
                    {
                        // data not found
                        var def = counterList[index].Definition;
                        if (String.IsNullOrEmpty(def.InstanceName))
                            excp = WmiCollector.CreateManagementException(ManagementStatus.NotFound,
                                                                          String.Format("Unable to find an instance of object {0}", def.ObjectName), 
                                                                          null);
                        else
                            excp = WmiCollector.CreateManagementException(ManagementStatus.InvalidObjectPath,
                                                                          String.Format("Unable to find an instance of object {0} with {1}", def.ObjectName, def.InstanceName),
                                                                          null);
                    }
                }
                else
                {
                    // data not found
                    var def = counterList[index].Definition;
                    if (String.IsNullOrEmpty(def.InstanceName))
                        excp = WmiCollector.CreateManagementException(ManagementStatus.NotFound,
                                                                      String.Format("Unable to find an instance of object {0}", def.ObjectName),
                                                                      null);
                    else
                        excp = WmiCollector.CreateManagementException(ManagementStatus.InvalidObjectPath,
                                                                      String.Format("Unable to find an instance of object {0} with {1}", def.ObjectName, def.InstanceName),
                                                                      null);
                }
            }
            else
            {
                excp = e.Exception;
            }

            if (excp != null)
            {
                ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Custom OS Counter return value is invalid. It must be a single, scalar, numeric value. The collection service returned the following error: {0}", excp, false);            
            }

            if (e.ElapsedMilliseconds.HasValue)
            {
                counterList[index].RunTime = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
            }

            CollectNext();
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }


        /// <summary>
        /// Starts the CustomCounterOS collector.
        /// </summary>
        private void StartCustomCounterOSCollector()
        {
            if (wmiConfig.DirectWmiEnabled)
            {
                StartMachineNameCollector();

                return;
            }

            StartGenericCollector(new Collector(CustomCounterOSCollector), counterList[index], "StartCustomCounterOSCollector", "CustomCounterOS", CustomCounterOSCallback, new object[] { });
        }

        /// <summary>
        /// Define the CustomCounterOS collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void CustomCounterOSCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildCustomCounterOSCommand(conn, ver, counterList[index].Definition, (int)wmiConfig.OleAutomationContext, wmiConfig.OleAutomationDisabled);
            sdtCollector = new SqlCollector(cmd, true);
            currentCollector = null; //= sdtCollector;
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(CustomCounterOSCallback));
        }

        /// <summary>
        /// Define the CustomCounterOS callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterOSCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                counterList[index].RunTime = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
            }

            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretCustomCounterOS(rd);
            }
            CollectNext();
        }

        /// <summary>
        /// Callback used to process the data returned from the CustomCounterOS collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterOSCallback(object sender, CollectorCompleteEventArgs e)
        {
            LOG.Verbose(
                String.Format("Custom counter ID {0} took {1} milliseconds to complete",
                              counterList[index].Definition.MetricID, e.ElapsedMilliseconds));
            GenericCallback(new CollectorCallback(CustomCounterOSCallback), counterList[index], "CustomCounterOSCallback", "CustomCounterOS",
                            new FailureDelegate(CustomCounterFailureDelegate), new FailureDelegate(CustomCounterFailureDelegate), sender, e);

        }

        /// <summary>
        /// Interpret CustomCounterOS data
        /// </summary>
        private void InterpretCustomCounterOS(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretCustomCounterOS"))
            {
                try
                {
                    if (dataReader.Read())
                    {
                        switch (dataReader.FieldCount)
                        {
                            case 1:
                                string available = dataReader.GetString(0);
                                if (available.ToLower() == "available")
                                {
                                    if (dataReader.NextResult() && dataReader.Read() && !dataReader.IsDBNull(0))
                                    {
                                        object value = dataReader.GetValue(0);
                                        // values are returned as strings - Convert.ToDecimal handles numeric values but not boolean strings
                                        if (value is string)
                                        {   // handle string to bool conversion
                                            if (String.Equals((string)value, Boolean.TrueString, StringComparison.InvariantCultureIgnoreCase))
                                                value = 1.0m;
                                            else
                                                if (String.Equals((string)value, Boolean.FalseString, StringComparison.InvariantCultureIgnoreCase))
                                                    value = 0.0m;
                                        }
                                        counterList[index].RawValue = Convert.ToDecimal(value);
                                    }
                                    else
                                    {
                                        throw new CollectionServiceException("No data was returned for configured counter"); 
                                    }
                                }
                                else
                                {
                                    throw new CollectionServiceException("Unable to retrieve custom counter: " + available); 
                                }
                                break;
                            case 3: // spOA error 
                                int hresult = dataReader.GetInt32(0);
                                string source = dataReader.GetString(1);
                                string message = dataReader.IsDBNull(2) ? "" : " " + dataReader.GetString(2);
                                switch (hresult)
                                {
                                    case (int)ManagementStatus.InvalidClass:
                                        message = "The object name in your counter definition does not exist on server hosting this instance.  " + message;
                                        break;
                                    case (int)ManagementStatus.InvalidObjectPath:
                                    case (int)ManagementStatus.NotFound:
                                        message = "The instance name in your counter definition does not exist on server hosting this instance.  " + message;
                                        break;
                                    case -2147352570:  // Method or property not found
                                        message = "The counter name could not be found for the object specified in the counter definition.  " + message;
                                        break;
                                }

                                throw new Win32Exception(hresult, String.Format("{2} rc=0x{1:X} {0}", source, hresult, message));
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Custom OS Counter return value is invalid. It must be a single, scalar, numeric value. The collection service returned the following error: {0}", e,
                                                        false);
                }
            }
        }

        /// <summary>
        /// Define the CustomCounterSQL collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void CustomCounterSQLCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildCustomCounterSQLCommand(conn, ver, counterList[index].Definition, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            currentCollector = sdtCollector;
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(CustomCounterSQLCallback));
        }

        /// <summary>
        /// Starts the Custom Counter SQL collector.
        /// </summary>
        private void StartCustomCounterSQLCollector()
        {
            StartGenericCollector(new Collector(CustomCounterSQLCollector), counterList[index], "StartCustomCounterSQLCollector", "Custom Counter SQL", null,new HandleSqlErrorDelegate(CustomCounterHandleSqlException), CustomCounterSQLCallback, new object[] { });
        }

        /// <summary>
        /// Define the CustomCounterSQL callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterSQLCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                counterList[index].RunTime = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
            }

            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretCustomCounterSQL(rd);
            }
            CollectNext();
        }

        /// <summary>
        /// Callback used to process the data returned from the CustomCounterSQL collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterSQLCallback(object sender, CollectorCompleteEventArgs e)
        {
            LOG.Verbose(
              String.Format("Custom counter ID {0} took {1} milliseconds to complete",
                            counterList[index].Definition.MetricID, e.ElapsedMilliseconds));
            GenericCallback(new CollectorCallback(CustomCounterSQLCallback), counterList[index], "CustomCounterSQLCallback", "Custom Counter SQL",
                            new FailureDelegate(CustomCounterFailureDelegate), new FailureDelegate(CustomCounterFailureDelegate), sender, e);
        }

        /// <summary>
        /// Interpret CustomCounterSQL data
        /// </summary>
        private void InterpretCustomCounterSQL(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretCustomCounterSQL"))
            {
                try
                {
                    if (dataReader.Read() && !dataReader.IsDBNull(0))
                    {
                        // reader should contain a long
                        counterList[index].RawValue = Convert.ToDecimal(dataReader.GetValue(0));
                    }
                    else
                    {
                        if (dataReader.NextResult() && dataReader.Read())
                        {   // object, counter or instance are invalid
                            throw new CollectionServiceException(dataReader.GetString(0));
                        }
                        // should only happen if the object,counter,instance are valid but value is null
                        throw new ApplicationException("No data was returned for configured counter");
                    }
                }
                catch (Exception e)
                {
                    if (e is CollectionServiceException)
                        ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, e.Message, e, false);
                    else
                        ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Custom SQL Counter return value is invalid. It must be a single, scalar, numeric value. The collection service returned the following error: {0}", e, false);                        
                }
            }
        }

        /// <summary>
        /// Define the CustomCounterSQLStatement collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void CustomCounterSQLStatementCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildCustomCounterSQLStatementCommand(conn, ver, counterList[index].Definition);
            sdtCollector = new SqlCollector(cmd, true);
            currentCollector = sdtCollector;
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(CustomCounterSQLStatementCallback));
        }

        /// <summary>
        /// Starts the Custom Counter SQL Statement collector.
        /// </summary>
        private void StartCustomCounterSQLStatementCollector()
        {
            StartGenericCollector(new Collector(CustomCounterSQLStatementCollector), counterList[index], "StartCustomCounterSQLStatementCollector", "Custom Counter SQL Statement", null, new HandleSqlErrorDelegate(CustomCounterHandleSqlException), null, new object[] { });
        }

        /// <summary>
        /// Define the CustomCounterSQLStatement callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterSQLStatementCallback(CollectorCompleteEventArgs e)
        {
            if (e.ElapsedMilliseconds.HasValue)
            {
                counterList[index].RunTime = TimeSpan.FromMilliseconds(e.ElapsedMilliseconds.Value);
            }

            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretCustomCounterSQLStatement(rd);
            }
            CollectNext();
        }

        /// <summary>
        /// Callback used to process the data returned from the CustomCounterSQLStatement collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void CustomCounterSQLStatementCallback(object sender, CollectorCompleteEventArgs e)
        {
            LOG.Verbose(
              String.Format("Custom counter ID {0} took {1} milliseconds to complete",
                            counterList[index].Definition.MetricID, e.ElapsedMilliseconds));
            GenericCallback(new CollectorCallback(CustomCounterSQLStatementCallback), counterList[index], "CustomCounterSQLStatementCallback", "Custom Counter SQL Statement", 
                new FailureDelegate(CustomCounterFailureDelegate), new FailureDelegate(CustomCounterFailureDelegate),sender, e);
        }

        /// <summary>
        /// Interpret CustomCounterSQLStatement data
        /// </summary>
        private void InterpretCustomCounterSQLStatement(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretCustomCounterSQLStatement"))
            {
                try
                {
                    if (dataReader.Read() && dataReader.FieldCount == 1 && !dataReader.IsDBNull(0))
                    {
                        if (dataReader.GetFieldType(0) == typeof (SqlDecimal) ||
                            dataReader.GetFieldType(0) == typeof (decimal))
                        {
                            SqlDecimal sqlDecimal = dataReader.GetSqlDecimal(0);
                            try
                            {
                                counterList[index].RawValue = sqlDecimal.Value;
                            }
                            catch (OverflowException oe)
                            {
                                if (sqlDecimal.Scale > 0)
                                {
                                    try
                                    {
                                        decimal decvalue;
                                        if (Decimal.TryParse(sqlDecimal.ToString(),
                                                             System.Globalization.NumberStyles.Number,
                                                             CultureInfo.CurrentCulture.NumberFormat, out decvalue))
                                        {
                                            sqlDecimal = new SqlDecimal(decvalue);
                                        }

                                        counterList[index].RawValue = sqlDecimal.Value;
                                    }   
                                    catch (OverflowException oe2)
                                    {
                                        throw new OverflowException(
                                            "The value returned has a precision larger than SQLdm is able to support.  Counter values are limited to a precision of 29 digits.",
                                            oe2);
                                    }
                                } else
                                    throw new OverflowException(
                                        "The value returned has a precision larger than SQLdm is able to support.  Counter values are limited to a precision of 29 digits.",
                                        oe);
                            }
                        }
                        else
                        {
                            counterList[index].RawValue = Convert.ToDecimal(dataReader.GetValue(0));
                        }
                    }
                    else
                    {
                        if (dataReader.FieldCount == 1)
                        {
                            throw new CollectionServiceException("No data was returned for configured counter");
                        }
                        else
                        {
                            throw new CollectionServiceException("More than one field was returned for configured counter.  Please return only a single field.");
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG,
                                                        "Custom SQL Statement return value is invalid. It must be a single, scalar, numeric value. The collection service returned the following error: {0}",
                                                        e,
                                                        false);
                }
            }
        }

        private void CollectVmCustomCounter()
        {
            try
            {
                CustomCounterDefinition definition = counterList[index].Definition;
                string[] objectParts = definition.ObjectName.Split(new char[] {'.'});

                string instance = definition.InstanceName;
                if (instance == null || instance.Equals("_Total"))
                    instance = String.Empty;

                // connect to vCenter
                if (_virtualizationConfiguration == null)
                    throw new CollectionServiceException("Virtualization configuration is not set for monitored server.");

                if (_virtualizationConfiguration.VCServerType.Equals("HyperV"))
                {
                    try
                    {
                        Stopwatch stopwatch_hyperV = new Stopwatch();
                        stopwatch_hyperV.Start();

                        HyperVService hyperVService = new HyperVService(_virtualizationConfiguration.VCAddress);
                        HyperVCustomCounter hyperVcustomCounter = new HyperVCustomCounter();
                        Dictionary<string, CustomCounterBasicInfo> hyperVCustomCounterObjects = hyperVcustomCounter.HyperVCustomCounterObjects;
                        string counterName = definition.CounterName;
                        string wmiClass = "";
                        string searcherKey = "";
                        string whereClause = "";

                        foreach (KeyValuePair<string, CustomCounterBasicInfo> item in hyperVCustomCounterObjects)
                        {
                            if (item.Value.CounterLabel.Equals(definition.CounterName))
                            {
                                wmiClass = item.Value.WmiClass;
                                searcherKey = item.Value.SearcherKey;
                                whereClause = item.Value.WhereClause;
                            }
                        }

                        ManagementObjectSearcher searcher = hyperVService.CollectCustomCounterValue(wmiClass, objectParts[0], _virtualizationConfiguration,whereClause);

                        ManagementObjectCollection collection = searcher.Get();
                        decimal? value1 = 0;
                        foreach (ManagementObject collectionObject in collection)
                        {
                            value1 =  Convert.ToInt32(collectionObject[searcherKey]);
                        }


                        stopwatch_hyperV.Stop();
                        counterList[index].RunTime = TimeSpan.FromMilliseconds(stopwatch_hyperV.ElapsedMilliseconds);
                        counterList[index].RawValue = value1;
                    }
                    catch (Exception e)
                    {
                        ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Custom VM Counter value is invalid.  The collection service returned the following error: {0}", e, false);
                    }
                    
                    return;

                }

                try
                {
                    if (_vmServices == null)
                    {
                        _vmServices = new ServiceUtil(_virtualizationConfiguration.VCAddress);
                        _vmServices.Connect(_virtualizationConfiguration.VCUser, _virtualizationConfiguration.VCPassword);
                    }
                }
                catch (Exception e)
                {
                    throw new CollectionServiceException(String.Format("Unable to connect to Virtualization Host: {0}", e.Message), e);
                }

                if (_vmServices.ConnectState != Common.VMware.ConnectState.Connected)
                    throw new CollectionServiceException("Connection dropped to Virtualization Host - retry again later.");

                if (_vmMoRef == null)
                {
                    _vmMoRef = _vmServices.getManagedObject(_virtualizationConfiguration.InstanceUUID, true, VIMSearchType.UUID);
                    _esxMoRef = _vmServices.getESXHost(_vmMoRef);
                }

                ManagedObjectReference objectReference = null;

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                if (objectParts[0].Equals("VM", StringComparison.InvariantCultureIgnoreCase))
                    objectReference = _vmMoRef;
                else
                    objectReference = _esxMoRef;

                bool exists;
                PerfCounterInfo counterInfo = null;
                Pair<string, string> cacheKey = new Pair<string, string>(_virtualizationConfiguration.VCAddress, objectParts[1] + "." + definition.CounterName);
                lock (vmcache)
                {
                    object cachedvalue = null;
                    exists = vmcache.TryGetValue(cacheKey, out cachedvalue);
                    counterInfo = cachedvalue as PerfCounterInfo;
                }
                if (!exists || counterInfo == null)
                {
                    // get and cache collection of all perf counters available
                    Dictionary<int, PerfCounterInfo> allCounters = null;
                    Pair<string, string> cacheKey2 = new Pair<string, string>(_virtualizationConfiguration.VCAddress, "PerfCounterInfo");
                    lock (vmcache)
                    {
                        object allCountersObject = null;
                        exists = vmcache.TryGetValue(cacheKey2, out allCountersObject);
                        allCounters = allCountersObject as Dictionary<int, PerfCounterInfo>;
                    }
                    if (!exists || allCounters == null)
                    {
                        // load and cache all counters
                        allCounters = _vmServices.GetPerfCounterInfo();
                        lock (vmcache)
                        {
                            if (!vmcache.ContainsKey(cacheKey2))
                                vmcache.Add(cacheKey2, allCounters);
                        }
                    }

                    foreach (PerfCounterInfo pci in allCounters.Values)
                    {
                        if (String.Equals(pci.groupInfo.key, objectParts[1], StringComparison.InvariantCultureIgnoreCase) &&
                            String.Equals(pci.nameInfo.key, definition.CounterName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            counterInfo = pci;
                            lock(vmcache)
                            {
                                if (!vmcache.ContainsKey(cacheKey))
                                    vmcache.Add(cacheKey, counterInfo);
                            }
                            break;
                        }
                    }
                }

               if (counterInfo == null)
                    throw new ArgumentException("Unable to find performance counter matching the configuration.");
                
                PerfMetricId metric = new PerfMetricId();
                metric.counterId = counterInfo.key;
                metric.instance = instance;

                long value = 0;
                PerfEntityMetricBase[] perfStats = _vmServices.getPerfStats(objectReference, metric, 1);
                if (perfStats.Length > 0)
                {
                    PerfMetricSeries[] vals = ((PerfEntityMetric) perfStats[0]).value;
                    foreach (PerfMetricSeries pms in vals)
                    {
                        PerfMetricId pmi = pms.id;
                        if (string.IsNullOrEmpty(pmi.instance))
                        {
                            if (pms is PerfMetricIntSeries)
                            {
                                PerfMetricIntSeries val = (PerfMetricIntSeries) pms;
                                value = ServiceUtil.calcAverage(val.value);
                            }
                        }
                    }
                }

                stopwatch.Stop();
                counterList[index].RunTime = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
                counterList[index].RawValue = value;
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Custom VM Counter value is invalid.  The collection service returned the following error: {0}", e, false);
            }
        }
       
        private void CustomCounterFailureDelegate(Snapshot snapshot, Exception e)
        {
            ProbeHelpers.LogAndAttachToSnapshot(counterList[index], LOG, "Error executing Custom Counter: {0}", e, false);
            CollectNext();
        }


        private void CustomCounterHandleSqlException(Snapshot snapshot, string collectorName, SqlException sqlException, SqlCollector collector)
        {
            if (collector != null)
            {
                HandleSqlException(snapshot, collectorName, sqlException, collector.SqlText);
                collector.Dispose();
            }
            else
            {
                HandleSqlException(snapshot, collectorName, sqlException);
            }

            // Set all remaining counters as failed
            for (int i = index; i < counterList.Count; i++)
            {
                counterList[i].SetError(
                   snapshot.Error != null ?snapshot.Error.Message : null,
                   snapshot.Error);
            }

            ExitCollection(null);
        }


        #endregion

        #region interface implementations


        #region IOnDemandProbe Members

        public IOnDemandContext Context
        {
            get
            {
                return context;
            }
            set
            {
                this.context = value;
            }
        }

        #endregion

        #endregion
    }
}
