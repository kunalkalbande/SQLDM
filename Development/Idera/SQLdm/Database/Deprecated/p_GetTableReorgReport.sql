if (object_id('[p_GetTableReorgReport]') is not null)
begin
drop procedure p_GetTableReorgReport
end
go
-- @XmlDocument - TableNames and AllowedDates parameters in XML format.
--  XmlDocument must contain at least one table name and one date range.
-- @ServerId - SQL Server instance ID.
-- @DbName - Database name.
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
create procedure [dbo].p_GetTableReorgReport
	@XmlDocument nText,
	@ServerId int,
	@DbName nvarchar(255),
	@Interval tinyint
as
begin
declare @xmlDoc int
set ansi_warnings off

declare @Tables table(
		TableName nvarchar(255)
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
insert into @Tables	
select
	TableName 
from openxml(@xmlDoc, '//Table', 1)
	with (TableName nvarchar(255))

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
	[TableName]
	,dbo.fn_RoundDateTime(@Interval, max(dateadd(hh, dates.UtcOffset, [UTCCollectionDateTime]))) as [Last Collection in Interval]
	,sum(ScanDensity * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as ScanDensity
	,sum(LogicalFragmentation * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as LogicalFragmentation
from
	[SQLServerDatabaseNames] dbnames (nolock)
	join [SQLServerTableNames] tnames (nolock)
	on dbnames.[DatabaseID] = tnames.[DatabaseID]
	join [TableReorganization] reorg (nolock)
	on tnames.[TableID] = reorg.[TableID]
	join [#DateRange] dates 
	on reorg.[UTCCollectionDateTime] BETWEEN dates.[UtcStart] and dates.[UtcEnd]
where 
	-- Filter for SQL Server		
	dbnames.[SQLServerID] = @ServerId
	and
	-- Filter for database		
	dbnames.[DatabaseName] = @DbName
	and
	-- Filter tables		
	tnames.[TableName] collate database_default in (select TableName collate database_default from @Tables)
group by
	[TableName]
	-- Always group by year at the least
	,datepart(yy, dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime]))
	-- Group by all intervals greater than or equal to the selected interval
	,case when @Interval <= 3 then datepart(mm,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) else datepart(yy,dateadd(hh, dates.UtcOffset, reorg.[UTCCollectionDateTime])) end
order by
	TableName
	,[Last Collection in Interval]

drop table #DateRange

end
