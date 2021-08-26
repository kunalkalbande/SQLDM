//------------------------------------------------------------------------------
// <copyright file="SnapshotState.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots.State
{
    using System;
    using System.Collections.Generic;
    using Idera.SQLdm.Common.Events;
    using Idera.SQLdm.Common.Thresholds;
    using Idera.SQLdm.Common;

    /// <summary>
    /// Abstract base class for snapshot states with helper method that determines a state
    /// for a given value / threshold.
    /// </summary>
    [Serializable]
    public abstract class SnapshotState
    {
        #region fields

        #endregion

        #region constructors

        #endregion

        #region properties

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="thresholds">The thresholds.</param>
        /// <returns></returns>
        public static MonitoredState GetState(IComparable value, MetricThresholdEntry thresholds)
        {
            if (thresholds == null || thresholds.IsEnabled == false)
                return MonitoredState.None;

            Threshold warning = null;
            Threshold critical = null;
            Threshold info = null;

            //SQLDM-30524
            if (MetricDefinition.GetMetric(thresholds.MetricID) == Metric.Custom)
            {
                info = thresholds.InfoThreshold;
                warning = thresholds.WarningThreshold;
                critical = thresholds.CriticalThreshold;
            }
            else
            {
				//START: SQLdm 10.0 (Tarun Sapra)- Making generic type of thresholds, and assigning value as per 'IsBaselineEnabled' flag
				if (thresholds.IsBaselineEnabled)
				{
					info = thresholds.BaselineInfoThreshold;
					warning = thresholds.BaselineWarningThreshold;
					critical = thresholds.BaselineCriticalThreshold;
				}
				else
				{
					info = thresholds.InfoThreshold;
					warning = thresholds.WarningThreshold;
					critical = thresholds.CriticalThreshold;
				}
			}
            //END: SQLdm 10.0 (Tarun Sapra)- Making generic type of thresholds, and assigning value as per 'IsBaselineEnabled' flag

            // PR 11512 - when the alert is enabled but both thresholds are disabled return OK instead of NONE
            if (critical.Enabled == false && warning.Enabled == false && info.Enabled == false)
                return MonitoredState.OK;

            // custom counters can be much larger than the thresholds.  Limit the value used to an INT64 compatible value
            if (MetricDefinition.GetMetric(thresholds.MetricID) == Metric.Custom)
            {
                if (value is decimal)
                {
                    if (Int64.MaxValue < (decimal)value)
                        value = Int64.MaxValue;
                    else
                        if (Int64.MinValue > (decimal)value)
                            value = Int64.MinValue;
                }
            }

            if (critical != null && critical.Enabled && critical.IsInViolation(value))
                return MonitoredState.Critical;
            if (warning != null && warning.Enabled && warning.IsInViolation(value))
                return MonitoredState.Warning;
            if (info != null && info.Enabled && info.IsInViolation(value))
                return MonitoredState.Informational;

            return MonitoredState.OK;
        }

        #endregion

        #region interface implementations

        #endregion

        #region nested types

        #endregion

    }
}