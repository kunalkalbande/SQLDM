----------------------------------------------------------------------------------------------
--
--
--	
----------------------------------------------------------------------------------------------

use tempdb;

declare 
	@servername varchar(255), 
	@sysperfinfoname varchar(255),
	@slashpos int;

declare @outputTable table (Name varchar(64), Value varchar(32));

select @servername = cast(serverproperty('servername') as nvarchar(255))

select @servername = upper(@servername) 

select @slashpos = charindex('\', @servername)  

if @slashpos <> 0 
	begin 
		select @sysperfinfoname = 'MSSQL$' + substring(@servername, @slashpos + 1, 30) 
	end  
else 
	begin 
		select @sysperfinfoname = 'SQLSERVER'
	end  


insert into @outputTable(Name, Value) select 'AsOf', convert(varchar,getdate(),21);
insert into @outputTable(Name, Value) select 'pack_sent', @@pack_sent;
insert into @outputTable(Name, Value) select 'pack_received', @@pack_received;
insert into @outputTable(Name, Value) select 'connections', @@connections;
insert into @outputTable(Name, Value) select wait_type, wait_time_ms from sys.dm_os_wait_stats where
											 wait_type = 'ASYNC_NETWORK_IO' or
											 wait_type = 'NET_WAITFOR_PACKET' or
											 wait_type = 'WRITELOG' or
											 wait_type = 'LOGBUFFER' or
											 wait_type = 'PAGEIOLATCH_NL' or
											 wait_type = 'PAGEIOLATCH_KP' or
											 wait_type = 'PAGEIOLATCH_SH' or
											 wait_type = 'PAGEIOLATCH_UP' or
											 wait_type = 'PAGEIOLATCH_EX' or
											 wait_type = 'PAGEIOLATCH_DT' or
											 wait_type = 'ASYNC_IO_COMPLETION' or
											 wait_type = 'IO_COMPLETION' or
											 wait_type = 'LOGMGR' or
											 wait_type = 'CXPACKET' or
											 wait_type = 'SOS_SCHEDULER_YIELD';

insert into @outputTable(Name, Value) 
	select '_total_waits', SUM(wait_time_ms) 
	from sys.dm_os_wait_stats 
	where wait_type not in ('BROKER_TASK_STOP',
							'SQLTRACE_BUFFER_FLUSH',
							'LAZYWRITER_SLEEP')

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
insert into @outputTable(Name, Value) 
	select 
		lower(RTRIM(counter_name)), 
		isnull(sum(convert(bigint,cntr_value)),0)
	from 
		sys.dm_os_performance_counters 
	where 
		(lower(object_name) = lower(@sysperfinfoname + ':SQL Statistics')
			and lower(counter_name) in ('batch requests/sec', 'sql compilations/sec', 'sql re-compilations/sec') )
		or (lower(object_name) = lower(@sysperfinfoname + ':buffer manager') 
			and lower(counter_name) in ('page life expectancy')) --'page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
		or (lower(object_name) = lower(@sysperfinfoname + ':buffer Manager') 
			and (lower(counter_name) like 'buffer cache hit ratio%'))
		or (lower(object_name) = lower(@sysperfinfoname + ':memory manager') 
			and lower(counter_name) in ('memory grants pending')) 
		or (lower(object_name) = lower(@sysperfinfoname + ':access methods') 
			and lower(counter_name) in ('page splits/sec', 'pages allocated/sec')) 
		--	or Lower(counter_name) = 'total pages')) 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':plan cache') 
		--	and Lower(counter_name) like 'cache hit ratio%' 
		--	and Lower(instance_name) <> '_total') 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':databases') 
		--	and Lower(counter_name) = 'log flushes/sec' 
		--	and Lower(instance_name) <> '_total') 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':memory manager') 
		--	and Lower(counter_name) in ('target server memory (kb)', 'total server memory (kb)')) 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':access methods') 
		--	and Lower(counter_name) in ('page splits/sec','full scans/sec', 'table lock escalations/sec', 'worktables created/sec', 'workfiles created/sec')) 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':locks') 
		--	and Lower(counter_name) = 'lock waits/sec' and Lower(instance_name) <> '_total') 
		--or (Lower(object_name) = Lower(@sysperfinfoname + ':buffer manager') 
		--	and Lower(counter_name) in ('page reads/sec', 'page writes/sec', 'lazy writes/sec','checkpoint pages/sec', 'checkpoint writes/sec', 'readahead pages/sec', 'page lookups/sec', 'page requests/sec', 'page life expectancy')) 
	group by 
		counter_name;

insert into @outputTable(Name, Value) select 'TempDbMetaDataWaits', count(*) from sys.sysprocesses where rtrim(waitresource) in ('2:1:1','2:1:3') or rtrim(waitresource) like '% 2:1:[13]';

select Name, Value from @outputTable;
