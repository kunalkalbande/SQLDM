--------------------------------------------------------------------------------
--  Batch: Blocking Check 2000
--  [1] - filtering SQL dm app
--------------------------------------------------------------------------------

if (select isnull(object_id('tempdb..#inputbuffer_blocking'), 0)) = 0 
begin 
	create table #inputbuffer_blocking (
		spid smallint default -1, 
		EventType nvarchar(260), 
		Parameters int,
		EventInfo nvarchar(260)) 
end

declare read_inputbuffer_blocking insensitive cursor 
for
	select distinct
		p.blocked
	from
		master..sysprocesses p (nolock) 
	where p.blocked <> 0
for read only
set nocount on 
open read_inputbuffer_blocking 
fetch read_inputbuffer_blocking into @spid
while @@fetch_status = 0 
begin 
	set @command = 'dbcc inputbuffer(' + convert(nvarchar(5),@spid) + ')'
	insert into #inputbuffer_blocking (EventType,Parameters,EventInfo)
		exec (@command)
	update #inputbuffer_blocking set spid = @spid where spid = -1
fetch read_inputbuffer_blocking into @spid
end
close read_inputbuffer_blocking 
deallocate read_inputbuffer_blocking 

select distinct
	spid = cast(p.spid as int), 
	program = p.program_name, 
	host = p.hostname, 
	login = p.loginame, 
	databasename = replace(db_name(ISNULL(convert(int,l.rsc_dbid),-1)),char(39),char(39)+char(39)), 
	object = ISNULL(l.rsc_objid,-1), 
	request_mode = substring (v.name, 1, 8),
	blockedwait = b.waittime,
	inputbuffer = i.EventInfo,
	starttime = dateadd(ms,b.waittime * -1,getutcdate()),
	p.last_batch,
	b.waitresource
from 
	master..sysprocesses p (nolock) 
	full join master..sysprocesses b (nolock) 
		on p.spid = b.blocked 
	left outer join master..syslockinfo l (nolock) 
		on b.spid = l.req_spid 
		and l.req_status in (2,3)
	left join #inputbuffer_blocking i
		on p.spid = i.spid,
	master.dbo.spt_values v
where 
	p.blocked = 0 
	and p.program_name not like 'SQLAgent%' 
	{1}
	and p.program_name not in ('SQL PerfMon') 
	and b.blocked <> 0 
	and b.waittime > {0} 
	and convert(int,l.[req_mode]) + 1 = v.number 
	and v.type = 'L' 

drop table #inputbuffer_blocking
