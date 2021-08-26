if (object_id('p_UpdateCounterStatus') is not null)
begin
drop procedure p_UpdateCounterStatus
end
go

CREATE PROCEDURE [dbo].p_UpdateCounterStatus(
	@Metric int,
	@Enabled bit
)
AS
begin
	declare @e int

	BEGIN TRANSACTION

	-- mark the custom counter disabled
	UPDATE [CustomCounterDefinition] 
		SET [Enabled] = @Enabled
		WHERE Metric = @Metric
	SET @e = @@error

	IF (@e = 0)
	BEGIN
		-- deactivate alerts
		UPDATE Alerts SET [Active] = 0
			WHERE [Metric] = @Metric	
		SET @e = @@error		
	END

	IF (@e = 0)
		COMMIT
	ELSE
		ROLLBACK		

	return @e
END	

