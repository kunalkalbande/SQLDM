if (object_id('[p_GetCPUSummary]') is not null)
begin
drop procedure [p_GetCPUSummary]
end
go
CREATE PROCEDURE [dbo].[p_GetCPUSummary]
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

--SQLdm (10.2)--Tushar--Saved maximum UTCCollectionDateTime from ServerStatistics table.
declare @MaximumUTCCollectionDateTimeServerStatistics datetime;
   select @MaximumUTCCollectionDateTimeServerStatistics = MAX(cast( dateadd(mi, @UTCOffset, [UTCCollectionDateTime]) as datetime)) FROM [ServerStatistics] where [ServerStatistics].SQLServerID = @ServerID

select m.InstanceName
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval],
		sum(o.[ProcessorTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [ProcessorTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) as OSProcessorTimePercent,
		sum(s1.[CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as SQLCPUActivityPercentage,
		sum(o.[ProcessorQueueLength] * TimeDeltaInSeconds) / nullif(sum(case when [ProcessorQueueLength] is not null then TimeDeltaInSeconds else 0 end),0) as OSProcessorQueueLength,
		sum(convert(float,s1.[SqlCompilations])) / nullif((sum(convert(float,case when s1.[SqlCompilations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as SqlCompilations,
		sum(convert(float,s1.[SqlRecompilations])) / nullif((sum(convert(float,case when s1.[SqlRecompilations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as SqlRecompilations,
		sum(convert(float,s1.[LockWaits])) / nullif((sum(convert(float,case when s1.[LockWaits] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as LockWaits,
		sum(convert(float,s1.[TableLockEscalations])) / nullif((sum(convert(float,case when s1.[TableLockEscalations] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as TableLockEscalations,
		--SQLdm (10.2)--Tushar--Changed the udf.fn_RoundDateTime method call to sql server native 'CAST' method for all the fields extracted from BaselineStatistics table.
		ProcessorTimePercentMean = (select TOP 1 Mean	--ProcessorTimePercent MetricID = 26 
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 26 AND cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		CPUActivityPercentageMean = (select TOP 1 Mean	--CPUActivityPercentage MetricID = 0
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 0 AND cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		ProcessorQueueLengthMean = (select TOP 1 Mean	--ProcessorQueueLength MetricID = 29
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 29 AND cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),		
		TableLockEscalationsMean = (select TOP 1 Mean --TableLockEscalations
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1 AND cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc)	
	from		
		[MonitoredSQLServers] m (nolock) left join
		[ServerStatistics] s1 (nolock)on m.[SQLServerID] = s1.[SQLServerID]left join 
		[OSStatistics] o (nolock)on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
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
