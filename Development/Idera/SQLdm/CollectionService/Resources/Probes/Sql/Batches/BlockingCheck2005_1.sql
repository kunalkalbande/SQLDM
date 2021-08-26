--------------------------------------------------------------------------------
--  Batch: Blocking Check 2005
--  Tables: sysprocesses, syslockinfo, spt_values
--  DBCC: DBCC Inputbuffer
--  Variables: 
--  [0] - wait time
--  [1] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
--  [2] - filtering SQL dm app
--    80: null
--    90: case when isnull(p.net_address, '') = '' then null else (select case when dmv.objectid is null then left(max(text),4000) else null end from sys.dm_exec_sql_text(p.sql_handle) dmv group by dmv.objectid) end
--------------------------------------------------------------------------------

declare @tempblocking table(
spid int,
program nchar(256),
host nvarchar(256),
login nvarchar(256),
databasename nvarchar(1000),
object int,
request_mode nvarchar(256),
blockedwait bigint,
inputbuffer nvarchar(4000),
starttime datetime,
blocking_last_batch datetime, 
waitresource nchar(256))

-- Minimum support for Azure
if exists (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[master].[dbo].[spt_values]') )
begin

insert into @tempblocking
select distinct
	spid = cast(p.spid as int), 
	program = p.program_name, 
	host = p.hostname, 
	login = p.loginame, 
	databasename = replace(db_name(ISNULL(convert(int,l.rsc_dbid),-1)),char(39),char(39)+char(39)), 
	object = ISNULL(l.rsc_objid,-1), 
	request_mode = substring (v.name, 1, 8),
	blockedwait = b.waittime,
	inputbuffer = {1},
	starttime = dateadd(ss,cast(b.waittime as float)/1000 * -1,getutcdate()),
	p.last_batch,
	b.waitresource
from 
	master..sysprocesses p (nolock) 
	left join master..sysprocesses b (nolock) 
		on p.spid = b.blocked 
	left outer join master..syslockinfo l (nolock) 
		on b.spid = l.req_spid 
		and l.req_status in (2,3)
	left join master.dbo.spt_values v
	on convert(int,l.[req_mode]) + 1 = v.number 
where 
	p.blocked = 0 
	and p.program_name not like 'SQLAgent%' 
	and p.program_name not in ('SQL PerfMon') 
	{2}
	and b.blocked <> 0 
	and b.waittime > {0} 
	and v.type = 'L' 

end
else
begin
	Print 'The table master.dbo.spt_values does not exists on this system.'
end

declare @inputbufferblockingcheck table (
inputspid smallint default -1, 
EventType nvarchar(260), 
Parameters int,
EventInfo nvarchar(4000)) 

declare @blockingcheckspid smallint, @blockingcheckcommand nvarchar(255)
declare read_inputbuffer_blockingcheck insensitive cursor 
for
	select 
		spid
	from
		@tempblocking
	where 
		isnull(inputbuffer,'') = ''
for read only
set nocount on 
open read_inputbuffer_blockingcheck 
fetch read_inputbuffer_blockingcheck into @blockingcheckspid
while @@fetch_status = 0 
begin 

  IF(IS_SRVROLEMEMBER('sysadmin') = 1)
  BEGIN
	set @blockingcheckcommand = 'dbcc inputbuffer(' + convert(nvarchar(5),@blockingcheckspid) + ')'
	insert into @inputbufferblockingcheck (EventType,Parameters,EventInfo)
		exec (@blockingcheckcommand)
	update @tempblocking 
		set inputbuffer = EventInfo from @inputbufferblockingcheck
		where spid = @blockingcheckspid

	delete from @inputbufferblockingcheck
  END
fetch read_inputbuffer_blockingcheck into @blockingcheckspid
end
close read_inputbuffer_blockingcheck 
deallocate read_inputbuffer_blockingcheck 

select * 
from @tempblocking

