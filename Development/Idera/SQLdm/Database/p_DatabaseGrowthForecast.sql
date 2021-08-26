if (object_id('p_DatabaseGrowthForecast') is not null)
begin
drop procedure p_DatabaseGrowthForecast
end
go
CREATE PROCEDURE [dbo].[p_DatabaseGrowthForecast]
			@DatabaseXML nvarchar(4000),
			@ServerId int,
			@UTCStart DateTime,
			@UTCEnd DateTime,
			@UTCOffset int,
			@Interval tinyint
AS
BEGIN
--SQLdm10.2 SET NOCOUNT ON added to prevent extra result sets from
SET NOCOUNT ON;
declare @xmlDoc int
set ansi_warnings off

declare @Databases table(
		DatabaseName nvarchar(255)
) 

-- Prepare XML document
exec sp_xml_preparedocument @xmlDoc output, @DatabaseXML

-- Extract the server IDs from the SML doc.
insert into @Databases	
select
	DatabaseName 
from openxml(@xmlDoc, '//Database', 1)
	with (DatabaseName nvarchar(255))

exec sp_xml_removedocument @xmlDoc

if (select isnull(object_id('tempdb..#DS'), 0)) <> 0 
begin
	drop table #DS
end

SELECT * INTO #DS FROM
(
select 
	[DatabaseName],
	dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]))) as [LastCollectioninInterval],
	(sum(DataFileSizeInKilobytes * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0)) / 1024 as DataFileSizeMb,
	(sum((DataSizeInKilobytes + IndexSizeInKilobytes + TextSizeInKilobytes)* TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0)) / 1024 as TotalSizeMb,
	(sum((DataFileSizeInKilobytes + DataExpansionInKilobytes) * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0)) / 1024 as AvailableDataSizeMb,
	0 as ForecastFileSize,
	0 as ForecastDBSize
from
	[SQLServerDatabaseNames] names (nolock)
	left join [DatabaseSize] ds (nolock)
	on names.[DatabaseID] = ds.[DatabaseID]
where 
	-- Filter for SQL Server		
	names.[SQLServerID] = @ServerId
	and
	-- Filter databases		
	names.[DatabaseName] collate database_default in (select DatabaseName collate database_default from @Databases)
	and 
	dbo.fn_RoundDateTime(@Interval, ds.[UTCCollectionDateTime]) BETWEEN @UTCStart and @UTCEnd
group by
	[DatabaseName]
	-- Always group by year at the least
	,datepart(yy, dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime]))
	-- Group by all intervals greater than or equal to the selected interval
	,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[UTCCollectionDateTime])) end

UNION 

SELECT
	[DatabaseName],
	dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime]))) AS [LastCollectioninInterval],
	(sum(TotalDataFileSizeInKilobytes * TotalTimeDeltaInSeconds) / nullif(sum(TotalTimeDeltaInSeconds),0)) / 1024 as DataFileSizeMb,
	(sum((TotalDataSizeInKilobytes + TotalIndexSizeInKilobytes + TotalTextSizeInKilobytes)* TotalTimeDeltaInSeconds) / nullif(sum(TotalTimeDeltaInSeconds),0)) / 1024 as TotalSizeMb,
	(sum((TotalDataFileSizeInKilobytes + TotalDataExpansionInKilobytes) * TotalTimeDeltaInSeconds) / nullif(sum(TotalTimeDeltaInSeconds),0)) / 1024 as AvailableDataSizeMb,
	0 as ForecastFileSize,
	0 as ForecastDBSize

from [SQLServerDatabaseNames] names (nolock)

left join [DatabaseSizeAggregation] ds (nolock) on names.[DatabaseID] = ds.[DatabaseID]
where 
	-- Filter for SQL Server		
	names.[SQLServerID] = @ServerId
	and
	-- Filter databases		
	names.[DatabaseName] collate database_default in (select DatabaseName collate database_default from @Databases)
	and 
	dbo.fn_RoundDateTime(@Interval, ds.[MaxUTCCollectionDateTime]) BETWEEN @UTCStart and @UTCEnd
group by
	[DatabaseName]
	-- Always group by year at the least
	,datepart(yy, dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime]))
	-- Group by all intervals greater than or equal to the selected interval
	,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, ds.[MaxUTCCollectionDateTime])) end

) AS DSAggregation
order by
	DatabaseName
	,[LastCollectioninInterval];


SELECT [DatabaseName],
	[LastCollectioninInterval],
	[DataFileSizeMb],
	[TotalSizeMb] ,
	[AvailableDataSizeMb],
	[ForecastFileSize],
	[ForecastDBSize]
FROM #DS
ORDER BY DatabaseName

BEGIN
	drop table #DS
END

END
GO