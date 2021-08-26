------------------------------------------------------------------------------
-- <copyright file="p_GetAlwaysOnDatabases.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------

IF (object_id('p_GetAlwaysOnDatabases') is not null)
BEGIN
DROP PROCEDURE [p_GetAlwaysOnDatabases]
END
GO
CREATE PROCEDURE [dbo].[p_GetAlwaysOnDatabases]
				@AvailabilityGroup nvarchar(256)
AS
BEGIN

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

	DECLARE @GroupReplicaLastUpdate table(
				GroupName sysname
				,ReplicaName nvarchar(256)
				,UTCCollectionDateTime datetime)
				
	INSERT INTO @GroupReplicaLastUpdate

		SELECT			[AOG].[GroupName] AS GroupName,
						[AOR].[ReplicaName] AS RepliclaName,
						
						MAX([AOS].[UTCCollectionDateTime])
						
			FROM		AlwaysOnAvailabilityGroups AS AOG (nolock)
						INNER JOIN AlwaysOnReplicas AS AOR (nolock) on [AOG].[GroupId]=[AOR].[GroupId]
						INNER JOIN AlwaysOnStatistics AS AOS (nolock) on [AOG].[GroupId]=[AOS].[GroupId] AND [AOR].[ReplicaId] = [AOS].[ReplicaId]
			
			GROUP BY	GroupName,
						ReplicaName
						
	IF @AvailabilityGroup='< All >'
	BEGIN
		SELECT DISTINCT [AOR].[ReplicaName] AS RepliclaName,
						[AOD].[DatabaseName] AS DatabaseName,
						[AOS].[IsFailoverReady] AS FailoverReady,
						[AOS].[SynchronizationState] AS SynchronizationState
						
			FROM		AlwaysOnReplicas AS AOR (nolock)
						INNER JOIN AlwaysOnDatabases AS AOD (nolock) on [AOR].[ReplicaId]=[AOD].[ReplicaId]
						INNER JOIN AlwaysOnStatistics AS AOS (nolock) on [AOD].[ReplicaId]=[AOS].[ReplicaId]
																	AND [AOD].[GroupId]=[AOS].[GroupId]
																	AND [AOD].[DatabaseID]=[AOS].[DatabaseId]
						INNER JOIN AlwaysOnAvailabilityGroups AS AOG (nolock) on [AOG].[GroupId] = [AOS].[GroupId]
						INNER JOIN #SecureMonitoredSQLServers AS smss (nolock) on smss.[SQLServerID] = AOR.[SQLServerID]
						JOIN @GroupReplicaLastUpdate AS grlu on grlu.[UTCCollectionDateTime] = [AOS].[UTCCollectionDateTime]
						
			WHERE		AOR.[Delete] = 0
		  
	END
	ELSE
	BEGIN
		SELECT DISTINCT [AOR].[ReplicaName] AS RepliclaName,
						[AOD].[DatabaseName] AS DatabaseName,
						[AOS].[IsFailoverReady] AS FailoverReady,
						[AOS].[SynchronizationState] AS SynchronizationState
						
			FROM		AlwaysOnReplicas AS AOR (nolock)
						INNER JOIN AlwaysOnDatabases AS AOD (nolock) on [AOR].[ReplicaId]=[AOD].[ReplicaId]
						INNER JOIN AlwaysOnStatistics AS AOS (nolock) on [AOD].[ReplicaId]=[AOS].[ReplicaId]
																	AND [AOD].[GroupId]=[AOS].[GroupId]
																	AND [AOD].[DatabaseID]=[AOS].[DatabaseId]
						INNER JOIN AlwaysOnAvailabilityGroups AS AOG (nolock) on [AOG].[GroupId] = [AOS].[GroupId]
						INNER JOIN #SecureMonitoredSQLServers AS smss (nolock) on smss.[SQLServerID] = AOR.[SQLServerID]
						JOIN @GroupReplicaLastUpdate AS grlu on grlu.[UTCCollectionDateTime] = [AOS].[UTCCollectionDateTime]
		
		WHERE			[AOG].[GroupName] = @AvailabilityGroup
						AND AOR.[Delete] = 0
	END

END
