--------------------------------------------------------------------------------
--  Batch: Sessions 2014
--  Variables:  
--  [0] - Process types to return
--	[1] - Session Count Segment
--  [2] - Session max rowcount
--  [3] - use tempdb if master compatibility mode is 80 or below
--  [4] - Inputbuffer limiter
--------------------------------------------------------------------------------
-- use {3}


{1}


IF OBJECT_ID('tempdb..#snaps') IS NOT NULL
	drop table #snaps
	
declare @DBCCBuffer 
table  
(
	EventType nvarchar(260), 
	Parameters int,
	EventInfo nvarchar(MAX),
	BufferID int identity(1, 1) not null primary key
) 

CREATE TABLE #TempdbQueries(
session_id smallint,
login_name nvarchar(128),
host_name nvarchar(128),
status nvarchar(30),
program_name nvarchar(128),
command nvarchar(32),
databaseName nvarchar(128),
cpu_time int,
memory_usage int,
reads bigint,
writes bigint,
logical_reads bigint,
blocking_session_id smallint,
block_count int,
login_time datetime,
last_request_start_time datetime,
last_request_end_time datetime,
open_tran int,
net_transport nvarchar(12),
client_net_address nvarchar(12),
wait_time int,
request_id int,
last_wait_type nvarchar(max),
wait_type nvarchar(max),
wait_resource nvarchar(max),
mostRecentSql nvarchar(max),
elapsed_time_seconds int,
transaction_isolation_level smallint,
objectid int,
sessionUserPagesAlloc int,
sessionUserPagesDealloc int,
taskUserPagesAlloc int,
taskUserPagesDealloc int,
sessionInternalPagesAlloc int,
sessionInternalPagesDealloc int,
taskInternalPagesAlloc int,
taskInternalPagesDealloc int,
sqlhandle int
)

create table #snaps(session_id bigint primary key clustered, elapsed_time_seconds bigint)

declare @snapshotdbs int
declare @maxInt int
Select @maxInt = cast(Power(cast(2 as varchar),(32) -1) - 1 as int)

select @snapshotdbs =  isnull(count(database_id),0) from sys.databases
	where snapshot_isolation_state in (1,3)
	or is_read_committed_snapshot_on = 1
	
--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'dm_tran_active_snapshot_database_transactions' is not avlbl
if @snapshotdbs > 0	AND 1 = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
begin

	insert into #snaps
	select 
	session_id,
	sum(elapsed_time_seconds)
	from sys.dm_tran_active_snapshot_database_transactions snaps 
	where snaps.is_snapshot = 1
	and elapsed_time_seconds > 0
	group by session_id

end
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'dm_tran_active_snapshot_database_transactions' is not avlbl in azure instance

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'sysprocesses' is not avlbl in azure instance
;with opentran as (select
		spid as session_id,
		max(dbid) as database_id,
		max(open_tran) open_tran,
		max(net_library) as net_transport,
		max(net_address) as client_net_address
	from
		sys.sysprocesses
	group by spid
),
 sess as (select  top 10000	
	sess.session_id,
	min(sess.login_name) as login_name,
	min(sess.host_name) as host_name,	
	min(sess.status) as status,
	min(sess.program_name) as program_name,
	sum(convert(bigint,sess.cpu_time)) as cpu_time,
	sum(convert(bigint,sess.memory_usage)) as memory_usage,
	sum(sess.reads) as reads,
	sum(sess.writes) as writes,
	sum(sess.logical_reads) as logical_reads,
	min(sess.login_time) as login_time,
	min(sess.transaction_isolation_level) as transaction_isolation_level,
	min(last_request_start_time) as last_request_start_time,
	min(last_request_end_time) as last_request_end_time,
	0 as sqlhandle--min(p.sql_handle) as sqlhandle
from
	sys.dm_exec_sessions sess		
	inner join sys.sysprocesses p on sess.session_id = p.spid
group by
	session_id),
