if (object_id('p_GetDiskStatisticsBaseline') is not null)
begin
drop procedure [p_GetDiskStatisticsBaseline]
end
go

CREATE PROCEDURE [dbo].[p_GetDiskStatisticsBaseline]
		@ServerID int,
		@UTCStart DateTime,
		@UTCEnd DateTime,
		@UTCOffset int,
		@Interval tinyint
AS
BEGIN
		SELECT		
		UTCCalculation = dbo.fn_RoundDateTime(1, max(dateadd(mi, @UTCOffset, UTCCalculation))),
		DiskQueueLengthMean = (select TOP 1 Mean	--DiskQueueLength MetricID = 31
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 31 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		DiskTimePercentMean = (select TOP 1 Mean	--DiskTimePercent MetricID = 30
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = 30 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		IOActivityPercentageMean = (select TOP 1 Mean	--IOActivityPercentage MetricID = -135
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -135 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		WorkFilesCreatedMean = (select TOP 1 Mean --WorkFilesCreated -82
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -82 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,
		TempDBSizeInKilobytesMean = (select TOP 1 Mean	--'%TempDBSizeInKilobytes%' -- -136
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -136 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),
		PageReadsMean = (select TOP 1 Mean	--'%PageReads%' -- -52
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -52 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),		
		PageWritesMean = (select TOP 1 Mean --'%PageWrites%' -- -1001
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -1001 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc),				
		PageSplitsMean = (select TOP 1 Mean	--'%PageSplits%' -- -55
				from BaselineStatistics
				where SQLServerID = @ServerID AND MetricID = -55 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,
		PageLookupsMean = (select TOP 1 Mean	--'%PageLookups%' -- -50
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -50 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60,		
		ReadAheadPagesMean = (select TOP 1 Mean --'%ReadAheadPages%' -- -63
				from BaselineStatistics
				where SQLServerID = @ServerID AND  MetricID = -63 and dbo.fn_RoundDateTime(2, UTCCalculation) = dbo.fn_RoundDateTime(2, bs.UTCCalculation)
				order by UTCCalculation desc) * 60	
		FROM		
			[BaselineStatistics] bs (nolock)
		WHERE bs.[SQLServerID] = @ServerID and bs.UTCCalculation between @UTCStart and @UTCEnd
		GROUP BY dbo.fn_RoundDateTime(2, bs.UTCCalculation)

END