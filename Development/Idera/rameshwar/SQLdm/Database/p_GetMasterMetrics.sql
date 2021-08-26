if (object_id('p_GetMasterMetrics') is not null)
begin
drop procedure p_GetMasterMetrics
end
go
CREATE PROCEDURE [dbo].[p_GetMasterMetrics]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint,
		@MasterMetric tinyint,
		@CompareToId int = null,
		@CompareStartRange DateTime = null
AS
BEGIN

-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years

--@MasterMetric 
-- 0 - Response Time
-- 1 - CPU Percentage
-- 2 - Memory Usage
-- 3 - % Disk Busy
-- 4 - Session Count

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]

declare @CompareEndRange DateTime
set @CompareStartRange = isnull(@CompareStartRange,@UTCStart)
set @CompareEndRange = dateadd(mi, datediff(mi, @UTCStart, @UTCEnd), @CompareStartRange)

declare @ServerStats table(
	InstanceName nvarchar(257),
	LastCollectioninInterval datetime,
	IntervalNumber int,
	MemoryUsed float,
	ResponseTimeInMilliseconds int,
	CPUActivityPercentage float,
	DiskTimePercent float,
	SessionCount int,
	MetricName nvarchar(128),
	MetricValue float,
	RowNumber int identity,
	IntervalTemp bigint
) 

