if (object_id('p_UpdateMetricThreshold') is not null)
begin
drop procedure [p_UpdateMetricThreshold]
end
go

create procedure [p_UpdateMetricThreshold] (
	@UserViewID int,
	@SQLServerID int,
	@Metric int,
	@Enabled bit,
	@WarningThreshold nvarchar(2048),
	@CriticalThreshold nvarchar(2048),
	@Data nvarchar(max),
	@UTCSnoozeStart datetime,
	@UTCSnoozeEnd datetime,
	@SnoozeStartUser nvarchar(255),
	@SnoozeEndUser nvarchar(255),
	@InfoThreshold nvarchar(2048),
	@ThresholdInstance nvarchar(256),
	@ThresholdInstanceType int,
	@ThresholdEnabled bit,
	@IsBaselineEnabled bit,
	@BaselineWarningThresholdXML nvarchar(256),
	@BaselineCriticalThresholdXML nvarchar(256),
	@BaselineInfoThresholdXML nvarchar(256)
)
as
begin
	DECLARE @err int 
	DECLARE @InstanceName nvarchar(256)
	DECLARE @ActiveSnapshotDateTime datetime
	DECLARE @OldWarningThreshold nvarchar(2048)
	DECLARE @OldOp nvarchar(32)
	DECLARE @NewOp nvarchar(32)
	DECLARE @ThresholdInstanceID int

	DECLARE @OldCriticalThreshold nvarchar(2048)
	DECLARE @OldInfoThreshold nvarchar(2048)
	DECLARE @UpdateThresholdValue bit
	SET @UpdateThresholdValue = 0

	set @err = 0
	set @ThresholdInstanceID = -2
	
	-- Get/Create the Threshold Instance ID for this Threshold Instance
	EXEC p_AddThresholdInstance @ThresholdInstanceType, @ThresholdInstance, @ThresholdInstanceID output
	if (@ThresholdInstanceID = -2)
	BEGIN
		RAISERROR('Using invalid Threshold Instance ID', 10, 1, -1)
		return -1
	END
		
	if (@SQLServerID IS NULL) 
	BEGIN
		IF (@UserViewID IS NULL)
		BEGIN
			RAISERROR('@SQLServerID or @UserViewID is required.', 10, 1, -1)
			RETURN -1
		END    
		ELSE
		BEGIN
			SELECT @OldInfoThreshold = [InfoThreshold]
						FROM [DefaultMetricThresholds]
						WHERE ([UserViewID] = @UserViewID) AND 
						  ([Metric] = @Metric) and
						  ([ThresholdInstanceID] = @ThresholdInstanceID)

			SELECT @OldWarningThreshold = [WarningThreshold]
				FROM [DefaultMetricThresholds]
				WHERE ([UserViewID] = @UserViewID) AND 
					  ([Metric] = @Metric) and
					  ([ThresholdInstanceID] = @ThresholdInstanceID)
			
			SELECT @OldCriticalThreshold = [CriticalThreshold]
					FROM [DefaultMetricThresholds]
					WHERE ([UserViewID] = @UserViewID) AND 
						  ([Metric] = @Metric) and
						  ([ThresholdInstanceID] = @ThresholdInstanceID)
			
			IF (@@ROWCOUNT > 0)
			BEGIN
				UPDATE[DefaultMetricThresholds] 
					SET [Enabled] = @Enabled,
						[InfoThreshold] = @InfoThreshold,
						[WarningThreshold] = @WarningThreshold, 
						[CriticalThreshold] = @CriticalThreshold,
						[Data] = @Data,
						[ThresholdEnabled] = @ThresholdEnabled,
						[IsBaselineEnabled] = @IsBaselineEnabled,
						[BaselineWarningThreshold] = @BaselineWarningThresholdXML,
						[BaselineCriticalThreshold] = @BaselineCriticalThresholdXML,
						[BaselineInfoThreshold] = @BaselineInfoThresholdXML
				WHERE ([UserViewID] = @UserViewID) AND ([Metric] = @Metric) AND ([ThresholdInstanceID] = @ThresholdInstanceID)

				-- SQLDM-26424 - Changed the approach to compare the new threshold values with old threshold values.
				-- Update the MetricThresholds table if any of the threshold values are changed.

				DECLARE @startNew INT
				DECLARE @endNew INT
				DECLARE @startOld INT
				DECLARE @endOld INT
				DECLARE @FirstPatn CHAR(27) 
				DECLARE @SecondPatn CHAR(8)
 
				DECLARE @substrIndex INT
				DECLARE @substrLength INT
				DECLARE @strLength INT
				
				SET @FirstPatn = '<Value xsi:type="xsd:long">';
				SET @SecondPatn = '</Value>';
 
				SET @startNew = PATINDEX('%' + @FirstPatn + '%', @InfoThreshold);
				SET @endNew = PATINDEX('%' + @SecondPatn + '%', @InfoThreshold);
				SET @startOld = PATINDEX('%' + @FirstPatn + '%', @OldInfoThreshold);
				SET @endOld = PATINDEX('%' + @SecondPatn + '%', @OldInfoThreshold);
				
				-- Set for @NewOp
				set @substrIndex = @startNew + LEN(@FirstPatn)
				set @substrLength = ( @endNew - @startNew ) - LEN(@FirstPatn)

				IF @substrLength > 0 and @substrIndex >= 0 
				BEGIN
					SET @NewOp = SUBSTRING(@InfoThreshold, @substrIndex, @substrLength)
				END
				ELSE
				BEGIN
					SET @NewOp = ''
				END
				
				-- Set for @OldOp
				set @substrIndex = @startOld + LEN(@FirstPatn)
				set @substrLength = ( @endOld - @startOld ) - LEN(@FirstPatn)
				
				IF @substrLength > 0 and @substrIndex >= 0 
				BEGIN
				    SET @OldOp = SUBSTRING(@OldInfoThreshold, @substrIndex, @substrLength)
				END
				ELSE
				BEGIN
					SET @OldOp = ''
				END
				
				-- Compare the informational threshold values and if they are same check for warning threshold values
				IF (@OldOp = @NewOp)
				BEGIN
					SET @startNew = PATINDEX('%' + @FirstPatn + '%', @WarningThreshold);
					SET @endNew = PATINDEX('%' + @SecondPatn + '%', @WarningThreshold);
					SET @startOld = PATINDEX('%' + @FirstPatn + '%', @OldWarningThreshold);
					SET @endOld = PATINDEX('%' + @SecondPatn + '%', @OldWarningThreshold);

					-- Set for @NewOp
					set @substrIndex = @startNew + LEN(@FirstPatn)
					set @substrLength = ( @endNew - @startNew ) - LEN(@FirstPatn)

					IF @substrLength > 0 and @substrIndex >= 0 
					BEGIN
						SET @NewOp = SUBSTRING(@WarningThreshold, @substrIndex, @substrLength)
					END
					ELSE
					BEGIN
						SET @NewOp = ''
					END
				
					-- Set for @OldOp
					set @substrIndex = @startOld + LEN(@FirstPatn)
					set @substrLength = ( @endOld - @startOld ) - LEN(@FirstPatn)
					
					IF @substrLength > 0 and @substrIndex >= 0 
					BEGIN
						SET @OldOp = SUBSTRING(@OldWarningThreshold, @substrIndex, @substrLength)
					END
					ELSE
					BEGIN
						SET @OldOp = ''
					END
				
					-- Compare the warning threshold values and if they are same check for critical threshold values
					IF (@OldOp = @NewOp)
					BEGIN
						SET @startNew = PATINDEX('%' + @FirstPatn + '%', @CriticalThreshold);
						SET @endNew = PATINDEX('%' + @SecondPatn + '%', @CriticalThreshold);
						SET @startOld = PATINDEX('%' + @FirstPatn + '%', @OldCriticalThreshold);
						SET @endOld = PATINDEX('%' + @SecondPatn + '%', @OldCriticalThreshold);
						
						-- Set for @NewOp
						set @substrIndex = @startNew + LEN(@FirstPatn)
						set @substrLength = ( @endNew - @startNew ) - LEN(@FirstPatn)

						IF @substrLength > 0 and @substrIndex >= 0 
						BEGIN
							SET @NewOp = SUBSTRING(@CriticalThreshold, @substrIndex, @substrLength)
						END
						ELSE
						BEGIN
							SET @NewOp = ''
						END

						-- Set for @OldOp
						set @substrIndex = @startOld + LEN(@FirstPatn)
						set @substrLength = ( @endOld - @startOld ) - LEN(@FirstPatn)
						
						IF @substrLength > 0 and @substrIndex >= 0 
						BEGIN
							SET @OldOp = SUBSTRING(@OldCriticalThreshold, @substrIndex, @substrLength)
						END
						ELSE
						BEGIN
							SET @OldOp = ''
						END
						IF (@OldOp <> @NewOp) -- if the critical threshold values are not same, set the variable UpdateThresholdValue to 1
						BEGIN
							SET @UpdateThresholdValue = 1
						END
					END
					ELSE -- if the warning threshold values are not same, set the variable UpdateThresholdValue to 1
					BEGIN
						SET @UpdateThresholdValue = 1
					END
				END
				ELSE -- if the informational threshold values are not same, set the variable UpdateThresholdValue to 1
				BEGIN
					SET @UpdateThresholdValue = 1
				END
				-- if UpdateThresholdValue is > 0 then it indicates change in the threshold values so we need to update MetricThresholds table
				IF (@UpdateThresholdValue > 0)
				BEGIN
					UPDATE MetricThresholds SET
						[InfoThreshold] = @InfoThreshold,
						[WarningThreshold] = @WarningThreshold,
						[CriticalThreshold] = @CriticalThreshold,
						[IsBaselineEnabled] = @IsBaselineEnabled,
						[BaselineWarningThreshold] = @BaselineWarningThresholdXML,
						[BaselineCriticalThreshold] = @BaselineCriticalThresholdXML,
						[BaselineInfoThreshold] = @BaselineInfoThresholdXML
					WHERE Metric = @Metric
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
			IF (EXISTS (select 
							[WarningThreshold] 
						from 
							[MetricThresholds] 
						where 
							[SQLServerID] = @SQLServerID AND 
							[Metric] = @Metric AND 
							[ThresholdInstanceID] = @ThresholdInstanceID)
				)
			BEGIN
				UPDATE[MetricThresholds] 
					SET [Enabled] = @Enabled,
						[InfoThreshold] = @InfoThreshold,
						[WarningThreshold] = @WarningThreshold, 
						[CriticalThreshold] = @CriticalThreshold,
						[Data] = @Data,
						[UTCSnoozeStart] = COALESCE(@UTCSnoozeStart,[UTCSnoozeStart]),
						[UTCSnoozeEnd] = COALESCE(@UTCSnoozeEnd,[UTCSnoozeEnd]),
						[SnoozeStartUser] = COALESCE(@SnoozeStartUser,[SnoozeStartUser]),
						[SnoozeEndUser] = COALESCE(@SnoozeEndUser,[SnoozeEndUser]),
						[ThresholdEnabled] = @ThresholdEnabled,
						[IsBaselineEnabled] = @IsBaselineEnabled,
						[BaselineWarningThreshold] = @BaselineWarningThresholdXML,
						[BaselineCriticalThreshold] = @BaselineCriticalThresholdXML,
						[BaselineInfoThreshold] = @BaselineInfoThresholdXML
				WHERE ([SQLServerID] = @SQLServerID) AND ([Metric] = @Metric) AND ([ThresholdInstanceID] = @ThresholdInstanceID)

				IF (@Enabled = 0 AND @ThresholdInstanceID = -1)
				BEGIN
					SELECT @InstanceName = [InstanceName],
						@ActiveSnapshotDateTime = [LastAlertRefreshTime]
						from MonitoredSQLServers (nolock) 
						where [SQLServerID] = @SQLServerID

					-- clear the active flag from any alerts outstanding for this metric
					UPDATE [Alerts] SET [Active] = 0
						WHERE 
							[ServerName] = @InstanceName and
							[UTCOccurrenceDateTime] = @ActiveSnapshotDateTime and
							[Active] = 1 and
							[Metric] = @Metric
				END
			END
		END
	END

	SELECT @err = @@error

	RETURN @err
end

