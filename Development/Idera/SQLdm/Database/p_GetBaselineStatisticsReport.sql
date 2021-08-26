
if (object_id('p_GetBaselineStatisticsReport') is not null)
begin
drop procedure p_GetBaselineStatisticsReport
end
go
CREATE PROCEDURE [dbo].[p_GetBaselineStatisticsReport]
			@ServerID int,
			@UTCStart DateTime = NULL,
			@UTCEnd DateTime = NULL,
			@MetricID int,
			@CompareSQLServerID int,
			@CompareStartRange DateTime = NULL,
			@CompareEndRange DateTime = NULL,
			@CompareMetricID int,
			@UTCOffset int = 0,
			@Interval tinyint
AS
BEGIN

DECLARE @ParmDefinition nvarchar(500); -- Param definition for EXECUTE sp_executesql 

DECLARE @SQLString NVARCHAR(MAX) -- String that will contain the main SQL to be executed
DECLARE @tableName NVARCHAR(256) -- Table that contains the Metric value refenced in BaselineMetadata (OSStatistics or ServerStatistics)
DECLARE @columnName NVARCHAR(256) -- Column inside OSStatistics or ServerStatistics with the metric value
DECLARE @compareTableName NVARCHAR(256) -- Table that contains the Metric value refenced in BaselineMetadata (OSStatistics or ServerStatistics)
DECLARE @compareColumnName NVARCHAR(256) -- Column inside OSStatistics or ServerStatistics with the metric value
DECLARE @regex NVARCHAR(10)
DECLARE @Today bit -- Tells if period is today. Case it is 1 then the Period is Today. Whenever Period = Today was entered, then the MetricValue calculation will be done against all data from today.
DECLARE @CompareToday bit -- Tells if period is today. Case it is 1 then the Period is Today. Whenever Period = Today was entered, then the MetricValue calculation will be done against all data from today.

SET @regex = '%[-/()*]%'
SET @CompareStartRange = isnull(@CompareStartRange, @UTCStart)
SET @Today = CASE WHEN (dbo.fn_RoundDateTime(2, @UTCStart) = dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, GETDATE()))) THEN 1 ELSE 0 END
SET @CompareToday = CASE WHEN (dbo.fn_RoundDateTime(2, @CompareStartRange) = dbo.fn_RoundDateTime(2, dateadd(mi, @UTCOffset, GETDATE()))) THEN 1 ELSE 0 END

-- Base tablename and columnname.
SELECT @tableName = StatisticTable, @columnName = MetricValue
FROM BaselineMetaData
WHERE MetricID = @MetricID
-- Compare tablename and columnname.
SELECT @compareTableName = StatisticTable, @compareColumnName = MetricValue
FROM BaselineMetaData
WHERE MetricID = @CompareMetricID

DECLARE @BStatistics TABLE(
	[UTCCollectionDateTime] datetime,    
	[ServerName] nvarchar(256),
    [MetricName] nvarchar(128),
    [Unit] nvarchar(32),
    [Mean] decimal(38, 5),
    [MetricValue] decimal(38, 5) ,
    [StdDeviation] decimal(38, 5),
    [Min] decimal(38, 5),
    [Max] decimal(38, 5),
	[Count] decimal(38, 5),
	Template nvarchar(1024),
	[MetricID] int
) 

SELECT @SQLString = '
						   
