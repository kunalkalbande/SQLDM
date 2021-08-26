//------------------------------------------------------------------------------
// <copyright file="ServicesSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents data for the Services on-demand view
    /// </summary>
    [Serializable]
    public class ServicesSnapshot : Snapshot
    {
        #region fields

        private Dictionary<ServiceName, Service> serviceDetails = new Dictionary<ServiceName, Service>();
        private bool osMetricsUnavailable = false;
        private bool lightweightPoolingEnabled = false;

        #endregion

        #region constructors

        public ServicesSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
            serviceDetails.Add(ServiceName.Agent, new Service(ServiceName.Agent));
            serviceDetails.Add(ServiceName.DTC, new Service(ServiceName.DTC));
            serviceDetails.Add(ServiceName.FullTextSearch, new Service(ServiceName.FullTextSearch));
            serviceDetails.Add(ServiceName.SqlServer, new Service(ServiceName.SqlServer));
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services 
            serviceDetails.Add(ServiceName.Browser, new Service(ServiceName.Browser));
            //SQLDM-26685 - Filtered out the ActiveDirectoryHelper option from the list of services displaying
            //serviceDetails.Add(ServiceName.ActiveDirectoryHelper, new Service(ServiceName.ActiveDirectoryHelper));
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services
            
        }

        public ServicesSnapshot(string instanceName)
            : base(instanceName)
        {
            serviceDetails.Add(ServiceName.Agent, new Service(ServiceName.Agent));
            serviceDetails.Add(ServiceName.DTC, new Service(ServiceName.DTC));
            serviceDetails.Add(ServiceName.FullTextSearch, new Service(ServiceName.FullTextSearch));
            serviceDetails.Add(ServiceName.SqlServer, new Service(ServiceName.SqlServer));
            //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services 
            serviceDetails.Add(ServiceName.Browser, new Service(ServiceName.Browser));
            //SQLDM-26685 - Filtered out the ActiveDirectoryHelper option from the list of services displaying
            //serviceDetails.Add(ServiceName.ActiveDirectoryHelper, new Service(ServiceName.ActiveDirectoryHelper));
            //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services

        }
        #endregion

        #region properties


        public Dictionary<ServiceName, Service> ServiceDetails
        {
            get { return serviceDetails; }
            internal set { serviceDetails = value; }
        }

        /// <summary>
        /// If true, batch was unable to get data from OS Metrics
        /// </summary>
        public bool OsMetricsUnavailable
        {
            get { return osMetricsUnavailable; }
            internal set { osMetricsUnavailable = value; }
        }


        public bool LightweightPoolingEnabled
        {
            get { return lightweightPoolingEnabled; }
            internal set { lightweightPoolingEnabled = value; }
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
