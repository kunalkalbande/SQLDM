//------------------------------------------------------------------------------
// <copyright file="ProcedureCacheProbe.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.Data;

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
    internal sealed class ProcedureCacheProbe : SqlBaseProbe
    {
        #region fields

        private ProcedureCache procedureCache = null;
        private ProcedureCacheConfiguration config = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcedureCacheProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skip permissions for CloudProviders</param>
        public ProcedureCacheProbe(SqlConnectionInfo connectionInfo, ProcedureCacheConfiguration config, int? cloudProviderId)
            : base(connectionInfo)
        {
            LOG = Logger.GetLogger("ProcedureCacheProbe");
            procedureCache = new ProcedureCache(connectionInfo);
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
            if (config != null && config.ReadyForCollection && cloudProviderId!=CLOUD_PROVIDER_ID_AZURE)//sqldm-30299 changes
            {
                StartProcedureCacheCollector();
            }
            else
            {
                FireCompletion(procedureCache, Result.Success);
            }
        }


        /// <summary>
        /// Define the ProcedureCache collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void ProcedureCacheCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildProcedureCacheCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(ProcedureCacheCallback));
        }

        /// <summary>
        /// Starts the Procedure Cache collector.
        /// </summary>
        private void StartProcedureCacheCollector()
        {
            StartGenericCollector(new Collector(ProcedureCacheCollector), procedureCache, "StartProcedureCacheCollector", "Procedure Cache", ProcedureCacheCallback, new object[] { });
        }

        /// <summary>
        /// Define the ProcedureCache callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ProcedureCacheCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretProcedureCache(rd);
            }
            FireCompletion(procedureCache, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the ProcedureCache collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void ProcedureCacheCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(ProcedureCacheCallback), procedureCache, "ProcedureCacheCallback", "Procedure Cache",
                            sender, e);
        }

        /// <summary>
        /// Interpret ProcedureCache data
        /// </summary>
        private void InterpretProcedureCache(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretProcedureCache"))
            {
                try
                {
                    ReadProcedureCacheSizes(dataReader);
                    ReadProcedureCacheCounters(dataReader);
                    ReadProcedureCacheHitRatio(dataReader);
                    if (config.ShowProcedureCacheList)
                    {
                        ReadProcedureCacheObjects(dataReader);
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(procedureCache, LOG, "Error interpreting Procedure Cache Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(procedureCache);
                }
            }
        }

        private void ReadProcedureCacheHitRatio(SqlDataReader dataReader)
        {
            try
            {
                while (dataReader.Read())
                {
                    if (!dataReader.IsDBNull(1))
                    {
                        switch (dataReader.GetString(1).ToLower().TrimEnd(new char[] { ' ' }))
                        {
                            case "cache hit ratio":
                                procedureCache.HitRatio = dataReader.IsDBNull(2) ? 0 : Convert.ToDouble(dataReader.GetValue(2));
                                break;
                            case "cache hit ratio base":
                                procedureCache.HitRatioBase = dataReader.IsDBNull(2) ? 0 : Convert.ToDouble(dataReader.GetValue(2));
                                break;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(procedureCache, LOG, "Error interpreting Procedure Cache Hit Ratio Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(procedureCache);
            }
            finally
            {
                // Move to the next recordset
                dataReader.NextResult();
            }
        }

       private void ReadProcedureCacheCounters(SqlDataReader dataReader)
        {
            string counterName = "";
            Double counterValue = 0;
            try
            {
                while (dataReader.Read())
                {
                    counterName = dataReader.IsDBNull(1) ? "miscellaneous" : dataReader.GetString(1).ToLower().TrimEnd(new char[] { ' ' });
                    counterValue = dataReader.IsDBNull(2) ? 0 : Convert.ToDouble(dataReader.GetValue(2));
                    if (procedureCache.ProductVersion.Major < 9)
                    {
                        switch (counterName)
                        {
                            case "adhoc sql plans":
                                procedureCache.ObjectTypes["adhoc"].HitRatio = (counterValue);
                                break;
                            case "misc. normalized trees":
                                procedureCache.ObjectTypes["rule"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["view"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["default"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["check"].HitRatio = (counterValue);
                                break;
                            case "prepared sql plans":
                                procedureCache.ObjectTypes["prepared"].HitRatio = (counterValue);
                                break;
                            case "procedure plans":
                                procedureCache.ObjectTypes["proc"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["extended procedure"].HitRatio = (counterValue);
                                break;
                            case "replication procedure plans":
                                procedureCache.ObjectTypes["replproc"].HitRatio = (counterValue);
                                break;
                            case "trigger plans":
                                procedureCache.ObjectTypes["trigger"].HitRatio = (counterValue);
                                break;
                        }
                    }
                    else
                    {
                        switch (counterName)
                        {
                            case "sql plans":
                                procedureCache.ObjectTypes["adhoc"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["prepared"].HitRatio = (counterValue);
                                break;
                            case "object plans":
                                procedureCache.ObjectTypes["proc"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["trigger"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["replproc"].HitRatio = (counterValue);
                                break;
                            case "bound trees":
                                procedureCache.ObjectTypes["rule"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["view"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["default"].HitRatio = (counterValue);
                                procedureCache.ObjectTypes["check"].HitRatio = (counterValue);
                                break;
                            case "extended stored procedures":
                                procedureCache.ObjectTypes["extended procedure"].HitRatio = (counterValue);
                                break;
                            case "temporary tables & table variables":
                                break;
                        }
                    }
                }
            }
              catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(procedureCache, LOG, "Error interpreting Procedure Cache Counters: {0}", e,
                                                    false);
                GenericFailureDelegate(procedureCache);
            }
            finally
            {
                // Move to the next recordset
                dataReader.NextResult();
            }
        }

        private void ReadProcedureCacheSizes(SqlDataReader dataReader)
        {
            string objectTypeName = "";
            Int64 counterValue = 0;
            try
            {
                while (dataReader.Read())
                {
                    objectTypeName = dataReader.IsDBNull(1) ? "miscellaneous" : dataReader.GetString(1).ToLower();
                    counterValue = dataReader.IsDBNull(2) ? 0 : Convert.ToInt64(dataReader.GetValue(2)) * 8;

                    if (!procedureCache.ObjectTypes.ContainsKey(objectTypeName))
                    {
                        procedureCache.ObjectTypes.Add(objectTypeName,new ProcedureCacheObjectType(objectTypeName));
                    }

                    procedureCache.ObjectTypes[objectTypeName].Size = new FileSize(counterValue);
                }
            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(procedureCache, LOG, "Error interpreting Procedure Cache Sizes: {0}", e,
                                                    false);
                GenericFailureDelegate(procedureCache);
            }
            finally
            {
                // Move to the next recordset
                dataReader.NextResult();
            }
        }

        private void ReadProcedureCacheObjects(SqlDataReader dataReader)
        {
            //ProcedureCacheObject cacheObject;
            try
            {
                DataRow dr;
                string objectType;
                Int64? useCount;
                procedureCache.ObjectList.BeginLoadData();
                do
                {
                    while (dataReader.Read())
                    {
                        dr = procedureCache.ObjectList.NewRow();
                        objectType = dataReader.IsDBNull(1) ? "miscellaneous" : dataReader.GetString(1).ToLower();
                        dr["Object Type"] = objectType;

                        if (!dataReader.IsDBNull(2))
                            dr["Command"] = (dataReader.GetString(2).Trim()).TrimStart(new char[] {'\n', '\r'});

                        if (!dataReader.IsDBNull(3))
                        {
                            FileSize objectSize = new FileSize(Convert.ToInt64(dataReader.GetValue(3))*8);
                            dr["Size"] = objectSize.Kilobytes;
                        }

                        if (!dataReader.IsDBNull(5)) dr["Reference Count"] = Convert.ToInt64(dataReader.GetValue(5));

                        if (!dataReader.IsDBNull(4))
                        {
                            useCount = Convert.ToInt64(dataReader.GetValue(4));
                            dr["Use Count"] = useCount;
                        }
                        else
                        {
                            useCount = null;
                        }

                        if (!dataReader.IsDBNull(6)) dr["User/Schema Name"] = dataReader.GetString(6);

                        if (!procedureCache.ObjectTypes.ContainsKey(objectType))
                        {
                            procedureCache.ObjectTypes.Add(objectType, new ProcedureCacheObjectType(objectType));
                        }

                        if (useCount.HasValue)
                        {
                            if (procedureCache.ObjectTypes[objectType].UseCount.HasValue)
                            {
                                procedureCache.ObjectTypes[objectType].UseCount += useCount;
                            }
                            else
                            {
                                procedureCache.ObjectTypes[objectType].UseCount = useCount;
                            }
                        }

                        if (procedureCache.ObjectTypes[objectType].ObjectCount.HasValue)
                        {
                            procedureCache.ObjectTypes[objectType].ObjectCount += 1;
                        }
                        else
                        {
                            procedureCache.ObjectTypes[objectType].ObjectCount = 1;
                        }


                        procedureCache.ObjectList.Rows.Add(dr);
                    }
                } while (dataReader.NextResult());
                procedureCache.ObjectList.EndLoadData();

            }
            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(procedureCache, LOG, "Error interpreting Procedure Cache Hit Ratio Collector: {0}", e,
                                                    false);
                GenericFailureDelegate(procedureCache);
            }
        }

        #endregion

        #region interface implementations

        #endregion
    }
}
