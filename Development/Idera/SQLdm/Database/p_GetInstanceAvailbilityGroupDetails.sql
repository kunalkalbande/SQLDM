
IF (OBJECT_ID('p_GetInstanceAvailbilityGroupDetails') IS NOT NULL)
BEGIN
  DROP PROC [p_GetInstanceAvailbilityGroupDetails]
END
GO
CREATE PROCEDURE [dbo].[p_GetInstanceAvailbilityGroupDetails]
@SQLServerId int,
@HistoryInMinutes int = null,
@UTCSnapshotCollectionDateTime datetime = null
AS
BEGIN

declare @BeginDateTime datetime
declare @EndDateTime datetime

if (@UTCSnapshotCollectionDateTime is null)
	select @EndDateTime = (select max([UTCCollectionDateTime]) from [AlwaysOnStatistics] (NOLOCK) where [SQLServerID] = @SQLServerId)
else
	select @EndDateTime = @UTCSnapshotCollectionDateTime

if (@HistoryInMinutes is null)
	select @BeginDateTime = @EndDateTime;
else
	select @BeginDateTime = dateadd(n, -@HistoryInMinutes, @EndDateTime);

WITH AVGs (GroupId, GroupName) AS 
(SELECT DISTINCT aog.GroupId, aog.GroupName
	FROM AlwaysOnAvailabilityGroups aog WITH(NOLOCK)
	INNER JOIN AlwaysOnReplicas aor  WITH(NOLOCK) ON aor.GroupId = aog.GroupId
	WHERE aor.SQLServerID = @SQLServerId AND aor.Active = 1 AND aor.[Delete] = 0 AND aog.Active = 1 AND aog.[Delete] = 0)

SELECT		DISTINCT 			
			AOG.GroupName
			,aor.ReplicaName
			,aor.ReplicaRole
			,AOS.SynchronizationHealth
			,isnull(AOS.DatabaseState,0) AS [DatabaseState]
			,sum(AOS.[RedoRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[RedoRate] is not null then TimeDeltaInSeconds else 0 end),0) as [RedoRate]
			,sum(AOS.[RedoQueueSize] * TimeDeltaInSeconds)/nullif(sum(case when AOS.[RedoQueueSize]is not null then TimeDeltaInSeconds else 0 end),0) AS [RedoQueue]
			,sum(AOS.[LogSendRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSendRate] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendRate]
			,sum(AOS.[LogSedQueueSize] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSedQueueSize] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendQueue]
			,AOS.UTCCollectionDateTime
		FROM 
			AlwaysOnReplicas aor 			
			INNER JOIN AVGs AS AOG (NOLOCK)
			ON AOG.GroupId = aor.GroupId
			LEFT OUTER JOIN [AlwaysOnStatistics] AS AOS (NOLOCK) 
				on AOS.GroupId = AOG.GroupId AND AOS.ReplicaId = aor.ReplicaId
		WHERE (AOS.UTCCollectionDateTime IS NULL  OR AOS.UTCCollectionDateTime between @BeginDateTime and @EndDateTime)
		GROUP BY
			AOG.GroupName
			,aor.ReplicaId 
			,aor.ReplicaName
			,aor.ReplicaRole
			,AOS.SynchronizationHealth
			,AOS.DatabaseState	
			,AOS.UTCCollectionDateTime
			ORDER BY AOS.UTCCollectionDateTime DESC
END

