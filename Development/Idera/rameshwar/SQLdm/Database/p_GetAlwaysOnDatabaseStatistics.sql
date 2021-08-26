------------------------------------------------------------------------------
-- <copyright file="p_GetAlwaysOnDatabaseStatistics" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------

IF (object_id('p_GetAlwaysOnDatabaseStatistics') is not null)
BEGIN
DROP PROCEDURE [p_GetAlwaysOnDatabaseStatistics]
END
GO
CREATE PROCEDURE [dbo].[p_GetAlwaysOnDatabaseStatistics]
				@AvailabilityGroup nvarchar(128),
				@ReplicaName nvarchar(256),
				@FailoverReadiness bit,
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

    DECLARE @GroupId AS uniqueidentifier
    DECLARE @ReplicaId AS uniqueidentifier
    
    SELECT @GroupId = GroupId
            FROM [AlwaysOnAvailabilityGroups]
            WHERE @AvailabilityGroup = GroupName
    
    SELECT @ReplicaId = ReplicaId
            FROM [AlwaysOnReplicas]
            WHERE @GroupId = GroupId AND @ReplicaName = ReplicaName
	
		SELECT		DISTINCT 
					dbo.fn_RoundDateTime(@Interval, max(dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime]))) AS [DateTime]
					,AOD.DatabaseName AS [DatabaseName]
					,AOS.GroupId AS [GroupId]
					,AOS.ReplicaId AS [ReplicaId]
					,AOS.GroupDatabaseId AS [GroupDatabaseId]
					,sum(AOS.[RedoRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[RedoRate] is not null then TimeDeltaInSeconds else 0 end),0) as [RedoRate]
					,sum(AOS.[RedoQueueSize] * TimeDeltaInSeconds)/nullif(sum(case when AOS.[RedoQueueSize]is not null then TimeDeltaInSeconds else 0 end),0) AS [RedoQueue]
					,sum(AOS.[LogSendRate] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSendRate] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendRate]
					,sum(AOS.[LogSedQueueSize] * TimeDeltaInSeconds) / nullif(sum(case when AOS.[LogSedQueueSize] is not null then TimeDeltaInSeconds else 0 end),0) as [LogSendQueue]
				FROM 
					[AlwaysOnStatistics] AS AOS (NOLOCK) 
					INNER JOIN [AlwaysOnDatabases] AS AOD (NOLOCK) on AOS.GroupId = AOD.GroupId 
                        AND AOS.ReplicaId = AOD.ReplicaId
						AND AOS.GroupDatabaseId = AOD.GroupDatabaseId
												
				WHERE dateadd(mi, @UTCOffset, AOS.[UTCCollectionDateTime]) BETWEEN @UTCStart AND @UTCEnd
				    AND @GroupId = AOS.GroupId
					AND @ReplicaId = AOS.ReplicaId
					AND @FailoverReadiness = AOS.IsFailoverReady

		GROUP BY
		AOD.DatabaseName
		,AOS.GroupId
		,AOS.ReplicaId
		,AOS.GroupDatabaseId		
END