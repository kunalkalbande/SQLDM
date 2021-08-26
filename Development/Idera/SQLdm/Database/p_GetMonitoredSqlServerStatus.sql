if (object_id('p_GetMonitoredSqlServerStatus') is not null)
begin
drop procedure [p_GetMonitoredSqlServerStatus]
end
go

create procedure [p_GetMonitoredSqlServerStatus]
	(@SelectedServerID int = NULL)
as
begin
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

declare @SQLServerID int, @ThresholdEnabled bit, @WarningThresholdXML nvarchar(1024), @CriticalThresholdXML nvarchar(1024)
declare @ReorgThresholds table(SQLServerID int, WarningThreshold int, CriticalThreshold int) 
declare @WarningThresholdValue float, @CriticalThresholdValue float
declare @WarningThresholdEnabled bit, @CriticalThresholdEnabled bit
declare @xmlDoc int
declare @Now datetime

declare @Snoozers table(InstanceID int not null, NumberSnoozed int not null, NumberThresholds int not null)
declare @AlertSummary table(ServerName nvarchar(256) COLLATE SQL_Latin1_General_CP1_CS_AS not null, NumberWarning int not null, NumberCritical int not null, NumberInfo int not null)
declare @SQLServers table(
		SQLServerID int,
		InstanceName nvarchar(256),
		LastScheduledCollectionTime datetime,
		LastDatabaseCollectionTime datetime,
		IsMaintenanceMode bit ) 

--SQLDM-25650 fix (start)
--SQLdm 10.1 (Pulkit Puri)--sync of alert count of web ui count with api
--SQlDM-28022 - Changing the logic to fill the #TempAlert table
    create table #IntermediateTempTable (InstanceName nvarchar(255) collate SQL_Latin1_General_CP1_CS_AS , 
								     Metric int, 
									 LastScheduledCollectionTime datetime, 
									 LastDatabaseCollectionTime datetime, 
									 LastAlertRefreshTime datetime,
									 IsSnoozed bit,
									 IsDBNetric bit)
	create index IDX_TEMP ON #IntermediateTempTable (InstanceName, Metric)
	
	declare @current datetime
	Select @current= DateAdd(second,10,GetUTCDate())
	declare @starting datetime 
	select @starting = DATEADD(day, -1, GETUTCDATE())
	declare @ending datetime
	select @ending= @current
		
	-- create filtered table of selected servers and metrics to include
	insert into #IntermediateTempTable
	select
			MS.[InstanceName],
			M.Metric, 
			MS.LastScheduledCollectionTime, 
			MS.LastDatabaseCollectionTime,
			MS.LastAlertRefreshTime,
			case when T.UTCSnoozeEnd > @current then 1 else 0 end,
			case
				when DBM.MetricID is null then
					0
				else
					1
			end
		FROM MetricMetaData M (NOLOCK)
		cross join MonitoredSQLServers MS (NOLOCK)
		join MetricInfo as MI on M.Metric = MI.Metric
		left outer join MetricThresholds T (NOLOCK) on
				MS.[SQLServerID] = T.[SQLServerID] and
				M.[Metric] = T.[Metric] 	
		left outer join DBMetrics DBM (NOLOCK) on
				DBM.MetricID = M.[Metric]
			WHERE 
				MS.Active = 1 and M.Deleted = 0 and
				(T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @current)

create table #TempAlert(
		ServerName nvarchar(256)collate SQL_Latin1_General_CP1_CS_AS , 
		Severity tinyint
		 )
create index IDX_IMDT ON #TempAlert (ServerName, Severity)

--Insert the values from web console alerts into temp table
insert into #TempAlert 
		SELECT I.[InstanceName] AS ServerName,A.[Severity]				
				from #IntermediateTempTable I				
					inner join Alerts A (nolock) on 
						A.[ServerName] = I.InstanceName collate SQL_Latin1_General_CP1_CS_AS  and
						((A.UTCOccurrenceDateTime = I.LastScheduledCollectionTime and I.IsDBNetric = 0) or (A.UTCOccurrenceDateTime = I.LastDatabaseCollectionTime and I.IsDBNetric = 1)) and
						A.Metric = I.Metric
				where
					(A.Metric = 48 or A.[Active] = 1)
					and I.IsSnoozed = 0
					and (A.[UTCOccurrenceDateTime] between @starting and @ending)

