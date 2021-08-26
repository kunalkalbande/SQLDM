--------------------------------------------------------------------------------
--  Batch: Sessions 2005
--  Variables:  [0] - Process types to return
--	[1] - Session Count Segment
--  [2] - Session max rowcount
--  [3] - use tempdb if master compatibility mode is 80 or below
--  [4] - Inputbuffer limiter
--------------------------------------------------------------------------------
use {3}


{1}

declare @TempdbQueries 
table
(
	session_id smallint
	,login_name nvarchar(128)
	,host_name nvarchar(128)
	,status nvarchar(30)
	,program_name nvarchar(128)
	,command nvarchar(16)
	,databaseName nvarchar(128)
	,cpu_time int
	,memory_usage int
	,reads bigint
	,writes bigint
	,logical_reads bigint
	,blocking_session_id smallint
	,block_count int
	,login_time datetime
	,last_request_start_time datetime
	,last_request_end_time datetime
	,open_tran smallint
	,net_transport nvarchar(40)
	,client_net_address varchar(48)
	,wait_time int
	,request_id int
	,last_wait_type nvarchar(60)
	,wait_type nvarchar(60)
	,wait_resource nvarchar(256)
	,mostRecentSql nvarchar(max)
	,elapsed_time_seconds bigint
	,transaction_isolation_level smallint
	,objectid int
	,sessionUserPagesAlloc int
	,sessionUserPagesDealloc int
	,taskUserPagesAlloc int
	,taskUserPagesDealloc int
	,sessionInternalPagesAlloc int
	,sessionInternalPagesDealloc int
	,taskInternalPagesAlloc int
	,taskInternalPagesDealloc int
)
	
declare @DBCCBuffer 
table  
(
	EventType nvarchar(260), 
	Parameters int,
	EventInfo nvarchar(4000)
) 



create table #snaps(session_id bigint primary key clustered, elapsed_time_seconds bigint)

declare @snapshotdbs int
declare @maxInt int
Select @maxInt = cast(Power(cast(2 as varchar),(32) -1) - 1 as int)

select @snapshotdbs =  isnull(count(database_id),0) from sys.databases
	where snapshot_isolation_state in (1,3)
	or is_read_committed_snapshot_on = 1
	
if @snapshotdbs > 0	
begin

	insert into #snaps
	select 
	session_id,
	elapsed_time_seconds
	from sys.dm_tran_active_snapshot_database_transactions snaps 
	where snaps.is_snapshot = 1
	and elapsed_time_seconds > 0

end


create table #SSU(
	session_id int primary key clustered,
	sessionUserPagesAlloc dec(38,0),
	sessionUserPagesDealloc dec(38,0),
	taskUserPagesAlloc dec(38,0),
	taskUserPagesDealloc dec(38,0),
	sessionInternalPagesAlloc dec(38,0),
	sessionInternalPagesDealloc dec(38,0),
	taskInternalPagesAlloc dec(38,0),
	taskInternalPagesDealloc dec(38,0),
	usingTempdb  dec(38,0)
	)


insert into #SSU
select
	ssu.session_id,
	sessionUserPagesAlloc = CASE WHEN sum(cast(ssu.user_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.user_objects_alloc_page_count as dec(38,0))) END,
	sessionUserPagesDealloc = CASE WHEN sum(cast(ssu.user_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.user_objects_dealloc_page_count as dec(38,0))) END,
	taskUserPagesAlloc = CASE WHEN sum(cast(tsu.user_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.user_objects_alloc_page_count as dec(38,0))) END,
	taskUserPagesDealloc = CASE WHEN sum(cast(tsu.user_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.user_objects_dealloc_page_count as dec(38,0))) END,
	sessionInternalPagesAlloc = CASE WHEN sum(cast(ssu.internal_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.internal_objects_alloc_page_count as dec(38,0))) END,
	sessionInternalPagesDealloc = CASE WHEN sum(cast(ssu.internal_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(ssu.internal_objects_dealloc_page_count as dec(38,0))) END,
	taskInternalPagesAlloc = CASE WHEN sum(cast(tsu.internal_objects_alloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.internal_objects_alloc_page_count as dec(38,0))) END,
	taskInternalPagesDealloc = CASE WHEN sum(cast(tsu.internal_objects_dealloc_page_count as dec(38,0))) > @maxInt THEN 0 ELSE sum(cast(tsu.internal_objects_dealloc_page_count as dec(38,0))) END,
	usingTempdb = sum(cast(ssu.user_objects_alloc_page_count as dec(38,0)) 
				+ cast(tsu.user_objects_alloc_page_count as dec(38,0))
				+ cast(ssu.internal_objects_alloc_page_count as dec(38,0))
				+ cast(tsu.internal_objects_alloc_page_count as dec(38,0))
				)
from
	sys.dm_db_session_space_usage ssu
	left join sys.dm_db_task_space_usage tsu
	on ssu.session_id = tsu.session_id
group by
	ssu.session_id	

create table #opentran(session_id bigint primary key clustered,database_id int,open_tran bigint)

insert into #opentran
	select
		spid,
		dbid,
		max(open_tran)
	from
		sys.sysprocesses
	group by
		spid,
		dbid
		
create table #sess(
	session_id smallint primary key clustered, 
	login_name nvarchar(128), 
	host_name nvarchar(128), 
	status nvarchar(30), 
	program_name nvarchar(128),
	cpu_time dec(38,0), 
	memory_usage dec(38,0),
	reads dec(38,0),
	writes dec(38,0),
	logical_reads dec(38,0),
	login_time datetime,
	transaction_isolation_level smallint, 
	last_request_start_time datetime, 
	last_request_end_time datetime)		
		
