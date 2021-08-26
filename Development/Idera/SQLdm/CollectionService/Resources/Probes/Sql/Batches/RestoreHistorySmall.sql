--------------------------------------------------------------------------------
--  Batch: Restore History Small
--  Tables: msdb..backupset, msdb..restorehistory, msdb..backupmediafamily
--  Variables: [0] - List of databases, [1] - days of history
--------------------------------------------------------------------------------
select 
	databasename = h.destination_database_name, 
	type = h.restore_type, 
	date = dateadd(mi,datediff(mi,getdate(),getutcdate()),h.restore_date),
	username = h.user_name, 
	path = f.physical_device_name,
	stopattime = case when h.stop_at is null then null else dateadd(mi,datediff(mi,getdate(),getutcdate()),h.stop_at) end, 
	stopatmark = h.stop_at_mark_name,
	h.replace,
	filename = 'n/a'
from 
	msdb..restorehistory h, 
	msdb..backupset b, 
	msdb..backupmediafamily f 
where 
	h.backup_set_id = b.backup_set_id 
	and f.media_set_id = b.media_set_id 
	and h.destination_database_name in ({0})
	and DateDiff(dd, h.restore_date, GETDATE()) <= {1}