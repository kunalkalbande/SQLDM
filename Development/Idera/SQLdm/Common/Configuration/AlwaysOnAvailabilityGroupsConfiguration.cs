//------------------------------------------------------------------------------
// <copyright file="AlwaysOnAvailabilityGroupsConfiguration.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;
using Idera.SQLdm.Common.Snapshots;

namespace Idera.SQLdm.Common.Configuration
{
    /// <summary>
    /// The configuration for the Availability Groups.
    /// </summary>
    [Serializable]
    public class AlwaysOnAvailabilityGroupsConfiguration : OnDemandConfiguration
    {
        /// <summary>
        /// Constructor AlwaysOnAvailabilityGroupsConfiguration class. Initialize the class according
        /// the monitored server ID an the previous value of the snapshot.
        /// </summary>
        /// <param name="monitoredServerId">The monitored server id, is required by the base class.</param>
        /// <param name="previousValue">Previous value of the snapshot.</param>
        public AlwaysOnAvailabilityGroupsConfiguration(int monitoredServerId, AlwaysOnAvailabilityGroupsSnapshot previousValue)
            : base(monitoredServerId)
        {
            this.PreviousValue = previousValue;
        }

        /// <summary>
        /// Previous value of the Snapshot.
        /// </summary>
        public AlwaysOnAvailabilityGroupsSnapshot PreviousValue { get; set; }
    }
}
