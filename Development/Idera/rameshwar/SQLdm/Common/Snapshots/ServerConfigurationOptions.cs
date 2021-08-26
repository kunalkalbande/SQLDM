//------------------------------------------------------------------------------
// <copyright file="ServerConfigurationOptions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents SQL Server configuration options sample.
    /// </summary>
    [Serializable]
    public sealed class ServerConfigurationOptions
    {
        #region fields

        private int _affinityMask;
        private bool _lightweightPooling;
        private bool _priorityBoost;
        private int _processorsUsed;
        private bool _setWorkingSetSize;

        #endregion

        #region constructors

        internal ServerConfigurationOptions(
            int affinityMask, bool lightweightPooling, bool priorityBoost, bool setWorkingSetSize, int processorCount)
        {
            _affinityMask = affinityMask;
            _processorsUsed = affinityMask > 0 ? CountOnes(affinityMask, processorCount) : processorCount;
            _lightweightPooling = lightweightPooling;
            _priorityBoost = priorityBoost;
            _setWorkingSetSize = setWorkingSetSize;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets a bit mask representing which processors in a symmetric multi processor (SMP)
        /// environment SQL Server will use.
        /// </summary>
        public int AffinityMask
        {
            get { return _affinityMask; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server is using fiber mode scheduling for 
        /// thread context switching.
        /// </summary>
        public bool LightWeightPooling
        {
            get { return _lightweightPooling; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server is running at a higher scheduling 
        /// priority that other processes on the same computer.
        /// </summary>
        public bool PriorityBoost
        {
            get { return _priorityBoost; }
        }

        /// <summary>
        /// Gets the total number of processors used based on the affinity mask.
        /// </summary>
        public int ProcessorsUsed
        {
            get { return _processorsUsed; }
        }

        /// <summary>
        /// Gets a value representing whether SQL Server uses a reserved physical memory space.
        /// </summary>
        public bool SetWorkingSetSize
        {
            get { return _setWorkingSetSize; }
        }

        #endregion

        #region methods

        /// <summary>
        /// Counts the number of ones in a bitmask.
        /// </summary>
        /// <param name="n">An integer bitmask.</param>
        /// <returns>The number of ones in the bitmask.</returns>
        private static int CountOnes(int n, int processorCount)
        {
            int count = 0;

            while (n != 0)
            {
                count += n & 1;
                n = n >> 1;
            }

            if (processorCount > count)
            {
                return count;
            }
            else
            {
                return processorCount;
            }
        }

        ///// <summary>
        ///// Dumps configuration options for a SQL Server overview sample to a string.
        ///// </summary>
        ///// <returns>Configuration options data.</returns>
        //public string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();

        //    dump.Append("AffinityMask: " + AffinityMask); dump.Append("\n");
        //    dump.Append("LightWeightPooling: " + LightWeightPooling); dump.Append("\n");
        //    dump.Append("PriorityBoost: " + PriorityBoost); dump.Append("\n");
        //    dump.Append("SetWorkingSetSize: " + SetWorkingSetSize); dump.Append("\n");

        //    return dump.ToString();
        //}

        #endregion
    }
}
