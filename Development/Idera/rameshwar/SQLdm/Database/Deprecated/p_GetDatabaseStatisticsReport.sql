if (object_id('[p_GetDatabaseStatisticsReport]') is not null)
begin
drop procedure p_GetDatabaseStatisticsReport
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
create procedure [dbo].p_GetDatabaseStatisticsReport
	@XmlDocument nText,
	@ServerId int,
	@Interval tinyint
as
begin
declare @xmlDoc int
set ansi_warnings off

declare @Databases table(
		DatabaseID int,
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

-- Extract the DatabaseNames from the XML doc and marry with the ID.
insert into @Databases	
select	names.DatabaseID, xmldoc.DatabaseName 
from openxml(@xmlDoc, '//Database', 1) with (DatabaseName nvarchar(255)) as xmldoc
	left join SQLServerDatabaseNames names (nolock) on 
		names.SQLServerID = @ServerId and
		names.DatabaseName collate database_default = xmldoc.DatabaseName collate database_default
where xmldoc.DatabaseName is not null

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
	,sum(convert(float,ds.Transactions)) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0) as Transactions
	,sum(convert(float,ds.LogFlushWaits)) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0) as LogFlushWaits
	,sum(convert(float,ds.LogFlushes)) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0) as LogFlushes
	,sum(convert(float,LogKilobytesFlushed)) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0) as LogKilobytesFlushed
	,sum(convert(float,LogCacheReads)) / nullif((sum(convert(float,TimeDeltaInSeconds)) / 60) ,0) as LogCacheReads
	,sum(LogCacheHitRatio/100.0 * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as LogCacheHitRatio
from
	@Databases names 
	join [DatabaseStatistics] ds (nolock)
	on names.[DatabaseID] = ds.[DatabaseID]
	join [#DateRange] dates
	on ds.[UTCCollectionDateTime] BETWEEN dates.[UtcStart] and dates.[UtcEnd]
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
	max(ds.[UTCCollectionDateTime])

drop table #DateRange

end
