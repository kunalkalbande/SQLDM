--------------------------------------------------------------------------------
--  Batch: Lock Details 2005
--  Tables: sys.dm_tran_locks, master.sys.sysprocesses, master..sysdatabases,
--	sys.partitions, sys.objects, sys.tables 
--  Variables:  [0] - Tempdb Filter
--  [1] - SPID Filter
--  [2] - Blocking Filter
--	[3] - Lock Counter Statistics Segment
--  [4] - Rowcount limiter
--  [5] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
--		80: null
--		90: case when isnull(net_address, '') = '' then null else (select case when dmv.objectid is null then left(max(text),4000) else null end from sys.dm_exec_sql_text(s.sql_handle) dmv group by dmv.objectid) end
--------------------------------------------------------------------------------
use master

create table #Locks(
blocking bit,
blocked_by int,
dbid int,
host nvarchar(300),
mode nvarchar(300),
program nvarchar(300),
spid int,
status nvarchar(300),
type nvarchar(300),
username nvarchar(300),
lockcount decimal(38,0),
waittime bigint,
objectid bigint,
userprocess bit
)

create clustered index IX_LockIndex on #Locks(dbid,objectid)

create table #LockObjects(
	dbid int,
	objectid bigint,
	objectname nvarchar(300),
	objectschema nvarchar(300))

declare @LockCommands table(
	spid int,
	commandtext nvarchar(max))

declare @command nvarchar(4000), @dbname nvarchar(300), @dbid int, @databasestatus int, @lookupobjects bit

insert into #Locks
select top {4}
	blocking = cast(coalesce(w2.session_id,0) as bit),
    blocked_by = coalesce(w.blocking_session_id,0),
    dbid = l.resource_database_id,
    host = s.host_name,
    mode = l.request_mode,
    program = s.program_name,
    spid = l.request_session_id,
    status = l.request_status,
    type = lower(l.resource_type),
    username = s.login_name,
    lockcount = l.count,
    waittime = isnull(w.wait_duration_ms,0),
    objectid = resource_associated_entity_id,
	is_user_process
from 
	(select 
		request_session_id, 
		resource_database_id, 
		request_mode, 
		request_status, 
		resource_type, 
		resource_associated_entity_id, 
		convert(dec(38,0),count(*)) as count 
	from 
		sys.dm_tran_locks 
	where 
		request_session_id != @@spid
		{0}
		{1}
	group by 
		request_session_id, 
		resource_database_id, 
		request_mode, 
		request_status, 
		resource_type, 
		resource_associated_entity_id) l
	left join sys.dm_os_waiting_tasks w
		on l.request_session_id = w.session_id 
	left join 
		(select distinct session_id, blocking_session_id from sys.dm_os_waiting_tasks) w2
		on l.request_session_id = w2.blocking_session_id 
	left join sys.dm_exec_sessions s
		on l.request_session_id = s.session_id
where 
	1=1
	{2}

{5}

if (select isnull(object_id('tempdb..#inputbuffer'), 0)) = 0 
begin 
	create table #inputbuffer (inputspid smallint default -1, EventType nvarchar(260), Parameters int,EventInfo nvarchar(4000)) 
end

declare @spid smallint
declare read_inputbuffer insensitive cursor 
for
	select distinct
		spid 
	from
		@LockCommands
	where 
		commandtext is null
for read only
set nocount on 
open read_inputbuffer 
fetch read_inputbuffer into @spid
while @@fetch_status = 0 
begin 
	set @command = 'dbcc inputbuffer(' + convert(nvarchar(5),@spid) + ')'
	insert into #inputbuffer (EventType,Parameters,EventInfo)
		exec (@command)
	update @LockCommands
		set commandtext = case when substring(EventInfo,len(EventInfo)-2,2) = ';1' then left(EventInfo,len(EventInfo)-2) else EventInfo end from #inputbuffer
		where spid = @spid
	truncate table #inputbuffer
