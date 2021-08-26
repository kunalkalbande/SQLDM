if (object_id('p_GetTables') is not null)
begin
drop procedure p_GetTables
end
go
CREATE PROCEDURE [dbo].p_GetTables(
	@InstanceId int,
	@DatabaseName nvarchar(255)
)
AS
BEGIN
	--- Gets the list of databases for the instance id and db name
	SELECT 
		[TableID],
		[TableName],
		[SchemaName],
		[SystemTable]
	FROM SQLServerTableNames t, SQLServerDatabaseNames d
	WHERE	d.[SQLServerID] = @InstanceId
		AND	d.[DatabaseName] = @DatabaseName
		AND d.[DatabaseID] = t.[DatabaseID]
END

