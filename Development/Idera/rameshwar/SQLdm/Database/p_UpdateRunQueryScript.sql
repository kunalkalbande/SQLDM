if (object_id('p_UpdateRunQueryScript') is not null)
begin
drop procedure [p_UpdateRunQueryScript]
end
go

create procedure [p_UpdateRunQueryScript]
	@ScriptID INT,
	@Name NVARCHAR(100),
	@ScriptText nvarchar(max)
as
begin
	UPDATE RunQueryScripts
	SET
		Name = @Name,
		ScriptText = @ScriptText
	WHERE
		ScriptID = @ScriptID
end
