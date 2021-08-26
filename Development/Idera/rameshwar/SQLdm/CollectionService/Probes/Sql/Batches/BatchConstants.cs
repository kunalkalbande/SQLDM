//------------------------------------------------------------------------------
// <copyright file="BatchFinder.cs" company="Idera, Inc.">
//     Copyright (c) Idera, Inc. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
using Idera.SQLdm.Common;
using Idera.SQLdm.Common.Services;


namespace Idera.SQLdm.CollectionService.Probes.Sql.Batches
{
    /// <summary>
    /// Contains constant fields required in batch creation
    /// </summary>
    class BatchConstants
    {
        static CommonAssemblyInfo assemblyInfo = new CommonAssemblyInfo();

        internal static string CopyrightNotice = string.Format("-- SQL Diagnostic Manager v{0}\r\n-- {1} \r\n\r\n", assemblyInfo.GetCommonAssemblyVersion(), assemblyInfo.GetCommonAssemblyCopyright());

        internal static string CustomCounterSQLStatementNotice = string.Format(
            "-- SQL Diagnostic Manager User Defined Batch v{0}\r\n -- The contents of this batch are entirely user-submitted below this notice \r\n \r\n", assemblyInfo.GetCommonAssemblyVersion());

        internal static string SwapBatchNotice = string.Format(
             "-- SQL Diagnostic Manager Swap Batch v{0}\r\n-- The contents of this batch may differ from built-in functionality  \r\n \r\n", assemblyInfo.GetCommonAssemblyVersion());

        internal const string CustomCounterNotice =
            "-- Portions of this batch are created with user-submitted variables\r\n";

        internal const string GetDateTime = "select convert(datetime,getutcdate(),121);\n";

        internal const string GetDateTimeLocal = "select convert(datetime,getdate(),121);\n";

        internal const string GetStartupTime =
            "select convert(datetime,dateadd(hh,datediff(hh,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from master..sysdatabases where dbid = db_id('tempdb') union select startdate = min(login_time) from master..sysprocesses) startdates\r\n";

        internal const string GetTimeAndConnectionCount2000 =
            "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from master..sysdatabases where dbid = db_id('tempdb') union select startdate = min(login_time) from master..sysprocesses) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from master..sysprocesses where program_name like '%SQL Diagnostic Manager Collection%' and hostname = host_name()\r\n";

        internal const string GetTimeAndConnectionCount2005 =
           "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from master..sysdatabases where dbid = db_id('tempdb') union select startdate = min(login_time) from master..sysprocesses) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from master.sys.dm_exec_sessions where program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name()\r\n";

        //START:SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding new batches in case of azure instance
        internal const string GetTimeAndConnectionCount2000_AZURE =
           "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from [{0}]..sysdatabases where lower(name) = 'tempdb' ) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from [{0}].sys.dm_exec_sessions where program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name()\r\n";

        internal const string GetTimeAndConnectionCount2005_AZURE =
           "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from [{0}]..sysdatabases where lower(name) = 'tempdb' ) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from [{0}].sys.dm_exec_sessions where program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name()\r\n";
        //END:SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Adding new batches in case of azure instance

		//SQLDM-31002
        internal const string EndIdleConnections_AZURE =
            "DECLARE @kill varchar(8000) = ''; SELECT @kill = @kill + 'KILL ' + CONVERT(varchar(5), c.session_id) + ';' FROM sys.dm_exec_connections AS c JOIN sys.dm_exec_sessions AS s ON c.session_id = s.session_id and status = 'sleeping' and program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name() ORDER BY c.connect_time ASC; EXEC(@kill)";
        //SQLDM-31002
		
        internal const string BatchTimeout = "set lock_timeout 20000";

        internal const string BatchHeader =
            "set transaction isolation level read uncommitted \r\n " + BatchTimeout + " \r\n set implicit_transactions off \r\n if @@trancount > 0 commit transaction \r\n set language us_english \r\n set cursor_close_on_commit off \r\n set query_governor_cost_limit 0 \r\n set numeric_roundabort off\r\n";

        internal const string GetEdition = "select serverproperty('Edition') ";

        internal const string FullTextFeatureQuery = "SELECT FullTextServiceProperty('IsFullTextInstalled') AS Result";

        internal const string GetMasterCompatibility =
            "select cast(cmptlevel as nvarchar(5)) from master..sysdatabases where dbid = db_id('master')";

        //SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- New SQL Command in case of azure db
        internal const string GetMasterCompatibility_AZURE = "select cast(cmptlevel as nvarchar(5)) from [{0}]..sysdatabases where lower(name) = '{0}'";
        //sqldm -30013 start
        internal const string GetMasterCompatibility_AZURE_MASTER = "select cast(cmptlevel as nvarchar(5)) from sysdatabases where lower(name) = 'master'";
        internal const string GetTimeAndConnectionCount2000_AZURE_MASTER =
   "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from sysdatabases where lower(name) = 'tempdb' ) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from sys.dm_exec_sessions where program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name()\r\n";

        internal const string GetTimeAndConnectionCount2005_AZURE_MASTER =
           "declare @startdate datetime select @startdate = convert(datetime,dateadd(mi,datediff(mi,getdate(),getutcdate()),min(startdate)),121) from (select startdate = crdate from sysdatabases where lower(name) = 'tempdb' ) startdates select UTCDateTime = convert(datetime,getutcdate(),121), LocalDateTime = convert(datetime,getdate(),121), StartDate = @startdate, CollectionServiceProcessCount = count(*) from sys.dm_exec_sessions where program_name like '%SQL Diagnostic Manager Collection%' and host_name = host_name()\r\n";
        //sqldm-30013 end
        internal const string SqlTextCompatibilityMode80 = @"cast(null as nvarchar(4000))";
        internal const string SqlTextCompatibilityMode90 =
            @"case when isnull(net_address, '') = '' then cast(null as nvarchar(max)) else (select case when dmv.objectid is null then cast(max(text) as nvarchar(max)) else cast(null as nvarchar(max)) end from sys.dm_exec_sql_text(a.sql_handle) dmv group by dmv.objectid) end";

