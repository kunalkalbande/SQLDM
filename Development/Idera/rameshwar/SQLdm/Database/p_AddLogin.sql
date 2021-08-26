if (object_id('p_AddLogin') is not null)
begin
drop procedure [p_AddLogin]
end
go

create procedure [p_AddLogin]
	@Login nvarchar(256),
	@IsSQLLogin bit,
	@Password nvarchar(256),
	@Database nvarchar(256),
	@LoginSID varbinary(85) output
as
begin
	DECLARE @err int
	DECLARE @sid varbinary(85)
	DECLARE @sidstr nvarchar(512)
	DECLARE @SQL nvarchar(1000);
	
	-- Check if the login exists.
	if @IsSQLLogin = 1
	begin
		Select @sid = sid
		From sys.server_principals
		Where UPPER(name) = UPPER(@Login)
		And type = 'S'; -- SQL login
	end
	else
	begin
		Select @sid = sid
		From sys.server_principals
		Where UPPER(name) = UPPER(@Login)
		And type In ('U', 'G'); -- Windows user or group
	end

	-- If login does not exist then create it.
	if @sid is null
	begin
		if @IsSQLLogin = 1
		begin
			Set @SQL = N'Create Login ' + QUOTENAME(@Login) + 
							' With Password = ''' + @Password + ''';';
			Exec sp_executesql @SQL;
			
			Select @sid = sid
			From sys.server_principals
			Where UPPER(name) = UPPER(@Login)
			And type = 'S'; -- SQL login
		end
		else
		begin
			Set @SQL = N'Create Login ' + QUOTENAME(@Login) + 
							' From Windows;';
			Exec sp_executesql @SQL;
			
			Select @sid = sid
			From sys.server_principals
			Where UPPER(name) = UPPER(@Login)
			And type In ('U', 'G'); -- Windows user or group
		end
	end

	-- Create user in SQLdm database and assign the DM console user role.
	if @sid is not null
	begin
		-- add user if one does not exist
		select @sidstr = master.dbo.fn_varbintohexstr(@sid)
		Set @SQL = 'Use ' + QUOTENAME(@Database) + ';
					If Not Exists (Select 1 From sys.database_principals
								Where sid = ' + @sidstr + ')
						Create User ' + QUOTENAME(@Login) + ' For Login ' + QUOTENAME(@Login) + ';';
		Exec sp_executesql @SQL;

		-- add user to role if the user is not a dbo.
		-- no check needed if member already exists, does not cause an error
		Set @SQL = 'Use ' + QUOTENAME(@Database) + ';
					If Exists (Select 1 From sys.database_principals
								Where name <> ''dbo''
								And sid = ' + @sidstr + ')
						Exec sp_addrolemember ''SQLdmConsoleUser'', ''' + @Login + ''';';
		Exec sp_executesql @SQL;


		-- set the return param value to sid.
		set @LoginSID = @sid
	end

	SELECT @err = @@error
	RETURN @err
end

