//------------------------------------------------------------------------------
// <copyright file="OnDemandConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Attributes;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Base class for configuration objects
    /// </summary>
    [Serializable]
    public class OnDemandConfiguration
    {
        #region fields

        private int monitoredServerId;
        protected Guid clientSessionId = Guid.NewGuid();

        #endregion

        #region constructors

        public OnDemandConfiguration(int monitoredServerId)
        {
            this.monitoredServerId = monitoredServerId;
        }

        #endregion

        #region properties

        [Browsable(false)]
        public int MonitoredServerId
        {
            get { return monitoredServerId; }
            set { monitoredServerId = value; }
        }

        [Browsable(false)]
        public bool ReadyForCollection
        {
            get { return true; }
        }

        /// <summary>
        /// Identifier for this client session.
        /// </summary>
        [Browsable(false)]
        [AuditableAttribute(false)]
        public Guid ClientSessionId
        {
            get { return clientSessionId; }
            set { clientSessionId = value; }
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
