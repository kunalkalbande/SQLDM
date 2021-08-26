if (object_id('[p_GetDatabaseGrowthReport]') is not null)
begin
drop procedure p_GetDatabaseGrowthReport
end
go
-- @XmlDocument - DatabaseNames and AllowedDates parameters in XML format.
--  XmlDocument must contain at least one DB name and one date range.
-- @ServerId - SQL Server instance ID.
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetDatabaseGrowthReport
	@XmlDocument nText,
	@ServerId int,
	@Interval tinyint
as
begin
declare @xmlDoc int
set ansi_warnings off

declare @Databases table(
		DatabaseName nvarchar(255)
) 

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
insert into @Databases	
select
	DatabaseName 
from openxml(@xmlDoc, '//Database', 1)
	with (DatabaseName nvarchar(255))

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
	[DatabaseName]
	,dbo.fn_RoundDateTime(@Interval, max(dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime]))) as [Last Collection in Interval]
	,sum(DataFileSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as DataFileSizeKb
	,sum(DataSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as DataSizeKb
	,sum(DataExpansionInKilobytes) as DataGrowth -- Placeholder to be filled in by the client
	,sum(PercentDataSize * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as DataSizePercent
	,sum(IndexSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as IndexSizeKb
	,sum(TextSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as TextSizeKb
	,sum(LogFileSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as LogFileSizeKb
	,sum((DataSizeInKilobytes + IndexSizeInKilobytes + TextSizeInKilobytes)* TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as TotalSizeKb
from
	[SQLServerDatabaseNames] names 
	join [DatabaseStatistics] ds
	on names.[DatabaseID] = ds.[DatabaseID]
	join [#DateRange] dates
	on ds.[UTCCollectionDateTime] BETWEEN dates.[UtcStart] and dates.[UtcEnd]
where 
	-- Filter for SQL Server		
	names.[SQLServerID] = @ServerId
	and
	-- Filter databases		
	names.[DatabaseName] collate database_default in (select DatabaseName collate database_default from @Databases)
group by
	[DatabaseName]
	-- Always group by year at the least
	,datepart(yy, dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime]))
	-- Group by all intervals greater than or equal to the selected interval
	,case when @Interval <= 3 then datepart(mm,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, ds.[UTCCollectionDateTime])) end
order by
	DatabaseName
	,[Last Collection in Interval]

drop table #DateRange



end
