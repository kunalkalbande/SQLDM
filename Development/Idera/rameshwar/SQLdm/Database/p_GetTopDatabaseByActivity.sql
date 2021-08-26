IF (OBJECT_ID('p_GetTopDatabaseByActivity') IS NOT NULL)
BEGIN
  DROP PROC [p_GetTopDatabaseByActivity]
END
GO
-- [p_GetTopDatabaseByActivity] 30
CREATE PROCEDURE [dbo].[p_GetTopDatabaseByActivity]
@TopX INT = NULL
AS
BEGIN

SELECT * INTO #DS FROM
(
	SELECT ds.DatabaseID,
		UTCCollectionDateTime,
		Transactions,
		TimeDeltaInSeconds,
		names.SQLServerID,
		names.DatabaseName,
		COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]
	FROM DatabaseStatistics ds (nolock)
	JOIN [SQLServerDatabaseNames] names (nolock) ON ds.DatabaseID = names.DatabaseID
	JOIN MonitoredSQLServers MS (nolock) ON names.SQLServerID = MS.SQLServerID
  WHERE MS.Active = 1

	UNION ALL

	SELECT ds.DatabaseID,
		MaxUTCCollectionDateTime AS UTCCollectionDateTime,
		TotalTransactions AS Transactions,
		TotalTimeDeltaInSeconds AS TimeDeltaInSeconds,
		names.SQLServerID,
		names.DatabaseName,
		COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName]
	FROM DatabaseStatisticsAggregation ds (nolock)
	JOIN [SQLServerDatabaseNames] names (nolock) ON ds.DatabaseID = names.DatabaseID
	JOIN MonitoredSQLServers MS (nolock) ON names.SQLServerID = MS.SQLServerID
  WHERE MS.Active = 1

) AS DSAggregation;

DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC)
	DECLARE @MetricID INT;
	SET @MetricID = 0; --this is the metric id for database activity
	
	INSERT INTO @Threshold EXEC p_PopulateMetricThresholds @MetricID;	

IF @TopX IS NOT null AND @TopX > 0
  SET ROWCOUNT @TopX;
  
  --SQLdm 10.2 (Tushar)- Changed the order of JOIN w.r.t number of rows in increasing order and calculating TransactionsPerSecond in this query only.
with DBMaxCollTime (SQLServerID, InstanceName, DatabaseID, DatabaseName, UTCCollectionDateTime,TransactionsPerSecond) as (
  select SQLServerID, InstanceName, DatabaseID, DatabaseName, MAX(UTCCollectionDateTime)
  ,sum(DS.Transactions) / nullif(sum(case when DS.Transactions is not null then DS.TimeDeltaInSeconds else 0 end)  ,0) TransactionsPerSecond
  --SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
  from #DS DS (nolock)
  GROUP BY SQLServerID, InstanceName, DatabaseID, DatabaseName
)
--SQLdm 10.2 (Tushar) -- Removed the JOIN with DatabaseStatistics from this query since it is already calculated in the first query.
	SELECT
		O.InstanceName, 
		O.UTCCollectionDateTime,
		O.SQLServerID, 
		O.DatabaseName, 
		O.TransactionsPerSecond,
		dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,O.TransactionsPerSecond,default,default,default,default,default) Severity 
	FROM (
			SELECT dct.UTCCollectionDateTime,
				dct.SQLServerID, 
				dct.InstanceName, 
				dct.DatabaseName,
				dct.TransactionsPerSecond--sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0) TransactionsPerSecond--dct.TransactionsPerSecond --sum(convert(float,ds.Transactions)) / nullif(sum(case when ds.Transactions is not null then ds.TimeDeltaInSeconds else 0 end)  ,0) TransactionsPerSecond
	FROM DBMaxCollTime dct
	) O 	
	JOIN @Threshold T ON T.SQLServerID = O.SQLServerID
	ORDER BY O.TransactionsPerSecond DESC

DROP TABLE #DS;
END