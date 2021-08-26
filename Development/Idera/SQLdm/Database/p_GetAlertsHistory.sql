if (object_id('p_GetAlertsHistory') is not null)
begin
drop procedure p_GetAlertsHistory
end
go
CREATE PROCEDURE [dbo].[p_GetAlertsHistory] (@AlertId INT,@StartDate DATETIME, @EndDate DATETIME, @Top INT = 20 )
AS
BEGIN
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED;

SELECT @StartDate = CASE WHEN ISDATE(@StartDate) = 1 THEN @StartDate ELSE (GETDATE()-4) END, 
	   @EndDate = CASE WHEN ISDATE(@EndDate) = 1 THEN @EndDate ELSE (GETDATE()-4) END

SELECT TOP(@Top) a.AlertID, a.UTCOccurrenceDateTime, a.Metric, a.Severity, a.ServerName, a.Active, a.Value
FROM Alerts a (NOLOCK)
INNER JOIN Alerts a1 (NOLOCK) ON a.Metric = a1.Metric 
	AND ((a.ServerName is NULL AND a1.ServerName IS NULL) OR a.ServerName collate database_default = a1.ServerName collate database_default) 
	AND ((a.DatabaseName is NULL AND a1.DatabaseName IS NULL) OR a.DatabaseName collate database_default  = a1.DatabaseName collate database_default) 
WHERE a1.AlertID = @AlertId AND a.UTCOccurrenceDateTime <= a1.UTCOccurrenceDateTime AND DATEDIFF(d,a.UTCOccurrenceDateTime, @StartDate) <=0
    AND DATEDIFF(d,a.UTCOccurrenceDateTime,@EndDate) >=0
ORDER BY UTCOccurrenceDateTime DESC
END


 
