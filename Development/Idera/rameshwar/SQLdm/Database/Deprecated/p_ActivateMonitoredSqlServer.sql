IF (object_id('p_ActivateMonitoredSqlServer') IS NOT NULL)
BEGIN
DROP PROCEDURE p_ActivateMonitoredSqlServer
END
GO

CREATE PROCEDURE [dbo].[p_ActivateMonitoredSqlServer]
(
	@SqlServerId INT
)
AS
BEGIN
	UPDATE [dbo].[MonitoredSQLServers]
	SET Active = 1,
		Deleted = 0
	WHERE SQLServerID = @SqlServerId

	IF @@error != 0 GOTO HANDLE_ERROR

	IF @@rowcount = 1 
	BEGIN
		-- get monitored server, linked counters & server tags
		exec p_GetMonitoredSqlServerById @SqlServerId, 1
	END

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while activating instance %d.', 10, 1, @SqlServerId)
        RETURN(@@error)
END