//------------------------------------------------------------------------------
// <copyright file="FreeProcedureCacheConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Frees the procedure cache
    /// </summary>
    [Serializable]
    public sealed class FreeProcedureCacheConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        #endregion

        #region constructors


        public FreeProcedureCacheConfiguration(int monitoredServerId)
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
