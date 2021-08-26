if (object_id('p_InsertApplicationName') is not null)
begin
drop procedure p_InsertApplicationName
end
go
create procedure p_InsertApplicationName
	@ApplicationName nvarchar(256),
	@ApplicationNameID int output
as
begin

	select @ApplicationNameID = ApplicationNameID
	from ApplicationNames
	where ApplicationName = @ApplicationName
	or (ApplicationName is null and @ApplicationName is null)

	if (nullif(@ApplicationNameID,0) is null)
	begin
		insert into ApplicationNames([ApplicationName]) values (@ApplicationName)
		select @ApplicationNameID = scope_identity()
	end

	
end