if (object_id('p_GetAlerts') is not null)
begin
drop procedure p_GetAlerts
end
go

CREATE PROCEDURE [dbo].[p_GetAlerts](
	@StartingAlertID bigint,
	@StartDate datetime,
	@EndDate datetime,
	@ServerXML nvarchar(max),
	@DatabaseName nvarchar(256),
	@TableName varchar(256),
	@Severity tinyint,
	@Metric int,
	@category nvarchar(128) = null, -- SQLDM 8.5 Mahesh - Added new param for rest service comsumption
	@Active bit,
	@MaxRows int = 0
)
AS
begin
	SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

	declare @starting datetime
	declare @e int
	declare @incop bit
	create table #IntermediateTable (InstanceName nvarchar(255) collate database_default not null, 
									 Metric int not null, 
									 LastScheduledCollectionTime datetime, 
									 LastDatabaseCollectionTime datetime, 
									 LastAlertRefreshTime datetime,
									 IsSnoozed bit,
									 IsMaintenanceMode bit,
									 CloudProviderId int)
	CREATE INDEX IDX_InstanceNameMetricAlerts on #IntermediateTable (InstanceName ASC, Metric ASC)								 
	
	
	declare @xmlDoc int
	declare @now datetime
	
	select @now = DateAdd(second,10,GetUTCDate())
	select @starting = coalesce(@StartDate, DATEADD(year, -10, GETUTCDATE()))
	set @incop = 0

	exec sp_xml_preparedocument @xmlDoc output, @ServerXML
	
	-- create filtered table of selected servers and metrics to include
	insert into #IntermediateTable
	select
			O.InstanceName,
			M.Metric, 
			MS.LastScheduledCollectionTime, 
			MS.LastDatabaseCollectionTime,
			MS.LastAlertRefreshTime,
			case when T.UTCSnoozeEnd > @now then 1 else 0 end,
			0, MS.CloudProviderId AS CloudProviderId
		from MetricMetaData M (NOLOCK)
		cross join openxml(@xmlDoc, '//Server', 1) with (InstanceName nvarchar(255)) as O
		join MonitoredSQLServers MS (nolock) on MS.InstanceName = O.InstanceName
		join MetricInfo as MI on M.Metric = MI.Metric
		left outer join MetricThresholds T (NOLOCK) on
				MS.[SQLServerID] = T.[SQLServerID] and
				M.[Metric] = T.[Metric] 	
			WHERE 
				MS.Active = 1 and M.Deleted = 0 and
				--Commented for DE3186 
	 			--(T.Enabled = 1 or T.Enabled is null) and
				(@Metric is null or M.[Metric] = @Metric) and
				(@category is null or MI.Category = @category) and
				(@Active is null 
				    or @Active = 0 
					or (T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @now)
				)  

	-- if an empty server node exists then set flag used to include operational alerts (servername is null)
	if (exists(select InstanceName from openxml(@xmlDoc, '//Server', 1) with (InstanceName nvarchar(255)) where InstanceName is null))
		set @incop = 1

	exec sp_xml_removedocument @xmlDoc


	update #IntermediateTable
	set IsMaintenanceMode = 1
	from
	#IntermediateTable I inner join Alerts A (NOLOCK) on A.ServerName collate database_default = I.InstanceName collate database_default 
	where A.Metric = 48 and I.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
		
	create table #RecommendationAlerts(AlertId bigint not null,
									   UTCOccurrenceDateTime datetime not null,
									   serverName nvarchar(255) collate database_default,
									   DatabaseName nvarchar(255) collate database_default,
									   TableName nvarchar(255) collate database_default,
									   Active bit,
									   Metric int,
									   Severity tinyint,
									   StateEvent tinyint,
									   Value float,
									   Heading nvarchar(256) collate database_default,
									   Message nvarchar(2048) collate database_default,
									   CloudProviderName varchar(500) collate database_default not null
									   )
	insert into #RecommendationAlerts
	 SELECT TOP 1 A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],A.[StateEvent],
					A.[Value],A.[Heading],A.[Message] ,CP.CloudProviderName AS ServerType
					from #IntermediateTable I (NOLOCK)
						inner join Alerts A (nolock) on 
							A.[ServerName] = I.InstanceName and
							A.Metric = I.Metric	
						left join CloudProviders CP on CP.CloudProviderId = I.CloudProviderId
					where
						A.Metric = 56 
						and I.IsSnoozed = 0
						and A.Heading = 'Recommendations Are Available'
						and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID)  
						and (@Severity is null or A.[Severity] = @Severity) 
					order by UTCOccurrenceDateTime DESC
		
	if (@Active is null or @Active = 0)
	begin -- return all alerts that match the filter
		if (@MaxRows is not null)
			set rowcount @MaxRows

		if (@EndDate is null)
		begin
			SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],
					case 
						when I.IsMaintenanceMode = 1 and A.Metric = 48
							then 1
						when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastScheduledCollectionTime] and
							 A.Metric not in (select MetricID from DBMetrics (NOLOCK)) and
							 I.IsSnoozed = 0
						then 1
						 when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastDatabaseCollectionTime] and
							 A.Metric in (select MetricID from DBMetrics (NOLOCK)) and
							 I.IsSnoozed = 0
						then 1							 
						else 0 
					end,
					A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message], CP.CloudProviderName AS ServerType
			FROM #IntermediateTable I 
				left join [Alerts] A (NOLOCK) on A.ServerName = I.InstanceName and A.Metric = I.Metric
				left join CloudProviders CP on CP.CloudProviderId = I.CloudProviderId
			WHERE 
				not (A.[UTCOccurrenceDateTime] < @starting) 
				and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
				and (@Severity is null or A.[Severity] = @Severity) 
				and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
				and (@TableName is null or A.[TableName] = @TableName) 
			UNION
			-- pick up operational alerts (server name is null)
			SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],
					A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message],CP.CloudProviderName AS ServerType
			FROM [Alerts] A (NOLOCK) 
			join MetricInfo as MI (NOLOCK) on A.Metric = MI.Metric
			join MonitoredSQLServers MS on MS.InstanceName = A.ServerName
			join CloudProviders CP on CP.CloudProviderId = MS.CloudProviderId
			WHERE 
				(@incop = 1) and
				(A.ServerName is null) and
				(A.[UTCOccurrenceDateTime] between @starting and @EndDate) and
				(@StartingAlertID is null or A.[AlertID] > @StartingAlertID) and
				(@Metric is null or A.[Metric] = @Metric) and
				(@category is null or MI.Category = @category) and
				(@Severity is null or A.[Severity] = @Severity)
			ORDER BY A.[UTCOccurrenceDateTime] DESC
		end
		else
		begin
			SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],
					case 
						when I.IsMaintenanceMode = 1 and A.Metric = 48
							then 1
						when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastScheduledCollectionTime] and
							 A.Metric not in (select MetricID from DBMetrics (NOLOCK)) and
							 I.IsSnoozed = 0
						then 1
						 when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastDatabaseCollectionTime] and
							 A.Metric in (select MetricID from DBMetrics (NOLOCK)) and
							 I.IsSnoozed = 0
						then 1							 
						else 0 
					end,
					A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message], CP.CloudProviderName AS ServerType
			FROM #IntermediateTable I 
				left join [Alerts] A (NOLOCK) on A.ServerName = I.InstanceName and A.Metric = I.Metric
				left join CloudProviders CP on CP.CloudProviderId = I.CloudProviderId
			WHERE 
				(A.[UTCOccurrenceDateTime] between @starting and @EndDate) and
				(@StartingAlertID is null or A.[AlertID] > @StartingAlertID) and
				(@DatabaseName is null or A.[TableName] = @DatabaseName) and
				(@TableName is null or A.[TableName] = @TableName) and
				(@Severity is null or A.[Severity] = @Severity)
			UNION
			-- pick up operational alerts (server name is null)
			SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],
					A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message],CP.CloudProviderName AS ServerType
			FROM [Alerts] A (NOLOCK) 
			Join MetricInfo MI (NOLOCK) on A.Metric = MI.Metric
			join MonitoredSQLServers MS on MS.InstanceName = A.ServerName
			join CloudProviders CP on CP.CloudProviderId = MS.CloudProviderId
			WHERE 
				(@incop = 1) and
				(A.ServerName is null) and
				(A.[UTCOccurrenceDateTime] between @starting and @EndDate) and
				(@StartingAlertID is null or A.[AlertID] > @StartingAlertID) and
				(@Metric is null or A.[Metric] = @Metric) and
				(@category is null or MI.Category = @category) and
				(@Severity is null or A.[Severity] = @Severity)
			ORDER BY A.[UTCOccurrenceDateTime] DESC
		end
	end
	else
	begin -- only return the last active alerts (relies on AlertID being sequentially assigned)
	
	if (@MaxRows is not null) -- -- SQLDM 8.5:Mahesh : Setting Maxrows for Active Alerts as well
			set rowcount @MaxRows
			--Ignoring the LastScheduledCollectionTime condition for AGRoleChnage metric to see the latest alerts in active alerts screen
			SELECT A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading]
			,A.[Message] ,CP.CloudProviderName AS ServerType
				from #IntermediateTable I
					inner join Alerts A (nolock) on 
						A.[ServerName] = I.InstanceName and
						(A.Metric = 116 or (A.UTCOccurrenceDateTime = I.LastScheduledCollectionTime)) and
						A.Metric = I.Metric	
					left join CloudProviders CP on CP.CloudProviderId = I.CloudProviderId
				where
					(A.Metric = 48 or I.IsMaintenanceMode = 0)
					and (A.[Active] = 1)
					and I.IsSnoozed = 0
					and A.Metric not in (select MetricID from DBMetrics (NOLOCK))
					and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
					and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
					and (@TableName is null or A.[TableName] = @TableName) 
					and (@Severity is null or A.[Severity] = @Severity) 
		union	
			SELECT A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],A.[StateEvent],
					A.[Value],A.[Heading],A.[Message] ,CP.CloudProviderName AS ServerType
					from #IntermediateTable I (NOLOCK)
						inner join Alerts A (nolock) on 
							A.[ServerName] = I.InstanceName and
							A.UTCOccurrenceDateTime = I.LastDatabaseCollectionTime and
							A.Metric = I.Metric	
						left join CloudProviders CP on CP.CloudProviderId = I.CloudProviderId
					where
						(A.Metric = 48 or I.IsMaintenanceMode = 0)
						and A.[Active] = 1
						and I.IsSnoozed = 0
						and A.Metric in (select MetricID from DBMetrics (NOLOCK))
						and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
						and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
						and (@TableName is null or A.[TableName] = @TableName) 
						and (@Severity is null or A.[Severity] = @Severity)
		union 
			SELECT * FROM #RecommendationAlerts
	end
	
	DROP TABLE #IntermediateTable
	SELECT @e = @@error
	RETURN @e
END	
 





GO


