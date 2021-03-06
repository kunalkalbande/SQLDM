if (object_id('p_GetAlertThreshold') is not null)

begin

drop procedure [p_GetAlertThreshold]

end
GO
/****** Object:  StoredProcedure [dbo].[p_GetAlertThreshold]    Script Date: 3/19/2019 11:51:22 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[p_GetAlertThreshold] 
(
@SqlServerID int
)
as 

		BEGIN
		CREATE TABLE #AlertThresholds 
		(			
			MetricID INT, 
			MetricName VARCHAR(300),
			Enabled BIT,
			TemplateName VARCHAR(300),
			WarningThresholdSrc NVARCHAR(200), 
			CriticalThresholdSrc NVARCHAR(200),
			InfoThresholdSrc NVARCHAR(200),
			criticalThresholdEnabled NVARCHAR(256),
			warningThresholdEnabled NVARCHAR(256),
			infoThresholdEnabled NVARCHAR(256),

			WarningThresholdTrg NVARCHAR(200), 
			CriticalThresholdTrg NVARCHAR(200),
			InfoThresholdTrg NVARCHAR(200)
		)
		
		DECLARE @MetricID INT,			 
			@MetricName VARCHAR(300),
			@Enabled BIT,
			@TemplateName VARCHAR(300),
			@WarningThresholdXMLSrc NVARCHAR(2048), 
			@CriticalThresholdXMLSrc NVARCHAR(2048),	
			@InfoThresholdXMLSrc NVARCHAR(2048), 

			@WarningThresholdXMLTrg NVARCHAR(2048), 
			@CriticalThresholdXMLTrg NVARCHAR(2048),	
			@InfoThresholdXMLTrg NVARCHAR(2048), 

			@xmlDoc INT, 
			@warningValueArraySrc NVARCHAR(4000), 		
			@criticalValueArraySrc NVARCHAR(4000), 		
			@infoValueArraySrc NVARCHAR(4000), 		

			@warningValueArrayTrg NVARCHAR(4000), 		
			@criticalValueArrayTrg NVARCHAR(4000), 		
			@infoValueArrayTrg NVARCHAR(4000), 		

			@serviceState NVARCHAR(256),
			@criticalThresholdEnabled NVARCHAR(256),
			@warningThresholdEnabled NVARCHAR(256),
			@infoThresholdEnabled NVARCHAR(256)

		SET @infoValueArraySrc = ''
		SET @warningValueArraySrc = ''
		SET @criticalValueArraySrc = ''
		SET @infoValueArrayTrg = ''
		SET @warningValueArrayTrg = ''
		SET @criticalValueArrayTrg = ''

		DECLARE  read_threshold INSENSITIVE CURSOR
		FOR 
				
				SELECT MI.Metric, MI.Name as MetricName,MT.Enabled,AI.Name as TemplateName, MT.WarningThreshold as 'WarningThresholdSrc',MT.CriticalThreshold as 'CriticalThresholdSrc',MT.InfoThreshold as 'InfoThresholdSrc'	
				,DT.WarningThreshold as 'WarningThresholdTrg', DT.CriticalThreshold as 'CriticalThresholdTrg',DT.InfoThreshold as 'InfoThresholdTrg'				
				FROM dbo.MetricThresholds MT
				INNER JOIN dbo.DefaultMetricThresholds DT ON MT.Metric = DT.Metric	
				INNER JOIN dbo.MetricInfo MI ON MI.Metric = MT.Metric
				INNER JOIN AlertInstanceTemplate AT ON AT.SQLServerID = MT.SQLServerID AND AT.TemplateID = DT.UserViewID
				INNER JOIN dbo.AlertTemplateLookup AI ON AI.TemplateID = AT.TemplateID
				WHERE  MT.SQLServerID = @SqlServerID				
		
		FOR READ only
		SET NOCOUNT ON
		OPEN read_threshold
		FETCH read_threshold
		INTO @MetricID, @MetricName, @Enabled, @TemplateName, @WarningThresholdXMLSrc, @CriticalThresholdXMLSrc, @InfoThresholdXMLSrc, @WarningThresholdXMLTrg, @CriticalThresholdXMLTrg, @InfoThresholdXMLTrg	
		
		WHILE @@FETCH_STATUS = 0
		BEGIN	
		   -- Source Value Parsing Starts:
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @WarningThresholdXMLSrc
			SELECT @warningThresholdEnabled = [Enabled], @warningValueArraySrc = Value
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Enabled NVARCHAR(256), Value NVARCHAR(2048))
			
			DECLARE read_threshold_entry CURSOR
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @warningValueArraySrc = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @warningValueArraySrc = @warningValueArraySrc + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc
			
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @CriticalThresholdXMLSrc
			SELECT @criticalValueArraySrc = Value, @criticalThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))

			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @criticalValueArraySrc = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @criticalValueArraySrc = @criticalValueArraySrc + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc			

			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @InfoThresholdXMLSrc
			SELECT @infoValueArraySrc = Value, @infoThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))
			
			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				SET @infoValueArraySrc = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @infoValueArraySrc = @infoValueArraySrc + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc						
			-- Source Value Parsing Ends:

			 -- Target Value Parsing Starts:
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @WarningThresholdXMLTrg
			SELECT @warningThresholdEnabled = [Enabled], @warningValueArrayTrg = Value
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Enabled NVARCHAR(256), Value NVARCHAR(2048))
			
			DECLARE read_threshold_entry CURSOR
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @warningValueArrayTrg = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @warningValueArrayTrg = @warningValueArrayTrg + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc
			
			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @CriticalThresholdXMLTrg
			SELECT @criticalValueArrayTrg = Value, @criticalThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))

			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			IF (@@FETCH_STATUS = 0)
			BEGIN
				SET @criticalValueArrayTrg = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @criticalValueArrayTrg = @criticalValueArrayTrg + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc			

			EXEC sp_xml_preparedocument @xmlDoc OUTPUT, @InfoThresholdXMLTrg
			SELECT @infoValueArrayTrg = Value, @infoThresholdEnabled = [Enabled] 
			FROM OPENXML(@xmlDoc, '/Threshold', 3) WITH (Value NVARCHAR(2048), Enabled NVARCHAR(256))
			
			DECLARE read_threshold_entry cursor
			FOR
				SELECT servicestate FROM OPENXML(@xmlDoc, '//anyType', 2) WITH (servicestate NVARCHAR(256) 'text()')

			OPEN read_threshold_entry
			FETCH read_threshold_entry INTO @serviceState
			
			if (@@FETCH_STATUS = 0)
			BEGIN
				SET @infoValueArraySrc = ''
			END
			
			WHILE @@fetch_status = 0
			BEGIN
				SET @infoValueArrayTrg = @infoValueArrayTrg + @serviceState + ', '
				FETCH read_threshold_entry INTO @serviceState
			END
	
			CLOSE read_threshold_entry
			DEALLOCATE read_threshold_entry
			EXEC sp_xml_removedocument @xmlDoc						
			-- Source Value Parsing Ends:



			INSERT INTO #AlertThresholds 
			(
				[MetricID], 
				[MetricName],
			    [Enabled],
			    [TemplateName],
			    [WarningThresholdSrc], 
			    [CriticalThresholdSrc],
				[InfoThresholdSrc],
				[criticalThresholdEnabled], 
				[warningThresholdEnabled], 
				[infoThresholdEnabled],
			    
				[WarningThresholdTrg], 
			    [CriticalThresholdTrg],
				[InfoThresholdTrg]
			)
			VALUES 
			(			
				@MetricID, 				
				@MetricName,
				@Enabled,
				@TemplateName,
				@warningValueArraySrc, 
				@criticalValueArraySrc, 
				@infoValueArraySrc,
				@criticalThresholdEnabled, 
				@warningThresholdEnabled, 
				@infoThresholdEnabled,			

				@warningValueArrayTrg, 
				@criticalValueArrayTrg, 
				@infoValueArrayTrg
			)
		
			FETCH read_threshold 
			INTO @MetricID, @MetricName, @Enabled, @TemplateName, @WarningThresholdXMLSrc, @CriticalThresholdXMLSrc, @InfoThresholdXMLSrc , @WarningThresholdXMLTrg, @CriticalThresholdXMLTrg, @InfoThresholdXMLTrg		
		END
		CLOSE read_threshold
		DEALLOCATE read_threshold
			
		SELECT A.MetricID,A.MetricName,A.Enabled, A.TemplateName,
			   A.[WarningThresholdSrc],
			   A.[CriticalThresholdSrc],
			   A.[InfoThresholdSrc],
			   A.[criticalThresholdEnabled],
			   A.[warningThresholdEnabled],
			   A.[infoThresholdEnabled],

			   A.[WarningThresholdTrg],
			   A.[CriticalThresholdTrg],
			   A.[InfoThresholdTrg]
		FROM 
		#AlertThresholds A

		
		DROP TABLE #AlertThresholds
END

