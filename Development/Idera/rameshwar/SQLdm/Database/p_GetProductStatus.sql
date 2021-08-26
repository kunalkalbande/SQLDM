--Gets the status of the product in terms of active alerts and other parameters
-- EXEC p_GetProductStatus
if (object_id('p_GetProductStatus') is not null)
begin
drop procedure p_GetProductStatus
end
go
CREATE PROCEDURE [dbo].[p_GetProductStatus]
@SqlIdList XML
AS
BEGIN
--This procedure is designed in a way of returning multiple result sets. For now, 
--it will return only one result set, i.e. alerts.

--Alerts Results Set
DECLARE @AllAlerts TABLE(AlertID INT,UTCOccurenceDateTime DATETIME,InstanceId INT,ServerName NVARCHAR(MAX),DatabaseName NVARCHAR(MAX), TableName NVARCHAR(MAX), Active BIT,Metric INT,Severity INT, PreviousSeverity INT,StateEvent INT, Value DECIMAL(16,2),Heading NVARCHAR(MAX), [Message] NVARCHAR(MAX))
INSERT INTO @AllAlerts exec p_GetAlertsForWebConsole null, null, null, null, null,null, 'Severity', 'desc', 10000, 1,null,null,null  
SELECT Severity,COUNT(0) AlertCount 
FROM @AllAlerts 
WHERE InstanceId IN(
		SELECT A.B.value('(ID)[1]', 'int' ) ID
		FROM    @SqlIdList.nodes('/Root/Source') A(B)) 
GROUP BY Severity
--SELECT Severity, COUNT(1) AlertCount
--FROM Alerts a (NOLOCK)
--WHERE a.Active = 1 AND UTCOccurrenceDateTime = @LatestDate
--GROUP BY Severity

END

