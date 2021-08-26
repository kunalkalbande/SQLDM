if (object_id('p_GetDefaultTemplateID') is not null)
begin
drop procedure p_GetDefaultTemplateID
end
go

CREATE PROCEDURE [dbo].p_GetDefaultTemplateID(
	@DefaultTemplateID int output)
as
BEGIN

	select @DefaultTemplateID = [TemplateID] from AlertTemplateLookup where [Default] = 1
	
END

