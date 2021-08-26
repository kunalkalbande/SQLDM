IF (object_id('p_GetInstanceSQLServerById') is not null)
BEGIN
	DROP PROCEDURE [p_GetInstanceSQLServerById]
END
GO
CREATE PROCEDURE [dbo].[p_GetInstanceSQLServerById](
	@InstanceName nvarchar(MAX)
)
AS
	BEGIN
		SELECT [SQLServerID],[InstanceName] 
		FROM [MonitoredSQLServers] m 
		WHERE m.InstanceName IN (select Value from dbo.fn_Split(@InstanceName,','))
	END

	
