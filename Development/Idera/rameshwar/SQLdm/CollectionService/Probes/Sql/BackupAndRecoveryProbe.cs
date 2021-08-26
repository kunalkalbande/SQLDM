//------------------------------------------------------------------------------
// <copyright file="BackupAndRecoveryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    using System;
    using System.Data.SqlClient;
    using BBS.TracerX;
    using Collectors;
    using Common;
    using Common.Configuration;
    using Common.Services;
    using Common.Snapshots;

    /// <summary>
    /// //SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) - New Probe class
    /// </summary>
    internal class BackupAndRecoveryProbe : SqlBaseProbe
    {
        #region fields

        private BackupAndRecoverySnapshot snapshot = null;
        private string db;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupAndRecoveryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public BackupAndRecoveryProbe(SqlConnectionInfo connectionInfo, string DB, int? cloudProviderId)
            : base(connectionInfo)
        {
            db = DB;
            snapshot = new BackupAndRecoverySnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
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
            StartBackupAndRecoveryCollector();
        }

        /// <summary>
        /// Define the BackupAndRecoveryProbe collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void BackupAndRecoveryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd = SqlCommandBuilder.BuildBackupAndRecoveryCommand(conn, ver, db, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(BackupAndRecoveryCallback));
        }

        /// <summary>
        /// Starts the BackupAndRecovery collector.
        /// </summary>
        void StartBackupAndRecoveryCollector()
        {
            StartGenericCollector(new Collector(BackupAndRecoveryCollector), snapshot, "StartBackupAndRecoveryCollector", "BackupAndRecovery", BackupAndRecoveryCallback, new object[] { });
        }

        /// <summary>
        /// Define the BackupAndRecovery callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void BackupAndRecoveryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretBackupAndRecovery(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the BackupAndRecovery collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void BackupAndRecoveryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(BackupAndRecoveryCallback), snapshot, "BackupAndRecoveryCallback", "BackupAndRecovery",
                            sender, e);
        }

        private void InterpretBackupAndRecovery(SqlDataReader datareader)
        {
            using (LOG.DebugCall("InterpretBackupAndRecovery"))
            {
                try
                {
                    snapshot.GeneralInfo.Columns.Add("Name", typeof(string));
                    snapshot.GeneralInfo.Columns.Add("Value", typeof(string));

                    snapshot.BackupfileInfo.Columns.Add("FileName", typeof(string));
                    snapshot.BackupfileInfo.Columns.Add("StartDateTime", typeof(DateTime));
                    snapshot.BackupfileInfo.Columns.Add("FileExists", typeof(Int32));

                    snapshot.DBFileInfo.Columns.Add("PhysicalName", typeof(string));

                    snapshot.BackupDateInfo.Columns.Add("DaysOld", typeof(Int32));
                    snapshot.BackupDateInfo.Columns.Add("BackupStartDate", typeof(DateTime));

                    snapshot.DBName = db;

                    while (datareader.Read())
                    {
                        snapshot.GeneralInfo.Rows.Add(
                           ProbeHelpers.ToString(datareader,"Name"),
                           ProbeHelpers.ToString(datareader,"Value")
                        );
                    }

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.BackupfileInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader,"FileName"),
                                ProbeHelpers.ToDateTime(datareader,"StartDateTime"),
                                ProbeHelpers.ToInt32(datareader,"FileExists")
                                );
                        }
                    }

                    if (datareader.NextResult())
                    {
                        while (datareader.Read())
                        {
                            snapshot.DBFileInfo.Rows.Add(
                                ProbeHelpers.ToString(datareader,"physical_name")
                                );
                        }
                    }

                    if (datareader.NextResult())
                    {
                        //Changes done to support SDR-R7 (while replaced with if same as doctor)
                        if (datareader.Read())
                        {
                            snapshot.BackupDateInfo.Rows.Add(
                                ProbeHelpers.ToInt32(datareader, "DaysOld"),
                                ProbeHelpers.ToDateTime(datareader, "BackupStartDate")
                                );
                        }
                        else
                        {
                            DateTime BackupStartDate;
                            if (snapshot.GeneralInfo.Rows.Count == 0 || !DateTime.TryParse(snapshot.GeneralInfo.Rows[0][1].ToString(), out BackupStartDate))
                                BackupStartDate = DateTime.MinValue;
                            snapshot.BackupDateInfo.Rows.Add(
                               Convert.ToInt64((DateTime.Now - BackupStartDate).TotalDays),
                                DateTime.MinValue);
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                        LOG,
                                                        "Error interpreting BackupAndRecovery Collector: {0}",
                                                        e,
                                                        false);
                    GenericFailureDelegate(snapshot);
                }
            }
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
