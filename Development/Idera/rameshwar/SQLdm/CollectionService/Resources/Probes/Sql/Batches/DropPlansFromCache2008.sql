--------------------------------------------------------------------------------
--  Batch: Drop plans from cache
--------------------------------------------------------------------------------
set nocount on
declare @plans table(id int identity, handle varchar(1000))
declare @id int, @command nvarchar(max)

insert into @plans(handle)
select distinct rtrim(convert(varchar(1000),plan_handle,1))
from 
sys.dm_exec_query_stats as qs
cross apply sys.dm_exec_sql_text(sql_handle) AS qt
where 
text like '%SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm%'

select @id = min(id) from @plans

while (isnull(@id,0) > 0)
begin
	select @command = 'dbcc freeproccache(' + handle + ') --SQLdmJjRj9pSdyOG85wJ6vVUDVK01sWrvSEy3bQPeCFgkveDvpawqXebqXRh8EE7tkcrNXepaFFn2MQ0vWDDgU4PLgOAFQaxLIgkP0sxaETBdh74x2YUc5u6AZQD1vxC4lYw1gt7jjBdUG2OSxr6Ecbhd1uGVtcDpPTbvbLck42shrtOIStIDoNbMufBVdmvERyeqM7NjEZhvSQLdm '
	from @plans where id = @id
	execute(@command)
	delete from @plans where id = @id
	select @id = min(id) from @plans
end 