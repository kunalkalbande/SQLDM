if (object_id('p_AddCounter') is not null)
begin
drop procedure p_AddCounter
end
go

CREATE PROCEDURE [dbo].p_AddCounter(
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
	@ServerType nvarchar(256),
	@profileId int,
	@AlertOnCollectionFailure bit,
	@ReturnMetricID int output
)
AS
begin
	declare @e int
	declare @MetricID int
	declare @InfoThresholdXML nvarchar(256)
	declare @WarningThresholdXML nvarchar(256)
	declare @CriticalThresholdXML nvarchar(256)
	declare @ComparisonText nvarchar(16)
	declare @Data nvarchar(256)
	declare @DisableInfoThreshold bit
	
	declare @CurrTemplateID int
	declare @sqlserverID int
	
	set @DisableInfoThreshold = 0
	
	if (@DefaultInfoValue < @MinValue) 
	begin
		set @DisableInfoThreshold = 1
		
		if (@ValueComparison = 0)
		begin
			set @DefaultInfoValue = @DefaultWarningValue - (@DefaultCriticalValue - @DefaultWarningValue)
			if (@DefaultInfoValue < @MinValue)
				set @DefaultInfoValue = @MinValue
		end
		else begin
			set @DefaultInfoValue = @DefaultWarningValue + (@DefaultWarningValue - @DefaultCriticalValue)
			if (@DefaultInfoValue > @MaxValue)
				set @DefaultInfoValue = @MaxValue
		end
	end
	IF (@MetricType = 4)
	begin
	INSERT INTO CustomCounterDefinition (
		[MetricType],
		[CalculationType],
		[Enabled],
		[Scale],
		[Object],
		[Counter],
		[Instance],
		[Batch],
		[ServerType],
		[AzureProfileId]
	) VALUES (
		@MetricType,
		@CalculationType,
		@CounterEnabled,
		@Scale,
		@ObjectName,
		@CounterName,
		@InstanceName,
		@Batch,
		@ServerType,
		@profileId
	)
	end
	else		
		begin
		INSERT INTO CustomCounterDefinition (
		[MetricType],
		[CalculationType],
		[Enabled],
		[Scale],
		[Object],
		[Counter],
		[Instance],
		[Batch],
		[ServerType]
	) VALUES (
		@MetricType,
		@CalculationType,
		1,
		@Scale,
		@ObjectName,
		@CounterName,
		@InstanceName,
		@Batch,
		@ServerType
	)
		end
	SET @e = @@error
	IF (@e = 0)
	begin
		SELECT @MetricID = SCOPE_IDENTITY()
		
		INSERT INTO MetricMetaData(
			[Metric],
			[Class],
			[Flags],
			[MinValue],
			[MaxValue],
			[DefaultWarningValue],
			[DefaultCriticalValue],
			[DoNotifications],
			[EventCategory],
			[DefaultMessageID],
			[AlertEnabledDefault],
			[ValueComparison],
			[ValueType],
			[Rank],
			[DefaultInfoValue],
			[TableName],
			[ColumnName],
			[BaselineMaxValue], --10.0 SQLdm srishti purohit adding baseline default meta data
			[BaselineDefaultWarningValue],
			[BaselineDefaultCriticalValue],
			[BaselineDefaultInfoValue]
		) VALUES (
			@MetricID,
			@Class,
			@Flags,
			@MinValue,
			@MaxValue,
			@DefaultWarningValue,
			@DefaultCriticalValue,
			@DoNotifications,
			@EventCategory,
			@DefaultMessageID,
			@AlertEnabledDefault,
			@ValueComparison,
			@ValueType,
			@Rank,
			@DefaultInfoValue,
			'CustomCounterStatistics',
			'RawValue',
			300,
			100,
			case when @ValueComparison = 0 then 120 else 50 end,
			case when @ValueComparison = 0 then 50 else 120 end
		)	
		SET @e = @@error
		IF (@e = 0)
		begin
			INSERT INTO MetricInfo (
				[Metric],
				[Rank],
				[Category],
				[Name],
				[Description]
			) VALUES (
				@MetricID,
				@Rank,
				@Category,
				@Name,
				@Description
			)

			SET @e = @@error
		end
		IF (@e = 0)
		begin
		IF (@MetricType = 4)
		begin
		set @sqlserverID = (select SQLServerID from MonitoredSQLServers where InstanceName = @InstanceName )
		
		IF @sqlserverID IS NOT NULL
		BEGIN
			INSERT INTO CustomCounterMap (
				[Metric],
				[SQLServerID]
			) VALUES (
				@MetricID,
				@sqlserverID
			)
		END

			SET @e = @@error
		end
		end
	end
	if (@e = 0)
	begin
		-- insert default alert configuration row
		set @WarningThresholdXML = 
			'<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Op="' 
		set @CriticalThresholdXML = @WarningThresholdXML
		set @InfoThresholdXML = @WarningThresholdXML
		
	    -- append comparison type
		if (@ValueComparison = 0)
			set @ComparisonText = 'GE'
		else
			set @ComparisonText = 'LE'
			

		set @InfoThresholdXML = @InfoThresholdXML + @ComparisonText + '" Enabled="'+ case when @DisableInfoThreshold = 1 then 'false' else 'true' end + '"><Value xsi:type="xsd:long">'
		set @WarningThresholdXML = @WarningThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:long">'
		set @CriticalThresholdXML = @CriticalThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:long">'

		-- append value

		set @InfoThresholdXML = @InfoThresholdXML + CAST(@DefaultInfoValue AS nvarchar(32)) + '</Value></Threshold>'
		set @WarningThresholdXML = @WarningThresholdXML + CAST(@DefaultWarningValue AS nvarchar(32)) + '</Value></Threshold>'
		set @CriticalThresholdXML = @CriticalThresholdXML + CAST(@DefaultCriticalValue AS nvarchar(32)) + '</Value></Threshold>'

			
		if (@AlertOnCollectionFailure = 1 AND @MetricType <> 4)
			set @Data = '<?xml version="1.0" encoding="utf-16"?><ObjectWrapper xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><Value xsi:type="xsd:boolean">true</Value></ObjectWrapper>'

		set @CurrTemplateID = -1
		
		select @CurrTemplateID = MIN(TemplateID) from AlertTemplateLookup where TemplateID > @CurrTemplateID
		
		while (@CurrTemplateID is not null)
		begin
		
			exec p_AddDefaultMetricThreshold 
				@CurrTemplateID, 
				@MetricID, 
				@AlertEnabledDefault, 
				@WarningThresholdXML, 
				@CriticalThresholdXML, 
				@Data,
				@InfoThresholdXML
				
			select @CurrTemplateID = MIN(TemplateID) from AlertTemplateLookup where TemplateID > @CurrTemplateID
			
		end
	end

	if (@e = 0) 
	begin
		SET @ReturnMetricID = @MetricID 
	end

	RETURN @e
END	
 

