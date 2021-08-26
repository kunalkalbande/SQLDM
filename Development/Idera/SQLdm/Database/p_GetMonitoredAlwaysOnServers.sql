------------------------------------------------------------------------------
-- <copyright file="p_GetMonitoredAlwaysOnServers.sql" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------

IF (object_id('p_GetMonitoredAlwaysOnServers') IS NOT NULL)
BEGIN
DROP PROCEDURE p_GetMonitoredAlwaysOnServers
END
GO
CREATE PROCEDURE [dbo].[p_GetMonitoredAlwaysOnServers] 
	 @tagID int = 0
	 ,@availabilityGroupName nvarchar(128)
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
	
	DECLARE @Servers TABLE (InstanceName nvarchar(512), SQLServerID int)
	
	IF @tagID = 0
	BEGIN
		INSERT @Servers
		SELECT DISTINCT AOR.ReplicaName as InstanceName, AOR.SQLServerID as SQLServerID
			FROM AlwaysOnStatistics AS AOS (nolock)
			INNER JOIN AlwaysOnReplicas AS AOR (nolock) ON AOR.GroupId=AOS.GroupId AND AOR.ReplicaId=AOS.ReplicaId
			INNER JOIN AlwaysOnAvailabilityGroups AS AOG (nolock) ON AOG.GroupId = AOS.GroupId
			INNER JOIN #SecureMonitoredSQLServers AS smss ON smss.SQLServerID = AOR.SQLServerID
			WHERE @availabilityGroupName=AOG.GroupName 
                AND AOR.[Delete] = 0
	END
	ELSE
	BEGIN	
		INSERT @Servers
		SELECT DISTINCT AOR.ReplicaName AS InstanceName, AOR.SQLServerID AS SQLServerID
			FROM AlwaysOnStatistics AS AOS (nolock)
			INNER JOIN AlwaysOnReplicas AS AOR (nolock) ON AOR.GroupId=AOS.GroupId AND AOR.ReplicaId=AOS.ReplicaId
			INNER JOIN AlwaysOnAvailabilityGroups AS AOG (nolock) ON AOG.GroupId = AOS.GroupId
			INNER JOIN #SecureMonitoredSQLServers AS smss ON smss.SQLServerID = AOR.SQLServerID
			INNER JOIN ServerTags AS st (nolock) ON AOR.SQLServerID=st.SQLServerId
			WHERE @availabilityGroupName=AOG.GroupName AND @tagID = st.TagId 
                AND AOR.[Delete] = 0
	END
		
		IF @@rowcount = 0
			SELECT '< Select a Server >' as InstanceName, 0 as SQLServerID
		ELSE
			SELECT '< All >' as InstanceName, 0 as SQLServerID
			UNION
			SELECT InstanceName, SQLServerID FROM @Servers
END