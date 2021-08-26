--------------------------------------------------------------------------------
--  TEMP TABLE: Processes Consuming CPU
--  Created Tables: tempdb..#processcpu
--  Tables: master..sysprocesses
--  Purpose: Track processes which are currently consuming CPU
--  Variables:  [0] - sql_handle on supported versions
--------------------------------------------------------------------------------

if (select isnull(object_id('tempdb..#sysprocessnoduplicates'),0)) = 0
begin
	select 
	ident = identity(bigint)
	,spid 
	,loginame 
	,hostname 
	,status 
	,program_name
	,cmd
	,dbid 
	,cpu
	,memusage 
	,physical_io 
	,blocked 
	,login_time 
	,last_batch 
	,open_tran 
	,net_address 
	,net_library 
	,waittime
	,ecid 
	,lastwaittype  
	,waitresource 
	{0}
	into #sysprocessnoduplicates
	from 
	master..sysprocesses
	{1}
	order by spid

	delete from p
		from #sysprocessnoduplicates p, #sysprocessnoduplicates p2
		where p.spid = p2.spid
		and p.ident > p2.ident
end


if (select isnull(object_id('tempdb..#processcpu'), 0)) = 0 
begin 
	create table #processcpu (spid smallint, cpu int) 
	insert into #processcpu 
		select spid, cpu from #sysprocessnoduplicates where cpu > 0 
end



--------------------------------------------------------------------------------
--  QUERY: Processes
--  Tables: #sysprocessnoduplicates, #processcpu
--  Returns:
--		System Processes Consuming CPU
--		User Processes Consuming CPU
--		Blocked Processes
--		Open Transactions
--		User Processes
--		System Processes
--		Distinct Client Computers
--		Lead Blockers
--		Active Sessions
--------------------------------------------------------------------------------
select 
	(select 
		cast(count(*) as int)
	from 
		#sysprocessnoduplicates p 
	where 
		p.cpu > 0 
		and ISNULL(net_address, '') = ''
		and not exists 
			(select * 
			from 
				#processcpu t 
			where 
				t.spid = p.spid 
				and p.cpu <= t.cpu
			)
	), 
	(select 
		cast(count(*) as int)
	from 
		#sysprocessnoduplicates p 
	where 
		p.cpu > 0 
		and ISNULL(net_address, '') <> ''
		and not exists 
			(select * 
			from 
				#processcpu t 
			where 
				t.spid = p.spid 
				and p.cpu <= t.cpu
			)
	), 
	sum(case when s1.blocked > 0 and s1.blocked <> s1.spid then cast(1 as int) else cast(0 as int) end) as blocked, 
	sum(case when s1.open_tran > 0 then cast(1 as int) else cast(0 as int) end), 
	sum(case when ISNULL(s1.net_address, '') <> '' then cast(1 as int) else cast(0 as int) end), 
	sum(case when ISNULL(s1.net_address, '') = ''  then cast(1 as int) else cast(0 as int) end), 
	cast(count(distinct(s1.hostname)) as int) - 1,
	sum(case when s2.blocked = 0 then cast(1 as int) else cast(0 as int) end),
	sum(case when rtrim(s1.status) <> 'sleeping' and ISNULL(s1.net_address, '') <> '' then cast(1 as int) else cast(0 as int) end)
from 
	#sysprocessnoduplicates s1
	left join #sysprocessnoduplicates s2
	on s1.blocked = s2.spid


--------------------------------------------------------------------------------
--  TEMP TABLE: Processes Consuming CPU
--  Created Tables: tempdb..#processcpu
--  Tables: master..sysprocesses
--  Purpose: Track processes which are currently consuming CPU - prepare for next refresh
--------------------------------------------------------------------------------
if (select isnull(object_id('tempdb..#processcpu'), 0)) <> 0 
begin
	truncate table #processcpu 
	insert into #processcpu 
		select 
			spid, 
			cpu 
		from 
			#sysprocessnoduplicates
		where 
			cpu > 0  
end

if (select isnull(object_id('tempdb..#keeptable'), 0)) = 0 
	drop table #sysprocessnoduplicates