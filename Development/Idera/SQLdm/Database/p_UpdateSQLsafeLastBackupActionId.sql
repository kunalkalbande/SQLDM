
IF (object_id('p_UpdateSQLsafeLastBackupActionId') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateSQLsafeLastBackupActionId
END
GO

CREATE PROCEDURE [dbo].[p_UpdateSQLsafeLastBackupActionId]
(
	@Id	INT,
	@ActionId INT
)
AS
BEGIN
	if exists (select [LastBackupActionId] from MonitoredServerSQLsafeInstance where SQLServerID = @Id)
	BEGIN
		update MonitoredServerSQLsafeInstance set LastBackupActionId = @ActionId where SQLServerID = @Id
	END
END