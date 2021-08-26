--------------------------------------------------------------------------------
--  Batch: Database Last Backup Date and Time
--  Variables: None
--------------------------------------------------------------------------------
-- Depends on this variable, previously declared:
--	@dbname nvarchar(255)

use master

select 
	max(backup_finish_date) as lastbackup
from 
	msdb..backupset 
where 
	type <> 'F' 
	and database_name = {0}
	collate database_default
