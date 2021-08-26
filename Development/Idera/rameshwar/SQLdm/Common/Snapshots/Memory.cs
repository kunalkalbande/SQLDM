//------------------------------------------------------------------------------
// <copyright file="Memory.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Collections.Generic;
using Idera.SQLdm.Common.Configuration;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// Represents diagnostic information about the memory of a SQL Server.
    /// </summary>
    [Serializable]
    public class Memory 
    {
        #region fields

        private FileSize cachePages = new FileSize();
        private FileSize committedPages = new FileSize();
        private FileSize connectionMemory = new FileSize();
        private FileSize freePages = new FileSize();
        private FileSize freeCachePages = new FileSize();
        private FileSize grantedWorkspaceMemory = new FileSize();
        private FileSize lockMemory = new FileSize();
        private FileSize optimizerMemory = new FileSize();
        private FileSize bufferCachePages = new FileSize();
        private List<PinnedTable> pinnedTables = new List<PinnedTable>();
        private FileSize totalServerMemory = new FileSize();
        private double? procedureCachePercentageUsed = null;
        private FileSize targetServerMemory = new FileSize();

        #endregion

        #region constructors

        #endregion

        #region properties

        /// <summary>
        /// Number of pages in the buffer pool (includes database, free, and stolen pages).
        /// DO NOT add to Committed as there is overlap
        /// </summary>
        public FileSize BufferCachePages
        {
            get { return bufferCachePages; }
            set { bufferCachePages = value; }
        }

        /// <summary>
        /// Total amount of memory used by procedure cache
        /// </summary>
        public FileSize CachePages
        {
            get { return cachePages; }
            internal set { cachePages = value; }
        }

        /// <summary>
        /// Number of pages in the buffer pool with database content.
        /// </summary>
        public FileSize CommittedPages
        {
            get { return committedPages; }
            internal set { committedPages = value; }
        }

        /// <summary>
        /// Total amount of dynamic memory the server is using for maintaining connections.
        /// </summary>
        public FileSize ConnectionMemory
        {
            get { return connectionMemory; }
            internal set { connectionMemory = value; }
        }

        /// <summary>
        /// Total number of pages on all free lists.
        /// </summary>
        public FileSize FreePages
        {
            get { return freePages; }
            internal set { freePages = value; }
        }

        /// <summary>
        /// Total amount of memory currently granted to executing processes such as hash, sort, bulk copy, and index creation operations.
        /// </summary>
        public FileSize GrantedWorkspaceMemory
        {
            get { return grantedWorkspaceMemory; }
            internal set { grantedWorkspaceMemory = value; }
        }

        /// <summary>
        /// Total amount of dynamic memory the server is using for locks.
        /// </summary>
        public FileSize LockMemory
        {
            get { return lockMemory; }
            internal set { lockMemory = value; }
        }

        /// <summary>
        /// Total amount of dynamic memory the server is using for query optimization.
        /// </summary>
        public FileSize OptimizerMemory
        {
            get { return optimizerMemory; }
            internal set { optimizerMemory = value; }
        }

        ///// <summary>
        ///// List of tables pinned in memory
        ///// </summary>
        //public List<PinnedTable> PinnedTables
        //{
        //    get { return pinnedTables; }
        //    internal set { pinnedTables = value; }
        //}

        /// <summary>
        /// The committed memory from the buffer pool (in kilobytes).
        /// BOL indicates this is NOT the total memory used by SQL Server
        /// </summary>
        public FileSize TotalServerMemory
        {
            get { return totalServerMemory; }
            internal set { totalServerMemory = value; }
        }

        /// <summary>
        /// Percentage of procedure cache currently in use
        /// </summary>
        public double? ProcedureCachePercentageUsed
        {
            get { return procedureCachePercentageUsed; }
            internal set { procedureCachePercentageUsed = value; }
        }

        /// <summary>
        /// Free procedure cache pages
        /// </summary>
        public FileSize FreeCachePages
        {
            get { return freeCachePages; }
            set { freeCachePages = value; }
        }

        public FileSize TargetServerMemory
        {
            get { return targetServerMemory; }
            internal set { targetServerMemory = value; }
        }





        #endregion

        #region methods

        #endregion
    }
}