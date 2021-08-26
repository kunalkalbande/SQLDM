//------------------------------------------------------------------------------
// <copyright file="SessionsProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Probes
{
    using System;
    using Idera.SQLdm.CollectionService.Probes.Collectors;
    using Idera.SQLdm.CollectionService.Probes.Sql;
    using Idera.SQLdm.Common.Configuration;
    using Idera.SQLdm.Common.Services;
    using BBS.TracerX;
    using System.Data.SqlClient;
    using SqlCommandBuilder = Idera.SQLdm.CollectionService.Probes.Sql.SqlCommandBuilder;

    /// <summary>
    /// On-demand probe for SQL Server overview
    /// </summary>
    internal class SessionsProbe : SqlBaseProbe
    {
        #region fields

        private static BBS.TracerX.Logger LOG = BBS.TracerX.Logger.GetLogger("SessionsProbe");
        private SessionsConfiguration _configuration = null;
        private SessionSnapshot refresh = null;
        

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerSummaryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="configuration">The configuration info.</param>
        public SessionsProbe(SqlConnectionInfo connectionInfo, SessionsConfiguration configuration)
            : base(connectionInfo)
        {
            _configuration = configuration;
            refresh = new SessionSnapshot(connectionInfo);
            refresh.Configuration = configuration;
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
            StartSessionsCollector();
        }

        /// <summary>
        /// Starts the session details collector.
        /// </summary>
        void StartSessionsCollector()
        {
            SqlCollector sdtCollector = null;

            try
            {
                //Start Server Overview Collection
                SqlConnection conn = OpenConnection();
                ServerVersion ver = new ServerVersion(conn.ServerVersion);
                if (ver.Major == 8 || ver.Major == 9)
                {
                    SqlCommand cmd = SqlCommandBuilder.BuildSessionsCommand(OpenConnection(), ver, _configuration);
                    sdtCollector = new SqlCollector(cmd, true);
                    sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(SessionsCallback));
                }
                else
                {
                    ProbeHelpers.LogAndAttachTraceInfo(refresh, LOG,
                                    "Monitored server " + refresh.ServerName +
                                    " is running an unsupported SQL Server version.  The reported version is " + ver.Version
                                    , true);
                    conn.Close();
                    FireCompletion(refresh, Result.Unsupported);
                }
            }
            catch (Exception exception)
            {
                LOG.WarnFormat("Error starting Session Details collector: {0}", exception.Message);

                if (sdtCollector != null)
                    sdtCollector.Dispose();
                FireCompletion(null, Result.Failure);
            }
        }

        /// <summary>
        /// Callback used to process the data returned from the sessions collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void SessionsCallback(object sender, CollectorCompleteEventArgs e)
        {
            try
            {
                using (ICollector collector = sender as ICollector)
                {
                    if (e.Result == Result.Success)
                    {
                        using (SqlDataReader rd = e.Value as SqlDataReader)
                        {
                            InterpretSessions(rd);
                        }
                        FireCompletion(refresh, Result.Success);
                    }
                    else
                    {
                        ProbeHelpers.LogAndAttachTraceInfo(refresh, LOG, "Error executing Server Overview Collector: {0}", e.Exception, false);
                        FireCompletion(null, Result.Failure);
                    }
                }

            }
            catch (Exception exception)
            {
                LOG.WarnFormat("Error processing Server Overview: {0}", exception.Message);

                FireCompletion(null, Result.Failure);
            }
        }

        private void InterpretSessions(SqlDataReader dataReader)
        {
            //ReadProcessesSummary(dataReader);
            ReadProcessesDetails(dataReader);
        }

        //private void ReadProcessesSummary(SqlDataReader dataReader)
        //{
        //    try
        //    {
        //        dataReader.Read();

        //        //processesDiagnostics.SetProcessesConsumingCpu(dataReader.GetInt32(1));
        //        processesDiagnostics.SetBlockedProcesses(dataReader.GetInt32(2));
        //        processesDiagnostics.SetOpenTransactions(dataReader.GetInt32(3));
        //        processesDiagnostics.SetUserProcesses(dataReader.GetInt32(4));
        //        processesDiagnostics.SetSystemProcesses(dataReader.GetInt32(5));
        //        processesDiagnostics.SetDistinctClientComputers(dataReader.GetInt32(6));
        //    }
        //    catch (Exception exception)
        //    {
        //        Logger.Error(SR.ExceptionReadProcessesSummaryFailed(_configuration.ConnectionInfo.ServerName), exception);
        //    }
        //    finally
        //    {
        //        dataReader.NextResult();
        //    }
        //}

        private void ReadProcessesDetails(SqlDataReader dataReader)
        {
            try
            {
                bool continueReading = dataReader.HasRows;

                while (continueReading)
                {
                    if (dataReader.Read())
                    {
                        ReadSessionDetails(dataReader);
                    }

                    continueReading = dataReader.NextResult();
                }
            }
            catch (Exception exception)
            {
                //Logger.Error(SR.ExceptionReadProcessesDetailsFailed(_configuration.ConnectionInfo.ServerName), exception);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private Session ReadSessionDetails(SqlDataReader dataReader)
        {
            Session session = null;

            try
            {
                session = new Session();

                session.Spid = Convert.ToInt32(dataReader.GetInt16(0));
                session.UserName = dataReader.GetString(1);
                session.Workstation = dataReader.GetString(2);
                session.Status = dataReader.GetString(3);
                session.Application = dataReader.GetString(4);
                session.Command = dataReader.GetString(5);
                session.Database = dataReader.GetString(6);
                session.Cpu = new CpuTime(dataReader.GetInt32(7));
                session.Memory = new FileSize(dataReader.GetInt32(8) * 8);
                session.PhysicalIo = dataReader.GetInt32(9);
                session.BlockedBy = Convert.ToInt32(dataReader.GetInt16(10));
                session.LoggedInSince = dataReader.GetDateTime(11);
                //Session.LastActivity = dataReader.GetString(12);
                session.OpenTransactions = Convert.ToInt32(dataReader.GetInt16(14));
                session.WorkstationNetAddress = dataReader.GetString(15);
                session.ExecutionContext = dataReader.GetInt32(16);

                refresh.SessionList.Add(session);
            }
            catch (Exception exception)
            {
                //Logger.Error(SR.ExceptionReadSessionDetailsFailed(_configuration.ConnectionInfo.ServerName), exception);
            }

            return session;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

       

    }
}
