----------------------------------------------------------------------------------------------
--  Return the number of times a table is updated per second.
--	
----------------------------------------------------------------------------------------------
declare @objid int;
declare @dbid int;
declare @minUp int;
declare @updates bigint;

set @dbid = db_id({Database});
set @objid = object_id({SchemaTable});

select @minUp = datediff(mi,create_date,getdate()) from sys.databases where name = 'tempdb';
if @minUp <= 0 set @minUp = 1;

select top 1 @updates = isnull(s.user_updates, 0) from sys.dm_db_index_usage_stats s
  where s.database_id = @dbid
  and s.object_id = @objid
  and index_id <= 1

select [UpdatesPerMin]=(@updates / (@minUp * 1.0))