if (object_id('p_GetAlertsCountPerCategory') is not null)
begin
drop procedure p_GetAlertsCountPerCategory
end
go
CREATE PROCEDURE [dbo].[p_GetAlertsCountPerCategory] (@AllowedSQLServers NVARCHAR(1000),@PerInstance int = 0)
AS
BEGIN

IF @PerInstance = 1
BEGIN
	--START: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts
	SELECT
		AllCategories.Category,
		AD.SQLServerID,
		AD.NumOfAlerts
	FROM
		( SELECT distinct Category 
		  from MetricInfo (NOLOCK) ) as AllCategories
		  left join
	--END: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts	
		( SELECT mi.Category, mss.SQLServerID, COUNT(1) AS NumOfAlerts
		  from MetricInfo mi (NOLOCK)
		  LEFT OUTER JOIN Alerts a (NOLOCK)
			ON mi.Metric = a.Metric
		  left outer join DBMetrics DBM (NOLOCK) on DBM.MetricID = mi.[Metric]
		  LEFT OUTER JOIN (SELECT SQLServerID,InstanceName,LastScheduledCollectionTime, LastDatabaseCollectionTime, Active FROM MonitoredSQLServers (NOLOCK) JOIN fn_Split(@AllowedSQLServers,',') allowedServers ON allowedServers.Value = MonitoredSQLServers.SQLServerID) AS mss 
			ON mss.InstanceName = a.ServerName
			   and (
	       			(DBM.MetricID is NULL AND mss.LastScheduledCollectionTime = a.UTCOccurrenceDateTime)
					or 
					(DBM.MetricID is NOT NULL AND mss.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime))
			WHERE a.Active = 1
					and mss.Active = 1
			GROUP BY mi.Category, mss.SQLServerID) as AD
			on AllCategories.Category = AD.Category  --SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts
END
ELSE
BEGIN
	--START: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts
	SELECT
		AllCategories.Category,
		AD.NumOfAlerts
	FROM
		( SELECT distinct Category 
		  from MetricInfo (NOLOCK) ) as AllCategories
		  left join
	--END: SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts
		( SELECT mi.Category, COUNT(1) AS NumOfAlerts 
		  from MetricInfo mi (NOLOCK)
	      LEFT OUTER JOIN Alerts a (NOLOCK)
	      ON mi.Metric = a.Metric
	      left outer join DBMetrics DBM on DBM.MetricID = mi.[Metric]
	      LEFT OUTER JOIN (SELECT SQLServerID,InstanceName,LastScheduledCollectionTime, LastDatabaseCollectionTime, Active FROM MonitoredSQLServers (NOLOCK) JOIN fn_Split(@AllowedSQLServers,',') allowedServers ON allowedServers.Value = MonitoredSQLServers.SQLServerID) AS mss 
	      ON mss.InstanceName = a.ServerName
	       and (
	       		(DBM.MetricID is NULL AND mss.LastScheduledCollectionTime = a.UTCOccurrenceDateTime)
	            or 
	            (DBM.MetricID is NOT NULL AND mss.LastDatabaseCollectionTime = a.UTCOccurrenceDateTime))
	      WHERE a.Active = 1
	            and mss.Active = 1 
	      GROUP BY mi.Category) as AD
		  on AllCategories.Category = AD.Category  --SQLdm 9.0 (Abhishek Joshi) --Web Console Improvements -send all the categories in the response, irrespective of numOfAlerts
		  order by AD.NumOfAlerts desc
END

END
