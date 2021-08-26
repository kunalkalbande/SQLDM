if (object_id('p_UpdateCounter') is not null)
begin
drop procedure p_UpdateCounter
end
go

CREATE PROCEDURE [dbo].p_UpdateCounter(
	@Metric int,
	@Name nvarchar(128),
	@Category nvarchar(64),	
	@Description nvarchar(512),
	@Class int, 
	@Flags int,
	@Scale float,
	@MinValue int,
	@MaxValue bigint,
	@DefaultInfoValue bigint,
	@DefaultWarningValue bigint,
	@DefaultCriticalValue bigint,
	@DoNotifications bit,
	@EventCategory int,
	@DefaultMessageID int,
	@AlertEnabledDefault bit, 
	@ValueComparison int,	
	@ValueType nvarchar(128),
	@Rank int,
	@MetricType int,
	@CalculationType int,
	@CounterEnabled bit,
	@ObjectName	nvarchar(256),
	@CounterName nvarchar(256),
	@InstanceName nvarchar(256),
	@Batch	nvarchar(max),
    @serverType nvarchar(256),
    @profileId bigint
)
AS
begin
	declare @e int
	declare @result int
	declare @DisableInfoThreshold bit
	declare @infoThresholdXML xml
	DECLARE @sqlserverID INT

	set @DisableInfoThreshold = 0
	
	if (@DefaultInfoValue < @MinValue) 
	begin
		set @DisableInfoThreshold = 1
	end
	  IF (@MetricType = 4)    
  begin
 UPDATE [CustomCounterDefinition] SET  
  [MetricType] = @MetricType,  
  [CalculationType] = @CalculationType,  
  [Scale] = @Scale,  
  [Object] = @ObjectName,  
  [Counter] = @CounterName,  
  [Instance] = @InstanceName,  
  [Batch] = @Batch,
  [ServerType] = @serverType,
  [AzureProfileId] = @profileId
 WHERE [Metric] = @Metric 
	-- Update CustomCounterMap with instance
	DELETE FROM 
		[CustomCounterMap]
	WHERE 
		[Metric] = @Metric
	-- Get Sql Server Id
	SET @sqlserverID = 
		(	SELECT
				[SQLServerID]
			FROM 
				[MonitoredSQLServers] 
			WHERE [InstanceName] = @InstanceName )
	-- Update Custom Counter Map
	IF @sqlserverID IS NOT NULL
	INSERT INTO 
		CustomCounterMap
		(	[Metric],
			[SQLServerID] )
	VALUES 
		(	@Metric,
			@sqlserverID )
 end
 else 
 begin

	UPDATE [CustomCounterDefinition] SET
		[MetricType] = @MetricType,
		[CalculationType] = @CalculationType,
		[Scale] = @Scale,
		[Object] = @ObjectName,
		[Counter] = @CounterName,
		[Instance] = @InstanceName,
		[Batch] = @Batch
	WHERE [Metric] = @Metric
	end

	SET @result = @@error

	UPDATE [MetricMetaData] SET
		[Class] = @Class,
		[Flags] = @Flags,
		[MinValue] = @MinValue,
		[MaxValue] = @MaxValue,
		[DefaultWarningValue] = @DefaultWarningValue,
		[DefaultCriticalValue] = @DefaultCriticalValue,
		[DefaultInfoValue] = case when @DefaultInfoValue < @MinValue then [DefaultInfoValue] else @DefaultInfoValue end,
		[DoNotifications] = @DoNotifications,
		[EventCategory] = @EventCategory,
		[DefaultMessageID] = @DefaultMessageID,
		[AlertEnabledDefault] = @AlertEnabledDefault,
		[ValueComparison] = @ValueComparison,
		[ValueType] = @ValueType,
		[Rank] = @Rank
	WHERE [Metric] = @Metric
	
	SET @e = @@error
	IF (@e <> 0 and @result = 0)
		SET @result = @e

	UPDATE [MetricInfo] SET
		[Rank] = @Rank,
		[Category] = @Category,
		[Name] = @Name,
		[Description] = @Description
	WHERE [Metric] = @Metric
	SET @e = @@error
	IF (@e <> 0 and @result = 0)
		SET @result = @e

	SELECT @infoThresholdXML = CAST(b.InfoThreshold as XML) from AlertTemplateLookup a left join DefaultMetricThresholds b on a.TemplateID = b.UserViewID where a.[Default] = 1 and b.Metric = @Metric
	
	if (@@ROWCOUNT > 0)
	BEGIN
		if (@DisableInfoThreshold = 1)
		begin
			set @infoThresholdXML.modify('replace value of (/Threshold/@Enabled)[1] with "false"')
		end
		else begin
			set @infoThresholdXML.modify('replace value of (/Threshold/@Enabled)[1] with "true"')
		end
		
		update DefaultMetricThresholds set InfoThreshold = CONVERT(nvarchar(2048), @infoThresholdXML) from AlertTemplateLookup a left join DefaultMetricThresholds b on a.TemplateID = b.UserViewID where a.[Default] = 1 and b.Metric = @Metric
	END
	RETURN @result
END	

