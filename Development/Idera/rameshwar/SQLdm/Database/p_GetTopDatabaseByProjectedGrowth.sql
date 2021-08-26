IF (OBJECT_ID('p_GetTopDatabaseByProjectedGrowth') IS NOT NULL)
BEGIN
  DROP PROC p_GetTopDatabaseByProjectedGrowth
END
GO
-- p_GetTopDatabaseByProjectedGrowth 20,null,1000
CREATE  PROCEDURE [dbo].[p_GetTopDatabaseByProjectedGrowth]
	@TopX INT = NULL,
	@UTCSnapshotCollectionDateTime datetime = null,
	@HistoryInDays int = null
AS
BEGIN	
declare @err int

declare @BeginDateTime datetime
declare @EndDateTime datetime

SELECT * INTO #DS FROM
(
	SELECT DatabaseID,
		UTCCollectionDateTime,
		DataSizeInKilobytes,
		IndexSizeInKilobytes,
		TextSizeInKilobytes
	FROM DatabaseSize
	
	UNION ALL
	
	SELECT DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalDataSizeInKilobytes AS DataSizeInKilobytes,
		TotalIndexSizeInKilobytes AS IndexSizeInKilobytes,
		TotalTextSizeInKilobytes AS TextSizeInKilobytes
	FROM DatabaseSizeAggregation
) AS DSAggregated;

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max(UTCCollectionDateTime) from [#DS])
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInDays is null)
	select @BeginDateTime = @EndDateTime;
else
	select @BeginDateTime = dateadd(DAY, -@HistoryInDays, @EndDateTime);

IF @TopX IS NOT null AND @TopX > 0
  SET ROWCOUNT @TopX;
  
with DBCollectionRange (SQLServerID, InstanceName, DatabaseID, DatabaseName, MaxCollectionTime, MinCollectionTime) as (
	select MS.SQLServerID, MS.InstanceName, DS.DatabaseID, names.DatabaseName, MAX(UTCCollectionDateTime), MIN(UTCCollectionDateTime)
	from #DS DS (nolock)
		Join [SQLServerDatabaseNames] names (nolock) 
			on DS.DatabaseID = names.DatabaseID
		Join MonitoredSQLServers MS (nolock)
			on names.SQLServerID = MS.SQLServerID
			   and MS.Active = 1
	where UTCCollectionDateTime >= @BeginDateTime
	group by MS.SQLServerID, MS.InstanceName, DS.DatabaseID, names.DatabaseName
)

select
	dcr.[SQLServerID],
	dcr.[InstanceName],
	dcr.[DatabaseID],
	dcr.[DatabaseName],	
	(
	    (select (isnull(DataSizeInKilobytes,0) + isnull(IndexSizeInKilobytes,0) + isnull(TextSizeInKilobytes,0)) as TotalSizeKb 
	    from #DS ds1
		where ds1.DatabaseID = dcr.DatabaseID
      		  and ds1.UTCCollectionDateTime = dcr.MaxCollectionTime)
          -
          (select (isnull(DataSizeInKilobytes,0) + isnull(IndexSizeInKilobytes,0) + isnull(TextSizeInKilobytes,0)) as TotalSizeKb 
	    from #DS ds1
		where ds1.DatabaseID = dcr.DatabaseID
      		  and ds1.UTCCollectionDateTime = dcr.MinCollectionTime)
	) as TotalSizeDiffKb,
	dcr.MaxCollectionTime as UTCCollectionDateTime,
	dcr.MinCollectionTime
from
	DBCollectionRange dcr
ORDER BY TotalSizeDiffKb desc
	
	select @err = @@error
return @err

DROP TABLE #DS
END
