//------------------------------------------------------------------------------
// <copyright file="Service.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a monitored OS Service
    /// </summary>
    [Serializable]
    public class Service
    {
        #region fields

        private ServiceState runningState = ServiceState.UnableToMonitor;
        private ServiceName serviceType;
        private string serviceName = null;
        private long? processID = null;
        private DateTime? runningSince = null;
        private string startupType = null;
        private string logOnAs = null;
        private string description = null;

        #endregion

        #region constructors

        public Service(ServiceName serviceType)
        {
            this.serviceType = serviceType;
        }

        #endregion

        #region properties

        public string Description
        {
            get { return description; }
            internal set { description = value; }
        }

        public ServiceState RunningState
        {
            get { return runningState; }
            internal set { runningState = value; }
        }

        public ServiceName ServiceType
        {
            get { return serviceType; }
            internal set { serviceType = value; }
        }

        public string ServiceName
        {
            get { return serviceName; }
            internal set { serviceName = value; }
        }

        public long? ProcessID
        {
            get { return processID; }
            internal set { processID = value; }
        }

        public DateTime? RunningSince
        {
            get { return runningSince; }
            internal set { runningSince = value; }
        }

        public string StartupType
        {
            get { return startupType; }
            internal set { startupType = value; }
        }

        public string LogOnAs
        {
            get { return logOnAs; }
            internal set { logOnAs = value; }
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
