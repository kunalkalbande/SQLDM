--------------------------------------------------------------------------------
--  Batch: Restore History Full
--  Tables: msdb..backupset, msdb..restorehistory, msdb..backupmediafamily, 
--	msdb..backupfile 
--  Variables: [0] - List of databases, [1] - days of history
--------------------------------------------------------------------------------
select
	databasename = b.database_name, 
	type = h.restore_type, 
	date = dateadd(mi,datediff(mi,getdate(),getutcdate()),h.restore_date),
	username = h.user_name, 
	path = m.physical_device_name,
	stopattime = case when h.stop_at is null then null else dateadd(mi,datediff(mi,getdate(),getutcdate()),h.stop_at) end, 
	stopatmark = h.stop_at_mark_name,
	h.replace,
	filename = rtrim(f.logical_name) 
from 
	msdb..backupset b, 
	msdb..restorehistory h, 
	msdb..backupmediafamily m, 
	msdb..backupfile f
where 
	h.backup_set_id = b.backup_set_id 
	and m.media_set_id = b.media_set_id 
	and h.backup_set_id = f.backup_set_id 
	and b.database_name in ({0})
	and DateDiff(dd, h.restore_date, GETDATE()) <= {1}
	