if (object_id('p_UpdateAlertTemplate') is not null)
begin
drop procedure p_UpdateAlertTemplate
end
go

CREATE PROCEDURE [dbo].p_UpdateAlertTemplate(
	@templateID int,
	@Name nvarchar(256), 
	@Description nvarchar(1024)
)
AS
begin
	declare @error int


	if (exists(select [TemplateID] from [dbo].[AlertTemplateLookup] where [TemplateID] = @templateID))
	begin
		UPDATE [dbo].[AlertTemplateLookup]
			SET [Name] = @Name,
				[Description] = @Description
			WHERE [TemplateID] = @templateID
		set @error = @@error
		
		if (@error > 0)
		begin
			raiserror('There was an error updating the AlertTemplateLookup table', @error, 0)
		end
	end
end
				