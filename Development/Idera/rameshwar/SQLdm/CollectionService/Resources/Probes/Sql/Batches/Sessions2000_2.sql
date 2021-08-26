--------------------------------------------------------------------------------
--  Batch: Sessions 2000
--  Tables: sysprocesses
--  Variables:  [0] - Process types to return
--	[1] - Session Count Segment
--  [2] - Session max rowcount
--  [3] - Inputbuffer limiter
--  [4] - SQLdm application name used at order time
--------------------------------------------------------------------------------
-- use master


if (select isnull(object_id('tempdb..#keeptable'), 0)) = 0 
begin 
	create table #keeptable (i int) 
end


{1}

--------------------------------------------------------------------------------
--  Query: Process List
--  Tables: Based on Process List Cursor
--  Returns:
--    SPID
--    Login name
--    Name of the workstation
--    Status
--    Name of application program
--    Command being executed
--    Name of database being used by the process
--    Cumulative CPU time
--    Number of procedure cache pages allocated to process - a negative number
--      indicates that the process is freeing memory allocated by another process.
--    Physical IO - Cumulative disk reads and writes
--    SPID of blocking process (0 if not blocked)
--    Count of blocked processes
--    Time at which a client process logged into the server. For system
--      processes, the time at which SQL Server startup occurred is stored.
--    Date/time of last batch
--    Time at which a client process logged into the server - formatted
--    Number of open transactions in process
--    Net-Address of workstation
--    Net-Library of session
--    Current wait time in milliseconds
--    ECID - Execution Context
--	  Last Wait Type
--	  Wait Resource
--    Last Command
--------------------------------------------------------------------------------
select {2}
	spid = cast(a.spid as int), 
	loginname = rtrim(convert(varchar(255),a.loginame)), 
	hostname = rtrim(a.hostname), 
	status = rtrim(a.status), 
	program_name = rtrim(a.program_name), 
	cmd = rtrim(a.cmd), 
	dbname = rtrim(db_name(a.dbid)), 
	cpu = a.cpu, 
	memusage = a.memusage, 
	physical_io = a.physical_io, 
	blocked = case when a.blocked = a.spid then 0 else cast(a.blocked as int) end, 
	blocking = (select count(b.spid) from #sysprocessnoduplicates b where b.blocked = a.spid and b.blocked <> b.spid),
	login_time = dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time), 
	last_batch = case 
	when a.spid < 5 or a.last_batch < '1/1/1990' 
	 then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate()) 
	else 
	dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch) 
	end , 
	open_tran = open_tran, 
	net_address = rtrim(net_address), 
	net_library = rtrim(net_library), 
	waittime = convert(bigint,waittime), 
	ecid = a.ecid, 
	lastwaittype =rtrim(a.lastwaittype), 
	waitresource =rtrim(a.waitresource)
 into #tempprocess
 from
   #sysprocessnoduplicates a (nolock)
 where
   1 = 1
   /*{0}*/
 
drop table #sysprocessnoduplicates

select * from #tempprocess

declare @spid int, @login_time datetime, @command nvarchar(255)
declare read_inputbuffer insensitive cursor 
for
	select {3}
		spid,
		login_time
	from
		#tempprocess
	where 
		isnull(net_address,'') <> ''
	order by
		case when program_name like {4} then 0 else 1 end desc,
		blocking desc,
		blocked desc,
		waittime desc,
		last_batch desc,
		open_tran desc,
		memusage desc,
		cpu desc,
		physical_io desc

--START: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: The cmd formed can not be executed in azure instance
--for read only
--set nocount on 
--open read_inputbuffer 
--fetch read_inputbuffer into @spid, @login_time
--while @@fetch_status = 0 
--begin 
	
--	set @command = 'dbcc inputbuffer(' + convert(nvarchar(10),@spid) + ')'
	
--	select @spid, @login_time	
--	exec (@command)

--fetch read_inputbuffer into @spid, @login_time
--end
--close read_inputbuffer 
--deallocate read_inputbuffer 
--END: SQLdm 10.0 (Tarun Sapra)- Minimal Cloud Support: The cmd formed can not be executed in azure instance

drop table #tempprocess
drop table #keeptable