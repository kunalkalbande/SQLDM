--------------------------------------------------------------------------------
--  Batch: Table Fragmentation Workload 2005
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

if (select isnull(count(*),0) from tempdb.sys.columns where object_id = object_id('tempdb..dmfragmentationworktable')) <> 9
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
		retry int default(0),
		fragmentation float) 
end		
else
	delete from tempdb..dmfragmentationworktable where servicename = host_name() and runtime != convert(datetime, '{2}', 121)
	
if not exists(select top 1 * from tempdb..dmfragmentationworktable where servicename = host_name() and runtime = convert(datetime, '{2}', 121))
begin
		insert into tempdb..dmfragmentationworktable(servicename,runtime) values (host_name(),convert(datetime, '{2}', 121))
	
	insert into @databases
              select 
                     database_id As dbid
              from 
                     sys.databases d (nolock) 
              where
                     has_dbaccess (name) = 1
                     and (
                           user_access <> 1 
                           or ( 
                                  user_access = 1 
                                  and not exists 
                                  (select * from master.sys.dm_exec_requests where database_id = d.database_id and session_id <> @@spid)
                                  and not exists
                                  (select * from master.sys.dm_tran_locks l where resource_database_id = d.database_id and l.request_session_id <> @@spid)
                                  )
                           )
                     and state = 0 -- online
					 and source_database_id is null
			and lower(name) not in  ('mssqlsystemresource'{0})
			
				
	select @dbid = min(dbid) from @databases

	while (isnull(@dbid,-1) >= 0)
	begin

		select @dbname = db_name(@dbid)
		if 	len(@dbname) > 0
			and has_dbaccess (@dbname) = 1 
			and exists (select 
                     database_id As dbid
              from sys.databases d (nolock) 
              where has_dbaccess (name) = 1
                    and (
                           user_access <> 1 --single user
                           or ( 
                                  user_access = 1 --is single user but we are the single user
                                  and not exists 
                                  (select * from master.sys.dm_exec_requests where database_id = d.database_id and session_id <> @@spid)
                                  and not exists
                                  (select * from master.sys.dm_tran_locks l where resource_database_id = d.database_id and l.request_session_id <> @@spid)
                               )
                           )
                     and state = 0 -- online
					 and database_id = @dbid
			)
		begin 
			set @command = 
				'insert into tempdb..dmfragmentationworktable(dbid,objectid,schemaname,objectname,servicename,runtime)
				select 
				@DBID,
				i.object_id,
				s.name,
				o.name,
				host_name(),
				convert(datetime, ''{2}'', 121) 
				from ' +
				quotename(@dbname) + '.sys.dm_db_partition_stats p
				inner join ' +
				quotename(@dbname) + '.sys.indexes i
				on p.object_id = i.object_id
				inner join ' +
				quotename(@dbname) + '.sys.objects o
				inner join ' +
				quotename(@dbname) + '.sys.schemas s on o.schema_id = s.schema_id
				on o.object_id = i.object_id
				where  
				o.type = ''U''
				and p.used_page_count >= {1}
				and p.row_count > 0
				and p.index_id = 1 
				and i.index_id = 1'
				set @parms = N'@DBID int'	
				exec sp_executesql @command,@parms,@DBID=@dbid
		end
		

	delete from @databases where dbid = @dbid
	select @dbid = min(dbid) from @databases

	end 

end
