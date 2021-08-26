IF (OBJECT_ID('p_GetInstancesList') IS NOT NULL)
BEGIN
  DROP PROC [p_GetInstancesList]
END
GO
-- [p_GetInstancesList] 30
CREATE PROCEDURE [dbo].[p_GetInstancesList]
AS
BEGIN

	SELECT mss.InstanceName, mss.SQLServerID , mss.FriendlyServerName -- Sqldm 10.1 (Pulkit Puri) FriendlyServerName
	FROM MonitoredSQLServers mss (NOLOCK)
	WHERE mss.Active = 1 AND mss.InstanceName IS NOT NULL

END