drop table #IntermediateTempTable

 --SQLDM-25650 fix (end)
 
 
insert into @SQLServers
select SQLServerID, InstanceName, LastScheduledCollectionTime, LastDatabaseCollectionTime, 0
from MonitoredSQLServers MS (NOLOCK)
where 	
	MS.Active = 1

update @SQLServers
set IsMaintenanceMode = 1
from
@SQLServers S inner join Alerts A (NOLOCK) on A.ServerName collate database_default = S.InstanceName collate database_default 
where A.Metric = 48 and S.LastScheduledCollectionTime = A.UTCOccurrenceDateTime

INSERT INTO @AlertSummary
--  SQLDM-25650 FIX (start)--populating correct values in xml
select  ServerName, 
		sum(case when Severity = 4 then 1 else 0 end),
		sum(case when Severity = 8 then 1 else 0 end),
		sum(case when Severity = 2 then 1 else 0 end) 
		from #TempAlert
		group by ServerName
		
drop table #TempAlert
--  SQLDM-25650 FIX (end)
set @Now = GetUTCDate()

INSERT INTO @Snoozers
select MS.SQLServerID,
	sum(case when UTCSnoozeEnd > @Now then 1 else 0 end),
	count(MT.SQLServerID)
		from MonitoredSQLServers MS (nolock)
		left outer join MetricThresholds MT (nolock) on
		    MS.SQLServerID = MT.SQLServerID
where
   (MS.Active = 1) and
   (@SelectedServerID is null or @SelectedServerID = MS.SQLServerID) 
group by MS.SQLServerID


Select 1 as [Tag], NULL as [Parent],
	@Now as [Servers!1!Retrieved],
	NULL as [Server!2!SQLServerID],
	NULL as [Server!2!InstanceName],
	NULL as [Server!2!MaintenanceModeEnabled],
	NULL as [Server!2!ServerVersion],
	NULL as [Server!2!ServerEdition],
	NULL as [Server!2!LastScheduledCollectionTime],
	NULL as [Server!2!LastReorgStatisticsRunTime],
	NULL as [Server!2!LastAlertRefreshTime],
	NULL as [Server!2!LastDatabaseRefreshTime],
	NULL as [Server!2!ActiveWarningAlerts],
	NULL as [Server!2!ActiveCriticalAlerts],
	NULL as [Server!2!ActiveInfoAlerts],
    NULL as [Server!2!CustomCounterCount],
	NULL as [Server!2!ThresholdCount],
	NULL as [Server!2!AlertsSnoozing],
	NULL as [Category!3!Name],
	NULL as [State!4!Rank],
	NULL as [State!4!Metric],
	NULL as [State!4!Severity],
	NULL as [State!4!Subject],
	NULL as [State!4!OccurenceTime],
	NULL as [Database!5!Id],
	NULL as [Database!5!Name],
    NULL as [Database!5!IsSystemDatabase],
	NULL as [Table!6!Id],
	NULL as [Table!6!Schema],
	NULL as [Table!6!Name],
	NULL as [Table!6!Severity],
	NULL as [Table!6!IsSystemTable],
	NULL as [Table!6!Fragmentation]

