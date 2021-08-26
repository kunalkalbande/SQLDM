
IF (object_id('p_AddSQLsafeReferenceLink') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddSQLsafeReferenceLink
END
GO

CREATE PROCEDURE [dbo].[p_AddSQLsafeReferenceLink]
(
	@Id	INT,
	@RepositoryId INT,
	@SQLSafeInstanceId INT,
	@BackupActionId INT,
	@DefragActionId INT
)
AS
BEGIN
	IF NOT EXISTS(Select LastBackupActionId from MonitoredServerSQLsafeInstance where SQLServerID = @Id)
	BEGIN
		INSERT INTO MonitoredServerSQLsafeInstance (SQLServerID, RepositoryId, RelatedInstanceId, LastBackupActionId, LastDefragActionId)
			VALUES(@Id, @RepositoryId, @SQLSafeInstanceId, @BackupActionId, @DefragActionId)
	END
	ELSE
	BEGIN
		UPDATE MonitoredServerSQLsafeInstance 
			set RepositoryId = @RepositoryId, RelatedInstanceId = @SQLSafeInstanceId, 
				LastBackupActionId = @BackupActionId, LastDefragActionId = @DefragActionId
			where SQLServerID = @Id
	End
END