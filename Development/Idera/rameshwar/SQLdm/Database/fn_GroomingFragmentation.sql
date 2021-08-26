
if (object_id('Grooming.fn_GroomingFragmentation') is not null)
begin
drop function Grooming.fn_GroomingFragmentation
end
go

create function Grooming.fn_GroomingFragmentation (
@database_id int,
@object_id int,
@index_id int)

returns @fragmentation table (
database_id smallint null, 
object_id int null, 
index_id int null, 
partition_number int null,
avg_fragmentation_in_percent float null)

begin

insert into @fragmentation 
select
database_id,
object_id,
index_id,
partition_number,
avg_fragmentation_in_percent
from
sys.dm_db_index_physical_stats (@database_id, @object_id, @index_id, null, 'limited')

return

end;