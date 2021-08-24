ALTER DATABASE AdventureWorks 
    SET WITNESS = 
    'TCP://bsearlel.redhouse.hq:5023'
GO

ALTER ENDPOINT [Mirroring] STATE = STARTED;

SELECT *
FROM sys.database_mirroring_endpoints;


SELECT 
      DB_NAME(database_id) AS 'DatabaseName'
    , mirroring_role_desc 
    , mirroring_safety_level_desc
    , mirroring_state_desc
    , mirroring_safety_sequence
    , mirroring_role_sequence
    , mirroring_partner_instance
    , mirroring_witness_name
    , mirroring_witness_state_desc
    , mirroring_failover_lsn
FROM sys.database_mirroring
WHERE mirroring_guid IS NOT NULL;

drop endpoint mirroring 

CREATE ENDPOINT [Mirroring] 
AS TCP (LISTENER_PORT = 5022)
FOR DATA_MIRRORING (ROLE = PARTNER);

ALTER DATABASE AdventureWorks 
  SET WITNESS = 'TCP://contractd1.redhouse.hq:5022'

-- Specify the witness from the principal server
ALTER DATABASE [AdventureWorks] SET WITNESS =
N'TCP://contractd1.redhouse.hq:5022';

SELECT 
      Database_name
    , safety_level_desc
    , safety_sequence_number
    , role_sequence_number
    , is_suspended
    , is_suspended_sequence_number
    , principal_server_name
    , mirror_server_name
FROM sys.database_mirroring_witnesses;

-- Specify the witness from the principal server
ALTER DATABASE [AdventureWorks] SET WITNESS =
N'TCP://bsearlel.redhouse.hq:5026';
