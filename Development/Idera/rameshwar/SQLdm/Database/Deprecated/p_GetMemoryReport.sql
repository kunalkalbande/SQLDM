if (object_id('[p_GetMemoryReport]') is not null)
begin
drop procedure p_GetMemoryReport
end
go
-- @XmlDocument - SQLServerID AllowedDate parameters in XML format.
--  XmlDocument must contain at least one SQLServer and one date range.
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetMemoryReport
	@XmlDocument nText,
	@Interval tinyint
as
begin
declare @xmlDoc int
set ansi_warnings off

declare @SQLServers table(
		SQLServerID int) 

-- Create a temporary table to contain the date ranges.
-- A table variable won't work for some reason.
if (select isnull(object_id('tempdb..#DateRange'), 0)) = 0 
begin
	  create table #DateRange (
		UtcStart datetime, -- UTC start time of the range.
		UtcEnd datetime,   -- UTC end time of the range.
		UtcOffset int	   -- The UTC offset of times in this range (added to convert to client's TZ).
	  )
End

-- Prepare XML document
exec sp_xml_preparedocument @xmlDoc output, @XmlDocument

-- Extract the server IDs from the SML doc.
insert into @SQLServers	
select
	SQLServerID 
from openxml(@xmlDoc, '//SQLServer', 1)
	with (SQLServerID int)

-- Extract the datetime ranges from the XML doc.
-- These specify which records will be selected from the ServerStatistics table
-- and how to convert their timestamps to the client's local time zone.
insert into #DateRange
select 
	UtcStart, UtcEnd, UtcOffset
from openxml(@xmlDoc, '//AllowedDates', 1)
	with (
		UtcStart datetime, -- UTC start time of the range.
		UtcEnd datetime,   -- UTC end time of the range.
		UtcOffset int	   -- The UTC offset of times in this range (added to UTC time to convert to client's TZ).
	)

exec sp_xml_removedocument @xmlDoc

select 
		count(*) as [Records]
		,dbo.fn_RoundDateTime(@Interval, max(dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime]))) as [Last Collection in Interval]
		,[InstanceName]
		,max([SqlMemoryAllocatedInKilobytes]) as [SqlMemoryAllocatedInKilobytes]
		,max([SqlMemoryUsedInKilobytes]) as [SqlMemoryUsedInKilobytes]
		,max([ProcedureCacheSizeInKilobytes]) as [ProcedureCacheSizeInKilobytes]
		,max([ProcedureCacheSizePercent]) as [ProcedureCacheSizePercent]
		,sum([BufferCacheHitRatioPercentage] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as [BufferCacheHitRatioPercentage]
		,sum([ProcedureCacheHitRatioPercentage] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as [ProcedureCacheHitRatioPercentage]
		,sum(convert(float,[WorkTablesCreated])) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0)as [WorkTablesCreated]
		,sum([OSAvailableMemoryInKilobytes] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as [OSAvailableMemoryInKilobytes]
		,sum([OSTotalPhysicalMemoryInKilobytes] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as [OSTotalPhysicalMemoryInKilobytes]
		,sum([PageLifeExpectancy] * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as [PageLifeExpectancy]
	from
		ServerStatistics s
		left join [MonitoredSQLServers] m
		on m.[SQLServerID] = s.[SQLServerID]
		left join [OSStatistics] o
		on o.[SQLServerID] = s.[SQLServerID] and o.[UTCCollectionDateTime] = s.[UTCCollectionDateTime]
		left join [#DateRange] dates
		on s.[UTCCollectionDateTime] BETWEEN dates.[UtcStart] and dates.[UtcEnd]
	where( 
		-- Filter for SQL Server		
		s.[SQLServerID] in (select SQLServerID from @SQLServers)
		and
		-- Filter on the allowed date ranges
		EXISTS (
			select * 
			from
				#DateRange dates
			where
				s.[UTCCollectionDateTime] BETWEEN dates.UtcStart and dates.UtcEnd
		)
	) group by
		[InstanceName]
		-- Always group by year at the least
		,datepart(yy, dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,case when @Interval <= 3 then datepart(mm,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) end
		,case when @Interval <= 2 then datepart(dd,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) end
		,case when @Interval <= 1 then datepart(hh,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) end
		,case when @Interval =  0 then datepart(mi,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, s.[UTCCollectionDateTime])) end
	order by
		[InstanceName]
		,max(s.[UTCCollectionDateTime])

	drop table #DateRange
end