--ssu as (
--select
--	ssu.session_id,
--	sessionUserPagesAlloc = CASE WHEN sum(cast(ssu.user_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.user_objects_alloc_page_count as dec(38,0))) END,
--	sessionUserPagesDealloc = CASE WHEN sum(cast(ssu.user_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.user_objects_dealloc_page_count as dec(38,0))) END,
--	taskUserPagesAlloc = CASE WHEN sum(cast(tsu.user_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.user_objects_alloc_page_count as dec(38,0))) END,
--	taskUserPagesDealloc = CASE WHEN sum(cast(tsu.user_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.user_objects_dealloc_page_count as dec(38,0))) END,
--	sessionInternalPagesAlloc = CASE WHEN sum(cast(ssu.internal_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.internal_objects_alloc_page_count as dec(38,0))) END,
--	sessionInternalPagesDealloc = CASE WHEN sum(cast(ssu.internal_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.internal_objects_dealloc_page_count as dec(38,0))) END,
--	taskInternalPagesAlloc = CASE WHEN sum(cast(tsu.internal_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.internal_objects_alloc_page_count as dec(38,0))) END,
--	taskInternalPagesDealloc = CASE WHEN sum(cast(tsu.internal_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.internal_objects_dealloc_page_count as dec(38,0))) END,
--	usingTempdb = sum(cast(ssu.user_objects_alloc_page_count as dec(38,0)) 
--				+ cast(tsu.user_objects_alloc_page_count as dec(38,0))
--				+ cast(ssu.internal_objects_alloc_page_count as dec(38,0))
--				+ cast(tsu.internal_objects_alloc_page_count as dec(38,0))
--				)
--from
--	tempdb.sys.dm_db_session_space_usage ssu
--	left join tempdb.sys.dm_db_task_space_usage tsu
--	on ssu.session_id = tsu.session_id
--group by
--	ssu.session_id),
cte_blocking_count(blocking_session_id,block_count)
as
(
	select 
		blocking_session_id,
		cast(count(*) as int)
	from sys.dm_exec_requests
	group by blocking_session_id
),
TempdbQueries as(
	select  {2}
		sess.session_id,
		sess.login_name,
		sess.host_name,
		status = isnull(req.status,sess.status),
		sess.program_name,
		req.command,
		databaseName = coalesce((select name from sys.databases where database_id=req.database_id),(select name from sys.databases where database_id=ot.database_id)),
		isnull(req.cpu_time,0) + sess.cpu_time as cpu_time,
		sess.memory_usage as memory_usage,
		isnull(req.reads,0) + sess.reads as reads,
		isnull(req.writes,0) + sess.writes as writes,
		isnull(req.logical_reads,0) + sess.logical_reads as logical_reads,
		nullif(req.blocking_session_id,0) as blocking_session_id,
		block_count,
		dateadd(mi,datediff(mi,getdate(),getutcdate()),sess.login_time) as login_time,
		dateadd(mi,datediff(mi,getdate(),getutcdate()),last_request_start_time) as last_request_start_time,
		dateadd(mi,datediff(mi,getdate(),getutcdate()),last_request_end_time) as last_request_end_time,
		null as open_tran,--open_tran,
		ot.net_transport,
		rtrim(ot.client_net_address) as client_net_address,
		wait_time,
		req.request_id,
		last_wait_type,
		wait_type,
		wait_resource,
		null as mostRecentSql,
		 --=case when isnumeric(req.statement_start_offset) = 0 or LEN(CONVERT(nvarchar(MAX), mostRecentSql.text)) * 2 < req.statement_start_offset
			--or req.statement_end_offset < req.statement_start_offset then mostRecentSql.text else 
			--SUBSTRING(mostRecentSql.text,req.statement_start_offset/2,--number of characters
   --         (CASE req.statement_end_offset 
			--  WHEN -1 THEN LEN(CONVERT(nvarchar(MAX), mostRecentSql.text)) * 2
   --           ELSE req.statement_end_offset END
			--	 - req.statement_start_offset)/2 
   --     ) end,
		elapsed_time_seconds,
		sess.transaction_isolation_level,
		null as objectid,--mostRecentSql.objectid,
		null as sessionUserPagesAlloc,--cast(sessionUserPagesAlloc as int) as sessionUserPagesAlloc,
		null as sessionUserPagesDealloc,--cast(sessionUserPagesDealloc as int) as sessionUserPagesDealloc,
		null as taskUserPagesAlloc,--cast(taskUserPagesAlloc as int) as taskUserPagesAlloc,
		null as taskUserPagesDealloc,--cast(taskUserPagesDealloc as int) as taskUserPagesDealloc,
		null as sessionInternalPagesAlloc,--cast(sessionInternalPagesAlloc as int) as sessionInternalPagesAlloc,
		null as sessionInternalPagesDealloc,--cast(sessionInternalPagesDealloc as int) as sessionInternalPagesDealloc,
		null as taskInternalPagesAlloc,--cast(taskInternalPagesAlloc as int) as taskInternalPagesAlloc,
		null as taskInternalPagesDealloc,--cast(taskInternalPagesDealloc as int) as taskInternalPagesDealloc,
		cast(isnull(req.sql_handle,sess.sqlhandle) as varbinary(64)) as sqlhandle
	from
		sess
		left join sys.dm_exec_requests req on sess.session_id = req.session_id
		--left join ssu on sess.session_id = ssu.session_id
		left join cte_blocking_count blk on sess.session_id = blk.blocking_session_id
		left join opentran ot on sess.session_id = ot.session_id
	
		left join #snaps snaps on sess.session_id = snaps.session_id
		outer apply sys.dm_exec_input_buffer( sess.sqlhandle,req.sql_handle) as mostRecentSql
	where
		1=1
		{0}
		)

