if (object_id('[p_GetSessionsSummary]') is not null)
begin
drop procedure [p_GetSessionsSummary]
end
go
CREATE PROCEDURE [dbo].[p_GetSessionsSummary]
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
		sum(s1.[ClientComputers] * TimeDeltaInSeconds) / nullif(sum(case when s1.[ClientComputers] is not null then TimeDeltaInSeconds else 0 end),0) as ClientComputers,
		sum(s1.[UserProcesses] * TimeDeltaInSeconds) / nullif(sum(case when s1.[UserProcesses] is not null then TimeDeltaInSeconds else 0 end),0) as UserProcesses,
		sum(convert(float,s1.[Logins])) / nullif((sum(convert(float,case when s1.[Logins] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as Logins,
		sum(convert(float,s1.[PacketErrors])) / nullif((sum(convert(float,case when s1.[PacketErrors] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as PacketErrors,
		sum(convert(float,s1.[PacketsReceived])) / nullif((sum(convert(float,case when s1.[PacketsReceived] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as PacketsReceived,
		sum(convert(float,s1.[PacketsSent])) / nullif((sum(convert(float,case when s1.[PacketsSent] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as PacketsSent,
		sum(convert(float,s1.[Transactions])) / nullif((sum(convert(float,case when s1.[Transactions] is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0)as Transactions,
		max(s1.[OldestOpenTransactionsInMinutes]) as OldestOpenTransactionsInMinutes,
		--SQLdm (10.2)--Tushar--Changed the udf.fn_RoundDateTime method call to sql server native 'CAST' method for all the fields extracted from BaselineStatistics table.
		ClientComputersMean = (select TOP 1 Mean	--'%ClientComputers%' -- 57
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 57 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		UserProcessesMean = (select TOP 1 Mean	--'%UserProcesses%' -- -78
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -78 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),		
		LoginsMean = (select TOP 1 Mean --'%Logins%' -- -137
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -137 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		PacketErrorsMean = (select TOP 1 Mean	--'%PacketErrors%' -- -43
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -43 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc) * 60,
		PacketsReceivedMean = (select TOP 1 Mean	--'%PacketsReceived%' -- -45
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -45 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc) * 60,		
		PacketsSentMean = (select TOP 1 Mean --'%PacketsSent%' -- -47
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -47 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc) * 60,				
		TransactionsMean = (select TOP 1 Mean	--'%Transactions%' -- -7
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -7 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc),
		OldestOpenTransactionsInMinutesMean = (select TOP 1 Mean	--'%OldestOpenTransactionsInMinutes%' -- 6
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 6 and cast(dateadd(mi, @UTCOffset, UTCCalculation) as datetime) = @MaximumUTCCollectionDateTimeServerStatistics
				order by UTCCalculation desc)				

	from
		[MonitoredSQLServers] m (nolock)
		left join [ServerStatistics] s1 (nolock)
		on m.[SQLServerID] = s1.[SQLServerID]
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
