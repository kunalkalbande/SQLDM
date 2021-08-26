select 
[DatabaseName]=db_name(),
[Schema]= ( Select sch.name from sys.objects o inner join sys.schemas sch on o.schema_id = sch.schema_id where o.object_id = asm.object_id),
[ObjectName]=object_name(asm.object_id),
[ObjectType]=ao.type_desc,
[SQL]=asm.definition, 
[uses_ansi_nulls]=asm.uses_ansi_nulls, 
[uses_quoted_identifier]=asm.uses_quoted_identifier
from sys.all_sql_modules asm
inner join sys.all_objects ao on ao.object_id = asm.object_id
where ao.type in ('P','V') and is_ms_shipped = 0 and 
(uses_ansi_nulls = 0 or uses_quoted_identifier = 0)
