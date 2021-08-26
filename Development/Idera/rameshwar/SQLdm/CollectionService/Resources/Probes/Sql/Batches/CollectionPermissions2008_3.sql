
---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get  Permissions Information for Current User
--  Batch: Get Permissions Batch (Permissions 2008 and above)
--  Description: Collects all the  Permissions related information for the user executing the batch
---------------------------------------------------------------------------------------------------

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @PermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @PermissionValue INT

---------------------------------------------------------------------------------------------------
-- Permission: ALTER ANY DATABASE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- BackupAndRecovery2005                                                               
-- Configuration2012                                                                   
-- DatabaseConfiguration2012                                                           
-- DatabaseSize2012                                                                    
-- DBSecurity2005                                                                      
-- DiskSize2005                                                                        
-- FileActivity2005                                                                    
-- FragmentationWorkload2005                                                           
-- GetWorstFillFactorIndexes2005                                                       
-- MirroredDatabaseScheduled2005                                                       
-- MirrorMetricsUpdate2005                                                             
-- MirrorMonitoringRealtime2005                                                        
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER ANY DATABASE'
INSERT INTO @PermissionsTable VALUES(
    'ALTERANYDATABASE',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER ANY EVENT SESSION
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ActiveWaits2012                                                                     
-- ActivityMonitor2012EX                                                               
-- ActivityMonitorBlocking2012EX                                                       
-- ActivityMonitorDeadlocks2012EX                                                      
-- ActivityMonitorRestart2012                                                          
-- ActivityMonitorStopEX                                                               
-- QueryMonitor2012EX                                                                  
-- QueryMonitorBatch2012                                                               
-- QueryMonitorExtendedEventsBatches2012                                               
-- QueryMonitorExtendedEventsSingleStmt2012                                               
-- QueryMonitorRestart2012EX                                                           
-- QueryMonitorStopEX                                                                  
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER ANY EVENT SESSION'
INSERT INTO @PermissionsTable VALUES(
    'ALTERANYEVENTSESSION',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER ANY LINKED SERVER
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DistributorQueue                                                                    
-- ReplicationCheck                                                  for Linked Servers
-- ReplicationCheck2005                                              for Linked Servers
-- ReplicationDistributorDetails2005                                                   
-- ReplicationDistributorDetails2005                                 for Linked Servers
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER ANY LINKED SERVER'
INSERT INTO @PermissionsTable VALUES(
    'ALTERANYLINKEDSERVER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER ANY LOGIN
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DistributorQueue                                                                    
-- ReplicationCheck                                                  for Linked Servers
-- ReplicationCheck2005                                              for Linked Servers
-- ReplicationDistributorDetails2005                                                   
-- ReplicationDistributorDetails2005                                 for Linked Servers
-- ServerConfiguration2014                                                             
-- ServerConfiguration2016                                                             
---------------------------------------------------------------------------------------------------

---------------------------------------------------------------------------------------------------
-- Permission: ALTER ANY USER
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- FullTextCatalogs2005                                                                
-- ServerConfiguration2014                                                             
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'ALTER ANY USER'
INSERT INTO @PermissionsTable VALUES(
    'ALTERANYUSER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER SERVER STATE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DropPlansFromCache2008                                                              
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER SERVER STATE'
INSERT INTO @PermissionsTable VALUES(
    'ALTERSERVERSTATE',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER SETTINGS
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ServerBasics2005                                                                    
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER SETTINGS'
INSERT INTO @PermissionsTable VALUES(
    'ALTERSETTINGS',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: ALTER TRACE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ActiveWaits2012                                                                     
-- ActivityMonitor2005                                                                 
-- ActivityMonitorBlocking2005                                                         
-- ActivityMonitorRead2005                                                             
-- ActivityMonitorRestart2005                                                          
-- ActivityMonitorStop2005                                                             
-- ActivityMonitorTraceDeadlocks                                                       
-- ActivityProfilerStopTrace2005                                                       
-- QueryMonitor2005                                                                    
-- QueryMonitorCheckSettings2005                                                       
-- QueryMonitorRead2005                                                                
-- QueryMonitorRestart2005                                                             
-- QueryMonitorStop2005                                                                
-- QueryMonitorStopTrace2005                                                           
-- QueryMonitorTraceBatches,                                                           
-- SessionDetailsTrace2005                                                             
-- StopSessionDetailsTrace2005                                                         
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'ALTER TRACE'
INSERT INTO @PermissionsTable VALUES(
    'ALTERTRACE',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: CONTROL SERVER
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Configuration2012                                                    may be disabled
-- ServerOverview2012                                                                  
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,NULL) where permission_name = 'CONTROL SERVER'
INSERT INTO @PermissionsTable VALUES(
    'CONTROLSERVER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: CREATE DATABASE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AlwaysOnTopologyDetail2012                                                          
-- DatabaseSize2012                                                                    
-- DatabaseSummary2012                                                                 
-- DiskSize2005                                                                        
-- FileActivity2005                                                                    
-- FileGroup2005                                                                       
-- FullTextCatalogs2005                                                                
-- LockDetails2005                                                                     
-- Reindex2005                                                                         
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
-- ReplicationSubscriberDetails2005                                                    
-- ServerDetails2012                                                                   
-- ServerOverview2012                                                                  
-- Sessions2005                                                                        
-- TableSummary2005                                                                    
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master','DATABASE') where permission_name = 'CREATE DATABASE'
INSERT INTO @PermissionsTable VALUES(
    'CREATEDATABASE',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: db_ddladmin Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- IndexStatistics                                                                     
-- LargeTableStats2008                                                                 
-- Reindex2005                                                                         
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_ROLEMEMBER('db_ddladmin')
INSERT INTO @PermissionsTable VALUES(
    'DBDDLADMINAccess',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: db_owner Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- IndexStatistics                                                                     
-- LargeTableStats2008                                                                 
-- Reindex2005                                                                         
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_ROLEMEMBER('db_owner')
INSERT INTO @PermissionsTable VALUES(
    'DBOWNERAccess',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: dbm_monitor Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- MirroredDatabaseScheduled2005                                             best quess
-- MirrorMonitoringHistory2005                                                         
-- MirrorMonitoringRealtime2005                                                        
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_ROLEMEMBER('dbm_monitor')
INSERT INTO @PermissionsTable VALUES(
    'DBMMONITORAccess',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTE SERVER
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- LogList2005                                                        May be restricted
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
-- ReplicationSubscriberDetails2005                                                    
-- ServerBasics2005                                                                    
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master','DATABASE') where permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTESERVER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERXPSQLAGENTENUMJOBS
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- LongJobsWithSteps2005                                                               
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('xp_sqlagent_enum_jobs', 'OBJECT') WHERE permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERXPSQLAGENTENUMJOBS',
	@PermissionValue
)

-- AlwaysOnTopology2012
-- AlwaysOnTopology2012
-- SessionDetailsTrace2005
-- StopSessionDetailsTrace2005
-- Lock Details 2005
-- LockDetails2005
-- SQLModuleOptions2005
-- SQLModuleOptions2005
-- DependentObjectsPrescriptiveRecommendation
-- DependentObjectsPrescriptiveRecommendation
-- DependentObjectsPrescriptiveRecommendation
-- DependentObjectsPrescriptiveRecommendation
-- DependentObjectsPrescriptiveRecommendation
-- FilteredIndex2008
-- NonIncrementalColumnStatOnPartitionedTable2014
-- Reindex2005
-- DependentObjectsPrescriptiveRecommendation
-- OverlappingIndexes2005
-- Permissions
-- FullTextCatalogs2005
-- TableSummary2005
-- HashIndex2014
-- FilteredIndex2008
-- OverlappingIndexes2005
-- Permissions
-- Reindex2005
-- DependentObjectsPrescriptiveRecommendation
-- DisabledIndexes2005
-- FilteredIndex2008
-- FragmentationWorkload2005
-- FragmentedIndexes2005
-- HighIndexUpdates2005
-- IndexContention2005
-- NonIncrementalColumnStatOnPartitionedTable2014
-- OverlappingIndexes2005
-- Permissions
-- RarelyUsedIndexOnInMemoryTable2014
-- Reindex2005
-- TableGrowth2005
-- TableSummary2005
-- DatabaseSize2012
-- DatabaseSummary2012
-- GetWorstFillFactorIndexes2005
-- TableGrowth2005
-- TableSummary2005
-- DependentObjectsPrescriptiveRecommendation
-- FragmentationWorkload2005
-- GetWorstFillFactorIndexes2005
-- IndexContention2005
-- Lock Details 2005
-- OutOfDateStats2005
-- OverlappingIndexes2005
-- Permissions
-- SQLModuleOptions2005
-- TableGrowth2005
-- TableSummary2005
-- DependentObjectsPrescriptiveRecommendation
-- QueryStore2016
-- LargeTableStats2008
-- NonIncrementalColumnStatOnPartitionedTable2014
-- NonIncrementalColumnStatOnPartitionedTable2014
-- DBSecurity2005
-- DependentObjectsPrescriptiveRecommendation
-- GetWorstFillFactorIndexes2005
-- DependentObjectsPrescriptiveRecommendation
-- HighIndexUpdates2005
-- DependentObjectsPrescriptiveRecommendation
-- Reindex2005
-- Reindex2005
-- DependentObjectsPrescriptiveRecommendation
-- FullTextColumns
-- Table Details
-- DatabaseSummary2012
-- FileGroup2005
-- DatabaseFiles2005
-- DatabaseSize2012
-- DatabaseSummary2012
-- FileGroup2005
-- DatabaseFiles2005
-- FullTextTables
-- FullTextTables
-- OutOfDateStats2005
-- Table Details
-- DatabaseSummary2012
-- FullTextCatalogs2005
-- FullTextTables
-- Reindex2005
-- ReplicationCheck
-- ReplicationCheck2005
-- Table Details
-- UpdateStatistics
-- IndexStatistics
-- Table Details
-- FullTextColumns
-- DBSecurity2005
-- SessionDetailsTrace2005
-- StopSessionDetailsTrace2005
-- FragmentationWorkload2005

---------------------------------------------------------------------------------------------------
-- Permission: MSDB Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobHistory                                                                     
-- AgentJobSummary                                                                     
-- BackupAndRecovery2005                                                               
-- BackupHistorySmall                                                                  
-- BombedJobs                                                                          
-- CompletedJobs                                                                       
-- DatabaseLastBackupDateTime                                                          
-- DatabaseSize2012                                                                    
-- DatabaseSummary2012                                                                 
-- FailedJobs2005                                                                      
-- LongJobs2005                                                                        
-- LongJobsByPercent                                                                   
-- LongJobsByRuntime                                                                   
-- LongJobsWithSteps2005                                                               
-- RestoreHistoryFull                                                                  
-- RestoreHistorySmall                                                                 
-- ServerConfiguration2014                                                             
-- ServerConfiguration2016                                                             
---------------------------------------------------------------------------------------------------

USE msdb
SELECT @PermissionValue = COUNT(*)
FROM sys.database_role_members AS dRo
JOIN sys.database_principals AS dPrinc
    ON dRo.member_principal_id = dPrinc.principal_id
JOIN sys.database_principals AS dRole
    ON dRo.role_principal_id = dRole.principal_id
WHERE dRole.name = 'db_datareader' AND dPrinc.name = USER_NAME(USER_ID())
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccess',
	@PermissionValue
)

USE master

-- msdb..sysjobhistory
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..sysjobhistory','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessSYSJOBHISTORY',
	@PermissionValue
)
-- msdb..restorehistory
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..restorehistory','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBRESTOREHISTORY ',
	@PermissionValue
)
-- msdb..sysjobs
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..sysjobs','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessSYSJOBS',
	@PermissionValue
)
-- msdb..sp_get_composite_job_info
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..sp_get_composite_job_info','OBJECT') where permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessSPGETCOMPOSITEJOBINFO',
	@PermissionValue
)
-- msdb..backupfile
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..backupfile','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessBACKUPFILE',
	@PermissionValue
)
-- msdb..backupmediafamily
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..backupmediafamily','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessBACKUPMEDIAFAMILY',
	@PermissionValue
)
-- msdb..backupset
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..backupset','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessBACKUPSET',
	@PermissionValue
)
-- msdb..syscategories
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..syscategories','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessSYSCATEGORIES',
	@PermissionValue
)
-- msdb..sysjobsteps
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('msdb..sysjobsteps','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'MSDBAccessSYSJOBSTEPS',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: TEMPDB Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- FailedVariables                                                                     
-- QueryMonitorWrite2012EX                                                             
---------------------------------------------------------------------------------------------------
USE tempdb
SELECT @PermissionValue = COUNT(*)
FROM sys.database_role_members AS dRo
JOIN sys.database_principals AS dPrinc
    ON dRo.member_principal_id = dPrinc.principal_id
JOIN sys.database_principals AS dRole
    ON dRo.role_principal_id = dRole.principal_id
WHERE dRole.name = 'db_datareader' AND dPrinc.name = USER_NAME(USER_ID())
INSERT INTO @PermissionsTable VALUES(
    'TEMPDBAccess',
	@PermissionValue
)

USE master


---------------------------------------------------------------------------------------------------
-- Permission: VIEW DEFINITION
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Configuration2012                                                                   
-- ServerBasics2005                                                                    
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'VIEW DEFINITION'
INSERT INTO @PermissionsTable VALUES(
    'VIEWDEFINITION',
	@PermissionValue
)

-- View Definition required for IS_SRVROLEMEMBER
IF @PermissionValue > 0
BEGIN

---------------------------------------------------------------------------------------------------
-- Permission: SETUPADMIN Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DistributorQueue                                                                    
-- ReplicationDistributorDetails2005                                                   
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_SRVROLEMEMBER('setupadmin')
INSERT INTO @PermissionsTable VALUES(
    'SETUPADMINMember',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: PUBLIC Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- LONGJOBSWITHSTEPS2005                                                                   
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_SRVROLEMEMBER('public')
INSERT INTO @PermissionsTable VALUES(
    'PUBLICMEMBER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission:  Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- LOGSCAN 2005                                                                   
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_SRVROLEMEMBER('securityadmin')
INSERT INTO @PermissionsTable VALUES(
    'SECURITYADMINMEMBER',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: SYSADMIN Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobSummary                                                                     
-- CustomCounterOS                                                                     
-- DBSecurity2005                                                                      
-- DiskSize2005                                                                        
-- DistributorQueue                                                                    
-- DTCStatus                                                                           
-- FileActivity2005                                                                    
-- FullTextSearchService2005                                                           
-- HashIndex2014                                                                       
-- IndexStatistics                                                                     
-- LargeTableStats2008                                                                 
-- Log Scan 2005                                                                       
-- LogList2005                                                                         
-- MirroredDatabaseScheduled2005                                             best quess
-- MirroredDatabaseScheduled2005                                    for mirror database
-- MirrorMetricsUpdate2005                                                             
-- MirrorMonitoringHistory2005                                                         
-- MirrorMonitoringRealtime2005                                                        
-- OS Metrics                                                                          
-- QueryMonitorBatch2012                                                               
-- QueryMonitorRead2012EX                                                              
-- Reindex2005                                                                         
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
-- ReplicationDistributorDetails2005                                                   
-- ReplicationStatus                                                                   
-- ServerBasics2005                                                                    
-- ServerConfiguration2014                                                             
-- ServerConfiguration2016                                                             
-- ServerOverview2012                                                                  
-- Services2005                                                                        
-- SessionDetails2005                                                                  
-- Sessions2005                                                                        
-- WmiConfigurationTest                                                                
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_SRVROLEMEMBER('sysadmin')
INSERT INTO @PermissionsTable VALUES(
    'SYSADMINMember',
	@PermissionValue
)

SELECT @PermissionValue = IS_SRVROLEMEMBER('serveradmin')
INSERT INTO @PermissionsTable VALUES(
    'SERVERADMIN',
	@PermissionValue
)

END

---------------------------------------------------------------------------------------------------
-- Permission: WINDOWS Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- QueryMonitorBatch2012                                                               
-- QueryMonitorRead2012EX                                                              
-- ServerConfiguration2016                                                             
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = IS_MEMBER(SYSTEM_USER)
INSERT INTO @PermissionsTable VALUES(
    'WINDOWSMember',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: SELECTSYSDMSERVERSERVICES Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobSummary                                                             
---------------------------------------------------------------------------------------------------
-- sys.dm_server_services
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.dm_server_services','OBJECT') where permission_name = 'SELECT'
INSERT INTO @PermissionsTable VALUES(
    'SELECTSYSDMSERVERSERVICES',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERXPSQLAGENTENUMJOBS Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobSummary                                                             
---------------------------------------------------------------------------------------------------
-- master..xp_sqlagent_enum_jobs
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_sqlagent_enum_jobs','OBJECT') where permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERXPSQLAGENTENUMJOBS',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERDBOXPREGREAD Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ServerDetails2012                                                             
---------------------------------------------------------------------------------------------------
-- master..xp_regread
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_regread','OBJECT') where permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERDBOXPREGREAD',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERDBOXPINSTANCEREGREAD Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ServerOverview2012                                                             
---------------------------------------------------------------------------------------------------
-- master..xp_instance_regread
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_instance_regread','OBJECT') where permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERDBOXPINSTANCEREGREAD',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERXPSERVICECONTROL Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DTC Status                                                             
---------------------------------------------------------------------------------------------------
-- master..xp_servicecontrol
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_servicecontrol', 'OBJECT') WHERE permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERXPSERVICECONTROL',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERXPLOGINCONFIG Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Server Overview                                                       
---------------------------------------------------------------------------------------------------
-- master..xp_loginconfig
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_loginconfig', 'OBJECT') WHERE permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERXPLOGINCONFIG',
	@PermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: Msdb SQLAgentOperatorRole
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobSummary                                                                     
---------------------------------------------------------------------------------------------------

USE msdb
SELECT @PermissionValue = COUNT(*)
FROM sys.database_role_members AS dRo
JOIN sys.database_principals AS dPrinc
    ON dRo.member_principal_id = dPrinc.principal_id
JOIN sys.database_principals AS dRole
    ON dRo.role_principal_id = dRole.principal_id
WHERE dRole.name = 'SQLAgentOperatorRole' AND dPrinc.name = USER_NAME(USER_ID())
INSERT INTO @PermissionsTable VALUES(
    'MsdbSQLAgentOperatorRole',
	@PermissionValue
)

USE master

---------------------------------------------------------------------------------------------------
-- Permission: Msdb SQLAgentReaderRole
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AgentJobSummary                                                                     
---------------------------------------------------------------------------------------------------

USE msdb
SELECT @PermissionValue = COUNT(*)
FROM sys.database_role_members AS dRo
JOIN sys.database_principals AS dPrinc
    ON dRo.member_principal_id = dPrinc.principal_id
JOIN sys.database_principals AS dRole
    ON dRo.role_principal_id = dRole.principal_id
WHERE dRole.name = 'SQLAgentReaderRole' AND dPrinc.name = USER_NAME(USER_ID())
INSERT INTO @PermissionsTable VALUES(
    'MsdbSQLAgentReaderRole',
	@PermissionValue
)

USE master

---------------------------------------------------------------------------------------------------
-- Permission: msdb db_owner Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Server Details                                                               
---------------------------------------------------------------------------------------------------
USE [msdb]
SELECT @PermissionValue = IS_ROLEMEMBER('db_owner')
INSERT INTO @PermissionsTable VALUES(
    'MsdbDbOwner',
	@PermissionValue
)

USE master

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTEMASTERXPREADERRORLOG
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- LogList2005                                                               
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = COUNT(*) FROM sys.fn_my_permissions('master..xp_readerrorlog', 'OBJECT') WHERE permission_name = 'EXECUTE'
INSERT INTO @PermissionsTable VALUES(
    'EXECUTEMASTERXPREADERRORLOG',
	@PermissionValue
)

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @PermissionsTable

END TRY

BEGIN CATCH
    declare @ErrorMessage nvarchar(max), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH
