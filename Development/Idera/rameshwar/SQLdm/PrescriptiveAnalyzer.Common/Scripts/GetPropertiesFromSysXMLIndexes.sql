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
	x.using_xml_index_id
	, x.secondary_type
FROM sys.xml_indexes x
WHERE ((x.object_id = {1}) AND (x.index_id = {2}))