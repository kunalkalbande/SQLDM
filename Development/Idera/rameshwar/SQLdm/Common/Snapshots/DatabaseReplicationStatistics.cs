//------------------------------------------------------------------------------
// <copyright file="DatabaseReplicationStatistics.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents replication statistics for a published database.
    /// </summary>
    [Serializable]
    public sealed class DatabaseReplicationStatistics
    {
        #region fields

        private Byte[] beginLsn;
        private Byte[] endLsn;
        private float? latency = null;
        private float? rate = null;
        private long? transactionCount = null;

        #endregion

        #region constructors

        internal DatabaseReplicationStatistics(Byte[] beginLsn, Byte[] endLsn, float? latency, float? rate, long? transactionCount)
        {
            this.beginLsn = beginLsn;
            this.endLsn = endLsn;
            this.latency = latency;
            this.rate = rate;
            this.transactionCount = transactionCount;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets the log sequence number of the current truncation polong? in the log.
        /// </summary>
        public Byte[] BeginLsn
        {
            get { return beginLsn; }
        }

        /// <summary>
        /// Gets the log sequence number of the next commit record awaiting delivery to the distribution database.
        /// </summary>
        public Byte[] EndLsn
        {
            get { return endLsn; }
        }

        /// <summary>
        /// Gets the average time, in seconds, that transactions were in the log before being distributed.
        /// </summary>
        public float? Latency
        {
            get { return latency; }
        }

        /// <summary>
        /// Gets the average number of transactions per second delivered to the distribution database.
        /// </summary>
        public float? Rate
        {
            get { return rate; }
        }

        /// <summary>
        /// Gets the number of transactions in the log awaiting delivery to the distribution database.
        /// </summary>
        public long? TransactionCount
        {
            get { return transactionCount; }
        }

        #endregion

        #region methods

        #endregion
    }
}
