if (object_id('p_GetDiskSummary') is not null)
begin
drop procedure p_GetDiskSummary
end
go
CREATE PROCEDURE [dbo].[p_GetDiskSummary]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint
AS
BEGIN

-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years

	select
		m.InstanceName
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime]))) as [LastCollectioninInterval]
		,sum([IOActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [IOActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [IOActivityPercentage]
		,sum(convert(float,[WorkFilesCreated])) / nullif((sum(convert(float,case when [WorkFilesCreated] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as [WorkFilesCreated]
		,max([TempDBSizeInKilobytes]) as TempDBSizeInKilobytes
		,sum(convert(float,[PageReads])) / nullif((sum(convert(float,case when [PageReads] is not null then TimeDeltaInSeconds else 0 end))) ,0)as [PageReads]
		,sum(convert(float,[PageWrites])) / nullif((sum(convert(float,case when [PageWrites] is not null then TimeDeltaInSeconds else 0 end))) ,0)as [PageWrites]
		,sum(convert(float,[PageSplits])) / nullif((sum(convert(float,case when [PageSplits] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as [PageSplits]
		,sum(convert(float,[PageLookups])) / nullif((sum(convert(float,case when [PageLookups] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as [PageLookups]
		,sum(convert(float,[ReadAheadPages])) / nullif((sum(convert(float,case when [ReadAheadPages] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as [ReadAheadPages]
		,sum([DiskQueueLength] * TimeDeltaInSeconds) / nullif(sum(case when [DiskQueueLength] is not null then TimeDeltaInSeconds else 0 end),0) as [DiskQueueLength]
		,sum([DiskTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [DiskTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) as [DiskTimePercent]
		,DiskQueueLengthMean = (select TOP 1 Mean	--DiskQueueLength MetricID = 31
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 31 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),
		DiskTimePercentMean = (select TOP 1 Mean	--DiskTimePercent MetricID = 30
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 30 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),
		IOActivityPercentageMean = (select TOP 1 Mean	--IOActivityPercentage MetricID = -135
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -135 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),		
		WorkFilesCreatedMean = (select TOP 1 Mean --WorkFilesCreated -82
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -82 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc) * 60,
		TempDBSizeInKilobytesMean = (select TOP 1 Mean	--'%TempDBSizeInKilobytes%' -- -136
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -136 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),
		PageReadsMean = (select TOP 1 Mean	--'%PageReads%' -- -52
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -52 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),		
		PageWritesMean = (select TOP 1 Mean --'%PageWrites%' -- -1001
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1001 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc),				
		PageSplitsMean = (select TOP 1 Mean	--'%PageSplits%' -- -55
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -55 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc) * 60,
		PageLookupsMean = (select TOP 1 Mean	--'%PageLookups%' -- -50
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -50 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc) * 60,		
		ReadAheadPagesMean = (select TOP 1 Mean --'%ReadAheadPages%' -- -63
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -63 and dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, UTCCalculation)) = MAX(dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])))
				order by UTCCalculation desc) * 60
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] ss (nolock) on m.[SQLServerID] = ss.[SQLServerID]
		left join [OSStatistics] o (nolock)
		on o.[SQLServerID] = ss.[SQLServerID] and o.[UTCCollectionDateTime] = ss.[UTCCollectionDateTime]
	where
		ss.[SQLServerID] = @ServerID
		and dbo.fn_RoundDateTime(@Interval, ss.[UTCCollectionDateTime]) between @UTCStart and @UTCEnd
	group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) end
		,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) end
		,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) end
		,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime])) end
	order by dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ss.[UTCCollectionDateTime]))) 
END
 
