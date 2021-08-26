if (object_id('p_GetLatestResponseTimesByInstance') is not null)
begin
drop procedure p_GetLatestResponseTimesByInstance
end
go
CREATE PROCEDURE [dbo].[p_GetLatestResponseTimesByInstance]
AS
BEGIN

create table #SecureMonitoredSQLServers(InstanceName nvarchar(256),SQLServerID int)

insert into #SecureMonitoredSQLServers
exec [p_GetReportServers]


SELECT T.SQLServerID, T.ServerHostName, T.InstanceName, T.ResponseTimeInMilliseconds, T.UTCCollectionDateTime FROM
(SELECT ss.SQLServerID, ss.ServerHostName, mss.InstanceName,  ss.ResponseTimeInMilliseconds, ss.UTCCollectionDateTime,  
ROW_NUMBER()OVER(PARTITION BY ss.SQLServerID ORDER BY ss.UTCCollectionDateTime DESC) AS GroupRank
FROM ServerStatistics ss (NOLOCK)
INNER JOIN MonitoredSQLServers mss (NOLOCK) ON ss.SQLServerID = mss.SQLServerID
INNER JOIN #SecureMonitoredSQLServers smss (nolock) on mss.SQLServerID = smss.SQLServerID WHERE mss.Active = 1) T
WHERE T.GroupRank = 1
ORDER BY T.ResponseTimeInMilliseconds DESC;

END