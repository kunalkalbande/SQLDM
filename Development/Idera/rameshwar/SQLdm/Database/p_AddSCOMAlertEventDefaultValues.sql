if (object_id('p_AddSCOMAlertEventDefaultValues') is not null)
begin
drop procedure [p_AddSCOMAlertEventDefaultValues]
end
go

CREATE PROCEDURE [dbo].[p_AddSCOMAlertEventDefaultValues] 
	@RuleID uniqueidentifier
AS
BEGIN
IF NOT EXISTS(SELECT 1 FROM SCOMAlertEvent
              WHERE RuleID = @RuleID) 
	BEGIN
		INSERT INTO [SCOMAlertEvent]                                                                                                                                                
		(
				[RuleID],
				[MetricID],
				[IsInSCOMAsAlert]
		)
		(SELECT @RuleID,[Metric],0 -- Default value to send every alert as event
				FROM [MetricInfo] 
				WHERE [Metric] NOT IN (SELECT [MetricID] FROM [SCOMAlertEvent]) )
	End
			
END
 

