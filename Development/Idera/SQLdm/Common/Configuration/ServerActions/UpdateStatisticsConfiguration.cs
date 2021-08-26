//------------------------------------------------------------------------------
// <copyright file="UpdateStatisticsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Update statistics for a table or index
    /// </summary>
    [Serializable]
    public sealed class UpdateStatisticsConfiguration : TableActionBaseConfiguration
    {
        #region fields

        #endregion

        #region constructors

        public UpdateStatisticsConfiguration(int monitoredServerId, string databaseName, int? tableId) : base(monitoredServerId, databaseName, tableId)
        {
        }

        public UpdateStatisticsConfiguration(int monitoredServerId, string databaseName, int? tableId, string indexName) : base(monitoredServerId, databaseName, tableId, indexName)
        {
        }

        #endregion

        #region properties

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
