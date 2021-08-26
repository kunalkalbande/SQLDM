---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Replication Distribution Permissions Information for Current User
--  Batch: Replication Distribution Permissions
--  Description: Collects Replication Distribution Permissions for the user executing the batch
--  Tables: sys.servers, MSdistribution_status, MSpublications,MSrepl_transactions, 
--          MSrepl_commands, MSsubscriptions, MSdistribution_history
--  Variables: 
--		[0] - Distribution Database name
---------------------------------------------------------------------------------------------------

set nocount on

use [{0}]

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @ReplicationDistributionPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @ReplicationDistributionPermissionValue INT

IF ((select count(*) FROM [{0}]..sysobjects WHERE name IN ('MSrepl_transactions','MSsubscriptions','MSdistribution_history'))<>3 
  OR (EXISTS(SELECT 1 FROM sys.fn_my_permissions('MSdistribution_history','OBJECT') WHERE permission_name = 'SELECT') AND 
  EXISTS(SELECT 1 FROM sys.fn_my_permissions('MSdistribution_status','OBJECT') WHERE permission_name = 'SELECT') AND 
  EXISTS(SELECT 1 FROM sys.fn_my_permissions('MSpublications','OBJECT') WHERE permission_name = 'SELECT') AND 
  EXISTS(SELECT 1 FROM sys.fn_my_permissions('MSrepl_transactions','OBJECT') WHERE permission_name = 'SELECT') AND 
  EXISTS(SELECT 1 FROM sys.fn_my_permissions('MSsubscriptions','OBJECT') WHERE permission_name = 'SELECT') ))
BEGIN
	SET @ReplicationDistributionPermissionValue = 1
END
INSERT INTO @ReplicationDistributionPermissionsTable VALUES(
    'ReplicationDistributionCheck',
	@ReplicationDistributionPermissionValue
)

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @ReplicationDistributionPermissionsTable

END TRY

BEGIN CATCH
    declare @ReplicationDistributionErrorMessage nvarchar(max), @ReplicationDistributionErrorSeverity int, @ReplicationDistributionErrorState int;
    select @ReplicationDistributionErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ReplicationDistributionErrorSeverity = ERROR_SEVERITY(), @ReplicationDistributionErrorState = ERROR_STATE();
    raiserror (@ReplicationDistributionErrorMessage, @ReplicationDistributionErrorSeverity, @ReplicationDistributionErrorState); -- raising the error and logging it as warning 
END CATCH
