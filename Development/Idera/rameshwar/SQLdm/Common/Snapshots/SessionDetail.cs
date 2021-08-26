//------------------------------------------------------------------------------
// <copyright file="ProcessDetail.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using System.Diagnostics;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;
using System.Runtime.Serialization;

    /// <summary>
    /// Detailed information on a single process
    /// <remarks>This is analagous to the Process Information view</remarks>
    /// </summary>
    [Serializable]
    public class SessionDetail : Session
    {
        #region fields

        private Int64? rowCount = null;
        private Int64? reads = null;
        private Int64? writes = null;
        private SessionCursorFetchStatus? cursorFetchStatus = null;
        private Int64? cursorSetRows = null;
        private int? lastError = null;
        private int? lineNumber = null;
        private TimeSpan? lockWaitTimeout = null;
        private Int64? textSize = null;
        private Int64? nestingLevel = null;
        private Int16? deadlockPriority = null;
        private TransactionIsolation? transactionIsolationLevel = null;
        private string language = null;
        private SessionOptions options = new SessionOptions();

        #endregion

        #region constructors

        public SessionDetail() : base()
        {
        }

        protected SessionDetail(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            rowCount = (Int64?)info.GetValue("rowCount", typeof(Int64?));
            reads = (Int64?)info.GetValue("reads", typeof(Int64?));
            writes = (Int64?)info.GetValue("writes", typeof(Int64?));
            cursorFetchStatus = (SessionCursorFetchStatus?)info.GetValue("cursorFetchStatus", typeof(SessionCursorFetchStatus?));
            cursorSetRows = (Int64?)info.GetValue("cursorSetRows", typeof(Int64?));
            lastError = (int?)info.GetValue("lastError", typeof(int?));
            lineNumber = (int?)info.GetValue("lineNumber", typeof(int?));
            lockWaitTimeout = (TimeSpan?)info.GetValue("lockWaitTimeout", typeof(TimeSpan?));
            textSize = (Int64?)info.GetValue("textSize", typeof(Int64?));
            nestingLevel = (Int64?)info.GetValue("nestingLevel", typeof(Int64?));
            deadlockPriority = (Int16?) info.GetValue("deadlockPriority", typeof (Int16?));
            transactionIsolationLevel = (TransactionIsolation?)info.GetValue("transactionIsolationLevel", typeof(TransactionIsolation?));
            language = info.GetString("language");
            options = (SessionOptions)info.GetValue("options", typeof(SessionOptions));
        }

        #endregion

        #region properties

        /// <summary>
        /// Number of rows affected by the last statement
        /// <remarks>This is not available on desktop editions of SQL Server.</remarks>
        /// </summary>
        public long? RowCount
        {
            get { return rowCount; }
            internal set { rowCount = value; }
        }

        /// <summary>
        /// Number of physical IO reads completed by the process
        /// </summary>
        public long? Reads
        {
            get { return reads; }
            internal set { reads = value; }
        }

        /// <summary>
        /// Number of physical IO writes completed by the process
        /// </summary>
        public long? Writes
        {
            get { return writes; }
            internal set { writes = value; }
        }

        /// <summary>
        /// Cursor fetch status
        /// </summary>
        public SessionCursorFetchStatus? CursorFetchStatus
        {
            get { return cursorFetchStatus; }
            internal set { cursorFetchStatus = value; }
        }

        /// <summary>
        /// TBD
        /// </summary>
        public long? CursorSetRows
        {
            get { return cursorSetRows; }
            internal set { cursorSetRows = value; }
        }

        /// <summary>
        /// The last error returned by the process
        /// </summary>
        public int? LastError
        {
            get { return lastError; }
            internal set { lastError = value; }
        }

        /// <summary>
        /// The current line number of the process
        /// </summary>
        public int? LineNumber
        {
            get { return lineNumber; }
            internal set { lineNumber = value; }
        }

        /// <summary>
        /// The length of time that a statement on this process will wait for a lock to be released
        /// </summary>
        public TimeSpan? LockWaitTimeout
        {
            get { return lockWaitTimeout; }
            internal set { lockWaitTimeout = value; }
        }

        /// <summary>
        /// The maximum text size of data to be returned
        /// </summary>
        public long? TextSize
        {
            get { return textSize; }
            internal set { textSize = value; }
        }

        /// <summary>
        /// Current trigger or procedure nesting level.
        /// </summary>
        public long? NestingLevel
        {
            get { return nestingLevel; }
            internal set { nestingLevel = value; }
        }

        /// <summary>
        /// The deadlock priority of the process - a value between -10 and 10
        /// </summary>
       public Int16? DeadlockPriority
        {
            get { return deadlockPriority; }
            internal set { deadlockPriority = value; }
        }

        /// <summary>
        /// The user friendly name of the deadlock priority
        /// </summary>
        public string DeadlockPriorityName
        {
            get {
                if (!DeadlockPriority.HasValue)
                    return null;
                return ConvertToDeadlockPriorityName(DeadlockPriority.Value);
            }
        }

        /// <summary>
        /// The current transaction isolation level of the process
        /// </summary>
        public TransactionIsolation? TransactionIsolationLevel
        {
            get { return transactionIsolationLevel; }
            internal set { transactionIsolationLevel = value; }
        }

        /// <summary>
        /// The language of the process
        /// </summary>
        public string Language
        {
            get { return language; }
            internal set { language = value; }
        }

        /// <summary>
        /// A list of the options currently set on the process
        /// </summary>
        public SessionOptions Options
        {
            get { return options; }
            internal set { options = value; }
        }


        #endregion

        #region events

        #endregion

        #region methods

        internal SessionCursorFetchStatus? StringToCursorFetchStatus(string s, ServerVersion ver)
        {
            switch (s)
            {
                case "0":
                    if (ver.Major > 8 || (CursorSetRows.HasValue && CursorSetRows > 0))
                        return SessionCursorFetchStatus.RowFetched;
                    else
                        return SessionCursorFetchStatus.NoCursor;
                case "1":  // SQL 2005+
                    return SessionCursorFetchStatus.NoCursor;
                case "-1":
                    return SessionCursorFetchStatus.EndOfResultSet;
                case "-2":
                    return SessionCursorFetchStatus.BaseRowInvalid;
                default:
                    return null;
            }
        }

        private string ConvertToDeadlockPriorityName(Int16 priority)
        {
            if (priority >= -10)
                return String.Format("Lowest ({0})", priority);
            if (priority > -10 && priority < -5)
                return String.Format("Below Low ({0})", priority);
            if (priority == -5)
                return String.Format("Low ({0})", priority);
            if (priority > -5 && priority < 0)
                return String.Format("Below Normal ({0})", priority);
            if (priority == 0)
                return String.Format("Normal ({0})", priority);
            if (priority > 0 && priority < 5)
                return String.Format("Above Normal ({0})", priority);
            if (priority == 5)
                return String.Format("High ({0})", priority);
            if (priority >5  && priority < 10)
                return String.Format("Above High ({0})", priority);
            if (priority >= 10)
                return String.Format("Highest ({0})", priority);
            return "Unknown";
        }

        #endregion

        #region interface implementations

        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("rowCount",rowCount);
            info.AddValue("reads",reads);
            info.AddValue("writes",writes);
            info.AddValue("cursorFetchStatus",cursorFetchStatus);
            info.AddValue("cursorSetRows",cursorSetRows);
            info.AddValue("lastError",lastError);
            info.AddValue("lineNumber",lineNumber);
            info.AddValue("lockWaitTimeout",lockWaitTimeout);
            info.AddValue("textSize",textSize);
            info.AddValue("nestingLevel",nestingLevel);
            info.AddValue("deadlockPriority",deadlockPriority);
            info.AddValue("transactionIsolationLevel",transactionIsolationLevel);
            info.AddValue("language",language);
            info.AddValue("options",options);
        }

        #endregion

        #region nested types

        #endregion

    }
}
