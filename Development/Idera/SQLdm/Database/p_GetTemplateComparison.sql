if (object_id('p_GetTemplateComparison') is not null)
begin
drop procedure p_GetTemplateComparison
end
go

CREATE PROCEDURE [dbo].[p_GetTemplateComparison] 
@SourceTemplate INT,
@TargetTemplate INT

AS
	BEGIN
		CREATE TABLE #AlertThresholds 
		(
			UserViewID INT,
			MetricID INT, 
			WarningThreshold NVARCHAR(200), 
			CriticalThreshold NVARCHAR(200),
			InfoThreshold NVARCHAR(200),
			criticalThresholdEnabled NVARCHAR(256),
			warningThresholdEnabled NVARCHAR(256),
			infoThresholdEnabled NVARCHAR(256),
			[Enabled] BIT
		)

		DECLARE @MetricStatus TABLE 
		(
			UserViewID INT,
			MetricID INT,
			MetricStatus BIT,
			[Enabled] BIT			
		)

		INSERT INTO @MetricStatus 
		SELECT UserViewID,Metric,[Enabled],ThresholdEnabled
		FROM DefaultMetricThresholds
		WHERE UserViewID IN (@SourceTemplate,@TargetTemplate)

		DECLARE @MetricID INT,
			@UserViewID INT,
			@InfoThresholdXML NVARCHAR(2048), 
			@WarningThresholdXML NVARCHAR(2048), 
			@CriticalThresholdXML NVARCHAR(2048),
			@enabled BIT,
			@thresholdInstanceId INT,
			@thresholdEnabled BIT,
			@xmlDoc INT, 
			@warningValueArray NVARCHAR(4000), 		
			@criticalValueArray NVARCHAR(4000), 		
			@infoValueArray NVARCHAR(4000), 		
			@serviceState NVARCHAR(256),
			@criticalThresholdEnabled NVARCHAR(256),
			@warningThresholdEnabled NVARCHAR(256),
			@infoThresholdEnabled NVARCHAR(256)

		SET @infoValueArray = ''
		SET @warningValueArray = ''
		SET @criticalValueArray = ''

		DECLARE  read_threshold INSENSITIVE CURSOR
		FOR 
		SELECT	[UserViewID],
				[Metric],
				[WarningThreshold],
				[CriticalThreshold],
				[InfoThreshold],
				[Enabled]
		FROM DefaultMetricThresholds
		WHERE (UserViewID = @SourceTemplate OR UserViewID = @TargetTemplate)

		FOR READ only
		SET NOCOUNT ON
		OPEN read_threshold
		FETCH read_threshold 
		INTO @UserViewID, @MetricID, @WarningThresholdXML, @CriticalThresholdXML, @InfoThresholdXML, @enabled
		
		WHILE @@FETCH_STATUS = 0
		BEGIN	
		
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @WarningThresholdXML
			SELECT @warningThresholdEnabled = [Enabled], @warningValueArray = Value
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Enabled NVARCHAR(256), Value NVARCHAR(2048))
			
			DECLARE read_threshold_entry CURSOR
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @warningValueArray = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @warningValueArray = @warningValueArray + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc
			
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @CriticalThresholdXML
			SELECT @criticalValueArray = Value, @criticalThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))

			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @criticalValueArray = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @criticalValueArray = @criticalValueArray + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc			

			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @InfoThresholdXML
			SELECT @infoValueArray = Value, @infoThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))
			
			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				SET @infoValueArray = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @infoValueArray = @infoValueArray + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc						
			
			INSERT INTO #AlertThresholds 
			(
				[UserViewID],
				[MetricID], 				
				[WarningThreshold], 
				[CriticalThreshold], 
				[InfoThreshold], 
				[criticalThresholdEnabled], 
				[warningThresholdEnabled], 
				[infoThresholdEnabled],
				[Enabled]
			)
			VALUES 
			(
				@UserViewID,
				@MetricID, 				
				@warningValueArray, 
				@criticalValueArray, 
				@infoValueArray, 
				@criticalThresholdEnabled, 
				@warningThresholdEnabled, 
				@infoThresholdEnabled,
				@enabled
			)
		
			FETCH read_threshold 
			INTO @UserViewID,@MetricID,@WarningThresholdXML, @CriticalThresholdXML, @InfoThresholdXML, @enabled
		END
		CLOSE read_threshold
		DEALLOCATE read_threshold
			
		SELECT A.UserViewID,A.MetricID,Temp.[Name],
			   A.[WarningThreshold] AS 'WarningThresholdSrc',
			   A.[CriticalThreshold] AS 'CriticalThresholdSrc',
			   A.[InfoThreshold] AS 'InfoThresholdSrc',
			   A.Enabled AS 'EnabledSrc',			   
			  -- Temp.UserViewID,
			   Temp.[WarningThreshold]  AS 'WarningThresholdTar',
			   Temp.[CriticalThreshold] AS 'CriticalThresholdTar',
			   Temp.[InfoThreshold] AS 'InfoThresholdTar',			 
			   A.infoThresholdEnabled,
			   A.warningThresholdEnabled,
			   A.criticalThresholdEnabled,
			   Temp.Enabled as 'EnabledTar',
			   Temp.ThresholdEnabled 
		FROM 
		(
			SELECT #AlertThresholds.UserViewID,#AlertThresholds.[MetricID],[WarningThreshold],[CriticalThreshold],[InfoThreshold],
				   MI.[Name],
				   M.MetricStatus AS 'Enabled',
				   M.Enabled AS 'ThresholdEnabled'
			FROM #AlertThresholds
			JOIN MetricInfo MI ON MI.Metric = #AlertThresholds.MetricID
			JOIN @MetricStatus M ON M.MetricID = #AlertThresholds.MetricID
									AND M.UserViewID = #AlertThresholds.UserViewID
			WHERE #AlertThresholds.UserViewID = @TargetTemplate	
		)Temp
		JOIN #AlertThresholds A ON A.MetricID = Temp.MetricID		
		AND A.UserViewID = @SourceTemplate

		DROP TABLE #AlertThresholds
END
 
