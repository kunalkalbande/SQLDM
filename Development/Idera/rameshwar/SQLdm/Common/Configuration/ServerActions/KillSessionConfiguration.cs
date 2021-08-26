//------------------------------------------------------------------------------
// <copyright file="KillSessionConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Kills a SQL Server process 
    /// </summary>
    [Serializable]
    public sealed class KillSessionConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private int killSpid;

        #endregion

        #region constructors

        public KillSessionConfiguration(int monitoredServerId, int killSpid) : base(monitoredServerId)
        {
            this.killSpid = killSpid;
        }

        #endregion

        #region properties

        /// <summary>
        /// The SPID of the session to be killed
        /// </summary>
        public int KillSpid
        {
            get { return killSpid; }
            set { killSpid = value; }
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
