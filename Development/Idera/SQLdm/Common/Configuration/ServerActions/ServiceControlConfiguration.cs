//------------------------------------------------------------------------------
// <copyright file="ServiceControlConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration.ServerActions
{
    using System;

    /// <summary>
    /// Control the running state of a service
    /// </summary>
    [Serializable]
    public class ServiceControlConfiguration : OnDemandConfiguration, IServerActionConfiguration
    {
        #region fields

        private ServiceName serviceToAffect;
        private ServiceControlAction action;

        #endregion

        #region constructors

        public ServiceControlConfiguration(int monitoredServerId, ServiceName serviceToAffect, ServiceControlAction action) : base(monitoredServerId)
        {
            this.serviceToAffect = serviceToAffect;
            this.action = action;
        }

        #endregion

        #region properties

        public ServiceName ServiceToAffect
        {
            get { return serviceToAffect; }
            set { serviceToAffect = value; }
        }

        public ServiceControlAction Action
        {
            get { return action; }
            set { action = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        public enum ServiceControlAction
        {
            Start,
            Stop,
            Pause,
            Continue
        }

        #endregion

    }
}
