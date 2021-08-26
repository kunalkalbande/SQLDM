SELECT NAME FROM sys.databases
WHERE NAME NOT IN ('model', 'msdb', 'tempdb')

-- Azure database names with elastic pool details
SELECT d.name, dso.elastic_pool_name 
FROM sys.databases d (NOLOCK)
LEFT JOIN sys.database_service_objectives dso (NOLOCK)
ON d.database_id = dso.database_id AND dso.service_objective = 'ElasticPool';