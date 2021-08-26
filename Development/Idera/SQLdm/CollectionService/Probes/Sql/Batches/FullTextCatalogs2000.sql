--------------------------------------------------------------------------------
--  Batch: Full Text Search Catalogs 2000
--  Tables: sysdatabases, sysfulltextcatalogs
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
		and status2 & 536870912 = 536870912
for read only
set nocount on 
open read_db_status 
fetch read_db_status into @dbname
while @@fetch_status = 0 
begin 
	set @command = 'use ' + quotename(@dbname) + '
	if DatabaseProperty(db_name(), ''IsFulltextEnabled'') <> 0
	if (select isnull(count(*),0) from sysfulltextcatalogs) <> 0
		select
			db_name(), 
			name,
			path,
			fulltextcatalogproperty(name,''AccentSensitivity'') as AccentSensitivity,
			fulltextcatalogproperty(name,''IndexSize'') as IndexSize,
			fulltextcatalogproperty(name,''ItemCount'') as ItemCount,
			case when isnull(fulltextcatalogproperty(name,''PopulateCompletionAge''),0) > 0 then dateadd(mi,datediff(mi,getdate(),getutcdate()),dateadd(ss,isnull(fulltextcatalogproperty(name,''PopulateCompletionAge''),0),''1990-01-01'')) else null end as LastPopulation,
			fulltextcatalogproperty(name,''PopulateStatus'') as PopulateStatus,
			fulltextcatalogproperty(name,''UniqueKeyCount'') as UniqueKeyCount,
			tablecount = (
				select 
					count(*) 
				from  
					sysobjects o (nolock)
				where o.type = ''U'' 
				and o.ftcatid = ft.ftcatid)
		from
			sysfulltextcatalogs ft'
	execute (@command)
	fetch read_db_status into @dbname
End 
Close read_db_status 
deallocate read_db_status 
