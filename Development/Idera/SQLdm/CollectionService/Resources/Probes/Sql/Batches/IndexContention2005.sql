----------------------------------------------------------------------------------------------
--  Test for index contention.
----------------------------------------------------------------------------------------------

declare @dbid int
select @dbid = db_id()

-- return the top 10 indexes that have page latch contention
select top 10
	[DatabaseName]=db_name(), 
	[Schema]=(Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = s.object_id), 
	[TableName]=object_name(s.object_id), 
	[IndexName]=i.name,
    [Partition]=s.partition_number,
	[PageLatchWaitCount]=page_latch_wait_count,
    [PageLatchWaitInMs]=page_latch_wait_in_ms,
    [AvgPageLatchWaitInMs]=case page_latch_wait_count when 0 then 0 else cast((1.0 * page_latch_wait_in_ms) / page_latch_wait_count as numeric(15,2)) end 
	from sys.dm_db_index_operational_stats (@dbid, NULL, NULL, NULL) s
		left outer join sys.indexes i on i.object_id = s.object_id and i.index_id = s.index_id
	where objectproperty(s.object_id,'IsUserTable') = 1
		and objectproperty(s.object_id,'IsMSShipped') = 0
		and s.page_latch_wait_count > 1
	order by [PageLatchWaitCount] desc

-- return the top 10 indexes that have page lock contention
select top 10
	[DatabaseName]=db_name(), 
	[Schema]=( Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = s.object_id), 
	[TableName]=object_name(s.object_id), 
	[IndexName]=i.name,
    [Partition]=s.partition_number,
    [PageLockCount]=page_lock_count, 
	[PageLockWaitCount]=page_lock_wait_count,
    [PageLockPercent]=case page_lock_count when 0 then 0 else cast((100.0 * page_lock_wait_count) / page_lock_count as numeric(15,2)) end, 
    [PageLockWaitInMs]=page_lock_wait_in_ms,
    [AvgPageLockWaitInMs]=case page_lock_wait_count when 0 then 0 else cast((1.0 * page_lock_wait_in_ms) / page_lock_wait_count as numeric(15,2)) end 
	from sys.dm_db_index_operational_stats (@dbid, NULL, NULL, NULL) s
		left outer join sys.indexes i on i.object_id = s.object_id and i.index_id = s.index_id
	where objectproperty(s.object_id,'IsUserTable') = 1
		and objectproperty(s.object_id,'IsMSShipped') = 0
		and s.page_lock_wait_count > 1
	order by [PageLockPercent] desc

-- return the top 10 indexes that have row lock contention
select top 10
	[DatabaseName]=db_name(), 
	[Schema]=(Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = s.object_id), 
	[TableName]=object_name(s.object_id), 
	[IndexName]=i.name,
    [Partition]=s.partition_number,
    [RowLockCount]=row_lock_count, 
	[RowLockWaitCount]=row_lock_wait_count,
    [RowLockPercent]=case row_lock_count when 0 then 0 else cast((100.0 * row_lock_wait_count) / row_lock_count as numeric(15,2)) end, 
    [RowLockWaitInMs]=row_lock_wait_in_ms,
    [AvgRowLockWaitInMs]=case row_lock_wait_count when 0 then 0 else cast((1.0 * row_lock_wait_in_ms) / row_lock_wait_count as numeric(15,2)) end 
	from sys.dm_db_index_operational_stats (@dbid, NULL, NULL, NULL) s
		left outer join sys.indexes i on i.object_id = s.object_id and i.index_id = s.index_id
	where objectproperty(s.object_id,'IsUserTable') = 1
		and objectproperty(s.object_id,'IsMSShipped') = 0
		and s.row_lock_wait_count > 1
	order by [RowLockPercent] desc

