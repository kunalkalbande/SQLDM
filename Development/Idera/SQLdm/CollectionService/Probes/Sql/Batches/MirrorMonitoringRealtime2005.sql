set nocount on

use msdb 
declare @dbname sysname, -- Name of database
@guid uniqueidentifier,
@partner_instance nvarchar(128),		--Partner instance
@mirroring_role int,     -- 1 = Principal, 2 = Mirror
@mirroring_state int,	 -- 0 = Suspended, 1 = Disconnected, 2 = Synchronizing, 3 = Pending Failover, 4 = Synchronized
@witness_state int,		 -- 1 = Connected, 2 = Disconnected
@safety_level int,       -- 0 = Unknown, 1 = Off\asynch, 2 = Full\synch
@witness_name nvarchar(128),
@partner_name nvarchar(128),
@connection_timeout int


declare mirrored_db_cursor cursor local for 			
--This is executed once for every database that is being monitored, and against each instance
--This is the sql for populating the realtime screen
select d.name, 
dm.mirroring_guid, 
dm.mirroring_partner_instance, 
dm.mirroring_role, 
dm.mirroring_state, 
dm.mirroring_witness_state,
dm.mirroring_safety_level, 
dm.mirroring_witness_name,
dm.mirroring_partner_name,
dm.mirroring_connection_timeout
from sys.database_mirroring dm join sys.databases d on (dm.database_id=d.database_id) 
where mirroring_guid is not null

open mirrored_db_cursor

fetch next from mirrored_db_cursor
into @dbname, @guid, @partner_instance, 
@mirroring_role, @mirroring_state, 
@witness_state, @safety_level, 
@witness_name, @partner_name, @connection_timeout

while @@fetch_status = 0
begin

   select 'Database' = @dbname,
   'LocalInstance' = SERVERPROPERTY('ServerName'),
   'PartnerInstance' = @partner_instance,
   'Mirroring State' = @mirroring_state,
   'Witness State' = @witness_state,
   'Guid' = @guid, 
   'Safety Level' = @safety_level,
   'WitnessName' = @witness_name,
   'PartnerName' = @partner_name,
   'Role' = @mirroring_role
   
   --setting @update_table to 1 so it updates the mirror monitoring system tables
   --This will reult in additional history but more importantly the data will be more current
   exec sys.sp_dbmmonitorresults @database_name=@dbname, @mode = 0, @update_table = 1
   
   fetch next from mirrored_db_cursor into @dbname, @guid, @partner_instance, 
   @mirroring_role, @mirroring_state, 
   @witness_state, @safety_level, 
   @witness_name, @partner_name, @connection_timeout
end

close mirrored_db_cursor
deallocate mirrored_db_cursor

select 'endMirrorMonitoring'