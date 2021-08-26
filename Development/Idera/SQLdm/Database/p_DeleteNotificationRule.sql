if (object_id('p_DeleteNotificationRule') is not null)
begin
drop procedure [p_DeleteNotificationRule]
end
go

create procedure [p_DeleteNotificationRule] 
	@RuleID uniqueidentifier
as
begin
	DECLARE @err int
	DECLARE @id uniqueidentifier
	
	DELETE FROM [NotificationRules] 
		WHERE [RuleID] = @RuleID 

	-- SQLDM-28158 - To keep SCOM Alert Event in sync with Notification Rule
	DELETE FROM [SCOMAlertEvent] 
		WHERE [RuleID] = @RuleID 

	SELECT @err = @@error

	RETURN @err
end
