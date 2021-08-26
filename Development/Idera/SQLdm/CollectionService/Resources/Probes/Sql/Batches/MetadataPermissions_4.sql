
---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get MetadataVisibility Permissions Information for Current User
--  Batch: Get MetadataVisibilityPermissions Batch (Permissions 2008 and above)
--  Description: Collects all the MetadataVisibility Permissions related information for the user executing the batch
---------------------------------------------------------------------------------------------------

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @MetadataVisibilityPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @MetadataVisibilityPermissionValue INT

---------------------------------------------------------------------------------------------------
-- Permission: Metadata Visibility
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- AlwaysOnTopology2012                                                                
-- DatabaseFiles2005                                                                   
-- DatabaseSize2012                                                                    
-- DBSecurity2005                                                                      
-- DependentObjectsPrescriptiveRecommendation                                               
-- DisabledIndexes2005                                                                 
-- FileGroup2005                                                                       
-- FilteredIndex2008                                                                   
-- FragmentationWorkload2005                                                           
-- FragmentedIndexes2005                                                               
-- FullTextCatalogs2005                                                                
-- FullTextColumns                                                                     
-- FullTextTables                                                                      
-- GetWorstFillFactorIndexes2005                                                       
-- HashIndex2014                                                                       
-- HighIndexUpdates2005                                                                
-- IndexContention2005                                                                 
-- IndexStatistics                                                                     
-- LargeTableStats2008                                                                 
-- Lock Details 2005                                                                   
-- LockDetails2005                                                                     
-- NonIncrementalColumnStatOnPartitionedTable2014                                               
-- OutOfDateStats2005                                                                  
-- OverlappingIndexes2005                                                              
-- QueryStore2016                                                                      
-- RarelyUsedIndexOnInMemoryTable2014                                                  
-- Reindex2005                                                                         
-- ReplicationCheck                                                                    
-- ReplicationCheck2005                                                                
-- SessionDetailsTrace2005                                                             
-- SQLModuleOptions2005                                                                
-- StopSessionDetailsTrace2005                                                         
-- Table Details                                                                       
-- TableGrowth2005                                                                     
-- TableSummary2005                                                                    
-- UpdateStatistics                                                                    
---------------------------------------------------------------------------------------------------
-- Metadata Visibility master.sys.availability_group_listener_ip_addresses
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.availability_group_listener_ip_addresses','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysavailabilitygrouplisteneripaddresses',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.availability_group_listeners
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.availability_group_listeners','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysavailabilitygrouplisteners',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.traces
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.traces','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersystraces',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.all_objects
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.all_objects','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysallobjects',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.all_sql_modules
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.all_sql_modules','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysallsqlmodules',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.assemblies
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.assemblies','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysassemblies',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.assembly_modules
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.assembly_modules','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysassemblymodules',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.assembly_references
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.assembly_references','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysassemblyreferences',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.assembly_types
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.assembly_types','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysassemblytypes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.columns
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.columns','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersyscolumns',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.foreign_keys
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.foreign_keys','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysforeignkeys',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.fulltext_catalogs
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.fulltext_catalogs','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysfulltextcatalogs',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.hash_indexes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.hash_indexes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersyshashindexes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.index_columns
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.index_columns','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysindexcolumns',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.indexes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.indexes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysindexes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.internal_tables
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.internal_tables','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysinternaltables',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.objects
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.objects','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysobjects',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.parameters
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.parameters','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysparameters',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.plan_guides
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.plan_guides','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysplanguides',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.stats
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.stats','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysstats',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.stats_columns
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.stats_columns','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysstatscolumns',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.symmetric_keys
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.symmetric_keys','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersyssymmetrickeys',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.synonyms
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.synonyms','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersyssynonyms',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.sysindexes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.sysindexes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersyssysindexes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.tables
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.tables','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersystables',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.types
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.types','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersystypes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.xml_indexes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.xml_indexes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysxmlindexes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.xml_schema_collections
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.xml_schema_collections','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysxmlschemacollections',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysdepends
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysdepends','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysdepends',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysfilegroups
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysfilegroups','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysfilegroups',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysfiles
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysfiles','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysfiles',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysfulltextcatalogs
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysfulltextcatalogs','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysfulltextcatalogs',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysindexes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysindexes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysindexes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysobjects
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysobjects','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysobjects',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysreferences
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysreferences','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysreferences',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility systypes
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('systypes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysystypes',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility sysusers
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sysusers','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysysusers',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility tempdb..sysobjects
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('tempdb..sysobjects','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitytempdbsysobjects',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility tempdb.sys.columns
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('tempdb.sys.columns','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitytempdbsyscolumns',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility syscolumns
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('syscolumns','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitysyscolumns',
	@MetadataVisibilityPermissionValue
)

-- Metadata Visibility master.sys.dm_db_stats_properties
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('master.sys.dm_db_stats_properties','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysdmdbstatsproperties',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: sys.allocation_units Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DatabaseSize2012															Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.allocation_units','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitySysAllocationUnits',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: sys.partitions Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DatabaseSize2012															Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.partitions','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitySysPartitions',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: tempdb.sys.database_files Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DatabaseSize2012															Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('tempdb.sys.database_files','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilityTempdbSysDatabaseFiles',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: sys.schemas Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- SQLModuleOptions2005														Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.schemas','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitySysSchemas',
	@MetadataVisibilityPermissionValue
)


---------------------------------------------------------------------------------------------------
-- Permission: sys.configurations Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Configurations2012														Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.configurations','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitySysConfigurations',
	@MetadataVisibilityPermissionValue
)


---------------------------------------------------------------------------------------------------
-- Permission: xp_msver Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Configurations2012														Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('xp_msver','OBJECT') where permission_name = 'EXECUTE'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilityXpMsver',
	@MetadataVisibilityPermissionValue
)
---------------------------------------------------------------------------------------------------
-- Permission: sys.partition_functions , sys.partition_schemes ,sys.sql_dependencies
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- DependentObjectsPrescriptiveRecommendation								Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.partition_functions','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataSysPartitionFunction',
	@MetadataVisibilityPermissionValue
)
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.partition_schemes','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataSysPartitionSchemes',
	@MetadataVisibilityPermissionValue
)
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.sql_dependencies','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataSysSqlDependencies',
	@MetadataVisibilityPermissionValue
)
---------------------------------------------------------------------------------------------------
-- Permission: sys.data_spaces
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- FullTextCatalogs2005 , TableSummary2005									Public Role Member
---------------------------------------------------------------------------------------------------
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.data_spaces','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataSysDataSpaces',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: Tempdb db_owner Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Server Details                                                               
---------------------------------------------------------------------------------------------------
USE [tempdb]
SELECT @MetadataVisibilityPermissionValue = IS_ROLEMEMBER('db_owner')
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'TempDbDbOwnerAccess',
	@MetadataVisibilityPermissionValue
)

