--------------------------------------------------------------------------------
--  Batch: Reindex
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - Index Name
--------------------------------------------------------------------------------
use [{0}]

declare @qualified_table_name nvarchar(500)
select 
	@qualified_table_name = quotename(user_name(uid)) + '.' + quotename(name) 
from
	sysobjects
where 
	id = {1} 

if (len(@qualified_table_name) > 0)
begin
	dbcc dbreindex (@qualified_table_name, '{2}') with no_infomsgs

	select @qualified_table_name

	if (len('{2}') > 0)
		dbcc showcontig (@qualified_table_name, '{2}') with tableresults
	else
		dbcc showcontig (@qualified_table_name) with tableresults

end

