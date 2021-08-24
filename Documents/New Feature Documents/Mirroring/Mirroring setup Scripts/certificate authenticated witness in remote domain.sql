-----------------------------
--on A
--from witness
-----------------------------
USE master;
CREATE LOGIN Clover_Witness_login WITH PASSWORD = '3Sample_Strong_Password!@#';
GO

CREATE USER Clover_Witness_user FOR LOGIN Clover_Witness_login;
GO
CREATE CERTIFICATE Clover_Witness_cert
   AUTHORIZATION Clover_Witness_user
   FROM FILE = 'C:\Clover_Witness_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [Clover_Witness_login];
GO

-----------------------------
--from witness
-----------------------------
USE master;
CREATE LOGIN Clover_Witness_login WITH PASSWORD = '3Sample_Strong_Password!@#';
GO

CREATE USER Clover_Witness_user FOR LOGIN Clover_Witness_login;
GO
CREATE CERTIFICATE Clover_Witness_cert
   AUTHORIZATION Clover_Witness_user
   FROM FILE = 'C:\Clover_Witness_cert.cer'
GO
GRANT CONNECT ON ENDPOINT::Endpoint_Mirroring TO [Clover_Witness_login];
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
    SET WITNESS = 'TCP://smm-clover.simpsons.qa:7024';
GO
--Change to high-performance mode by turning off transacton safety.
ALTER DATABASE sample3 
    SET PARTNER SAFETY FULL
GO

select * from sys.database_mirroring
select * from sys.database_mirroring_endpoints
select * from sys.database_mirroring_witnesses
select * from sys.dm_db_mirroring_connections