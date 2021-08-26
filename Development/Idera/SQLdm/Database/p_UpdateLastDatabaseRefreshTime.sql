if (object_id('p_UpdateLastDatabaseRefreshTime') is not null)
begin
drop procedure [p_UpdateLastDatabaseRefreshTime]
end
go

create procedure [p_UpdateLastDatabaseRefreshTime] (
	@SqlServerID int,
    @UTCLastDatabaseCollectionTime datetime = null
)
as
begin
	DECLARE @err int

	if @UTCLastDatabaseCollectionTime is null
		SET @UTCLastDatabaseCollectionTime = GetUTCDate()

	UPDATE MonitoredSQLServers SET
			[LastDatabaseCollectionTime] = @UTCLastDatabaseCollectionTime,
			[LastAlertRefreshTime] = @UTCLastDatabaseCollectionTime
			
		WHERE [SQLServerID] = @SqlServerID

	SELECT @err = @@error
	
	RETURN @err
end
