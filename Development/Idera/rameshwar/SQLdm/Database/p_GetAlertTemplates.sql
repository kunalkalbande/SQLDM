if (object_id('p_GetAlertTemplates') is not null)
begin
drop procedure p_GetAlertTemplates
end
go

CREATE PROCEDURE [dbo].p_GetAlertTemplates(
	@templateID int
)
AS
begin
	IF (@templateID IS NULL)
	BEGIN
		SELECT [TemplateID], [Name], [Description], [Default] FROM [AlertTemplateLookup] 
	END
	ELSE
	BEGIN
		SELECT [TemplateID], [Name], [Description], [Default]  FROM [AlertTemplateLookup] where @templateID = [TemplateID]
	END
END	

