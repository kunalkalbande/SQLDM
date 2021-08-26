if (object_id('p_GetSessionSummaryBaseline') is not null)
begin
drop procedure [p_GetSessionSummaryBaseline]
end
go

CREATE PROCEDURE [dbo].[p_GetSessionSummaryBaseline]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint
AS
BEGIN
		SELECT		
		UTCCalculation = dbo.fn_RoundDateTime(1, max(dateadd(mi, @UTCOffset, UTCCalculation))),
		ClientComputersMean = (select TOP 1 Mean	--'%ClientComputers%' -- 57
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 57 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		OldestOpenTransactionsInMinutesMean = (select TOP 1 Mean	--'%OldestOpenTransactionsInMinutes%' -- 6
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 6 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		UserProcessesMean = (select TOP 1 Mean	--'%UserProcesses%' -- -78
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -78 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		LoginsMean = (select TOP 1 Mean --'%Logins%' -- -137
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -137 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		PacketErrorsMean = (select TOP 1 Mean	--'%PacketErrors%' -- -43
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -43 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,
		PacketsReceivedMean = (select TOP 1 Mean	--'%PacketsReceived%' -- -45
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -45 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,		
		PacketsSentMean = (select TOP 1 Mean --'%PacketsSent%' -- -47
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -47 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,				
		TransactionsMean = (select TOP 1 Mean	--'%Transactions%' -- -7
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -7 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc)		
		FROM		
			[BaselineStatistics] bs (nolock)
		WHERE bs.[SQLServerID] = @ServerID and bs.UTCCalculation between @UTCStart and @UTCEnd
		GROUP BY dbo.fn_RoundDateTime(2, bs.UTCCalculation)

END