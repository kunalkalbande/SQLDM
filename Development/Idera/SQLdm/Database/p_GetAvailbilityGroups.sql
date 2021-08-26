if (object_id('p_GetAvailbilityGroups') is not null)
begin
drop procedure p_GetAvailbilityGroups
end
go
--Ankit Nagpal SQLDM 10.0.0 
CREATE PROCEDURE [dbo].[p_GetAvailbilityGroups]
@SQLServerId int,
@StartTime datetime , 
@EndTime datetime

AS
BEGIN

declare @BeginDateTime datetime
declare @EndDateTime datetime

select @EndDateTime = @EndTime

select @BeginDateTime = @StartTime;

WITH AVGs (GroupId, GroupName , ServerSourceName ,ListenerDnsName ,ListenerIpAddress , ListenerPort) AS 
(SELECT DISTINCT aog.GroupId, aog.GroupName , aog.ServerSourceName , aog.ListenerDnsName , aog.ListenerIpAddress , aog.ListenerPort
	FROM AlwaysOnAvailabilityGroups aog WITH(NOLOCK)
	INNER JOIN AlwaysOnReplicas aor  WITH(NOLOCK) ON aor.GroupId = aog.GroupId
	WHERE aor.SQLServerID = @SQLServerId AND aor.Active = 1 AND aor.[Delete] = 0 AND aog.Active = 1 AND aog.[Delete] = 0)

SELECT		DISTINCT 			
			AOG.GroupName,
			AOG.GroupId , 
			aor.ServerSourceName ,	-- Use AOR Server Source Name to maintain has relationship with ReplicaName
			AOG.ListenerDnsName ,
			AOG.ListenerIpAddress ,
			AOG.ListenerPort    
			,aor.ReplicaName
			,aor.ReplicaRole
			,aor.ReplicaId
			,aor.SQLServerID
			,aor.FailoverMode
			,aor.AvailabilityMode
			,aor.PrimaryConnectionMode
			,aor.SecondaryConnectionMode
			,AOS.SynchronizationHealth
			,isnull(AOS.DatabaseState,0) AS [DatabaseState]
			,sum(AOS.[RedoRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[RedoRate] is not null then TimeDeltaInSeconds else 0 end),0) as [RedoRate]
			,sum(AOS.[RedoQueueSize] * TimeDeltaInSeconds)/nullif(sum(case when AOS.[RedoQueueSize]is not null then TimeDeltaInSeconds else 0 end),0) AS [RedoQueue]
			,sum(AOS.[LogSendRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSendRate] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendRate]
			,sum(AOS.[LogSedQueueSize] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSedQueueSize] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendQueue]
			,AOS.UTCCollectionDateTime
			,AOS.AlwaysOnStatisticsID
			,AOS.DatabaseId
			,AOS.IsFailoverReady
			,AOS.SynchronizationState
			,AOS.SynchronizationHealth
			,AOS.DatabaseState
			,AOS.IsSuspended
			,AOS.LastHardenedTime
			,AOS.OperationalState
			,AOS.ConnectedState
			,AOS.SynchronizationHealthAvailabilityReplica
			,AOS.LastConnectErrorNumber
			,AOS.LastConnectErrorDescription
			,AOS.LastConnectErrorTimestamp
			,AOS.EstimatedDataLossTime
			,AOS.SynchronizationPerformance
			,AOS.FilestreamSendRate
			,AOS.EstimatedRecoveryTime
			,AOS.GroupDatabaseId
			,AOD.AlwaysOnDatabasesID
			,AOD.DatabaseName
			,AOD.ServerSourceName
		FROM 
			AlwaysOnReplicas aor 			
			INNER JOIN AVGs AS AOG (NOLOCK)
			ON AOG.GroupId = aor.GroupId
			-- SQLDM-26121 History view of Availability group not displaying correctly
			INNER JOIN [AlwaysOnDatabases] AS AOD (NOLOCK) ON aor.GroupId = AOD.GroupId AND aor.ServerSourceName = AOD.ServerSourceName AND aor.ReplicaId = AOD.ReplicaId
 			LEFT OUTER JOIN [AlwaysOnStatistics] AS AOS (NOLOCK) 
				on AOS.GroupId = AOD.GroupId AND AOS.ReplicaId = AOD.ReplicaId AND AOD.GroupDatabaseId = AOS.GroupDatabaseId AND AOS.DatabaseId = AOD.DatabaseID AND AOS.SQLServerID = aor.SQLServerID
		WHERE (AOS.UTCCollectionDateTime IS NULL  OR AOS.UTCCollectionDateTime between @BeginDateTime and @EndDateTime)
		GROUP BY
			AOG.GroupName,
			AOG.GroupId,
			aor.ServerSourceName ,
			AOG.ListenerDnsName ,
			AOG.ListenerIpAddress ,
			AOG.ListenerPort 
			,aor.ReplicaId 
			,aor.ReplicaName
			,aor.ReplicaRole
			,aor.SQLServerID
			,aor.FailoverMode
			,aor.AvailabilityMode
			,aor.PrimaryConnectionMode
			,aor.SecondaryConnectionMode
			,AOS.AlwaysOnStatisticsID
			,AOS.SynchronizationHealth
			,AOS.DatabaseState	
			,AOS.UTCCollectionDateTime
			,AOS.DatabaseId
			,AOS.IsFailoverReady
			,AOS.SynchronizationState
			,AOS.SynchronizationHealth
			,AOS.DatabaseState
			,AOS.IsSuspended
			,AOS.LastHardenedTime
			,AOS.OperationalState
			,AOS.ConnectedState
			,AOS.SynchronizationHealthAvailabilityReplica
			,AOS.LastConnectErrorNumber
			,AOS.LastConnectErrorDescription
			,AOS.LastConnectErrorTimestamp
			,AOS.EstimatedDataLossTime
			,AOS.SynchronizationPerformance
			,AOS.FilestreamSendRate
			,AOS.EstimatedRecoveryTime
			,AOS.GroupDatabaseId
			,AOD.AlwaysOnDatabasesID
			,AOD.DatabaseName
			,AOD.ServerSourceName
			ORDER BY AOS.UTCCollectionDateTime DESC
END
 


GO

