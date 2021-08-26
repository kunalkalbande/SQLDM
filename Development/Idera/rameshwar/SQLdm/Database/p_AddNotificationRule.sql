if (object_id('p_AddNotificationRule') is not null)
begin
drop procedure [p_AddNotificationRule]
end
go

create procedure [p_AddNotificationRule] 
	@SerializedObject nvarchar(max),
	@ReturnRuleID uniqueidentifier output
as
begin
	DECLARE @err int
	DECLARE @id uniqueidentifier
	
	SELECT @id = NEWID()
	INSERT	into NotificationRules (RuleID, SerializedObject) VALUES (@id, @SerializedObject)

	SELECT @err = @@error

	if (@err = 0)
	BEGIN	
		SELECT @ReturnRuleID = @id
	END

	RETURN @err
end