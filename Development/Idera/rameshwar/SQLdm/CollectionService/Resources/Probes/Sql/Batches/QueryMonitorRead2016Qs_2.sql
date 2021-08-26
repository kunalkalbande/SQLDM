-- Batch  Read Query Monitor Query Store 2016
-- Added new batch for SQLdm 10.4 by Varun Chopra
-- QueryMonitorRead2016Qs.sql
-- (0) topPlanCategory            -  Top Plan Category i.e Duration / Reads / Writes / CPU
-- (1) FilterString               -  Filters for Query Store with generic predicates on SQL Text and Duration / Reads / Writes / CPU
-- (2) TopPlanCountFilter         -  Top X Queries to be selected  to be handled in C# cumulative
-- (3) Session Filter             -  To apply the Application name and exclude SQLdm Filters
-- (4) Database Filter            -  To add conditions like db in / db not in / db matches / db not matches
-- (5) Collect Actual Plans       -  Set to 1 if we need to collect actual plan, 0 otherwise to be handled in C#
-- (6) LastStartTime			  -  Last Start Time (GETUTCDATE() for first run and value for subsequent runs)
--
-- Create the temp table QMQueryStoreState if not exists for latest query data and offset to continue reading from LastStartTime
-- LastStartTime defaults to UTC Time in the begining
-- Generally, QueryMonitorQsState gets refreshed during restart and older values will persist if query store is disabled currently

IF EXISTS (SELECT 1 FROM sys.database_query_store_options WHERE actual_state = 1 OR actual_state = 2) AND {4}
BEGIN
	DECLARE @DBName NVARCHAR(MAX) = DB_NAME(DB_ID());
	DECLARE @LastStartTime DATETIMEOFFSET = {6};

	WITH qd
	AS (
		SELECT so.type AS StatementType
			,qsrs.last_duration AS Duration
			,qsrs.last_logical_io_reads AS Reads
			,qsrs.last_logical_io_writes AS Writes
			,qsrs.last_cpu_time AS CPU
			,qsqt.query_sql_text AS TextData
			,so.name AS ObjectName
			,qsrs.last_execution_time AS StartTime
			,qsp.plan_id
			,--  For merging with non top query data
			qsq.query_hash -- For merging with Sessions
		FROM sys.query_store_query qsq
		INNER JOIN sys.query_store_query_text qsqt ON qsqt.query_text_id = qsq.query_text_id
		INNER JOIN sys.query_store_plan qsp ON qsp.query_id = qsq.query_id
		INNER JOIN sys.query_store_runtime_stats qsrs ON qsrs.plan_id = qsp.plan_id
		LEFT JOIN sys.objects so ON so.object_id = qsq.object_id
        {1}
		)
		,
		-- session data
	sd
	AS (
		SELECT des.nt_user_name AS NTUserName
			,des.host_name AS HostName
			,des.program_name AS ApplicationName
			,des.login_name AS LoginName
			,des.session_id AS SPID
			,der.query_hash
		FROM sys.dm_exec_sessions des
		INNER JOIN sys.dm_exec_requests der ON der.session_id = des.session_id
        {3}
		)
		,QueryData
	AS (
		SELECT qd.StatementType
			,qd.Duration
			,sd.NTUserName
			,sd.HostName
			,sd.ApplicationName
			,sd.LoginName
			,qd.Reads
			,qd.Writes
			,qd.CPU
			,qd.TextData
			,qd.ObjectName
			,sd.SPID
			,qd.StartTime
			,qd.plan_id AS PlanId
		FROM qd
		LEFT JOIN sd ON qd.query_hash = sd.query_hash
		)
		,QueryPlanData
	AS (
		SELECT qsp.plan_id AS PlanId
			,qsp.query_plan AS QueryPlan
		FROM sys.query_store_plan qsp
		WHERE 1 = {5}
			AND qsp.plan_id IN (
				SELECT TOP ({2}) PlanId
				FROM QueryData
				ORDER BY {0} DESC
				)
		)
	SELECT StatementType
		,Duration
		,@DBName AS DBName
		,NTUserName
		,HostName
		,ApplicationName
		,LoginName
		,Reads
		,Writes
		,CPU
		,TextData
		,ObjectName
		,SPID
		,StartTime
		,qpd.QueryPlan
	FROM QueryData queryData
	LEFT JOIN QueryPlanData qpd ON queryData.PlanId = qpd.PlanId
	ORDER BY Duration DESC
END