insert into #sess	
select {2}	
	sess.session_id,
	min(sess.login_name),
	min(sess.host_name),	
	min(sess.status),
	min(sess.program_name),
	sum(sess.cpu_time),
	sum(sess.memory_usage),
	sum(sess.reads),
	sum(sess.writes),
	sum(sess.logical_reads),
	min(sess.login_time),
	min(sess.transaction_isolation_level),
	min(last_request_start_time),
	min(last_request_end_time)
from
	sys.dm_exec_sessions sess	
group by
	session_id	
		
;with 
cte_blocking_count(blocking_session_id,block_count)
as
(
	select 
		blocking_session_id,
		cast(count(*) as int)
	from sys.dm_exec_requests
	group by blocking_session_id
)
insert into @TempdbQueries
	select {2}
		sess.session_id,
		sess.login_name,
		sess.host_name,
		status = isnull(req.status,sess.status),
		sess.program_name,
		req.command,
		databaseName = coalesce(db_name(req.database_id),db_name(ot.database_id)),
		sess.cpu_time,
		sess.memory_usage,
		sess.reads,
		sess.writes,
		sess.logical_reads,
		nullif(req.blocking_session_id,0),
		block_count,
		sess.login_time,
		dateadd(mi,datediff(mi,getdate(),getutcdate()),last_request_start_time),
		dateadd(mi,datediff(mi,getdate(),getutcdate()),last_request_end_time),
		open_tran,
		net_transport,
		client_net_address,
		wait_time ,
		req.request_id,
		last_wait_type,
		wait_type,
		wait_resource,
		mostRecentSql = mostRecentSql.text,
		elapsed_time_seconds,
		sess.transaction_isolation_level,
		mostRecentSql.objectid,
		sessionUserPagesAlloc,
		sessionUserPagesDealloc,
		taskUserPagesAlloc,
		taskUserPagesDealloc,
		sessionInternalPagesAlloc,
		sessionInternalPagesDealloc,
		taskInternalPagesAlloc,
		taskInternalPagesDealloc
	
	from
		#sess sess
		left join sys.dm_exec_connections conn
		on conn.session_id = sess.session_id
		left join sys.dm_exec_requests req
		on sess.session_id = req.session_id
		left join #SSU ssu
		on sess.session_id = ssu.session_id
		left join cte_blocking_count blk
		on sess.session_id = blk.blocking_session_id
		left join #opentran ot
		on sess.session_id = ot.session_id
		left join #snaps snaps 
		on sess.session_id = snaps.session_id
		outer apply sys.dm_exec_sql_text(conn.most_recent_sql_handle) as mostRecentSql
	where
		1=1
		{0}
		
drop table #snaps
drop table #SSU
drop table #opentran
drop table #sess		
		
declare @spid int, @counter int
set @counter = 1
select @spid = min(session_id) from @TempdbQueries where objectid is not null or mostRecentSql is null and datalength(rtrim(client_net_address)) <> 0 
while @spid is not null
begin

set @counter = @counter + 1

begin try
	insert into @DBCCBuffer
	exec sp_executesql N'dbcc inputbuffer(@spid) with no_infomsgs',N'@spid int',@spid;

	update @TempdbQueries 
		set objectid = null,
		mostRecentSql =  EventInfo
	from @DBCCBuffer
		where session_id = @spid
		
end try
begin catch
	update @TempdbQueries set objectid = null, mostRecentSql = '(unknown)' where session_id = @spid
end catch

set @spid = null

select @spid = min(session_id) from @TempdbQueries where objectid is not null or mostRecentSql is null and datalength(rtrim(client_net_address)) <> 0 

if @counter > {4}
	set @spid = null

end


select  *
from @TempdbQueries




