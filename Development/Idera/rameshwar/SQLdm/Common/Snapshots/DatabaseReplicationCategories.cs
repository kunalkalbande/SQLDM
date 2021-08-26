//------------------------------------------------------------------------------
// <copyright file="DatabaseReplicationCategories.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{
    /// <summary>
    /// Represents replication categories enabled for the sampled database.
    /// </summary>
    [Serializable]
    public sealed class DatabaseReplicationCategories
    {
        #region constants

        private const int PublishedBit = 1;
        private const int SubscribedBit = 2;
        private const int MergePublishedBit = 4;
        private const int MergeSubscribedBit = 8;
        private const int DistributionDatabaseBit = 16;

        #endregion

        #region fields

        private int categoryMask;

        #endregion

        #region constructors

        internal DatabaseReplicationCategories(int categoryMask)
        {
            this.categoryMask = categoryMask;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets whether the database is the distribution database
        /// </summary>
        public bool DistributionDatabase
        {
            get { return (DistributionDatabaseBit & categoryMask) == DistributionDatabaseBit; }
        }

        /// <summary>
        /// Gets whether the database is merge published for replication.
        /// </summary>
        public bool MergePublished
        {
            get { return (MergePublishedBit & categoryMask) == MergePublishedBit; }
        }

        /// <summary>
        /// Gets whether the database is merge subscribed for replication.
        /// </summary>
        public bool MergeSubscribed
        {
            get { return (MergeSubscribedBit & categoryMask) == MergeSubscribedBit; }
        }

        /// <summary>
        /// Gets whether the database is published for replication.
        /// </summary>
        public bool Published
        {
            get { return (PublishedBit & categoryMask) == PublishedBit; }
        }

        /// <summary>
        /// Gets whether the database is subscribed for replication.
        /// </summary>
        public bool Subscribed
        {
            get { return (SubscribedBit & categoryMask) == SubscribedBit; }
        }

        #endregion

        #region methods

        #endregion
    }
}
