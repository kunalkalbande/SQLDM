IF (object_id('p_AppSecDisable') is not null)
BEGIN
drop procedure p_AppSecDisable
END
GO

CREATE PROCEDURE [dbo].[p_AppSecDisable]
AS
BEGIN
	declare @e int

	-- set security enabled flag
	-- if ApplicationSecurityEnabled flag is in the table then clear it.
	-- else create one and set it to 0.
	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled') = 1)
	begin
		update dbo.[RepositoryInfo]
		set [Internal_Value] = 0
		where [Name] = 'ApplicationSecurityEnabled'
	end
	else
	begin
		insert into dbo.[RepositoryInfo]
		values ('ApplicationSecurityEnabled',0,'')
	end
	
	-- disable all permission rules.
	update dbo.[Permission]
	set [Enabled] = 0

	select @e = @@error
	return @e
END
GO
