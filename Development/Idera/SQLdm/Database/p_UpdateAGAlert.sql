-- SQLdm 10.2.1 (Vamshi Krishna)

-- [p_UpdateAGAlert] Updates the AGGroupRoleChange Alerts in the Alerts Table

--DECLARE @UTCOccurrenceDateTime datetime
--SET @UTCOccurrenceDateTime = DATEADD(minute,-330,GETDATE())

--DECLARE @ServerName nvarchar(256)
--SET @ServerName = 'ServerName'

--DECLARE @Metric int
--SET @Metric = 116

--DECLARE @MinutesAgeAlerts int
--SET @MinutesAgeAlerts = 2
-- exec p_UpdateAGAlert @UTCOccurrenceDateTime,@ServerName, @Metric, @MinutesAgeAlerts

if (object_id('p_UpdateAGAlert') is not null)
begin
drop procedure [p_UpdateAGAlert]
end
go

create procedure [p_UpdateAGAlert] 
	@UTCOccurrenceDateTime datetime,
	@ServerName nvarchar(256),
	@Metric int,
	@MinutesAgeAlerts int
as
begin
	DECLARE @err int
	DECLARE @alertid bigint
	DECLARE @active bit

	IF (@ServerName <> '')
	BEGIN
			UPDATE [Alerts] SET
				[Active] = 0
			WHERE
				([Active] = 1 and [Metric] = @Metric) and
				([ServerName] = @ServerName) and
				(DATEADD(minute,@MinutesAgeAlerts,[UTCOccurrenceDateTime]) < @UTCOccurrenceDateTime) and
				Severity <> 1 -- Ignoring the OK alerts
	END

	SELECT @err = @@error

	RETURN @err
end
