if (object_id('p_InsertHostName') is not null)
begin
drop procedure p_InsertHostName
end
go
create procedure p_InsertHostName
	@HostName nvarchar(256),
	@HostNameID int output
as
begin

	select @HostNameID = HostNameID
	from HostNames
	where HostName = @HostName
	or (HostName is null and @HostName is null)

	if (nullif(@HostNameID,0) is null)
	begin
		insert into HostNames([HostName]) values (@HostName)
		select @HostNameID = scope_identity()
	end

	
end