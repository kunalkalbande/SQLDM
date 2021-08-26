--  Batch: Always On Toplogy
--  Tables: sys.availability_groups 
--			sys.availability_group_listeners
--			sys.dm_hadr_availability_replica_states
--			sys.availability_group_listener_ip_addresses
--			sys.availability_replicas
--			sys.databases
--------------------------------------------------------------------------------

-- Get servername
declare @hostName sysname
select @hostName = convert(sysname, serverproperty('servername'))

--Availability Group
select	ag.group_id
		,name as 'Groupname'
		,dns_name as 'listenerDNSName'
		,port as 'listnerPort'
		,ip_address as 'listenerIPAddress'
		,@hostName as 'ServerSourceName'
from master.sys.availability_groups ag
left join master.sys.availability_group_listeners agl on ag.group_id = agl.group_id
left join master.sys.availability_group_listener_ip_addresses aglip on agl.listener_id = aglip.listener_id

--Availability Replicas
select  Replicas.replica_id
		,Replicas.group_id
		,replica_server_name as 'Replica_Name'
		,failover_mode
		,availability_mode
		,role as 'Replica_Role'
		,drs.synchronization_health
		,redo_queue_size
		,redo_rate
		,log_send_queue_size
		,log_send_rate
		,primary_role_allow_connections
		,primary_role_allow_connections_desc
		,secondary_role_allow_connections
		,secondary_role_allow_connections_desc
		, ReplicaStates.role as Role,
		@hostName as 'ServerSourceName'
	from master.sys.availability_replicas as Replicas
		join master.sys.dm_hadr_availability_replica_states as ReplicaStates on Replicas.replica_id = ReplicaStates.replica_id
		join master.sys.dm_hadr_database_replica_states drs on Replicas.replica_id = drs.replica_id

--Availability Databases
select	dbrs.replica_id
		,group_id
		,db.name as 'DatabaseName'
		,db.database_id,
        dbrs.group_database_id,
		@hostName as 'ServerSourceName'
from master.sys.dm_hadr_database_replica_states dbrs
join master.sys.databases db on db.database_id = dbrs.database_id
