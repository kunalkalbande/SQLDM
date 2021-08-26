--------------------------------------------------------------------------------
--  Batch: Stop Session Details Trace 2000
--  Tables: tempdb..SQLdm_Session_Traces 
--  Variables: 
--  [0] - Client session ID - identifier for a given client session
--------------------------------------------------------------------------------

set nocount on 
declare 
	@P1 int, 
	@rowcount int, 
	@client_session_id nvarchar(255)

select @client_session_id = '{0}'

begin transaction 

-- Set up temp table
if not exists 
	(
	select * 
	from tempdb..sysobjects 
	where id = object_id('tempdb..SQLdm_Session_Traces') 
	and type = 'U') 
	create table tempdb..SQLdm_Session_Traces (host_name varchar(255), desktop_id varchar(255), stoptime datetime, queue_number int, spid int) 

-- Get saved data from temp table
select 
	@P1 = queue_number
from 
	tempdb..SQLdm_Session_Traces 
where 
	host_name = host_name() 
	and desktop_id = @client_session_id

select @rowcount = @@rowcount 

-- If we found an active trace, stop it
if @rowcount > 0 
begin
	exec sp_trace_setstatus @P1, 0 
	exec sp_trace_setstatus @P1, 2 
end 

delete from tempdb..SQLdm_Session_Traces where host_name = host_name() and desktop_id = @client_session_id

exitbatch:

-- Clean up any stopped traces
delete from tempdb..SQLdm_Session_Traces where datediff(mi,stoptime,getdate()) > 0

commit