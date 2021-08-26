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
	i.is_unique
	,i.type
	,i.index_id
	,i.is_padded
	,i.fill_factor
	,i.ignore_dup_key
	,i.allow_row_locks
	,i.allow_page_locks
	,i.data_space_id
	,i.is_primary_key
	,i.is_unique_constraint
FROM sys.indexes i
WHERE ((i.object_id = {1}) AND (i.name = {2}))