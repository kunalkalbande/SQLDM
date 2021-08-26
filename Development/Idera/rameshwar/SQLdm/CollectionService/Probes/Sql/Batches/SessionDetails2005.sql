--------------------------------------------------------------------------------
--  Batch: Session Details
--  Tables: tempdb..SQLdm_Session_Traces, #TraceEvents 
--  Variables:  [0] - Session ID to filter upon
--  [1] - Session trace (if applicable)
--  [2] - use sys.dm_exec_sql_text instead of dbcc inputbuffer if compatibility mode is 90 or above
--		80: EventInfo
--		90: case when isnull(net_address, '') = '' then EventInfo else (select case when dmv.objectid is null then left(max(text),4000) else EventInfo end from sys.dm_exec_sql_text(a.sql_handle) dmv group by dmv.objectid) end
--------------------------------------------------------------------------------

set transaction isolation level read uncommitted 
set lock_timeout 20000 set implicit_transactions off 
if @@trancount > 0 commit transaction
set language us_english
set cursor_close_on_commit off 
set query_governor_cost_limit 0 

use master

declare @sessiondetailscommand nvarchar(300)
declare @SessionDetailsBuffer table  (inputspid int default {0}, EventType nvarchar(260), Parameters int,EventInfo nvarchar(4000)) 
select @sessiondetailscommand = 'dbcc inputbuffer({0})' 

insert into @SessionDetailsBuffer (EventType,Parameters,EventInfo)
	exec (@sessiondetailscommand)


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
		dateadd(mi,datediff(mi,getdate(),getutcdate()),a.last_batch)
	end ,
	open_tran,
	rtrim(net_address),
	rtrim(net_library),
	convert(bigint,waittime),
	a.ecid,
	rtrim(a.lastwaittype),
	rtrim(a.waitresource),
	{2}
 from
   master..sysprocesses a (nolock)
   left join @SessionDetailsBuffer b on a.spid = b.inputspid
 where
   spid = {0}

select 
	isnull(s.row_count,0), 
	isnull(s.reads,0), 
	isnull(s.writes,0), 
	isnull(c.fetch_status,1), 
	(select isnull(count(transaction_id),0) from sys.dm_tran_session_transactions where session_id = {0}) as transactions, 
	isnull(s.prev_error,0), 
	isnull(s.lock_timeout,0), 
	isnull(s.text_size,0), 
	isnull(s.deadlock_priority,0), 
	isnull(s.transaction_isolation_level,0), 
	isnull(s.language,''), 
	isnull(s.quoted_identifier,0), 
	isnull(s.arithabort,0), 
	isnull(s.ansi_null_dflt_on,0), 
	isnull(s.ansi_defaults,0), 
	isnull(s.ansi_warnings,0), 
	isnull(s.ansi_padding,0), 
	isnull(s.ansi_nulls,0), 
	isnull(s.concat_null_yields_null ,0), 
	isnull(r.nest_level,0) 
from 
	sys.dm_exec_sessions s 
	left join sys.dm_exec_cursors({0}) c 
	on s.session_id = c.session_id 
	left join sys.dm_exec_requests r 
	on s.session_id = r.session_id 
Where 
	s.session_id = {0}


{1}



