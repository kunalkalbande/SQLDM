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
    class WmiConnectionTestProbe: SqlBaseProbe
    {
        #region fields

        private WmiConfigurationTestSnapshot snapshot = null;
        private TestWmiConfiguration configuration = null;

        private bool tryDirectWmi;
        private string machineName;
        private string serverName;
        private string agentName;
        private WmiCollector _wmicollector;

        private ImpersonationContext impersonation;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WmiConnectionTestProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public WmiConnectionTestProbe(SqlConnectionInfo connectionInfo, TestWmiConfiguration configuration, int? cloudProviderId) : base(connectionInfo)
        {
            LOG = Logger.GetLogger("WmiConnectionTestProbe");
            snapshot = new WmiConfigurationTestSnapshot(connectionInfo.InstanceName);
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
            if (configuration != null && configuration.ReadyForCollection)
            {
                StartOleCollector();
            }
            else
            {
                FireCompletion(snapshot, Result.Success);
            }
        }

        /// <summary>
        /// Define the Session collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void OleCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildWmiTestCommand(conn, ver, configuration);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(OleCallback));
        }

        /// <summary>
        /// Starts the session list collector.
        /// </summary>
        void StartOleCollector()
        {
            StartGenericCollector(new Collector(OleCollector), snapshot, "StartOleCollector", "sp_OA Check/Test", OleCallback, new object[] { });
        }

        /// <summary>
        /// Define the Sessions callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OleCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretOAResults(rd);
            }
            if (tryDirectWmi)
                StartDirectCollector();
            else
                FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the sessions collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void OleCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(OleCallback), snapshot, "OleCallback", "sp_OA Check/Test", sender, e);
        }

        private void StartDirectCollector()
        {
            var wmiConfig = configuration.WmiConfig;
            var opts = WmiCollector.CreateConnectionOptions(machineName, wmiConfig, out impersonation);

            _wmicollector = new WmiCollector(machineName, opts, impersonation);
            _wmicollector.Query = new WqlObjectQuery("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");

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
                    var item = result[0] as ManagementBaseObject;
                    if (item != null)
                        snapshot.CollectedValue = item["TotalPhysicalMemory"].ToString();
                }
            }

            FireCompletion(snapshot, e.Result);
        }

        private object InterpretObject(Collectors.WmiCollector collector, ManagementBaseObject newObject)
        {
            return newObject;
        }

        private void InterpretOAResults(SqlDataReader rd)
        {
            if (rd.Read())
            {
                var oastatus = rd.GetString(0);

                if (configuration.WmiConfig.DirectWmiEnabled)
                {
                    if (rd.NextResult() && rd.Read())
                    {
                        machineName = rd.GetString(0);
                        serverName = rd.GetString(1);
                        tryDirectWmi = true;
                    }
                }
                else
                if (!configuration.WmiConfig.OleAutomationDisabled)
                {
                    if (!String.IsNullOrEmpty(oastatus) && "available" != oastatus)
                        snapshot.OleAutomationStatus = oastatus;

                    if (rd.NextResult() && rd.Read())
                    {
                        snapshot.CollectedValue = rd.GetValue(0).ToString();
                    }
                }
            }
        }

        #endregion

    }
}
