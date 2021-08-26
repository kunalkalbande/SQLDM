set concat_null_yields_null on

declare
	@dbname nvarchar(255),
	@dbid smallint, 
	@command nvarchar(4000)

declare proccache insensitive cursor 
for
	select distinct
		db_name(dbid),
		dbid
	from
		master..syscacheobjects
for read only
set nocount on 
open proccache 
fetch proccache into @dbname, @dbid
while @@fetch_status = 0 
begin 
	if 	has_dbaccess (@dbname) = 1 
		and databaseproperty(@dbname, 'IsInLoad') = 0 
		and databaseproperty(@dbname, 'IsSuspect') = 0 
		and isnull(databaseproperty(@dbname, 'IsInRecovery'),0) = 0 
		and isnull(databaseproperty(@dbname, 'IsNotRecovered'),0) = 0 
		and databaseproperty(@dbname, 'IsOffline') = 0 
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
		and @dbname is not null
	begin 
			set @command = 
			'use ' + quotename(@dbname) + '
			 select
			 top({0})
			 ''detail'',
			 ''Type'' =
			   case
				 when (status & 1) = 1 or cacheobjtype = ''Cursor Parse Tree''
				   then ''Cursors''
				 when cacheobjtype = ''Extended Proc''
				   then ''Extended Procedure''
				 else
				   objtype
			   end,
			 ''Name / SQL Command'' =
			   case
				 when objtype in (''Adhoc'', ''Prepared'')
				   then sql
				 when objtype = ''SysTab''
				   then quotename(DB_NAME(dbid)) + ''.'' + schema_name(uid) + ''.'' + sql
				 else
				   isnull(DB_NAME(dbid) + ''.'' + schema_name(uid) + ''.'','''') + sql
			   end ,
			 pagesused,
			 usecounts,
			 refcounts, '
			 + '''User'' = isnull(schema_name(uid), ''Shared'')
			from
			 master..syscacheobjects
			where dbid = ' + cast(@dbid as nvarchar(5))
	end
	else
	begin
	if @dbname is null
		begin
			set @command = 
				'use [master]
				 select
   			 top({0})
				 ''detail'',
				 ''Type'' =
				   case
					 when (status & 1) = 1 or cacheobjtype = ''Cursor Parse Tree''
					   then ''Cursors''
					 when cacheobjtype = ''Extended Proc''
					   then ''Extended Procedure''
					 else
					   objtype
				   end,
				 ''Name / SQL Command'' =
				   case
					 when objtype in (''Adhoc'', ''Prepared'')
					   then sql
					 when objtype = ''SysTab''
					   then quotename(DB_NAME(dbid)) + ''.sys.'' + sql
					 else
					   isnull(DB_NAME(dbid) + ''.sys.'','''') + sql
				   end ,
				 pagesused,
				 usecounts,
				 refcounts, '
				 + '''User'' = ''sys''
				from
				 master..syscacheobjects
				where dbid = ' + cast(@dbid as nvarchar(5))
		end
		else
			set @command = 
				'use [master]
				 select
   			 top({0})
				 ''detail'',
				 ''Type'' =
				   case
					 when (status & 1) = 1 or cacheobjtype = ''Cursor Parse Tree''
					   then ''Cursors''
					 when cacheobjtype = ''Extended Proc''
					   then ''Extended Procedure''
					 else
					   objtype
				   end,
				 ''Name / SQL Command'' =
				   case
					 when objtype in (''Adhoc'', ''Prepared'')
					   then sql
					 when objtype = ''SysTab''
					   then quotename(DB_NAME(dbid)) + ''.unknown.'' + sql
					 else
					   isnull(DB_NAME(dbid) + ''.unknown.'','''') + sql
				   end ,
				 pagesused,
				 usecounts,
				 refcounts, '
				 + '''User'' = ''unknown''
				from
				 master..syscacheobjects
				where dbid = ' + cast(@dbid as nvarchar(5))
		end

			execute(@command)
 
	fetch proccache into @dbname, @dbid
End 
Close proccache 
deallocate proccache 




