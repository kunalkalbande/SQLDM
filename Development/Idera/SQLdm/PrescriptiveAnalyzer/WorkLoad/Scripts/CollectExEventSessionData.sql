
declare @xefilename as nvarchar(4000);
set @xefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @xefilename = left(@xefilename,len(@xefilename) - charindex('\',reverse(@xefilename))) + '\' + replace(Host_name(),' ','') + ISNULL(Convert(nvarchar(100),SERVERPROPERTY ('InstanceName')),'Default') + '{0}' +'*.xel';


DECLARE @EventSequence bigint;
SET @EventSequence =  {1};

{2}


;WITH raw_data(t,fname,foffset) AS
  (
	Select  CAST(event_data AS XML),file_name,file_offset
	FROM sys.fn_xe_file_target_read_file(@xefilename, NULL, NULL, NULL)
  ),
  xml_data (ed,fname,foffset) AS
(
  SELECT e.query('.') ,fname,foffset
  FROM raw_data 
  CROSS APPLY t.nodes('/event') AS x(e)
)

SELECT			 convert(int,xe.trace_event_id) as EventClass,
				 [Duration],
				 NTUserName,
				 HostName,
				 ApplicationName = isnull(ApplicationName,''),
				 LoginName,
				 [Reads],
				 [Writes],
				 ISNULL(CPU,0)/1000 as CPU,
				 ISNULL([statement],ISNULL(batch_text,Flag_Text)) as TextData,
				 ObjectName,
				 Convert(int,SPID) as SPID,
				 Convert(int,DatabaseID) DatabaseID,
				 Convert(int,EventSequence) EventSequence,
				 fname,
				 foffset


FROM
(
  SELECT 
     ed.value('event[1]/@name','varchar(50)') AS event_name
	,ed.value('(event/action[@name="username"]/value)[1]', 'varchar(max)') AS LoginName 
	,ed.value('(event/data[@name="duration"]/value)[1]', 'bigint') AS [Duration]  
	,ed.value('(event/data[@name="cpu_time"]/value)[1]', 'int') AS [CPU]	
	,ed.value('(event/data[@name="logical_reads"]/value)[1]', 'bigint') AS [Reads] 
	,ed.value('(event/data[@name="writes"]/value)[1]', 'bigint') AS [Writes] 
	,ed.value('(event/action[@name="nt_username"]/value)[1]', 'nvarchar(max)') AS NTUserName 
	,ed.value('(event/action[@name="client_app_name"]/value)[1]', 'nvarchar(max)') AS ApplicationName 
	,ed.value('(event/action[@name="client_hostname"]/value)[1]', 'nvarchar(max)') AS HostName 
	,ed.value('(event/action[@name="object_name"]/value)[1]', 'nvarchar(max)') AS ObjectName 
	,ed.value('(event/data[@name="batch_text"]/value)[1]', 'varchar(max)') AS [batch_text]
	,ed.value('(event/data[@name="statement"]/value)[1]', 'varchar(max)') AS [statement]
	,ed.value('(event/data[@name="user_info"]/value)[1]', 'nvarchar(max)') AS [Flag_Text]
	,ed.value('(event/action[@name="session_id"]/value)[1]', 'nvarchar(max)') AS SPID  
	,ed.value('(event/action[@name="database_id"]/value)[1]', 'varchar(max)') AS DatabaseID 
	,ed.value('(event/action[@name="event_sequence"]/value)[1]', 'varchar(max)') AS EventSequence
	,fname
	,foffset
	
  FROM xml_data  
) AS Queryresult JOIn sys.trace_xe_event_map xe ON xe.xe_event_name=Queryresult.event_name
  Where EventSequence > @EventSequence
