//------------------------------------------------------------------------------
// <copyright file="DatabaseFileInfoProbe.cs" company="Idera, Inc.">
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
    internal class GetMasterFilesProbe : SqlBaseProbe
    {
        #region fields

        private GetMasterFilesSnapshot snapshot = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFileInfo"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public GetMasterFilesProbe(SqlConnectionInfo connectionInfo, int? cloudProviderId)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            snapshot = new GetMasterFilesSnapshot(connectionInfo);
            LOG = Logger.GetLogger("SessionsProbe");
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
            StartDatabaseFileInfoCollector();
        }

        /// <summary>
        /// Define the DatabaseFileInfo collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        void DatabaseFileInfoCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                           SqlCommandBuilder.BuildGetMasterFilesCommand(conn, ver, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(DatabaseFileInfoCallback));
        }

        /// <summary>
        /// Starts the DatabaseFileInfo collector.
        /// </summary>
        void StartDatabaseFileInfoCollector()
        {
            StartGenericCollector(new Collector(DatabaseFileInfoCollector), snapshot, "StartDatabaseFileInfoCollector", "DatabaseFileInfo", DatabaseFileInfoCallback, new object[] { });
        }

        /// <summary>
        /// Define the DatabaseFileInfo callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseFileInfoCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretDatabaseFileInfo(rd);
            }
            FireCompletion(snapshot, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the DatabaseFileInfo collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        void DatabaseFileInfoCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(DatabaseFileInfoCallback), snapshot, "DatabaseFileInfoCallback", "DatabaseFileInfo",
                            sender, e);
        }

        private void InterpretDatabaseFileInfo(SqlDataReader datareader)
        {
            snapshot.DatabaseFileInfo.Columns.Add("database_id", typeof(int));
            snapshot.DatabaseFileInfo.Columns.Add("dbname", typeof(string));
            snapshot.DatabaseFileInfo.Columns.Add("file_name", typeof(string));

            snapshot.DatabaseFileInfo.Columns.Add("physical_name", typeof(string));
            snapshot.DatabaseFileInfo.Columns.Add("file_id", typeof(int));
            snapshot.DatabaseFileInfo.Columns.Add("type", typeof(byte));

            snapshot.DatabaseFileInfo.Columns.Add("type_desc", typeof(string));
            snapshot.DatabaseFileInfo.Columns.Add("growth", typeof(Int32));
            snapshot.DatabaseFileInfo.Columns.Add("is_percent_growth", typeof(bool));

            snapshot.DatabaseFileInfo.Columns.Add("size", typeof(long));
            snapshot.DatabaseFileInfo.Columns.Add("initial_size", typeof(int));

            snapshot.DatabaseFileInfo.Columns.Add("max_size", typeof(int));
            snapshot.DatabaseFileInfo.Columns.Add("is_auto_shrink_on", typeof(bool));

            while (datareader.Read())
            {
                snapshot.DatabaseFileInfo.Rows.Add(
                    ProbeHelpers.ToInt32(datareader,"database_id"),
                    ProbeHelpers.ToString(datareader,"dbname"),
                    ProbeHelpers.ToString(datareader,"file_name"),

                    ProbeHelpers.ToString(datareader,"physical_name"),
                    ProbeHelpers.ToInt32(datareader,"file_id"),
                    ProbeHelpers.ToByte(datareader, "type"),

                    ProbeHelpers.ToString(datareader,"type_desc"),
                    ProbeHelpers.ToInt32(datareader,"growth"),
                    ProbeHelpers.ToBoolean(datareader,"is_percent_growth"),

                    ProbeHelpers.ToInt64(datareader,"size"),
                    ProbeHelpers.ToInt32(datareader,"initial_size"),

                    ProbeHelpers.ToInt32(datareader,"max_size"),
                    ProbeHelpers.ToBoolean(datareader,"is_auto_shrink_on")
                );
            }
        }


        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
