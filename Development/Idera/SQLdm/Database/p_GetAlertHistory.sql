if (object_id('p_GetAlertHistory') is not null)
begin
drop procedure [p_GetAlertHistory]
end
go
create procedure [dbo].[p_GetAlertHistory]
				@SQLServerIDs nvarchar(max) = null,
				@UTCStart DateTime,
				@UTCEnd DateTime,
				@UTCOffset int
as
begin
		declare @xmlDoc int
		declare @SQLServers table(
					SQLServerID int,
					InstanceName nvarchar(256)) 

		create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

		insert into #SecureMonitoredSQLServers
		exec [p_GetReportServers]
		

		if @SQLServerIDs is not null 
		begin
			-- Prepare XML document if there is one
			exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

			insert into @SQLServers
			select
				ID, InstanceName
			from openxml(@xmlDoc, '//Srvr', 1) with (ID int)
				join #SecureMonitoredSQLServers (nolock) on ID = SQLServerID

			if @SQLServerIDs is not null
				exec sp_xml_removedocument @xmlDoc
		end
		else
		begin
			insert into @SQLServers
			select SQLServerID, InstanceName 
				from #SecureMonitoredSQLServers (nolock)
		end

		select	ms.InstanceName
				,ms.SQLServerID as SQLServerID
				,a.Severity as Status
				,dateadd(mi, @UTCOffset, a.UTCOccurrenceDateTime) as Occurred
				,a.Heading as Title
		from @SQLServers ms 
			join Alerts a (NOLOCK) on a.[ServerName] collate database_default = ms.[InstanceName] collate database_default
		where dateadd(minute, datediff(minute, 0, a.[UTCOccurrenceDateTime]), 0) between @UTCStart and @UTCEnd
		order by
			ms.InstanceName,
			Occurred asc
end