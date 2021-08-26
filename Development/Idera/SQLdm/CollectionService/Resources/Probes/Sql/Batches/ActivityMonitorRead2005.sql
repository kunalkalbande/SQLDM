-- Read Query Monitor Trace Segment
-- [0] config file setting allowing the user to limit the returned rows to only those with sql_text of less that a certain length

declare @flagtime nvarchar(256) 

-- Trigger user-generated flag event
set @flagtime = 'SQLdm3 - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
exec @rc = sp_trace_generateevent 82, @flagtime, NULL  

select @flagtime

select @tracefilename = @tracefilename + '.trc' 

select 
	EventClass, 
	isnull(Duration,0) as [Duration],
	isnull(db_name(DatabaseID),'') as DBName,	
	EndTime as CompletionTime,
	RTRIM(NTDomainName) + '\' + NTUserName as NTUserName,
	HostName,	
	ApplicationName = isnull(ApplicationName,''),	
	LoginName,	
	'ObjectID' = cast(isnull(ObjectID,0) as int),
	TextData = cast(TextData as nvarchar(max)),
	ObjectName,
	SPID,
	'StartTime' = dateadd(mi,datediff(mi,getdate(),getutcdate()),StartTime),
	DatabaseID
from 
	::fn_trace_gettable(@tracefilename, default) 
where 
	EventClass <= 148
	and ((EventClass =92 or EventClass = 93) or (TextData is not null {0}))