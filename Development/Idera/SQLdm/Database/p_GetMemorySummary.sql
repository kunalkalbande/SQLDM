if (object_id('p_GetMemorySummary') is not null)
begin
drop procedure p_GetMemorySummary
end
go
CREATE PROCEDURE [dbo].[p_GetMemorySummary]
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
--,sum([IOActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [IOActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [IOActivityPercentage]
	
	--SQLdm (10.2)--Tushar--Saved maximum UTCCollectionDateTime from ServerStatistics table.
declare @MaximumUTCCollectionDateTimeServerStatistics datetime;
   select @MaximumUTCCollectionDateTimeServerStatistics = MAX(cast( dateadd(mi, @UTCOffset, [UTCCollectionDateTime]) as datetime)) FROM [ServerStatistics] where [ServerStatistics].SQLServerID = @ServerID
	
	select
		m.InstanceName
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval]
		,sum([OSTotalPhysicalMemoryInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [OSTotalPhysicalMemoryInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [OSTotalPhysicalMemoryInKilobytes]
		,sum([OSAvailableMemoryInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [OSAvailableMemoryInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [OSAvailableMemoryInKilobytes]
		,sum([SqlMemoryAllocatedInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [SqlMemoryAllocatedInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [SqlMemoryAllocatedInKilobytes]
		,sum([SqlMemoryUsedInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [SqlMemoryUsedInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [SqlMemoryUsedInKilobytes]
		,sum([PageLifeExpectancy] * TimeDeltaInSeconds) / nullif(sum(case when [PageLifeExpectancy] is not null then TimeDeltaInSeconds else 0 end),0) as [PageLifeExpectancy]
		,sum([BufferCacheHitRatioPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [BufferCacheHitRatioPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [BufferCacheHitRatioPercentage]
		,sum([BufferCacheSizeInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [BufferCacheSizeInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [BufferCacheSizeInKilobytes]
		,sum([ProcedureCacheSizeInKilobytes] * TimeDeltaInSeconds) / nullif(sum(case when [ProcedureCacheSizeInKilobytes] is not null then TimeDeltaInSeconds else 0 end),0) as [ProcedureCacheSizeInKilobytes]
		,sum([ProcedureCacheHitRatioPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [ProcedureCacheHitRatioPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [ProcedureCacheHitRatioPercentage]
		,sum(convert(float,[WorkTablesCreated])) / nullif((sum(convert(float,case when [WorkTablesCreated] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as [WorkTablesCreated]
		,PageLifeExpectancyMean = (select TOP 1 Mean	--'%PageLifeExpectancy%' -- 76
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 76 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		ProcedureCacheHitRatioPercentageMean = (select TOP 1 Mean	--'%ProcedureCacheHitRatioPercentage%' -- 81
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 81 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		OSTotalPhysicalMemoryInKilobytesMean = (select TOP 1 Mean	--'%OSTotalPhysicalMemoryInKilobytes%' -- -1002
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1002 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),		
		OSAvailableMemoryInKilobytesMean = (select TOP 1 Mean --'%OSAvailableMemoryInKilobytes%' -- -1000
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1000 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		SqlMemoryAllocatedInKilobytesMean = (select TOP 1 Mean	--'%SqlMemoryAllocatedInKilobytes%' -- -70
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -70 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		SqlMemoryUsedInKilobytesMean = (select TOP 1 Mean	--'%SqlMemoryUsedInKilobytes%' -- -71
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -71 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),		
		BufferCacheHitRatioPercentageMean = (select TOP 1 Mean --'%BufferCacheHitRatioPercentage%' -- -9
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -9 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),				
		BufferCacheSizeInKilobytesMean = (select TOP 1 Mean	--'%BufferCacheSizeInKilobytes%' -- -10
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -10 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		ProcedureCacheSizeInKilobytesMean = (select TOP 1 Mean	--'%ProcedureCacheSizeInKilobytes%' -- -1003
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1003 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),		
		WorkTablesCreatedMean = (select TOP 1 Mean --'%WorkTablesCreated%' -- -84
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -84 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc) * 60
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
		left join [OSStatistics] o (nolock)
		on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
	where
		s1.[SQLServerID] = @ServerID
		and dbo.fn_RoundDateTime(@Interval, s1.[UTCCollectionDateTime]) between @UTCStart and @UTCEnd
	group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
END
 
