
if (object_id('p_GetCPUSummaryBaseline') is not null)
begin
drop procedure p_GetCPUSummaryBaseline
end
go
CREATE PROCEDURE [dbo].[p_GetCPUSummaryBaseline]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint
AS
BEGIN
		SELECT		
		UTCCalculation = dbo.fn_RoundDateTime(1, max(dateadd(mi, @UTCOffset, UTCCalculation))),
		ProcessorTimePercentMean = (select TOP 1 Mean	--ProcessorTimePercent MetricID = 26 
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 26 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		CPUActivityPercentageMean = (select TOP 1 Mean	--CPUActivityPercentage MetricID = 0
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 0 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		ProcessorQueueLengthMean = (select TOP 1 Mean	--ProcessorQueueLength MetricID = 29
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = 29 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		TableLockEscalationsMean = (select TOP 1 Mean --TableLockEscalations
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc)	
		FROM		
			[BaselineStatistics] bs (nolock)
		WHERE bs.[SQLServerID] = @ServerID and bs.UTCCalculation between @UTCStart and @UTCEnd
		GROUP BY dbo.fn_RoundDateTime(2, bs.UTCCalculation)

END
 
