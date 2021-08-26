if (object_id('p_GetTopInstancesByAlerts') is not null)
begin
drop procedure p_GetTopInstancesByAlerts
end
GO


/*-- 
SQLdm 8.5 (Ankit Srivastava): for Top X API- Most Instance Alerts for last 7 days
EXEC  [p_GetTopInstancesByAlerts] 1
--*/

Create PROCEDURE [dbo].[p_GetTopInstancesByAlerts](
  @TopX int=NULL
)
AS
BEGIN

IF @TopX IS NOT null AND @TopX > 0
  SET ROWCOUNT @TopX


  SELECT mss.SQLServerID ,COALESCE(mss.[FriendlyServerName],mss.[InstanceName]) AS [InstanceName], count(a.AlertID) as NumOfAlerts, max(a.Severity) as MaxSeverity
  --SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
  FROM Alerts a WITH(NOLOCK)
  left outer join DBMetrics DBM (nolock) on DBM.MetricID = a.[Metric]
    INNER join MonitoredSQLServers mss WITH(NOLOCK)
  ON mss.InstanceName = a.ServerName
     and (
      (DBM.MetricID is NULL AND mss.LastScheduledCollectionTime = a.UTCOccurrenceDateTime)
        or 
        (DBM.MetricID is NOT NULL AND mss.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime))
  WHERE mss.Active=1 and a.Active=1
  group by mss.InstanceName, mss.[FriendlyServerName], mss.SQLServerID --SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
  order by NumOfAlerts desc 

END



