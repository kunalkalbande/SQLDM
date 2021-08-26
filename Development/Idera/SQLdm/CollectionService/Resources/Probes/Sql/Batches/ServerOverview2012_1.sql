-- SQL diagnostic manager v10.0.0.0
-- Copyright © Idera, Inc. 2003-2015 

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
		
		set nocount on
		--------------------------------------------------------------------------------
		--  XP: Product version, Language, Processor Count and Physical Memory
		--  Returns:
		--		Index 
		--		Name 
		--		Internal_Value 
		--		Character_Value  
		--------------------------------------------------------------------------------
		exec master..xp_msver 

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
		Select 
			cast(cast(value as varbinary) as bigint)
		from 
			master..syscurconfigs 
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
		Select 
			value, 
			@@max_connections 
		from 
			master..syscurconfigs 
		where config = 103 

		--------------------------------------------------------------------------------
		--  QUERY: Throughput
		--------------------------------------------------------------------------------
		set arithabort on

		--SQLDM-21167: removig SQL cpu from sys.dm_os_ring_buffers
		/*declare @timestamp bigint

		select @timestamp = cpu_ticks / (cpu_ticks/ms_ticks) from sys.dm_os_sys_info
		;with SystemHealthRingBuffer(record,timestamp)
		as
		(
		select top 30
		record = convert(xml,record),
		@timestamp - timestamp
		from sys.dm_os_ring_buffers
		where ring_buffer_type = 'RING_BUFFER_SCHEDULER_MONITOR'
		and record like '%<SystemHealth>%'
		order by timestamp desc
		)
		select 
			dateadd(ms, (timestamp * -1) % 1000, dateadd(s, (timestamp * -1) / 1000, getutcdate())),
			SqlCpu = record.value('(./Record/SchedulerMonitorEvent/SystemHealth/ProcessUtilization)[1]', 'int')
		from SystemHealthRingBuffer*/

		select SERVERPROPERTY('processid') as SQLserverID

		select cpu_count AS logicalCPUs
		from  sys.dm_os_sys_info

		select 
			@@cpu_busy, 
			@@idle, 
			@@io_busy, 
			@@pack_received, 
			@@pack_sent, 
			@@packet_errors, 
			@@total_read, 
			@@total_write, 
			@@total_errors, 
			@@connections,
			@@timeticks

		--------------------------------------------------------------------------------
		--  Query: Procedure Cache
		--  DBCC: ProcCache
		--  Returns:
		--		num proc buffs - Total number of pages used by all entries in the 
		--			procedure cache.
		--		num proc buffs used - Total number of pages used by all entries that 
		--			are currently being used.
		--		num proc buffs active - For backward compatibility only. Total number of 
		--			pages used by all entries that are currently being used.
		--		proc cache size - Total number of entries in the procedure cache.
		--		proc cache used -  Total number of entries that are currently being used.
		--		proc cache active -  For backward compatibility only. Total number of 
		--			entries that are currently being used.
		--------------------------------------------------------------------------------
		--SQLdm 10.0 (Tarun Sapra)- following command isnt available, thus we are sending empty table 
		--dbcc proccache 
		--DECLARE @tempVar table (
		--[num proc buffs] int,
		--[num proc buffs used] int,
		--[num proc buffs active] int,
		--[proc cache size] int,
		--[proc cache used] int,
		--[proc cache active] int
		--)
		--select * from @tempVar
			Declare @newDb nvarchar(1000)
			Select top 1 @newDb=name from sys.databases where name not in ('master', 'msdb', 'model', 'tempdb', 'rdsadmin')
			If(@newDb is not null)
			Begin
			set @newDb = 'Use '+ @newDb +' dbcc proccache'
			exec (@newDb)
			end
			IF (@newDb is null)
			Begin 
			set @newDb = 'Create Database dbccDB'
			exec (@newDb)
			set @newDb = 'Use dbccDB dbcc proccache'
			exec (@newDb)
			set @newDb = 'Drop Database dbccDB'
			exec (@newDb)
			End 

		--------------------------------------------------------------------------------
		--  QUERY: IsClustered
		--  Returns:
		--		"isclustered"
		--		Physical computer (node) name
		--------------------------------------------------------------------------------
		select serverproperty('isclustered') , ServerProperty('ComputerNamePhysicalNetBIOS')

		--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
		--problem - cloud server doesnt support registry reading
		--exec master..xp_regread 'HKEY_LOCAL_Machine','SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName\','ComputerName'
		select 'ComputerName' as Value,'CloudServer' as Data
		--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

		Declare @sysperfinfoname sysname

		Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		If @sysperfinfoname Is Null
			   Set @sysperfinfoname = N'sqlserver';
		Else
			   Set @sysperfinfoname = N'mssql$' + lower(@sysperfinfoname);


		declare @status int 

		--------------------------------------------------------------------------------
		--  QUERY: Total Locks
		--  Tables: master..sysperfinfo
		--  Returns: Lock Count
		--------------------------------------------------------------------------------
		select
		  case
			when
			  cntr_value >= 0
			then
			  convert(dec(20,0),cntr_value)
			else
			  convert(dec(20,0), (2147483647.0 - (-2147483648.0 - cntr_value))) + 1
		  end
		from
		  master..sysperfinfo (nolock)
		where
		  lower(object_name) = lower(@sysperfinfoname + ':locks')
		  and lower(instance_name) = '_total'
		  and lower(counter_name) = lower('lock requests/sec')

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

		{1}

		{2}

		--------------------------------------------------------------------------------
		--  QUERY: Performance Counters
		--  Tables: master..sysperfinfo
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
		select 
			'total pages', 
			isnull(sum(convert(bigint,cntr_value/8)),0)
		from 
			master.sys.dm_os_performance_counters 
		where (Lower(object_name) = Lower(@sysperfinfoname + ':Memory Manager') 
				and Lower(counter_name) = 'Total Server Memory (KB)')
		group by 
			counter_name
		union
		select 
			lower(RTRIM(counter_name)), 
			isnull(sum(convert(bigint,cntr_value)),0)
		from 
			master.sys.dm_os_performance_counters 
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



		declare 
			@command nvarchar(2048), 
			@dbname nvarchar(255), 
			@mode smallint

		if (select isnull(object_id('tempdb..#totaldbsizes'), 0)) = 0 
			create table #totaldbsizes (usedsize dec(15,0)) 
		else 
			truncate table #totaldbsizes 


		declare read_db_sizes insensitive cursor 
		for 
			select 
				name, 
				mode
			from 
				master..sysdatabases d (nolock) 
			where
				lower(name) <> 'mssqlsystemresource'
				and has_dbaccess (name) = 1 
				and mode = 0 
				and isnull(databasepropertyex(name, 'IsInLoad'),0) = 0 
				and isnull(databasepropertyex(name, 'IsSuspect'),0) = 0 
				and isnull(databasepropertyex(name, 'IsInRecovery'),0) = 0 
				and isnull(databasepropertyex(name, 'IsNotRecovered'),0) = 0 
				and isnull(databasepropertyex(name, 'IsOffline'),0) = 0 
				and isnull(databasepropertyex(name, 'IsShutDown'),0) = 0 
				and (
					isnull(databasepropertyex(name, 'IsSingleUser'),0) = 0 
					or ( 
						isnull(databasepropertyex(name, 'IsSingleUser'),0) = 1 
						and not exists 
						(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
						and not exists
						(select * from master.sys.dm_tran_locks l where resource_database_id = d.dbid and l.request_session_id <> @@spid)
						)
					)
				and status & 32 <> 32 
				and status & 64 <> 64 
				and status & 128 <> 128 
				and status & 256 <> 256 
				and status & 512 <> 512 
		for read only
		set nocount on 
		open read_db_sizes 
		fetch read_db_sizes into @dbname, @mode
		while @@fetch_status = 0 
		begin 
			-- Double check to make sure the status hasn't changed
			if has_dbaccess (@dbname) = 1 
				and isnull(databasepropertyex(@dbname, 'IsInLoad'),0) = 0 
				and isnull(databasepropertyex(@dbname, 'IsSuspect'),0) = 0 
				and isnull(databasepropertyex(@dbname, 'IsInRecovery'),0) = 0 
				and isnull(databasepropertyex(@dbname, 'IsNotRecovered'),0) = 0 
				and isnull(databasepropertyex(@dbname, 'IsOffline'),0) = 0 
				and isnull(databasepropertyex(@dbname, 'IsShutDown'),0) = 0 
				and (
					isnull(databasepropertyex(@dbname, 'IsSingleUser'),0) = 0 
					or ( 
						isnull(databasepropertyex(@dbname, 'IsSingleUser'),0) = 1 
						and not exists 
						(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
						and not exists
						(select * from master.sys.dm_tran_locks l where resource_database_id = db_id(@dbname) and l.request_session_id <> @@spid)
						)
					)
			--begin
				-- Fixed rally issue DE20472 Aditya Shukla SQLdm 8.6
				--select @command = 'insert into #totaldbsizes select sum(convert(dec(15,0),used_pages))*8 from [' 
				--+ replace(@dbname,char(93),char(93)+char(93)) 
				--+ '].sys.allocation_units'
				
				--SQLdm 10.0 (Tarun Sapra)- since "dbcc showfilestats" cant be executed with the current permissions, so we arent populating "#totaldbsizes" table
				--select @command = 'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] '
				--				   +'create table #filestats (Fileid int, FSFileGroup int, TotalExtents int, UsedExtents int, FSName sysname, FSFileName nchar(520)) '
				--				   +'insert #filestats exec(''dbcc showfilestats'') ' 
				--				   +'insert into #totaldbsizes select CONVERT(DECIMAL(38, 0), UsedExtents) * CONVERT(DECIMAL(38,0), 64) from #filestats'
			
			--end
			Declare @newDb1 nvarchar(1000)
			Select top 1 @newDb1=name from sys.databases where name not in ('master', 'msdb', 'model', 'tempdb', 'rdsadmin')
			If(@newDb1 is not null)
			Begin
			select @command = 'use [' + replace(@newDb1,char(93),char(93)+char(93)) + '] '
								   +'create table #filestats (Fileid int, FSFileGroup int, TotalExtents int, UsedExtents int, FSName sysname, FSFileName nchar(520)) '
								   +'IF ((IS_SRVROLEMEMBER(''sysadmin'') = 1) OR (IS_ROLEMEMBER(''db_owner'') = 1)) '
								   + 'insert #filestats exec(''dbcc showfilestats'') ' 
								   +'insert into #totaldbsizes select CONVERT(DECIMAL(38, 0), UsedExtents) * CONVERT(DECIMAL(38,0), 64) from #filestats'
			exec (@command)
			end
			IF (@newDb1 is null)
			Begin 
			set @newDb1 = 'Create Database dbccDB'
			exec (@newDb1)
			select @command = 'use [' + replace(@newDb1,char(93),char(93)+char(93)) + '] '
								   +'create table #filestats (Fileid int, FSFileGroup int, TotalExtents int, UsedExtents int, FSName sysname, FSFileName nchar(520)) '
								   +'IF ((IS_SRVROLEMEMBER(''sysadmin'') = 1) OR (IS_ROLEMEMBER(''db_owner'') = 1)) '
								   + 'insert #filestats exec(''dbcc showfilestats'') ' 
								   +'insert into #totaldbsizes select CONVERT(DECIMAL(38, 0), UsedExtents) * CONVERT(DECIMAL(38,0), 64) from #filestats'
			exec (@command)
			set @newDb1 = 'Drop Database dbccDB'
			exec (@newDb1)
			End
	
			fetch read_db_sizes into @dbname, @mode
		End 
		Close read_db_sizes 
		deallocate read_db_sizes 


		declare @templogsize dec(12,0), @tempdevsize dec(12,0)

		select @templogsize = (cast(size as dec(12,0)) * 8) 
		from tempdb.sys.database_files
		where type = 1

		select @tempdevsize = (cast(size as dec) * 8)
		from tempdb.sys.database_files
		where type = 0

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
		select
		'summary',
		(select
		count(*)
		from
		master..sysdatabases)
		,sum(case when type = 0 then 1 else 0 end )
		,sum(case when type = 1 then 1 else 0 end )
		,sum(case
		when type = 0 and database_id <> db_id('tempdb')
		then isnull(convert(dec(12,0),size) * 8,0)
		when type = 0 and database_id = db_id('tempdb')
		then @tempdevsize
		else 0 end )
		,sum(case
		when type = 1 and database_id <> db_id('tempdb')
		then isnull(convert(dec(12,0),size) * 8,0)
		when type = 1 and database_id = db_id('tempdb')
		then @templogsize
		else 0 end )
		,(select sum(usedsize) from #totaldbsizes)
		from master.sys.master_files
		drop table #totaldbsizes


        IF ((IS_SRVROLEMEMBER('sysadmin') = 1) OR (IS_ROLEMEMBER('db_owner') = 1))
        BEGIN
          DBCC SQLPERF (LOGSPACE) 
        END
        ELSE
        BEGIN
            DECLARE @TempSqlPerfLogSpace TABLE
            (
            	[Database Name] NVARCHAR(MAX),
            	[Log Size (MB)] REAL,
            	[Log Space Used (%)] REAL,
            	[Status] INT
            )
            DECLARE @LogSpaceCommand NVARCHAR(MAX);
            SELECT @LogSpaceCommand=
            '
                    USE [?]
            		SELECT ''?'' AS ''Database Name'', 
            		       ddlsu.total_log_size_in_bytes*1.0/1024/1024 AS ''Log Size (MB)'', 
                           ddlsu.used_log_space_in_percent AS ''Log Space Used (%)'', 
            			   0 AS ''Status''
            		FROM sys.dm_db_log_space_usage ddlsu
            '
            INSERT INTO @TempSqlPerfLogSpace
              EXEC sp_MSforeachdb @LogSpaceCommand, '?'
            
            SELECT [Database Name],
        	  [Log Size (MB)],
              [Log Space Used (%)],
              [Status]
        	FROM @TempSqlPerfLogSpace
        END


		Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
		If @sysperfinfoname Is Null
			   Set @sysperfinfoname = N'sqlserver';
		Else
			   Set @sysperfinfoname = N'mssql$' + lower(@sysperfinfoname);

		--This total pages gets read into the refresh.memorystatistics.buffercachepages object
		select 'Cache Pages', sum(convert(dec(20,0),size_in_bytes))/1024
		from master.sys.dm_exec_cached_plans
		union
		select 'Total pages'
		 ,cntr_value/8
		from
		 master..sysperfinfo
		where
		 counter_name = 'Total Server Memory (KB)'
		union
		select
		 case
		   when counter_name = 'Database pages' or counter_name = 'Database Cache Memory (KB)'
			 then 'Committed Pages'
			 when counter_name = 'Free Memory (KB)' then 'free pages'
		   else
			 counter_name
		 end,
		 case when counter_name = 'Free Memory (KB)' or counter_name = 'Database Cache Memory (KB)' then cntr_value/8 
		 else cntr_value  
		 end
		from
		 master..sysperfinfo
		where
		 counter_name in
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
		select 
			cast(database_id as bigint), 
			name
		from
			master.sys.databases

		select 
			counter_name, 
			convert(bigint,isnull(cntr_value,0)), 
			instance_name 
		from 
			master.sys.dm_os_performance_counters
		where 
			Lower(object_name) = Lower(@sysperfinfoname + ':databases') 
			and Lower(counter_name) in ('transactions/sec', 'log flushes/sec', 'log bytes flushed/sec', 'log flush waits/sec', 'log cache reads/sec', 'log cache hit ratio', 'log cache hit ratio base') 
			and Lower(instance_name) <> '_total' 
			and lower(instance_name) <> 'mssqlsystemresource'
		order by 
			instance_name 

		select 
			db_name(database_id),
			NumberReads = sum(cast(num_of_reads as dec(38,0))),
			NumberWrites = sum(cast(num_of_writes as dec(38,0))),
			BytesRead = sum(cast(num_of_bytes_read as dec(38,0))),
			BytesWritten = sum(cast(num_of_bytes_written as dec(38,0))),
			IoStallMS = sum(cast(io_stall as dec(38,0)))
		from 
			sys.dm_io_virtual_file_stats(null,null)
		group by
			database_id
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