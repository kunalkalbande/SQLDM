//------------------------------------------------------------------------------
// <copyright file="DiskSizeDetailsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System.ComponentModel;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    using System;
using System.Collections.Generic;

    /// <summary>
    /// Configuration class for DiskSize details on-demand probe //--SQLdm 9.1 (Ankit Srivastava) - Filegroup and Mount Point Monitoring Improvements -Added new class for disk size config
    /// </summary>
    [Serializable]
    public sealed class DiskSizeDetailsConfiguration : SessionSummaryConfiguration
    {
        #region constants
        
        #endregion

        #region fields

        DiskSizeDetails diskSizeDetails=null;
        

        DatabaseProbeConfiguration config = null;

        WmiConfiguration wmiConfig=null;


        #endregion

        #region constructors

        /// <summary>
        /// Initialize a new instance of the DiskSizeDetailsConfiguration class.
        /// </summary>
        /// <param name="monitoredServerId"></param>
        /// <param name="diskSizeDetails"></param>
        /// <param name="config"></param>
        /// <param name="wmiConfig"></param>
        public DiskSizeDetailsConfiguration(int monitoredServerId, DiskSizeDetails diskSizeDetails)
            : base(monitoredServerId)
        {
            this.diskSizeDetails = diskSizeDetails;
            this.config = new DatabaseProbeConfiguration(monitoredServerId);
            this.wmiConfig = new WmiConfiguration();
        }


        #endregion

        #region properties

        public DiskSizeDetails DiskSizeDetails
        {
            get { return diskSizeDetails; }
            set { diskSizeDetails = value; }
        }

        public DatabaseProbeConfiguration DatabaseSizeConfig
        {
            get { return config; }
            set { config = value; }
        }

        public WmiConfiguration WmiConfig
        {
            get { return wmiConfig; }
            set { wmiConfig = value; }
        }
      


        #endregion

        #region events

        #endregion

        #region methods

        public void SetPreviousSnapshot(DiskSizeDetails diskSizeDetails)
        {
            if (diskSizeDetails != null)
            {
                ServerStartTime = diskSizeDetails.ServerStartupTime;
            }
        }

        #endregion

        #region nested types

        #endregion

    }
}
