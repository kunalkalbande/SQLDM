if (object_id('p_GetAlertTemplatesByTemplateName') is not null)
begin
drop procedure p_GetAlertTemplatesByTemplateName
end
go

CREATE PROCEDURE [dbo].p_GetAlertTemplatesByTemplateName(
	@Name nvarchar(256)
)
AS
	BEGIN
		SELECT [TemplateID] FROM [AlertTemplateLookup] where @Name = [Name]
	END
	
