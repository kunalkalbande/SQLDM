
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
-- Permission: ADMIN Member
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
---------------------------------------------------------------------------------------------------
SELECT @PermissionValue = HAS_PERMS_BY_NAME('master', 'DATABASE', 'VIEW DATABASE STATE')
INSERT INTO @PermissionsTable VALUES(
    'ViewDatabaseStateMaster',
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

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @PermissionsTable

END TRY

BEGIN CATCH
    declare @ErrorMessage varchar(8000), @ErrorSeverity int, @ErrorState int;
    select @ErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as varchar(5)), @ErrorSeverity = ERROR_SEVERITY(), @ErrorState = ERROR_STATE();
    raiserror (@ErrorMessage, @ErrorSeverity, @ErrorState); -- raising the error and logging it as warning 
END CATCH
