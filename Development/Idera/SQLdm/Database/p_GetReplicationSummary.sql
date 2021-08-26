if (object_id('[p_GetReplicationSummary]') is not null)
begin
drop procedure [p_GetReplicationSummary]
end
go
CREATE PROCEDURE [dbo].[p_GetReplicationSummary]
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

IF(@Interval = 0)
-- @Interval - Granularity of calculation:
--	0 - Minutes Start
BEGIN
select m.InstanceName
		,dateadd(minute, datediff(minute, 0, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))), 0) as [LastCollectioninInterval],
		max(s1.[ReplicationLatencyInSeconds]) as ReplicationLatencyInSeconds,
		sum(s1.[ReplicationSubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationSubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationSubscribed],
		sum(s1.[ReplicationUndistributed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUndistributed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUndistributed],
		sum(s1.[ReplicationUnsubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUnsubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUnsubscribed],
		[ReplicationLatencyInSecondsBaseline] = (select TOP 1 Mean	--[ReplicationLatencyInSeconds] MetricID = 17
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 17 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0)= MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),
		[ReplicationSubscribedBaseline] = (select TOP 1 Mean	--[ReplicationSubscribed] MetricID = -1005
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1005 AND 
				-- Using Cast instead of fn_RoundDateTime()
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc) * 60,
		[ReplicationUndistributedBaseline] = (select TOP 1 Mean	--[ReplicationUndistributed] MetricID = 4
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 4 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),		
		[ReplicationUnsubscribedBaseline] = (select TOP 1 Mean --[ReplicationUnsubscribed] = 5
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 5 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc)
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		s1.[SQLServerID] = @ServerID
		and dateadd(minute, datediff(minute, 0, s1.[UTCCollectionDateTime]), 0) BETWEEN @UTCStart and @UTCEnd
group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(mi,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
-- @Interval - Granularity of calculation:
--	0 - Minutes End
END
ELSE IF (@Interval = 1)
BEGIN
-- @Interval - Granularity of calculation:
--	1 - Hours Start
select m.InstanceName
		,dateadd(hour, datediff(hour, 0, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))), 0)
		,max(s1.[ReplicationLatencyInSeconds]) as ReplicationLatencyInSeconds,
		sum(s1.[ReplicationSubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationSubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationSubscribed],
		sum(s1.[ReplicationUndistributed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUndistributed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUndistributed],
		sum(s1.[ReplicationUnsubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUnsubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUnsubscribed],
		[ReplicationLatencyInSecondsBaseline] = (select TOP 1 Mean	--[ReplicationLatencyInSeconds] MetricID = 17
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 17 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0)= MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),
		[ReplicationSubscribedBaseline] = (select TOP 1 Mean	--[ReplicationSubscribed] MetricID = -1005
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1005 AND 
				-- Using Cast instead of fn_RoundDateTime()
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc) * 60,
		[ReplicationUndistributedBaseline] = (select TOP 1 Mean	--[ReplicationUndistributed] MetricID = 4
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 4 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),		
		[ReplicationUnsubscribedBaseline] = (select TOP 1 Mean --[ReplicationUnsubscribed] = 5
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 5 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc)
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		s1.[SQLServerID] = @ServerID
		and 
		dateadd(hour, datediff(hour, 0, s1.[UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
-- @Interval - Granularity of calculation:
--	1 - Hours End
END

ELSE IF (@Interval = 2)
BEGIN
-- @Interval - Granularity of calculation:
--	2 - Days Start
select m.InstanceName
		,dateadd(day, datediff(day, 0, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))), 0)
		,max(s1.[ReplicationLatencyInSeconds]) as ReplicationLatencyInSeconds,
		sum(s1.[ReplicationSubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationSubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationSubscribed],
		sum(s1.[ReplicationUndistributed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUndistributed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUndistributed],
		sum(s1.[ReplicationUnsubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUnsubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUnsubscribed],
		[ReplicationLatencyInSecondsBaseline] = (select TOP 1 Mean	--[ReplicationLatencyInSeconds] MetricID = 17
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 17 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0)= MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),
		[ReplicationSubscribedBaseline] = (select TOP 1 Mean	--[ReplicationSubscribed] MetricID = -1005
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1005 AND 
				-- Using Cast instead of fn_RoundDateTime()
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc) * 60,
		[ReplicationUndistributedBaseline] = (select TOP 1 Mean	--[ReplicationUndistributed] MetricID = 4
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 4 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),		
		[ReplicationUnsubscribedBaseline] = (select TOP 1 Mean --[ReplicationUnsubscribed] = 5
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 5 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc)
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		s1.[SQLServerID] = @ServerID
		and dateadd(day, datediff(day, 0, s1.[UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		,datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
-- @Interval - Granularity of calculation:
--	2 - Days End
END

ELSE IF (@Interval = 3)
BEGIN
-- @Interval - Granularity of calculation:
--	3 - Months Start
select m.InstanceName
		,
		dateadd(month, datediff(month, 0, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))), 0) as [LastCollectioninInterval],
		max(s1.[ReplicationLatencyInSeconds]) as ReplicationLatencyInSeconds,
		sum(s1.[ReplicationSubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationSubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationSubscribed],
		sum(s1.[ReplicationUndistributed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUndistributed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUndistributed],
		sum(s1.[ReplicationUnsubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUnsubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUnsubscribed],
		[ReplicationLatencyInSecondsBaseline] = (select TOP 1 Mean	--[ReplicationLatencyInSeconds] MetricID = 17
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 17 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0)= MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),
		[ReplicationSubscribedBaseline] = (select TOP 1 Mean	--[ReplicationSubscribed] MetricID = -1005
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1005 AND 
				-- Using Cast instead of fn_RoundDateTime()
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc) * 60,
		[ReplicationUndistributedBaseline] = (select TOP 1 Mean	--[ReplicationUndistributed] MetricID = 4
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 4 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),		
		[ReplicationUnsubscribedBaseline] = (select TOP 1 Mean --[ReplicationUnsubscribed] = 5
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 5 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc)
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		s1.[SQLServerID] = @ServerID
		and dateadd(month, datediff(month, 0, s1.[UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
group BY
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
-- @Interval - Granularity of calculation:
--	3 - Months End
END

ELSE IF (@Interval = 4)
BEGIN
-- @Interval - Granularity of calculation:
--	4 - Years Start
select m.InstanceName
		,
		dateadd(month, datediff(month, 0, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))), 0) AS [LastCollectioninInterval],
		max(s1.[ReplicationLatencyInSeconds]) as ReplicationLatencyInSeconds,
		sum(s1.[ReplicationSubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationSubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationSubscribed],
		sum(s1.[ReplicationUndistributed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUndistributed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUndistributed],
		sum(s1.[ReplicationUnsubscribed] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ReplicationUnsubscribed] is not null then TimeDeltaInSeconds else 0 end),0) as [ReplicationUnsubscribed],
		[ReplicationLatencyInSecondsBaseline] = (select TOP 1 Mean	--[ReplicationLatencyInSeconds] MetricID = 17
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 17 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0)= MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),
		[ReplicationSubscribedBaseline] = (select TOP 1 Mean	--[ReplicationSubscribed] MetricID = -1005
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1005 AND 
				-- Using Cast instead of fn_RoundDateTime()
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc) * 60,
		[ReplicationUndistributedBaseline] = (select TOP 1 Mean	--[ReplicationUndistributed] MetricID = 4
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 4 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc),		
		[ReplicationUnsubscribedBaseline] = (select TOP 1 Mean --[ReplicationUnsubscribed] = 5
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 5 AND 
				-- Using Cast instead of fn_RoundDateTime()
				dateadd(day, datediff(day, 0, dateadd(mi, @UTCOffset, UTCCalculation)), 0) = MAX(
				DATEADD(day, datediff(day, 0, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])), 0))
				order by UTCCalculation desc)
	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
	where
		s1.[SQLServerID] = @ServerID
		and 
		dateadd(month, datediff(month, 0, s1.[UTCCollectionDateTime]), 0) between @UTCStart and @UTCEnd
group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
-- @Interval - Granularity of calculation:
--	4 - Years End
END

END
