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
	[Database]=db_name(database_id), 
	[IO]=sum(num_of_bytes_read + num_of_bytes_written) 
	from sys.dm_io_virtual_file_stats(null, null) 
	where database_id > 4
	group by database_id
	order by 2 desc
