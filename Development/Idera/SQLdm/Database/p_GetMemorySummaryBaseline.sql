if (object_id('p_GetMemorySummaryBaseline') is not null)
begin
drop procedure [p_GetMemorySummaryBaseline]
end
go

CREATE PROCEDURE [dbo].[p_GetMemorySummaryBaseline]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint
AS
BEGIN
		SELECT		
		UTCCalculation = dbo.fn_RoundDateTime(1, max(dateadd(mi, @UTCOffset, UTCCalculation))),
		PageLifeExpectancyMean = (select TOP 1 Mean	--'%PageLifeExpectancy%' -- 76
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 76 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		ProcedureCacheHitRatioPercentageMean = (select TOP 1 Mean	--'%ProcedureCacheHitRatioPercentage%' -- 81
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 81 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		OSTotalPhysicalMemoryInKilobytesMean = (select TOP 1 Mean	--'%OSTotalPhysicalMemoryInKilobytes%' -- -1002
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1002 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		OSAvailableMemoryInKilobytesMean = (select TOP 1 Mean --'%OSAvailableMemoryInKilobytes%' -- -1000
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1000 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		SqlMemoryAllocatedInKilobytesMean = (select TOP 1 Mean	--'%SqlMemoryAllocatedInKilobytes%' -- -70
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -70 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		SqlMemoryUsedInKilobytesMean = (select TOP 1 Mean	--'%SqlMemoryUsedInKilobytes%' -- -71
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -71 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		BufferCacheHitRatioPercentageMean = (select TOP 1 Mean --'%BufferCacheHitRatioPercentage%' -- -9
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -9 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),				
		BufferCacheSizeInKilobytesMean = (select TOP 1 Mean	--'%BufferCacheSizeInKilobytes%' -- -10
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -10 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		ProcedureCacheSizeInKilobytesMean = (select TOP 1 Mean	--'%ProcedureCacheSizeInKilobytes%' -- -1003
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1003 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		WorkTablesCreatedMean = (select TOP 1 Mean --'%WorkTablesCreated%' -- -84
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -84 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60
		FROM		
			[BaselineStatistics] bs (nolock)
		WHERE bs.[SQLServerID] = @ServerID and bs.UTCCalculation between @UTCStart and @UTCEnd
		GROUP BY dbo.fn_RoundDateTime(2, bs.UTCCalculation)

END


