//------------------------------------------------------------------------------
// <copyright file="ProcessDetail.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Collections.Generic;

    ///// <summary>
    ///// Detailed information on a single process
    ///// <remarks>This is analagous to the Process Information view</remarks>
    ///// </summary>
    //class ProcessDetail : Process
    //{
    //    #region fields

    //    private string  _inputBuffer = null;
    //    private Int64?  _rowCount = null;
    //    private Int64? _reads = null;
    //    private Int64? _writes = null;
    //    private Int64? _readAheadSlots = null;
    //    private string _cursorFetchStatus = null;
    //    private Int64? _cursorSetRows = null;
    //    private int? _lastError = null;
    //    private int? _lineNumber = null;
    //    private TimeSpan? _lockWaitTimeout = null;
    //    private Int64? _textSize = null;
    //    private Int64? _nestingLevel = null;
    //    private DeadlockPrioritySetting? _deadlockPriority = null;
    //    private TransactionIsolation? _transactionIsolationLevel = null;
    //    private string _language = null;
    //    private List<SqlOptions> _options = null;
    //    private string _currentCommand = null;

    //    #endregion

    //    #region constructors

    //    #endregion

    //    #region properties
        
    //    /// <summary>
    //    /// Input buffer for the process, representing the most recent SQL statement
    //    /// </summary>
    //    public string InputBuffer
    //    {
    //        get { return _inputBuffer; }
    //    }

    //    /// <summary>
    //    /// Number of rows affected by the last statement
    //    /// <remarks>This is not available on desktop editions of SQL Server.</remarks>
    //    /// </summary>
    //    public long? RowCount
    //    {
    //        get { return _rowCount; }
    //    }

    //    /// <summary>
    //    /// Number of physical IO reads completed by the process
    //    /// </summary>
    //    public long? Reads
    //    {
    //        get { return _reads; }
    //    }

    //    /// <summary>
    //    /// Number of physical IO writes completed by the process
    //    /// </summary>
    //    public long? Writes
    //    {
    //        get { return _writes; }
    //    }

    //    /// <summary>
    //    /// TBD
    //    /// </summary>
    //    public long? ReadAheadSlots
    //    {
    //        get { return _readAheadSlots; }
    //    }

    //    /// <summary>
    //    /// Cursor fetch status  - This may be changed to an enumeration
    //    /// </summary>
    //    public string CursorFetchStatus
    //    {
    //        get { return _cursorFetchStatus; }
    //    }

    //    /// <summary>
    //    /// TBD
    //    /// </summary>
    //    public long? CursorSetRows
    //    {
    //        get { return _cursorSetRows; }
    //    }

    //    /// <summary>
    //    /// The last error returned by the process
    //    /// </summary>
    //    public int? LastError
    //    {
    //        get { return _lastError; }
    //    }

    //    /// <summary>
    //    /// The current line number of the process
    //    /// </summary>
    //    public int? LineNumber
    //    {
    //        get { return _lineNumber; }
    //    }

    //    /// <summary>
    //    /// The length of time that a statement on this process will wait for a lock to be released
    //    /// </summary>
    //    public TimeSpan? LockWaitTimeout
    //    {
    //        get { return _lockWaitTimeout; }
    //    }

    //    /// <summary>
    //    /// The maximum text size of data to be returned
    //    /// </summary>
    //    public long? TextSize
    //    {
    //        get { return _textSize; }
    //    }

    //    /// <summary>
    //    /// Current trigger or procedure nesting level.
    //    /// </summary>
    //    public long? NestingLevel
    //    {
    //        get { return _nestingLevel; }
    //    }

    //    /// <summary>
    //    /// The deadlock priority of the process
    //    /// <remarks>Need to elaborate on this</remarks>
    //    /// </summary>
    //    /// TODO: Elaborate on deadlock priority
    //    public DeadlockPrioritySetting? DeadlockPriority
    //    {
    //        get { return _deadlockPriority; }
    //    }

    //    /// <summary>
    //    /// The current transaction isolation level of the process
    //    /// </summary>
    //    public TransactionIsolation? TransactionIsolationLevel
    //    {
    //        get { return _transactionIsolationLevel; }
    //    }

    //    /// <summary>
    //    /// The language of the process
    //    /// </summary>
    //    public string Language
    //    {
    //        get { return _language; }
    //    }

    //    /// <summary>
    //    /// A list of the options currently set on the process
    //    /// <remarks>Will be revisited...</remarks>
    //    /// </summary>
    //    public List<SqlOptions> Options
    //    {
    //        get { return _options; }
    //    }

    //    /// <summary>
    //    /// The current command on the process
    //    /// <remarks>Needs elaboration</remarks>
    //    /// </summary>
    //    public string CurrentCommand
    //    {
    //        get { return _currentCommand; }
    //    }

    //    #endregion

    //    #region events

    //    #endregion

    //    #region methods

    //    #endregion

    //    #region interface implementations

    //    #endregion

    //    #region nested types

    //    #endregion

    //}
}
