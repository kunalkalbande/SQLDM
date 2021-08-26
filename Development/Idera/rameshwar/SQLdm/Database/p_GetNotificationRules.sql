if (object_id('p_GetNotificationRules') is not null)
begin
drop procedure [p_GetNotificationRules]
end
go

create procedure [p_GetNotificationRules]
	@RuleID uniqueidentifier
as
begin
	DECLARE @err int
	IF (@RuleID IS NULL)
	BEGIN	
		SELECT [RuleID], [SerializedObject] from [NotificationRules]
	END
	ELSE
	BEGIN
		SELECT [RuleID], [SerializedObject] from [NotificationRules]
			WHERE [RuleID] = @RuleID
	END
	SELECT @err = @@error

	RETURN @err
end