fetch read_inputbuffer into @spid
end
close read_inputbuffer 
deallocate read_inputbuffer 

drop table #inputbuffer

declare read_db_status insensitive cursor 
for 
	select distinct 
		db_name(dbid),
		dbid
	from 
		#Locks
	where
		isnull(objectid,0) > 0
		and len(isnull(db_name(dbid),'')) > 0
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname, @dbid
while @@fetch_status = 0 
begin 
	
	select @databasestatus  = 0

	-- Add status bits
	select @databasestatus = 
		case when isnull(mirr.mirroring_role,0) = 2 and isnull(db.state,0) = 1 then 8 else 0 end								--Restoring Mirror
		+ case when isnull(databasepropertyex(@dbname,'IsInStandby'),0) = 1 or db.is_in_standby = 1 then 16 else 0 end 			--Standby
		+ case when isnull(mirr.mirroring_role,0) <> 2 and (isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 1 
			or isnull(db.state,0) = 1) then 32 else 0 end																		--Restoring (non-mirror)
		+ case when isnull(db.state,0) = 3 then 64 else 0 end																	--Pre-Recovery
		+ case when isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 1 or isnull(db.state,0) = 2 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 1 or isnull(db.state,0) = 4 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@dbname, 'IsOffline'),0) = 1 or isnull(db.state,0) = 6 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@dbname, 'IsReadOnly'),0) = 1 or isnull(db.is_read_only,0) = 1 then 1024 else 0 end --Read Only
		+ case when isnull(databaseproperty(@dbname, 'IsDboOnly'),0) = 1 then 2048 else 0 end									--DBO Use
		+ case when isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 or db.user_access = 1 then 4096 else 0 end			--Single User
		+ case when isnull(databaseproperty(@dbname, 'IsEmergencyMode'),0) = 1 or isnull(db.state,0) = 5 then 32768 else 0 end	--Emergency Mode	
		+ case when isnull(db.is_cleanly_shutdown,0) = 1 then 1073741824 else 0 end												--Cleanly Shutdown
	from 
		master.sys.databases db
		left outer join sys.database_mirroring mirr 
		on mirr.database_id = db.database_id
	where 
		db.database_id = db_id(@dbname)

	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
			or (@databasestatus & 4096 = 4096 
				and not exists 
				(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = db_id(@dbname) and l.request_session_id <> @@spid)
			))  --Single User
		)
			select @lookupobjects = 1

	if	@lookupobjects = 1 and (has_dbaccess (@dbname) <> 1)
		select @databasestatus = @databasestatus + 8192, @lookupobjects = 0
	
	if @lookupobjects = 1
	begin

		set @command = 
		'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] 
		update #Locks
		set
			objectid = object_id
		from
			#Locks l
			left join sys.partitions p
			on l.objectid = p.hobt_id
		where
			lower(l.type) in (''page'', ''pag'', ''key'', ''rid'', ''hbt'', ''hobt'')
			and l.dbid = db_id()

		insert into #LockObjects 
		select distinct
			db_id(),
			object_id,
			objectname = name,
			objectschema = schema_name(schema_id)	
		from
			#Locks l
			left join sys.all_objects o
			on l.objectid = o.object_id
		where
			l.dbid = db_id()
			and name is not null'

		exec sp_executesql @command

	end

fetch read_db_status into @dbname,@dbid
End 
Close read_db_status 
deallocate read_db_status 

select 
	blocking,
    blocked_by,
    db_name(l.dbid),
    host,
    mode,
	objectname,
	objectschema,    
	program,
    l.spid,
    status,
    type,
    username,
    lockcount,
    waittime,
	commandtext
from #Locks l 
left join #LockObjects lo
on 
l.objectid = lo.objectid 
and l.dbid = lo.dbid
left join @LockCommands lc
on l.spid = lc.spid


drop table #Locks
drop table #LockObjects

declare 
@servername varchar(255), 
@sysperfinfoname varchar(255),
@slashpos int

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

{3}
