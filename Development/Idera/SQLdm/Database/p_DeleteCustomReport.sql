if (object_id('p_DeleteCustomReport') is not null)
begin
drop procedure p_DeleteCustomReport
end
go

create proc p_DeleteCustomReport(@id int)
as
begin
	delete from CustomReports where ID = @id
end