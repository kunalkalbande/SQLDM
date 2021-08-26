//------------------------------------------------------------------------------
// <copyright file="LockDetailsProbe.cs" company="Idera, Inc.">
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
    using Idera.SQLdm.CollectionService.Helpers;
    using System.Threading;
    using System.Collections.Generic;

    /// <summary>
    /// On Demand Probe for Lock Details
    /// </summary>
    internal sealed class LockDetailsProbe : SqlBaseProbe
    {
        #region fields

        private LockDetails lockDetails = null;
        private LockDetailsConfiguration config = null;
        private List<string> cloudDBNames = new List<string>();
        private int numberOfDatabases = 0;
        //private int? cloudProviderId = null;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LockDetailsProbe"/> class.
        /// </summary>
        /// <param name="connectionInfo">The connection info.</param>
        /// <param name="cloudProviderId">Skips permission check for cloud servers</param>
        public LockDetailsProbe(SqlConnectionInfo connectionInfo, LockDetailsConfiguration configuration,int? cloudProviderId = null)
            : base(connectionInfo)
        {
            this.cloudProviderId = cloudProviderId;
            LOG = Logger.GetLogger("LockDetailsProbe");
            lockDetails = new LockDetails(connectionInfo);
            config = configuration;
            if (config != null && config.PreviousLockStatistics != null)
            {
                lockDetails.LockCounters = config.PreviousLockStatistics;
            }
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
            if (config != null && config.ReadyForCollection && cloudProviderId != CLOUD_PROVIDER_ID_AZURE)
            {
                StartLockDetailsCollector();
            }
            else if(config != null && config.ReadyForCollection && cloudProviderId==CLOUD_PROVIDER_ID_AZURE)
            {
                cloudDBNames = CollectionHelper.GetDatabases(connectionInfo, LOG);

                numberOfDatabases = 0;
                if (cloudDBNames.Count > 0)
                {
                    StartLockDetailsCollectorAzure();
                }
                else
                {
                    FireCompletion(lockDetails, Result.Success);
                }
            }
            else
            {
                FireCompletion(lockDetails, Result.Success);
            }
        }
        // Azure Db Lock Stats Collector
        /// <summary>
        /// Define the LockDetails collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void LockDetailsCollectorAzure(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver,string dbName)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildLockDetailsCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LockDetailsCallbackAzure));
        }

        /// <summary>
        /// Starts the Lock Details collector.
        /// </summary>
        private void StartLockDetailsCollectorAzure()
        { 
            StartGenericCollectorDatabase(new CollectorDatabase(LockDetailsCollectorAzure),
                lockDetails,
                "StartLockDetailsCollector", 
                "Lock Details",
              LockDetailsCallbackAzure, cloudDBNames[numberOfDatabases], new object[] { });
        }

       

        /// <summary>
        /// Callback used to process the data returned from the LockDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LockDetailsCallbackAzure(object sender, CollectorCompleteEventArgs e)
        {
            if (Thread.CurrentThread.IsThreadPoolThread)
            {
                LOG.Debug("Pushing LockDetailsCallbackAzure to work queue.");
                QueueCallback(lockDetails, sender as SqlCollector, LockDetailsCallbackAzure, e);
                return;
            }

            using (LOG.DebugCall("LockDetailsCallbackAzure"))
            {
                NextCollector nextCollector = new NextCollector(StartLockDetailsCollectorAzure);
                if (cloudProviderId == CLOUD_PROVIDER_ID_AZURE)
                {
                    Interlocked.Increment(ref numberOfDatabases);

                }

                // Ensure that the Lock Details Probe is completed for the last database
                if (numberOfDatabases >= cloudDBNames.Count)
                {
                    FireCompletion(lockDetails, Result.Success);
                }
                else
                {
                    GenericCallback(new CollectorCallback(LockDetailsCallbackAzure), lockDetails,
                         "LockDetailsCallbackAzure", "Lock Details Azure", new FailureDelegate(GenericFailureDelegate), new FailureDelegate(GenericFailureDelegate),
                            nextCollector, sender, e, true, true);
                    /*GenericCallback(new CollectorCallback(LockDetailsCallbackAzure), lockDetails, "LockDetailsCallbackAzure", "Lock Details Azure", nextCollector,
                        sender, e);*/
                }
            }
 
        }
        /// <summary>
        /// Define the LockDetails callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LockDetailsCallbackAzure(CollectorCompleteEventArgs e)
        {
            if (e.Result == Result.Success)
            {
                using (SqlDataReader rd = e.Value as SqlDataReader)
                {
                    InterpretLockDetails(rd);
                }
            }
            if(numberOfDatabases>=cloudDBNames.Count)
            {
                FireCompletion(lockDetails, Result.Success);
            }
        }












        /// <summary>
        /// Define the LockDetails collector
        /// </summary>
        /// <param name="conn">Open SQL connection</param>
        /// <param name="sdtCollector">Standard SQL collector</param>
        /// <param name="ver">Server version</param>
        private void LockDetailsCollector(SqlConnection conn, SqlCollector sdtCollector, ServerVersion ver)
        {
            SqlCommand cmd =
                SqlCommandBuilder.BuildLockDetailsCommand(conn, ver, config, cloudProviderId);
            sdtCollector = new SqlCollector(cmd, true);
            sdtCollector.BeginCollection(new EventHandler<CollectorCompleteEventArgs>(LockDetailsCallback));
        }

        /// <summary>
        /// Starts the Lock Details collector.
        /// </summary>
        private void StartLockDetailsCollector()
        {
            // Passing Permission Violation Callback LockDetailsCallback
            StartGenericCollector(new Collector(LockDetailsCollector), lockDetails, "StartLockDetailsCollector", "Lock Details", LockDetailsCallback, new object[] { });
        }

        /// <summary>
        /// Define the LockDetails callback
        /// </summary>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LockDetailsCallback(CollectorCompleteEventArgs e)
        {
            using (SqlDataReader rd = e.Value as SqlDataReader)
            {
                InterpretLockDetails(rd);
            }
            FireCompletion(lockDetails, Result.Success);
        }

        /// <summary>
        /// Callback used to process the data returned from the LockDetails collector.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The CollectorCompleteEventArgs instance containing the event data.</param>
        private void LockDetailsCallback(object sender, CollectorCompleteEventArgs e)
        {
            GenericCallback(new CollectorCallback(LockDetailsCallback), lockDetails, "LockDetailsCallback", "Lock Details",
                            sender, e);
        }

        /// <summary>
        /// Interpret LockDetails data
        /// </summary>
        private void InterpretLockDetails(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("InterpretLockDetails"))
            {
                ReadLockList(dataReader);
                ProbeHelpers.ReadLockStatistics(dataReader, lockDetails, LOG, lockDetails.LockCounters, GenericFailureDelegate);
            }
        }

        private void ReadLockList(SqlDataReader dataReader)
        {
            using (LOG.DebugCall("ReadLockList"))
            {
                try
                {
                    while (dataReader.Read())
                    {
                        if (dataReader.FieldCount >= 15)
                        {
                            Lock lockItem = new Lock();
                            if (!dataReader.IsDBNull(0)) lockItem.Blocking = dataReader.GetBoolean(0);
                            if (!dataReader.IsDBNull(1)) lockItem.Blocked_by = dataReader.GetInt32(1);
                            if (!dataReader.IsDBNull(2)) lockItem.Database = dataReader.GetString(2);
                            if (!dataReader.IsDBNull(3)) lockItem.Host = dataReader.GetString(3);
                            if (!dataReader.IsDBNull(4)) lockItem.ModeShortString = dataReader.GetString(4);
                            if (!dataReader.IsDBNull(5)) lockItem.ObjectName = dataReader.GetString(5);
                            if (!dataReader.IsDBNull(6)) lockItem.ObjectSchema = dataReader.GetString(6);
                            if (!dataReader.IsDBNull(7)) lockItem.Application = dataReader.GetString(7);
                            if (!dataReader.IsDBNull(8)) lockItem.Spid = dataReader.GetInt32(8);
                            if (!dataReader.IsDBNull(9)) lockItem.StatusString = dataReader.GetString(9);
                            if (!dataReader.IsDBNull(10)) lockItem.TypeString = dataReader.GetString(10);
                            if (!dataReader.IsDBNull(11)) lockItem.User = dataReader.GetString(11);
                            if (!dataReader.IsDBNull(12)) lockItem.InstanceCount = dataReader.GetDecimal(12);
                            if (!dataReader.IsDBNull(13)) lockItem.WaitTime = TimeSpan.FromMilliseconds(dataReader.GetInt64(13));
                            if (!dataReader.IsDBNull(14)) lockItem.Command = dataReader.GetString(14);

                            lockDetails.LockList.Add(lockItem.Id,lockItem);
                            
                        }
                        else
                        {
                            lockDetails.HasBeenRowLimited = true;
                            LOG.Warn(
                                "The lock details rowcount limiter has been exceeded.  Collection of the lock list has been cancelled to prevent server congestion.");
                        }
                    }
                }
                catch (Exception e)
                {
                    ProbeHelpers.LogAndAttachToSnapshot(lockDetails, LOG, "Error interpreting Lock Details Collector: {0}", e,
                                                        false);
                    GenericFailureDelegate(lockDetails);
                }
                finally
                {
                    dataReader.NextResult();
                }
            }
        }
        #endregion

        #region interface implementations

        #endregion
    }
}
