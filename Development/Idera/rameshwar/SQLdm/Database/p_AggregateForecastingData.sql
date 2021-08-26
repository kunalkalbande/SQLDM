if (object_id('p_AggregateForecastingData') is not null)
begin
drop procedure p_AggregateForecastingData
end
go
CREATE procedure p_AggregateForecastingData
 as
begin
set nocount on
set ansi_warnings off

-- executing Database Size Aggregation Proc

print 'p_AggregateDatabaseSize'
exec p_AggregateDatabaseSize

-- executing Disk Drive Statistics Aggregation Proc
print 'p_AggregateDiskDriveStatisticsData'
exec p_AggregateDiskDriveStatisticsData

-- executing Database File Statistics Aggregation Proc
print 'p_AggregateDatabaseFileStatisticsData'
exec p_AggregateDatabaseFileStatisticsData

-- executing Table Growth Aggregation Proc
print 'p_AggregateTableGrowthData'
exec p_AggregateTableGrowthData

-- executing Databse Size DateTime Aggregation Proc
print 'p_AggregateDatabaseSizeDateTime'
exec p_AggregateDatabaseSizeDateTime

-- executing Databse statistics Aggregation Proc
print 'p_AggregateDatabaseStatistics'
exec p_AggregateDatabaseStatistics

end 
