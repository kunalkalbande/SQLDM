--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------
-- SQL diagnostic manager v10.0.0.0
-- Copyright © Idera, Inc. 2003-2015 
--------------------------------------------------------------

set transaction isolation level read uncommitted 
 set lock_timeout 20000 
 set implicit_transactions off 
 if @@trancount > 0 commit transaction 
 set language us_english 
 set cursor_close_on_commit off 
 set query_governor_cost_limit 0 
 set numeric_roundabort off
--------------------------------------------------------------------------------
--  Batch: Server Overview 2012
--  Tables: 
--  XSP:	
--	Variables:	
--  [0] - sp_OACreate context
--  [1] - Session Count Segment
--	[2] - Lock Counter Statistics Segment
--	[3] - Disable OLE
--------------------------------------------------------------------------------\

--------------------------------------------------------------------------------\
--IF ( IS_SRVROLEMEMBER(N'securityadmin') = 1)
BEGIN
	SET nocount on
	SELECT DB_NAME();
	
	--START (RRG): Get internal for Instance Name when in Azure Platform
	declare @sysperfinfoname sysname
	Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
	declare @MasterDatabaseStatePermission int
	SELECT @MasterDatabaseStatePermission = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
	--extract one row that contains the actual instance name and exclude others
	--there should be only one instance name in this format MSSQL$XXXXXXXXXXXX where X is HEX char A-F and numbers 0-9
	if (@MasterDatabaseStatePermission = 1) 
	BEGIN
		select @sysperfinfoname = (select top 1 object_name from sys.dm_os_performance_counters (nolock) where lower(object_name) like '%MSSQL$%')
	END
	select @sysperfinfoname = SUBSTRING(@sysperfinfoname, 1, PATINDEX('%:%',@sysperfinfoname)-1)
	--END (RRG): Get internal for Instance Name when in Azure Platform

	-- Execute Server level details only on master database
	IF DB_NAME() = 'master' --AND @MasterDatabaseStatePermission = 1
	BEGIN
		--------------------------------------------------------------------------------
		--  XP: Product version, Language, Processor Count and Physical Memory
		--  Returns:
		--		Index 
		--		Name 
		--		Internal_Value 
		--		Character_Value  
		--------------------------------------------------------------------------------
		--START (RRG): master..xp_msver is Available for Azure SQL Database Managed Instance
		--	exec master..xp_msver
		DECLARE @tempVar_SerevrOverview_1 TABLE
		(
			[Index] int, 
			[Name] nvarchar(1024),
			[Internal_Value] bigint,
			[Character_Value] nvarchar(max)
		)
		INSERT INTO @tempVar_SerevrOverview_1 VALUES(1,'ProductName',NULL,CONVERT(NVARCHAR(MAX),SERVERPROPERTY('Edition'))) 
		INSERT INTO @tempVar_SerevrOverview_1 VALUES(2,'ProductVersion',NULL,CONVERT(NVARCHAR(MAX),SERVERPROPERTY('ProductVersion'))) 
		INSERT INTO @tempVar_SerevrOverview_1 VALUES(14,'SpecialBuild',NULL,CONVERT(NVARCHAR(MAX),SERVERPROPERTY('ProductBuild'))) 
		SELECT * FROM @tempVar_SerevrOverview_1
		--END RRG

		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - registry reads are not allowed in cloud systems
		--exec master.dbo.xp_instance_regread N'HKEY_LOCAL_MACHINE', N'SOFTWARE\MICROSOFT\Windows NT\CurrentVersion', N'ProductName' 
		select 'ProductName' as Value,'CloudServer' as Data
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

		select serverproperty('MachineName')
		select serverproperty('Edition')
		select @@version


		-------------------------------------------------------------------------------
		--  QUERY: Affinity Mask
		--  Tables: syscurconfigs
		--  Returns:
		--    Value
		--  Criteria: 
		--  (1) Returns value for the following configuration option:
		--			1535 Affinity Mask (Bit mask - default 0, which is none)
		-------------------------------------------------------------------------------
		--START (RRG): syscurconfigs is available in Azure SQL Database
		select 
			AffinityMask = cast(cast(value as varbinary) as bigint)
		from 
			syscurconfigs 
		where config = 1535 

		--------------------------------------------------------------------------------
		--  QUERY: Max user connections
		--  Tables: master..syscurconfigs 
		--  Returns:
		--    Max user connections from syscurconfigs
		--    Max user connections from @@max_connections
		--  Notes:
		--  (1) Use the first if non-zero, else the second
		--------------------------------------------------------------------------------
		--START (RRG): syscurconfigs is available in Azure SQL Database and Managed Instance
		Select 
			value, 
			MaxConnections = @@max_connections 
		from 
			syscurconfigs 
		where config = 103 
		--END (RRG): syscurconfigs is available in Azure SQL Database and Managed Instance

		select SERVERPROPERTY('processid') as SQLserverID
		
		select NULL AS logicalCPUs

		--------------------------------------------------------------------------------
		--  QUERY: IsClustered
		--  Returns:
		--		"isclustered"
		--		Physical computer (node) name
		--------------------------------------------------------------------------------
		select IsClustered = serverproperty('isclustered'), ComputerNamePhysicalNetBIOS = ServerProperty('ComputerNamePhysicalNetBIOS')

		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - cloud server doesnt support registry reading
		--exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'
		select 'ComputerName' as Value,'CloudServer' as Data
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

				--------------------------------------------------------------------------------
		--  XP: Login mode
		--  Returns:  Login Mode
		--------------------------------------------------------------------------------
		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - doesnt have permission to execute xp_loginconfig in case of cloud server
		--exec @status = master..xp_loginconfig 'login mode' 
		--SET @status = 1
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

		--if @status <> 0 
		--	select 'cannot find login mode'
		select 'login mode' as 'name','Mixed' as 'config_value'
	
		--------------------------------------------------------------------------------
		--  XP: Audit Level
		--  Returns:  Audit Level
		--------------------------------------------------------------------------------
		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - doesnt have permission to execute xp_loginconfig in case of cloud server
		--exec @status = master..xp_loginconfig 'login mode' 
		--SET @status = 1
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support 

		--if @status <> 0 
		--	select 'cannot find audit level' 
		select 'audit level' as 'name','failure' as 'config_value'
		
		declare 
			@command nvarchar(2048), 
			@dbname nvarchar(255), 
			@mode smallint

		if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
			create table #totaldbsizes (usedsize dec(15,0)) 
		else 
			truncate table #totaldbsizes 

		--START (RRG): This section is setup for the next section
		declare @templogsize dec(12,0), @tempdevsize dec(12,0)

		--START (RRG): This section works in both Azure SQL Database and Managed Instance
		select @templogsize = (cast(size as dec(12,0)) * 8) 
		from tempdb.sys.database_files
		where type = 1

		select @tempdevsize = (cast(size as dec) * 8)
		from tempdb.sys.database_files
		where type = 0
		--END (RRG): This section works in both Azure SQL Database and Managed Instance

		--END (RRG): This section is setup for the next section

		----------------------------------------------------------------------------------
		---- QUERY: Database summary information
		---- Tables: master..sysdatabases, tempdb..#totaldbsizes
		---- Returns:
		---- Total number of databases
		---- Number of data files
		---- Number of log files
		---- Total data file space allocated
		---- Total log file space allocated
		---- Total data file space used
		----------------------------------------------------------------------------------
		DECLARE @tempVar_ServerOverview_8 TABLE
		(
			[type] INT,
			[database_id] INT,
			[size] INT
		)
		select
			'summary'
			,(
				select count(*)
				from
				sysdatabases
			)
			,sum(case when type = 0 then 1 else 0 end )
			,sum(case when type = 1 then 1 else 0 end )
			,sum(case
					when type = 0 and database_id <> db_id('tempdb')
						then isnull(convert(dec(12,0),size) * 8,0)
					when type = 0 and database_id = db_id('tempdb')
						then @tempdevsize
					else 0 end)
			,sum(case
					when type = 1 and database_id <> db_id('tempdb')
						then isnull(convert(dec(12,0),size) * 8,0)
					when type = 1 and database_id = db_id('tempdb')
						then @templogsize
					else 0 end)
			,(
				select sum(usedsize) 
				from #totaldbsizes)
		from @tempVar_ServerOverview_8
		
		drop table #totaldbsizes

		-- (RRG): This will only master and current User Database for Azure SQL Database
		SELECT cast(database_id AS BIGINT)
			,name
			,0 AS DatabaseStatus
		FROM sys.databases


	    --START (RRG): sys.dm_os_performance_counters is available for both Azure SQL Database
		--  *** For databases that are not System Databases the Instance_Name is returning an encoded string (i.e 0a7f1646-3690-4b53-8a27-7823d33687f8) on Azure SQL Database
		--  ***   Fixed by joining to sys.databases
		if (@MasterDatabaseStatePermission = 1) 
		BEGIN
			select 
				counter_name, 
				convert(bigint,isnull(cntr_value,0)), 
				--instance_name,
				DBName = d.name
			from 
				sys.dm_os_performance_counters pc
				inner join sys.databases d on d.physical_database_name = pc.instance_name
			where 			
				lower(object_name) = lower(@sysperfinfoname + ':databases') 
				and lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
				and lower(instance_name) <> '_total' 
				and lower(instance_name) <> 'mssqlsystemresource'
		END
		ELSE
			BEGIN
				SELECT NULL, NULL, NULL
			END
		--END (RRG): sys.dm_os_performance_counters is available for both Azure SQL Database

		-- START: SQLdm 11.0 - Azure Support: Memory Chart
		SELECT *
			FROM sys.resource_stats as rs
			INNER JOIN (
				SELECT database_name
					,MAX(end_time) AS end_time
				FROM sys.resource_stats
				GROUP BY database_name
				) AS grouped_rs ON rs.database_name = grouped_rs.database_name
				AND rs.end_time = grouped_rs.end_time
				inner join sys.databases db
				ON db.name = rs.database_name

		-- END

	END

-- End of Per Sql Server Script
-- Per Database Script

SELECT DATABASEPROPERTYEX (DB_NAME(DB_ID()), 'MaxSizeInBytes') AS [MaxSizeInBytes], DB_NAME() AS database_name OPTION (RECOMPILE)

-- cpu usage chart
if HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE') = 1 
BEGIN
	 SELECT  top 1 DB_NAME() AS database_name,  avg_cpu_percent, avg_data_io_percent, avg_log_write_percent , 
	 dtu_limit, cpu_limit FROM sys.dm_db_resource_stats ORDER by end_time desc;
END
else
	BEGIN
		SELECT DB_NAME() AS database_name, null, null, null, null, null
	END


		--START (RRG): The following builtin function work in Azure SQL Database
		select 
			cpu_busy = @@cpu_busy, 
			idle = @@idle, 
			io_busy = @@io_busy, 
			pack_received = @@pack_received, 
			pack_sent = @@pack_sent, 
			packet_errors = @@packet_errors, 
			total_read = @@total_read, 
			total_write = @@total_write, 
			total_errors = @@total_errors, 
			connections = @@connections,
			timeticks = @@timeticks
		--END (RRG): The following builtint functions work in Azure SQL Database

		--------------------------------------------------------------------------------
		--  QUERY: Throughput
		--------------------------------------------------------------------------------
		set arithabort on



		--START (RRG): sys.dm_os_sys_info is available in Azure SQL Database
		--declare @timestamp bigint
		--select @timestamp = cpu_ticks / (cpu_ticks/ms_ticks) from sys.dm_os_sys_info

		--START: SQLdm 10.0 (Tarun Sapra): Minimal Cloud Support- sys.dm_os_ring_buffers isnt available
		--;with SystemHealthRingBuffer(record,timestamp)
		--as
		--(
		--select top 30
		--record = convert(xml,record),
		--@timestamp - timestamp
		--from sys.dm_os_ring_buffers
		--where ring_buffer_type = 'RING_BUFFER_SCHEDULER_MONITOR'
		--and record like '%<SystemHealth>%'
		--order by timestamp desc
		--)
		--select 
		--	dateadd(ms, (timestamp * -1) % 1000, dateadd(s, (timestamp * -1) / 1000, getutcdate())),
		--	SqlCpu = record.value('(./Record/SchedulerMonitorEvent/SystemHealth/ProcessUtilization)[1]', 'int')
		--from SystemHealthRingBuffer

		--DECLARE @tempVar_ServerOverview_3 TABLE
		--(
		--	DateStamp DATETIME,
		--	SqlCpu INT
		--)
		--SELECT * FROM @tempVar_ServerOverview_3
		--START (RRG):sys.dm_os_sys_info is available in Azure SQL Database

		declare @status int  

		--------------------------------------------------------------------------------
		--  QUERY: Total Locks
		--  Tables: master..sysperfinfo
		--  Returns: Lock Count
		--------------------------------------------------------------------------------
		--START (RRG): sysperfinfo is available in both Azure SQL Database and Managed Instance
		--          should consider adopting sys.dm_os_performance_counters instead of sysperfinfo
		if (@MasterDatabaseStatePermission = 1) 
		BEGIN
		select
		  case
			when cntr_value >= 0
			then convert(dec(20,0),cntr_value)
			else convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1
		  end
		from sys.dm_os_performance_counters (nolock)
		where lower(object_name) = lower(@sysperfinfoname + ':locks')
		  and lower(instance_name) = '_total'
		  and lower(counter_name) = lower('lock requests/sec')
		END
		else
			BEGIN
					SELECT CONVERT(dec(20,0), 0)
			END

		--END (RRG): sysperfinfo is available in Azure SQL Database

		{1}

		{2}

		--------------------------------------------------------------------------------
		--  QUERY: Performance Counters
		--  Tables: sys.dm_os_performance_counters
		--  Returns:
		--		batch requests/sec
		--		buffer cache hit ratio
		--		buffer cache hit ratio base
		--		cache hit ratio
		--		cache hit ratio base
		--		checkpoint pages/sec
		--		full scans/sec
		--		lazy writes/sec
		--		lock waits/sec
		--		log flushes/sec
		--		page life expectancy
		--		page lookups/sec
		--		page reads/sec
		--		page splits/sec
		--		page writes/sec
		--		readahead pages/sec
		--		sql compilations/sec
		--		sql re-compilations/sec
		--		table lock escalations/sec
		--		target server memory(kb)
		--		total server memory (kb)
		--		workfiles created/sec
		--		worktables created/sec
		--------------------------------------------------------------------------------
		--This total pages gets read into the refresh.statistics.buffercachesize object
		
		--START (RRG): Performance Counters are available in Azure SQL Database
		if (@MasterDatabaseStatePermission = 1) 
		BEGIN
			select 
				PerfCounterName = 'total pages', 
				CounterValue = isnull(sum(convert(bigint,cntr_value/8)),0)
			from 
				sys.dm_os_performance_counters 
			where (Lower(object_name) = Lower(@sysperfinfoname + ':Memory Manager') 
					and Lower(counter_name) = 'Total Server Memory (KB)')
			group by 
				counter_name
			union
			select 
				PerfCounterName = lower(RTRIM(counter_name)), 
				CounterValue = isnull(sum(convert(bigint,cntr_value)),0)
			from 
				sys.dm_os_performance_counters 
			where 
				(lower(object_name) = lower(@sysperfinfoname + ':SQL Statistics')
					and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
				or (Lower(object_name) = Lower(@sysperfinfoname + ':Buffer Manager') 
					and (Lower(counter_name) like 'buffer cache hit ratio%'
					or Lower(counter_name) = 'total pages')) 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':plan cache') 
					and Lower(counter_name) like 'cache hit ratio%' 
					and Lower(instance_name) <> '_total') 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':databases') 
					and Lower(counter_name) in ( 'log flushes/sec','transactions/sec')
					and Lower(instance_name) <> '_total') 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':memory manager') 
					and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':access methods') 
					and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':locks') 
					and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
				or (Lower(object_name) = Lower(@sysperfinfoname + ':buffer manager') 
					and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
			group by 
				counter_name
		END
		else
			BEGIN
				SELECT NULL, NULL
			END
		--END (RRG): Performance Counters are available in Azure SQL Database

		--END (RRG):: This section needs to be revisited. It may or may not work depending on what is reworked above this
		--  sysdatabases is available in Azure SQL Database

		--START (RRG): DBCC SQLPERF is available in Azure SQL Database
		--   NOTE: In Azure SQL Database the user database names are may be encoded with a GUID
		--         For this to work it will need to be determined if the encoded GUIDs can be translated to the actual database names
IF 1 = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'VIEW DATABASE STATE')
BEGIN
BEGIN TRY
SELECT DB_NAME() AS [Database Name]
	,CAST(total_log_size_in_bytes AS REAL) / 1024 / 1024 AS [Log Size (MB)]
				,used_log_space_in_percent AS [Log Space Used (%)]
				,0 AS [Status]
	
