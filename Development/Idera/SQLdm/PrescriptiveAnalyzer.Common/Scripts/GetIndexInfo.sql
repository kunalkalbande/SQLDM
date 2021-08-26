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


select 
	[ObjectName]=object_name({0}),
	[IsTable]=objectproperty({0}, 'IsTable'),
	[IsMSShipped]=objectproperty({0}, 'IsMSShipped'),
	[IndexName]=(select name from sys.indexes where object_id = {0} and index_id = {1}),
	[SchemaName]= (select s.name from sys.objects o inner join sys.schemas s on o.schema_id = s.schema_id where o.object_id = {0});
