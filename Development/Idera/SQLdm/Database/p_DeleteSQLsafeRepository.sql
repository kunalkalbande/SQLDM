
IF (object_id('p_DeleteSQLsafeRepository') IS NOT NULL)
BEGIN
DROP PROCEDURE p_DeleteSQLsafeRepository
END
GO

CREATE PROCEDURE [dbo].[p_DeleteSQLsafeRepository]
(
	@Id INT
)
AS
BEGIN
	IF EXISTS(SELECT RepositoryId from SQLsafeConnections where RepositoryId = @Id)
	BEGIN
		UPDATE SQLsafeConnections set Active = 0, Deleted = 1 where RepositoryId = @Id
	END
END