        #region Direct WMI

        internal const string GetMachineName = "select ISNULL(serverproperty('MachineName'),'CloudServer')";//SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support:Changed the batch constant to not to break in case of cloud support
        internal const string GetComputerBIOSName = "select ISNULL(serverproperty('ComputerNamePhysicalNetBIOS'),'CloudServer')";//SQLdm 9.1 (Ankit Srivastava) - Rally Defect DE15255 -- New Constant which will fetch the Computer NetBIOS name 

        internal const string InsertDiskDrive = @"sp_executesql @statement=N'
-- SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm
if (select isnull(object_id(''tempdb..Disks''), 0)) = 0  
begin
	create table tempdb..Disks(drive_letter nvarchar(256),unused_size dec(38,0),total_size dec(38,0), hostname sysname)
end
else
	delete from tempdb..Disks where hostname = host_name()
";

        internal const string GetInstanceDrives = "SELECT DISTINCT LEFT(UPPER([physical_name]), 1) FROM sys.master_files";

        internal const string GetInstanceDrives2005 = "SELECT DISTINCT LEFT(UPPER([filename]), 1) FROM master.dbo.sysaltfiles";

        internal const string DeleteOtherDrives = "delete from #disk_drives where drive_letter not in (SELECT DISTINCT LEFT(UPPER([physical_name]), 1) FROM sys.master_files)";

        internal const string DeleteOtherDrives2000 = "delete from #disk_drives where drive_letter not in (SELECT DISTINCT LEFT(UPPER([filename]), 1) FROM master.dbo.sysaltfiles)";

        #endregion

        //SQLdm 9.0 (Ankit Srivastava) --Query Monitoring with Extended Event Session - Added Query batch constants
        #region Query Monitor Extended Events
        /// <summary>
        /// Query Monitor Duration Predicate
        /// {0} - Duration in milliseconds for 2000, microseconds for 2005
        /// </summary>
        internal const string QueryMonitorDurationPredicate =
            " duration >= {0} ";

        /// <summary>
        /// Query Monitor CPU Time Consumed Predicate
        /// {0} - CPU time in milliseconds
        /// </summary>
        internal const string QueryMonitorCPUTimeConsumedPredicate =
            " cpu_time >= {0} ";

        /// <summary>
        /// Query Monitor CPU Time Consumed Predicate
        /// {0} - CPU time in milliseconds
        /// </summary>
        /// <remarks>
        /// SQLDM 10.2.2 - SQLDM-28101 Query information not collected on certain instances through Extended Events. Using SQL Tracing collects the Query statistics.
        /// </remarks>
        internal const string QueryMonitorCPUTimeConsumedPredicate2008 =
            " cpu >= {0} ";

        /// <summary>
        /// Query Monitor Reads Predicate
        /// {0} - Number of logical reads
        /// </summary>
        internal const string QueryMonitorReadsPredicate =
            " Reads >= {0} ";

        /// <summary>
        /// Query Monitor Writes Predicate
        /// {0} - Number of physical writes
        /// </summary>
        internal const string QueryMonitorWritesPredicate =
            " Writes >= {0} "; 

        /// <summary>
        /// Query Monitor TextData filter for 2008
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorTextDataFilter2008 =
            " {0} TextData {1} '{2}'";  // SQLdm 9.1 (Ankit Srivastava) changed the filter name according to the batch

        /// <summary>
        /// Query Monitor TextData filter for 2012 and above
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorTextDataFilter2012 =
            " {0} ISNULL([statement],[batch_text]) {1} '{2}' ";

        //Start - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - new filter formats for SQL 2008 for LIKE and NOT LIKE 
        /// <summary>
        /// Query Monitor Application filter for SQL Server 2008
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorApplicationFilter2008 =
            " {0} ApplicationName {1} '{2}' ";

        /// <summary>
        /// Query Monitor Database filter for SQL Server 2008 
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorDatabaseFilter2008 =
            " {0} db_name(DatabaseID) {1} '{2}' ";
        //End - SQLdm 9.0 (Ankit Srivastava) - Query Monitoring Improvement with Extended Events - new filter formats for SQL 2008 for LIKE and NOT LIKE 

        #endregion

        #region Query Monitor Trace

        /// <summary>
        /// Query Monitor Duration Filter
        /// {0} - Duration in milliseconds for 2000, microseconds for 2005
        /// </summary>
        internal const string QueryMonitorDurationFilter =
            "select @duration_filter = {0} exec sp_trace_setfilter @P1, 13, 0, 2, @duration_filter \n";

        /// <summary>
        /// Query Monitor CPU Time Consumed Filter
        /// {0} - CPU time in milliseconds
        /// </summary>
        internal const string QueryMonitorCPUTimeConsumedFilter =
            "select @cpu_filter = {0} exec sp_trace_setfilter @P1, 18, 0, 2, @cpu_filter \n";

        /// <summary>
        /// Query Monitor Reads Filter
        /// {0} - Number of logical reads
        /// </summary>
        internal const string QueryMonitorReadsFilter =
            "select @reads_filter = {0} exec sp_trace_setfilter @P1, 16, 0, 2, @reads_filter \n";

        /// <summary>
        /// Query Monitor Writes Filter
        /// {0} - Number of physical writes
        /// </summary>
        internal const string QueryMonitorWritesFilter =
            "select @writes_filter = {0} exec sp_trace_setfilter @P1, 17, 0, 2, @writes_filter \n";

        ///// <summary>
        ///// Query Monitor Exclude Profiler Filter
        ///// </summary>
        //internal const string QueryMonitorExcludeProfiler =
        //    "exec sp_trace_setfilter @P1, 10, 0, 1, N'SQLProfiler' \n";

        ///// <summary>
        ///// Query Monitor Exclude SQL Agent Filter
        ///// </summary>
        //internal const string QueryMonitorExcludeSQLAgent =
        //    "exec sp_trace_setfilter @P1, 10, 0, 7, N'SQLAgent%' \n";

        ///// <summary>
        ///// Query Monitor Exclude SQL DMO Filter
        ///// </summary>
        //internal const string QueryMonitorExcludeSQLDMO =
        //    "exec sp_trace_setfilter @P1, 10, 0, 7, N'SQLDMO%' \n";

