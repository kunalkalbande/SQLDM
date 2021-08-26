//------------------------------------------------------------------------------
// <copyright file="BackupRestoreHistoryProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using BBS.TracerX;
using Idera.SQLdm.CollectionService.Probes.Collectors;
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Services;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class BackupRestoreHistoryProbe : SqlBaseProbe
    {
        #region fields

        private BackupRestoreHistory backupRestore = null;
        private BackupRestoreHistoryConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BackupRestoreHistoryProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public BackupRestoreHistoryProbe(SqlConnectionInfo connectionInfo, BackupRestoreHistoryConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("BackupRestoreHistoryProbe");
            backupRestore = new BackupRestoreHistory(connectionInfo);
            this.config = config;
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
            if (config != null && config.ReadyForCollection)
            {
                StartBackupRestoreHistoryCollector();
            }
            else
            {
                FireCompletion(backupRestore, Result.Success);
            }
        }

        /// <summary>
        /// Define the BackupRestoreHistory collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void BackupRestoreHistoryCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildBackupRestoreHistoryCommand(conn, ver, config);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(BackupRestoreHistoryCallback));
        }

        /// <summary>
        /// Starts the Backup Restore History collector.
        /// </summary>
        private void StartBackupRestoreHistoryCollector()
        {
            StartGenericCollector(new Collector(BackupRestoreHistoryCollector), backupRestore,
                                   "StartBackupRestoreHistoryCollector", "Backup Restore History", BackupRestoreHistoryCallback, new object[] {config.ShowBackups, config.ShowRestores, config.ShowLogicalFileNames });
        }

        /// <summary>
        /// Define the BackupRestoreHistory callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void BackupRestoreHistoryCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretBackupRestoreHistory(rd);
            }
            FireCompletion(backupRestore, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the BackupRestoreHistory collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void BackupRestoreHistoryCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(BackupRestoreHistoryCallback), backupRestore,
                            "BackupRestoreHistoryCallback", "Backup Restore History",
                            sender, e);
        }

        /// <summary>
        /// Interpret BackupRestoreHistory data
        /// </summary>
        private void InterpretBackupRestoreHistory(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretBackupRestoreHistory"))
            {
                try
                {
                    if (config.ShowBackups)
                    {
                        backupRestore.BackupHistory.BeginLoadData();
                        ReadBackupHistory(dataReader);
                        backupRestore.BackupHistory.EndLoadData();
                    }

                    if (config.ShowRestores)
                    {
                        backupRestore.RestoreHistory.BeginLoadData();
                        ReadRestoreHistory(dataReader);
                        backupRestore.RestoreHistory.BeginLoadData();
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(backupRestore, LOG,
                                                        "Error interpreting Backup Restore History Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(backupRestore);
                }
            }
        }

        private void ReadBackupHistory(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    DataRow dr = backupRestore.BackupHistory.NewRow();
                    for (int i = 0; i < 7; i++)
                    {
                        if (!dataReader.IsDBNull(i))
                        {
                            switch (i)
                            {
                                case 1:
                                    dr[i] = (BackupType) dataReader.GetString(i).ToCharArray()[0];
                                    break;
                                case 4:
                                    FileSize fs = new FileSize();
                                    fs.Bytes = dataReader.GetInt64(i);
                                    dr[i] = fs;
                                    break;
                                default:
                                    dr[i] = dataReader.GetValue(i);
                                    break;
                            }
                        }
                    }
                    backupRestore.BackupHistory.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(backupRestore, LOG,
                                                    "Error interpreting Backup History Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(backupRestore);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadRestoreHistory(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    DataRow dr = backupRestore.RestoreHistory.NewRow();
                    for (int i = 0; i < dataReader.FieldCount; i++)
                    {
                        if (!dataReader.IsDBNull(i))
                        {
                            switch (i)
                            {
                                case 1:
                                    dr[i] = (BackupType) dataReader.GetString(i).ToCharArray()[0];
                                    break;
                                case 7:
                                    dr[i] = dataReader.GetBoolean(7);
                                    break;
                                default:
                                    dr[i] = dataReader.GetValue(i);
                                    break;
                            }
                        }
                    }
                    backupRestore.RestoreHistory.Rows.Add(dr);
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(backupRestore, LOG,
                                                    "Error interpreting Restore History Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(backupRestore);
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}