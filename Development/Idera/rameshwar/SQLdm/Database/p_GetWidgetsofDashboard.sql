if (object_id('p_GetWidgetsofDashboard') is not null)
begin
drop procedure [p_GetWidgetsofDashboard]
end
go

create procedure [p_GetWidgetsofDashboard] 

@dashboardId int
AS
BEGIN
	
	declare @e int
	
	BEGIN TRANSACTION
	
	SET @e = @@error
	IF (@e = 0)
	BEGIN

	DECLARE @match int;
	SELECT @match = 0;

	SELECT widget.[WidgetID]
      ,[WidgetName]
      ,[WidgetTypeID]
      ,[MetricID]
      ,[MatchId]
      ,(SELECT STUFF((SELECT ', ' + CAST([TagId] AS VARCHAR(10)) [text()]
         FROM [WidgetTagMapping] 
         WHERE [WidgetID] = wtm.[WidgetID]
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM [WidgetTagMapping] wtm
		GROUP BY [WidgetID]
		HAVING widget.[WidgetID] = [WidgetID]) AS TagIds
      ,[RecordCreatedTimestamp]
      ,[RecordUpdateDateTimestamp]	
	  

	-- Get comma saparated surce ids. Depending upon MatchId
      ,
	  (CASE [MatchId]
      WHEN 4 THEN 
	  (SELECT STUFF((SELECT ', ' + CAST([SQLServerID] AS VARCHAR(10))[text()] FROM [MonitoredSQLServers] 
	  WHERE [Active] = 1 AND [Deleted] = 0 
	  FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		) 
		ELSE
	  (SELECT STUFF((SELECT ', ' + CAST([SourceServerID] AS VARCHAR(10)) [text()]
         FROM [WidgetSourceMapping] 
         WHERE [WidgetID] = wsm.[WidgetID]
         FOR XML PATH(''), TYPE)
        .value('.','NVARCHAR(MAX)'),1,2,' ') List_Output
		FROM [WidgetSourceMapping] wsm
		GROUP BY [WidgetID]
		HAVING widget.[WidgetID] = [WidgetID]) 
		END) AS SourceIds
		  FROM [dbo].[CustomDashboardWidgets] as widget
		WHERE [DashboardID] = @dashboardId
	
	END
	IF (@e = 0)
		COMMIT
	ELSE
		ROLLBACK		

	return @e

END
 
GO 

