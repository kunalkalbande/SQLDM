--{0}-Name of the database

-- Azure database names with elastic pool details
SELECT d.name, dso.elastic_pool_name 
FROM sys.databases d (NOLOCK)
LEFT JOIN sys.database_service_objectives dso (NOLOCK)
ON d.database_id = dso.database_id AND dso.service_objective = 'ElasticPool'
where d.name='{0}';