--------------------------------------------------------------------------------
--  Batch: Table Details 2000
--  Tables: 	sysindexes, sysobjects, sysusers, sysfilegroups, sysfulltextcatalogs
--  Variables: 
--		[0] - Target Database
--		[1] - Include System and/or User tables
--------------------------------------------------------------------------------
use [{0}]

declare @compat float

select 
	@compat = convert(float, cmptlevel)
from 
	master..sysdatabases 
where 
	dbid  = db_id()

select @compat

if (@compat <> 60)
begin
	use [{0}]
end
else
begin
	use [master]
end

if (@compat <> 60)
begin

select 
	ObjectId = o.id,
	DBName = db_name(),  --Database Name
	TableName = o.name,       --Table Name
	DataSizeKB = --Defined as data pages used (from sysindexes)
	sum(
	  case when indid in (0,1) -- Exclude text/image data
		  then (convert(dec(12,0),dpages) * 8)  
		else 0 
	  end), 
	IndexSizeKB =  -- This is the difference between pages used and data pages used
	sum(
		case when indid in (0,1) -- Exclude text/image data
			then (convert(dec(12,0),used) * 8) - (convert(dec(12,0),dpages) * 8)  
		else 0 
	end),  
	TextSizeKB =  -- Text/image data is special and is calculated as pages used for indid 255
	 sum(
	  case when indid = 255  --Indid 255 always refers to text/image for 2000
		  then (convert(dec(12,0),used) * 8) 
		else 0 
	  end), 
	NumRows = sum(case when indid < 2 then i.rows else 0 end ),      --Rows in table
	Owner = ISNULL(u.name,'unknown'),--Table Owner 
	issystemtable = case when category & 2 = 2 then cast(1 as bit) else cast(0 as bit) end,
	filegroupname = case when (i.groupid = 0) then 'No File' else fg.groupname end,
	createddate = o.crdate,
	bcp = case when userstat & 2 = 2 then cast(1 as bit) else cast(0 as bit) end,
	fulltext = f.name,
	pinned = case when o.status & 1048576 <> 0 then cast(1 as bit) else cast(0 as bit) end
into #tablesummary
from 
	[{0}]..sysindexes i (nolock)
	inner join [{0}]..sysobjects o (nolock) on i.id = o.id 
	left join [{0}]..sysusers u (nolock) on o.uid = u.uid 
	left join [{0}]..sysfilegroups fg on i.groupid = fg.groupid
	left join [{0}]..sysfulltextcatalogs f on o.ftcatid = f.ftcatid
where 
	i.indid in (0, 1, 255) 
	and (db_name() <> 'tempdb' or o.name not like '#%' )
	and o.type in ('U','S')
group by o.name, o.id, o.type, o.uid, u.name, o.category, fg.groupname, i.groupid,o.crdate,userstat,f.name, o.status
  order by o.name

select * 
from
	#tablesummary
{1}

select 	
	sum(DataSizeKB), 
	sum(IndexSizeKB),
	sum(TextSizeKB)
from
	#tablesummary

end
