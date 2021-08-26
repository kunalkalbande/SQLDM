---------------------------------------------------------------------------------------------------
--  SQLdm 10.3 (Varun Chopra)-- Created batch to get Replication Permissions Information for Current User
--  Batch: Replication Check Permissions
--  Description: Collects Replication Check Permissions for the user executing the batch
---------------------------------------------------------------------------------------------------

BEGIN TRY

-- Table: Permissions Table to store the permissions
-- Permission Name : Name of the permissions
-- Permission Value : Value of the permissions
DECLARE @ReplicationPermissionsTable TABLE
(
	PermissionName nvarchar(256),
	PermissionValue int
)

-- Permission Value : To store value of the permission
DECLARE @ReplicationPermissionValue INT
-- Table to store the temporary replication Permissions for Publishers / Subscribers / Distributors
DECLARE @TempReplicationPermissions TABLE
(
  ReplPermissions INT
)

--Category	Role		Replication Description
--1			Publisher	Published for snapshot or transactional replication.	
--4			Publisher	Published for merge replication.
--DECLARE @PublisherPermissionsCommand nvarchar(2000);
--SET @PublisherPermissionsCommand = 'USE [?] 
--DECLARE @c INT, @p INT; SET @p=0;
--SELECT @c=category from master..sysdatabases d(nolock) WHERE has_dbaccess(name)=1 AND mode=0 AND isnull(databasepropertyex(name,''IsInLoad''),0)=0 
--	AND isnull(databasepropertyex(name,''IsSuspect''),0)=0 AND isnull(databasepropertyex(name,''IsInRecovery''),0)=0 AND 
--	isnull(databasepropertyex(name,''IsNotRecovered''),0)=0 AND isnull(databasepropertyex(name,''IsOffline''),0)=0 AND 
--	isnull(databasepropertyex(name,''IsShutDown''),0)=0 AND 
--	 (isnull(databasepropertyex(name,''IsSingleUser''),0)=0 OR 
--	 (isnull(databasepropertyex(name,''IsSingleUser''),0)=1 AND not exists (select * from master..sysprocesses p WHERE dbid=d.dbid AND p.spid<>@@spid)
--	AND not exists (select * from master..syslockinfo l WHERE rsc_dbid=d.dbid AND l.req_spid<>@@spid))) 
--	AND status&32<>32 AND status&64<>64AND status&128<>128 AND status&256<>256 AND status&512<>512 
--	AND name=''?'' 
--	AND (category=1 OR category=4);
--IF ISNULL(@c,0)=0
--	RETURN
--IF IS_MEMBER(''db_owner'')>0 AND 
--	((@c=1 AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''syspublications'',''OBJECT'') WHERE permission_name=''SELECT'') AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''syssubscriptions'',''OBJECT'') WHERE permission_name=''SELECT'') AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''sysarticles'',''OBJECT'') WHERE permission_name=''SELECT'') AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''master..sysservers'',''OBJECT'') WHERE permission_name=''SELECT''))
--	OR 
--	(@c = 4 AND ((select count(*) from master..sysobjects WHERE name IN (''sysmergepublications'',''sysmergesubscriptions'',''sysmergearticles''))<>3 OR (EXISTS(SELECT 1 FROM sys.fn_my_permissions(''sysmergepublications'',''OBJECT'') WHERE permission_name=''SELECT'') AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''sysmergesubscriptions'',''OBJECT'') WHERE permission_name=''SELECT'') AND EXISTS(SELECT 1 FROM sys.fn_my_permissions(''sysmergearticles'',''OBJECT'') WHERE permission_name=''SELECT'')))))
--BEGIN
--	SET @p = 1
--END
--SELECT @p'

INSERT INTO @TempReplicationPermissions
VALUES (1)
--EXEC sp_MSforeachdb @PublisherPermissionsCommand

--Category	Role		Replication Description
--2			Subscriber	Subscribed to a snapshot or transactional publication.
--8			Subscriber	Subscribed to a merge publication.
-- No dynamic conditions required -  Requires Control on Master Permission

--Category	Role		Replication Description
--16		Distributor	Distribution database.
--DECLARE @DistributorPermissionsCommand nvarchar(2000);
--SET @DistributorPermissionsCommand = 'USE [?] 
--DECLARE @c INT, @p INT; SET @p=0;
--SELECT @c=category from master..sysdatabases d(nolock) WHERE has_dbaccess(name)=1 AND mode=0 AND isnull(databasepropertyex(name,''IsInLoad''),0)=0 
--	AND isnull(databasepropertyex(name,''IsSuspect''),0)=0 AND isnull(databasepropertyex(name,''IsInRecovery''),0)=0 AND 
--	isnull(databasepropertyex(name,''IsNotRecovered''),0)=0 AND isnull(databasepropertyex(name,''IsOffline''),0)=0 AND 
--	isnull(databasepropertyex(name,''IsShutDown''),0)=0 AND 
--	 (isnull(databasepropertyex(name,''IsSingleUser''),0)=0 OR 
--	 (isnull(databasepropertyex(name,''IsSingleUser''),0)=1 AND not exists (select * from master..sysprocesses p WHERE dbid=d.dbid AND p.spid<>@@spid)
--	AND not exists (select * from master..syslockinfo l WHERE rsc_dbid=d.dbid AND l.req_spid<>@@spid))) 
--	AND status&32<>32 AND status&64<>64AND status&128<>128 AND status&256<>256 AND status&512<>512 
--	AND name=''?'' 
--	AND (category=16);
--IF ISNULL(@c,0)=0
--	RETURN
--IF @c = 16 AND ((select count(*) FROM master..sysobjects WHERE name IN (''MSrepl_transactions'',''MSsubscriptions'',''MSdistribution_history'',''MSrepl_commands''))<>4 
--  OR (EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSdistribution_history'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSdistribution_status'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSmerge_subscriptions'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSpublications'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSrepl_commands'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSrepl_transactions'',''OBJECT'') WHERE permission_name = ''SELECT'') AND 
--  EXISTS(SELECT 1 FROM sys.fn_my_permissions(''MSsubscriptions'',''OBJECT'') WHERE permission_name = ''SELECT'')))
--BEGIN
--	SET @p = 1
--END
--SELECT @p'

INSERT INTO @TempReplicationPermissions
VALUES (1)
--EXEC sp_MSforeachdb @DistributorPermissionsCommand

-- Select Replication Permissions when there is any case where publisher/subscriber/distributor doesn't have required permissions
SELECT @ReplicationPermissionValue = CASE WHEN COUNT(*) > 0 THEN 0 ELSE 1 END
FROM @TempReplicationPermissions WHERE ReplPermissions = 0

INSERT INTO @ReplicationPermissionsTable VALUES(
    'ReplicationCheck',
	@ReplicationPermissionValue
)

-- SELECT Values FROM @PermissionsTable
SELECT PermissionName, PermissionValue FROM @ReplicationPermissionsTable

END TRY

BEGIN CATCH
    declare @ReplicationErrorMessage nvarchar(max), @ReplicationErrorSeverity int, @ReplicationErrorState int;
    select @ReplicationErrorMessage = ERROR_MESSAGE() + ' Line ' + cast(ERROR_LINE() as nvarchar(5)), @ReplicationErrorSeverity = ERROR_SEVERITY(), @ReplicationErrorState = ERROR_STATE();
    raiserror (@ReplicationErrorMessage, @ReplicationErrorSeverity, @ReplicationErrorState); -- raising the error and logging it as warning 
END CATCH