UNION ALL
Select 2 as [Tag], 1 as [Parent], 
	NULL,
	SQLServerID,  
	InstanceName,
	MaintenanceModeEnabled,
	ServerVersion,
	ServerEdition,
	LastScheduledCollectionTime,
	LastReorgStatisticsRunTime,
	LastAlertRefreshTime,
	LastDatabaseCollectionTime,
	COALESCE(NumberWarning,0),
	COALESCE(NumberCritical,0),
	COALESCE(NumberInfo,0),
	-- count of assigned custom counters (direct assign or from tags)
	(select count(distinct CCD.Metric) from [CustomCounterDefinition] CCD (nolock)
		LEFT OUTER JOIN CustomCounterMap CCM (nolock) on CCM.SQLServerID = [MonitoredSQLServers].[SQLServerID] and CCM.Metric = CCD.Metric
		LEFT OUTER JOIN ServerTags ST (nolock) on ST.[SQLServerId] = [MonitoredSQLServers].[SQLServerID]
		LEFT OUTER JOIN CustomCounterTags CCT (nolock) on CCT.Metric = CCD.Metric and ST.TagId = CCT.TagId
		WHERE CCM.Metric is not null or CCT.Metric is not null),
	[NumberThresholds],
	[NumberSnoozed],
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
from MonitoredSQLServers (nolock)
	left outer join @AlertSummary on ( InstanceName  = ServerName or FriendlyServerName = ServerName)
	left outer join @Snoozers on MonitoredSQLServers.SQLServerID = InstanceID
	where (Active = 1) and (@SelectedServerID is null or @SelectedServerID = MonitoredSQLServers.SQLServerID)

UNION ALL
Select distinct 3 as [Tag], 2 as [Parent],
	NULL,
	SQLServerID,
	InstanceName,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	Category, 
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
from MonitoredSQLServers (nolock), MetricInfo (nolock)
	where (Active = 1) and
		  (@SelectedServerID is null or @SelectedServerID = SQLServerID)

UNION ALL
Select 4 as [Tag], 3 as [Parent],
	NULL,
	MS.SQLServerID,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	MI.Category,
	MI.[Rank],
	A.Metric,
	A.Severity,
	A.Heading,
	A.UTCOccurrenceDateTime,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
