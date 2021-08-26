-- SQL doctor
-- Copyright © 2010-2013, Idera, Inc., All Rights Reserved.

set transaction isolation level read uncommitted; 
set lock_timeout 20000; 
set implicit_transactions off; 
if @@trancount > 0 commit transaction; 
set language us_english; 
set cursor_close_on_commit off; 
set query_governor_cost_limit 0; 
set numeric_roundabort off; 
set deadlock_priority low; 
set nocount on; 

select top 100 
	[Database]=db_name(st.database_id), 
	[ObjectID]=st.object_id,
	[IO]=sum((st.user_seeks*10) + (st.user_scans*2) + st.user_lookups)
	from sys.dm_db_index_usage_stats st 
	where st.database_id > 4
	group by st.database_id, st.object_id
	order by 3 desc;
