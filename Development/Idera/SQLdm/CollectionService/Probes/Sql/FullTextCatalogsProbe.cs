//------------------------------------------------------------------------------
// <copyright file="FullTextCatalogsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data;
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


    /// <summary>
    /// Full Text Catalogs on-demand probe
    /// </summary>
    internal sealed class FullTextCatalogsProbe : SqlBaseProbe
    {
        #region fields

        private FullTextCatalogs fullTextCatalogs = null;
        private ClusterCollectionSetting clusterCollectionSetting = ClusterCollectionSetting.Default;

        private WmiConfiguration _wmiConfig;
        private ServiceStatusProbe _statusProbe;
        private IAsyncResult _statusIAR;
        private string _machineName;
        private string _serviceName;
        private int _majorVersion;
        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FullTextCatalogsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public FullTextCatalogsProbe(SqlConnectionInfo connectionInfo, WmiConfiguration wmiConfiguration, ClusterCollectionSetting clusterCollectionSetting, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("FullTextCatalogsProbe");
            this.cloudProviderId = cloudProviderId;
            fullTextCatalogs = new FullTextCatalogs(connectionInfo);
            _wmiConfig = wmiConfiguration;
            this.clusterCollectionSetting = clusterCollectionSetting;
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
            if (cloudProviderId != CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
                StartFullTextCatalogsCollector();
            else
            {
                FireCompletion(fullTextCatalogs, Result.Success);
            }
        }

        /// <summary>
        /// Define the FullTextCatalogs collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FullTextCatalogsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildFullTextCatalogsCommand(conn, ver);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FullTextCatalogsCallback));
        }

        /// <summary>
        /// Starts the Full Text Catalogs collector.
        /// </summary>
        private void StartFullTextCatalogsCollector()
        {
            StartGenericCollector(new Collector(FullTextCatalogsCollector), fullTextCatalogs, "StartFullTextCatalogsCollector", "Full Text Catalogs", FullTextCatalogsCallback, new object[] { });
        }

        /// <summary>
        /// Define the FullTextCatalogs callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextCatalogsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretFullTextCatalogs(rd);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the FullTextCatalogs collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextCatalogsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(
                new CollectorCallback(FullTextCatalogsCallback), 
                fullTextCatalogs,
                "FullTextCatalogsCallback", 
                "Full Text Catalogs",
                new NextCollector(StartFullTextSearchServiceCollector),
                sender, 
                e);
        }

        /// <summary>
        /// Define the FullTextSearchService collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void FullTextSearchServiceCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            var hasServiceControlExecuteAccess = CollectionPermissions != CollectionPermissions.None &&
                                                 CollectionPermissions.HasFlag(CollectionPermissions
                                                     .EXECUTEMASTERXPSERVICECONTROL);
            SqlCommand cmd = SqlCommandBuilder.BuildFullTextSearchServiceCommand(conn, ver, _wmiConfig, clusterCollectionSetting, hasServiceControlExecuteAccess);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(FullTextSearchServiceCallback));
        }

        /// <summary>
        /// Starts the Full Text Search Service collector.
        /// </summary>
        private void StartFullTextSearchServiceCollector()
        {
            StartGenericCollector(
                new Collector(FullTextSearchServiceCollector), 
                fullTextCatalogs,
                "StartFullTextSearchServiceCollector", 
                "Full Text Search Service", FullTextSearchServiceCallback, new object[] { });
        }

        /// <summary>
        /// FullText Status failure delegate
        /// We will receive a SQL error if FullText is not installed so we need to handle that
        /// </summary>
        /// <param name="snapshot">Snapshot to modify</param>
        private void FullTextSearchServiceFailureDelegate(Snapshot snapshot, Exception e)
        {
            fullTextCatalogs.FullTextServiceStatus = ServiceState.NotInstalled;
            FireCompletion(fullTextCatalogs, Result.Success);
        }

        /// <summary>
        /// Define the FullTextSearchService callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextSearchServiceCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                if (_wmiConfig.DirectWmiEnabled)
                {
                    if (rd.Read())
                    {
                        //_machineName = rd.GetString(0);//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe to get this value
                        _serviceName = rd.GetString(1);
                        _majorVersion = rd.GetInt32(2);

                        StartMachineNameCollector();
                        //StartServiceStatusProbe();//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe ServiceStatusProbe
                        return;
                    }
                }

                InterpretFullTextSearchService(rd);
            }
            FireCompletion(fullTextCatalogs, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the FullTextSearchService collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void FullTextSearchServiceCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(
                new CollectorCallback(FullTextSearchServiceCallback), 
                fullTextCatalogs,
                "FullTextSearchServiceCallback", 
                "Full Text Search Service",
                new FailureDelegate(FullTextSearchServiceFailureDelegate),
                new FailureDelegate(GenericFailureDelegate),
                sender, 
                e, true);
        }

        /// <summary>
        /// Interpret FullTextSearchService data
        /// </summary>
        private void InterpretFullTextSearchService(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFullTextSearchService"))
            {
                try
                {
                    if (dataReader.HasRows)
                    {
                        // SQLdm 10.3 (Varun Chopra) Linux Support for Full Test Service
                        fullTextCatalogs.FullTextServiceStatus = ProbeHelpers.ReadServiceState(dataReader, LOG, cloudProviderId);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(fullTextCatalogs, LOG, "Error interpreting Full Text Search Service Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(fullTextCatalogs);
                }
            }
        }

		//START SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm
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
                return;
            if (e.Snapshot != null)
            {
                var _machineSnapshot = e.Snapshot as MachineNameSnapshot;
                if (_machineSnapshot != null)
                    _machineName = _machineSnapshot.MachineName;
            }

            ((IDisposable)sender).Dispose();

            // start the next probe 
            StartServiceStatusProbe();
        }
		//END SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- Instead Calling MachineNameProbe 1) ServerName 2) NetBIOS name 3)The name that is registered with SQLdm

        private void StartServiceStatusProbe()
        {
            _statusProbe = new ServiceStatusProbe(_machineName, _wmiConfig);
            _statusProbe.ServiceNames = new string[] { _serviceName };
            _statusIAR = _statusProbe.BeginProbe(OnServiceStatusComplete);
        }

        private void OnServiceStatusComplete(object sender, ProbeCompleteEventArgs args)
        {
            var probedata = args.Data as Dictionary<string, Service>;
            if (probedata != null && probedata.Count > 0)
            {
                var entries = Algorithms.ToArray(probedata);
                fullTextCatalogs.FullTextServiceStatus = entries[0].Value.RunningState;
            }

            FireCompletion(fullTextCatalogs, Result.Success);
        }


        /// <summary>
        /// Interpret FullTextCatalogs data
        /// </summary>
        private void InterpretFullTextCatalogs(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretFullTextCatalogs"))
            {
                try
                {
                    fullTextCatalogs.Catalogs.BeginLoadData();
                    
                        if (dataReader.Read())
                        {
                            if (!dataReader.IsDBNull(0)) fullTextCatalogs.ProcessPriority = dataReader.GetInt32(0);
                            dataReader.NextResult();
                        }
                        do
                        {
                        while (dataReader.Read())
                        {
                            DataRow dr = fullTextCatalogs.Catalogs.NewRow();
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                if (!dataReader.IsDBNull(i))
                                {
                                    switch (i)
                                    {
                                        case 4:
                                            dr[i] = new FileSize(dataReader.GetInt32(i)*1024);
                                            break;
                                        case 7:
                                            dr[i] = (FullTextSearchPopulationStatus) dataReader.GetInt32(i);
                                            break;
                                        default:
                                            dr[i] = dataReader.GetValue(i);
                                            break;
                                    }
                                }
                            }
                            fullTextCatalogs.Catalogs.Rows.Add(dr);
                        }
                    } while (dataReader.NextResult());
                    fullTextCatalogs.Catalogs.EndLoadData();
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(fullTextCatalogs, LOG, "Error interpreting Full Text Catalogs Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(fullTextCatalogs);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion
    }
}
