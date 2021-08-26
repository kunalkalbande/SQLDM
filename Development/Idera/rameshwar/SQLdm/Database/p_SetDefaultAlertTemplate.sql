if (object_id('p_SetDefaultAlertTemplate') is not null)
begin
drop procedure p_SetDefaultAlertTemplate
end
go

CREATE PROCEDURE [dbo].p_SetDefaultAlertTemplate(
	@templateID int
)
AS
begin
	declare @currentDefault int
	declare @error int
	
	select @currentDefault = TemplateID from dbo.AlertTemplateLookup where [Default] = 1
	set @error = @@ERROR
	
	if (@error = 0)
	BEGIN
		update dbo.AlertTemplateLookup set [Default] = 0 where TemplateID = @currentDefault
		set @error = @@ERROR
	END
	
	if (@error = 0)
	BEGIN
		update dbo.AlertTemplateLookup set [Default] = 1 where TemplateID = @templateID
		set @error = @@ERROR
	END
	
	RETURN @error
END
