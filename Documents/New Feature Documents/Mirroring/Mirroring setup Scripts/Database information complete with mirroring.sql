
                create table #tmp_sp_db_vardecimal_storage_format (dbname sysname null, vardecimal_enabled varchar(3) null)
                if exists (select o.object_id from sys.system_objects o where o.name=N'sp_db_vardecimal_storage_format')
                begin
                    insert into #tmp_sp_db_vardecimal_storage_format exec sys.sp_db_vardecimal_storage_format
                end
            


SELECT
dtb.name AS [Name],
(select count(*) from master.dbo.sysprocesses p where dtb.database_id=p.dbid) AS [ActiveConnections],
dtb.compatibility_level AS [CompatibilityLevel],
dtb.create_date AS [CreateDate],
CAST(0 AS float) AS [DataSpaceUsage],
CAST(0 AS bit) AS [DboLogin],
(select top 1 ds.name from sys.data_spaces as ds where ds.is_default=1) AS [DefaultFileGroup],
'' AS [DefaultSchema],
dtb.database_id AS [ID],
CAST(0 AS float) AS [IndexSpaceUsage],
CAST(has_dbaccess(dtb.name) AS bit) AS [IsAccessible],
CAST(0 AS bit) AS [IsDbAccessAdmin],
CAST(0 AS bit) AS [IsDbBackupOperator],
CAST(0 AS bit) AS [IsDbDatareader],
CAST(0 AS bit) AS [IsDbDatawriter],
CAST(0 AS bit) AS [IsDbDdlAdmin],
CAST(0 AS bit) AS [IsDbDenyDatareader],
CAST(0 AS bit) AS [IsDbDenyDatawriter],
CAST(0 AS bit) AS [IsDbOwner],
CAST(0 AS bit) AS [IsDbSecurityAdmin],
dtb.is_fulltext_enabled AS [IsFullTextEnabled],
CAST(case when dtb.name in ('master','model','msdb','tempdb') then 1 else dtb.is_distributor end AS bit) AS [IsSystemObject],
(select max(backup_finish_date) from msdb..backupset where type = 'D' and database_name = dtb.name) AS [LastBackupDate],
(select max(backup_finish_date) from msdb..backupset where type = 'L' and database_name = dtb.name) AS [LastLogBackupDate],
suser_sname(dtb.owner_sid) AS [Owner],
null AS [PrimaryFilePath],
dtb.is_published*1+dtb.is_subscribed*2+dtb.is_merge_published*4 AS [ReplicationOptions],
CAST(0 AS float) AS [Size],
CAST(0 AS float) AS [SpaceAvailable],

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
'' AS [UserName],
CAST(CHARINDEX(N'_CS_', CAST(DATABASEPROPERTYEX(dtb.name, 'Collation') AS nvarchar(255))) AS bit) AS [CaseSensitive],
dtb.collation_name AS [Collation],
CAST(( case LOWER(convert( nvarchar(128), DATABASEPROPERTYEX(dtb.name, 'Updateability'))) when 'read_write' then 1 else 0 end) AS bit) AS [IsUpdateable],
CAST(DATABASEPROPERTYEX(dtb.name, 'Version') AS int) AS [Version],
dtb.is_auto_create_stats_on AS [AutoCreateStatisticsEnabled],
dtb.is_auto_update_stats_on AS [AutoUpdateStatisticsEnabled],
drs.database_guid AS [DatabaseGuid],
ISNULL(DB_NAME(dtb.source_database_id), N'') AS [DatabaseSnapshotBaseName],
ISNULL((select top 1 ftc.name from sys.fulltext_catalogs as ftc where ftc.is_default=1),N'') AS [DefaultFullTextCatalog],
CAST(isnull(dtb.source_database_id, 0) AS bit) AS [IsDatabaseSnapshot],
CAST((select count(1) from sys.databases dtbmir where dtbmir.source_database_id = dtb.database_id) AS bit) AS [IsDatabaseSnapshotBase],
0 AS [IsMailHost],
CAST(case when dmi.mirroring_partner_name is null then 0 else 1 end AS bit) AS [IsMirroringEnabled],

                case
                    when vardec.vardecimal_enabled = 'ON' then cast(1 as bit)
                    else cast(0 as bit)
                end
             AS [IsVarDecimalStorageFormatEnabled],
dtb.log_reuse_wait AS [LogReuseWaitStatus],
dmi.mirroring_failover_lsn AS [MirroringFailoverLogSequenceNumber],
dmi.mirroring_guid AS [MirroringID],
dmi.mirroring_partner_name AS [MirroringPartner],
dmi.mirroring_partner_instance AS [MirroringPartnerInstance],
dmi.mirroring_role AS [MirroringRole],
dmi.mirroring_role_sequence AS [MirroringRoleSequence],
coalesce(dmi.mirroring_safety_level + 1, 0) AS [MirroringSafetyLevel],
dmi.mirroring_safety_sequence AS [MirroringSafetySequence],
coalesce(dmi.mirroring_state + 1, 0) AS [MirroringStatus],
dmi.mirroring_witness_name AS [MirroringWitness],
coalesce(dmi.mirroring_witness_state + 1, 0) AS [MirroringWitnessStatus],
drs.recovery_fork_guid AS [RecoveryForkGuid],
dtb.service_broker_guid AS [ServiceBrokerGuid],
dtb.name AS [DatabaseName],
dtb.name AS [DatabaseName2]
FROM
master.sys.databases AS dtb
LEFT OUTER JOIN sys.master_files AS df ON df.database_id = dtb.database_id and 1=df.data_space_id and 1 = df.file_id
LEFT OUTER JOIN sys.database_recovery_status AS drs ON drs.database_id = dtb.database_id
LEFT OUTER JOIN sys.database_mirroring AS dmi ON dmi.database_id = dtb.database_id
LEFT OUTER JOIN #tmp_sp_db_vardecimal_storage_format as vardec ON dtb.name = vardec.dbname
WHERE
(dtb.name=N'sample')

                drop table #tmp_sp_db_vardecimal_storage_format
            
