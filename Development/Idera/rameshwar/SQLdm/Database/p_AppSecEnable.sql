IF (object_id('p_AppSecEnable') is not null)
BEGIN
drop procedure p_AppSecEnable
END
GO

CREATE PROCEDURE [dbo].[p_AppSecEnable]
AS
BEGIN
	declare @e int

	-- set security enabled flag
	-- if ApplicationSecurityEnabled flag is in the table then set it.
	-- else create one and set it to 1.
	if ((select count([Internal_Value]) from dbo.[RepositoryInfo] where [Name] = 'ApplicationSecurityEnabled') = 1)
	begin
		update dbo.[RepositoryInfo]
		set [Internal_Value] = 1
		where [Name] = 'ApplicationSecurityEnabled'
	end
	else
	begin
		insert into dbo.[RepositoryInfo]
		values ('ApplicationSecurityEnabled',1,'')
	end

	select @e = @@error
	return @e
END
GO
