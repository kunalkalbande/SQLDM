SELECT
dme.role AS [ServerMirroringRole],
ISNULL(cert.name,N'') AS [Certificate],
case 
                            when dme.encryption_algorithm = 0 then 0 
                            when dme.encryption_algorithm in (3,4,7,8) then 1 
                            when dme.encryption_algorithm in (1,2,5,6) then 2 
                            else 0 
                        end
         AS [EndpointEncryption],
case dme.encryption_algorithm 
                            when 0 then 0
                            when 1 then 1
                            when 2 then 2
                            when 3 then 1
                            when 4 then 2
                            when 5 then 4
                            when 6 then 3
                            when 7 then 4
                            when 8 then 3
                            else 0
                        end
         AS [EndpointEncryptionAlgorithm],
dme.connection_auth AS [EndpointAuthenticationOrder],
CAST(case when dme.endpoint_id < 65536 then 1 else 0 end AS bit) AS [IsSystemObject]
FROM
sys.endpoints AS e
INNER JOIN sys.database_mirroring_endpoints AS dme ON dme.endpoint_id=e.endpoint_id
LEFT OUTER JOIN sys.certificates AS cert ON cert.certificate_id = dme.certificate_id
WHERE
(e.name=N'Mirroring')

