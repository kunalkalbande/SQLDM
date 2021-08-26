
---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Minimum Permissions Information for Current User
--  Batch: Get MinimumPermissions Batch (Permissions 2008 and above)
--  Description: Collects all the Minimum Permissions related information for the user executing the batch
---------------------------------------------------------------------------------------------------

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @MinimumPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @MinimumPermissionValue INT

---------------------------------------------------------------------------------------------------
-- Permission: Reader Permissions db_datareader
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description                                                                 
---------------------------------------------------------------------------------------------------
SELECT @MinimumPermissionValue = IS_ROLEMEMBER ( 'db_datareader', USER_NAME()  ) | IS_ROLEMEMBER ( 'db_datawriter', USER_NAME()  ) | IS_ROLEMEMBER ( 'db_owner', USER_NAME()  )
INSERT INTO @MinimumPermissionsTable VALUES(
    'DbDataReaderAccess',
	@MinimumPermissionValue
)
---------------------------------------------------------------------------------------------------
-- Permission: VIEW DEFINITION
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
---------------------------------------------------------------------------------------------------
SELECT @MinimumPermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'VIEW DEFINITION'
INSERT INTO @MinimumPermissionsTable VALUES(
    'VIEWDEFINITION',
	@MinimumPermissionValue
)
---------------------------------------------------------------------------------------------------
-- Permission: VIEW DATABASE STATE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- ActiveWaits2012                                                on Azure SQL Database
-- ActivityMonitor2012EX                                          on Azure SQL Database
-- ActivityMonitorRestart2012                                                          
-- BlockingCheck2005                                                                   
-- BufferPoolExtIO2014                                                                 
-- ColumnStoreIndex2016                                                                
-- DatabaseFiles2005                                                                   
-- DatabaseSummary2012                                                                 
-- DropPlansFromCache2008                                                              
-- FileGroup2005                                                     Azure SQL Database
-- FragmentationWorkload2005                                         Azure SQL Database
-- FragmentedIndexes2005                                             Azure SQL Database
-- FullTextCatalogs2005                                              Azure SQL Database
-- GetAdhocCachedPlanBytes2005                                       Azure SQL Database
-- GetLockedPageKB2005                                               Azure SQL Database
-- GetLockedPageKB2008                                               Azure SQL Database
-- HashIndex2014                                                     Azure SQL Database
-- HighCPUTimeProcedure2016                                          Azure SQL Database
-- HighIndexUpdates2005                                              Azure SQL Database
-- IndexContention2005                                                                 
-- Lock Details 2005                                         Azure SQL Database Premium
-- LockDetails2005                                           Azure SQL Database Premium
-- Memory2012                                                Azure SQL Database Premium
-- MirroredDatabaseScheduled2005                             Azure SQL Database Premium
-- NUMANodeCounters2005                                      SQL Database Premium Tiers
-- OldestOpenTransaction2005                                 SQL Database Premium Tiers
-- OverlappingIndexes2005                                    SQL Database Premium Tiers
-- ProcedureCacheList2005                                    SQL Database Premium Tiers
-- QueryAnalyzer2016                                                                   
-- QueryMonitorBatch2012                                     SQL Database Premium Tiers
-- QueryMonitorRestart2012EX                                 SQL Database Premium Tiers
-- QueryStore2016                                                                      
-- RarelyUsedIndexOnInMemoryTable2014                                                  
-- Reindex2005                                                                         
-- Reorganization2005                                                                  
-- SampleServerResources2005                                 Azure SQL Database Premium
-- ServerDetails2012                                                                   
-- ServerOverview2012                                                                  
-- SessionDetails2005                                                                  
-- Sessions2005                                                                        
-- SessionSummary2005                                                                  
-- TableGrowth2005                                                                     
-- WaitingBatches2005                                                                  
-- WaitStatistics                                                                      
---------------------------------------------------------------------------------------------------
SELECT @MinimumPermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'VIEW DATABASE STATE'
INSERT INTO @MinimumPermissionsTable VALUES(
    'VIEWDATABASESTATE',
	@MinimumPermissionValue
)
---------------------------------------------------------------------------------------------------
-- Permission: SELECT
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description                                           
---------------------------------------------------------------------------------------------------
SELECT @MinimumPermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'SELECT'
INSERT INTO @MinimumPermissionsTable VALUES(
    'SelectAccess',
	@MinimumPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTE
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description                                           
---------------------------------------------------------------------------------------------------
SELECT @MinimumPermissionValue = COUNT(*) FROM sys.fn_my_permissions(NULL,'DATABASE') where permission_name = 'EXECUTE'
INSERT INTO @MinimumPermissionsTable VALUES(
    'ExecuteAccess',
	@MinimumPermissionValue
)
-- SELECT Values FROM @MinimumPermissionsTable
SELECT PermissionName, PermissionValue FROM @MinimumPermissionsTable

END TRY

BEGIN CATCH
    declare @MinimumErrorMessage nvarchar(max), @MinimumErrorSeverity int, @MinimumErrorState int;
    select @MinimumErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @MinimumErrorSeverity = ERROR_SEVERITY(), @MinimumErrorState = ERROR_STATE();
    raiserror (@MinimumErrorMessage, @MinimumErrorSeverity, @MinimumErrorState); -- raising the error and logging it as warning 
END CATCH
