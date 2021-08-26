if (object_id('p_InsertLoginName') is not null)
begin
drop procedure p_InsertLoginName
end
go
create procedure p_InsertLoginName
	@LoginName nvarchar(256),
	@LoginNameID int output
as
begin

	select @LoginNameID = LoginNameID
	from LoginNames
	where LoginName = @LoginName
	or (LoginName is null and @LoginName is null)

	if (nullif(@LoginNameID,0) is null)
	begin
		insert into LoginNames([LoginName]) values (@LoginName)
		select @LoginNameID = scope_identity()
	end

	
end