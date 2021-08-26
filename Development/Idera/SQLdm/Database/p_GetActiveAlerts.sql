if (object_id('p_GetActiveAlerts') is not null)
begin
drop procedure [p_GetActiveAlerts]
end
go
create procedure [dbo].[p_GetActiveAlerts]
				@SQLServerIDs nvarchar(max) = null
as
begin
	declare @xmlDoc int
	declare @now datetime

	select @now = DateAdd(second,10,GetUTCDate())

	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]
	

		declare @SQLServers table(
				SQLServerID int,
				InstanceName nvarchar(256),
				LastScheduledCollectionTime datetime,
				LastDatabaseCollectionTime datetime,
				IsMaintenanceMode bit ) 

		if @SQLServerIDs is not null 
		begin
			exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

			insert into @SQLServers
				select ID, smss.InstanceName, LastScheduledCollectionTime, LastDatabaseCollectionTime,0
				from openxml(@xmlDoc, '//Srvr', 1) with (ID int)
					join MonitoredSQLServers mss (nolock) on mss.SQLServerID = ID
					inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID

			exec sp_xml_removedocument @xmlDoc
		end
		else
		begin
			insert into @SQLServers
				select smss.SQLServerID, smss.InstanceName, LastScheduledCollectionTime, LastDatabaseCollectionTime,0
				from MonitoredSQLServers mss (nolock)
					inner join #SecureMonitoredSQLServers smss on mss.SQLServerID = smss.SQLServerID
				where Active = 1
		end
		
		
		update @SQLServers
		set IsMaintenanceMode = 1
		from
		@SQLServers S inner join Alerts A (NOLOCK) on A.ServerName collate database_default = S.InstanceName collate database_default 
		where A.Metric = 48 and S.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
			
		
		select	distinct A.ServerName as InstanceName
				,A.Severity as Status
				,A.Heading as Title
				,A.Message as Description
				,S.SQLServerID
		from 
			@SQLServers S
			left join Alerts A (NOLOCK) on A.ServerName collate database_default = S.InstanceName collate database_default 
			And (S.LastScheduledCollectionTime = A.UTCOccurrenceDateTime
			Or S.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)
			left join MetricThresholds T (nolock) on
				S.[SQLServerID] = T.[SQLServerID] and
				A.[Metric] = T.[Metric] 	
		where 
		(S.IsMaintenanceMode = 0 or A.Metric = 48)
		and (T.[UTCSnoozeEnd] is null or T.[UTCSnoozeEnd] < @now)	
		and (
			((A.Metric not in (select MetricID from DBMetrics) and S.LastScheduledCollectionTime = A.UTCOccurrenceDateTime) -- Add in scheduled refresh
			or (A.Metric  in (select MetricID from DBMetrics) and S.LastDatabaseCollectionTime = A.UTCOccurrenceDateTime)) -- Add in database refresh
			) 

end