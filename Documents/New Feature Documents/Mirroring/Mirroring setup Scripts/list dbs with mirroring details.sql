SELECT
dtb.name AS [Name],
dtb.database_id AS [ID],
suser_sname(dtb.owner_sid) AS [Owner],
dtb.create_date AS [CreateDate],
dtb.is_published*1+dtb.is_subscribed*2+dtb.is_merge_published*4 AS [ReplicationOptions],
dtb.service_broker_guid AS [ServiceBrokerGuid],
dtb.is_fulltext_enabled AS [IsFullTextEnabled],
CAST(isnull(dtb.source_database_id, 0) AS bit) AS [IsDatabaseSnapshot],
ISNULL(DB_NAME(dtb.source_database_id), N'') AS [DatabaseSnapshotBaseName],
dmi.mirroring_partner_name AS [MirroringPartner],
dmi.mirroring_partner_instance AS [MirroringPartnerInstance],
dmi.mirroring_role AS [MirroringRole],
coalesce(dmi.mirroring_safety_level + 1, 0) AS [MirroringSafetyLevel],
coalesce(dmi.mirroring_state + 1, 0) AS [MirroringStatus],
dmi.mirroring_witness_name AS [MirroringWitness],
coalesce(dmi.mirroring_witness_state + 1, 0) AS [MirroringWitnessStatus],
CAST(case when dmi.mirroring_partner_name is null then 0 else 1 end AS bit) AS [IsMirroringEnabled],
dmi.mirroring_guid AS [MirroringID],
dmi.mirroring_role_sequence AS [MirroringRoleSequence],
dmi.mirroring_safety_sequence AS [MirroringSafetySequence],
dmi.mirroring_failover_lsn AS [MirroringFailoverLogSequenceNumber],
dtb.log_reuse_wait AS [LogReuseWaitStatus],

case 
    when DATABASEPROPERTY(dtb.name,'IsShutDown') is null then 0x200 
    else 0 
end |
case 
    when 1 = dtb.is_in_standby then 0x40 
    else 0 
end |
case 
    when 1 = dtb.is_cleanly_shutdown then 0x80 
    else 0 
end |
case dtb.state 
    when 1 then 0x2   
    when 2 then 0x8 
    when 3 then 0x4 
    when 4 then 0x10 
    when 5 then 0x100 
    when 6 then 0x20 
    else 1
end
             AS [Status],
drs.recovery_fork_guid AS [RecoveryForkGuid],
drs.database_guid AS [DatabaseGuid],
dtb.is_auto_create_stats_on AS [AutoCreateStatisticsEnabled],
dtb.is_auto_update_stats_on AS [AutoUpdateStatisticsEnabled],
CAST(case when dtb.name in ('master','model','msdb','tempdb') then 1 else dtb.is_distributor end AS bit) AS [IsSystemObject],
CAST(( case LOWER(convert( nvarchar(128), DATABASEPROPERTYEX(dtb.name, 'Updateability'))) when 'read_write' then 1 else 0 end) AS bit) AS [IsUpdateable],
CAST(CHARINDEX(N'_CS_', CAST(DATABASEPROPERTYEX(dtb.name, 'Collation') AS nvarchar(255))) AS bit) AS [CaseSensitive],
CAST(has_dbaccess(dtb.name) AS bit) AS [IsAccessible]
FROM
master.sys.databases AS dtb
LEFT OUTER JOIN sys.database_mirroring AS dmi ON dmi.database_id = dtb.database_id
LEFT OUTER JOIN sys.database_recovery_status AS drs ON drs.database_id = dtb.database_id
where dmi.mirroring_guid is not null

select * from sys.endpoints where type = 4
select 'protocol' = coalesce(te.protocol_desc, he.protocol_desc), 'port' = coalesce(te.port, case he.is_clear_port_enabled when 1 then clear_port else ssl_port end),* from sys.database_mirroring_endpoints dme 
left outer join sys.tcp_endpoints te on dme.name = te.name
left outer join sys.http_endpoints he on dme.name = he.name
where dme.type = 4 --DATABASE_MIRRORING

SELECT * FROM sys.tcp_endpoints;   
SELECT * FROM sys.http_endpoints;              