        ///// <summary>
        ///// Query Monitor Exclude SQLdm
        ///// </summary>
        //internal const string QueryMonitorExcludeSQLdm =
        //    "exec sp_trace_setfilter @P1, 10, 0, 7, N'{0}%'\n";

        /// <summary>
        /// Query Monitor Filter String
        /// 0 - Column ID
        /// 1 - Logical Operator (0 AND 1 OR)
        /// 2 - Comparison Operator (0 = 1 <> 2 > 3 < 4 >= 5 <= 6 LIKE 7 NOT LIKE)  //> 
        /// 3 - String to filter
        /// </summary>
        internal const string QueryMonitorFilterString =
            "exec sp_trace_setfilter @P1, {0}, {1}, {2}, N'{3}' \n";
        /// <summary>
        /// Eventclas 92 and 93 are data and log file autogrow
        /// </summary>
        internal const string SQLTextLengthLimiter =
            "and (datalength(TextData) < {0} and EventClass <> 92 and EventClass <> 93)";

        // Default query monitor settings
        internal const int DefaultQueryMonitorTraceFileSizeMegabytes = 5;
        internal const int DefaultQueryMonitorTraceFileRollover = 3;
        internal const int DefaultQueryMonitorTraceRecordsPerRefresh = 1000;

        #endregion

        #region  QueryStore Constants
        /// <summary>
        /// SQL Procedure / Triggers / Functions Filter based on sys.objects type
        /// </summary>
        internal const string SqlProcedureFilterQs = @"
                    so.type IN(''P'',''PC'',''RF'',''X'',''TA'',''TR'',''AF'',''FN'',''FS'',''FT'',''IF'',''TF'')";

        /// <summary>
        /// SQL Statements Filter based on sys.objects type
        /// </summary>
        internal const string SqlStatementsFilterQs = @"
                    so.type IS NULL";

        /// <summary>
        /// For Top Filter and indexing on Duration
        /// </summary>
        internal const string TopPlanDurationCategoryQs = @"Duration";

        /// <summary>
        /// For Top Filter and indexing on Reads
        /// </summary>
        internal const string TopPlanReadCategoryQs = @"Reads";

        /// <summary>
        /// For Top Filter and indexing on Writes
        /// </summary>
        internal const string TopPlanWriteCategoryQs = @"Writes";

        /// <summary>
        /// For Top Filter and indexing on CPU
        /// </summary>
        internal const string TopPlanCpuCategoryQs = @"CPU";

        /// <summary>
        /// Query Monitor Duration Predicate
        /// {0} - Duration in microseconds
        /// </summary>
        internal const string QueryMonitorDurationPredicateQs =
            " qsrs.last_duration >= {0} ";

        /// <summary>
        /// Query Waits Exclude Waits
        /// {0} - Comma separated Wait Category Int
        /// </summary>
        internal const string QueryWaitsExcludedPredicateQs =
            " qsws.wait_category NOT IN ({0}) ";

        /// <summary>
        /// Query Waits Exclude Waits
        /// {0} - Comma separated Wait Category String
        /// </summary>
        internal const string QueryWaitsExcludedStringPredicateQs =
            " qsws.wait_category_desc NOT IN ({0}) ";

        /// <summary>
        /// ''{0}'' for string format
        /// </summary>
        internal const string FirstParameterInSqlQuotes = "''{0}''";

        /// <summary>
        /// Top Wait Filter on row count
        /// </summary>
        internal const string TopWaitFilterQs = "TOP ({0})";

        /// <summary>
        /// ''{0}'', for string format
        /// </summary>
        internal const string FirstParameterInSqlQuotesWithComma = "''{0}'',";

        /// <summary>
        /// {0} for string format
        /// </summary>
        internal const string FirstParameter = "{0}";

        /// <summary>
        /// {0}, for string format
        /// </summary>
        internal const string FirstParameterWithComma = "{0},";

        /// <summary>
        /// Query Monitor CPU Time Consumed Predicate
        /// {0} - CPU time in microseconds
        /// </summary>
        internal const string QueryMonitorCpuTimeConsumedPredicateQs =
            " qsrs.last_cpu_time >= {0} ";

        /// <summary>
        /// Query Monitor Reads Predicate
        /// {0} - Number of logical reads 
        /// </summary>
        /// <remarks>
        /// Last number of logical IO reads for the query plan within the aggregation interval. (expressed as a number of 8KB pages read).
        /// </remarks>
        internal const string QueryMonitorReadsPredicateQs =
            " qsrs.last_logical_io_reads >= {0} ";

        /// <summary>
        /// Query Monitor Writes Predicate
        /// {0} - Number of physical writes
        /// </summary>
        /// <remarks>
        /// Last number of logical IO writes for the query plan within the aggregation interval.
        /// </remarks>
        internal const string QueryMonitorWritesPredicateQs =
            " qsrs.last_logical_io_writes >= {0} ";


        /// <summary>
        /// Query Monitor TextData filter for Query Store
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorTextDataFilterQs =
            " {0} qsqt.query_sql_text {1} ''{2}''";

        /// <summary>
        /// SQL Text Length Limiter
        /// </summary>
        internal const string SqlTextLengthLimiterQs =
            " DATALENGTH(qsqt.query_sql_text) < {0} ";

        /// <summary>
        /// Allow Not Null Sql Text Only
        /// </summary>
        internal const string SqlTextNotNullConditionQs =
            " (qsqt.query_sql_text IS NOT NULL) ";
        
        /// <summary>
        /// Query Monitor Application filter for Query Store
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        internal const string QueryMonitorApplicationFilterQs =
            " {0} des.program_name {1} ''{2}'' ";

        /// <summary>
        /// Query Monitor Database filter for Query Store
        /// {0} - Logical operator
        /// {1} - comparison operator
        /// {2} - value
        /// </summary>
        /// <remarks>
        /// Inside if condition when looping through each database since query store is on db level
        /// </remarks>
        internal const string QueryMonitorDatabaseFilterQs =
            " {0} ''?'' {1} ''{2}'' ";

