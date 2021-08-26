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
USE {0}

SELECT
	ds.name
FROM sys.tables t
	INNER JOIN sys.data_spaces ds
		ON (t.filestream_data_space_id = ds.data_space_id)
WHERE (t.object_id = {1})