use tempdb;

;with querystats as
(
	select top (200)
        [execution_count]=min(qs.execution_count),
        [total_logical_reads]=sum(qs.total_logical_reads),
        [min_logical_reads]=sum(qs.min_logical_reads),
        [max_logical_reads]=sum(qs.max_logical_reads),
        [total_logical_writes]=sum(qs.total_logical_writes),
        [min_logical_writes]=sum(qs.min_logical_writes),
        [max_logical_writes]=sum(qs.max_logical_writes),
        [total_worker_time]=sum(qs.total_worker_time),
        [min_worker_time]=sum(qs.min_worker_time),
        [max_worker_time]=sum(qs.max_worker_time),
        [total_elapsed_time]=sum(qs.total_elapsed_time), 
        [min_elapsed_time]=sum(qs.min_elapsed_time), 
        [max_elapsed_time]=sum(qs.max_elapsed_time),
		qs.sql_handle,
		qs.plan_handle
		from sys.dm_exec_query_stats as qs 
		where last_execution_time >= dateadd(day, -1, getdate())
		group by qs.sql_handle, qs.plan_handle
		order by [total_elapsed_time] desc
)
select 
		[EventSequence]=0,
		[DatabaseID]=isnull(st.dbid, (select cast(value as int) from sys.dm_exec_plan_attributes(qs.plan_handle) where attribute = 'dbid')),
		st.objectid,
		[ObjectName]='',-- object_name(st.objectid, st.dbid), this requires SQL2005 SP2, remove the usage of object_name(id, dbid)
		cp.objtype,
        qs.execution_count,
        qs.total_logical_reads,
        qs.min_logical_reads,
        [Reads]=qs.max_logical_reads,
        qs.total_logical_writes,
        qs.min_logical_writes,
        [Writes]=qs.max_logical_writes,
        qs.total_worker_time,
        qs.min_worker_time,
        [CPU]=qs.max_worker_time,
        qs.total_elapsed_time, 
        qs.min_elapsed_time, 
        [Duration]=qs.max_elapsed_time, 
        [TextData]=st.text,
		[Plan]=qp.query_plan
	from querystats as qs
	join sys.dm_exec_cached_plans cp ON qs.plan_handle = cp.plan_handle 
	cross apply sys.dm_exec_query_plan(qs.plan_handle) as qp
	cross apply sys.dm_exec_sql_text(qs.sql_handle) st
	order by total_elapsed_time desc;