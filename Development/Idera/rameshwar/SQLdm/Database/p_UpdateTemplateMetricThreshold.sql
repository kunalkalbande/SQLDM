IF (object_id('p_UpdateTemplateMetricThreshold') is not null)
BEGIN
DROP PROCEDURE p_UpdateTemplateMetricThreshold
END
GO

CREATE PROCEDURE [dbo].[p_UpdateTemplateMetricThreshold]
--DECLARE 
@TemplateName VARCHAR(248) ,
@Name VARCHAR(MAX),
@InfoThershold BIGINT ,
@WarningThershold BIGINT ,
@CriticalThershold BIGINT,
@AlertEnabledDefault BIT 
AS	
	BEGIN
		CREATE TABLE #MetricMetaData (      
		ID INT IDENTITY(1,1), 
		UserViewID int	,
		Metric int,
		AlertEnabledDefault bit,
		DefaultWarningValue bigint,
		DefaultCriticalValue bigint,DefaultInfoValue bigint,
		BaselineDefaultWarningValue bigint,
		BaselineDefaultCriticalValue bigint, 
		BaselineDefaultInfoValue bigint
		);

			DECLARE  @UserViewID INT = NULL,
			         @MetricID INT = NULL

            SELECT @UserViewID = TemplateID FROM AlertTemplateLookup WHERE Name=@TemplateName
			Select @MetricID = Metric FROM MetricInfo WHERE Name = @Name

			--UPDATE MetricMetaData
			--SET DefaultWarningValue=@WarningThershold,
			--DefaultCriticalValue = @CriticalThershold,
			--DefaultInfoValue = @InfoThershold,
			--AlertEnabledDefault = @AlertEnabledDefault
			--WHERE Metric=@MetricID

			INSERT INTO #MetricMetaData  
			SELECT @UserViewID, @MetricID, @AlertEnabledDefault,@WarningThershold, @CriticalThershold,@InfoThershold,@WarningThershold,@CriticalThershold, @InfoThershold 
						
			DECLARE @WarningThresholdXML nvarchar(256)
			DECLARE @CriticalThresholdXML nvarchar(256)
			DECLARE @InfoThresholdXML nvarchar(256)
			DECLARE @ComparisonText nvarchar(16)
			DECLARE @DefaultWarningValue bigint
			DECLARE @DefaultCriticalValue bigint
			DECLARE @DefaultInfoValue bigint
			DECLARE @BaselineWarningThresholdXML nvarchar(MAX)
			DECLARE @BaselineCriticalThresholdXML nvarchar(MAX)
			DECLARE @BaselineInfoThresholdXML nvarchar(MAX)
			DECLARE
			@BaselineDefaultWarningValue bigint,
			@BaselineDefaultCriticalValue bigint, 
			@BaselineDefaultInfoValue bigint

			
			-- insert default alert configuration row
			set @WarningThresholdXML = 
			'<?xml version="1.0" encoding="utf-16"?><Threshold xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema" Op="' 
			set @CriticalThresholdXML = @WarningThresholdXML
			set @InfoThresholdXML = @WarningThresholdXML
			SET @BaselineWarningThresholdXML = @WarningThresholdXML
			SET @BaselineCriticalThresholdXML = @WarningThresholdXML
			SET @BaselineInfoThresholdXML = @WarningThresholdXML
			set @ComparisonText = 'GE'
			
			IF (@TemplateName = 'Default Template' AND @Name = 'Blocked Sessions (Count)')
				BEGIN
					set @WarningThresholdXML = @WarningThresholdXML + @ComparisonText + '" Enabled="false"><Value xsi:type="xsd:float">'
				END
			ELSE
				BEGIN
					set @WarningThresholdXML = @WarningThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
				END

			set @CriticalThresholdXML = @CriticalThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
			set @InfoThresholdXML = @InfoThresholdXML + @ComparisonText + '" Enabled="false"><Value xsi:type="xsd:float">'

			SET @BaselineWarningThresholdXML = @BaselineWarningThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
			SET @BaselineCriticalThresholdXML = @BaselineCriticalThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
			SET @BaselineInfoThresholdXML = @BaselineInfoThresholdXML + @ComparisonText + '" Enabled="true"><Value xsi:type="xsd:float">'
		
			SELECT 
				@UserViewID = UserViewID, 
				@MetricID = Metric, 
				@AlertEnabledDefault = AlertEnabledDefault,  
				@DefaultWarningValue = DefaultWarningValue,
				@DefaultCriticalValue = DefaultCriticalValue,
				@AlertEnabledDefault = [AlertEnabledDefault],
				@DefaultInfoValue = DefaultInfoValue,
				@BaselineDefaultWarningValue = BaselineDefaultWarningValue,
				@BaselineDefaultCriticalValue =BaselineDefaultCriticalValue, 
				@BaselineDefaultInfoValue =BaselineDefaultInfoValue
				--
			FROM #MetricMetaData 

	  

			SET @WarningThresholdXML = @WarningThresholdXML + CAST(@DefaultWarningValue AS nvarchar(32)) + '</Value></Threshold>'
			SET @CriticalThresholdXML = @CriticalThresholdXML + CAST(@DefaultCriticalValue AS nvarchar(32)) + '</Value></Threshold>'
			SET @InfoThresholdXML = @InfoThresholdXML + CAST(@DefaultInfoValue AS nvarchar(32)) + '</Value></Threshold>'

			SET @BaselineWarningThresholdXML = @BaselineWarningThresholdXML + CAST(@BaselineDefaultWarningValue AS nvarchar(32)) + '</Value></Threshold>'
			SET @BaselineCriticalThresholdXML = @BaselineCriticalThresholdXML + CAST(@BaselineDefaultCriticalValue AS nvarchar(32)) + '</Value></Threshold>'
			SET @BaselineInfoThresholdXML = @BaselineInfoThresholdXML + CAST(@BaselineDefaultInfoValue AS nvarchar(32)) + '</Value></Threshold>'
	

     		UPDATE[DefaultMetricThresholds] 
			SET [Enabled] = @AlertEnabledDefault,
						[InfoThreshold] = @InfoThresholdXML,
						[WarningThreshold] = @WarningThresholdXML, 
						[CriticalThreshold] = @CriticalThresholdXML,
						[Data] = NULL,
						[ThresholdEnabled] = 1,
						[IsBaselineEnabled] = 0,
						[BaselineWarningThreshold] = @BaselineWarningThresholdXML,
						[BaselineCriticalThreshold] = @BaselineCriticalThresholdXML,
						[BaselineInfoThreshold] = @BaselineInfoThresholdXML
			WHERE ([UserViewID] = @UserViewID) AND ([Metric] = @MetricID) 
				
			DROP TABLE #MetricMetaData

	END
 

