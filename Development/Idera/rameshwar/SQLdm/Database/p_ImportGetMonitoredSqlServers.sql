if (object_id('p_ImportGetMonitoredSqlServers') is not null)
begin
drop procedure p_ImportGetMonitoredSqlServers
end
go
CREATE PROCEDURE [dbo].[p_ImportGetMonitoredSqlServers]
(
	@ActiveOnly BIT = 0
)
AS
BEGIN
	DECLARE @error int

	SELECT 
		[SQLServerID],
		[InstanceName],
		[ServerVersion],
		[Active],
		[RegisteredDate],
		[EarliestDateImportedFromLegacySQLdm]
	FROM [MonitoredSQLServers]
	WHERE
		((@ActiveOnly = 0) OR ([Active] = 1))

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server instances.', 10, 1)
        RETURN(@error)			
END