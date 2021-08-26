IF (object_id('p_DeactivateMonitoredSqlServer') IS NOT NULL)
BEGIN
DROP PROCEDURE p_DeactivateMonitoredSqlServer
END
GO

CREATE PROCEDURE [dbo].[p_DeactivateMonitoredSqlServer]
(
	@SqlServerId INT
)
AS
BEGIN
	UPDATE MonitoredSQLServers 
	SET Active = 0--, FriendlyServerName='' --SQLdm 10.1 (Pulkit Puri) --SQLDM-25856 (Commenting out the friendly server name =' ' )
	WHERE SQLServerID = @SqlServerId

	IF @@error != 0 GOTO HANDLE_ERROR

	DELETE FROM CustomCounterMap
		WHERE SQLServerID = @SqlServerId

	DELETE FROM ServerTags
		WHERE SQLServerId = @SqlServerId

	-- delete alerts 
--	DELETE FROM Alerts
--		WHERE ServerName in (SELECT InstanceName from MonitoredSQLServers 
--						      WHERE SQLServerID = @SqlServerId
--						     )
	-- delete todos
--	DELETE FROM Tasks
--		WHERE ServerName in (SELECT InstanceName from MonitoredSQLServers 
--						      WHERE SQLServerID = @SqlServerId
--						     )

	RETURN(0)

	HANDLE_ERROR:
		RAISERROR('An error occurred while deactivating instance %d.', 10, 1, @SqlServerId)
        RETURN(@@error)
END