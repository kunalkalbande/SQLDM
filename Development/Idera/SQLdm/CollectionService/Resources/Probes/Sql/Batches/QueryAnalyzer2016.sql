----------------------------------------------------------------------------------------------
-- //SQLDm 10.0 Srishti Purohit - New Recommendations - SDR-Q46, SDR-Q47, SDR-Q48,SDR-Q49, Q50 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------

Select dbname = DB_NAME();

select actual_state from sys.database_query_store_options;

-- SDR-Q46

Select Top(10) rs.avg_duration,
	qt.query_sql_text,
	q.query_id,
    qt.query_text_id,
	p.plan_id,
    last_execution_time = DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), rs.last_execution_time )
From sys.query_store_query_text AS qt 
Inner Join sys.query_store_query AS q 
    ON qt.query_text_id = q.query_text_id 
Inner Join sys.query_store_plan AS p 
    ON q.query_id = p.query_id 
Inner Join sys.query_store_runtime_stats AS rs 
    ON p.plan_id = rs.plan_id
Where rs.last_execution_time > DateAdd(hour, -1, getutcdate())
Order By rs.avg_duration Desc;



-- SDR-Q47

Select Top(10) rs.avg_physical_io_reads,
	qt.query_sql_text, 
    q.query_id,
	qt.query_text_id,
	p.plan_id,
	rs.runtime_stats_id, 
    start_time = DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), rsi.start_time),
	end_time = DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), rsi.end_time),
	rs.avg_rowcount,
	rs.count_executions
From sys.query_store_query_text AS qt 
Inner Join sys.query_store_query AS q 
    ON qt.query_text_id = q.query_text_id 
Inner Join sys.query_store_plan AS p 
    ON q.query_id = p.query_id 
Inner Join sys.query_store_runtime_stats AS rs 
    ON p.plan_id = rs.plan_id 
Inner Join sys.query_store_runtime_stats_interval AS rsi 
    ON rsi.runtime_stats_interval_id = rs.runtime_stats_interval_id
Where rsi.start_time >= DateAdd(hour, -24, getutcdate()) 
Order By rs.avg_physical_io_reads Desc;


-- SDR-Q48

Select qt.query_sql_text, 
    q.query_id, 
    qt.query_text_id, 
    rs1.runtime_stats_id AS runtime_stats_id_1,
    interval_1 = DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), rsi1.start_time), 
    p1.plan_id AS plan_1, 
    rs1.avg_duration AS avg_duration_1, 
    rs2.avg_duration AS avg_duration_2,
    p2.plan_id AS plan_2, 
    interval_2 = DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), rsi2.start_time), 
    rs2.runtime_stats_id AS runtime_stats_id_2
From sys.query_store_query_text AS qt 
Inner Join sys.query_store_query AS q 
    ON qt.query_text_id = q.query_text_id 
Inner Join sys.query_store_plan AS p1 
    ON q.query_id = p1.query_id 
Inner Join sys.query_store_runtime_stats AS rs1 
    ON p1.plan_id = rs1.plan_id 
Inner Join sys.query_store_runtime_stats_interval AS rsi1 
    ON rsi1.runtime_stats_interval_id = rs1.runtime_stats_interval_id 
	Inner Join sys.query_store_plan AS p2 
    ON q.query_id = p2.query_id 
Inner Join sys.query_store_runtime_stats AS rs2 
    ON p2.plan_id = rs2.plan_id 
Inner Join sys.query_store_runtime_stats_interval AS rsi2 
    ON rsi2.runtime_stats_interval_id = rs2.runtime_stats_interval_id
Where rsi1.start_time > DATEADD(hour, -48, GETUTCDATE()) 
    AND rsi2.start_time > rsi1.start_time 
    AND p1.plan_id <> p2.plan_id
    AND rs2.avg_duration > rs1.avg_duration * 2
Order By q.query_id, rsi1.start_time, rsi2.start_time;


-- SDR- Q49

