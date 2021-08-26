-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup and Mountpoint Monitoring Improvements
-- retrieves the data for the Disk Space Usage report

 --exec p_GetDiskSpaceUsageReportData   @SQLServerIDs = '<Srvrs><Srvr ID="37"/><Srvr ID="41"/></Srvrs>',
	--									@SortBy = 'UnusedSizeKB',
	--									@SortOrder = 'DESC'

if (object_id('p_GetDiskSpaceUsageReportData') is not null)
begin
	drop procedure [p_GetDiskSpaceUsageReportData]
end
go
--SQLDM-26805 - Code changes to exclude the disks in the disk space usage report
create procedure [dbo].[p_GetDiskSpaceUsageReportData]
	@SQLServerIDs nvarchar(max) = null,
	@DiskNameFilter nvarchar(10),
	@SortBy nvarchar(50) = 'InstanceName',
	@SortOrder nvarchar(10) = 'ASC'
as
begin
	declare @Query nvarchar(max),
			@ParameterDefinition nvarchar(4000);

	create table #SQLServersCollectionTime(
				SQLServerID int,
				UTCCollectionDateTime datetime );

	create table #FileStats(
				SQLServerID int,
				UTCCollectionDateTime datetime,
				DriveName nvarchar(256),
				SQLDataUsedMB decimal(18,0),
				SQLDataFreeMB decimal(18,0),
				SQLLogFileMB decimal(18,0),
				TotalInitialSizeMB decimal(18,0) );

	create table #DiskIO (
				SQLServerID int,
				DriveName nvarchar(256),
				AvgDiskReadsPerSec int,                -- last 24 hours reads/writes from most recent collection time
				AvgDiskWritesPerSec int );

	create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	SELECT * INTO #DFS FROM 
	(
		SELECT FileID,
			UTCCollectionDateTime,
			DriveName,
			UsedSpace,
			AvailableSpace,
			InitialSize
		FROM DatabaseFileStatistics
		
		UNION ALL 
		
		SELECT FileID,
			MaxUTCCollectionDateTime AS UTCCollectionDateTime,
			DriveName,
			TotalUsedSpace AS UsedSpace,
			TotalAvailableSpace AS AvailableSpace,
			TotalInitialSize AS InitialSize
		FROM DatabaseFileStatisticsAggregation
	) AS DFSAggregated;

	SELECT * INTO #DDS FROM 
	(
		SELECT SQLServerID,
			UTCCollectionDateTime,
			DriveName,
			TotalSizeKB,
			UnusedSizeKB,
			DiskReadsPerSecond,
			DiskWritesPerSecond
		FROM DiskDriveStatistics
		
		UNION ALL 
		
		SELECT SQLServerID AS SQLServerID,
			MaxUTCCollectionDateTime as UTCCollectionDateTime,
			DriveName,
			TotalSizeKBPerDay AS TotalSizeKB,
			TotalUnusedSizeKB AS UnusedSizeKB,
			TotalDiskReadsPerSecond AS DiskReadsPerSecond,
			TotalDiskWritesPerSecond AS DiskWritesPerSecond
		FROM DiskDriveStatisticsAggregation
	) AS DDSAggregated;

	
	insert into #SecureMonitoredSQLServers
	exec [p_GetReportServers]
				
	select @ParameterDefinition = ' @SortBy nvarchar(50),
									@SortOrder nvarchar(10) ';
	if @SQLServerIDs is not null 
		begin
			declare @xmlDoc int
			exec sp_xml_preparedocument @xmlDoc output, @SQLServerIDs
									
			insert into #SQLServersCollectionTime select dds.SQLServerID, max(UTCCollectionDateTime) from 
										  openxml(@xmlDoc, '//Srvr', 1) with (ID int)
											join DiskDriveStatistics dds (nolock) on dds.SQLServerID = ID
											inner join #SecureMonitoredSQLServers smss on dds.SQLServerID = smss.SQLServerID
										  group by dds.SQLServerID;
			exec sp_xml_removedocument @xmlDoc
		end
	else
		begin
			insert into #SQLServersCollectionTime select dds.SQLServerID, max(UTCCollectionDateTime) from DiskDriveStatistics dds
			inner join #SecureMonitoredSQLServers smss on dds.SQLServerID = smss.SQLServerID
										  group by dds.SQLServerID;
		end

			insert into #FileStats select
							  FST.SQLServerID, 
							  FST.UTCCollectionDateTime, 
							  FST.DriveName, 
							  sum(FST.SQLDataUsedMB) as SQLDataUsedMB, 
							  sum(FST.SQLDataFreeMB) as SQLDataFreeMB, 
							  sum(FST.SQLLogFileMB) as SQLLogFileMB,
							  sum(FST.TotalInitialSize) as TotalInitialSizeMB
							from (
								select 
									SSDN.SQLServerID, 
									DFS.UTCCollectionDateTime,
									DFS.DriveName,
									SQLDataUsedMB = (case when DF.FileGroupName <> 'n/a' then sum(DFS.UsedSpace) else 0 end),
									SQLDataFreeMB = (case when DF.FileGroupName <> 'n/a' then sum(DFS.AvailableSpace) else 0 end),
									SQLLogFileMB = (case when DF.FileGroupName = 'n/a' then sum(DFS.InitialSize) else 0 end),
									sum(DFS.InitialSize) as TotalInitialSize
								from  
									#DFS as DFS
									inner join 
									DatabaseFiles as DF 
									on DF.FileID = DFS.FileID
									inner join 
									SQLServerDatabaseNames as SSDN 
									on SSDN.DatabaseID = DF.DatabaseID
									inner join
									#SQLServersCollectionTime as SSCT
									on (SSCT.SQLServerID = SSDN.SQLServerID and DFS.UTCCollectionDateTime = SSCT.UTCCollectionDateTime)
								group by
									SSDN.SQLServerID, DFS.UTCCollectionDateTime, DFS.DriveName, DF.FileGroupName) as FST
								group by
									FST.SQLServerID, FST.UTCCollectionDateTime, FST.DriveName
								
	insert into #DiskIO select 
							DDS.SQLServerID,
							DDS.DriveName,
							avg(DDS.DiskReadsPerSecond) as AvgDiskReadsPerSec,
							avg(DDS.DiskWritesPerSecond) as AvgDiskWritesPerSec
						from 
							#DDS as DDS
							left join
							#SQLServersCollectionTime as SSCT
							on (SSCT.SQLServerID = DDS.SQLServerID and SSCT.UTCCollectionDateTime = DDS.UTCCollectionDateTime)
						where
							DDS.UTCCollectionDateTime between dateadd(hour,-24,SSCT.UTCCollectionDateTime) and SSCT.UTCCollectionDateTime
						group by 
							DDS.SQLServerID, DDS.DriveName

	select @Query = '  select
							MSS.SQLServerID,
							MSS.InstanceName,
							DDS.DriveName as DiskName,
							ISNULL(DDS.TotalSizeKB/1024,0) TotalSizeMB,
							ISNULL(FS.SQLDataUsedMB,0) SQLDataUsedMB,
							ISNULL(FS.SQLDataFreeMB,0) SQLDataFreeMB,
							ISNULL(SQLLogFileMB,0) SQLLogFileMB,
							ISNULL((((DDS.TotalSizeKB  - DDS.UnusedSizeKB)/1024) - TotalInitialSizeMB),0) as NonSQLDiskUsageMB,
							ISNULL(DDS.UnusedSizeKB/1024,0) as FreeDiskSpaceMB,
							ISNULL(DIO.AvgDiskReadsPerSec,0) AvgDiskReadsPerSec,
							ISNULL(DIO.AvgDiskWritesPerSec,0) AvgDiskWritesPerSec
					   from
						    #DDS as DDS
						    inner join
							MonitoredSQLServers as MSS
							on DDS.SQLServerID = MSS.SQLServerID
							inner join
							#SQLServersCollectionTime as SSCT
							on (SSCT.SQLServerID = DDS.SQLServerID and SSCT.UTCCollectionDateTime = DDS.UTCCollectionDateTime)
							left join 
							#FileStats as FS
							on (FS.DriveName COLLATE DATABASE_DEFAULT = DDS.DriveName COLLATE DATABASE_DEFAULT and DDS.SQLServerID = FS.SQLServerID and FS.UTCCollectionDateTime = DDS.UTCCollectionDateTime)
							left join
							#DiskIO as DIO
							on (DIO.DriveName COLLATE DATABASE_DEFAULT = DDS.DriveName COLLATE DATABASE_DEFAULT and DDS.SQLServerID = DIO.SQLServerID)
					   where
						DDS.DriveName NOT IN (SELECT Value FROM fn_Split('''+ @DiskNameFilter + ''','',''))';

	--if @SQLServerID is not null
	--	select @Query = @Query + ' and DDS.SQLServerID = @SQLServerID ';

	select @Query = @Query + ' order by ' + @SortBy + ' ' + @SortOrder;

	execute sp_executesql @Query, 
                          @ParameterDefinition, 
						  --@SQLServerID,
						  @SortBy,
						  @SortOrder

	drop table #SQLServersCollectionTime
	drop table #FileStats
	drop table #DiskIO
	drop table #DFS
	drop table #DDS
end

