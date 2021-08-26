--------------------------------------------------------------------------------
--  Batch: Backup History Small
--  Tables: msdb..backupset, msdb..backupmediafamily
--  Variables: [0] - List of databases, [1] - days of history
--------------------------------------------------------------------------------
select 
	databasename = s.database_name, 
	type = s.type, 
	date = dateadd(mi,datediff(mi,getdate(),getutcdate()),s.backup_finish_date),
	username = s.user_name, 
	size = convert(bigint,s.backup_size),
	path = f.physical_device_name,
	filename = 'n/a'
from 
	 msdb..backupset s, 
	 msdb..backupmediafamily f 
where 
	 s.media_set_id = f.media_set_id 
	 and s.database_name in ({0})
	 and datediff(dd, s.backup_finish_date, getdate()) <= {1}
