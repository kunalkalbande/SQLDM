//------------------------------------------------------------------------------
// <copyright file="ReindexConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Reindex server action configuration
    /// </summary>
    [Serializable]
    public sealed class ReindexConfiguration : TableActionBaseConfiguration
    {
        #region fields

        #endregion

        #region constructors

        public ReindexConfiguration(int monitoredServerId, string databaseName, int? tableId) : base(monitoredServerId, databaseName, tableId)
        {
        }

        public ReindexConfiguration(int monitoredServerId, string databaseName, int? tableId, string indexName) : base(monitoredServerId, databaseName, tableId, indexName)
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
