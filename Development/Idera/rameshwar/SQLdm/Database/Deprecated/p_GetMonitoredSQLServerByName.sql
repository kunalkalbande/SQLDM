if (object_id('p_GetMonitoredSQLServerByName') is not null)
begin
drop procedure p_GetMonitoredSQLServerByName
end
go
CREATE PROCEDURE [dbo].[p_GetMonitoredSQLServerByName](
	@InstanceName nvarchar(256)
)
AS
BEGIN
	DECLARE @e INT

	IF (@InstanceName IS NULL) 
	BEGIN
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
		FROM [MonitoredSQLServers]
	END
	ELSE
	BEGIN
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
		FROM [MonitoredSQLServers]
		WHERE ([InstanceName] = @InstanceName)
	END

	SELECT @e = @@error
	RETURN @e
END


