if (object_id('p_UpsertCustomDashboardWidget') is not null)
begin
drop procedure [p_UpsertCustomDashboardWidget]
end
go

create procedure [p_UpsertCustomDashboardWidget] 
@dashboardID int,
@widgetName nvarchar(500),
@widgetTypeID int,
@metricID INT ,
@match INT,
@tagId XML,
@list XML,

@recordTimestamp datetime,
@widgetID int OUT
as
begin
DECLARE @countCustomDashboard int
DECLARE @sourceIdCount INT
DECLARE @recSourceCount INT


BEGIN TRY
   -- Start A Transaction
   BEGIN TRANSACTION

-- check if for record, it is update or insert call
	--select @widgetID = [WidgetID]
	--from [CustomDashboardWidgets] 
	--where [WidgetName] = @widgetName AND [DashboardID] =@dashboardID
	
	
	--set @recordTimestamp= CURRENT_TIMESTAMP
	-- update if dashboarrd widget exists.
	if (@widgetID  IS NOT NULL )
		begin
			-- update existing record in table
			IF(@match = 3)
			BEGIN
				update [dbo].[CustomDashboardWidgets]
				set [WidgetTypeID] = @widgetTypeID, [MetricID]=@metricID, 
				[MatchId] =@match, 
				[WidgetName] = @widgetName,
				[RecordUpdateDateTimestamp] = @recordTimestamp
				where [WidgetID] = @widgetID AND DashboardID =@dashboardID

				DELETE FROM [dbo].[WidgetTagMapping] WHERE WidgetID = @widgetID
				--update widget-tag mapping table too
				CREATE TABLE #tempTagUpdate(widgetId int, tagId int)
					INSERT INTO #tempTagUpdate SELECT @widgetID, A.B.value('(ID)[1]', 'int' ) ID
					FROM    @tagId.nodes('/Root/Tag') A(B)

				
					--insert into widget-tag mapping table too
					INSERT INTO [dbo].[WidgetTagMapping](WidgetID, TagId)			
					SELECT widgetId,tagId from #tempTagUpdate 
				DELETE FROM [dbo].WidgetSourceMapping WHERE WidgetID = @widgetID

			END
			ELSE IF(@match = 4)
			BEGIN
				update [dbo].[CustomDashboardWidgets]
				set [WidgetTypeID] = @widgetTypeID, [MetricID]=@metricID, [WidgetName] = @widgetName,
				[MatchId] =@match,
				[RecordUpdateDateTimestamp] = @recordTimestamp
				where [WidgetID] = @widgetID AND DashboardID =@dashboardID

				
			END
			ELSE
			BEGIN
				update [dbo].[CustomDashboardWidgets]
				set [WidgetTypeID] = @widgetTypeID, [MetricID]=@metricID, 
				[MatchId] =@match, 
				[RecordUpdateDateTimestamp] = @recordTimestamp,[WidgetName] = @widgetName
				where [WidgetID] = @widgetID AND DashboardID =@dashboardID
				
				DELETE FROM [dbo].[WidgetTagMapping] WHERE WidgetID = @widgetID
				DELETE FROM [dbo].WidgetSourceMapping WHERE WidgetID = @widgetID
				--update widget-source mapping table too
				CREATE TABLE #tempSourceUpdate(widgetId int, sourceId int)
		INSERT INTO #tempSourceUpdate SELECT @widgetID, A.B.value('(ID)[1]', 'int' ) ID
		FROM    @list.nodes('/Root/Source') A(B)

		SET @sourceIdCount = (SELECT COUNT([SQLServerID]) FROM [MonitoredSQLServers] WHERE [Active] = 1 AND [Deleted] = 0 AND [SQLServerID] IN (SELECT sourceId FROM #tempSourceUpdate))
		SET @recSourceCount = (SELECT COUNT(sourceId) FROM #tempSourceUpdate)
		if(@sourceIdCount = @recSourceCount )
		BEGIN  
				--update widget-source mapping table too
				

				INSERT INTO [dbo].WidgetSourceMapping(WidgetID, SourceServerID)			
				SELECT widgetId,sourceId from #tempSourceUpdate 
		END 
		ELSE
		BEGIN
			ROLLBACK
			RAISERROR('Sources supplied is/are invalid.', 16, 1);
		END

			END
		end
	else
		begin
		IF(@match = 4)
			BEGIN
			INSERT INTO [dbo].[CustomDashboardWidgets] WITH(TABLOCK)
				   ([DashboardID]
				   ,[WidgetName]
				   ,[WidgetTypeID]
				   ,[MetricID]
				   ,[MatchId] 
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 VALUES
				   (@dashboardID, @widgetName, @widgetTypeID,@metricID,@match,@recordTimestamp,@recordTimestamp)

				   SET @widgetID = SCOPE_IDENTITY()
			END
		ELSE IF(@match = 3)
		BEGIN
			INSERT INTO [dbo].[CustomDashboardWidgets] WITH(TABLOCK)
				   ([DashboardID]
				   ,[WidgetName]
				   ,[WidgetTypeID]
				   ,[MetricID]
				   ,[MatchId] 
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 VALUES
				   (@dashboardID, @widgetName, @widgetTypeID,@metricID,@match,@recordTimestamp,@recordTimestamp)
				   
				   SET @widgetID = SCOPE_IDENTITY()
				   CREATE TABLE #tempTag(widgetId int, tagId int)
					INSERT INTO #tempTag SELECT @widgetID, A.B.value('(ID)[1]', 'int' ) ID
					FROM    @tagId.nodes('/Root/Tag') A(B)

				
					--insert into widget-tag mapping table too
					INSERT INTO [dbo].[WidgetTagMapping](WidgetID, TagId)			
					SELECT widgetId,tagId from #tempTag 
		END
		ELSE
		BEGIN
		
			-- insert new record in table
			INSERT INTO [dbo].[CustomDashboardWidgets] WITH(TABLOCK)
				   ([DashboardID]
				   ,[WidgetName]
				   ,[WidgetTypeID]
				   ,[MetricID]
				   ,[MatchId]
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 VALUES
				   (@dashboardID, @widgetName, @widgetTypeID,@metricID,@match,@recordTimestamp,@recordTimestamp)
				   
				SET @widgetID = SCOPE_IDENTITY()
				 
		CREATE TABLE #tempSource(widgetId int, sourceId int)
		INSERT INTO #tempSource SELECT @widgetID, A.B.value('(ID)[1]', 'int' ) ID
		FROM    @list.nodes('/Root/Source') A(B)

		SET @sourceIdCount = (SELECT COUNT([SQLServerID]) FROM [MonitoredSQLServers] WHERE [Active] = 1 AND [Deleted] = 0 AND [SQLServerID] IN (SELECT sourceId FROM #tempSource))
		SET @recSourceCount = (SELECT COUNT(sourceId) FROM #tempSource)
		if(@sourceIdCount = @recSourceCount )
		BEGIN  
				--update widget-source mapping table too
				
				INSERT INTO [dbo].WidgetSourceMapping(WidgetID, SourceServerID)			
				SELECT widgetId,sourceId from #tempSource 
		END 
		ELSE
		BEGIN
			ROLLBACK
			RAISERROR('Sources supplied is/are invalid.', 16, 1);
		END
		END
	END
COMMIT

END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
END CATCH

end
 
GO 
