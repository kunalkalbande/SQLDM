//------------------------------------------------------------------------------
// <copyright file="DatabaseConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents the configuration settings on all server databases
    /// </summary>
    [Serializable]
    public sealed class DatabaseConfigurationSnapshot: Snapshot
    {
        #region fields

        private DataTable configurationSettings = new DataTable("ConfigurationSettings");

        #endregion

        #region constructors

            internal DatabaseConfigurationSnapshot(SqlConnectionInfo info)
        : base(info.InstanceName)
        {
            ConfigurationSettings.RemotingFormat = SerializationFormat.Binary;
            ConfigurationSettings.Columns.Add("DatabaseName", typeof(string));
            ConfigurationSettings.Columns.Add("Collation", typeof(string));
            ConfigurationSettings.Columns.Add("IsAnsiNullDefault", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAnsiNullsEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAnsiPaddingEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAnsiWarningsEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsArithmeticAbortEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAutoClose", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAutoCreateStatistics", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAutoShrink", typeof(bool));
            ConfigurationSettings.Columns.Add("IsAutoUpdateStatistics", typeof(bool));
            ConfigurationSettings.Columns.Add("IsCloseCursorsOnCommitEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsFulltextEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsInStandBy", typeof(bool));
            ConfigurationSettings.Columns.Add("IsLocalCursorsDefault", typeof(bool));
            ConfigurationSettings.Columns.Add("IsMergePublished", typeof(bool));
            ConfigurationSettings.Columns.Add("IsNullConcat", typeof(bool));
            ConfigurationSettings.Columns.Add("IsNumericRoundAbortEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsParameterizationForced", typeof(bool));
            ConfigurationSettings.Columns.Add("IsQuotedIdentifiersEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsPublished", typeof(bool));
            ConfigurationSettings.Columns.Add("IsRecursiveTriggersEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsSubscribed", typeof(bool));
            ConfigurationSettings.Columns.Add("IsSyncWithBackup", typeof(bool));
            ConfigurationSettings.Columns.Add("IsTornPageDetectionEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("Recovery", typeof(string));
            ConfigurationSettings.Columns.Add("Status", typeof(string));
            ConfigurationSettings.Columns.Add("Updateability", typeof(string));
            ConfigurationSettings.Columns.Add("UserAccess", typeof(string));
            ConfigurationSettings.Columns.Add("Version", typeof(int));
            ConfigurationSettings.Columns.Add("Compatibility", typeof(float));

            //SQL 2005 Only
            ConfigurationSettings.Columns.Add("IsDbChainingOn", typeof(bool));
            ConfigurationSettings.Columns.Add("IsDateCorrelationOn", typeof(bool));
            ConfigurationSettings.Columns.Add("IsVardecimalEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("PageVerifyOption", typeof(string));
            ConfigurationSettings.Columns.Add("IsAutoUpdateStatsAsyncOn", typeof(bool));
            ConfigurationSettings.Columns.Add("IsBrokerEnabled", typeof(bool));
            ConfigurationSettings.Columns.Add("IsTrustworthy", typeof(bool));
            ConfigurationSettings.Columns.Add("SnapshotIsolationState", typeof(string));
            ConfigurationSettings.Columns.Add("IsReadOnly", typeof(bool)); // SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new column
            
            //SQL 2012 Only
            ConfigurationSettings.Columns.Add("Containment", typeof(int)); // SQLdm 10.0 (Praveen Suhalka) (Embedded SQLDoctor Analytics) -- new column
        }

                    

        #endregion

        #region properties

        public DataTable ConfigurationSettings
        {
            get { return configurationSettings; }
            internal set { configurationSettings = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}
