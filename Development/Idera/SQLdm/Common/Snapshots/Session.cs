//------------------------------------------------------------------------------
// <copyright file="Process.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using Idera.SQLdm.Common.Auditing;
using Wintellect.PowerCollections;

namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// A single process to be displayed in the process list
    /// </summary>
    [Serializable]
    public class Session : ISerializable, IFilterableSession, IAuditable
    {
        #region fields

        private string      application;
        private int?        blockedBy;
        private int         blockingCount;
        private string      command;
        private TimeSpan    cpu = new TimeSpan();
        private TimeSpan    cpuDelta = new TimeSpan();
        private string      database;
        private Int64?      physicalIo;
        private Int64?      physicalIoDelta;
        private Int64?      executionContext;
        private DateTime?    lastActivity;
        private string      lastCommand;
        private DateTime?   loggedInSince;
        private FileSize    memory = new FileSize();
        private string      netLibrary = null;
        private Int64?      openTransactions;
        private int?        spid;
        private SessionStatus status = SessionStatus.Other;
        private string      userName;
        private TimeSpan    waitTime;
        private string      workstation;
        private string      workstationNetAddress;
        private string      waitType;
        private string      waitResource;
        private TimeSpan?   versionStoreElapsedTime = null;
        // Auto-implemented properties below

        #endregion

        #region constructors

        public Session()
        {
        }

        protected Session(SerializationInfo info, StreamingContext context)
        {
            application = info.GetString("application");
            blockedBy = (int?)info.GetValue("blockedBy", typeof(int?));
            blockingCount = info.GetInt32("blockingCount");
            command = info.GetString("command");
            cpu = (TimeSpan)info.GetValue("cpu", typeof(TimeSpan));
            cpuDelta = (TimeSpan)info.GetValue("cpuDelta", typeof(TimeSpan));
            database = info.GetString("database");
            physicalIo = (Int64?)info.GetValue("physicalIo", typeof(Int64?));
            physicalIoDelta = (Int64?)info.GetValue("physicalIoDelta", typeof(Int64?));
            executionContext = (Int64?)info.GetValue("executionContext", typeof(Int64?));
            lastActivity = (DateTime?)info.GetValue("lastActivity", typeof(DateTime?));
            lastCommand = info.GetString("lastCommand");
            loggedInSince = (DateTime?)info.GetValue("loggedInSince", typeof(DateTime?));
            memory = (FileSize)info.GetValue("memory", typeof(FileSize));
            netLibrary = info.GetString("netLibrary");
            openTransactions = (Int64?)info.GetValue("openTransactions", typeof(Int64?));
            spid = (int?)info.GetValue("spid", typeof(int?));
            status = (SessionStatus)info.GetValue("status", typeof(SessionStatus));
            userName = info.GetString("userName");
            waitTime = (TimeSpan)info.GetValue("waitTime", typeof(TimeSpan));
            workstation = info.GetString("workstation");
            workstationNetAddress = info.GetString("workstationNetAddress");
            waitType = info.GetString("waitType");
            waitResource = info.GetString("waitResource");
            try
            {
                versionStoreElapsedTime = (TimeSpan?)info.GetValue("versionStoreElapsedTime", typeof(TimeSpan?));
                TransactionIsolationLevel = (TransactionIsolation)info.GetValue("TransactionIsolationLevel", typeof(TransactionIsolation));
                SessionUserAllocatedTotal = (FileSize)info.GetValue("SessionUserAllocatedTotal", typeof(FileSize));
                SessionUserDeallocatedTotal = (FileSize)info.GetValue("SessionUserDeallocatedTotal", typeof(FileSize));
                SessionInternalAllocatedTotal = (FileSize)info.GetValue("SessionInternalAllocatedTotal", typeof(FileSize));
                SessionInternalDeallocatedTotal = (FileSize)info.GetValue("SessionInternalDeallocatedTotal", typeof(FileSize));
                TaskUserAllocatedTotal = (FileSize)info.GetValue("TaskUserAllocatedTotal", typeof(FileSize));
                TaskUserDeallocatedTotal = (FileSize)info.GetValue("TaskUserDeallocatedTotal", typeof(FileSize));
                TaskInternalAllocatedTotal = (FileSize)info.GetValue("TaskInternalAllocatedTotal", typeof(FileSize));
                TaskInternalDeallocatedTotal = (FileSize)info.GetValue("TaskInternalDeallocatedTotal", typeof(FileSize));
            }
            catch (Exception)
            {
               // Don't error out when there's no data for new types
            }
            
            
        }

        #endregion

        #region properties

        /// <summary>
        /// Name of the application using the process
        /// </summary>
        public string Application
        {
            get
            {
                if (IsUserProcess) return application;
                if (!String.IsNullOrEmpty(command))
                    return "(SYSTEM: " + command + ")";
                return "(SYSTEM)";
            }
            internal set
            {
                if (value.Length > 0)
                {
                    application = value;
                }
            }
        }

        /// <summary>
        /// SPID of the process that is blocking this process, or null if not blocked
        /// </summary>
        public int? BlockedBy
        {
            get { return blockedBy; }
            internal set
            {
                //A value of zero indicates that the session is not blocked
                if (value > 0)
                    blockedBy = value;
            }
        }

        /// <summary>
        /// Returns true if session is blocking at least one other session
        /// </summary>
        public bool Blocking
        {
            get { return (BlockingCount > 0); }
        }

        /// <summary>
        /// Count of sessions blocked by this session
        /// </summary>
        public int BlockingCount
        {
            get { return blockingCount; }
            internal set { blockingCount = value; }
        }

        /// <summary>
        /// Command being executed by the process
        /// </summary>
        public string Command
        {
            get { return command; }
            internal set
            {
                if (value.Length > 0)
                {
                    command = value;
                }
            }
        }

        /// <summary>
        /// Cumulative CPU time utilized by the process
        /// </summary>
        public TimeSpan Cpu
        {
            get { return cpu; }
            internal set { cpu = value; }
        }

        /// <summary>
        /// CPU time utilized by the process between two refreshes
        /// </summary>
        public TimeSpan CpuDelta
        {
            get { return cpuDelta; }
            internal set { cpuDelta = value; }
        }

        /// <summary>
        /// Name of the database being used by the process.
        /// <remarks>This is the name of the database as designed by a "use [name]" statement.
        /// This will not reflect databases being used in any other way.</remarks>
        /// </summary>
        public string Database
        {
            get { return database; }
            internal set
            {
                if (value.Length > 0)
                {
                    database = value;
                }
            }
        }

        /// <summary>
        /// FOR SQLDM INTERNAL USE
        /// Unique identifier for a session between refreshes
        /// </summary>
        public Pair<int?, DateTime?> InternalSessionIdentifier
        {
            get { return new Pair<int?, DateTime?>(Spid, LoggedInSince); }
        }

        /// <summary>
        /// Returns whether the given session is a system process
        /// </summary>
        public bool IsSystemProcess
        {
            //Whether this is a system process is determined by the workstation address
            get { return !(IsUserProcess); }
        }

        /// <summary>
        /// Returns whether the given session is a user process
        /// </summary>
        public bool IsUserProcess
        {
            //Whether this is a system process is determined by the workstation address
            get { return (workstationNetAddress != null && workstationNetAddress.Length > 0); }
        }

        /// <summary>
        /// Cumulative physical disk reads and writes
        /// </summary>
        public Int64? PhysicalIo
        {
            get { return physicalIo; }
            internal set { physicalIo = value; }
        }

        /// <summary>
        /// Physical disk reads and writes between refreshes
        /// </summary>
        public long? PhysicalIoDelta
        {
            get { return physicalIoDelta; }
            internal set { physicalIoDelta = value; }
        }

        /// <summary>
        /// Execution context ID.  This is used to identify subthreads of a single process.
        /// </summary>
        public Int64? ExecutionContext
        {
            get { return executionContext; }
            internal set { executionContext = value; }
        }

        /// <summary>
        /// Time that last activity occurred for the process
        /// </summary>
        public DateTime? LastActivity
        {
            get { return lastActivity; }
            internal set { lastActivity = value; }
        }

        /// <summary>
        /// Last command that occurred on this session
        /// </summary>
        public string LastCommand
        {
            get { return lastCommand; }
            internal set { lastCommand = value; }
        }

        /// <summary>
        /// Time at which the process logged into the server.  For SQL Server system processes,
        /// this is the time at which the server started.  SPID 1-5 can be reliably considered 
        /// system processes for the purposes of this calculation.
        /// </summary>
        public DateTime? LoggedInSince
        {
            get { return loggedInSince; }
            internal set { loggedInSince = value; }
        }

        /// <summary>
        /// Memory allocated to process.
        /// <remarks>A negative number indicates that the process is currently freeing 
        /// memory allocated by another process</remarks>
        /// </summary>
        public FileSize Memory
        {
            get { return memory; }
            internal set { memory = value; }
        }

        /// <summary>
        /// Net library utilized by the session
        /// </summary>
        public string NetLibrary
        {
            get { return netLibrary; }
            internal set
            {
                if (value.Length > 0)
                {
                    netLibrary = value;
                }
            }
        }

        /// <summary>
        /// Number of open transactions on this process.
        /// </summary>
        public Int64? OpenTransactions
        {
            get { return openTransactions; }
            internal set { openTransactions = value; }
        }

        /// <summary>
        /// The SPID of the process.  
        /// <remarks>In SQL 7 and 2000 this stood for Server Process ID - in 
        /// SQL 2005 the vocabulary has been changed and this is now Session ID.  This acts as the
        /// unique identifier for the process.  SPID 1-5 can be reliably considered SQL Server system
        /// processes.</remarks>
        /// </summary>
        public int? Spid
        {
            get { return spid; }
            internal set { spid = value; }
        }

        /// <summary>
        /// Status of the process (for example, "running" or "sleeping")
        /// </summary>
        public SessionStatus Status
        {
            get { return status; }
            internal set { status = value; }
        }

        /// <summary>
        /// The login name used by the process
        /// </summary>
        public string UserName
        {
            get { return userName; }
            internal set
            {
                if (value.Length > 0)
                {
                    userName = value;
                }
            }
        }

        /// <summary>
        /// Wait time of the session
        /// </summary>
        public TimeSpan WaitTime
        {
            get { return waitTime; }
            internal set { waitTime = value; }
        }

        /// <summary>
        /// A string indicating the name of the last or current wait type.
        /// </summary>
        public string WaitType
        {
            get
            {
                // Do not return the wait type if the session is not waiting
                if (waitTime.TotalMilliseconds > 0)
                {
                    return waitType;
                }
                else
                {
                    return null;
                }
            }
            internal set {
                // Do not set to null string
                if (value.Length > 0)
                {
                    waitType = value;
                }
            }
        }

        /// <summary>
        /// Textual representation of a lock resource.
        /// </summary>
        public string WaitResource
        {
            get
            {
                // Do not return the wait resource if the session is not waiting
                if (waitTime.TotalMilliseconds > 0)
                {
                    return waitResource;
                }
                else
                {
                    return null;
                }
            }
            internal set
            {
                // Do not set to null string
                if (value.Length > 0)
                {
                    waitResource = value;
                }
            }
        }

        /// <summary>
        /// The name of the workstation which owns this process.
        /// Note: workstation can briefly be null while IsUserProcess is TRUE
        /// </summary>
        public string Workstation
        {
            get {
                return IsUserProcess ? workstation : "(SYSTEM)"; ;
            }
            internal set
            {
                if (value.Length > 0)
                {
                    workstation = value;
                }
            }
        }

        /// <summary>
        /// Unique identifier for the network adapter of the process workstation
        /// </summary>
        public string WorkstationNetAddress
        {
            get { return workstationNetAddress; }
            internal set { 
                if (value.Length > 0)
                {
                    workstationNetAddress = value;
                }
            }
        }

        public TimeSpan? VersionStoreElapsedTime
        {
            get { return versionStoreElapsedTime; }
            internal set { versionStoreElapsedTime = value; }
        }

        public TransactionIsolation TransactionIsolationLevel { get; internal set; }

        public FileSize SessionUserAllocatedTotal { get; internal set; }
        public FileSize SessionUserDeallocatedTotal { get; internal set; }
        public FileSize TaskUserAllocatedTotal { get; internal set; }
        public FileSize TaskUserDeallocatedTotal { get; internal set; }
        public FileSize SessionInternalAllocatedTotal { get; internal set; }
        public FileSize SessionInternalDeallocatedTotal { get; internal set; }
        public FileSize TaskInternalAllocatedTotal { get; internal set; }
        public FileSize TaskInternalDeallocatedTotal { get; internal set; }

        public FileSize SessionUserSpaceUsed
        {
            get
            {
                if (SessionUserAllocatedTotal != null && SessionUserAllocatedTotal.Bytes.HasValue 
                    && SessionUserDeallocatedTotal != null && SessionUserDeallocatedTotal.Bytes.HasValue)
                {
                    if (SessionUserAllocatedTotal.Kilobytes > SessionUserDeallocatedTotal.Kilobytes)
                        return new FileSize(SessionUserAllocatedTotal.Kilobytes - SessionUserDeallocatedTotal.Kilobytes);
                    return new FileSize(0);
                }
                return new FileSize();
            }
        }

        public FileSize SessionInternalSpaceUsed
        {
            get
            {
                if (SessionInternalAllocatedTotal != null && SessionInternalAllocatedTotal.Bytes.HasValue
                    && SessionInternalDeallocatedTotal != null && SessionInternalDeallocatedTotal.Bytes.HasValue)
                {
                    if (SessionInternalAllocatedTotal.Kilobytes > SessionInternalDeallocatedTotal.Kilobytes)
                        return new FileSize(SessionInternalAllocatedTotal.Kilobytes - SessionInternalDeallocatedTotal.Kilobytes);
                    return new FileSize(0);
                }
                return new FileSize();
            }
        }

        public FileSize TaskUserSpaceUsed
        {
            get
            {
                if (TaskUserAllocatedTotal != null 
                    && TaskUserAllocatedTotal.Bytes.HasValue 
                    && TaskUserDeallocatedTotal !=null 
                    && TaskUserDeallocatedTotal.Bytes.HasValue)
                {
                    if (TaskUserAllocatedTotal.Kilobytes > TaskUserDeallocatedTotal.Kilobytes)
                        return new FileSize(TaskUserAllocatedTotal.Kilobytes - TaskUserDeallocatedTotal.Kilobytes);
                    return new FileSize(0);
                }
                return new FileSize();
            }
        }

        public FileSize TaskInternalSpaceUsed
        {
            get
            {
                if (TaskInternalAllocatedTotal != null && TaskInternalAllocatedTotal.Bytes.HasValue && TaskInternalDeallocatedTotal != null && TaskInternalDeallocatedTotal.Bytes.HasValue)
                {
                    if (TaskInternalAllocatedTotal.Kilobytes > TaskInternalDeallocatedTotal.Kilobytes)
                        return new FileSize(TaskInternalAllocatedTotal.Kilobytes - TaskInternalDeallocatedTotal.Kilobytes);
                    return new FileSize(0);
                }
                return new FileSize();
            }
        }

        public FileSize TotalUserSpaceUsed
        {
            get
            {
                if (SessionUserSpaceUsed != null && SessionUserSpaceUsed.Bytes.HasValue && TaskUserSpaceUsed != null && TaskUserSpaceUsed.Bytes.HasValue)
                {
                    return new FileSize(SessionUserSpaceUsed.Kilobytes + TaskUserSpaceUsed.Kilobytes);
                }
                return new FileSize();
            }
        }

        public FileSize TotalInternalSpaceUsed
        {
            get
            {
                if (SessionInternalSpaceUsed != null && SessionInternalSpaceUsed.Bytes.HasValue && TaskInternalSpaceUsed != null && TaskInternalSpaceUsed.Bytes.HasValue)
                {
                    return new FileSize(SessionInternalSpaceUsed.Kilobytes + TaskInternalSpaceUsed.Kilobytes);
                }
                return new FileSize();
            }
        }

        public FileSize TotalTaskSpaceUsed
        {
            get
            {
                if (TaskUserSpaceUsed != null && TaskUserSpaceUsed.Bytes.HasValue && TaskInternalSpaceUsed != null && TaskInternalSpaceUsed.Bytes.HasValue)
                {
                    return new FileSize(TaskUserSpaceUsed.Kilobytes + TaskInternalSpaceUsed.Kilobytes);
                }
                return new FileSize();
            }
        }

        /// <summary>
        /// This is the totaluserspaceused.
        /// TotalInternalSpaceUsed removed from calc 08-03-2012
        /// </summary>
        public FileSize TotalTempdbSpaceUsed
        {
            get
            {
                if (TotalUserSpaceUsed != null && TotalUserSpaceUsed.Bytes.HasValue)
                {
                    return new FileSize(TotalUserSpaceUsed.Kilobytes);
                }
                return new FileSize();
            }
        }

        #endregion

        #region methods


        internal static SessionStatus ConvertToSessionStatus(string status)
        {
            switch (status.ToLower().TrimEnd(new char[] { ' ' }))
            {
                case "running":
                    return SessionStatus.Running;
                case "runnable":
                    return SessionStatus.Runnable;
                case "sleeping":
                    return SessionStatus.Sleeping;
                case "background":
                    return SessionStatus.Background;
                case "suspended":
                    return SessionStatus.Suspended;
                default:
                    return SessionStatus.Other;
            }
        }

        #endregion


        #region ISerializable Members



        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("application",application);
            info.AddValue("blockedBy",blockedBy);
            info.AddValue("blockingCount",blockingCount);
            info.AddValue("command",command);
            info.AddValue("cpu",cpu);
            info.AddValue("cpuDelta",cpuDelta);
            info.AddValue("database",database);
            info.AddValue("physicalIo",physicalIo);
            info.AddValue("physicalIoDelta",physicalIoDelta);
            info.AddValue("executionContext",executionContext);
            info.AddValue("lastActivity",lastActivity);
            info.AddValue("lastCommand",lastCommand);
            info.AddValue("loggedInSince",loggedInSince);
            info.AddValue("memory",memory);
            info.AddValue("netLibrary",netLibrary);
            info.AddValue("openTransactions",openTransactions);
            info.AddValue("spid",spid);
            info.AddValue("status",status);
            info.AddValue("userName",userName);
            info.AddValue("waitTime",waitTime);
            info.AddValue("workstation",workstation);
            info.AddValue("workstationNetAddress",workstationNetAddress);
            info.AddValue("waitType",waitType);
            info.AddValue("waitResource", waitResource);
            info.AddValue("versionStoreElapsedTime",versionStoreElapsedTime);
            info.AddValue("TransactionIsolationLevel",TransactionIsolationLevel);
            info.AddValue("SessionUserAllocatedTotal",SessionUserAllocatedTotal);
            info.AddValue("SessionUserDeallocatedTotal",SessionUserDeallocatedTotal);
            info.AddValue("TaskUserAllocatedTotal",TaskUserAllocatedTotal);
            info.AddValue("TaskUserDeallocatedTotal",TaskUserDeallocatedTotal);
            info.AddValue("SessionInternalAllocatedTotal",SessionInternalAllocatedTotal);
            info.AddValue("SessionInternalDeallocatedTotal",SessionInternalDeallocatedTotal);
            info.AddValue("TaskInternalAllocatedTotal",TaskInternalAllocatedTotal);
            info.AddValue("TaskInternalDeallocatedTotal",TaskInternalDeallocatedTotal);
        }

        #endregion

        #region IFilterableSession Members

        string IFilterableSession.Application
        {
            get { return Application; }
        }
        string IFilterableSession.Host
        {
            get { return Workstation; }
        }
        string IFilterableSession.Login
        {
            get { return UserName; }
        }

        #endregion

        /// <summary>
        /// Returns an Auditable Entity
        /// </summary>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity()
        {
            AuditableEntity auditableEntity = new AuditableEntity();
            auditableEntity.Name = this.Workstation;

            // Create metadata.
            auditableEntity.AddMetadataProperty("SPID", this.spid.ToString());
            auditableEntity.AddMetadataProperty("Status", this.status.ToString());
            auditableEntity.AddMetadataProperty("User", this.userName);
            auditableEntity.AddMetadataProperty("Session's Workstation", this.workstation);
            auditableEntity.AddMetadataProperty("Application", this.application);

            return auditableEntity;
        }

        /// <summary>
        /// Returns an Auditable Entity based on an oldValue
        /// </summary>
        /// <param name="oldValue"></param>
        /// <returns></returns>
        public AuditableEntity GetAuditableEntity(IAuditable oldValue)
        {
            return this.GetAuditableEntity();
        }
    }

    public interface IFilterableSession
    {
        string Application { get; }
        string Host { get; }
        string Login { get; }
    }
}