        /// <summary>
        /// Always true filter required for database filter if no filtering required
        /// </summary>
        internal const string QueryMonitorAlwaysTrueFilterQs = "1=1";

        /// <summary>
        /// Passed when need to collect actual plan
        /// </summary>
        internal const string CollectActualPlanTrueQs = "1";

        /// <summary>
        /// Passed when need to collect estimated plan
        /// </summary>
        internal const string CollectActualPlanFalseQs = "0";

        /// <summary>
        /// Resume from last execution time stored as state in temp table
        /// </summary>
        internal const string QueryMonitorStateCheckQs = " WHERE (@LastStartTime IS NULL OR qsrs.last_execution_time > @LastStartTime) ";
        
		/// <summary>
        /// Resume from last execution time stored as state in temp table
        /// </summary>
        internal const string QueryWaitsStateCheckQs = " WHERE DATEADD(microsecond, -qsrs.last_duration,qsrs.last_execution_time) > @LastStartTime ";

        /// <summary>
        /// Exclude System Filter
        /// </summary>
        internal const string QueryWaitsExcludeSystemFilterQs = " WHERE des.is_user_process <> 0  AND des.session_id >= 50 ";

        /// <summary>
        /// Query Waits Duration Filter >= 50 ms
        /// </summary>
        internal const string QueryWaitsDurationFilterQs = " qsws.last_query_wait_time_ms >= 50 ";

        /// <summary>
        /// Space Connector " "
        /// </summary>
        internal const string SpaceConnector = " ";

        /// <summary>
        /// AndConnector = " AND "
        /// </summary>
        internal const string AndConnector = " AND ";

        /// <summary>
        /// NotConnector = " <> "
        /// </summary>
        internal const string NotConnector = " <> ";

        /// <summary>
        /// NotLikeConnector = " NOT LIKE "
        /// </summary>
        internal const string NotLikeConnector = " NOT LIKE ";

        /// <summary>
        /// LeftParenthesisConnector = " ( "
        /// </summary>
        internal const string LeftParenthesisConnector = " ( ";

        /// <summary>
        /// OrConnector = " OR "
        /// </summary>
        internal const string OrConnector = " OR ";

        /// <summary>
        /// EqualsConnector = " = "
        /// </summary>
        internal const string EqualsConnector = " = ";

        /// <summary>
        /// LikeConnector = " LIKE "
        /// </summary>
        internal const string LikeConnector = " LIKE ";

        /// <summary>
        /// RightParenthesisConnector = " ) "
        /// </summary>
        internal const string RightParenthesisConnector = " ) ";

        /// <summary>
        /// Conditional Where Statement Connector " WHERE "
        /// </summary>
        internal const string WhereConnector = " WHERE ";

        /// <summary>
        /// Used in Like and Not Like Filters
        /// </summary>
        internal const char PercentageChar = '%';

        /// <summary>
        /// For excluding SQLdm
        /// </summary>
        internal const string DiagnosticManExcludeFilter = "DiagnosticMan";

        /// <summary>
        /// Denotes Top Filter and indexing on Duration
        /// </summary>
        internal const int ConfigTopPlanDurationCategoryFilter = 0;

        /// <summary>
        /// Denotes Top Filter and indexing on Read
        /// </summary>
        internal const int ConfigTopPlanReadCategoryFilter = 1;

        /// <summary>
        /// Denotes Top Filter and indexing on CPU
        /// </summary>
        internal const int ConfigTopPlanCpuCategoryFilter = 2;

        /// <summary>
        /// Denotes Top Filter and indexing on Write
        /// </summary>
        internal const int ConfigTopPlanWriteCategoryFilter = 3;

        #endregion

        #region Sessions Segments

        /// <summary>
        /// Exclude System Processes
        /// </summary>
        internal const string ExcludeSystemProcesses =
            "and datalength(rtrim(a.net_address)) <> 0 \n";

        internal const string ExcludeSystemProcesses2005 =
            "and datalength(rtrim(client_net_address)) <> 0 \n";

        internal const string ExcludeDMProcesses =
            "and a.program_name <> 'DiagnosticMan' and a.program_name <> 'SQL diagnostic manager' and a.program_name not like '{0}%' \n";

        internal const string ExcludeDMProcesses2005 =
            "and isnull(sess.program_name,'system') <> 'DiagnosticMan' and isnull(sess.program_name,'system') <> 'SQL diagnostic manager' and isnull(sess.program_name,'system') not like '{0}%' \n";

        internal const string IncludeActiveProcesses =
            "and rtrim(a.status) <> 'sleeping' \n";

        internal const string IncludeActiveProcesses2005 =
            "and rtrim(isnull(req.status,sess.status)) <> 'sleeping' \n";

        internal const string IncludeBlockedProcesses =
            "and a.blocked <> 0 \n";

        internal const string IncludeBlockedProcesses2005 =
            "and isnull(req.blocking_session_id,0) <> 0 \n";

        internal const string IncludeBlockingBlockedProcesses =
            "and a.blocked <> 0 and exists (Select * from master..sysprocesses b (nolock) where b.blocked = a.spid) \n";

        internal const string IncludeBlockingBlockedProcesses2005 =
           "and isnull(req.blocking_session_id,0) <> 0 and block_count > 0 \n";

        internal const string IncludeBlockingOrBlockedProcesses =
            "and a.blocked <> 0 or exists (Select * from master..sysprocesses b (nolock) where b.blocked = a.spid) \n";

        internal const string IncludeBlockingOrBlockedProcesses2005 =
            "and isnull(req.blocking_session_id,0) <> 0 or block_count > 0 \n";

        internal const string IncludeLeadBlockers =
            "and a.blocked = 0 and exists (Select * from master..sysprocesses b (nolock) where b.blocked = a.spid) \n";

        internal const string IncludeLeadBlockers2005 =
            "and isnull(req.blocking_session_id,0)  = 0 and block_count > 0  \n";

        internal const string IncludeLockingProcesses =
            "and exists (Select * from master..syslockinfo l (nolock) where l.req_spid = a.spid) \n";

