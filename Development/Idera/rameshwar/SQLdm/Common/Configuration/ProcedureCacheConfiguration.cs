//------------------------------------------------------------------------------
// <copyright file="ProcedureCacheConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// Configuration object for procedure cache view
    /// </summary>
    [Serializable]
    public sealed class ProcedureCacheConfiguration : OnDemandConfiguration
    {
        #region fields

        private bool showProcedureCacheList = true;

        #endregion

        #region constructors

        public ProcedureCacheConfiguration(int monitoredServerId) : base(monitoredServerId)
        {
        }

        public ProcedureCacheConfiguration(int monitoredServerId, bool showProcedureCacheList) : base(monitoredServerId)
        {
            this.showProcedureCacheList = showProcedureCacheList;
        }

        #endregion

        #region properties

        /// <summary>
        /// Control retrieval and display of list of objects in procedure cache
        /// </summary>
        public bool ShowProcedureCacheList
        {
            get { return showProcedureCacheList; }
            set { showProcedureCacheList = value; }
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