FROM sys.dm_db_log_space_usage
END TRY
BEGIN CATCH
	SELECT DB_NAME() AS [Database Name]
	,NULL AS [Log Size (MB)]
	,NULL AS [Log Space Used (%)]
	,0 AS [Status]
END CATCH
END
ELSE
BEGIN
	SELECT DB_NAME() AS [Database Name]
	,NULL AS [Log Size (MB)]
	,NULL AS [Log Space Used (%)]
	,0 AS [Status]
END

		--END (RRG): DBCC SQLPERF is available in Azure SQL Database

		----This total pages gets read into the refresh.memorystatistics.buffercachepages object
		if (@MasterDatabaseStatePermission = 1) 
		BEGIN
			select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
			from sys.dm_exec_cached_plans
			union
			select 'Total pages', convert(dec(20,0),cntr_value)/8
			from sys.dm_os_performance_counters 
			where counter_name = 'Total Server Memory (KB)'
			union
			select
				case
					when counter_name = 'Database pages' or counter_name = 'Database Cache Memory (KB)'
						then 'Committed Pages'
					when counter_name = 'Free Memory (KB)' then 'free pages'
					else counter_name
				end,
				case 
					when counter_name = 'Free Memory (KB)' or counter_name = 'Database Cache Memory (KB)' then convert(dec(20,0),cntr_value)/8 
					else cntr_value  
				end
			from sys.dm_os_performance_counters
			where counter_name in
				(
					'Procedure cache pages',
					'Total Server Memory (KB)',
					'Connection Memory (KB)',
					'Lock Memory (KB)',
					'Optimizer Memory (KB)',
					'Granted Workspace Memory (KB)',
					'Database Cache Memory (KB)',
					'Free Memory (KB)',
					'Total Server Memory (KB)'
				)
			order by 1
		END
		else
			BEGIN
				SELECT NULL, NULL
			END
		--END (RRG): This will work in Azure SQL Database

		--START (RRG): Need to reformulate query to accomodate Azure SQL Database returning NULL for database name and
		--    Azure SQL Database Managed Instance is capable of returning data using original query
		if (@MasterDatabaseStatePermission = 1) 
		BEGIN
			select 
				DBName = name,
				NumberReads = sum(cast(num_of_reads as dec(38,0))),
				NumberWrites = sum(cast(num_of_writes as dec(38,0))),
				BytesRead = sum(cast(num_of_bytes_read as dec(38,0))),
				BytesWritten = sum(cast(num_of_bytes_written as dec(38,0))),
				IoStallMS = sum(cast(io_stall as dec(38,0)))
			from 
				sys.dm_io_virtual_file_stats(null,null) AS vfs 
				INNER JOIN sys.databases AS d ON vfs.database_id = d.database_id --JOIN is needed to avoid NULL database names
			WHERE
				d.name = DB_NAME()
			group by
				name
		END
		ELSE
			BEGIN
				SELECT 	DB_NAME() AS DBName, NULL AS NumberReads, NULL AS NumberWrites, NULL AS BytesRead, NULL AS BytesWritten, NULL AS IoStallMS
			END

		--END (RRG): Need to reformulate query to accomodate Azure SQL Database returning NULL when not a system database

	END

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: We assume that in case of rds, user type is sysadmin
--ELSE
--	BEGIN
--				--------------------------------------------------------------------------------
--		--  Batch: Server Overview 2005
--		--  Tables: 
--		--	[3] - Disable OLE
--		--------------------------------------------------------------------------------\
--		select 'IsNotSysAdmin'as SQLPermission
	
