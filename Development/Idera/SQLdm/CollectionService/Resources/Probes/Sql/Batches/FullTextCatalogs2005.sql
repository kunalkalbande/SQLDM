--------------------------------------------------------------------------------
--  Batch: Full Text Search Catalogs 2000
--  Tables: sysdatabases, sys.fulltext_catalogs, sys.database_principals,
--		sys.data_spaces
--  Variables: None
--------------------------------------------------------------------------------

select fulltextserviceproperty('ResourceUsage')


declare 
	@command nvarchar(4000), 
	@dbname sysname

declare read_db_status insensitive cursor 
for 
	select 
		name 
	from 
		master..sysdatabases d(nolock) 
	where 
		name <> 'tempdb' 
		and cmptlevel > 60
		and lower(name) <> 'mssqlsystemresource'
		and has_dbaccess (name) = 1 
		and mode = 0 
		and isnull(databasepropertyex(name, 'IsInLoad'),0) = 0 
		and isnull(databasepropertyex(name, 'IsSuspect'),0) = 0 
		and isnull(databasepropertyex(name, 'IsInRecovery'),0) = 0 
		and isnull(databasepropertyex(name, 'IsNotRecovered'),0) = 0 
		and isnull(databasepropertyex(name, 'IsOffline'),0) = 0 
		and isnull(databasepropertyex(name, 'IsShutDown'),0) = 0 
		and (
			isnull(databasepropertyex(name, 'IsSingleUser'),0) = 0 
			or ( 
				isnull(databasepropertyex(name, 'IsSingleUser'),0) = 1 
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
		and status2 & 536870912 = 536870912
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname
while @@fetch_status = 0 
begin 
	

	set @command = 'use ' + quotename(@dbname) + '
	if databasepropertyex(db_name(), ''IsFulltextEnabled'') <> 0
	if (select isnull(count(*),0) from sysfulltextcatalogs) <> 0
		select
			db_name(), 
			ft.name,
			path,
			fulltextcatalogproperty(ft.name,''AccentSensitivity'') as AccentSensitivity,
			fulltextcatalogproperty(ft.name,''IndexSize'') as IndexSize,
			fulltextcatalogproperty(ft.name,''ItemCount'') as ItemCount,
			case when isnull(fulltextcatalogproperty(ft.name,''PopulateCompletionAge''),0) > 0 then dateadd(mi,datediff(mi,getdate(),getutcdate()),dateadd(ss,isnull(fulltextcatalogproperty(ft.name,''PopulateCompletionAge''),0),''1990-01-01'')) else null end as LastPopulation,
			fulltextcatalogproperty(ft.name,''PopulateStatus'') as PopulateStatus,
			fulltextcatalogproperty(ft.name,''UniqueKeyCount'') as UniqueKeyCount,
			tablecount = (
				select 
					count(*) 
				from  
					sysobjects o (nolock)
				where o.type = ''U'' 
				and o.ftcatid = ft.fulltext_catalog_id),
			dp.name,
			ft.is_default,
			ds.name
		from
			sys.fulltext_catalogs ft
			left join sys.database_principals dp on ft.principal_id=dp.principal_id
			left join sys.data_spaces ds on ft.data_space_id = ds.data_space_id'
	execute (@command)
	fetch read_db_status into @dbname
End 
Close read_db_status 
deallocate read_db_status 


