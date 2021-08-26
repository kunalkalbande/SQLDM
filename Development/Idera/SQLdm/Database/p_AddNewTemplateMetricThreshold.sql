IF (object_id('p_AddNewTemplateMetricThreshold') is not null)
BEGIN
DROP PROCEDURE p_AddNewTemplateMetricThreshold
END
GO

CREATE PROCEDURE [dbo].[p_AddNewTemplateMetricThreshold]
@TemplateName VARCHAR(MAX) = NULL,
@IsCloudTemplate INT = 0
AS
BEGIN

	BEGIN TRY
	DECLARE @TemplateId INT = NULL
		IF NOT EXISTS( Select 1 FROM dbo.AlertTemplateLookup WHERE Name=@TemplateName ) -- Condition to check Whether Name Exist or NOT
	BEGIN	
		--SET @TemplateId = ( SELECT MAX(TemplateID) + 1 FROM dbo.AlertTemplateLookup)
			INSERT INTO dbo.AlertTemplateLookup(Name,Description,[Default])
			VALUES( @TemplateName,'SQLdm '+ @TemplateName +' created by Management Services',0)
            
			SELECT @TemplateId= MAX(TemplateID)  FROM dbo.AlertTemplateLookup			
	END

 --ELSE
	-- BEGIN
	--	   SELECT @TemplateId = TemplateID  FROM dbo.AlertTemplateLookup WHERE Name=@TemplateName
	-- END
  
   	IF(@TemplateId IS NOT NULL)
	BEGIN
     CREATE TABLE #MetricMetaData (      
		ID INT IDENTITY(1,1), 
		UserViewID int	,
		Metric int,
		AlertEnabledDefault bit,		
		DefaultWarningValue bigint,
		DefaultCriticalValue bigint,
		DefaultInfoValue bigint,
		BaselineDefaultWarningValue bigint,
		BaselineDefaultCriticalValue bigint, 
		BaselineDefaultInfoValue bigint
		);

            --Loop through for dbo.AlertTemplateLookup

			INSERT INTO #MetricMetaData  
			SELECT @TemplateId,
					MMD.Metric, 
					(CASE   
							WHEN	 @IsCloudTemplate = 2 AND MMD.Metric IN (138, 139, 140, 141, 142, 143, 144, 145) THEN 0
							WHEN	 @IsCloudTemplate = 1 AND MMD.Metric IN (131, 132, 133, 134, 135, 136, 137,142, 175,176,177,178,179,180) THEN 0
							--WHEN	 @IsCloudTemplate = 2 AND MMD.Metric NOT IN (131, 132, 133, 134, 135, 136, 137) THEN MI.PaaS
							--WHEN	 @IsCloudTemplate = 1 AND MMD.Metric IN (138, 139, 140, 141, 142, 143, 144, 145) THEN 1
							--WHEN	 @IsCloudTemplate = 1 AND MMD.Metric NOT IN (138, 139, 140, 141, 142, 143, 144, 145) THEN MI.PaaS
							ELSE MMD.AlertEnabledDefault 
						  END) AS 'AlertEnabledDefault',				    
					MMD.DefaultWarningValue,
					MMD.DefaultCriticalValue,
					MMD.DefaultInfoValue,
					MMD.BaselineDefaultWarningValue,
					MMD.BaselineDefaultCriticalValue, 
					MMD.BaselineDefaultInfoValue 
			FROM dbo.MetricMetaData MMD
			JOIN dbo.MetricInfo MI ON MMD.Metric = MI.Metric

			DECLARE @Index INT = 1
			DECLARE @TotalRows INT
			SELECT @TotalRows = COUNT(1) FROM #MetricMetaData

			DECLARE @UserViewID int
			DECLARE @MetricID int
			DECLARE @AlertEnabledDefault bit

			DECLARE @WarningThresholdXML nvarchar(256)
			DECLARE @CriticalThresholdXML nvarchar(256)
			DECLARE @InfoThresholdXML nvarchar(256)
			DECLARE @ComparisonText nvarchar(16)
			DECLARE @DefaultWarningValue bigint
			DECLARE @DefaultCriticalValue bigint
			DECLARE @DefaultInfoValue bigint

			WHILE @Index <= @TotalRows
	BEGIN

	 SELECT 
		@UserViewID = UserViewID, 
		@MetricID = [Metric], 
		@AlertEnabledDefault = [AlertEnabledDefault],  		
		@DefaultWarningValue = [DefaultWarningValue],
		@DefaultCriticalValue = [DefaultCriticalValue],		
		@DefaultInfoValue = [DefaultInfoValue]
	 FROM #MetricMetaData WHERE ID = @Index  
	
