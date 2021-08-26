if (object_id('p_AddAlertTemplate') is not null)
begin
drop procedure p_AddAlertTemplate
end
go

CREATE PROCEDURE [dbo].p_AddAlertTemplate(
	@SourceType int,
	@SourceID int,
	@Name nvarchar(256), 
	@Description nvarchar(1024),
--	@Default bit, 
	@templateID int output
)
AS
begin
	DECLARE @ID int
	declare @e int

	INSERT INTO [AlertTemplateLookup] values (@Name, @Description, 0)
	SET @e = @@error
	SELECT @ID = SCOPE_IDENTITY()
	
	IF (@e <> 0)
	BEGIN
		return @e
	END
	
	IF (@SourceType = 0)
	begin
		INSERT INTO [DefaultMetricThresholds] (
			[UserViewID],
			[Metric],
			[Enabled],
			[InfoThreshold],
			[WarningThreshold],
			[CriticalThreshold],
			[Data],
			[ThresholdInstanceID],
			[ThresholdEnabled],
			[IsBaselineEnabled],
			[BaselineCriticalThreshold]
				,[BaselineInfoThreshold],
				 [BaselineWarningThreshold]) 
			select 
				@ID,
				[Metric],
				[Enabled],
				[InfoThreshold],
				[WarningThreshold],
				[CriticalThreshold],
				[Data],
				[ThresholdInstanceID],
				[ThresholdEnabled],
				[IsBaselineEnabled],
				[BaselineCriticalThreshold]
				,[BaselineInfoThreshold],
				 [BaselineWarningThreshold]
				FROM 
					[DefaultMetricThresholds] 
				where 
					[UserViewID] = @SourceID
		
		SET @e = @@error	
	END
	ELSE
	BEGIN
		INSERT INTO [DefaultMetricThresholds] (
			[UserViewID],
			[Metric],
			[Enabled],
			[InfoThreshold],
			[WarningThreshold],
			[CriticalThreshold],
			[Data],
			[ThresholdInstanceID],
			[ThresholdEnabled],
			[IsBaselineEnabled],
			[BaselineCriticalThreshold]
				,[BaselineInfoThreshold],
				 [BaselineWarningThreshold]) 
			select 
				@ID,
				[Metric],
				[Enabled],
				[InfoThreshold],
				[WarningThreshold],
				[CriticalThreshold],
				[Data],
				[ThresholdInstanceID],
				[ThresholdEnabled],
				[IsBaselineEnabled],
				 [BaselineCriticalThreshold]
				,[BaselineInfoThreshold],
				 [BaselineWarningThreshold]
				FROM 
					[MetricThresholds]
				where 
					[SQLServerID] = @SourceID
		SET @e = @@error	
	END
	
	if (@e <> 0)
	BEGIN
		DELETE FROM [AlertTemplateLookup] WHERE [TemplateID] = @ID
	    return @e
	END
		
	SELECT @templateID = @ID 
	
	RETURN @e
END	

