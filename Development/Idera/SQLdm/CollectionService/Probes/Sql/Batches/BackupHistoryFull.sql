--------------------------------------------------------------------------------
--  Batch: Backup History Full
--  Tables: msdb..backupset, msdb..backupmediafamily, msdb..backupfile 
--  Variables: [0] - List of databases, [1] - days of history
--------------------------------------------------------------------------------
select 
	databasename = s.database_name,
	type = s.type, 
	date = dateadd(mi,datediff(mi,getdate(),getutcdate()),s.backup_finish_date),
	username = s.user_name, 
	size = convert(bigint,f.backed_up_page_count) * 8 * 1024,
	path = m.physical_device_name,
	rtrim(f.logical_name) 
from 
 msdb..backupset s, 
 msdb..backupmediafamily m, 
 msdb..backupfile f
where 
 s.media_set_id = m.media_set_id 
 and s.backup_set_id = f.backup_set_id 
 and s.database_name in ({0})
  and datediff(dd, s.backup_finish_date, getdate()) <= {1}
