IF (object_id('p_DeleteMonitoredSqlServer') IS NOT NULL)
BEGIN
DROP PROCEDURE p_DeleteMonitoredSqlServer
END
GO

CREATE PROCEDURE [dbo].[p_DeleteMonitoredSqlServer]
(
	@SqlServerId INT
)
AS
BEGIN
	-- This just flags a server for deletion.  
	-- p_GroomRepository does the actual deletion on a scheduled basis.


	UPDATE AlwaysOnReplicas
	SET SQLServerID = -1
	WHERE SQLServerID = @SqlServerId
    
	UPDATE MonitoredSQLServers 
	SET Active = 0, Deleted = 1--, FriendlyServerName='' --SQLdm 10.1 (Pulkit Puri) --SQLDM-25856 (Commenting out the friendly server name =' ' )
	WHERE SQLServerID = @SqlServerId

	IF @@error != 0 GOTO HANDLE_ERROR
	
	DELETE FROM CustomCounterMap
		WHERE SQLServerID = @SqlServerId

	DELETE FROM ServerTags
		WHERE SQLServerId = @SqlServerId

	-- Grooming and Aggregation Requirement to delete cascade with MonitoredSqlServer
	DELETE FROM AnalysisConfiguration WHERE MonitoredServerID = @SqlServerId;

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
		RAISERROR('An error occurred while deleting instance %d.', 10, 1, @SqlServerId)
        RETURN(@@error)
END