--		--------------------------------------------------------------------------------
--		--  SQL: Product Edition 
--		--  Returns:
--		--		SQL edition 
--		--------------------------------------------------------------------------------
--		select serverproperty('Edition')as Edition

--		--------------------------------------------------------------------------------
--		--  QUERY: IsClustered
--		--  Returns:
--		--		"isclustered"
--		--		Physical computer (node) name
--		--------------------------------------------------------------------------------
--		select serverproperty('isclustered')as IsClustered 

--		--------------------------------------------------------------------------------
--		--  QUERY: get Properties using XP_MSVER
--		--  Returns:
--		--		"isclustered"
--		--		Physical computer (node) name
--		--------------------------------------------------------------------------------
--		EXEC    master.dbo.xp_msver ProcessorCount;


--		IF OBJECT_ID('tempdb..#TempTableVer') IS NOT NULL
--		BEGIN
--			DROP TABLE #TempTableVer
--		END
		
--		CREATE TABLE #TempTableVer
--		(
--		[Index] VARCHAR(MAX),
--		Name VARCHAR(MAX),
--		Internal_Value NVARCHAR(MAX),
--		Character_Value NVARCHAR(MAX)
--		)

--		INSERT INTO #TempTableVer EXEC xp_msver

--		IF EXISTS(select 1 from #TempTableVer WHERE Name='ProcessorActiveMask'and Internal_Value is null)
--			BEGIN
--			  EXEC    master.dbo.xp_msver ProcessorCount;
--			END
--		ELSE
--			BEGIN
--			   EXEC    master.dbo.xp_msver ProcessorActiveMask;
--			END
--		DROP TABLE #TempTableVer


--		select serverproperty('MachineName')as Host

--		SELECT OSVersion =RIGHT(@@version, LEN(@@version)- 3 -charindex (' ON ',@@VERSION)) 

--		EXEC    master.dbo.xp_msver PhysicalMemory
		
		

--		select COUNT(name)as TotalDB from master..sysdatabases


--		--------------------------------------------------------------------------------
--		--  QUERY: data/log size
--		--  Returns:
--		--		file size in MBS
--		--------------------------------------------------------------------------------
--			IF OBJECT_ID('tempdb..#Tmp') IS NOT NULL
--			BEGIN
--				DROP TABLE #Tmp
--			END
			
--			CREATE TABLE #tmp
--			(
--				name varchar(200), 
--				[size] varchar(20), 
--				owner varchar(50), 
--				dbid int, 
--				created datetime, 
--				status varchar(4000), 
--				compat int
--			) 
--			INSERT INTO #tmp EXEC sp_helpdb
		
--			IF OBJECT_ID('tempdb..#FileSizes') IS NOT NULL
--			BEGIN
--				DROP TABLE #FileSizes
--			END
--			CREATE  TABLE #FileSizes (
--				[DBName] sysname,
--				[FILE Name] VARCHAR(MAX),
--				[Type file] VARCHAR(MAX),
--				[SIZE] DECIMAL(12,2)
--			)

--			DECLARE @SQL NVARCHAR(MAX)
--			SET @SQL = ''

--			SELECT @SQL = @SQL + 'USE'  + QUOTENAME(name) + 'insert into #FileSizes
--			select ' + QUOTENAME(name,'''') + ', Name, type_desc, size/128 from sys.database_files '
--			FROM #tmp
 
--			EXECUTE (@SQL)
--			--SELECT * FROM #FileSizes ORDER BY DBName, [FILE Name]
--			SELECT CONVERT(INT,SUM(SIZE)) as dataSize FROM #FileSizes WHERE [Type file]='ROWS'
--			SELECT CONVERT(INT,SUM(SIZE)) as logSize FROM #FileSizes WHERE [Type file]='LOG'
--			drop Table #FileSizes
--			drop table #tmp	
			
			
--	END
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: We assume that in case of rds, user type is sysadmin
