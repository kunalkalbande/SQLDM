
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

-- Metadata Visibility sys.plan_guides
SELECT @MetadataVisibilityPermissionValue = COUNT(*) FROM sys.fn_my_permissions('sys.plan_guides','OBJECT') where permission_name = 'SELECT'
INSERT INTO @MetadataVisibilityPermissionsTable VALUES(
    'MetadataVisibilitymastersysplanguides',
	@MetadataVisibilityPermissionValue
)

--USE [master]

-- SELECT Values FROM @MetadataVisibilityPermissionsTable
SELECT PermissionName, PermissionValue FROM @MetadataVisibilityPermissionsTable

END TRY

BEGIN CATCH
    declare @MetadataVisibilityErrorMessage nvarchar(max), @MetadataVisibilityErrorSeverity int, @MetadataVisibilityErrorState int;
    select @MetadataVisibilityErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @MetadataVisibilityErrorSeverity = ERROR_SEVERITY(), @MetadataVisibilityErrorState = ERROR_STATE();
    raiserror (@MetadataVisibilityErrorMessage, @MetadataVisibilityErrorSeverity, @MetadataVisibilityErrorState); -- raising the error and logging it as warning 
END CATCH
