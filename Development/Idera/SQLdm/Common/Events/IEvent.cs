//------------------------------------------------------------------------------
// <copyright file="IEvent.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Events
{
    using System;
    using Objects;

    /// <summary>
    /// Enter a description for this interface
    /// </summary>
    public interface IEvent
    {
        #region properties

        /// <summary>
        /// Gets the event code.
        /// </summary>
        /// <value>The event code.</value>
        int MetricID { get; }

        /// <summary>
        /// Gets the occurance time.
        /// </summary>
        /// <value>The occurance time.</value>
        DateTime OccuranceTime { get; }

        /// <summary>
        /// Gets the state of the monitored.
        /// </summary>
        /// <value>The state of the monitored.</value>
        MonitoredState MonitoredState { get; }

        MonitoredObjectName MonitoredObject { get; }

        object Value { get; }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to metric
        /// </summary>
        object MetricValue { get; }
        /// <summary>
        /// SQLdm10.1[Srishti Purohit] -- To updated alert msg according to baseline thrshold check for metric
        /// </summary>
        bool IsBaselineEnabled { get; }

        object AdditionalData { get; set; }

        bool StateChanged { get; set; }

        MonitoredState PreviousState { get; set; }

        #endregion

        #region events

        #endregion

        #region methods

        #endregion
    }
}
