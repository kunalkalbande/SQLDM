if (object_id('fn_GetDatabaseCreationDate') is not null)
begin
drop function fn_GetDatabaseCreationDate
end
go
create function fn_GetDatabaseCreationDate()
	returns DateTime
begin
		return (Select create_date As crdate
		From master.sys.databases
		Where name = DB_NAME())
end
go
