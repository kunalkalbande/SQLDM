if (object_id('p_GetMostCriticalServers') is not null)
begin
drop procedure [p_GetMostCriticalServers]
end
go

create procedure [dbo].[p_GetMostCriticalServers]
as
begin
	declare @now datetime

	declare @IntermediateTable table(
		SQLServerID int,
		InstanceName nvarchar(255) collate database_default,      
		Metric int, 
		LastAlertRefreshTime datetime, 
		IsSnoozed bit,
		[Rank] int)

	select @now = DateAdd(second,10,GetUTCDate())

	insert into @IntermediateTable
	select      
	    MS.SQLServerID,
		MS.InstanceName,
		M.Metric, 
		MS.LastAlertRefreshTime, 
		case when T.UTCSnoozeEnd > @now then 1 else 0 end,
		M.[Rank]
	from MonitoredSQLServers MS (nolock)
	join MetricThresholds T (nolock) on MS.SQLServerID = T.SQLServerID
	join MetricMetaData M (nolock) on T.Metric = M.Metric
	WHERE 
		MS.Active = 1 and M.Deleted = 0 and
	    (T.Enabled = 1 or T.Enabled is null) and
		(T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @now)  


	SELECT 
		I.SQLServerID
		,InstanceName=MAX(I.InstanceName)
		,AlertCount=COUNT(*)
		,AlertScore=SUM(I.[Rank] * A.[Severity]) 
	from 
		@IntermediateTable I
		inner join Alerts A (nolock) on 
			A.[ServerName] = I.InstanceName and
			A.UTCOccurrenceDateTime = I.LastAlertRefreshTime and
			A.Metric = I.Metric     
	where
		A.[Active] = 1
		and I.IsSnoozed = 0
	GROUP BY I.SQLServerID
	ORDER BY 4 DESC, 3 DESC, 1 ASC
end