--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

-- SQL Diagnostic Manager v10.3.0.92
-- Copyright (c) Idera, Inc. 2003-2018 

set transaction isolation level read uncommitted 
 set lock_timeout 20000 
 set implicit_transactions off 
 if @@trancount > 0 commit transaction 
 set language us_english 
 set cursor_close_on_commit off 
 set query_governor_cost_limit 0 
 set numeric_roundabort off
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
--use master

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
declare @MasterDatabaseStatePermission int
SELECT @MasterDatabaseStatePermission = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
--RRG NOTE
-- This is problematic because it requires access to 'master' as currently designed
-- The Azure admin account does not have the required permissions to access 'master'
-- Will need more in depth analysis to determine how this data is being used in SQL DM

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: assigned user doesn't have permissions to 'dm_tran_locks'
IF @MasterDatabaseStatePermission = 1
BEGIN
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
		
		
	group by 
		request_session_id, 
		resource_database_id, 
		request_mode, 
		request_status, 
		resource_type, 
		resource_associated_entity_id),
	distinctWaitingTasks as (select distinct session_id, blocking_session_id from sys.dm_os_waiting_tasks)
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: assigned user doesn't have permissions to 'dm_tran_locks'

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: No insertion into #Locks table as referred tables/ctes are either empty or not avlbl
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
END
IF @MasterDatabaseStatePermission = 1
BEGIN
{5}
END
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: No insertion into #Locks and #LocksCommand table as referred tables/ctes are either empty or not avlbl

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
	SELECT distinct N'Select ''' + Cast(spid as nvarchar) + ''', 0, ''''; begin try dbcc inputbuffer(' + Cast(spid as nvarchar) + N') with no_infomsgs end try begin catch select ''RPC Event'',0,''(unknown)'' end catch; '
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
	
	-- Add status bits
	select @databasestatus  = 0
	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
			--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Single user can't be checked in azure instance
			--or (@databasestatus & 4096 = 4096 
			--	and not exists 
			--	(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
			--	and not exists
			--	(select * from master.sys.dm_tran_locks l where resource_database_id = db_id(@dbname) and l.request_session_id <> @@spid)
			--)
			--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Single user can't be checked in azure instance
			)  --Single User
		)
			select @lookupobjects = 1

	if	@lookupobjects = 1 --and (has_dbaccess (@dbname) <> 1)--SQLdm 10.0 (Tarun Sapra)- has_dbaccess is not avlbl in case of azure db
		select @databasestatus = @databasestatus + 8192, @lookupobjects = 0
	
	--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Following dynamic query can not be executed, if the monitored instance is hosted on azure
	/*if @lookupobjects = 1
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

	end*/
	--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Following dynamic query can not be executed, if the monitored instance is hosted on azure
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
