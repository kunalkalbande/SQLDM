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

create clustered index IX_LockIndex on #Locks(dbid,spid,objectid)

create table #LockObjects(
	dbid int,
	objectid bigint,
	objectname nvarchar(300),
	objectschema nvarchar(300))

create clustered index IX_LockObjectsIndex on #LockObjects(dbid,objectid)

declare @LockCommands table (
	spid int,
	commandtext nvarchar(max))

declare @command nvarchar(4000), @dbname nvarchar(300), @dbid int, @databasestatus int, @lookupobjects bit

;with tranLocks as (select 
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
		request_session_id != convert(int,@@spid)
		{0}
		{1}
	group by 
		request_session_id, 
		resource_database_id, 
		request_mode, 
		request_status, 
		resource_type, 
		resource_associated_entity_id),
	distinctWaitingTasks as (select distinct session_id, blocking_session_id from sys.dm_os_waiting_tasks)

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
from tranLocks l
	left join sys.dm_os_waiting_tasks w	on l.request_session_id = w.session_id 
	left join distinctWaitingTasks w2 on l.request_session_id = w2.blocking_session_id 
	left join sys.dm_exec_sessions s on l.request_session_id = s.session_id
where 
	1=1
	{2}

{5}

Declare @SQL nvarchar(max)
create table #DBCCBuffer
(
	EventType nvarchar(260), 
	Parameters int,
	EventInfo nvarchar(4000),
	BufferID int identity(1, 1) not null primary key
) 

-- Create a string query to run dbcc inputbuffer for all spids
Set @SQL = (
	SELECT distinct N'Select ''' + Cast(spid as nvarchar) + ''', 0, ''''; begin try dbcc inputbuffer(' + Cast(spid as nvarchar) + N') with no_infomsgs end try begin catch SELECT event_type,parameters,event_info FROM sys.dm_exec_input_buffer(' + Cast(spid as nvarchar) + N', NULL) end catch; '
		from @LockCommands
	where commandtext is null and spid is not null
	FOR XML PATH(''))
	--select @SQL
-- Execute the string 1 time
Insert Into #DBCCBuffer (EventType, Parameters, EventInfo)
exec sp_executesql @SQL;

-- Correlate the spids to the dbcc output
-- and then update the @lockCommands table
;with BufferInfo As (Select Cast(DB1.EventType as int) As SPID,
		DB2.EventInfo
	From #DBCCBuffer DB1
	Inner Join #DBCCBuffer DB2 On DB2.BufferID = DB1.BufferID + 1
	And IsNumeric(convert(varchar(520),DB2.EventType)) = 0 And IsNumeric(convert(varchar(520),DB1.EventType)) = 1)
Update l
Set l.commandtext =  case when substring(B.EventInfo,len(B.EventInfo)-2,2) = ';1' then left(B.EventInfo,len(B.EventInfo)-2) else B.EventInfo end
From @LockCommands l
Inner Join BufferInfo B On l.spid = B.SPID

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
		+ case when isnull(mirr.mirroring_role,0) <> 2 and (isnull(databasepropertyex(@dbname, 'IsInLoad'),0) = 1 
			or isnull(db.state,0) = 1) then 32 else 0 end																		--Restoring (non-mirror)
		+ case when isnull(db.state,0) = 3 then 64 else 0 end																	--Pre-Recovery
		+ case when isnull(databasepropertyex(@dbname, 'IsInRecovery'),0) = 1 or isnull(db.state,0) = 2 then 128 else 0 end		--Recovering
		+ case when isnull(databasepropertyex(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databasepropertyex(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databasepropertyex(@dbname, 'IsShutDown'),0) = 1 or isnull(db.state,0) = 4 then 256 else 0 end				--Suspect
		+ case when isnull(databasepropertyex(@dbname, 'IsOffline'),0) = 1 or isnull(db.state,0) = 6 then 512 else 0 end			--Offline
		+ case when isnull(databasepropertyex(@dbname, 'IsReadOnly'),0) = 1 or isnull(db.is_read_only,0) = 1 then 1024 else 0 end --Read Only
		+ case when isnull(databasepropertyex(@dbname, 'IsDboOnly'),0) = 1 then 2048 else 0 end									--DBO Use
		+ case when isnull(databasepropertyex(@dbname, 'IsSingleUser'),0) = 1 or db.user_access = 1 then 4096 else 0 end			--Single User
		+ case when isnull(databasepropertyex(@dbname, 'IsEmergencyMode'),0) = 1 or isnull(db.state,0) = 5 then 32768 else 0 end	--Emergency Mode	
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
where lc.commandtext <> '(unknown)'

drop table #Locks
drop table #LockObjects
drop table #DBCCBuffer

Declare @sysperfinfoname sysname

Set @sysperfinfoname = Convert(nvarchar(128), ServerProperty('InstanceName'))
If @sysperfinfoname Is Null
       Set @sysperfinfoname = N'sqlserver';
Else
       Set @sysperfinfoname = N'mssql$' + lower(@sysperfinfoname);


{3}
