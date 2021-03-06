/****** Object:  StoredProcedure [dbo].[p_GetCustomReportsDataSet]    Script Date: 12-05-2014 16:38:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF (OBJECT_ID('p_GetCustomReportsDataSet') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetCustomReportsDataSet
END
GO

CREATE proc [dbo].[p_GetCustomReportsDataSet](@UTCOffset int,
@Interval int,
@ServerID int,
@UTCStart DateTime,
@UTCEnd DateTime,
@reportName nvarchar(255))
as
begin
--SQLdm 10.1.3 (Varun Chopra): SQLDM-22899 support multiple Custom Counters in report
IF OBJECT_ID('tempdb..#tempCustomCounterTable') IS NOT NULL
drop table #tempCustomCounterTable
IF OBJECT_ID('tempdb..#ReportsCounters') IS NOT NULL
drop table #ReportsCounters


DECLARE @NonVirtualizationSQLString nvarchar(max); --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - Changed the variable name from @SQLString
DECLARE @ParmDefinition nvarchar(500);
declare @counterID int
declare @counterName nvarchar(256)
declare @counterAggregation int
declare @counterSource int
declare @counterSQLWeightedAverage nvarchar(2000)
declare @counterSQLMax nvarchar(2000)
declare @counterSQLPerMinute nvarchar(2000)

declare @maxCounterTemplate nvarchar(500)
declare @weightedAverageCounterTemplate nvarchar(500)
declare @perMinuteCounterTemplate nvarchar(500)
declare @nonCustomCounterSelect nvarchar(4000)
declare @finalSelectMetrics nvarchar(1000)

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
Declare @VirtualizationSQLString nvarchar(max)
Declare @virtualizationPerMinuteCounterTemplate nvarchar(500)
declare @virtualizationWeightedAverageCounterTemplate nvarchar(500)
declare @VirtualizationCounterSelect nvarchar(4000)
declare @virtualizationFinalSelectMetrics nvarchar(1000)
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here




select @maxCounterTemplate = 'round(max(convert(float,{prefix}.{metric})),2) as {metric}, '
--Moved below
--select @weightedAverageCounterTemplate = 'round(sum({prefix}.{metric} * TimeDeltaInSeconds) / nullif(sum(case when {prefix}.{metric} is not null then TimeDeltaInSeconds else 0 end),0),2) as {metric}, '
select @perMinuteCounterTemplate = 'round(sum(convert(float,{prefix}.{metric})) / nullif((sum(convert(float,case when {prefix}.{metric} is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0),2) as {metric}, '
select @nonCustomCounterSelect = ''
select @finalSelectMetrics = ''

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
select @virtualizationPerMinuteCounterTemplate='round(avg(convert(float,{prefix}.{metric})),2) as {metric}, '
select @VirtualizationCounterSelect=''
select @virtualizationFinalSelectMetrics=''
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here

set nocount on

--declare @UTCOffset int
--declare @Interval int
--declare @ServerID int
--declare @UTCStart DateTime
--declare @UTCEnd DateTime
--declare @reportName nvarchar(255)
--select @reportName = 'Report1'
--select @UTCOffset = -6
--select @Interval = 0
--select @ServerID = 1
--select @UTCStart = '1/1/1900'
--select @UTCEnd = '1/1/2011'

if not exists(select * from CustomReports where reportName = @reportName)
return
select @counterSQLWeightedAverage = 'select m.InstanceName,'
+ ' dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))) as [LastCollectioninInterval],'
+ ' ccs.MetricID, '
+ ' sum(((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) * ccs.TimeDeltaInSeconds) / nullif(sum(case when ccs.RawValue is not null then ccs.TimeDeltaInSeconds else 0 end),0) as Value ' 
+ ' from  CustomCounterStatistics ccs (nolock)'
+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
+ ' where ccs.[SQLServerID] = @ServerID and ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
+ ' and ccs.MetricID = @CounterID'
+ ' group by [InstanceName] , ccs.MetricID,' 
+ ' datepart(yy, dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))'
+ ' ,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'

select @counterSQLMax = 'select m.InstanceName,'
+ ' dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))) as [LastCollectioninInterval],'
+ ' ccs.MetricID, '
+ ' max((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) as Value ' 
+ ' from  CustomCounterStatistics ccs '
+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
+ ' where ccs.[SQLServerID] = @ServerID and ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
+ ' and ccs.MetricID = @CounterID'
+ ' group by [InstanceName] , ccs.MetricID,' 
+ ' datepart(yy, dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))'
+ ' ,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'

select @counterSQLPerMinute = 'select m.InstanceName,'
+ ' dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))) as [LastCollectioninInterval],'
+ ' ccs.MetricID, '
+ ' sum(convert(float,((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale))) / nullif((sum(convert(float,case when ((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as Value ' 
+ ' from  CustomCounterStatistics ccs (nolock)'
+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
+ ' where ccs.[SQLServerID] = @ServerID and ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
+ ' and ccs.MetricID = @CounterID'
+ ' group by [InstanceName] , ccs.MetricID,' 
+ ' datepart(yy, dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime]))'
+ ' ,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ccs.[UTCCollectionDateTime])) end'

SET @ParmDefinition = N'@Interval int, @UTCOffset int, @ServerID int, @UTCStart DateTime, @UTCEnd DateTime, @CounterID int';

--SQLdm 10.1.3 (Varun Chopra): SQLDM-22899 support multiple Custom Counters in report
create table #ReportsCounters(InstanceName nvarchar(256), LastCollectioninInterval DateTime)
insert into #ReportsCounters
select m.InstanceName 
,dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval]
from [MonitoredSQLServers] m (nolock)
left join [ServerStatistics] s1 (nolock)
on m.[SQLServerID] = s1.[SQLServerID]
left join [OSStatistics] o (nolock)
on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]
where s1.[SQLServerID] = @ServerID and s1.[UTCCollectionDateTime] between @UTCStart and @UTCEnd
group by [InstanceName] 
,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))
,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end
,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end

declare customCounterCursor insensitive cursor for

with DistinctMetrics(Metric, Name)as (
 select Metric = min(Metric),Name
 from MetricInfo
 group by Name
)  

select mi.Metric, crc.CounterName, crc.Aggregation, crc.Source
 from CustomReports cr inner join CustomReportsCounters crc on cr.ID = crc.ID
left join DistinctMetrics mi on crc.CounterName = replace(mi.Name,' ','_') 
where reportName = @reportName-- and Source = 2
order by GraphNumber asc

open customCounterCursor 
fetch next from customCounterCursor into @counterID, @counterName, @counterAggregation, @counterSource

declare @metricDeclaration nvarchar(80)
declare @averageDivisor nvarchar(150)

declare @countOfCustomGraphs int
select @countOfCustomGraphs = 0

--SQLdm 10.1.3 (Varun Chopra): SQLDM-22899 support multiple Custom Counters in report
create table #tempCustomCounterTable(InstanceName nvarchar(256), LastCollectioninInterval DateTime, metricid int, metricvalue decimal);

while @@fetch_status = 0
begin

-- If Metric SQLMemoryUsagePercent then we do not want to look for that column on ServerStatistics table.
if (@counterName = 'SQLMemoryUsagePercent') 
begin
	set @metricDeclaration = '{prefix}.SqlMemoryUsedInKilobytes * 100 / {prefix}.TotalServerMemoryInKilobytes'
	set @averageDivisor = 'nullif(sum(case when {prefix}.SqlMemoryUsedInKilobytes is not null AND {prefix}.TotalServerMemoryInKilobytes is not null then TimeDeltaInSeconds'
end
else
begin
	set @metricDeclaration = '{prefix}.{metric}'
	set @averageDivisor = 'nullif(sum(case when {prefix}.{metric} is not null then TimeDeltaInSeconds'
end

select @weightedAverageCounterTemplate = 'round(sum(' + @metricDeclaration + ' * TimeDeltaInSeconds) / ' + @averageDivisor  + ' else 0 end),0),2) as {metric}, '
select @virtualizationWeightedAverageCounterTemplate= 'round(avg(convert(float,{prefix}.{metric})),2) as {metric}, '  ---SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - added newly

if @counterSource = 0 --Server
begin
select @finalSelectMetrics = @finalSelectMetrics + 'nonCustom.[' + @counterName + '], '
if @counterAggregation = 0--max
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@maxCounterTemplate,'{prefix}','s1'),'{metric}',@counterName)
end
if(@counterAggregation = 1)--weighted average
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@weightedAverageCounterTemplate,'{prefix}','s1'),'{metric}',@counterName)
end
if @counterAggregation = 2--per minute
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@perMinuteCounterTemplate,'{prefix}','s1'),'{metric}',@counterName)
end
end
if @counterSource = 1 -- os
begin
select @finalSelectMetrics = @finalSelectMetrics + 'nonCustom.[' + @counterName + '], '
if @counterAggregation = 0--max
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@maxCounterTemplate,'{prefix}','o'),'{metric}',@counterName)
end
if(@counterAggregation = 1)--weighted average
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@weightedAverageCounterTemplate,'{prefix}','o'),'{metric}',@counterName)
end
if @counterAggregation = 2--per minute
begin
select @nonCustomCounterSelect = @nonCustomCounterSelect + replace(replace(@perMinuteCounterTemplate,'{prefix}','o'),'{metric}',@counterName)
end
end

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
if @counterSource = 3 -- Virtualization
begin
select @virtualizationFinalSelectMetrics = @virtualizationFinalSelectMetrics + 'virt.[' + @counterName + '], '
if @counterAggregation = 0--max
begin
select @VirtualizationCounterSelect = @VirtualizationCounterSelect + replace(replace(@maxCounterTemplate,'{prefix}','vm'),'{metric}',@counterName)
end
if(@counterAggregation = 1)--weighted average
begin
select @VirtualizationCounterSelect = @VirtualizationCounterSelect + replace(replace(@virtualizationWeightedAverageCounterTemplate,'{prefix}','vm'),'{metric}',@counterName)
end
if @counterAggregation = 2--per minute
begin
select @VirtualizationCounterSelect = @VirtualizationCounterSelect + replace(replace(@virtualizationPerMinuteCounterTemplate,'{prefix}','vm'),'{metric}',@counterName)
end
end
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here

if @counterSource = 2 --custom    
begin
select @finalSelectMetrics = @finalSelectMetrics + 'custom.[' + @counterName + '], '

--SQLdm 10.1.3 (Varun Chopra): SQLDM-22899 support multiple Custom Counters in report - begins here
execute('alter table #ReportsCounters add [' + @counterName + '] decimal')

if @counterAggregation = 0--max
begin
insert into #tempCustomCounterTable
EXECUTE sp_executesql @counterSQLMax, @ParmDefinition, @Interval = @Interval, @UTCOffset=@UTCOffset, @ServerID = @ServerID, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterID = @counterID
end
if(@counterAggregation = 1)--weighted average
begin
insert into #tempCustomCounterTable
EXECUTE sp_executesql @counterSQLWeightedAverage, @ParmDefinition, @Interval = @Interval, @UTCOffset=@UTCOffset, @ServerID = @ServerID, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterID = @counterID
end
if @counterAggregation = 2--per minute
begin
insert into #tempCustomCounterTable
EXECUTE sp_executesql @counterSQLPerMinute, @ParmDefinition, @Interval = @Interval, @UTCOffset=@UTCOffset, @ServerID = @ServerID, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterID = @counterID
end

exec('UPDATE reports
SET
    reports.' + @counterName + ' = tcct.metricvalue
FROM
    #ReportsCounters AS reports
    INNER JOIN #tempCustomCounterTable AS tcct
    ON reports.LastCollectioninInterval = tcct.LastCollectioninInterval')

-- Refresh table for next custom counter in iteration
DELETE FROM #tempCustomCounterTable;

--SQLdm 10.1.3 (Varun Chopra): SQLDM-22899 support multiple Custom Counters in report - ends here

select @countOfCustomGraphs = @countOfCustomGraphs + 1
end --end of source = 2 (custom)

fetch next from customCounterCursor
into @counterID, @counterName,@counterAggregation, @counterSource
end

close customCounterCursor
deallocate customCounterCursor

IF OBJECT_ID('tempdb..#tempCustomCounterTable') IS NOT NULL
drop table #tempCustomCounterTable
select @nonCustomCounterSelect = substring(@nonCustomCounterSelect,0,len(@nonCustomCounterSelect))
select @finalSelectMetrics = substring(@finalSelectMetrics,0,len(@finalSelectMetrics))

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
select @VirtualizationCounterSelect=SUBSTRING(@VirtualizationCounterSelect,0,len(@VirtualizationCounterSelect))
select @virtualizationFinalSelectMetrics=substring(@virtualizationFinalSelectMetrics,0,len(@virtualizationFinalSelectMetrics))
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here

declare @NumberOfCustomGraphs int
select @NumberOfCustomGraphs = @countOfCustomGraphs

select @NonVirtualizationSQLString = '' 
select @VirtualizationSQLString =''


SET @ParmDefinition = N'@Interval int, @UTCOffset int, @ServerID int, @UTCStart DateTime, @UTCEnd DateTime';

if(Len(@finalSelectMetrics) >0)
select @NonVirtualizationSQLString = N'select nonCustom.InstanceName, nonCustom.LastCollectioninInterval, ' + @finalSelectMetrics + ' from ('
else
	select @NonVirtualizationSQLString = N'select nonCustom.InstanceName, nonCustom.LastCollectioninInterval from ('
select @NonVirtualizationSQLString = @NonVirtualizationSQLString + ' select m.InstanceName as InstanceName, '
+ ' dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))) as [LastCollectioninInterval], '

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
if(Len(@virtualizationFinalSelectMetrics) >0)
begin
select @VirtualizationSQLString = N'select virt.InstanceName, virt.LastCollectioninInterval, ' + @virtualizationFinalSelectMetrics + ' from ('
select @virtualizationFinalSelectMetrics= ' , '+@virtualizationFinalSelectMetrics
end
else
	select @VirtualizationSQLString = N'select virt.InstanceName, virt.LastCollectioninInterval from ('

select @VirtualizationSQLString = @VirtualizationSQLString + ' select m.InstanceName as InstanceName, '
+ ' dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime]))) as [LastCollectioninInterval], '
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here

if(len(@VirtualizationCounterSelect) > 0)
begin
	select @VirtualizationSQLString = @VirtualizationSQLString + @VirtualizationCounterSelect  --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - added newly
end
else 
begin
	select @VirtualizationSQLString = substring(@VirtualizationSQLString,0,len(@VirtualizationSQLString))  --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - added newly

end
if(len(@nonCustomCounterSelect) > 0)
begin
	select @NonVirtualizationSQLString = @NonVirtualizationSQLString + @nonCustomCounterSelect
end
else 
begin
	select @NonVirtualizationSQLString = substring(@NonVirtualizationSQLString,0,len(@NonVirtualizationSQLString))
end
	
select @NonVirtualizationSQLString = @NonVirtualizationSQLString + ' from [MonitoredSQLServers] m (nolock)'
+ ' left join [ServerStatistics] s1 (nolock)'
+ ' on m.[SQLServerID] = s1.[SQLServerID]'
+ ' left join [OSStatistics] o (nolock)'
+ ' on o.[SQLServerID] = s1.[SQLServerID] and o.[UTCCollectionDateTime] = s1.[UTCCollectionDateTime]'
+ ' where m.[SQLServerID] = @ServerID and s1.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
+ ' group by [InstanceName] '
+ ' ,datepart(yy, dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime]))'
+ ' ,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, s1.[UTCCollectionDateTime])) end'

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
select @VirtualizationSQLString = @VirtualizationSQLString + ' from [MonitoredSQLServers] m (nolock)'
+' left join [VMStatistics] vm (nolock) '
+' on vm.[SQLServerID] = m.[SQLServerID] '
+ ' where vm.[SQLServerID] = @ServerID and vm.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
+ ' group by [InstanceName] '
+ ' ,datepart(yy, dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime]))'
+ ' ,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end'
+ ' ,case when @Interval = 0 then datepart(mi,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, vm.[UTCCollectionDateTime])) end'
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here

select @NonVirtualizationSQLString = @NonVirtualizationSQLString + ') as nonCustom'
+ ' left join #ReportsCounters custom on custom.LastCollectioninInterval = nonCustom.LastCollectioninInterval and custom.InstanceName = nonCustom.InstanceName collate database_default ' --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - removed order by clause

select @VirtualizationSQLString = @VirtualizationSQLString + ') as virt'
+ ' left join #ReportsCounters custom on custom.LastCollectioninInterval = virt.LastCollectioninInterval and custom.InstanceName = virt.InstanceName collate database_default ' --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters -removed order by clause


--EXECUTE sp_executesql @SQLString, @ParmDefinition, @Interval = @Interval, @UTCOffset=@UTCOffset, @ServerID = @ServerID, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd --SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - commented this part

--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - starts here
declare @finalString nvarchar(max)
select @finalString='Select nonVirt.* '+@virtualizationFinalSelectMetrics+' from ( ' +@NonVirtualizationSQLString+' ) as nonVirt left Join ( '+@VirtualizationSQLString+' ) as virt '
+' on nonVirt.InstanceName=virt.InstanceName and nonVirt.LastCollectioninInterval=virt.LastCollectioninInterval '
+' order by nonVirt.LastCollectioninInterval asc'
EXECUTE sp_executesql @finalString, @ParmDefinition, @Interval = @Interval, @UTCOffset=@UTCOffset, @ServerID = @ServerID, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd
--SQLdm 8.5 (Ankit Srivastava): for Virtualization Counters - ends here


IF OBJECT_ID('tempdb..#ReportsCounters') IS NOT NULL
drop table #ReportsCounters



end -- end proc
 
