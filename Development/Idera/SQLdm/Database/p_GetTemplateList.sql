
if (object_id('p_GetTemplateList') is not null)
begin
	drop procedure [p_GetTemplateList]
end
GO
create PROCEDURE [dbo].[p_GetTemplateList]
AS
	BEGIN
		SELECT TemplateID,[Name] 
		FROM AlertTemplateLookup
	END
	


