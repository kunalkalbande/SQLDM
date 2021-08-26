if (object_id('p_GetMonitoredSqlServerNames') is not null)
begin
drop procedure p_GetMonitoredSqlServerNames
end
go
CREATE PROCEDURE [dbo].[p_GetMonitoredSqlServerNames]
(
	@CollectionServiceId UNIQUEIDENTIFIER = NULL,
	@ActiveOnly BIT = 1
)
AS
BEGIN
	DECLARE @error int

	SELECT 
		[SQLServerID],
		[InstanceName],
		[Active]
	FROM [MonitoredSQLServers] (NOLOCK)
	WHERE
		((@CollectionServiceId IS NULL) OR ([CollectionServiceID] = @CollectionServiceId)) AND
		((@ActiveOnly = 0) OR ([Active] = 1))
	
	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server names.', 10, 1)
        RETURN(@error)			
END