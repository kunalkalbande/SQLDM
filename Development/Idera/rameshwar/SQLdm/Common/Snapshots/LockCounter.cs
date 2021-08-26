//------------------------------------------------------------------------------
// <copyright file="LockCounter.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a set of counters for a lock object
    /// </summary>
    [Serializable]
    public sealed class LockCounter : ISerializable
    {
        #region fields

        private Int64? requests;
        private Int64? timeouts;
        private Int64? deadlocks;
        private Int64? waits;
        private TimeSpan? waitTime;

        private Int64? requestsTotal;
        private Int64? timeoutsTotal;
        private Int64? deadlocksTotal;
        private Int64? waitsTotal;
        private TimeSpan? waitTimeTotal;

        private Int64? outstandingGranted;
        private Int64? outstandingWaiting;
        private Int64? outstandingConverted;


        #endregion

        #region constructors
        
        internal LockCounter()
        {
            requests = null;
            timeouts = null;
            deadlocks = null;
            waits = null;
            waitTime = null;
            requestsTotal = null;
            timeoutsTotal = null;
            deadlocksTotal = null;
            waitsTotal = null;
            waitTimeTotal = null;
            outstandingGranted = 0;
            outstandingWaiting = 0;
            outstandingConverted = 0;
        }

        internal LockCounter(
            Int64? deadlocksTotal,
            Int64? requestsTotal,
            Int64? timeoutsTotal,
            Int64? waitsTotal,
            TimeSpan? waitTimeTotal
            ) : this()
        {
            DeadlocksTotal = deadlocksTotal;
            RequestsTotal = requestsTotal;
            TimeoutsTotal = timeoutsTotal;
            WaitsTotal = waitsTotal;
            WaitTimeTotal = waitTimeTotal;
        }

        public LockCounter(SerializationInfo info, StreamingContext context)
        {
            requests = (Int64?)info.GetValue("requests", typeof(Int64?));
            timeouts = (Int64?)info.GetValue("timeouts", typeof(Int64?));
            deadlocks = (Int64?)info.GetValue("deadlocks", typeof(Int64?));
            waits = (Int64?)info.GetValue("waits", typeof(Int64?));
            waitTime = (TimeSpan?)info.GetValue("waitTime", typeof(TimeSpan?));
            requestsTotal = (Int64?)info.GetValue("requestsTotal", typeof(Int64?));
            timeoutsTotal = (Int64?)info.GetValue("timeoutsTotal", typeof(Int64?));
            deadlocksTotal = (Int64?)info.GetValue("deadlocksTotal", typeof(Int64?));
            waitsTotal = (Int64?)info.GetValue("waitsTotal", typeof(Int64?));
            waitTimeTotal = (TimeSpan?)info.GetValue("waitTimeTotal", typeof(TimeSpan?));
            outstandingGranted = (Int64?)info.GetValue("outstandingGranted", typeof(Int64?));
            outstandingWaiting = (Int64?)info.GetValue("outstandingWaiting", typeof(Int64?));
            outstandingConverted = (Int64?)info.GetValue("outstandingConverted", typeof(Int64?));
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets deadlocks for the counter object
        /// </summary>
        public Int64? Deadlocks
        {
            get { return deadlocks; }
            internal set { deadlocks = value; }
        }

        /// <summary>
        /// Gets total deadlocks for the counter object
        /// </summary>
        public Int64? DeadlocksTotal
        {
            get { return deadlocksTotal; }
            internal set
            {
                Deadlocks = CalculateCounterDelta(DeadlocksTotal, value);
                deadlocksTotal = value;
            }
        }

        /// <summary>
        /// Gets requests for the counter object
        /// </summary>
        public Int64? Requests
        {
            get { return requests; }
            internal set { requests = value; }
        }

        /// <summary>
        /// Gets total requests for the counter object
        /// </summary>
        public Int64? RequestsTotal
        {
            get { return requestsTotal; }
            internal set
            {
                Requests = CalculateCounterDelta(RequestsTotal, value);
                requestsTotal = value;
            }
        }

        /// <summary>
        /// Gets timeouts for the counter object
        /// </summary>
        public Int64? Timeouts
        {
            get { return timeouts; }
            internal set { timeouts = value; }
        }

        /// <summary>
        /// Gets timeouts for the counter object
        /// </summary>
        public Int64? TimeoutsTotal
        {
            get { return timeoutsTotal; }
            internal set
            {
                Timeouts = CalculateCounterDelta(TimeoutsTotal, value);
                timeoutsTotal = value;
            }
        }

        /// <summary>
        /// Gets waits for the counter object
        /// </summary>
        public Int64? Waits
        {
            get { return waits; }
            internal set { waits = value; }
        }

        /// <summary>
        /// Gets waits for the counter object
        /// </summary>
        public Int64? WaitsTotal
        {
            get { return waitsTotal; }
            internal set
            {
                Waits = CalculateCounterDelta(WaitsTotal, value);
                waitsTotal = value;
            }
        }

        /// <summary>
        /// Gets waitTime for the counter object
        /// </summary>
        public TimeSpan? WaitTime
        {
            get { return waitTime; }
            internal set
            {
                waitTime = value;
            }
        }

        /// <summary>
        /// Gets waitTime for the counter object
        /// </summary>
        public TimeSpan? WaitTimeTotal
        {
            get { return waitTimeTotal; }
            internal set
            {
                if (WaitTimeTotal.HasValue) 
                {
                    WaitTime = CalculateCounterDelta(WaitTimeTotal,value);
                }
                waitTimeTotal = value;
            }
        }


        /// <summary>
        /// Gets Ratio of locks which have to wait.
        /// </summary>
        public Double? WaitRatio
        {
            get
            {
                if (!requests.HasValue || !waits.HasValue)
                {
                    return null;
                }

                if (requests > 0 && waits > 0)
                {
                    return ((Double?)waits / (Double?)requests);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets Ratio of locks which have to wait.
        /// </summary>
        public Double? WaitRatioSinceStartup
        {
            get
            {
                if (!requestsTotal.HasValue || !waitsTotal.HasValue)
                {
                    return null;
                }

                if (requestsTotal > 0 && waitsTotal > 0)
                {
                    return ((Double?)waitsTotal / (Double?)requestsTotal);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets Ratio of locks which timeout
        /// </summary>
        public Double? TimeoutRatio
        {
            get
            {
                if (!requests.HasValue || !timeouts.HasValue)
                {
                    return null;
                }

                if (requests > 0 && timeouts > 0)
                {
                    return ((Double?)timeouts / (Double?)requests);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets Ratio of locks which timeout
        /// </summary>
        public Double? TimeoutRatioSinceStartup
        {
            get
            {
                if (!requestsTotal.HasValue || !timeoutsTotal.HasValue)
                {
                    return null;
                }

                if (requestsTotal > 0 && timeoutsTotal > 0)
                {
                    return ((Double?)timeoutsTotal / (Double?)requestsTotal);
                }
                else
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Gets Ratio of locks which deadlock
        /// </summary>
        public Double? DeadlockRatio
        {
            get
            {
                if(!requests.HasValue || !deadlocks.HasValue)
                {
                    return null;
                }

                if (requests > 0 && deadlocks > 0)
                {
                    return ((Double?)deadlocks / (Double?)requests);
                }
                else
                {
                    return 0;
                }
            }
        }
        /// <summary>
        /// Gets Ratio of locks which deadlock
        /// </summary>
        public Double? DeadlockRatioSinceStartup
        {
            get
            {
                if (!requestsTotal.HasValue || !deadlocksTotal.HasValue)
                {
                    return null;
                }

                if (requestsTotal > 0 && deadlocksTotal > 0)
                {
                    return ((Double?)deadlocksTotal / (Double?)requestsTotal);
                }
                else
                {
                    return 0;
                }
            }
        }


        /// <summary>
        /// Gets average lock wait time
        /// </summary>
        public TimeSpan? AverageWaitTime
        {
            get
            {
                if (!waitTime.HasValue || !waits.HasValue || waitTime.Value == null)
                {
                    return null;
                }

                if (waitTime.Value.TotalMilliseconds > 0 && waits.Value > 0)
                {
                    return TimeSpan.FromMilliseconds(((Double)waitTime.Value.TotalMilliseconds / (Double)waits));
                }
                else
                {
                    return TimeSpan.FromMilliseconds(0);
                }
            }
        }

        /// <summary>
        /// Gets average lock wait time
        /// </summary>
        public TimeSpan? AverageWaitTimeSinceStartup
        {
            get
            {
                if (!waitTimeTotal.HasValue || !waitsTotal.HasValue || waitTimeTotal.Value == null)
                {
                    return null;
                }


                if (waitTimeTotal.Value.TotalMilliseconds > 0 && waitsTotal > 0)
                {
                    return TimeSpan.FromMilliseconds((waitTimeTotal.Value.TotalMilliseconds / (Double)waitsTotal));
                }
                else
                {
                    return TimeSpan.FromMilliseconds(0);
                }
            }
        }


        public long? OutstandingGranted
        {
            get { return outstandingGranted; }
            internal set { outstandingGranted = value; }
        }

        public long? OutstandingWaiting
        {
            get { return outstandingWaiting; }
            internal set { outstandingWaiting = value; }
        }

        public long? OutstandingConverted
        {
            get { return outstandingConverted; }
            internal set { outstandingConverted = value; }
        }

        #endregion

        #region events

        #endregion

        #region methods


        private static Int64? CalculateCounterDelta(Int64? previousCounter, Int64? currentCounter)
        {
            if (previousCounter.HasValue && currentCounter.HasValue)
            {
                Int64? counterDelta = currentCounter.Value - previousCounter.Value;
                if (counterDelta < 0)
                    return null;
                else
                    return counterDelta;
            }
            else
            {
                return null;
            }
        }

        private static TimeSpan? CalculateCounterDelta(Nullable<TimeSpan> previousCounter, Nullable<TimeSpan> currentCounter)
        {
            if (previousCounter.HasValue && currentCounter.HasValue && previousCounter.Value != null && currentCounter.Value != null)
            {
                TimeSpan? counterDelta = TimeSpan.FromMilliseconds(currentCounter.Value.TotalMilliseconds - previousCounter.Value.TotalMilliseconds);
                if (counterDelta.Value.TotalMilliseconds < 0)
                    return null;
                else
                    return counterDelta;
            }
            else
            {
                return null;
            }
        }


        #endregion

        #region interface implementations
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("requests", requests);
            info.AddValue("timeouts", timeouts);
            info.AddValue("deadlocks", deadlocks);
            info.AddValue("waits", waits);
            info.AddValue("waitTime", waitTime);
            info.AddValue("requestsTotal", requestsTotal);
            info.AddValue("timeoutsTotal", timeoutsTotal);
            info.AddValue("deadlocksTotal", deadlocksTotal);
            info.AddValue("waitsTotal", waitsTotal);
            info.AddValue("waitTimeTotal", waitTimeTotal);
            info.AddValue("outstandingGranted", outstandingGranted);
            info.AddValue("outstandingWaiting", outstandingWaiting);
            info.AddValue("outstandingConverted", outstandingConverted);
        }

        #endregion

        #region nested types

        #endregion

    }
}