---------------------------------------------------------------------------------------------------
-- Permission: Tempdb Db Data Writer Access
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Server Details                                                               
---------------------------------------------------------------------------------------------------

SELECT @MetadataVisibilityPermissionValue = COUNT(*)
FROM sys.database_role_members AS dRo
JOIN sys.database_principals AS dPrinc
    ON dRo.member_principal_id = dPrinc.principal_id
JOIN sys.database_principals AS dRole
    ON dRo.role_principal_id = dRole.principal_id
WHERE dRole.name = 'db_datawriter' AND dPrinc.name = USER_NAME(USER_ID())
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'TempDbDataWriter',
	@MetadataVisibilityPermissionValue
)

USE [master]

-- SELECT Values FROM @MetadataVisibilityPermissionsTable
SELECT PermissionName, PermissionValue FROM @MetadataVisibilityPermissionsTable

END TRY

BEGIN CATCH
    declare @MetadataVisibilityErrorMessage nvarchar(max), @MetadataVisibilityErrorSeverity int, @MetadataVisibilityErrorState int;
    select @MetadataVisibilityErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @MetadataVisibilityErrorSeverity = ERROR_SEVERITY(), @MetadataVisibilityErrorState = ERROR_STATE();
    raiserror (@MetadataVisibilityErrorMessage, @MetadataVisibilityErrorSeverity, @MetadataVisibilityErrorState); -- raising the error and logging it as warning 
END CATCH
