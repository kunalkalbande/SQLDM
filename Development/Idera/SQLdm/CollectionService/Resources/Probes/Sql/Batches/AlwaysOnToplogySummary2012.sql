--  Batch: Always On Toplogy
--  Tables: sys.availability_groups 
--			sys.availability_group_listeners
--------------------------------------------------------------------------------

-- Get servername
declare @hostName sysname
select @hostName = convert(sysname, serverproperty('servername'))

--Availability Groups
select	ag.group_id
		,name as 'Groupname'
from master.sys.availability_groups ag

--Availability Replicas
select  Replicas.replica_id
		,Replicas.group_id
		,replica_server_name as 'Replica_Name'
	from master.sys.availability_replicas as Replicas
