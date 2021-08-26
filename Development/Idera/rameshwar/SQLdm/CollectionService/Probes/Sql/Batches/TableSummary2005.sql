--------------------------------------------------------------------------------
--  Batch: Table Summary 2005
--  Tables: sys.allocation_units,sys.partitions,sys.objects,sys.schemas
--		sys.internal_tables,sys.filegroups, sys.indexes, sysobjects,sys.fulltext_catalogs
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

-- Prepare the XML index size information for later use
declare @xmlIndexes table
(parentid int, usedpages dec(12,0))

insert into @xmlIndexes
select
	parentid = it.parent_id,
	usedpages = sum(convert(dec(12,0),au.used_pages)) 
from 
	sys.partitions p, 
	sys.allocation_units au, 
	sys.internal_tables it
where 
	p.partition_id = au.container_id 
	and p.object_id = it.object_id 
	and it.internal_type in (202,204) 
group by it.parent_id


-- Main size calculation query
-- Any change to sizestats() should be considered high risk - make changes to outer query where possible
;with sizestats(
	objectid,
	DataSizeKB,
	TextSizeKB,
	UsedPages,
	Rows)
as
(
select
	p.object_id,
	-- SQLDM-28603. Update TableSize calculation
	-- Update DataSize calculation for all types of indexes.
	DataSizeKB = sum(case when it.internal_type in (202,204) then 0 else convert(dec(12,0),au.used_pages) end) * 8,
	-- Update TextSize calculation for all types of data except dropped rows data.
	TextSizeKB = sum(case when it.internal_type in (202,204) then 0 when au.type != 0 then convert(dec(12,0),au.data_pages) else 0 end) * 8, 
	UsedPages = sum(convert(dec(12,0),au.used_pages)) * 8,
	Rows = sum(case when (p.index_id < 2) and (au.type = 1) then p.rows	else 0 end)
from
	sys.partitions p
	inner join sys.allocation_units au on p.partition_id = au.container_id
	left join sys.internal_tables it on p.object_id = it.object_id 
where au.type = 2
group by
	p.object_id

UNION

select
	p.object_id,
	-- SQLDM-28603. Update TableSize calculation
	-- Update DataSize calculation for all types of indexes.
	DataSizeKB = sum(case when it.internal_type in (202,204) then 0 else convert(dec(12,0),au.used_pages) end) * 8,
	-- Update TextSize calculation for all types of data except dropped rows data.
	TextSizeKB = sum(case when it.internal_type in (202,204) then 0 when au.type != 0 then convert(dec(12,0),au.data_pages) else 0 end) * 8, 
	UsedPages = sum(convert(dec(12,0),au.used_pages)) * 8,
	Rows = sum(case when (p.index_id < 2) and (au.type = 1) then p.rows	else 0 end)
from
	sys.partitions p
	inner join sys.allocation_units au on p.hobt_id = au.container_id 
	left join sys.internal_tables it on p.object_id = it.object_id 
where au.type in (1,3) 
group by
	p.object_id

)
select 
	ObjectID = o.object_id,
	DBName = db_name(),
	TableName = o.name,
	DataSizeKB = size.DataSizeKB,
	IndexSizeKB = 
		case 
			when size.UsedPages < 0 then -1  --No data
			else 
			isnull(
			((size.UsedPages + (isnull(xi.usedpages,0) * 8 )) -- Add XML indexes to pages used
			- size.TextSizeKB) -- Subtract all data pages
			,0) end,
	TextSizeKB = size.TextSizeKB,
	NumRows = size.Rows,
	Owner = isnull(s.name,'unknown'),
	issystemtable = o.is_ms_shipped,
	DataSpaceName = ds.name,
	CreateDate = o.create_date,
	bcp = case when userstat & 2 = 2 then cast(1 as bit) else cast(0 as bit) end,
	fulltext = f.name,
	pinned = cast(0 as bit),
	DataSpaceType = ds.type
into
	#tablesummary
from 
	sys.objects o
	inner join sysobjects o2 on (o.object_id = o2.id)
	inner join sizestats size on (o.object_id = size.objectid)
	inner join sys.indexes i on (i.object_id = o.object_id and i.index_id < 2)
	left join sys.schemas s on ( o.schema_id = s.schema_id ) 
	left join sys.fulltext_catalogs f on o2.ftcatid = f.fulltext_catalog_id
	left join @xmlIndexes xi on o.object_id = xi.parentid
	left join sys.data_spaces ds on ds.data_space_id = i.data_space_id
where
	(db_name() <> 'tempdb' or o.name not like '#%' )
	and o.type in ('U','S')


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

drop table #tablesummary