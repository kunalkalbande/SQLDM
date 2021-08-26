if (object_id('p_GetAlertsCountPerDatabase') is not null)
begin
drop procedure p_GetAlertsCountPerDatabase
end
go
CREATE PROCEDURE [dbo].[p_GetAlertsCountPerDatabase]
AS
BEGIN
SELECT mss.SQLServerID, COALESCE(mss.[FriendlyServerName],mss.[InstanceName]) AS ServerName, a.DatabaseName, COUNT(1) AS NumOfAlerts 
FROM Alerts a (NOLOCK)
INNER join MonitoredSQLServers mss (NOLOCK) ON a.ServerName = mss.InstanceName	
WHERE a.Active = 1 
GROUP BY mss.SQLServerID, COALESCE(mss.[FriendlyServerName],mss.[InstanceName]), a.DatabaseName
END
