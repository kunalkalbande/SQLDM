----------------------------------------------------------------------------------------------
-- //SQLdm 10.0 - Srishti Purohit - New Recommendations - SDR-I25, SDR-I26, SDR-I28 - Adding New Batch
--
--	
----------------------------------------------------------------------------------------------
select serverproperty('Edition');


   Select
    DatabaseName = DB_NAME(), 
	SchemaName = OBJECT_SCHEMA_NAME(HI.object_id),
    TableName = OBJECT_NAME(HI.object_id),
	IndexName = HI.name,
	HIS.total_bucket_count,
	HIS.empty_bucket_count,
	EmptyBucketPercent = FLOOR((HIS.empty_bucket_count * 100.0)/HIS.total_bucket_count),
	HIS.avg_chain_length,
	HIS.max_chain_length
From sys.hash_indexes HI
Inner Join sys.dm_db_xtp_hash_index_stats HIS On HIS.object_id = HI.object_id And HIS.index_id = HI.index_id;

-- //SQLdm 10.0 - Srishti Purohit - New Recommendations - SDR-I27
WITH XMLNAMESPACES ('http://schemas.microsoft.com/sqlserver/2004/07/showplan' AS sqlx) 
SELECT DISTINCT 
	dbname = DB_NAME(),
    --ProcedureName = IsNull(object_name(st.objectid, st.dbid), N'Ad hoc or object not found'),
    --qp.query_plan AS QueryPlan,
	IndexName = I.n.value('(//sqlx:IndexScan/sqlx:Object/@Index)[1]', 'sysname'),
	SchemaName = I.n.value('(//sqlx:IndexScan/sqlx:Object/@Schema)[1]', 'sysname'),
	TableName = I.n.value('(//sqlx:IndexScan/sqlx:Object/@Table)[1]', 'sysname')
FROM sys.dm_exec_cached_plans AS cp 
Cross Apply sys.dm_exec_query_plan(cp.plan_handle) AS qp 
Cross Apply sys.dm_exec_sql_text(cp.plan_handle) AS st
Cross Apply qp.query_plan.nodes('//sqlx:IndexScan[@Storage="MemoryOptimized"]') As I(n)
Inner Join sys.hash_indexes HI On Quotename(HI.name) = I.n.value('(//sqlx:IndexScan/sqlx:Object/@Index)[1]', 'sysname')
And HI.object_id = OBJECT_ID(I.n.value('(//sqlx:IndexScan/sqlx:Object/@Schema)[1]', 'sysname') + N'.' + I.n.value('(//sqlx:IndexScan/sqlx:Object/@Table)[1]', 'sysname'))
WHERE qp.dbid = DB_ID()
AND I.n.exist('//sqlx:IndexScan/sqlx:Object[@IndexKind="NonClusteredHash"]') = 1;
