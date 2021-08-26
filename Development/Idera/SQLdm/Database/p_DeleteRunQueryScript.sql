if (object_id('p_DeleteRunQueryScript') is not null)
begin
drop procedure [p_DeleteRunQueryScript]
end
go

create procedure [p_DeleteRunQueryScript]
	@ScriptID INT
as
begin
	DELETE FROM RunQueryScripts
	WHERE ScriptID = @ScriptID
end
