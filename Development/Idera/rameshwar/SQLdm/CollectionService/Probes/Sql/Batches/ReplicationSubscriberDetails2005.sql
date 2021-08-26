--------------------------------------------------------------------------------
--  Batch: Subscribed Databases
--  Tables: sysdatabases
--------------------------------------------------------------------------------
declare @dbname sysname 
declare subscribed_databases 
insensitive cursor for 
	select 
		name 
	from 
		master..sysdatabases d(nolock) 
	where 
		(category & 2 = 2 or category & 8 = 8) 
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
for read only 
open subscribed_databases 
fetch subscribed_databases into @dbname 
while @@fetch_status = 0 
begin 
	select 'subscribed_database', @dbname 
	exec('exec [' + @dbname + ']..sp_MSenumsubscriptions') 
	fetch subscribed_databases into @dbname 
end 
Close subscribed_databases 
deallocate subscribed_databases 