        internal const string IncludeLockingProcesses2005 =
    "";

        internal const string IncludeNonSharedLockingProcesses =
            "and exists (Select * from master..syslockinfo l (nolock) where l.req_spid = a.spid and req_mode not in (8,3)) \n";

        internal const string IncludeNonSharedLockingProcesses2005 =
            "";


        internal const string IncludeProcessesWithOpenTransactions =
            "and open_tran >= 1 \n";

        internal const string IncludeProcessesConsumingCPU =
            "and a.cpu > 0 \n";

        internal const string IncludeProcessesConsumingCPU2005 =
         "and sess.cpu_time > 0 \n";

        internal const string IncludeProcessesSearchTerm =
            "and (a.spid like '{0}' or lower(a.loginame) like lower('{0}') or lower(a.hostname) like lower('{0}') or lower(a.program_name) like lower('{0}') or lower(db_name(a.dbid)) like lower('{0}')) \n";



        internal const string IncludeProcessesSearchTerm2005 =
            "and sess.session_id like '{0}' or lower(sess.login_name) like lower('{0}') or lower(sess.host_name) like lower('{0}') or lower(sess.program_name) like lower('{0}') or lower(coalesce(db_name(req.database_id),db_name(ot.database_id))) like lower('{0}')";

        internal const string IncludeTempdbAffecting = "and (elapsed_time_seconds > 0 or usingTempdb > 0)";


        internal const string KillSession =
            "kill {0}";


        #endregion

        #region session details

        internal const string SessionDetailsCompatibilityMode80 = @"EventInfo";
        internal const string SessionDetailsCompatibilityMode90 =
            @"case when isnull(net_address, '') = '' then EventInfo else (select case when dmv.objectid is null then cast(max(text) as nvarchar(max)) else EventInfo end from sys.dm_exec_sql_text(a.sql_handle) dmv group by dmv.objectid) end";


        #endregion

        #region Locks

        internal const string TempdbLockFilter2000 = "and rsc_dbid <> db_id('tempdb') ";
        internal const string TempdbLockFilter2005 = "and resource_database_id <> db_id('tempdb') ";

        internal const string HideSharedLocks2000 = "and req_mode <> 3 ";
        internal const string HideSharedLocks2005 = "and lower(request_mode) <> 's' ";


        internal const string FilterForSpid2000 =
            "		req_spid = {0}";
        internal const string FilterForSpid2005 =
            "		and request_session_id = {0}";

        internal const string FilterForBlocking2000 =
            "	and cast(coalesce(p2.spid,0) as bit) > 0";
        internal const string FilterForBlocking2005 =
            "	and cast(coalesce(w2.session_id,0) as bit) > 0";

        internal const string FilterForBlocked2000 =
            "	and coalesce(p.blocked,0) > 0";
        internal const string FilterForBlocked2005 =
            "	and coalesce(w.blocking_session_id,0) > 0";

        internal const string FilterForBlockedandBlocking2000 =
           "	and cast(coalesce(p2.spid,0) as bit) > 0 or coalesce(p.blocked,0) > 0 ";
        internal const string FilterForBlockedandBlocking2005 =
                   "	and cast(coalesce(w2.session_id,0) as bit) > 0 or coalesce(w.blocking_session_id,0) > 0 ";

        internal const string LockDetailsSqlTextCompatibilityMode80 =
            @"insert into @LockCommands
	select distinct 
		spid,
		null
	from
		#Locks l";

        internal const string LockDetailsSqlTextCompatibilityMode90 =
            @"insert into @LockCommands
	select distinct 
		spid,
		case when t.objectid is null then cast(t.text as nvarchar(max)) else null end
	from
		#Locks l
	left join sys.dm_exec_requests r
		on l.spid = r.session_id
	left join sys.dm_exec_connections c
		on l.spid = c.session_id
	outer apply sys.dm_exec_sql_text(coalesce(r.sql_handle,c.most_recent_sql_handle)) as t 
	where
		l.userprocess = 1";


        #endregion

        #region Configuration

        internal const string Reconfigure = "exec sp_configure '{0}','{1}' \n reconfigure with override\n\n";

        #endregion

        #region Shut Down Server

        internal const string ShutdownSQLServer = "shutdown";
        internal const string ShutdownSQLServerWithNoWait = "shutdown with nowait";

        #endregion

        #region Recyle Error Logs

        internal const string RecycleLog = "DBCC ERRORLOG ";

        internal const string RecycleAgentLog = "exec msdb.dbo.sp_cycle_agent_errorlog ";

        #endregion

        #region Change Number of Error Logs

        internal const string SetLimitedLogs =
            "exec master..xp_instance_regwrite N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'NumErrorLogs', REG_DWORD, {0} \n";

        internal const string SetUnlimitedLogs =
            "exec master..xp_instance_regdeletevalue N'HKEY_LOCAL_MACHINE', N'Software\\Microsoft\\MSSQLServer\\MSSQLServer', N'NumErrorLogs' \n";

        #endregion

        #region Error Log

        internal const string SqlErrorLog2000 = "exec xp_readerrorlog {0} \n";

        internal const string SqlErrorLog2005Header =
            "declare @templog table(LogDate datetime,ProcessInfo nvarchar(512), Text nvarchar(4000))\n";

        internal const string SqlErrorLog2005 =
            @"insert into @templog exec xp_readerrorlog {0}
select * from @templog where LogDate between {1} and {2}
delete from @templog
";

        internal const string AgentLog2005 =
            @"insert into @templog execute master.dbo.xp_readerrorlog {0},2,null,null,null,null,N'asc'; 
select * from @templog where LogDate between {1} and {2}
delete from @templog
";
        internal const string RDSSqlErrorLog2005Header =
           "declare @templog table(LogDate datetime,ProcessInfo nvarchar(512), Text nvarchar(4000))\n";

        internal const string RDSSqlErrorLog2005 =
            @"insert into @templog exec rdsadmin.dbo.rds_read_error_log {0}
select * from @templog where LogDate between {1} and {2}
delete from @templog
";

