//------------------------------------------------------------------------------
// <copyright file="Enumerations.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace Idera.SQLdm.Common.Snapshots
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Represents the various audit level types supported by SQL Server.
    /// </summary>
    [Serializable]
    public enum AuditLevelType
    {
        /// <summary>
        /// Do not log authentication attempts.
        /// </summary>
        None,

        /// <summary>
        /// Log successful authentication.
        /// </summary>
        Success,

        /// <summary>
        /// Log failed authentication.
        /// </summary>
        Failure,

        /// <summary>
        /// Log all authentication attempts regardless of success or failure.
        /// </summary>
        All,

        /// <summary>
        /// Audit level is unknown.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents possible autogrow settings on database files
    /// </summary>
   [Serializable]
   public enum AutogrowStatus
   {
       /// <summary>
       /// All files of the given type allow autogrow
       /// </summary>
       AutogrowOn,

       /// <summary>
       /// None of the files of the given type allow autogrow
       /// </summary>
       AutogrowOff,

       /// <summary>
       /// There are a mix of files with and without autogrow
       /// </summary>
       Mixed,

       /// <summary>
       /// The autogrow status could not be determined
       /// </summary>
       Unknown
   }

   /// <summary>
   /// Represents possible database backup types
   /// </summary>
   [Serializable]
    public enum BackupType
    {
        Database = (int)'D',
        DatabaseDifferential = (int)'I',
        Log = (int)'L',
        File = (int)'F',
        FileDifferential = (int)'G',
        Partial = (int)'P',
        PartialDifferential = (int)'Q'
    }

    /// <summary>
    /// Represents possible database status states.
    /// </summary>
    [Flags]
    [Serializable]
    public enum DatabaseStatus
    {
        /// <summary>
        /// Indicates that the database is ready for use.
        /// </summary>
        [Description("Normal")]
        Normal = 0,

        ///<summary>
        /// Indicates that the database status is undetermined
        /// Does not match sysdatabases
        /// </summary>
        [Description("Undetermined")]
        Undetermined = 4, // was -1

        ///<summary>
        /// Indicates that the database is a restoring mirror
        /// Does not match sysdatabases
        /// </summary>
        [Description("Restoring Mirror")]
        RestoringMirror = 8, // was 10

        ///<summary>
        /// Indicates that the database is in standby
        /// Does not match sysdatabases
        /// </summary>
        [Description("Standby")]
        Standby = 16, // was 5

        /// <summary>
        /// Indicates that the database is loading.
        /// </summary>
        [Description("Restoring")]
        Loading = 32,

        /// <summary>
        /// Indicates that the database is in a pre-recovery state.
        /// </summary>
        [Description("Recovery Pending")]
        PreRecovery = 64,

        /// <summary>
        /// Indicates that recovery is underway for the database.
        /// </summary>
        [Description("Recovering")]
        Recovering = 128,

        /// <summary>
        /// Indicates that the integrity of the database is suspect.
        /// </summary>
        [Description("Suspect")]
        Suspect = 256,

        /// <summary>
        /// Indicates that the database has been placed offline by a system or user action.
        /// </summary>
        [Description("Offline")]
        Offline = 512,

        ///<summary>    
        /// Indicates that the database is read-only
        /// </summary>
        [Description("Read Only")]
        ReadOnly = 1024,

        /// <summary>
        /// Indicates that the database is in DBO Use Only mode
        /// </summary>
        [Description("DBO Use Only")]
        DboUseOnly = 2048,

        /// <summary>
        /// Indicates that database is in single user mode
        /// </summary>
        [Description("Single User")]
        SingleUser = 4096,

        /// <summary>
        /// Indicates that the database is in a loading, offline, recovering,
        /// or suspect state.
        /// Does not match sysdatabases
        /// </summary>
        [Description("Inaccessible")]
        Inaccessible = 8192, // was 992
            
        /// <summary>
        /// Indicates that emergency mode has been initiated on the database.
        /// </summary>
        [Description("Emergency Mode")]
        EmergencyMode = 32768,

        /// <summary>
        /// Indicates that the secondary replica is 'Read-Intent only'
        /// </summary>
        [Description("Read-Intent only availability replica")]
        ReadOnlyAvailabilityReplica = 65536,

        /// <summary>
        /// Indicates the secondary replica is not readable.
        /// </summary>
        [Description("Unreadable availability replica")]
        UnreadableAvailabilityReplica = 131072,

        /// <summary>
        /// Indicates that the database was cleanly shutdown.
        /// </summary>
        [Description("Cleanly Shutdown")]
        CleanlyShutdown = 1073741824


    }
    
    /// <summary>
    /// Represents the various dependency object types
    /// </summary>
    [Serializable]
    public enum DependencyType
    {

        /// <summary>
        /// After trigger object
        /// </summary>
        AfterTrigger,

        /// <summary>
        /// Check constraint object
        /// </summary>				
        CheckConstraint,

        /// <summary>
        /// Default value object
        /// </summary>							
        Default,

        /// <summary>
        /// First delete trigger object
        /// </summary>											
        FirstDeleteTrigger,

        /// <summary>
        /// First insert trigger object
        /// </summary>	
        FirstInsertTrigger,

        /// <summary>
        /// First update trigger object
        /// </summary>	
        FirstUpdateTrigger,

        /// <summary>
        /// Function object
        /// </summary>	
        Function,

        /// <summary>
        /// Inline table function object
        /// </summary>										
        InlineTableFunction,

        /// <summary>
        /// InsteadOf trigger object
        /// </summary>					
        InsteadOfTrigger,

        /// <summary>
        /// Last delete trigger object
        /// </summary>		
        LastDeleteTrigger,

        /// <summary>
        /// Last insert trigger object
        /// </summary>		
        LastInsertTrigger,

        /// <summary>
        /// Last update trigger object
        /// </summary>		
        LastUpdateTrigger,

        /// <summary>
        /// Primary key object
        /// </summary>		
        PrimaryKey,

        /// <summary>
        /// Startup procedure object
        /// </summary>									
        StartupProcedure,

        /// <summary>
        /// Stored procedure object
        /// </summary>						
        StoredProcedure,

        /// <summary>
        /// Foreign key object
        /// </summary>							
        ForeignKey,

        /// <summary>
        /// Table function object
        /// </summary>									
        TableFunction,

        /// <summary>
        /// Trigger object
        /// </summary>								
        Trigger,

        /// <summary>
        /// View object
        /// </summary>											
        View,

        /// <summary>
        /// Dependency type is unknown.
        /// </summary>
        Unknown
    }


    /// <summary>
    /// Represents the various dependency action categories
    /// </summary>
    [Serializable]
    public enum DependencyAction
    {
        /// <summary>
        /// Integrity dependency action
        /// </summary>
        Integrity,

        /// <summary>
        /// Read dependency action
        /// </summary>
        Read,

        /// <summary>
        /// Select * dependency action
        /// </summary>
        SelectAll,

        /// <summary>
        /// Update dependency action
        /// </summary>
        Update,

        /// <summary>
        /// Update and Select * dependency action
        /// </summary>
        UpdateSelectAll,

        /// <summary>
        /// There is no action to the dependency
        /// </summary>
        None,

        /// <summary>
        /// The dependency action is unknown
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents the two main SQL Server Log message types
    /// </summary>
    [Serializable]
    public enum ErrorLogMessageType
    {
        /// <summary>
        /// Error Message
        /// </summary>
        Error = 1,

        /// <summary>
        /// Warning Message
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Informational Message
        /// </summary>
        Informational = 3
    }

    /// <summary>
    /// Enumeration describing the manner in which the log file was closed
    /// </summary>
    [Serializable]
    public enum ErrorLogTerminationType
    {
        /// <summary>
        /// Abnormal shutdown (no reason given in log)
        /// </summary>
        AbormalShutdown,

        /// <summary>
        /// Normal shutdown (the SQL Server was sucessfully shut down)
        /// </summary>
        NormalShutdown,

        /// <summary>
        /// Log recycle (indicates that the log was cycled at this point)
        /// </summary>
        LogRecycle,

        /// <summary>
        /// Not applicable (applies to the Active log, which has no termination type)
        /// </summary>
        NotApplicable,

        /// <summary>
        /// Undetermined
        /// </summary>
        Undetermined
    }

    /// <summary>
    /// Enumeration describing the various file size denominations
    /// </summary>
    /// <preliminary>This may be removed if not found useful for the FileSize object</preliminary>
    [Serializable]
    public enum FileSizeDenomination
    {
        /// <summary>
        /// Representation of a Byte file size denomination
        /// </summary>
        Byte,

        /// <summary>
        /// Representation of a Kilobyte file size denomination
        /// </summary>
        Kilobyte,

        /// <summary>
        /// Representation of a Megabyte file size denomination
        /// </summary>
        Megabyte,

        /// <summary>
        /// Representation of a Gigabyte file size denomination
        /// </summary>
        Gigabyte,

        /// <summary>
        /// Representation of a Terabyte file size denomination
        /// </summary>
        Terabyte
    }

    /// <summary>
    /// Represents the population status of a full text catalog
    /// </summary>
    [Serializable]
    public enum FullTextSearchPopulationStatus
    {
        Idle = 0,
        FullPopulationInProgress =1,
        Paused = 2,
        Throttled = 3,
        Recoverying = 4,
        Shutdown = 5,
        IncrementalPopulationInProgress = 6,
        BuildingIndex = 7,
        PausedDiskIsFull = 8,
        ChangeTracking = 9
    }
    
    /// <summary>
    /// Represents the run statuses for a job
    /// </summary>
    [Serializable]
    public enum JobRunStatus
    {
        /// <summary>
        /// Job is executing
        /// </summary>
        Executing,

        /// <summary>
        /// Job is waiting for thread
        /// </summary>
        WaitingForThread,

        /// <summary>
        /// Job is between retries
        /// </summary>
        BetweenRetries,

        /// <summary>
        /// Job is not running
        /// </summary>
        NotRunning,

        /// <summary>
        /// Job is suspended
        /// </summary>
        Suspended,

        /// <summary>
        /// Job is waiting for step completion
        /// </summary>
        WaitingForStepCompletion,

        /// <summary>
        /// Job is cleaning up
        /// </summary>
        CleanUp,

        /// <summary>
        /// Job status is unknown
        /// </summary>
        Unknown

    }

    /// <summary>
    /// Represents the various job step run statuses
    /// </summary>
    [Serializable]
    public enum JobStepRunStatus
    {
        /// <summary>
        /// Job Step Failed
        /// </summary>
        Failed,

        /// <summary>
        /// Job Step Succeeded
        /// </summary>
        Succeeded,

        /// <summary>
        /// Job Step Cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// Job Step is Retrying
        /// </summary>
        Retry,

        /// <summary>
        /// Job Step is In Progress
        /// </summary>
        InProgress,

        /// <summary>
        /// Job Step has not been run
        /// </summary>
        NotYetRun,

        /// <summary>
        /// Job Step Status Unknown
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents the various job step run statuses
    /// </summary>
    [Serializable]
    public enum JobStepCompletionStatus
    {
        /// <summary>
        /// Job Step Failed
        /// </summary>
        Failed,

        /// <summary>
        /// Job Step Succeeded
        /// </summary>
        Succeeded,

        /// <summary>
        /// Job Step Cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// Job Step is Retrying
        /// </summary>
        Retry,

        /// <summary>
        /// Job Step Status Unknown
        /// </summary>
        Unknown
    }




    [Serializable]
    public enum JobControlAction
    {
        Start,
        Stop
    }

    
    /// <summary>
    /// Represents possible lock modes.
    /// </summary>
    [Serializable]
    public enum LockMode
    {
        NULL = 1,
        Sch_S = 2,
        Sch_M = 3,
        S = 4,
        U = 5,
        X = 6,
        IS = 7,
        IU = 8,
        IX = 9,
        SIU = 10,
        SIX = 11,
        UIX = 12,
        BU = 13,
        RangeS_S = 14,
        RangeS_U = 15,
        RangeIn_Null = 16,
        RangeIn_S = 17,
        RangeIn_U = 18,
        RangeIn_X = 19,
        RangeX_S = 20,
        RangeX_U = 21,
        RangeX_X = 22,
        Unknown = 23
    }

    /// <summary>
    /// Represents possible locks status states.
    /// </summary>
    [Serializable]
    public enum LockStatus
    {
        Converting = 2,
        Granted = 1,
        Waiting = 3,
        Unknown = 0
    }

    /// <summary>
    /// Represents possible lock types
    /// </summary>
    [Serializable]
    public enum LockType
    {
        /// <summary>
        /// Indicates AllocationUnit-level lock
        /// </summary>
        AllocationUnit,

        /// <summary>
        /// Indicates application-level lock
        /// </summary>
        Application,

        /// <summary>
        /// Indicates database-level lock
        /// </summary>
        Database,

        /// <summary>
        /// Indicates extent-level lock
        /// </summary>
        Extent,
        
        /// <summary>
        /// Indicates file-level lock
        /// </summary>
        File,

        /// <summary>
        /// Indicates HOBT-level lock
        /// </summary>
        HoBT,

        /// <summary>
        /// Indicates index-level lock
        /// </summary>
        Index,
        
        /// <summary>
        /// Indicates key-level lock
        /// </summary>
        Key,
        
        /// <summary>
        /// Indicates metadata-level lock
        /// </summary>
        Metadata,


        /// <summary>
        /// Indicates object-level lock
        /// </summary>
        Object,
        
        /// <summary>
        /// Indicates page-level lock
        /// </summary>
        Page,


        /// <summary>
        /// Indicates row-level lock
        /// </summary>
        RowId,

        /// <summary>
        /// Indicates table-level lock
        /// </summary>
        Table,

        /// <summary>
        /// Indicates lock of unknown type
        /// </summary>
        Unknown

    }

    /// <summary>
    /// Represents the various login mode types supported by SQL Server.
    /// </summary>
    [Serializable]
    public enum LoginModeType
    {
        /// <summary>
        /// Allows mixed mode authentication.
        /// </summary>
        Mixed,

        /// <summary>
        /// Allows only Windows integrated authentication.
        /// </summary>
        WindowsAuthentication,

        /// <summary>
        /// The supported login mode is unknown.
        /// </summary>
        Unknown
    }

    /// <summary>
    /// Represents the different SQL Server log file types
    /// </summary>
    [Serializable]
    public enum LogFileType
    {
        SQLServer,
        Agent
    }

    [Serializable]
    public enum OSMetricsStatus {
        [Description("Available")]
        Available, 
        [Description("Disabled")]
        Disabled, 
        [Description("WMI Service Unreachable")]
        WMIServiceUnreachable,
        [Description("OLE Automation Unavailable")]
        OLEAutomationUnavailable,
        [Description("OLE Automation Unavailable due to Lightweight Pooling")]
        UnavailableDueToLightweightPooling,
        [Description("WMI Service Timedout")]
        WMIServiceTimedOut
    }

    
    
    /// <summary>
    /// Represents database recovery models
    /// </summary>
    [Serializable]
    public enum RecoveryModel
    {
        Full = 1,
        Bulk_Logged = 2,
        Simple = 3
    }
    
    /// <summary>
    /// Represents SQL Server replication status states.
    /// </summary>
    [Serializable]
    public enum ReplicationState
    {
        #region identifiers

        /// <summary>
        /// The SQL Server is a distributor/subscriber.
        /// </summary>
        DistributorSubscriber = 0,

        /// <summary>
        /// Replication is not installed on the SQL Server.
        /// </summary>
        NotInstalled = 1,

        /// <summary>
        /// The SQL Server publishes/subscribes to another SQL Server.
        /// </summary>
        PublishesSubscribes = 2,

        /// <summary>
        /// Replication is running locally.
        /// </summary>
        RunningLocally = 3,

        /// <summary>
        /// Replication is unknown.
        /// </summary>
        Unknown = 4,

        /// <summary>
        /// Replication is installed, but further state information is unknown
        /// </summary>
        Installed = 5,

        /// <summary>
        /// Replication may or may not be installed - we were told not to check
        /// </summary>
        CollectionDisabled = -1

        #endregion
    }

    /// <summary>
    /// Represents the monitored SQL Server services
    /// </summary>
    [Serializable]
    public enum ServiceName
    {
        [Description("SQL Server Agent")]
        Agent = 0,
        [Description("Distributed Transaction Coordinator")]
        DTC = 1,
        [Description("Full-Text Search")]
        FullTextSearch = 2,
        [Description("SQL Server")]
        SqlServer = 3,
        //START: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services
        [Description("SQL Server Browser")]
        Browser = 4,
        [Description("SQL Active Directory Helper")]
        ActiveDirectoryHelper = 5
        //END: SQLdm 9.1 (Abhishek Joshi) -Monitor additonal SQL Server services --added the new SQL Server services
    }

    /// <summary>
    /// Represents various SQL Server service states.
    /// What about Starting, Stopping, and Pausing? See System.ServiceProcess.ServiceControllerStatus
    /// </summary>
    [Serializable]
    public enum ServiceState
    {
        #region identifiers

        /// <summary>
        /// The service is running.
        /// </summary>
        Running,

        /// <summary>
        /// The service is paused.
        /// </summary>
        Paused,

        /// <summary>
        /// The service is stopped.
        /// </summary>
        Stopped,

        /// <summary>
        /// The service is not installed.
        /// </summary>
        [Description("Not Installed")]
        NotInstalled,

        /// <summary>
        /// The service state is undetermined. There is no way to know if the server (service) is down, not installed, etc.
        /// </summary>
        [Description("Unable To Connect")]
        UnableToConnect,

        /// <summary>
        /// The service is starting
        /// </summary>
        StartPending,

        /// <summary>
        /// The service is stopping
        /// </summary>
        StopPending,

        /// <summary>
        /// The service is about to continue
        /// </summary>
        ContinuePending,

        /// <summary>
        /// The service is pausing
        /// </summary>
        PausePending,

        /// <summary>
        /// The service state cannot be monitored
        /// </summary>
        [Description("Unable To Monitor")]
        UnableToMonitor,

        /// <summary>
        /// SQLDM 10.1 Barkha khatri SQLDM-25629 fix-- Service state for non-admin cases
        /// </summary>
        [Description("Truncated functionalities available as user is non-admin")]
        TruncatedFunctionalityAvailable
        #endregion
    }

    /// <summary>
    /// Represents various SQL Server session states
    /// </summary>
    [Serializable]
    public enum SessionStatus
    {
        Running,
        Runnable,
        Sleeping,
        Background,
        Suspended,
        Ended,
        Other
    }

    [Serializable]
    public enum SessionCursorFetchStatus
    {
        [Description("No Cursor")]
        NoCursor,
        [Description("Row Fetched")]
        RowFetched,
        [Description("End Of Result Set")]
        EndOfResultSet,
        [Description("Base Row Invalid")]
        BaseRowInvalid
    }

    /// <summary>
    /// Represents the SQL options that can be set to ON or OFF using the Set statement
    /// </summary>
    [Serializable]
    public enum SqlOptions
    {
        // TODO : Define SQL options
        ansi_defaults,
        ansu_null_dflt_off,
        ansi_null_dflt_on,
        ansi_nulls,
        ansi_padding,
        ansi_warnings,
        arithabort,
        arithignore,
        concat_null_yields_null,
        cursor_close_on_commit,
        disable_def_cnst_chk,
        fmtonly,
        forceplan,
        impliit_transactions,
        nocount,
        noexec,
        numeric_roundabort,
        parseonly,
        quoted_identifier,
        remote_proc_transactions,
        showplan_all,
        showplan_text,
        showplan_xml,
        statistics_io,
        statistics_profile,
        statistics_time,
        statitics_xml,
        xact_abort
    }

    /// <summary>
    /// Represents the transaction isolation level of a process
    /// <remarks>Snapshot is only avilable in SQL 2005</remarks>
    /// </summary>
    [Serializable]
    public enum TransactionIsolation
    {
        [Description("unspecified")]
        unspecified = 0,
        [Description("read uncommitted")]
        read_uncommitted = 1,
        [Description("read committed")]
        read_committed = 2,
        [Description("reapeatable read")]
        reapeatable_read = 3,
        [Description("serializable")]
        serializable = 4,
        [Description("snapshot")]
        snapshot = 5
    }

    [Serializable]
    public enum IncompleteQueryMonitorDataReason
    {
        [Description("No data was returned for the given filter set.")]
        NoData = 0,
        [Description("The Query Monitor trace is not running.")]
        QueryMonitorOff = 1,
        [Description("SQLdm is currently upgrading previously collected Query Monitor data.")]
        QueryMonitorUpgrading = 2
    }


    #region Trace Enumerations

    [Serializable]
    public enum TraceLogicalOperator
    {
        And = 0,
        Or = 1
    }

    [Serializable]
    public enum TraceComparisonOperator
    {
        Equals = 0,
        NotEquals = 1,
        GT = 2,
        LT = 3,
        GTE = 4,
        LTE = 5 ,
        Like = 6,
        NotLike = 7 
    }

    /// <summary>
    /// Represents the possible columns of a SQL trace
    /// </Summary>
    [Serializable]
    public enum TraceColumn
    {

        // Text value dependent on the event class that is captured in the trace.
        TextData = 1,

        // Binary value dependent on the event class captured in the trace.
        BinaryData = 2,

        // ID of the database specified by the USE database statement, or the 
        // default database if no USE database statement is issued for a given connection.
        DatabaseID = 3,

        // System-assigned ID of the transaction.
        TransactionID = 4,

        // Contains the number of the line that contains the error.
        LineNumber = 5,

        // Microsoft Windows user name.
        NTUserName = 6,

        // Windows domain to which the user belongs.
        NTDomainName = 7,

        // Name of the client computer that originated the request.
        HostName = 8,

        // ID assigned by the client computer to the process in which the client application is running.
        ClientProcessID = 9,

        // Name of the client application that created the connection to an instance of SQL Server. 
        // This column is populated with the values passed by the application rather than the displayed name of the program.
        ApplicationName = 10,

        // SQL Server login name of the client.
        LoginName = 11,

        // Server Process ID assigned by SQL Server to the process associated with the client.
        SPID = 12,

        // Amount of elapsed time (in milliseconds) taken by the event. This data column is not populated by the Hash Warning event.
        Duration = 13,

        // Time at which the event started, when available.
        StartTime = 14,

        // Time at which the event ended. This column is not populated for starting event classes, such as SQL:BatchStarting 
        // or SP:Starting. It is also not populated by the Hash Warning event.
        EndTime = 15,

        // Number of logical disk reads performed by the server on behalf of the event.
        // This column is not populated by the Lock:Released event.
        Reads = 16,

        // Number of physical disk writes performed by the server on behalf of the event.
        Writes = 17,

        // Amount of CPU time (in milliseconds) used by the event.
        CPU = 18,

        // Represents the bitmap of permissions; used by Security Auditing.
        Permissions = 19,

        // Severity level of an exception.
        Severity = 20,

        // Type of event subclass. This data column is not populated for all event classes.
        EventSubClass = 21,

        // System-assigned ID of the object.
        ObjectID = 22,

        // Success of the permissions usage attempt; used for auditing.
        Success = 23,

        // ID for the index on the object affected by the event. To determine the index ID for an object, 
        // use the indid column of the sysindexes system table.
        IndexID = 24,

        // Integer value dependent on the event class captured in the trace.
        IntegerData = 25,

        // Name of the instance of SQL Server (either servername or servername\instancename) being traced.
        ServerName = 26,

        // Type of event class being recorded.
        EventClass = 27,

        // Type of object (such as table, function, or stored procedure).
        ObjectType = 28,

        // The nesting level at which this stored procedure is executing. See @@NESTLEVEL (Transact-SQL).
        NestLevel = 29,

        // Server state, in case of an error.
        State = 30,

        // Error number.
        Error = 31,

        // Lock mode of the lock acquired. This column is not populated by the Lock:Released event.
        Mode = 32,

        // Handle of the object referenced in the event.
        Handle = 33,

        // Name of object accessed.
        ObjectName = 34,

        // Name of the database specified in the USE database statement.
        DatabaseName = 35,

        // Logical name of the file name modified.
        FileName = 36,

        // Owner name of the referenced object.
        OwnerName = 37,

        // Name of the database or server-wide role targeted by a statement.
        RoleName = 38,

        // User name of the target of some action.
        TargetUserName = 39,

        // SQL Server database username of the client.
        DBUserName = 40,

        // Security identifier (SID) of the logged-in user.
        LoginSid = 41,

        // Login name of the target of some action.
        TargetLoginName = 42,

        // SID of the login that is the target of some action.
        TargetLoginSid = 43,

        // Column-level permissions status; used by Security Auditing.
        ColumnPermissions = 44,

        // Name of the linked server.
        LinkedServerName = 45,

        // Name of the OLE DB provider.
        ProviderName = 46,

        // Name of the OLE DB method.
        MethodName = 47,

        // Number of rows in the batch.
        RowCounts = 48,

        // ID of the request containing the statement.
        RequestID = 49,

        // A token to describe the current transaction.
        XactSequence = 50,

        // Sequence number for this event.
        EventSequence = 51,

        // bigint value, which is dependent on the event class captured in the trace.
        BigintData1 = 52,

        // bigint value, which is dependent on the event class captured in the trace.
        BigintData2 = 53,

        // GUID value, which is dependent on the event class captured in the trace.
        GUID = 54,

        // Integer value, which is dependent on the event class captured in the trace.
        IntegerData2 = 55,

        // ID of the related object or entity, if available.
        ObjectID2 = 56,

        // Integer value, which is dependent on the event class captured in the trace.
        Type = 57,

        // Type of the object that owns the lock. For lock events only.
        OwnerID = 58,

        // Name of the schema the object is within.
        ParentName = 59,

        // Indicates whether the event occurred on a system process or a user process. 1 = system, 0 = user.
        IsSystem = 60,

        // Starting offset of the statement within the stored procedure or batch.
        Offset = 61,

        // ID of the database in which the source of the object exists.
        SourceDatabaseID = 62,

        // 64-bit hash based on the text of an ad hoc query or the database and object ID of an SQL object. 
        // This value can be passed to sys.dm_exec_sql_text() to retrieve the associated SQL text.
        SqlHandle = 63,

        // The login name of the user who originated the session. For example, if you connect to SQL Server using Login1 and 
        // execute a statement as Login2, SessionLoginName displays Login1, while LoginName displays Login2. 
        // This data column displays both SQL Server and Windows logins.
        SessionLoginName = 64
    }

    /// <summary>
    /// Represents the possible events of a SQL trace
    /// </summary>
    [Serializable]
    public enum TraceEvent
    {
        
        // Occurs when a remote procedure call (RPC) has completed.
        RPCCompleted = 10,

        // Occurs when an RPC has started.
        RPCStarting = 11,

        // Occurs when a Transact-SQL batch has completed.
        SQLBatchCompleted = 12,

        // Occurs when a Transact-SQL batch has started.
        SQLBatchStarting = 13,

        // Occurs when a user successfully logs in to Microsoft SQL Server.
        AuditLogin = 14,

        // Occurs when a user logs out of SQL Server.
        AuditLogout = 15,

        // Occurs when attention events, such as client-interrupt requests or broken client connections, happen.
        Attention = 16,

        // Detects all activity by users connected to SQL Server before the trace started.
        ExistingConnection = 17,

        // Occurs when the SQL Server service state is modified.
        AuditServerStartsAndStops = 18,

        // Tracks Microsoft Distributed Transaction Coordinator (MS DTC) coordinated transactions between two or more databases.
        DTCTransaction = 19,

        // Indicates that a login attempt to SQL Server from a client failed.
        AuditLoginFailed = 20,

        // Indicates that events have been logged in the Microsoft Windows application log.
        EventLog = 21,

        // Indicates that error events have been logged in the SQL Server error log.
        ErrorLog = 22,

        // Indicates that a lock on a resource, such as a page, has been released.
        LockReleased = 23,

        // Indicates acquisition of a lock on a resource, such as a data page.
        LockAcquired = 24,

        // Indicates that two concurrent transactions have deadlocked each other by trying to obtain incompatible locks on resources the other transaction owns.
        LockDeadlock = 25,

        // Indicates that the acquisition of a lock on a resource has been canceled (for example, due to a deadlock).
        LockCancel = 26,

        // Indicates that a request for a lock on a resource, such as a page, has timed out due to another transaction holding a blocking lock on the required resource. Time-out is determined by the @@LOCK_TIMEOUT function, and can be set with the SET LOCK_TIMEOUT statement.
        LockTimeout = 27,

        // Occurs before a SELECT, INSERT, or UPDATE statement is executed.
        DegreeOfParallelismEvent = 28,

        // Indicates that an exception has occurred in SQL Server.
        Exception = 33,

        // Indicates when a stored procedure is not found in the procedure cache.
        SPCacheMiss = 34,

        // Indicates when an item is inserted into the procedure cache.
        SPCacheInsert = 35,

        // Indicates when an item is removed from the procedure cache.
        SPCacheRemove = 36,

        // Indicates that a stored procedure was recompiled.
        SPRecompile = 37,

        // Indicates when a stored procedure is found in the procedure cache.
        SPCacheHit = 38,

        // Occurs when the Transact-SQL statement has started.
        SQLStmtStarting = 40,

        // Occurs when the Transact-SQL statement has completed.
        SQLStmtCompleted = 41,

        // Indicates when the stored procedure has started.
        SPStarting = 42,

        // Indicates when the stored procedure has completed.
        SPCompleted = 43,

        // Indicates that a Transact-SQL statement within a stored procedure has started executing.
        SPStmtStarting = 44,

        // Indicates that a Transact-SQL statement within a stored procedure has finished executing.
        SPStmtCompleted = 45,

        // Indicates that an object has been created, such as for CREATE INDEX, CREATE TABLE, and CREATE DATABASE statements.
        ObjectCreated = 46,

        // Indicates that an object has been deleted, such as in DROP INDEX and DROP TABLE statements.
        ObjectDeleted = 47,

        // Tracks Transact-SQL BEGIN, COMMIT, SAVE, and ROLLBACK TRANSACTION statements.
        SQLTransaction = 50,

        // Indicates when a table or index scan has started.
        ScanStarted = 51,

        // Indicates when a table or index scan has stopped.
        ScanStopped = 52,

        // Indicates when a cursor is opened on a Transact-SQL statement by ODBC, OLE DB, or DB-Library.
        CursorOpen = 53,

        // Tracks when transactions are written to the transaction log.
        TransactionLog = 54,

        // Indicates that a hashing operation (for example, hash join, hash aggregate, hash union, and hash distinct) that is not processing on a buffer partition has reverted to an alternate plan. This can occur because of recursion depth, data skew, trace flags, or bit counting.
        HashWarning = 55,

        // Indicates an automatic updating of index statistics has occurred.
        AutoStats = 58,

        // Produced for each of the events leading up to the deadlock.
        LockDeadlockChain = 59,

        // Indicates that a finer-grained lock has been converted to a coarser-grained lock (for example, a page lock escalated or converted to a table lock).
        LockEscalation = 60,

        // Indicates that an OLE DB error has occurred.
        OLEDBErrors = 61,

        // Indicates any warnings that occurred during the execution of a SQL Server statement or stored procedure.
        ExecutionWarnings = 67,

        // Displays the plan tree of the Transact-SQL statement executed.
        ShowplanTextUnencoded = 68,

        // Indicates sort operations that do not fit into memory. Does not include sort operations involving the creating of indexes; only sort operations within a query (such as an ORDER BY clause used in a SELECT statement).
        SortWarnings = 69,

        // Indicates when a cursor on a Transact-SQL statement is prepared for use by ODBC, OLE DB, or DB-Library.
        CursorPrepare = 70,

        // ODBC, OLE DB, or DB-Library has prepared a Transact-SQL statement or statements for use.
        PrepareSQL = 71,

        // ODBC, OLE DB, or DB-Library has executed a prepared Transact-SQL statement or statements.
        ExecPreparedSQL = 72,

        // ODBC, OLE DB, or DB-Library has unprepared (deleted) a prepared Transact-SQL statement or statements.
        UnprepareSQL = 73,

        // A cursor previously prepared on a Transact-SQL statement by ODBC, OLE DB, or DB-Library is executed.
        CursorExecute = 74,

        // A cursor opened on a Transact-SQL statement by ODBC or DB-Library has been recompiled either directly or due to a schema change.
        CursorRecompile = 75,

        // A cursor on a Transact-SQL statement is converted by SQL Server from one type to another.
        CursorImplicitConversion = 76,

        // A prepared cursor on a Transact-SQL statement is unprepared (deleted) by ODBC, OLE DB, or DB-Library.
        CursorUnprepare = 77,

        // A cursor previously opened on a Transact-SQL statement by ODBC, OLE DB, or DB-Library is closed.
        CursorClose = 78,

        // Column statistics that could have been useful for the optimizer are not available.
        MissingColumnStatistics = 79,

        // Query that has no join predicate is being executed. This could result in a long-running query.
        MissingJoinPredicate = 80,

        // SQL Server memory usage has increased or decreased by either 1 megabyte (MB) or 5 percent of the maximum server memory, whichever is greater.
        ServerMemoryChange = 81,

        // Event data defined by the user.
        // This is used by SQLdm on SQL 2005
        UserConfigurable0 = 82,
            
        // Event data defined by the user.
        UserConfigurable1 = 83,

        // Event data defined by the user.
        UserConfigurable2 = 84,

        // Event data defined by the user.
        UserConfigurable3 = 85,

        // Event data defined by the user.
        UserConfigurable4 = 86,

        // Event data defined by the user.
        UserConfigurable5 = 87,

        // Event data defined by the user.
        UserConfigurable6 = 88,

        // Event data defined by the user.
        UserConfigurable7 = 89,

        // Event data defined by the user.
        UserConfigurable8 = 90,

        // Event data defined by the user.
        UserConfigurable9 = 91,
            
        // Indicates that a data file was extended automatically by the server.
        DataFileAutoGrow = 92,

        // Indicates that a data file was extended automatically by the server.
        LogFileAutoGrow = 93,

        // Indicates that a data file was shrunk automatically by the server.
        DataFileAutoShrink = 94,

        // Indicates that a log file was shrunk automatically by the server.
        LogFileAutoShrink = 95,

        // Displays the query plan tree of the SQL statement from the query optimizer. Note that the TextData column does not contain the Showplan for this event.
        ShowplanText = 96,

        // Displays the query plan with full compile-time details of the SQL statement executed. Note that the TextData column does not contain the Showplan for this event.
        ShowplanAll = 97,

        // Displays the query plan with full run-time details of the SQL statement executed. Note that the TextData column does not contain the Showplan for this event.
        ShowplanStatisticsProfile = 98,

        // Produces output values of the parameters for every RPC.
        RPCOutputParameter = 100,

        // Occurs every time a GRANT, DENY, REVOKE for a statement permission is issued by any user in SQL Server.
        AuditStatementGDREvent = 102,

        // Occurs every time a GRANT, DENY, REVOKE for an object permission is issued by any user in SQL Server.
        AuditObjectGDREvent = 103,

        // Occurs when a SQL Server login is added or removed; for sp_addlogin and sp_droplogin.
        AuditAddLoginEvent = 104,

        // Occurs when a Windows login right is added or removed; for sp_grantlogin, sp_revokelogin, and sp_denylogin.
        AuditLoginGDREvent = 105,

        // Occurs when a property of a login, except passwords, is modified; for sp_defaultdb and sp_defaultlanguage.
        AuditLoginChangePropertyEvent = 106,

        // Occurs when a SQL Server login password is changed.
        AuditLoginChangePasswordEvent = 107,

        // Occurs when a login is added or removed from a fixed server role; for sp_addsrvrolemember, and sp_dropsrvrolemember.
        AuditAddLogintoServerRoleEvent=108,

        // Occurs when a login is added or removed as a database user (Windows or SQL Server) to a database; for sp_grantdbaccess, sp_revokedbaccess, sp_adduser, and sp_dropuser.
        AuditAddDBUserEvent=109,

        // Occurs when a login is added or removed as a database user (fixed or user-defined) to a database; for sp_addrolemember, sp_droprolemember, and sp_changegroup.
        AuditAddMembertoDBRoleEvent=110,

        // Occurs when a login is added or removed as a database user to a database; for sp_addrole and sp_droprole.
        AuditAddRoleEvent=111,

        // Occurs when a password of an application role is changed.
        AuditAppRoleChangePasswordEvent=112,

        // Occurs when a statement permission (such as CREATE TABLE) is used.
        AuditStatementPermissionEvent=113,

        // Occurs when an object permission (such as SELECT) is used, both successfully or unsuccessfully.
        AuditSchemaObjectAccessEvent=114,

        // Occurs when a BACKUP or RESTORE command is issued.
        AuditBackupRestoreEvent=115,

        // Occurs when DBCC commands are issued.
        AuditDBCCEvent=116,

        // Occurs when audit trace modifications are made.
        AuditChangeAuditEvent=117,

        // Occurs when a CREATE, ALTER, and DROP object commands are issued.
        AuditObjectDerivedPermissionEvent=118,

        // Occurs when OLE DB provider calls are made for distributed queries and remote stored procedures.
        OLEDBCallEvent=119,

        // Occurs when OLE DB QueryInterface calls are made for distributed queries and remote stored procedures.
        OLEDBQueryInterfaceEvent=120,

        // Occurs when a data request call is made to the OLE DB provider.
        OLEDBDataReadEvent=121,

        // Occurs when an SQL statement executes. Include this event to identify Showplan operators. Each event is stored in a well-formed XML document. Note that the Binary column for this event contains the encoded Showplan. Use SQL Server Profiler to open the trace and view the Showplan.
        ShowplanXML=122,

        // Occurs when a full text query executes.
        SQLFullTextQuery=123,

        // Reports the progress of a Service Broker conversation.
        BrokerConversation=124,

        // Occurs when you use a feature that will be removed from a future version of SQL Server.
        DeprecationAnnouncement=125,

        // Occurs when you use a feature that will be removed from the next major release of SQL Server.
        DeprecationFinalSupport=126,

        // Occurs when communication buffers in a parallel query plan have been temporarily written to the tempdb database.
        ExchangeSpillEvent=127,

        // Occurs when a database is created, altered, or dropped.
        AuditDatabaseManagementEvent=128,

        // Occurs when a CREATE, ALTER, or DROP statement executes on database objects, such as schemas.
        AuditDatabaseObjectManagementEvent=129,

        // Occurs when principals, such as users, are created, altered, or dropped from a database.
        AuditDatabasePrincipalManagementEvent=130,

        // Occurs when server objects are created, altered, or dropped.
        AuditSchemaObjectManagementEvent=131,

        // Occurs when there is an impersonation within server scope, such as EXECUTE AS LOGIN.
        AuditServerPrincipalImpersonationEvent=132,

        // Occurs when an impersonation occurs within the database scope, such as EXECUTE AS USER or SETUSER.
        AuditDatabasePrincipalImpersonationEvent=133,

        // Occurs when the owner is changed for objects in server scope.
        AuditServerObjectTakeOwnershipEvent=134,

        // Occurs when a change of owner for objects within database scope occurs.
        AuditDatabaseObjectTakeOwnershipEvent=135,

        // Occurs when Service Broker creates a new conversation group or drops an existing conversation group.
        BrokerConversationGroup=136,

        // Occurs when a process has been blocked for more than a specified amount of time. Does not include system processes or processes that are waiting on non deadlock-detectable resources. Use sp_configure to configure the threshold and frequency at which reports are generated.
        BlockedProcessReport=137,

        // Reports the status of a transport connection managed by Service Broker.
        BrokerConnection=138,

        // Occurs when Service Broker forwards a message.
        BrokerForwardedMessageSent=139,

        // Occurs when Service Broker drops a message that was intended to be forwarded.
        BrokerForwardedMessageDropped=140,

        // Occurs when Service Broker determines the routing for a message.
        BrokerMessageClassify=141,

        // Indicates that errors have occurred in the Service Broker transport layer. The error number and state values indicate the source of the error.
        BrokerTransmission=142,

        // Indicates a poison message was detected because there were five consecutive transaction rollbacks on a Service Broker queue. The event contains the database ID and queue ID of the queue that contains the poison message.
        BrokerQueueDisabled=143,

        // Occurs when an SQL statement executes. Identifies the Showplan operators and displays complete, compile-time data. Note that the Binary column for this event contains the encoded Showplan. Use SQL Server Profiler to open the trace and view the Showplan.
        ShowplanXMLStatisticsProfile=146,

        // Occurs when an attempt to acquire a lock is canceled because the attempt was part of a deadlock and was chosen as the deadlock victim. Provides an XML description of a deadlock.
        DeadlockGraph=148,

        // Occurs when Service Broker sends or receives a message acknowledgement.
        BrokerRemoteMessageAcknowledgement=149,

        // Occurs when a trace file closes during rollback.
        TraceFileClose=150,

        // Occurs when ALTER AUTHORIZATION is used to change the owner of a database and permissions are checked to do that.
        AuditChangeDatabaseOwner=152,

        // Occurs when ALTER AUTHORIZATION is used to assign an owner to an object and permissions are checked to do that.
        AuditSchemaObjectTakeOwnershipEvent=153,

        // Occurs when a full-text crawl (population) starts. Use to check if a crawl request is picked up by worker tasks.
        FTCrawlStarted=155,

        // Occurs when a full-text crawl (population) stops. Stops occur when a crawl completes successfully or when a fatal error occurs.
        FTCrawlStopped=156,

        // Occurs when an exception is encountered during a full-text crawl. Usually causes the full-text crawl to stop.
        FTCrawlAborted=157,

        // Reports audit messages related to Service Broker dialog security.
        AuditBrokerConversation=158,

        // Reports audit messages related to Service Broker transport security.
        AuditBrokerLogin=159,

        // Occurs when Service Broker is unable to retain a received message that should have been delivered to a service.
        BrokerMessageUndeliverable=160,

        // Occurs when Service Broker receives a corrupted message.
        BrokerCorruptedMessage=161,

        // Displays error messages that users see in the case of an error or exception.
        UserErrorMessage=162,

        // Occurs when a queue monitor starts an activation stored procedure, sends a QUEUE_ACTIVATION notification, or when an activation stored procedure started by a queue monitor exits.
        BrokerActivation=163,

        // Occurs when a database object is altered.
        ObjectAltered=164,

        // Occurs when a compiled query plan has been cached for the first time, recompiled, or removed from the plan cache.
        Performancestatistics=165,

        // Occurs when a statement-level recompilation occurs.
        SQLStmtRecompile=166,

        // Occurs when the state of a mirrored database changes.
        DatabaseMirroringStateChange=167,

        // Occurs when an SQL statement executes. Displays the complete, compile-time data. Note that the Binary column for this event contains the encoded Showplan. Use SQL Server Profiler to open the trace and view the Showplan.
        ShowplanXMLForQueryCompile=168,

        // Occurs when an SQL statement compiles. Displays complete, compile-time data. Use to identify Showplan operators.
        ShowplanAllForQueryCompile=169,

        // Indicates that a grant, deny, or revoke event for permissions in server scope occurred, such as creating a login.
        AuditServerScopeGDREvent=170,

        // Indicates that a grant, deny, or revoke event for a schema object, such as a table or function, occurred.
        AuditServerObjectGDREvent=171,

        // Indicates that a grant, deny, or revoke event for database objects, such as assemblies and schemas, occurred.
        AuditDatabaseObjectGDREvent=172,

        // Occurs when Security Audit operations such as altering settings, resources, external access, or authorization are used.
        AuditServerOperationEvent=173,

        // Occurs when a statement checks for the ALTER TRACE permission.
        AuditServerAlterTraceEvent=175,

        // Occurs when server objects are created, altered, or dropped.
        AuditServerObjectManagementEvent=176,

        // Occurs when server principals are created, altered, or dropped.
        AuditServerPrincipalManagementEvent=177,

        // Occurs when database operations occur, such as checkpoint or subscribe query notification.
        AuditDatabaseOperationEvent=178,

        // Occurs when database objects, such as schemas, are accessed.
        AuditDatabaseObjectAccessEvent=180,

        // Occurs when a BEGIN TRANSACTION request starts.
        TMBeginTranstarting=181,

        // Occurs when a BEGIN TRANSACTION request completes.
        TMBeginTrancompleted=182,

        // Occurs when a PROMOTE TRANSACTION request starts.
        TMPromoteTranstarting=183,

        // Occurs when a PROMOTE TRANSACTION request completes.
        TMPromoteTrancompleted=184,

        // Occurs when a COMMIT TRANSACTION request starts.
        TMCommitTranstarting=185,

        // Occurs when a COMMIT TRANSACTION request completes.
        TMCommitTrancompleted=186,

        // Occurs when a ROLLBACK TRANSACTION request starts.
        TMRollbackTranstarting=187,

        // Occurs when a ROLLBACK TRANSACTION request completes.
        TMRollbackTrancompleted=188,

        // Occurs when a request for a lock on a resource, such as a page, times out.
        LockTimeoutGT0=189,

        // Reports the progress of an online index build operation while the build process is running.
        ProgressReportOnlineIndexOperation=190,

        // Occurs when a SAVE TRANSACTION request starts.
        TMSaveTranstarting=191,

        // Occurs when a SAVE TRANSACTION request completes.
        TMSaveTrancompleted=192,

        // Occurs when a background job terminates abnormally.
        BackgroundJobError=193,

        // Occurs when a distributed query runs and collects information corresponding to the provider connection.
        OLEDBProviderInformation=194,

        // Occurs when a tape mount request is received.
        MountTape=195,

        // Occurs when a request to load a CLR assembly occurs.
        AssemblyLoad=196,

        // Occurs when an XQuery expression is executed. This event class provides the static type of the XQuery expression.
        XQueryStaticType=198,

        // Occurs when a query registration cannot be subscribed. The TextData column contains information about the event.
        QNsubscription=199,

        // Information about active subscriptions is stored in internal parameter tables. This event class occurs when a parameter table is created or deleted. Typically, these tables are created or deleted when the database is restarted. The TextData column contains information about the event.
        QNparametertable=200,

        // A query template represents a class of subscription queries. Typically, queries in the same class are identical except for their parameter values. This event class occurs when a new subscription request falls into an already existing class of (Match), a new class (Create), or a Drop class, which indicates cleanup of templates for query classes without active subscriptions. The TextData column contains information about the event.
        QNtemplate=201,

        // Tracks internal activities of query notifications. The TextData column contains information about the event.
        QNdynamics=202
    }

    #endregion

    /// <summary>
    /// Represents the worst performing statement types
    /// </summary>
    [Serializable]
    public enum WorstPerformingStatementType
    {
        StoredProcedure,
        SingleStatement,
        Batch
    }

    [Serializable]
    public enum TraceEventType
    {
        Exception = 33,
        StatementStarting = 40,
        StatementCompleted = 41,
        StoredProcedureStarting = 42,
        StoredProcedureCompleted = 43
    }

    [Serializable]
    public enum ContinuousCollectorRunStatus
    {
        NotCreated,
        Created,
        Running,
        Stopped,
        ReadyToDispose,
        Disposed,
        Unknown
    }

    [Serializable]
    public enum ClusterCollectionSetting
    {
        Default = 0,
        ForceClusteredWithRegread = 1
    }

    [Serializable]
    public enum FileActivityFileType
    {
        Data = 0,
        Log = 1,
        Unknown = 2
    }

    [Serializable]
    public enum TableFragmentationCollectorStatus
    {
        Stopped = 0,
        Stopping = 1,
        Starting = 2,
        Running = 3
    }

    /// <summary>
    /// Represents alertable snapshot types
    /// </summary>
    [Flags]
    [Serializable]
    public enum AlertableSnapshotType
    {

        
        ScheduledRefresh = 0,

        
        DatabaseSize = 4

        
    }

    /// <summary>
    /// Represents the states of sys.availability_replicas.failover_mode object, the state None
    /// indicates no valid value, it is not part of sys.availability_replicas.failover_mode
    /// </summary>
    [Serializable]
    public enum FailOverMode
    {
        /// <summary>
        /// No valid value.
        /// </summary>
        [Description("None")]
        None = 255,

        /// <summary>
        /// Manual failover.
        /// </summary>
        [Description("Manual failover")]
        Manual = 1,

        /// <summary>
        /// Automatic failover.
        /// </summary>
        [Description("Automatic failover")]
        Automatic = 0
    }

    /// <summary>
    /// Represent the states for sys.availability_replicas.availability_mode object, the state
    /// None indicates the no valid value, it is not part of sys.availability_replicas.availability_mode
    /// </summary>
    [Serializable]
    public enum AvailabilityMode
    {
        /// <summary>
        /// No valid value.
        /// </summary>
        [Description("None")]
        None = 255,

        /// <summary>
        /// Asynchronous commit.
        /// </summary>
        [Description("Asynchronous commit")]
        Asynchronous = 0,

        /// <summary>
        /// Synchronous commit. 
        /// </summary>
        [Description("Synchronous commit")]
        Synchronous = 1
    }

    [Serializable]
    public enum PrimaryConnectionMode
    {
        [Description("None")]
        None = 0,

        [Description("All")]
        All = 2,

        [Description("Read write")]
        ReadWrite = 3
    }

    [Serializable]
    public enum SecondaryConnectionMode
    {
        [Description("None")]
        None = 255,

        [Description("No")]
        No = 0,

        [Description("Read-Intent only")]
        ReadIntentOnly = 1,

        [Description("Yes")]
        Yes = 2
    }

    [Serializable]
    public enum AlwaysOnSynchronizationState
    {
        [Description("None")]
        None = 255,

        [Description("Not synchronizing")]
        NotSynchronizing = 0,

        [Description("Synchronizing")]
        Synchronizing = 1,

        [Description("Synchronized")]
        Synchronized = 2,

        [Description("Reverting")]
        Reverting = 3,

        [Description("Initializing")]
        Initializing = 4
    }

    [Serializable]
    public enum AlwaysOnSynchronizationHealth
    {
        [Description("None")]
        None = 255,

        [Description("Not healthy")]
        NotHealthy = 0,

        [Description("Partially healthy")]
        PartiallyHealthy = 1,

        [Description("Healthy")]
        Healthy = 2
    }

    [Serializable]
    public enum ReplicaRole
    {
        [Description("None")]
        None = 255,

        [Description("Resolving")]
        Resolving = 0,

        [Description("Primary")]
        Primary = 1,

        [Description("Secondary")]
        Secondary = 2
    }

    [Serializable]
    public enum OperationalState
    {
        [Description("Replica is not local")]
        ReplicaIsNotLocal = 255,

        [Description("Pending failover")]
        PendingFailover = 0,

        [Description("Pending")]
        Pending = 1,

        [Description("Online")]
        Online = 2,

        [Description("Offline")]
        Offline = 3,

        [Description("Failed")]
        Failed =  4,

        [Description("Failed, no quorum")]
        FailedNoQuorum = 5
    }

    [Serializable]
    public enum ConnectedState
    {
        [Description("Disconnected")]
        Disconnected = 0,

        [Description("Connected")]
        Connected = 1
    }

    [Serializable]
    public enum DatabaseStatusNoMaskBit
    {
        [Description("Online")]
        Online = 0,

        [Description("Restoring")]
        Restoring = 1,

        [Description("Recovering")]
        Recovering = 2,

        [Description("Recovery pending")]
        RecoveryPending = 3,

        [Description("Suspect")]
        Suspect = 4,

        [Description("Emergency")]
        Emergency = 5,

        [Description("Offline")]
        Offline = 6
    }
}
