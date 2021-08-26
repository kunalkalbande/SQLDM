-- SQLdm 9.1 (Abhishek Joshi)

-- Filegroup and Mountpoint Monitoring Improvements
-- retrieves the data for the Disk Space History report

 --exec p_GetDiskSpaceHistoryReportData  @SQLServerID = 37,
	--					         	   @DiskNameFilter = 'C',
	--								   @UTCStart = '2015-02-19 00:00:00.000',
 --                                      @UTCEnd = '2015-03-25 00:00:00.000',
 --                                      @UTCOffset = 0,  
 --                                      @Interval = 1

if (object_id('p_GetDiskSpaceHistoryReportData') is not null)
begin
	drop procedure [p_GetDiskSpaceHistoryReportData]
end
go

create procedure [dbo].[p_GetDiskSpaceHistoryReportData]
	@SQLServerID int,
	@DiskNameFilter nvarchar(256),
	@UTCStart dateTime,
	@UTCEnd dateTime,
	@UTCOffset int = 0,
	@Interval int
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
								group by
									SSDN.SQLServerID, DFS.UTCCollectionDateTime, DFS.DriveName, DF.FileGroupName ) as FST
								group by
									FST.SQLServerID, FST.UTCCollectionDateTime, FST.DriveName

	Select 
			[LastCollectioninInterval] = dbo.fn_RoundDateTime(@Interval, LastCollectioninInterval),
			[DiskName],
			IntervalNumber = dense_rank() Over (Order By IntervalTemp),
			AverageTotalSizeMB,
			AverageSQLDataUsedMB,
			AverageSQLDataFreeMB,
			AverageSQLLogFileMB,
			AverageNonSQLDiskUsageMB,
			AverageFreeDiskSpaceMB,
			AverageDiskReadsPerSecond,
			AverageDiskWritesPerSecond
	FROM
	(
	select
		[LastCollectioninInterval] = max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])),
		DDS.DriveName as DiskName,
		IntervalTemp = case
				 when isnull(@Interval,5) = 5
						then 0
				 when isnull(@Interval,5) = 4
						then datediff(yyyy, @UTCStart, max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 3
						then datediff(mm, @UTCStart, max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 2
						then datediff(dd, @UTCStart, max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) = 1
						then datediff(hh, @UTCStart, max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])))
				 when isnull(@Interval,5) =  0
						then datediff(mi, @UTCStart, max(dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])))
				 end,
		ISNULL(avg(DDS.TotalSizeKB/1024),0) as AverageTotalSizeMB,
		ISNULL(avg(FS.SQLDataUsedMB),0) as AverageSQLDataUsedMB,
		ISNULL(avg(FS.SQLDataFreeMB),0) as AverageSQLDataFreeMB,
		ISNULL(avg(FS.SQLLogFileMB),0) as AverageSQLLogFileMB,
		ISNULL(avg(((DDS.TotalSizeKB - DDS.UnusedSizeKB)/1024) - TotalInitialSizeMB),0) as AverageNonSQLDiskUsageMB,
		ISNULL(avg(DDS.UnusedSizeKB/1024),0) as AverageFreeDiskSpaceMB,
		ISNULL(avg(DDS.DiskReadsPerSecond),0) as AverageDiskReadsPerSecond,
		ISNULL(avg(DDS.DiskWritesPerSecond),0) as AverageDiskWritesPerSecond
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
		dbo.fn_RoundDateTime(@Interval, DDS.UTCCollectionDateTime) between @UTCStart and @UTCEnd
	group by
		DDS.DriveName, case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else 1 end
	,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, DDS.[UTCCollectionDateTime])) end
		) as Result

	drop table #FileStats
	drop table #DFS
	drop table #DDS

end
go
