if (object_id('p_DeleteCounter') is not null)
begin
drop procedure p_DeleteCounter
end
go

CREATE PROCEDURE [dbo].p_DeleteCounter(
	@Metric int
)
AS
begin
	declare @e int

	BEGIN TRANSACTION

	-- mark the custom counter deleted
	UPDATE [MetricMetaData] 
		SET [Deleted] = 1
		WHERE Metric = @Metric

	SET @e = @@error
	IF (@e = 0)
	BEGIN
		-- mark the custom counter disabled
		UPDATE [CustomCounterDefinition] 
			SET [Enabled] = 0
			WHERE Metric = @Metric

		SET @e = @@error
		IF (@e = 0)
		BEGIN
			-- unmap the counter from instance
			DELETE FROM [CustomCounterMap]
				WHERE Metric = @Metric
		
			SET @e = @@error
		
			if (@e = 0)
			BEGIN
				DELETE FROM DefaultMetricThresholds 
					WHERE Metric = @Metric

				SET @e = @@error	
			END			
			if (@e = 0)
			BEGIN
				DELETE FROM MetricThresholds 
					WHERE Metric = @Metric
	
				SET @e = @@error	
			END
			
			if (@e = 0)			
			BEGIN
				UPDATE Alerts SET Active = 0
					WHERE AlertID in (
						SELECT AlertID FROM MonitoredSQLServers M (NOLOCK)
							join Alerts A (NOLOCK) on A.ServerName = M.InstanceName and
								A.UTCOccurrenceDateTime = M.LastScheduledCollectionTime and
								A.Active = 1 
						WHERE M.Active = 1 and
							  A.Metric = @Metric)
				
				SET @e = @@error		
			END
		END
	END

	IF (@e = 0)
		COMMIT
	ELSE
		ROLLBACK		

	return @e
END	

