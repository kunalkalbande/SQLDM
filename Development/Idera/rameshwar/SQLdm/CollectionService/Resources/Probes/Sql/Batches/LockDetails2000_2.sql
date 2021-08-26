--------------------------------------------------------------------------------
--  Batch: Lock Details 2000
--  Tables: master..syslockinfo, master..sysprocesses, master..sysdatabases
--  Variables:  [0] - Tempdb Filter
--  [1] - SPID Filter
--  [2] - Blocking Filter
--	[3] - Lock Counter Statistics Segment
--  [4] - Rowcount Max
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
commandtext nvarchar(4000))

declare @command nvarchar(4000), @dbname nvarchar(300), @dbid int, @databasestatus int, @lookupobjects bit

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support
/*
insert into #Locks
select top {4}
	blocking = cast(coalesce(p2.spid,0) as bit),
    blocked_by = coalesce(p.blocked,0),
    dbid = rsc_dbid,
    host = hostname,
    mode = req_mode,
    program = program_name,
    spid = req_spid,
    status = 
		case
			when req_status = 1 then 'grant'
			when req_status = 2 then 'convert'
			when req_status = 3 then 'wait'
		end,
    type = 	
		case 
			when rsc_type = 2 then 'database'
			when rsc_type = 3 then 'file'
			when rsc_type = 4 then 'index'
			when rsc_type = 5 then 'table'
			when rsc_type = 6 then 'page'
			when rsc_type = 7 then 'key'
			when rsc_type = 8 then 'extent'
			when rsc_type = 9 then 'rid'
			when rsc_type = 10 then 'application'
		 end,
    username = loginame,
    lockcount = l.count,
    waittime,
    objectid = rsc_objid,
	case when req_spid >= 50 then 1 else 0 end
from 
	(select 
		req_spid, 
		rsc_dbid, 
		req_mode, 
		req_status, 
		rsc_type, 
		rsc_objid, 
		convert(dec(38,0),count(*)) as count 
	from 
		master..syslockinfo l
	where 
		req_spid != @@spid
		{0}
	group by 
		req_spid, 
		rsc_dbid, 
		req_mode, 
		req_status, 
		rsc_type, 
		rsc_objid) l
	left join (select distinct spid, blocked, hostname, program_name, loginame, waittime from master..sysprocesses) p
	on l.req_spid = p.spid
	left join (select distinct spid, blocked from master..sysprocesses) p2
	on l.req_spid = p2.blocked
where 
	1=1
	{2}

insert into @LockCommands
	select distinct 
		spid,
		null
	from
		#Locks l
*/
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support

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
		case when isnull(databasepropertyex(@dbname,'IsInStandby'),0) = 1 then 16 else 0 end 									--Standby
		+ case when isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 1 or db.status & 32 = 32 then 32 else 0 end				--Restoring (non-mirror)
		+ case when db.status & 64 = 64 then 64 else 0 end																			--Pre-Recovery
		+ case when isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 1 or db.status & 128 = 128 then 128 else 0 end		--Recovering
		+ case when isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 1 or isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 1 
			or isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 1 or db.status & 256 = 256 then 256 else 0 end				--Suspect
		+ case when isnull(databaseproperty(@dbname, 'IsOffline'),0) = 1 or db.status & 512 = 512 then 512 else 0 end			--Offline
		+ case when isnull(databaseproperty(@dbname, 'IsReadOnly'),0) = 1 or db.status & 1024 = 1024 then 1024 else 0 end		--Read Only
		+ case when isnull(databaseproperty(@dbname, 'IsDboOnly'),0) = 1 or db.status & 2048 = 2048 then 2048 else 0 end		--DBO Use
		+ case when isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 or db.status & 4096 = 4096 then 4096 else 0 end		--Single User
		+ case when isnull(databaseproperty(@dbname, 'IsEmergencyMode'),0) = 1 or db.status & 32768 = 32768 then 32768 else 0 end	--Emergency Mode	
		+ case when db.status & 1073741824 = 1073741824 then 1073741824 else 0 end												--Cleanly Shutdown
	from 
		sysdatabases db
	where 
		db.dbid = @dbid

	if
		(@databasestatus & 8 <> 8 --Restoring Mirror
		and @databasestatus & 32 <> 32 --Restoring
		and @databasestatus & 64 <> 64 --Pre-Recovery
		and @databasestatus & 128 <> 128 --Recovering
		and @databasestatus & 256 <> 256 --Suspect
		and @databasestatus & 512 <> 512 --Offline
		and (@databasestatus & 4096 <> 4096
			--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'sysprocesses' and 'syslockinfo' are not avlbl
			--or (@databasestatus & 4096 = 4096 
			--	and not exists 
			--	(select * from master..sysprocesses p where dbid = @dbid and p.spid <> @@spid)
			--	and not exists
			--	(select * from master..syslockinfo l where rsc_dbid = @dbid and l.req_spid <> @@spid)
			--)
			--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: 'sysprocesses' and 'syslockinfo' are not avlbl
			)  --Single User
		)
			select @lookupobjects = 1

	if	@lookupobjects = 1 --and (has_dbaccess (@dbname) <> 1)
		select @databasestatus = @databasestatus + 8192, @lookupobjects = 0
	
	--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: Following dynamic query can not be executed, if the monitored instance is hosted on azure
	/*if @lookupobjects = 1
	begin

		set @command = 
		'use [' + replace(@dbname,char(93),char(93)+char(93)) + '] 

		insert into #LockObjects 
		select distinct
			db_id(),
			id,
			objectname = name,
			objectschema = user_name(uid)	
		from
			#Locks l
			left join sysobjects o
			on l.objectid = o.id
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
    mode = convert(nvarchar(386),substring (v.name, 1, 8)),
	objectname,
	objectschema,    
	program,
    l.spid,
    l.status,
    l.type,
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
left join spt_values v
on convert(int,mode) + 1 = v.number and v.type = 'L' 


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