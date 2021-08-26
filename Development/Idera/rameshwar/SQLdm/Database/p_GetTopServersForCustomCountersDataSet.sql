IF (OBJECT_ID('p_GetTopServersForCustomCountersDataSet') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetTopServersForCustomCountersDataSet
END
GO
--exec p_GetTopServersForCustomCountersDataSet 3,'2015-09-01 14:10:19.983','2015-09-10 14:10:19.983','r7'
CREATE proc [dbo].[p_GetTopServersForCustomCountersDataSet]
(
--START: SP Params
@NumServers INT, --= 3;
@UTCStart DateTime, --= '2015-09-01 14:10:19.983';
@UTCEnd DateTime, --= '2015-09-10 14:10:19.983';
@reportName nvarchar(255) --= 'fifth'
--END: SP Params
) AS
BEGIN

	DECLARE @maxCounterTemplate nvarchar(500)
	DECLARE @perMinuteCounterTemplate nvarchar(500)
	DECLARE @counterSQLWeightedAverage nvarchar(max)
	DECLARE @counterSQLMax nvarchar(max)
	DECLARE @counterSQLPerMinute nvarchar(max)
	DECLARE @ParmDefinition nvarchar(500)
	declare @weightedAverageCounterTemplate nvarchar(500)
	declare @virtualizationWeightedAverageCounterTemplate nvarchar(500)
	Declare @virtualizationPerMinuteCounterTemplate nvarchar(500)
	DECLARE @counterSource int
	declare @counterAggregation int
	declare @counterName nvarchar(256)
	declare @counterID int
	DECLARE @tempQuery nvarchar(max)
	

	--final table which would be returned by this sp having metrics info containing top servers
	IF OBJECT_ID('tempdb..#resultTable') IS NOT NULL
	drop table #resultTable
	create table #resultTable(SQLServerID int,InstanceName nvarchar(256), CounterName nvarchar(256), Value float)

	--for aggregation type = 0, and non custom counters
	select @maxCounterTemplate = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' round(max(convert(float,{prefix}.{metric})),2) as Value'
	+ ' from  {prefix} (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on {prefix}.SQLServerID = m.SQLServerID'
	+ ' {getTimeDeltaInSeconds}' --template for the counters whose value need 'TimeDeltaInSeconds' during expression calculation
	+ ' where {prefix}.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	--for aggregation type = 2, and server,os counters
	select @perMinuteCounterTemplate = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' round(sum(convert(float,{prefix}.{metric})) / nullif((sum(convert(float,case when {prefix}.{metric} is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0),2) as Value'
	+ ' from  {prefix} (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on {prefix}.SQLServerID = m.SQLServerID'
	+ ' {getTimeDeltaInSeconds}'
	+ ' where {prefix}.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	--for aggregation type = 1, and virtual counters
	select @virtualizationPerMinuteCounterTemplate = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' round(avg(convert(float,{prefix}.{metric})),2) as Value'
	+ ' from  {prefix} (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on {prefix}.SQLServerID = m.SQLServerID'
	+ ' {getTimeDeltaInSeconds}'
	+ ' where {prefix}.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	--for aggregation type = 1, and custom counters
	select @counterSQLWeightedAverage = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' sum(((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) * ccs.TimeDeltaInSeconds) / nullif(sum(case when ccs.RawValue is not null then ccs.TimeDeltaInSeconds else 0 end),0) as Value ' 
	+ ' from  CustomCounterStatistics ccs (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
	+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
	+ ' where ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' and ccs.MetricID = @CounterID'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	--for aggregation type = 0, and custom counters
	select @counterSQLMax = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' max((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) as Value ' 
	+ ' from  CustomCounterStatistics ccs (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
	+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
	+ ' where ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' and ccs.MetricID = @CounterID'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	--for aggregation type = 2, and custom counters
	select @counterSQLPerMinute = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
	+ ' @CounterName as CounterName,'
	+ ' sum(convert(float,((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale))) / nullif((sum(convert(float,case when ((case ccd.CalculationType when 0 then ccs.RawValue else ccs.DeltaValue end) * ccd.Scale) is not null then TimeDeltaInSeconds else 0 end)) / 60) ,0) as Value ' 
	+ ' from  CustomCounterStatistics ccs (nolock)'
	+ ' inner join MonitoredSQLServers m (nolock) on ccs.SQLServerID = m.SQLServerID'
	+ ' inner join CustomCounterDefinition ccd (nolock) on ccd.Metric = ccs.MetricID'
	+ ' where ccs.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
	+ ' and ccs.MetricID = @CounterID'
	+ ' group by m.SQLServerID,m.InstanceName'
	+ ' order by Value desc'

	SET @ParmDefinition = N'@UTCStart DateTime, @UTCEnd DateTime, @CounterName nvarchar(50), @CounterID int';


	declare customCounterCursor insensitive cursor for

	with DistinctMetrics(Metric, Name)as (
	select Metric = min(Metric),Name
	from MetricInfo
	group by Name
	)  

	select mi.Metric, crc.CounterName, crc.Aggregation, crc.Source
	from CustomReports cr inner join CustomReportsCounters crc on cr.ID = crc.ID
	left join DistinctMetrics mi on crc.CounterName = replace(mi.Name,' ','_') 
	where reportName = @reportName
	order by GraphNumber asc

	--NOTE: For Only Custom Counters, we want MetricID but for others it will have null value

	open customCounterCursor 
	fetch next from customCounterCursor into @counterID,@counterName, @counterAggregation, @counterSource

		declare @metricDeclaration nvarchar(80)
		declare @averageDivisor nvarchar(150)
	
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
		
			--for aggregation type = 1, and os,servers counters
			select @weightedAverageCounterTemplate = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
			+ ' @CounterName as CounterName,'
			+ ' round(sum(' + @metricDeclaration + ' * TimeDeltaInSeconds) / ' + @averageDivisor  + ' else 0 end),0),2) as Value'
			+ ' from  {prefix} (nolock)'
			+ ' inner join MonitoredSQLServers m (nolock) on {prefix}.SQLServerID = m.SQLServerID'
			+ ' {getTimeDeltaInSeconds}'
			+ ' where {prefix}.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
			+ ' group by m.SQLServerID,m.InstanceName'
			+ ' order by Value desc'
		
		
			--for aggregation type = 1, and virtual counters  
			select @virtualizationWeightedAverageCounterTemplate = 'select top '+CONVERT(nvarchar(10),@NumServers)+' m.SQLServerID,m.InstanceName,'
			+ ' @CounterName as CounterName,'
			+ ' round(avg(convert(float,{prefix}.{metric})),2) as Value'
			+ ' from  {prefix} (nolock)'
			+ ' inner join MonitoredSQLServers m (nolock) on {prefix}.SQLServerID = m.SQLServerID'
			+ ' {getTimeDeltaInSeconds}'
			+ ' where {prefix}.[UTCCollectionDateTime] between @UTCStart and @UTCEnd'
			+ ' group by m.SQLServerID,m.InstanceName'
			+ ' order by Value desc'

		
			if @counterSource = 0 --Server
			begin
				if @counterAggregation = 0--max
				begin
					select @tempQuery = replace(replace(replace(@maxCounterTemplate,'{prefix}','ServerStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')		
					insert into #resultTable						
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID
				end
				if(@counterAggregation = 1)--weighted average
				begin
					select @tempQuery = replace(replace(replace(@weightedAverageCounterTemplate,'{prefix}','ServerStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')	
					insert into #resultTable						
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID				
				end
				if @counterAggregation = 2--per minute
				begin
					select @tempQuery = replace(replace(replace(@perMinuteCounterTemplate,'{prefix}','ServerStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')	
					insert into #resultTable						
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID								
				end
			end
		
		
			if @counterSource = 1 --OS
			begin
				if @counterAggregation = 0--max
				begin
					select @tempQuery = replace(replace(replace(@maxCounterTemplate,'{prefix}','OSStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')	
					insert into #resultTable			
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID
				end
				if(@counterAggregation = 1)--weighted average
				begin
					select @tempQuery = replace(@weightedAverageCounterTemplate,'{metric}',@counterName)
					select @tempQuery = REPLACE(@tempQuery,'{getTimeDeltaInSeconds}',' right join ServerStatistics on ServerStatistics.[SQLServerID] = {prefix}.[SQLServerID] and ServerStatistics.[UTCCollectionDateTime] = {prefix}.[UTCCollectionDateTime]')
					select @tempQuery = REPLACE(@tempQuery,'{prefix}','OSStatistics')									
					insert into #resultTable			
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID			
				end
				if @counterAggregation = 2--per minute
				begin				
					select @tempQuery = replace(@perMinuteCounterTemplate,'{metric}',@counterName)
					select @tempQuery = REPLACE(@tempQuery,'{getTimeDeltaInSeconds}',' right join ServerStatistics on ServerStatistics.[SQLServerID] = {prefix}.[SQLServerID] and ServerStatistics.[UTCCollectionDateTime] = {prefix}.[UTCCollectionDateTime]')
					select @tempQuery = REPLACE(@tempQuery,'{prefix}','OSStatistics')
					insert into #resultTable			
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID							
				end
			end
		
			if @counterSource = 2 --Custom
			begin
				if @counterAggregation = 0--max
				begin
					insert into #resultTable		
					execute sp_executesql @counterSQLMax, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID
				end
				if(@counterAggregation = 1)--weighted average
				begin
					insert into #resultTable			
					execute sp_executesql @counterSQLWeightedAverage, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID			
				end
				if @counterAggregation = 2--per minute
				begin
					insert into #resultTable		
					execute sp_executesql @counterSQLPerMinute, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID								
				end
			end
		
			if @counterSource = 3 --Virtualization
			begin
				if @counterAggregation = 0--max
				begin			
					select @tempQuery = replace(replace(replace(@maxCounterTemplate,'{prefix}','VMStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')		
					insert into #resultTable	
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID
				end
				if(@counterAggregation = 1)--weighted average
				begin
					select @tempQuery = replace(replace(replace(@virtualizationWeightedAverageCounterTemplate,'{prefix}','VMStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')				
					insert into #resultTable
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID				
				end
				if @counterAggregation = 2--per minute
				begin
					select @tempQuery = replace(replace(replace(@virtualizationPerMinuteCounterTemplate,'{prefix}','VMStatistics'),'{metric}',@counterName),'{getTimeDeltaInSeconds}','')		
					insert into #resultTable
					execute sp_executesql @tempQuery, @ParmDefinition, @UTCStart = @UTCStart, @UTCEnd = @UTCEnd, @CounterName = @counterName, @CounterID = @counterID								
				end
			end
		
			fetch next from customCounterCursor
			into @counterID, @counterName,@counterAggregation, @counterSource
		end--while loop ends here
	close customCounterCursor
	deallocate customCounterCursor

	select SQLServerID,InstanceName,CounterName,ISNULL(Value,-1) AS Value from #resultTable

end--end of proc
