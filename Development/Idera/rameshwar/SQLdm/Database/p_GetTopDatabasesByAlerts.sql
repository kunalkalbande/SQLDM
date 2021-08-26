if (object_id('p_GetTopDatabasesByAlerts') is not null)
begin
drop procedure p_GetTopDatabasesByAlerts
end
GO

/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- Databases by most alerts
EXEC  [p_GetTopDatabasesByAlerts] 100
--*/

Create PROCEDURE [dbo].[p_GetTopDatabasesByAlerts](
  @TopX int=null
)
AS
BEGIN

IF @TopX IS NOT null AND @TopX > 0
  SET ROWCOUNT @TopX

SELECT  mss.SQLServerID, a.DatabaseName,  COALESCE(mss.[FriendlyServerName],mss.[InstanceName]) AS [InstanceName],--SQLDm 10.1 (Pulkit Puri)
 count(1) as NumOfAlerts, MAX(a.Severity) as MaxSeverity
 
FROM Alerts a WITH (NOLOCK)
left outer join DBMetrics DBM (nolock) on DBM.MetricID = a.[Metric]
LEFT OUTER JOIN MonitoredSQLServers mss (nolock)
ON mss.InstanceName = a.ServerName
   and (
   		(DBM.MetricID is NULL AND mss.LastScheduledCollectionTime = a.UTCOccurrenceDateTime)
        or 
        (DBM.MetricID is NOT NULL AND mss.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime))
where mss.Active=1 and a.Active = 1 and DatabaseName is not null and mss.InstanceName is not null
group by DatabaseName ,mss.InstanceName,mss.[FriendlyServerName],mss.SQLServerID
order by NumOfAlerts DESC

end




GO


