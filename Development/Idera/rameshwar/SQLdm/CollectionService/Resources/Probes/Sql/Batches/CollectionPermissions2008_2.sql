
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
-- Permission: CONTROL SERVER - Used for Azure
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- Configuration2012                                                    may be disabled
-- ServerOverview2012                                                                  
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = HAS_PERMS_BY_NAME(DB_NAME(), 'DATABASE', 'CONTROL')
INSERT INTO @PermissionsTable VALUES(
    'ControlDb',
	@PermissionValue
)

-- ---------------------------------------------------------------------------------------------------
-- -- Permission: Reader Permissions db_datareader Used for Azure
-- ---------------------------------------------------------------------------------------------------
-- -- SQL Object Name                                                          Description                                                                 
-- ---------------------------------------------------------------------------------------------------
-- SELECT @PermissionValue = IS_ROLEMEMBER ( 'db_datawriter', USER_NAME()  ) | IS_ROLEMEMBER ( 'db_owner', USER_NAME()  ) 
-- INSERT INTO @PermissionsTable VALUES(
    -- 'DbDataWriterAccess',
	-- @PermissionValue
-- )

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @PermissionsTable

END TRY

BEGIN CATCH
    declare @ErrorMessage varchar(8000), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as varchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH
