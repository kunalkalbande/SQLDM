--------------------------------------------------------------------------------
--  Batch: Database and Table Growth 2000
--  Tables: master..sysdatabases and sysindexes, sysobjects, sysusers for each database
--  XSP: none
--	System Variables: none
--  Variables: 
--		{0} - Databases to exclude from growth query
--		{1} - rowcount limit
--		{2} - Timeout time in seconds
--		{3} - Already completed databases
--		{4} - max or min
--------------------------------------------------------------------------------

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
					(select * from master..syslockinfo l where rsc_dbid = d.dbid and l.req_spid <> @@spid)
					)
				)
			and status & 32 <> 32 
			and status & 64 <> 64 
			and status & 128 <> 128 
			and status & 256 <> 256 
			and status & 512 <> 512 

select @dbid = min(dbid), @rowcount = 0 from @read_db_status

while (isnull(@dbid,-1) >= 0)
begin
    if (GetUTCDate() > @TimeoutTime)           
	break;
	
	if (@rowcount > @rowcountlimit)
	break;

	select @dbname = db_name(@dbid)
	
	if 	len(@dbname) > 0
		and HAS_DBACCESS (@dbname) = 1 
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
				(select * from master..sysprocesses p where dbid = db_id(@dbname) and p.spid <> @@spid)
				)
			)
	begin 
		if lower(@dbname) not in  ('mssqlsystemresource'{0}) 
		begin 
			select @command = 'select db_name(@DBID),o.name,''dataspace'' = sum(case when indid in (0,1) then (convert(dec(12,0),dpages) * 8)  else 0 end), '
				+ '''indexspace'' = sum(case when indid in (0,1) then (convert(dec(12,0),used) * 8) - (convert(dec(12,0),dpages) * 8)  else 0 end),  '
				+ '''textused'' = sum(case when indid = 255 then (convert(dec(12,0),used) * 8) else 0 end), sum(case when indid < 2 then cast(i.rows as bigint) else cast(0 as bigint) end), case when o.type = ''S'' then 1 else 0 end, ISNULL(u.name,''unknown''), ' 
				+ 'issystemtable = case when category & 2 = 2 then cast(1 as bit) else cast(0 as bit) end, @DBID '
				+  'from ' + quotename(@dbname) + '..sysindexes i (nolock) inner join ' 
				+ quotename(@dbname) + '..sysobjects o (nolock) on  i.id = o.id inner join ' 
				+ quotename(@dbname) + '..sysusers u (nolock) on o.uid = u.uid where i.indid in (0, 1, 255) and upper(o.type) = ''U'' ' 
	
		-- Exclude temporary tables if tempdb is scanned
		if lower(@dbname) = 'tempdb' 
			select @command = @command + 'and o.name not like ''#%'' ' 

		-- Add group by clauses
		select @command = @command + 'group by o.name, o.id, o.type, o.uid, u.name, o.category  order by o.name ' 
		print @command
		set @parms = N'@DBID int'	
		exec sp_executesql @command,@parms,@DBID=@dbid
		select @rowcount = @rowcount + @@rowcount
	end 
	end
	
	delete from @read_db_status where dbid = @dbid
	select @dbid = {4}(dbid) from @read_db_status
end 

