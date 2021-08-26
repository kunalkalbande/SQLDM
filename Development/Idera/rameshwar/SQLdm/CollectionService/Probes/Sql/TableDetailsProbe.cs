//------------------------------------------------------------------------------
// <copyright file="TableDetailsProbe.cs" company="Idera, Inc.">
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
    using Common.Snapshots;
    using Common.Services;


    /// <summary>
    /// Enter a description for this class
    /// </summary>
    internal sealed class TableDetailsProbe : SqlBaseProbe
    {
        #region fields

        private TableDetail tableDetail = null;
        private TableDetailConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TableDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public TableDetailsProbe(SqlConnectionInfo connectionInfo, TableDetailConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("TableDetailsProbe");
            tableDetail = new TableDetail(connectionInfo);
            this.cloudProviderId = cloudProviderId;
            this.config = config;
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
                StartTableDetailsCollector();
            }
            else
            {
                FireCompletion(tableDetail, Result.Success);
            }
           }


        /// <summary>
        /// Define the TableDetails collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void TableDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildTableDetailsCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(TableDetailsCallback));
        }

        /// <summary>
        /// Starts the Table Details collector.
        /// </summary>
        private void StartTableDetailsCollector()
        {
            StartGenericCollector(new Collector(TableDetailsCollector), tableDetail, "StartTableDetailsCollector", "Table Details", TableDetailsCallback, new object[] { });
        }

        /// <summary>
        /// Define the TableDetails callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretTableDetails(rd);
            }
            FireCompletion(tableDetail, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the TableDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void TableDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(TableDetailsCallback), tableDetail, "TableDetailsCallback", "Table Details",
                            sender, e);
        }

        /// <summary>
        /// Interpret TableDetails data
        /// </summary>
        private void InterpretTableDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretTableDetails"))
            {
                try
                {
                    ReadTableName(dataReader);
                    ReadTableReferencedBy(dataReader);
                    ReadTableReferences(dataReader);
                    ReadIndexes(dataReader);
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(tableDetail, LOG, "Error interpreting Table Details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(tableDetail);
                }
            }
        }


        private void ReadTableName(SqlDataReader dataReader)
        {
            try
            {
                if (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(0)) tableDetail.TableId = dataReader.GetInt32(0);
                    if (!dataReader.IsDBNull(1)) tableDetail.DatabaseName = dataReader.GetString(1);
                    if (!dataReader.IsDBNull(2)) tableDetail.TableName = dataReader.GetString(2);
                    if (!dataReader.IsDBNull(3)) tableDetail.Schema = dataReader.GetString(3);
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(tableDetail, LOG, "Error interpreting Table Name: {0}", e,
                                                          false);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadTableReferencedBy(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    string user = dataReader.GetString(0);
                    string name = dataReader.GetString(1);
                    string keyName = dataReader.GetString(2);
                    string typeFromBatch = dataReader.GetString(3);
                    string action = dataReader.GetString(4);
                    tableDetail.ReferencedBy.Add(new TableDependency(action, name, keyName, typeFromBatch, user));
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(tableDetail, LOG, "Error interpreting Table Referenced By: {0}", e,
                                                          false);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadTableReferences(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    string keyName = dataReader.GetString(0);
                    string typeFromBatch = dataReader.GetString(1);
                    string action = dataReader.GetString(2);
                    // Set name and user equal to the parent object, as these are by definition child objects
                    tableDetail.References.Add(new TableDependency(action, tableDetail.TableName, keyName, typeFromBatch, tableDetail.Schema));
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(tableDetail, LOG, "Error interpreting Table References: {0}", e,
                                                         false);
            }
            finally
            {
                dataReader.NextResult();
            }
        }

        private void ReadIndexes(SqlDataReader dataReader)
        {
            try
            {
                int indexID = 0;
                int dataSpaceUsed = 0;
                int imageTextSpaceUsed = 0;
                int tableSpaceUsed = 0;
                int totalSpaceUsed = 0;
                int nonClusteredIndexSize = 0;
                int indexSpaceUsed = 0;

                int indid = 0;
                int name = 1;
                int used = 2;
                int rowcnt = 3;
                int dpages = 4;
                int statDate = 5;
                int updates = 6;
                int unique = 7;
                int clustered = 8;
                int fillfactor = 9;
                int indexLevels = 10;
                int filegroupName = 11;

                Index clusteredIndex = new Index();

                while (dataReader.Read())
                {
                    indexID = dataReader.GetInt16(indid);
                    Type a = dataReader.GetFieldType(fillfactor);
                    a = dataReader.GetFieldType(indexLevels);
                    Index i = new Index();
                    switch (indexID)
                    {
                        // Table or Clustered Index
                        case 0:
                        case 1:

                            //Set data space used to Dpages reading from table or clustered index
                            dataSpaceUsed = dataReader.GetInt32(dpages);

                            //Set total table space used to Used reading from table or clustered index
                            tableSpaceUsed = dataReader.GetInt32(used);

                            if (indexID == 1)
                            {
                                clusteredIndex.Name = dataReader.GetString(name);
                                if (!dataReader.IsDBNull(name)) clusteredIndex.Name = dataReader.GetString(name);
                                if (!dataReader.IsDBNull(rowcnt)) clusteredIndex.Tablerows = dataReader.GetInt32(rowcnt);
                                if (!dataReader.IsDBNull(statDate)) clusteredIndex.LastUpdate = dataReader.GetDateTime(statDate);
                                if (!dataReader.IsDBNull(unique)) clusteredIndex.Unique = dataReader.GetBoolean(unique);
                                if (!dataReader.IsDBNull(updates)) clusteredIndex.RowsModifiedSinceStatistics = dataReader.GetInt32(updates);
                                if (!dataReader.IsDBNull(clustered)) clusteredIndex.Clustered = dataReader.GetBoolean(clustered);
                                if (!dataReader.IsDBNull(fillfactor)) clusteredIndex.FillFactor = dataReader.GetInt16(fillfactor);
                                if (!dataReader.IsDBNull(indexLevels)) clusteredIndex.Levels = dataReader.GetInt32(indexLevels);
                                if (!dataReader.IsDBNull(filegroupName)) clusteredIndex.FilegroupName = dataReader.GetString(filegroupName);
                            }

                            break;

                        // Text and Image Data
                        case 255:
                            // Set text and image size used to Used reading from Text and Image Data
                            imageTextSpaceUsed = dataReader.GetInt32(used);

                            break;
                        // Non-Clustered Indexes
                        default:
                            nonClusteredIndexSize += dataReader.GetInt32(used) * 8;

                            if (!dataReader.IsDBNull(name)) i.Name = dataReader.GetString(name);
                            if (!dataReader.IsDBNull(used)) i.Size.Pages = dataReader.GetInt32(used);
                            if (!dataReader.IsDBNull(rowcnt)) i.Tablerows = dataReader.GetInt32(rowcnt);
                            if (!dataReader.IsDBNull(statDate)) i.LastUpdate = dataReader.GetDateTime(statDate);
                            if (!dataReader.IsDBNull(unique)) i.Unique = dataReader.GetBoolean(unique);
                            if (!dataReader.IsDBNull(updates)) i.RowsModifiedSinceStatistics = dataReader.GetInt32(updates);
                            if (!dataReader.IsDBNull(clustered)) i.Clustered = dataReader.GetBoolean(clustered);
                            if (!dataReader.IsDBNull(indexLevels)) i.Levels = dataReader.GetInt32(indexLevels);
                            if (!dataReader.IsDBNull(fillfactor)) i.FillFactor = dataReader.GetInt16(fillfactor);
                            if (!dataReader.IsDBNull(filegroupName)) i.FilegroupName = dataReader.GetString(filegroupName);

                            tableDetail.Indexes.Add(i);
                            break;
                    }
                }

                //Convert pages to kilobytes
                dataSpaceUsed = dataSpaceUsed * 8;
                tableSpaceUsed = tableSpaceUsed * 8;
                imageTextSpaceUsed = imageTextSpaceUsed * 8;

                //Set index space used to difference between total table space used and data space used
                indexSpaceUsed = tableSpaceUsed - dataSpaceUsed;

                //Set total space used to total of table, clustered index, and text and image data
                totalSpaceUsed = tableSpaceUsed + imageTextSpaceUsed;

                //Set clustered index
                if (clusteredIndex.Name != null)
                {
                    int clusteredIndexSize = totalSpaceUsed - (dataSpaceUsed + nonClusteredIndexSize + imageTextSpaceUsed);

                    clusteredIndex.Size.Kilobytes = clusteredIndexSize;
                    tableDetail.Indexes.Add(clusteredIndex);
                }

            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(tableDetail, LOG, "Error interpreting Table Indexes: {0}", e,
                                                        false);
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
