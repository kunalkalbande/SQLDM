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
	sit.tessellation_scheme
	,sit.bounding_box_xmin
	,sit.bounding_box_ymin
	,sit.bounding_box_xmax
	,sit.bounding_box_ymax
	,sit.level_1_grid_desc
	,sit.level_2_grid_desc
	,sit.level_3_grid_desc
	,sit.level_4_grid_desc
	,sit.cells_per_object
FROM sys.spatial_index_tessellations sit
WHERE ((sit.object_id = {1}) AND (sit.index_id = {2}))