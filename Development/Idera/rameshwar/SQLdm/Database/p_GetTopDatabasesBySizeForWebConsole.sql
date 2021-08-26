IF (OBJECT_ID('p_GetTopDatabasesBySizeForWebConsole') IS NOT NULL)
BEGIN
  DROP PROCEDURE [p_GetTopDatabasesBySizeForWebConsole]
END
GO
-- [p_GetTopDatabasesBySizeForWebConsole] 30
CREATE PROCEDURE [dbo].[p_GetTopDatabasesBySizeForWebConsole]
@TopX INT = NULL
AS
BEGIN
	DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC,InstanceName varchar(255),FriendlyServerName nvarchar(256))
	DECLARE @MetricID INT;
	SET @MetricID = 109; --this is the metric id for database size
	
	SELECT * INTO #DS FROM
	(
		SELECT DatabaseID,
			UTCCollectionDateTime,
			DataFileSizeInKilobytes		
		FROM DatabaseSize
	
		UNION ALL
	
		SELECT DatabaseID,
			MaxUTCCollectionDateTime AS UTCCollectionDateTime,
			MaxDataFileSizeInKilobytes AS DataFileSizeInKilobytes
		FROM DatabaseSizeAggregation
	) AS DSAggregated;



	INSERT INTO @Threshold EXEC p_PopulateMetricThresholdsNew @MetricID;
	
	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;
		
	with DBMaxCollTime (SQLServerID, InstanceName, DatabaseID, DatabaseName, MaxCollectionTime) as (
		select MS.SQLServerID, COALESCE(MS.[FriendlyServerName],MS.[InstanceName]) AS [InstanceName],--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
		 DS.DatabaseID, names.DatabaseName, MAX(UTCCollectionDateTime)
		from @Threshold MS
			Join [SQLServerDatabaseNames] names (nolock) 
				on MS.SQLServerID = names.SQLServerID 
			Join #DS DS (nolock)
				on names.DatabaseID = DS.DatabaseID
	    group by MS.SQLServerID, MS.InstanceName,MS.[FriendlyServerName], DS.DatabaseID, names.DatabaseName
	)
	SELECT mct.SQLServerID InstanceID,
		mct.DatabaseID,
		mct.DatabaseName,
		 mct.InstanceName,
		mct.MaxCollectionTime as UTCCollectionDateTime,
		(SS.DataFileSizeInKilobytes/1024) DataFileSizeMB,
		dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,(SS.DataFileSizeInKilobytes/1024),default,default,default,default,default) Severity 
	FROM @Threshold T 
		JOIN DBMaxCollTime mct  (nolock)
			ON T.SQLServerID = mct.SQLServerID
		JOIN #DS SS  (nolock)
			ON mct.DatabaseID = SS.DatabaseID AND SS.UTCCollectionDateTime = mct.MaxCollectionTime
	ORDER BY DataFileSizeMB DESC

	DROP TABLE #DS;
END

