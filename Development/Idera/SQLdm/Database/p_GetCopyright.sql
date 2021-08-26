
if (object_id('p_GetCopyright') is not null)
begin
drop procedure [p_GetCopyright]
end
go

create procedure [p_GetCopyright]
as
begin
	select Character_Value as [copyright] from RepositoryInfo where [Name]='Copyright'
end
