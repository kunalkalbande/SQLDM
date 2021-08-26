using System;
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration.ServerActions;
using Idera.SQLdm.Common.Configuration;
using Idera.SQLdm.Common.Snapshots;
using System.Data.SqlClient;
using Idera.PrescriptiveAnalytics.PrescriptiveAnalyzer.Common.Recommendations;

namespace Idera.SQLdm.CollectionService.Probes.Sql
{
    internal class PrescriptiveOptimizationProbe : SqlBaseProbe
    {
        PrescriptiveScriptConfiguration configuration = null;
        bool isUndoScript = false;
        //private Snapshot snapshot = null;
        //Creating new snapshot to support storage of status and error of optimization
        private PrescriptiveOptimizationStatusSnapshot snapshot = null;

        public PrescriptiveOptimizationProbe(SqlConnectionInfo connectionInfo, PrescriptiveScriptConfiguration configuration, int? cloudProviderId)
            : base(connectionInfo)
        {
            //new ServerActionSnapshot(connectionInfo.InstanceName);
            this.configuration = configuration;
            snapshot = new PrescriptiveOptimizationStatusSnapshot(this.configuration.Recommendation);
            this.cloudProviderId = cloudProviderId;
        }

        /// <summary>
        /// Starts the probe, virtual method called by BaseProbe.BeginProbe().
        /// If this returns cleanly, probe must call FireCompletion()
        /// </summary>
        protected override void Start()
        {
            if (configuration != null && ((OnDemandConfiguration)configuration).ReadyForCollection)
            {
                StartOptimizationCollector();
            }            
            else
            {
                FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Success);
            }
        }

