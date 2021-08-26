if (object_id('p_UpdateStatisticsRunTime') is not null)
begin
drop procedure p_UpdateStatisticsRunTime
end
go
create procedure p_UpdateStatisticsRunTime
@SQLServerID int,
@LastGrowthStatisticsRunTime datetime = null,
@LastGrowthStatisticsRunTimeUTC datetime = null,
@LastReorgStatisticsRunTime datetime = null,
@LastReorgStatisticsRunTimeUTC datetime = null
as 
begin

	update MonitoredSQLServers 
	set 
		[LastGrowthStatisticsRunTime] = isnull(@LastGrowthStatisticsRunTime,[LastGrowthStatisticsRunTime]) ,
		[LastGrowthStatisticsRunTimeUTC] = isnull(@LastGrowthStatisticsRunTimeUTC,[LastGrowthStatisticsRunTimeUTC]) ,
		[LastReorgStatisticsRunTime] = isnull(@LastReorgStatisticsRunTime,[LastReorgStatisticsRunTime]) ,
		[LastReorgStatisticsRunTimeUTC] = isnull(@LastReorgStatisticsRunTimeUTC,[LastReorgStatisticsRunTimeUTC])
	where [SQLServerID] = @SQLServerID

end
