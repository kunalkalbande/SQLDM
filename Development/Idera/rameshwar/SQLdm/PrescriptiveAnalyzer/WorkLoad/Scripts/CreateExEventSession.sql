

DECLARE @minDuration bigint
DECLARE @maxDuration bigint
DECLARE @sysSPID int
DECLARE @applicationName nvarchar(1024)
DECLARE @dbid int 
DECLARE @sql nvarchar(max);
DECLARE @xefilename as nvarchar(4000);

SET @minDuration = {0}
SET @maxDuration = {1}
SET @applicationName = N{2}
SET @dbid = isnull(db_id({3}),0)

set @xefilename = cast(ServerProperty('ErrorLogFileName') as nvarchar(4000)) 
set @xefilename = left(@xefilename,len(@xefilename) - charindex('\',reverse(@xefilename))) + '\' + replace(Host_name(),' ','') + ISNULL(Convert(nvarchar(100),SERVERPROPERTY ('InstanceName')),'Default') + '{4}' +'.xel';


set @sql = 'CREATE EVENT SESSION {5}
ON SERVER


ADD EVENT sqlserver.sql_batch_completed(
	      ACTION(	package0.event_sequence,
					sqlserver.username,
					sqlserver.client_app_name,
					sqlserver.client_hostname,
					sqlserver.database_id,
					sqlserver.nt_username,
					sqlserver.server_principal_name,
					sqlserver.session_id
				)
		Where sqlserver.database_id <> 32767 
		and sqlserver.session_id > 50 
		and sqlserver.client_app_name not like N''Idera SQL doctor%'' 
		and sqlserver.client_app_name not like N''SQL diagnostic manager%'''
set @sql = 	@sql +	' and duration > ' +CAST (@minDuration AS NVARCHAR(100))
IF(@maxDuration > 0) Begin set @sql = 	@sql +	' and duration < ' + CAST (@maxDuration AS NVARCHAR(100))  end
IF(@dbid > 0) Begin set @sql = 	@sql +	' and sqlserver.database_id = ' + CAST (@dbid AS NVARCHAR(100)) end
IF(@applicationName <> N'') Begin set @sql = 	@sql +	' and sqlserver.client_app_name like ''' + @applicationName + ''''  End
set @sql = 	@sql +	' ),'

set @sql = 	@sql + 'ADD EVENT sqlserver.rpc_completed(
	    ACTION(		package0.event_sequence,
					sqlserver.username,
					sqlserver.client_app_name,
					sqlserver.client_hostname,
					sqlserver.database_id,
					sqlserver.nt_username,
					sqlserver.server_principal_name,
					sqlserver.session_id
				)
		Where sqlserver.database_id <> 32767 
		and sqlserver.session_id > 50 
		and sqlserver.client_app_name not like N''Idera SQL doctor%'' 
		and sqlserver.client_app_name not like N''SQL diagnostic manager%'''
set @sql = 	@sql +	' and duration > ' +CAST (@minDuration AS NVARCHAR(100))
IF(@maxDuration > 0) Begin set @sql = 	@sql +	' and duration < ' +CAST (@maxDuration AS NVARCHAR(100))  end
IF(@dbid > 0) Begin set @sql = 	@sql +	' and sqlserver.database_id = ' + CAST (@dbid AS NVARCHAR(100)) end
IF(@applicationName <> N'') Begin set @sql = 	@sql +	' and sqlserver.client_app_name like ''' + @applicationName + '''' End
set @sql = 	@sql +	' )'

set @sql = 	@sql + ' ADD TARGET package0.event_file (SET filename = ''' + @xefilename + ''',max_file_size=({7}), max_rollover_files=(1))
					WITH (MAX_MEMORY={8} MB,EVENT_RETENTION_MODE={9},MAX_DISPATCH_LATENCY={10} SECONDS,MAX_EVENT_SIZE={11} MB,
					MEMORY_PARTITION_MODE={12},TRACK_CAUSALITY={13},STARTUP_STATE={14})    {6}'

exec(@sql);