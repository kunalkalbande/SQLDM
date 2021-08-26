//------------------------------------------------------------------------------
// <copyright file="DatabaseSizeConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------


namespace Idera.SQLdm.Common.Configuration
{
    using System;
    using Idera.SQLdm.Common.Snapshots;

    /// <summary>
    /// File Activity Configuration
    /// </summary>
    [Serializable]
    public class DatabaseSizeConfiguration : OnDemandConfiguration
    {
        #region fields

        private DatabaseSizeSnapshot previousValues;

        #endregion

        #region constructors

        public DatabaseSizeConfiguration(int monitoredServerId, DatabaseSizeSnapshot previousValues)
            : base(monitoredServerId)
        {
            this.previousValues = previousValues;
        }

        #endregion

        #region properties

        public DatabaseSizeSnapshot PreviousValues
        {
            get { return previousValues; }
            set { previousValues = value; }
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
