//------------------------------------------------------------------------------
// <copyright file="ShutdownSQLServerConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Shut down SQL Server
    /// </summary>
    [Serializable]
    public sealed class ShutdownSQLServerConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private bool withNoWait = false;

        #endregion

        #region constructors

        public ShutdownSQLServerConfiguration(int monitoredServerId, bool withNoWait) : base(monitoredServerId)
        {
            this.withNoWait = withNoWait;
        }

        #endregion

        #region properties

        /// <summary>
        /// When WithNoWait is true, the SQL Server will roll back open transactions
        /// Otherwise the server will wait until open transactions complete
        /// </summary>
        public bool WithNoWait
        {
            get { return withNoWait; }
            set { withNoWait = value; }
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
