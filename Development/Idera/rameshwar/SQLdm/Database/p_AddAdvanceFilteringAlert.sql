if (object_id('p_AddAdvanceFilteringAlert') is not null)
begin
drop procedure p_AddAdvanceFilteringAlert
end
go
CREATE PROCEDURE [dbo].[p_AddAdvanceFilteringAlert] 
(
	-- Table column params
	@ServerName				nvarchar(256),
	@Metric					int,	
	@CurrentThresholdType	varchar(MAX),
	@DBName varchar(150)
)
AS
BEGIN
	DECLARE
		@CurrentVoilentionCount int,
		@TotalSnapShot int,
		@ConfigVoilentionCount int,
		@ConfigTotalSnapShot int,
		@ConfigData varchar(MAX),
		@AlertID int,	
		@ThresholdRefreshTimeValue int,
		@ThresholdTimeValue int,
		@xmlDoc int,
  		--@MonitoredState varchar(50),
		@TempTotalCount int,
		@InstanceId int,
		@TemplateId int,
		@AlertThresholdType	varchar(MAX)

		SET @AlertThresholdType = @CurrentThresholdType

		SELECT @InstanceId = SQLServerID FROM MonitoredSQLServers WHERE InstanceName = @ServerName 
		--SELECT @TemplateId = TemplateID FROM AlertInstanceTemplate WHERE SQLServerID = @InstanceId

		SELECT	TOP 1 @ConfigData = Data 
		FROM	MetricThresholds 
		WHERE	Metric = @Metric 
		AND		SQLServerID=@InstanceId

	 
	-- select @AlertID = AlertID  from Alerts where Metric=@Metric and Alerts.ServerName=@ServerName  and Alerts.UTCOccurrenceDateTime=@SnapShotTime --SK	
	-- Check if the advanced filter configuration exists for the metric

	IF @ConfigData IS NOT NULL
		BEGIN
			--IF Database is Null then SELECT ID FROM AdvanceFilteringAlert WHERE  ServerName = @ServerName AND Metric = @Metric			 
			--ELSE SELECT ID FROM AdvanceFilteringAlert WHERE  ServerName = @ServerName AND Metric = @Metric AND DatabaseName = @DBName

			IF EXISTS(SELECT ID FROM AdvanceFilteringAlert WHERE  ServerName = @ServerName AND Metric = @Metric AND (DatabaseName = @DBName OR @DBName IS Null))
				BEGIN
				-- Read current record from AdvanceFilteringAlert for @AlertTemplateId, @ServerName, @Metric
				-- if record exists read CurrentVoilentionCount and TotalSnapShot
					SELECT	@CurrentVoilentionCount = CurrentVoilentionCount, 
							@TotalSnapShot = TotalSnapShot
					FROM	AdvanceFilteringAlert
					WHERE	ServerName = @ServerName
					AND		Metric = @Metric AND (DatabaseName = @DBName OR @DBName IS Null)
					
		
					-- Read alter filter advanced configuation from DefaultMetricThresholds.Data XML for @Metric
					-- parse @ConfigData to set following values
					
					SET @ConfigData = REPLACE(@ConfigData, '<?xml version="1.0" encoding="utf-16"?>', '')
		
					EXEC sp_xml_preparedocument  @xmlDoc output, @ConfigData
					
					SELECT	@ThresholdTimeValue = ThresholdTime,
							@ThresholdRefreshTimeValue = ThresholdRefreshTime
							--@MonitoredState = AlertFilterIndex
					from	openxml(@xmlDoc, '/AdvancedAlertConfigurationSettings', 2) WITH
					(
						--AlertFilterIndex nvarchar(max),
						ThresholdTime int,
						ThresholdRefreshTime int
					)

					SELECT @ConfigVoilentionCount = @ThresholdTimeValue, @ConfigTotalSnapShot = @ThresholdRefreshTimeValue
														
					IF(@ConfigTotalSnapShot <= @TotalSnapShot + 1)
						BEGIN
							---- reset counter

							SELECT @TempTotalCount = count(1) FROM  
							(
								SELECT ThresholdType
								FROM  AdvanceFilteringAlert  
								WHERE   Metric=@Metric AND ServerName=@ServerName AND (DatabaseName = @DBName OR @DBName IS Null)
							) AS A  CROSS APPLY dbo.fn_Split(ThresholdType, ',')
							--where Value = @MonitoredState

							-- SELECT @TempTotalCount, @ConfigTotalSnapShot, @MonitoredState 

							--if(@TempTotalCount =@ConfigTotalSnapShot-1 or @TempTotalCount >=@ConfigTotalSnapShot )
							if(@ConfigTotalSnapShot = @TotalSnapShot + 1)
								BEGIN
									UPDATE	AdvanceFilteringAlert
									SET		ThresholdType = '',
											CurrentVoilentionCount = 0,
											TotalSnapShot = 0
									WHERE	ServerName = @ServerName 
									AND		Metric = @Metric
									AND (DatabaseName = @DBName OR @DBName IS Null)
								END			
								
								--select @TempTotalCount as '@TempTotalCount'
								--select @ConfigVoilentionCount as '@ConfigVoilentionCount'
								--select @ConfigTotalSnapShot  as '@ConfigTotalSnapShot'
								
							if(@TempTotalCount >= @ConfigVoilentionCount) -- OR @MonitoredState <> @AlertThresholdType)
								BEGIN
									--print 'Return 1 !!'								
									SELECT 1 --As 'GenerateAlertA'
								END
							ELSE
								BEGIN
									--print 'Return 0 !!'
									SELECT 0 --As 'GenerateAlertB'
								END
						END
					ELSE
						BEGIN
							SELECT	@CurrentThresholdType= a.ThresholdType + ','+@CurrentThresholdType  
							FROM	AdvanceFilteringAlert a 
							WHERE   Metric=@Metric AND ServerName=@ServerName AND (DatabaseName = @DBName OR @DBName IS Null)

							UPDATE	AdvanceFilteringAlert
							SET	    CurrentVoilentionCount = @CurrentVoilentionCount + 1,
									TotalSnapShot = @TotalSnapShot + 1,
									ThresholdType =@CurrentThresholdType
							WHERE	ServerName = @ServerName
							AND		Metric = @Metric
							AND (DatabaseName = @DBName OR @DBName IS Null)

						
							--	print 'Record updated !!'
							-- return false to avoid genrating alert
							SELECT 0 --As 'GenerateAlertC'

							--if(@MonitoredState <> @AlertThresholdType)
							--	BEGIN
							--		--print 'Return 1 !!'
							--		SELECT 1
							--	END
							--ELSE
							--	BEGIN
							--		--print 'Return 0 !!'
							--		SELECT 0
							--	END
							
						END
				END
			ELSE
				BEGIN
					INSERT INTO AdvanceFilteringAlert
						(ServerName,Metric,ThresholdType, CurrentVoilentionCount, TotalSnapShot, DatabaseName) 
					VALUES
						(@ServerName,@Metric,@CurrentThresholdType, 1, 1, @DBName)	

					-- return false to avoid genrating alert
				--	print 'Record inserted !!'
					SELECT 0--	As 'GenerateAlertD' 
				END
		END
	ELSE
		BEGIN
			print 'Config not found'
			SELECT  1-- As 'GenerateAlertE'
		END
END
 



