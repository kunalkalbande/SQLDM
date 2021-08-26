--------------------------------------------------------------------------------
--  Batch: Session Details
--  Tables: tempdb..SQLdm_Session_Traces, #TraceEvents 
--  Variables:  [0] - Session ID to filter upon
--  [1] - Session trace (if applicable)
--------------------------------------------------------------------------------

set transaction isolation level read uncommitted 
set lock_timeout 20000 set implicit_transactions off 
if @@trancount > 0 commit transaction
set language us_english
set cursor_close_on_commit off 
set query_governor_cost_limit 0 

use master

select
	cast(a.spid as int),
	rtrim(convert(varchar(255),a.loginame)),
	rtrim(a.hostname),
	rtrim(a.status),
	rtrim(a.program_name),
	rtrim(a.cmd),
	rtrim(db_name(a.dbid)),
	a.cpu,
	a.memusage,
	a.physical_io,
	case when a.blocked = a.spid then 0 else cast(a.blocked as int) end,
	(select count(b.spid) from master..sysprocesses b where b.blocked = a.spid),
	dateadd(mi,datediff(mi,getdate(),getutcdate()),a.login_time),
	case
	 when a.spid < 5
	   then dateadd(mi,datediff(mi,getdate(),getutcdate()),getdate())
	 else
		dateadd(hh,datediff(hh,a.last_batch,getutcdate()),a.last_batch)
	end ,
	open_tran,
	rtrim(net_address),
	rtrim(net_library),
	convert(bigint,waittime),
	a.ecid,
	rtrim(a.lastwaittype),
	rtrim(a.waitresource)
 from
   master..sysprocesses a (nolock)
 where
   spid = {0}

-- Return inpubuffer
dbcc inputbuffer ({0}) 

-- Set traceon
dbcc traceon(3604)  

-- Return DBCC PSS data
if charindex ('Desktop Edition', @@version) = 0 
	dbcc pss(0, {0}, 0) 

{1}