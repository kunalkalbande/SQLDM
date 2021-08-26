--------------------------------------------------------------------------------
--  Batch: Index Statistics
--  Tables: sysobjects
--  Variables: 
--		[0] - Target Database
--		[1] - Table ID
--		[2] - Index Name
--		[3] - user_name or schema_name
--------------------------------------------------------------------------------
use [{0}]

declare @qualified_table_name nvarchar(500)
select 
	@qualified_table_name = quotename({3}(uid)) + '.' + quotename(name) 
from
	sysobjects
where 
	id = {1} 
IF ((IS_SRVROLEMEMBER('sysadmin') = 1) OR (IS_ROLEMEMBER('db_owner') =1) OR (IS_ROLEMEMBER('db_ddladmin') =1))
BEGIN
	if (len(@qualified_table_name) > 0)
		dbcc show_statistics (@qualified_table_name, '{2}')
END

