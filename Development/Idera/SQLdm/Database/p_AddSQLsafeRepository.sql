
IF (object_id('p_AddSQLsafeRepository') IS NOT NULL)
BEGIN
DROP PROCEDURE p_AddSQLsafeRepository
END
GO

CREATE PROCEDURE [dbo].[p_AddSQLsafeRepository]
(
	@InstanceName nvarchar(256),
	@DatabaseName nvarchar(128),
	@SecurityMode bit,
	@UserName nvarchar(128),
	@EncryptedPassword nvarchar(128),
	@FriendlyName nvarchar(256),
	@Id int out
)
AS
BEGIN
	
	select @Id = -1

	IF NOT EXISTS(select RepositoryId from SQLsafeConnections where InstanceName = @InstanceName)
	BEGIN
		INSERT INTO SQLsafeConnections (FriendlyName, InstanceName, DatabaseName, Active, Deleted, SecurityMode, UserName, EncryptedPassword)
			values(@FriendlyName, @InstanceName, @DatabaseName, 1, 0, @SecurityMode, @UserName, @EncryptedPassword)

		select @Id = @@IDENTITY
	END
END