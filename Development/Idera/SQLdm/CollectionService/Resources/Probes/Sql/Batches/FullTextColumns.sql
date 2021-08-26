--------------------------------------------------------------------------------
--  Batch: Full Text Search Columns
--  Tables: syscolumns, systypes, syslanguages
--  Variables: 
--		[0] - Selected database
--		[1] - Selected full text table
--------------------------------------------------------------------------------
use [{0}]

select 
	col.name,
	type = usr.name 
	+ case 
		when usr.name <> typ.name and typ.variable = 1 
			then ' (' + typ.name + '(' + case when col.length < 0 then 'max' else cast(col.length as nvarchar(8)) end + '))'
		when usr.name <> typ.name and typ.variable = 0
			then ' (' + typ.name + ')'
		when usr.name = typ.name and typ.variable = 1 
			then '(' + case when col.length < 0 then 'max' else cast(col.length as nvarchar(8)) end + ')'
		else ''
	end,
	language = sl.alias,
	columnproperty(col.id, col.name, 'IsFulltextIndexed')
from 
	syscolumns col
	left join systypes usr on usr.xusertype = col.xusertype
    left join systypes typ on col.xtype = typ.xusertype and typ.xusertype = typ.xtype
	left outer join master..syslanguages AS sl ON sl.lcid=col.language
where
	col.id={1}
    and typ.name in ('char', 'varchar', 'nchar', 'nvarchar', 'text', 'ntext', 'image', 'xml', 'varbinary')