        internal const string RDSAgentLog2005 =
            @"insert into @templog execute rdsadmin.dbo.rds_read_error_log {0},2,null,null,null,null,N'asc'; 
select * from @templog where LogDate between {1} and {2}
delete from @templog
";

        internal const string AgentLog2000Header =
@"set nocount on 
declare @errorlog_path  nvarchar(4000) 
execute master.dbo.xp_instance_regread N'hkey_local_machine', N'software\microsoft\mssqlserver\sqlserveragent', N'errorlogfile',@errorlog_path output,N'no_output' 
";

        internal const string AgentLog2000Segment =
@"if (@errorlog_path is not null) 
begin 
    set @errorlog_path = left(@errorlog_path,len(@errorlog_path) - charindex('\',reverse(@errorlog_path))) + '\' + '{0}' 
    exec master..xp_readerrorlog -1, @errorlog_path  
end 
else
begin
	select 'not available'
end 
";

        internal const string LogScanVariableSegment =
            @"-- SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm
if (select isnull(object_id('tempdb..LogScanVars'), 0)) = 0  
begin
	create table tempdb..LogScanVars(StartDateTime datetime, hostname sysname)
end
else
	delete from tempdb..LogScanVars where hostname = host_name()

insert into tempdb..LogScanVars(StartDateTime,hostname) values ('{0}', host_name())";

        internal const string LogScanVariableSegmentUnsupportedTempdb =
            @"-- SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm
-- Unsupported table creation into tempdb database";


        #endregion

        #region free procedure cache

        internal const string FreeProcCache = "dbcc freeproccache \n";

        #endregion

        #region DbOwner Command

        internal const string DbOwnerPermissionsCommand2000 = @"
PRINT [{0}] 
SELECT 'DbOwner' AS PermissionName, 1 AS PermissionValue
";

        internal const string DbOwnerPermissionsCommand = @"
PRINT [{0}] 
SELECT 'DbOwner' AS PermissionName, IS_ROLEMEMBER('DB_OWNER') AS PermissionValue
";


        #endregion

        #region Job Filters

        internal const string GetCategoryJobStepList = "select c.name, sj.name, js.step_name from msdb..syscategories c left join msdb..sysjobs sj on c.category_id = sj.category_id left join msdb..sysjobsteps js on sj.job_id = js.job_id where sj.name is not null order by c.category_id, sj.job_id, js.step_id";

        internal const string CategoryEquals = "c.name collate SQL_Latin1_General_CP1_CI_AS = '{0}'";
        internal const string CategoryLike = "c.name collate SQL_Latin1_General_CP1_CI_AS like '{0}'";

        internal const string JobNameEquals = "j.name collate SQL_Latin1_General_CP1_CI_AS = '{0}'";
        internal const string JobNameLike = "j.name collate SQL_Latin1_General_CP1_CI_AS like '{0}'";

        internal const string IncludeStep0 = "h.step_id = 0 or ";
        internal const string StepNameEquals = "h.step_name collate SQL_Latin1_General_CP1_CI_AS = '{0}'";
        internal const string StepNameLike = "h.step_name collate SQL_Latin1_General_CP1_CI_AS like '{0}'";

        internal const string LJIncludeStep0 = "s.step_id = 0 or ";
        internal const string LJStepNameEquals = "s.step_id = 0 or s.step_name collate SQL_Latin1_General_CP1_CI_AS = '{0}'";
        internal const string LJStepNameLike = "s.step_id = 0 or s.step_name collate SQL_Latin1_General_CP1_CI_AS like '{0}'";

        internal const string IncludeJobSteps = " and h.step_id = h1.step_id ";

        #endregion

        #region Long and Bombed Jobs

        internal const string CategoryFilter = " and c.name collate SQL_Latin1_General_CP1_CI_AS not in ({0}) ";
        internal const string CategoryLikeFilter = " and c.name collate SQL_Latin1_General_CP1_CI_AS not like ('{0}') ";

        internal const string JobNameFilter = " and j.name collate SQL_Latin1_General_CP1_CI_AS not in ({0}) ";
        internal const string JobNameLikeFilter = " and j.name collate SQL_Latin1_General_CP1_CI_AS not like ('{0}') ";

        internal const string OnFailAction = " and s.on_fail_action = 2 ";
        internal const string JobNotInTempTable = " and h.job_id not in (select [job id] from #tempfailedjobs) ";

        internal const string LongJobIncludeSteps1 = " and h.step_id = r.current_step ";
        internal const string LongJobIncludeSteps2 = " and a.step_id = r.current_step ";
        internal const string LongJobStep0Only = " and h.step_id = 0 ";

        #endregion

        #region Resource Check

        internal const string ResetResourceCheckTable =
            "if (select isnull(object_id('tempdb..Resource_Check'), 0)) <> 0 \n delete from tempdb..Resource_Check where collectionservice = host_name() \n";


        internal const string ResourceCheckInputbufferCompatibilityMode80 = @"commandtext COLLATE DATABASE_DEFAULT";
        internal const string ResourceCheckInputbufferCompatibilityMode90 = @"case when isnull(net_address, '') = '' then commandtext COLLATE DATABASE_DEFAULT else (select case when dmv.objectid is null then left(max(text),4000) else commandtext COLLATE DATABASE_DEFAULT end from sys.dm_exec_sql_text(a.sql_handle) dmv group by dmv.objectid) end";

        #endregion

        #region database probes

        internal const string ExcludeSystemDatabases = "and name not in ('master','model','tempdb','msdb') and category & 16 <> 16\n";
        internal const string ExcludeSystemDatabasesSysDatabases2005 = "and name not in ('master','model','tempdb','msdb') and is_distributor <> 1\n";
        internal const string IncludePresentFileBackupsOnly2005 = " and f.is_present = 1 ";

        internal const string SessionCountEnabled =
            "Processes = (select isnull(count(*),0) from master..sysprocesses where dbid = ' + cast(@dbid as nvarchar(100))  + ' and spid <> ' + cast(@@spid as nvarchar(100)) + '), '";
        internal const string SessionCountDisabled = "Processes = null,'";

        internal const string OldestOpenTransaction =
            @"
		set @command = 'DBCC OPENTRAN(' + quotename(@dbname)   + ') WITH TABLERESULTS'

        IF ((IS_SRVROLEMEMBER('sysadmin') = 1) OR (IS_ROLEMEMBER('db_owner') = 1))
        BEGIN
		insert into #Open_Tran 
			execute (@command)
        END

		if (select count(*) from #Open_Tran) > 0
		begin

		select 
			'OLDACT_STARTTIME',
			dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, RecValue) )
		from 
			#Open_Tran 
		where RecID = 'OLDACT_STARTTIME' 

		select
			'OLDACT_SPID',
			convert(int, replace(RecValue,'s',''))
		from
			#Open_Tran
		where RecID = 'OLDACT_SPID' 
		end

		truncate table #Open_Tran";

        internal const string OldestOpenTransaction2000 =
            @"
		set @command = 'DBCC OPENTRAN(' + quotename(@dbname)   + ') WITH TABLERESULTS'
		insert into #Open_Tran 
			execute (@command)
		if (select count(*) from #Open_Tran) > 0
		begin
		select 
			'OLDACT_STARTTIME',
			dateadd(mi,datediff(mi,getdate(),getutcdate()),convert(datetime, RecValue) )
		from 
			#Open_Tran 
		where RecID = 'OLDACT_STARTTIME' 
		select
			'OLDACT_SPID',
			convert(int, replace(RecValue,'s',''))
		from
			#Open_Tran
		where RecID = 'OLDACT_SPID' 
		end
		truncate table #Open_Tran";
        internal const string SpecifiedDiskSizeRDS = @"
IF OBJECT_ID('master.sys.master_files', 'V') IS NOT NULL
BEGIN
        update #disk_drives set unused_size = (SELECT DISTINCT dovs.available_bytes FROM sys.master_files mf CROSS APPLY sys.dm_os_volume_stats(mf.database_id, mf.FILE_ID) dovs)
		update #disk_drives set total_size = (SELECT DISTINCT dovs.total_bytes FROM sys.master_files mf CROSS APPLY sys.dm_os_volume_stats(mf.database_id, mf.FILE_ID) dovs)
END
";
        internal const string SpecifiedDiskDrivesSegment =
            @"declare @SpecifiedDrives table(drivename nvarchar(256))

insert into @SpecifiedDrives
{0}

delete from #disk_drives where lower(drive_letter)  {1} not in (select lower(drivename)  {1} from @SpecifiedDrives)

insert into #disk_drives(drive_letter)
	select drivename from @SpecifiedDrives
	where lower(drivename) {1} not in (select lower(drive_letter) {1} from #disk_drives)
";
        internal const string MountPointPathSegment2005 =
           @"select distinct left(physical_name,len(physical_name) - charindex('\',reverse(physical_name)))
from
master.sys.master_files
where {0}
union
select distinct left(physical_name, 1)
from
master.sys.master_files
where {1}
";


        internal const string MountPointPathSegment2000 =
           @"select distinct left(filename,len(rtrim(filename)) - charindex('\',reverse(rtrim(filename))))
from
master..sysaltfiles
where {0}
union
select distinct left(filename, 1)
from
master..sysaltfiles
where {1}
";

        internal const string MountPointPathWhereSegment2005 = @"lower(physical_name) {1} like '{0}\%'";

        internal const string MountPointPathWhereSegment2000 = @"lower(filename) {1} like '{0}\%'";

        internal const string FileActivitySpecifiedDrivesSegment = @"insert into #disk_drives(drive_letter)
{0}";

        internal const string CookedDiskDriveSegment =
            @"
                -- Cooked Disk Drive Segment
			
						exec sp_OADestroy @CounterObject
						
						select @PercentFreeSpace = null, @PercentIdleTime = null, @PercentIdleTime_Base = null, @TotalDiskSizeBytes = null

						select @CounterName = 'Win32_PerfFormattedData_PerfDisk_LogicalDisk.Name=''' +  rtrim(case when charindex(':',@driveletter) > 0 then @driveletter else @driveletter + ':' end) + '''' 

						exec sp_OAMethod @WmiService, 
		 					'Get', 
		 					@CounterObject output, 
		 					@CounterName

						exec sp_OAMethod @CounterObject, 'Refresh_'

						waitfor delay '{0}'

						exec sp_OAMethod @CounterObject, 'Refresh_'
			
						exec sp_OAGetProperty @CounterObject, 
							'PercentFreeSpace', 
							@PercentFreeSpace output 

						if (isnull(cast(@PercentIdleTime_Base as float),0) = 0)
						begin
							exec sp_OAGetProperty @CounterObject, 
								'PercentIdleTime', 
								@PercentIdleTime output 
							set @PercentIdleTime_Base = -1
						end

						select @TotalDiskSizeBytes = cast(cast(@FreeDiskSizeBytes as dec(38,18)) / nullif((cast(@PercentFreeSpace as float) / 100 ),0) as dec(38,18))

";

        #endregion

        #region Table Summary

        internal const string TableTypeFilter = "where issystemtable = {0} \n";  // 0 = user, 1 = system

        #endregion

        #region Agent Jobs

        internal const string FilterForRunningJobs = "@execution_status = 0 \n";
        internal const string FilterForJobId = "and sj.job_id in ({0}) \n";

        #endregion

        #region Replication

        internal const string AlternateDistributionStatus = "exec sp_get_distributor ";
        internal const string PublicationCounters = "declare @dbName nvarchar(255) select @dbName = quotename(N'{0}') exec(N'master..sp_replcounters ' + @dbName)";
        internal const string SubscriptionsEnum = "declare @dbName nvarchar(255) select @dbName = quotename(N'{0}') exec(@dbName + N'..sp_MSenumsubscriptions ''both''')";
        internal const string MergeSubscriptions = "declare @dbName nvarchar(255) select @dbName = quotename(N'{0}') exec(@dbName + N'..sp_MSenum_merge_subscriptions @publisher = N''{1}'', @publisher_db = N''{2}'', @publication = N''{3}''')";
        #endregion

        #region Service Control

        internal const string ServiceControlCommand = "exec master..xp_servicecontrol N'{0}',N'{1}'\n";
        internal const string ServiceStart = "Start";
        internal const string ServiceStop = "Stop";
        internal const string ServicePause = "Pause";
        internal const string ServiceContinue = "Continue";


        #endregion

        #region FullText actions

        internal const string FullTextAction = "PRINT [{0}] \n exec dbo.sp_fulltext_catalog @ftcat='{1}',@action={2} \n";
        internal const string FullTextRebuild = "rebuild";
        internal const string FullTextRepopulate = "start_full";
        internal const string FullTextOptimize = "PRINT [{0}] \n alter fulltext catalog [{1}] reorganize \n";

        #endregion

        #region Start/Stop Agent Jobs

        internal const string StartAgentJob = "exec msdb..sp_start_job @job_name = '{0}'";
        internal const string StopAgentJob = "exec msdb..sp_stop_job @job_name = '{0}'";

        #endregion

        #region Session Filters

        internal const string SessionLike = " and rtrim({0}) collate SQL_Latin1_General_CP1_CI_AS not like '{1}' ";
        internal const string SessionMatch = " and rtrim({0}) collate SQL_Latin1_General_CP1_CI_AS not in ({1}) ";
        internal const string SessionApp = "a.program_name";
        internal const string SessionHost = "a.hostname";
        internal const string SessionUser = "a.loginame";

        #endregion

        #region blocking check

        internal const string BlockingCheckInputbufferCompatibilityMode80 = @"cast(null as nvarchar(4000))";
        internal const string BlockingCheckInputbufferCompatibilityMode90 = @"case when isnull(p.net_address, '') = '' then null else (select case when dmv.objectid is null then left(max(text),4000) else null end from sys.dm_exec_sql_text(p.sql_handle) dmv group by dmv.objectid) end";

        #endregion

        #region reorg

        internal const string ReorgDatabaseRetryQuery = "delete from @databases where dbid not in ";

        #endregion

        #region Cluster Node

        internal const string CurrentClusterNode =
            @"select ServerProperty('IsClustered'), ServerProperty('ComputerNamePhysicalNetBIOS')
exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'";

		//SQLDM-30197
		internal const string PreferredClusterNode = 
			@"select ServerProperty('IsClustered'), ServerProperty('MachineName')
exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'";
			
        internal const string ClusterSettingFalse = "and 1=0";

        #endregion

        #region waits

        internal const string ExcludeLike = "and lower({0}) not like lower('{1}')";
        internal const string ExcludeMatch = "and lower({0}) <> lower('{1}')";

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
        internal const string IncludeLike = "and lower({0}) like lower('{1}')";
        internal const string IncludeMatch = "and lower({0}) = lower('{1}')";

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

        internal const string ExcludePredicateWaitType = " AND [package0].[not_equal_uint64]([wait_type],({0}))";
        internal const string ExcludePredicateDurationLessThan = " [package0].[greater_than_equal_uint64]([duration],({0}))";
        internal const string ExcludePredicateSystem = " AND [sqlserver].[is_system]=({0})";
        internal const string ExcludePredicateGreaterThanSqlText = " AND [sqlserver].[greater_than_i_sql_unicode_string]([sqlserver].[sql_text],N''{0}'')";
        internal const string ExcludePredicateNotLikeSqlText = " AND NOT [sqlserver].[like_i_sql_unicode_string]([sqlserver].[sql_text],N''{0}'')";
        internal const string ExcludePredicateNotEqualSqlText = " AND [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[sql_text],N''{0}'')";
        internal const string ExcludePredicateNotLikeAppName = " AND NOT [sqlserver].[like_i_sql_unicode_string]([sqlserver].[client_app_name],N''{0}'')";
        internal const string ExcludePredicateNotEqualAppName = " AND [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[client_app_name],N''{0}'')";
        internal const string ExcludePredicateNotLikeDbName = " AND NOT [sqlserver].[like_i_sql_unicode_string]([sqlserver].[database_name],N''{0}'')";
        internal const string ExcludePredicateNotEqualDbName = " AND [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[database_name],N''{0}'')";

        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters
        internal const string IncludePredicateLikeSqlText = " AND [sqlserver].[like_i_sql_unicode_string]([sqlserver].[sql_text],N''{0}'')";
        internal const string IncludePredicateEqualSqlText = " AND NOT [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[sql_text],N''{0}'')";
        internal const string IncludePredicateLikeAppName = " AND [sqlserver].[like_i_sql_unicode_string]([sqlserver].[client_app_name],N''{0}'')";
        internal const string IncludePredicateEqualAppName = " AND NOT [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[client_app_name],N''{0}'')";
        internal const string IncludePredicateLikeDbName = " AND [sqlserver].[like_i_sql_unicode_string]([sqlserver].[database_name],N''{0}'')";
        internal const string IncludePredicateEqualDbName = " AND NOT [sqlserver].[not_equal_i_sql_unicode_string]([sqlserver].[database_name],N''{0}'')";
        //SQLdm 8.5 (Ankit Srivastava): for Inclusion Filters

        internal const string ExcludePredicateActiveWaits =
            "WHERE ({0} AND [package0].[equal_uint64]([opcode],(1)) {1} {2} AND [sqlserver].[session_id]>=(50) {3} {4} {5})";

        #endregion

        #region Jobs and Steps

        internal const string GetJobsMode = "exec msdb..sp_help_job";
        internal const string GetStepsMode = "exec msdb..sp_help_job @job_name='{0}', @job_aspect='STEPS'";
        internal const string GreaterThanStringPredicate2008 = "[package0].[greater_than_i_unicode_string]";
        internal const string LikeStringPredicate2012 = "[sqlserver].[like_i_sql_unicode_string]";
        internal const string QueryMonitorDateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss";
        // SQLdm 10.3 (Varun Chopra) Linux Support for Batch Execution
        internal const string AdvancedOptionsHeader = @"
exec sp_configure 'show advanced options', 1;  
 GO  
 RECONFIGURE;  
 GO  
";

        #endregion Jobs and Steps
    }
}
