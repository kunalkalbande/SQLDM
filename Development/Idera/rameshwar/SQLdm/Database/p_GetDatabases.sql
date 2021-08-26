if (object_id('p_GetDatabases') is not null)
begin
drop procedure p_GetDatabases
end
go
CREATE PROCEDURE [dbo].p_GetDatabases(
	@InstanceId int
)
AS
BEGIN
	--- Gets the list of databases for the instance id
;WITH cte_DistinctDatabases(DatabaseID, DatabaseName)
AS
(
	SELECT 
		DatabaseID = MIN(DatabaseID)
		,DatabaseName
	FROM SQLServerDatabaseNames
	WHERE SQLServerID = @InstanceId and DatabaseName is not null and len(DatabaseName) > 0
	GROUP BY DatabaseName
)

SELECT
	cte.DatabaseID
	,cte.DatabaseName
	,SystemDatabase
FROM
	SQLServerDatabaseNames dn
	inner join cte_DistinctDatabases cte on dn.DatabaseID = cte.DatabaseID
END
