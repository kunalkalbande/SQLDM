if (object_id('p_GetAlertsForInstance') is not null)
begin
drop procedure p_GetAlertsForInstance
end
go

CREATE PROCEDURE p_GetAlertsForInstance(
	@StartingAlertID bigint,
	@StartDate datetime,
	@EndDate datetime,
	@SQLServerID int,
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
	create table #IntermediateTable (InstanceName nvarchar(255) collate database_default,
	                                 CloudProviderName nvarchar(255),
									 Metric int, 
									 LastScheduledCollectionTime datetime, 
									 LastDatabaseCollectionTime datetime, 
									 LastAlertRefreshTime datetime,
									 IsSnoozed bit,
									 IsMaintenanceMode bit)
	declare @now datetime
	
	select @now = DateAdd(second,10,GetUTCDate())
	select @starting = coalesce(@StartDate, DATEADD(year, -10, GETUTCDATE()))
	
	-- SqlDM 10.2 (Anshul Aggarwal) - Fetches Alerts for specific instance.

	-- create filtered table of selected servers and metrics to include
	insert into #IntermediateTable
	select
			MS.InstanceName,
			CP.CloudProviderName,
			M.Metric, 
			MS.LastScheduledCollectionTime, 
			MS.LastDatabaseCollectionTime,
			MS.LastAlertRefreshTime,
			case when T.UTCSnoozeEnd > @now then 1 else 0 end,
			0
		from MetricMetaData M (NOLOCK)
		join MonitoredSQLServers MS (nolock) on MS.SQLServerID = @SQLServerID
		join MetricInfo as MI on M.Metric = MI.Metric
		join CloudProviders CP on CP.CloudProviderId = MS.CloudProviderId
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

	update #IntermediateTable
	set IsMaintenanceMode = 1
	from
	#IntermediateTable I inner join Alerts A (NOLOCK) on A.ServerName collate database_default = I.InstanceName collate database_default 
	where A.Metric = 48 and I.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
		
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
							 A.Metric not in (select MetricID from DBMetrics) and
							 I.IsSnoozed = 0
						then 1
						 when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastDatabaseCollectionTime] and
							 A.Metric in (select MetricID from DBMetrics) and
							 I.IsSnoozed = 0
						then 1							 
						else 0 
					end,
					A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message],I.CloudProviderName as 'ServerType'
			FROM #IntermediateTable I 
				left join [Alerts] A (NOLOCK) 
					on A.ServerName = I.InstanceName
						and A.Metric = I.Metric
			WHERE 
				not (A.[UTCOccurrenceDateTime] < @starting) 
				and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
				and (@Severity is null or A.[Severity] = @Severity) 
				and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
				and (@TableName is null or A.[TableName] = @TableName) 
		end
		else
		begin
			select @EndDate = (select max(UTCCollectionDateTime) from [ServerActivity] where [SQLServerID] = @SQLServerID 
			AND isnull(RefreshType,0) = 0
			AND (UTCCollectionDateTime between @starting AND @EndDate))
			SELECT	A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],
					case 
						when I.IsMaintenanceMode = 1 and A.Metric = 48
							then 1
						when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastScheduledCollectionTime] and
							 A.Metric not in (select MetricID from DBMetrics) and
							 I.IsSnoozed = 0
						then 1
						 when I.IsMaintenanceMode = 0 and A.[Active] = 1 and 
							 A.[UTCOccurrenceDateTime] = I.[LastDatabaseCollectionTime] and
							 A.Metric in (select MetricID from DBMetrics) and
							 I.IsSnoozed = 0
						then 1							 
						else 0 
					end,
					A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message]
			FROM #IntermediateTable I 
				left join [Alerts] A (NOLOCK) 
					on A.ServerName = I.InstanceName and
					   A.Metric = I.Metric
			WHERE 
				(A.[UTCOccurrenceDateTime] = @EndDate) and
				(@StartingAlertID is null or A.[AlertID] > @StartingAlertID) and
				(@DatabaseName is null or A.[TableName] = @DatabaseName) and
				(@TableName is null or A.[TableName] = @TableName) and
				(@Severity is null or A.[Severity] = @Severity)
		end
	end
	else
	begin -- only return the last active alerts (relies on AlertID being sequentially assigned)
	
	if (@MaxRows is not null) -- -- SQLDM 8.5:Mahesh : Setting Maxrows for Active Alerts as well
			set rowcount @MaxRows

			SELECT A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message],I.CloudProviderName as 'ServerType'
				from #IntermediateTable I
					inner join Alerts A (nolock) on 
						A.[ServerName] = I.InstanceName and
						A.UTCOccurrenceDateTime = I.LastScheduledCollectionTime and
						A.Metric = I.Metric	
				where
					(A.Metric = 48 or I.IsMaintenanceMode = 0)
					and A.[Active] = 1
					and I.IsSnoozed = 0
					and A.Metric not in (select MetricID from DBMetrics)
					and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
					and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
					and (@TableName is null or A.[TableName] = @TableName) 
					and (@Severity is null or A.[Severity] = @Severity) 
		union	
			SELECT A.[AlertID],A.[UTCOccurrenceDateTime],A.[ServerName],A.[DatabaseName],A.[TableName],A.[Active],A.[Metric],A.[Severity],A.[StateEvent],A.[Value],A.[Heading],A.[Message],I.CloudProviderName as 'ServerType'
					from #IntermediateTable I (NOLOCK)
						inner join Alerts A (nolock) on 
							A.[ServerName] = I.InstanceName and
							A.UTCOccurrenceDateTime = I.LastDatabaseCollectionTime and
							A.Metric = I.Metric	
					where
						(A.Metric = 48 or I.IsMaintenanceMode = 0)
						and A.[Active] = 1
						and I.IsSnoozed = 0
						and A.Metric in (select MetricID from DBMetrics)
						and (@StartingAlertID is null or A.[AlertID] > @StartingAlertID) 
						and (@DatabaseName is null or A.[TableName] = @DatabaseName) 
						and (@TableName is null or A.[TableName] = @TableName) 
						and (@Severity is null or A.[Severity] = @Severity) 

	end

	SELECT @e = @@error
	RETURN @e
END	
 
