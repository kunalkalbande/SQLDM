if (object_id('p_AddDefaultAlertTemplate') is not null)
begin
drop procedure [p_AddDefaultAlertTemplate]
end
go

create procedure [p_AddDefaultAlertTemplate]
	@templateID int output
as
begin
	DECLARE @defaultID int
	DECLARE @err int
	set @err = 0

	IF (not exists(SELECT [TemplateID] FROM AlertTemplateLookup where [Default] = 1))
	BEGIN
			INSERT INTO AlertTemplateLookup (
				[Name],
				[Description],
				[Default]) 
			VALUES (
				'Default Template', 
				'SQLdm Default Template created by Management Services', 
				1)
			
			set @err = @@error
			
			SELECT @templateID = SCOPE_IDENTITY()
	END

	RETURN @err
end

