--SQLdm 10.1 (Barkha Khatri) SysAdmin feature-updating sql server permissions at launch time
IF (object_id('p_UpdateSysAdminPermissions') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateSysAdminPermissions
END
GO

CREATE PROCEDURE [dbo].[p_UpdateSysAdminPermissions](
	@data xml
)
AS
begin
IF OBJECT_ID('Permissiondb..#SysAdminPermissions') IS NOT NULL
  DROP TABLE #SysAdminPermissions

CREATE TABLE #SysAdminPermissions(Id int,IsUserSysAdmin bit)

INSERT INTO #SysAdminPermissions (Id, IsUserSysAdmin)
SELECT
Server.value('(ID/text())[1]','int') as Id,
Server.value('(IsUserSysAdmin/text())[1]','bit') as IsUserSysAdin
FROM @data.nodes('/Servers/Server') Servers(Server)

UPDATE dbo.MonitoredSQLServers    
  SET IsUserSysAdmin = Permission.IsUserSysAdmin   
FROM dbo.MonitoredSQLServers
  INNER JOIN #SysAdminPermissions Permission
    ON dbo.MonitoredSQLServers.SQLServerID=Permission.Id


END	



