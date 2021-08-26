------------------------------------------------------------------------------
-- <copyright file="p_GetAlwaysOnAvailabilityReplica" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------
IF (object_id('p_GetAlwaysOnAvailabilityReplica') is not null)
BEGIN
DROP PROCEDURE [p_GetAlwaysOnAvailabilityReplica]
END
GO
CREATE PROCEDURE [dbo].[p_GetAlwaysOnAvailabilityReplica]
				@AvailabilityGroup nvarchar(128)
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
	
	IF @AvailabilityGroup='< All >'
	BEGIN
	
		SELECT DISTINCT	[AOG].[GroupName] AS GroupName,
						[AOR].[ReplicaName] AS RepliclaName,
						[AOR].[FailoverMode] AS FailoverMode,
						[AOR].[AvailabilityMode] AS AvailabilityMode,
						[AOR].[SecondaryConnectionMode] AS SecondConnMode,
						[AOR].[ReplicaRole] as ReplicaRole
						
			FROM		AlwaysOnAvailabilityGroups AS AOG (nolock)
						INNER JOIN AlwaysOnReplicas AS AOR (nolock) on [AOG].[GroupId]=[AOR].[GroupId]
						INNER JOIN #SecureMonitoredSQLServers AS smss on AOR.[SQLServerID] = smss.[SQLServerID]
			
			WHERE		AOR.[Delete] = 0
			
	END
	ELSE
	BEGIN
		SELECT DISTINCT [AOG].[GroupName] AS GroupName,
						[AOR].[ReplicaName] AS RepliclaName,
						[AOR].[FailoverMode] AS FailoverMode,
						[AOR].[AvailabilityMode] AS AvailabilityMode,
						[AOR].[SecondaryConnectionMode] AS SecondConnMode,
						[AOR].[ReplicaRole] as ReplicaRole
						
			FROM		AlwaysOnAvailabilityGroups AS AOG (nolock)
						INNER JOIN AlwaysOnReplicas AS AOR (nolock) on [AOG].[GroupId]=[AOR].[GroupId]
						INNER JOIN #SecureMonitoredSQLServers AS smss on AOR.[SQLServerID] = smss.[SQLServerID]
						
			WHERE		@AvailabilityGroup=[AOG].[GroupName]
						AND AOR.[Delete] = 0
			  
	END
END
