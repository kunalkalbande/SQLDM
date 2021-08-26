if (object_id('p_DeleteMetricThresholdInstance') is not null)
begin
drop procedure [p_DeleteMetricThresholdInstance]
end
go

create procedure [p_DeleteMetricThresholdInstance]
	@UserViewID int,
	@SQLServerID int,
	@Metric int,
	@ThresholdInstanceName nvarchar(256),
	@ThresholdInstanceType int
as
BEGIN
	declare @InstanceID int 
	declare @err int

	-- check to see if trying to delete the default instance
	if (@ThresholdInstanceName is null or @ThresholdInstanceName = '')
	BEGIN
		RAISERROR('Cannot delete the Default Threshold Instance', 10,1,-1)
		return (-1)
	END
	
	if (exists 
			(select 
				InstanceID 
			 from 
				MetricThresholdInstances 
			 where 
				InstanceType = @ThresholdInstanceType and 
				ThresholdInstanceName = @ThresholdInstanceName
			)
		)
		begin
			select 
				@InstanceID = InstanceID 
			from 
				MetricThresholdInstances 
			where 
				InstanceType = @ThresholdInstanceType and 
				ThresholdInstanceName = @ThresholdInstanceName
				
			select @err = @@ERROR
		END
	else
	begin
		select @err = 666
		RAISERROR('An instanceID for the specified Threshold Instance does not exist', 10, 1, @err)
		return (@err)
	end

	-- determine if we are deleting an Template Threshold or a Server Threshold
	if (@SQLServerID IS NULL) 
	BEGIN
		IF (@UserViewID IS NULL)
		BEGIN
			RAISERROR('@SQLServerID or @UserViewID is required.', 10, 1, -1)
			RETURN -1
		END    
		ELSE
		BEGIN
			set @err = 0
			
			delete from DefaultMetricThresholds 
				where 
					UserViewID = @UserViewID and 
					Metric = @Metric and 
					ThresholdInstanceID = @InstanceID

			select @err = @@ERROR
			
			IF @err <> 0 BEGIN
				RAISERROR('An error occurred while deleting a threshold instance from an Alert Template', 10, 1, @err)
				RETURN(@err)
			END
			
			RETURN (0)
		end
	end
	else
	begin
		IF (@UserViewID IS NOT NULL)
		BEGIN
			RAISERROR('@SQLServerID and @UserViewID are mutually exclusive.', 10, 1, -1)
			RETURN -1
		END
		ELSE
		BEGIN
			DELETE FROM MetricThresholds 
				where
					[SQLServerID] = @SQLServerID AND 
					[Metric] = @Metric AND 
					[ThresholdInstanceID] = @InstanceID

			select @err = @@ERROR
			
			IF @err <> 0 BEGIN
				RAISERROR('An error occurred while deleting a threshold instance from an Alert Template', 10, 1, @err)
				RETURN(@err)
			END
			
			RETURN (0)
		end
	end
END					