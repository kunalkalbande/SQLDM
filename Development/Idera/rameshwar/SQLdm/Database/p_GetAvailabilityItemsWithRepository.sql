if (object_id('p_GetAvailabilityItemsWithRepository') is not null)
begin
drop procedure [p_GetAvailabilityItemsWithRepository]
end
go

create procedure [p_GetAvailabilityItemsWithRepository]
as
begin
	select dbrs.replica_id
		,group_id
		,db.database_id,
		dbrs.group_database_id
	from master.sys.dm_hadr_database_replica_states dbrs
	join master.sys.databases db on db.database_id = dbrs.database_id
	where db.name = DB_NAME() and db.is_read_only <> 0
end