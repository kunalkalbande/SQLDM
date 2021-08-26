IF (object_id('[p_UpdateSCOMAlertsEvent]') IS NOT NULL)
BEGIN
DROP PROCEDURE [p_UpdateSCOMAlertsEvent]
END
GO

CREATE PROCEDURE [dbo].[p_UpdateSCOMAlertsEvent]
	  @metricId int,
	 @ruleId uniqueidentifier,
	 @alertFlag bit
AS
BEGIN
IF NOT EXISTS 
    (SELECT RuleID
    FROM SCOMAlertEvent
    WHERE RuleID = @ruleId AND MetricID = @metricId and IsInSCOMAsAlert = @alertFlag)
    BEGIN
INSERT
INTO	SCOMAlertEvent
	(RuleID,MetricID,IsInSCOMAsAlert)
values(@ruleId,@metricId,@alertFlag)
END
END