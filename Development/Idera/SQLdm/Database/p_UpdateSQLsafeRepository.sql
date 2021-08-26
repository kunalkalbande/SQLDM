
IF (object_id('p_UpdateSQLsafeRepository') IS NOT NULL)
BEGIN
DROP PROCEDURE p_UpdateSQLsafeRepository
END
GO

CREATE PROCEDURE [dbo].[p_UpdateSQLsafeRepository]
(
	@Id INT,
	@DatabaseName nvarchar(128),
	@SecurityMode bit,
	@UserName nvarchar(128),
	@EncryptedPassword nvarchar(128),
	@FriendlyName nvarchar(256)
)
AS
BEGIN
	IF EXISTS(SELECT RepositoryId from SQLsafeConnections where RepositoryId = @Id)
	BEGIN
		update SQLsafeConnections set DatabaseName = @DatabaseName, FriendlyName = @FriendlyName, SecurityMode = @SecurityMode, UserName = @UserName, EncryptedPassword = @EncryptedPassword
			where RepositoryId = @Id
	END
END