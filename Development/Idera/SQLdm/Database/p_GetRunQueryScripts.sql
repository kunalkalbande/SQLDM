if (object_id('p_GetRunQueryScripts') is not null)
begin
drop procedure [p_GetRunQueryScripts]
end
go

create procedure [p_GetRunQueryScripts]
as
begin
	SELECT
		ScriptID,
		Type,
		Name,
		ScriptText
	FROM RunQueryScripts
end
