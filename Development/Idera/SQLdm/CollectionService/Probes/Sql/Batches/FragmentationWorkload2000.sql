--------------------------------------------------------------------------------
--  Batch: Table Fragmentation Workload 2000
--  Variables: 
--		[0] - Database exclude string
--		[1] - Minimum Table Size
--		[2] - Timestamp
--------------------------------------------------------------------------------
use master
declare 
@command nvarchar(2048),
@dbid int,
@dbname sysname,
@parms nvarchar(100)

set nocount on

declare @databases table(dbid int)

if (select isnull(count(*),0) from tempdb..syscolumns where id = object_id('tempdb..dmfragmentationworktable')) <> 8
begin
	if (select isnull(object_id('tempdb..dmfragmentationworktable'), 0)) <> 0
		exec('drop table tempdb..dmfragmentationworktable')
		
	create table tempdb..dmfragmentationworktable
		(rowid bigint identity primary key clustered,
		dbid int,
		schemaname nvarchar(128),
		objectid int,
		objectname nvarchar(128),
		servicename nvarchar(256), 
		runtime datetime, 
		retry int default(0)) 
end		
else
	delete from tempdb..dmfragmentationworktable where servicename = host_name() and runtime != convert(datetime, '{2}', 121)
	

if not exists(select top 1 * from tempdb..dmfragmentationworktable where servicename = host_name() and runtime = convert(datetime, '{2}', 121))
begin
	insert into tempdb..dmfragmentationworktable(servicename,runtime) values (host_name(),convert(datetime, '{2}', 121))
	
	insert into @databases
		select 
			dbid
		from 
			master..sysdatabases d (nolock) 
		where
			lower(name) <> 'mssqlsystemresource'
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
			and name not in  ('mssqlsystemresource'{0})
			
				
	select @dbid = min(dbid) from @databases

	while (isnull(@dbid,-1) >= 0)
	begin

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
			set @command = 
				'insert into tempdb..dmfragmentationworktable(dbid,objectid,schemaname,objectname,servicename,runtime)
				select 
				@DBID,
				o.id,
				u.name,
				o.name,
				host_name(),
				convert(datetime, ''{2}'', 121) 
				from ' +
				quotename(@dbname) + '..sysobjects o
				inner join ' +
				quotename(@dbname) + '..sysindexes i
				on o.id = i.id
				inner join ' +
				quotename(@dbname) + '..sysusers u
				on o.uid = u.uid
				where indid = 1 and rowcnt > 0 and used >= {1} and type=''U'''
				set @parms = N'@DBID int'	
				exec sp_executesql @command,@parms,@DBID=@dbid
		end
		
	delete from @databases where dbid = @dbid
	select @dbid = min(dbid) from @databases

	end 
end