DECLARE @recent_start_time datetimeoffset;
DECLARE @recent_end_time datetimeoffset;
SET @recent_start_time = DATEADD(hour, -1, SYSUTCDATETIME());
SET @recent_end_time = SYSUTCDATETIME();

--- "History" workload
DECLARE @history_start_time datetimeoffset;
DECLARE @history_end_time datetimeoffset;
SET @history_start_time = DATEADD(hour, -24, SYSUTCDATETIME());
SET @history_end_time = SYSUTCDATETIME();

WITH historical
AS (SELECT p.query_id query_id, 
		CONVERT(float, SUM(rs.avg_duration*rs.count_executions)) total_duration, 
		SUM(rs.count_executions) count_executions,
		COUNT(distinct p.plan_id) num_plans 
	FROM sys.query_store_runtime_stats AS rs
	Inner Join sys.query_store_plan p ON p.plan_id = rs.plan_id
	WHERE  (rs.first_execution_time >= @history_start_time
	AND rs.last_execution_time < @history_end_time)
	OR (rs.first_execution_time <= @history_start_time
		AND rs.last_execution_time > @history_start_time)
	OR (rs.first_execution_time <= @history_end_time
		AND rs.last_execution_time > @history_end_time)
	GROUP BY p.query_id),
recent
AS (SELECT p.query_id query_id, 
		total_duration = CONVERT(float, SUM(rs.avg_duration * rs.count_executions)), 
		count_executions = SUM(rs.count_executions),
		num_plans = COUNT(distinct p.plan_id)
	FROM sys.query_store_runtime_stats AS rs
	Inner Join sys.query_store_plan p ON p.plan_id = rs.plan_id
	WHERE  (rs.first_execution_time >= @recent_start_time
		AND rs.last_execution_time < @recent_end_time)
	OR (rs.first_execution_time <= @recent_start_time
		AND rs.last_execution_time > @recent_start_time)
	OR (rs.first_execution_time <= @recent_end_time
		AND rs.last_execution_time > @recent_end_time)
	GROUP BY p.query_id),
results
AS (SELECT historical.query_id,
		qt.query_sql_text AS query_text,
		additional_duration_workload = ROUND(CONVERT(float, recent.total_duration / recent.count_executions - historical.total_duration / historical.count_executions) * (recent.count_executions), 2),
		total_duration_recent = ROUND(recent.total_duration, 2), 
		total_duration_hist = ROUND(historical.total_duration, 2),
		recent.count_executions AS count_executions_recent,
		historical.count_executions AS count_executions_hist   
	FROM historical 
	Inner Join recent 
		ON historical.query_id = recent.query_id 
	Inner Join sys.query_store_query AS q 
		ON q.query_id = historical.query_id
	Inner Join sys.query_store_query_text AS qt 
		ON q.query_text_id = qt.query_text_id)
SELECT Top(10) results.query_id,
	results.query_text,
	results.additional_duration_workload,
	results.total_duration_recent,
	results.total_duration_hist,
	count_executions_recent = ISNULL(results.count_executions_recent, 0),
	count_executions_hist = ISNULL(results.count_executions_hist, 0) 
From results
Where additional_duration_workload > 0
Order By additional_duration_workload Desc
Option (Merge Join);


--SDR-Q50

SELECT q.query_id ,
	PlanCount = COUNT(*),
	ProcedureName = Max(object_schema_name(q.object_id) + N'.' + object_name(q.object_id)),
	SQLText = Max(qt.query_sql_text),
	LastCompileTime = Max(DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), p.last_compile_start_time)), 
	LastExecutionTime = Max(DateAdd(minute, -(DateDiff(minute, getdate(), getutcdate())), p.last_execution_time))
FROM sys.query_store_query_text AS qt
Inner Join sys.query_store_query AS q
    ON qt.query_text_id = q.query_text_id
Inner Join sys.query_store_plan AS p
    ON p.query_id = q.query_id
Where p.last_compile_start_time > DateAdd(hour, -48, getutcdate())
Group By q.query_id
Having COUNT(distinct plan_id) > 4;