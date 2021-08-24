drop database sample
--create sample database
USE master
GO
CREATE DATABASE sample
ON 
( NAME = sample,
   FILENAME = 'c:\databases\sample.mdf',
   SIZE = 4,
   MAXSIZE = 10,
   FILEGROWTH = 5 ),
( NAME = index_1,
   FILENAME = 'c:\databases\index_1.ndf',
   SIZE = 2,
   MAXSIZE = 10,
   FILEGROWTH = 5 )
LOG ON
( NAME = sample_log,
   FILENAME = 'c:\databases\sample_log.ldf',
   SIZE = 5MB,
   MAXSIZE = 25MB,
   FILEGROWTH = 5MB )
GO

------------------------------
--step 1 - Check what the recovery model is
------------------------------
use master
select DATABASEPROPERTYEX('sample','recovery')
go
------------------------------
--step 2 - Set to full recovery model
------------------------------
use master
alter database sample set recovery FULL
go
------------------------------
--step 3 - Backup the database
------------------------------
backup database sample to disk = 'c:\samplebackup.bak'
------------------------------
--step 3.1 - Backup the log - http://forums.microsoft.com/TechNet/ShowPost.aspx?PostID=350229&SiteID=17
------------------------------
use master
backup log sample to disk = 'c:\samplelogbackup.bak'
------------------------------
--step 4 - create a database with the same name on the mirror
------------------------------
USE master
GO
CREATE DATABASE sample
ON 
( NAME = sample,
   FILENAME = 'c:\databases\sample.mdf',
   SIZE = 4,
   MAXSIZE = 10,
   FILEGROWTH = 5 ),
( NAME = index_1,
   FILENAME = 'c:\databases\index_1.ndf',
   SIZE = 2,
   MAXSIZE = 10,
   FILEGROWTH = 5 )
LOG ON
( NAME = sample_log,
   FILENAME = 'c:\databases\sample_log.ldf',
   SIZE = 5MB,
   MAXSIZE = 25MB,
   FILEGROWTH = 5MB )
GO
------------------------------
--step 5 - Restore the database to the mirror - connect to mirror server
------------------------------
--alter database sample set partner  OFF
Restore database sample 
from disk = 'c:\samplebackup.bak'
with move 'sample' to 'c:\databases\sample.mdf',
	move 'index_1' to 'c:\databases\index1.ndf',
	move 'sample_log' to 'c:\databases\sample_log.ldf',
replace,
norecovery
------------------------------
--step 5.1 - Restore the log to the mirror - connect to mirror server
------------------------------
restore log sample 
from disk = 'c:\samplelogbackup.bak'
with norecovery
------------------------------
--step 6 - Create endpoint on Principal, mirror and witness
--I will not actually do it here because I already have endpoints set up
--Only one endpoint is required per server.
--The recommended maximum number of mirrored databases is 10
------------------------------
--drop endpoint endpoint_mirroring
CREATE ENDPOINT [Mirroring] 
AS TCP (LISTENER_PORT = 5022)
FOR DATA_MIRRORING (ROLE = PARTNER, ENCRYPTION = SUPPORTED);

ALTER ENDPOINT [Mirroring] STATE = STARTED;

--mirror
CREATE ENDPOINT [Mirroring] 
AS TCP (LISTENER_PORT = 5022)
FOR DATA_MIRRORING (ROLE = PARTNER, ENCRYPTION = SUPPORTED);

ALTER ENDPOINT [Mirroring] STATE = STARTED;

--witness
CREATE ENDPOINT [Mirroring] 
AS TCP (LISTENER_PORT = 5022)
FOR DATA_MIRRORING (ROLE = WITNESS, ENCRYPTION = SUPPORTED);

ALTER ENDPOINT [Mirroring] STATE = STARTED;

------------------------------
--step 7 - Define the principle from the mirror
------------------------------
-- Specify the partner from the mirror server
ALTER DATABASE [sample] SET PARTNER =
N'TCP://bsearlel.redhouse.hq:5022';
------------------------------
--step 8 - Define the mirror from the principle
------------------------------
-- Specify the partner from the principal server
ALTER DATABASE [sample] SET PARTNER =
N'TCP://bsearled.redhouse.hq:5022';
------------------------------
--step 9 - Define the witness from the principal
------------------------------
-- Specify the witness from the principal server
ALTER DATABASE [sample] SET WITNESS =
N'TCP://contractd1.redhouse.hq:5022';
------------------------------
--step 10 - specify the safety level of the witness, starting mirroring
------------------------------
-- Set the safety level from the principal server
ALTER DATABASE [sample] SET SAFETY FULL;

--------------------------------------------------------------------------
--at this point the mirror process starts and the statuses of the servers
--should reflect the principal\mirror roles
-------------------------------------------------------------------------
------------------------------------
-- DBA wants to fail over from A to B - connect to Principal A
-------------------------------------
use master
ALTER DATABASE sample SET PARTNER FAILOVER;
------------------------------------
-- DBA wants to stop the mirroring
-------------------------------------
use master
ALTER DATABASE sample SET PARTNER OFF;
-------------------------------------
-- DBA wants to view mirror history for sample database 
-- connect to the partner whose history you want to see
-------------------------------------
use msdb
exec sys.sp_dbmmonitorresults @database_name=N'sample', @mode = 0, @update_table = 1
--
use sample
insert into test values(11)

select
'role' = case role when 0 then 'Principal' when 1 then 'Mirror' else 'DUNNO' end , 
'status' = case status when 0 then 'Suspended' when 1 then 'Disconnected' when 2 then 'Synchronizing' when 3 then 'Pending failover' when 4 then 'Synchronized' end,
'witness_status' = case witness_status when 1 then 'Connected' when 2 then 'Disconnected' end, log_flush_rate, send_queue_size, send_rate, redo_queue_size, redo_rate, transaction_delay, transactions_per_sec, time, end_of_log_lsn, failover_lsn, local_time
from msdb.dbo.dbm_monitor_data
where database_id = db_id('sample') 
order by time desc

exec sys.sp_dbmmonitorresults @database_name=N'sample', @mode = 4, @update_table = 1

SELECT * FROM sys.dm_os_performance_counters
WHERE OBJECT_NAME = 'SQLServer:Database Mirroring'



select * from sys.database_mirroring
select * from sys.database_mirroring_endpoints
select * from sys.database_mirroring_witnesses
select * from sys.dm_db_mirroring_connections
