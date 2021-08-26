-- SQLdm 9.1 (Sanjali Makkar)

-- Gets the status of the instances in terms of active alerts and other parameters
-- EXEC p_GetInstanceStatus 

if (object_id('p_GetInstanceStatus') is not null)
begin
drop procedure p_GetInstanceStatus
end
go

CREATE PROCEDURE [dbo].[p_GetInstanceStatus]
@SqlIdList XML--,
--@SQLServerID INT = NULL
AS
BEGIN

--This procedure is designed in a way of returning multiple result sets. 

declare @ServerActivityByMaxCollectionTime TABLE (  
InstanceID int,   
UTCCollectionDateTime DateTime)  

--START SQLdm 9.1 (Sanjali Makkar): Gets Health Index coefficients for specific alerts   
INSERT INTO @ServerActivityByMaxCollectionTime  
SELECT mss.SQLServerID, MAX(sa.UTCCollectionDateTime) UTCCollectionDateTime 
FROM MonitoredSQLServers mss (NOLOCK)  
LEFT OUTER JOIN ServerActivity sa (NOLOCK) ON mss.SQLServerID = sa.SQLServerID  
WHERE mss.Active = 1 AND mss.SQLServerID IN (SELECT A.B.value('(ID)[1]', 'int' ) ID
		FROM    @SqlIdList.nodes('/Root/Source') A(B)) AND sa.StateOverview IS NOT NULL
GROUP BY mss.SQLServerID  

-- Get state overview data  
SELECT ISNULL(samax.InstanceID,0) InstanceID, sa.[StateOverview]  
from @ServerActivityByMaxCollectionTime samax 
	JOIN ServerActivity sa (NOLOCK) on samax.InstanceID = sa.SQLServerID and  sa.UTCCollectionDateTime = samax.UTCCollectionDateTime

--Instances Overview Result Set
SELECT COUNT(0) ServerCount, Active FROM MonitoredSQLServers (NOLOCK) WHERE Deleted = 0 
GROUP BY Active

END