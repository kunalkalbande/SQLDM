if (object_id('p_GetMonitoredSqlServerId') is not null)
begin
drop procedure p_GetMonitoredSqlServerId
end
go
CREATE PROCEDURE [dbo].p_GetMonitoredSqlServerId(
	@InstanceName nvarchar(255)
)
AS
BEGIN
	DECLARE @error int

	SELECT	[SQLServerID]
	FROM	[MonitoredSQLServers]
	WHERE	lower([InstanceName]) collate database_default = lower(@InstanceName)

	SELECT @error = @@error

	IF @error != 0 GOTO HANDLE_ERROR

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while getting the monitored SQL Server instance %s.', 10, 1, @InstanceName)
        RETURN(@error)
END