From @SQLServers MS 
	inner join Alerts A (nolock) on 
		MS.InstanceName = A.ServerName
		And (MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
			Or MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
	inner join MetricInfo MI (NOLOCK) on
		A.Metric = MI.Metric
	left outer join [MetricThresholds] MT (NOLOCK) on 
		MS.SQLServerID = MT.SQLServerID and
		A.Metric = MT.Metric
where 
	(@SelectedServerID is null or @SelectedServerID = MS.SQLServerID) and
	(MT.UTCSnoozeEnd is null or MT.UTCSnoozeEnd <= @Now)
	and (
			(
			IsMaintenanceMode = 1 and
			(A.Metric = 48 and A.Metric = 48 and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
			)
			or
			(
			IsMaintenanceMode = 0 and
				(
				(A.Metric not in (select MetricID from DBMetrics (NOLOCK)) and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
				or (A.Metric  in (select MetricID from DBMetrics (NOLOCK)) and MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
				)
			)
		)

UNION ALL
Select distinct 5 as [Tag], 3 as [Parent],
	NULL,
	MS.SQLServerID,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	MI.Category,
	NULL, --MI.Rank,
	NULL, --A.Metric,
	NULL, --A.Severity,
	NULL, --A.Heading,
	NULL, --A.UTCOccurrenceDateTime,
	DB.DatabaseID,
	A.DatabaseName,
	DB.SystemDatabase,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL
From @SQLServers MS 
	inner join Alerts A (nolock) on 
		MS.InstanceName = A.ServerName
		And (MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
			Or MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
	inner join MetricInfo MI (nolock) on
		A.Metric = MI.Metric
	left outer join [MetricThresholds] MT (NOLOCK) on 
		MS.SQLServerID = MT.SQLServerID and
		A.Metric = MT.Metric
	left outer join SQLServerDatabaseNames DB (nolock) on
		MS.SQLServerID = DB.SQLServerID and 
		A.DatabaseName = DB.DatabaseName
where (A.DatabaseName is not null) and
	  (DB.IsDeleted = 0) -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 	
       and (@SelectedServerID is null or @SelectedServerID = MS.SQLServerID) and
	  (MT.UTCSnoozeEnd is null or MT.UTCSnoozeEnd <= @Now)
	   and (
			(
			IsMaintenanceMode = 1 and
			(A.Metric = 48 and A.Metric = 48 and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
			)
			or
			(
			IsMaintenanceMode = 0 and
				(
				(A.Metric not in (select MetricID from DBMetrics (NOLOCK)) and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
				or (A.Metric  in (select MetricID from DBMetrics (NOLOCK)) and MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
				)
			)
		)


-- UNION ALL
--Select 6 as [Tag], 5 as [Parent],
--	NULL,
--	OE.ServerID,
--	NULL,
--	NULL,
--	NULL,
--	NULL,
--	NULL,
--	NULL,
--	NULL,
--	MI.Category,
--	MI.Rank,
--	OE.Metric,
--	NULL,
--	NULL,
--	OE.OccurenceTime,
--	OE.DatabaseID,
--	DB.DatabaseName,
--	NULL,
--	TR.TableID,
--	TB.SchemaName,
--	TB.TableName,
--	case 
--		when convert(int,100 - TR.ScanDensity) >= RT.CriticalThreshold then 4 
--		when convert(int,100 - TR.ScanDensity) >= RT.WarningThreshold then 2 
--		else 1 
--	end,
--	TB.SystemTable,
--	convert(int,100 - TR.ScanDensity)
--From OutstandingEvents OE (nolock)
--	left join  MetricInfo MI (nolock) on OE.Metric = MI.Metric
--	left join SQLServerDatabaseNames DB (nolock)
--		on DB.SQLServerID = OE.ServerID and
--	       DB.DatabaseID = OE.DatabaseID
--	left join SQLServerTableNames TB (nolock)
--		on TB.DatabaseID = DB.DatabaseID 
--	left join MonitoredSQLServers MS (nolock)
--		on DB.SQLServerID = MS.SQLServerID
--	left outer join @ReorgThresholds RT 
--		on RT.SQLServerID = OE.ServerID	
--	left outer join TableReorganization TR (nolock)
--		on TR.TableID = TB.TableID and
--		   TR.UTCCollectionDateTime = MS.LastReorgStatisticsRunTimeUTC
--where 
--  	  (@SelectedServerID is null or @SelectedServerID = OE.ServerID) and
--	  OE.DatabaseID != -1 and 
--	  OE.Metric = 7 and 
--	  TR.TableID is not null and
--	  TR.ScanDensity < 100 and
--	  (100 - TR.ScanDensity) >= RT.WarningThreshold 

UNION ALL
Select 4 as [Tag], 5 as [Parent],
	NULL,
	MS.SQLServerID,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	MI.Category,
	MI.[Rank],
	A.Metric,
	A.Severity,
	A.Heading,
	A.UTCOccurrenceDateTime,
	DB.DatabaseID,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL,
	NULL 
From @SQLServers MS 
	inner join Alerts A (nolock) on 
		MS.InstanceName = A.ServerName 
		And (MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
			Or MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
	inner join MetricInfo MI (nolock) on
		A.Metric = MI.Metric
	left outer join [MetricThresholds] MT (NOLOCK) on 
		MS.SQLServerID = MT.SQLServerID and
		A.Metric = MT.Metric
	left outer join SQLServerDatabaseNames DB (nolock) on
		MS.SQLServerID = DB.SQLServerID and 
		A.DatabaseName = DB.DatabaseName
where 
	  (@SelectedServerID is null or @SelectedServerID = MS.SQLServerID) and
	   (DB.IsDeleted = 0) -- SQLdm kit1 (Barkha Khatri) DE44508 fix -using only current databases 	
	  and(A.DatabaseName is not null) and
	  (MT.UTCSnoozeEnd is null or MT.UTCSnoozeEnd <= @Now)
	  and (
			(
			IsMaintenanceMode = 1 and
			(A.Metric = 48 and A.Metric = 48 and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
			)
			or
			(
			IsMaintenanceMode = 0 and
				(
				(A.Metric not in (select MetricID from DBMetrics (NOLOCK) ) and MS.LastScheduledCollectionTime = A.UTCOccurrenceDateTime)
				or (A.Metric  in (select MetricID from DBMetrics (NOLOCK) ) and MS.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
				)
			)
		)

order by [Server!2!SQLServerID],
		 [Category!3!Name],
		 [Database!5!Id],
		 [Table!6!Id],
		 [Parent],[Tag],
		 [State!4!Severity] DESC,
		 [State!4!Rank],
		 [State!4!OccurenceTime] DESC
		
FOR XML EXPLICIT

end