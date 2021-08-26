--------------------------------------------------------------------------------
--  Batch: Full Text Search Tables
--  Tables: sysobjects, sysindexes, sysfulltextcatalogs
--  Variables: 
--		[0] - Selected database
--		[1] - Selected full text catalog
--		[2] - schema_name or user_name
--------------------------------------------------------------------------------
use [{0}]
select 
	o.name, 
	{2}(o.uid), 
	i.rowcnt,
	o.id
from 
	sysobjects o (nolock), 
	sysindexes i(nolock) 
	where o.type = 'U' 
	and ftcatid = (select isnull(ftcatid,0) from sysfulltextcatalogs where name = '{1}')
	and i.indid in (0, 1) 
	and i.id = o.id 

