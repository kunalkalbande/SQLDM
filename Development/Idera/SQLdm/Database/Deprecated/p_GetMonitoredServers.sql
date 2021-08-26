if (object_id('p_GetMonitoredServers') is not null)
begin
drop procedure p_GetMonitoredServers
end
go
CREATE PROCEDURE [dbo].[p_GetMonitoredServers](
	@SQLServerID INT,
	@CollectionServiceId UNIQUEIDENTIFIER
)
AS
BEGIN
	DECLARE @e INT

	IF (@SQLServerID IS NULL) 
	BEGIN
		IF (@CollectionServiceId IS NULL) BEGIN
			SELECT 
				[SQLServerID],
				[InstanceName],
				[Active],
				[RegisteredDate],
				[UseIntegratedSecurity],
				[Username],
				[Password],
				[MaintenanceModeEnabled],
				[ScheduledCollectionIntervalInSeconds],
				[CollectionServiceID],
				[EncryptData],
				[TrustServerCert],
				[MaintenanceModeType],
				[MaintenanceModeStart],
				[MaintenanceModeStop],
				[MaintenanceModeDurationSeconds],
				[MaintenanceModeDays],
				[MaintenanceModeRecurringStart]
			FROM MonitoredSQLServers
		END
		ELSE BEGIN
			SELECT 
				[SQLServerID],
				[InstanceName],
				[Active],
				[RegisteredDate],
				[UseIntegratedSecurity],
				[Username],
				[Password],
				[MaintenanceModeEnabled],
				[ScheduledCollectionIntervalInSeconds],
				[CollectionServiceID],
				[EncryptData],
				[TrustServerCert],
				[MaintenanceModeType],
				[MaintenanceModeStart],
				[MaintenanceModeStop],
				[MaintenanceModeDurationSeconds],
				[MaintenanceModeDays],
				[MaintenanceModeRecurringStart]
			FROM MonitoredSQLServers
			WHERE ([CollectionServiceID] = @CollectionServiceId)
		END
	END
	ELSE BEGIN
		IF (@CollectionServiceId IS NULL) BEGIN
			SELECT 
				[SQLServerID],
				[InstanceName],
				[Active],
				[RegisteredDate],
				[UseIntegratedSecurity],
				[Username],
				[Password],
				[MaintenanceModeEnabled],
				[ScheduledCollectionIntervalInSeconds],
				[CollectionServiceID],
				[EncryptData],
				[TrustServerCert],
				[MaintenanceModeType],
				[MaintenanceModeStart],
				[MaintenanceModeStop],
				[MaintenanceModeDurationSeconds],
				[MaintenanceModeDays],
				[MaintenanceModeRecurringStart]
			FROM MonitoredSQLServers
			WHERE ([SQLServerID] = @SQLServerID)
		END
		ELSE BEGIN
			SELECT 
				[SQLServerID],
				[InstanceName],
				[Active],
				[RegisteredDate],
				[UseIntegratedSecurity],
				[Username],
				[Password],
				[MaintenanceModeEnabled],
				[ScheduledCollectionIntervalInSeconds],
				[CollectionServiceID],
				[EncryptData],
				[TrustServerCert],
				[MaintenanceModeType],
				[MaintenanceModeStart],
				[MaintenanceModeStop],
				[MaintenanceModeDurationSeconds],
				[MaintenanceModeDays],
				[MaintenanceModeRecurringStart]
			FROM MonitoredSQLServers
			WHERE ([SQLServerID] = @SQLServerID) AND ([CollectionServiceID] = @CollectionServiceId)
		END
	END

	SELECT @e = @@error
	RETURN @e
END


