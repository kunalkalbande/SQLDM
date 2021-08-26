---------------------------------------
-- Azure SQL Database Managed Instance
---------------------------------------

--------------------------------------------------------------------------------
--  Batch: Update statistics
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - Index Name
--		[3] - user_name or schema_name
--------------------------------------------------------------------------------
--START (RRG): USE not supported, should call within context of a User Database
--use [{0}]
--END (RRG): USE not supported, should call within context of a User Database

declare @qualified_table_name nvarchar(500)
select 
	@qualified_table_name = quotename({3}(uid)) + '.' + quotename(name) 
from
	sysobjects (nolock)
where 
	id = {1} 

if (len(@qualified_table_name) > 0)
begin
	declare @command nvarchar(3000)
	set @command = 'update statistics ' + replace(@qualified_table_name,'''','''''') + ' {2}'
	print @command
	exec(@command)
end	
