if (object_id('p_AddMetricThresholdInstance') is not null)
begin
drop procedure [p_AddMetricThresholdInstance]
end
go

create procedure [p_AddMetricThresholdInstance]
	@UserViewID int,
	@SQLServerID int,
	@Metric int,
	@Enabled bit,
	@WarningThreshold nvarchar(2048),
	@CriticalThreshold nvarchar(2048),
	@Data nvarchar(max),
	@InfoThreshold nvarchar(2048),
	@ThresholdInstanceType int,
	@ThresholdInstanceName nvarchar(256),
	@ThresholdEnabled bit,
	@IsBaselineEnabled bit,
	@BaselineWarningThreshold nvarchar(2048),
	@BaselineCriticalThreshold nvarchar(2048),
	@BaselineInfoThreshold nvarchar(2048)
as
begin
	DECLARE @ThresholdInstanceID int
	DECLARE @err int
	set @err = 0

	exec p_AddThresholdInstance 
		@ThresholdInstanceType, 
		@ThresholdInstanceName, 
		@ThresholdInstanceID output 
	
	select @err = @@ERROR
	
	if (@err > 0)
	begin
		RAISERROR('Error generating instance ID in AddMetricThresholdInstance',10,1,-1)
		return (-1)
	end
	
	
	if (@SQLServerID is NULL)
	begin
		if (@UserViewID is NULL)
		begin
			RAISERROR('@SQLServerID or @UserViewID is required.', 10, 1, -1)
			RETURN -1
		END    
		ELSE
		BEGIN
			set @err = 0
			
			if (exists (select InfoThreshold from DefaultMetricThresholds 
							where UserViewID = @UserViewID and 
								  Metric = @Metric and 
								  ThresholdInstanceID = @ThresholdInstanceID))
			BEGIN
				RAISERROR('Threshold Instance already exists',10,1,-1)
				return -1
			END
			ELSE
			BEGIN
				insert into DefaultMetricThresholds (
					UserViewID,
					Metric,
					Enabled,
					InfoThreshold,
					WarningThreshold,
					CriticalThreshold,
					Data,
					ThresholdInstanceID,
					ThresholdEnabled,
					[IsBaselineEnabled],					
					[BaselineWarningThreshold],
					[BaselineCriticalThreshold],
					[BaselineInfoThreshold] )
				values (
					@UserViewID,
					@Metric,
					@Enabled,
					@InfoThreshold,
					@WarningThreshold,
					@CriticalThreshold,
					@Data,
					@ThresholdInstanceID,
					@ThresholdEnabled,
					@IsBaselineEnabled,
					@BaselineWarningThreshold ,
					@BaselineCriticalThreshold ,
					@BaselineInfoThreshold)
					
				select @err = @@ERROR
				IF @err <> 0 
				BEGIN
					RAISERROR('An error occurred while adding a new DefaultMetricThreshold instance', 10, 1, @err)
					RETURN(@err)
				END
			END
		END
	END
	ELSE
	BEGIN
		IF (@UserViewID IS NOT NULL)
		BEGIN
			RAISERROR('@SQLServerID and @UserViewID are mutually exclusive.', 10, 1, -1)
			RETURN -1
		END
		ELSE
		BEGIN
			set @err = 0
			
			if (exists (select InfoThreshold from MetricThresholds 
							where SQLServerID = @SQLServerID and 
								  Metric = @Metric and 
								  ThresholdInstanceID = @ThresholdInstanceID))
			BEGIN
				RAISERROR('Threshold Instance already exists',10,1,-1)
				return -1
			END
			ELSE
			BEGIN
				insert into MetricThresholds (
					SQLServerID, 
					Metric, 
					Enabled, 
					InfoThreshold, 
					WarningThreshold, 
					CriticalThreshold, 
					Data, 
					ThresholdInstanceID,
					ThresholdEnabled,
					IsBaselineEnabled,
					[BaselineWarningThreshold],
					[BaselineCriticalThreshold],
					[BaselineInfoThreshold] 
				)
					values(
					@SQLServerID, 
					@Metric, 
					@Enabled, 
					@InfoThreshold, 
					@WarningThreshold, 
					@CriticalThreshold, 
					@Data, 
					@ThresholdInstanceID,
					@ThresholdEnabled,
					@IsBaselineEnabled,
					@BaselineWarningThreshold,
					@BaselineCriticalThreshold,
					@BaselineInfoThreshold
				)
					
				select @err = @@ERROR
				
				IF @err <> 0 BEGIN
					RAISERROR('An error occurred while inserting a threshold instance.', 10, 1, @err)
					RETURN(@err)
				END
			
				RETURN (0)
			end
		END
	end
end

