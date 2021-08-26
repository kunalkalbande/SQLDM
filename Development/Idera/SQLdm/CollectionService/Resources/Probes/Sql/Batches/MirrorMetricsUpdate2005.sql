set nocount on

use msdb 

declare @dbname sysname
declare mirrored_dbs cursor local for 			
--This is executed once for every database that is being monitored, and against each instance
--This is the sql for updating the mirror monitor system tables
select d.name
from sys.database_mirroring dm join sys.databases d on (dm.database_id=d.database_id) 
where mirroring_guid is not null

open mirrored_dbs

	fetch next from mirrored_dbs
	into @dbname

	while @@fetch_status = 0
	begin
	   	IF IS_SRVROLEMEMBER('sysadmin') = 1
	   	BEGIN
	   	    exec sp_dbmmonitorupdate @dbname
	   	END
	   fetch next from mirrored_dbs into @dbname
	end
 
close mirrored_dbs
deallocate mirrored_dbs