//------------------------------------------------------------------------------
// <copyright file="Process.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;

    /// <summary>
    /// A single process to be displayed in the process list
    /// </summary>
    public class Session 
    {
        #region fields

        private string      _application = null;
        private Int64?      _blockedBy;
        private string      _command;
        private CpuTime     _cpu;
        private TimeSpan    _currentWaitTime;
        private string      _database;
        private Int64?      _physicalIo;
        private Int64?      _executionContext;
        private TimeSpan    _lastActivity;
        private DateTime?   _loggedInSince;
        private FileSize    _memory = null;
        private Int64?      _openTransactions;
        private Int64?      _spid;
        private string      _status;
        private string      _userName;
        private string      _workstation;
        private string      _workstationNetAddress;

        #endregion

        #region constructors

        //public Process() 
        //{
        //}

        #endregion

        #region properties

        /// <summary>
        /// Name of the application using the process
        /// </summary>
        public string Application
        {
            get { return _application; }
        }

        /// <summary>
        /// SPID of the process that is blocking this process, or null if not blocked
        /// </summary>
        public Int64? BlockedBy
        {
            get { return _blockedBy; }
        }

        /// <summary>
        /// Command being executed by the process
        /// </summary>
        public string Command
        {
            get { return _command; }
        }

        /// <summary>
        /// Cumulative CPU time utilized by the process
        /// </summary>
        public CpuTime Cpu
        {
            get { return _cpu; }
        }

        /// <summary>
        /// Wait time in milliseconds.
        /// <remarks>Returns 0 for a process that is not waiting.</remarks>
        /// </summary>
        public TimeSpan CurrentWaitTime
        {
            get { return _currentWaitTime; }
        }

        /// <summary>
        /// Name of the database being used by the process.
        /// <remarks>This is the name of the database as designed by a "use [name]" statement.
        /// This will not reflect databases being used in any other way.</remarks>
        /// </summary>
        public string Database
        {
            get { return _database; }
        }

        /// <summary>
        /// Cumulative physical disk reads and writes
        /// </summary>
        public Int64? PhysicalIo
        {
            get { return _physicalIo; }
        }

        /// <summary>
        /// Execution context ID.  This is used to identify subthreads of a single process.
        /// </summary>
        public Int64? ExecutionContext
        {
            get { return _executionContext; }
        }

        /// <summary>
        /// Time since last activity for the process
        /// <remarks>This may be removed or slightly changed.  This is currently being 
        /// calculated in the batch and may not belong in the object model at all.</remarks>
        /// </summary>
        public TimeSpan LastActivity
        {
            get { return _lastActivity; }
        }

        /// <summary>
        /// Time at which the process logged into the server.  For SQL Server system processes,
        /// this is the time at which the server started.  SPID 1-5 can be reliably considered 
        /// system processes for the purposes of this calculation.
        /// </summary>
        public DateTime? LoggedInSince
        {
            get { return _loggedInSince; }
        }

        /// <summary>
        /// Memory allocated to process.
        /// <remarks>A negative number indicates that the process is currently freeing 
        /// memory allocated by another process</remarks>
        /// </summary>
        public FileSize Memory
        {
            get { return _memory; }
        }

        /// <summary>
        /// Number of open transactions on this process.
        /// </summary>
        public Int64? OpenTransactions
        {
            get { return _openTransactions; }
        }

        /// <summary>
        /// The SPID of the process.  
        /// <remarks>In SQL 7 and 2000 this stood for Server Process ID - in 
        /// SQL 2005 the vocabulary has been changed and this is now Session ID.  This acts as the
        /// unique identifier for the process.  SPID 1-5 can be reliably considered SQL Server system
        /// processes.</remarks>
        /// </summary>
        public Int64? Spid
        {
            get { return _spid; }
        }

        /// <summary>
        /// Status of the process (for example, "running" or "sleeping")
        /// </summary>
        public string Status
        {
            get { return _status; }
        }

        /// <summary>
        /// The login name used by the process
        /// </summary>
        public string UserName
        {
            get { return _userName; }
        }

        /// <summary>
        /// The name of the workstation which owns this process
        /// </summary>
        public string Workstation
        {
            get { return _workstation; }
        }

        /// <summary>
        /// Unique identifier for the network adapter of the process workstation
        /// </summary>
        public string WorkstationNetAddress
        {
            get { return _workstationNetAddress; }
        }

        #endregion

        #region methods

        ///// <summary>
        ///// Dumps sample data to a string.
        ///// </summary>
        ///// <returns>Sample data.</returns>
        //public string Dump()
        //{
        //    StringBuilder dump = new StringBuilder();

        //    dump.Append("Memory: " + Memory); dump.Append("\n");
        //    dump.Append("Application: " + Application); dump.Append("\n");
        //    dump.Append("BlockedBy: " + BlockedBy); dump.Append("\n");
        //    dump.Append("Command: " + Command); dump.Append("\n");
        //    dump.Append("CpuTime: " + CpuTime); dump.Append("\n");
        //    dump.Append("CurrentWaitTime: " + CurrentWaitTime); dump.Append("\n");
        //    dump.Append("Database: " + Database); dump.Append("\n");
        //    dump.Append("PhysicalIo: " + PhysicalIo); dump.Append("\n");
        //    dump.Append("ExecutionContext: " + ExecutionContext); dump.Append("\n");
        //    dump.Append("LastActivity: " + LastActivity); dump.Append("\n");
        //    dump.Append("LoggedInSince: " + LoggedInSince); dump.Append("\n");
        //    dump.Append("OpenTransactions: " + OpenTransactions); dump.Append("\n");
        //    dump.Append("Spid: " + Spid); dump.Append("\n");
        //    dump.Append("Status: " + Status); dump.Append("\n");
        //    dump.Append("UserName: " + UserName); dump.Append("\n");
        //    dump.Append("Workstation: " + Workstation); dump.Append("\n");
        //    dump.Append("WorkstationNetAddress: " + WorkstationNetAddress); dump.Append("\n");

        //    foreach (Lock processLock in Locks)
        //    {
        //        dump.Append(processLock.Dump());
        //    }

        //    return dump.ToString();
        //}

        //Internal void SetMemory(FileSize memory)
        //{
        //    _Memory = memory;
        //}

        //Internal void SetApplication(string application)
        //{
        //    _application = application;
        //}

        //Internal void SetBlockedBy(Int64? blockedBy)
        //{
        //    _blockedBy = blockedBy;
        //}

        //Internal void SetCommand(string command)
        //{
        //    _command = command;
        //}

        //Internal void SetCpuTime(Int64? cpuTime)
        //{
        //    _cpuTime = cpuTime;
        //}

        //Internal void SetCurrentWaitTime(Int64? currentWaitTime)
        //{
        //    _currentWaitTime = currentWaitTime;
        //}

        //Internal void SetDatabase(string database)
        //{
        //    _database = database;
        //}

        //Internal void SetPhysicalIo(Int64? physicalIo)
        //{
        //    _physicalIo = physicalIo;
        //}

        //Internal void SetExecutionContext(Int64? executionContext)
        //{
        //    _executionContext = executionContext;
        //}

        //Internal void SetLastActivity(string lastActivity)
        //{
        //    _lastActivity = lastActivity;
        //}

        //Internal void SetLoggedInSince(DateTime loggedInSince)
        //{
        //    _loggedInSince = loggedInSince;
        //}

        //Internal void SetOpenTransactions(Int64? openTransactions)
        //{
        //    _openTransactions = openTransactions;
        //}

        //Internal void SetSpid(Int64? spid)
        //{
        //    _spid = spid;
        //}

        //Internal void SetStatus(string status)
        //{
        //    _status = status;
        //}

        //Internal void SetUserName(string userName)
        //{
        //    _userName = userName;
        //}

        //Internal void SetWorkstation(string workstation)
        //{
        //    _workstation = workstation;
        //}

        //Internal void SetWorkstationNetAddress(string workstationNetAddress)
        //{
        //    _workstationNetAddress = workstationNetAddress;
        //}

        #endregion

    }
}
