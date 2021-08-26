------------------------------------------------------------------------------
-- <copyright file="p_GetAlwaysOnStatistics.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------

IF (object_id('p_GetAlwaysOnStatistics') is not null)
BEGIN
DROP PROCEDURE [p_GetAlwaysOnStatistics]
END
GO
CREATE PROCEDURE [dbo].[p_GetAlwaysOnStatistics]
				@SQLServerIDs nvarchar(max) = null,
				@AvailabilityGroup nvarchar(128),
				@UTCStart DateTime,
				@UTCEnd DateTime,
				@UTCOffset int,
				@Interval tinyint
-- @Interval - Granularity of calculation:
--	0 - Minutes
--	1 - Hours
--	2 - Days
--	3 - Months
--	4 - Years
AS
BEGIN
	DECLARE @xmlDoc int

	IF @SQLServerIDs is not null 
	BEGIN
		DECLARE @SQLServers table(
				SQLServerID int) 

		-- Prepare XML document if there is one
		EXEC sp_xml_preparedocument @xmlDoc output, @SQLServerIDs

		INSERT INTO @SQLServers
		SELECT ID 
		FROM openxml(@xmlDoc, '//Srvr', 1)
			with (ID int)
	END
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF (object_id('#SecureMonitoredSQLServers') IS NOT NULL)
	BEGIN
	    DROP TABLE #SecureMonitoredSQLServers
	END
	CREATE TABLE #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	INSERT INTO #SecureMonitoredSQLServers
	EXEC [p_GetReportServers]
	
	SELECT		DISTINCT 
					dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime]))) AS [DateTime]
					,AOG.GroupName AS [AvailabilityGroups]
					,MS.InstanceName AS [Name]
					,AOR.ReplicaName AS [Replica]
					,AOS.IsFailoverReady AS [FailoverReadiness]
					,sum(AOS.[RedoRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[RedoRate] is not null then TimeDeltaInSeconds else 0 end),0) as [RedoRate]
					,sum(AOS.[RedoQueueSize] * TimeDeltaInSeconds)/nullif(sum(case when AOS.[RedoQueueSize]is not null then TimeDeltaInSeconds else 0 end),0) AS [RedoQueue]
					,sum(AOS.[LogSendRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSendRate] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendRate]
					,sum(AOS.[LogSedQueueSize] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSedQueueSize] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendQueue]
				FROM 
						@SQLServers S 
						INNER JOIN [AlwaysOnReplicas] AS AOR on S.[SQLServerID] = [AOR].[SQLServerID]
						INNER JOIN [AlwaysOnStatistics] AS AOS on [AOR].[ReplicaId] = [AOS].[ReplicaId]
						INNER JOIN [AlwaysOnDatabases] AS AOD on [AOR].[ReplicaId] = [AOD].[ReplicaId]
															AND [AOR].[GroupId] = [AOD].[GroupId]
						INNER JOIN [AlwaysOnAvailabilityGroups] AS AOG on AOS.[GroupId] = [AOG].GroupId
						INNER JOIN #SecureMonitoredSQLServers AS MS on S.SQLServerID = MS.SQLServerID
						
				WHERE AOS.[UTCCollectionDateTime] BETWEEN @UTCStart AND @UTCEnd
						  AND @AvailabilityGroup = AOG.GroupName
						  AND dbo.fn_RoundDateTime(@Interval, AOS.[UTCCollectionDateTime]) between @UTCStart and @UTCEnd
		GROUP BY
		AOG.GroupName
		,MS.InstanceName
		,AOR.ReplicaName
		,AOS.IsFailoverReady
		-- Always group by year at the least
		,datepart(yy, dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime]))
		-- Group by all intervals greater than or equal to the selected interval
		,case when @Interval <= 3 then datepart(mm,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) end
		,case when @Interval <= 2 then datepart(dd,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) end
		,case when @Interval <= 1 then datepart(hh,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) end
		,case when @Interval =  0 then datepart(mi,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) else datepart(yy,dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime])) end
END
				