-- insert default alert configuration row
	set @WarningThresholdXML = 
	'<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Op="' 
	set @CriticalThresholdXML = @WarningThresholdXML
	set @InfoThresholdXML = @WarningThresholdXML

	set @ComparisonText = 'GE'
			
	set @WarningThresholdXML = @WarningThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
	set @CriticalThresholdXML = @CriticalThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
		
	IF (@MetricID > 145 AND @MetricID <=174)
	BEGIN
		SET @InfoThresholdXML = @InfoThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
	END
	ELSE
	BEGIN
		SET @InfoThresholdXML = @InfoThresholdXML + @ComparisonText + '" Enabled="false"><Value xsi:type="xsd:float">'
	END

	SET @WarningThresholdXML = @WarningThresholdXML + CAST(@DefaultWarningValue AS nvarchar(32)) + '</Value></Threshold>'
	SET @CriticalThresholdXML = @CriticalThresholdXML + CAST(@DefaultCriticalValue AS nvarchar(32)) + '</Value></Threshold>'
	SET @InfoThresholdXML = @InfoThresholdXML + CAST(@DefaultInfoValue AS nvarchar(32)) + '</Value></Threshold>'

	DECLARE @BaselineWarningThreshold nvarchar(2048)
	DECLARE @BaselineCriticalThreshold nvarchar(2048)
	DECLARE @BaselineInfoThreshold nvarchar(2048)
	DECLARE @xmlValueForBaselineToCastWarning xml
	DECLARE @xmlValueForBaselineToCastCritical xml
	DECLARE @xmlValueForBaselineToCastInfo xml
	DECLARE @WarningThresholdXML10 nvarchar(512),
				@CriticalThresholdXML10 nvarchar(512),
				@InfoThresholdXML10 nvarchar(512),
				@BaselineDefaultWarningValue10 nvarchar(256),
				@BaselineDefaultCriticalValue10 nvarchar(256),
				@BaselineDefaultInfoValue10 nvarchar(256),
				@xmSignature nvarchar(256)

				select @xmlValueForBaselineToCastWarning = CAST(@WarningThresholdXML as XML) ,
				 @xmlValueForBaselineToCastCritical = CAST(@CriticalThresholdXML as XML) ,
				@xmlValueForBaselineToCastInfo = CAST(@InfoThresholdXML as XML)
				-- append value			
				select @BaselineDefaultWarningValue10 = [BaselineDefaultWarningValue],
						@BaselineDefaultCriticalValue10 = [BaselineDefaultCriticalValue],
						@BaselineDefaultInfoValue10 = [BaselineDefaultInfoValue]
				from [dbo].[MetricMetaData] 
				where Metric = @MetricID
				
			--ANSI_WARNINGS is not honored when passing parameters in a stored procedure , so setting it on	
				SET @WarningThresholdXML10 = CONVERT( nvarchar(512), @xmlValueForBaselineToCastWarning.query('data(//@Op)'))
				SET @CriticalThresholdXML10 = CONVERT(nvarchar(512), @xmlValueForBaselineToCastCritical.query('data(//@Op)'))
				SET @InfoThresholdXML10 = CONVERT(nvarchar(512), @xmlValueForBaselineToCastInfo.query('data(//@Op)'))
	
				-- START --Srishti Purohit -- To accomadate baseline alert feature
				
				SET @xmSignature ='<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Op="'

				SET @xmSignature = @xmSignature 
			
				set	@BaselineWarningThreshold = @xmSignature + @WarningThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' + @BaselineDefaultWarningValue10 + '</Value></Threshold>'
				set	@BaselineCriticalThreshold = @xmSignature + @CriticalThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' +  @BaselineDefaultCriticalValue10 + '</Value></Threshold>'
				set	@BaselineInfoThreshold = @xmSignature + @InfoThresholdXML10 + '" Enabled="true"><Value xsi:type="xsd:float">' +  @BaselineDefaultInfoValue10 + '</Value></Threshold>'
				-- END --Srishti Purohit -- To accomadate baseline alert feature
		--END Baseline support
				INSERT	into DefaultMetricThresholds (UserViewID, Metric, Enabled, WarningThreshold, CriticalThreshold, Data, InfoThreshold, IsBaselineEnabled, [BaselineWarningThreshold] ,[BaselineCriticalThreshold], [BaselineInfoThreshold]) 
				VALUES (@UserViewID, @MetricID, @AlertEnabledDefault, @WarningThresholdXML, @CriticalThresholdXML, NULL, @InfoThresholdXML, 0, @BaselineWarningThreshold ,@BaselineCriticalThreshold ,@BaselineInfoThreshold)


	 --EXEC p_AddDefaultMetricThreshold 
		--		@UserViewID, 
		--		@MetricID, 
		--		@AlertEnabledDefault, 
		--		@WarningThresholdXML, 
		--		@CriticalThresholdXML, 
		--		null,
		--		@InfoThresholdXML




	 SET @Index = @Index + 1
	END


			DROP TABLE #MetricMetaData
END
	END TRY

	BEGIN CATCH
		SELECT  ERROR_MESSAGE()
	END CATCH
END  



GO

 




 




