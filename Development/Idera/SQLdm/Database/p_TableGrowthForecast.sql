if (object_id('p_TableGrowthForecast') is not null)
begin
drop procedure p_TableGrowthForecast
end
go
CREATE PROCEDURE [dbo].[p_TableGrowthForecast]
			@TableXML nvarchar(4000),
			@Database nvarchar(256),
			@ServerId int,
			@UTCStart DateTime,
			@UTCEnd DateTime,
			@UTCOffset int,
			@Interval tinyint
AS
BEGIN
declare @xmlDoc int
set ansi_warnings off

declare @Tables table(
		TableName nvarchar(255)
) 

-- Prepare XML document
exec sp_xml_preparedocument @xmlDoc output, @TableXML

-- Extract the server IDs from the SML doc.
insert into @Tables	
select
	TableName 
from openxml(@xmlDoc, '//Table', 1)
	with (TableName nvarchar(255))

exec sp_xml_removedocument @xmlDoc

select 
	[SchemaName] + '.' + [TableName] as TableName,
	dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime]))) as [LastCollectioninInterval],
	sum((DataSize + [TextSize] + IndexSize) * TimeDeltaInSeconds) / nullif(sum(TimeDeltaInSeconds),0) as TotalSize
from
	[SQLServerDatabaseNames] dbnames (nolock)
	join [SQLServerTableNames] tnames (nolock)
	on dbnames.[DatabaseID] = tnames.[DatabaseID]
	join [TableGrowth] growth (nolock)
	on tnames.[TableID] = growth.[TableID]
where 
	-- Filter for SQL Server		
	dbnames.[SQLServerID] = @ServerId
	and
	-- Filter for database		
	dbnames.[DatabaseName] = @Database
	and
	-- Filter tables		
	tnames.[SchemaName] + '.' + tnames.[TableName] collate database_default in (select TableName collate database_default from @Tables)
	and
    growth.[UTCCollectionDateTime] BETWEEN @UTCStart and @UTCEnd
group by
	[TableName], [SchemaName]
	-- Always group by year at the least
	,datepart(yy, dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime]))
	-- Group by all intervals greater than or equal to the selected interval
	,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) end
	,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) end
	,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) end
	,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, growth.[UTCCollectionDateTime])) end
order by
	[TableName]
	,[LastCollectioninInterval] 
END
 
