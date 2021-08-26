//------------------------------------------------------------------------------
// <copyright file="MirroringPartnerActionConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Perform a mirror action on a mirror database
    /// </summary>
    [Serializable]
    public sealed class MirroringPartnerActionConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields
        string _Database;
        MirroringPartnerActions _action;
        #endregion

        #region constructors

        public MirroringPartnerActionConfiguration(int monitoredServerId, string Database, MirroringPartnerActions Action)
            : base(monitoredServerId)
        {
            this.Database = Database;
            this._action = Action;
        }

        #endregion

        #region properties

        public string Database
        {
            get { return _Database; }
            set { _Database = value; }
        }

        public MirroringPartnerActions Action
        {
            get { return _action; }
            set { _action = value; }
        }
        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types
        /// <summary>
        /// Mirror actions that are available to be performed against the mirrored database
        /// </summary>
        public enum MirroringPartnerActions
        {
            Failover,
            Suspend,
            Resume
        }

        #endregion

    }
}