;with BaselineStatisticsBase(UTCCalculation,[Template], [Mean],[StdDeviation],[Min],[Max],[Count])
as (
 select 
	UTCCalculation,
	s.Template,	
	[Mean],
	[StdDeviation],
	[Min],
	[Max],
	[Count]
 from 		
	BaselineStatistics bs INNER JOIN   
	BaselineTemplates bt ON bt.TemplateID = bs.TemplateID AND bt.SQLServerID = bs.SQLServerID 
	cross apply dbo.fn_GetFormattedTemplate(bt.Template) s'

Select @SQLString = @SQLString + 
	case @Today when 0 then ' where bs.MetricID = @MetricID and bs.SQLServerID = @ServerID and UTCCalculation BETWEEN @UTCStart AND @UTCEnd )  '
					   else ' where bs.MetricID = @MetricID and bs.SQLServerID = @ServerID and dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) = dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, getdate())))
					   ' end
					   
Select @SQLString = @SQLString + '
SELECT 			
	[UTCCollectionDateTime] = dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, S.[UTCCollectionDateTime]))),	
	[ServerName] = MAX(
		case 
			when @ServerID = @CompareSQLServerID
		then 
			ms.InstanceName + '' (Base)''
		else ms.InstanceName end),			
	[MetricName] = (Select Name from BaselineMetaData where MetricID = @MetricID),		
	[Unit] = (Select Unit from BaselineMetaData where MetricID = @MetricID),
	[Mean] = (Select top 1 Mean from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
	[MetricValue] = AVG({columnName}),		
	[StdDeviation] = (Select top 1 StdDeviation from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
	[Min] = (Select top 1 [Min] from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
	[Max] = (Select top 1 [Max] from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),	
	[Count] = (Select top 1 [Count] from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
	[Template] = (Select top 1 [Template] from BaselineStatisticsBase bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
	MetricID = @MetricID
	
FROM MonitoredSQLServers ms INNER JOIN
{tableName} on ms.SQLServerID = S.SQLServerID

WHERE ms.SQLServerID = @ServerID AND UTCCollectionDateTime BETWEEN @UTCStart AND @UTCEnd
group by
	case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else 1 end
	,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
	,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
	,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
	,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
order by UTCCollectionDateTime asc'

IF(@CompareSQLServerID >= 0)
BEGIN
	select @SQLString = @SQLString + '
	;with BaselineStatisticsCompare(UTCCalculation,[Template], [Mean],[StdDeviation],[Min],[Max],[Count])
	as (
	 select 
		UTCCalculation,
		s.[Template],	
		[Mean],
		[StdDeviation],
		[Min],
		[Max],
		[Count]
	 from 
		BaselineStatistics bs INNER JOIN   
		BaselineTemplates bt ON bt.TemplateID = bs.TemplateID AND bt.SQLServerID = bs.SQLServerID 
		cross apply dbo.fn_GetFormattedTemplate(bt.Template) s'
	Select @SQLString = @SQLString + 

	case @CompareToday when 0 then ' where bs.MetricID = @CompareMetricID and bs.SQLServerID = @CompareSQLServerID and UTCCalculation BETWEEN @CompareStartRange AND @CompareEndRange )  '
					   else ' where bs.MetricID = @CompareMetricID and bs.SQLServerID = @CompareSQLServerID and dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) = dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, getdate())))
					   ' end
					   
Select @SQLString = @SQLString + '


	SELECT 			
		[UTCCollectionDateTime] = dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, S.[UTCCollectionDateTime]))),	
		[ServerName] = MAX(
			case 
				when @ServerID = @CompareSQLServerID
			then 
				ms.InstanceName + '' (Compare)''
			else ms.InstanceName end),			
		[MetricName] = (Select Name from BaselineMetaData where MetricID = @CompareMetricID),		
		[Unit] = (Select Unit from BaselineMetaData where MetricID = @CompareMetricID),
		[Mean] = (Select top 1 Mean from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
		[MetricValue] = AVG({compareColumnName}),		
		[StdDeviation] = (Select top 1 StdDeviation from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
		[Min] = (Select top 1 [Min] from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
		[Max] = (Select top 1 [Max] from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),	
		[Count] = (Select top 1 [Count] from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
		[Template] = (Select top 1 [Template] from BaselineStatisticsCompare bs where dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, UTCCalculation)) =  dbo.fn_RoundDateTime(2, DATEADD(MI, @UTCOffset, MAX(UTCCollectionDateTime))) order by UTCCalculation desc),
		MetricID = @CompareMetricID
		
	FROM MonitoredSQLServers ms INNER JOIN
	{compareTableName} on ms.SQLServerID = S.SQLServerID

	WHERE ms.SQLServerID = @CompareSQLServerID AND UTCCollectionDateTime BETWEEN @CompareStartRange AND @CompareEndRange
	group by
		case when isnull(@Interval,5) <= 4 then datepart(yy, dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else 1 end
		,case when isnull(@Interval,5) <= 3 then datepart(mm,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
		,case when isnull(@Interval,5) <= 2 then datepart(dd,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
		,case when isnull(@Interval,5) <= 1 then datepart(hh,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
		,case when isnull(@Interval,5) =  0 then datepart(mi,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) else datepart(yy,dateadd(mi, @UTCOffset, UTCCollectionDateTime)) end
	order by UTCCollectionDateTime asc'
	
	SELECT @SQLString = REPLACE(@SQLString, '{compareTableName}', @compareTableName)
	SELECT @compareColumnName = CASE WHEN @compareColumnName not like @regex
				THEN 'S.' + @compareColumnName
				ELSE @compareColumnName 
				END
	SELECT @SQLString = REPLACE(@SQLString, '{compareColumnName}', @compareColumnName)
END

SELECT @SQLString = REPLACE(@SQLString, '{tableName}', @tableName)
SELECT @columnName = CASE WHEN @columnName not like @regex
				THEN 'S.' + @columnName
				ELSE @columnName 
				END				
SELECT @SQLString = REPLACE(@SQLString, '{columnName}', @columnName )

SET @ParmDefinition = N'@Interval int, @UTCOffset int, @ServerID int, @CompareSQLServerID int, @UTCStart DateTime, @UTCEnd DateTime, @CompareStartRange DateTime, @CompareEndRange DateTime, @MetricID int, @CompareMetricID int';

INSERT INTO @BStatistics (UTCCollectionDateTime, ServerName, MetricName, Unit, Mean, MetricValue, StdDeviation, [Min], [Max], [Count], [Template], [MetricID])
EXECUTE sp_executesql @SQLString, 
					  @ParmDefinition, 
					  @Interval = @Interval, 
					  @UTCOffset = @UTCOffset, 
					  @ServerID = @ServerID, 
					  @CompareSQLServerID = @CompareSQLServerID,
					  @UTCStart = @UTCStart, 
					  @UTCEnd= @UTCEnd, 
					  @CompareStartRange = @CompareStartRange, 
					  @CompareEndRange = @CompareEndRange, 
					  @MetricID = @MetricID,
					  @CompareMetricID = @CompareMetricID

--Need to escalate the units in order to avoid the values to split into 2 lines on the grid.
SELECT 
	[UTCCalculation] = [UTCCollectionDateTime],
	[ServerName],
    [MetricName],
	[Unit] = (SELECT CASE Unit
					When 'KB' THEN 'MB'
					When 'MB' THEN 'GB'
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 'Milliseconds' ELSE 'Seconds' END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 'Seconds' ELSE 'Minutes' END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE Unit
					END),
    [Mean]= Mean / (SELECT CASE Unit 
					When 'KB' THEN 1024
					When 'MB' THEN 1024
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 1 ELSE 1000 END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 1 ELSE 60 END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE 1
			END),
    [MetricValue] = [MetricValue] / (SELECT CASE Unit 
					When 'KB' THEN 1024
					When 'MB' THEN 1024
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 1 ELSE 1000 END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 1 ELSE 60 END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE 1
			END),
    [StdDeviation] = [StdDeviation] / (SELECT CASE Unit 
					When 'KB' THEN 1024
					When 'MB' THEN 1024
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 1 ELSE 1000 END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 1 ELSE 60 END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE 1
			END),
    [Min] = [Min] / (SELECT CASE Unit 
					When 'KB' THEN 1024
					When 'MB' THEN 1024
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 1 ELSE 1000 END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 1 ELSE 60 END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE 1
			END),
    [Max] = [Max] / (SELECT CASE Unit 
					When 'KB' THEN 1024
					When 'MB' THEN 1024
					When 'Milliseconds' THEN CASE When MetricID = 22 OR MetricID = -93 OR MetricID = -100 OR MetricID = -101 THEN 1 ELSE 1000 END -- We need to add Response Time, VM CPU Swap Wait, VM CPU Ready to the exception
					When 'Seconds' THEN CASE When MetricID = 76 OR MetricID = 17 THEN 1 ELSE 60 END -- We need to add PageLifeExpectancy and Replication Latency to the exception
					ELSE 1
			END),
	[Count],
	Template,
	MetricID
FROM @BStatistics 
ORDER BY [UTCCalculation] ASC, [ServerName] ASC


END
