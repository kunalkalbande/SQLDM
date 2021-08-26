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
	ic.is_descending_key
	,ic.is_included_column
	,ic.partition_ordinal
	,ic.key_ordinal
	,c.name
FROM sys.index_columns ic
	INNER JOIN sys.columns c
		ON ((ic.object_id = c.object_id) AND (ic.column_id = c.column_id))
WHERE ((ic.object_id = {1}) AND (ic.index_id = {2}))
