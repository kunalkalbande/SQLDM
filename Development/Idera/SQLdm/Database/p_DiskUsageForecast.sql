if (object_id('p_DiskUsageForecast') is not null)
begin
drop procedure p_DiskUsageForecast
end
go
CREATE PROCEDURE [dbo].[p_DiskUsageForecast]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint,
		@DriveName nvarchar(256) = null
AS
BEGIN
	select	
		count(*) as [Records],
		dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime]))) as [LastCollectioninInterval],
		dd.DriveName,
		max((dd.TotalSizeKB - dd.UnusedSizeKB) / 1024) /1024 as TotalUsedGB,
		max((dd.TotalSizeKB / 1024)) / 1024 as TotalSizeGB
	from DiskDrives dd (nolock)
	where dd.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
		and @ServerID = dd.SQLServerID
		and dd.DriveName IN (SELECT Item FROM dbo.SplitString(@DriveName, ','))
		and dd.DriveName <> 'No Drives Configured'
	group by
			dd.DriveName
			-- Always group by year at the least
			,datepart(yy, dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime]))
			-- Group by all intervals greater than or equal to the selected interval
			,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
			,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, dd.[UTCCollectionDateTime])) end
		order by
			dd.DriveName
			,max(dd.[UTCCollectionDateTime])
END
 
