--------------------------------------------------------------------------------
--  Batch: Query Monitor Trace 2000
--  Tables: tempdb..SQLdm_Query_Monitor_Traces 
--  XSP:  none
--	Variables:  [0] - Query Monitor Trace Batch Segments
--  [1] - Query Monitor Filter Statements
--  [2] - Read Query Monitor
--  [3] - Query Stop Time
-------------------------------------------------------------------------------\
set nocount on 

declare @P1 int, 
	@P2 int,
	@on bit, 
	@counter bigint, 
	@rowcount bigint, 
	@rc varchar, 
	@eventclass int, 
	@previouseventclass int, 
	@sqltext varchar(3061), 
	@duration bigint, 
	@reads bigint, 
	@writes bigint, 
	@cpu bigint, 
	@date varchar(20), 
	@time varchar(8), 
	@mm varchar(2), 
	@dd varchar(2), 
	@hh varchar(2), 
	@mi varchar(2), 
	@ss varchar(2), 
	@hex varbinary(30), 
	@ntusername varchar(128), 
	@ntdomainname varchar(128), 
	@hostname varchar(128), 
	@appname varchar(128), 
	@sqlname varchar(255), 
	@DBID bigint,
	@retrycounter bigint,
	@spid int,
	@enablealternate bit

set @retrycounter = 0
select @on = 1 
set @enablealternate = 1

if not exists (select * from tempdb..sysobjects where id = object_id('tempdb..SQLdm_Query_Monitor_Traces') and type = 'U') 
	create table tempdb..SQLdm_Query_Monitor_Traces (
		host_name varchar(255), 
		queue_number int,
		alternate bit default 0) 
		
if  exists(select * from tempdb..syscolumns where id = object_id('tempdb..SQLdm_Query_Monitor'))
	begin
		declare @moveStmt nvarchar(300)
		set @moveStmt = 'insert into tempdb..SQLdm_Query_Monitor_Traces(host_name,queue_number)
		select host_name,queue_number from tempdb..SQLdm_Query_Monitor where host_name not in (select host_name from tempdb..SQLdm_Query_Monitor_Traces)'
		exec sp_executesql @moveStmt
	end

select 
	@P1 = queue_number 
from 
	tempdb..SQLdm_Query_Monitor_Traces 
where 
	host_name = HOST_NAME() 
	and alternate = 0
	and queue_number > 0
	
--select @rowcount = @@rowcount 


select 
	@P2 = queue_number 
from 
	tempdb..SQLdm_Query_Monitor_Traces 
where 
	host_name = HOST_NAME() 
	and alternate = 1
	and queue_number > 0
	
if @P1 is not null and (@enablealternate = 0 or @P2 is not null)
	goto correct_trace 

recreate_trace: 
delete from tempdb..SQLdm_Query_Monitor_Traces 
where host_name = HOST_NAME() 


declare 
	@stoptime datetime

set @stoptime = {3}
set @stoptime = dateadd(hh,-datediff(hh,getdate(),getutcdate()),@stoptime)

exec @rc = sp_trace_create @P1 output, 1, NULL, NULL, @stoptime 
exec @rc = sp_trace_create @P2 output, 1, NULL, NULL, @stoptime 

if @P1 <> 0 
	insert into tempdb..SQLdm_Query_Monitor_Traces 
	values (HOST_NAME(), @P1, 0) 


if isnull(@P2,0) <> 0 
	insert into tempdb..SQLdm_Query_Monitor_Traces 
	values (HOST_NAME(), @P2, 1) 	

-- Begin Query Monitor Trace Segments

{0}

-- End Query Monitor Trace Segments


declare 
	@cpu_filter int, 
	@reads_filter bigint, 
	@writes_filter bigint,
	@duration_filter bigint

-- Begin Filter Statements

{1} 

-- End Filter Statements


correct_trace: 

exec @rc = sp_trace_setstatus @P1, 1 
if @rc = 9 
begin
	set @retrycounter = @retrycounter + 1
	if @retrycounter < 10
	begin
		waitfor delay '00:00:01'
		goto recreate_trace 
	end
	else
	begin
		goto end_of_batch
	end
end

if (@enablealternate=1 and @P2 is not null)
begin
	exec @rc = sp_trace_setstatus @P2, 1 
	if @rc = 9 
	begin
		set @retrycounter = @retrycounter + 1
		if @retrycounter < 10
		begin
			waitfor delay '00:00:01'
			goto recreate_trace 
		end
		else
		begin
			goto end_of_batch
		end
	end
end	
else
begin
	if (@P2 is not null)
	begin
		exec @rc = sp_trace_setstatus @P2, 0
		exec @rc = sp_trace_setstatus @P2, 2
	end
end

{2}

end_of_batch:
