if (object_id('p_CreateCopyOfCustomDashboard') is not null)
begin
drop procedure [p_CreateCopyOfCustomDashboard]
end
go

create procedure [p_CreateCopyOfCustomDashboard] 
@sourceCustomDashboardID INT,
@copyCustomDashboardID INT OUT

AS
begin
DECLARE @countCustomDashboard INT

BEGIN TRY
   -- Start A Transaction
BEGIN TRANSACTION




-- check if for record for given dashboard ID
	select @countCustomDashboard = count(CustomDashboardName) 
		FROM CustomDashboard WITH(NOLOCK)
		WHERE CustomDashboardId = @sourceCustomDashboardID
	
	
	--set @@RecordTimestamp= CURRENT_TIMESTAMP
	-- update if permission exists.
	if (@countCustomDashboard != 0 )
		begin

			-- insert existing record in new row of table
			-- insert new record in table
			INSERT INTO [dbo].[CustomDashboard] WITH(TABLOCK)
				   ([CustomDashboardName]
				   ,[IsDefaultOnUI]
				   ,[UserSID]
				   ,[Tags]
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 (-- get record for dashboard ID
			 SELECT 'Copy of '+[CustomDashboardName],0
			  ,UserSID
			  ,Tags
			  ,CURRENT_TIMESTAMP,CURRENT_TIMESTAMP
			FROM CustomDashboard WITH (NOLOCK)
			WHERE CustomDashboardId = @sourceCustomDashboardID)
	
			
			SET @copyCustomDashboardID = SCOPE_IDENTITY()

			INSERT INTO [dbo].[CustomDashboardWidgets]
					([DashboardID]
				   ,[WidgetName]
				   ,[WidgetTypeID]
				   ,[MetricID]
				   ,[MatchId] 
				   ,[RecordCreatedTimestamp]
				   ,[RecordUpdateDateTimestamp])
			 SELECT @copyCustomDashboardID
				   ,[WidgetName]
				   ,[WidgetTypeID]
				   ,[MetricID]
				   ,[MatchId]
				   ,CURRENT_TIMESTAMP,CURRENT_TIMESTAMP FROM [CustomDashboardWidgets] WITH (NOLOCK) WHERE [DashboardID] = @sourceCustomDashboardID
					--TODO: pass the timestamps from the code
				   INSERT INTO [dbo].[WidgetTagMapping](WidgetID, TagId)			
					(SELECT CDW2.[WidgetID],WTM.TagId 
					FROM [WidgetTagMapping] WTM WITH (NOLOCK)
					JOIN [CustomDashboardWidgets] CDW WITH (NOLOCK) ON CDW.[WidgetID] = WTM.[WidgetID]
					JOIN [CustomDashboardWidgets] CDW2 WITH (NOLOCK) ON CDW.[WidgetName] = CDW2.[WidgetName]	
					WHERE CDW.DashboardID = @sourceCustomDashboardID AND CDW2.DashboardID = @copyCustomDashboardID)

					INSERT INTO [dbo].[WidgetSourceMapping](WidgetID, SourceServerID)			
					(SELECT CDW2.[WidgetID],WSM.SourceServerID 
					FROM [WidgetSourceMapping] WSM WITH (NOLOCK)
					JOIN [CustomDashboardWidgets] CDW WITH (NOLOCK) ON CDW.[WidgetID] = WSM.[WidgetID]
					JOIN [CustomDashboardWidgets] CDW2 WITH (NOLOCK) ON CDW.[WidgetName] = CDW2.[WidgetName]	
					WHERE CDW.DashboardID = @sourceCustomDashboardID AND CDW2.DashboardID = @copyCustomDashboardID)


		end
	else
		begin
			SET @copyCustomDashboardID = 0
		   end
	

	COMMIT

END TRY
BEGIN CATCH
 Print 'Transaction Failed - Will Rollback'
  -- Any Error Occurred during Transaction. Rollback
  ROLLBACK  -- Roll back
END CATCH

RETURN @copyCustomDashboardID;
end
 
GO 
