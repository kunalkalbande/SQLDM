//------------------------------------------------------------------------------
// <copyright file="BufferPoolExtIOSnapshot.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Data;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents the BufferPoolExtIO info on a monitored server //SQLdm 10.0 (Srishti Purohit) (SDR-M33) -- new Snapshot class
    /// </summary>
    [Serializable]
    public sealed class BufferPoolExtIOSnapshot : Snapshot
    {
        #region fields

        private int state;
        private int interval;
        private List<long> currentValue = new List<long>();
        
        #endregion

        #region constructors

        internal BufferPoolExtIOSnapshot(SqlConnectionInfo info)
            : base(info.InstanceName)
        {
        }

        #endregion

        #region properties

        public int State
        {
            get { return state; }
            internal set { state = value; }
        }
        
        public int Interval
        {
            get { return interval; }
            set { interval = value; }
        }
        public List<long> CurrentValue
        {
            get { return currentValue; }
            set { currentValue = value; }
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
