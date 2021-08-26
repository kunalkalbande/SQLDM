//------------------------------------------------------------------------------
// <copyright file="DatabaseOptions.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System;

namespace Idera.SQLdm.Common.Snapshots
{

    /// <summary>
    /// Represents the state of database options.
    /// </summary>
    [Serializable]
    public sealed class DatabaseOptions
    {
        #region constants

        // Status 1 bit masks
        private const int AutoCloseBit = 1;
        private const int AutoShrinkBit = 4194304;
        private const int BulkCopyBit = 4;
        private const int DboUseOnlyBit = 2048;
        private const int OfflineBit = 512;
        private const int ReadOnlyBit = 1024;
        private const int SingleUserBit = 4096;
        private const int TornPageDetectionBit = 16;
        private const int TruncateLogOnCheckpointBit = 8;
        private const int StandbyBit = 2097152;

        // Status 2 bit masks
        private const int AnsiNullDefaultBit = 16384;
        private const int AnsiNullsBit = 67108864;
        private const int AnsiWarningsBit = 268435456;
        private const int AutoCreateStatisticsBit = 16777216;
        private const int AutoUpdateStatisticsBit = 1073741824;
        private const int ConcatNullYieldsNullBit = 65536;
        private const int CursorCloseOnCommitBit = 33554432;
        private const int DefaultToLocalCursorBit = 1048576;
        private const int FullTextIndexingBit = 536870912;
        private const int QuoteDelimiterBit = 8388608;
        private const int RecursiveTriggersBit = 131072;

        #endregion

        #region fields

        private int _statusMask1;
        private int _statusMask2;

        #endregion

        #region constructors

        internal DatabaseOptions(int statusMask1, int statusMask2)
        {
            _statusMask1 = statusMask1;
            _statusMask2 = statusMask2;
        }

        #endregion

        #region properties

        /// <summary>
        /// Gets whether CREATE TABLE follows the SQL-92 rules to determine if a column allows null values.
        /// </summary>
        public bool AnsiNullDefault
        {
            get { return (AnsiNullDefaultBit & _statusMask2) == AnsiNullDefaultBit; }
        }

        /// <summary>
        /// Gets whether all comparisons to a null value evaluate to UNKNOWN. 
        /// When false, comparisons of non-UNICODE values to a null value 
        /// evaluate to TRUE if both values are NULL.
        /// </summary>
        public bool AnsiNulls
        {
            get { return (AnsiNullsBit & _statusMask2) == AnsiNullsBit; }
        }

        /// <summary>
        /// Gets whether errors or warnings are issued when conditions such as "divide by zero" occur.
        /// </summary>
        public bool AnsiWarnings
        {
            get { return (AnsiWarningsBit & _statusMask2) == AnsiWarningsBit; }
        }

        /// <summary>
        /// Gets whether the database is shutdown cleanly and its resources are freed after the last user logs off.
        /// </summary>
        public bool AutoClose
        {
            get { return (AutoCloseBit & _statusMask1) == AutoCloseBit; }
        }

        /// <summary>
        /// Gets whether statistics are automatically created on columns used in a predicate.
        /// </summary>
        public bool AutoCreateStatistics
        {
            get { return (AutoCreateStatisticsBit & _statusMask2) == AutoCreateStatisticsBit; }
        }

        /// <summary>
        /// Gets whether the database files are candidates for automatic periodic shrinking.
        /// </summary>
        public bool AutoShrink
        {
            get { return (AutoShrinkBit & _statusMask1) == AutoShrinkBit; }
        }

        /// <summary>
        /// Gets whether existing statistics are automatically updated when the statistics become 
        /// out-of-date because the data in the tables has changed.
        /// </summary>
        public bool AutoUpdateStatistics
        {
            get { return (AutoUpdateStatisticsBit & _statusMask2) == AutoUpdateStatisticsBit; }
        }

        /// <summary>
        /// Gets whether if either operand in a concatenation operation is NULL, the result is NULL.
        /// </summary>
        public bool ConcatNullYieldsNull
        {
            get { return (ConcatNullYieldsNullBit & _statusMask2) == ConcatNullYieldsNullBit; }
        }

        /// <summary>
        /// Gets whether any cursors that are open when a transaction is committed or rolled back are closed.
        /// </summary>
        public bool CursorCloseOnCommit
        {
            get { return (CursorCloseOnCommitBit & _statusMask2) == CursorCloseOnCommitBit; }
        }

        /// <summary>
        /// Gets whether only the database owner can use the database.
        /// </summary>
        public bool DboUseOnly
        {
            get { return (DboUseOnlyBit & _statusMask1) == DboUseOnlyBit; }
        }

        /// <summary>
        /// Gets whether cursor declarations default to LOCAL.
        /// </summary>
        public bool DefaultToLocal
        {
            get { return (DefaultToLocalCursorBit & _statusMask2) == DefaultToLocalCursorBit; }
        }

        /// <summary>
        /// Gets whether full-text indexing is enabled.
        /// </summary>
        public bool FullTextIndexing
        {
            get { return (FullTextIndexingBit & _statusMask2) == FullTextIndexingBit; }
        }

        /// <summary>
        /// Gets whether the database is offline.
        /// </summary>
        public bool Offline
        {
            get { return (OfflineBit & _statusMask1) == OfflineBit; }
        }

        /// <summary>
        /// Gets whether double quotation marks can be used to enclose delimited identifiers.
        /// </summary>
        public bool QuoteDelimiter
        {
            get { return (QuoteDelimiterBit & _statusMask2) == QuoteDelimiterBit; }
        }

        /// <summary>
        /// Gets whether users can only read data in the database, not modify it. 
        /// </summary>
        public bool ReadOnly
        {
            get { return (ReadOnlyBit & _statusMask1) == ReadOnlyBit; }
        }

        /// <summary>
        /// Gets whether recursive firing of triggers is enabled.
        /// </summary>
        public bool RecursiveTriggers
        {
            get { return (RecursiveTriggersBit & _statusMask2) == RecursiveTriggersBit; }
        }

        /// <summary>
        /// Gets whether the SELECT INTO statement and fast bulk copies are allowed.
        /// </summary>
        public bool SelectIntoBulkCopy
        {
            get { return (BulkCopyBit & _statusMask1) == BulkCopyBit; }
        }

        /// <summary>
        /// Gets whether only one user at a time can access the database.
        /// </summary>
        public bool SingleUser
        {
            get { return (SingleUserBit & _statusMask1) == SingleUserBit; }
        }

        /// <summary>
        /// Gets whether incomplete pages can be detected.
        /// </summary>
        public bool TornPageDetection
        {
            get { return (TornPageDetectionBit & _statusMask1) == TornPageDetectionBit; }
        }

        /// <summary>
        /// Gets whether a checkpoint truncates the inactive part of the log when the 
        /// database is in log truncate mode. This is the only option you can set for 
        /// the master database.
        /// </summary>
        public bool TruncateLogOnCheckpoint
        {
            get { return (TruncateLogOnCheckpointBit & _statusMask1) == TruncateLogOnCheckpointBit; }
        }

        /// <summary>
        /// Gets whether a database is in standby mode
        /// </summary>
        public bool Standby
        {
            get { return (StandbyBit & _statusMask1) == StandbyBit; }
        }

        #endregion

        #region methods

       

        #endregion
    }
}
