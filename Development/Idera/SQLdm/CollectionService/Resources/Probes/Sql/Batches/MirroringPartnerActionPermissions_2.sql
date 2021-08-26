
---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Mirroring Partner Action Permissions Information for Current User
--  Batch: Get Mirroring Permissions Batch (Permissions 2008 and above)
--  Description: Collects the Alter Database Permissions related information for the user executing the batch
---------------------------------------------------------------------------------------------------
--  Batch: MirroringPartnerActionPermissions
--  Variables: 
--		[0] - Target Database
--------------------------------------------------------------------------------

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @MirroringPartnerActionPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @MirroringPartnerActionPermissionValue INT

--USE {0}

---------------------------------------------------------------------------------------------------
-- Permission: EXECUTE SERVER
---------------------------------------------------------------------------------------------------
-- SQL Object Name                                                          Description
-- MirroringPartnerAction												Alter Database Permissions
---------------------------------------------------------------------------------------------------
SELECT @MirroringPartnerActionPermissionValue = COUNT(*) FROM sys.fn_my_permissions('{0}','DATABASE') where permission_name = 'ALTER'
INSERT INTO @MirroringPartnerActionPermissionsTable VALUES(
    'MIRRORINGPARTNERACTION',
	@MirroringPartnerActionPermissionValue
)


-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @MirroringPartnerActionPermissionsTable

--USE MASTER

END TRY

BEGIN CATCH
    declare @MirroringPartnerActionErrorMessage nvarchar(max), @MirroringPartnerActionErrorSeverity int, @MirroringPartnerActionErrorState int;
    select @MirroringPartnerActionErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @MirroringPartnerActionErrorSeverity = ERROR_SEVERITY(), @MirroringPartnerActionErrorState = ERROR_STATE();
    raiserror (@MirroringPartnerActionErrorMessage, @MirroringPartnerActionErrorSeverity, @MirroringPartnerActionErrorState); -- raising the error and logging it as warning 
END CATCH