insert into @ServerStats(InstanceName,LastCollectioninInterval, IntervalTemp,CPUActivityPercentage,DiskTimePercent,MemoryUsed,
ResponseTimeInMilliseconds,SessionCount,MetricName,MetricValue)

	select
		InstanceName = case 
			when @ServerID = @CompareToId
		then 
			m.InstanceName + ' (Base)'
		else m.InstanceName end
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval]
		,IntervalTemp = case
					 when isnull(@Interval,5) = 5
							then 0
					 when isnull(@Interval,5) = 4
							then datediff(yyyy, @UTCStart, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 3
							then datediff(mm, @UTCStart, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 2
							then datediff(dd, @UTCStart, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 1
							then datediff(hh, @UTCStart, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) =  0
							then datediff(mi, @UTCStart, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 end
		,sum([CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [CPUActivityPercentage]
		,sum([DiskTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [DiskTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) as [DiskTimePercent]
		,sum(cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) * TimeDeltaInSeconds) / nullif(sum(case when cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) is not null then TimeDeltaInSeconds else 0 end),0) / 1024 / 1024
		,sum([ResponseTimeInMilliseconds] * TimeDeltaInSeconds) / nullif(sum(case when [ResponseTimeInMilliseconds] is not null then TimeDeltaInSeconds else 0 end),0) as [ResponseTimeInMilliseconds]
		,sum(([SystemProcesses] + UserProcesses) * TimeDeltaInSeconds) / nullif(sum(case when ([SystemProcesses] + UserProcesses) is not null then TimeDeltaInSeconds else 0 end),0)
		,case when @MasterMetric = 0 then 'Response Time (ms)' 
		     when @MasterMetric = 1 then '% CPU Activity' 
		     when @MasterMetric = 2 then 'Memory Usage (GB)' 
		     when @MasterMetric = 3 then '% Disk Busy' 
		else
		     'Session Count' 
		end
		,case when @MasterMetric = 0 then sum([ResponseTimeInMilliseconds] * TimeDeltaInSeconds) / nullif(sum(case when [ResponseTimeInMilliseconds] is not null then TimeDeltaInSeconds else 0 end),0)
		     when @MasterMetric = 1 then sum([CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0)
		     when @MasterMetric = 2 then sum(cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) * TimeDeltaInSeconds) / nullif(sum(case when cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) is not null then TimeDeltaInSeconds else 0 end),0) / 1024 / 1024
		     when @MasterMetric = 3 then sum([DiskTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [DiskTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) 
		else
		     max([SystemProcesses] + UserProcesses)
		end
	from
		[ServerStatistics] s1 (nolock)
		left join [#SecureMonitoredSQLServers] m (nolock)
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
union
	select 
		InstanceName= case 
			when @ServerID = @CompareToId
		then 
			m.InstanceName + ' (Compare)'
		else m.InstanceName end
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval]
		,IntervalTemp = case
					 when isnull(@Interval,5) = 5
							then 0
					 when isnull(@Interval,5) = 4
							then datediff(yyyy, @CompareStartRange, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 3
							then datediff(mm, @CompareStartRange, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 2
							then datediff(dd, @CompareStartRange, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) = 1
							then datediff(hh, @CompareStartRange, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 when isnull(@Interval,5) =  0
							then datediff(mi, @CompareStartRange, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])))
					 end
		,sum([CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0) as [CPUActivityPercentage]
		,sum([DiskTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [DiskTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) as [DiskTimePercent]
		,sum(cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) * TimeDeltaInSeconds) / nullif(sum(case when cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) is not null then TimeDeltaInSeconds else 0 end),0) / 1024 / 1024
		,sum([ResponseTimeInMilliseconds] * TimeDeltaInSeconds) / nullif(sum(case when [ResponseTimeInMilliseconds] is not null then TimeDeltaInSeconds else 0 end),0) as [ResponseTimeInMilliseconds]
		,sum(([SystemProcesses] + UserProcesses) * TimeDeltaInSeconds) / nullif(sum(case when ([SystemProcesses] + UserProcesses) is not null then TimeDeltaInSeconds else 0 end),0)
		,case when @MasterMetric = 0 then 'Response Time (ms)' 
		     when @MasterMetric = 1 then '% CPU Activity' 
		     when @MasterMetric = 2 then 'Memory Usage (GB)' 
		     when @MasterMetric = 3 then '% Disk Busy' 
		else
		     'Session Count' 
		end
		,case when @MasterMetric = 0 then sum([ResponseTimeInMilliseconds] * TimeDeltaInSeconds) / nullif(sum(case when [ResponseTimeInMilliseconds] is not null then TimeDeltaInSeconds else 0 end),0)
		     when @MasterMetric = 1 then sum([CPUActivityPercentage] * TimeDeltaInSeconds) / nullif(sum(case when [CPUActivityPercentage] is not null then TimeDeltaInSeconds else 0 end),0)
		     when @MasterMetric = 2 then sum(cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) * TimeDeltaInSeconds) / nullif(sum(case when cast(([OSTotalPhysicalMemoryInKilobytes] - [OSAvailableMemoryInKilobytes]) as float) is not null then TimeDeltaInSeconds else 0 end),0) / 1024 / 1024
		     when @MasterMetric = 3 then sum([DiskTimePercent] * TimeDeltaInSeconds) / nullif(sum(case when [DiskTimePercent] is not null then TimeDeltaInSeconds else 0 end),0) 
		else
		     max([SystemProcesses] + UserProcesses)
		end
	from
		[ServerStatistics] s1 (nolock)
		left join [#SecureMonitoredSQLServers] m (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
		left join [OSStatistics] o (nolock)
		on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
	where 
		s1.[SQLServerID] = @CompareToId
		and dbo.fn_RoundDateTime(@Interval, s1.[UTCCollectionDateTime]) between @CompareStartRange and @CompareEndRange
	group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
		,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
	order by
		IntervalTemp asc, 
		InstanceName asc
 
declare @rownum int, @intervalcounter int, @previnterval bigint
	select 
		@rownum = isnull(min(RowNumber),-1) ,
		@intervalcounter = 0,
		@previnterval = 0
	from @ServerStats
	while @rownum > 0
	begin
		update @ServerStats
			set IntervalNumber = case when IntervalTemp = @previnterval then @intervalcounter else @intervalcounter + 1 end
		where 
			RowNumber = @rownum

		select
			@intervalcounter = case when IntervalTemp = @previnterval then @intervalcounter 
									else @intervalcounter + 1 end,
			@previnterval = IntervalTemp
		from 
			@ServerStats
		where 
			RowNumber = @rownum
		
		select 
			@rownum = isnull(min(RowNumber),-1) 
		from 
			@ServerStats
		where 
			RowNumber > @rownum
	end

	select 
		InstanceName,
		LastCollectioninInterval,
		IntervalNumber,
		MemoryUsed,
		ResponseTimeInMilliseconds,
		CPUActivityPercentage,
		DiskTimePercent,
		SessionCount,
		MetricName,
		MetricValue,
		@Interval as Interval,
		ISNULL(@CompareToId, -1) as Compare
	from 
		@ServerStats
	order by
		IntervalNumber asc, 
		[InstanceName] asc
		
END
 
