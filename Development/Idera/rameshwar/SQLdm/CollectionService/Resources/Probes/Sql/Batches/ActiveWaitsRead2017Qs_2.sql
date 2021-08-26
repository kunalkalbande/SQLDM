-- Batch Read Query Waits Data using Query Store 2017
-- Added new batch for SQLdm 10.4 by Varun Chopra
-- ActiveWaitsRead2017Qs.sql
-- (0) FilterString               -  Filters for Query Store for Waits with generic predicates on SQL Text and Duration / Reads / Writes / CPU
-- (1) Session Filter             -  To apply the Application name and exclude SQLdm Filters
-- (2) Database Filter            -  To add conditions like db in / db not in / db matches / db not matches
-- (3) TopPlanCountFilter         -  Top X Queries to be selected
-- (4) LastStartTime			  -  Last Start Time (GETUTCDATE() for first run and value for subsequent runs)

-- NOTE:
-- [Ticks] calculated using StartTime.Ticks; 
-- [MSTicks] calculated -1 * (long.Parse(row["WaitDuration"].ToString())
--
-- Create the temp table ActiveWaitsQsState if not exists for latest query data and offset to continue reading from LastStartTime
-- LastStartTime defaults to UTC Time in the begining
-- Generally, ActiveWaitsQsState gets refreshed during restart and older values will persist if query store is disabled currently

IF EXISTS (
		SELECT 1
		FROM sys.database_query_store_options
		WHERE actual_state = 1
			OR actual_state = 2
		)
	AND {2}
BEGIN
	DECLARE @DBName NVARCHAR(MAX) = DB_NAME(DB_ID());
	DECLARE @LastStartTime DATETIMEOFFSET = {4};

	WITH qd
	AS (
		SELECT qsws.last_query_wait_time_ms AS [WaitDuration]
			,qsws.wait_category_desc AS [Wait Type]
			,qsqt.query_sql_text AS [statement_txt]
			,DATEADD(microsecond, - qsrs.last_duration, qsrs.last_execution_time) AS [StartTime]
			,qsq.query_hash -- For merging with Sessions
		FROM sys.query_store_wait_stats qsws
		INNER JOIN sys.query_store_plan qsp ON qsp.plan_id = qsws.plan_id
		INNER JOIN sys.query_store_query qsq ON qsp.query_id = qsq.query_id
		INNER JOIN sys.query_store_query_text qsqt ON qsqt.query_text_id = qsq.query_text_id
		INNER JOIN sys.query_store_runtime_stats_interval qsrsi ON qsws.runtime_stats_interval_id = qsrsi.runtime_stats_interval_id
		INNER JOIN sys.query_store_runtime_stats qsrs ON qsrs.plan_id = qsp.plan_id
		LEFT JOIN sys.objects so ON so.object_id = qsq.object_id
		{0}
		)
		,
		-- session data
	sd
	AS (
		SELECT des.session_id AS [session_id]
			,des.host_name AS [HostName]
			,des.program_name AS [program_name]
			,des.login_name AS [LoginName]
			,der.query_hash
		FROM sys.dm_exec_requests der
		LEFT JOIN sys.dm_exec_sessions des ON der.session_id = des.session_id
		{1}
		)
	SELECT {3}
		qd.[WaitDuration]
		,sd.[session_id]
		,qd.[Wait Type]
		,sd.HostName
		,sd.[program_name]
		,sd.[LoginName]
		,@DBName AS [DatabaseName]
		,qd.[statement_txt]
		,qd.[StartTime]
	FROM qd
	LEFT OUTER JOIN sd ON qd.query_hash = sd.query_hash
	GROUP BY
		[session_id],
		[Wait Type],
		[WaitDuration],
		[HostName],
		[program_name],
		[LoginName],
		[statement_txt],
		[StartTime]
END