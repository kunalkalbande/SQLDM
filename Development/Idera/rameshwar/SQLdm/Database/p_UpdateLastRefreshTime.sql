if (object_id('p_UpdateLastRefreshTime') is not null)
begin
drop procedure [p_UpdateLastRefreshTime]
end
go

create procedure [p_UpdateLastRefreshTime] (
	@SqlServerID int,
    @UTCLastScheduledCollectionTime datetime = null
)
as
begin
	DECLARE @err int

	if @UTCLastScheduledCollectionTime is null
		SET @UTCLastScheduledCollectionTime = GetUTCDate()

	UPDATE MonitoredSQLServers SET
			[LastScheduledCollectionTime] = @UTCLastScheduledCollectionTime,
			[LastAlertRefreshTime] = @UTCLastScheduledCollectionTime
		WHERE [SQLServerID] = @SqlServerID

	SELECT @err = @@error
	
	RETURN @err
end
