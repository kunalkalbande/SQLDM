if (object_id('p_GetServerInventory') is not null)
begin
drop procedure [p_GetServerInventory]
end
go
CREATE procedure [dbo].[p_GetServerInventory]
				@ServerName nvarchar(256) = null,
				@ServerVersion nvarchar(30) = null,
				@OSVersion nvarchar(256) = null,
				@NumberOfProcessors int = null,
				@PhysicalMemory bigint = null,
				@Clustered bit = null,
				@Owner nvarchar(256) = null,
				@TagId int = null,
				@SQLServerIDs nvarchar(max) = null
as
begin
	declare @xmlDoc int

	declare @SQLServers table(SQLServerID int, InstanceName nvarchar(256), UTCCollectionDateTime datetime NULL) 

	if @NumberOfProcessors < 1
	 set @NumberOfProcessors = null

	if @PhysicalMemory < 1
	 set @PhysicalMemory = null

	if len(@ServerName) < 1
	 set @ServerName = null

	if len(@ServerVersion) < 1
	 set @ServerVersion = null

	if len(@OSVersion) < 1
	 set @OSVersion = null

	if len(@Owner) < 1
	 set @Owner = null

	if @TagId < 1
		set @TagId = null
		
		
	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]		

	if @SQLServerIDs is not null 
	Begin
		-- Prepare XML document if there is one
		exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

		insert into @SQLServers
		select ID, [InstanceName], (select max(ss.[UTCCollectionDateTime]) from ServerStatistics ss (nolock) where ss.SQLServerID = ms.SQLServerID)
		from openxml(@xmlDoc, '//Srvr', 1) with (ID int)
			join #SecureMonitoredSQLServers ms (nolock) on ID = SQLServerID
		where lower(ms.InstanceName) like lower(coalesce(@ServerName, ms.InstanceName)) and
			  (@TagId is null or SQLServerID in (select SQLServerId from ServerTags (nolock) where TagId = @TagId))

		exec sp_xml_removedocument @xmlDoc
	end
	else 
	begin
		insert into @SQLServers
		select ms.SQLServerID, ms.InstanceName, (select max(ss.[UTCCollectionDateTime]) from ServerStatistics ss (nolock) where ss.SQLServerID = ms.SQLServerID)
		from #SecureMonitoredSQLServers ms (nolock)
		where lower(ms.InstanceName) collate database_default like lower(coalesce(@ServerName, ms.InstanceName)) collate database_default and
			  (@TagId is null or ms.SQLServerID in (select SQLServerId from ServerTags (nolock) where TagId = @TagId))
	end

	select	ms.InstanceName 
			,max(ss.SQLServerID) as SQLServerID
			,max(ss.ServerVersion) as ServerVersion
			,max(WindowsVersion) as OSVersion
			,max(ProcessorCount) as ProcessorCount
			,IsClustered
			--,Owner
			--,ServerTag
			,max(PhysicalMemoryInKilobytes) as PhysicalMemoryKB
	from @SQLServers ms 
		 left join ServerStatistics ss (nolock) on  ss.SQLServerID = ms.SQLServerID 
												and ss.UTCCollectionDateTime = ms.UTCCollectionDateTime
	where 
		lower(WindowsVersion) like lower(coalesce(@OSVersion, WindowsVersion))
		and [dbo].[fn_ServerVersionnVarcharToBigInt](ss.ServerVersion) <= coalesce([dbo].[fn_ServerVersionnVarcharToBigInt](@ServerVersion), [dbo].[fn_ServerVersionnVarcharToBigInt](ss.ServerVersion))
		and ProcessorCount <= coalesce(@NumberOfProcessors, ProcessorCount)
		and IsClustered = coalesce(@Clustered, IsClustered)
		and PhysicalMemoryInKilobytes <= coalesce(@PhysicalMemory, PhysicalMemoryInKilobytes)
	group by InstanceName, IsClustered
end