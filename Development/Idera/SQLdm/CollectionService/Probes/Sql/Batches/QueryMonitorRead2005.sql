-- Read Query Monitor Trace Segment


declare @flagtime nvarchar(256) 

-- Trigger user-generated flag event
set @flagtime = 'SQLdm - ' + host_name() + ' - ' + convert(nvarchar(50),@currenttime,121) 
exec @rc = sp_trace_generateevent 82, @flagtime, NULL  

if (select isnull(object_id('tempdb..#DMTraceEvents'), 0)) <> 0 
	drop table #DMTraceEvents 

select @tracefilename = @tracefilename + '.trc' 

-- Populate temporary table
select 
	identity(int, 1, 1) as RowNumber,
	EventClass, 
	Duration,	
	DatabaseID,	
	EndTime,	
	NTUserName,	
	NTDomainName,	
	HostName,	
	ApplicationName = isnull(ApplicationName,''),	
	LoginName,	
	Reads,
	Writes,
	CPU,
	TextData = cast(TextData as nvarchar(max)),
	ObjectName,
	SPID,
	StartTime
into 
	#DMTraceEvents 
from 
	::fn_trace_gettable(@tracefilename, default) 
where 
	EventClass <= 148  

declare 
	@LastReadRowNumber int, 
	@CurrentReadRowNumber int 

-- Find the flag we just raised as our endpoint
select 
	@CurrentReadRowNumber = RowNumber 
from 
	#DMTraceEvents 
where 
	EventClass = 82 
	and TextData = @flagtime 

-- Find the flag we may have raised on a previous refresh as our startpoint
select 
	@LastReadRowNumber = max(RowNumber) 
from 
	#DMTraceEvents 
where
 	lower(TextData) like lower('SQLdm - ' + host_name() + ' - %')
	and EventClass = 82
	and RowNumber < @CurrentReadRowNumber 


select top {0}
	'eventclass' = 
	case 
		when EventClass in (41, 45) 
		and charindex (' (', convert(varchar(100),substring(TextData, 1, 100))) = 0 
		and (charindex ('EXEC ', upper(convert(varchar(100),substring(TextData, 1, 100)))) > 0 
		or charindex ('EXECUTE ', upper(convert(varchar(100),substring(TextData, 1, 100)))) > 0) 
			then 43 
		else EventClass 
	end, 
	'duration(ms)' = cast(isnull(Duration,0) / 1000 as bigint), 
	'completion time' =  dateadd(mi,datediff(mi,getdate(),getutcdate()),EndTime),
	'DBName' = isnull(db_name(DatabaseID),''),
	'nt username' = RTRIM(NTDomainName) + '\' + NTUserName, 
	'host name' = HostName, 
	'app name' = ApplicationName, 
	'sql user name' = LoginName, 
	'reads' = cast(isnull(Reads,0) as bigint), 
	'writes' = cast(isnull(Writes,0) as bigint), 
	'cpu' = cast(isnull(CPU,0) as bigint), 
	'sql 1' = case when ObjectName is not null then '-- ' + ObjectName + char(13) + char(10) + ' ' + TextData else TextData end,
	'spid' = SPID,
	'starttime' = dateadd(mi,datediff(mi,getdate(),getutcdate()),StartTime),
	DatabaseID
from 
	#DMTraceEvents 
where 
	RowNumber > isnull(@LastReadRowNumber,0) 
	and RowNumber < isnull(@CurrentReadRowNumber,10001) 
	and EventClass in (12,41,43,45,148,92,93)  

if (select isnull(object_id('tempdb..#DMTraceEvents'), 0)) <> 0 
	drop table #DMTraceEvents 