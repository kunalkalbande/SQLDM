IF (object_id('p_GetEarliestDataAvailable') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetEarliestDataAvailable
END
GO

CREATE PROCEDURE [dbo].[p_GetEarliestDataAvailable]
(
	@SqlServerId int,
	@EarliestStatisticsAvailable datetime output
)
AS
BEGIN

	DECLARE @err INT
	
	SELECT @err = @@error
	IF (@err = 0)
	BEGIN
		SELECT TOP 1 @EarliestStatisticsAvailable = [UTCCollectionDateTime] 
		FROM ServerStatistics
		WHERE [SQLServerID] = @SqlServerId
		ORDER BY [UTCCollectionDateTime] ASC
	
		SELECT @err = @@error
	END
	
	RETURN @err

END