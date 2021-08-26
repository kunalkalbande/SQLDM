if (object_id('p_DeleteAlertTemplate') is not null)
begin
drop procedure p_DeleteAlertTemplate
end
go

CREATE PROCEDURE [dbo].p_DeleteAlertTemplate(
	@templateID int
)
AS
begin
	IF (@templateID IS NULL)
	BEGIN
		DELETE FROM [AlertTemplateLookup] where [Default] <> 1
	END
	ELSE
	BEGIN
		DECLARE @DefaultTemplate int
		select @DefaultTemplate = TemplateID from [AlertTemplateLookup] where [Default] = 1

		if (@DefaultTemplate <> @templateID)
		BEGIN
			DELETE FROM [AlertTemplateLookup] where @templateID = [TemplateID]
		END
		ELSE
		BEGIN
			RAISERROR('Cannot delete the default template', 15, 0)
		END
	END
END	

