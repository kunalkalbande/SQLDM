if (object_id('p_UpdateNotificationRule') is not null)
begin
drop procedure [p_UpdateNotificationRule]
end
go

create procedure [p_UpdateNotificationRule] 
	@RuleID uniqueidentifier,
	@SerializedObject nvarchar(max)
as
begin
	DECLARE @err int
	DECLARE @id uniqueidentifier
	
	UPDATE [NotificationRules] SET [SerializedObject] = @SerializedObject
		WHERE [RuleID] = @RuleID 
	
	-- SQLDM-28158 - To keep SCOM Alert Event in sync with Notification Rule
	DELETE FROM [SCOMAlertEvent] 
	WHERE [RuleID] = @RuleID 

	SELECT @err = @@error

	RETURN @err
end
