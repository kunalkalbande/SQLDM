--------------------------------------------------------------
-- SQLDM 11 - Recommended changes for Azure SQL Database v12
--------------------------------------------------------------

--------------------------------------------------------------------------------
--  Batch: Procedure Cache List 2005
--  Tables: sys.syscacheobjects, sys.sysprocesses, sys.dm_tran_locks
--  XSP:
--	Variables:
--  [0] - select top value
--
--  RRG: This script has been modified to work only on Azure SQL DB User Databases
--     retricted permissions prevent it from running on master database
--
--  *** This script needs further review, not sure it will work in current state
--  ***  of modification
--------------------------------------------------------------------------------
		
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
		sys.syscacheobjects
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
				(select * from sys.sysprocesses p where dbid = @dbid and p.spid <> @@spid)
				and not exists
				(select * from sys.dm_tran_locks l where resource_database_id = @dbid and l.request_session_id <> @@spid)
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
				   then quotename(DB_NAME(dbid) COLLATE DATABASE_DEFAULT) + ''.'' + schema_name(uid) COLLATE DATABASE_DEFAULT + ''.'' + sql
				 else
				   isnull(DB_NAME(dbid) COLLATE DATABASE_DEFAULT + ''.'' + schema_name(uid) COLLATE DATABASE_DEFAULT + ''.'','''') + sql
			   end ,
			 pagesused,
			 usecounts,
			 refcounts, '
			 + '''User'' = isnull(schema_name(uid) COLLATE DATABASE_DEFAULT , ''Shared'')
			from
			 sys.syscacheobjects
			where dbid = ' + cast(@dbid as nvarchar(5))
	end
	else
	begin
	if @dbname is null
		begin
			set @command = 
				'--use [master]
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
					   then quotename(DB_NAME(dbid) COLLATE DATABASE_DEFAULT) + ''.sys.'' + sql
					 else
					   isnull(DB_NAME(dbid) COLLATE DATABASE_DEFAULT + ''.sys.'','''') + sql
				   end ,
				 pagesused,
				 usecounts,
				 refcounts, '
				 + '''User'' = ''sys''
				from
				 sys.syscacheobjects
				where dbid = ' + cast(@dbid as nvarchar(5))
		end
		else
			set @command = 
				'--use [master]
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
					   then quotename(DB_NAME(dbid) COLLATE DATABASE_DEFAULT) + ''.unknown.'' + sql
					 else
					   isnull(DB_NAME(dbid) COLLATE DATABASE_DEFAULT + ''.unknown.'','''') + sql
				   end ,
				 pagesused,
				 usecounts,
				 refcounts, '
				 + '''User'' = ''unknown''
				from
				 sys.syscacheobjects
				where dbid = ' + cast(@dbid as nvarchar(5))
		end

			execute(@command)
 
	fetch proccache into @dbname, @dbid
End 
Close proccache 
deallocate proccache 
