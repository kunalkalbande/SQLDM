------------------------------------------------------------------------------
-- <copyright file="p_GetAlwaysOnAGBasedActiveServers" company="Idera, Inc.">
--     Copyright (c) Idera, Inc. All rights reserved.
-- </copyright>
------------------------------------------------------------------------------

IF (object_id('p_GetAlwaysOnAGBasedActiveServers') is not null)
BEGIN
DROP PROCEDURE [p_GetAlwaysOnAGBasedActiveServers]
END
GO
CREATE PROCEDURE [dbo].[p_GetAlwaysOnAGBasedActiveServers]
				@tagId int = 0
AS
BEGIN
	
	SET NOCOUNT ON;
	
	IF (object_id('#SecureMonitoredSQLServers') IS NOT NULL)
	BEGIN
	    DROP TABLE #SecureMonitoredSQLServers
	END
	CREATE TABLE #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

	INSERT INTO #SecureMonitoredSQLServers
	EXEC [p_GetReportServers]
	
	DECLARE @AvailabilityGroup TABLE (GroupName nvarchar(510), GroupId uniqueidentifier)
	
	IF @tagId = 0
	BEGIN
		INSERT @AvailabilityGroup
		
			SELECT		DISTINCT
							AOG.[GroupName],
							AOG.[GroupId]
						FROM 
							AlwaysOnAvailabilityGroups AS AOG (nolock)
							INNER JOIN AlwaysOnReplicas AS AOR (nolock) on AOG.[GroupId] = AOR.[GroupId]
							INNER JOIN #SecureMonitoredSQLServers AS mss on mss.[SQLServerID] = AOR.[SQLServerID]
						WHERE
							AOG.[Active] = 1 
							AND AOR.[Delete] = 0
	END
	ELSE
	BEGIN
		INSERT @AvailabilityGroup
		
			SELECT		DISTINCT
							AOG.[GroupName],
							AOG.[GroupId]
						FROM 
							AlwaysOnAvailabilityGroups AS AOG (nolock)
							INNER JOIN AlwaysOnReplicas AS AOR (nolock) on AOG.[GroupId] = AOR.[GroupId]
							INNER JOIN #SecureMonitoredSQLServers AS mss on mss.[SQLServerID] = AOR.[SQLServerID]
							INNER JOIN ServerTags AS st (nolock) ON AOR.SQLServerID=st.SQLServerId
						WHERE
							AOG.[Active] = 1 
							AND AOR.[Delete] = 0
							AND @tagId = st.TagId
	END
				    
    IF @@rowcount = 0
		SELECT '< No Availability Groups >' AS GroupName, '00000000-0000-0000-0000-000000000000' AS GroupId
	ELSE
		SELECT GroupName, GroupId FROM @AvailabilityGroup
END
