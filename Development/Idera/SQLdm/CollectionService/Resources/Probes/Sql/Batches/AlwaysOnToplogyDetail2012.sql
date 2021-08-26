--  Batch: Always On Toplogy
--  Tables: sys.dm_hadr_availability_replica_states
--			sys.availability_group_listener_ip_addresses
--			sys.availability_replicas
--			sys.databases
--------------------------------------------------------------------------------

--Availability Databases
select	dbrs.replica_id
		,group_id
		,db.name as 'DatabaseName'
		,db.database_id,
        dbrs.group_database_id
from master.sys.dm_hadr_database_replica_states dbrs
join master.sys.databases db on db.database_id = dbrs.database_id
where group_id = '{0}' and dbrs.replica_id = '{1}'
