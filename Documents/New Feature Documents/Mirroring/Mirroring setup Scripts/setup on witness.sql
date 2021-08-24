
drop endpoint mirroring

CREATE ENDPOINT Mirroring
    STATE=STARTED 
    AS TCP (LISTENER_PORT=5026) 
    FOR DATABASE_MIRRORING (ROLE=WITNESS)
GO
 
ALTER ENDPOINT [Endpoint_Mirroring] STATE = STARTED;

SELECT *
FROM sys.database_mirroring_endpoints;