SELECT database_name, max(end_time) AS CurrentTime, SKU, DTU_Limit
FROM sys.resource_stats
WHERE database_name = '{dbname}' AND end_time >= dateadd(dd,-1,current_timestamp)
GROUP BY database_name, SKU, DTU_Limit
ORDER BY CurrentTime DESC;