//------------------------------------------------------------------------------
// <copyright file="RecycleLogConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Recycle SQL Server log
    /// </summary>
    [Serializable]
    public sealed class RecycleLogConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        #endregion

        #region constructors

        public RecycleLogConfiguration(int monitoredServerId)
            : base(monitoredServerId)
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
