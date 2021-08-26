IF (OBJECT_ID('p_GetDatabaseOverview') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetDatabaseOverview
END
GO

CREATE PROCEDURE [dbo].[p_GetDatabaseOverview]
				@ServerId int
AS
BEGIN

declare @Active bit
declare @DBN table(DatabaseID int, DatabaseName nvarchar(256), CreationDateTime datetime, UTCCollectionDateTime datetime, IsSystemDatabase bit,AlertsCount BIGINT)

select @Active = [Active] from MonitoredSQLServers (nolock) where [SQLServerID] = @ServerId

SELECT * INTO #DS FROM
(
	SELECT DatabaseID, UTCCollectionDateTime FROM DatabaseSize
	UNION ALL
	SELECT DatabaseID, MaxUTCCollectionDateTime AS UTCCollectionDateTime FROM DatabaseSizeAggregation
) AS DSAggregated;


if @Active = 1
begin
insert into @DBN
select distinct(ds.DatabaseID), dn.DatabaseName, dn.CreationDateTime, max(ds.UTCCollectionDateTime),dn.SystemDatabase [IsSystemDatabase], (SELECT COUNT(0) FROM Alerts WHERE DatabaseName = dn.DatabaseName) AlertsCount
from #DS ds (nolock) inner join SQLServerDatabaseNames dn (nolock) on ds.DatabaseID = dn.DatabaseID
where dn.SQLServerID = @ServerId
--This is being done because it is possible to refresh the report during a scheduled refresh and get a subset of the databases
--by doing this we get all databases that have been updated in the last 10 minutes of the latest refresh 
--and DATEDIFF(MI, ds.UTCCollectionDateTime ,(select MAX(ds.UTCCollectionDateTime) from DatabaseStatistics ds inner join SQLServerDatabaseNames dn2 on ds.DatabaseID = dn2.DatabaseID
--where dn2.SQLServerID = dn.SQLServerID)) < 10
--and ds.UTCCollectionDateTime = (select MAX(UTCCollectionDateTime) from DatabaseStatistics (nolock))
and ds.UTCCollectionDateTime= ( --SQLdm 10.2 (Tushar)--Fix for defect SQLDM-27644--Adding null check on aggregate function becasue without this query is going in infinite loop for sql server ids which dont have any data in DatabaseSize table.
select ISNULL(MAX(ds.UTCCollectionDateTime),NULL) from DatabaseSize ds (nolock) inner join SQLServerDatabaseNames dn2 (nolock) on ds.DatabaseID = dn2.DatabaseID
where dn2.SQLServerID = dn.SQLServerID)
group by ds.DatabaseID, dn.DatabaseName, dn.CreationDateTime,dn.SystemDatabase
end

select 
	[DatabaseName],
	[DatabaseStatus],
	dbn.[CreationDateTime],
	dbn.[UTCCollectionDateTime],
	(DataFileSizeInKilobytes + LogFileSizeInKilobytes) / 1024 as TotalSizeMb,
	--SQLdm 8.5 (Gaurav): Added to bring this new field in the response [START]
	dbn.[IsSystemDatabase],
	dbn.[AlertsCount]
	--SQLdm 8.5 (Gaurav): Added to bring this new field in the response [END]
from @DBN dbn
	join [DatabaseSize] ds (nolock)
	on  dbn.[DatabaseID] = ds.[DatabaseID] and
		dbn.[UTCCollectionDateTime] = ds.[UTCCollectionDateTime]
order by
	dbn.[DatabaseName],
	dbn.[UTCCollectionDateTime]

DROP TABLE #DS;
END
 
 
