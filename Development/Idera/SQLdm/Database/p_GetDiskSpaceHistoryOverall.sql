-- SQLdm 9.1 (Ankit Srivastava)

-- Filegroup and Mountpoint Monitoring Improvements
-- retrieves the overall data for the Disk Space History report

 --exec p_GetDiskSpaceHistoryOverall  @SQLServerID = 37,
	--					         	     @DiskNameFilter = 'C',
	--								     @UTCStart = '2015-02-19 00:00:00.000',
 --                                      @UTCEnd = '2015-02-25 00:00:00.000',
 --                                      @UTCOffset = 0

if (object_id('p_GetDiskSpaceHistoryOverall') is not null)
begin
	drop procedure [p_GetDiskSpaceHistoryOverall]
end
go

create procedure [dbo].[p_GetDiskSpaceHistoryOverall]
	@SQLServerID int,
	@DiskNameFilter nvarchar(256),
	@UTCStart dateTime,
	@UTCEnd dateTime,
	@UTCOffset int = 0
as
begin
	create table #FileStats(
				SQLServerID int,
				UTCCollectionDateTime datetime,
				DriveName nvarchar(256),
				SQLDataUsedMB decimal(18,0),
				SQLDataFreeMB decimal(18,0),
				SQLLogFileMB decimal(18,0),
				TotalInitialSizeMB decimal(18,0) );

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
								where
									SSDN.SQLServerID = @SQLServerID and DFS.DriveName = @DiskNameFilter 
									and dbo.fn_RoundDateTime(0, [UTCCollectionDateTime]) between @UTCStart and @UTCEnd
								group by
									SSDN.SQLServerID, DFS.UTCCollectionDateTime, DFS.DriveName, DF.FileGroupName ) as FST
								group by
									FST.SQLServerID, FST.UTCCollectionDateTime, FST.DriveName

	select
		[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])),
		DDS.DriveName DiskName,
		ISNULL(max(FS.SQLDataUsedMB ),0)  SQLDataUsedMB,
		ISNULL(max(FS.SQLDataFreeMB ),0)  SQLDataFreeMB,
		ISNULL(max(FS.SQLLogFileMB ),0)  SQLLogFileMB,
		ISNULL(max(((DDS.TotalSizeKB - DDS.UnusedSizeKB)/1024 - TotalInitialSizeMB)),0)  as NonSQLDiskUsageMB,
		ISNULL(max(DDS.UnusedSizeKB/1024),0)  as FreeDiskSpaceMB,
		ISNULL(avg(DDS.DiskReadsPerSecond),0)  DiskReadsPerSecond,
		ISNULL(avg(DDS.DiskWritesPerSecond),0)  DiskWritesPerSecond
	from 
		#DDS as DDS
		left join
		#FileStats as FS
		on (DDS.DriveName COLLATE DATABASE_DEFAULT = FS.DriveName COLLATE DATABASE_DEFAULT and DDS.SQLServerID = FS.SQLServerID and DDS.UTCCollectionDateTime = FS.UTCCollectionDateTime)
	where
		DDS.SQLServerID = @SQLServerID 
		and 
		DDS.DriveName = @DiskNameFilter
		and
		 DDS.UTCCollectionDateTime between @UTCStart and @UTCEnd
	Group by
	DDS.DriveName
	drop table #FileStats
	drop table #DFS
	drop table #DDS

end
go
