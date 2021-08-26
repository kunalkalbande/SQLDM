if (object_id('fn_GetDatabaseVersion') is not null)
begin
drop function fn_GetDatabaseVersion
end
go
create function fn_GetDatabaseVersion()
	returns nvarchar(16)
begin
	--For SQLdm internal use. Do not call this function directly.

	return ('11.1')

end

go