        /// <summary>
        /// Starts the Configuration collector.
        /// </summary>
        void StartOptimizationCollector()
        {
            try
            {
                //using (var connection = new SqlConnection(connectionInfo.ConnectionString))
                //{
                    //connection.Open();
                    //SqlCommandBuilder.RunPrescriptiveOptimizeCommand(connectionInfo, connection, configuration);
                    SqlConnection connection = OpenConnection();
                    if (configuration.ScriptType == PrescriptiveScriptType.Optimize)
                    {
                        isUndoScript = false;
                        BeginOptimization(snapshot.Recommendations, connection);
                    }
                    else
                    {
                        isUndoScript = true;
                        BeginUndo(snapshot.Recommendations, connection);
                    }


                    //try
                    //{
                    //    foreach (SqlCommand cm in cmd)
                    //    {
                    //        cm.Transaction = transaction;
                    //        cm.ExecuteNonQuery();
                    //    }
                    //    transaction.Commit();
                    //    FireCompletion(snapshot, Result.Success);
                    //}
                    //catch(Exception e)
                    //{
                    //    transaction.Rollback();
                    //    ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                    //                               LOG,
                    //                               "Error Executing Optimization Collector: {0}",
                    //                               e,
                    //                               false);
                    //    GenericFailureDelegate(snapshot);
                    //}
                //}

                FireCompletion(snapshot, Idera.SQLdm.Common.Services.Result.Success);

            }

            catch (Exception e)
            {
                ProbeHelpers.LogAndAttachToSnapshot(snapshot,
                                                    LOG,
                                                    "Error Executing Optimization Collector: {0}",
                                                    e,
                                                    false);
                GenericFailureDelegate(snapshot);
            }
        }
        private void BeginOptimization(List<IRecommendation> recommendations, SqlConnection connection)
        {
            try
            {
                List<SqlCommand> cmd = null;
                LOG.InfoFormat("Running optimization scripts for recommendations.");
                foreach (var recommendation in recommendations)
                {
                    try
                    {
                        string tsql = string.Empty;

                        cmd = SqlCommandBuilder.BuildPrescriptiveOptimizeCommand(connectionInfo, connection, recommendation, isUndoScript);

                        bool isTransactionLessScript = false;
                        ITransactionLessScript transactionLessScript = recommendation as ITransactionLessScript;
                        if (transactionLessScript != null)
                        {
                            if (isUndoScript && transactionLessScript.IsUndoScriptTransactionLess)
                                isTransactionLessScript = true;
                            if (!isUndoScript && transactionLessScript.IsScriptTransactionLess)
                                isTransactionLessScript = true;
                        }
                        if (isTransactionLessScript)
                            RunBatches(connection, null, cmd, recommendation);
                        else
                        {
                            using (SqlTransaction tran = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                            {
                                RunBatches(connection, tran, cmd, recommendation);
                            }
                        } 
                        //using (SqlTransaction tran = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        //{
                        //    RunBatches(connection, tran, cmd, recommendation);
                        //}
                        recommendation.OptimizationStatus = RecommendationOptimizationStatus.OptimizationCompleted;
                        recommendation.OptimizationErrorMessage = string.Empty;
                        LOG.InfoFormat("Optimization completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        recommendation.OptimizationStatus = RecommendationOptimizationStatus.OptimizationException;
                        recommendation.OptimizationErrorMessage = ex.Message;
                        LOG.InfoFormat("Optimization failed. Exception occured : " + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void BeginUndo(List<IRecommendation> recommendations, SqlConnection connection)
        {
            List<SqlCommand> cmd = null;
            LOG.InfoFormat("Running undo scripts for recommendations.");
            foreach (var recommendation in recommendations)
            {
                try
                {
                    string tsql = string.Empty;

                    cmd = SqlCommandBuilder.BuildPrescriptiveOptimizeCommand(connectionInfo, connection, recommendation, isUndoScript);
                    bool isTransactionLessScript = false;
                    ITransactionLessScript transactionLessScript = recommendation as ITransactionLessScript;
                    if (transactionLessScript != null)
                    {
                        if (isUndoScript && transactionLessScript.IsUndoScriptTransactionLess)
                            isTransactionLessScript = true;
                        if (!isUndoScript && transactionLessScript.IsScriptTransactionLess)
                            isTransactionLessScript = true;
                    }
                    if (isTransactionLessScript)
                        RunBatches(connection, null, cmd, recommendation);
                    else
                    {
                        using (SqlTransaction tran = connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
                        {
                            RunBatches(connection, tran, cmd, recommendation);
                        }
                    }
                    recommendation.OptimizationStatus = RecommendationOptimizationStatus.OptimizationUndone;
                    recommendation.OptimizationErrorMessage = string.Empty;
                    LOG.InfoFormat("Undo completed successfully.");
                }
                catch (Exception ex)
                {
                    recommendation.OptimizationStatus = RecommendationOptimizationStatus.OptimizationUndoneException;
                    recommendation.OptimizationErrorMessage = ex.Message;
                    LOG.InfoFormat("Undo failed. Exception occured : "+ ex.Message);
                }

            }
        }
        //To run batches of scripts
        private void RunBatches(SqlConnection conn, SqlTransaction transaction, List<SqlCommand> cmdList, IRecommendation currentRecommendation)
        {
            if (null == cmdList) return;
            int runCount = 0;
            string currentBatchCommand = string.Empty;
            try
            {
                foreach (var cmd in cmdList)
                {
                    currentBatchCommand = cmd.ToString();
                    if (transaction != null)
                        cmd.Transaction = transaction;
                    cmd.ExecuteNonQuery();
                    runCount++;
                }
                if (transaction != null)
                    transaction.Commit();
            }
            catch (Exception exOuter)
            {
                if (transaction != null)
                    transaction.Rollback();
                LOG.Error(currentBatchCommand + " for recommedation " + currentRecommendation.FindingText + " Failed Exception: ", exOuter);
                throw exOuter;
            }
            finally
            {
                LOG.InfoFormat(runCount + " batches had run for recommendation.");
            }

            LOG.InfoFormat("RunBatches successfully run.");
        }
       
    }
}
