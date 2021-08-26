--------------------------------------------------------------------------------
--  Batch: Oldest Open Transaction 2005
--  Tables: dm_tran_session_transactions, sysprocesses, dm_tran_active_transactions
--  Variables: [0] - Filter statements for where clause
--	[1] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
--		80: null
--		90: case when isnull(net_address, '') = '' then cast(null as nvarchar(4000)) else (select case when dmv.objectid is null then left(max(text),4000) else cast(null as nvarchar(4000)) end from sys.dm_exec_sql_text(s.sql_handle) dmv group by dmv.objectid) end
--------------------------------------------------------------------------------


declare 
@spid smallint, 
@command nvarchar(255)

if (HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE') = 1)
begin
select top 1
	spid = cast(a.spid as int),
	login_name = rtrim(convert(varchar(255),a.loginame)),
	hostname = rtrim(a.hostname),
	status = rtrim(a.status),
	program_name = rtrim(a.program_name),
	cmd = rtrim(a.cmd),
	dbname = rtrim(db_name(a.dbid)),
	cpu = a.cpu,
	memusage = a.memusage,
	physical_io = a.physical_io,
	blocked = cast(a.blocked as int),
	blocking = (select count(b.spid) from sysprocesses b where b.blocked = a.spid),
	login_time = dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time),
	last_batch = case
	 when a.spid < 5
	   then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate())
	 else
		dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch)
	end ,
	open_tran,
	net_address = rtrim(net_address),
	net_library = rtrim(net_library),
	waittime = convert(bigint,waittime),
	ecid = a.ecid,
	lastwaittype = rtrim(a.lastwaittype),
	waitresource = rtrim(a.waitresource),
	commandtext = {1},
	runtime = datediff(s,database_transaction_begin_time,getdate()),
	transaction_begin_time = dateadd(mi,datediff(mi,getdate(),getutcdate()),transaction_begin_time)
into
	#tempoldestopen
from
	sys.dm_tran_session_transactions t
	left join  sysprocesses a (nolock)
	on t.session_id = a.spid
	left join sys.dm_tran_active_transactions t2
	on t.transaction_id = t2.transaction_id
	inner join sys.dm_tran_database_transactions dtdt 
	on dtdt.transaction_id = t.transaction_id 
where 
	dtdt.database_transaction_begin_lsn is not NULL
	{0}
order by 
	datediff(s,database_transaction_begin_time,getdate()) desc

set @spid = -1

select @spid = spid
from #tempoldestopen
where commandtext is null

if @spid > 0
begin
	declare @OldestOpenBuffer table  (EventType nvarchar(260), Parameters int,EventInfo nvarchar(4000)) 

	select @command = 'dbcc inputbuffer(' + convert(nvarchar(5),@spid) + ')' 

	insert into @OldestOpenBuffer (EventType,Parameters,EventInfo)
		exec (@command)

	update #tempoldestopen
	set commandtext = EventInfo from @OldestOpenBuffer
	where spid = @spid

end


select * from
#tempoldestopen

drop table #tempoldestopen
END
ELSE
BEGIN
	select NULL AS spid
	, NULL AS login_name	
	, NULL AS hostname	
	, NULL AS status	
	, NULL AS program_name	
	, NULL AS cmd	
	, NULL AS dbname	
	, NULL AS cpu	
	, NULL AS memusage	
	, NULL AS physical_io	
	, NULL AS blocked	
	, NULL AS blocking	
	, NULL AS login_time	
	, NULL AS last_batch	
	, NULL AS open_tran	
	, NULL AS net_address	
	, NULL AS net_library	
	, NULL AS waittime	
	, NULL AS ecid	
	, NULL AS lastwaittype	
	, NULL AS waitresource	
	, NULL AS commandtext	
	, NULL AS runtime	
	, NULL AS transaction_begin_time
END
