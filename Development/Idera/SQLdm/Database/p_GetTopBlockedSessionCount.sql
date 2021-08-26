-- SQLDM 8.5:Mahesh : Added for Rest service consumption
if (object_id('p_GetTopBlockedSessionCount') is not null)
begin
drop procedure p_GetTopBlockedSessionCount
end
go
-- exec p_GetTopBlockedSessionCount 30
CREATE PROCEDURE [dbo].[p_GetTopBlockedSessionCount]
@TopX INT = NULL
AS
BEGIN
	DECLARE @Threshold TABLE(SQLServerID INT, MetricID INT, WarningThreshold NUMERIC, CriticalThreshold NUMERIC, InfoThreshold NUMERIC)
	DECLARE @MetricID INT;
	SET @MetricID = 58; --this is the metric id for blocked session count
	
	INSERT INTO @Threshold EXEC p_PopulateMetricThresholds @MetricID;
	


	IF @TopX IS NOT null AND @TopX > 0
		SET ROWCOUNT @TopX;	

	WITH picked([UTCCollectionDateTime],SQLServerID) AS
	( Select max([UTCCollectionDateTime]), SQLServerID from ServerStatistics (nolock) group by SQLServerID) 


	SELECT
	 COALESCE(MSS.[FriendlyServerName],MSS.[InstanceName]) AS [InstanceName]--SQLdm 10.1 - (Pulkit Puri) - to add friendly server name
	,SS.[UTCCollectionDateTime]
	,SS.[SQLServerID]
	,SS.[BlockedProcesses] as [BlockedSessionCount]
	,dbo.fn_GetSeverityByMetricValue(T.CriticalThreshold,T.WarningThreshold,T.InfoThreshold,[BlockedProcesses],default,default,default,default,default) Severity 
	--,[LeadBlockers] as [Lead Blockers]
    --,[LockStatistics] as [Lock Statistics]
from
	picked 
	left join [ServerStatistics] SS (nolock) on picked.[SQLServerID] = SS.[SQLServerID] and picked.[UTCCollectionDateTime] = SS.[UTCCollectionDateTime]
	--left join [ServerActivity] SA (nolock) on SA.[SQLServerID] = SS.[SQLServerID] and SA.[UTCCollectionDateTime] = SS.[UTCCollectionDateTime]
	left join [MonitoredSQLServers] MSS (nolock) on MSS.[SQLServerID]=SS.[SQLServerID]
	left JOIN @Threshold T ON T.SQLServerID = SS.SQLServerID	 
WHERE MSS.Active = 1
Order by 
	[BlockedProcesses] DESC
END

