if (object_id('p_InsertRunQueryScript') is not null)
begin
drop procedure [p_InsertRunQueryScript]
end
go

create procedure [p_InsertRunQueryScript]
	@Name NVARCHAR(100),
	@ScriptText nvarchar(max)
as
begin
	DECLARE @ReturnValue INT;
	
	INSERT INTO RunQueryScripts 
		(Type, Name, ScriptText)
	VALUES
		(1, @Name, @ScriptText)
		
	SELECT @ReturnValue = SCOPE_IDENTITY();
	SELECT @ReturnValue;
end
