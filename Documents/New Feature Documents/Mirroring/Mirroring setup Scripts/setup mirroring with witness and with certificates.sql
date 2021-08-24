--Host A
USE master;
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '<1_Strong_Password!>';
GO
USE master;
CREATE CERTIFICATE HOST_A_cert 
   WITH SUBJECT = 'HOST_A certificate';
GO

CREATE ENDPOINT Endpoint_Mirroring
   STATE = STARTED
   AS TCP (
      LISTENER_PORT=7024
      , LISTENER_IP = ALL
   ) 
   FOR DATABASE_MIRRORING ( 
      AUTHENTICATION = CERTIFICATE HOST_A_cert
      , ENCRYPTION = REQUIRED ALGORITHM AES
      , ROLE = ALL
   );
GO

BACKUP CERTIFICATE HOST_A_cert TO FILE = 'C:\HOST_A_cert.cer';
GO
-------------------------------
---now go to host B
-------------------------------
USE master;
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '<Strong_Password_#2>';
GO
CREATE CERTIFICATE HOST_B_cert 
   WITH SUBJECT = 'HOST_B certificate for database mirroring';
GO

CREATE ENDPOINT Endpoint_Mirroring
   STATE = STARTED
   AS TCP (
      LISTENER_PORT=7024
      , LISTENER_IP = ALL
   ) 
   FOR DATABASE_MIRRORING ( 
      AUTHENTICATION = CERTIFICATE HOST_B_cert
      , ENCRYPTION = REQUIRED ALGORITHM AES
      , ROLE = ALL
   );
GO
BACKUP CERTIFICATE HOST_B_cert TO FILE = 'C:\HOST_B_cert.cer';
GO 
-------------------------------
---now go to witness - configure outbound connections
-------------------------------
USE master;
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '<Strong_Password_#3>';
GO
CREATE CERTIFICATE Witness_cert 
   WITH SUBJECT = 'Witness certificate for database mirroring';
GO

CREATE ENDPOINT Endpoint_Mirroring
   STATE = STARTED
   AS TCP (
      LISTENER_PORT=7024
      , LISTENER_IP = ALL
   ) 
   FOR DATABASE_MIRRORING ( 
      AUTHENTICATION = CERTIFICATE Witness_cert
      , ENCRYPTION = REQUIRED ALGORITHM AES
      , ROLE = ALL
   );
GO
BACKUP CERTIFICATE Witness_cert TO FILE = 'C:\Witness_cert.cer';
GO 

----------------------------------------
-- Now configure inbound on host A
--from B
----------------------------------------
drop login host_b_login
drop user host_b_user
drop certificate host_b_cert

USE master;
CREATE LOGIN HOST_B_login WITH PASSWORD = '1Sample_Strong_Password!@#';
GO

CREATE USER HOST_B_user FOR LOGIN HOST_B_login;
GO
CREATE CERTIFICATE HOST_B_cert
   AUTHORIZATION HOST_B_user
   FROM FILE = 'C:\HOST_B_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [HOST_B_login];
GO
-----------------------------
--from witness
-----------------------------
USE master;
CREATE LOGIN Witness_login WITH PASSWORD = '3Sample_Strong_Password!@#';
GO

CREATE USER Witness_user FOR LOGIN witness_login;
GO
CREATE CERTIFICATE Witness_cert
   AUTHORIZATION Witness_user
   FROM FILE = 'C:\Witness_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [Witness_login];
GO

--------------------------
--Configure B for inbound connections
--------------------------
USE master;
CREATE LOGIN HOST_A_login WITH PASSWORD = '=Sample#2_Strong_Password2';
GO
CREATE USER HOST_A_user FOR LOGIN HOST_A_login;
GO
CREATE CERTIFICATE HOST_A_cert
   AUTHORIZATION HOST_A_user
   FROM FILE = 'C:\HOST_A_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [HOST_A_login];
GO
---------------------------------
--from witness
---------------------------------
USE master;
CREATE LOGIN Witness_login WITH PASSWORD = '3Sample_Strong_Password!@#';
GO

CREATE USER Witness_user FOR LOGIN witness_login;
GO
CREATE CERTIFICATE Witness_cert
   AUTHORIZATION Witness_user
   FROM FILE = 'C:\Witness_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [Witness_login];
GO
--------------------------
--Configure Witness for inbound connections
--from A
--------------------------
USE master;
CREATE LOGIN HOST_A_login WITH PASSWORD = '=Sample#2_Strong_Password2';
GO
CREATE USER HOST_A_user FOR LOGIN HOST_A_login;
GO
CREATE CERTIFICATE HOST_A_cert
   AUTHORIZATION HOST_A_user
   FROM FILE = 'C:\HOST_A_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [HOST_A_login];
GO
--------------------------
--from B
--------------------------
USE master;
CREATE LOGIN HOST_B_login WITH PASSWORD = '=Sample#3_Strong_Password2';
GO
CREATE USER HOST_B_user FOR LOGIN HOST_B_login;
GO
CREATE CERTIFICATE HOST_B_cert
   AUTHORIZATION HOST_B_user
   FROM FILE = 'C:\HOST_B_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [HOST_B_login];
GO

---------------------------------
--configure partner databases
--------------------------------
--on host b set up A
--At HOST_B, set server instance on HOST_A as partner (principal server):
ALTER DATABASE sample3 
    SET PARTNER = 'TCP://bsearlel.redhouse.hq:7024';
GO

--At HOST_A, set server instance on HOST_B as partner (mirror server).
ALTER DATABASE sample3 
    SET PARTNER = 'TCP://bsearled.redhouse.hq:7024';
GO
--at principle(can only happen at principal. Mirror might have already stated) set witness
ALTER DATABASE sample3 
    SET WITNESS = 'TCP://contractd1.redhouse.hq:7024';
GO
--Change to high-performance mode by turning off transacton safety.
ALTER DATABASE sample3 
    SET PARTNER SAFETY FULL
GO

alter endpoint Endpoint_Mirroring  state = started

select * from sys.endpoints
select * from sys.database_mirroring
select * from sys.database_mirroring_endpoints
select * from sys.database_mirroring_witnesses
select * from sys.dm_db_mirroring_connections