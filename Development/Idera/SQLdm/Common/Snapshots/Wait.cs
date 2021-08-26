//------------------------------------------------------------------------------
// <copyright file="Wait.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents a single SQL Server wait
    /// </summary>
    [Serializable]
    public class Wait
    {
        #region fields

        private string waitType = null;
        private Int64? waitingTasksCountTotal = null;
        private TimeSpan? waitTimeTotal = new TimeSpan();
        private TimeSpan? maxWaitTime = new TimeSpan();
        private TimeSpan? resourceWaitTimeTotal = new TimeSpan();

        private Int64? waitingTasksCountDelta = null;
        private TimeSpan? waitTimeDelta = new TimeSpan();
        private TimeSpan? resourceWaitTimeDelta = new TimeSpan();

        private TimeSpan? timeDelta = new TimeSpan();

        #endregion

        #region constructors

        #endregion

        #region properties

        public string WaitType
        {
            get { return waitType; }
            internal set { waitType = value; }
        }

        public long? WaitingTasksCountTotal
        {
            get { return waitingTasksCountTotal; }
            internal set { waitingTasksCountTotal = value; }
        }

        public TimeSpan? WaitTimeTotal
        {
            get { return waitTimeTotal; }
            internal set { waitTimeTotal = value; }
        }

        public TimeSpan? MaxWaitTime
        {
            get { return maxWaitTime; }
            internal set { maxWaitTime = value; }
        }

        public TimeSpan? SignalWaitTimeTotal
        {
            get
            {
                if (waitTimeTotal.HasValue && resourceWaitTimeTotal.HasValue)
                {
                    return waitTimeTotal.Value.Subtract(resourceWaitTimeTotal.Value);
                }
                else
                {
                    return new TimeSpan();
                }
            }
        }

        public TimeSpan? ResourceWaitTimeTotal
        {
            get { return resourceWaitTimeTotal; }
            internal set { resourceWaitTimeTotal = value; }
        }

        public long? WaitingTasksCountDelta
        {
            get { return waitingTasksCountDelta; }
            internal set { waitingTasksCountDelta = value; }
        }

        public TimeSpan? WaitTimeDelta
        {
            get { return waitTimeDelta; }
            internal set { waitTimeDelta = value; }
        }

        public TimeSpan? SignalWaitTimeDelta
        {
            get
            {
                if (waitTimeDelta.HasValue && resourceWaitTimeDelta.HasValue)
                {
                    return waitTimeDelta.Value.Subtract(resourceWaitTimeDelta.Value);
                }
                else
                {
                    return new TimeSpan();
                }
            }
        }

        public TimeSpan? ResourceWaitTimeDelta
        {
            get { return resourceWaitTimeDelta; }
            internal set { resourceWaitTimeDelta = value; }
        }

        public TimeSpan? TimeDelta
        {
            get { return timeDelta; }
            internal set { timeDelta = value; }
        }

        

        #region Calculations

        public double? WaitingTasksPerSecond
        {
            get
            {
                if (waitingTasksCountDelta.HasValue && timeDelta.HasValue && timeDelta.Value.TotalMilliseconds > 0)
                {
                    return (waitingTasksCountDelta.Value / timeDelta.Value.TotalSeconds);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? TotalWaitMillisecondsPerSecond
        {
            get
            {
                if (WaitTimeDelta.HasValue && timeDelta.HasValue && timeDelta.Value.TotalMilliseconds > 0)
                {
                    return (WaitTimeDelta.Value.TotalMilliseconds / timeDelta.Value.TotalSeconds);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? SignalWaitMillisecondsPerSecond
        {
            get
            {
                if (SignalWaitTimeDelta.HasValue && timeDelta.HasValue && timeDelta.Value.TotalMilliseconds > 0)
                {
                    return (SignalWaitTimeDelta.Value.TotalMilliseconds / timeDelta.Value.TotalSeconds);
                }
                else
                {
                    return null;
                }
            }
        }

        public double? ResourceWaitMillisecondsPerSecond
        {
            get
            {
                if (ResourceWaitTimeDelta.HasValue && timeDelta.HasValue && timeDelta.Value.TotalMilliseconds > 0)
                {
                    return (ResourceWaitTimeDelta.Value.TotalMilliseconds / timeDelta.Value.TotalSeconds);
                }
                else
                {
                    return null;
                }
            }
        }


        #endregion

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