INSERT INTO #TempdbQueries SELECT * FROM TempdbQueries
--select * into #TempdbQueries from TempdbQueries
		
drop table #snaps

Declare @SQL nvarchar(max)

-- Create a string query to run dbcc inputbuffer for all spids
Set @SQL = (
	SELECT {4} N'Select ''' + Cast(session_id as nvarchar) + ''', '''', ''''; begin try dbcc inputbuffer(' + Cast(session_id as nvarchar) + N') with no_infomsgs end try begin catch IF 1 = HAS_PERMS_BY_NAME(''master'', ''DATABASE'', ''VIEW DATABASE STATE'') BEGIN SELECT event_type, parameters, event_info FROM sys.dm_exec_input_buffer(' + Cast(session_id as nvarchar) + N', NULL) END ELSE BEGIN select ''RPC Event'',0,''(unknown)'' END end catch; '
	FROM #TempdbQueries
	WHERE objectid is not null
	or mostRecentSql is null
	and datalength(rtrim(client_net_address)) <> 0 
	FOR XML PATH(''))
	
-- Execute the string 1 time
Insert Into @DBCCBuffer (EventType, Parameters, EventInfo)
exec sp_executesql @SQL;

declare @nonNullObjects int
declare @start datetime
declare @end datetime


select @start = getdate();
-- Correlate the spids to the dbcc output
-- and then update the temp table
With BufferInfo
As (Select Cast(DB1.EventType as int) As SPID,
		DB2.EventInfo
	From @DBCCBuffer DB1
	Inner Join @DBCCBuffer DB2 On DB2.BufferID = DB1.BufferID + 1
	And IsNumeric(DB2.EventType) = 0 And IsNumeric(DB1.EventType) = 1)
Update TQ
Set objectid = null,
	mostRecentSql =  B.EventInfo
From #TempdbQueries TQ
Inner Join BufferInfo B On TQ.session_id = B.SPID;

select  *
from #TempdbQueries

drop table #TempdbQueries


