//------------------------------------------------------------------------------
// <copyright file="DistributionQueueConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Configuration
{
    using System;

    /// <summary>
    /// SQLdm 10.4 Nikhil Bansal
    /// Configuration object for Estimated Query Plan On Demand
    /// </summary>
    [Serializable]
    public class EstimatedQueryPlanConfiguration : OnDemandConfiguration
    {
        #region fields

        private string queryText;
        private string databaseName;

        #endregion

        #region constructors
        
        public EstimatedQueryPlanConfiguration(int monitoredServerId, string queryText, string databaseName) 
            : base(monitoredServerId)
        {
            this.queryText = queryText;
            this.databaseName = databaseName;
        }

        #endregion

        #region properties

        public string QueryText
        {
            get { return queryText; }
            internal set { queryText = value; }
        }

        public string DatabaseName
        {
            get { return databaseName; }
            internal set { databaseName = value; }
        }

        #endregion
    }
}
