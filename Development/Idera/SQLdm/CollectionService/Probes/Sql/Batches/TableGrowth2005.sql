--------------------------------------------------------------------------------
--  Batch: Database and Table Growth 2005
--  Tables: 
--  XSP: none
--	System Variables: none
--  Variables: 
--		{0} - Databases to exclude from growth query
--		{1} - rowcount limit
--		{2} - Timeout time in seconds
--		{3} - Already completed databases
--		{4} - max or min
--------------------------------------------------------------------------------
set nocount on

begin try

declare
	@dbname nvarchar(255),
	@dbid smallint, 
	@command nvarchar(4000),
	@TimeoutTime datetime,
	@timeoutseconds int,
	@parms nvarchar(100),
	@rowcount int,
	@rowcountlimit int

set @rowcountlimit = {1}	
set @timeoutseconds = {2}	


select @TimeoutTime = dateadd(ss,@timeoutseconds, getutcdate())

declare @read_db_status table(dbid int)

insert into @read_db_status (dbid)
	select 
		dbid
	from 
		master..sysdatabases d (nolock) 
	where
		dbid not in({3})
		and lower(name) <> 'mssqlsystemresource'
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databaseproperty(name, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(name, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(name, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(name, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(name, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(name, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = d.dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = d.dbid and l.request_session_id <> @@spid)
				)
			)
		and status & 32 <> 32 
		and status & 64 <> 64 
		and status & 128 <> 128 
		and status & 256 <> 256 
		and status & 512 <> 512 
		and name not in  ('mssqlsystemresource'{0})
		

select @dbid = min(dbid), @rowcount = 0 from @read_db_status

while (isnull(@dbid,-1) >= 0)
begin
    if (GetUTCDate() > @TimeoutTime)           
	raiserror (N'Timeout', 11, 1);
	
	if (@rowcount > @rowcountlimit)
	raiserror (N'Rowcount Limit', 11, 1);

	select @dbname = db_name(@dbid)

	if 	len(@dbname) > 0
		and has_dbaccess (@dbname) = 1 
		and isnull(databaseproperty(@dbname, 'IsInLoad'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsSuspect'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsOffline'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsShutDown'),0) = 0 
		and (
			isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databaseproperty(@dbname, 'IsSingleUser'),0) = 1 
				and not exists 
				(select * from master..sysprocesses p where dbid = @dbid and p.spid <> @@spid)
				and not exists
				(select * from master.sys.dm_tran_locks l where resource_database_id = @dbid and l.request_session_id <> @@spid)
				)
			)
	begin 
		if lower(@dbname) not in  ('mssqlsystemresource')--{0})
		begin 
			
			select @command = ';with dailystats(TableName,TableID,DataSizeKB,TextSizeKB,used_pages,NumRows,TableType,TableOwner, issystemtable) as (select
				o.name,o.object_id,''dataspace'' = sum(case when it.internal_type IN (202,204) then 0 when p.index_id < 2 then convert(dec(12,0),au.data_pages) else 0 end) * 8,
				''textused'' = sum(case when it.internal_type IN (202,204) then 0 when au.type != 1 then convert(dec(12,0),au.used_pages) else 0 end) * 8, 
				used_pages = sum(convert(dec(12,0),au.used_pages)) * 8, 
				sum(case when (p.index_id < 2) and (au.type = 1) then p.rows else 0 end),case when o.type = ''S'' then 1 else 0 end, ISNULL(s.name,''unknown''), ' 
				+ 'issystemtable = o.is_ms_shipped'
				+ ' from ' + quotename(@dbname) + '.sys.allocation_units au 
				left join ' + quotename(@dbname) + '.sys.partitions p on (au.type = 2 and p.partition_id = au.container_id) OR (au.type IN (1,3) AND p.hobt_id = au.container_id) 
				inner join ' + quotename(@dbname) + '.sys.objects o on o.object_id = p.object_id left join ' + quotename(@dbname)  
				+ '.sys.schemas s on o.schema_id = s.schema_id left join ' + quotename(@dbname)  
				+ '.sys.internal_tables it on p.object_id = it.object_id where o.type =''U'' ' 

			if lower(@dbname) = 'tempdb'  
				select @command = @command + ' and o.name not like ''#%'' '  

			select @command = @command + ' group by o.name, o.object_id, o.type, s.name,o.is_ms_shipped )' 

		select @command = @command + 'select db_name(@DBID),isnull(TableName,''''),
			isnull(DataSizeKB,0),case when used_pages < 0 then -1 else isnull((used_pages - DataSizeKB - TextSizeKB),0) end 
			as IndexSizeKB,isnull(TextSizeKB,0),
			isnull(NumRows,0),isnull(TableType,''U''),isnull(TableOwner,''unknown''), issystemtable,@DBID ' 
			+ ' from dailystats d left join ' + quotename(@dbname) + '.sys.indexes i on d.TableID = i.object_id ' 
			+' group by TableName,DataSizeKB,TextSizeKB,used_pages,NumRows,TableType,TableOwner,issystemtable order by TableName'
		set @parms = N'@DBID int'	
		exec sp_executesql @command,@parms,@DBID=@dbid
		select @rowcount = @rowcount + @@rowcount
	end 
	end
	
	delete from @read_db_status where dbid = @dbid
	select @dbid = {4}(dbid) from @read_db_status
end 

end try
begin catch

select error_message()

end catch
