-- Read Query Monitor Trace Segment
-- [0] config file setting allowing the user to limit the returned rows to only those with sql_text of less that a certain length

declare @flagtime nvarchar(256) 

-- Trigger user-generated flag event
set @flagtime = 'SQLdm1 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
exec @rc = sp_trace_generateevent 82, @flagtime, NULL  

select @flagtime

select @tracefilename = @tracefilename + '.trc' 

select 
	EventClass, 
	isnull(Duration,0) as [Duration],
	isnull(db_name(DatabaseID),'') as DBName,	
	dateadd(mi,datediff(mi,getdate(),getutcdate()),isnull(EndTime,0)) as CompletionTime,
	RTRIM(NTDomainName) + '\' + NTUserName as NTUserName,
	HostName,	
	ApplicationName = isnull(ApplicationName,''),	
	LoginName,	
	'Reads' = cast(isnull(Reads,0) as bigint), 
	'Writes' = cast(isnull(Writes,0) as bigint), 
	'CPU' = cast(isnull(CPU,0) as bigint), 	
	TextData = cast(TextData as nvarchar(max)),
	ObjectName,
	SPID,
	StartTime,
	DatabaseID
from 
	::fn_trace_gettable(@tracefilename, default) 
where 
	EventClass <= 148
	and ((EventClass =92 or EventClass = 93) or (TextData is not null {0}))