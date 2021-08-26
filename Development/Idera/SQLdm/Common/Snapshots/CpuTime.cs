//------------------------------------------------------------------------------
// <copyright file="CpuTime.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents elapsed time on a CPU
    /// </summary>
    [Serializable]
    public sealed class CpuTime
    {
        #region constants

        /// <summary>
        /// Default value for @@timeticks from SQL Server.  Multiply by cpu count to obtain value in microseconds.
        /// </summary>
        public const int DefaultCpuTimeTicks = 31250;
        
        #endregion

        #region fields

        private Int64? _ticks = null;
        private int _timeTicks = DefaultCpuTimeTicks;

        #endregion

        #region constructors

        public CpuTime()
        {
        }

        /// <summary>
        /// Initialize CpuTime object with ticks and use default @@timeTicks
        /// </summary>
        /// <param name="ticks">CPU ticks</param>
        public CpuTime(Int64? ticks)
        {
            _ticks = ticks;
        }

        /// <summary>
        /// Initialize CpuTime object with ticks and @@timeTicks from SQL Server
        /// </summary>
        /// <param name="ticks">CPU ticks</param>
        /// <param name="timeTicks">@@timeticks value from SQL Server</param>
        public CpuTime(Int64? ticks, int? timeTicks)
        {
            _ticks = ticks;
            if (timeTicks.HasValue){_timeTicks = timeTicks.Value;}
        }

        #endregion

        #region properties

        /// <summary>
        /// Return CPU time as a timespan
        /// </summary>
        public TimeSpan CpuTimeSpan
        {
            get
            {
                if (_ticks.HasValue)
                {
                    return new TimeSpan(ConvertTicksToMicroseconds(_ticks.Value, _timeTicks)*100);
                }
                else
                {
                    return new TimeSpan(0);
                }
                
            }
        }

        /// <summary>
        /// Get or set CPU ticks
        /// </summary>
        public Int64? Ticks
        {
            get
            {
                return _ticks;
            }
            set
            {
                _ticks = value;
            }
        }

        /// <summary>
        /// Get or set CPU timeticks (ratio of ticks to microseconds)
        /// </summary>
        public int TimeTicks
        {
            get
            {
                return _timeTicks;
            }
            set
            {
                _timeTicks = value;
            }
        }

        #endregion

        #region events

        #endregion

        #region methods

        /// <summary>
        /// Convert CPU ticks to microseconds using the default CPU timeticks value
        /// </summary>
        /// <param name="ticks">CPU ticks</param>
        /// <returns>Value in microseconds</returns>
        public static Int64 ConvertTicksToMicroseconds(Int64 ticks)
        {
            return ConvertTicksToMicroseconds(ticks, DefaultCpuTimeTicks);
        }

        /// <summary>
        /// Convert CPU ticks to microseconds
        /// </summary>
        /// <param name="ticks">CPU ticks</param>
        /// <param name="timeTicks">@@timeticks value from SQL Server</param>
        /// <returns>Value in microseconds</returns>
        public static Int64 ConvertTicksToMicroseconds(Int64 ticks, Int64 timeTicks)
        {
            return ticks * timeTicks;
        }


        public static double? ComputeCpuPercentage(Int64 firstValue, Int64 secondValue, int? timeTicks, int? numberOfProcessors, DateTime firstTimeStamp, DateTime secondTimeStamp)
        {
            if (secondValue > firstValue)
            {
                return ComputeCpuPercentage((secondValue - firstValue), timeTicks.HasValue?timeTicks.Value:DefaultCpuTimeTicks, numberOfProcessors.HasValue?(numberOfProcessors > 0)?numberOfProcessors:1:1, secondTimeStamp - firstTimeStamp);
            }
            else
            {
                return null;
            }
        }

        public static double? ComputeCpuPercentage(Int64 cpuDelta, int timeTicks, int? numberOfProcessors, TimeSpan intervalLength)
        {
            if (numberOfProcessors > 0 && intervalLength != null && intervalLength.TotalMilliseconds > 0)
            {
                return 100 * (((cpuDelta * (timeTicks / 1000f))) / numberOfProcessors) / intervalLength.TotalMilliseconds;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region interface implementations

        #endregion


    }
}
